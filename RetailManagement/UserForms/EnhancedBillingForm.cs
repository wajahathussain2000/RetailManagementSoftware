using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RetailManagement.Database;
using RetailManagement.Models;

namespace RetailManagement.UserForms
{
    public partial class EnhancedBillingForm : Form
    {
        private DataTable billItems;
        private decimal grandTotal = 0;
        private decimal totalDiscount = 0;
        private decimal totalTax = 0;
        private decimal netAmount = 0;

        // Form Controls
        private Panel pnlTop;
        private Panel pnlMain;
        private Panel pnlBottom;
        private GroupBox gbBillHeader;
        private GroupBox gbItemEntry;
        private GroupBox gbBillItems;
        private GroupBox gbBillTotals;
        private GroupBox gbPayment;

        // Bill Header Controls
        private TextBox txtBillNumber;
        private DateTimePicker dtpBillDate;
        private ComboBox cmbCustomer;
        private TextBox txtCustomerPhone;
        private TextBox txtCustomerAddress; // TODO: Implement customer address functionality

        // Item Entry Controls
        private TextBox txtBarcode;
        private Button btnScanBarcode;
        private ComboBox cmbItem;
        private TextBox txtItemName; // TODO: Implement item name display
        private TextBox txtQuantity;
        private TextBox txtUnitPrice;
        private TextBox txtMRP;
        private TextBox txtDiscount;
        private TextBox txtDiscountPercent; // TODO: Implement percentage discount
        private TextBox txtTaxRate;
        private TextBox txtItemTotal;
        private Button btnAddItem;
        private Button btnClearItem;
        private Label lblScanStatus;

        // Bill Items Grid
        private DataGridView dgvBillItems;

        // Totals Controls
        private Label lblTotalItems;
        private Label lblTotalQuantity;
        private Label lblSubTotal;
        private Label lblTotalDiscount;
        private Label lblTotalTax;
        private Label lblGrandTotal;

        // Payment Controls
        private ComboBox cmbPaymentMethod;
        private TextBox txtPaidAmount;
        private TextBox txtChangeAmount;
        private CheckBox chkIsCreditSale;
        private TextBox txtRemarks;

        // Action Buttons
        private Button btnSave;
        private Button btnPrint;
        private Button btnClear;
        private Button btnClose;

        public EnhancedBillingForm()
        {
            InitializeComponent();
            InitializeCustomComponents();
            InitializeBillItems();
            LoadCustomers();
            LoadItems();
            GenerateBillNumber();
            SetDefaultValues();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form
            this.Text = "Enhanced Billing System";
            this.Size = new Size(1400, 900);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Panels
            this.pnlTop = new Panel();
            this.pnlMain = new Panel();
            this.pnlBottom = new Panel();

            this.pnlTop.Dock = DockStyle.Top;
            this.pnlTop.Height = 80;
            this.pnlTop.BackColor = Color.FromArgb(0, 122, 204);

            this.pnlMain.Dock = DockStyle.Fill;
            this.pnlMain.Padding = new Padding(10);

            this.pnlBottom.Dock = DockStyle.Bottom;
            this.pnlBottom.Height = 70;
            this.pnlBottom.BackColor = Color.FromArgb(240, 240, 240);

            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.pnlBottom);
            this.Controls.Add(this.pnlTop);

            // Title
            Label lblTitle = new Label();
            lblTitle.Text = "🧾 Enhanced Billing System";
            lblTitle.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(20, 20);
            lblTitle.AutoSize = true;

            Label lblSubtitle = new Label();
            lblSubtitle.Text = "Professional Point of Sale System with Barcode Support";
            lblSubtitle.Font = new Font("Segoe UI", 10);
            lblSubtitle.ForeColor = Color.LightGray;
            lblSubtitle.Location = new Point(20, 50);
            lblSubtitle.AutoSize = true;

            this.pnlTop.Controls.Add(lblTitle);
            this.pnlTop.Controls.Add(lblSubtitle);

            this.ResumeLayout(false);
        }

        private void InitializeCustomComponents()
        {
            // Group Boxes
            this.gbBillHeader = new GroupBox();
            this.gbItemEntry = new GroupBox();
            this.gbBillItems = new GroupBox();
            this.gbBillTotals = new GroupBox();
            this.gbPayment = new GroupBox();

            this.gbBillHeader.Text = "📋 Bill Information";
            this.gbItemEntry.Text = "🔍 Item Entry & Barcode Scanning";
            this.gbBillItems.Text = "🛒 Bill Items";
            this.gbBillTotals.Text = "💰 Bill Totals";
            this.gbPayment.Text = "💳 Payment Information";

            this.gbBillHeader.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            this.gbItemEntry.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            this.gbBillItems.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            this.gbBillTotals.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            this.gbPayment.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            // Layout
            this.gbBillHeader.Location = new Point(10, 10);
            this.gbBillHeader.Size = new Size(450, 120);

            this.gbItemEntry.Location = new Point(470, 10);
            this.gbItemEntry.Size = new Size(870, 120);

            this.gbBillItems.Location = new Point(10, 140);
            this.gbBillItems.Size = new Size(1070, 350);

            this.gbBillTotals.Location = new Point(1090, 140);
            this.gbBillTotals.Size = new Size(250, 350);

            this.gbPayment.Location = new Point(10, 500);
            this.gbPayment.Size = new Size(1330, 100);

            this.pnlMain.Controls.Add(this.gbBillHeader);
            this.pnlMain.Controls.Add(this.gbItemEntry);
            this.pnlMain.Controls.Add(this.gbBillItems);
            this.pnlMain.Controls.Add(this.gbBillTotals);
            this.pnlMain.Controls.Add(this.gbPayment);

            // Initialize Controls
            InitializeBillHeaderControls();
            InitializeItemEntryControls();
            InitializeBillItemsGrid();
            InitializeTotalsControls();
            InitializePaymentControls();
            InitializeActionButtons();
        }

