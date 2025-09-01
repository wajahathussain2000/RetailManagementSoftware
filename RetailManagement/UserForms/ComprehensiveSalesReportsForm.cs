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
    public partial class ComprehensiveSalesReportsForm : Form
    {
        private ReportViewer reportViewer;
        private ComboBox cmbReportType;
        private ComboBox cmbTimePeriod;
        private DateTimePicker dtpFromDate;
        private DateTimePicker dtpToDate;
        private ComboBox cmbMonth;
        private ComboBox cmbYear;
        private ComboBox cmbCategory;
        private ComboBox cmbSalesPerson;
        private CheckBox chkIncludeReturns;
        private CheckBox chkGroupByCategory;
        private Button btnGenerate;
        private Button btnExport;
        private Button btnPrint;
        private Button btnRefresh;
        private GroupBox groupFilters;
        private GroupBox groupOptions;
        private TabControl tabControl;
        private TabPage tabReports;
        private TabPage tabCharts;
        private DataTable currentReportData;
        private Panel chartPanel;
        private Label lblTotalSales;
        private Label lblTotalTransactions;
        private Label lblAverageTransaction;
        private Label lblTopProduct;

        public ComprehensiveSalesReportsForm()
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
            this.cmbTimePeriod = new System.Windows.Forms.ComboBox();
            this.dtpFromDate = new System.Windows.Forms.DateTimePicker();
            this.dtpToDate = new System.Windows.Forms.DateTimePicker();
            this.cmbMonth = new System.Windows.Forms.ComboBox();
            this.cmbYear = new System.Windows.Forms.ComboBox();
            this.cmbCategory = new System.Windows.Forms.ComboBox();
            this.cmbSalesPerson = new System.Windows.Forms.ComboBox();
            this.chkIncludeReturns = new System.Windows.Forms.CheckBox();
            this.chkGroupByCategory = new System.Windows.Forms.CheckBox();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabReports = new System.Windows.Forms.TabPage();
            this.tabCharts = new System.Windows.Forms.TabPage();
            this.chartPanel = new System.Windows.Forms.Panel();
            this.lblTotalSales = new System.Windows.Forms.Label();
            this.lblTotalTransactions = new System.Windows.Forms.Label();
            this.lblAverageTransaction = new System.Windows.Forms.Label();
            this.lblTopProduct = new System.Windows.Forms.Label();
            
            this.groupFilters.SuspendLayout();
            this.groupOptions.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabReports.SuspendLayout();
            this.tabCharts.SuspendLayout();
            this.chartPanel.SuspendLayout();
            this.SuspendLayout();
            
            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1400, 800);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.groupOptions);
            this.Controls.Add(this.groupFilters);
            this.Name = "ComprehensiveSalesReportsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Comprehensive Sales Reports - Retail Management System";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            
            // groupFilters
            this.groupFilters.Controls.Add(this.cmbSalesPerson);
            this.groupFilters.Controls.Add(this.cmbCategory);
            this.groupFilters.Controls.Add(this.cmbYear);
            this.groupFilters.Controls.Add(this.cmbMonth);
            this.groupFilters.Controls.Add(this.dtpToDate);
            this.groupFilters.Controls.Add(this.dtpFromDate);
            this.groupFilters.Controls.Add(this.cmbTimePeriod);
            this.groupFilters.Controls.Add(this.cmbReportType);
            this.groupFilters.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupFilters.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.groupFilters.Location = new System.Drawing.Point(0, 0);
            this.groupFilters.Name = "groupFilters";
            this.groupFilters.Size = new System.Drawing.Size(1400, 80);
            this.groupFilters.TabIndex = 0;
            this.groupFilters.TabStop = false;
            this.groupFilters.Text = "Report Filters";
            
            // Report Type
            this.cmbReportType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbReportType.FormattingEnabled = true;
            this.cmbReportType.Location = new System.Drawing.Point(15, 25);
            this.cmbReportType.Name = "cmbReportType";
            this.cmbReportType.Size = new System.Drawing.Size(150, 21);
            this.cmbReportType.TabIndex = 0;
            this.cmbReportType.SelectedIndexChanged += new System.EventHandler(this.cmbReportType_SelectedIndexChanged);
            
            // Time Period
            this.cmbTimePeriod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTimePeriod.FormattingEnabled = true;
            this.cmbTimePeriod.Location = new System.Drawing.Point(180, 25);
            this.cmbTimePeriod.Name = "cmbTimePeriod";
            this.cmbTimePeriod.Size = new System.Drawing.Size(120, 21);
            this.cmbTimePeriod.TabIndex = 1;
            this.cmbTimePeriod.SelectedIndexChanged += new System.EventHandler(this.cmbTimePeriod_SelectedIndexChanged);
            
            // From Date
            this.dtpFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFromDate.Location = new System.Drawing.Point(315, 25);
            this.dtpFromDate.Name = "dtpFromDate";
            this.dtpFromDate.Size = new System.Drawing.Size(100, 20);
            this.dtpFromDate.TabIndex = 2;
            
            // To Date
            this.dtpToDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpToDate.Location = new System.Drawing.Point(425, 25);
            this.dtpToDate.Name = "dtpToDate";
            this.dtpToDate.Size = new System.Drawing.Size(100, 20);
            this.dtpToDate.TabIndex = 3;
            
            // Month
            this.cmbMonth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMonth.FormattingEnabled = true;
            this.cmbMonth.Location = new System.Drawing.Point(315, 25);
            this.cmbMonth.Name = "cmbMonth";
            this.cmbMonth.Size = new System.Drawing.Size(100, 21);
            this.cmbMonth.TabIndex = 4;
            this.cmbMonth.Visible = false;
            
            // Year
            this.cmbYear.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbYear.FormattingEnabled = true;
            this.cmbYear.Location = new System.Drawing.Point(425, 25);
            this.cmbYear.Name = "cmbYear";
            this.cmbYear.Size = new System.Drawing.Size(80, 21);
            this.cmbYear.TabIndex = 5;
            this.cmbYear.Visible = false;
            
            // Category
            this.cmbCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCategory.FormattingEnabled = true;
            this.cmbCategory.Location = new System.Drawing.Point(540, 25);
            this.cmbCategory.Name = "cmbCategory";
            this.cmbCategory.Size = new System.Drawing.Size(120, 21);
            this.cmbCategory.TabIndex = 6;
            
            // Sales Person
            this.cmbSalesPerson.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSalesPerson.FormattingEnabled = true;
            this.cmbSalesPerson.Location = new System.Drawing.Point(675, 25);
            this.cmbSalesPerson.Name = "cmbSalesPerson";
            this.cmbSalesPerson.Size = new System.Drawing.Size(120, 21);
            this.cmbSalesPerson.TabIndex = 7;
            
            // groupOptions
            this.groupOptions.Controls.Add(this.btnRefresh);
            this.groupOptions.Controls.Add(this.btnPrint);
            this.groupOptions.Controls.Add(this.btnExport);
            this.groupOptions.Controls.Add(this.btnGenerate);
            this.groupOptions.Controls.Add(this.chkGroupByCategory);
            this.groupOptions.Controls.Add(this.chkIncludeReturns);
            this.groupOptions.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupOptions.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.groupOptions.Location = new System.Drawing.Point(0, 80);
            this.groupOptions.Name = "groupOptions";
            this.groupOptions.Size = new System.Drawing.Size(1400, 60);
            this.groupOptions.TabIndex = 1;
            this.groupOptions.TabStop = false;
            this.groupOptions.Text = "Report Options";
            
            // Include Returns
            this.chkIncludeReturns.AutoSize = true;
            this.chkIncludeReturns.Location = new System.Drawing.Point(15, 25);
            this.chkIncludeReturns.Name = "chkIncludeReturns";
            this.chkIncludeReturns.Size = new System.Drawing.Size(120, 19);
            this.chkIncludeReturns.TabIndex = 0;
            this.chkIncludeReturns.Text = "Include Returns";
            this.chkIncludeReturns.UseVisualStyleBackColor = true;
            
            // Group By Category
            this.chkGroupByCategory.AutoSize = true;
            this.chkGroupByCategory.Location = new System.Drawing.Point(150, 25);
            this.chkGroupByCategory.Name = "chkGroupByCategory";
            this.chkGroupByCategory.Size = new System.Drawing.Size(140, 19);
            this.chkGroupByCategory.TabIndex = 1;
            this.chkGroupByCategory.Text = "Group by Category";
            this.chkGroupByCategory.UseVisualStyleBackColor = true;
            
            // Generate Button
            this.btnGenerate.BackColor = System.Drawing.Color.Green;
            this.btnGenerate.ForeColor = System.Drawing.Color.White;
            this.btnGenerate.Location = new System.Drawing.Point(310, 22);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(90, 25);
            this.btnGenerate.TabIndex = 2;
            this.btnGenerate.Text = "Generate";
            this.btnGenerate.UseVisualStyleBackColor = false;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            
            // Export Button
            this.btnExport.BackColor = System.Drawing.Color.Blue;
            this.btnExport.ForeColor = System.Drawing.Color.White;
            this.btnExport.Location = new System.Drawing.Point(410, 22);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(90, 25);
            this.btnExport.TabIndex = 3;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = false;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            
            // Print Button
            this.btnPrint.BackColor = System.Drawing.Color.DarkBlue;
            this.btnPrint.ForeColor = System.Drawing.Color.White;
            this.btnPrint.Location = new System.Drawing.Point(510, 22);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(90, 25);
            this.btnPrint.TabIndex = 4;
            this.btnPrint.Text = "Print";
            this.btnPrint.UseVisualStyleBackColor = false;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            
            // Refresh Button
            this.btnRefresh.BackColor = System.Drawing.Color.Orange;
            this.btnRefresh.ForeColor = System.Drawing.Color.White;
            this.btnRefresh.Location = new System.Drawing.Point(610, 22);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(90, 25);
            this.btnRefresh.TabIndex = 5;
            this.btnRefresh.Text = "Refresh Data";
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            
            // Tab Control
            this.tabControl.Controls.Add(this.tabReports);
            this.tabControl.Controls.Add(this.tabCharts);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 140);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(1400, 660);
            this.tabControl.TabIndex = 2;
            
            // Tab Reports
            this.tabReports.Location = new System.Drawing.Point(4, 22);
            this.tabReports.Name = "tabReports";
            this.tabReports.Padding = new System.Windows.Forms.Padding(3);
            this.tabReports.Size = new System.Drawing.Size(1392, 634);
            this.tabReports.TabIndex = 0;
            this.tabReports.Text = "Detailed Reports";
            this.tabReports.UseVisualStyleBackColor = true;
            
            // Tab Charts
            this.tabCharts.Controls.Add(this.chartPanel);
            this.tabCharts.Location = new System.Drawing.Point(4, 22);
            this.tabCharts.Name = "tabCharts";
            this.tabCharts.Padding = new System.Windows.Forms.Padding(3);
            this.tabCharts.Size = new System.Drawing.Size(1392, 634);
            this.tabCharts.TabIndex = 1;
            this.tabCharts.Text = "Charts & Analytics";
            this.tabCharts.UseVisualStyleBackColor = true;
            
            // Chart Panel
            this.chartPanel.Controls.Add(this.lblTopProduct);
            this.chartPanel.Controls.Add(this.lblAverageTransaction);
            this.chartPanel.Controls.Add(this.lblTotalTransactions);
            this.chartPanel.Controls.Add(this.lblTotalSales);
            this.chartPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartPanel.Location = new System.Drawing.Point(3, 3);
            this.chartPanel.Name = "chartPanel";
            this.chartPanel.Size = new System.Drawing.Size(1386, 628);
            this.chartPanel.TabIndex = 0;
            
            // Summary Labels
            this.lblTotalSales.AutoSize = true;
            this.lblTotalSales.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold);
            this.lblTotalSales.ForeColor = System.Drawing.Color.Green;
            this.lblTotalSales.Location = new System.Drawing.Point(20, 20);
            this.lblTotalSales.Name = "lblTotalSales";
            this.lblTotalSales.Size = new System.Drawing.Size(200, 26);
            this.lblTotalSales.TabIndex = 0;
            this.lblTotalSales.Text = "Total Sales: ₹0.00";
            
            this.lblTotalTransactions.AutoSize = true;
            this.lblTotalTransactions.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold);
            this.lblTotalTransactions.ForeColor = System.Drawing.Color.Blue;
            this.lblTotalTransactions.Location = new System.Drawing.Point(20, 60);
            this.lblTotalTransactions.Name = "lblTotalTransactions";
            this.lblTotalTransactions.Size = new System.Drawing.Size(200, 24);
            this.lblTotalTransactions.TabIndex = 1;
            this.lblTotalTransactions.Text = "Total Transactions: 0";
            
            this.lblAverageTransaction.AutoSize = true;
            this.lblAverageTransaction.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lblAverageTransaction.ForeColor = System.Drawing.Color.Purple;
            this.lblAverageTransaction.Location = new System.Drawing.Point(20, 100);
            this.lblAverageTransaction.Name = "lblAverageTransaction";
            this.lblAverageTransaction.Size = new System.Drawing.Size(250, 20);
            this.lblAverageTransaction.TabIndex = 2;
            this.lblAverageTransaction.Text = "Average Transaction: ₹0.00";
            
            this.lblTopProduct.AutoSize = true;
            this.lblTopProduct.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lblTopProduct.ForeColor = System.Drawing.Color.DarkOrange;
            this.lblTopProduct.Location = new System.Drawing.Point(20, 140);
            this.lblTopProduct.Name = "lblTopProduct";
            this.lblTopProduct.Size = new System.Drawing.Size(200, 20);
            this.lblTopProduct.TabIndex = 3;
            this.lblTopProduct.Text = "Top Selling Product: N/A";
            
            this.groupFilters.ResumeLayout(false);
            this.groupOptions.ResumeLayout(false);
            this.groupOptions.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.tabReports.ResumeLayout(false);
            this.tabCharts.ResumeLayout(false);
            this.chartPanel.ResumeLayout(false);
            this.chartPanel.PerformLayout();
            this.ResumeLayout(false);
        }

        private void InitializeReportViewer()
        {
            reportViewer = new ReportViewer
            {
                Dock = DockStyle.Fill,
                ProcessingMode = ProcessingMode.Local,
                ShowBackButton = false,
                ShowCredentialPrompts = false,
                ShowDocumentMapButton = false,
                ShowFindControls = true,
                ShowPageNavigationControls = true,
                ShowParameterPrompts = false,
                ShowPrintButton = true,
                ShowProgress = true,
                ShowRefreshButton = true,
                ShowStopButton = true,
                ShowToolBar = true,
                ShowZoomControl = true
            };
            
            tabReports.Controls.Add(reportViewer);
        }

        private void LoadDropdownData()
        {
            try
            {
                // Load Report Types
                cmbReportType.Items.Clear();
                cmbReportType.Items.Add(new ComboBoxItem { Text = "Sales Summary", Value = "SUMMARY" });
                cmbReportType.Items.Add(new ComboBoxItem { Text = "Sales Details", Value = "DETAILS" });
                cmbReportType.Items.Add(new ComboBoxItem { Text = "Product Performance", Value = "PRODUCT" });
                cmbReportType.Items.Add(new ComboBoxItem { Text = "Category Analysis", Value = "CATEGORY" });
                cmbReportType.Items.Add(new ComboBoxItem { Text = "Payment Method Analysis", Value = "PAYMENT" });
                cmbReportType.Items.Add(new ComboBoxItem { Text = "Hourly Sales Pattern", Value = "HOURLY" });
                cmbReportType.Items.Add(new ComboBoxItem { Text = "Comparative Analysis", Value = "COMPARATIVE" });
                cmbReportType.DisplayMember = "Text";
                cmbReportType.ValueMember = "Value";
                cmbReportType.SelectedIndex = 0;
                
                // Load Time Periods
                cmbTimePeriod.Items.Clear();
                cmbTimePeriod.Items.Add(new ComboBoxItem { Text = "Custom Range", Value = "CUSTOM" });
                cmbTimePeriod.Items.Add(new ComboBoxItem { Text = "Today", Value = "TODAY" });
                cmbTimePeriod.Items.Add(new ComboBoxItem { Text = "Yesterday", Value = "YESTERDAY" });
                cmbTimePeriod.Items.Add(new ComboBoxItem { Text = "This Week", Value = "THISWEEK" });
                cmbTimePeriod.Items.Add(new ComboBoxItem { Text = "Last Week", Value = "LASTWEEK" });
                cmbTimePeriod.Items.Add(new ComboBoxItem { Text = "This Month", Value = "THISMONTH" });
                cmbTimePeriod.Items.Add(new ComboBoxItem { Text = "Last Month", Value = "LASTMONTH" });
                cmbTimePeriod.Items.Add(new ComboBoxItem { Text = "This Quarter", Value = "THISQUARTER" });
                cmbTimePeriod.Items.Add(new ComboBoxItem { Text = "Last Quarter", Value = "LASTQUARTER" });
                cmbTimePeriod.Items.Add(new ComboBoxItem { Text = "This Year", Value = "THISYEAR" });
                cmbTimePeriod.Items.Add(new ComboBoxItem { Text = "Last Year", Value = "LASTYEAR" });
                cmbTimePeriod.Items.Add(new ComboBoxItem { Text = "Monthly View", Value = "MONTHLY" });
                cmbTimePeriod.Items.Add(new ComboBoxItem { Text = "Yearly View", Value = "YEARLY" });
                cmbTimePeriod.DisplayMember = "Text";
                cmbTimePeriod.ValueMember = "Value";
                cmbTimePeriod.SelectedIndex = 0;
                
                // Load Months
                cmbMonth.Items.Clear();
                for (int i = 1; i <= 12; i++)
                {
                    cmbMonth.Items.Add(new ComboBoxItem 
                    { 
                        Text = new DateTime(2023, i, 1).ToString("MMMM"), 
                        Value = i 
                    });
                }
                cmbMonth.DisplayMember = "Text";
                cmbMonth.ValueMember = "Value";
                cmbMonth.SelectedIndex = DateTime.Now.Month - 1;
                
                // Load Years
                cmbYear.Items.Clear();
                int currentYear = DateTime.Now.Year;
                for (int i = currentYear - 5; i <= currentYear + 1; i++)
                {
                    cmbYear.Items.Add(new ComboBoxItem { Text = i.ToString(), Value = i });
                }
                cmbYear.DisplayMember = "Text";
                cmbYear.ValueMember = "Value";
                cmbYear.SelectedValue = currentYear;
                
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
                
                // Load Sales Persons
                string salesPersonQuery = @"SELECT DISTINCT u.UserID, u.Username, u.FullName 
                                          FROM Users u 
                                          INNER JOIN Sales s ON u.UserID = s.CreatedBy 
                                          WHERE u.IsActive = 1 
                                          ORDER BY u.FullName";
                DataTable salesPersons = DatabaseConnection.ExecuteQuery(salesPersonQuery);
                
                cmbSalesPerson.Items.Clear();
                cmbSalesPerson.Items.Add(new ComboBoxItem { Text = "All Sales Persons", Value = 0 });
                foreach (DataRow row in salesPersons.Rows)
                {
                    cmbSalesPerson.Items.Add(new ComboBoxItem 
                    { 
                        Text = row["FullName"].ToString(), 
                        Value = Convert.ToInt32(row["UserID"]) 
                    });
                }
                cmbSalesPerson.DisplayMember = "Text";
                cmbSalesPerson.ValueMember = "Value";
                cmbSalesPerson.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading dropdown data: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetDefaultDates()
        {
            dtpFromDate.Value = DateTime.Today.AddDays(-30); // Last 30 days
            dtpToDate.Value = DateTime.Today;
        }

        private void cmbReportType_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Enable/disable relevant options based on report type
            ComboBoxItem selectedType = (ComboBoxItem)cmbReportType.SelectedItem;
            if (selectedType != null)
            {
                string reportType = selectedType.Value.ToString();
                
                // Show/hide category grouping option
                chkGroupByCategory.Visible = (reportType == "SUMMARY" || reportType == "DETAILS" || reportType == "PRODUCT");
                
                // Enable category filter for relevant reports
                cmbCategory.Enabled = (reportType != "CATEGORY");
            }
        }

        private void cmbTimePeriod_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxItem selectedPeriod = (ComboBoxItem)cmbTimePeriod.SelectedItem;
            if (selectedPeriod != null)
            {
                string period = selectedPeriod.Value.ToString();
                
                // Show/hide date controls based on period selection
                bool showCustomDates = (period == "CUSTOM");
                bool showMonthYear = (period == "MONTHLY" || period == "YEARLY");
                
                dtpFromDate.Visible = showCustomDates;
                dtpToDate.Visible = showCustomDates;
                cmbMonth.Visible = showMonthYear && period == "MONTHLY";
                cmbYear.Visible = showMonthYear;
                
                // Set automatic date ranges
                if (!showCustomDates && !showMonthYear)
                {
                    SetAutomaticDateRange(period);
                }
            }
        }

        private void SetAutomaticDateRange(string period)
        {
            DateTime now = DateTime.Now;
            
            switch (period)
            {
                case "TODAY":
                    dtpFromDate.Value = now.Date;
                    dtpToDate.Value = now.Date;
                    break;
                case "YESTERDAY":
                    dtpFromDate.Value = now.Date.AddDays(-1);
                    dtpToDate.Value = now.Date.AddDays(-1);
                    break;
                case "THISWEEK":
                    int daysFromMonday = (int)now.DayOfWeek - 1;
                    if (daysFromMonday < 0) daysFromMonday = 6; // Sunday
                    dtpFromDate.Value = now.Date.AddDays(-daysFromMonday);
                    dtpToDate.Value = now.Date;
                    break;
                case "LASTWEEK":
                    int daysFromLastMonday = (int)now.DayOfWeek - 1 + 7;
                    if (daysFromLastMonday >= 7) daysFromLastMonday -= 7;
                    dtpFromDate.Value = now.Date.AddDays(-daysFromLastMonday - 6);
                    dtpToDate.Value = now.Date.AddDays(-daysFromLastMonday);
                    break;
                case "THISMONTH":
                    dtpFromDate.Value = new DateTime(now.Year, now.Month, 1);
                    dtpToDate.Value = now.Date;
                    break;
                case "LASTMONTH":
                    DateTime lastMonth = now.AddMonths(-1);
                    dtpFromDate.Value = new DateTime(lastMonth.Year, lastMonth.Month, 1);
                    dtpToDate.Value = new DateTime(lastMonth.Year, lastMonth.Month, DateTime.DaysInMonth(lastMonth.Year, lastMonth.Month));
                    break;
                case "THISQUARTER":
                    int currentQuarter = (now.Month - 1) / 3 + 1;
                    dtpFromDate.Value = new DateTime(now.Year, (currentQuarter - 1) * 3 + 1, 1);
                    dtpToDate.Value = now.Date;
                    break;
                case "LASTQUARTER":
                    int lastQuarter = (now.Month - 1) / 3;
                    if (lastQuarter == 0)
                    {
                        lastQuarter = 4;
                        dtpFromDate.Value = new DateTime(now.Year - 1, 10, 1);
                        dtpToDate.Value = new DateTime(now.Year - 1, 12, 31);
                    }
                    else
                    {
                        dtpFromDate.Value = new DateTime(now.Year, (lastQuarter - 1) * 3 + 1, 1);
                        dtpToDate.Value = new DateTime(now.Year, lastQuarter * 3, DateTime.DaysInMonth(now.Year, lastQuarter * 3));
                    }
                    break;
                case "THISYEAR":
                    dtpFromDate.Value = new DateTime(now.Year, 1, 1);
                    dtpToDate.Value = now.Date;
                    break;
                case "LASTYEAR":
                    dtpFromDate.Value = new DateTime(now.Year - 1, 1, 1);
                    dtpToDate.Value = new DateTime(now.Year - 1, 12, 31);
                    break;
            }
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            try
            {
                GenerateReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating report: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerateReport()
        {
            try
            {
                // Get filter values
                ComboBoxItem selectedType = (ComboBoxItem)cmbReportType.SelectedItem;
                ComboBoxItem selectedPeriod = (ComboBoxItem)cmbTimePeriod.SelectedItem;
                ComboBoxItem selectedCategory = (ComboBoxItem)cmbCategory.SelectedItem;
                ComboBoxItem selectedSalesPerson = (ComboBoxItem)cmbSalesPerson.SelectedItem;
                
                string reportType = selectedType?.Value.ToString() ?? "SUMMARY";
                string timePeriod = selectedPeriod?.Value.ToString() ?? "CUSTOM";
                string category = selectedCategory?.Value.ToString() ?? "";
                int salesPersonID = selectedSalesPerson != null ? (int)selectedSalesPerson.Value : 0;
                
                DateTime fromDate, toDate;
                GetDateRange(timePeriod, out fromDate, out toDate);
                
                // Call appropriate stored procedure based on report type
                string storedProcedure = GetStoredProcedureName(reportType);
                currentReportData = ExecuteReportQuery(storedProcedure, fromDate, toDate, category, salesPersonID, 
                    chkIncludeReturns.Checked, chkGroupByCategory.Checked);
                
                if (currentReportData != null && currentReportData.Rows.Count > 0)
                {
                    // Generate the report
                    string reportPath = GetReportPath(reportType);
                    GenerateRDLCReport(reportPath, reportType, fromDate, toDate);
                    
                    // Update analytics
                    UpdateAnalytics();
                }
                else
                {
                    MessageBox.Show("No data found for the selected criteria.", "Information",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearReport();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating report: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GetDateRange(string timePeriod, out DateTime fromDate, out DateTime toDate)
        {
            if (timePeriod == "MONTHLY")
            {
                ComboBoxItem selectedMonth = (ComboBoxItem)cmbMonth.SelectedItem;
                ComboBoxItem selectedYear = (ComboBoxItem)cmbYear.SelectedItem;
                
                int month = selectedMonth != null ? (int)selectedMonth.Value : DateTime.Now.Month;
                int year = selectedYear != null ? (int)selectedYear.Value : DateTime.Now.Year;
                
                fromDate = new DateTime(year, month, 1);
                toDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));
            }
            else if (timePeriod == "YEARLY")
            {
                ComboBoxItem selectedYear = (ComboBoxItem)cmbYear.SelectedItem;
                int year = selectedYear != null ? (int)selectedYear.Value : DateTime.Now.Year;
                
                fromDate = new DateTime(year, 1, 1);
                toDate = new DateTime(year, 12, 31);
            }
            else
            {
                fromDate = dtpFromDate.Value.Date;
                toDate = dtpToDate.Value.Date;
            }
        }

        private string GetStoredProcedureName(string reportType)
        {
            switch (reportType)
            {
                case "SUMMARY": return "sp_GetSalesSummaryReport";
                case "DETAILS": return "sp_GetSalesDetailsReport";
                case "PRODUCT": return "sp_GetProductPerformanceReport";
                case "CATEGORY": return "sp_GetCategoryAnalysisReport";
                case "PAYMENT": return "sp_GetPaymentMethodAnalysisReport";
                case "HOURLY": return "sp_GetHourlySalesPatternReport";
                case "COMPARATIVE": return "sp_GetComparativeSalesReport";
                default: return "sp_GetSalesSummaryReport";
            }
        }

        private string GetReportPath(string reportType)
        {
            switch (reportType)
            {
                case "SUMMARY": return "Reports/SalesSummaryReport.rdlc";
                case "DETAILS": return "Reports/SalesDetailsReport.rdlc";
                case "PRODUCT": return "Reports/ProductPerformanceReport.rdlc";
                case "CATEGORY": return "Reports/CategoryAnalysisReport.rdlc";
                case "PAYMENT": return "Reports/PaymentMethodReport.rdlc";
                case "HOURLY": return "Reports/HourlySalesReport.rdlc";
                case "COMPARATIVE": return "Reports/ComparativeSalesReport.rdlc";
                default: return "Reports/SalesSummaryReport.rdlc";
            }
        }

        private DataTable ExecuteReportQuery(string storedProcedure, DateTime fromDate, DateTime toDate, 
            string category, int salesPersonID, bool includeReturns, bool groupByCategory)
        {
            SqlParameter[] parameters = {
                new SqlParameter("@FromDate", fromDate),
                new SqlParameter("@ToDate", toDate.AddDays(1).AddTicks(-1)),
                new SqlParameter("@Category", string.IsNullOrEmpty(category) ? (object)DBNull.Value : category),
                new SqlParameter("@SalesPersonID", salesPersonID == 0 ? (object)DBNull.Value : salesPersonID),
                new SqlParameter("@IncludeReturns", includeReturns),
                new SqlParameter("@GroupByCategory", groupByCategory)
            };
            
            return DatabaseConnection.ExecuteQuery($"EXEC {storedProcedure} @FromDate, @ToDate, @Category, @SalesPersonID, @IncludeReturns, @GroupByCategory", parameters);
        }

        private void GenerateRDLCReport(string reportPath, string reportType, DateTime fromDate, DateTime toDate)
        {
            try
            {
                // Clear previous report
                reportViewer.Reset();
                
                // Set report path
                reportViewer.LocalReport.ReportPath = reportPath;
                
                // Set data source
                string dataSetName = GetDataSetName(reportType);
                ReportDataSource rds = new ReportDataSource(dataSetName, currentReportData);
                reportViewer.LocalReport.DataSources.Clear();
                reportViewer.LocalReport.DataSources.Add(rds);
                
                // Set report parameters
                SetReportParameters(reportType, fromDate, toDate);
                
                // Refresh the report
                reportViewer.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating RDLC report: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetDataSetName(string reportType)
        {
            switch (reportType)
            {
                case "SUMMARY": return "SalesSummaryDataSet";
                case "DETAILS": return "SalesDetailsDataSet";
                case "PRODUCT": return "ProductPerformanceDataSet";
                case "CATEGORY": return "CategoryAnalysisDataSet";
                case "PAYMENT": return "PaymentMethodDataSet";
                case "HOURLY": return "HourlySalesDataSet";
                case "COMPARATIVE": return "ComparativeSalesDataSet";
                default: return "SalesSummaryDataSet";
            }
        }

        private void SetReportParameters(string reportType, DateTime fromDate, DateTime toDate)
        {
            try
            {
                List<ReportParameter> parameters = new List<ReportParameter>();
                
                // Common parameters
                parameters.Add(new ReportParameter("FromDate", fromDate.ToString("dd/MM/yyyy")));
                parameters.Add(new ReportParameter("ToDate", toDate.ToString("dd/MM/yyyy")));
                parameters.Add(new ReportParameter("GeneratedDate", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")));
                parameters.Add(new ReportParameter("CompanyName", "Retail Management System"));
                parameters.Add(new ReportParameter("ReportTitle", GetReportTitle(reportType)));
                
                // Report-specific parameters
                ComboBoxItem selectedCategory = (ComboBoxItem)cmbCategory.SelectedItem;
                ComboBoxItem selectedSalesPerson = (ComboBoxItem)cmbSalesPerson.SelectedItem;
                
                if (selectedCategory != null)
                    parameters.Add(new ReportParameter("CategoryFilter", selectedCategory.Text));
                if (selectedSalesPerson != null)
                    parameters.Add(new ReportParameter("SalesPersonFilter", selectedSalesPerson.Text));
                
                parameters.Add(new ReportParameter("IncludeReturns", chkIncludeReturns.Checked.ToString()));
                parameters.Add(new ReportParameter("GroupByCategory", chkGroupByCategory.Checked.ToString()));
                
                reportViewer.LocalReport.SetParameters(parameters);
            }
            catch (Exception)
            {
                // Handle parameter errors silently
            }
        }

        private string GetReportTitle(string reportType)
        {
            switch (reportType)
            {
                case "SUMMARY": return "Sales Summary Report";
                case "DETAILS": return "Detailed Sales Report";
                case "PRODUCT": return "Product Performance Report";
                case "CATEGORY": return "Category Analysis Report";
                case "PAYMENT": return "Payment Method Analysis Report";
                case "HOURLY": return "Hourly Sales Pattern Report";
                case "COMPARATIVE": return "Comparative Sales Analysis Report";
                default: return "Sales Report";
            }
        }

        private void UpdateAnalytics()
        {
            if (currentReportData != null && currentReportData.Rows.Count > 0)
            {
                try
                {
                    // Calculate analytics based on available columns
                    decimal totalSales = 0;
                    int totalTransactions = currentReportData.Rows.Count;
                    string topProduct = "N/A";
                    
                    // Try to get total sales from different possible column names
                    string[] salesColumns = { "NetAmount", "TotalAmount", "SalesAmount", "Amount" };
                    string salesColumn = null;
                    
                    foreach (string col in salesColumns)
                    {
                        if (currentReportData.Columns.Contains(col))
                        {
                            salesColumn = col;
                            break;
                        }
                    }
                    
                    if (salesColumn != null)
                    {
                        totalSales = currentReportData.AsEnumerable()
                            .Sum(row => row.Field<decimal?>(salesColumn) ?? 0);
                    }
                    
                    // Try to get top product
                    if (currentReportData.Columns.Contains("ItemName") || currentReportData.Columns.Contains("ProductName"))
                    {
                        string productColumn = currentReportData.Columns.Contains("ItemName") ? "ItemName" : "ProductName";
                        var topProductRow = currentReportData.AsEnumerable()
                            .GroupBy(row => row.Field<string>(productColumn))
                            .OrderByDescending(g => g.Sum(row => row.Field<decimal?>(salesColumn ?? "NetAmount") ?? 0))
                            .FirstOrDefault();
                        
                        if (topProductRow != null)
                            topProduct = topProductRow.Key;
                    }
                    
                    // Update labels
                    lblTotalSales.Text = $"Total Sales: ₹{totalSales:N2}";
                    lblTotalTransactions.Text = $"Total Transactions: {totalTransactions:N0}";
                    lblAverageTransaction.Text = totalTransactions > 0 ? 
                        $"Average Transaction: ₹{(totalSales / totalTransactions):N2}" : 
                        "Average Transaction: ₹0.00";
                    lblTopProduct.Text = $"Top Selling Product: {topProduct}";
                }
                catch (Exception)
                {
                    // Handle analytics calculation errors silently
                }
            }
            else
            {
                lblTotalSales.Text = "Total Sales: ₹0.00";
                lblTotalTransactions.Text = "Total Transactions: 0";
                lblAverageTransaction.Text = "Average Transaction: ₹0.00";
                lblTopProduct.Text = "Top Selling Product: N/A";
            }
        }

        private void ClearReport()
        {
            reportViewer.Reset();
            currentReportData = null;
            UpdateAnalytics();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentReportData == null || currentReportData.Rows.Count == 0)
                {
                    MessageBox.Show("Please generate a report first.", "Information",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "PDF Files|*.pdf|Excel Files|*.xlsx|Word Documents|*.docx";
                saveDialog.Title = "Export Sales Report";
                
                ComboBoxItem selectedType = (ComboBoxItem)cmbReportType.SelectedItem;
                string reportName = selectedType?.Text ?? "SalesReport";
                saveDialog.FileName = $"{reportName}_{DateTime.Now:yyyyMMdd_HHmmss}";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    string extension = Path.GetExtension(saveDialog.FileName).ToLower();
                    
                    Warning[] warnings;
                    string[] streamids;
                    string mimeType;
                    string encoding;
                    string deviceInfo = "";
                    
                    byte[] bytes = null;
                    
                    switch (extension)
                    {
                        case ".pdf":
                            deviceInfo = "<DeviceInfo>" +
                                        "<OutputFormat>PDF</OutputFormat>" +
                                        "<PageWidth>11in</PageWidth>" +
                                        "<PageHeight>8.5in</PageHeight>" +
                                        "<MarginTop>0.5in</MarginTop>" +
                                        "<MarginLeft>0.5in</MarginLeft>" +
                                        "<MarginRight>0.5in</MarginRight>" +
                                        "<MarginBottom>0.5in</MarginBottom>" +
                                        "</DeviceInfo>";
                            bytes = reportViewer.LocalReport.Render("PDF", deviceInfo, out mimeType, out encoding, out extension, out streamids, out warnings);
                            break;
                        case ".xlsx":
                            bytes = reportViewer.LocalReport.Render("EXCELOPENXML", deviceInfo, out mimeType, out encoding, out extension, out streamids, out warnings);
                            break;
                        case ".docx":
                            bytes = reportViewer.LocalReport.Render("WORDOPENXML", deviceInfo, out mimeType, out encoding, out extension, out streamids, out warnings);
                            break;
                    }
                    
                    if (bytes != null)
                    {
                        using (FileStream fs = new FileStream(saveDialog.FileName, FileMode.Create))
                        {
                            fs.Write(bytes, 0, bytes.Length);
                        }
                        
                        MessageBox.Show("Report exported successfully!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        
                        // Ask if user wants to open the file
                        if (MessageBox.Show("Do you want to open the exported file?", "Open File",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start(saveDialog.FileName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exporting report: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentReportData == null || currentReportData.Rows.Count == 0)
                {
                    MessageBox.Show("Please generate a report first.", "Information",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Switch to reports tab if not already there
                tabControl.SelectedTab = tabReports;
                
                // Use the ReportViewer's built-in print functionality
                reportViewer.PrintDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error printing report: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                LoadDropdownData();
                MessageBox.Show("Data refreshed successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error refreshing data: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    
}
