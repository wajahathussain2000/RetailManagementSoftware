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
using System.Media;

namespace RetailManagement.UserForms
{
    public partial class EnhancedPOSForm : Form
    {
        private DataTable billItems;
        private int selectedCustomerID = 0;
        private int currentUserID = 1;
        private Timer barcodeTimer;
        private string scannedBarcode = "";

        public EnhancedPOSForm()
        {
            InitializeComponent();
            InitializeForm();
        }

        public EnhancedPOSForm(int userID) : this()
        {
            currentUserID = userID;
        }

        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.dgvBillItems = new System.Windows.Forms.DataGridView();
            
            // Header Panel Controls
            this.lblTitle = new System.Windows.Forms.Label();
            this.txtBillNumber = new System.Windows.Forms.TextBox();
            this.lblBillNumber = new System.Windows.Forms.Label();
            this.dtpBillDate = new System.Windows.Forms.DateTimePicker();
            this.lblBillDate = new System.Windows.Forms.Label();
            this.cmbCustomer = new System.Windows.Forms.ComboBox();
            this.lblCustomer = new System.Windows.Forms.Label();
            this.btnNewCustomer = new System.Windows.Forms.Button();
            this.lblCustomerBalance = new System.Windows.Forms.Label();
            
            // Barcode Panel
            this.groupBoxBarcode = new System.Windows.Forms.GroupBox();
            this.txtBarcode = new System.Windows.Forms.TextBox();
            this.lblBarcode = new System.Windows.Forms.Label();
            this.btnScanBarcode = new System.Windows.Forms.Button();
            this.lblScanStatus = new System.Windows.Forms.Label();
            
            // Item Selection Panel
            this.groupBoxItemSelection = new System.Windows.Forms.GroupBox();
            this.cmbItem = new System.Windows.Forms.ComboBox();
            this.lblItem = new System.Windows.Forms.Label();
            this.txtItemSearch = new System.Windows.Forms.TextBox();
            this.lblItemSearch = new System.Windows.Forms.Label();
            this.lblItemDetails = new System.Windows.Forms.Label();
            this.lblStockStatus = new System.Windows.Forms.Label();
            this.lblExpiryWarning = new System.Windows.Forms.Label();
            
            // Quantity and Pricing Panel
            this.groupBoxQuantityPrice = new System.Windows.Forms.GroupBox();
            this.txtQuantity = new System.Windows.Forms.TextBox();
            this.lblQuantity = new System.Windows.Forms.Label();
            this.btnMinus = new System.Windows.Forms.Button();
            this.btnPlus = new System.Windows.Forms.Button();
            this.txtUnitPrice = new System.Windows.Forms.TextBox();
            this.lblUnitPrice = new System.Windows.Forms.Label();
            this.txtMRP = new System.Windows.Forms.TextBox();
            this.lblMRP = new System.Windows.Forms.Label();
            this.txtDiscount = new System.Windows.Forms.TextBox();
            this.lblDiscount = new System.Windows.Forms.Label();
            this.txtDiscountPercent = new System.Windows.Forms.TextBox();
            this.lblDiscountPercent = new System.Windows.Forms.Label();
            this.btnAddItem = new System.Windows.Forms.Button();
            this.btnClearItem = new System.Windows.Forms.Button();
            
            // Summary Panel
            this.groupBoxSummary = new System.Windows.Forms.GroupBox();
            this.txtGrossAmount = new System.Windows.Forms.TextBox();
            this.lblGrossAmount = new System.Windows.Forms.Label();
            this.txtTotalDiscount = new System.Windows.Forms.TextBox();
            this.lblTotalDiscount = new System.Windows.Forms.Label();
            this.txtTaxableAmount = new System.Windows.Forms.TextBox();
            this.lblTaxableAmount = new System.Windows.Forms.Label();
            this.txtCGST = new System.Windows.Forms.TextBox();
            this.lblCGST = new System.Windows.Forms.Label();
            this.txtSGST = new System.Windows.Forms.TextBox();
            this.lblSGST = new System.Windows.Forms.Label();
            this.txtIGST = new System.Windows.Forms.TextBox();
            this.lblIGST = new System.Windows.Forms.Label();
            this.txtTotalTax = new System.Windows.Forms.TextBox();
            this.lblTotalTax = new System.Windows.Forms.Label();
            this.txtRoundOff = new System.Windows.Forms.TextBox();
            this.lblRoundOff = new System.Windows.Forms.Label();
            this.txtNetAmount = new System.Windows.Forms.TextBox();
            this.lblNetAmount = new System.Windows.Forms.Label();
            
            // Payment Panel
            this.groupBoxPayment = new System.Windows.Forms.GroupBox();
            this.txtCashAmount = new System.Windows.Forms.TextBox();
            this.lblCashAmount = new System.Windows.Forms.Label();
            this.txtCardAmount = new System.Windows.Forms.TextBox();
            this.lblCardAmount = new System.Windows.Forms.Label();
            this.txtEasyPaisaAmount = new System.Windows.Forms.TextBox();
            this.lblEasyPaisaAmount = new System.Windows.Forms.Label();
            this.txtJazzCashAmount = new System.Windows.Forms.TextBox();
            this.lblJazzCashAmount = new System.Windows.Forms.Label();
            this.txtWalletAmount = new System.Windows.Forms.TextBox();
            this.lblWalletAmount = new System.Windows.Forms.Label();
            this.txtCreditAmount = new System.Windows.Forms.TextBox();
            this.lblCreditAmount = new System.Windows.Forms.Label();
            this.txtTotalPaid = new System.Windows.Forms.TextBox();
            this.lblTotalPaid = new System.Windows.Forms.Label();
            this.txtBalance = new System.Windows.Forms.TextBox();
            this.lblBalance = new System.Windows.Forms.Label();
            