        private void InitializeBillHeaderControls()
        {
            // Bill Number
            Label lblBillNumber = new Label();
            lblBillNumber.Text = "Bill Number:";
            lblBillNumber.Location = new Point(15, 30);
            lblBillNumber.Size = new Size(80, 20);

            this.txtBillNumber = new TextBox();
            this.txtBillNumber.Location = new Point(100, 30);
            this.txtBillNumber.Size = new Size(120, 25);
            this.txtBillNumber.ReadOnly = true;
            this.txtBillNumber.BackColor = Color.LightGray;

            // Bill Date
            Label lblBillDate = new Label();
            lblBillDate.Text = "Bill Date:";
            lblBillDate.Location = new Point(240, 30);
            lblBillDate.Size = new Size(60, 20);

            this.dtpBillDate = new DateTimePicker();
            this.dtpBillDate.Location = new Point(310, 30);
            this.dtpBillDate.Size = new Size(120, 25);
            this.dtpBillDate.Format = DateTimePickerFormat.Short;

            // Customer
            Label lblCustomer = new Label();
            lblCustomer.Text = "Customer:";
            lblCustomer.Location = new Point(15, 65);
            lblCustomer.Size = new Size(70, 20);

            this.cmbCustomer = new ComboBox();
            this.cmbCustomer.Location = new Point(90, 65);
            this.cmbCustomer.Size = new Size(200, 25);
            this.cmbCustomer.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbCustomer.SelectedIndexChanged += CmbCustomer_SelectedIndexChanged;

            // Customer Phone
            Label lblPhone = new Label();
            lblPhone.Text = "Phone:";
            lblPhone.Location = new Point(300, 65);
            lblPhone.Size = new Size(50, 20);

            this.txtCustomerPhone = new TextBox();
            this.txtCustomerPhone.Location = new Point(350, 65);
            this.txtCustomerPhone.Size = new Size(80, 25);
            this.txtCustomerPhone.ReadOnly = true;

            this.gbBillHeader.Controls.Add(lblBillNumber);
            this.gbBillHeader.Controls.Add(this.txtBillNumber);
            this.gbBillHeader.Controls.Add(lblBillDate);
            this.gbBillHeader.Controls.Add(this.dtpBillDate);
            this.gbBillHeader.Controls.Add(lblCustomer);
            this.gbBillHeader.Controls.Add(this.cmbCustomer);
            this.gbBillHeader.Controls.Add(lblPhone);
            this.gbBillHeader.Controls.Add(this.txtCustomerPhone);
        }

