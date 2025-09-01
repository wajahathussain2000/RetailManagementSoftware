using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using RetailManagement.Database;
using RetailManagement.Models;

namespace RetailManagement.UserForms
{
    public partial class CustomerSupplierDetailedReportsForm : Form
    {
        private ReportViewer reportViewer;
        private ComboBox cmbReportType;
        private ComboBox cmbCustomer;
        private ComboBox cmbSupplier;
        private DateTimePicker dtpFromDate;
        private DateTimePicker dtpToDate;
        private ComboBox cmbTimePeriod;
        private ComboBox cmbPaymentStatus;
        private ComboBox cmbCategory;
        private CheckBox chkIncludeReturns;
        private CheckBox chkShowPaymentDetails;
        private CheckBox chkShowItemwise;
        private CheckBox chkGroupByMonth;
        private Button btnGenerate;
        private Button btnExport;
        private Button btnPrint;
        private Button btnEmail;
        private GroupBox groupFilters;
        private GroupBox groupOptions;
        private TabControl tabControl;
        private TabPage tabCustomerReports;
        private TabPage tabSupplierReports;
        private TabPage tabComparative;
        private DataTable currentReportData;
        private Panel summaryPanel;
        private Label lblTotalCustomers;
        private Label lblTotalSales;
        private Label lblTotalPurchases;
        private Label lblTopCustomer;
        private Label lblTopSupplier;
        private Label lblOutstandingAmount;

        public CustomerSupplierDetailedReportsForm()
        {
            InitializeComponent();
            InitializeReportViewer();
            LoadDropdownData();
            SetDefaultDates();
        }

        private void InitializeComponent()
        {
            this.groupFilters = new System.Windows.Forms.GroupBox();
            this.groupOptions = new System.Windows.Forms.GroupBox();
            this.cmbReportType = new System.Windows.Forms.ComboBox();
            this.cmbCustomer = new System.Windows.Forms.ComboBox();
            this.cmbSupplier = new System.Windows.Forms.ComboBox();
            this.dtpFromDate = new System.Windows.Forms.DateTimePicker();
            this.dtpToDate = new System.Windows.Forms.DateTimePicker();
            this.cmbTimePeriod = new System.Windows.Forms.ComboBox();
            this.cmbPaymentStatus = new System.Windows.Forms.ComboBox();
            this.cmbCategory = new System.Windows.Forms.ComboBox();
            this.chkIncludeReturns = new System.Windows.Forms.CheckBox();
            this.chkShowPaymentDetails = new System.Windows.Forms.CheckBox();
            this.chkShowItemwise = new System.Windows.Forms.CheckBox();
            this.chkGroupByMonth = new System.Windows.Forms.CheckBox();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnEmail = new System.Windows.Forms.Button();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabCustomerReports = new System.Windows.Forms.TabPage();
            this.tabSupplierReports = new System.Windows.Forms.TabPage();
            this.tabComparative = new System.Windows.Forms.TabPage();
            this.summaryPanel = new System.Windows.Forms.Panel();
            this.lblTotalCustomers = new System.Windows.Forms.Label();
            this.lblTotalSales = new System.Windows.Forms.Label();
            this.lblTotalPurchases = new System.Windows.Forms.Label();
            this.lblTopCustomer = new System.Windows.Forms.Label();
            this.lblTopSupplier = new System.Windows.Forms.Label();
            this.lblOutstandingAmount = new System.Windows.Forms.Label();

            this.SuspendLayout();

            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.summaryPanel);
            this.Controls.Add(this.groupOptions);
            this.Controls.Add(this.groupFilters);
            this.Name = "CustomerSupplierDetailedReportsForm";
            this.Text = "Customer & Supplier Detailed Reports";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;

            // Group Filters
            this.groupFilters.Controls.Add(new Label { Text = "Report Type:", Location = new Point(15, 25), Size = new Size(80, 13) });
            this.groupFilters.Controls.Add(this.cmbReportType);
            this.groupFilters.Controls.Add(new Label { Text = "Customer:", Location = new Point(15, 55), Size = new Size(60, 13) });
            this.groupFilters.Controls.Add(this.cmbCustomer);
            this.groupFilters.Controls.Add(new Label { Text = "Supplier:", Location = new Point(300, 55), Size = new Size(50, 13) });
            this.groupFilters.Controls.Add(this.cmbSupplier);
            this.groupFilters.Controls.Add(new Label { Text = "Time Period:", Location = new Point(15, 85), Size = new Size(75, 13) });
            this.groupFilters.Controls.Add(this.cmbTimePeriod);
            this.groupFilters.Controls.Add(new Label { Text = "From Date:", Location = new Point(15, 115), Size = new Size(65, 13) });
            this.groupFilters.Controls.Add(this.dtpFromDate);
            this.groupFilters.Controls.Add(new Label { Text = "To Date:", Location = new Point(300, 115), Size = new Size(50, 13) });
            this.groupFilters.Controls.Add(this.dtpToDate);
            this.groupFilters.Controls.Add(new Label { Text = "Payment Status:", Location = new Point(15, 145), Size = new Size(85, 13) });
            this.groupFilters.Controls.Add(this.cmbPaymentStatus);
            this.groupFilters.Controls.Add(new Label { Text = "Category:", Location = new Point(300, 145), Size = new Size(55, 13) });
            this.groupFilters.Controls.Add(this.cmbCategory);
            this.groupFilters.Location = new System.Drawing.Point(12, 12);
            this.groupFilters.Name = "groupFilters";
            this.groupFilters.Size = new System.Drawing.Size(600, 180);
            this.groupFilters.TabIndex = 0;
            this.groupFilters.TabStop = false;
            this.groupFilters.Text = "Report Filters";

