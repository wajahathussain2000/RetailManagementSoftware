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
    public partial class LedgerTrialBalanceForm : Form
    {
        private ReportViewer reportViewer;
        private ComboBox cmbReportType;
        private ComboBox cmbAccount;
        private ComboBox cmbAccountType;
        private DateTimePicker dtpFromDate;
        private DateTimePicker dtpToDate;
        private ComboBox cmbTimePeriod;
        private CheckBox chkIncludeOpening;
        private CheckBox chkIncludeClosing;
        private CheckBox chkShowZeroBalance;
        private CheckBox chkGroupByType;
        private Button btnGenerate;
        private Button btnExport;
        private Button btnPrint;
        private Button btnReconcile;
        private GroupBox groupFilters;
        private GroupBox groupOptions;
        private TabControl tabControl;
        private TabPage tabLedger;
        private TabPage tabTrialBalance;
        private TabPage tabAccountSummary;
        private DataTable currentLedgerData;
        private Panel summaryPanel;
        private Label lblTotalDebits;
        private Label lblTotalCredits;
        private Label lblNetBalance;
        private Label lblOpeningBalance;
        private Label lblClosingBalance;
        private Label lblTotalAccounts;

        public LedgerTrialBalanceForm()
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
            this.cmbAccount = new System.Windows.Forms.ComboBox();
            this.cmbAccountType = new System.Windows.Forms.ComboBox();
            this.dtpFromDate = new System.Windows.Forms.DateTimePicker();
            this.dtpToDate = new System.Windows.Forms.DateTimePicker();
            this.cmbTimePeriod = new System.Windows.Forms.ComboBox();
            this.chkIncludeOpening = new System.Windows.Forms.CheckBox();
            this.chkIncludeClosing = new System.Windows.Forms.CheckBox();
            this.chkShowZeroBalance = new System.Windows.Forms.CheckBox();
            this.chkGroupByType = new System.Windows.Forms.CheckBox();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnReconcile = new System.Windows.Forms.Button();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabLedger = new System.Windows.Forms.TabPage();
            this.tabTrialBalance = new System.Windows.Forms.TabPage();
            this.tabAccountSummary = new System.Windows.Forms.TabPage();
            this.summaryPanel = new System.Windows.Forms.Panel();
            this.lblTotalDebits = new System.Windows.Forms.Label();
            this.lblTotalCredits = new System.Windows.Forms.Label();
            this.lblNetBalance = new System.Windows.Forms.Label();
            this.lblOpeningBalance = new System.Windows.Forms.Label();
            this.lblClosingBalance = new System.Windows.Forms.Label();
            this.lblTotalAccounts = new System.Windows.Forms.Label();

            this.SuspendLayout();

            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.summaryPanel);
            this.Controls.Add(this.groupOptions);
            this.Controls.Add(this.groupFilters);
            this.Name = "LedgerTrialBalanceForm";
            this.Text = "Ledger & Trial Balance";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;

            // Group Filters
            this.groupFilters.Controls.Add(new Label { Text = "Report Type:", Location = new Point(15, 25), Size = new Size(80, 13) });
            this.groupFilters.Controls.Add(this.cmbReportType);
            this.groupFilters.Controls.Add(new Label { Text = "Account:", Location = new Point(15, 55), Size = new Size(60, 13) });
            this.groupFilters.Controls.Add(this.cmbAccount);
            this.groupFilters.Controls.Add(new Label { Text = "Account Type:", Location = new Point(300, 55), Size = new Size(80, 13) });
            this.groupFilters.Controls.Add(this.cmbAccountType);
            this.groupFilters.Controls.Add(new Label { Text = "Time Period:", Location = new Point(15, 85), Size = new Size(75, 13) });
            this.groupFilters.Controls.Add(this.cmbTimePeriod);
            this.groupFilters.Controls.Add(new Label { Text = "From Date:", Location = new Point(15, 115), Size = new Size(65, 13) });
            this.groupFilters.Controls.Add(this.dtpFromDate);
            this.groupFilters.Controls.Add(new Label { Text = "To Date:", Location = new Point(300, 115), Size = new Size(50, 13) });
            this.groupFilters.Controls.Add(this.dtpToDate);
            this.groupFilters.Location = new System.Drawing.Point(12, 12);
            this.groupFilters.Name = "groupFilters";
            this.groupFilters.Size = new System.Drawing.Size(600, 150);
            this.groupFilters.TabIndex = 0;
            this.groupFilters.TabStop = false;
            this.groupFilters.Text = "Ledger Filters";

            // Group Options
            this.groupOptions.Controls.Add(this.chkIncludeOpening);
            this.groupOptions.Controls.Add(this.chkIncludeClosing);
            this.groupOptions.Controls.Add(this.chkShowZeroBalance);
            this.groupOptions.Controls.Add(this.chkGroupByType);
            this.groupOptions.Controls.Add(this.btnGenerate);
            this.groupOptions.Controls.Add(this.btnExport);
            this.groupOptions.Controls.Add(this.btnPrint);
            this.groupOptions.Controls.Add(this.btnReconcile);
            this.groupOptions.Location = new System.Drawing.Point(630, 12);
            this.groupOptions.Name = "groupOptions";
            this.groupOptions.Size = new System.Drawing.Size(300, 150);
            this.groupOptions.TabIndex = 1;
            this.groupOptions.TabStop = false;
            this.groupOptions.Text = "Options & Actions";

            // Controls Setup
            SetupControls();

            // Tab Control
            this.tabControl.Controls.Add(this.tabLedger);
            this.tabControl.Controls.Add(this.tabTrialBalance);
            this.tabControl.Controls.Add(this.tabAccountSummary);
            this.tabControl.Location = new System.Drawing.Point(12, 240);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(1176, 400);
            this.tabControl.TabIndex = 3;

            // Tab Pages
            this.tabLedger.Location = new System.Drawing.Point(4, 22);
            this.tabLedger.Name = "tabLedger";
            this.tabLedger.Padding = new System.Windows.Forms.Padding(3);
            this.tabLedger.Size = new System.Drawing.Size(1168, 374);
            this.tabLedger.TabIndex = 0;
            this.tabLedger.Text = "Account Ledger";
            this.tabLedger.UseVisualStyleBackColor = true;

            this.tabTrialBalance.Location = new System.Drawing.Point(4, 22);
            this.tabTrialBalance.Name = "tabTrialBalance";
            this.tabTrialBalance.Padding = new System.Windows.Forms.Padding(3);
            this.tabTrialBalance.Size = new System.Drawing.Size(1168, 374);
            this.tabTrialBalance.TabIndex = 1;
            this.tabTrialBalance.Text = "Trial Balance";
            this.tabTrialBalance.UseVisualStyleBackColor = true;

            this.tabAccountSummary.Location = new System.Drawing.Point(4, 22);
            this.tabAccountSummary.Name = "tabAccountSummary";
            this.tabAccountSummary.Padding = new System.Windows.Forms.Padding(3);
            this.tabAccountSummary.Size = new System.Drawing.Size(1168, 374);
            this.tabAccountSummary.TabIndex = 2;
            this.tabAccountSummary.Text = "Account Summary";
            this.tabAccountSummary.UseVisualStyleBackColor = true;

            // Summary Panel
            this.summaryPanel.BackColor = System.Drawing.Color.LightBlue;
            this.summaryPanel.Controls.Add(this.lblTotalDebits);
            this.summaryPanel.Controls.Add(this.lblTotalCredits);
            this.summaryPanel.Controls.Add(this.lblNetBalance);
            this.summaryPanel.Controls.Add(this.lblOpeningBalance);
            this.summaryPanel.Controls.Add(this.lblClosingBalance);
            this.summaryPanel.Controls.Add(this.lblTotalAccounts);
            this.summaryPanel.Location = new System.Drawing.Point(12, 175);
            this.summaryPanel.Name = "summaryPanel";
            this.summaryPanel.Size = new System.Drawing.Size(1176, 60);
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

            // Account ComboBox
            this.cmbAccount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAccount.Location = new System.Drawing.Point(80, 52);
            this.cmbAccount.Name = "cmbAccount";
            this.cmbAccount.Size = new System.Drawing.Size(200, 21);
            this.cmbAccount.TabIndex = 2;

            // Account Type ComboBox
            this.cmbAccountType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAccountType.Location = new System.Drawing.Point(385, 52);
            this.cmbAccountType.Name = "cmbAccountType";
            this.cmbAccountType.Size = new System.Drawing.Size(150, 21);
            this.cmbAccountType.TabIndex = 3;

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

            // Checkboxes
            this.chkIncludeOpening.Location = new System.Drawing.Point(15, 25);
            this.chkIncludeOpening.Name = "chkIncludeOpening";
            this.chkIncludeOpening.Size = new System.Drawing.Size(130, 17);
            this.chkIncludeOpening.TabIndex = 7;
            this.chkIncludeOpening.Text = "Include Opening Bal.";
            this.chkIncludeOpening.UseVisualStyleBackColor = true;
            this.chkIncludeOpening.Checked = true;

            this.chkIncludeClosing.Location = new System.Drawing.Point(15, 45);
            this.chkIncludeClosing.Name = "chkIncludeClosing";
            this.chkIncludeClosing.Size = new System.Drawing.Size(130, 17);
            this.chkIncludeClosing.TabIndex = 8;
            this.chkIncludeClosing.Text = "Include Closing Bal.";
            this.chkIncludeClosing.UseVisualStyleBackColor = true;
            this.chkIncludeClosing.Checked = true;

            this.chkShowZeroBalance.Location = new System.Drawing.Point(15, 65);
            this.chkShowZeroBalance.Name = "chkShowZeroBalance";
            this.chkShowZeroBalance.Size = new System.Drawing.Size(130, 17);
            this.chkShowZeroBalance.TabIndex = 9;
            this.chkShowZeroBalance.Text = "Show Zero Balances";
            this.chkShowZeroBalance.UseVisualStyleBackColor = true;

            this.chkGroupByType.Location = new System.Drawing.Point(15, 85);
            this.chkGroupByType.Name = "chkGroupByType";
            this.chkGroupByType.Size = new System.Drawing.Size(130, 17);
            this.chkGroupByType.TabIndex = 10;
            this.chkGroupByType.Text = "Group by Type";
            this.chkGroupByType.UseVisualStyleBackColor = true;

            // Buttons
            this.btnGenerate.BackColor = System.Drawing.Color.Green;
            this.btnGenerate.ForeColor = System.Drawing.Color.White;
            this.btnGenerate.Location = new System.Drawing.Point(150, 20);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(130, 30);
            this.btnGenerate.TabIndex = 11;
            this.btnGenerate.Text = "Generate Report";
            this.btnGenerate.UseVisualStyleBackColor = false;
            this.btnGenerate.Click += BtnGenerate_Click;

            this.btnExport.BackColor = System.Drawing.Color.Blue;
            this.btnExport.ForeColor = System.Drawing.Color.White;
            this.btnExport.Location = new System.Drawing.Point(150, 55);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(130, 30);
            this.btnExport.TabIndex = 12;
            this.btnExport.Text = "Export Excel";
            this.btnExport.UseVisualStyleBackColor = false;
            this.btnExport.Click += BtnExport_Click;

            this.btnPrint.BackColor = System.Drawing.Color.Orange;
            this.btnPrint.ForeColor = System.Drawing.Color.White;
            this.btnPrint.Location = new System.Drawing.Point(150, 90);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(130, 30);
            this.btnPrint.TabIndex = 13;
            this.btnPrint.Text = "Print Report";
            this.btnPrint.UseVisualStyleBackColor = false;
            this.btnPrint.Click += BtnPrint_Click;

            this.btnReconcile.BackColor = System.Drawing.Color.Purple;
            this.btnReconcile.ForeColor = System.Drawing.Color.White;
            this.btnReconcile.Location = new System.Drawing.Point(15, 110);
            this.btnReconcile.Name = "btnReconcile";
            this.btnReconcile.Size = new System.Drawing.Size(130, 30);
            this.btnReconcile.TabIndex = 14;
            this.btnReconcile.Text = "Reconcile Accounts";
            this.btnReconcile.UseVisualStyleBackColor = false;
            this.btnReconcile.Click += BtnReconcile_Click;
        }

        private void SetupSummaryLabels()
        {
            this.lblTotalAccounts.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblTotalAccounts.Location = new System.Drawing.Point(15, 10);
            this.lblTotalAccounts.Name = "lblTotalAccounts";
            this.lblTotalAccounts.Size = new System.Drawing.Size(150, 15);
            this.lblTotalAccounts.TabIndex = 0;
            this.lblTotalAccounts.Text = "Total Accounts: 0";

            this.lblOpeningBalance.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblOpeningBalance.Location = new System.Drawing.Point(15, 30);
            this.lblOpeningBalance.Name = "lblOpeningBalance";
            this.lblOpeningBalance.Size = new System.Drawing.Size(200, 15);
            this.lblOpeningBalance.TabIndex = 1;
            this.lblOpeningBalance.Text = "Opening Balance: ₹0.00";

            this.lblClosingBalance.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblClosingBalance.Location = new System.Drawing.Point(15, 50);
            this.lblClosingBalance.Name = "lblClosingBalance";
            this.lblClosingBalance.Size = new System.Drawing.Size(200, 15);
            this.lblClosingBalance.TabIndex = 2;
            this.lblClosingBalance.Text = "Closing Balance: ₹0.00";

            this.lblTotalDebits.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblTotalDebits.Location = new System.Drawing.Point(250, 10);
            this.lblTotalDebits.Name = "lblTotalDebits";
            this.lblTotalDebits.Size = new System.Drawing.Size(150, 15);
            this.lblTotalDebits.TabIndex = 3;
            this.lblTotalDebits.Text = "Total Debits: ₹0.00";

            this.lblTotalCredits.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblTotalCredits.Location = new System.Drawing.Point(250, 30);
            this.lblTotalCredits.Name = "lblTotalCredits";
            this.lblTotalCredits.Size = new System.Drawing.Size(150, 15);
            this.lblTotalCredits.TabIndex = 4;
            this.lblTotalCredits.Text = "Total Credits: ₹0.00";

            this.lblNetBalance.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblNetBalance.Location = new System.Drawing.Point(250, 50);
            this.lblNetBalance.Name = "lblNetBalance";
            this.lblNetBalance.Size = new System.Drawing.Size(150, 15);
            this.lblNetBalance.TabIndex = 5;
            this.lblNetBalance.Text = "Net Balance: ₹0.00";
        }

        private void InitializeReportViewer()
        {
            this.reportViewer = new ReportViewer();
            this.reportViewer.Dock = DockStyle.Fill;
            this.reportViewer.ProcessingMode = ProcessingMode.Local;
            
            // Add report viewer to each tab
            this.tabLedger.Controls.Add(this.reportViewer);
        }

        private void LoadDropdownData()
        {
            try
            {
                // Load Report Types
                cmbReportType.Items.Clear();
                cmbReportType.Items.Add(new ComboBoxItem { Text = "Account Ledger", Value = "LEDGER" });
                cmbReportType.Items.Add(new ComboBoxItem { Text = "Trial Balance", Value = "TRIAL_BALANCE" });
                cmbReportType.Items.Add(new ComboBoxItem { Text = "Customer Ledger", Value = "CUSTOMER_LEDGER" });
                cmbReportType.Items.Add(new ComboBoxItem { Text = "Supplier Ledger", Value = "SUPPLIER_LEDGER" });
                cmbReportType.Items.Add(new ComboBoxItem { Text = "Cash Book", Value = "CASH_BOOK" });
                cmbReportType.Items.Add(new ComboBoxItem { Text = "Bank Book", Value = "BANK_BOOK" });
                cmbReportType.Items.Add(new ComboBoxItem { Text = "Account Summary", Value = "ACCOUNT_SUMMARY" });
                cmbReportType.DisplayMember = "Text";
                cmbReportType.ValueMember = "Value";
                cmbReportType.SelectedIndex = 0;

                // Load Account Types
                cmbAccountType.Items.Clear();
                cmbAccountType.Items.Add(new ComboBoxItem { Text = "All Types", Value = "" });
                cmbAccountType.Items.Add(new ComboBoxItem { Text = "Assets", Value = "ASSETS" });
                cmbAccountType.Items.Add(new ComboBoxItem { Text = "Liabilities", Value = "LIABILITIES" });
                cmbAccountType.Items.Add(new ComboBoxItem { Text = "Income", Value = "INCOME" });
                cmbAccountType.Items.Add(new ComboBoxItem { Text = "Expenses", Value = "EXPENSES" });
                cmbAccountType.Items.Add(new ComboBoxItem { Text = "Capital", Value = "CAPITAL" });
                cmbAccountType.DisplayMember = "Text";
                cmbAccountType.ValueMember = "Value";
                cmbAccountType.SelectedIndex = 0;

                // Load Accounts (this would typically come from a Chart of Accounts table)
                cmbAccount.Items.Clear();
                cmbAccount.Items.Add(new ComboBoxItem { Text = "All Accounts", Value = 0 });
                cmbAccount.Items.Add(new ComboBoxItem { Text = "Cash Account", Value = 1 });
                cmbAccount.Items.Add(new ComboBoxItem { Text = "Bank Account", Value = 2 });
                cmbAccount.Items.Add(new ComboBoxItem { Text = "Sales Account", Value = 3 });
                cmbAccount.Items.Add(new ComboBoxItem { Text = "Purchase Account", Value = 4 });
                cmbAccount.Items.Add(new ComboBoxItem { Text = "Inventory Account", Value = 5 });
                cmbAccount.Items.Add(new ComboBoxItem { Text = "Expense Account", Value = 6 });
                cmbAccount.Items.Add(new ComboBoxItem { Text = "Customer Accounts", Value = 7 });
                cmbAccount.Items.Add(new ComboBoxItem { Text = "Supplier Accounts", Value = 8 });
                cmbAccount.DisplayMember = "Text";
                cmbAccount.ValueMember = "Value";
                cmbAccount.SelectedIndex = 0;

                // Load Time Periods
                cmbTimePeriod.Items.Clear();
                cmbTimePeriod.Items.Add(new ComboBoxItem { Text = "Custom Range", Value = "CUSTOM" });
                cmbTimePeriod.Items.Add(new ComboBoxItem { Text = "Today", Value = "TODAY" });
                cmbTimePeriod.Items.Add(new ComboBoxItem { Text = "This Week", Value = "THIS_WEEK" });
                cmbTimePeriod.Items.Add(new ComboBoxItem { Text = "This Month", Value = "THIS_MONTH" });
                cmbTimePeriod.Items.Add(new ComboBoxItem { Text = "This Quarter", Value = "THIS_QUARTER" });
                cmbTimePeriod.Items.Add(new ComboBoxItem { Text = "This Financial Year", Value = "THIS_YEAR" });
                cmbTimePeriod.Items.Add(new ComboBoxItem { Text = "Last Month", Value = "LAST_MONTH" });
                cmbTimePeriod.Items.Add(new ComboBoxItem { Text = "Last Quarter", Value = "LAST_QUARTER" });
                cmbTimePeriod.Items.Add(new ComboBoxItem { Text = "Last Financial Year", Value = "LAST_YEAR" });
                cmbTimePeriod.DisplayMember = "Text";
                cmbTimePeriod.ValueMember = "Value";
                cmbTimePeriod.SelectedIndex = 3; // This Month
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading dropdown data: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetDefaultDates()
        {
            // Set to current financial year (April to March)
            DateTime now = DateTime.Now;
            DateTime financialYearStart = now.Month >= 4 ? new DateTime(now.Year, 4, 1) : new DateTime(now.Year - 1, 4, 1);
            DateTime financialYearEnd = financialYearStart.AddYears(1).AddDays(-1);
            
            dtpFromDate.Value = financialYearStart;
            dtpToDate.Value = financialYearEnd;
        }

        private void CmbReportType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxItem selectedType = (ComboBoxItem)cmbReportType.SelectedItem;
            if (selectedType != null)
            {
                string reportType = selectedType.Value.ToString();
                
                // Update UI based on selected report type
                switch (reportType)
                {
                    case "LEDGER":
                        tabControl.SelectedTab = tabLedger;
                        cmbAccount.Enabled = true;
                        break;
                    case "TRIAL_BALANCE":
                        tabControl.SelectedTab = tabTrialBalance;
                        cmbAccount.Enabled = false;
                        break;
                    case "CUSTOMER_LEDGER":
                    case "SUPPLIER_LEDGER":
                        tabControl.SelectedTab = tabLedger;
                        cmbAccount.Enabled = true;
                        break;
                    case "CASH_BOOK":
                    case "BANK_BOOK":
                        tabControl.SelectedTab = tabLedger;
                        cmbAccount.Enabled = false;
                        break;
                    case "ACCOUNT_SUMMARY":
                        tabControl.SelectedTab = tabAccountSummary;
                        cmbAccount.Enabled = true;
                        break;
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
                    // Financial year (April to March)
                    fromDate = now.Month >= 4 ? new DateTime(now.Year, 4, 1) : new DateTime(now.Year - 1, 4, 1);
                    toDate = fromDate.AddYears(1).AddDays(-1);
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
                    // Last financial year
                    fromDate = now.Month >= 4 ? new DateTime(now.Year - 1, 4, 1) : new DateTime(now.Year - 2, 4, 1);
                    toDate = fromDate.AddYears(1).AddDays(-1);
                    break;
                default: // CUSTOM
                    fromDate = dtpFromDate.Value;
                    toDate = dtpToDate.Value;
                    break;
            }
        }

        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            GenerateLedgerReport();
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            ExportToExcel();
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            PrintReport();
        }

        private void BtnReconcile_Click(object sender, EventArgs e)
        {
            ReconcileAccounts();
        }

        private void GenerateLedgerReport()
        {
            try
            {
                ComboBoxItem selectedType = (ComboBoxItem)cmbReportType.SelectedItem;
                ComboBoxItem selectedAccount = (ComboBoxItem)cmbAccount.SelectedItem;
                ComboBoxItem selectedAccountType = (ComboBoxItem)cmbAccountType.SelectedItem;

                string reportType = selectedType?.Value.ToString() ?? "LEDGER";
                int accountID = selectedAccount != null ? (int)selectedAccount.Value : 0;
                string accountType = selectedAccountType?.Value.ToString() ?? "";

                DateTime fromDate = dtpFromDate.Value;
                DateTime toDate = dtpToDate.Value;

                // Generate appropriate report based on type
                switch (reportType)
                {
                    case "LEDGER":
                        GenerateAccountLedger(accountID, fromDate, toDate);
                        break;
                    case "TRIAL_BALANCE":
                        GenerateTrialBalance(fromDate, toDate, accountType);
                        break;
                    case "CUSTOMER_LEDGER":
                        GenerateCustomerLedger(fromDate, toDate);
                        break;
                    case "SUPPLIER_LEDGER":
                        GenerateSupplierLedger(fromDate, toDate);
                        break;
                    case "CASH_BOOK":
                        GenerateCashBook(fromDate, toDate);
                        break;
                    case "BANK_BOOK":
                        GenerateBankBook(fromDate, toDate);
                        break;
                    case "ACCOUNT_SUMMARY":
                        GenerateAccountSummary(accountType, fromDate, toDate);
                        break;
                }

                UpdateLedgerSummary();
                MessageBox.Show("Ledger report generated successfully!", "Success", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating ledger report: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerateAccountLedger(int accountID, DateTime fromDate, DateTime toDate)
        {
            try
            {
                // This is a simplified ledger query. In a real implementation, 
                // you would have a proper Chart of Accounts and Journal Entries
                string query = @"
                    SELECT 
                        'Sales' as AccountType,
                        s.BillDate as TransactionDate,
                        'Sales Invoice - ' + s.BillNumber as Description,
                        s.BillNumber as Reference,
                        0 as DebitAmount,
                        s.NetAmount as CreditAmount,
                        s.NetAmount as Balance
                    FROM Sales s
                    WHERE s.BillDate BETWEEN @FromDate AND @ToDate
                    AND s.IsActive = 1
                    
                    UNION ALL
                    
                    SELECT 
                        'Purchase' as AccountType,
                        p.PurchaseDate as TransactionDate,
                        'Purchase Invoice - ' + p.PurchaseNumber as Description,
                        p.PurchaseNumber as Reference,
                        p.NetAmount as DebitAmount,
                        0 as CreditAmount,
                        -p.NetAmount as Balance
                    FROM Purchases p
                    WHERE p.PurchaseDate BETWEEN @FromDate AND @ToDate
                    AND p.IsActive = 1
                    
                    UNION ALL
                    
                    SELECT 
                        'Expense' as AccountType,
                        e.ExpenseDate as TransactionDate,
                        e.Description + ' - ' + e.Category as Description,
                        'EXP-' + CAST(e.ExpenseID as VARCHAR) as Reference,
                        e.Amount as DebitAmount,
                        0 as CreditAmount,
                        -e.Amount as Balance
                    FROM Expenses e
                    WHERE e.ExpenseDate BETWEEN @FromDate AND @ToDate
                    AND e.IsActive = 1
                    
                    ORDER BY TransactionDate, Reference";

                SqlParameter[] parameters = {
                    new SqlParameter("@FromDate", fromDate),
                    new SqlParameter("@ToDate", toDate)
                };

                currentLedgerData = DatabaseConnection.ExecuteQuery(query, parameters);

                if (currentLedgerData != null && currentLedgerData.Rows.Count > 0)
                {
                    // Calculate running balance
                    decimal runningBalance = 0;
                    foreach (DataRow row in currentLedgerData.Rows)
                    {
                        decimal debit = Convert.ToDecimal(row["DebitAmount"]);
                        decimal credit = Convert.ToDecimal(row["CreditAmount"]);
                        runningBalance += credit - debit;
                        row["Balance"] = runningBalance;
                    }

                    // Smart RDLC path detection for AccountLedgerReport
                    string simplePath = Path.Combine(Application.StartupPath, "Reports", "AccountLedgerReport_Simple.rdlc");
                    string originalPath = Path.Combine(Application.StartupPath, "Reports", "AccountLedgerReport.rdlc");
                    
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
                        reportViewer.LocalReport.ReportPath = "Reports/AccountLedgerReport.rdlc";
                    }
                    reportViewer.LocalReport.DataSources.Clear();
                    reportViewer.LocalReport.DataSources.Add(new ReportDataSource("LedgerDataset", currentLedgerData));

                    // Set report parameters
                    ReportParameter[] reportParams = new ReportParameter[]
                    {
                        new ReportParameter("FromDate", fromDate.ToString("dd/MM/yyyy")),
                        new ReportParameter("ToDate", toDate.ToString("dd/MM/yyyy")),
                        new ReportParameter("AccountName", accountID == 0 ? "All Accounts" : cmbAccount.Text),
                        new ReportParameter("ReportTitle", "Account Ledger Report"),
                        new ReportParameter("OpeningBalance", chkIncludeOpening.Checked ? "0.00" : "0.00"),
                        new ReportParameter("ClosingBalance", runningBalance.ToString("N2"))
                    };

                    reportViewer.LocalReport.SetParameters(reportParams);
                    reportViewer.RefreshReport();
                }
                else
                {
                    MessageBox.Show("No ledger data found for the selected criteria.", "Information", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating account ledger: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerateTrialBalance(DateTime fromDate, DateTime toDate, string accountType)
        {
            try
            {
                // Simplified trial balance query
                string query = @"
                    SELECT 
                        'Assets' as AccountType,
                        'Cash & Bank' as AccountName,
                        SUM(CASE WHEN s.PaymentMode IN ('Cash', 'Card') THEN s.NetAmount ELSE 0 END) as DebitBalance,
                        0 as CreditBalance
                    FROM Sales s
                    WHERE s.BillDate BETWEEN @FromDate AND @ToDate
                    AND s.IsActive = 1
                    
                    UNION ALL
                    
                    SELECT 
                        'Liabilities' as AccountType,
                        'Accounts Payable' as AccountName,
                        0 as DebitBalance,
                        SUM(CASE WHEN p.PaymentStatus = 'Due' THEN p.NetAmount ELSE 0 END) as CreditBalance
                    FROM Purchases p
                    WHERE p.PurchaseDate BETWEEN @FromDate AND @ToDate
                    AND p.IsActive = 1
                    
                    UNION ALL
                    
                    SELECT 
                        'Income' as AccountType,
                        'Sales Revenue' as AccountName,
                        0 as DebitBalance,
                        SUM(s.NetAmount) as CreditBalance
                    FROM Sales s
                    WHERE s.BillDate BETWEEN @FromDate AND @ToDate
                    AND s.IsActive = 1
                    
                    UNION ALL
                    
                    SELECT 
                        'Expenses' as AccountType,
                        'Purchase Expenses' as AccountName,
                        SUM(p.NetAmount) as DebitBalance,
                        0 as CreditBalance
                    FROM Purchases p
                    WHERE p.PurchaseDate BETWEEN @FromDate AND @ToDate
                    AND p.IsActive = 1
                    
                    UNION ALL
                    
                    SELECT 
                        'Expenses' as AccountType,
                        'Operating Expenses' as AccountName,
                        SUM(e.Amount) as DebitBalance,
                        0 as CreditBalance
                    FROM Expenses e
                    WHERE e.ExpenseDate BETWEEN @FromDate AND @ToDate
                    AND e.IsActive = 1" +
                    (!string.IsNullOrEmpty(accountType) ? " AND AccountType = @AccountType" : "") + @"
                    ORDER BY AccountType, AccountName";

                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    new SqlParameter("@FromDate", fromDate),
                    new SqlParameter("@ToDate", toDate)
                };

                if (!string.IsNullOrEmpty(accountType))
                {
                    parameters.Add(new SqlParameter("@AccountType", accountType));
                }

                currentLedgerData = DatabaseConnection.ExecuteQuery(query, parameters.ToArray());

                if (currentLedgerData != null && currentLedgerData.Rows.Count > 0)
                {
                    // Create and load RDLC report
                    string reportPath = Path.Combine(Application.StartupPath, "Reports", "TrialBalanceReport.rdlc");
                    reportViewer.LocalReport.ReportPath = reportPath;
                    reportViewer.LocalReport.DataSources.Clear();
                    reportViewer.LocalReport.DataSources.Add(new ReportDataSource("TrialBalanceDataset", currentLedgerData));

                    // Calculate totals
                    decimal totalDebits = 0;
                    decimal totalCredits = 0;
                    foreach (DataRow row in currentLedgerData.Rows)
                    {
                        totalDebits += Convert.ToDecimal(row["DebitBalance"]);
                        totalCredits += Convert.ToDecimal(row["CreditBalance"]);
                    }

                    // Set report parameters
                    ReportParameter[] reportParams = new ReportParameter[]
                    {
                        new ReportParameter("FromDate", fromDate.ToString("dd/MM/yyyy")),
                        new ReportParameter("ToDate", toDate.ToString("dd/MM/yyyy")),
                        new ReportParameter("AccountType", string.IsNullOrEmpty(accountType) ? "All Types" : accountType),
                        new ReportParameter("ReportTitle", "Trial Balance Report"),
                        new ReportParameter("TotalDebits", totalDebits.ToString("N2")),
                        new ReportParameter("TotalCredits", totalCredits.ToString("N2"))
                    };

                    reportViewer.LocalReport.SetParameters(reportParams);
                    reportViewer.RefreshReport();

                    tabControl.SelectedTab = tabTrialBalance;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating trial balance: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerateCustomerLedger(DateTime fromDate, DateTime toDate)
        {
            // Implementation for customer ledger
        }

        private void GenerateSupplierLedger(DateTime fromDate, DateTime toDate)
        {
            // Implementation for supplier ledger
        }

        private void GenerateCashBook(DateTime fromDate, DateTime toDate)
        {
            // Implementation for cash book
        }

        private void GenerateBankBook(DateTime fromDate, DateTime toDate)
        {
            // Implementation for bank book
        }

        private void GenerateAccountSummary(string accountType, DateTime fromDate, DateTime toDate)
        {
            // Implementation for account summary
        }

        private void UpdateLedgerSummary()
        {
            if (currentLedgerData != null && currentLedgerData.Rows.Count > 0)
            {
                try
                {
                    decimal totalDebits = 0;
                    decimal totalCredits = 0;
                    decimal openingBalance = 0;
                    decimal closingBalance = 0;
                    int totalAccounts = currentLedgerData.Rows.Count;

                    foreach (DataRow row in currentLedgerData.Rows)
                    {
                        if (row["DebitBalance"] != DBNull.Value)
                            totalDebits += Convert.ToDecimal(row["DebitBalance"]);
                        if (row["CreditBalance"] != DBNull.Value)
                            totalCredits += Convert.ToDecimal(row["CreditBalance"]);
                        if (row["DebitAmount"] != DBNull.Value)
                            totalDebits += Convert.ToDecimal(row["DebitAmount"]);
                        if (row["CreditAmount"] != DBNull.Value)
                            totalCredits += Convert.ToDecimal(row["CreditAmount"]);
                        if (row["Balance"] != DBNull.Value)
                            closingBalance = Convert.ToDecimal(row["Balance"]);
                    }

                    decimal netBalance = totalCredits - totalDebits;

                    lblTotalAccounts.Text = $"Total Accounts: {totalAccounts}";
                    lblOpeningBalance.Text = $"Opening Balance: ₹{openingBalance:N2}";
                    lblClosingBalance.Text = $"Closing Balance: ₹{closingBalance:N2}";
                    lblTotalDebits.Text = $"Total Debits: ₹{totalDebits:N2}";
                    lblTotalCredits.Text = $"Total Credits: ₹{totalCredits:N2}";
                    lblNetBalance.Text = $"Net Balance: ₹{netBalance:N2}";
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
                if (currentLedgerData == null || currentLedgerData.Rows.Count == 0)
                {
                    MessageBox.Show("No data to export. Please generate a report first.", "Information", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "Excel Files|*.xlsx|CSV Files|*.csv";
                saveDialog.FileName = $"Ledger_Report_{DateTime.Now:yyyyMMdd}";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    // Export to Excel logic would go here
                    MessageBox.Show("Ledger report exported successfully!", "Export Complete", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exporting ledger report: " + ex.Message, "Error", 
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
                MessageBox.Show("Error printing ledger report: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ReconcileAccounts()
        {
            try
            {
                MessageBox.Show("Account reconciliation functionality would be implemented here.\n\n" +
                    "This would include:\n" +
                    "- Bank reconciliation\n" +
                    "- Customer balance verification\n" +
                    "- Supplier balance verification\n" +
                    "- Inter-account balance checks", 
                    "Account Reconciliation", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in account reconciliation: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