            // Action Buttons
            this.btnSaveBill = new System.Windows.Forms.Button();
            this.btnPrintBill = new System.Windows.Forms.Button();
            this.btnHold = new System.Windows.Forms.Button();
            this.btnRetrieveHold = new System.Windows.Forms.Button();
            this.btnCancelBill = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            
            // Shortcut Buttons
            this.btn1 = new System.Windows.Forms.Button();
            this.btn2 = new System.Windows.Forms.Button();
            this.btn3 = new System.Windows.Forms.Button();
            this.btn4 = new System.Windows.Forms.Button();
            this.btn5 = new System.Windows.Forms.Button();
            this.btn6 = new System.Windows.Forms.Button();
            this.btn7 = new System.Windows.Forms.Button();
            this.btn8 = new System.Windows.Forms.Button();
            this.btn9 = new System.Windows.Forms.Button();
            this.btn0 = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnEnter = new System.Windows.Forms.Button();
            
            ((System.ComponentModel.ISupportInitialize)(this.dgvBillItems)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.groupBoxBarcode.SuspendLayout();
            this.groupBoxItemSelection.SuspendLayout();
            this.groupBoxQuantityPrice.SuspendLayout();
            this.groupBoxSummary.SuspendLayout();
            this.groupBoxPayment.SuspendLayout();
            this.SuspendLayout();
            
            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1400, 800);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Name = "EnhancedPOSForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Enhanced POS System";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.EnhancedPOSForm_KeyDown);
            
