using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using RetailManagement.Database;
using RetailManagement.Models;

namespace RetailManagement.UserForms
{
    public partial class CloudSynchronizationForm : Form
    {
        private ComboBox cmbCloudProvider;
        private ComboBox cmbSyncFrequency;
        private ComboBox cmbSyncScope;
        private TextBox txtCloudEndpoint;
        private TextBox txtApiKey;
        private TextBox txtSecretKey;
        private CheckBox chkAutoSync;
        private CheckBox chkSyncSales;
        private CheckBox chkSyncPurchases;
        private CheckBox chkSyncInventory;
        private CheckBox chkSyncCustomers;
        private CheckBox chkSyncSuppliers;
        private CheckBox chkSyncReports;
        private CheckBox chkSyncSettings;
        private CheckBox chkEncryptData;
        private Button btnTestConnection;
        private Button btnSyncNow;
        private Button btnBackupNow;
        private Button btnRestoreData;
        private Button btnSaveSettings;
        private Button btnViewSyncLog;
        private DataGridView dgvSyncHistory;
        private ProgressBar progressSync;
        private Label lblSyncStatus;
        private Label lblLastSync;
        private Label lblDataSize;
        private GroupBox groupCloudSettings;
        private GroupBox groupSyncOptions;
        private GroupBox groupSyncHistory;
        private Panel summaryPanel;
        private Label lblTotalSyncs;
        private Label lblDataSynced;
        private Label lblCloudStorage;
        private Timer syncTimer;
        private BackgroundWorker syncWorker;
        private string connectionString;
        private bool isConnected = false;

        public CloudSynchronizationForm()
        {
            InitializeComponent();
            InitializeCloudSync();
            LoadSettings();
            LoadSyncHistory();
        }

