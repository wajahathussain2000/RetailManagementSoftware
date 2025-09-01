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
    public partial class EnhancedPurchaseForm : Form
    {
        private DataTable purchaseItems;
        private int selectedCompanyID = 0;
        private decimal totalGrossAmount = 0;
        private decimal totalDiscount = 0;
        private decimal totalTax = 0;
        private decimal totalNetAmount = 0;
        private int currentUserID = 1;
        private bool isEditMode = false;
        private int editPurchaseID = 0;

        public EnhancedPurchaseForm()
        {
            InitializeComponent();
            InitializeForm();
        }

        public EnhancedPurchaseForm(int userID) : this()
        {
            currentUserID = userID;
        }

        public EnhancedPurchaseForm(int userID, int purchaseID) : this(userID)
        {
            isEditMode = true;
            editPurchaseID = purchaseID;
            LoadPurchaseForEdit();
        }

        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.dgvPurchaseItems = new System.Windows.Forms.DataGridView();
            
            // Header Panel Controls
            this.lblTitle = new System.Windows.Forms.Label();
            this.txtPurchaseNumber = new System.Windows.Forms.TextBox();
            this.lblPurchaseNumber = new System.Windows.Forms.Label();
            this.dtpPurchaseDate = new System.Windows.Forms.DateTimePicker();
            this.lblPurchaseDate = new System.Windows.Forms.Label();
            this.cmbSupplier = new System.Windows.Forms.ComboBox();
            this.lblSupplier = new System.Windows.Forms.Label();
            this.txtInvoiceNumber = new System.Windows.Forms.TextBox();
            this.lblInvoiceNumber = new System.Windows.Forms.Label();
            this.dtpInvoiceDate = new System.Windows.Forms.DateTimePicker();
            this.lblInvoiceDate = new System.Windows.Forms.Label();
            
            // Item Entry Panel Controls
            this.groupBoxItemEntry = new System.Windows.Forms.GroupBox();
            this.cmbItem = new System.Windows.Forms.ComboBox();
            this.lblItem = new System.Windows.Forms.Label();
            this.txtBarcode = new System.Windows.Forms.TextBox();
            this.lblBarcode = new System.Windows.Forms.Label();
            this.btnScanBarcode = new System.Windows.Forms.Button();
            this.lblBarcodeStatus = new System.Windows.Forms.Label();
            this.txtBatchNumber = new System.Windows.Forms.TextBox();
            this.lblBatchNumber = new System.Windows.Forms.Label();
            this.dtpMfgDate = new System.Windows.Forms.DateTimePicker();
            this.lblMfgDate = new System.Windows.Forms.Label();
            this.dtpExpiryDate = new System.Windows.Forms.DateTimePicker();
            this.lblExpiryDate = new System.Windows.Forms.Label();
            this.txtQuantity = new System.Windows.Forms.TextBox();
            this.lblQuantity = new System.Windows.Forms.Label();
            this.txtFreeQuantity = new System.Windows.Forms.TextBox();
            this.lblFreeQuantity = new System.Windows.Forms.Label();
            this.txtUnitPrice = new System.Windows.Forms.TextBox();
            this.lblUnitPrice = new System.Windows.Forms.Label();
            this.txtMRP = new System.Windows.Forms.TextBox();
            this.lblMRP = new System.Windows.Forms.Label();
            this.txtDiscount = new System.Windows.Forms.TextBox();
            this.lblDiscount = new System.Windows.Forms.Label();
            this.txtDiscountPercent = new System.Windows.Forms.TextBox();
            this.lblDiscountPercent = new System.Windows.Forms.Label();
            this.txtGSTRate = new System.Windows.Forms.TextBox();
            this.lblGSTRate = new System.Windows.Forms.Label();
            this.txtTotalAmount = new System.Windows.Forms.TextBox();
            this.lblTotalAmount = new System.Windows.Forms.Label();
            this.btnAddItem = new System.Windows.Forms.Button();
            this.btnClearItem = new System.Windows.Forms.Button();
            
            // Summary Panel Controls
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
            this.txtNetAmount = new System.Windows.Forms.TextBox();
            this.lblNetAmount = new System.Windows.Forms.Label();
            this.txtRoundOff = new System.Windows.Forms.TextBox();
            this.lblRoundOff = new System.Windows.Forms.Label();
            
            // Action Buttons
            this.btnSave = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            
            ((System.ComponentModel.ISupportInitialize)(this.dgvPurchaseItems)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.groupBoxItemEntry.SuspendLayout();
            this.groupBoxSummary.SuspendLayout();
            this.SuspendLayout();
            
            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "EnhancedPurchaseForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Enhanced Purchase Entry";
            
            // Panel 1 - Header
            this.panel1.Controls.Add(this.dtpInvoiceDate);
            this.panel1.Controls.Add(this.lblInvoiceDate);
            this.panel1.Controls.Add(this.txtInvoiceNumber);
            this.panel1.Controls.Add(this.lblInvoiceNumber);
            this.panel1.Controls.Add(this.cmbSupplier);
            this.panel1.Controls.Add(this.lblSupplier);
            this.panel1.Controls.Add(this.dtpPurchaseDate);
            this.panel1.Controls.Add(this.lblPurchaseDate);
            this.panel1.Controls.Add(this.txtPurchaseNumber);
            this.panel1.Controls.Add(this.lblPurchaseNumber);
            this.panel1.Controls.Add(this.lblTitle);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1200, 100);
            this.panel1.TabIndex = 0;
            this.panel1.BackColor = System.Drawing.Color.LightBlue;
            
            // Panel 2 - Item Entry and Summary
            this.panel2.Controls.Add(this.groupBoxSummary);
            this.panel2.Controls.Add(this.groupBoxItemEntry);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 100);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1200, 200);
            this.panel2.TabIndex = 1;
            
            // Panel 3 - Grid and Actions
            this.panel3.Controls.Add(this.btnClose);
            this.panel3.Controls.Add(this.btnCancel);
            this.panel3.Controls.Add(this.btnPrint);
            this.panel3.Controls.Add(this.btnSave);
            this.panel3.Controls.Add(this.dgvPurchaseItems);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 300);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1200, 400);
            this.panel3.TabIndex = 2;
            
            // Header Controls
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(12, 15);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(250, 26);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Enhanced Purchase Entry";
            
            this.lblPurchaseNumber.AutoSize = true;
            this.lblPurchaseNumber.Location = new System.Drawing.Point(12, 50);
            this.lblPurchaseNumber.Name = "lblPurchaseNumber";
            this.lblPurchaseNumber.Size = new System.Drawing.Size(85, 13);
            this.lblPurchaseNumber.TabIndex = 1;
            this.lblPurchaseNumber.Text = "Purchase No:";
            
            this.txtPurchaseNumber.Location = new System.Drawing.Point(100, 47);
            this.txtPurchaseNumber.Name = "txtPurchaseNumber";
            this.txtPurchaseNumber.ReadOnly = true;
            this.txtPurchaseNumber.Size = new System.Drawing.Size(120, 20);
            this.txtPurchaseNumber.TabIndex = 2;
            
            this.lblPurchaseDate.AutoSize = true;
            this.lblPurchaseDate.Location = new System.Drawing.Point(230, 50);
            this.lblPurchaseDate.Name = "lblPurchaseDate";
            this.lblPurchaseDate.Size = new System.Drawing.Size(85, 13);
            this.lblPurchaseDate.TabIndex = 3;
            this.lblPurchaseDate.Text = "Purchase Date:";
            
            this.dtpPurchaseDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpPurchaseDate.Location = new System.Drawing.Point(320, 47);
            this.dtpPurchaseDate.Name = "dtpPurchaseDate";
            this.dtpPurchaseDate.Size = new System.Drawing.Size(100, 20);
            this.dtpPurchaseDate.TabIndex = 4;
            
            this.lblSupplier.AutoSize = true;
            this.lblSupplier.Location = new System.Drawing.Point(430, 50);
            this.lblSupplier.Name = "lblSupplier";
            this.lblSupplier.Size = new System.Drawing.Size(50, 13);
            this.lblSupplier.TabIndex = 5;
            this.lblSupplier.Text = "Supplier:";
            
            this.cmbSupplier.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSupplier.FormattingEnabled = true;
            this.cmbSupplier.Location = new System.Drawing.Point(485, 47);
            this.cmbSupplier.Name = "cmbSupplier";
            this.cmbSupplier.Size = new System.Drawing.Size(200, 21);
            this.cmbSupplier.TabIndex = 6;
            
            this.lblInvoiceNumber.AutoSize = true;
            this.lblInvoiceNumber.Location = new System.Drawing.Point(12, 75);
            this.lblInvoiceNumber.Name = "lblInvoiceNumber";
            this.lblInvoiceNumber.Size = new System.Drawing.Size(70, 13);
            this.lblInvoiceNumber.TabIndex = 7;
            this.lblInvoiceNumber.Text = "Invoice No:";
            
            this.txtInvoiceNumber.Location = new System.Drawing.Point(100, 72);
            this.txtInvoiceNumber.Name = "txtInvoiceNumber";
            this.txtInvoiceNumber.Size = new System.Drawing.Size(120, 20);
            this.txtInvoiceNumber.TabIndex = 8;
            
            this.lblInvoiceDate.AutoSize = true;
            this.lblInvoiceDate.Location = new System.Drawing.Point(230, 75);
            this.lblInvoiceDate.Name = "lblInvoiceDate";
            this.lblInvoiceDate.Size = new System.Drawing.Size(75, 13);
            this.lblInvoiceDate.TabIndex = 9;
            this.lblInvoiceDate.Text = "Invoice Date:";
            
            this.dtpInvoiceDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpInvoiceDate.Location = new System.Drawing.Point(320, 72);
            this.dtpInvoiceDate.Name = "dtpInvoiceDate";
            this.dtpInvoiceDate.Size = new System.Drawing.Size(100, 20);
            this.dtpInvoiceDate.TabIndex = 10;
            
            // Item Entry GroupBox
            this.groupBoxItemEntry.Controls.Add(this.btnClearItem);
            this.groupBoxItemEntry.Controls.Add(this.btnAddItem);
            this.groupBoxItemEntry.Controls.Add(this.lblTotalAmount);
            this.groupBoxItemEntry.Controls.Add(this.txtTotalAmount);
            this.groupBoxItemEntry.Controls.Add(this.lblGSTRate);
            this.groupBoxItemEntry.Controls.Add(this.txtGSTRate);
            this.groupBoxItemEntry.Controls.Add(this.lblDiscountPercent);
            this.groupBoxItemEntry.Controls.Add(this.txtDiscountPercent);
            this.groupBoxItemEntry.Controls.Add(this.lblDiscount);
            this.groupBoxItemEntry.Controls.Add(this.txtDiscount);
            this.groupBoxItemEntry.Controls.Add(this.lblMRP);
            this.groupBoxItemEntry.Controls.Add(this.txtMRP);
            this.groupBoxItemEntry.Controls.Add(this.lblUnitPrice);
            this.groupBoxItemEntry.Controls.Add(this.txtUnitPrice);
            this.groupBoxItemEntry.Controls.Add(this.lblFreeQuantity);
            this.groupBoxItemEntry.Controls.Add(this.txtFreeQuantity);
            this.groupBoxItemEntry.Controls.Add(this.lblQuantity);
            this.groupBoxItemEntry.Controls.Add(this.txtQuantity);
            this.groupBoxItemEntry.Controls.Add(this.lblExpiryDate);
            this.groupBoxItemEntry.Controls.Add(this.dtpExpiryDate);
            this.groupBoxItemEntry.Controls.Add(this.lblMfgDate);
            this.groupBoxItemEntry.Controls.Add(this.dtpMfgDate);
            this.groupBoxItemEntry.Controls.Add(this.lblBatchNumber);
            this.groupBoxItemEntry.Controls.Add(this.txtBatchNumber);
            this.groupBoxItemEntry.Controls.Add(this.btnScanBarcode);
            this.groupBoxItemEntry.Controls.Add(this.lblBarcodeStatus);
            this.groupBoxItemEntry.Controls.Add(this.lblBarcode);
            this.groupBoxItemEntry.Controls.Add(this.txtBarcode);
            this.groupBoxItemEntry.Controls.Add(this.lblItem);
            this.groupBoxItemEntry.Controls.Add(this.cmbItem);
            this.groupBoxItemEntry.Location = new System.Drawing.Point(12, 6);
            this.groupBoxItemEntry.Name = "groupBoxItemEntry";
            this.groupBoxItemEntry.Size = new System.Drawing.Size(750, 188);
            this.groupBoxItemEntry.TabIndex = 0;
            this.groupBoxItemEntry.TabStop = false;
            this.groupBoxItemEntry.Text = "Item Entry";
            
            // DataGridView
            this.dgvPurchaseItems.AllowUserToAddRows = false;
            this.dgvPurchaseItems.AllowUserToDeleteRows = true;
            this.dgvPurchaseItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPurchaseItems.Location = new System.Drawing.Point(12, 12);
            this.dgvPurchaseItems.Name = "dgvPurchaseItems";
            this.dgvPurchaseItems.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPurchaseItems.Size = new System.Drawing.Size(1176, 300);
            this.dgvPurchaseItems.TabIndex = 0;
            
            // Action Buttons
            this.btnSave.Location = new System.Drawing.Point(12, 320);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 35);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "Save Purchase";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            
            this.btnPrint.Location = new System.Drawing.Point(125, 320);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(100, 35);
            this.btnPrint.TabIndex = 2;
            this.btnPrint.Text = "Print Invoice";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            
            this.btnCancel.Location = new System.Drawing.Point(240, 320);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 35);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            
            this.btnClose.Location = new System.Drawing.Point(355, 320);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(100, 35);
            this.btnClose.TabIndex = 4;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            
            ((System.ComponentModel.ISupportInitialize)(this.dgvPurchaseItems)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.groupBoxItemEntry.ResumeLayout(false);
            this.groupBoxItemEntry.PerformLayout();
            this.groupBoxSummary.ResumeLayout(false);
            this.groupBoxSummary.PerformLayout();
            // Configure lblBarcodeStatus
            this.lblBarcodeStatus.Text = "Ready to scan";
            this.lblBarcodeStatus.Location = new System.Drawing.Point(400, 30);
            this.lblBarcodeStatus.Size = new System.Drawing.Size(200, 20);
            this.lblBarcodeStatus.ForeColor = System.Drawing.Color.Gray;
            this.lblBarcodeStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);

            this.ResumeLayout(false);
        }

        #region Control Declarations
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.DataGridView dgvPurchaseItems;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.TextBox txtPurchaseNumber;
        private System.Windows.Forms.Label lblPurchaseNumber;
        private System.Windows.Forms.DateTimePicker dtpPurchaseDate;
        private System.Windows.Forms.Label lblPurchaseDate;
        private System.Windows.Forms.ComboBox cmbSupplier;
        private System.Windows.Forms.Label lblSupplier;
        private System.Windows.Forms.TextBox txtInvoiceNumber;
        private System.Windows.Forms.Label lblInvoiceNumber;
        private System.Windows.Forms.DateTimePicker dtpInvoiceDate;
        private System.Windows.Forms.Label lblInvoiceDate;
        private System.Windows.Forms.GroupBox groupBoxItemEntry;
        private System.Windows.Forms.ComboBox cmbItem;
        private System.Windows.Forms.Label lblItem;
        private System.Windows.Forms.TextBox txtBarcode;
        private System.Windows.Forms.Label lblBarcode;
        private System.Windows.Forms.Button btnScanBarcode;
        private System.Windows.Forms.Label lblBarcodeStatus;
        private System.Windows.Forms.TextBox txtBatchNumber;
        private System.Windows.Forms.Label lblBatchNumber;
        private System.Windows.Forms.DateTimePicker dtpMfgDate;
        private System.Windows.Forms.Label lblMfgDate;
        private System.Windows.Forms.DateTimePicker dtpExpiryDate;
        private System.Windows.Forms.Label lblExpiryDate;
        private System.Windows.Forms.TextBox txtQuantity;
        private System.Windows.Forms.Label lblQuantity;
        private System.Windows.Forms.TextBox txtFreeQuantity;
        private System.Windows.Forms.Label lblFreeQuantity;
        private System.Windows.Forms.TextBox txtUnitPrice;
        private System.Windows.Forms.Label lblUnitPrice;
        private System.Windows.Forms.TextBox txtMRP;
        private System.Windows.Forms.Label lblMRP;
        private System.Windows.Forms.TextBox txtDiscount;
        private System.Windows.Forms.Label lblDiscount;
        private System.Windows.Forms.TextBox txtDiscountPercent;
        private System.Windows.Forms.Label lblDiscountPercent;
        private System.Windows.Forms.TextBox txtGSTRate;
        private System.Windows.Forms.Label lblGSTRate;
        private System.Windows.Forms.TextBox txtTotalAmount;
        private System.Windows.Forms.Label lblTotalAmount;
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
        private System.Windows.Forms.TextBox txtNetAmount;
        private System.Windows.Forms.Label lblNetAmount;
        private System.Windows.Forms.TextBox txtRoundOff;
        private System.Windows.Forms.Label lblRoundOff;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnClose;
        #endregion

        private void InitializeForm()
        {
            InitializeDataTable();
            LoadSuppliers();
            LoadItems();
            SetupDataGridView();
            GeneratePurchaseNumber();
            SetupEventHandlers();
            SetDefaultValues();
        }

        private void InitializeDataTable()
        {
            purchaseItems = new DataTable();
            purchaseItems.Columns.Add("ItemID", typeof(int));
            purchaseItems.Columns.Add("ItemName", typeof(string));
            purchaseItems.Columns.Add("BatchNumber", typeof(string));
            purchaseItems.Columns.Add("MfgDate", typeof(DateTime));
            purchaseItems.Columns.Add("ExpiryDate", typeof(DateTime));
            purchaseItems.Columns.Add("Quantity", typeof(int));
            purchaseItems.Columns.Add("FreeQuantity", typeof(int));
            purchaseItems.Columns.Add("UnitPrice", typeof(decimal));
            purchaseItems.Columns.Add("MRP", typeof(decimal));
            purchaseItems.Columns.Add("Discount", typeof(decimal));
            purchaseItems.Columns.Add("DiscountPercent", typeof(decimal));
            purchaseItems.Columns.Add("TaxableAmount", typeof(decimal));
            purchaseItems.Columns.Add("GST_Rate", typeof(decimal));
            purchaseItems.Columns.Add("CGST", typeof(decimal));
            purchaseItems.Columns.Add("SGST", typeof(decimal));
            purchaseItems.Columns.Add("IGST", typeof(decimal));
            purchaseItems.Columns.Add("TotalAmount", typeof(decimal));
        }

        private void SetupDataGridView()
        {
            dgvPurchaseItems.DataSource = purchaseItems;
            dgvPurchaseItems.AutoGenerateColumns = true;
            
            // Format columns
            if (dgvPurchaseItems.Columns.Count > 0)
            {
                dgvPurchaseItems.Columns["ItemID"].Visible = false;
                dgvPurchaseItems.Columns["ItemName"].HeaderText = "Item Name";
                dgvPurchaseItems.Columns["ItemName"].Width = 200;
                dgvPurchaseItems.Columns["BatchNumber"].HeaderText = "Batch #";
                dgvPurchaseItems.Columns["MfgDate"].HeaderText = "Mfg Date";
                dgvPurchaseItems.Columns["ExpiryDate"].HeaderText = "Expiry Date";
                dgvPurchaseItems.Columns["Quantity"].HeaderText = "Qty";
                dgvPurchaseItems.Columns["FreeQuantity"].HeaderText = "Free";
                dgvPurchaseItems.Columns["UnitPrice"].HeaderText = "Unit Price";
                dgvPurchaseItems.Columns["UnitPrice"].DefaultCellStyle.Format = "N2";
                dgvPurchaseItems.Columns["MRP"].DefaultCellStyle.Format = "N2";
                dgvPurchaseItems.Columns["TotalAmount"].HeaderText = "Total";
                dgvPurchaseItems.Columns["TotalAmount"].DefaultCellStyle.Format = "N2";
            }
        }

        private void LoadSuppliers()
        {
            try
            {
                string query = "SELECT CompanyID, CompanyName FROM Companies WHERE IsActive = 1 ORDER BY CompanyName";
                DataTable suppliers = DatabaseConnection.ExecuteQuery(query);
                
                cmbSupplier.DisplayMember = "CompanyName";
                cmbSupplier.ValueMember = "CompanyID";
                cmbSupplier.DataSource = suppliers;
                cmbSupplier.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading suppliers: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadItems()
        {
            try
            {
                string query = "SELECT ItemID, ItemName FROM Items WHERE IsActive = 1 ORDER BY ItemName";
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

        private void GeneratePurchaseNumber()
        {
            try
            {
                string query = "EXEC sp_GetNextPurchaseNumber";
                DataTable result = DatabaseConnection.ExecuteQuery(query);
                
                if (result.Rows.Count > 0)
                {
                    txtPurchaseNumber.Text = result.Rows[0][0].ToString();
                }
            }
            catch (Exception ex)
            {
                txtPurchaseNumber.Text = "PO" + DateTime.Now.ToString("yyyyMMdd") + "0001";
            }
        }

        private void SetupEventHandlers()
        {
            cmbSupplier.SelectedIndexChanged += CmbSupplier_SelectedIndexChanged;
            cmbItem.SelectedIndexChanged += CmbItem_SelectedIndexChanged;
            txtBarcode.KeyDown += TxtBarcode_KeyDown;
            txtQuantity.TextChanged += CalculateItemTotal;
            txtFreeQuantity.TextChanged += CalculateItemTotal;
            txtUnitPrice.TextChanged += CalculateItemTotal;
            txtDiscount.TextChanged += CalculateItemTotal;
            txtDiscountPercent.TextChanged += CalculateItemTotal;
            txtGSTRate.TextChanged += CalculateItemTotal;
            btnAddItem.Click += BtnAddItem_Click;
            btnClearItem.Click += BtnClearItem_Click;
            btnScanBarcode.Click += BtnScanBarcode_Click;
            dgvPurchaseItems.UserDeletingRow += DgvPurchaseItems_UserDeletingRow;
        }

        private void SetDefaultValues()
        {
            dtpPurchaseDate.Value = DateTime.Now;
            dtpInvoiceDate.Value = DateTime.Now;
            dtpMfgDate.Value = DateTime.Now;
            dtpExpiryDate.Value = DateTime.Now.AddYears(2);
            txtGSTRate.Text = "12.00";
            txtFreeQuantity.Text = "0";
            lblBarcodeStatus.Text = "Ready to scan";
            lblBarcodeStatus.ForeColor = Color.Gray;
        }

        private void CmbSupplier_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSupplier.SelectedValue != null)
            {
                selectedCompanyID = Convert.ToInt32(cmbSupplier.SelectedValue);
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
                string query = @"SELECT ItemName, Barcode, PurchasePrice, SalePrice, MRP, GST_Rate 
                               FROM Items WHERE ItemID = @ItemID";
                SqlParameter[] parameters = { new SqlParameter("@ItemID", itemID) };
                DataTable result = DatabaseConnection.ExecuteQuery(query, parameters);
                
                if (result.Rows.Count > 0)
                {
                    DataRow row = result.Rows[0];
                    txtBarcode.Text = row["Barcode"].ToString();
                    txtUnitPrice.Text = row["PurchasePrice"].ToString();
                    txtMRP.Text = row["MRP"].ToString();
                    txtGSTRate.Text = row["GST_Rate"].ToString();
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
                SearchItemByBarcode();
            }
        }

        private void SearchItemByBarcode()
        {
            if (!string.IsNullOrEmpty(txtBarcode.Text))
            {
                try
                {
                    string query = @"
                        EXEC sp_SearchItemByBarcode @Barcode";
                    SqlParameter[] parameters = { new SqlParameter("@Barcode", txtBarcode.Text.Trim()) };
                    DataTable result = DatabaseConnection.ExecuteQuery(query, parameters);
                    
                    if (result.Rows.Count > 0)
                    {
                        int itemID = Convert.ToInt32(result.Rows[0]["ItemID"]);
                        cmbItem.SelectedValue = itemID;
                        
                        // Auto-fill price information
                        if (result.Rows[0]["PurchasePrice"] != DBNull.Value)
                        {
                            txtUnitPrice.Text = Convert.ToDecimal(result.Rows[0]["PurchasePrice"]).ToString("N2");
                        }
                        
                        // Show success feedback
                        lblBarcodeStatus.Text = "✓ Item Found: " + result.Rows[0]["ItemName"].ToString();
                        lblBarcodeStatus.ForeColor = Color.Green;
                        System.Media.SystemSounds.Beep.Play();
                        
                        txtQuantity.Focus();
                        txtQuantity.SelectAll();
                    }
                    else
                    {
                        lblBarcodeStatus.Text = "✗ Item Not Found";
                        lblBarcodeStatus.ForeColor = Color.Red;
                        System.Media.SystemSounds.Hand.Play();
                        
                        // Offer to create new item
                        DialogResult result2 = MessageBox.Show(
                            $"Item with barcode '{txtBarcode.Text}' not found.\n\nWould you like to add this as a new item?", 
                            "Item Not Found", 
                            MessageBoxButtons.YesNo, 
                            MessageBoxIcon.Question);
                        
                        if (result2 == DialogResult.Yes)
                        {
                            // Open Items form for new item creation
                            MessageBox.Show("New Item creation form will be opened.", "Information", 
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                catch (Exception ex)
                {
                    lblBarcodeStatus.Text = "✗ Scan Error";
                    lblBarcodeStatus.ForeColor = Color.Red;
                    MessageBox.Show("Error searching item: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnScanBarcode_Click(object sender, EventArgs e)
        {
            // Enhanced barcode scanner integration
            BarcodeScannerDialog scannerDialog = new BarcodeScannerDialog();
            if (scannerDialog.ShowDialog() == DialogResult.OK)
            {
                txtBarcode.Text = scannerDialog.ScannedBarcode;
                SearchItemByBarcode();
            }
            
            // Focus back to barcode textbox for manual entry
            txtBarcode.Focus();
            txtBarcode.SelectAll();
        }

        private void CalculateItemTotal(object sender, EventArgs e)
        {
            try
            {
                decimal quantity = string.IsNullOrEmpty(txtQuantity.Text) ? 0 : decimal.Parse(txtQuantity.Text);
                decimal unitPrice = string.IsNullOrEmpty(txtUnitPrice.Text) ? 0 : decimal.Parse(txtUnitPrice.Text);
                decimal discount = string.IsNullOrEmpty(txtDiscount.Text) ? 0 : decimal.Parse(txtDiscount.Text);
                decimal discountPercent = string.IsNullOrEmpty(txtDiscountPercent.Text) ? 0 : decimal.Parse(txtDiscountPercent.Text);
                decimal gstRate = string.IsNullOrEmpty(txtGSTRate.Text) ? 0 : decimal.Parse(txtGSTRate.Text);
                
                decimal grossAmount = quantity * unitPrice;
                decimal discountAmount = discount + (grossAmount * discountPercent / 100);
                decimal taxableAmount = grossAmount - discountAmount;
                decimal taxAmount = taxableAmount * gstRate / 100;
                decimal totalAmount = taxableAmount + taxAmount;
                
                txtTotalAmount.Text = totalAmount.ToString("N2");
            }
            catch
            {
                txtTotalAmount.Text = "0.00";
            }
        }

        private void BtnAddItem_Click(object sender, EventArgs e)
        {
            if (ValidateItemEntry())
            {
                AddItemToPurchase();
                CalculatePurchaseTotals();
                ClearItemEntry();
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
            
            if (string.IsNullOrEmpty(txtUnitPrice.Text) || !decimal.TryParse(txtUnitPrice.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Please enter a valid unit price.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            
            return true;
        }

        private void AddItemToPurchase()
        {
            DataRow newRow = purchaseItems.NewRow();
            
            newRow["ItemID"] = cmbItem.SelectedValue;
            newRow["ItemName"] = cmbItem.Text;
            newRow["BatchNumber"] = txtBatchNumber.Text;
            newRow["MfgDate"] = dtpMfgDate.Value;
            newRow["ExpiryDate"] = dtpExpiryDate.Value;
            newRow["Quantity"] = int.Parse(txtQuantity.Text);
            newRow["FreeQuantity"] = string.IsNullOrEmpty(txtFreeQuantity.Text) ? 0 : int.Parse(txtFreeQuantity.Text);
            newRow["UnitPrice"] = decimal.Parse(txtUnitPrice.Text);
            newRow["MRP"] = string.IsNullOrEmpty(txtMRP.Text) ? 0 : decimal.Parse(txtMRP.Text);
            newRow["Discount"] = string.IsNullOrEmpty(txtDiscount.Text) ? 0 : decimal.Parse(txtDiscount.Text);
            newRow["DiscountPercent"] = string.IsNullOrEmpty(txtDiscountPercent.Text) ? 0 : decimal.Parse(txtDiscountPercent.Text);
            newRow["GST_Rate"] = string.IsNullOrEmpty(txtGSTRate.Text) ? 0 : decimal.Parse(txtGSTRate.Text);
            
            // Calculate amounts
            decimal quantity = int.Parse(txtQuantity.Text);
            decimal unitPrice = decimal.Parse(txtUnitPrice.Text);
            decimal discount = string.IsNullOrEmpty(txtDiscount.Text) ? 0 : decimal.Parse(txtDiscount.Text);
            decimal discountPercent = string.IsNullOrEmpty(txtDiscountPercent.Text) ? 0 : decimal.Parse(txtDiscountPercent.Text);
            decimal gstRate = string.IsNullOrEmpty(txtGSTRate.Text) ? 0 : decimal.Parse(txtGSTRate.Text);
            
            decimal grossAmount = quantity * unitPrice;
            decimal discountAmount = discount + (grossAmount * discountPercent / 100);
            decimal taxableAmount = grossAmount - discountAmount;
            decimal cgst = taxableAmount * (gstRate / 2) / 100;
            decimal sgst = taxableAmount * (gstRate / 2) / 100;
            decimal igst = 0; // For inter-state transactions
            decimal totalAmount = taxableAmount + cgst + sgst + igst;
            
            newRow["TaxableAmount"] = taxableAmount;
            newRow["CGST"] = cgst;
            newRow["SGST"] = sgst;
            newRow["IGST"] = igst;
            newRow["TotalAmount"] = totalAmount;
            
            purchaseItems.Rows.Add(newRow);
        }

        private void CalculatePurchaseTotals()
        {
            totalGrossAmount = 0;
            totalDiscount = 0;
            totalTax = 0;
            totalNetAmount = 0;
            
            decimal totalTaxableAmount = 0;
            decimal totalCGST = 0;
            decimal totalSGST = 0;
            decimal totalIGST = 0;
            
            foreach (DataRow row in purchaseItems.Rows)
            {
                decimal quantity = Convert.ToDecimal(row["Quantity"]);
                decimal unitPrice = Convert.ToDecimal(row["UnitPrice"]);
                decimal discount = Convert.ToDecimal(row["Discount"]);
                decimal discountPercent = Convert.ToDecimal(row["DiscountPercent"]);
                
                decimal grossAmount = quantity * unitPrice;
                decimal itemDiscount = discount + (grossAmount * discountPercent / 100);
                
                totalGrossAmount += grossAmount;
                totalDiscount += itemDiscount;
                totalTaxableAmount += Convert.ToDecimal(row["TaxableAmount"]);
                totalCGST += Convert.ToDecimal(row["CGST"]);
                totalSGST += Convert.ToDecimal(row["SGST"]);
                totalIGST += Convert.ToDecimal(row["IGST"]);
                totalNetAmount += Convert.ToDecimal(row["TotalAmount"]);
            }
            
            totalTax = totalCGST + totalSGST + totalIGST;
            
            // Update summary controls
            txtGrossAmount.Text = totalGrossAmount.ToString("N2");
            txtTotalDiscount.Text = totalDiscount.ToString("N2");
            txtTaxableAmount.Text = totalTaxableAmount.ToString("N2");
            txtCGST.Text = totalCGST.ToString("N2");
            txtSGST.Text = totalSGST.ToString("N2");
            txtIGST.Text = totalIGST.ToString("N2");
            txtTotalTax.Text = totalTax.ToString("N2");
            
            // Round off calculation
            decimal roundedAmount = Math.Round(totalNetAmount);
            decimal roundOff = roundedAmount - totalNetAmount;
            txtRoundOff.Text = roundOff.ToString("N2");
            txtNetAmount.Text = roundedAmount.ToString("N2");
            
            totalNetAmount = roundedAmount;
        }

        private void ClearItemEntry()
        {
            cmbItem.SelectedIndex = -1;
            txtBarcode.Text = "";
            txtBatchNumber.Text = "";
            dtpMfgDate.Value = DateTime.Now;
            dtpExpiryDate.Value = DateTime.Now.AddYears(2);
            txtQuantity.Text = "";
            txtFreeQuantity.Text = "0";
            txtUnitPrice.Text = "";
            txtMRP.Text = "";
            txtDiscount.Text = "";
            txtDiscountPercent.Text = "";
            txtGSTRate.Text = "12.00";
            txtTotalAmount.Text = "";
        }

        private void BtnClearItem_Click(object sender, EventArgs e)
        {
            ClearItemEntry();
        }

        private void DgvPurchaseItems_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete this item?", "Confirm Delete",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                // Recalculate totals after deletion
                this.BeginInvoke(new Action(() => CalculatePurchaseTotals()));
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (ValidatePurchase())
            {
                try
                {
                    SavePurchase();
                    MessageBox.Show("Purchase saved successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    if (!isEditMode)
                    {
                        ClearForm();
                        GeneratePurchaseNumber();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error saving purchase: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private bool ValidatePurchase()
        {
            if (selectedCompanyID == 0)
            {
                MessageBox.Show("Please select a supplier.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            
            if (purchaseItems.Rows.Count == 0)
            {
                MessageBox.Show("Please add at least one item.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            
            return true;
        }

        private void SavePurchase()
        {
            using (SqlConnection connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Save purchase header
                        string purchaseQuery = @"INSERT INTO Purchases (
                            PurchaseNumber, InvoiceNumber, CompanyID, UserID, PurchaseDate, InvoiceDate,
                            GrossAmount, TotalDiscount, TaxableAmount, CGST, SGST, IGST, TotalTax, 
                            NetAmount, RoundOffAmount, IsActive, CreatedDate
                        ) VALUES (
                            @PurchaseNumber, @InvoiceNumber, @CompanyID, @UserID, @PurchaseDate, @InvoiceDate,
                            @GrossAmount, @TotalDiscount, @TaxableAmount, @CGST, @SGST, @IGST, @TotalTax,
                            @NetAmount, @RoundOffAmount, 1, @CreatedDate
                        ); SELECT SCOPE_IDENTITY();";
                        
                        SqlCommand purchaseCmd = new SqlCommand(purchaseQuery, connection, transaction);
                        purchaseCmd.Parameters.AddRange(new SqlParameter[] {
                            new SqlParameter("@PurchaseNumber", txtPurchaseNumber.Text),
                            new SqlParameter("@InvoiceNumber", txtInvoiceNumber.Text ?? ""),
                            new SqlParameter("@CompanyID", selectedCompanyID),
                            new SqlParameter("@UserID", currentUserID),
                            new SqlParameter("@PurchaseDate", dtpPurchaseDate.Value),
                            new SqlParameter("@InvoiceDate", dtpInvoiceDate.Value),
                            new SqlParameter("@GrossAmount", totalGrossAmount),
                            new SqlParameter("@TotalDiscount", totalDiscount),
                            new SqlParameter("@TaxableAmount", decimal.Parse(txtTaxableAmount.Text)),
                            new SqlParameter("@CGST", decimal.Parse(txtCGST.Text)),
                            new SqlParameter("@SGST", decimal.Parse(txtSGST.Text)),
                            new SqlParameter("@IGST", decimal.Parse(txtIGST.Text)),
                            new SqlParameter("@TotalTax", totalTax),
                            new SqlParameter("@NetAmount", totalNetAmount),
                            new SqlParameter("@RoundOffAmount", decimal.Parse(txtRoundOff.Text)),
                            new SqlParameter("@CreatedDate", DateTime.Now)
                        });
                        
                        int purchaseID = Convert.ToInt32(purchaseCmd.ExecuteScalar());
                        
                        // Save purchase items
                        foreach (DataRow row in purchaseItems.Rows)
                        {
                            SavePurchaseItem(connection, transaction, purchaseID, row);
                        }
                        
                        // Log activity
                        LogUserActivity(connection, transaction, "INSERT", "Purchases", 
                            $"Created purchase {txtPurchaseNumber.Text}");
                        
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private void SavePurchaseItem(SqlConnection connection, SqlTransaction transaction, int purchaseID, DataRow item)
        {
            // Insert purchase item
            string itemQuery = @"INSERT INTO PurchaseItems (
                PurchaseID, ItemID, BatchNumber, ManufacturingDate, ExpiryDate, Quantity, FreeQuantity,
                UnitPrice, MRP, Discount, DiscountPercent, TaxableAmount, GST_Rate, CGST, SGST, IGST, TotalAmount
            ) VALUES (
                @PurchaseID, @ItemID, @BatchNumber, @ManufacturingDate, @ExpiryDate, @Quantity, @FreeQuantity,
                @UnitPrice, @MRP, @Discount, @DiscountPercent, @TaxableAmount, @GST_Rate, @CGST, @SGST, @IGST, @TotalAmount
            )";
            
            SqlCommand itemCmd = new SqlCommand(itemQuery, connection, transaction);
            itemCmd.Parameters.AddRange(new SqlParameter[] {
                new SqlParameter("@PurchaseID", purchaseID),
                new SqlParameter("@ItemID", item["ItemID"]),
                new SqlParameter("@BatchNumber", item["BatchNumber"]),
                new SqlParameter("@ManufacturingDate", item["MfgDate"]),
                new SqlParameter("@ExpiryDate", item["ExpiryDate"]),
                new SqlParameter("@Quantity", item["Quantity"]),
                new SqlParameter("@FreeQuantity", item["FreeQuantity"]),
                new SqlParameter("@UnitPrice", item["UnitPrice"]),
                new SqlParameter("@MRP", item["MRP"]),
                new SqlParameter("@Discount", item["Discount"]),
                new SqlParameter("@DiscountPercent", item["DiscountPercent"]),
                new SqlParameter("@TaxableAmount", item["TaxableAmount"]),
                new SqlParameter("@GST_Rate", item["GST_Rate"]),
                new SqlParameter("@CGST", item["CGST"]),
                new SqlParameter("@SGST", item["SGST"]),
                new SqlParameter("@IGST", item["IGST"]),
                new SqlParameter("@TotalAmount", item["TotalAmount"])
            });
            
            itemCmd.ExecuteNonQuery();
            
            // The trigger will automatically create/update the item batch and update stock
        }

        private void LogUserActivity(SqlConnection connection, SqlTransaction transaction, string action, string module, string description)
        {
            try
            {
                string query = @"INSERT INTO UserActivityLog (UserID, Activity, ModuleName, Description, LogDate) 
                               VALUES (@UserID, @Activity, @ModuleName, @Description, @LogDate)";
                
                SqlCommand cmd = new SqlCommand(query, connection, transaction);
                cmd.Parameters.AddRange(new SqlParameter[] {
                    new SqlParameter("@UserID", currentUserID),
                    new SqlParameter("@Activity", action),
                    new SqlParameter("@ModuleName", module),
                    new SqlParameter("@Description", description),
                    new SqlParameter("@LogDate", DateTime.Now)
                });
                
                cmd.ExecuteNonQuery();
            }
            catch
            {
                // Log errors silently
            }
        }

        private void ClearForm()
        {
            purchaseItems.Rows.Clear();
            cmbSupplier.SelectedIndex = -1;
            txtInvoiceNumber.Text = "";
            dtpPurchaseDate.Value = DateTime.Now;
            dtpInvoiceDate.Value = DateTime.Now;
            selectedCompanyID = 0;
            
            // Clear summary
            txtGrossAmount.Text = "0.00";
            txtTotalDiscount.Text = "0.00";
            txtTaxableAmount.Text = "0.00";
            txtCGST.Text = "0.00";
            txtSGST.Text = "0.00";
            txtIGST.Text = "0.00";
            txtTotalTax.Text = "0.00";
            txtNetAmount.Text = "0.00";
            txtRoundOff.Text = "0.00";
            
            ClearItemEntry();
        }

        private void LoadPurchaseForEdit()
        {
            // Implementation for loading existing purchase for editing
            // This would be used when editing an existing purchase
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Print functionality will be implemented with reporting engine.", "Information",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to cancel? All unsaved changes will be lost.", 
                "Confirm Cancel", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                ClearForm();
                GeneratePurchaseNumber();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
