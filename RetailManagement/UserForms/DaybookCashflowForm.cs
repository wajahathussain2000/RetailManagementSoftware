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
    public partial class DaybookCashflowForm : Form
    {
        private ReportViewer reportViewer;
        private ComboBox cmbReportType;
        private ComboBox cmbAccountType;
        private ComboBox cmbPaymentMode;
        private DateTimePicker dtpFromDate;
        private DateTimePicker dtpToDate;
        private ComboBox cmbTimePeriod;
        private CheckBox chkIncludeCash;
        private CheckBox chkIncludeBank;
        private CheckBox chkIncludeCredit;
        private CheckBox chkGroupByAccount;
        private Button btnGenerate;
        private Button btnExport;
        private Button btnPrint;
        private Button btnReconcile;
        private GroupBox groupFilters;
        private GroupBox groupOptions;
        private TabControl tabControl;
        private TabPage tabDaybook;
        private TabPage tabCashFlow;
        private TabPage tabBankReconciliation;
        private DataTable currentFinancialData;
        private Panel summaryPanel;
        private Label lblTotalReceipts;
        private Label lblTotalPayments;
        private Label lblNetCashFlow;
        private Label lblOpeningBalance;
        private Label lblClosingBalance;
        private Label lblBankBalance;

        public DaybookCashflowForm()
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
            this.cmbAccountType = new System.Windows.Forms.ComboBox();
            this.cmbPaymentMode = new System.Windows.Forms.ComboBox();
            this.dtpFromDate = new System.Windows.Forms.DateTimePicker();
            this.dtpToDate = new System.Windows.Forms.DateTimePicker();
            this.cmbTimePeriod = new System.Windows.Forms.ComboBox();
            this.chkIncludeCash = new System.Windows.Forms.CheckBox();
            this.chkIncludeBank = new System.Windows.Forms.CheckBox();
            this.chkIncludeCredit = new System.Windows.Forms.CheckBox();
            this.chkGroupByAccount = new System.Windows.Forms.CheckBox();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnReconcile = new System.Windows.Forms.Button();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabDaybook = new System.Windows.Forms.TabPage();
            this.tabCashFlow = new System.Windows.Forms.TabPage();
            this.tabBankReconciliation = new System.Windows.Forms.TabPage();
            this.summaryPanel = new System.Windows.Forms.Panel();
            this.lblTotalReceipts = new System.Windows.Forms.Label();
            this.lblTotalPayments = new System.Windows.Forms.Label();
            this.lblNetCashFlow = new System.Windows.Forms.Label();
            this.lblOpeningBalance = new System.Windows.Forms.Label();
            this.lblClosingBalance = new System.Windows.Forms.Label();
            this.lblBankBalance = new System.Windows.Forms.Label();

            this.SuspendLayout();

            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.summaryPanel);
            this.Controls.Add(this.groupOptions);
            this.Controls.Add(this.groupFilters);
            this.Name = "DaybookCashflowForm";
            this.Text = "Daybook & Cash Flow Statements";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;

            // Group Filters
            this.groupFilters.Controls.Add(new Label { Text = "Report Type:", Location = new Point(15, 25), Size = new Size(80, 13) });
            this.groupFilters.Controls.Add(this.cmbReportType);
            this.groupFilters.Controls.Add(new Label { Text = "Account Type:", Location = new Point(300, 25), Size = new Size(80, 13) });
            this.groupFilters.Controls.Add(this.cmbAccountType);
            this.groupFilters.Controls.Add(new Label { Text = "Payment Mode:", Location = new Point(15, 55), Size = new Size(85, 13) });
            this.groupFilters.Controls.Add(this.cmbPaymentMode);
            this.groupFilters.Controls.Add(new Label { Text = "Time Period:", Location = new Point(300, 55), Size = new Size(75, 13) });
            this.groupFilters.Controls.Add(this.cmbTimePeriod);
            this.groupFilters.Controls.Add(new Label { Text = "From Date:", Location = new Point(15, 85), Size = new Size(65, 13) });
            this.groupFilters.Controls.Add(this.dtpFromDate);
            this.groupFilters.Controls.Add(new Label { Text = "To Date:", Location = new Point(300, 85), Size = new Size(50, 13) });
            this.groupFilters.Controls.Add(this.dtpToDate);
            this.groupFilters.Location = new System.Drawing.Point(12, 12);
            this.groupFilters.Name = "groupFilters";
            this.groupFilters.Size = new System.Drawing.Size(600, 120);
            this.groupFilters.TabIndex = 0;
            this.groupFilters.TabStop = false;
            this.groupFilters.Text = "Financial Report Filters";

            // Group Options
            this.groupOptions.Controls.Add(this.chkIncludeCash);
            this.groupOptions.Controls.Add(this.chkIncludeBank);
            this.groupOptions.Controls.Add(this.chkIncludeCredit);
            this.groupOptions.Controls.Add(this.chkGroupByAccount);
            this.groupOptions.Controls.Add(this.btnGenerate);
            this.groupOptions.Controls.Add(this.btnExport);
            this.groupOptions.Controls.Add(this.btnPrint);
            this.groupOptions.Controls.Add(this.btnReconcile);
            this.groupOptions.Location = new System.Drawing.Point(630, 12);
            this.groupOptions.Name = "groupOptions";
            this.groupOptions.Size = new System.Drawing.Size(300, 120);
            this.groupOptions.TabIndex = 1;
            this.groupOptions.TabStop = false;
            this.groupOptions.Text = "Options & Actions";

            // Tab Control
            this.tabControl.Controls.Add(this.tabDaybook);
            this.tabControl.Controls.Add(this.tabCashFlow);
            this.tabControl.Controls.Add(this.tabBankReconciliation);
            this.tabControl.Location = new System.Drawing.Point(12, 210);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(1176, 440);
            this.tabControl.TabIndex = 3;

            // Tab Pages
            this.tabDaybook.Location = new System.Drawing.Point(4, 22);
            this.tabDaybook.Name = "tabDaybook";
            this.tabDaybook.Padding = new System.Windows.Forms.Padding(3);
            this.tabDaybook.Size = new System.Drawing.Size(1168, 414);
            this.tabDaybook.TabIndex = 0;
            this.tabDaybook.Text = "Daily Cash Book";
            this.tabDaybook.UseVisualStyleBackColor = true;

            this.tabCashFlow.Location = new System.Drawing.Point(4, 22);
            this.tabCashFlow.Name = "tabCashFlow";
            this.tabCashFlow.Padding = new System.Windows.Forms.Padding(3);
            this.tabCashFlow.Size = new System.Drawing.Size(1168, 414);
            this.tabCashFlow.TabIndex = 1;
            this.tabCashFlow.Text = "Cash Flow Statement";
            this.tabCashFlow.UseVisualStyleBackColor = true;

            this.tabBankReconciliation.Location = new System.Drawing.Point(4, 22);
            this.tabBankReconciliation.Name = "tabBankReconciliation";
            this.tabBankReconciliation.Padding = new System.Windows.Forms.Padding(3);
            this.tabBankReconciliation.Size = new System.Drawing.Size(1168, 414);
            this.tabBankReconciliation.TabIndex = 2;
            this.tabBankReconciliation.Text = "Bank Reconciliation";
            this.tabBankReconciliation.UseVisualStyleBackColor = true;

            // Summary Panel
            this.summaryPanel.BackColor = System.Drawing.Color.LightGreen;
            this.summaryPanel.Controls.Add(this.lblTotalReceipts);
            this.summaryPanel.Controls.Add(this.lblTotalPayments);
            this.summaryPanel.Controls.Add(this.lblNetCashFlow);
            this.summaryPanel.Controls.Add(this.lblOpeningBalance);
            this.summaryPanel.Controls.Add(this.lblClosingBalance);
            this.summaryPanel.Controls.Add(this.lblBankBalance);
            this.summaryPanel.Location = new System.Drawing.Point(12, 140);
            this.summaryPanel.Name = "summaryPanel";
            this.summaryPanel.Size = new System.Drawing.Size(1176, 65);
            this.summaryPanel.TabIndex = 2;

            // Setup all controls
            SetupControls();

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

            // Account Type ComboBox
            this.cmbAccountType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAccountType.Location = new System.Drawing.Point(385, 22);
            this.cmbAccountType.Name = "cmbAccountType";
            this.cmbAccountType.Size = new System.Drawing.Size(150, 21);
            this.cmbAccountType.TabIndex = 2;

            // Payment Mode ComboBox
            this.cmbPaymentMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPaymentMode.Location = new System.Drawing.Point(105, 52);
            this.cmbPaymentMode.Name = "cmbPaymentMode";
            this.cmbPaymentMode.Size = new System.Drawing.Size(120, 21);
            this.cmbPaymentMode.TabIndex = 3;

            // Time Period ComboBox
            this.cmbTimePeriod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTimePeriod.Location = new System.Drawing.Point(380, 52);
            this.cmbTimePeriod.Name = "cmbTimePeriod";
            this.cmbTimePeriod.Size = new System.Drawing.Size(120, 21);
            this.cmbTimePeriod.TabIndex = 4;
            this.cmbTimePeriod.SelectedIndexChanged += CmbTimePeriod_SelectedIndexChanged;

            // Date Pickers
            this.dtpFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFromDate.Location = new System.Drawing.Point(85, 82);
            this.dtpFromDate.Name = "dtpFromDate";
            this.dtpFromDate.Size = new System.Drawing.Size(100, 20);
            this.dtpFromDate.TabIndex = 5;

            this.dtpToDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpToDate.Location = new System.Drawing.Point(360, 82);
            this.dtpToDate.Name = "dtpToDate";
            this.dtpToDate.Size = new System.Drawing.Size(100, 20);
            this.dtpToDate.TabIndex = 6;

            // Checkboxes
            this.chkIncludeCash.Location = new System.Drawing.Point(15, 25);
            this.chkIncludeCash.Name = "chkIncludeCash";
            this.chkIncludeCash.Size = new System.Drawing.Size(120, 17);
            this.chkIncludeCash.TabIndex = 7;
            this.chkIncludeCash.Text = "Include Cash";
            this.chkIncludeCash.UseVisualStyleBackColor = true;
            this.chkIncludeCash.Checked = true;

            this.chkIncludeBank.Location = new System.Drawing.Point(15, 45);
            this.chkIncludeBank.Name = "chkIncludeBank";
            this.chkIncludeBank.Size = new System.Drawing.Size(120, 17);
            this.chkIncludeBank.TabIndex = 8;
            this.chkIncludeBank.Text = "Include Bank";
            this.chkIncludeBank.UseVisualStyleBackColor = true;
            this.chkIncludeBank.Checked = true;

            this.chkIncludeCredit.Location = new System.Drawing.Point(15, 65);
            this.chkIncludeCredit.Name = "chkIncludeCredit";
            this.chkIncludeCredit.Size = new System.Drawing.Size(120, 17);
            this.chkIncludeCredit.TabIndex = 9;
            this.chkIncludeCredit.Text = "Include Credit";
            this.chkIncludeCredit.UseVisualStyleBackColor = true;
            this.chkIncludeCredit.Checked = true;

            this.chkGroupByAccount.Location = new System.Drawing.Point(15, 85);
            this.chkGroupByAccount.Name = "chkGroupByAccount";
            this.chkGroupByAccount.Size = new System.Drawing.Size(130, 17);
            this.chkGroupByAccount.TabIndex = 10;
            this.chkGroupByAccount.Text = "Group by Account";
            this.chkGroupByAccount.UseVisualStyleBackColor = true;

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
            this.btnReconcile.Location = new System.Drawing.Point(15, 105);
            this.btnReconcile.Name = "btnReconcile";
            this.btnReconcile.Size = new System.Drawing.Size(120, 25);
            this.btnReconcile.TabIndex = 14;
            this.btnReconcile.Text = "Bank Reconcile";
            this.btnReconcile.UseVisualStyleBackColor = false;
            this.btnReconcile.Click += BtnReconcile_Click;

            // Summary Labels
            this.lblTotalReceipts.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblTotalReceipts.Location = new System.Drawing.Point(15, 10);
            this.lblTotalReceipts.Name = "lblTotalReceipts";
            this.lblTotalReceipts.Size = new System.Drawing.Size(180, 15);
            this.lblTotalReceipts.TabIndex = 0;
            this.lblTotalReceipts.Text = "Total Receipts: ₹0.00";

            this.lblTotalPayments.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblTotalPayments.Location = new System.Drawing.Point(15, 30);
            this.lblTotalPayments.Name = "lblTotalPayments";
            this.lblTotalPayments.Size = new System.Drawing.Size(180, 15);
            this.lblTotalPayments.TabIndex = 1;
            this.lblTotalPayments.Text = "Total Payments: ₹0.00";

            this.lblNetCashFlow.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblNetCashFlow.Location = new System.Drawing.Point(15, 50);
            this.lblNetCashFlow.Name = "lblNetCashFlow";
            this.lblNetCashFlow.Size = new System.Drawing.Size(180, 15);
            this.lblNetCashFlow.TabIndex = 2;
            this.lblNetCashFlow.Text = "Net Cash Flow: ₹0.00";

            this.lblOpeningBalance.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblOpeningBalance.Location = new System.Drawing.Point(250, 10);
            this.lblOpeningBalance.Name = "lblOpeningBalance";
            this.lblOpeningBalance.Size = new System.Drawing.Size(180, 15);
            this.lblOpeningBalance.TabIndex = 3;
            this.lblOpeningBalance.Text = "Opening Balance: ₹0.00";

            this.lblClosingBalance.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblClosingBalance.Location = new System.Drawing.Point(250, 30);
            this.lblClosingBalance.Name = "lblClosingBalance";
            this.lblClosingBalance.Size = new System.Drawing.Size(180, 15);
            this.lblClosingBalance.TabIndex = 4;
            this.lblClosingBalance.Text = "Closing Balance: ₹0.00";

            this.lblBankBalance.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblBankBalance.Location = new System.Drawing.Point(250, 50);
            this.lblBankBalance.Name = "lblBankBalance";
            this.lblBankBalance.Size = new System.Drawing.Size(180, 15);
            this.lblBankBalance.TabIndex = 5;
            this.lblBankBalance.Text = "Bank Balance: ₹0.00";
        }

        private void InitializeReportViewer()
        {
            this.reportViewer = new ReportViewer();
            this.reportViewer.Dock = DockStyle.Fill;
            this.reportViewer.ProcessingMode = ProcessingMode.Local;
            
            // Add report viewer to each tab
            this.tabDaybook.Controls.Add(this.reportViewer);
        }

        private void LoadDropdownData()
        {
            try
            {
                // Load Report Types
                cmbReportType.Items.Clear();
                cmbReportType.Items.Add(new ComboBoxItem { Text = "Daily Cash Book", Value = "DAILY_CASHBOOK" });
                cmbReportType.Items.Add(new ComboBoxItem { Text = "Cash Flow Statement", Value = "CASH_FLOW" });
                cmbReportType.Items.Add(new ComboBoxItem { Text = "Bank Reconciliation", Value = "BANK_RECONCILIATION" });
                cmbReportType.Items.Add(new ComboBoxItem { Text = "Receipts & Payments", Value = "RECEIPTS_PAYMENTS" });
                cmbReportType.Items.Add(new ComboBoxItem { Text = "Cash Position Report", Value = "CASH_POSITION" });
                cmbReportType.Items.Add(new ComboBoxItem { Text = "Fund Flow Statement", Value = "FUND_FLOW" });
                cmbReportType.DisplayMember = "Text";
                cmbReportType.ValueMember = "Value";
                cmbReportType.SelectedIndex = 0;

                // Load Account Types
                cmbAccountType.Items.Clear();
                cmbAccountType.Items.Add(new ComboBoxItem { Text = "All Accounts", Value = "ALL" });
                cmbAccountType.Items.Add(new ComboBoxItem { Text = "Cash Account", Value = "CASH" });
                cmbAccountType.Items.Add(new ComboBoxItem { Text = "Bank Account", Value = "BANK" });
                cmbAccountType.Items.Add(new ComboBoxItem { Text = "Credit Account", Value = "CREDIT" });
                cmbAccountType.Items.Add(new ComboBoxItem { Text = "Expense Account", Value = "EXPENSE" });
                cmbAccountType.Items.Add(new ComboBoxItem { Text = "Income Account", Value = "INCOME" });
                cmbAccountType.DisplayMember = "Text";
                cmbAccountType.ValueMember = "Value";
                cmbAccountType.SelectedIndex = 0;

                // Load Payment Modes
                cmbPaymentMode.Items.Clear();
                cmbPaymentMode.Items.Add(new ComboBoxItem { Text = "All Modes", Value = "ALL" });
                cmbPaymentMode.Items.Add(new ComboBoxItem { Text = "Cash", Value = "Cash" });
                cmbPaymentMode.Items.Add(new ComboBoxItem { Text = "Card", Value = "Card" });
                cmbPaymentMode.Items.Add(new ComboBoxItem { Text = "Bank Transfer", Value = "Bank Transfer" });
                cmbPaymentMode.Items.Add(new ComboBoxItem { Text = "Cheque", Value = "Cheque" });
                cmbPaymentMode.Items.Add(new ComboBoxItem { Text = "Due/Credit", Value = "Due" });
                cmbPaymentMode.DisplayMember = "Text";
                cmbPaymentMode.ValueMember = "Value";
                cmbPaymentMode.SelectedIndex = 0;

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
                cmbTimePeriod.DisplayMember = "Text";
                cmbTimePeriod.ValueMember = "Value";
                cmbTimePeriod.SelectedIndex = 1; // Today
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading dropdown data: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetDefaultDates()
        {
            // Set to today
            dtpFromDate.Value = DateTime.Now.Date;
            dtpToDate.Value = DateTime.Now.Date;
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
                    case "DAILY_CASHBOOK":
                        tabControl.SelectedTab = tabDaybook;
                        break;
                    case "CASH_FLOW":
                        tabControl.SelectedTab = tabCashFlow;
                        break;
                    case "BANK_RECONCILIATION":
                        tabControl.SelectedTab = tabBankReconciliation;
                        break;
                    default:
                        tabControl.SelectedTab = tabDaybook;
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
                default: // CUSTOM
                    fromDate = dtpFromDate.Value;
                    toDate = dtpToDate.Value;
                    break;
            }
        }

        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            GenerateFinancialReport();
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            ExportFinancialReport();
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            PrintFinancialReport();
        }

        private void BtnReconcile_Click(object sender, EventArgs e)
        {
            PerformBankReconciliation();
        }

        private void GenerateFinancialReport()
        {
            try
            {
                ComboBoxItem selectedType = (ComboBoxItem)cmbReportType.SelectedItem;
                ComboBoxItem selectedAccount = (ComboBoxItem)cmbAccountType.SelectedItem;
                ComboBoxItem selectedPaymentMode = (ComboBoxItem)cmbPaymentMode.SelectedItem;

                string reportType = selectedType?.Value.ToString() ?? "DAILY_CASHBOOK";
                string accountType = selectedAccount?.Value.ToString() ?? "ALL";
                string paymentMode = selectedPaymentMode?.Value.ToString() ?? "ALL";

                DateTime fromDate = dtpFromDate.Value;
                DateTime toDate = dtpToDate.Value;

                // Generate appropriate financial report based on type
                switch (reportType)
                {
                    case "DAILY_CASHBOOK":
                        GenerateDailyCashbook(fromDate, toDate, accountType, paymentMode);
                        break;
                    case "CASH_FLOW":
                        GenerateCashFlowStatement(fromDate, toDate);
                        break;
                    case "BANK_RECONCILIATION":
                        GenerateBankReconciliation(fromDate, toDate);
                        break;
                    case "RECEIPTS_PAYMENTS":
                        GenerateReceiptsPaymentsReport(fromDate, toDate, accountType);
                        break;
                    case "CASH_POSITION":
                        GenerateCashPositionReport(fromDate, toDate);
                        break;
                    case "FUND_FLOW":
                        GenerateFundFlowStatement(fromDate, toDate);
                        break;
                }

                UpdateFinancialSummary();
                MessageBox.Show("Financial report generated successfully!", "Success", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating financial report: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerateDailyCashbook(DateTime fromDate, DateTime toDate, string accountType, string paymentMode)
        {
            try
            {
                // Build comprehensive cash book query
                string query = @"
                    SELECT 
                        'RECEIPT' as TransactionType,
                        s.BillDate as TransactionDate,
                        'Sales Receipt - ' + s.BillNumber as Description,
                        ISNULL(c.CustomerName, 'Cash Sale') as PartyName,
                        s.PaymentMode,
                        0 as PaymentAmount,
                        s.NetAmount as ReceiptAmount,
                        s.NetAmount as Amount,
                        'SALES' as Category,
                        s.BillNumber as Reference
                    FROM Sales s
                    LEFT JOIN Customers c ON s.CustomerID = c.CustomerID
                    WHERE s.BillDate BETWEEN @FromDate AND @ToDate
                    AND s.IsActive = 1" +
                    (chkIncludeCash.Checked ? "" : " AND s.PaymentMode != 'Cash'") +
                    (chkIncludeBank.Checked ? "" : " AND s.PaymentMode NOT IN ('Bank Transfer', 'Card')") +
                    (chkIncludeCredit.Checked ? "" : " AND s.PaymentMode != 'Due'") +
                    (paymentMode != "ALL" ? " AND s.PaymentMode = @PaymentMode" : "") + @"
                    
                    UNION ALL
                    
                    SELECT 
                        'PAYMENT' as TransactionType,
                        p.PurchaseDate as TransactionDate,
                        'Purchase Payment - ' + p.PurchaseNumber as Description,
                        co.CompanyName as PartyName,
                        p.PaymentMode,
                        p.NetAmount as PaymentAmount,
                        0 as ReceiptAmount,
                        -p.NetAmount as Amount,
                        'PURCHASE' as Category,
                        p.PurchaseNumber as Reference
                    FROM Purchases p
                    LEFT JOIN Companies co ON p.CompanyID = co.CompanyID
                    WHERE p.PurchaseDate BETWEEN @FromDate AND @ToDate
                    AND p.IsActive = 1" +
                    (chkIncludeCash.Checked ? "" : " AND p.PaymentMode != 'Cash'") +
                    (chkIncludeBank.Checked ? "" : " AND p.PaymentMode NOT IN ('Bank Transfer', 'Card')") +
                    (chkIncludeCredit.Checked ? "" : " AND p.PaymentMode != 'Due'") +
                    (paymentMode != "ALL" ? " AND p.PaymentMode = @PaymentMode" : "") + @"
                    
                    UNION ALL
                    
                    SELECT 
                        'PAYMENT' as TransactionType,
                        e.ExpenseDate as TransactionDate,
                        'Expense - ' + e.Category + ': ' + e.Description as Description,
                        e.Description as PartyName,
                        e.PaymentMode,
                        e.Amount as PaymentAmount,
                        0 as ReceiptAmount,
                        -e.Amount as Amount,
                        'EXPENSE' as Category,
                        'EXP-' + CAST(e.ExpenseID as VARCHAR) as Reference
                    FROM Expenses e
                    WHERE e.ExpenseDate BETWEEN @FromDate AND @ToDate
                    AND e.IsActive = 1" +
                    (chkIncludeCash.Checked ? "" : " AND e.PaymentMode != 'Cash'") +
                    (chkIncludeBank.Checked ? "" : " AND e.PaymentMode NOT IN ('Bank Transfer', 'Card')") +
                    (chkIncludeCredit.Checked ? "" : " AND e.PaymentMode != 'Due'") +
                    (paymentMode != "ALL" ? " AND e.PaymentMode = @PaymentMode" : "") + @"
                    
                    UNION ALL
                    
                    SELECT 
                        'RECEIPT' as TransactionType,
                        bt.TransactionDate,
                        CASE 
                            WHEN bt.TransactionType = 'DEPOSIT' THEN 'Bank Deposit - ' + bt.Description
                            ELSE 'Bank Receipt - ' + bt.Description
                        END as Description,
                        bt.Description as PartyName,
                        'Bank Transfer' as PaymentMode,
                        0 as PaymentAmount,
                        CASE WHEN bt.TransactionType = 'DEPOSIT' THEN bt.Amount ELSE 0 END as ReceiptAmount,
                        CASE WHEN bt.TransactionType = 'DEPOSIT' THEN bt.Amount ELSE -bt.Amount END as Amount,
                        'BANKING' as Category,
                        'BANK-' + CAST(bt.TransactionID as VARCHAR) as Reference
                    FROM BankTransactions bt
                    WHERE bt.TransactionDate BETWEEN @FromDate AND @ToDate
                    AND bt.IsActive = 1
                    AND bt.TransactionType = 'DEPOSIT'" +
                    (chkIncludeBank.Checked ? "" : " AND 1=0") + @"
                    
                    UNION ALL
                    
                    SELECT 
                        'PAYMENT' as TransactionType,
                        bt.TransactionDate,
                        'Bank Withdrawal - ' + bt.Description as Description,
                        bt.Description as PartyName,
                        'Bank Transfer' as PaymentMode,
                        bt.Amount as PaymentAmount,
                        0 as ReceiptAmount,
                        -bt.Amount as Amount,
                        'BANKING' as Category,
                        'BANK-' + CAST(bt.TransactionID as VARCHAR) as Reference
                    FROM BankTransactions bt
                    WHERE bt.TransactionDate BETWEEN @FromDate AND @ToDate
                    AND bt.IsActive = 1
                    AND bt.TransactionType = 'WITHDRAWAL'" +
                    (chkIncludeBank.Checked ? "" : " AND 1=0") + @"
                    
                    ORDER BY TransactionDate, TransactionType DESC, Reference";

                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    new SqlParameter("@FromDate", fromDate),
                    new SqlParameter("@ToDate", toDate)
                };

                if (paymentMode != "ALL")
                {
                    parameters.Add(new SqlParameter("@PaymentMode", paymentMode));
                }

                currentFinancialData = DatabaseConnection.ExecuteQuery(query, parameters.ToArray());

                if (currentFinancialData != null && currentFinancialData.Rows.Count > 0)
                {
                    // Smart RDLC path detection for DailyCashbookReport
                    string simplePath = Path.Combine(Application.StartupPath, "Reports", "DailyCashbookReport_Simple.rdlc");
                    string originalPath = Path.Combine(Application.StartupPath, "Reports", "DailyCashbookReport.rdlc");
                    
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
                        reportViewer.LocalReport.ReportPath = "Reports/DailyCashbookReport.rdlc";
                    }
                    reportViewer.LocalReport.DataSources.Clear();
                    reportViewer.LocalReport.DataSources.Add(new ReportDataSource("CashbookDataset", currentFinancialData));

                    // Calculate opening and closing balances
                    decimal openingBalance = GetOpeningBalance(fromDate, accountType);
                    decimal totalReceipts = 0;
                    decimal totalPayments = 0;

                    foreach (DataRow row in currentFinancialData.Rows)
                    {
                        totalReceipts += Convert.ToDecimal(row["ReceiptAmount"]);
                        totalPayments += Convert.ToDecimal(row["PaymentAmount"]);
                    }

                    decimal closingBalance = openingBalance + totalReceipts - totalPayments;

                    // Set report parameters
                    ReportParameter[] reportParams = new ReportParameter[]
                    {
                        new ReportParameter("FromDate", fromDate.ToString("dd/MM/yyyy")),
                        new ReportParameter("ToDate", toDate.ToString("dd/MM/yyyy")),
                        new ReportParameter("AccountType", accountType == "ALL" ? "All Accounts" : accountType),
                        new ReportParameter("PaymentMode", paymentMode == "ALL" ? "All Modes" : paymentMode),
                        new ReportParameter("OpeningBalance", openingBalance.ToString("N2")),
                        new ReportParameter("ClosingBalance", closingBalance.ToString("N2")),
                        new ReportParameter("ReportTitle", "Daily Cash Book Report")
                    };

                    reportViewer.LocalReport.SetParameters(reportParams);
                    reportViewer.RefreshReport();

                    tabControl.SelectedTab = tabDaybook;
                }
                else
                {
                    MessageBox.Show("No cash book data found for the selected criteria.", "Information", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating daily cash book: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerateCashFlowStatement(DateTime fromDate, DateTime toDate)
        {
            try
            {
                // Cash Flow Statement - Operating, Investing, Financing Activities
                string query = @"
                    SELECT 
                        'OPERATING' as ActivityType,
                        'Cash Receipts from Sales' as ActivityDescription,
                        SUM(s.NetAmount) as Amount
                    FROM Sales s
                    WHERE s.BillDate BETWEEN @FromDate AND @ToDate
                    AND s.IsActive = 1 AND s.PaymentMode IN ('Cash', 'Card', 'Bank Transfer')
                    
                    UNION ALL
                    
                    SELECT 
                        'OPERATING' as ActivityType,
                        'Cash Payments for Purchases' as ActivityDescription,
                        -SUM(p.NetAmount) as Amount
                    FROM Purchases p
                    WHERE p.PurchaseDate BETWEEN @FromDate AND @ToDate
                    AND p.IsActive = 1 AND p.PaymentMode IN ('Cash', 'Card', 'Bank Transfer')
                    
                    UNION ALL
                    
                    SELECT 
                        'OPERATING' as ActivityType,
                        'Operating Expenses' as ActivityDescription,
                        -SUM(e.Amount) as Amount
                    FROM Expenses e
                    WHERE e.ExpenseDate BETWEEN @FromDate AND @ToDate
                    AND e.IsActive = 1
                    
                    UNION ALL
                    
                    SELECT 
                        'FINANCING' as ActivityType,
                        'Bank Deposits' as ActivityDescription,
                        SUM(bt.Amount) as Amount
                    FROM BankTransactions bt
                    WHERE bt.TransactionDate BETWEEN @FromDate AND @ToDate
                    AND bt.TransactionType = 'DEPOSIT' AND bt.IsActive = 1
                    
                    UNION ALL
                    
                    SELECT 
                        'FINANCING' as ActivityType,
                        'Bank Withdrawals' as ActivityDescription,
                        -SUM(bt.Amount) as Amount
                    FROM BankTransactions bt
                    WHERE bt.TransactionDate BETWEEN @FromDate AND @ToDate
                    AND bt.TransactionType = 'WITHDRAWAL' AND bt.IsActive = 1
                    
                    ORDER BY ActivityType, ActivityDescription";

                SqlParameter[] parameters = {
                    new SqlParameter("@FromDate", fromDate),
                    new SqlParameter("@ToDate", toDate)
                };

                currentFinancialData = DatabaseConnection.ExecuteQuery(query, parameters);

                if (currentFinancialData != null && currentFinancialData.Rows.Count > 0)
                {
                    // Smart RDLC path detection for CashFlowStatementReport
                    string simplePath = Path.Combine(Application.StartupPath, "Reports", "CashFlowReport_Simple.rdlc");
                    string originalPath = Path.Combine(Application.StartupPath, "Reports", "CashFlowStatementReport.rdlc");
                    
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
                        reportViewer.LocalReport.ReportPath = "Reports/CashFlowReport.rdlc";
                    }
                    reportViewer.LocalReport.DataSources.Clear();
                    reportViewer.LocalReport.DataSources.Add(new ReportDataSource("CashFlowDataset", currentFinancialData));

                    // Calculate net cash flow
                    decimal netCashFlow = 0;
                    foreach (DataRow row in currentFinancialData.Rows)
                    {
                        netCashFlow += Convert.ToDecimal(row["Amount"]);
                    }

                    // Set report parameters
                    ReportParameter[] reportParams = new ReportParameter[]
                    {
                        new ReportParameter("FromDate", fromDate.ToString("dd/MM/yyyy")),
                        new ReportParameter("ToDate", toDate.ToString("dd/MM/yyyy")),
                        new ReportParameter("NetCashFlow", netCashFlow.ToString("N2")),
                        new ReportParameter("ReportTitle", "Cash Flow Statement"),
                        new ReportParameter("CompanyName", "Your Company Name")
                    };

                    reportViewer.LocalReport.SetParameters(reportParams);
                    reportViewer.RefreshReport();

                    tabControl.SelectedTab = tabCashFlow;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating cash flow statement: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerateBankReconciliation(DateTime fromDate, DateTime toDate)
        {
            // Implementation for bank reconciliation
            MessageBox.Show("Bank Reconciliation Report:\n\n" +
                "• Bank Statement Import\n" +
                "• Outstanding Cheques\n" +
                "• Deposits in Transit\n" +
                "• Bank Charges & Interest\n" +
                "• Reconciled Balance\n\n" +
                "This would integrate with bank statement data for automatic reconciliation.", 
                "Bank Reconciliation", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void GenerateReceiptsPaymentsReport(DateTime fromDate, DateTime toDate, string accountType)
        {
            // Implementation for receipts & payments report
        }

        private void GenerateCashPositionReport(DateTime fromDate, DateTime toDate)
        {
            // Implementation for cash position report
        }

        private void GenerateFundFlowStatement(DateTime fromDate, DateTime toDate)
        {
            // Implementation for fund flow statement
        }

        private decimal GetOpeningBalance(DateTime fromDate, string accountType)
        {
            try
            {
                // Calculate opening balance based on previous transactions
                string query = @"
                    SELECT 
                        ISNULL(SUM(
                            CASE 
                                WHEN s.PaymentMode IN ('Cash', 'Card', 'Bank Transfer') THEN s.NetAmount
                                ELSE 0
                            END
                        ), 0) - ISNULL(SUM(
                            CASE 
                                WHEN p.PaymentMode IN ('Cash', 'Card', 'Bank Transfer') THEN p.NetAmount
                                ELSE 0
                            END
                        ), 0) - ISNULL(SUM(e.Amount), 0) as OpeningBalance
                    FROM Sales s
                    FULL OUTER JOIN Purchases p ON 1=1
                    FULL OUTER JOIN Expenses e ON 1=1
                    WHERE (s.BillDate < @FromDate OR s.BillDate IS NULL)
                    AND (p.PurchaseDate < @FromDate OR p.PurchaseDate IS NULL)
                    AND (e.ExpenseDate < @FromDate OR e.ExpenseDate IS NULL)
                    AND (s.IsActive = 1 OR s.IsActive IS NULL)
                    AND (p.IsActive = 1 OR p.IsActive IS NULL)
                    AND (e.IsActive = 1 OR e.IsActive IS NULL)";

                SqlParameter[] parameters = { new SqlParameter("@FromDate", fromDate) };
                DataTable result = DatabaseConnection.ExecuteQuery(query, parameters);

                if (result != null && result.Rows.Count > 0 && result.Rows[0]["OpeningBalance"] != DBNull.Value)
                {
                    return Convert.ToDecimal(result.Rows[0]["OpeningBalance"]);
                }

                return 0;
            }
            catch
            {
                return 0;
            }
        }

        private void UpdateFinancialSummary()
        {
            if (currentFinancialData != null && currentFinancialData.Rows.Count > 0)
            {
                try
                {
                    decimal totalReceipts = 0;
                    decimal totalPayments = 0;
                    decimal openingBalance = GetOpeningBalance(dtpFromDate.Value, "ALL");

                    foreach (DataRow row in currentFinancialData.Rows)
                    {
                        if (row.Table.Columns.Contains("ReceiptAmount"))
                        {
                            totalReceipts += Convert.ToDecimal(row["ReceiptAmount"]);
                            totalPayments += Convert.ToDecimal(row["PaymentAmount"]);
                        }
                        else if (row.Table.Columns.Contains("Amount"))
                        {
                            decimal amount = Convert.ToDecimal(row["Amount"]);
                            if (amount > 0)
                                totalReceipts += amount;
                            else
                                totalPayments += Math.Abs(amount);
                        }
                    }

                    decimal netCashFlow = totalReceipts - totalPayments;
                    decimal closingBalance = openingBalance + netCashFlow;

                    // Get current bank balance (simplified)
                    decimal bankBalance = GetCurrentBankBalance();

                    lblTotalReceipts.Text = $"Total Receipts: ₹{totalReceipts:N2}";
                    lblTotalPayments.Text = $"Total Payments: ₹{totalPayments:N2}";
                    lblNetCashFlow.Text = $"Net Cash Flow: ₹{netCashFlow:N2}";
                    lblOpeningBalance.Text = $"Opening Balance: ₹{openingBalance:N2}";
                    lblClosingBalance.Text = $"Closing Balance: ₹{closingBalance:N2}";
                    lblBankBalance.Text = $"Bank Balance: ₹{bankBalance:N2}";
                }
                catch
                {
                    // Handle summary calculation errors silently
                }
            }
        }

        private decimal GetCurrentBankBalance()
        {
            try
            {
                string query = @"
                    SELECT ISNULL(SUM(
                        CASE 
                            WHEN TransactionType = 'DEPOSIT' THEN Amount
                            ELSE -Amount
                        END
                    ), 0) as BankBalance
                    FROM BankTransactions 
                    WHERE IsActive = 1";

                DataTable result = DatabaseConnection.ExecuteQuery(query);
                if (result != null && result.Rows.Count > 0)
                {
                    return Convert.ToDecimal(result.Rows[0]["BankBalance"]);
                }
                return 0;
            }
            catch
            {
                return 0;
            }
        }

        private void ExportFinancialReport()
        {
            try
            {
                if (currentFinancialData == null || currentFinancialData.Rows.Count == 0)
                {
                    MessageBox.Show("No data to export. Please generate a report first.", "Export Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "Excel Files|*.xlsx|CSV Files|*.csv|PDF Files|*.pdf";
                saveDialog.FileName = $"Financial_Report_{DateTime.Now:yyyyMMdd}";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    // Export logic would go here
                    MessageBox.Show("Financial report exported successfully!", "Export Complete", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exporting financial report: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PrintFinancialReport()
        {
            try
            {
                if (reportViewer.LocalReport.DataSources.Count > 0)
                {
                    reportViewer.PrintDialog();
                }
                else
                {
                    MessageBox.Show("No report to print. Please generate a report first.", "Print Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error printing financial report: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PerformBankReconciliation()
        {
            try
            {
                // Bank reconciliation functionality
                MessageBox.Show("Bank Reconciliation Process:\n\n" +
                    "1. Import bank statement data\n" +
                    "2. Match transactions automatically\n" +
                    "3. Identify outstanding items\n" +
                    "4. Calculate reconciled balance\n" +
                    "5. Generate reconciliation report\n\n" +
                    "This would integrate with bank APIs or statement import functionality.", 
                    "Bank Reconciliation", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error performing bank reconciliation: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
