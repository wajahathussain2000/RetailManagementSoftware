using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using RetailManagement.Database;
using System.IO;
using System.Threading.Tasks;

namespace RetailManagement.UserForms
{
    public partial class DatabaseBackupManager : Form
    {
        private Timer autoBackupTimer;
        private string defaultBackupPath;

        public DatabaseBackupManager()
        {
            InitializeComponent();
            InitializeBackupManager();
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panel1 = new Panel();
            this.panel2 = new Panel();
            this.groupBox1 = new GroupBox();
            this.groupBox2 = new GroupBox();
            this.groupBox3 = new GroupBox();
            
            // Controls
            this.lblBackupPath = new Label();
            this.txtBackupPath = new TextBox();
            this.btnBrowse = new Button();
            this.btnBackupNow = new Button();
            this.btnRestore = new Button();
            this.btnScheduleBackup = new Button();
            this.btnViewBackups = new Button();
            
            this.chkAutoBackup = new CheckBox();
            this.cmbBackupFrequency = new ComboBox();
            this.dtpBackupTime = new DateTimePicker();
            this.lblStatus = new Label();
            this.progressBar = new ProgressBar();
            
            this.dgvBackupHistory = new DataGridView();
            this.lblLastBackup = new Label();
            this.lblBackupSize = new Label();
            
            // Form properties
            this.Text = "Database Backup Manager";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;
            
            SetupControls();
            SetupEvents();
        }

