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
    public partial class SupplierManagementForm : Form
    {
        private DataTable suppliersData;
        private int selectedSupplierID = 0;
        private bool isEditMode = false;

        // Form Controls
        private Panel pnlTop;
        private Panel pnlMain;
        private Panel pnlButtons;
        private GroupBox gbSupplierInfo;
        private GroupBox gbContactInfo;
        private GroupBox gbFinancialInfo;
        private GroupBox gbSupplierList;
        
        // Supplier Info Controls
        private TextBox txtCompanyName;
        private TextBox txtContactPerson;
        private ComboBox cmbSupplierType;
        private CheckBox chkIsActive;
        
        // Contact Info Controls
        private TextBox txtPhone;
        private TextBox txtEmail;
        private TextBox txtAddress;
        private TextBox txtCity;
        private TextBox txtState;
        private TextBox txtPostalCode; // TODO: Implement postal code functionality
        private TextBox txtCountry; // TODO: Implement country functionality
        
        // Financial Info Controls
        private TextBox txtOpeningBalance;
        private TextBox txtCurrentBalance;
        private TextBox txtCreditLimit;
        private TextBox txtPaymentTerms;
        private Label lblBalanceStatus;
        
        // List and Action Controls
        private DataGridView dgvSuppliers;
        private TextBox txtSearch;
        private Button btnSearch;
        private Button btnRefresh;
        private Button btnNew;
        private Button btnEdit;
        private Button btnSave;
        private Button btnCancel;
        private Button btnDelete;
        private Button btnViewLedger;
        private Button btnMakePayment;
        private Button btnClose;

        public SupplierManagementForm()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("SupplierManagementForm: Starting initialization");
                
                InitializeComponent();
                System.Diagnostics.Debug.WriteLine("SupplierManagementForm: Basic components initialized");
                
                InitializeCustomComponents();
                System.Diagnostics.Debug.WriteLine("SupplierManagementForm: Custom components initialized");
                
                LoadSuppliers();
                System.Diagnostics.Debug.WriteLine("SupplierManagementForm: Suppliers loaded");
                
                SetFormMode(false);
                System.Diagnostics.Debug.WriteLine("SupplierManagementForm: Form mode set, initialization complete");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SupplierManagementForm: Error during initialization: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"SupplierManagementForm: Stack trace: {ex.StackTrace}");
                
                MessageBox.Show($"❌ Error initializing Supplier Management form:\n\n{ex.Message}\n\nPlease check the debug output for details.", 
                    "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    
                // Try to show a minimal form at least
                try
                {
                    InitializeComponent();
                }
                catch (Exception ex2)
                {
                    System.Diagnostics.Debug.WriteLine($"SupplierManagementForm: Critical error in basic initialization: {ex2.Message}");
                }
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // Form
            this.Text = "Supplier Management";
            this.Size = new Size(1200, 700); // Reduced height to ensure buttons are visible
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            
            // Panels
            this.pnlTop = new Panel();
            this.pnlMain = new Panel();
            this.pnlButtons = new Panel();
            
            this.pnlTop.Dock = DockStyle.Top;
            this.pnlTop.Height = 60;
            this.pnlTop.BackColor = Color.FromArgb(0, 122, 204);
            
            this.pnlMain.Dock = DockStyle.Fill;
            this.pnlMain.Padding = new Padding(10);
            
            this.pnlButtons.Dock = DockStyle.Bottom;
            this.pnlButtons.Height = 70; // Increased height
            this.pnlButtons.BackColor = Color.FromArgb(220, 220, 220); // More distinct color
            this.pnlButtons.Visible = true;
            this.pnlButtons.Enabled = true;
            this.pnlButtons.BorderStyle = BorderStyle.FixedSingle; // Add border for visibility
            
            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.pnlTop);
            this.Controls.Add(this.pnlButtons); // Add buttons panel last to ensure it's on top
            
            // Title
            Label lblTitle = new Label();
            lblTitle.Text = "🏢 Supplier Management";
            lblTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(20, 15);
            lblTitle.AutoSize = true;
            this.pnlTop.Controls.Add(lblTitle);
            
            this.ResumeLayout(false);
        }

        private void InitializeCustomComponents()
        {
            // Group Boxes
            this.gbSupplierInfo = new GroupBox();
            this.gbContactInfo = new GroupBox();
            this.gbFinancialInfo = new GroupBox();
            this.gbSupplierList = new GroupBox();
            
            this.gbSupplierInfo.Text = "📋 Supplier Information";
            this.gbContactInfo.Text = "📞 Contact Information";
            this.gbFinancialInfo.Text = "💰 Financial Information";
            this.gbSupplierList.Text = "📊 Suppliers List";
            
            this.gbSupplierInfo.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            this.gbContactInfo.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            this.gbFinancialInfo.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            this.gbSupplierList.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            
            // Layout
            this.gbSupplierInfo.Location = new Point(10, 10);
            this.gbSupplierInfo.Size = new Size(350, 200);
            
            this.gbContactInfo.Location = new Point(370, 10);
            this.gbContactInfo.Size = new Size(350, 200);
            
            this.gbFinancialInfo.Location = new Point(730, 10);
            this.gbFinancialInfo.Size = new Size(350, 200);
            
            this.gbSupplierList.Location = new Point(10, 220);
            this.gbSupplierList.Size = new Size(1070, 350); // Reduced height to make room for buttons
            
            this.pnlMain.Controls.Add(this.gbSupplierInfo);
            this.pnlMain.Controls.Add(this.gbContactInfo);
            this.pnlMain.Controls.Add(this.gbFinancialInfo);
            this.pnlMain.Controls.Add(this.gbSupplierList);
            
            // Initialize Controls
            InitializeSupplierInfoControls();
            InitializeContactInfoControls();
            InitializeFinancialInfoControls();
            InitializeListControls();
            InitializeActionButtons();
            
            // Debug: Check if buttons panel is properly configured
            System.Diagnostics.Debug.WriteLine($"InitializeCustomComponents: pnlButtons created - Visible: {pnlButtons.Visible}, Height: {pnlButtons.Height}, Controls: {pnlButtons.Controls.Count}");
        }

        private void InitializeSupplierInfoControls()
        {
            // Company Name
            Label lblCompanyName = new Label();
            lblCompanyName.Text = "Company Name:";
            lblCompanyName.Location = new Point(15, 30);
            lblCompanyName.Size = new Size(100, 20);
            
            this.txtCompanyName = new TextBox();
            this.txtCompanyName.Location = new Point(15, 50);
            this.txtCompanyName.Size = new Size(320, 25);
            
            // Contact Person
            Label lblContactPerson = new Label();
            lblContactPerson.Text = "Contact Person:";
            lblContactPerson.Location = new Point(15, 80);
            lblContactPerson.Size = new Size(100, 20);
            
            this.txtContactPerson = new TextBox();
            this.txtContactPerson.Location = new Point(15, 100);
            this.txtContactPerson.Size = new Size(320, 25);
            
            // Supplier Type
            Label lblSupplierType = new Label();
            lblSupplierType.Text = "Supplier Type:";
            lblSupplierType.Location = new Point(15, 130);
            lblSupplierType.Size = new Size(100, 20);
            
            this.cmbSupplierType = new ComboBox();
            this.cmbSupplierType.Location = new Point(15, 150);
            this.cmbSupplierType.Size = new Size(150, 25);
            this.cmbSupplierType.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbSupplierType.Items.AddRange(new string[] { "Manufacturer", "Distributor", "Wholesaler", "Local Supplier" });
            this.cmbSupplierType.SelectedIndex = 0;
            
            // Is Active
            this.chkIsActive = new CheckBox();
            this.chkIsActive.Text = "Active";
            this.chkIsActive.Location = new Point(200, 150);
            this.chkIsActive.Size = new Size(80, 25);
            this.chkIsActive.Checked = true;
            
            this.gbSupplierInfo.Controls.Add(lblCompanyName);
            this.gbSupplierInfo.Controls.Add(this.txtCompanyName);
            this.gbSupplierInfo.Controls.Add(lblContactPerson);
            this.gbSupplierInfo.Controls.Add(this.txtContactPerson);
            this.gbSupplierInfo.Controls.Add(lblSupplierType);
            this.gbSupplierInfo.Controls.Add(this.cmbSupplierType);
            this.gbSupplierInfo.Controls.Add(this.chkIsActive);
        }

        private void InitializeContactInfoControls()
        {
            // Phone
            Label lblPhone = new Label();
            lblPhone.Text = "Phone:";
            lblPhone.Location = new Point(15, 30);
            lblPhone.Size = new Size(60, 20);
            
            this.txtPhone = new TextBox();
            this.txtPhone.Location = new Point(15, 50);
            this.txtPhone.Size = new Size(150, 25);
            
            // Email
            Label lblEmail = new Label();
            lblEmail.Text = "Email:";
            lblEmail.Location = new Point(180, 30);
            lblEmail.Size = new Size(60, 20);
            
            this.txtEmail = new TextBox();
            this.txtEmail.Location = new Point(180, 50);
            this.txtEmail.Size = new Size(150, 25);
            
            // Address
            Label lblAddress = new Label();
            lblAddress.Text = "Address:";
            lblAddress.Location = new Point(15, 80);
            lblAddress.Size = new Size(60, 20);
            
            this.txtAddress = new TextBox();
            this.txtAddress.Location = new Point(15, 100);
            this.txtAddress.Size = new Size(320, 25);
            this.txtAddress.Multiline = true;
            this.txtAddress.Height = 40;
            
            // City and State
            Label lblCity = new Label();
            lblCity.Text = "City:";
            lblCity.Location = new Point(15, 150);
            lblCity.Size = new Size(40, 20);
            
            this.txtCity = new TextBox();
            this.txtCity.Location = new Point(60, 150);
            this.txtCity.Size = new Size(100, 25);
            
            Label lblState = new Label();
            lblState.Text = "State:";
            lblState.Location = new Point(170, 150);
            lblState.Size = new Size(40, 20);
            
            this.txtState = new TextBox();
            this.txtState.Location = new Point(215, 150);
            this.txtState.Size = new Size(120, 25);
            
            this.gbContactInfo.Controls.Add(lblPhone);
            this.gbContactInfo.Controls.Add(this.txtPhone);
            this.gbContactInfo.Controls.Add(lblEmail);
            this.gbContactInfo.Controls.Add(this.txtEmail);
            this.gbContactInfo.Controls.Add(lblAddress);
            this.gbContactInfo.Controls.Add(this.txtAddress);
            this.gbContactInfo.Controls.Add(lblCity);
            this.gbContactInfo.Controls.Add(this.txtCity);
            this.gbContactInfo.Controls.Add(lblState);
            this.gbContactInfo.Controls.Add(this.txtState);
        }

        private void InitializeFinancialInfoControls()
        {
            // Opening Balance
            Label lblOpeningBalance = new Label();
            lblOpeningBalance.Text = "Opening Balance:";
            lblOpeningBalance.Location = new Point(15, 30);
            lblOpeningBalance.Size = new Size(100, 20);
            
            this.txtOpeningBalance = new TextBox();
            this.txtOpeningBalance.Location = new Point(15, 50);
            this.txtOpeningBalance.Size = new Size(120, 25);
            this.txtOpeningBalance.Text = "0.00";
            this.txtOpeningBalance.TextAlign = HorizontalAlignment.Right;
            
            // Current Balance
            Label lblCurrentBalance = new Label();
            lblCurrentBalance.Text = "Current Balance:";
            lblCurrentBalance.Location = new Point(150, 30);
            lblCurrentBalance.Size = new Size(100, 20);
            
            this.txtCurrentBalance = new TextBox();
            this.txtCurrentBalance.Location = new Point(150, 50);
            this.txtCurrentBalance.Size = new Size(120, 25);
            this.txtCurrentBalance.Text = "0.00";
            this.txtCurrentBalance.TextAlign = HorizontalAlignment.Right;
            this.txtCurrentBalance.ReadOnly = true;
            this.txtCurrentBalance.BackColor = Color.LightGray;
            
            // Credit Limit
            Label lblCreditLimit = new Label();
            lblCreditLimit.Text = "Credit Limit:";
            lblCreditLimit.Location = new Point(15, 80);
            lblCreditLimit.Size = new Size(100, 20);
            
            this.txtCreditLimit = new TextBox();
            this.txtCreditLimit.Location = new Point(15, 100);
            this.txtCreditLimit.Size = new Size(120, 25);
            this.txtCreditLimit.Text = "0.00";
            this.txtCreditLimit.TextAlign = HorizontalAlignment.Right;
            
            // Payment Terms
            Label lblPaymentTerms = new Label();
            lblPaymentTerms.Text = "Payment Terms (Days):";
            lblPaymentTerms.Location = new Point(150, 80);
            lblPaymentTerms.Size = new Size(130, 20);
            
            this.txtPaymentTerms = new TextBox();
            this.txtPaymentTerms.Location = new Point(150, 100);
            this.txtPaymentTerms.Size = new Size(120, 25);
            this.txtPaymentTerms.Text = "30";
            this.txtPaymentTerms.TextAlign = HorizontalAlignment.Right;
            
            // Balance Status
            this.lblBalanceStatus = new Label();
            this.lblBalanceStatus.Text = "Balance Status: Current";
            this.lblBalanceStatus.Location = new Point(15, 135);
            this.lblBalanceStatus.Size = new Size(300, 40);
            this.lblBalanceStatus.ForeColor = Color.Green;
            this.lblBalanceStatus.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            
            this.gbFinancialInfo.Controls.Add(lblOpeningBalance);
            this.gbFinancialInfo.Controls.Add(this.txtOpeningBalance);
            this.gbFinancialInfo.Controls.Add(lblCurrentBalance);
            this.gbFinancialInfo.Controls.Add(this.txtCurrentBalance);
            this.gbFinancialInfo.Controls.Add(lblCreditLimit);
            this.gbFinancialInfo.Controls.Add(this.txtCreditLimit);
            this.gbFinancialInfo.Controls.Add(lblPaymentTerms);
            this.gbFinancialInfo.Controls.Add(this.txtPaymentTerms);
            this.gbFinancialInfo.Controls.Add(this.lblBalanceStatus);
        }

        private void InitializeListControls()
        {
            // Search Controls
            Label lblSearch = new Label();
            lblSearch.Text = "Search Suppliers:";
            lblSearch.Location = new Point(15, 25);
            lblSearch.Size = new Size(100, 20);
            
            this.txtSearch = new TextBox();
            this.txtSearch.Location = new Point(120, 25);
            this.txtSearch.Size = new Size(200, 25);
            this.txtSearch.TextChanged += TxtSearch_TextChanged;
            
            this.btnSearch = new Button();
            this.btnSearch.Text = "🔍 Search";
            this.btnSearch.Location = new Point(330, 25);
            this.btnSearch.Size = new Size(80, 28);
            this.btnSearch.Click += BtnSearch_Click;
            
            this.btnRefresh = new Button();
            this.btnRefresh.Text = "🔄 Refresh";
            this.btnRefresh.Location = new Point(420, 25);
            this.btnRefresh.Size = new Size(80, 28);
            this.btnRefresh.Click += BtnRefresh_Click;
            
            // DataGridView
            this.dgvSuppliers = new DataGridView();
            this.dgvSuppliers.Location = new Point(15, 60);
            this.dgvSuppliers.Size = new Size(1040, 270); // Reduced height to match gbSupplierList
            this.dgvSuppliers.AllowUserToAddRows = false;
            this.dgvSuppliers.AllowUserToDeleteRows = false;
            this.dgvSuppliers.ReadOnly = true;
            this.dgvSuppliers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvSuppliers.MultiSelect = false;
            this.dgvSuppliers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvSuppliers.SelectionChanged += DgvSuppliers_SelectionChanged;
            
            this.gbSupplierList.Controls.Add(lblSearch);
            this.gbSupplierList.Controls.Add(this.txtSearch);
            this.gbSupplierList.Controls.Add(this.btnSearch);
            this.gbSupplierList.Controls.Add(this.btnRefresh);
            this.gbSupplierList.Controls.Add(this.dgvSuppliers);
        }

        private void InitializeActionButtons()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("InitializeActionButtons: Creating action buttons");
                
                int buttonWidth = 100;
                int buttonHeight = 35;
                int buttonSpacing = 110;
                int startX = 20;
                int startY = 17; // Increased top margin
            
            this.btnNew = new Button();
            this.btnNew.Text = "➕ New";
            this.btnNew.Location = new Point(startX, startY);
            this.btnNew.Size = new Size(buttonWidth, buttonHeight);
            this.btnNew.BackColor = Color.FromArgb(40, 167, 69);
            this.btnNew.ForeColor = Color.White;
            this.btnNew.FlatStyle = FlatStyle.Flat;
            this.btnNew.Click += BtnNew_Click;
            
            this.btnEdit = new Button();
            this.btnEdit.Text = "✏️ Edit";
            this.btnEdit.Location = new Point(startX + buttonSpacing, startY);
            this.btnEdit.Size = new Size(buttonWidth, buttonHeight);
            this.btnEdit.BackColor = Color.FromArgb(255, 193, 7);
            this.btnEdit.ForeColor = Color.Black;
            this.btnEdit.FlatStyle = FlatStyle.Flat;
            this.btnEdit.Click += BtnEdit_Click;
            
            this.btnSave = new Button();
            this.btnSave.Text = "💾 Save";
            this.btnSave.Location = new Point(startX + buttonSpacing * 2, startY);
            this.btnSave.Size = new Size(buttonWidth, buttonHeight);
            this.btnSave.BackColor = Color.FromArgb(0, 123, 255);
            this.btnSave.ForeColor = Color.White;
            this.btnSave.FlatStyle = FlatStyle.Flat;
            this.btnSave.Click += BtnSave_Click;
            
            this.btnCancel = new Button();
            this.btnCancel.Text = "❌ Cancel";
            this.btnCancel.Location = new Point(startX + buttonSpacing * 3, startY);
            this.btnCancel.Size = new Size(buttonWidth, buttonHeight);
            this.btnCancel.BackColor = Color.FromArgb(108, 117, 125);
            this.btnCancel.ForeColor = Color.White;
            this.btnCancel.FlatStyle = FlatStyle.Flat;
            this.btnCancel.Click += BtnCancel_Click;
            
            this.btnDelete = new Button();
            this.btnDelete.Text = "🗑️ Delete";
            this.btnDelete.Location = new Point(startX + buttonSpacing * 4, startY);
            this.btnDelete.Size = new Size(buttonWidth, buttonHeight);
            this.btnDelete.BackColor = Color.FromArgb(220, 53, 69);
            this.btnDelete.ForeColor = Color.White;
            this.btnDelete.FlatStyle = FlatStyle.Flat;
            this.btnDelete.Click += BtnDelete_Click;
            
            this.btnViewLedger = new Button();
            this.btnViewLedger.Text = "📊 Ledger";
            this.btnViewLedger.Location = new Point(startX + buttonSpacing * 5, startY);
            this.btnViewLedger.Size = new Size(buttonWidth, buttonHeight);
            this.btnViewLedger.BackColor = Color.FromArgb(23, 162, 184);
            this.btnViewLedger.ForeColor = Color.White;
            this.btnViewLedger.FlatStyle = FlatStyle.Flat;
            this.btnViewLedger.Click += BtnViewLedger_Click;
            
            this.btnMakePayment = new Button();
            this.btnMakePayment.Text = "💳 Payment";
            this.btnMakePayment.Location = new Point(startX + buttonSpacing * 6, startY);
            this.btnMakePayment.Size = new Size(buttonWidth, buttonHeight);
            this.btnMakePayment.BackColor = Color.FromArgb(111, 66, 193);
            this.btnMakePayment.ForeColor = Color.White;
            this.btnMakePayment.FlatStyle = FlatStyle.Flat;
            this.btnMakePayment.Click += BtnMakePayment_Click;
            
            this.btnClose = new Button();
            this.btnClose.Text = "🚪 Close";
            this.btnClose.Location = new Point(startX + buttonSpacing * 7, startY);
            this.btnClose.Size = new Size(buttonWidth, buttonHeight);
            this.btnClose.BackColor = Color.FromArgb(52, 58, 64);
            this.btnClose.ForeColor = Color.White;
            this.btnClose.FlatStyle = FlatStyle.Flat;
            this.btnClose.Click += BtnClose_Click;
            
                this.pnlButtons.Controls.Add(this.btnNew);
                this.pnlButtons.Controls.Add(this.btnEdit);
                this.pnlButtons.Controls.Add(this.btnSave);
                this.pnlButtons.Controls.Add(this.btnCancel);
                this.pnlButtons.Controls.Add(this.btnDelete);
                this.pnlButtons.Controls.Add(this.btnViewLedger);
                this.pnlButtons.Controls.Add(this.btnMakePayment);
                this.pnlButtons.Controls.Add(this.btnClose);
                
                System.Diagnostics.Debug.WriteLine($"InitializeActionButtons: Successfully created and added {this.pnlButtons.Controls.Count} buttons to pnlButtons");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"InitializeActionButtons: Error creating buttons: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"InitializeActionButtons: Stack trace: {ex.StackTrace}");
                throw; // Re-throw to be caught by constructor
            }
        }

        private void LoadSuppliers()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("LoadSuppliers: Starting to load suppliers from database");
                string query = @"
                    SELECT 
                        c.CompanyID,
                        c.CompanyName,
                        ISNULL(c.ContactPerson, '') as ContactPerson,
                        ISNULL(c.Phone, '') as Phone,
                        ISNULL(c.Email, '') as Email,
                        ISNULL(c.Address, '') as Address,
                        '' as City,
                        '' as State,
                        0 as OpeningBalance,
                        0 as CurrentBalance,
                        0 as CreditLimit,
                        30 as PaymentTerms,
                        c.IsActive,
                        COUNT(p.PurchaseID) as TotalPurchases,
                        ISNULL(SUM(p.TotalAmount), 0) as TotalPurchaseAmount
                    FROM Companies c
                    LEFT JOIN Purchases p ON c.CompanyID = p.CompanyID AND p.IsActive = 1
                    GROUP BY c.CompanyID, c.CompanyName, c.ContactPerson, c.Phone, c.Email, 
                             c.Address, c.IsActive
                    ORDER BY c.CompanyName";
                
                this.suppliersData = DatabaseConnection.ExecuteQuery(query);
                
                System.Diagnostics.Debug.WriteLine($"Loaded {this.suppliersData.Rows.Count} suppliers from database");
                
                // Set data source
                this.dgvSuppliers.DataSource = this.suppliersData;
                
                // Allow DataGridView to fully initialize
                Application.DoEvents();
                System.Threading.Thread.Sleep(100); // Small delay to ensure full initialization
                
                // Format columns after DataGridView is ready
                FormatDataGridView();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading suppliers: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormatDataGridView()
        {
            try
            {
                // Add delay to ensure DataGridView is fully initialized
                Application.DoEvents();
                
                if (dgvSuppliers == null || dgvSuppliers.Columns == null || dgvSuppliers.Columns.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("DataGridView or columns not ready for formatting");
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"Formatting DataGridView with {dgvSuppliers.Columns.Count} columns");
                
                // Hide columns with enhanced null checks
                SafeSetColumnProperty("CompanyID", col => col.Visible = false);
                
                // Set header texts with enhanced null checks
                SafeSetColumnProperty("CompanyName", col => col.HeaderText = "Company Name");
                SafeSetColumnProperty("ContactPerson", col => col.HeaderText = "Contact Person");
                SafeSetColumnProperty("Phone", col => col.HeaderText = "Phone");
                SafeSetColumnProperty("Email", col => col.HeaderText = "Email");
                SafeSetColumnProperty("Address", col => col.HeaderText = "Address");
                SafeSetColumnProperty("City", col => col.HeaderText = "City");
                SafeSetColumnProperty("State", col => col.HeaderText = "State");
                SafeSetColumnProperty("OpeningBalance", col => col.HeaderText = "Opening Balance");
                SafeSetColumnProperty("CurrentBalance", col => col.HeaderText = "Current Balance");
                SafeSetColumnProperty("CreditLimit", col => col.HeaderText = "Credit Limit");
                SafeSetColumnProperty("PaymentTerms", col => col.HeaderText = "Payment Terms");
                SafeSetColumnProperty("IsActive", col => col.HeaderText = "Active");
                SafeSetColumnProperty("TotalPurchases", col => col.HeaderText = "Total Orders");
                SafeSetColumnProperty("TotalPurchaseAmount", col => col.HeaderText = "Total Purchase Amount");
                
                // Format currency columns
                SafeSetColumnProperty("OpeningBalance", col => col.DefaultCellStyle.Format = "N2");
                SafeSetColumnProperty("CurrentBalance", col => col.DefaultCellStyle.Format = "N2");
                SafeSetColumnProperty("CreditLimit", col => col.DefaultCellStyle.Format = "N2");
                SafeSetColumnProperty("TotalPurchaseAmount", col => col.DefaultCellStyle.Format = "N2");
                
                // Set widths with enhanced null checks
                SafeSetColumnProperty("CompanyName", col => col.Width = 200);
                SafeSetColumnProperty("ContactPerson", col => col.Width = 150);
                SafeSetColumnProperty("Phone", col => col.Width = 120);
                SafeSetColumnProperty("Email", col => col.Width = 180);
                
                System.Diagnostics.Debug.WriteLine("DataGridView formatting completed successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in FormatDataGridView: {ex.Message}");
                MessageBox.Show($"Error formatting data grid: {ex.Message}", "Formatting Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void SafeSetColumnProperty(string columnName, Action<DataGridViewColumn> setProperty)
        {
            try
            {
                if (dgvSuppliers?.Columns != null && dgvSuppliers.Columns.Contains(columnName))
                {
                    var column = dgvSuppliers.Columns[columnName];
                    if (column != null)
                    {
                        setProperty(column);
                        System.Diagnostics.Debug.WriteLine($"Successfully set property for column: {columnName}");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"Column {columnName} is null");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Column {columnName} does not exist");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting property for column {columnName}: {ex.Message}");
            }
        }

        private void SetFormMode(bool editMode)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"SetFormMode: Setting edit mode to {editMode}");
                
                this.isEditMode = editMode;
                
                // Enable/disable input controls with null checks
                if (txtCompanyName != null) txtCompanyName.Enabled = editMode;
                if (txtContactPerson != null) txtContactPerson.Enabled = editMode;
                if (cmbSupplierType != null) cmbSupplierType.Enabled = editMode;
                if (chkIsActive != null) chkIsActive.Enabled = editMode;
                if (txtPhone != null) txtPhone.Enabled = editMode;
                if (txtEmail != null) txtEmail.Enabled = editMode;
                if (txtAddress != null) txtAddress.Enabled = editMode;
                if (txtCity != null) txtCity.Enabled = editMode;
                if (txtState != null) txtState.Enabled = editMode;
                if (txtOpeningBalance != null) txtOpeningBalance.Enabled = editMode;
                if (txtCreditLimit != null) txtCreditLimit.Enabled = editMode;
                if (txtPaymentTerms != null) txtPaymentTerms.Enabled = editMode;
                
                // Button states with null checks
                if (btnNew != null) btnNew.Enabled = !editMode;
                if (btnEdit != null) btnEdit.Enabled = !editMode && selectedSupplierID > 0;
                if (btnSave != null) btnSave.Enabled = editMode;
                if (btnCancel != null) btnCancel.Enabled = editMode;
                if (btnDelete != null) btnDelete.Enabled = !editMode && selectedSupplierID > 0;
                if (btnViewLedger != null) btnViewLedger.Enabled = !editMode && selectedSupplierID > 0;
                if (btnMakePayment != null) btnMakePayment.Enabled = !editMode && selectedSupplierID > 0;
                
                if (dgvSuppliers != null) dgvSuppliers.Enabled = !editMode;
                
                System.Diagnostics.Debug.WriteLine($"SetFormMode: Successfully set mode to {editMode}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SetFormMode: Error setting form mode: {ex.Message}");
            }
        }

        private void ClearForm()
        {
            txtCompanyName.Clear();
            txtContactPerson.Clear();
            cmbSupplierType.SelectedIndex = 0;
            chkIsActive.Checked = true;
            txtPhone.Clear();
            txtEmail.Clear();
            txtAddress.Clear();
            txtCity.Clear();
            txtState.Clear();
            txtOpeningBalance.Text = "0.00";
            txtCurrentBalance.Text = "0.00";
            txtCreditLimit.Text = "0.00";
            txtPaymentTerms.Text = "30";
            lblBalanceStatus.Text = "Balance Status: New";
            lblBalanceStatus.ForeColor = Color.Blue;
            selectedSupplierID = 0;
        }

        private void LoadSelectedSupplier()
        {
            if (dgvSuppliers.CurrentRow != null)
            {
                DataRowView row = (DataRowView)dgvSuppliers.CurrentRow.DataBoundItem;
                
                selectedSupplierID = Convert.ToInt32(row["CompanyID"]);
                txtCompanyName.Text = row["CompanyName"].ToString();
                txtContactPerson.Text = row["ContactPerson"].ToString();
                txtPhone.Text = row["Phone"].ToString();
                txtEmail.Text = row["Email"].ToString();
                txtAddress.Text = row["Address"].ToString();
                txtCity.Text = row["City"].ToString();
                txtState.Text = row["State"].ToString();
                txtOpeningBalance.Text = Convert.ToDecimal(row["OpeningBalance"]).ToString("N2");
                txtCurrentBalance.Text = Convert.ToDecimal(row["CurrentBalance"]).ToString("N2");
                txtCreditLimit.Text = Convert.ToDecimal(row["CreditLimit"]).ToString("N2");
                txtPaymentTerms.Text = row["PaymentTerms"].ToString();
                chkIsActive.Checked = Convert.ToBoolean(row["IsActive"]);
                
                UpdateBalanceStatus();
            }
        }

        private void UpdateBalanceStatus()
        {
            decimal currentBalance = Convert.ToDecimal(txtCurrentBalance.Text);
            decimal creditLimit = Convert.ToDecimal(txtCreditLimit.Text);
            
            if (currentBalance == 0)
            {
                lblBalanceStatus.Text = "Balance Status: ✅ Cleared";
                lblBalanceStatus.ForeColor = Color.Green;
            }
            else if (currentBalance > 0 && currentBalance <= creditLimit)
            {
                lblBalanceStatus.Text = $"Balance Status: ⚠️ Outstanding: {currentBalance:N2}";
                lblBalanceStatus.ForeColor = Color.Orange;
            }
            else if (currentBalance > creditLimit)
            {
                lblBalanceStatus.Text = $"Balance Status: ❌ Overdue: {currentBalance:N2}";
                lblBalanceStatus.ForeColor = Color.Red;
            }
            else
            {
                lblBalanceStatus.Text = $"Balance Status: 💰 Advance: {Math.Abs(currentBalance):N2}";
                lblBalanceStatus.ForeColor = Color.Blue;
            }
        }

        private bool ValidateForm()
        {
            // Required field validation
            if (string.IsNullOrWhiteSpace(txtCompanyName.Text))
            {
                MessageBox.Show("⚠️ Please enter company name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCompanyName.Focus();
                return false;
            }
            
            if (txtCompanyName.Text.Trim().Length < 2)
            {
                MessageBox.Show("⚠️ Company name must be at least 2 characters long.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCompanyName.Focus();
                return false;
            }
            
            // Optional email validation
            if (!string.IsNullOrWhiteSpace(txtEmail.Text) && !IsValidEmail(txtEmail.Text))
            {
                MessageBox.Show("⚠️ Please enter a valid email address.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return false;
            }
            
            // Optional phone validation
            if (!string.IsNullOrWhiteSpace(txtPhone.Text) && txtPhone.Text.Trim().Length < 10)
            {
                MessageBox.Show("⚠️ Phone number must be at least 10 digits.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPhone.Focus();
                return false;
            }
            
            return true;
        }
        
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool SaveSupplier()
        {
            if (!ValidateForm()) return false;
            
            try
            {
                string query;
                List<SqlParameter> parameters = new List<SqlParameter>();
                
                if (selectedSupplierID == 0) // New supplier
                {
                    query = @"
                        INSERT INTO Companies (CompanyName, ContactPerson, Phone, Email, Address, IsActive, CreatedDate)
                        VALUES (@CompanyName, @ContactPerson, @Phone, @Email, @Address, @IsActive, GETDATE());
                        SELECT SCOPE_IDENTITY();";
                }
                else // Update existing
                {
                    query = @"
                        UPDATE Companies SET 
                            CompanyName = @CompanyName,
                            ContactPerson = @ContactPerson,
                            Phone = @Phone,
                            Email = @Email,
                            Address = @Address,
                            IsActive = @IsActive
                        WHERE CompanyID = @CompanyID";
                    
                    parameters.Add(new SqlParameter("@CompanyID", selectedSupplierID));
                }
                
                parameters.AddRange(new SqlParameter[]
                {
                    new SqlParameter("@CompanyName", txtCompanyName.Text.Trim()),
                    new SqlParameter("@ContactPerson", string.IsNullOrWhiteSpace(txtContactPerson.Text) ? (object)DBNull.Value : txtContactPerson.Text.Trim()),
                    new SqlParameter("@Phone", string.IsNullOrWhiteSpace(txtPhone.Text) ? (object)DBNull.Value : txtPhone.Text.Trim()),
                    new SqlParameter("@Email", string.IsNullOrWhiteSpace(txtEmail.Text) ? (object)DBNull.Value : txtEmail.Text.Trim()),
                    new SqlParameter("@Address", string.IsNullOrWhiteSpace(txtAddress.Text) ? (object)DBNull.Value : txtAddress.Text.Trim()),
                    new SqlParameter("@IsActive", chkIsActive.Checked)
                });
                
                if (selectedSupplierID == 0)
                {
                    object result = DatabaseConnection.ExecuteScalar(query, parameters.ToArray());
                    selectedSupplierID = Convert.ToInt32(result);
                    MessageBox.Show($"✅ Supplier '{txtCompanyName.Text}' created successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    System.Diagnostics.Debug.WriteLine($"New supplier created with ID: {selectedSupplierID}");
                }
                else
                {
                    DatabaseConnection.ExecuteNonQuery(query, parameters.ToArray());
                    MessageBox.Show($"✅ Supplier '{txtCompanyName.Text}' updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    System.Diagnostics.Debug.WriteLine($"Supplier ID {selectedSupplierID} updated successfully");
                }
                
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Error saving supplier: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"Error saving supplier: {ex.Message}");
                return false;
            }
        }

        // Event Handlers
        private void DgvSuppliers_SelectionChanged(object sender, EventArgs e)
        {
            if (!isEditMode)
            {
                LoadSelectedSupplier();
                SetFormMode(false);
            }
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            if (suppliersData != null)
            {
                string searchText = txtSearch.Text.Trim();
                if (string.IsNullOrEmpty(searchText))
                {
                    (dgvSuppliers.DataSource as DataTable).DefaultView.RowFilter = "";
                }
                else
                {
                    (dgvSuppliers.DataSource as DataTable).DefaultView.RowFilter = 
                        $"CompanyName LIKE '%{searchText}%' OR ContactPerson LIKE '%{searchText}%' OR Phone LIKE '%{searchText}%'";
                }
            }
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            TxtSearch_TextChanged(sender, e);
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadSuppliers();
            txtSearch.Clear();
        }

        private void BtnNew_Click(object sender, EventArgs e)
        {
            ClearForm();
            SetFormMode(true);
            txtCompanyName.Focus();
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (selectedSupplierID > 0)
            {
                SetFormMode(true);
                txtCompanyName.Focus();
            }
            else
            {
                MessageBox.Show("Please select a supplier to edit.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (SaveSupplier())
            {
                SetFormMode(false);
                LoadSuppliers();
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            SetFormMode(false);
            if (selectedSupplierID > 0)
            {
                LoadSelectedSupplier();
            }
            else
            {
                ClearForm();
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (selectedSupplierID > 0)
            {
                string supplierName = txtCompanyName.Text;
                DialogResult result = MessageBox.Show(
                    $"Are you sure you want to deactivate supplier '{supplierName}'?\n\n" +
                    "⚠️ This will remove the supplier from the active list but preserve all transaction history.", 
                    "Confirm Deactivate Supplier", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Question);
                
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        string query = "UPDATE Companies SET IsActive = 0 WHERE CompanyID = @CompanyID";
                        SqlParameter[] parameters = { new SqlParameter("@CompanyID", selectedSupplierID) };
                        
                        DatabaseConnection.ExecuteNonQuery(query, parameters);
                        MessageBox.Show($"✅ Supplier '{supplierName}' has been deactivated successfully!\n\n" +
                                      "The supplier has been removed from the active list but all transaction history is preserved.", 
                                      "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        
                        System.Diagnostics.Debug.WriteLine($"Supplier ID {selectedSupplierID} ({supplierName}) deactivated");
                        
                        LoadSuppliers(); // Refresh the list to remove deactivated supplier
                        ClearForm();
                        SetFormMode(false);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"❌ Error deactivating supplier: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        System.Diagnostics.Debug.WriteLine($"Error deactivating supplier: {ex.Message}");
                    }
                }
            }
            else
            {
                MessageBox.Show("⚠️ Please select a supplier to deactivate.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnViewLedger_Click(object sender, EventArgs e)
        {
            if (selectedSupplierID > 0)
            {
                // Open supplier ledger form
                SupplierLedger ledgerForm = new SupplierLedger();
                ledgerForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please select a supplier to view ledger.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnMakePayment_Click(object sender, EventArgs e)
        {
            if (selectedSupplierID > 0)
            {
                MessageBox.Show("Supplier Payment form will be implemented.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // TODO: Open supplier payment form
            }
            else
            {
                MessageBox.Show("Please select a supplier to make payment.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