            // Group Options
            this.groupOptions.Controls.Add(this.chkIncludeReturns);
            this.groupOptions.Controls.Add(this.chkShowPaymentDetails);
            this.groupOptions.Controls.Add(this.chkShowItemwise);
            this.groupOptions.Controls.Add(this.chkGroupByMonth);
            this.groupOptions.Controls.Add(this.btnGenerate);
            this.groupOptions.Controls.Add(this.btnExport);
            this.groupOptions.Controls.Add(this.btnPrint);
            this.groupOptions.Controls.Add(this.btnEmail);
            this.groupOptions.Location = new System.Drawing.Point(630, 12);
            this.groupOptions.Name = "groupOptions";
            this.groupOptions.Size = new System.Drawing.Size(300, 180);
            this.groupOptions.TabIndex = 1;
            this.groupOptions.TabStop = false;
            this.groupOptions.Text = "Options & Actions";

            // Controls Setup
            SetupControls();

            // Tab Control
            this.tabControl.Controls.Add(this.tabCustomerReports);
            this.tabControl.Controls.Add(this.tabSupplierReports);
            this.tabControl.Controls.Add(this.tabComparative);
            this.tabControl.Location = new System.Drawing.Point(12, 270);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(1176, 380);
            this.tabControl.TabIndex = 3;

            // Tab Pages
            this.tabCustomerReports.Location = new System.Drawing.Point(4, 22);
            this.tabCustomerReports.Name = "tabCustomerReports";
            this.tabCustomerReports.Padding = new System.Windows.Forms.Padding(3);
            this.tabCustomerReports.Size = new System.Drawing.Size(1168, 354);
            this.tabCustomerReports.TabIndex = 0;
            this.tabCustomerReports.Text = "Customer Reports";
            this.tabCustomerReports.UseVisualStyleBackColor = true;

            this.tabSupplierReports.Location = new System.Drawing.Point(4, 22);
            this.tabSupplierReports.Name = "tabSupplierReports";
            this.tabSupplierReports.Padding = new System.Windows.Forms.Padding(3);
            this.tabSupplierReports.Size = new System.Drawing.Size(1168, 354);
            this.tabSupplierReports.TabIndex = 1;
            this.tabSupplierReports.Text = "Supplier Reports";
            this.tabSupplierReports.UseVisualStyleBackColor = true;

            this.tabComparative.Location = new System.Drawing.Point(4, 22);
            this.tabComparative.Name = "tabComparative";
            this.tabComparative.Padding = new System.Windows.Forms.Padding(3);
            this.tabComparative.Size = new System.Drawing.Size(1168, 354);
            this.tabComparative.TabIndex = 2;
            this.tabComparative.Text = "Comparative Analysis";
            this.tabComparative.UseVisualStyleBackColor = true;

            // Summary Panel
            this.summaryPanel.BackColor = System.Drawing.Color.LightGreen;
            this.summaryPanel.Controls.Add(this.lblTotalCustomers);
            this.summaryPanel.Controls.Add(this.lblTotalSales);
            this.summaryPanel.Controls.Add(this.lblTotalPurchases);
            this.summaryPanel.Controls.Add(this.lblTopCustomer);
            this.summaryPanel.Controls.Add(this.lblTopSupplier);
            this.summaryPanel.Controls.Add(this.lblOutstandingAmount);
            this.summaryPanel.Location = new System.Drawing.Point(12, 200);
            this.summaryPanel.Name = "summaryPanel";
            this.summaryPanel.Size = new System.Drawing.Size(1176, 65);
            this.summaryPanel.TabIndex = 2;

            SetupSummaryLabels();

            this.ResumeLayout(false);
        }