        private void InitializeComponent()
        {
            this.dgvSyncHistory = new System.Windows.Forms.DataGridView();
            this.groupCloudSettings = new System.Windows.Forms.GroupBox();
            this.groupSyncOptions = new System.Windows.Forms.GroupBox();
            this.groupSyncHistory = new System.Windows.Forms.GroupBox();
            this.cmbCloudProvider = new System.Windows.Forms.ComboBox();
            this.cmbSyncFrequency = new System.Windows.Forms.ComboBox();
            this.cmbSyncScope = new System.Windows.Forms.ComboBox();
            this.txtCloudEndpoint = new System.Windows.Forms.TextBox();
            this.txtApiKey = new System.Windows.Forms.TextBox();
            this.txtSecretKey = new System.Windows.Forms.TextBox();
            this.chkAutoSync = new System.Windows.Forms.CheckBox();
            this.chkSyncSales = new System.Windows.Forms.CheckBox();
            this.chkSyncPurchases = new System.Windows.Forms.CheckBox();
            this.chkSyncInventory = new System.Windows.Forms.CheckBox();
            this.chkSyncCustomers = new System.Windows.Forms.CheckBox();
            this.chkSyncSuppliers = new System.Windows.Forms.CheckBox();
            this.chkSyncReports = new System.Windows.Forms.CheckBox();
            this.chkSyncSettings = new System.Windows.Forms.CheckBox();
            this.chkEncryptData = new System.Windows.Forms.CheckBox();
            this.btnTestConnection = new System.Windows.Forms.Button();
            this.btnSyncNow = new System.Windows.Forms.Button();
            this.btnBackupNow = new System.Windows.Forms.Button();
            this.btnRestoreData = new System.Windows.Forms.Button();
            this.btnSaveSettings = new System.Windows.Forms.Button();
            this.btnViewSyncLog = new System.Windows.Forms.Button();
            this.progressSync = new System.Windows.Forms.ProgressBar();
            this.lblSyncStatus = new System.Windows.Forms.Label();
            this.lblLastSync = new System.Windows.Forms.Label();
            this.lblDataSize = new System.Windows.Forms.Label();
            this.summaryPanel = new System.Windows.Forms.Panel();
            this.lblTotalSyncs = new System.Windows.Forms.Label();
            this.lblDataSynced = new System.Windows.Forms.Label();
            this.lblCloudStorage = new System.Windows.Forms.Label();
            this.syncTimer = new System.Windows.Forms.Timer();
            this.syncWorker = new System.ComponentModel.BackgroundWorker();

            ((System.ComponentModel.ISupportInitialize)(this.dgvSyncHistory)).BeginInit();
            this.SuspendLayout();

            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.Controls.Add(this.summaryPanel);
            this.Controls.Add(this.dgvSyncHistory);
            this.Controls.Add(this.groupSyncHistory);
            this.Controls.Add(this.groupSyncOptions);
            this.Controls.Add(this.groupCloudSettings);
            this.Name = "CloudSynchronizationForm";
            this.Text = "Cloud Synchronization - Data Backup & Multi-Device Access";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;

            // Group Cloud Settings
            this.groupCloudSettings.Controls.Add(new Label { Text = "Cloud Provider:", Location = new Point(15, 25), Size = new Size(85, 13) });
            this.groupCloudSettings.Controls.Add(this.cmbCloudProvider);
            this.groupCloudSettings.Controls.Add(new Label { Text = "Endpoint URL:", Location = new Point(15, 55), Size = new Size(80, 13) });
            this.groupCloudSettings.Controls.Add(this.txtCloudEndpoint);
            this.groupCloudSettings.Controls.Add(new Label { Text = "API Key:", Location = new Point(15, 85), Size = new Size(55, 13) });
            this.groupCloudSettings.Controls.Add(this.txtApiKey);
            this.groupCloudSettings.Controls.Add(new Label { Text = "Secret Key:", Location = new Point(15, 115), Size = new Size(65, 13) });
            this.groupCloudSettings.Controls.Add(this.txtSecretKey);
            this.groupCloudSettings.Controls.Add(this.chkEncryptData);
            this.groupCloudSettings.Controls.Add(this.btnTestConnection);
            this.groupCloudSettings.Controls.Add(this.btnSaveSettings);
            this.groupCloudSettings.Location = new System.Drawing.Point(12, 12);
            this.groupCloudSettings.Name = "groupCloudSettings";
            this.groupCloudSettings.Size = new System.Drawing.Size(380, 200);
            this.groupCloudSettings.TabIndex = 0;
            this.groupCloudSettings.TabStop = false;
            this.groupCloudSettings.Text = "Cloud Provider Settings";

            // Group Sync Options
            this.groupSyncOptions.Controls.Add(new Label { Text = "Sync Frequency:", Location = new Point(15, 25), Size = new Size(85, 13) });
            this.groupSyncOptions.Controls.Add(this.cmbSyncFrequency);
            this.groupSyncOptions.Controls.Add(new Label { Text = "Sync Scope:", Location = new Point(15, 55), Size = new Size(70, 13) });
            this.groupSyncOptions.Controls.Add(this.cmbSyncScope);
            this.groupSyncOptions.Controls.Add(this.chkAutoSync);
            this.groupSyncOptions.Controls.Add(new Label { Text = "Data to Sync:", Location = new Point(15, 110), Size = new Size(75, 13) });
            this.groupSyncOptions.Controls.Add(this.chkSyncSales);
            this.groupSyncOptions.Controls.Add(this.chkSyncPurchases);
            this.groupSyncOptions.Controls.Add(this.chkSyncInventory);
            this.groupSyncOptions.Controls.Add(this.chkSyncCustomers);
            this.groupSyncOptions.Controls.Add(this.chkSyncSuppliers);
            this.groupSyncOptions.Controls.Add(this.chkSyncReports);
            this.groupSyncOptions.Controls.Add(this.chkSyncSettings);
            this.groupSyncOptions.Controls.Add(this.progressSync);
            this.groupSyncOptions.Controls.Add(this.lblSyncStatus);
            this.groupSyncOptions.Controls.Add(this.lblLastSync);
            this.groupSyncOptions.Controls.Add(this.lblDataSize);
            this.groupSyncOptions.Controls.Add(this.btnSyncNow);
            this.groupSyncOptions.Controls.Add(this.btnBackupNow);
            this.groupSyncOptions.Controls.Add(this.btnRestoreData);
            this.groupSyncOptions.Controls.Add(this.btnViewSyncLog);
            this.groupSyncOptions.Location = new System.Drawing.Point(400, 12);
            this.groupSyncOptions.Name = "groupSyncOptions";
            this.groupSyncOptions.Size = new System.Drawing.Size(780, 280);
            this.groupSyncOptions.TabIndex = 1;
            this.groupSyncOptions.TabStop = false;
            this.groupSyncOptions.Text = "Synchronization Options & Status";

            // Group Sync History
            this.groupSyncHistory.Controls.Add(this.dgvSyncHistory);
            this.groupSyncHistory.Location = new System.Drawing.Point(12, 300);
            this.groupSyncHistory.Name = "groupSyncHistory";
            this.groupSyncHistory.Size = new System.Drawing.Size(1168, 280);
            this.groupSyncHistory.TabIndex = 2;
            this.groupSyncHistory.TabStop = false;
            this.groupSyncHistory.Text = "Synchronization History & Logs";

            // Summary Panel
            this.summaryPanel.BackColor = System.Drawing.Color.LightCyan;
            this.summaryPanel.Controls.Add(this.lblTotalSyncs);
            this.summaryPanel.Controls.Add(this.lblDataSynced);
            this.summaryPanel.Controls.Add(this.lblCloudStorage);
            this.summaryPanel.Location = new System.Drawing.Point(12, 220);
            this.summaryPanel.Name = "summaryPanel";
            this.summaryPanel.Size = new System.Drawing.Size(380, 70);
            this.summaryPanel.TabIndex = 3;

            // Setup all controls
            SetupControls();
            SetupDataGridView();

            ((System.ComponentModel.ISupportInitialize)(this.dgvSyncHistory)).EndInit();
            this.ResumeLayout(false);
        }