        private void InitializeItemEntryControls()
        {
            // Barcode
            Label lblBarcode = new Label();
            lblBarcode.Text = "📱 Barcode:";
            lblBarcode.Location = new Point(15, 30);
            lblBarcode.Size = new Size(70, 20);
            lblBarcode.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            this.txtBarcode = new TextBox();
            this.txtBarcode.Location = new Point(90, 30);
            this.txtBarcode.Size = new Size(120, 25);
            this.txtBarcode.Font = new Font("Consolas", 10);
            this.txtBarcode.KeyDown += TxtBarcode_KeyDown;

            this.btnScanBarcode = new Button();
            this.btnScanBarcode.Text = "🔍 Scan";
            this.btnScanBarcode.Location = new Point(220, 28);
            this.btnScanBarcode.Size = new Size(60, 28);
            this.btnScanBarcode.BackColor = Color.FromArgb(40, 167, 69);
            this.btnScanBarcode.ForeColor = Color.White;
            this.btnScanBarcode.FlatStyle = FlatStyle.Flat;
            this.btnScanBarcode.Click += BtnScanBarcode_Click;

            // Scan Status
            this.lblScanStatus = new Label();
            this.lblScanStatus.Text = "Ready to scan";
            this.lblScanStatus.Location = new Point(290, 32);
            this.lblScanStatus.Size = new Size(100, 20);
            this.lblScanStatus.ForeColor = Color.Gray;
            this.lblScanStatus.Font = new Font("Segoe UI", 8);

            // Item Selection
            Label lblItem = new Label();
            lblItem.Text = "Item:";
            lblItem.Location = new Point(15, 65);
            lblItem.Size = new Size(40, 20);

            this.cmbItem = new ComboBox();
            this.cmbItem.Location = new Point(60, 65);
            this.cmbItem.Size = new Size(200, 25);
            this.cmbItem.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbItem.SelectedIndexChanged += CmbItem_SelectedIndexChanged;

            // Quantity
            Label lblQuantity = new Label();
            lblQuantity.Text = "Qty:";
            lblQuantity.Location = new Point(270, 65);
            lblQuantity.Size = new Size(30, 20);

            this.txtQuantity = new TextBox();
            this.txtQuantity.Location = new Point(305, 65);
            this.txtQuantity.Size = new Size(60, 25);
            this.txtQuantity.Text = "1";
            this.txtQuantity.TextAlign = HorizontalAlignment.Right;
            this.txtQuantity.TextChanged += CalculateItemTotal;

            // Unit Price
            Label lblUnitPrice = new Label();
            lblUnitPrice.Text = "Price:";
            lblUnitPrice.Location = new Point(375, 65);
            lblUnitPrice.Size = new Size(40, 20);

            this.txtUnitPrice = new TextBox();
            this.txtUnitPrice.Location = new Point(420, 65);
            this.txtUnitPrice.Size = new Size(70, 25);
            this.txtUnitPrice.TextAlign = HorizontalAlignment.Right;
            this.txtUnitPrice.TextChanged += CalculateItemTotal;

            // MRP
            Label lblMRP = new Label();
            lblMRP.Text = "MRP:";
            lblMRP.Location = new Point(500, 65);
            lblMRP.Size = new Size(35, 20);

            this.txtMRP = new TextBox();
            this.txtMRP.Location = new Point(540, 65);
            this.txtMRP.Size = new Size(70, 25);
            this.txtMRP.TextAlign = HorizontalAlignment.Right;
            this.txtMRP.ReadOnly = true;
            this.txtMRP.BackColor = Color.LightGray;

            // Discount
            Label lblDiscount = new Label();
            lblDiscount.Text = "Disc:";
            lblDiscount.Location = new Point(620, 65);
            lblDiscount.Size = new Size(35, 20);

            this.txtDiscount = new TextBox();
            this.txtDiscount.Location = new Point(660, 65);
            this.txtDiscount.Size = new Size(50, 25);
            this.txtDiscount.Text = "0";
            this.txtDiscount.TextAlign = HorizontalAlignment.Right;
            this.txtDiscount.TextChanged += CalculateItemTotal;

            // Tax Rate
            Label lblTaxRate = new Label();
            lblTaxRate.Text = "Tax%:";
            lblTaxRate.Location = new Point(720, 65);
            lblTaxRate.Size = new Size(35, 20);

            this.txtTaxRate = new TextBox();
            this.txtTaxRate.Location = new Point(760, 65);
            this.txtTaxRate.Size = new Size(40, 25);
            this.txtTaxRate.Text = "18";
            this.txtTaxRate.TextAlign = HorizontalAlignment.Right;
            this.txtTaxRate.TextChanged += CalculateItemTotal;

            // Item Total
            Label lblItemTotal = new Label();
            lblItemTotal.Text = "Total:";
            lblItemTotal.Location = new Point(810, 65);
            lblItemTotal.Size = new Size(40, 20);

            this.txtItemTotal = new TextBox();
            this.txtItemTotal.Location = new Point(855, 65);
            this.txtItemTotal.Size = new Size(80, 25);
            this.txtItemTotal.TextAlign = HorizontalAlignment.Right;
            this.txtItemTotal.ReadOnly = true;
            this.txtItemTotal.BackColor = Color.LightBlue;
            this.txtItemTotal.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            // Action Buttons
            this.btnAddItem = new Button();
            this.btnAddItem.Text = "➕ Add";
            this.btnAddItem.Location = new Point(750, 90);
            this.btnAddItem.Size = new Size(80, 28);
            this.btnAddItem.BackColor = Color.FromArgb(0, 123, 255);
            this.btnAddItem.ForeColor = Color.White;
            this.btnAddItem.FlatStyle = FlatStyle.Flat;
            this.btnAddItem.Click += BtnAddItem_Click;

            this.btnClearItem = new Button();
            this.btnClearItem.Text = "🗑️ Clear";
            this.btnClearItem.Location = new Point(840, 90);
            this.btnClearItem.Size = new Size(80, 28);
            this.btnClearItem.BackColor = Color.FromArgb(108, 117, 125);
            this.btnClearItem.ForeColor = Color.White;
            this.btnClearItem.FlatStyle = FlatStyle.Flat;
            this.btnClearItem.Click += BtnClearItem_Click;

            this.gbItemEntry.Controls.Add(lblBarcode);
            this.gbItemEntry.Controls.Add(this.txtBarcode);
            this.gbItemEntry.Controls.Add(this.btnScanBarcode);
            this.gbItemEntry.Controls.Add(this.lblScanStatus);
            this.gbItemEntry.Controls.Add(lblItem);
            this.gbItemEntry.Controls.Add(this.cmbItem);
            this.gbItemEntry.Controls.Add(lblQuantity);
            this.gbItemEntry.Controls.Add(this.txtQuantity);
            this.gbItemEntry.Controls.Add(lblUnitPrice);
            this.gbItemEntry.Controls.Add(this.txtUnitPrice);
            this.gbItemEntry.Controls.Add(lblMRP);
            this.gbItemEntry.Controls.Add(this.txtMRP);
            this.gbItemEntry.Controls.Add(lblDiscount);
            this.gbItemEntry.Controls.Add(this.txtDiscount);
            this.gbItemEntry.Controls.Add(lblTaxRate);
            this.gbItemEntry.Controls.Add(this.txtTaxRate);
            this.gbItemEntry.Controls.Add(lblItemTotal);
            this.gbItemEntry.Controls.Add(this.txtItemTotal);
            this.gbItemEntry.Controls.Add(this.btnAddItem);
            this.gbItemEntry.Controls.Add(this.btnClearItem);
        }

        private void InitializeBillItems()
        {
            this.billItems = new DataTable();
            billItems.Columns.Add("ItemID", typeof(int));
            billItems.Columns.Add("ItemName", typeof(string));
            billItems.Columns.Add("Barcode", typeof(string));
            billItems.Columns.Add("Quantity", typeof(decimal));
            billItems.Columns.Add("UnitPrice", typeof(decimal));
            billItems.Columns.Add("MRP", typeof(decimal));
            billItems.Columns.Add("Discount", typeof(decimal));
            billItems.Columns.Add("TaxRate", typeof(decimal));
            billItems.Columns.Add("TaxAmount", typeof(decimal));
            billItems.Columns.Add("Total", typeof(decimal));
        }