            // Panel 1 - Header (Top)
            this.panel1.BackColor = System.Drawing.Color.DarkBlue;
            this.panel1.Controls.Add(this.lblCustomerBalance);
            this.panel1.Controls.Add(this.btnNewCustomer);
            this.panel1.Controls.Add(this.cmbCustomer);
            this.panel1.Controls.Add(this.lblCustomer);
            this.panel1.Controls.Add(this.dtpBillDate);
            this.panel1.Controls.Add(this.lblBillDate);
            this.panel1.Controls.Add(this.txtBillNumber);
            this.panel1.Controls.Add(this.lblBillNumber);
            this.panel1.Controls.Add(this.lblTitle);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1400, 80);
            this.panel1.TabIndex = 0;
            
            // Panel 2 - Item Entry (Left)
            this.panel2.BackColor = System.Drawing.Color.LightGray;
            this.panel2.Controls.Add(this.groupBoxPayment);
            this.panel2.Controls.Add(this.groupBoxQuantityPrice);
            this.panel2.Controls.Add(this.groupBoxItemSelection);
            this.panel2.Controls.Add(this.groupBoxBarcode);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel2.Location = new System.Drawing.Point(0, 80);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(350, 720);
            this.panel2.TabIndex = 1;
            
            // Panel 3 - Bill Items and Summary (Center)
            this.panel3.Controls.Add(this.groupBoxSummary);
            this.panel3.Controls.Add(this.dgvBillItems);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(350, 80);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(750, 720);
            this.panel3.TabIndex = 2;
            
            // Panel 4 - Actions and Keypad (Right)
            this.panel4.BackColor = System.Drawing.Color.LightBlue;
            this.panel4.Controls.Add(this.btnEnter);
            this.panel4.Controls.Add(this.btnClear);
            this.panel4.Controls.Add(this.btn0);
            this.panel4.Controls.Add(this.btn9);
            this.panel4.Controls.Add(this.btn8);
            this.panel4.Controls.Add(this.btn7);
            this.panel4.Controls.Add(this.btn6);
            this.panel4.Controls.Add(this.btn5);
            this.panel4.Controls.Add(this.btn4);
            this.panel4.Controls.Add(this.btn3);
            this.panel4.Controls.Add(this.btn2);
            this.panel4.Controls.Add(this.btn1);
            this.panel4.Controls.Add(this.btnClose);
            this.panel4.Controls.Add(this.btnCancelBill);
            this.panel4.Controls.Add(this.btnRetrieveHold);
            this.panel4.Controls.Add(this.btnHold);
            this.panel4.Controls.Add(this.btnPrintBill);
            this.panel4.Controls.Add(this.btnSaveBill);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel4.Location = new System.Drawing.Point(1100, 80);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(300, 720);
            this.panel4.TabIndex = 3;
            
            // Header Controls Layout
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(12, 15);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(200, 29);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Enhanced POS";
            
            this.lblBillNumber.AutoSize = true;
            this.lblBillNumber.ForeColor = System.Drawing.Color.White;
            this.lblBillNumber.Location = new System.Drawing.Point(12, 50);
            this.lblBillNumber.Name = "lblBillNumber";
            this.lblBillNumber.Size = new System.Drawing.Size(55, 13);
            this.lblBillNumber.TabIndex = 1;
            this.lblBillNumber.Text = "Bill No:";
            
            this.txtBillNumber.Location = new System.Drawing.Point(70, 47);
            this.txtBillNumber.Name = "txtBillNumber";
            this.txtBillNumber.ReadOnly = true;
            this.txtBillNumber.Size = new System.Drawing.Size(120, 20);
            this.txtBillNumber.TabIndex = 2;
            
            this.lblBillDate.AutoSize = true;
            this.lblBillDate.ForeColor = System.Drawing.Color.White;
            this.lblBillDate.Location = new System.Drawing.Point(200, 50);
            this.lblBillDate.Name = "lblBillDate";
            this.lblBillDate.Size = new System.Drawing.Size(55, 13);
            this.lblBillDate.TabIndex = 3;
            this.lblBillDate.Text = "Date:";
            
            this.dtpBillDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpBillDate.Location = new System.Drawing.Point(260, 47);
            this.dtpBillDate.Name = "dtpBillDate";
            this.dtpBillDate.Size = new System.Drawing.Size(100, 20);
            this.dtpBillDate.TabIndex = 4;
            
            this.lblCustomer.AutoSize = true;
            this.lblCustomer.ForeColor = System.Drawing.Color.White;
            this.lblCustomer.Location = new System.Drawing.Point(380, 50);
            this.lblCustomer.Name = "lblCustomer";
            this.lblCustomer.Size = new System.Drawing.Size(60, 13);
            this.lblCustomer.TabIndex = 5;
            this.lblCustomer.Text = "Customer:";
            
            this.cmbCustomer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCustomer.FormattingEnabled = true;
            this.cmbCustomer.Location = new System.Drawing.Point(445, 47);
            this.cmbCustomer.Name = "cmbCustomer";
            this.cmbCustomer.Size = new System.Drawing.Size(200, 21);
            this.cmbCustomer.TabIndex = 6;
            
            this.btnNewCustomer.Location = new System.Drawing.Point(655, 46);
            this.btnNewCustomer.Name = "btnNewCustomer";
            this.btnNewCustomer.Size = new System.Drawing.Size(60, 23);
            this.btnNewCustomer.TabIndex = 7;
            this.btnNewCustomer.Text = "New";
            this.btnNewCustomer.UseVisualStyleBackColor = true;
            this.btnNewCustomer.Click += new System.EventHandler(this.btnNewCustomer_Click);
            
            this.lblCustomerBalance.AutoSize = true;
            this.lblCustomerBalance.ForeColor = System.Drawing.Color.Yellow;
            this.lblCustomerBalance.Location = new System.Drawing.Point(730, 50);
            this.lblCustomerBalance.Name = "lblCustomerBalance";
            this.lblCustomerBalance.Size = new System.Drawing.Size(100, 13);
            this.lblCustomerBalance.TabIndex = 8;
            this.lblCustomerBalance.Text = "Balance: ₹0.00";
            
            // DataGridView
            this.dgvBillItems.AllowUserToAddRows = false;
            this.dgvBillItems.AllowUserToDeleteRows = true;
            this.dgvBillItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBillItems.Location = new System.Drawing.Point(12, 12);
            this.dgvBillItems.Name = "dgvBillItems";
            this.dgvBillItems.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvBillItems.Size = new System.Drawing.Size(726, 500);
            this.dgvBillItems.TabIndex = 0;
            this.dgvBillItems.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.dgvBillItems.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvBillItems_KeyDown);
            
            // Initialize all group boxes and their controls
            InitializeGroupBoxes();
            
            ((System.ComponentModel.ISupportInitialize)(this.dgvBillItems)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.groupBoxBarcode.ResumeLayout(false);
            this.groupBoxBarcode.PerformLayout();
            this.groupBoxItemSelection.ResumeLayout(false);
            this.groupBoxItemSelection.PerformLayout();
            this.groupBoxQuantityPrice.ResumeLayout(false);
            this.groupBoxQuantityPrice.PerformLayout();
            this.groupBoxSummary.ResumeLayout(false);
            this.groupBoxSummary.PerformLayout();
            this.groupBoxPayment.ResumeLayout(false);
            this.groupBoxPayment.PerformLayout();
            this.ResumeLayout(false);
        }

        #region Control Declarations
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.DataGridView dgvBillItems;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.TextBox txtBillNumber;
        private System.Windows.Forms.Label lblBillNumber;
        private System.Windows.Forms.DateTimePicker dtpBillDate;
        private System.Windows.Forms.Label lblBillDate;
        private System.Windows.Forms.ComboBox cmbCustomer;
        private System.Windows.Forms.Label lblCustomer;
        private System.Windows.Forms.Button btnNewCustomer;
        private System.Windows.Forms.Label lblCustomerBalance;
        private System.Windows.Forms.GroupBox groupBoxBarcode;
        private System.Windows.Forms.TextBox txtBarcode;
        private System.Windows.Forms.Label lblBarcode;
        private System.Windows.Forms.Button btnScanBarcode;
        private System.Windows.Forms.Label lblScanStatus;
        private System.Windows.Forms.GroupBox groupBoxItemSelection;
        private System.Windows.Forms.ComboBox cmbItem;
        private System.Windows.Forms.Label lblItem;
        private System.Windows.Forms.TextBox txtItemSearch;
        private System.Windows.Forms.Label lblItemSearch;
        private System.Windows.Forms.Label lblItemDetails;
        private System.Windows.Forms.Label lblStockStatus;
        private System.Windows.Forms.Label lblExpiryWarning;
        private System.Windows.Forms.GroupBox groupBoxQuantityPrice;
        private System.Windows.Forms.TextBox txtQuantity;
        private System.Windows.Forms.Label lblQuantity;
        private System.Windows.Forms.Button btnMinus;
        private System.Windows.Forms.Button btnPlus;
        private System.Windows.Forms.TextBox txtUnitPrice;
        private System.Windows.Forms.Label lblUnitPrice;
        private System.Windows.Forms.TextBox txtMRP;
        private System.Windows.Forms.Label lblMRP;
        private System.Windows.Forms.TextBox txtDiscount;
        private System.Windows.Forms.Label lblDiscount;
        private System.Windows.Forms.TextBox txtDiscountPercent;
        private System.Windows.Forms.Label lblDiscountPercent;
        private System.Windows.Forms.Button btnAddItem;
        private System.Windows.Forms.Button btnClearItem;
        private System.Windows.Forms.GroupBox groupBoxSummary;
        private System.Windows.Forms.TextBox txtGrossAmount;
        private System.Windows.Forms.Label lblGrossAmount;
        private System.Windows.Forms.TextBox txtTotalDiscount;
        private System.Windows.Forms.Label lblTotalDiscount;
        private System.Windows.Forms.TextBox txtTaxableAmount;
        private System.Windows.Forms.Label lblTaxableAmount;
        private System.Windows.Forms.TextBox txtCGST;
        private System.Windows.Forms.Label lblCGST;
        private System.Windows.Forms.TextBox txtSGST;
        private System.Windows.Forms.Label lblSGST;
        private System.Windows.Forms.TextBox txtIGST;
        private System.Windows.Forms.Label lblIGST;
        private System.Windows.Forms.TextBox txtTotalTax;
        private System.Windows.Forms.Label lblTotalTax;
        private System.Windows.Forms.TextBox txtRoundOff;
        private System.Windows.Forms.Label lblRoundOff;
        private System.Windows.Forms.TextBox txtNetAmount;
        private System.Windows.Forms.Label lblNetAmount;
        private System.Windows.Forms.GroupBox groupBoxPayment;
        private System.Windows.Forms.TextBox txtCashAmount;
        private System.Windows.Forms.Label lblCashAmount;
        private System.Windows.Forms.TextBox txtCardAmount;
        private System.Windows.Forms.Label lblCardAmount;
        private System.Windows.Forms.TextBox txtEasyPaisaAmount;
        private System.Windows.Forms.Label lblEasyPaisaAmount;
        private System.Windows.Forms.TextBox txtJazzCashAmount;
        private System.Windows.Forms.Label lblJazzCashAmount;
        private System.Windows.Forms.TextBox txtWalletAmount;
        private System.Windows.Forms.Label lblWalletAmount;
        private System.Windows.Forms.TextBox txtCreditAmount;
        private System.Windows.Forms.Label lblCreditAmount;
        private System.Windows.Forms.TextBox txtTotalPaid;
        private System.Windows.Forms.Label lblTotalPaid;
        private System.Windows.Forms.TextBox txtBalance;
        private System.Windows.Forms.Label lblBalance;
        private System.Windows.Forms.Button btnSaveBill;
        private System.Windows.Forms.Button btnPrintBill;
        private System.Windows.Forms.Button btnHold;
        private System.Windows.Forms.Button btnRetrieveHold;
        private System.Windows.Forms.Button btnCancelBill;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btn1;
        private System.Windows.Forms.Button btn2;
        private System.Windows.Forms.Button btn3;
        private System.Windows.Forms.Button btn4;
        private System.Windows.Forms.Button btn5;
        private System.Windows.Forms.Button btn6;
        private System.Windows.Forms.Button btn7;
        private System.Windows.Forms.Button btn8;
        private System.Windows.Forms.Button btn9;
        private System.Windows.Forms.Button btn0;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnEnter;
        #endregion

        private void InitializeGroupBoxes()
        {
            // Initialize Barcode GroupBox
            this.groupBoxBarcode.Controls.Add(this.lblScanStatus);
            this.groupBoxBarcode.Controls.Add(this.btnScanBarcode);
            this.groupBoxBarcode.Controls.Add(this.lblBarcode);
            this.groupBoxBarcode.Controls.Add(this.txtBarcode);
            this.groupBoxBarcode.Location = new System.Drawing.Point(12, 12);
            this.groupBoxBarcode.Name = "groupBoxBarcode";
            this.groupBoxBarcode.Size = new System.Drawing.Size(326, 80);
            this.groupBoxBarcode.TabIndex = 0;
            this.groupBoxBarcode.TabStop = false;
            this.groupBoxBarcode.Text = "Barcode Scanner";
            
            // Initialize all controls with proper layouts...
            // This is a simplified version - full implementation would have detailed positioning
        }

        private void InitializeForm()
        {
            InitializeDataTable();
            LoadCustomers();
            LoadItems();
            SetupDataGridView();
            GenerateBillNumber();
            SetupEventHandlers();
            SetupBarcodeTimer();
            SetDefaultValues();
        }

        private void InitializeDataTable()
        {
            billItems = new DataTable();
            billItems.Columns.Add("ItemID", typeof(int));
            billItems.Columns.Add("ItemName", typeof(string));
            billItems.Columns.Add("BatchID", typeof(int));
            billItems.Columns.Add("BatchNumber", typeof(string));
            billItems.Columns.Add("ExpiryDate", typeof(DateTime));
            billItems.Columns.Add("Quantity", typeof(int));
            billItems.Columns.Add("UnitPrice", typeof(decimal));
            billItems.Columns.Add("MRP", typeof(decimal));
            billItems.Columns.Add("Discount", typeof(decimal));
            billItems.Columns.Add("DiscountPercent", typeof(decimal));
            billItems.Columns.Add("TaxableAmount", typeof(decimal));
            billItems.Columns.Add("GST_Rate", typeof(decimal));
            billItems.Columns.Add("CGST", typeof(decimal));
            billItems.Columns.Add("SGST", typeof(decimal));
            billItems.Columns.Add("IGST", typeof(decimal));
            billItems.Columns.Add("TotalAmount", typeof(decimal));
        }

        private void SetupDataGridView()
        {
            dgvBillItems.DataSource = billItems;
            dgvBillItems.AutoGenerateColumns = true;
            
            if (dgvBillItems.Columns.Count > 0)
            {
                dgvBillItems.Columns["ItemID"].Visible = false;
                dgvBillItems.Columns["BatchID"].Visible = false;
                dgvBillItems.Columns["ItemName"].HeaderText = "Item";
                dgvBillItems.Columns["ItemName"].Width = 200;
                dgvBillItems.Columns["BatchNumber"].HeaderText = "Batch";
                dgvBillItems.Columns["ExpiryDate"].HeaderText = "Expiry";
                dgvBillItems.Columns["Quantity"].HeaderText = "Qty";
                dgvBillItems.Columns["UnitPrice"].HeaderText = "Price";
                dgvBillItems.Columns["UnitPrice"].DefaultCellStyle.Format = "N2";
                dgvBillItems.Columns["TotalAmount"].HeaderText = "Total";
                dgvBillItems.Columns["TotalAmount"].DefaultCellStyle.Format = "N2";
                
                // Hide detailed columns for cleaner view
                dgvBillItems.Columns["MRP"].Visible = false;
                dgvBillItems.Columns["Discount"].Visible = false;
                dgvBillItems.Columns["DiscountPercent"].Visible = false;
                dgvBillItems.Columns["TaxableAmount"].Visible = false;
                dgvBillItems.Columns["GST_Rate"].Visible = false;
                dgvBillItems.Columns["CGST"].Visible = false;
                dgvBillItems.Columns["SGST"].Visible = false;
                dgvBillItems.Columns["IGST"].Visible = false;
            }
        }

        private void LoadCustomers()
        {
            try
            {
                string query = @"SELECT CustomerID, CustomerName, CurrentBalance, CreditLimit 
                               FROM Customers WHERE IsActive = 1 ORDER BY CustomerName";
                DataTable customers = DatabaseConnection.ExecuteQuery(query);
                
                cmbCustomer.DisplayMember = "CustomerName";
                cmbCustomer.ValueMember = "CustomerID";
                cmbCustomer.DataSource = customers;
                cmbCustomer.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading customers: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadItems()
        {
            try
            {
                string query = @"SELECT ItemID, ItemName, SalePrice, MRP, GST_Rate, StockQuantity 
                               FROM Items WHERE IsActive = 1 AND IsBlocked = 0 ORDER BY ItemName";
                DataTable items = DatabaseConnection.ExecuteQuery(query);
                
                cmbItem.DisplayMember = "ItemName";
                cmbItem.ValueMember = "ItemID";
                cmbItem.DataSource = items;
                cmbItem.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading items: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerateBillNumber()
        {
            try
            {
                string query = "EXEC sp_GetNextBillNumber";
                DataTable result = DatabaseConnection.ExecuteQuery(query);
                
                if (result.Rows.Count > 0)
                {
                    txtBillNumber.Text = result.Rows[0][0].ToString();
                }
            }
            catch (Exception ex)
            {
                txtBillNumber.Text = "BILL" + DateTime.Now.ToString("yyyyMMdd") + "0001";
            }
        }

        private void SetupEventHandlers()
        {
            cmbCustomer.SelectedIndexChanged += CmbCustomer_SelectedIndexChanged;
            cmbItem.SelectedIndexChanged += CmbItem_SelectedIndexChanged;
            txtBarcode.KeyDown += TxtBarcode_KeyDown;
            txtBarcode.TextChanged += TxtBarcode_TextChanged;
            txtItemSearch.TextChanged += TxtItemSearch_TextChanged;
            txtQuantity.TextChanged += CalculateItemTotal;
            txtUnitPrice.TextChanged += CalculateItemTotal;
            txtDiscount.TextChanged += CalculateItemTotal;
            txtDiscountPercent.TextChanged += CalculateItemTotal;
            btnAddItem.Click += BtnAddItem_Click;
            btnClearItem.Click += BtnClearItem_Click;
            btnMinus.Click += BtnMinus_Click;
            btnPlus.Click += BtnPlus_Click;
            btnSaveBill.Click += BtnSaveBill_Click;
            btnPrintBill.Click += BtnPrintBill_Click;
            btnCancelBill.Click += BtnCancelBill_Click;
            btnClose.Click += BtnClose_Click;
            dgvBillItems.UserDeletingRow += DgvBillItems_UserDeletingRow;
            
            // Payment amount change events
            txtCashAmount.TextChanged += CalculatePaymentTotal;
            txtCardAmount.TextChanged += CalculatePaymentTotal;
            txtEasyPaisaAmount.TextChanged += CalculatePaymentTotal;
            txtJazzCashAmount.TextChanged += CalculatePaymentTotal;
            txtWalletAmount.TextChanged += CalculatePaymentTotal;
            txtCreditAmount.TextChanged += CalculatePaymentTotal;
            
            // Number pad events
            SetupNumberPadEvents();
        }

        private void SetupBarcodeTimer()
        {
            barcodeTimer = new Timer();
            barcodeTimer.Interval = 100; // 100ms delay for barcode processing
            barcodeTimer.Tick += BarcodeTimer_Tick;
        }

        private void SetDefaultValues()
        {
            dtpBillDate.Value = DateTime.Now;
            txtQuantity.Text = "1";
            txtCashAmount.Text = "0";
            txtCardAmount.Text = "0";
            txtEasyPaisaAmount.Text = "0";
            txtJazzCashAmount.Text = "0";
            txtWalletAmount.Text = "0";
            txtCreditAmount.Text = "0";
        }

        private void CmbCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbCustomer.SelectedValue != null)
            {
                selectedCustomerID = Convert.ToInt32(cmbCustomer.SelectedValue);
                LoadCustomerBalance();
            }
        }

        private void LoadCustomerBalance()
        {
            try
            {
                string query = "EXEC sp_GetCustomerBalance @CustomerID";
                SqlParameter[] parameters = { new SqlParameter("@CustomerID", selectedCustomerID) };
                DataTable result = DatabaseConnection.ExecuteQuery(query, parameters);
                
                if (result.Rows.Count > 0)
                {
                    decimal balance = Convert.ToDecimal(result.Rows[0]["OutstandingBalance"]);
                    lblCustomerBalance.Text = $"Balance: ₹{balance:N2}";
                    
                    if (balance > 0)
                        lblCustomerBalance.ForeColor = Color.Red;
                    else if (balance < 0)
                        lblCustomerBalance.ForeColor = Color.Green;
                    else
                        lblCustomerBalance.ForeColor = Color.Yellow;
                }
            }
            catch (Exception ex)
            {
                lblCustomerBalance.Text = "Balance: PKR 0.00";
            }
        }

        private void CmbItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbItem.SelectedValue != null)
            {
                LoadItemDetails(Convert.ToInt32(cmbItem.SelectedValue));
            }
        }

        private void LoadItemDetails(int itemID)
        {
            try
            {
                string query = @"SELECT i.ItemName, i.SalePrice, i.MRP, i.GST_Rate, i.StockQuantity,
                               i.Barcode, i.IsPrescriptionRequired,
                               MIN(ib.ExpiryDate) as EarliestExpiry,
                               SUM(ib.QuantityAvailable) as BatchStock
                               FROM Items i
                               LEFT JOIN ItemBatches ib ON i.ItemID = ib.ItemID AND ib.IsActive = 1
                               WHERE i.ItemID = @ItemID
                               GROUP BY i.ItemID, i.ItemName, i.SalePrice, i.MRP, i.GST_Rate, 
                                       i.StockQuantity, i.Barcode, i.IsPrescriptionRequired";
                
                SqlParameter[] parameters = { new SqlParameter("@ItemID", itemID) };
                DataTable result = DatabaseConnection.ExecuteQuery(query, parameters);
                
                if (result.Rows.Count > 0)
                {
                    DataRow row = result.Rows[0];
                    txtBarcode.Text = row["Barcode"].ToString();
                    txtUnitPrice.Text = row["SalePrice"].ToString();
                    txtMRP.Text = row["MRP"].ToString();
                    
                    // Update item details label
                    lblItemDetails.Text = $"MRP: ₹{row["MRP"]} | GST: {row["GST_Rate"]}%";
                    
                    // Update stock status
                    int stockQty = Convert.ToInt32(row["StockQuantity"]);
                    lblStockStatus.Text = $"Stock: {stockQty}";
                    
                    if (stockQty <= 0)
                    {
                        lblStockStatus.ForeColor = Color.Red;
                        lblStockStatus.Text += " (Out of Stock)";
                    }
                    else if (stockQty <= 10) // Assuming reorder level
                    {
                        lblStockStatus.ForeColor = Color.Orange;
                        lblStockStatus.Text += " (Low Stock)";
                    }
                    else
                    {
                        lblStockStatus.ForeColor = Color.Green;
                    }
                    
                    // Check expiry
                    if (row["EarliestExpiry"] != DBNull.Value)
                    {
                        DateTime earliestExpiry = Convert.ToDateTime(row["EarliestExpiry"]);
                        int daysToExpiry = (earliestExpiry - DateTime.Now).Days;
                        
                        if (daysToExpiry <= 0)
                        {
                            lblExpiryWarning.Text = "⚠ EXPIRED BATCHES";
                            lblExpiryWarning.ForeColor = Color.Red;
                            lblExpiryWarning.Visible = true;
                        }
                        else if (daysToExpiry <= 30)
                        {
                            lblExpiryWarning.Text = $"⚠ Expires in {daysToExpiry} days";
                            lblExpiryWarning.ForeColor = Color.Orange;
                            lblExpiryWarning.Visible = true;
                        }
                        else
                        {
                            lblExpiryWarning.Visible = false;
                        }
                    }
                    else
                    {
                        lblExpiryWarning.Visible = false;
                    }
                    
                    // Check if prescription required
                    bool prescriptionRequired = Convert.ToBoolean(row["IsPrescriptionRequired"]);
                    if (prescriptionRequired)
                    {
                        MessageBox.Show("⚠ This medicine requires a prescription!", "Prescription Required",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading item details: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TxtBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ProcessBarcodeInput();
            }
        }

        private void TxtBarcode_TextChanged(object sender, EventArgs e)
        {
            // Reset timer for barcode scanner input processing
            barcodeTimer.Stop();
            scannedBarcode = txtBarcode.Text;
            barcodeTimer.Start();
        }

        private void BarcodeTimer_Tick(object sender, EventArgs e)
        {
            barcodeTimer.Stop();
            if (!string.IsNullOrEmpty(scannedBarcode))
            {
                ProcessBarcodeInput();
            }
        }

        private void ProcessBarcodeInput()
        {
            if (!string.IsNullOrEmpty(txtBarcode.Text))
            {
                try
                {
                    string query = "SELECT ItemID, ItemName FROM Items WHERE Barcode = @Barcode AND IsActive = 1";
                    SqlParameter[] parameters = { new SqlParameter("@Barcode", txtBarcode.Text.Trim()) };
                    DataTable result = DatabaseConnection.ExecuteQuery(query, parameters);
                    
                    if (result.Rows.Count > 0)
                    {
                        int itemID = Convert.ToInt32(result.Rows[0]["ItemID"]);
                        cmbItem.SelectedValue = itemID;
                        lblScanStatus.Text = "✓ Item Found";
                        lblScanStatus.ForeColor = Color.Green;
                        
                        // Play success sound
                        SystemSounds.Beep.Play();
                        
                        // Auto-add item if quantity is set
                        if (!string.IsNullOrEmpty(txtQuantity.Text) && txtQuantity.Text != "0")
                        {
                            BtnAddItem_Click(null, null);
                        }
                        else
                        {
                            txtQuantity.Focus();
                            txtQuantity.SelectAll();
                        }
                    }
                    else
                    {
                        lblScanStatus.Text = "✗ Item Not Found";
                        lblScanStatus.ForeColor = Color.Red;
                        SystemSounds.Hand.Play();
                    }
                }
                catch (Exception ex)
                {
                    lblScanStatus.Text = "✗ Scan Error";
                    lblScanStatus.ForeColor = Color.Red;
                    MessageBox.Show("Error processing barcode: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void TxtItemSearch_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtItemSearch.Text))
            {
                try
                {
                    string searchText = txtItemSearch.Text.ToLower();
                    var items = ((DataTable)cmbItem.DataSource).AsEnumerable()
                        .Where(row => row.Field<string>("ItemName").ToLower().Contains(searchText))
                        .CopyToDataTable();
                    
                    if (items.Rows.Count > 0)
                    {
                        cmbItem.SelectedValue = items.Rows[0]["ItemID"];
                    }
                }
                catch
                {
                    // Handle search errors silently
                }
            }
        }

        private void CalculateItemTotal(object sender, EventArgs e)
        {
            // This method calculates the total for current item entry
            // Implementation would calculate based on quantity, price, discount, tax
        }

        private void BtnMinus_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtQuantity.Text, out int qty) && qty > 1)
            {
                txtQuantity.Text = (qty - 1).ToString();
            }
        }

        private void BtnPlus_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtQuantity.Text, out int qty))
            {
                txtQuantity.Text = (qty + 1).ToString();
            }
        }

        private void BtnAddItem_Click(object sender, EventArgs e)
        {
            if (ValidateItemEntry())
            {
                AddItemToBill();
                CalculateBillTotals();
                ClearItemEntry();
                txtBarcode.Focus();
            }
        }

        private bool ValidateItemEntry()
        {
            if (cmbItem.SelectedValue == null)
            {
                MessageBox.Show("Please select an item.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            
            if (string.IsNullOrEmpty(txtQuantity.Text) || !int.TryParse(txtQuantity.Text, out int qty) || qty <= 0)
            {
                MessageBox.Show("Please enter a valid quantity.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            
            // Check stock availability
            if (!CheckStockAvailability(Convert.ToInt32(cmbItem.SelectedValue), qty))
            {
                return false;
            }
            
            return true;
        }

        private bool CheckStockAvailability(int itemID, int requestedQty)
        {
            try
            {
                string query = "SELECT StockQuantity FROM Items WHERE ItemID = @ItemID";
                SqlParameter[] parameters = { new SqlParameter("@ItemID", itemID) };
                DataTable result = DatabaseConnection.ExecuteQuery(query, parameters);
                
                if (result.Rows.Count > 0)
                {
                    int availableStock = Convert.ToInt32(result.Rows[0]["StockQuantity"]);
                    if (requestedQty > availableStock)
                    {
                        MessageBox.Show($"Insufficient stock! Available: {availableStock}", "Stock Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void AddItemToBill()
        {
            // Implementation for adding item to bill
            // This would handle batch selection, expiry checking, price calculation, etc.
        }

        private void CalculateBillTotals()
        {
            // Implementation for calculating all bill totals
            // This would sum up all items and calculate taxes, discounts, etc.
        }

        private void ClearItemEntry()
        {
            cmbItem.SelectedIndex = -1;
            txtBarcode.Text = "";
            txtQuantity.Text = "1";
            txtUnitPrice.Text = "";
            txtMRP.Text = "";
            txtDiscount.Text = "";
            txtDiscountPercent.Text = "";
            lblItemDetails.Text = "";
            lblStockStatus.Text = "";
            lblExpiryWarning.Visible = false;
            lblScanStatus.Text = "";
        }

        private void BtnClearItem_Click(object sender, EventArgs e)
        {
            ClearItemEntry();
        }

        private void CalculatePaymentTotal(object sender, EventArgs e)
        {
            // Implementation for calculating total payments and balance
        }

        private void SetupNumberPadEvents()
        {
            // Setup number pad button events for POS interface
        }

        private void DgvBillItems_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (MessageBox.Show("Remove this item from bill?", "Confirm Remove",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                this.BeginInvoke(new Action(() => CalculateBillTotals()));
            }
        }

        private void dgvBillItems_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (dgvBillItems.SelectedRows.Count > 0)
                {
                    if (MessageBox.Show("Remove selected item from bill?", "Confirm Remove",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        dgvBillItems.Rows.RemoveAt(dgvBillItems.SelectedRows[0].Index);
                        CalculateBillTotals();
                    }
                }
            }
        }

        private void EnhancedPOSForm_KeyDown(object sender, KeyEventArgs e)
        {
            // Handle global keyboard shortcuts
            switch (e.KeyCode)
            {
                case Keys.F1:
                    txtBarcode.Focus();
                    break;
                case Keys.F2:
                    BtnAddItem_Click(null, null);
                    break;
                case Keys.F9:
                    BtnSaveBill_Click(null, null);
                    break;
                case Keys.F10:
                    BtnPrintBill_Click(null, null);
                    break;
                case Keys.Escape:
                    BtnCancelBill_Click(null, null);
                    break;
            }
        }

        private void btnNewCustomer_Click(object sender, EventArgs e)
        {
            // Open new customer form
            MessageBox.Show("New customer form will be implemented.", "Information",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnSaveBill_Click(object sender, EventArgs e)
        {
            if (ValidateBill())
            {
                try
                {
                    SaveBill();
                    MessageBox.Show("Bill saved successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearBill();
                    GenerateBillNumber();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error saving bill: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private bool ValidateBill()
        {
            if (selectedCustomerID == 0)
            {
                MessageBox.Show("Please select a customer.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            
            if (billItems.Rows.Count == 0)
            {
                MessageBox.Show("Please add at least one item.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            
            return true;
        }

        private void SaveBill()
        {
            using (SqlConnection connection = new SqlConnection(DatabaseConnection.GetConnectionStringPublic()))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Calculate totals
                        decimal totalAmount = 0;
                        decimal discountAmount = 0;
                        decimal taxAmount = 0;
                        decimal netAmount = 0;

                        foreach (DataRow row in billItems.Rows)
                        {
                            totalAmount += Convert.ToDecimal(row["Amount"]);
                            discountAmount += Convert.ToDecimal(row["Discount"]);
                            taxAmount += Convert.ToDecimal(row["Tax"]);
                        }
                        netAmount = totalAmount - discountAmount + taxAmount;

                        // Insert into Sales table
                        string salesQuery = @"
                            INSERT INTO Sales (BillNumber, CustomerID, SaleDate, TotalAmount, Discount, TaxAmount, NetAmount, 
                                             PaymentMethod, PaymentStatus, CashAmount, CardAmount, EasyPaisaAmount, JazzCashAmount, 
                                             IsCredit, IsActive, CreatedDate, CreatedBy)
                            OUTPUT INSERTED.SaleID
                            VALUES (@BillNumber, @CustomerID, @SaleDate, @TotalAmount, @Discount, @TaxAmount, @NetAmount,
                                   @PaymentMethod, @PaymentStatus, @CashAmount, @CardAmount, @EasyPaisaAmount, @JazzCashAmount,
                                   @IsCredit, 1, GETDATE(), @CreatedBy)";

                        SqlParameter[] salesParams = {
                            new SqlParameter("@BillNumber", txtBillNumber.Text),
                            new SqlParameter("@CustomerID", selectedCustomerID),
                            new SqlParameter("@SaleDate", dtpBillDate.Value),
                            new SqlParameter("@TotalAmount", totalAmount),
                            new SqlParameter("@Discount", discountAmount),
                            new SqlParameter("@TaxAmount", taxAmount),
                            new SqlParameter("@NetAmount", netAmount),
                            new SqlParameter("@PaymentMethod", GetPrimaryPaymentMethod()),
                            new SqlParameter("@PaymentStatus", "Paid"),
                            new SqlParameter("@CashAmount", decimal.Parse(txtCashAmount.Text)),
                            new SqlParameter("@CardAmount", decimal.Parse(txtCardAmount.Text)),
                            new SqlParameter("@EasyPaisaAmount", decimal.Parse(txtEasyPaisaAmount?.Text ?? "0")),
                            new SqlParameter("@JazzCashAmount", decimal.Parse(txtJazzCashAmount?.Text ?? "0")),
                            new SqlParameter("@IsCredit", decimal.Parse(txtCreditAmount?.Text ?? "0") > 0),
                            new SqlParameter("@CreatedBy", currentUserID)
                        };

                        // Execute and get SaleID
                        object result = DatabaseConnection.ExecuteScalar(salesQuery, salesParams, transaction);
                        int saleID = Convert.ToInt32(result);

                        // Insert SaleItems
                        foreach (DataRow row in billItems.Rows)
                        {
                            string itemsQuery = @"
                                INSERT INTO SaleItems (SaleID, ItemID, Quantity, UnitPrice, TotalAmount, DiscountAmount, TaxAmount)
                                VALUES (@SaleID, @ItemID, @Quantity, @UnitPrice, @TotalAmount, @DiscountAmount, @TaxAmount)";

                            SqlParameter[] itemParams = {
                                new SqlParameter("@SaleID", saleID),
                                new SqlParameter("@ItemID", row["ItemID"]),
                                new SqlParameter("@Quantity", row["Quantity"]),
                                new SqlParameter("@UnitPrice", row["Price"]),
                                new SqlParameter("@TotalAmount", row["Amount"]),
                                new SqlParameter("@DiscountAmount", row["Discount"]),
                                new SqlParameter("@TaxAmount", row["Tax"])
                            };

                            DatabaseConnection.ExecuteNonQuery(itemsQuery, itemParams, transaction);

                            // Update stock quantity
                            string stockQuery = @"
                                UPDATE Items 
                                SET StockQuantity = StockQuantity - @Quantity,
                                    LastSaleDate = GETDATE()
                                WHERE ItemID = @ItemID";

                            SqlParameter[] stockParams = {
                                new SqlParameter("@Quantity", row["Quantity"]),
                                new SqlParameter("@ItemID", row["ItemID"])
                            };

                            DatabaseConnection.ExecuteNonQuery(stockQuery, stockParams, transaction);
                        }

                        transaction.Commit();
                        MessageBox.Show("Bill saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearBill();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("Error saving bill: " + ex.Message);
                    }
                }
            }
        }

        private string GetPrimaryPaymentMethod()
        {
            decimal cashAmount = decimal.Parse(txtCashAmount.Text);
            decimal cardAmount = decimal.Parse(txtCardAmount.Text);
            decimal easyPaisaAmount = decimal.Parse(txtEasyPaisaAmount?.Text ?? "0");
            decimal jazzCashAmount = decimal.Parse(txtJazzCashAmount?.Text ?? "0");

            if (cashAmount > cardAmount && cashAmount > easyPaisaAmount && cashAmount > jazzCashAmount)
                return "Cash";
            else if (cardAmount > easyPaisaAmount && cardAmount > jazzCashAmount)
                return "Card";
            else if (easyPaisaAmount > jazzCashAmount)
                return "EasyPaisa";
            else if (jazzCashAmount > 0)
                return "JazzCash";
            else
                return "Cash";
        }

        private void ClearBill()
        {
            billItems.Rows.Clear();
            cmbCustomer.SelectedIndex = -1;
            selectedCustomerID = 0;
            lblCustomerBalance.Text = "Balance: PKR 0.00";
            ClearItemEntry();
            ClearPaymentAmounts();
            ClearSummaryAmounts();
        }

        private void ClearPaymentAmounts()
        {
            txtCashAmount.Text = "0";
            txtCardAmount.Text = "0";
            txtEasyPaisaAmount.Text = "0";
            txtJazzCashAmount.Text = "0";
            txtWalletAmount.Text = "0";
            txtCreditAmount.Text = "0";
            txtTotalPaid.Text = "0.00";
            txtBalance.Text = "0.00";
        }

        private void ClearSummaryAmounts()
        {
            txtGrossAmount.Text = "0.00";
            txtTotalDiscount.Text = "0.00";
            txtTaxableAmount.Text = "0.00";
            txtCGST.Text = "0.00";
            txtSGST.Text = "0.00";
            txtIGST.Text = "0.00";
            txtTotalTax.Text = "0.00";
            txtRoundOff.Text = "0.00";
            txtNetAmount.Text = "0.00";
        }

        private void BtnPrintBill_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Print functionality will be implemented with reporting engine.", "Information",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnCancelBill_Click(object sender, EventArgs e)
        {
            if (billItems.Rows.Count > 0)
            {
                if (MessageBox.Show("Cancel current bill? All items will be removed.", "Confirm Cancel",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    ClearBill();
                    GenerateBillNumber();
                }
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            if (billItems.Rows.Count > 0)
            {
                if (MessageBox.Show("Close without saving? All unsaved data will be lost.", "Confirm Close",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return;
                }
            }
            this.Close();
        }
    }
}