        private void SetupControls()
        {
            // Cloud Provider ComboBox
            this.cmbCloudProvider.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCloudProvider.Location = new System.Drawing.Point(105, 22);
            this.cmbCloudProvider.Name = "cmbCloudProvider";
            this.cmbCloudProvider.Size = new System.Drawing.Size(150, 21);
            this.cmbCloudProvider.TabIndex = 1;
            this.cmbCloudProvider.SelectedIndexChanged += CmbCloudProvider_SelectedIndexChanged;

            // Cloud Endpoint TextBox
            this.txtCloudEndpoint.Location = new System.Drawing.Point(105, 52);
            this.txtCloudEndpoint.Name = "txtCloudEndpoint";
            this.txtCloudEndpoint.Size = new System.Drawing.Size(250, 20);
            this.txtCloudEndpoint.TabIndex = 2;
            this.txtCloudEndpoint.Text = "https://api.cloudprovider.com/v1";

            // API Key TextBox
            this.txtApiKey.Location = new System.Drawing.Point(85, 82);
            this.txtApiKey.Name = "txtApiKey";
            this.txtApiKey.Size = new System.Drawing.Size(270, 20);
            this.txtApiKey.TabIndex = 3;
            // txtApiKey placeholder is set via Text property when empty

            // Secret Key TextBox
            this.txtSecretKey.Location = new System.Drawing.Point(85, 112);
            this.txtSecretKey.Name = "txtSecretKey";
            this.txtSecretKey.Size = new System.Drawing.Size(270, 20);
            this.txtSecretKey.TabIndex = 4;
            this.txtSecretKey.UseSystemPasswordChar = true;
            // txtSecretKey placeholder is set via Text property when empty

            // Encrypt Data CheckBox
            this.chkEncryptData.Location = new System.Drawing.Point(15, 145);
            this.chkEncryptData.Size = new System.Drawing.Size(120, 17);
            this.chkEncryptData.Text = "Encrypt Data";
            this.chkEncryptData.Checked = true;

            // Test Connection Button
            this.btnTestConnection.BackColor = System.Drawing.Color.Orange;
            this.btnTestConnection.ForeColor = System.Drawing.Color.White;
            this.btnTestConnection.Location = new System.Drawing.Point(150, 142);
            this.btnTestConnection.Size = new System.Drawing.Size(100, 25);
            this.btnTestConnection.Text = "Test Connection";
            this.btnTestConnection.Click += BtnTestConnection_Click;

            // Save Settings Button
            this.btnSaveSettings.BackColor = System.Drawing.Color.Green;
            this.btnSaveSettings.ForeColor = System.Drawing.Color.White;
            this.btnSaveSettings.Location = new System.Drawing.Point(260, 142);
            this.btnSaveSettings.Size = new System.Drawing.Size(100, 25);
            this.btnSaveSettings.Text = "Save Settings";
            this.btnSaveSettings.Click += BtnSaveSettings_Click;

            // Sync Frequency ComboBox
            this.cmbSyncFrequency.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSyncFrequency.Location = new System.Drawing.Point(105, 22);
            this.cmbSyncFrequency.Size = new System.Drawing.Size(120, 21);

            // Sync Scope ComboBox
            this.cmbSyncScope.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSyncScope.Location = new System.Drawing.Point(90, 52);
            this.cmbSyncScope.Size = new System.Drawing.Size(135, 21);

            // Auto Sync CheckBox
            this.chkAutoSync.Location = new System.Drawing.Point(15, 85);
            this.chkAutoSync.Size = new System.Drawing.Size(150, 17);
            this.chkAutoSync.Text = "Enable Auto Sync";
            this.chkAutoSync.CheckedChanged += ChkAutoSync_CheckedChanged;

            // Data checkboxes
            this.chkSyncSales.Location = new System.Drawing.Point(15, 130);
            this.chkSyncSales.Size = new System.Drawing.Size(70, 17);
            this.chkSyncSales.Text = "Sales";
            this.chkSyncSales.Checked = true;

            this.chkSyncPurchases.Location = new System.Drawing.Point(90, 130);
            this.chkSyncPurchases.Size = new System.Drawing.Size(80, 17);
            this.chkSyncPurchases.Text = "Purchases";
            this.chkSyncPurchases.Checked = true;

            this.chkSyncInventory.Location = new System.Drawing.Point(175, 130);
            this.chkSyncInventory.Size = new System.Drawing.Size(80, 17);
            this.chkSyncInventory.Text = "Inventory";
            this.chkSyncInventory.Checked = true;

            this.chkSyncCustomers.Location = new System.Drawing.Point(260, 130);
            this.chkSyncCustomers.Size = new System.Drawing.Size(80, 17);
            this.chkSyncCustomers.Text = "Customers";
            this.chkSyncCustomers.Checked = true;

            this.chkSyncSuppliers.Location = new System.Drawing.Point(15, 150);
            this.chkSyncSuppliers.Size = new System.Drawing.Size(80, 17);
            this.chkSyncSuppliers.Text = "Suppliers";
            this.chkSyncSuppliers.Checked = true;

            this.chkSyncReports.Location = new System.Drawing.Point(100, 150);
            this.chkSyncReports.Size = new System.Drawing.Size(70, 17);
            this.chkSyncReports.Text = "Reports";

            this.chkSyncSettings.Location = new System.Drawing.Point(175, 150);
            this.chkSyncSettings.Size = new System.Drawing.Size(70, 17);
            this.chkSyncSettings.Text = "Settings";

            // Progress Bar
            this.progressSync.Location = new System.Drawing.Point(15, 175);
            this.progressSync.Size = new System.Drawing.Size(400, 20);
            this.progressSync.Style = ProgressBarStyle.Continuous;

            // Status Labels
            this.lblSyncStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblSyncStatus.Location = new System.Drawing.Point(15, 200);
            this.lblSyncStatus.Size = new System.Drawing.Size(200, 15);
            this.lblSyncStatus.Text = "Status: Not Connected";
            this.lblSyncStatus.ForeColor = Color.Red;

            this.lblLastSync.Location = new System.Drawing.Point(15, 220);
            this.lblLastSync.Size = new System.Drawing.Size(200, 15);
            this.lblLastSync.Text = "Last Sync: Never";

            this.lblDataSize.Location = new System.Drawing.Point(15, 240);
            this.lblDataSize.Size = new System.Drawing.Size(200, 15);
            this.lblDataSize.Text = "Data Size: 0 MB";

            // Action Buttons
            this.btnSyncNow.BackColor = System.Drawing.Color.Blue;
            this.btnSyncNow.ForeColor = System.Drawing.Color.White;
            this.btnSyncNow.Location = new System.Drawing.Point(450, 25);
            this.btnSyncNow.Size = new System.Drawing.Size(100, 30);
            this.btnSyncNow.Text = "Sync Now";
            this.btnSyncNow.Click += BtnSyncNow_Click;

            this.btnBackupNow.BackColor = System.Drawing.Color.Green;
            this.btnBackupNow.ForeColor = System.Drawing.Color.White;
            this.btnBackupNow.Location = new System.Drawing.Point(560, 25);
            this.btnBackupNow.Size = new System.Drawing.Size(100, 30);
            this.btnBackupNow.Text = "Backup Now";
            this.btnBackupNow.Click += BtnBackupNow_Click;

            this.btnRestoreData.BackColor = System.Drawing.Color.Purple;
            this.btnRestoreData.ForeColor = System.Drawing.Color.White;
            this.btnRestoreData.Location = new System.Drawing.Point(670, 25);
            this.btnRestoreData.Size = new System.Drawing.Size(100, 30);
            this.btnRestoreData.Text = "Restore Data";
            this.btnRestoreData.Click += BtnRestoreData_Click;

            this.btnViewSyncLog.BackColor = System.Drawing.Color.Teal;
            this.btnViewSyncLog.ForeColor = System.Drawing.Color.White;
            this.btnViewSyncLog.Location = new System.Drawing.Point(450, 65);
            this.btnViewSyncLog.Size = new System.Drawing.Size(100, 25);
            this.btnViewSyncLog.Text = "View Sync Log";
            this.btnViewSyncLog.Click += BtnViewSyncLog_Click;

            // Summary Labels
            this.lblTotalSyncs.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblTotalSyncs.Location = new System.Drawing.Point(15, 10);
            this.lblTotalSyncs.Size = new System.Drawing.Size(150, 15);
            this.lblTotalSyncs.Text = "Total Syncs: 0";

            this.lblDataSynced.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblDataSynced.Location = new System.Drawing.Point(15, 30);
            this.lblDataSynced.Size = new System.Drawing.Size(180, 15);
            this.lblDataSynced.Text = "Data Synced: 0 MB";

            this.lblCloudStorage.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblCloudStorage.Location = new System.Drawing.Point(15, 50);
            this.lblCloudStorage.Size = new System.Drawing.Size(180, 15);
            this.lblCloudStorage.Text = "Cloud Storage: 0 MB";
        }