        private void InitializeBillItemsGrid()
        {
            this.dgvBillItems = new DataGridView();
            this.dgvBillItems.Location = new Point(15, 25);
            this.dgvBillItems.Size = new Size(1040, 310);
            this.dgvBillItems.AllowUserToAddRows = false;
            this.dgvBillItems.AllowUserToDeleteRows = true;
            this.dgvBillItems.ReadOnly = true;
            this.dgvBillItems.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvBillItems.MultiSelect = false;
            this.dgvBillItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvBillItems.DataSource = this.billItems;
            this.dgvBillItems.UserDeletingRow += DgvBillItems_UserDeletingRow;

            this.gbBillItems.Controls.Add(this.dgvBillItems);

            // Format columns
            FormatBillItemsGrid();
        }

        private void FormatBillItemsGrid()
        {
            if (dgvBillItems.Columns.Count > 0)
            {
                dgvBillItems.Columns["ItemID"].Visible = false;
                dgvBillItems.Columns["ItemName"].HeaderText = "Item Name";
                dgvBillItems.Columns["Barcode"].HeaderText = "Barcode";
                dgvBillItems.Columns["Quantity"].HeaderText = "Qty";
                dgvBillItems.Columns["UnitPrice"].HeaderText = "Unit Price";
                dgvBillItems.Columns["MRP"].HeaderText = "MRP";
                dgvBillItems.Columns["Discount"].HeaderText = "Discount";
                dgvBillItems.Columns["TaxRate"].HeaderText = "Tax %";
                dgvBillItems.Columns["TaxAmount"].HeaderText = "Tax Amount";
                dgvBillItems.Columns["Total"].HeaderText = "Total";

                // Format currency columns
                dgvBillItems.Columns["UnitPrice"].DefaultCellStyle.Format = "N2";
                dgvBillItems.Columns["MRP"].DefaultCellStyle.Format = "N2";
                dgvBillItems.Columns["Discount"].DefaultCellStyle.Format = "N2";
                dgvBillItems.Columns["TaxAmount"].DefaultCellStyle.Format = "N2";
                dgvBillItems.Columns["Total"].DefaultCellStyle.Format = "N2";

                // Set widths
                dgvBillItems.Columns["ItemName"].Width = 300;
                dgvBillItems.Columns["Barcode"].Width = 120;
                dgvBillItems.Columns["Quantity"].Width = 60;
            }
        }

        private void InitializeTotalsControls()
        {
            int yPos = 30;
            int spacing = 40;

            // Total Items
            Label lblTotalItemsLabel = new Label();
            lblTotalItemsLabel.Text = "Total Items:";
            lblTotalItemsLabel.Location = new Point(15, yPos);
            lblTotalItemsLabel.Size = new Size(80, 20);

            this.lblTotalItems = new Label();
            this.lblTotalItems.Text = "0";
            this.lblTotalItems.Location = new Point(150, yPos);
            this.lblTotalItems.Size = new Size(80, 20);
            this.lblTotalItems.TextAlign = ContentAlignment.MiddleRight;
            this.lblTotalItems.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            yPos += spacing;

            // Total Quantity
            Label lblTotalQuantityLabel = new Label();
            lblTotalQuantityLabel.Text = "Total Qty:";
            lblTotalQuantityLabel.Location = new Point(15, yPos);
            lblTotalQuantityLabel.Size = new Size(80, 20);

            this.lblTotalQuantity = new Label();
            this.lblTotalQuantity.Text = "0";
            this.lblTotalQuantity.Location = new Point(150, yPos);
            this.lblTotalQuantity.Size = new Size(80, 20);
            this.lblTotalQuantity.TextAlign = ContentAlignment.MiddleRight;
            this.lblTotalQuantity.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            yPos += spacing;

            // Sub Total
            Label lblSubTotalLabel = new Label();
            lblSubTotalLabel.Text = "Sub Total:";
            lblSubTotalLabel.Location = new Point(15, yPos);
            lblSubTotalLabel.Size = new Size(80, 20);

            this.lblSubTotal = new Label();
            this.lblSubTotal.Text = "0.00";
            this.lblSubTotal.Location = new Point(150, yPos);
            this.lblSubTotal.Size = new Size(80, 20);
            this.lblSubTotal.TextAlign = ContentAlignment.MiddleRight;
            this.lblSubTotal.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            yPos += spacing;

            // Total Discount
            Label lblTotalDiscountLabel = new Label();
            lblTotalDiscountLabel.Text = "Discount:";
            lblTotalDiscountLabel.Location = new Point(15, yPos);
            lblTotalDiscountLabel.Size = new Size(80, 20);

            this.lblTotalDiscount = new Label();
            this.lblTotalDiscount.Text = "0.00";
            this.lblTotalDiscount.Location = new Point(150, yPos);
            this.lblTotalDiscount.Size = new Size(80, 20);
            this.lblTotalDiscount.TextAlign = ContentAlignment.MiddleRight;
            this.lblTotalDiscount.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            this.lblTotalDiscount.ForeColor = Color.Red;

            yPos += spacing;

            // Total Tax
            Label lblTotalTaxLabel = new Label();
            lblTotalTaxLabel.Text = "Tax:";
            lblTotalTaxLabel.Location = new Point(15, yPos);
            lblTotalTaxLabel.Size = new Size(80, 20);

            this.lblTotalTax = new Label();
            this.lblTotalTax.Text = "0.00";
            this.lblTotalTax.Location = new Point(150, yPos);
            this.lblTotalTax.Size = new Size(80, 20);
            this.lblTotalTax.TextAlign = ContentAlignment.MiddleRight;
            this.lblTotalTax.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            this.lblTotalTax.ForeColor = Color.Orange;

            yPos += spacing;

            // Grand Total
            Label lblGrandTotalLabel = new Label();
            lblGrandTotalLabel.Text = "GRAND TOTAL:";
            lblGrandTotalLabel.Location = new Point(15, yPos);
            lblGrandTotalLabel.Size = new Size(100, 25);
            lblGrandTotalLabel.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblGrandTotalLabel.ForeColor = Color.DarkBlue;

            this.lblGrandTotal = new Label();
            this.lblGrandTotal.Text = "0.00";
            this.lblGrandTotal.Location = new Point(120, yPos);
            this.lblGrandTotal.Size = new Size(110, 25);
            this.lblGrandTotal.TextAlign = ContentAlignment.MiddleRight;
            this.lblGrandTotal.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            this.lblGrandTotal.ForeColor = Color.DarkBlue;
            this.lblGrandTotal.BorderStyle = BorderStyle.FixedSingle;
            this.lblGrandTotal.BackColor = Color.LightYellow;

            this.gbBillTotals.Controls.Add(lblTotalItemsLabel);
            this.gbBillTotals.Controls.Add(this.lblTotalItems);
            this.gbBillTotals.Controls.Add(lblTotalQuantityLabel);
            this.gbBillTotals.Controls.Add(this.lblTotalQuantity);
            this.gbBillTotals.Controls.Add(lblSubTotalLabel);
            this.gbBillTotals.Controls.Add(this.lblSubTotal);
            this.gbBillTotals.Controls.Add(lblTotalDiscountLabel);
            this.gbBillTotals.Controls.Add(this.lblTotalDiscount);
            this.gbBillTotals.Controls.Add(lblTotalTaxLabel);
            this.gbBillTotals.Controls.Add(this.lblTotalTax);
            this.gbBillTotals.Controls.Add(lblGrandTotalLabel);
            this.gbBillTotals.Controls.Add(this.lblGrandTotal);
        }