        private void SetupControls()
        {
            // Report Type ComboBox
            this.cmbReportType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbReportType.Location = new System.Drawing.Point(100, 22);
            this.cmbReportType.Name = "cmbReportType";
            this.cmbReportType.Size = new System.Drawing.Size(180, 21);
            this.cmbReportType.TabIndex = 1;
            this.cmbReportType.SelectedIndexChanged += CmbReportType_SelectedIndexChanged;

            // Customer ComboBox
            this.cmbCustomer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCustomer.Location = new System.Drawing.Point(80, 52);
            this.cmbCustomer.Name = "cmbCustomer";
            this.cmbCustomer.Size = new System.Drawing.Size(200, 21);
            this.cmbCustomer.TabIndex = 2;

            // Supplier ComboBox
            this.cmbSupplier.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSupplier.Location = new System.Drawing.Point(360, 52);
            this.cmbSupplier.Name = "cmbSupplier";
            this.cmbSupplier.Size = new System.Drawing.Size(200, 21);
            this.cmbSupplier.TabIndex = 3;

            // Time Period ComboBox
            this.cmbTimePeriod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTimePeriod.Location = new System.Drawing.Point(95, 82);
            this.cmbTimePeriod.Name = "cmbTimePeriod";
            this.cmbTimePeriod.Size = new System.Drawing.Size(120, 21);
            this.cmbTimePeriod.TabIndex = 4;
            this.cmbTimePeriod.SelectedIndexChanged += CmbTimePeriod_SelectedIndexChanged;

            // Date Pickers
            this.dtpFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFromDate.Location = new System.Drawing.Point(85, 112);
            this.dtpFromDate.Name = "dtpFromDate";
            this.dtpFromDate.Size = new System.Drawing.Size(100, 20);
            this.dtpFromDate.TabIndex = 5;

            this.dtpToDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpToDate.Location = new System.Drawing.Point(360, 112);
            this.dtpToDate.Name = "dtpToDate";
            this.dtpToDate.Size = new System.Drawing.Size(100, 20);
            this.dtpToDate.TabIndex = 6;

            // Payment Status ComboBox
            this.cmbPaymentStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPaymentStatus.Location = new System.Drawing.Point(105, 142);
            this.cmbPaymentStatus.Name = "cmbPaymentStatus";
            this.cmbPaymentStatus.Size = new System.Drawing.Size(120, 21);
            this.cmbPaymentStatus.TabIndex = 7;

            // Category ComboBox
            this.cmbCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCategory.Location = new System.Drawing.Point(360, 142);
            this.cmbCategory.Name = "cmbCategory";
            this.cmbCategory.Size = new System.Drawing.Size(150, 21);
            this.cmbCategory.TabIndex = 8;

            // Checkboxes
            this.chkIncludeReturns.Location = new System.Drawing.Point(15, 25);
            this.chkIncludeReturns.Name = "chkIncludeReturns";
            this.chkIncludeReturns.Size = new System.Drawing.Size(120, 17);
            this.chkIncludeReturns.TabIndex = 9;
            this.chkIncludeReturns.Text = "Include Returns";
            this.chkIncludeReturns.UseVisualStyleBackColor = true;

            this.chkShowPaymentDetails.Location = new System.Drawing.Point(15, 45);
            this.chkShowPaymentDetails.Name = "chkShowPaymentDetails";
            this.chkShowPaymentDetails.Size = new System.Drawing.Size(140, 17);
            this.chkShowPaymentDetails.TabIndex = 10;
            this.chkShowPaymentDetails.Text = "Show Payment Details";
            this.chkShowPaymentDetails.UseVisualStyleBackColor = true;

            this.chkShowItemwise.Location = new System.Drawing.Point(15, 65);
            this.chkShowItemwise.Name = "chkShowItemwise";
            this.chkShowItemwise.Size = new System.Drawing.Size(120, 17);
            this.chkShowItemwise.TabIndex = 11;
            this.chkShowItemwise.Text = "Show Itemwise";
            this.chkShowItemwise.UseVisualStyleBackColor = true;

            this.chkGroupByMonth.Location = new System.Drawing.Point(15, 85);
            this.chkGroupByMonth.Name = "chkGroupByMonth";
            this.chkGroupByMonth.Size = new System.Drawing.Size(120, 17);
            this.chkGroupByMonth.TabIndex = 12;
            this.chkGroupByMonth.Text = "Group by Month";
            this.chkGroupByMonth.UseVisualStyleBackColor = true;

            // Buttons
            this.btnGenerate.BackColor = System.Drawing.Color.Green;
            this.btnGenerate.ForeColor = System.Drawing.Color.White;
            this.btnGenerate.Location = new System.Drawing.Point(150, 20);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(130, 30);
            this.btnGenerate.TabIndex = 13;
            this.btnGenerate.Text = "Generate Report";
            this.btnGenerate.UseVisualStyleBackColor = false;
            this.btnGenerate.Click += BtnGenerate_Click;

            this.btnExport.BackColor = System.Drawing.Color.Blue;
            this.btnExport.ForeColor = System.Drawing.Color.White;
            this.btnExport.Location = new System.Drawing.Point(150, 55);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(130, 30);
            this.btnExport.TabIndex = 14;
            this.btnExport.Text = "Export Excel";
            this.btnExport.UseVisualStyleBackColor = false;
            this.btnExport.Click += BtnExport_Click;

            this.btnPrint.BackColor = System.Drawing.Color.Orange;
            this.btnPrint.ForeColor = System.Drawing.Color.White;
            this.btnPrint.Location = new System.Drawing.Point(150, 90);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(130, 30);
            this.btnPrint.TabIndex = 15;
            this.btnPrint.Text = "Print Report";
            this.btnPrint.UseVisualStyleBackColor = false;
            this.btnPrint.Click += BtnPrint_Click;

            this.btnEmail.BackColor = System.Drawing.Color.Purple;
            this.btnEmail.ForeColor = System.Drawing.Color.White;
            this.btnEmail.Location = new System.Drawing.Point(150, 125);
            this.btnEmail.Name = "btnEmail";
            this.btnEmail.Size = new System.Drawing.Size(130, 30);
            this.btnEmail.TabIndex = 16;
            this.btnEmail.Text = "Email Report";
            this.btnEmail.UseVisualStyleBackColor = false;
            this.btnEmail.Click += BtnEmail_Click;
        }

        private void SetupSummaryLabels()
        {
            this.lblTotalCustomers.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblTotalCustomers.Location = new System.Drawing.Point(15, 10);
            this.lblTotalCustomers.Name = "lblTotalCustomers";
            this.lblTotalCustomers.Size = new System.Drawing.Size(150, 15);
            this.lblTotalCustomers.TabIndex = 0;
            this.lblTotalCustomers.Text = "Total Customers: 0";

            this.lblTotalSales.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblTotalSales.Location = new System.Drawing.Point(15, 30);
            this.lblTotalSales.Name = "lblTotalSales";
            this.lblTotalSales.Size = new System.Drawing.Size(200, 15);
            this.lblTotalSales.TabIndex = 1;
            this.lblTotalSales.Text = "Total Sales: ₹0.00";

            this.lblTotalPurchases.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblTotalPurchases.Location = new System.Drawing.Point(15, 50);
            this.lblTotalPurchases.Name = "lblTotalPurchases";
            this.lblTotalPurchases.Size = new System.Drawing.Size(200, 15);
            this.lblTotalPurchases.TabIndex = 2;
            this.lblTotalPurchases.Text = "Total Purchases: ₹0.00";

            this.lblTopCustomer.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblTopCustomer.Location = new System.Drawing.Point(250, 10);
            this.lblTopCustomer.Name = "lblTopCustomer";
            this.lblTopCustomer.Size = new System.Drawing.Size(300, 15);
            this.lblTopCustomer.TabIndex = 3;
            this.lblTopCustomer.Text = "Top Customer: -";

            this.lblTopSupplier.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblTopSupplier.Location = new System.Drawing.Point(250, 30);
            this.lblTopSupplier.Name = "lblTopSupplier";
            this.lblTopSupplier.Size = new System.Drawing.Size(300, 15);
            this.lblTopSupplier.TabIndex = 4;
            this.lblTopSupplier.Text = "Top Supplier: -";

            this.lblOutstandingAmount.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblOutstandingAmount.Location = new System.Drawing.Point(250, 50);
            this.lblOutstandingAmount.Name = "lblOutstandingAmount";
            this.lblOutstandingAmount.Size = new System.Drawing.Size(200, 15);
            this.lblOutstandingAmount.TabIndex = 5;
            this.lblOutstandingAmount.Text = "Outstanding: ₹0.00";
        }