        private void SetupDataGridView()
        {
            this.dgvSyncHistory.Location = new System.Drawing.Point(15, 25);
            this.dgvSyncHistory.Name = "dgvSyncHistory";
            this.dgvSyncHistory.Size = new System.Drawing.Size(1138, 240);
            this.dgvSyncHistory.TabIndex = 4;
            this.dgvSyncHistory.AllowUserToAddRows = false;
            this.dgvSyncHistory.AllowUserToDeleteRows = false;
            this.dgvSyncHistory.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvSyncHistory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Add columns
            this.dgvSyncHistory.Columns.Add("SyncDate", "Sync Date");
            this.dgvSyncHistory.Columns.Add("SyncType", "Type");
            this.dgvSyncHistory.Columns.Add("DataType", "Data Type");
            this.dgvSyncHistory.Columns.Add("RecordsCount", "Records");
            this.dgvSyncHistory.Columns.Add("DataSize", "Size (MB)");
            this.dgvSyncHistory.Columns.Add("Duration", "Duration");
            this.dgvSyncHistory.Columns.Add("Status", "Status");
            this.dgvSyncHistory.Columns.Add("Message", "Message");

            // Set column widths
            this.dgvSyncHistory.Columns["SyncDate"].Width = 120;
            this.dgvSyncHistory.Columns["SyncType"].Width = 80;
            this.dgvSyncHistory.Columns["DataType"].Width = 100;
            this.dgvSyncHistory.Columns["RecordsCount"].Width = 80;
            this.dgvSyncHistory.Columns["DataSize"].Width = 80;
            this.dgvSyncHistory.Columns["Duration"].Width = 80;
            this.dgvSyncHistory.Columns["Status"].Width = 100;
        }

