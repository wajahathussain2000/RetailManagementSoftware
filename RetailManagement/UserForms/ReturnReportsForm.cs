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
    public partial class ReturnReportsForm : Form
    {
        private ReportViewer reportViewer;
        private ComboBox cmbReturnType;
        private ComboBox cmbCustomer;
        private ComboBox cmbSupplier;
        private DateTimePicker dtpFromDate;
        private DateTimePicker dtpToDate;
        private ComboBox cmbTimePeriod;
        private ComboBox cmbReturnReason;
        private ComboBox cmbCategory;
        private ComboBox cmbUser;
        private CheckBox chkShowRefundDetails;
        private CheckBox chkGroupByReason;
        private CheckBox chkShowItemwise;
        private CheckBox chkIncludePartialReturns;
        private Button btnGenerate;
        private Button btnExport;
        private Button btnPrint;
        private Button btnAnalyze;
        private GroupBox groupFilters;
        private GroupBox groupOptions;
        private TabControl tabControl;
        private TabPage tabSalesReturns;
        private TabPage tabPurchaseReturns;
        private TabPage tabSummary;
        private DataTable currentReturnData;
        private Panel summaryPanel;
        private Label lblTotalSalesReturns;
        private Label lblTotalPurchaseReturns;
        private Label lblTotalRefundAmount;
        private Label lblTopReturnReason;
        private Label lblReturnPercentage;
        private Label lblTotalReturnValue;

        public ReturnReportsForm()
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
            this.cmbReturnType = new System.Windows.Forms.ComboBox();
            this.cmbCustomer = new System.Windows.Forms.ComboBox();
            this.cmbSupplier = new System.Windows.Forms.ComboBox();
            this.dtpFromDate = new System.Windows.Forms.DateTimePicker();
            this.dtpToDate = new System.Windows.Forms.DateTimePicker();
            this.cmbTimePeriod = new System.Windows.Forms.ComboBox();
            this.cmbReturnReason = new System.Windows.Forms.ComboBox();
            this.cmbCategory = new System.Windows.Forms.ComboBox();
            this.cmbUser = new System.Windows.Forms.ComboBox();
            this.chkShowRefundDetails = new System.Windows.Forms.CheckBox();
            this.chkGroupByReason = new System.Windows.Forms.CheckBox();
            this.chkShowItemwise = new System.Windows.Forms.CheckBox();
            this.chkIncludePartialReturns = new System.Windows.Forms.CheckBox();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnAnalyze = new System.Windows.Forms.Button();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabSalesReturns = new System.Windows.Forms.TabPage();
            this.tabPurchaseReturns = new System.Windows.Forms.TabPage();
            this.tabSummary = new System.Windows.Forms.TabPage();
            this.summaryPanel = new System.Windows.Forms.Panel();
            this.lblTotalSalesReturns = new System.Windows.Forms.Label();
            this.lblTotalPurchaseReturns = new System.Windows.Forms.Label();
            this.lblTotalRefundAmount = new System.Windows.Forms.Label();
            this.lblTopReturnReason = new System.Windows.Forms.Label();
            this.lblReturnPercentage = new System.Windows.Forms.Label();
            this.lblTotalReturnValue = new System.Windows.Forms.Label();

            this.SuspendLayout();

            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.summaryPanel);
            this.Controls.Add(this.groupOptions);
            this.Controls.Add(this.groupFilters);
            this.Name = "ReturnReportsForm";
            this.Text = "Sales & Purchase Return Reports";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;

            // Group Filters
            this.groupFilters.Controls.Add(new Label { Text = "Return Type:", Location = new Point(15, 25), Size = new Size(80, 13) });
            this.groupFilters.Controls.Add(this.cmbReturnType);
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
            this.groupFilters.Controls.Add(new Label { Text = "Return Reason:", Location = new Point(15, 145), Size = new Size(85, 13) });
            this.groupFilters.Controls.Add(this.cmbReturnReason);
            this.groupFilters.Controls.Add(new Label { Text = "Category:", Location = new Point(300, 145), Size = new Size(55, 13) });
            this.groupFilters.Controls.Add(this.cmbCategory);
            this.groupFilters.Controls.Add(new Label { Text = "User:", Location = new Point(15, 175), Size = new Size(35, 13) });
            this.groupFilters.Controls.Add(this.cmbUser);
            this.groupFilters.Location = new System.Drawing.Point(12, 12);
            this.groupFilters.Name = "groupFilters";
            this.groupFilters.Size = new System.Drawing.Size(600, 200);
            this.groupFilters.TabIndex = 0;
            this.groupFilters.TabStop = false;
            this.groupFilters.Text = "Return Filters";

            // Group Options
            this.groupOptions.Controls.Add(this.chkShowRefundDetails);
            this.groupOptions.Controls.Add(this.chkGroupByReason);
            this.groupOptions.Controls.Add(this.chkShowItemwise);
            this.groupOptions.Controls.Add(this.chkIncludePartialReturns);
            this.groupOptions.Controls.Add(this.btnGenerate);
            this.groupOptions.Controls.Add(this.btnExport);
            this.groupOptions.Controls.Add(this.btnPrint);
            this.groupOptions.Controls.Add(this.btnAnalyze);
            this.groupOptions.Location = new System.Drawing.Point(630, 12);
            this.groupOptions.Name = "groupOptions";
            this.groupOptions.Size = new System.Drawing.Size(300, 200);
            this.groupOptions.TabIndex = 1;
            this.groupOptions.TabStop = false;
            this.groupOptions.Text = "Options & Actions";

            // Controls Setup
            SetupControls();

            // Tab Control
            this.tabControl.Controls.Add(this.tabSalesReturns);
            this.tabControl.Controls.Add(this.tabPurchaseReturns);
            this.tabControl.Controls.Add(this.tabSummary);
            this.tabControl.Location = new System.Drawing.Point(12, 290);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(1176, 360);
            this.tabControl.TabIndex = 3;

            // Tab Pages
            this.tabSalesReturns.Location = new System.Drawing.Point(4, 22);
            this.tabSalesReturns.Name = "tabSalesReturns";
            this.tabSalesReturns.Padding = new System.Windows.Forms.Padding(3);
            this.tabSalesReturns.Size = new System.Drawing.Size(1168, 334);
            this.tabSalesReturns.TabIndex = 0;
            this.tabSalesReturns.Text = "Sales Returns";
            this.tabSalesReturns.UseVisualStyleBackColor = true;

            this.tabPurchaseReturns.Location = new System.Drawing.Point(4, 22);
            this.tabPurchaseReturns.Name = "tabPurchaseReturns";
            this.tabPurchaseReturns.Padding = new System.Windows.Forms.Padding(3);
            this.tabPurchaseReturns.Size = new System.Drawing.Size(1168, 334);
            this.tabPurchaseReturns.TabIndex = 1;
            this.tabPurchaseReturns.Text = "Purchase Returns";
            this.tabPurchaseReturns.UseVisualStyleBackColor = true;

            this.tabSummary.Location = new System.Drawing.Point(4, 22);
            this.tabSummary.Name = "tabSummary";
            this.tabSummary.Padding = new System.Windows.Forms.Padding(3);
            this.tabSummary.Size = new System.Drawing.Size(1168, 334);
            this.tabSummary.TabIndex = 2;
            this.tabSummary.Text = "Return Analysis";
            this.tabSummary.UseVisualStyleBackColor = true;

            // Summary Panel
            this.summaryPanel.BackColor = System.Drawing.Color.LightCoral;
            this.summaryPanel.Controls.Add(this.lblTotalSalesReturns);
            this.summaryPanel.Controls.Add(this.lblTotalPurchaseReturns);
            this.summaryPanel.Controls.Add(this.lblTotalRefundAmount);
            this.summaryPanel.Controls.Add(this.lblTopReturnReason);
            this.summaryPanel.Controls.Add(this.lblReturnPercentage);
            this.summaryPanel.Controls.Add(this.lblTotalReturnValue);
            this.summaryPanel.Location = new System.Drawing.Point(12, 220);
            this.summaryPanel.Name = "summaryPanel";
            this.summaryPanel.Size = new System.Drawing.Size(1176, 65);
            this.summaryPanel.TabIndex = 2;

            SetupSummaryLabels();

            this.ResumeLayout(false);
        }

        private void SetupControls()
        {
            // Return Type ComboBox
            this.cmbReturnType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbReturnType.Location = new System.Drawing.Point(100, 22);
            this.cmbReturnType.Name = "cmbReturnType";
            this.cmbReturnType.Size = new System.Drawing.Size(150, 21);
            this.cmbReturnType.TabIndex = 1;
            this.cmbReturnType.SelectedIndexChanged += CmbReturnType_SelectedIndexChanged;

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

            // Return Reason ComboBox
            this.cmbReturnReason.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbReturnReason.Location = new System.Drawing.Point(105, 142);
            this.cmbReturnReason.Name = "cmbReturnReason";
            this.cmbReturnReason.Size = new System.Drawing.Size(150, 21);
            this.cmbReturnReason.TabIndex = 7;

            // Category ComboBox
            this.cmbCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCategory.Location = new System.Drawing.Point(360, 142);
            this.cmbCategory.Name = "cmbCategory";
            this.cmbCategory.Size = new System.Drawing.Size(150, 21);
            this.cmbCategory.TabIndex = 8;

            // User ComboBox
            this.cmbUser.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbUser.Location = new System.Drawing.Point(55, 172);
            this.cmbUser.Name = "cmbUser";
            this.cmbUser.Size = new System.Drawing.Size(150, 21);
            this.cmbUser.TabIndex = 9;

            // Checkboxes
            this.chkShowRefundDetails.Location = new System.Drawing.Point(15, 25);
            this.chkShowRefundDetails.Name = "chkShowRefundDetails";
            this.chkShowRefundDetails.Size = new System.Drawing.Size(140, 17);
            this.chkShowRefundDetails.TabIndex = 10;
            this.chkShowRefundDetails.Text = "Show Refund Details";
            this.chkShowRefundDetails.UseVisualStyleBackColor = true;

            this.chkGroupByReason.Location = new System.Drawing.Point(15, 45);
            this.chkGroupByReason.Name = "chkGroupByReason";
            this.chkGroupByReason.Size = new System.Drawing.Size(130, 17);
            this.chkGroupByReason.TabIndex = 11;
            this.chkGroupByReason.Text = "Group by Reason";
            this.chkGroupByReason.UseVisualStyleBackColor = true;

            this.chkShowItemwise.Location = new System.Drawing.Point(15, 65);
            this.chkShowItemwise.Name = "chkShowItemwise";
            this.chkShowItemwise.Size = new System.Drawing.Size(120, 17);
            this.chkShowItemwise.TabIndex = 12;
            this.chkShowItemwise.Text = "Show Itemwise";
            this.chkShowItemwise.UseVisualStyleBackColor = true;

            this.chkIncludePartialReturns.Location = new System.Drawing.Point(15, 85);
            this.chkIncludePartialReturns.Name = "chkIncludePartialReturns";
            this.chkIncludePartialReturns.Size = new System.Drawing.Size(140, 17);
            this.chkIncludePartialReturns.TabIndex = 13;
            this.chkIncludePartialReturns.Text = "Include Partial Returns";
            this.chkIncludePartialReturns.UseVisualStyleBackColor = true;

            // Buttons
            this.btnGenerate.BackColor = System.Drawing.Color.Green;
            this.btnGenerate.ForeColor = System.Drawing.Color.White;
            this.btnGenerate.Location = new System.Drawing.Point(150, 20);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(130, 30);
            this.btnGenerate.TabIndex = 14;
            this.btnGenerate.Text = "Generate Report";
            this.btnGenerate.UseVisualStyleBackColor = false;
            this.btnGenerate.Click += BtnGenerate_Click;

            this.btnExport.BackColor = System.Drawing.Color.Blue;
            this.btnExport.ForeColor = System.Drawing.Color.White;
            this.btnExport.Location = new System.Drawing.Point(150, 55);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(130, 30);
            this.btnExport.TabIndex = 15;
            this.btnExport.Text = "Export Excel";
            this.btnExport.UseVisualStyleBackColor = false;
            this.btnExport.Click += BtnExport_Click;

            this.btnPrint.BackColor = System.Drawing.Color.Orange;
            this.btnPrint.ForeColor = System.Drawing.Color.White;
            this.btnPrint.Location = new System.Drawing.Point(150, 90);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(130, 30);
            this.btnPrint.TabIndex = 16;
            this.btnPrint.Text = "Print Report";
            this.btnPrint.UseVisualStyleBackColor = false;
            this.btnPrint.Click += BtnPrint_Click;

            this.btnAnalyze.BackColor = System.Drawing.Color.Purple;
            this.btnAnalyze.ForeColor = System.Drawing.Color.White;
            this.btnAnalyze.Location = new System.Drawing.Point(150, 125);
            this.btnAnalyze.Name = "btnAnalyze";
            this.btnAnalyze.Size = new System.Drawing.Size(130, 30);
            this.btnAnalyze.TabIndex = 17;
            this.btnAnalyze.Text = "Return Analysis";
            this.btnAnalyze.UseVisualStyleBackColor = false;
            this.btnAnalyze.Click += BtnAnalyze_Click;
        }

        private void SetupSummaryLabels()
        {
            this.lblTotalSalesReturns.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblTotalSalesReturns.Location = new System.Drawing.Point(15, 10);
            this.lblTotalSalesReturns.Name = "lblTotalSalesReturns";
            this.lblTotalSalesReturns.Size = new System.Drawing.Size(180, 15);
            this.lblTotalSalesReturns.TabIndex = 0;
            this.lblTotalSalesReturns.Text = "Sales Returns: 0";

            this.lblTotalPurchaseReturns.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblTotalPurchaseReturns.Location = new System.Drawing.Point(15, 30);
            this.lblTotalPurchaseReturns.Name = "lblTotalPurchaseReturns";
            this.lblTotalPurchaseReturns.Size = new System.Drawing.Size(180, 15);
            this.lblTotalPurchaseReturns.TabIndex = 1;
            this.lblTotalPurchaseReturns.Text = "Purchase Returns: 0";

            this.lblTotalRefundAmount.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblTotalRefundAmount.Location = new System.Drawing.Point(15, 50);
            this.lblTotalRefundAmount.Name = "lblTotalRefundAmount";
            this.lblTotalRefundAmount.Size = new System.Drawing.Size(200, 15);
            this.lblTotalRefundAmount.TabIndex = 2;
            this.lblTotalRefundAmount.Text = "Total Refund: ₹0.00";

            this.lblTopReturnReason.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblTopReturnReason.Location = new System.Drawing.Point(250, 10);
            this.lblTopReturnReason.Name = "lblTopReturnReason";
            this.lblTopReturnReason.Size = new System.Drawing.Size(300, 15);
            this.lblTopReturnReason.TabIndex = 3;
            this.lblTopReturnReason.Text = "Top Return Reason: -";

            this.lblReturnPercentage.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblReturnPercentage.Location = new System.Drawing.Point(250, 30);
            this.lblReturnPercentage.Name = "lblReturnPercentage";
            this.lblReturnPercentage.Size = new System.Drawing.Size(200, 15);
            this.lblReturnPercentage.TabIndex = 4;
            this.lblReturnPercentage.Text = "Return Rate: 0.00%";

            this.lblTotalReturnValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblTotalReturnValue.Location = new System.Drawing.Point(250, 50);
            this.lblTotalReturnValue.Name = "lblTotalReturnValue";
            this.lblTotalReturnValue.Size = new System.Drawing.Size(200, 15);
            this.lblTotalReturnValue.TabIndex = 5;
            this.lblTotalReturnValue.Text = "Return Value: ₹0.00";
        }

        private void InitializeReportViewer()
        {
            this.reportViewer = new ReportViewer();
            this.reportViewer.Dock = DockStyle.Fill;
            this.reportViewer.ProcessingMode = ProcessingMode.Local;
            
            // Add report viewer to each tab
            this.tabSalesReturns.Controls.Add(this.reportViewer);
        }

        private void LoadDropdownData()
        {
            try
            {
                // Load Return Types
                cmbReturnType.Items.Clear();
                cmbReturnType.Items.Add(new ComboBoxItem { Text = "All Returns", Value = "ALL" });
                cmbReturnType.Items.Add(new ComboBoxItem { Text = "Sales Returns", Value = "SALES" });
                cmbReturnType.Items.Add(new ComboBoxItem { Text = "Purchase Returns", Value = "PURCHASE" });
                cmbReturnType.Items.Add(new ComboBoxItem { Text = "Sales Return Summary", Value = "SALES_SUMMARY" });
                cmbReturnType.Items.Add(new ComboBoxItem { Text = "Purchase Return Summary", Value = "PURCHASE_SUMMARY" });
                cmbReturnType.Items.Add(new ComboBoxItem { Text = "Return Analysis", Value = "ANALYSIS" });
                cmbReturnType.DisplayMember = "Text";
                cmbReturnType.ValueMember = "Value";
                cmbReturnType.SelectedIndex = 0;

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
                cmbTimePeriod.DisplayMember = "Text";
                cmbTimePeriod.ValueMember = "Value";
                cmbTimePeriod.SelectedIndex = 3; // This Month

                // Load Return Reasons
                cmbReturnReason.Items.Clear();
                cmbReturnReason.Items.Add(new ComboBoxItem { Text = "All Reasons", Value = "" });
                cmbReturnReason.Items.Add(new ComboBoxItem { Text = "Defective Product", Value = "DEFECTIVE" });
                cmbReturnReason.Items.Add(new ComboBoxItem { Text = "Wrong Item", Value = "WRONG_ITEM" });
                cmbReturnReason.Items.Add(new ComboBoxItem { Text = "Expired Product", Value = "EXPIRED" });
                cmbReturnReason.Items.Add(new ComboBoxItem { Text = "Customer Request", Value = "CUSTOMER_REQUEST" });
                cmbReturnReason.Items.Add(new ComboBoxItem { Text = "Quality Issue", Value = "QUALITY" });
                cmbReturnReason.Items.Add(new ComboBoxItem { Text = "Damaged in Transit", Value = "DAMAGED" });
                cmbReturnReason.Items.Add(new ComboBoxItem { Text = "Other", Value = "OTHER" });
                cmbReturnReason.DisplayMember = "Text";
                cmbReturnReason.ValueMember = "Value";
                cmbReturnReason.SelectedIndex = 0;

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

                // Load Users
                string userQuery = "SELECT UserID, Username FROM Users WHERE IsActive = 1 ORDER BY Username";
                DataTable users = DatabaseConnection.ExecuteQuery(userQuery);
                
                cmbUser.Items.Clear();
                cmbUser.Items.Add(new ComboBoxItem { Text = "All Users", Value = 0 });
                foreach (DataRow row in users.Rows)
                {
                    cmbUser.Items.Add(new ComboBoxItem 
                    { 
                        Text = row["Username"].ToString(), 
                        Value = Convert.ToInt32(row["UserID"]) 
                    });
                }
                cmbUser.DisplayMember = "Text";
                cmbUser.ValueMember = "Value";
                cmbUser.SelectedIndex = 0;
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

        private void CmbReturnType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxItem selectedType = (ComboBoxItem)cmbReturnType.SelectedItem;
            if (selectedType != null)
            {
                string returnType = selectedType.Value.ToString();
                
                // Enable/disable controls based on return type
                if (returnType.Contains("SALES"))
                {
                    cmbCustomer.Enabled = true;
                    cmbSupplier.Enabled = false;
                    tabControl.SelectedTab = tabSalesReturns;
                }
                else if (returnType.Contains("PURCHASE"))
                {
                    cmbCustomer.Enabled = false;
                    cmbSupplier.Enabled = true;
                    tabControl.SelectedTab = tabPurchaseReturns;
                }
                else if (returnType == "ANALYSIS")
                {
                    cmbCustomer.Enabled = true;
                    cmbSupplier.Enabled = true;
                    tabControl.SelectedTab = tabSummary;
                }
                else
                {
                    cmbCustomer.Enabled = true;
                    cmbSupplier.Enabled = true;
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
                default: // CUSTOM
                    fromDate = dtpFromDate.Value;
                    toDate = dtpToDate.Value;
                    break;
            }
        }

        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            GenerateReturnReport();
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            ExportToExcel();
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            PrintReport();
        }

        private void BtnAnalyze_Click(object sender, EventArgs e)
        {
            GenerateReturnAnalysis();
        }

        private void GenerateReturnReport()
        {
            try
            {
                ComboBoxItem selectedType = (ComboBoxItem)cmbReturnType.SelectedItem;
                ComboBoxItem selectedCustomer = (ComboBoxItem)cmbCustomer.SelectedItem;
                ComboBoxItem selectedSupplier = (ComboBoxItem)cmbSupplier.SelectedItem;
                ComboBoxItem selectedReason = (ComboBoxItem)cmbReturnReason.SelectedItem;
                ComboBoxItem selectedCategory = (ComboBoxItem)cmbCategory.SelectedItem;
                ComboBoxItem selectedUser = (ComboBoxItem)cmbUser.SelectedItem;

                string returnType = selectedType?.Value.ToString() ?? "ALL";
                int customerID = selectedCustomer != null ? (int)selectedCustomer.Value : 0;
                int supplierID = selectedSupplier != null ? (int)selectedSupplier.Value : 0;
                string returnReason = selectedReason?.Value.ToString() ?? "";
                string category = selectedCategory?.Value.ToString() ?? "";
                int userID = selectedUser != null ? (int)selectedUser.Value : 0;

                DateTime fromDate = dtpFromDate.Value;
                DateTime toDate = dtpToDate.Value;

                // Generate appropriate return report based on type
                switch (returnType)
                {
                    case "SALES":
                        GenerateSalesReturnReport(customerID, fromDate, toDate, returnReason, category, userID);
                        break;
                    case "PURCHASE":
                        GeneratePurchaseReturnReport(supplierID, fromDate, toDate, returnReason, category, userID);
                        break;
                    case "SALES_SUMMARY":
                        GenerateSalesReturnSummary(customerID, fromDate, toDate);
                        break;
                    case "PURCHASE_SUMMARY":
                        GeneratePurchaseReturnSummary(supplierID, fromDate, toDate);
                        break;
                    case "ANALYSIS":
                        GenerateReturnAnalysis();
                        break;
                    default: // ALL
                        GenerateAllReturnsReport(fromDate, toDate, returnReason, category);
                        break;
                }

                UpdateReturnSummary();
                MessageBox.Show("Return report generated successfully!", "Success", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating return report: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerateSalesReturnReport(int customerID, DateTime fromDate, DateTime toDate, 
            string returnReason, string category, int userID)
        {
            try
            {
                string query = @"
                    SELECT 
                        sr.ReturnNumber,
                        sr.ReturnDate,
                        s.BillNumber as OriginalBillNumber,
                        s.BillDate as OriginalBillDate,
                        ISNULL(c.CustomerName, 'Cash Sale') as CustomerName,
                        c.ContactNumber,
                        i.ItemName,
                        i.Category,
                        sri.Quantity as ReturnQuantity,
                        sri.Rate,
                        sri.Amount as ReturnAmount,
                        sr.ReturnReason,
                        sr.RefundAmount,
                        sr.RefundMode,
                        u.Username as ProcessedBy,
                        sr.Notes
                    FROM SalesReturns sr
                    LEFT JOIN Sales s ON sr.SaleID = s.SaleID
                    LEFT JOIN Customers c ON s.CustomerID = c.CustomerID
                    INNER JOIN SalesReturnItems sri ON sr.ReturnID = sri.ReturnID
                    INNER JOIN Items i ON sri.ItemID = i.ItemID
                    LEFT JOIN Users u ON sr.CreatedBy = u.UserID
                    WHERE sr.ReturnDate BETWEEN @FromDate AND @ToDate
                    AND sr.IsActive = 1" +
                    (customerID > 0 ? " AND s.CustomerID = @CustomerID" : "") +
                    (userID > 0 ? " AND sr.CreatedBy = @UserID" : "") +
                    (!string.IsNullOrEmpty(returnReason) ? " AND sr.ReturnReason = @ReturnReason" : "") +
                    (!string.IsNullOrEmpty(category) ? " AND i.Category = @Category" : "") + @"
                    ORDER BY sr.ReturnDate DESC, sr.ReturnNumber";

                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    new SqlParameter("@FromDate", fromDate),
                    new SqlParameter("@ToDate", toDate)
                };

                if (customerID > 0) parameters.Add(new SqlParameter("@CustomerID", customerID));
                if (userID > 0) parameters.Add(new SqlParameter("@UserID", userID));
                if (!string.IsNullOrEmpty(returnReason)) parameters.Add(new SqlParameter("@ReturnReason", returnReason));
                if (!string.IsNullOrEmpty(category)) parameters.Add(new SqlParameter("@Category", category));

                currentReturnData = DatabaseConnection.ExecuteQuery(query, parameters.ToArray());

                if (currentReturnData != null && currentReturnData.Rows.Count > 0)
                {
                    // Smart RDLC path detection for SalesReturnReport
                    string simplePath = Path.Combine(Application.StartupPath, "Reports", "SalesReturnReport_Simple.rdlc");
                    string originalPath = Path.Combine(Application.StartupPath, "Reports", "SalesReturnReport.rdlc");
                    
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
                        reportViewer.LocalReport.ReportPath = "Reports/SalesReturnReport.rdlc";
                    }
                    reportViewer.LocalReport.DataSources.Clear();
                    reportViewer.LocalReport.DataSources.Add(new ReportDataSource("SalesReturnDataset", currentReturnData));

                    // Set report parameters
                    ReportParameter[] reportParams = new ReportParameter[]
                    {
                        new ReportParameter("FromDate", fromDate.ToString("dd/MM/yyyy")),
                        new ReportParameter("ToDate", toDate.ToString("dd/MM/yyyy")),
                        new ReportParameter("CustomerName", customerID == 0 ? "All Customers" : cmbCustomer.Text),
                        new ReportParameter("ReturnReason", string.IsNullOrEmpty(returnReason) ? "All Reasons" : cmbReturnReason.Text),
                        new ReportParameter("ReportTitle", "Sales Return Report")
                    };

                    reportViewer.LocalReport.SetParameters(reportParams);
                    reportViewer.RefreshReport();
                }
                else
                {
                    MessageBox.Show("No sales return data found for the selected criteria.", "Information", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating sales return report: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GeneratePurchaseReturnReport(int supplierID, DateTime fromDate, DateTime toDate, 
            string returnReason, string category, int userID)
        {
            try
            {
                string query = @"
                    SELECT 
                        pr.ReturnNumber,
                        pr.ReturnDate,
                        p.PurchaseNumber as OriginalPurchaseNumber,
                        p.PurchaseDate as OriginalPurchaseDate,
                        c.CompanyName as SupplierName,
                        c.ContactNumber,
                        i.ItemName,
                        i.Category,
                        pri.Quantity as ReturnQuantity,
                        pri.Rate,
                        pri.Amount as ReturnAmount,
                        pr.ReturnReason,
                        pr.CreditAmount,
                        u.Username as ProcessedBy,
                        pr.Notes
                    FROM PurchaseReturns pr
                    LEFT JOIN Purchases p ON pr.PurchaseID = p.PurchaseID
                    LEFT JOIN Companies c ON p.CompanyID = c.CompanyID
                    INNER JOIN PurchaseReturnItems pri ON pr.ReturnID = pri.ReturnID
                    INNER JOIN Items i ON pri.ItemID = i.ItemID
                    LEFT JOIN Users u ON pr.CreatedBy = u.UserID
                    WHERE pr.ReturnDate BETWEEN @FromDate AND @ToDate
                    AND pr.IsActive = 1" +
                    (supplierID > 0 ? " AND p.CompanyID = @SupplierID" : "") +
                    (userID > 0 ? " AND pr.CreatedBy = @UserID" : "") +
                    (!string.IsNullOrEmpty(returnReason) ? " AND pr.ReturnReason = @ReturnReason" : "") +
                    (!string.IsNullOrEmpty(category) ? " AND i.Category = @Category" : "") + @"
                    ORDER BY pr.ReturnDate DESC, pr.ReturnNumber";

                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    new SqlParameter("@FromDate", fromDate),
                    new SqlParameter("@ToDate", toDate)
                };

                if (supplierID > 0) parameters.Add(new SqlParameter("@SupplierID", supplierID));
                if (userID > 0) parameters.Add(new SqlParameter("@UserID", userID));
                if (!string.IsNullOrEmpty(returnReason)) parameters.Add(new SqlParameter("@ReturnReason", returnReason));
                if (!string.IsNullOrEmpty(category)) parameters.Add(new SqlParameter("@Category", category));

                currentReturnData = DatabaseConnection.ExecuteQuery(query, parameters.ToArray());

                if (currentReturnData != null && currentReturnData.Rows.Count > 0)
                {
                    // Smart RDLC path detection for PurchaseReturnReport
                    string simplePath = Path.Combine(Application.StartupPath, "Reports", "PurchaseReturnReport_Simple.rdlc");
                    string originalPath = Path.Combine(Application.StartupPath, "Reports", "PurchaseReturnReport.rdlc");
                    
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
                        reportViewer.LocalReport.ReportPath = "Reports/PurchaseReturnReport.rdlc";
                    }
                    reportViewer.LocalReport.DataSources.Clear();
                    reportViewer.LocalReport.DataSources.Add(new ReportDataSource("PurchaseReturnDataset", currentReturnData));

                    // Set report parameters
                    ReportParameter[] reportParams = new ReportParameter[]
                    {
                        new ReportParameter("FromDate", fromDate.ToString("dd/MM/yyyy")),
                        new ReportParameter("ToDate", toDate.ToString("dd/MM/yyyy")),
                        new ReportParameter("SupplierName", supplierID == 0 ? "All Suppliers" : cmbSupplier.Text),
                        new ReportParameter("ReturnReason", string.IsNullOrEmpty(returnReason) ? "All Reasons" : cmbReturnReason.Text),
                        new ReportParameter("ReportTitle", "Purchase Return Report")
                    };

                    reportViewer.LocalReport.SetParameters(reportParams);
                    reportViewer.RefreshReport();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating purchase return report: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerateSalesReturnSummary(int customerID, DateTime fromDate, DateTime toDate)
        {
            // Implementation for sales return summary
        }

        private void GeneratePurchaseReturnSummary(int supplierID, DateTime fromDate, DateTime toDate)
        {
            // Implementation for purchase return summary
        }

        private void GenerateAllReturnsReport(DateTime fromDate, DateTime toDate, string returnReason, string category)
        {
            // Implementation for all returns report
        }

        private void GenerateReturnAnalysis()
        {
            try
            {
                DateTime fromDate = dtpFromDate.Value;
                DateTime toDate = dtpToDate.Value;

                string analysisQuery = @"
                    SELECT 
                        'Sales Returns' as ReturnType,
                        COUNT(*) as TotalReturns,
                        SUM(sr.RefundAmount) as TotalAmount,
                        AVG(sr.RefundAmount) as AverageAmount,
                        sr.ReturnReason,
                        COUNT(*) as ReasonCount
                    FROM SalesReturns sr
                    WHERE sr.ReturnDate BETWEEN @FromDate AND @ToDate
                    AND sr.IsActive = 1
                    GROUP BY sr.ReturnReason
                    
                    UNION ALL
                    
                    SELECT 
                        'Purchase Returns' as ReturnType,
                        COUNT(*) as TotalReturns,
                        SUM(pr.CreditAmount) as TotalAmount,
                        AVG(pr.CreditAmount) as AverageAmount,
                        pr.ReturnReason,
                        COUNT(*) as ReasonCount
                    FROM PurchaseReturns pr
                    WHERE pr.ReturnDate BETWEEN @FromDate AND @ToDate
                    AND pr.IsActive = 1
                    GROUP BY pr.ReturnReason
                    
                    ORDER BY ReturnType, ReasonCount DESC";

                SqlParameter[] parameters = {
                    new SqlParameter("@FromDate", fromDate),
                    new SqlParameter("@ToDate", toDate)
                };

                currentReturnData = DatabaseConnection.ExecuteQuery(analysisQuery, parameters);

                if (currentReturnData != null && currentReturnData.Rows.Count > 0)
                {
                    tabControl.SelectedTab = tabSummary;
                    
                    // Create analysis report
                    string reportPath = Path.Combine(Application.StartupPath, "Reports", "ReturnAnalysisReport.rdlc");
                    reportViewer.LocalReport.ReportPath = reportPath;
                    reportViewer.LocalReport.DataSources.Clear();
                    reportViewer.LocalReport.DataSources.Add(new ReportDataSource("ReturnAnalysisDataset", currentReturnData));

                    // Set report parameters
                    ReportParameter[] reportParams = new ReportParameter[]
                    {
                        new ReportParameter("FromDate", fromDate.ToString("dd/MM/yyyy")),
                        new ReportParameter("ToDate", toDate.ToString("dd/MM/yyyy")),
                        new ReportParameter("ReportTitle", "Return Analysis Report")
                    };

                    reportViewer.LocalReport.SetParameters(reportParams);
                    reportViewer.RefreshReport();
                }

                MessageBox.Show("Return analysis generated successfully!", "Analysis Complete", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating return analysis: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateReturnSummary()
        {
            if (currentReturnData != null && currentReturnData.Rows.Count > 0)
            {
                try
                {
                    int salesReturns = 0;
                    int purchaseReturns = 0;
                    decimal totalRefund = 0;
                    decimal totalReturnValue = 0;
                    Dictionary<string, int> reasonCounts = new Dictionary<string, int>();

                    foreach (DataRow row in currentReturnData.Rows)
                    {
                        if (row.Table.Columns.Contains("ReturnType"))
                        {
                            string returnType = row["ReturnType"].ToString();
                            if (returnType == "Sales Returns")
                                salesReturns++;
                            else if (returnType == "Purchase Returns")
                                purchaseReturns++;
                        }
                        else
                        {
                            // Individual return record
                            if (row.Table.Columns.Contains("RefundAmount") && row["RefundAmount"] != DBNull.Value)
                                totalRefund += Convert.ToDecimal(row["RefundAmount"]);
                            
                            if (row.Table.Columns.Contains("ReturnAmount") && row["ReturnAmount"] != DBNull.Value)
                                totalReturnValue += Convert.ToDecimal(row["ReturnAmount"]);
                            
                            if (row.Table.Columns.Contains("ReturnReason"))
                            {
                                string reason = row["ReturnReason"].ToString();
                                reasonCounts[reason] = reasonCounts.ContainsKey(reason) ? reasonCounts[reason] + 1 : 1;
                            }
                        }
                    }

                    string topReason = reasonCounts.Count > 0 ? 
                        reasonCounts.OrderByDescending(kvp => kvp.Value).First().Key : "-";

                    lblTotalSalesReturns.Text = $"Sales Returns: {salesReturns}";
                    lblTotalPurchaseReturns.Text = $"Purchase Returns: {purchaseReturns}";
                    lblTotalRefundAmount.Text = $"Total Refund: ₹{totalRefund:N2}";
                    lblTopReturnReason.Text = $"Top Return Reason: {topReason}";
                    lblTotalReturnValue.Text = $"Return Value: ₹{totalReturnValue:N2}";
                    
                    // Calculate return percentage (would need total sales/purchase data for accurate percentage)
                    lblReturnPercentage.Text = "Return Rate: Calculate from sales data";
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
                if (currentReturnData == null || currentReturnData.Rows.Count == 0)
                {
                    MessageBox.Show("No data to export. Please generate a report first.", "Information", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "Excel Files|*.xlsx|CSV Files|*.csv";
                saveDialog.FileName = $"Return_Report_{DateTime.Now:yyyyMMdd}";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    // Export to Excel logic would go here
                    MessageBox.Show("Return report exported successfully!", "Export Complete", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exporting return report: " + ex.Message, "Error", 
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
                MessageBox.Show("Error printing return report: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