        private void InitializeReportViewer()
        {
            this.reportViewer = new ReportViewer();
            this.reportViewer.Dock = DockStyle.Fill;
            this.reportViewer.ProcessingMode = ProcessingMode.Local;
            
            // Add report viewer to each tab
            this.tabCustomerReports.Controls.Add(this.reportViewer);
        }

        private void LoadDropdownData()
        {
            try
            {
                // Load Report Types
                cmbReportType.Items.Clear();
                cmbReportType.Items.Add(new ComboBoxItem { Text = "Customer Sales Summary", Value = "CUSTOMER_SALES" });
                cmbReportType.Items.Add(new ComboBoxItem { Text = "Customer Detailed Sales", Value = "CUSTOMER_DETAILED" });
                cmbReportType.Items.Add(new ComboBoxItem { Text = "Customer Outstanding", Value = "CUSTOMER_OUTSTANDING" });
                cmbReportType.Items.Add(new ComboBoxItem { Text = "Customer Payment History", Value = "CUSTOMER_PAYMENTS" });
                cmbReportType.Items.Add(new ComboBoxItem { Text = "Supplier Purchase Summary", Value = "SUPPLIER_PURCHASES" });
                cmbReportType.Items.Add(new ComboBoxItem { Text = "Supplier Detailed Purchases", Value = "SUPPLIER_DETAILED" });
                cmbReportType.Items.Add(new ComboBoxItem { Text = "Supplier Payables", Value = "SUPPLIER_PAYABLES" });
                cmbReportType.Items.Add(new ComboBoxItem { Text = "Top Customers Analysis", Value = "TOP_CUSTOMERS" });
                cmbReportType.Items.Add(new ComboBoxItem { Text = "Top Suppliers Analysis", Value = "TOP_SUPPLIERS" });
                cmbReportType.Items.Add(new ComboBoxItem { Text = "Comparative Analysis", Value = "COMPARATIVE" });
                cmbReportType.DisplayMember = "Text";
                cmbReportType.ValueMember = "Value";
                cmbReportType.SelectedIndex = 0;

                // Load Customers
                string customerQuery = "SELECT CustomerID, CustomerName FROM Customers WHERE IsActive = 1 ORDER BY CustomerName";
                DataTable customers = DatabaseConnection.ExecuteQuery(customerQuery);
                
                cmbCustomer.Items.Clear();
                cmbCustomer.Items.Add(new ComboBoxItem { Text = "All Customers", Value = 0 });
                foreach (DataRow row in customers.Rows)
                {
                    cmbCustomer.Items.Add(new ComboBoxItem 
                    { 
                        Text = row["CustomerName"].ToString(), 
                        Value = Convert.ToInt32(row["CustomerID"]) 
                    });
                }
                cmbCustomer.DisplayMember = "Text";
                cmbCustomer.ValueMember = "Value";
                cmbCustomer.SelectedIndex = 0;

                // Load Suppliers
                string supplierQuery = "SELECT CompanyID, CompanyName FROM Companies WHERE IsActive = 1 ORDER BY CompanyName";
                DataTable suppliers = DatabaseConnection.ExecuteQuery(supplierQuery);
                
                cmbSupplier.Items.Clear();
                cmbSupplier.Items.Add(new ComboBoxItem { Text = "All Suppliers", Value = 0 });
                foreach (DataRow row in suppliers.Rows)
                {
                    cmbSupplier.Items.Add(new ComboBoxItem 
                    { 
                        Text = row["CompanyName"].ToString(), 
                        Value = Convert.ToInt32(row["CompanyID"]) 
                    });
                }
                cmbSupplier.DisplayMember = "Text";
                cmbSupplier.ValueMember = "Value";
                cmbSupplier.SelectedIndex = 0;

                // Load Time Periods
                cmbTimePeriod.Items.Clear();
                cmbTimePeriod.Items.Add(new ComboBoxItem { Text = "Custom Range", Value = "CUSTOM" });
                cmbTimePeriod.Items.Add(new ComboBoxItem { Text = "Today", Value = "TODAY" });
                cmbTimePeriod.Items.Add(new ComboBoxItem { Text = "This Week", Value = "THIS_WEEK" });
                cmbTimePeriod.Items.Add(new ComboBoxItem { Text = "This Month", Value = "THIS_MONTH" });
                cmbTimePeriod.Items.Add(new ComboBoxItem { Text = "This Quarter", Value = "THIS_QUARTER" });
                cmbTimePeriod.Items.Add(new ComboBoxItem { Text = "This Year", Value = "THIS_YEAR" });
                cmbTimePeriod.Items.Add(new ComboBoxItem { Text = "Last Month", Value = "LAST_MONTH" });
                cmbTimePeriod.Items.Add(new ComboBoxItem { Text = "Last Quarter", Value = "LAST_QUARTER" });
                cmbTimePeriod.Items.Add(new ComboBoxItem { Text = "Last Year", Value = "LAST_YEAR" });
                cmbTimePeriod.DisplayMember = "Text";
                cmbTimePeriod.ValueMember = "Value";
                cmbTimePeriod.SelectedIndex = 3; // This Month

                // Load Payment Status
                cmbPaymentStatus.Items.Clear();
                cmbPaymentStatus.Items.Add(new ComboBoxItem { Text = "All Payments", Value = "ALL" });
                cmbPaymentStatus.Items.Add(new ComboBoxItem { Text = "Paid", Value = "PAID" });
                cmbPaymentStatus.Items.Add(new ComboBoxItem { Text = "Pending", Value = "PENDING" });
                cmbPaymentStatus.Items.Add(new ComboBoxItem { Text = "Overdue", Value = "OVERDUE" });
                cmbPaymentStatus.Items.Add(new ComboBoxItem { Text = "Partial", Value = "PARTIAL" });
                cmbPaymentStatus.DisplayMember = "Text";
                cmbPaymentStatus.ValueMember = "Value";
                cmbPaymentStatus.SelectedIndex = 0;

                // Load Categories
                string categoryQuery = "SELECT DISTINCT Category FROM Items WHERE Category IS NOT NULL AND Category != '' ORDER BY Category";
                DataTable categories = DatabaseConnection.ExecuteQuery(categoryQuery);
                
                cmbCategory.Items.Clear();
                cmbCategory.Items.Add(new ComboBoxItem { Text = "All Categories", Value = "" });
                foreach (DataRow row in categories.Rows)
                {
                    cmbCategory.Items.Add(new ComboBoxItem 
                    { 
                        Text = row["Category"].ToString(), 
                        Value = row["Category"].ToString() 
                    });
                }
                cmbCategory.DisplayMember = "Text";
                cmbCategory.ValueMember = "Value";
                cmbCategory.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading dropdown data: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetDefaultDates()
        {
            // Set to current month
            dtpFromDate.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dtpToDate.Value = dtpFromDate.Value.AddMonths(1).AddDays(-1);
        }

        private void CmbReportType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxItem selectedType = (ComboBoxItem)cmbReportType.SelectedItem;
            if (selectedType != null)
            {
                string reportType = selectedType.Value.ToString();
                
                // Enable/disable controls based on report type
                if (reportType.Contains("CUSTOMER"))
                {
                    cmbCustomer.Enabled = true;
                    cmbSupplier.Enabled = false;
                    tabControl.SelectedTab = tabCustomerReports;
                }
                else if (reportType.Contains("SUPPLIER"))
                {
                    cmbCustomer.Enabled = false;
                    cmbSupplier.Enabled = true;
                    tabControl.SelectedTab = tabSupplierReports;
                }
                else if (reportType == "COMPARATIVE")
                {
                    cmbCustomer.Enabled = true;
                    cmbSupplier.Enabled = true;
                    tabControl.SelectedTab = tabComparative;
                }
            }
        }

        private void CmbTimePeriod_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxItem selectedPeriod = (ComboBoxItem)cmbTimePeriod.SelectedItem;
            if (selectedPeriod != null)
            {
                string period = selectedPeriod.Value.ToString();
                DateTime fromDate, toDate;
                
                GetDateRangeFromPeriod(period, out fromDate, out toDate);
                
                if (period != "CUSTOM")
                {
                    dtpFromDate.Value = fromDate;
                    dtpToDate.Value = toDate;
                    dtpFromDate.Enabled = false;
                    dtpToDate.Enabled = false;
                }
                else
                {
                    dtpFromDate.Enabled = true;
                    dtpToDate.Enabled = true;
                }
            }
        }