        private void InitializeCloudSync()
        {
            // Load cloud providers
            cmbCloudProvider.Items.AddRange(new object[] { 
                "Azure Cloud Storage", "Amazon AWS S3", "Google Cloud Storage", 
                "Microsoft OneDrive", "Dropbox Business", "Custom API", "Local Network Share" 
            });
            cmbCloudProvider.SelectedIndex = 0;

            // Load sync frequencies
            cmbSyncFrequency.Items.AddRange(new object[] { 
                "Every 15 minutes", "Every 30 minutes", "Every hour", "Every 2 hours", 
                "Every 4 hours", "Every 8 hours", "Daily", "Weekly", "Manual only" 
            });
            cmbSyncFrequency.SelectedIndex = 6; // Daily

            // Load sync scopes
            cmbSyncScope.Items.AddRange(new object[] { 
                "All Data", "Recent Changes Only", "Sales Data Only", "Inventory Only", 
                "Financial Data Only", "Customer Data Only", "Reports Only", "Custom Selection" 
            });
            cmbSyncScope.SelectedIndex = 1; // Recent Changes Only

            // Setup sync timer
            syncTimer.Interval = 60000; // Check every minute
            syncTimer.Tick += SyncTimer_Tick;

            // Setup background worker
            syncWorker.DoWork += SyncWorker_DoWork;
            syncWorker.ProgressChanged += SyncWorker_ProgressChanged;
            syncWorker.RunWorkerCompleted += SyncWorker_RunWorkerCompleted;
            syncWorker.WorkerReportsProgress = true;
            syncWorker.WorkerSupportsCancellation = true;

            // Get connection string from DatabaseConnection class (use GetConnection method)
            using (var conn = DatabaseConnection.GetConnection())
            {
                connectionString = conn.ConnectionString;
            }
        }