        private void InitializePaymentControls()
        {
            // Payment Method
            Label lblPaymentMethod = new Label();
            lblPaymentMethod.Text = "Payment Method:";
            lblPaymentMethod.Location = new Point(15, 30);
            lblPaymentMethod.Size = new Size(100, 20);

            this.cmbPaymentMethod = new ComboBox();
            this.cmbPaymentMethod.Location = new Point(120, 30);
            this.cmbPaymentMethod.Size = new Size(120, 25);
            this.cmbPaymentMethod.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbPaymentMethod.Items.AddRange(new string[] { "Cash", "Card", "UPI", "Bank Transfer", "Cheque" });
            this.cmbPaymentMethod.SelectedIndex = 0;

            // Paid Amount
            Label lblPaidAmount = new Label();
            lblPaidAmount.Text = "Paid Amount:";
            lblPaidAmount.Location = new Point(260, 30);
            lblPaidAmount.Size = new Size(80, 20);

            this.txtPaidAmount = new TextBox();
            this.txtPaidAmount.Location = new Point(350, 30);
            this.txtPaidAmount.Size = new Size(100, 25);
            this.txtPaidAmount.TextAlign = HorizontalAlignment.Right;
            this.txtPaidAmount.TextChanged += TxtPaidAmount_TextChanged;

            // Change Amount
            Label lblChangeAmount = new Label();
            lblChangeAmount.Text = "Change:";
            lblChangeAmount.Location = new Point(470, 30);
            lblChangeAmount.Size = new Size(50, 20);

            this.txtChangeAmount = new TextBox();
            this.txtChangeAmount.Location = new Point(530, 30);
            this.txtChangeAmount.Size = new Size(100, 25);
            this.txtChangeAmount.TextAlign = HorizontalAlignment.Right;
            this.txtChangeAmount.ReadOnly = true;
            this.txtChangeAmount.BackColor = Color.LightGray;

            // Credit Sale Checkbox
            this.chkIsCreditSale = new CheckBox();
            this.chkIsCreditSale.Text = "Credit Sale";
            this.chkIsCreditSale.Location = new Point(650, 30);
            this.chkIsCreditSale.Size = new Size(90, 25);
            this.chkIsCreditSale.CheckedChanged += ChkIsCreditSale_CheckedChanged;

            // Remarks
            Label lblRemarks = new Label();
            lblRemarks.Text = "Remarks:";
            lblRemarks.Location = new Point(15, 65);
            lblRemarks.Size = new Size(60, 20);

            this.txtRemarks = new TextBox();
            this.txtRemarks.Location = new Point(80, 65);
            this.txtRemarks.Size = new Size(400, 25);

            this.gbPayment.Controls.Add(lblPaymentMethod);
            this.gbPayment.Controls.Add(this.cmbPaymentMethod);
            this.gbPayment.Controls.Add(lblPaidAmount);
            this.gbPayment.Controls.Add(this.txtPaidAmount);
            this.gbPayment.Controls.Add(lblChangeAmount);
            this.gbPayment.Controls.Add(this.txtChangeAmount);
            this.gbPayment.Controls.Add(this.chkIsCreditSale);
            this.gbPayment.Controls.Add(lblRemarks);
            this.gbPayment.Controls.Add(this.txtRemarks);
        }

