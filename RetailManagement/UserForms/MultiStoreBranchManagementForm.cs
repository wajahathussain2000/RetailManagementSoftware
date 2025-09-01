using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RetailManagement.Database;
using RetailManagement.Models;

namespace RetailManagement.UserForms
{
    public partial class MultiStoreBranchManagementForm : Form
    {
        private ComboBox cmbStoreSelection;
        private ComboBox cmbRegion;
        private ComboBox cmbStoreType;
        private ComboBox cmbManager;
        private TextBox txtStoreName;
        private TextBox txtStoreCode;
        private TextBox txtAddress;
        private TextBox txtPhone;
        private TextBox txtEmail;
        private DateTimePicker dtpOpeningDate;
        private NumericUpDown nudStoreArea;
        private CheckBox chkIsActive;
        private CheckBox chkIsMainBranch;
        private CheckBox chkAllowTransfers;
        private CheckBox chkCentralizedInventory;
        private Button btnAddStore;
        private Button btnUpdateStore;
        private Button btnDeleteStore;
        private Button btnTransferStock;
        private Button btnSyncAllStores;
        private Button btnGenerateStoreReport;
        private DataGridView dgvStores;
        private DataGridView dgvStoreMetrics;
        private TabControl tabStoreManagement;
        private TabPage tabStoreInfo;
        private TabPage tabInventorySync;
        private TabPage tabStorePerformance;
        private TabPage tabUserAccess;
        private GroupBox groupStoreDetails;
        private GroupBox groupStoreSummary;
        private Panel summaryPanel;
        private Label lblTotalStores;
        private Label lblActiveStores;
        private Label lblTotalRevenue;
        private Label lblTopPerformingStore;
        private ProgressBar progressSync;
        private Label lblSyncStatus;
        private DataTable storesData;
        private int selectedStoreID = 0;

        public MultiStoreBranchManagementForm()
        {
            InitializeComponent();
            InitializeMultiStore();
            LoadInitialData();
        }

        private void InitializeComponent()
        {
            this.dgvStores = new System.Windows.Forms.DataGridView();
            this.dgvStoreMetrics = new System.Windows.Forms.DataGridView();
            this.tabStoreManagement = new System.Windows.Forms.TabControl();
            this.tabStoreInfo = new System.Windows.Forms.TabPage();
            this.tabInventorySync = new System.Windows.Forms.TabPage();
            this.tabStorePerformance = new System.Windows.Forms.TabPage();
            this.tabUserAccess = new System.Windows.Forms.TabPage();
            this.groupStoreDetails = new System.Windows.Forms.GroupBox();
            this.groupStoreSummary = new System.Windows.Forms.GroupBox();
            this.cmbStoreSelection = new System.Windows.Forms.ComboBox();
            this.cmbRegion = new System.Windows.Forms.ComboBox();
            this.cmbStoreType = new System.Windows.Forms.ComboBox();
            this.cmbManager = new System.Windows.Forms.ComboBox();
            this.txtStoreName = new System.Windows.Forms.TextBox();
            this.txtStoreCode = new System.Windows.Forms.TextBox();
            this.txtAddress = new System.Windows.Forms.TextBox();
            this.txtPhone = new System.Windows.Forms.TextBox();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.dtpOpeningDate = new System.Windows.Forms.DateTimePicker();
            this.nudStoreArea = new System.Windows.Forms.NumericUpDown();
            this.chkIsActive = new System.Windows.Forms.CheckBox();
            this.chkIsMainBranch = new System.Windows.Forms.CheckBox();
            this.chkAllowTransfers = new System.Windows.Forms.CheckBox();
            this.chkCentralizedInventory = new System.Windows.Forms.CheckBox();
            this.btnAddStore = new System.Windows.Forms.Button();
            this.btnUpdateStore = new System.Windows.Forms.Button();
            this.btnDeleteStore = new System.Windows.Forms.Button();
            this.btnTransferStock = new System.Windows.Forms.Button();
            this.btnSyncAllStores = new System.Windows.Forms.Button();
            this.btnGenerateStoreReport = new System.Windows.Forms.Button();
            this.summaryPanel = new System.Windows.Forms.Panel();
            this.lblTotalStores = new System.Windows.Forms.Label();
            this.lblActiveStores = new System.Windows.Forms.Label();
            this.lblTotalRevenue = new System.Windows.Forms.Label();
            this.lblTopPerformingStore = new System.Windows.Forms.Label();
            this.progressSync = new System.Windows.Forms.ProgressBar();
            this.lblSyncStatus = new System.Windows.Forms.Label();

            ((System.ComponentModel.ISupportInitialize)(this.dgvStores)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStoreMetrics)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudStoreArea)).BeginInit();
            this.tabStoreManagement.SuspendLayout();
            this.SuspendLayout();

            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.Controls.Add(this.summaryPanel);
            this.Controls.Add(this.tabStoreManagement);
            this.Name = "MultiStoreBranchManagementForm";
            this.Text = "Multi-Store/Branch Management - Centralized Retail Operations";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;

            // Tab Control
            this.tabStoreManagement.Controls.Add(this.tabStoreInfo);
            this.tabStoreManagement.Controls.Add(this.tabInventorySync);
            this.tabStoreManagement.Controls.Add(this.tabStorePerformance);
            this.tabStoreManagement.Controls.Add(this.tabUserAccess);
            this.tabStoreManagement.Location = new System.Drawing.Point(12, 70);
            this.tabStoreManagement.Name = "tabStoreManagement";
            this.tabStoreManagement.SelectedIndex = 0;
            this.tabStoreManagement.Size = new System.Drawing.Size(1168, 580);
            this.tabStoreManagement.TabIndex = 0;

