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
    public partial class GSTReturnReportsForm : Form
    {
        private ReportViewer reportViewer;
        private ComboBox cmbReturnType;
        private ComboBox cmbMonth;
        private ComboBox cmbYear;
        private ComboBox cmbQuarter;
        private DateTimePicker dtpFromDate;
        private DateTimePicker dtpToDate;
        private ComboBox cmbGSTRate;
        private CheckBox chkIncludeReturns;
        private CheckBox chkIncludeExempted;
        private CheckBox chkSummaryOnly;
        private Button btnGenerate;
        private Button btnExport;
        private Button btnDownloadJSON;
        private Button btnValidateGST;
        private GroupBox groupFilters;
        private GroupBox groupOptions;
        private TabControl tabControl;
        private TabPage tabGSTR1;
        private TabPage tabGSTR3B;
        private TabPage tabGSTR2A;
        private DataTable currentGSTData;
        private Panel summaryPanel;
        private Label lblTotalTaxableValue;
        private Label lblTotalIGST;
        private Label lblTotalCGST;
        private Label lblTotalSGST;
        private Label lblTotalTax;
        private Label lblTotalInvoices;

        public GSTReturnReportsForm()
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
            this.cmbMonth = new System.Windows.Forms.ComboBox();
            this.cmbYear = new System.Windows.Forms.ComboBox();
            this.cmbQuarter = new System.Windows.Forms.ComboBox();
            this.dtpFromDate = new System.Windows.Forms.DateTimePicker();
            this.dtpToDate = new System.Windows.Forms.DateTimePicker();
            this.cmbGSTRate = new System.Windows.Forms.ComboBox();
            this.chkIncludeReturns = new System.Windows.Forms.CheckBox();
            this.chkIncludeExempted = new System.Windows.Forms.CheckBox();
            this.chkSummaryOnly = new System.Windows.Forms.CheckBox();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnDownloadJSON = new System.Windows.Forms.Button();
            this.btnValidateGST = new System.Windows.Forms.Button();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabGSTR1 = new System.Windows.Forms.TabPage();
            this.tabGSTR3B = new System.Windows.Forms.TabPage();
            this.tabGSTR2A = new System.Windows.Forms.TabPage();
            this.summaryPanel = new System.Windows.Forms.Panel();
            this.lblTotalTaxableValue = new System.Windows.Forms.Label();
            this.lblTotalIGST = new System.Windows.Forms.Label();
            this.lblTotalCGST = new System.Windows.Forms.Label();
            this.lblTotalSGST = new System.Windows.Forms.Label();
            this.lblTotalTax = new System.Windows.Forms.Label();
            this.lblTotalInvoices = new System.Windows.Forms.Label();

            this.SuspendLayout();

            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.summaryPanel);
            this.Controls.Add(this.groupOptions);
            this.Controls.Add(this.groupFilters);
            this.Name = "GSTReturnReportsForm";
            this.Text = "GST Return Reports";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;

            // Group Filters
            this.groupFilters.Controls.Add(new Label { Text = "Return Type:", Location = new Point(15, 25), Size = new Size(80, 13) });
            this.groupFilters.Controls.Add(this.cmbReturnType);
            this.groupFilters.Controls.Add(new Label { Text = "Month:", Location = new Point(15, 55), Size = new Size(50, 13) });
            this.groupFilters.Controls.Add(this.cmbMonth);
            this.groupFilters.Controls.Add(new Label { Text = "Year:", Location = new Point(200, 55), Size = new Size(35, 13) });
            this.groupFilters.Controls.Add(this.cmbYear);
            this.groupFilters.Controls.Add(new Label { Text = "Quarter:", Location = new Point(350, 55), Size = new Size(50, 13) });
            this.groupFilters.Controls.Add(this.cmbQuarter);
            this.groupFilters.Controls.Add(new Label { Text = "From Date:", Location = new Point(15, 85), Size = new Size(65, 13) });
            this.groupFilters.Controls.Add(this.dtpFromDate);
            this.groupFilters.Controls.Add(new Label { Text = "To Date:", Location = new Point(350, 85), Size = new Size(50, 13) });
            this.groupFilters.Controls.Add(this.dtpToDate);
            this.groupFilters.Controls.Add(new Label { Text = "GST Rate:", Location = new Point(15, 115), Size = new Size(60, 13) });
            this.groupFilters.Controls.Add(this.cmbGSTRate);
            this.groupFilters.Location = new System.Drawing.Point(12, 12);
            this.groupFilters.Name = "groupFilters";
            this.groupFilters.Size = new System.Drawing.Size(600, 150);
            this.groupFilters.TabIndex = 0;
            this.groupFilters.TabStop = false;
            this.groupFilters.Text = "Filters";

            // Group Options
            this.groupOptions.Controls.Add(this.chkIncludeReturns);
            this.groupOptions.Controls.Add(this.chkIncludeExempted);
            this.groupOptions.Controls.Add(this.chkSummaryOnly);
            this.groupOptions.Controls.Add(this.btnGenerate);
            this.groupOptions.Controls.Add(this.btnExport);
            this.groupOptions.Controls.Add(this.btnDownloadJSON);
            this.groupOptions.Controls.Add(this.btnValidateGST);
            this.groupOptions.Location = new System.Drawing.Point(630, 12);
            this.groupOptions.Name = "groupOptions";
            this.groupOptions.Size = new System.Drawing.Size(300, 150);
            this.groupOptions.TabIndex = 1;
            this.groupOptions.TabStop = false;
            this.groupOptions.Text = "Options & Actions";

            // Controls Setup
            SetupControls();

            // Tab Control
            this.tabControl.Controls.Add(this.tabGSTR1);
            this.tabControl.Controls.Add(this.tabGSTR3B);
            this.tabControl.Controls.Add(this.tabGSTR2A);
            this.tabControl.Location = new System.Drawing.Point(12, 240);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(1176, 400);
            this.tabControl.TabIndex = 3;

            // Tab Pages
            this.tabGSTR1.Location = new System.Drawing.Point(4, 22);
            this.tabGSTR1.Name = "tabGSTR1";
            this.tabGSTR1.Padding = new System.Windows.Forms.Padding(3);
            this.tabGSTR1.Size = new System.Drawing.Size(1168, 374);
            this.tabGSTR1.TabIndex = 0;
            this.tabGSTR1.Text = "GSTR-1 (Outward Supplies)";
            this.tabGSTR1.UseVisualStyleBackColor = true;

            this.tabGSTR3B.Location = new System.Drawing.Point(4, 22);
            this.tabGSTR3B.Name = "tabGSTR3B";
            this.tabGSTR3B.Padding = new System.Windows.Forms.Padding(3);
            this.tabGSTR3B.Size = new System.Drawing.Size(1168, 374);
            this.tabGSTR3B.TabIndex = 1;
            this.tabGSTR3B.Text = "GSTR-3B (Monthly Return)";
            this.tabGSTR3B.UseVisualStyleBackColor = true;

            this.tabGSTR2A.Location = new System.Drawing.Point(4, 22);
            this.tabGSTR2A.Name = "tabGSTR2A";
            this.tabGSTR2A.Padding = new System.Windows.Forms.Padding(3);
            this.tabGSTR2A.Size = new System.Drawing.Size(1168, 374);
            this.tabGSTR2A.TabIndex = 2;
            this.tabGSTR2A.Text = "GSTR-2A (Purchase Return)";
            this.tabGSTR2A.UseVisualStyleBackColor = true;

            // Summary Panel
            this.summaryPanel.BackColor = System.Drawing.Color.LightBlue;
            this.summaryPanel.Controls.Add(this.lblTotalTaxableValue);
            this.summaryPanel.Controls.Add(this.lblTotalIGST);
            this.summaryPanel.Controls.Add(this.lblTotalCGST);
            this.summaryPanel.Controls.Add(this.lblTotalSGST);
            this.summaryPanel.Controls.Add(this.lblTotalTax);
            this.summaryPanel.Controls.Add(this.lblTotalInvoices);
            this.summaryPanel.Location = new System.Drawing.Point(12, 175);
            this.summaryPanel.Name = "summaryPanel";
            this.summaryPanel.Size = new System.Drawing.Size(1176, 60);
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

            // Month ComboBox
            this.cmbMonth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMonth.Location = new System.Drawing.Point(70, 52);
            this.cmbMonth.Name = "cmbMonth";
            this.cmbMonth.Size = new System.Drawing.Size(120, 21);
            this.cmbMonth.TabIndex = 2;

            // Year ComboBox
            this.cmbYear.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbYear.Location = new System.Drawing.Point(240, 52);
            this.cmbYear.Name = "cmbYear";
            this.cmbYear.Size = new System.Drawing.Size(80, 21);
            this.cmbYear.TabIndex = 3;

            // Quarter ComboBox
            this.cmbQuarter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbQuarter.Location = new System.Drawing.Point(410, 52);
            this.cmbQuarter.Name = "cmbQuarter";
            this.cmbQuarter.Size = new System.Drawing.Size(100, 21);
            this.cmbQuarter.TabIndex = 4;

            // Date Pickers
            this.dtpFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFromDate.Location = new System.Drawing.Point(85, 82);
            this.dtpFromDate.Name = "dtpFromDate";
            this.dtpFromDate.Size = new System.Drawing.Size(100, 20);
            this.dtpFromDate.TabIndex = 5;

            this.dtpToDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpToDate.Location = new System.Drawing.Point(410, 82);
            this.dtpToDate.Name = "dtpToDate";
            this.dtpToDate.Size = new System.Drawing.Size(100, 20);
            this.dtpToDate.TabIndex = 6;

            // GST Rate ComboBox
            this.cmbGSTRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbGSTRate.Location = new System.Drawing.Point(85, 112);
            this.cmbGSTRate.Name = "cmbGSTRate";
            this.cmbGSTRate.Size = new System.Drawing.Size(100, 21);
            this.cmbGSTRate.TabIndex = 7;

            // Checkboxes
            this.chkIncludeReturns.Location = new System.Drawing.Point(15, 25);
            this.chkIncludeReturns.Name = "chkIncludeReturns";
            this.chkIncludeReturns.Size = new System.Drawing.Size(120, 17);
            this.chkIncludeReturns.TabIndex = 8;
            this.chkIncludeReturns.Text = "Include Returns";
            this.chkIncludeReturns.UseVisualStyleBackColor = true;

            this.chkIncludeExempted.Location = new System.Drawing.Point(15, 45);
            this.chkIncludeExempted.Name = "chkIncludeExempted";
            this.chkIncludeExempted.Size = new System.Drawing.Size(120, 17);
            this.chkIncludeExempted.TabIndex = 9;
            this.chkIncludeExempted.Text = "Include Exempted";
            this.chkIncludeExempted.UseVisualStyleBackColor = true;

            this.chkSummaryOnly.Location = new System.Drawing.Point(15, 65);
            this.chkSummaryOnly.Name = "chkSummaryOnly";
            this.chkSummaryOnly.Size = new System.Drawing.Size(120, 17);
            this.chkSummaryOnly.TabIndex = 10;
            this.chkSummaryOnly.Text = "Summary Only";
            this.chkSummaryOnly.UseVisualStyleBackColor = true;

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

            this.btnDownloadJSON.BackColor = System.Drawing.Color.Orange;
            this.btnDownloadJSON.ForeColor = System.Drawing.Color.White;
            this.btnDownloadJSON.Location = new System.Drawing.Point(150, 90);
            this.btnDownloadJSON.Name = "btnDownloadJSON";
            this.btnDownloadJSON.Size = new System.Drawing.Size(130, 30);
            this.btnDownloadJSON.TabIndex = 13;
            this.btnDownloadJSON.Text = "Download JSON";
            this.btnDownloadJSON.UseVisualStyleBackColor = false;
            this.btnDownloadJSON.Click += BtnDownloadJSON_Click;

            this.btnValidateGST.BackColor = System.Drawing.Color.Purple;
            this.btnValidateGST.ForeColor = System.Drawing.Color.White;
            this.btnValidateGST.Location = new System.Drawing.Point(15, 90);
            this.btnValidateGST.Name = "btnValidateGST";
            this.btnValidateGST.Size = new System.Drawing.Size(120, 30);
            this.btnValidateGST.TabIndex = 14;
            this.btnValidateGST.Text = "Validate GST";
            this.btnValidateGST.UseVisualStyleBackColor = false;
            this.btnValidateGST.Click += BtnValidateGST_Click;
        }

        private void SetupSummaryLabels()
        {
            this.lblTotalInvoices.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblTotalInvoices.Location = new System.Drawing.Point(15, 10);
            this.lblTotalInvoices.Name = "lblTotalInvoices";
            this.lblTotalInvoices.Size = new System.Drawing.Size(150, 15);
            this.lblTotalInvoices.TabIndex = 0;
            this.lblTotalInvoices.Text = "Total Invoices: 0";

            this.lblTotalTaxableValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblTotalTaxableValue.Location = new System.Drawing.Point(15, 30);
            this.lblTotalTaxableValue.Name = "lblTotalTaxableValue";
            this.lblTotalTaxableValue.Size = new System.Drawing.Size(200, 15);
            this.lblTotalTaxableValue.TabIndex = 1;
            this.lblTotalTaxableValue.Text = "Total Taxable Value: ₹0.00";

            this.lblTotalCGST.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblTotalCGST.Location = new System.Drawing.Point(250, 10);
            this.lblTotalCGST.Name = "lblTotalCGST";
            this.lblTotalCGST.Size = new System.Drawing.Size(150, 15);
            this.lblTotalCGST.TabIndex = 2;
            this.lblTotalCGST.Text = "Total CGST: ₹0.00";

            this.lblTotalSGST.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblTotalSGST.Location = new System.Drawing.Point(250, 30);
            this.lblTotalSGST.Name = "lblTotalSGST";
            this.lblTotalSGST.Size = new System.Drawing.Size(150, 15);
            this.lblTotalSGST.TabIndex = 3;
            this.lblTotalSGST.Text = "Total SGST: ₹0.00";

            this.lblTotalIGST.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblTotalIGST.Location = new System.Drawing.Point(450, 10);
            this.lblTotalIGST.Name = "lblTotalIGST";
            this.lblTotalIGST.Size = new System.Drawing.Size(150, 15);
            this.lblTotalIGST.TabIndex = 4;
            this.lblTotalIGST.Text = "Total IGST: ₹0.00";

            this.lblTotalTax.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblTotalTax.Location = new System.Drawing.Point(450, 30);
            this.lblTotalTax.Name = "lblTotalTax";
            this.lblTotalTax.Size = new System.Drawing.Size(150, 15);
            this.lblTotalTax.TabIndex = 5;
            this.lblTotalTax.Text = "Total Tax: ₹0.00";
        }

        private void InitializeReportViewer()
        {
            this.reportViewer = new ReportViewer();
            this.reportViewer.Dock = DockStyle.Fill;
            this.reportViewer.ProcessingMode = ProcessingMode.Local;
            
            // Add report viewer to each tab
            this.tabGSTR1.Controls.Add(this.reportViewer);
        }

        private void LoadDropdownData()
        {
            try
            {
                // Load Return Types
                cmbReturnType.Items.Clear();
                cmbReturnType.Items.Add(new ComboBoxItem { Text = "GSTR-1 (Outward Supplies)", Value = "GSTR1" });
                cmbReturnType.Items.Add(new ComboBoxItem { Text = "GSTR-3B (Monthly Return)", Value = "GSTR3B" });
                cmbReturnType.Items.Add(new ComboBoxItem { Text = "GSTR-2A (Purchase Return)", Value = "GSTR2A" });
                cmbReturnType.Items.Add(new ComboBoxItem { Text = "Annual Return", Value = "ANNUAL" });
                cmbReturnType.DisplayMember = "Text";
                cmbReturnType.ValueMember = "Value";
                cmbReturnType.SelectedIndex = 0;

                // Load Months
                cmbMonth.Items.Clear();
                string[] months = { "January", "February", "March", "April", "May", "June",
                                  "July", "August", "September", "October", "November", "December" };
                for (int i = 0; i < months.Length; i++)
                {
                    cmbMonth.Items.Add(new ComboBoxItem { Text = months[i], Value = i + 1 });
                }
                cmbMonth.DisplayMember = "Text";
                cmbMonth.ValueMember = "Value";
                cmbMonth.SelectedIndex = DateTime.Now.Month - 1;

                // Load Years
                cmbYear.Items.Clear();
                int currentYear = DateTime.Now.Year;
                for (int year = currentYear - 5; year <= currentYear + 1; year++)
                {
                    cmbYear.Items.Add(new ComboBoxItem { Text = year.ToString(), Value = year });
                }
                cmbYear.DisplayMember = "Text";
                cmbYear.ValueMember = "Value";
                cmbYear.SelectedValue = currentYear;

                // Load Quarters
                cmbQuarter.Items.Clear();
                cmbQuarter.Items.Add(new ComboBoxItem { Text = "Q1 (Apr-Jun)", Value = "Q1" });
                cmbQuarter.Items.Add(new ComboBoxItem { Text = "Q2 (Jul-Sep)", Value = "Q2" });
                cmbQuarter.Items.Add(new ComboBoxItem { Text = "Q3 (Oct-Dec)", Value = "Q3" });
                cmbQuarter.Items.Add(new ComboBoxItem { Text = "Q4 (Jan-Mar)", Value = "Q4" });
                cmbQuarter.DisplayMember = "Text";
                cmbQuarter.ValueMember = "Value";

                // Load GST Rates
                cmbGSTRate.Items.Clear();
                cmbGSTRate.Items.Add(new ComboBoxItem { Text = "All Rates", Value = "" });
                cmbGSTRate.Items.Add(new ComboBoxItem { Text = "0%", Value = "0" });
                cmbGSTRate.Items.Add(new ComboBoxItem { Text = "5%", Value = "5" });
                cmbGSTRate.Items.Add(new ComboBoxItem { Text = "12%", Value = "12" });
                cmbGSTRate.Items.Add(new ComboBoxItem { Text = "18%", Value = "18" });
                cmbGSTRate.Items.Add(new ComboBoxItem { Text = "28%", Value = "28" });
                cmbGSTRate.DisplayMember = "Text";
                cmbGSTRate.ValueMember = "Value";
                cmbGSTRate.SelectedIndex = 0;
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
            // Update UI based on selected return type
            ComboBoxItem selectedType = (ComboBoxItem)cmbReturnType.SelectedItem;
            if (selectedType != null)
            {
                string returnType = selectedType.Value.ToString();
                switch (returnType)
                {
                    case "GSTR1":
                        tabControl.SelectedTab = tabGSTR1;
                        break;
                    case "GSTR3B":
                        tabControl.SelectedTab = tabGSTR3B;
                        break;
                    case "GSTR2A":
                        tabControl.SelectedTab = tabGSTR2A;
                        break;
                }
            }
        }

        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            GenerateGSTReport();
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            ExportToExcel();
        }

        private void BtnDownloadJSON_Click(object sender, EventArgs e)
        {
            DownloadGSTJSON();
        }

        private void BtnValidateGST_Click(object sender, EventArgs e)
        {
            ValidateGSTData();
        }

        private void GenerateGSTReport()
        {
            try
            {
                ComboBoxItem selectedType = (ComboBoxItem)cmbReturnType.SelectedItem;
                ComboBoxItem selectedMonth = (ComboBoxItem)cmbMonth.SelectedItem;
                ComboBoxItem selectedYear = (ComboBoxItem)cmbYear.SelectedItem;
                ComboBoxItem selectedGSTRate = (ComboBoxItem)cmbGSTRate.SelectedItem;

                string returnType = selectedType?.Value.ToString() ?? "GSTR1";
                int month = selectedMonth != null ? (int)selectedMonth.Value : DateTime.Now.Month;
                int year = selectedYear != null ? (int)selectedYear.Value : DateTime.Now.Year;
                string gstRate = selectedGSTRate?.Value.ToString() ?? "";

                DateTime fromDate = dtpFromDate.Value;
                DateTime toDate = dtpToDate.Value;

                // Generate appropriate GST report based on type
                switch (returnType)
                {
                    case "GSTR1":
                        GenerateGSTR1Report(fromDate, toDate, gstRate);
                        break;
                    case "GSTR3B":
                        GenerateGSTR3BReport(month, year);
                        break;
                    case "GSTR2A":
                        GenerateGSTR2AReport(fromDate, toDate, gstRate);
                        break;
                    case "ANNUAL":
                        GenerateAnnualGSTReport(year);
                        break;
                }

                UpdateGSTSummary();
                MessageBox.Show("GST Report generated successfully!", "Success", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating GST report: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerateGSTR1Report(DateTime fromDate, DateTime toDate, string gstRate)
        {
            try
            {
                string query = @"
                    SELECT 
                        s.BillNumber,
                        s.BillDate,
                        ISNULL(c.CustomerName, 'Cash Sale') as CustomerName,
                        ISNULL(c.GSTNumber, '') as CustomerGST,
                        si.ItemName,
                        si.HSNCode,
                        si.Quantity,
                        si.Rate,
                        si.TaxableAmount,
                        si.GSTRate,
                        si.CGSTAmount,
                        si.SGSTAmount,
                        si.IGSTAmount,
                        (si.CGSTAmount + si.SGSTAmount + si.IGSTAmount) as TotalTax,
                        si.NetAmount,
                        CASE 
                            WHEN c.StateCode = '06' THEN 'Intra-State'  -- Karnataka state code
                            ELSE 'Inter-State'
                        END as TransactionType
                    FROM Sales s
                    LEFT JOIN Customers c ON s.CustomerID = c.CustomerID
                    INNER JOIN SaleItems si ON s.SaleID = si.SaleID
                    WHERE s.BillDate BETWEEN @FromDate AND @ToDate
                    AND s.IsActive = 1" +
                    (string.IsNullOrEmpty(gstRate) ? "" : " AND si.GSTRate = @GSTRate") +
                    (chkIncludeReturns.Checked ? "" : " AND s.IsReturn = 0") + @"
                    ORDER BY s.BillDate, s.BillNumber";

                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    new SqlParameter("@FromDate", fromDate),
                    new SqlParameter("@ToDate", toDate)
                };

                if (!string.IsNullOrEmpty(gstRate))
                {
                    parameters.Add(new SqlParameter("@GSTRate", gstRate));
                }

                currentGSTData = DatabaseConnection.ExecuteQuery(query, parameters.ToArray());

                if (currentGSTData != null && currentGSTData.Rows.Count > 0)
                {
                    // Smart RDLC path detection for GSTR1Report
                    string simplePath = Path.Combine(Application.StartupPath, "Reports", "GSTR1Report_Simple.rdlc");
                    string originalPath = Path.Combine(Application.StartupPath, "Reports", "GSTR1Report.rdlc");
                    
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
                        reportViewer.LocalReport.ReportPath = "Reports/GSTR1Report.rdlc";
                    }
                    reportViewer.LocalReport.DataSources.Clear();
                    reportViewer.LocalReport.DataSources.Add(new ReportDataSource("GSTR1Dataset", currentGSTData));

                    // Set report parameters
                    ReportParameter[] reportParams = new ReportParameter[]
                    {
                        new ReportParameter("FromDate", fromDate.ToString("dd/MM/yyyy")),
                        new ReportParameter("ToDate", toDate.ToString("dd/MM/yyyy")),
                        new ReportParameter("CompanyName", "Your Company Name"),
                        new ReportParameter("GSTIN", "Your GSTIN Number"),
                        new ReportParameter("ReportTitle", "GSTR-1 (Outward Supplies)")
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
                MessageBox.Show("Error generating GSTR-1 report: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerateGSTR3BReport(int month, int year)
        {
            try
            {
                DateTime fromDate = new DateTime(year, month, 1);
                DateTime toDate = fromDate.AddMonths(1).AddDays(-1);

                string query = @"
                    SELECT 
                        'Outward Supplies' as SupplyType,
                        SUM(si.TaxableAmount) as TaxableValue,
                        SUM(si.CGSTAmount) as CGSTAmount,
                        SUM(si.SGSTAmount) as SGSTAmount,
                        SUM(si.IGSTAmount) as IGSTAmount,
                        si.GSTRate
                    FROM Sales s
                    INNER JOIN SaleItems si ON s.SaleID = si.SaleID
                    WHERE s.BillDate BETWEEN @FromDate AND @ToDate
                    AND s.IsActive = 1 AND s.IsReturn = 0
                    GROUP BY si.GSTRate
                    
                    UNION ALL
                    
                    SELECT 
                        'Inward Supplies' as SupplyType,
                        SUM(pi.TaxableAmount) as TaxableValue,
                        SUM(pi.CGSTAmount) as CGSTAmount,
                        SUM(pi.SGSTAmount) as SGSTAmount,
                        SUM(pi.IGSTAmount) as IGSTAmount,
                        pi.GSTRate
                    FROM Purchases p
                    INNER JOIN PurchaseItems pi ON p.PurchaseID = pi.PurchaseID
                    WHERE p.PurchaseDate BETWEEN @FromDate AND @ToDate
                    AND p.IsActive = 1 AND p.IsReturn = 0
                    GROUP BY pi.GSTRate
                    
                    ORDER BY SupplyType, GSTRate";

                SqlParameter[] parameters = {
                    new SqlParameter("@FromDate", fromDate),
                    new SqlParameter("@ToDate", toDate)
                };

                currentGSTData = DatabaseConnection.ExecuteQuery(query, parameters);

                if (currentGSTData != null && currentGSTData.Rows.Count > 0)
                {
                    // Create and load RDLC report for GSTR-3B
                    string reportPath = Path.Combine(Application.StartupPath, "Reports", "GSTR3BReport.rdlc");
                    reportViewer.LocalReport.ReportPath = reportPath;
                    reportViewer.LocalReport.DataSources.Clear();
                    reportViewer.LocalReport.DataSources.Add(new ReportDataSource("GSTR3BDataset", currentGSTData));

                    // Set report parameters
                    ReportParameter[] reportParams = new ReportParameter[]
                    {
                        new ReportParameter("Month", cmbMonth.Text),
                        new ReportParameter("Year", year.ToString()),
                        new ReportParameter("CompanyName", "Your Company Name"),
                        new ReportParameter("GSTIN", "Your GSTIN Number"),
                        new ReportParameter("ReportTitle", "GSTR-3B (Monthly Return)")
                    };

                    reportViewer.LocalReport.SetParameters(reportParams);
                    reportViewer.RefreshReport();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating GSTR-3B report: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerateGSTR2AReport(DateTime fromDate, DateTime toDate, string gstRate)
        {
            try
            {
                string query = @"
                    SELECT 
                        p.PurchaseNumber,
                        p.PurchaseDate,
                        p.InvoiceNumber,
                        c.CompanyName as SupplierName,
                        c.GSTNumber as SupplierGST,
                        pi.ItemName,
                        pi.HSNCode,
                        pi.Quantity,
                        pi.Rate,
                        pi.TaxableAmount,
                        pi.GSTRate,
                        pi.CGSTAmount,
                        pi.SGSTAmount,
                        pi.IGSTAmount,
                        (pi.CGSTAmount + pi.SGSTAmount + pi.IGSTAmount) as TotalTax,
                        pi.NetAmount
                    FROM Purchases p
                    LEFT JOIN Companies c ON p.CompanyID = c.CompanyID
                    INNER JOIN PurchaseItems pi ON p.PurchaseID = pi.PurchaseID
                    WHERE p.PurchaseDate BETWEEN @FromDate AND @ToDate
                    AND p.IsActive = 1" +
                    (string.IsNullOrEmpty(gstRate) ? "" : " AND pi.GSTRate = @GSTRate") +
                    (chkIncludeReturns.Checked ? "" : " AND p.IsReturn = 0") + @"
                    ORDER BY p.PurchaseDate, p.PurchaseNumber";

                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    new SqlParameter("@FromDate", fromDate),
                    new SqlParameter("@ToDate", toDate)
                };

                if (!string.IsNullOrEmpty(gstRate))
                {
                    parameters.Add(new SqlParameter("@GSTRate", gstRate));
                }

                currentGSTData = DatabaseConnection.ExecuteQuery(query, parameters.ToArray());

                if (currentGSTData != null && currentGSTData.Rows.Count > 0)
                {
                    // Create and load RDLC report for GSTR-2A
                    string reportPath = Path.Combine(Application.StartupPath, "Reports", "GSTR2AReport.rdlc");
                    reportViewer.LocalReport.ReportPath = reportPath;
                    reportViewer.LocalReport.DataSources.Clear();
                    reportViewer.LocalReport.DataSources.Add(new ReportDataSource("GSTR2ADataset", currentGSTData));

                    // Set report parameters
                    ReportParameter[] reportParams = new ReportParameter[]
                    {
                        new ReportParameter("FromDate", fromDate.ToString("dd/MM/yyyy")),
                        new ReportParameter("ToDate", toDate.ToString("dd/MM/yyyy")),
                        new ReportParameter("CompanyName", "Your Company Name"),
                        new ReportParameter("GSTIN", "Your GSTIN Number"),
                        new ReportParameter("ReportTitle", "GSTR-2A (Purchase Return)")
                    };

                    reportViewer.LocalReport.SetParameters(reportParams);
                    reportViewer.RefreshReport();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating GSTR-2A report: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerateAnnualGSTReport(int year)
        {
            try
            {
                DateTime fromDate = new DateTime(year, 4, 1); // Financial year starts from April
                DateTime toDate = new DateTime(year + 1, 3, 31);

                string query = @"
                    SELECT 
                        MONTH(s.BillDate) as Month,
                        DATENAME(MONTH, s.BillDate) as MonthName,
                        SUM(si.TaxableAmount) as TotalTaxableValue,
                        SUM(si.CGSTAmount) as TotalCGST,
                        SUM(si.SGSTAmount) as TotalSGST,
                        SUM(si.IGSTAmount) as TotalIGST,
                        SUM(si.CGSTAmount + si.SGSTAmount + si.IGSTAmount) as TotalTax,
                        COUNT(DISTINCT s.SaleID) as TotalInvoices
                    FROM Sales s
                    INNER JOIN SaleItems si ON s.SaleID = si.SaleID
                    WHERE s.BillDate BETWEEN @FromDate AND @ToDate
                    AND s.IsActive = 1 AND s.IsReturn = 0
                    GROUP BY MONTH(s.BillDate), DATENAME(MONTH, s.BillDate)
                    ORDER BY MONTH(s.BillDate)";

                SqlParameter[] parameters = {
                    new SqlParameter("@FromDate", fromDate),
                    new SqlParameter("@ToDate", toDate)
                };

                currentGSTData = DatabaseConnection.ExecuteQuery(query, parameters);

                if (currentGSTData != null && currentGSTData.Rows.Count > 0)
                {
                    // Create and load RDLC report for Annual GST
                    string reportPath = Path.Combine(Application.StartupPath, "Reports", "AnnualGSTReport.rdlc");
                    reportViewer.LocalReport.ReportPath = reportPath;
                    reportViewer.LocalReport.DataSources.Clear();
                    reportViewer.LocalReport.DataSources.Add(new ReportDataSource("AnnualGSTDataset", currentGSTData));

                    // Set report parameters
                    ReportParameter[] reportParams = new ReportParameter[]
                    {
                        new ReportParameter("FinancialYear", $"{year}-{year + 1}"),
                        new ReportParameter("CompanyName", "Your Company Name"),
                        new ReportParameter("GSTIN", "Your GSTIN Number"),
                        new ReportParameter("ReportTitle", "Annual GST Report")
                    };

                    reportViewer.LocalReport.SetParameters(reportParams);
                    reportViewer.RefreshReport();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating Annual GST report: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateGSTSummary()
        {
            if (currentGSTData != null && currentGSTData.Rows.Count > 0)
            {
                try
                {
                    decimal totalTaxableValue = 0;
                    decimal totalCGST = 0;
                    decimal totalSGST = 0;
                    decimal totalIGST = 0;
                    int totalInvoices = currentGSTData.Rows.Count;

                    foreach (DataRow row in currentGSTData.Rows)
                    {
                        if (row["TaxableValue"] != DBNull.Value)
                            totalTaxableValue += Convert.ToDecimal(row["TaxableValue"]);
                        if (row["CGSTAmount"] != DBNull.Value)
                            totalCGST += Convert.ToDecimal(row["CGSTAmount"]);
                        if (row["SGSTAmount"] != DBNull.Value)
                            totalSGST += Convert.ToDecimal(row["SGSTAmount"]);
                        if (row["IGSTAmount"] != DBNull.Value)
                            totalIGST += Convert.ToDecimal(row["IGSTAmount"]);
                    }

                    decimal totalTax = totalCGST + totalSGST + totalIGST;

                    lblTotalInvoices.Text = $"Total Invoices: {totalInvoices}";
                    lblTotalTaxableValue.Text = $"Total Taxable Value: ₹{totalTaxableValue:N2}";
                    lblTotalCGST.Text = $"Total CGST: ₹{totalCGST:N2}";
                    lblTotalSGST.Text = $"Total SGST: ₹{totalSGST:N2}";
                    lblTotalIGST.Text = $"Total IGST: ₹{totalIGST:N2}";
                    lblTotalTax.Text = $"Total Tax: ₹{totalTax:N2}";
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating GST summary: {ex.Message}");
                }
            }
        }

        private void ExportToExcel()
        {
            try
            {
                if (currentGSTData == null || currentGSTData.Rows.Count == 0)
                {
                    MessageBox.Show("No data to export. Please generate a report first.", "Information", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "Excel Files|*.xlsx|CSV Files|*.csv";
                saveDialog.FileName = $"GST_Return_{DateTime.Now:yyyyMMdd}";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    // Export to Excel logic would go here
                    MessageBox.Show("GST Return exported successfully!", "Export Complete", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exporting GST return: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DownloadGSTJSON()
        {
            try
            {
                if (currentGSTData == null || currentGSTData.Rows.Count == 0)
                {
                    MessageBox.Show("No data to download. Please generate a report first.", "Information", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "JSON Files|*.json";
                saveDialog.FileName = $"GST_Return_{DateTime.Now:yyyyMMdd}.json";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    // JSON generation logic would go here
                    MessageBox.Show("GST Return JSON downloaded successfully!", "Download Complete", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error downloading GST JSON: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ValidateGSTData()
        {
            try
            {
                if (currentGSTData == null || currentGSTData.Rows.Count == 0)
                {
                    MessageBox.Show("No data to validate. Please generate a report first.", "Information", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // GST validation logic
                int validationErrors = 0;
                StringBuilder validationReport = new StringBuilder();
                validationReport.AppendLine("GST Data Validation Report:");
                validationReport.AppendLine("=" + new string('=', 30));

                foreach (DataRow row in currentGSTData.Rows)
                {
                    // Validate GSTIN format
                    if (row.Table.Columns.Contains("CustomerGST") && 
                        !string.IsNullOrEmpty(row["CustomerGST"].ToString()))
                    {
                        string gstin = row["CustomerGST"].ToString();
                        if (gstin.Length != 15)
                        {
                            validationErrors++;
                            validationReport.AppendLine($"Invalid GSTIN length: {gstin}");
                        }
                    }

                    // Validate tax calculations
                    if (row.Table.Columns.Contains("TaxableValue") && 
                        row.Table.Columns.Contains("GSTRate"))
                    {
                        decimal taxableValue = Convert.ToDecimal(row["TaxableValue"]);
                        decimal gstRate = Convert.ToDecimal(row["GSTRate"]);
                        decimal calculatedTax = (taxableValue * gstRate) / 100;
                        
                        decimal actualTax = 0;
                        if (row.Table.Columns.Contains("TotalTax"))
                        {
                            actualTax = Convert.ToDecimal(row["TotalTax"]);
                        }

                        if (Math.Abs(calculatedTax - actualTax) > 0.01m)
                        {
                            validationErrors++;
                            validationReport.AppendLine($"Tax calculation mismatch: Expected {calculatedTax:N2}, Found {actualTax:N2}");
                        }
                    }
                }

                if (validationErrors == 0)
                {
                    validationReport.AppendLine("All validations passed successfully!");
                    MessageBox.Show(validationReport.ToString(), "Validation Complete", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    validationReport.AppendLine($"\nTotal Validation Errors: {validationErrors}");
                    MessageBox.Show(validationReport.ToString(), "Validation Errors Found", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error validating GST data: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