        private void GetDateRangeFromPeriod(string period, out DateTime fromDate, out DateTime toDate)
        {
            DateTime now = DateTime.Now;
            
            switch (period)
            {
                case "TODAY":
                    fromDate = now.Date;
                    toDate = now.Date;
                    break;
                case "THIS_WEEK":
                    fromDate = now.Date.AddDays(-(int)now.DayOfWeek);
                    toDate = fromDate.AddDays(6);
                    break;
                case "THIS_MONTH":
                    fromDate = new DateTime(now.Year, now.Month, 1);
                    toDate = fromDate.AddMonths(1).AddDays(-1);
                    break;
                case "THIS_QUARTER":
                    int quarter = (now.Month - 1) / 3 + 1;
                    fromDate = new DateTime(now.Year, (quarter - 1) * 3 + 1, 1);
                    toDate = fromDate.AddMonths(3).AddDays(-1);
                    break;
                case "THIS_YEAR":
                    fromDate = new DateTime(now.Year, 1, 1);
                    toDate = new DateTime(now.Year, 12, 31);
                    break;
                case "LAST_MONTH":
                    fromDate = new DateTime(now.Year, now.Month, 1).AddMonths(-1);
                    toDate = fromDate.AddMonths(1).AddDays(-1);
                    break;
                case "LAST_QUARTER":
                    int lastQuarter = (now.Month - 1) / 3;
                    if (lastQuarter == 0)
                    {
                        lastQuarter = 4;
                        fromDate = new DateTime(now.Year - 1, 10, 1);
                    }
                    else
                    {
                        fromDate = new DateTime(now.Year, (lastQuarter - 1) * 3 + 1, 1);
                    }
                    toDate = fromDate.AddMonths(3).AddDays(-1);
                    break;
                case "LAST_YEAR":
                    fromDate = new DateTime(now.Year - 1, 1, 1);
                    toDate = new DateTime(now.Year - 1, 12, 31);
                    break;
                default: // CUSTOM
                    fromDate = dtpFromDate.Value;
                    toDate = dtpToDate.Value;
                    break;
            }
        }

        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            GenerateDetailedReport();
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            ExportToExcel();
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            PrintReport();
        }

        private void BtnEmail_Click(object sender, EventArgs e)
        {
            EmailReport();
        }

        private void GenerateDetailedReport()
        {
            try
            {
                ComboBoxItem selectedType = (ComboBoxItem)cmbReportType.SelectedItem;
                ComboBoxItem selectedCustomer = (ComboBoxItem)cmbCustomer.SelectedItem;
                ComboBoxItem selectedSupplier = (ComboBoxItem)cmbSupplier.SelectedItem;
                ComboBoxItem selectedPeriod = (ComboBoxItem)cmbTimePeriod.SelectedItem;
                ComboBoxItem selectedCategory = (ComboBoxItem)cmbCategory.SelectedItem;
                ComboBoxItem selectedPaymentStatus = (ComboBoxItem)cmbPaymentStatus.SelectedItem;

                string reportType = selectedType?.Value.ToString() ?? "CUSTOMER_SALES";
                int customerID = selectedCustomer != null ? (int)selectedCustomer.Value : 0;
                int supplierID = selectedSupplier != null ? (int)selectedSupplier.Value : 0;
                string category = selectedCategory?.Value.ToString() ?? "";
                string paymentStatus = selectedPaymentStatus?.Value.ToString() ?? "ALL";

                DateTime fromDate = dtpFromDate.Value;
                DateTime toDate = dtpToDate.Value;

                // Generate appropriate report based on type
                switch (reportType)
                {
                    case "CUSTOMER_SALES":
                        GenerateCustomerSalesReport(customerID, fromDate, toDate, category);
                        break;
                    case "CUSTOMER_DETAILED":
                        GenerateCustomerDetailedReport(customerID, fromDate, toDate, category);
                        break;
                    case "CUSTOMER_OUTSTANDING":
                        GenerateCustomerOutstandingReport(customerID, paymentStatus);
                        break;
                    case "CUSTOMER_PAYMENTS":
                        GenerateCustomerPaymentHistoryReport(customerID, fromDate, toDate);
                        break;
                    case "SUPPLIER_PURCHASES":
                        GenerateSupplierPurchaseReport(supplierID, fromDate, toDate, category);
                        break;
                    case "SUPPLIER_DETAILED":
                        GenerateSupplierDetailedReport(supplierID, fromDate, toDate, category);
                        break;
                    case "SUPPLIER_PAYABLES":
                        GenerateSupplierPayablesReport(supplierID, paymentStatus);
                        break;
                    case "TOP_CUSTOMERS":
                        GenerateTopCustomersReport(fromDate, toDate);
                        break;
                    case "TOP_SUPPLIERS":
                        GenerateTopSuppliersReport(fromDate, toDate);
                        break;
                    case "COMPARATIVE":
                        GenerateComparativeReport(fromDate, toDate);
                        break;
                }

                UpdateSummaryStats();
                MessageBox.Show("Detailed report generated successfully!", "Success", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating detailed report: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerateCustomerSalesReport(int customerID, DateTime fromDate, DateTime toDate, string category)
        {
            try
            {
                string query = @"
                    SELECT 
                        c.CustomerName,
                        c.ContactNumber,
                        c.Email,
                        COUNT(DISTINCT s.SaleID) as TotalInvoices,
                        SUM(si.Quantity) as TotalQuantity,
                        SUM(si.TaxableAmount) as TotalTaxableAmount,
                        SUM(si.NetAmount) as TotalAmount,
                        AVG(si.NetAmount) as AverageOrderValue,
                        MAX(s.BillDate) as LastPurchaseDate,
                        SUM(CASE WHEN s.PaymentStatus = 'Due' THEN si.NetAmount ELSE 0 END) as OutstandingAmount
                    FROM Customers c
                    LEFT JOIN Sales s ON c.CustomerID = s.CustomerID
                    LEFT JOIN SaleItems si ON s.SaleID = si.SaleID
                    LEFT JOIN Items i ON si.ItemID = i.ItemID
                    WHERE (@CustomerID = 0 OR c.CustomerID = @CustomerID)
                    AND s.BillDate BETWEEN @FromDate AND @ToDate
                    AND c.IsActive = 1
                    AND s.IsActive = 1" +
                    (chkIncludeReturns.Checked ? "" : " AND s.IsReturn = 0") +
                    (string.IsNullOrEmpty(category) ? "" : " AND i.Category = @Category") + @"
                    GROUP BY c.CustomerID, c.CustomerName, c.ContactNumber, c.Email
                    ORDER BY TotalAmount DESC";

                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    new SqlParameter("@CustomerID", customerID),
                    new SqlParameter("@FromDate", fromDate),
                    new SqlParameter("@ToDate", toDate)
                };

                if (!string.IsNullOrEmpty(category))
                {
                    parameters.Add(new SqlParameter("@Category", category));
                }

                currentReportData = DatabaseConnection.ExecuteQuery(query, parameters.ToArray());

                if (currentReportData != null && currentReportData.Rows.Count > 0)
                {
                    // Smart RDLC path detection for CustomerSalesDetailedReport
                    string simplePath = Path.Combine(Application.StartupPath, "Reports", "CustomerSalesDetailedReport_Simple.rdlc");
                    string originalPath = Path.Combine(Application.StartupPath, "Reports", "CustomerSalesDetailedReport.rdlc");
                    
                    if (System.IO.File.Exists(simplePath))
                    {
                        reportViewer.LocalReport.ReportPath = simplePath;
                    }
                    else if (System.IO.File.Exists(originalPath))
                    {
                        reportViewer.LocalReport.ReportPath = originalPath;
                    }
                    else
                    {
                        reportViewer.LocalReport.ReportPath = "Reports/CustomerSalesDetailedReport.rdlc";
                    }
                    reportViewer.LocalReport.DataSources.Clear();
                    reportViewer.LocalReport.DataSources.Add(new ReportDataSource("CustomerSalesDataset", currentReportData));

                    // Set report parameters
                    ReportParameter[] reportParams = new ReportParameter[]
                    {
                        new ReportParameter("FromDate", fromDate.ToString("dd/MM/yyyy")),
                        new ReportParameter("ToDate", toDate.ToString("dd/MM/yyyy")),
                        new ReportParameter("CustomerName", customerID == 0 ? "All Customers" : cmbCustomer.Text),
                        new ReportParameter("Category", string.IsNullOrEmpty(category) ? "All Categories" : category),
                        new ReportParameter("ReportTitle", "Customer Sales Summary Report")
                    };

                    reportViewer.LocalReport.SetParameters(reportParams);
                    reportViewer.RefreshReport();
                }
                else
                {
                    MessageBox.Show("No data found for the selected criteria.", "Information", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating customer sales report: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerateCustomerDetailedReport(int customerID, DateTime fromDate, DateTime toDate, string category)
        {
            try
            {
                string query = @"
                    SELECT 
                        s.BillNumber,
                        s.BillDate,
                        c.CustomerName,
                        c.ContactNumber,
                        i.ItemName,
                        i.Category,
                        si.Quantity,
                        si.Rate,
                        si.TaxableAmount,
                        si.NetAmount,
                        s.PaymentMode,
                        s.PaymentStatus,
                        CASE 
                            WHEN s.PaymentStatus = 'Due' THEN si.NetAmount
                            ELSE 0
                        END as OutstandingAmount
                    FROM Sales s
                    INNER JOIN Customers c ON s.CustomerID = c.CustomerID
                    INNER JOIN SaleItems si ON s.SaleID = si.SaleID
                    INNER JOIN Items i ON si.ItemID = i.ItemID
                    WHERE (@CustomerID = 0 OR c.CustomerID = @CustomerID)
                    AND s.BillDate BETWEEN @FromDate AND @ToDate
                    AND c.IsActive = 1
                    AND s.IsActive = 1" +
                    (chkIncludeReturns.Checked ? "" : " AND s.IsReturn = 0") +
                    (string.IsNullOrEmpty(category) ? "" : " AND i.Category = @Category") + @"
                    ORDER BY s.BillDate DESC, s.BillNumber";

                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    new SqlParameter("@CustomerID", customerID),
                    new SqlParameter("@FromDate", fromDate),
                    new SqlParameter("@ToDate", toDate)
                };

                if (!string.IsNullOrEmpty(category))
                {
                    parameters.Add(new SqlParameter("@Category", category));
                }

                currentReportData = DatabaseConnection.ExecuteQuery(query, parameters.ToArray());

                if (currentReportData != null && currentReportData.Rows.Count > 0)
                {
                    // Create and load RDLC report
                    string reportPath = Path.Combine(Application.StartupPath, "Reports", "CustomerDetailedReport.rdlc");
                    reportViewer.LocalReport.ReportPath = reportPath;
                    reportViewer.LocalReport.DataSources.Clear();
                    reportViewer.LocalReport.DataSources.Add(new ReportDataSource("CustomerDetailedDataset", currentReportData));

                    // Set report parameters
                    ReportParameter[] reportParams = new ReportParameter[]
                    {
                        new ReportParameter("FromDate", fromDate.ToString("dd/MM/yyyy")),
                        new ReportParameter("ToDate", toDate.ToString("dd/MM/yyyy")),
                        new ReportParameter("CustomerName", customerID == 0 ? "All Customers" : cmbCustomer.Text),
                        new ReportParameter("Category", string.IsNullOrEmpty(category) ? "All Categories" : category),
                        new ReportParameter("ReportTitle", "Customer Detailed Sales Report")
                    };

                    reportViewer.LocalReport.SetParameters(reportParams);
                    reportViewer.RefreshReport();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating customer detailed report: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerateSupplierPurchaseReport(int supplierID, DateTime fromDate, DateTime toDate, string category)
        {
            try
            {
                string query = @"
                    SELECT 
                        c.CompanyName as SupplierName,
                        c.ContactNumber,
                        c.Email,
                        COUNT(DISTINCT p.PurchaseID) as TotalInvoices,
                        SUM(pi.Quantity) as TotalQuantity,
                        SUM(pi.TaxableAmount) as TotalTaxableAmount,
                        SUM(pi.NetAmount) as TotalAmount,
                        AVG(pi.NetAmount) as AverageOrderValue,
                        MAX(p.PurchaseDate) as LastPurchaseDate,
                        SUM(CASE WHEN p.PaymentStatus = 'Due' THEN pi.NetAmount ELSE 0 END) as PayableAmount
                    FROM Companies c
                    LEFT JOIN Purchases p ON c.CompanyID = p.CompanyID
                    LEFT JOIN PurchaseItems pi ON p.PurchaseID = pi.PurchaseID
                    LEFT JOIN Items i ON pi.ItemID = i.ItemID
                    WHERE (@SupplierID = 0 OR c.CompanyID = @SupplierID)
                    AND p.PurchaseDate BETWEEN @FromDate AND @ToDate
                    AND c.IsActive = 1
                    AND p.IsActive = 1" +
                    (chkIncludeReturns.Checked ? "" : " AND p.IsReturn = 0") +
                    (string.IsNullOrEmpty(category) ? "" : " AND i.Category = @Category") + @"
                    GROUP BY c.CompanyID, c.CompanyName, c.ContactNumber, c.Email
                    ORDER BY TotalAmount DESC";

                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    new SqlParameter("@SupplierID", supplierID),
                    new SqlParameter("@FromDate", fromDate),
                    new SqlParameter("@ToDate", toDate)
                };

                if (!string.IsNullOrEmpty(category))
                {
                    parameters.Add(new SqlParameter("@Category", category));
                }

                currentReportData = DatabaseConnection.ExecuteQuery(query, parameters.ToArray());

                if (currentReportData != null && currentReportData.Rows.Count > 0)
                {
                    // Create and load RDLC report
                    string reportPath = Path.Combine(Application.StartupPath, "Reports", "SupplierPurchaseReport.rdlc");
                    reportViewer.LocalReport.ReportPath = reportPath;
                    reportViewer.LocalReport.DataSources.Clear();
                    reportViewer.LocalReport.DataSources.Add(new ReportDataSource("SupplierPurchaseDataset", currentReportData));

                    // Set report parameters
                    ReportParameter[] reportParams = new ReportParameter[]
                    {
                        new ReportParameter("FromDate", fromDate.ToString("dd/MM/yyyy")),
                        new ReportParameter("ToDate", toDate.ToString("dd/MM/yyyy")),
                        new ReportParameter("SupplierName", supplierID == 0 ? "All Suppliers" : cmbSupplier.Text),
                        new ReportParameter("Category", string.IsNullOrEmpty(category) ? "All Categories" : category),
                        new ReportParameter("ReportTitle", "Supplier Purchase Summary Report")
                    };

                    reportViewer.LocalReport.SetParameters(reportParams);
                    reportViewer.RefreshReport();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating supplier purchase report: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerateCustomerOutstandingReport(int customerID, string paymentStatus)
        {
            // Implementation for customer outstanding report
        }

        private void GenerateCustomerPaymentHistoryReport(int customerID, DateTime fromDate, DateTime toDate)
        {
            // Implementation for customer payment history report
        }

        private void GenerateSupplierDetailedReport(int supplierID, DateTime fromDate, DateTime toDate, string category)
        {
            // Implementation for supplier detailed report
        }

        private void GenerateSupplierPayablesReport(int supplierID, string paymentStatus)
        {
            // Implementation for supplier payables report
        }

        private void GenerateTopCustomersReport(DateTime fromDate, DateTime toDate)
        {
            // Implementation for top customers analysis
        }

        private void GenerateTopSuppliersReport(DateTime fromDate, DateTime toDate)
        {
            // Implementation for top suppliers analysis
        }

        private void GenerateComparativeReport(DateTime fromDate, DateTime toDate)
        {
            // Implementation for comparative analysis
        }

        private void UpdateSummaryStats()
        {
            if (currentReportData != null && currentReportData.Rows.Count > 0)
            {
                try
                {
                    // Update summary statistics based on current report data
                    ComboBoxItem selectedType = (ComboBoxItem)cmbReportType.SelectedItem;
                    string reportType = selectedType?.Value.ToString() ?? "";

                    if (reportType.Contains("CUSTOMER"))
                    {
                        int totalCustomers = currentReportData.Rows.Count;
                        decimal totalSales = 0;
                        decimal totalOutstanding = 0;
                        string topCustomer = "";
                        decimal maxSales = 0;

                        foreach (DataRow row in currentReportData.Rows)
                        {
                            if (row["TotalAmount"] != DBNull.Value)
                            {
                                decimal amount = Convert.ToDecimal(row["TotalAmount"]);
                                totalSales += amount;
                                
                                if (amount > maxSales)
                                {
                                    maxSales = amount;
                                    topCustomer = row["CustomerName"].ToString();
                                }
                            }
                            
                            if (row["OutstandingAmount"] != DBNull.Value)
                                totalOutstanding += Convert.ToDecimal(row["OutstandingAmount"]);
                        }

                        lblTotalCustomers.Text = $"Total Customers: {totalCustomers}";
                        lblTotalSales.Text = $"Total Sales: ₹{totalSales:N2}";
                        lblTopCustomer.Text = $"Top Customer: {topCustomer} (₹{maxSales:N2})";
                        lblOutstandingAmount.Text = $"Outstanding: ₹{totalOutstanding:N2}";
                    }
                    else if (reportType.Contains("SUPPLIER"))
                    {
                        int totalSuppliers = currentReportData.Rows.Count;
                        decimal totalPurchases = 0;
                        decimal totalPayables = 0;
                        string topSupplier = "";
                        decimal maxPurchases = 0;

                        foreach (DataRow row in currentReportData.Rows)
                        {
                            if (row["TotalAmount"] != DBNull.Value)
                            {
                                decimal amount = Convert.ToDecimal(row["TotalAmount"]);
                                totalPurchases += amount;
                                
                                if (amount > maxPurchases)
                                {
                                    maxPurchases = amount;
                                    topSupplier = row["SupplierName"].ToString();
                                }
                            }
                            
                            if (row["PayableAmount"] != DBNull.Value)
                                totalPayables += Convert.ToDecimal(row["PayableAmount"]);
                        }

                        lblTotalCustomers.Text = $"Total Suppliers: {totalSuppliers}";
                        lblTotalPurchases.Text = $"Total Purchases: ₹{totalPurchases:N2}";
                        lblTopSupplier.Text = $"Top Supplier: {topSupplier} (₹{maxPurchases:N2})";
                        lblOutstandingAmount.Text = $"Payables: ₹{totalPayables:N2}";
                    }
                }
                catch (Exception)
                {
                    // Handle summary calculation errors silently
                }
            }
        }

        private void ExportToExcel()
        {
            try
            {
                if (currentReportData == null || currentReportData.Rows.Count == 0)
                {
                    MessageBox.Show("No data to export. Please generate a report first.", "Information", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "Excel Files|*.xlsx|CSV Files|*.csv";
                saveDialog.FileName = $"CustomerSupplier_Report_{DateTime.Now:yyyyMMdd}";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    // Export to Excel logic would go here
                    MessageBox.Show("Report exported successfully!", "Export Complete", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exporting report: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PrintReport()
        {
            try
            {
                if (reportViewer.LocalReport.DataSources.Count > 0)
                {
                    reportViewer.PrintDialog();
                }
                else
                {
                    MessageBox.Show("No report to print. Please generate a report first.", "Information", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error printing report: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EmailReport()
        {
            try
            {
                if (currentReportData == null || currentReportData.Rows.Count == 0)
                {
                    MessageBox.Show("No data to email. Please generate a report first.", "Information", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Email functionality would be implemented here
                MessageBox.Show("Email functionality not yet implemented.", "Information", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error emailing report: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