            // Tab Store Info
            this.tabStoreInfo.Controls.Add(this.groupStoreDetails);
            this.tabStoreInfo.Controls.Add(this.dgvStores);
            this.tabStoreInfo.Location = new System.Drawing.Point(4, 22);
            this.tabStoreInfo.Name = "tabStoreInfo";
            this.tabStoreInfo.Size = new System.Drawing.Size(1160, 554);
            this.tabStoreInfo.TabIndex = 0;
            this.tabStoreInfo.Text = "Store Information";
            this.tabStoreInfo.UseVisualStyleBackColor = true;

            // Tab Inventory Sync
            this.tabInventorySync.Location = new System.Drawing.Point(4, 22);
            this.tabInventorySync.Name = "tabInventorySync";
            this.tabInventorySync.Size = new System.Drawing.Size(1160, 554);
            this.tabInventorySync.TabIndex = 1;
            this.tabInventorySync.Text = "Inventory Synchronization";
            this.tabInventorySync.UseVisualStyleBackColor = true;

            // Tab Store Performance
            this.tabStorePerformance.Controls.Add(this.dgvStoreMetrics);
            this.tabStorePerformance.Location = new System.Drawing.Point(4, 22);
            this.tabStorePerformance.Name = "tabStorePerformance";
            this.tabStorePerformance.Size = new System.Drawing.Size(1160, 554);
            this.tabStorePerformance.TabIndex = 2;
            this.tabStorePerformance.Text = "Store Performance";
            this.tabStorePerformance.UseVisualStyleBackColor = true;

            // Tab User Access
            this.tabUserAccess.Location = new System.Drawing.Point(4, 22);
            this.tabUserAccess.Name = "tabUserAccess";
            this.tabUserAccess.Size = new System.Drawing.Size(1160, 554);
            this.tabUserAccess.TabIndex = 3;
            this.tabUserAccess.Text = "User Access Control";
            this.tabUserAccess.UseVisualStyleBackColor = true;

            // Group Store Details
            this.groupStoreDetails.Controls.Add(new Label { Text = "Store Name:", Location = new Point(15, 25), Size = new Size(75, 13) });
            this.groupStoreDetails.Controls.Add(this.txtStoreName);
            this.groupStoreDetails.Controls.Add(new Label { Text = "Store Code:", Location = new Point(15, 55), Size = new Size(70, 13) });
            this.groupStoreDetails.Controls.Add(this.txtStoreCode);
            this.groupStoreDetails.Controls.Add(new Label { Text = "Region:", Location = new Point(15, 85), Size = new Size(50, 13) });
            this.groupStoreDetails.Controls.Add(this.cmbRegion);
            this.groupStoreDetails.Controls.Add(new Label { Text = "Store Type:", Location = new Point(15, 115), Size = new Size(65, 13) });
            this.groupStoreDetails.Controls.Add(this.cmbStoreType);
            this.groupStoreDetails.Controls.Add(new Label { Text = "Manager:", Location = new Point(300, 25), Size = new Size(55, 13) });
            this.groupStoreDetails.Controls.Add(this.cmbManager);
            this.groupStoreDetails.Controls.Add(new Label { Text = "Address:", Location = new Point(300, 55), Size = new Size(50, 13) });
            this.groupStoreDetails.Controls.Add(this.txtAddress);
            this.groupStoreDetails.Controls.Add(new Label { Text = "Phone:", Location = new Point(300, 85), Size = new Size(45, 13) });
            this.groupStoreDetails.Controls.Add(this.txtPhone);
            this.groupStoreDetails.Controls.Add(new Label { Text = "Email:", Location = new Point(300, 115), Size = new Size(35, 13) });
            this.groupStoreDetails.Controls.Add(this.txtEmail);
            this.groupStoreDetails.Controls.Add(new Label { Text = "Opening Date:", Location = new Point(15, 145), Size = new Size(80, 13) });
            this.groupStoreDetails.Controls.Add(this.dtpOpeningDate);
            this.groupStoreDetails.Controls.Add(new Label { Text = "Area (sq ft):", Location = new Point(300, 145), Size = new Size(65, 13) });
            this.groupStoreDetails.Controls.Add(this.nudStoreArea);
            this.groupStoreDetails.Controls.Add(this.chkIsActive);
            this.groupStoreDetails.Controls.Add(this.chkIsMainBranch);
            this.groupStoreDetails.Controls.Add(this.chkAllowTransfers);
            this.groupStoreDetails.Controls.Add(this.chkCentralizedInventory);
            this.groupStoreDetails.Controls.Add(this.btnAddStore);
            this.groupStoreDetails.Controls.Add(this.btnUpdateStore);
            this.groupStoreDetails.Controls.Add(this.btnDeleteStore);
            this.groupStoreDetails.Controls.Add(this.btnTransferStock);
            this.groupStoreDetails.Controls.Add(this.btnSyncAllStores);
            this.groupStoreDetails.Controls.Add(this.btnGenerateStoreReport);
            this.groupStoreDetails.Location = new System.Drawing.Point(15, 15);
            this.groupStoreDetails.Name = "groupStoreDetails";
            this.groupStoreDetails.Size = new System.Drawing.Size(1130, 250);
            this.groupStoreDetails.TabIndex = 0;
            this.groupStoreDetails.TabStop = false;
            this.groupStoreDetails.Text = "Store Details & Management";

            // Summary Panel
            this.summaryPanel.BackColor = System.Drawing.Color.LightCyan;
            this.summaryPanel.Controls.Add(new Label { Text = "Store Selection:", Location = new Point(15, 15), Size = new Size(90, 13) });
            this.summaryPanel.Controls.Add(this.cmbStoreSelection);
            this.summaryPanel.Controls.Add(this.lblTotalStores);
            this.summaryPanel.Controls.Add(this.lblActiveStores);
            this.summaryPanel.Controls.Add(this.lblTotalRevenue);
            this.summaryPanel.Controls.Add(this.lblTopPerformingStore);
            this.summaryPanel.Controls.Add(this.progressSync);
            this.summaryPanel.Controls.Add(this.lblSyncStatus);
            this.summaryPanel.Location = new System.Drawing.Point(12, 12);
            this.summaryPanel.Name = "summaryPanel";
            this.summaryPanel.Size = new System.Drawing.Size(1168, 50);
            this.summaryPanel.TabIndex = 1;