        private void InitializeActionButtons()
        {
            int buttonWidth = 120;
            int buttonHeight = 40;
            int buttonSpacing = 140;
            int startX = 50;
            int startY = 15;

            this.btnSave = new Button();
            this.btnSave.Text = "💾 Save Bill";
            this.btnSave.Location = new Point(startX, startY);
            this.btnSave.Size = new Size(buttonWidth, buttonHeight);
            this.btnSave.BackColor = Color.FromArgb(40, 167, 69);
            this.btnSave.ForeColor = Color.White;
            this.btnSave.FlatStyle = FlatStyle.Flat;
            this.btnSave.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            this.btnSave.Click += BtnSave_Click;

            this.btnPrint = new Button();
            this.btnPrint.Text = "🖨️ Print";
            this.btnPrint.Location = new Point(startX + buttonSpacing, startY);
            this.btnPrint.Size = new Size(buttonWidth, buttonHeight);
            this.btnPrint.BackColor = Color.FromArgb(0, 123, 255);
            this.btnPrint.ForeColor = Color.White;
            this.btnPrint.FlatStyle = FlatStyle.Flat;
            this.btnPrint.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            this.btnPrint.Click += BtnPrint_Click;

            this.btnClear = new Button();
            this.btnClear.Text = "🗑️ Clear All";
            this.btnClear.Location = new Point(startX + buttonSpacing * 2, startY);
            this.btnClear.Size = new Size(buttonWidth, buttonHeight);
            this.btnClear.BackColor = Color.FromArgb(255, 193, 7);
            this.btnClear.ForeColor = Color.Black;
            this.btnClear.FlatStyle = FlatStyle.Flat;
            this.btnClear.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            this.btnClear.Click += BtnClear_Click;

            this.btnClose = new Button();
            this.btnClose.Text = "🚪 Close";
            this.btnClose.Location = new Point(startX + buttonSpacing * 3, startY);
            this.btnClose.Size = new Size(buttonWidth, buttonHeight);
            this.btnClose.BackColor = Color.FromArgb(220, 53, 69);
            this.btnClose.ForeColor = Color.White;
            this.btnClose.FlatStyle = FlatStyle.Flat;
            this.btnClose.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            this.btnClose.Click += BtnClose_Click;

            this.pnlBottom.Controls.Add(this.btnSave);
            this.pnlBottom.Controls.Add(this.btnPrint);
            this.pnlBottom.Controls.Add(this.btnClear);
            this.pnlBottom.Controls.Add(this.btnClose);
        }

        private void LoadCustomers()
        {
            try
            {
                string query = "SELECT CustomerID, CustomerName FROM Customers WHERE IsActive = 1 ORDER BY CustomerName";
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

        private void GenerateBillNumber()
        {
            try
            {
                string query = "SELECT 'BILL' + RIGHT('00000' + CAST(ISNULL(MAX(CAST(SUBSTRING(BillNumber, 5, LEN(BillNumber)) AS INT)), 0) + 1 AS VARCHAR), 5) FROM Sales WHERE BillNumber LIKE 'BILL%'";
                object result = DatabaseConnection.ExecuteScalar(query);
                txtBillNumber.Text = result?.ToString() ?? "BILL00001";
            }
            catch (Exception ex)
            {
                txtBillNumber.Text = "BILL" + DateTime.Now.ToString("yyyyMMdd") + "0001";
            }
        }

        private void SetDefaultValues()
        {
            dtpBillDate.Value = DateTime.Now;
            txtQuantity.Text = "1";
            txtDiscount.Text = "0";
            txtTaxRate.Text = "18";
            txtPaidAmount.Text = "0.00";
            txtChangeAmount.Text = "0.00";
            lblScanStatus.Text = "Ready to scan";
            lblScanStatus.ForeColor = Color.Gray;
        }

        // Event Handlers
        private void CmbCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbCustomer.SelectedValue != null)
            {
                try
                {
                    string query = "SELECT Phone FROM Customers WHERE CustomerID = @CustomerID";
                    SqlParameter[] parameters = { new SqlParameter("@CustomerID", cmbCustomer.SelectedValue) };
                    DataTable result = DatabaseConnection.ExecuteQuery(query, parameters);

                    if (result.Rows.Count > 0)
                    {
                        txtCustomerPhone.Text = result.Rows[0]["Phone"].ToString();
                    }
                }
                catch (Exception ex)
                {
                    // Ignore error
                }
            }
        }