        private void SetupControls()
        {
            // Header Panel
            panel1.Dock = DockStyle.Top;
            panel1.Height = 60;
            panel1.BackColor = Color.FromArgb(52, 152, 219);
            this.Controls.Add(panel1);

            Label titleLabel = new Label();
            titleLabel.Text = "Database Backup Manager";
            titleLabel.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            titleLabel.ForeColor = Color.White;
            titleLabel.Location = new Point(20, 15);
            titleLabel.AutoSize = true;
            panel1.Controls.Add(titleLabel);

            // Main Panel
            panel2.Dock = DockStyle.Fill;
            panel2.Padding = new Padding(20);
            this.Controls.Add(panel2);

            // Manual Backup Group
            groupBox1.Text = "Manual Backup";
            groupBox1.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            groupBox1.Location = new Point(20, 20);
            groupBox1.Size = new Size(740, 120);
            panel2.Controls.Add(groupBox1);

            lblBackupPath.Text = "Backup Location:";
            lblBackupPath.Location = new Point(20, 30);
            lblBackupPath.AutoSize = true;
            groupBox1.Controls.Add(lblBackupPath);

            txtBackupPath.Location = new Point(20, 50);
            txtBackupPath.Size = new Size(500, 25);
            groupBox1.Controls.Add(txtBackupPath);

            btnBrowse.Text = "Browse";
            btnBrowse.Location = new Point(530, 50);
            btnBrowse.Size = new Size(80, 25);
            btnBrowse.BackColor = Color.FromArgb(52, 152, 219);
            btnBrowse.ForeColor = Color.White;
            groupBox1.Controls.Add(btnBrowse);

            btnBackupNow.Text = "Backup Now";
            btnBackupNow.Location = new Point(620, 50);
            btnBackupNow.Size = new Size(100, 25);
            btnBackupNow.BackColor = Color.FromArgb(46, 204, 113);
            btnBackupNow.ForeColor = Color.White;
            groupBox1.Controls.Add(btnBackupNow);

            btnRestore.Text = "Restore Database";
            btnRestore.Location = new Point(20, 85);
            btnRestore.Size = new Size(120, 25);
            btnRestore.BackColor = Color.FromArgb(231, 76, 60);
            btnRestore.ForeColor = Color.White;
            groupBox1.Controls.Add(btnRestore);

            progressBar.Location = new Point(150, 85);
            progressBar.Size = new Size(470, 25);
            progressBar.Visible = false;
            groupBox1.Controls.Add(progressBar);

            lblStatus.Text = "Ready";
            lblStatus.Location = new Point(630, 85);
            lblStatus.AutoSize = true;
            lblStatus.ForeColor = Color.Green;
            groupBox1.Controls.Add(lblStatus);

            // Auto Backup Group
            groupBox2.Text = "Automatic Backup Settings";
            groupBox2.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            groupBox2.Location = new Point(20, 160);
            groupBox2.Size = new Size(740, 100);
            panel2.Controls.Add(groupBox2);

            chkAutoBackup.Text = "Enable Automatic Backup";
            chkAutoBackup.Location = new Point(20, 30);
            chkAutoBackup.AutoSize = true;
            groupBox2.Controls.Add(chkAutoBackup);

            Label lblFrequency = new Label();
            lblFrequency.Text = "Frequency:";
            lblFrequency.Location = new Point(200, 30);
            lblFrequency.AutoSize = true;
            groupBox2.Controls.Add(lblFrequency);

            cmbBackupFrequency.Location = new Point(270, 27);
            cmbBackupFrequency.Size = new Size(120, 25);
            cmbBackupFrequency.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbBackupFrequency.Items.AddRange(new object[] { "Daily", "Weekly", "Monthly" });
            cmbBackupFrequency.SelectedIndex = 0;
            groupBox2.Controls.Add(cmbBackupFrequency);

            Label lblTime = new Label();
            lblTime.Text = "Time:";
            lblTime.Location = new Point(410, 30);
            lblTime.AutoSize = true;
            groupBox2.Controls.Add(lblTime);

            dtpBackupTime.Location = new Point(450, 27);
            dtpBackupTime.Size = new Size(120, 25);
            dtpBackupTime.Format = DateTimePickerFormat.Time;
            dtpBackupTime.ShowUpDown = true;
            groupBox2.Controls.Add(dtpBackupTime);

            btnScheduleBackup.Text = "Save Schedule";
            btnScheduleBackup.Location = new Point(590, 27);
            btnScheduleBackup.Size = new Size(100, 25);
            btnScheduleBackup.BackColor = Color.FromArgb(155, 89, 182);
            btnScheduleBackup.ForeColor = Color.White;
            groupBox2.Controls.Add(btnScheduleBackup);

            // Backup History Group
            groupBox3.Text = "Backup History";
            groupBox3.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            groupBox3.Location = new Point(20, 280);
            groupBox3.Size = new Size(740, 250);
            panel2.Controls.Add(groupBox3);

            lblLastBackup.Text = "Last Backup: Loading...";
            lblLastBackup.Location = new Point(20, 30);
            lblLastBackup.AutoSize = true;
            groupBox3.Controls.Add(lblLastBackup);

            lblBackupSize.Text = "Database Size: Loading...";
            lblBackupSize.Location = new Point(400, 30);
            lblBackupSize.AutoSize = true;
            groupBox3.Controls.Add(lblBackupSize);

            btnViewBackups.Text = "Refresh History";
            btnViewBackups.Location = new Point(620, 25);
            btnViewBackups.Size = new Size(100, 25);
            btnViewBackups.BackColor = Color.FromArgb(52, 73, 94);
            btnViewBackups.ForeColor = Color.White;
            groupBox3.Controls.Add(btnViewBackups);

            // DataGridView for backup history
            dgvBackupHistory.Location = new Point(20, 60);
            dgvBackupHistory.Size = new Size(700, 170);
            dgvBackupHistory.AutoGenerateColumns = false;
            SetupBackupHistoryGrid();
            groupBox3.Controls.Add(dgvBackupHistory);
        }

        private void SetupBackupHistoryGrid()
        {
            dgvBackupHistory.Columns.Clear();
            
            dgvBackupHistory.Columns.Add("BackupDate", "Backup Date");
            dgvBackupHistory.Columns.Add("BackupType", "Type");
            dgvBackupHistory.Columns.Add("BackupSize", "Size (MB)");
            dgvBackupHistory.Columns.Add("BackupPath", "File Path");
            dgvBackupHistory.Columns.Add("Status", "Status");
            dgvBackupHistory.Columns.Add("UserName", "Created By");

            // Configure columns
            dgvBackupHistory.Columns["BackupDate"].Width = 130;
            dgvBackupHistory.Columns["BackupType"].Width = 80;
            dgvBackupHistory.Columns["BackupSize"].Width = 80;
            dgvBackupHistory.Columns["BackupPath"].Width = 250;
            dgvBackupHistory.Columns["Status"].Width = 80;
            dgvBackupHistory.Columns["UserName"].Width = 100;

            // Formatting
            dgvBackupHistory.Columns["BackupSize"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private void InitializeBackupManager()
        {
            // Set default backup path
            defaultBackupPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), 
                "RetailManagement Backups");
            
            if (!Directory.Exists(defaultBackupPath))
            {
                Directory.CreateDirectory(defaultBackupPath);
            }
            
            txtBackupPath.Text = defaultBackupPath;

            // Initialize auto backup timer
            autoBackupTimer = new Timer();
            autoBackupTimer.Interval = 60000; // Check every minute
            autoBackupTimer.Tick += AutoBackupTimer_Tick;

            LoadBackupSettings();
            LoadBackupHistory();
            LoadDatabaseInfo();
        }