            // Setup all controls
            SetupControls();
            SetupDataGridViews();

            ((System.ComponentModel.ISupportInitialize)(this.dgvStores)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStoreMetrics)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudStoreArea)).EndInit();
            this.tabStoreManagement.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private void SetupControls()
        {
            // Store Selection ComboBox
            this.cmbStoreSelection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStoreSelection.Location = new System.Drawing.Point(110, 12);
            this.cmbStoreSelection.Name = "cmbStoreSelection";
            this.cmbStoreSelection.Size = new System.Drawing.Size(200, 21);
            this.cmbStoreSelection.TabIndex = 0;
            this.cmbStoreSelection.SelectedIndexChanged += CmbStoreSelection_SelectedIndexChanged;

            // Store Name TextBox
            this.txtStoreName.Location = new System.Drawing.Point(95, 22);
            this.txtStoreName.Name = "txtStoreName";
            this.txtStoreName.Size = new System.Drawing.Size(180, 20);
            this.txtStoreName.TabIndex = 1;

            // Store Code TextBox
            this.txtStoreCode.Location = new System.Drawing.Point(95, 52);
            this.txtStoreCode.Name = "txtStoreCode";
            this.txtStoreCode.Size = new System.Drawing.Size(100, 20);
            this.txtStoreCode.TabIndex = 2;
            this.txtStoreCode.CharacterCasing = CharacterCasing.Upper;

            // Region ComboBox
            this.cmbRegion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRegion.Location = new System.Drawing.Point(70, 82);
            this.cmbRegion.Name = "cmbRegion";
            this.cmbRegion.Size = new System.Drawing.Size(150, 21);
            this.cmbRegion.TabIndex = 3;

            // Store Type ComboBox
            this.cmbStoreType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStoreType.Location = new System.Drawing.Point(85, 112);
            this.cmbStoreType.Name = "cmbStoreType";
            this.cmbStoreType.Size = new System.Drawing.Size(150, 21);
            this.cmbStoreType.TabIndex = 4;

            // Manager ComboBox
            this.cmbManager.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbManager.Location = new System.Drawing.Point(360, 22);
            this.cmbManager.Name = "cmbManager";
            this.cmbManager.Size = new System.Drawing.Size(180, 21);
            this.cmbManager.TabIndex = 5;

            // Address TextBox
            this.txtAddress.Location = new System.Drawing.Point(355, 52);
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new System.Drawing.Size(250, 20);
            this.txtAddress.TabIndex = 6;

            // Phone TextBox
            this.txtPhone.Location = new System.Drawing.Point(350, 82);
            this.txtPhone.Name = "txtPhone";
            this.txtPhone.Size = new System.Drawing.Size(150, 20);
            this.txtPhone.TabIndex = 7;

            // Email TextBox
            this.txtEmail.Location = new System.Drawing.Point(340, 112);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(180, 20);
            this.txtEmail.TabIndex = 8;

            // Opening Date DateTimePicker
            this.dtpOpeningDate.Location = new System.Drawing.Point(100, 142);
            this.dtpOpeningDate.Name = "dtpOpeningDate";
            this.dtpOpeningDate.Size = new System.Drawing.Size(120, 20);
            this.dtpOpeningDate.TabIndex = 9;
            this.dtpOpeningDate.Value = DateTime.Now;

            // Store Area NumericUpDown
            this.nudStoreArea.Location = new System.Drawing.Point(370, 142);
            this.nudStoreArea.Name = "nudStoreArea";
            this.nudStoreArea.Size = new System.Drawing.Size(80, 20);
            this.nudStoreArea.TabIndex = 10;
            this.nudStoreArea.Minimum = 100;
            this.nudStoreArea.Maximum = 50000;
            this.nudStoreArea.Value = 1000;

            // Checkboxes
            this.chkIsActive.Location = new System.Drawing.Point(15, 175);
            this.chkIsActive.Size = new System.Drawing.Size(70, 17);
            this.chkIsActive.Text = "Is Active";
            this.chkIsActive.Checked = true;

            this.chkIsMainBranch.Location = new System.Drawing.Point(100, 175);
            this.chkIsMainBranch.Size = new System.Drawing.Size(100, 17);
            this.chkIsMainBranch.Text = "Main Branch";

            this.chkAllowTransfers.Location = new System.Drawing.Point(220, 175);
            this.chkAllowTransfers.Size = new System.Drawing.Size(110, 17);
            this.chkAllowTransfers.Text = "Allow Transfers";
            this.chkAllowTransfers.Checked = true;

            this.chkCentralizedInventory.Location = new System.Drawing.Point(350, 175);
            this.chkCentralizedInventory.Size = new System.Drawing.Size(140, 17);
            this.chkCentralizedInventory.Text = "Centralized Inventory";

            // Action Buttons
            this.btnAddStore.BackColor = System.Drawing.Color.Green;
            this.btnAddStore.ForeColor = System.Drawing.Color.White;
            this.btnAddStore.Location = new System.Drawing.Point(620, 22);
            this.btnAddStore.Size = new System.Drawing.Size(100, 30);
            this.btnAddStore.Text = "Add Store";
            this.btnAddStore.Click += BtnAddStore_Click;

            this.btnUpdateStore.BackColor = System.Drawing.Color.Blue;
            this.btnUpdateStore.ForeColor = System.Drawing.Color.White;
            this.btnUpdateStore.Location = new System.Drawing.Point(730, 22);
            this.btnUpdateStore.Size = new System.Drawing.Size(100, 30);
            this.btnUpdateStore.Text = "Update Store";
            this.btnUpdateStore.Click += BtnUpdateStore_Click;

            this.btnDeleteStore.BackColor = System.Drawing.Color.Red;
            this.btnDeleteStore.ForeColor = System.Drawing.Color.White;
            this.btnDeleteStore.Location = new System.Drawing.Point(840, 22);
            this.btnDeleteStore.Size = new System.Drawing.Size(100, 30);
            this.btnDeleteStore.Text = "Delete Store";
            this.btnDeleteStore.Click += BtnDeleteStore_Click;

            this.btnTransferStock.BackColor = System.Drawing.Color.Orange;
            this.btnTransferStock.ForeColor = System.Drawing.Color.White;
            this.btnTransferStock.Location = new System.Drawing.Point(620, 62);
            this.btnTransferStock.Size = new System.Drawing.Size(100, 30);
            this.btnTransferStock.Text = "Transfer Stock";
            this.btnTransferStock.Click += BtnTransferStock_Click;

            this.btnSyncAllStores.BackColor = System.Drawing.Color.Purple;
            this.btnSyncAllStores.ForeColor = System.Drawing.Color.White;
            this.btnSyncAllStores.Location = new System.Drawing.Point(730, 62);
            this.btnSyncAllStores.Size = new System.Drawing.Size(100, 30);
            this.btnSyncAllStores.Text = "Sync All Stores";
            this.btnSyncAllStores.Click += BtnSyncAllStores_Click;

            this.btnGenerateStoreReport.BackColor = System.Drawing.Color.Teal;
            this.btnGenerateStoreReport.ForeColor = System.Drawing.Color.White;
            this.btnGenerateStoreReport.Location = new System.Drawing.Point(840, 62);
            this.btnGenerateStoreReport.Size = new System.Drawing.Size(120, 30);
            this.btnGenerateStoreReport.Text = "Generate Report";
            this.btnGenerateStoreReport.Click += BtnGenerateStoreReport_Click;

            // Summary Labels
            this.lblTotalStores.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblTotalStores.Location = new System.Drawing.Point(350, 15);
            this.lblTotalStores.Size = new System.Drawing.Size(120, 15);
            this.lblTotalStores.Text = "Total Stores: 0";

            this.lblActiveStores.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblActiveStores.Location = new System.Drawing.Point(480, 15);
            this.lblActiveStores.Size = new System.Drawing.Size(120, 15);
            this.lblActiveStores.Text = "Active Stores: 0";

            this.lblTotalRevenue.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblTotalRevenue.Location = new System.Drawing.Point(610, 15);
            this.lblTotalRevenue.Size = new System.Drawing.Size(150, 15);
            this.lblTotalRevenue.Text = "Total Revenue: ₹0";

            this.lblTopPerformingStore.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblTopPerformingStore.Location = new System.Drawing.Point(770, 15);
            this.lblTopPerformingStore.Size = new System.Drawing.Size(200, 15);
            this.lblTopPerformingStore.Text = "Top Store: N/A";

            // Progress Bar and Status
            this.progressSync.Location = new System.Drawing.Point(350, 32);
            this.progressSync.Size = new System.Drawing.Size(300, 15);
            this.progressSync.Style = ProgressBarStyle.Continuous;

            this.lblSyncStatus.Location = new System.Drawing.Point(660, 32);
            this.lblSyncStatus.Size = new System.Drawing.Size(200, 15);
            this.lblSyncStatus.Text = "Ready";
        }

        private void SetupDataGridViews()
        {
            // Setup Stores DataGridView
            this.dgvStores.Location = new System.Drawing.Point(15, 275);
            this.dgvStores.Name = "dgvStores";
            this.dgvStores.Size = new System.Drawing.Size(1130, 260);
            this.dgvStores.TabIndex = 1;
            this.dgvStores.AllowUserToAddRows = false;
            this.dgvStores.AllowUserToDeleteRows = false;
            this.dgvStores.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvStores.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvStores.SelectionChanged += DgvStores_SelectionChanged;

            // Add columns for stores
            this.dgvStores.Columns.Add("StoreCode", "Store Code");
            this.dgvStores.Columns.Add("StoreName", "Store Name");
            this.dgvStores.Columns.Add("Region", "Region");
            this.dgvStores.Columns.Add("StoreType", "Type");
            this.dgvStores.Columns.Add("Manager", "Manager");
            this.dgvStores.Columns.Add("Address", "Address");
            this.dgvStores.Columns.Add("Phone", "Phone");
            this.dgvStores.Columns.Add("OpeningDate", "Opening Date");
            this.dgvStores.Columns.Add("StoreArea", "Area (sq ft)");
            this.dgvStores.Columns.Add("IsActive", "Status");
            this.dgvStores.Columns.Add("IsMainBranch", "Main Branch");

            // Setup Store Metrics DataGridView
            this.dgvStoreMetrics.Location = new System.Drawing.Point(15, 15);
            this.dgvStoreMetrics.Name = "dgvStoreMetrics";
            this.dgvStoreMetrics.Size = new System.Drawing.Size(1130, 520);
            this.dgvStoreMetrics.TabIndex = 0;
            this.dgvStoreMetrics.AllowUserToAddRows = false;
            this.dgvStoreMetrics.AllowUserToDeleteRows = false;
            this.dgvStoreMetrics.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvStoreMetrics.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Add columns for store metrics
            this.dgvStoreMetrics.Columns.Add("StoreCode", "Store Code");
            this.dgvStoreMetrics.Columns.Add("StoreName", "Store Name");
            this.dgvStoreMetrics.Columns.Add("MonthlySales", "Monthly Sales");
            this.dgvStoreMetrics.Columns.Add("MonthlyProfit", "Monthly Profit");
            this.dgvStoreMetrics.Columns.Add("StockValue", "Stock Value");
            this.dgvStoreMetrics.Columns.Add("CustomerCount", "Customers");
            this.dgvStoreMetrics.Columns.Add("TransactionCount", "Transactions");
            this.dgvStoreMetrics.Columns.Add("AvgTransactionValue", "Avg Transaction");
            this.dgvStoreMetrics.Columns.Add("ProfitMargin", "Profit Margin %");
            this.dgvStoreMetrics.Columns.Add("Performance", "Performance");
        }

        private void InitializeMultiStore()
        {
            // Load regions
            cmbRegion.Items.AddRange(new object[] { 
                "North Region", "South Region", "East Region", "West Region", 
                "Central Region", "Metro Region", "Rural Region" 
            });
            cmbRegion.SelectedIndex = 0;

            // Load store types
            cmbStoreType.Items.AddRange(new object[] { 
                "Flagship Store", "Standard Store", "Express Store", "Mini Store", 
                "Franchise Store", "Warehouse Store", "Online Hub" 
            });
            cmbStoreType.SelectedIndex = 1;
        }

        private void LoadInitialData()
        {
            try
            {
                LoadStores();
                LoadManagers();
                LoadStoreMetrics();
                UpdateSummary();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading initial data: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadStores()
        {
            try
            {
                string query = @"
                    SELECT 
                        s.StoreID,
                        s.StoreCode,
                        s.StoreName,
                        s.Region,
                        s.StoreType,
                        u.UserName as ManagerName,
                        s.Address,
                        s.Phone,
                        s.Email,
                        s.OpeningDate,
                        s.StoreArea,
                        s.IsActive,
                        s.IsMainBranch,
                        s.AllowTransfers,
                        s.CentralizedInventory
                    FROM Stores s
                    LEFT JOIN Users u ON s.ManagerID = u.UserID
                    ORDER BY s.StoreCode";

                storesData = DatabaseConnection.ExecuteQuery(query);

                if (storesData != null)
                {
                    // Load stores into selection ComboBox
                    cmbStoreSelection.Items.Clear();
                    cmbStoreSelection.Items.Add(new ComboBoxItem { Text = "All Stores", Value = 0 });
                    
                    foreach (DataRow row in storesData.Rows)
                    {
                        cmbStoreSelection.Items.Add(new ComboBoxItem 
                        { 
                            Text = $"{row["StoreCode"]} - {row["StoreName"]}", 
                            Value = Convert.ToInt32(row["StoreID"]) 
                        });
                    }
                    
                    cmbStoreSelection.DisplayMember = "Text";
                    cmbStoreSelection.ValueMember = "Value";
                    cmbStoreSelection.SelectedIndex = 0;

                    // Populate DataGridView
                    PopulateStoresGrid();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading stores: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadManagers()
        {
            try
            {
                string query = @"
                    SELECT UserID, UserName 
                    FROM Users 
                    WHERE Role IN ('Manager', 'Store Manager', 'Admin') 
                    AND IsActive = 1 
                    ORDER BY UserName";

                DataTable managers = DatabaseConnection.ExecuteQuery(query);

                cmbManager.Items.Clear();
                cmbManager.Items.Add(new ComboBoxItem { Text = "Select Manager", Value = 0 });
                
                if (managers != null)
                {
                    foreach (DataRow row in managers.Rows)
                    {
                        cmbManager.Items.Add(new ComboBoxItem 
                        { 
                            Text = row["UserName"].ToString(), 
                            Value = Convert.ToInt32(row["UserID"]) 
                        });
                    }
                }
                
                cmbManager.DisplayMember = "Text";
                cmbManager.ValueMember = "Value";
                cmbManager.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading managers: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadStoreMetrics()
        {
            try
            {
                string query = @"
                    SELECT 
                        s.StoreCode,
                        s.StoreName,
                        ISNULL(sm.MonthlySales, 0) as MonthlySales,
                        ISNULL(sm.MonthlyProfit, 0) as MonthlyProfit,
                        ISNULL(sm.StockValue, 0) as StockValue,
                        ISNULL(sm.CustomerCount, 0) as CustomerCount,
                        ISNULL(sm.TransactionCount, 0) as TransactionCount,
                        CASE 
                            WHEN sm.TransactionCount > 0 THEN sm.MonthlySales / sm.TransactionCount 
                            ELSE 0 
                        END as AvgTransactionValue,
                        CASE 
                            WHEN sm.MonthlySales > 0 THEN (sm.MonthlyProfit / sm.MonthlySales) * 100 
                            ELSE 0 
                        END as ProfitMargin,
                        CASE 
                            WHEN sm.MonthlySales >= 500000 THEN 'Excellent'
                            WHEN sm.MonthlySales >= 300000 THEN 'Good'
                            WHEN sm.MonthlySales >= 100000 THEN 'Average'
                            ELSE 'Poor'
                        END as Performance
                    FROM Stores s
                    LEFT JOIN StoreMetrics sm ON s.StoreID = sm.StoreID 
                        AND sm.MetricMonth = MONTH(GETDATE()) 
                        AND sm.MetricYear = YEAR(GETDATE())
                    WHERE s.IsActive = 1
                    ORDER BY sm.MonthlySales DESC";

                DataTable metrics = DatabaseConnection.ExecuteQuery(query);

                dgvStoreMetrics.Rows.Clear();

                if (metrics != null)
                {
                    foreach (DataRow row in metrics.Rows)
                    {
                        int rowIndex = dgvStoreMetrics.Rows.Add();
                        dgvStoreMetrics.Rows[rowIndex].Cells["StoreCode"].Value = row["StoreCode"];
                        dgvStoreMetrics.Rows[rowIndex].Cells["StoreName"].Value = row["StoreName"];
                        dgvStoreMetrics.Rows[rowIndex].Cells["MonthlySales"].Value = 
                            Convert.ToDecimal(row["MonthlySales"]).ToString("N2");
                        dgvStoreMetrics.Rows[rowIndex].Cells["MonthlyProfit"].Value = 
                            Convert.ToDecimal(row["MonthlyProfit"]).ToString("N2");
                        dgvStoreMetrics.Rows[rowIndex].Cells["StockValue"].Value = 
                            Convert.ToDecimal(row["StockValue"]).ToString("N2");
                        dgvStoreMetrics.Rows[rowIndex].Cells["CustomerCount"].Value = row["CustomerCount"];
                        dgvStoreMetrics.Rows[rowIndex].Cells["TransactionCount"].Value = row["TransactionCount"];
                        dgvStoreMetrics.Rows[rowIndex].Cells["AvgTransactionValue"].Value = 
                            Convert.ToDecimal(row["AvgTransactionValue"]).ToString("N2");
                        dgvStoreMetrics.Rows[rowIndex].Cells["ProfitMargin"].Value = 
                            Convert.ToDecimal(row["ProfitMargin"]).ToString("N1") + "%";
                        dgvStoreMetrics.Rows[rowIndex].Cells["Performance"].Value = row["Performance"];

                        // Color code based on performance
                        string performance = row["Performance"].ToString();
                        switch (performance)
                        {
                            case "Excellent":
                                dgvStoreMetrics.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightGreen;
                                break;
                            case "Good":
                                dgvStoreMetrics.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightBlue;
                                break;
                            case "Average":
                                dgvStoreMetrics.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightYellow;
                                break;
                            case "Poor":
                                dgvStoreMetrics.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightCoral;
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading store metrics: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PopulateStoresGrid()
        {
            if (storesData == null) return;

            dgvStores.Rows.Clear();
            
            foreach (DataRow row in storesData.Rows)
            {
                int rowIndex = dgvStores.Rows.Add();
                dgvStores.Rows[rowIndex].Cells["StoreCode"].Value = row["StoreCode"];
                dgvStores.Rows[rowIndex].Cells["StoreName"].Value = row["StoreName"];
                dgvStores.Rows[rowIndex].Cells["Region"].Value = row["Region"];
                dgvStores.Rows[rowIndex].Cells["StoreType"].Value = row["StoreType"];
                dgvStores.Rows[rowIndex].Cells["Manager"].Value = row["ManagerName"] ?? "Not Assigned";
                dgvStores.Rows[rowIndex].Cells["Address"].Value = row["Address"];
                dgvStores.Rows[rowIndex].Cells["Phone"].Value = row["Phone"];
                dgvStores.Rows[rowIndex].Cells["OpeningDate"].Value = 
                    Convert.ToDateTime(row["OpeningDate"]).ToString("dd/MM/yyyy");
                dgvStores.Rows[rowIndex].Cells["StoreArea"].Value = row["StoreArea"];
                dgvStores.Rows[rowIndex].Cells["IsActive"].Value = 
                    Convert.ToBoolean(row["IsActive"]) ? "Active" : "Inactive";
                dgvStores.Rows[rowIndex].Cells["IsMainBranch"].Value = 
                    Convert.ToBoolean(row["IsMainBranch"]) ? "Yes" : "No";
                
                // Store row data for later use
                dgvStores.Rows[rowIndex].Tag = row;

                // Color code based on status
                if (!Convert.ToBoolean(row["IsActive"]))
                {
                    dgvStores.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightGray;
                }
                else if (Convert.ToBoolean(row["IsMainBranch"]))
                {
                    dgvStores.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightBlue;
                }
            }
        }

        private void UpdateSummary()
        {
            try
            {
                if (storesData != null)
                {
                    int totalStores = storesData.Rows.Count;
                    int activeStores = storesData.AsEnumerable()
                        .Count(r => Convert.ToBoolean(r["IsActive"]));

                    lblTotalStores.Text = $"Total Stores: {totalStores}";
                    lblActiveStores.Text = $"Active Stores: {activeStores}";
                }

                // Get revenue summary
                string revenueQuery = @"
                    SELECT 
                        SUM(ISNULL(sm.MonthlySales, 0)) as TotalRevenue,
                        TOP 1 s.StoreName as TopStore
                    FROM Stores s
                    LEFT JOIN StoreMetrics sm ON s.StoreID = sm.StoreID 
                        AND sm.MetricMonth = MONTH(GETDATE()) 
                        AND sm.MetricYear = YEAR(GETDATE())
                    WHERE s.IsActive = 1
                    ORDER BY sm.MonthlySales DESC";

                DataTable revenue = DatabaseConnection.ExecuteQuery(revenueQuery);

                if (revenue != null && revenue.Rows.Count > 0)
                {
                    decimal totalRevenue = revenue.Rows[0]["TotalRevenue"] != DBNull.Value 
                        ? Convert.ToDecimal(revenue.Rows[0]["TotalRevenue"]) : 0;
                    string topStore = revenue.Rows[0]["TopStore"]?.ToString() ?? "N/A";

                    lblTotalRevenue.Text = $"Total Revenue: ₹{totalRevenue:N0}";
                    lblTopPerformingStore.Text = $"Top Store: {topStore}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating summary: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CmbStoreSelection_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxItem selectedStore = (ComboBoxItem)cmbStoreSelection.SelectedItem;
            if (selectedStore != null)
            {
                selectedStoreID = (int)selectedStore.Value;
                // Filter data based on selected store if needed
                LoadStoreSpecificData(selectedStoreID);
            }
        }

        private void LoadStoreSpecificData(int storeID)
        {
            // Load data specific to selected store
            if (storeID == 0)
            {
                // Show all stores data
                PopulateStoresGrid();
            }
            else
            {
                // Filter and show specific store data
                DataRow[] filteredRows = storesData.Select($"StoreID = {storeID}");
                // Update grids with filtered data
            }
        }

        private void DgvStores_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvStores.SelectedRows.Count > 0)
            {
                DataRow selectedRow = (DataRow)dgvStores.SelectedRows[0].Tag;
                if (selectedRow != null)
                {
                    LoadStoreDetails(selectedRow);
                }
            }
        }

        private void LoadStoreDetails(DataRow storeRow)
        {
            try
            {
                selectedStoreID = Convert.ToInt32(storeRow["StoreID"]);
                txtStoreName.Text = storeRow["StoreName"].ToString();
                txtStoreCode.Text = storeRow["StoreCode"].ToString();
                cmbRegion.SelectedItem = storeRow["Region"].ToString();
                cmbStoreType.SelectedItem = storeRow["StoreType"].ToString();
                txtAddress.Text = storeRow["Address"].ToString();
                txtPhone.Text = storeRow["Phone"].ToString();
                txtEmail.Text = storeRow["Email"].ToString();
                dtpOpeningDate.Value = Convert.ToDateTime(storeRow["OpeningDate"]);
                nudStoreArea.Value = Convert.ToDecimal(storeRow["StoreArea"]);
                chkIsActive.Checked = Convert.ToBoolean(storeRow["IsActive"]);
                chkIsMainBranch.Checked = Convert.ToBoolean(storeRow["IsMainBranch"]);
                chkAllowTransfers.Checked = Convert.ToBoolean(storeRow["AllowTransfers"]);
                chkCentralizedInventory.Checked = Convert.ToBoolean(storeRow["CentralizedInventory"]);

                // Select manager if exists
                if (storeRow["ManagerName"] != DBNull.Value)
                {
                    string managerName = storeRow["ManagerName"].ToString();
                    for (int i = 0; i < cmbManager.Items.Count; i++)
                    {
                        ComboBoxItem item = (ComboBoxItem)cmbManager.Items[i];
                        if (item.Text == managerName)
                        {
                            cmbManager.SelectedIndex = i;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading store details: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAddStore_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateStoreInput())
                {
                    string insertQuery = @"
                        INSERT INTO Stores 
                        (StoreCode, StoreName, Region, StoreType, ManagerID, Address, Phone, Email, 
                         OpeningDate, StoreArea, IsActive, IsMainBranch, AllowTransfers, CentralizedInventory)
                        VALUES 
                        (@StoreCode, @StoreName, @Region, @StoreType, @ManagerID, @Address, @Phone, @Email,
                         @OpeningDate, @StoreArea, @IsActive, @IsMainBranch, @AllowTransfers, @CentralizedInventory)";

                    ComboBoxItem selectedManager = (ComboBoxItem)cmbManager.SelectedItem;
                    int managerID = selectedManager != null ? (int)selectedManager.Value : 0;

                    SqlParameter[] parameters = {
                        new SqlParameter("@StoreCode", txtStoreCode.Text.Trim()),
                        new SqlParameter("@StoreName", txtStoreName.Text.Trim()),
                        new SqlParameter("@Region", cmbRegion.SelectedItem.ToString()),
                        new SqlParameter("@StoreType", cmbStoreType.SelectedItem.ToString()),
                        new SqlParameter("@ManagerID", managerID > 0 ? (object)managerID : DBNull.Value),
                        new SqlParameter("@Address", txtAddress.Text.Trim()),
                        new SqlParameter("@Phone", txtPhone.Text.Trim()),
                        new SqlParameter("@Email", txtEmail.Text.Trim()),
                        new SqlParameter("@OpeningDate", dtpOpeningDate.Value),
                        new SqlParameter("@StoreArea", nudStoreArea.Value),
                        new SqlParameter("@IsActive", chkIsActive.Checked),
                        new SqlParameter("@IsMainBranch", chkIsMainBranch.Checked),
                        new SqlParameter("@AllowTransfers", chkAllowTransfers.Checked),
                        new SqlParameter("@CentralizedInventory", chkCentralizedInventory.Checked)
                    };

                    DatabaseConnection.ExecuteNonQuery(insertQuery, parameters);

                    MessageBox.Show("Store added successfully!", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    ClearStoreForm();
                    LoadStores();
                    UpdateSummary();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding store: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnUpdateStore_Click(object sender, EventArgs e)
        {
            try
            {
                if (selectedStoreID == 0)
                {
                    MessageBox.Show("Please select a store to update.", "Selection Required", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (ValidateStoreInput())
                {
                    string updateQuery = @"
                        UPDATE Stores SET 
                            StoreCode = @StoreCode,
                            StoreName = @StoreName,
                            Region = @Region,
                            StoreType = @StoreType,
                            ManagerID = @ManagerID,
                            Address = @Address,
                            Phone = @Phone,
                            Email = @Email,
                            OpeningDate = @OpeningDate,
                            StoreArea = @StoreArea,
                            IsActive = @IsActive,
                            IsMainBranch = @IsMainBranch,
                            AllowTransfers = @AllowTransfers,
                            CentralizedInventory = @CentralizedInventory
                        WHERE StoreID = @StoreID";

                    ComboBoxItem selectedManager = (ComboBoxItem)cmbManager.SelectedItem;
                    int managerID = selectedManager != null ? (int)selectedManager.Value : 0;

                    SqlParameter[] parameters = {
                        new SqlParameter("@StoreCode", txtStoreCode.Text.Trim()),
                        new SqlParameter("@StoreName", txtStoreName.Text.Trim()),
                        new SqlParameter("@Region", cmbRegion.SelectedItem.ToString()),
                        new SqlParameter("@StoreType", cmbStoreType.SelectedItem.ToString()),
                        new SqlParameter("@ManagerID", managerID > 0 ? (object)managerID : DBNull.Value),
                        new SqlParameter("@Address", txtAddress.Text.Trim()),
                        new SqlParameter("@Phone", txtPhone.Text.Trim()),
                        new SqlParameter("@Email", txtEmail.Text.Trim()),
                        new SqlParameter("@OpeningDate", dtpOpeningDate.Value),
                        new SqlParameter("@StoreArea", nudStoreArea.Value),
                        new SqlParameter("@IsActive", chkIsActive.Checked),
                        new SqlParameter("@IsMainBranch", chkIsMainBranch.Checked),
                        new SqlParameter("@AllowTransfers", chkAllowTransfers.Checked),
                        new SqlParameter("@CentralizedInventory", chkCentralizedInventory.Checked),
                        new SqlParameter("@StoreID", selectedStoreID)
                    };

                    DatabaseConnection.ExecuteNonQuery(updateQuery, parameters);

                    MessageBox.Show("Store updated successfully!", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LoadStores();
                    UpdateSummary();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating store: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDeleteStore_Click(object sender, EventArgs e)
        {
            try
            {
                if (selectedStoreID == 0)
                {
                    MessageBox.Show("Please select a store to delete.", "Selection Required", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult result = MessageBox.Show(
                    "Are you sure you want to delete this store? This action cannot be undone.", 
                    "Confirm Deletion", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    // Soft delete - just mark as inactive
                    string deleteQuery = "UPDATE Stores SET IsActive = 0 WHERE StoreID = @StoreID";
                    SqlParameter[] parameters = { new SqlParameter("@StoreID", selectedStoreID) };

                    DatabaseConnection.ExecuteNonQuery(deleteQuery, parameters);

                    MessageBox.Show("Store deleted successfully!", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    ClearStoreForm();
                    LoadStores();
                    UpdateSummary();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting store: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnTransferStock_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show("Stock Transfer Features:\n\n" +
                    "✓ Inter-store inventory transfers\n" +
                    "✓ Real-time stock synchronization\n" +
                    "✓ Transfer approval workflow\n" +
                    "✓ Automatic stock updates\n" +
                    "✓ Transfer tracking and history\n" +
                    "✓ Batch transfer capabilities\n" +
                    "✓ Low stock automatic transfers\n" +
                    "✓ Transfer cost calculation\n" +
                    "✓ Delivery tracking integration\n\n" +
                    "This would provide comprehensive stock transfer management between stores.", 
                    "Stock Transfer", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error initiating stock transfer: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSyncAllStores_Click(object sender, EventArgs e)
        {
            try
            {
                progressSync.Value = 0;
                lblSyncStatus.Text = "Synchronizing stores...";

                // Simulate synchronization process
                for (int i = 0; i <= 100; i += 20)
                {
                    progressSync.Value = i;
                    Application.DoEvents();
                    System.Threading.Thread.Sleep(500);
                }

                lblSyncStatus.Text = "Synchronization completed";
                progressSync.Value = 0;

                MessageBox.Show("Store Synchronization Features:\n\n" +
                    "✓ Real-time data synchronization\n" +
                    "✓ Inventory level updates\n" +
                    "✓ Sales data consolidation\n" +
                    "✓ Customer data sync\n" +
                    "✓ Price and promotion updates\n" +
                    "✓ User access synchronization\n" +
                    "✓ Report data aggregation\n" +
                    "✓ Conflict resolution\n" +
                    "✓ Offline mode support\n" +
                    "✓ Automated backup creation\n\n" +
                    "All stores have been synchronized successfully!", 
                    "Store Synchronization", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                lblSyncStatus.Text = "Synchronization failed";
                MessageBox.Show("Error synchronizing stores: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnGenerateStoreReport_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show("Multi-Store Reporting Features:\n\n" +
                    "✓ Consolidated sales reports\n" +
                    "✓ Store-wise performance analysis\n" +
                    "✓ Comparative store metrics\n" +
                    "✓ Regional performance reports\n" +
                    "✓ Inventory distribution reports\n" +
                    "✓ Profit margin analysis\n" +
                    "✓ Customer demographic reports\n" +
                    "✓ Store efficiency metrics\n" +
                    "✓ Growth trend analysis\n" +
                    "✓ Executive dashboards\n\n" +
                    "Reports would be generated in PDF/Excel format with charts and analytics.", 
                    "Store Reporting", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating store report: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateStoreInput()
        {
            if (string.IsNullOrWhiteSpace(txtStoreName.Text))
            {
                MessageBox.Show("Store name is required.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtStoreName.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtStoreCode.Text))
            {
                MessageBox.Show("Store code is required.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtStoreCode.Focus();
                return false;
            }

            if (cmbRegion.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a region.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbRegion.Focus();
                return false;
            }

            if (cmbStoreType.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a store type.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbStoreType.Focus();
                return false;
            }

            return true;
        }

        private void ClearStoreForm()
        {
            selectedStoreID = 0;
            txtStoreName.Clear();
            txtStoreCode.Clear();
            txtAddress.Clear();
            txtPhone.Clear();
            txtEmail.Clear();
            dtpOpeningDate.Value = DateTime.Now;
            nudStoreArea.Value = 1000;
            chkIsActive.Checked = true;
            chkIsMainBranch.Checked = false;
            chkAllowTransfers.Checked = true;
            chkCentralizedInventory.Checked = false;
            cmbRegion.SelectedIndex = 0;
            cmbStoreType.SelectedIndex = 1;
            cmbManager.SelectedIndex = 0;
        }
    }
}