        private void TxtBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SearchItemByBarcode();
                e.Handled = true;
            }
        }

        private void SearchItemByBarcode()
        {
            if (!string.IsNullOrEmpty(txtBarcode.Text))
            {
                try
                {
                    string query = @"
                        SELECT i.ItemID, i.ItemName, i.Price, i.MRP, i.PurchasePrice, i.Barcode
                        FROM Items i 
                        WHERE i.Barcode = @Barcode AND i.IsActive = 1";
                    SqlParameter[] parameters = { new SqlParameter("@Barcode", txtBarcode.Text.Trim()) };
                    DataTable result = DatabaseConnection.ExecuteQuery(query, parameters);

                    if (result.Rows.Count > 0)
                    {
                        int itemID = Convert.ToInt32(result.Rows[0]["ItemID"]);
                        cmbItem.SelectedValue = itemID;
                        txtUnitPrice.Text = Convert.ToDecimal(result.Rows[0]["Price"]).ToString("N2");
                        txtMRP.Text = Convert.ToDecimal(result.Rows[0]["MRP"]).ToString("N2");

                        lblScanStatus.Text = "✓ Item Found";
                        lblScanStatus.ForeColor = Color.Green;
                        SystemSounds.Beep.Play();

                        txtQuantity.Focus();
                        txtQuantity.SelectAll();
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

        private void BtnScanBarcode_Click(object sender, EventArgs e)
        {
            // Placeholder for external barcode scanner integration
            MessageBox.Show("Connect barcode scanner and scan product.\n\nFor manual entry, type barcode and press Enter.", 
                "Barcode Scanner", MessageBoxButtons.OK, MessageBoxIcon.Information);
            txtBarcode.Focus();
        }

        private void CmbItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbItem.SelectedValue != null)
            {
                try
                {
                    string query = "SELECT Price, MRP, PurchasePrice, Barcode FROM Items WHERE ItemID = @ItemID";
                    SqlParameter[] parameters = { new SqlParameter("@ItemID", cmbItem.SelectedValue) };
                    DataTable result = DatabaseConnection.ExecuteQuery(query, parameters);

                    if (result.Rows.Count > 0)
                    {
                        txtUnitPrice.Text = Convert.ToDecimal(result.Rows[0]["Price"]).ToString("N2");
                        txtMRP.Text = Convert.ToDecimal(result.Rows[0]["MRP"]).ToString("N2");
                        txtBarcode.Text = result.Rows[0]["Barcode"].ToString();
                    }
                }
                catch (Exception ex)
                {
                    // Ignore error
                }
            }

            CalculateItemTotal(sender, e);
        }

        private void CalculateItemTotal(object sender, EventArgs e)
        {
            try
            {
                decimal quantity = Convert.ToDecimal(txtQuantity.Text.IsNullOrEmpty() ? "0" : txtQuantity.Text);
                decimal unitPrice = Convert.ToDecimal(txtUnitPrice.Text.IsNullOrEmpty() ? "0" : txtUnitPrice.Text);
                decimal discount = Convert.ToDecimal(txtDiscount.Text.IsNullOrEmpty() ? "0" : txtDiscount.Text);
                decimal taxRate = Convert.ToDecimal(txtTaxRate.Text.IsNullOrEmpty() ? "0" : txtTaxRate.Text);

                decimal subtotal = quantity * unitPrice;
                decimal discountAmount = discount;
                decimal taxableAmount = subtotal - discountAmount;
                decimal taxAmount = (taxableAmount * taxRate) / 100;
                decimal total = taxableAmount + taxAmount;

                txtItemTotal.Text = total.ToString("N2");
            }
            catch
            {
                txtItemTotal.Text = "0.00";
            }
        }

        private void BtnAddItem_Click(object sender, EventArgs e)
        {
            if (ValidateItemEntry())
            {
                AddItemToBill();
                ClearItemEntry();
                txtBarcode.Focus();
            }
        }

        private bool ValidateItemEntry()
        {
            if (cmbItem.SelectedValue == null)
            {
                MessageBox.Show("Please select an item.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!decimal.TryParse(txtQuantity.Text, out decimal quantity) || quantity <= 0)
            {
                MessageBox.Show("Please enter valid quantity.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!decimal.TryParse(txtUnitPrice.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Please enter valid unit price.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void AddItemToBill()
        {
            try
            {
                DataRow newRow = billItems.NewRow();
                newRow["ItemID"] = cmbItem.SelectedValue;
                newRow["ItemName"] = cmbItem.Text;
                newRow["Barcode"] = txtBarcode.Text;
                newRow["Quantity"] = Convert.ToDecimal(txtQuantity.Text);
                newRow["UnitPrice"] = Convert.ToDecimal(txtUnitPrice.Text);
                newRow["MRP"] = Convert.ToDecimal(txtMRP.Text);
                newRow["Discount"] = Convert.ToDecimal(txtDiscount.Text);
                newRow["TaxRate"] = Convert.ToDecimal(txtTaxRate.Text);

                decimal quantity = Convert.ToDecimal(txtQuantity.Text);
                decimal unitPrice = Convert.ToDecimal(txtUnitPrice.Text);
                decimal discount = Convert.ToDecimal(txtDiscount.Text);
                decimal taxRate = Convert.ToDecimal(txtTaxRate.Text);

                decimal subtotal = quantity * unitPrice;
                decimal taxableAmount = subtotal - discount;
                decimal taxAmount = (taxableAmount * taxRate) / 100;
                decimal total = taxableAmount + taxAmount;

                newRow["TaxAmount"] = taxAmount;
                newRow["Total"] = total;

                billItems.Rows.Add(newRow);
                CalculateBillTotals();
                UpdatePaymentFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding item: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearItemEntry()
        {
            txtBarcode.Clear();
            cmbItem.SelectedIndex = -1;
            txtQuantity.Text = "1";
            txtUnitPrice.Clear();
            txtMRP.Clear();
            txtDiscount.Text = "0";
            txtTaxRate.Text = "18";
            txtItemTotal.Text = "0.00";
            lblScanStatus.Text = "Ready to scan";
            lblScanStatus.ForeColor = Color.Gray;
        }

        private void BtnClearItem_Click(object sender, EventArgs e)
        {
            ClearItemEntry();
            txtBarcode.Focus();
        }

        private void DgvBillItems_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            DialogResult result = MessageBox.Show("Remove this item from bill?", "Confirm Delete", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
            if (result == DialogResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                CalculateBillTotals();
                UpdatePaymentFields();
            }
        }

        private void CalculateBillTotals()
        {
            try
            {
                int totalItems = billItems.Rows.Count;
                decimal totalQuantity = 0;
                decimal subTotal = 0;
                totalDiscount = 0;
                totalTax = 0;

                foreach (DataRow row in billItems.Rows)
                {
                    totalQuantity += Convert.ToDecimal(row["Quantity"]);
                    subTotal += Convert.ToDecimal(row["Quantity"]) * Convert.ToDecimal(row["UnitPrice"]);
                    totalDiscount += Convert.ToDecimal(row["Discount"]);
                    totalTax += Convert.ToDecimal(row["TaxAmount"]);
                }

                grandTotal = subTotal - totalDiscount + totalTax;
                netAmount = grandTotal;

                // Update labels
                lblTotalItems.Text = totalItems.ToString();
                lblTotalQuantity.Text = totalQuantity.ToString("N0");
                lblSubTotal.Text = subTotal.ToString("N2");
                lblTotalDiscount.Text = totalDiscount.ToString("N2");
                lblTotalTax.Text = totalTax.ToString("N2");
                lblGrandTotal.Text = grandTotal.ToString("N2");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error calculating totals: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdatePaymentFields()
        {
            if (!chkIsCreditSale.Checked)
            {
                txtPaidAmount.Text = grandTotal.ToString("N2");
                TxtPaidAmount_TextChanged(null, null);
            }
        }

        private void TxtPaidAmount_TextChanged(object sender, EventArgs e)
        {
            try
            {
                decimal paidAmount = Convert.ToDecimal(txtPaidAmount.Text.IsNullOrEmpty() ? "0" : txtPaidAmount.Text);
                decimal change = paidAmount - grandTotal;
                txtChangeAmount.Text = Math.Max(0, change).ToString("N2");
            }
            catch
            {
                txtChangeAmount.Text = "0.00";
            }
        }

        private void ChkIsCreditSale_CheckedChanged(object sender, EventArgs e)
        {
            if (chkIsCreditSale.Checked)
            {
                txtPaidAmount.Text = "0.00";
                txtPaidAmount.Enabled = false;
                txtChangeAmount.Text = "0.00";
            }
            else
            {
                txtPaidAmount.Enabled = true;
                UpdatePaymentFields();
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (ValidateBill() && SaveBill())
            {
                MessageBox.Show("Bill saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                GenerateBillNumber();
                ClearAll();
            }
        }

        private bool ValidateBill()
        {
            if (billItems.Rows.Count == 0)
            {
                MessageBox.Show("Please add at least one item to the bill.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!chkIsCreditSale.Checked)
            {
                decimal paidAmount = Convert.ToDecimal(txtPaidAmount.Text.IsNullOrEmpty() ? "0" : txtPaidAmount.Text);
                if (paidAmount < grandTotal)
                {
                    MessageBox.Show("Paid amount cannot be less than grand total for cash sale.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            return true;
        }

        private bool SaveBill()
        {
            try
            {
                // Insert sale header
                string saleQuery = @"
                    INSERT INTO Sales (BillNumber, CustomerID, SaleDate, TotalAmount, NetAmount, Discount, PaymentMethod, IsCredit, IsActive, CreatedDate) 
                    VALUES (@BillNumber, @CustomerID, @SaleDate, @TotalAmount, @NetAmount, @Discount, @PaymentMethod, @IsCredit, 1, GETDATE());
                    SELECT SCOPE_IDENTITY();";

                List<SqlParameter> saleParams = new List<SqlParameter>
                {
                    new SqlParameter("@BillNumber", txtBillNumber.Text.Trim()),
                    new SqlParameter("@CustomerID", cmbCustomer.SelectedValue ?? (object)DBNull.Value),
                    new SqlParameter("@SaleDate", dtpBillDate.Value),
                    new SqlParameter("@TotalAmount", grandTotal),
                    new SqlParameter("@NetAmount", netAmount),
                    new SqlParameter("@Discount", totalDiscount),
                    new SqlParameter("@PaymentMethod", cmbPaymentMethod.Text),
                    new SqlParameter("@IsCredit", chkIsCreditSale.Checked)
                };

                int saleID = Convert.ToInt32(DatabaseConnection.ExecuteScalar(saleQuery, saleParams.ToArray()));

                // Insert sale items
                foreach (DataRow row in billItems.Rows)
                {
                    string itemQuery = @"
                        INSERT INTO SaleItems (SaleID, ItemID, Quantity, Price, Rate, TotalAmount) 
                        VALUES (@SaleID, @ItemID, @Quantity, @Price, @Rate, @TotalAmount)";

                    List<SqlParameter> itemParams = new List<SqlParameter>
                    {
                        new SqlParameter("@SaleID", saleID),
                        new SqlParameter("@ItemID", row["ItemID"]),
                        new SqlParameter("@Quantity", row["Quantity"]),
                        new SqlParameter("@Price", row["UnitPrice"]),
                        new SqlParameter("@Rate", row["UnitPrice"]),
                        new SqlParameter("@TotalAmount", row["Total"])
                    };

                    DatabaseConnection.ExecuteQuery(itemQuery, itemParams.ToArray());

                    // Update stock
                    string stockQuery = "UPDATE Items SET StockQuantity = StockQuantity - @Quantity WHERE ItemID = @ItemID";
                    SqlParameter[] stockParams = {
                        new SqlParameter("@Quantity", row["Quantity"]),
                        new SqlParameter("@ItemID", row["ItemID"])
                    };
                    DatabaseConnection.ExecuteQuery(stockQuery, stockParams);
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving bill: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Print functionality will be implemented.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Clear all items and start new bill?", "Confirm Clear", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
            if (result == DialogResult.Yes)
            {
                ClearAll();
            }
        }

        private void ClearAll()
        {
            billItems.Clear();
            ClearItemEntry();
            cmbCustomer.SelectedIndex = -1;
            txtCustomerPhone.Clear();
            txtPaidAmount.Text = "0.00";
            txtChangeAmount.Text = "0.00";
            chkIsCreditSale.Checked = false;
            txtRemarks.Clear();
            CalculateBillTotals();
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }

    // Extension method for null check
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }
    }
}