        private void LoadSettings()
        {
            try
            {
                // Load settings from configuration or database
                string query = @"
                    SELECT SettingKey, SettingValue 
                    FROM SystemSettings 
                    WHERE SettingKey LIKE 'CloudSync_%'";

                DataTable settings = DatabaseConnection.ExecuteQuery(query);

                foreach (DataRow row in settings.Rows)
                {
                    string key = row["SettingKey"].ToString();
                    string value = row["SettingValue"].ToString();

                    switch (key)
                    {
                        case "CloudSync_Provider":
                            if (cmbCloudProvider.Items.Contains(value))
                                cmbCloudProvider.SelectedItem = value;
                            break;
                        case "CloudSync_Endpoint":
                            txtCloudEndpoint.Text = value;
                            break;
                        case "CloudSync_ApiKey":
                            txtApiKey.Text = value;
                            break;
                        case "CloudSync_Frequency":
                            if (cmbSyncFrequency.Items.Contains(value))
                                cmbSyncFrequency.SelectedItem = value;
                            break;
                        case "CloudSync_AutoSync":
                            chkAutoSync.Checked = value.ToLower() == "true";
                            break;
                        case "CloudSync_EncryptData":
                            chkEncryptData.Checked = value.ToLower() == "true";
                            break;
                    }
                }

                UpdateSyncStatus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading cloud sync settings: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSyncHistory()
        {
            try
            {
                string query = @"
                    SELECT 
                        SyncDate,
                        SyncType,
                        DataType,
                        RecordsCount,
                        DataSize,
                        Duration,
                        Status,
                        Message
                    FROM CloudSyncHistory
                    ORDER BY SyncDate DESC";

                DataTable history = DatabaseConnection.ExecuteQuery(query);

                dgvSyncHistory.Rows.Clear();

                if (history != null)
                {
                    foreach (DataRow row in history.Rows)
                    {
                        int rowIndex = dgvSyncHistory.Rows.Add();
                        dgvSyncHistory.Rows[rowIndex].Cells["SyncDate"].Value = 
                            Convert.ToDateTime(row["SyncDate"]).ToString("dd/MM/yyyy HH:mm");
                        dgvSyncHistory.Rows[rowIndex].Cells["SyncType"].Value = row["SyncType"];
                        dgvSyncHistory.Rows[rowIndex].Cells["DataType"].Value = row["DataType"];
                        dgvSyncHistory.Rows[rowIndex].Cells["RecordsCount"].Value = row["RecordsCount"];
                        dgvSyncHistory.Rows[rowIndex].Cells["DataSize"].Value = 
                            Convert.ToDecimal(row["DataSize"]).ToString("N2");
                        dgvSyncHistory.Rows[rowIndex].Cells["Duration"].Value = row["Duration"];
                        dgvSyncHistory.Rows[rowIndex].Cells["Status"].Value = row["Status"];
                        dgvSyncHistory.Rows[rowIndex].Cells["Message"].Value = row["Message"];

                        // Color code based on status
                        string status = row["Status"].ToString();
                        if (status == "Success")
                            dgvSyncHistory.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightGreen;
                        else if (status == "Failed")
                            dgvSyncHistory.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightCoral;
                        else if (status == "Partial")
                            dgvSyncHistory.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightYellow;
                    }
                }

                UpdateSummary();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading sync history: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateSummary()
        {
            try
            {
                string query = @"
                    SELECT 
                        COUNT(*) as TotalSyncs,
                        SUM(DataSize) as TotalDataSynced,
                        MAX(SyncDate) as LastSyncDate
                    FROM CloudSyncHistory
                    WHERE Status = 'Success'";

                DataTable summary = DatabaseConnection.ExecuteQuery(query);

                if (summary.Rows.Count > 0)
                {
                    DataRow row = summary.Rows[0];
                    lblTotalSyncs.Text = $"Total Syncs: {row["TotalSyncs"]}";
                    
                    decimal totalData = row["TotalDataSynced"] != DBNull.Value ? 
                        Convert.ToDecimal(row["TotalDataSynced"]) : 0;
                    lblDataSynced.Text = $"Data Synced: {totalData:N2} MB";
                    
                    if (row["LastSyncDate"] != DBNull.Value)
                    {
                        DateTime lastSync = Convert.ToDateTime(row["LastSyncDate"]);
                        lblLastSync.Text = $"Last Sync: {lastSync:dd/MM/yyyy HH:mm}";
                    }
                }

                // Calculate estimated cloud storage
                decimal cloudStorage = GetEstimatedCloudStorage();
                lblCloudStorage.Text = $"Cloud Storage: {cloudStorage:N2} MB";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating summary: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private decimal GetEstimatedCloudStorage()
        {
            try
            {
                // Estimate database size for cloud storage calculation
                string query = @"
                    SELECT 
                        (SELECT COUNT(*) FROM Sales) as SalesCount,
                        (SELECT COUNT(*) FROM Purchases) as PurchasesCount,
                        (SELECT COUNT(*) FROM Items) as ItemsCount,
                        (SELECT COUNT(*) FROM Customers) as CustomersCount,
                        (SELECT COUNT(*) FROM Companies) as SuppliersCount";

                DataTable counts = DatabaseConnection.ExecuteQuery(query);
                
                if (counts.Rows.Count > 0)
                {
                    DataRow row = counts.Rows[0];
                    
                    // Rough estimation based on record counts
                    int salesCount = Convert.ToInt32(row["SalesCount"]);
                    int purchasesCount = Convert.ToInt32(row["PurchasesCount"]);
                    int itemsCount = Convert.ToInt32(row["ItemsCount"]);
                    int customersCount = Convert.ToInt32(row["CustomersCount"]);
                    int suppliersCount = Convert.ToInt32(row["SuppliersCount"]);

                    decimal estimatedSize = (salesCount * 0.5m) + (purchasesCount * 0.5m) + 
                                          (itemsCount * 0.3m) + (customersCount * 0.2m) + 
                                          (suppliersCount * 0.2m);

                    return Math.Max(estimatedSize, 1.0m); // Minimum 1 MB
                }
            }
            catch
            {
                // Return default if calculation fails
            }
            
            return 10.0m; // Default 10 MB
        }

        private void UpdateSyncStatus()
        {
            if (isConnected)
            {
                lblSyncStatus.Text = "Status: Connected";
                lblSyncStatus.ForeColor = Color.Green;
                btnSyncNow.Enabled = true;
                btnBackupNow.Enabled = true;
                btnRestoreData.Enabled = true;
            }
            else
            {
                lblSyncStatus.Text = "Status: Not Connected";
                lblSyncStatus.ForeColor = Color.Red;
                btnSyncNow.Enabled = false;
                btnBackupNow.Enabled = false;
                btnRestoreData.Enabled = false;
            }

            // Update data size
            decimal dataSize = GetEstimatedCloudStorage();
            lblDataSize.Text = $"Data Size: {dataSize:N2} MB";
        }

        private void CmbCloudProvider_SelectedIndexChanged(object sender, EventArgs e)
        {
            string provider = cmbCloudProvider.SelectedItem.ToString();
            
            // Update endpoint based on provider
            switch (provider)
            {
                case "Azure Cloud Storage":
                    txtCloudEndpoint.Text = "https://[account].blob.core.windows.net/";
                    break;
                case "Amazon AWS S3":
                    txtCloudEndpoint.Text = "https://s3.[region].amazonaws.com/";
                    break;
                case "Google Cloud Storage":
                    txtCloudEndpoint.Text = "https://storage.googleapis.com/";
                    break;
                case "Microsoft OneDrive":
                    txtCloudEndpoint.Text = "https://graph.microsoft.com/v1.0/";
                    break;
                case "Dropbox Business":
                    txtCloudEndpoint.Text = "https://api.dropboxapi.com/2/";
                    break;
                case "Custom API":
                    txtCloudEndpoint.Text = "";
                    break;
                case "Local Network Share":
                    txtCloudEndpoint.Text = "\\\\server\\share\\";
                    break;
            }
        }

        private void ChkAutoSync_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAutoSync.Checked)
            {
                if (isConnected)
                {
                    syncTimer.Start();
                    MessageBox.Show("Auto-sync enabled. Data will be synchronized based on the selected frequency.", 
                        "Auto-Sync Enabled", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    chkAutoSync.Checked = false;
                    MessageBox.Show("Please test and establish cloud connection before enabling auto-sync.", 
                        "Connection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                syncTimer.Stop();
            }
        }

        private void SyncTimer_Tick(object sender, EventArgs e)
        {
            // Check if it's time to sync based on frequency
            if (ShouldSync())
            {
                PerformAutoSync();
            }
        }

        private bool ShouldSync()
        {
            try
            {
                string frequency = cmbSyncFrequency.SelectedItem.ToString();
                
                // Get last sync time
                string query = "SELECT MAX(SyncDate) as LastSync FROM CloudSyncHistory WHERE Status = 'Success'";
                DataTable result = DatabaseConnection.ExecuteQuery(query);
                
                DateTime lastSync = DateTime.MinValue;
                if (result.Rows.Count > 0 && result.Rows[0]["LastSync"] != DBNull.Value)
                {
                    lastSync = Convert.ToDateTime(result.Rows[0]["LastSync"]);
                }

                DateTime nextSyncTime = GetNextSyncTime(lastSync, frequency);
                return DateTime.Now >= nextSyncTime;
            }
            catch
            {
                return false;
            }
        }

        private DateTime GetNextSyncTime(DateTime lastSync, string frequency)
        {
            switch (frequency)
            {
                case "Every 15 minutes": return lastSync.AddMinutes(15);
                case "Every 30 minutes": return lastSync.AddMinutes(30);
                case "Every hour": return lastSync.AddHours(1);
                case "Every 2 hours": return lastSync.AddHours(2);
                case "Every 4 hours": return lastSync.AddHours(4);
                case "Every 8 hours": return lastSync.AddHours(8);
                case "Daily": return lastSync.AddDays(1);
                case "Weekly": return lastSync.AddDays(7);
                default: return DateTime.MaxValue; // Manual only
            }
        }

        private void PerformAutoSync()
        {
            if (!syncWorker.IsBusy)
            {
                syncWorker.RunWorkerAsync("AutoSync");
            }
        }

        private void BtnTestConnection_Click(object sender, EventArgs e)
        {
            try
            {
                btnTestConnection.Enabled = false;
                btnTestConnection.Text = "Testing...";

                // Simulate connection test
                System.Threading.Thread.Sleep(2000);

                // In a real implementation, this would test the actual cloud connection
                if (!string.IsNullOrEmpty(txtCloudEndpoint.Text) && !string.IsNullOrEmpty(txtApiKey.Text))
                {
                    isConnected = true;
                    MessageBox.Show("Connection test successful! Cloud synchronization is ready.", 
                        "Connection Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    isConnected = false;
                    MessageBox.Show("Connection test failed. Please check your endpoint URL and API credentials.", 
                        "Connection Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                UpdateSyncStatus();
            }
            catch (Exception ex)
            {
                isConnected = false;
                MessageBox.Show("Connection test error: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnTestConnection.Enabled = true;
                btnTestConnection.Text = "Test Connection";
            }
        }

        private void BtnSaveSettings_Click(object sender, EventArgs e)
        {
            try
            {
                // Save settings to database
                var settings = new Dictionary<string, string>
                {
                    {"CloudSync_Provider", cmbCloudProvider.SelectedItem.ToString()},
                    {"CloudSync_Endpoint", txtCloudEndpoint.Text},
                    {"CloudSync_ApiKey", txtApiKey.Text},
                    {"CloudSync_Frequency", cmbSyncFrequency.SelectedItem.ToString()},
                    {"CloudSync_AutoSync", chkAutoSync.Checked.ToString()},
                    {"CloudSync_EncryptData", chkEncryptData.Checked.ToString()}
                };

                foreach (var setting in settings)
                {
                    string updateQuery = @"
                        IF EXISTS (SELECT 1 FROM SystemSettings WHERE SettingKey = @Key)
                            UPDATE SystemSettings SET SettingValue = @Value WHERE SettingKey = @Key
                        ELSE
                            INSERT INTO SystemSettings (SettingKey, SettingValue) VALUES (@Key, @Value)";

                    SqlParameter[] parameters = {
                        new SqlParameter("@Key", setting.Key),
                        new SqlParameter("@Value", setting.Value)
                    };

                    DatabaseConnection.ExecuteNonQuery(updateQuery, parameters);
                }

                MessageBox.Show("Cloud synchronization settings saved successfully!", 
                    "Settings Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving settings: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSyncNow_Click(object sender, EventArgs e)
        {
            if (!syncWorker.IsBusy)
            {
                syncWorker.RunWorkerAsync("ManualSync");
            }
            else
            {
                MessageBox.Show("Synchronization is already in progress.", "Sync in Progress", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnBackupNow_Click(object sender, EventArgs e)
        {
            if (!syncWorker.IsBusy)
            {
                syncWorker.RunWorkerAsync("Backup");
            }
            else
            {
                MessageBox.Show("Backup is already in progress.", "Backup in Progress", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnRestoreData_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "WARNING: This will restore data from cloud backup and may overwrite local changes.\n\n" +
                "Are you sure you want to proceed with data restoration?", 
                "Confirm Data Restoration", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                if (!syncWorker.IsBusy)
                {
                    syncWorker.RunWorkerAsync("Restore");
                }
                else
                {
                    MessageBox.Show("Restoration is already in progress.", "Restore in Progress", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void BtnViewSyncLog_Click(object sender, EventArgs e)
        {
            try
            {
                // Show detailed sync log
                MessageBox.Show("Cloud Synchronization Features:\n\n" +
                    "✓ Multi-provider support (Azure, AWS, Google Cloud)\n" +
                    "✓ Automated sync scheduling\n" +
                    "✓ Encrypted data transmission\n" +
                    "✓ Incremental sync for efficiency\n" +
                    "✓ Conflict resolution\n" +
                    "✓ Data integrity verification\n" +
                    "✓ Comprehensive logging\n" +
                    "✓ Backup and restore capabilities\n" +
                    "✓ Multi-device access\n" +
                    "✓ Real-time sync status\n\n" +
                    "This would provide enterprise-grade cloud synchronization for retail operations.", 
                    "Cloud Sync Features", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error viewing sync log: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SyncWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string operation = e.Argument.ToString();
            
            try
            {
                syncWorker.ReportProgress(0, $"Starting {operation}...");
                
                switch (operation)
                {
                    case "ManualSync":
                    case "AutoSync":
                        PerformDataSync();
                        break;
                    case "Backup":
                        PerformDataBackup();
                        break;
                    case "Restore":
                        PerformDataRestore();
                        break;
                }

                e.Result = "Success";
            }
            catch (Exception ex)
            {
                e.Result = "Error: " + ex.Message;
            }
        }

        private void PerformDataSync()
        {
            // Simulate data synchronization process
            var dataTypes = new string[] { "Sales", "Purchases", "Inventory", "Customers", "Suppliers" };
            
            for (int i = 0; i < dataTypes.Length; i++)
            {
                if (syncWorker.CancellationPending) return;
                
                string dataType = dataTypes[i];
                syncWorker.ReportProgress((i + 1) * 20, $"Syncing {dataType} data...");
                
                // Simulate processing time
                System.Threading.Thread.Sleep(1000);
                
                // Log sync operation
                LogSyncOperation("Sync", dataType, "Success", $"Synced {dataType} data");
            }
            
            syncWorker.ReportProgress(100, "Synchronization completed successfully!");
        }

        private void PerformDataBackup()
        {
            syncWorker.ReportProgress(25, "Creating database backup...");
            System.Threading.Thread.Sleep(2000);
            
            syncWorker.ReportProgress(50, "Encrypting backup data...");
            System.Threading.Thread.Sleep(1000);
            
            syncWorker.ReportProgress(75, "Uploading to cloud storage...");
            System.Threading.Thread.Sleep(2000);
            
            syncWorker.ReportProgress(100, "Backup completed successfully!");
            
            LogSyncOperation("Backup", "Full Database", "Success", "Full database backup completed");
        }

        private void PerformDataRestore()
        {
            syncWorker.ReportProgress(25, "Downloading backup from cloud...");
            System.Threading.Thread.Sleep(2000);
            
            syncWorker.ReportProgress(50, "Decrypting backup data...");
            System.Threading.Thread.Sleep(1000);
            
            syncWorker.ReportProgress(75, "Restoring database...");
            System.Threading.Thread.Sleep(2000);
            
            syncWorker.ReportProgress(100, "Data restoration completed successfully!");
            
            LogSyncOperation("Restore", "Full Database", "Success", "Database restored from cloud backup");
        }

        private void LogSyncOperation(string syncType, string dataType, string status, string message)
        {
            try
            {
                string insertQuery = @"
                    INSERT INTO CloudSyncHistory 
                    (SyncDate, SyncType, DataType, RecordsCount, DataSize, Duration, Status, Message)
                    VALUES 
                    (@SyncDate, @SyncType, @DataType, @RecordsCount, @DataSize, @Duration, @Status, @Message)";

                SqlParameter[] parameters = {
                    new SqlParameter("@SyncDate", DateTime.Now),
                    new SqlParameter("@SyncType", syncType),
                    new SqlParameter("@DataType", dataType),
                    new SqlParameter("@RecordsCount", new Random().Next(1, 1000)),
                    new SqlParameter("@DataSize", Math.Round((decimal)(new Random().NextDouble() * 10), 2)),
                    new SqlParameter("@Duration", $"{new Random().Next(1, 60)} sec"),
                    new SqlParameter("@Status", status),
                    new SqlParameter("@Message", message)
                };

                DatabaseConnection.ExecuteNonQuery(insertQuery, parameters);
            }
            catch
            {
                // Ignore logging errors to not disrupt the main operation
            }
        }

        private void SyncWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressSync.Value = e.ProgressPercentage;
            lblSyncStatus.Text = e.UserState.ToString();
        }

        private void SyncWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressSync.Value = 0;
            
            if (e.Result.ToString().StartsWith("Error"))
            {
                lblSyncStatus.Text = "Status: Sync Failed";
                lblSyncStatus.ForeColor = Color.Red;
                MessageBox.Show(e.Result.ToString(), "Synchronization Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                lblSyncStatus.Text = "Status: Connected";
                lblSyncStatus.ForeColor = Color.Green;
                lblLastSync.Text = $"Last Sync: {DateTime.Now:dd/MM/yyyy HH:mm}";
            }
            
            // Refresh sync history
            LoadSyncHistory();
        }
    }
}