        private void SetupEvents()
        {
            btnBrowse.Click += BtnBrowse_Click;
            btnBackupNow.Click += BtnBackupNow_Click;
            btnRestore.Click += BtnRestore_Click;
            btnScheduleBackup.Click += BtnScheduleBackup_Click;
            btnViewBackups.Click += BtnViewBackups_Click;
            chkAutoBackup.CheckedChanged += ChkAutoBackup_CheckedChanged;
            this.Load += DatabaseBackupManager_Load;
        }

        private void DatabaseBackupManager_Load(object sender, EventArgs e)
        {
            LoadBackupHistory();
        }

        private async void BtnBackupNow_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtBackupPath.Text))
            {
                MessageBox.Show("Please select a backup location.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                btnBackupNow.Enabled = false;
                progressBar.Visible = true;
                progressBar.Style = ProgressBarStyle.Marquee;
                lblStatus.Text = "Creating backup...";
                lblStatus.ForeColor = Color.Orange;

                await Task.Run(() => CreateDatabaseBackup());

                lblStatus.Text = "Backup completed successfully!";
                lblStatus.ForeColor = Color.Green;
                
                MessageBox.Show("Database backup created successfully!", "Success", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadBackupHistory();
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Backup failed!";
                lblStatus.ForeColor = Color.Red;
                MessageBox.Show("Error creating backup: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnBackupNow.Enabled = true;
                progressBar.Visible = false;
            }
        }

        private void CreateDatabaseBackup()
        {
            try
            {
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string backupFileName = $"RetailManagementDB_Backup_{timestamp}.bak";
                string backupFilePath = Path.Combine(txtBackupPath.Text, backupFileName);

                string backupQuery = $@"BACKUP DATABASE [RetailManagementDB] 
                                      TO DISK = '{backupFilePath}' 
                                      WITH FORMAT, INIT, SKIP, NOREWIND, NOUNLOAD, STATS = 10";

                DatabaseConnection.ExecuteNonQuery(backupQuery);

                // Log backup in database
                LogBackupActivity(backupFilePath, "Manual", "Completed");

                // Update last backup info
                this.Invoke(new Action(() => {
                    lblLastBackup.Text = $"Last Backup: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
                }));
            }
            catch (Exception ex)
            {
                // Log failed backup
                LogBackupActivity("", "Manual", "Failed: " + ex.Message);
                throw;
            }
        }

        private void LogBackupActivity(string backupPath, string backupType, string status)
        {
            try
            {
                long fileSize = 0;
                if (File.Exists(backupPath))
                {
                    FileInfo fileInfo = new FileInfo(backupPath);
                    fileSize = fileInfo.Length;
                }

                string query = @"INSERT INTO DatabaseBackup (BackupDate, BackupType, BackupPath, BackupSize, Status, CreatedBy, CreatedDate)
                               VALUES (@BackupDate, @BackupType, @BackupPath, @BackupSize, @Status, @CreatedBy, GETDATE())";

                SqlParameter[] parameters = {
                    new SqlParameter("@BackupDate", DateTime.Now),
                    new SqlParameter("@BackupType", backupType),
                    new SqlParameter("@BackupPath", backupPath),
                    new SqlParameter("@BackupSize", fileSize),
                    new SqlParameter("@Status", status),
                    new SqlParameter("@CreatedBy", UserSession.UserID)
                };

                DatabaseConnection.ExecuteNonQuery(query, parameters);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error logging backup activity: {ex.Message}");
            }
        }

        private void LoadBackupHistory()
        {
            try
            {
                string query = @"SELECT TOP 10 
                               db.BackupDate,
                               db.BackupType,
                               CAST(db.BackupSize / 1048576.0 AS DECIMAL(10,2)) as BackupSizeMB,
                               db.BackupPath,
                               db.Status,
                               u.FullName as UserName
                               FROM DatabaseBackup db
                               LEFT JOIN Users u ON db.CreatedBy = u.UserID
                               ORDER BY db.BackupDate DESC";

                DataTable result = DatabaseConnection.ExecuteQuery(query);
                dgvBackupHistory.DataSource = result;

                // Update last backup info
                if (result.Rows.Count > 0)
                {
                    DateTime lastBackup = Convert.ToDateTime(result.Rows[0]["BackupDate"]);
                    lblLastBackup.Text = $"Last Backup: {lastBackup:dd/MM/yyyy HH:mm:ss}";
                }
                else
                {
                    lblLastBackup.Text = "Last Backup: No backups found";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading backup history: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadDatabaseInfo()
        {
            try
            {
                string query = @"SELECT 
                               SUM(CAST(FILEPROPERTY(name, 'SpaceUsed') AS bigint) * 8192) / 1048576.0 AS DatabaseSizeMB
                               FROM sys.database_files";

                object result = DatabaseConnection.ExecuteScalar(query);
                if (result != null)
                {
                    decimal sizeMB = Convert.ToDecimal(result);
                    lblBackupSize.Text = $"Database Size: {sizeMB:F2} MB";
                }
            }
            catch (Exception ex)
            {
                lblBackupSize.Text = "Database Size: Unable to calculate";
                System.Diagnostics.Debug.WriteLine($"Error getting database size: {ex.Message}");
            }
        }

        private void LoadBackupSettings()
        {
            try
            {
                string query = "SELECT SettingValue FROM SystemSettings WHERE SettingKey = 'AutoBackupEnabled'";
                object result = DatabaseConnection.ExecuteScalar(query);
                chkAutoBackup.Checked = result?.ToString() == "1";

                query = "SELECT SettingValue FROM SystemSettings WHERE SettingKey = 'BackupFrequency'";
                result = DatabaseConnection.ExecuteScalar(query);
                if (result != null)
                {
                    cmbBackupFrequency.Text = result.ToString();
                }

                query = "SELECT SettingValue FROM SystemSettings WHERE SettingKey = 'BackupTime'";
                result = DatabaseConnection.ExecuteScalar(query);
                if (result != null && TimeSpan.TryParse(result.ToString(), out TimeSpan backupTime))
                {
                    dtpBackupTime.Value = DateTime.Today.Add(backupTime);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading backup settings: {ex.Message}");
            }
        }

        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select backup location";
                folderDialog.SelectedPath = txtBackupPath.Text;

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    txtBackupPath.Text = folderDialog.SelectedPath;
                }
            }
        }

        private void BtnRestore_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openDialog = new OpenFileDialog())
            {
                openDialog.Filter = "Backup files (*.bak)|*.bak|All files (*.*)|*.*";
                openDialog.Title = "Select backup file to restore";

                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    DialogResult result = MessageBox.Show(
                        "WARNING: Restoring will replace current database. Continue?", 
                        "Confirm Restore", 
                        MessageBoxButtons.YesNo, 
                        MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        RestoreDatabase(openDialog.FileName);
                    }
                }
            }
        }

        private async void RestoreDatabase(string backupPath)
        {
            try
            {
                btnRestore.Enabled = false;
                progressBar.Visible = true;
                progressBar.Style = ProgressBarStyle.Marquee;
                lblStatus.Text = "Restoring database...";
                lblStatus.ForeColor = Color.Orange;

                await Task.Run(() => {
                    string restoreQuery = $@"
                        ALTER DATABASE [RetailManagementDB] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                        RESTORE DATABASE [RetailManagementDB] FROM DISK = '{backupPath}' WITH REPLACE;
                        ALTER DATABASE [RetailManagementDB] SET MULTI_USER;";

                    DatabaseConnection.ExecuteNonQuery(restoreQuery);
                });

                lblStatus.Text = "Database restored successfully!";
                lblStatus.ForeColor = Color.Green;

                MessageBox.Show("Database restored successfully! Application will restart.", "Success", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                Application.Restart();
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Restore failed!";
                lblStatus.ForeColor = Color.Red;
                MessageBox.Show("Error restoring database: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnRestore.Enabled = true;
                progressBar.Visible = false;
            }
        }

        private void BtnScheduleBackup_Click(object sender, EventArgs e)
        {
            try
            {
                SaveBackupSettings();
                
                if (chkAutoBackup.Checked)
                {
                    autoBackupTimer.Start();
                    MessageBox.Show("Automatic backup scheduled successfully!", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    autoBackupTimer.Stop();
                    MessageBox.Show("Automatic backup disabled.", "Information", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving backup settings: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveBackupSettings()
        {
            // Save auto backup settings to database
            string[] settings = {
                $"AutoBackupEnabled|{(chkAutoBackup.Checked ? "1" : "0")}",
                $"BackupFrequency|{cmbBackupFrequency.Text}",
                $"BackupTime|{dtpBackupTime.Value.TimeOfDay}",
                $"BackupPath|{txtBackupPath.Text}"
            };

            foreach (string setting in settings)
            {
                string[] parts = setting.Split('|');
                string key = parts[0];
                string value = parts[1];

                string query = @"IF EXISTS (SELECT 1 FROM SystemSettings WHERE SettingKey = @Key)
                               UPDATE SystemSettings SET SettingValue = @Value WHERE SettingKey = @Key
                               ELSE
                               INSERT INTO SystemSettings (SettingKey, SettingValue) VALUES (@Key, @Value)";

                SqlParameter[] parameters = {
                    new SqlParameter("@Key", key),
                    new SqlParameter("@Value", value)
                };

                DatabaseConnection.ExecuteNonQuery(query, parameters);
            }
        }

        private void ChkAutoBackup_CheckedChanged(object sender, EventArgs e)
        {
            cmbBackupFrequency.Enabled = chkAutoBackup.Checked;
            dtpBackupTime.Enabled = chkAutoBackup.Checked;
        }

        private void BtnViewBackups_Click(object sender, EventArgs e)
        {
            LoadBackupHistory();
            LoadDatabaseInfo();
        }

        private void AutoBackupTimer_Tick(object sender, EventArgs e)
        {
            if (chkAutoBackup.Checked && ShouldCreateAutoBackup())
            {
                Task.Run(() => CreateDatabaseBackup());
            }
        }

        private bool ShouldCreateAutoBackup()
        {
            try
            {
                string query = @"SELECT TOP 1 BackupDate FROM DatabaseBackup 
                               WHERE BackupType = 'Automatic' AND Status = 'Completed'
                               ORDER BY BackupDate DESC";

                object result = DatabaseConnection.ExecuteScalar(query);
                
                if (result == null) return true; // No previous backup

                DateTime lastBackup = Convert.ToDateTime(result);
                DateTime now = DateTime.Now;
                TimeSpan scheduledTime = dtpBackupTime.Value.TimeOfDay;
                
                // Check if it's time for backup based on frequency
                switch (cmbBackupFrequency.Text)
                {
                    case "Daily":
                        return now.Date > lastBackup.Date && 
                               now.TimeOfDay >= scheduledTime && 
                               now.TimeOfDay <= scheduledTime.Add(TimeSpan.FromMinutes(1));
                    
                    case "Weekly":
                        return now.Date >= lastBackup.Date.AddDays(7) && 
                               now.TimeOfDay >= scheduledTime && 
                               now.TimeOfDay <= scheduledTime.Add(TimeSpan.FromMinutes(1));
                    
                    case "Monthly":
                        return now.Date >= lastBackup.Date.AddMonths(1) && 
                               now.TimeOfDay >= scheduledTime && 
                               now.TimeOfDay <= scheduledTime.Add(TimeSpan.FromMinutes(1));
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        #region Designer Variables
        private System.ComponentModel.IContainer components = null;
        private Panel panel1;
        private Panel panel2;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private GroupBox groupBox3;
        private Label lblBackupPath;
        private TextBox txtBackupPath;
        private Button btnBrowse;
        private Button btnBackupNow;
        private Button btnRestore;
        private Button btnScheduleBackup;
        private Button btnViewBackups;
        private CheckBox chkAutoBackup;
        private ComboBox cmbBackupFrequency;
        private DateTimePicker dtpBackupTime;
        private Label lblStatus;
        private ProgressBar progressBar;
        private DataGridView dgvBackupHistory;
        private Label lblLastBackup;
        private Label lblBackupSize;
        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                autoBackupTimer?.Dispose();
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}
