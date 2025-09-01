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
    public partial class DailyReportsForm : Form
    {
        private ReportViewer reportViewer;
        private ComboBox cmbReportType;
        private DateTimePicker dtpFromDate;
        private DateTimePicker dtpToDate;
        private ComboBox cmbCustomer;
        private ComboBox cmbSupplier;
        private ComboBox cmbPaymentMode;
        private Button btnGenerate;
        private Button btnExport;
        private Button btnPrint;
        private GroupBox groupFilters;
        private DataTable currentReportData;

        public DailyReportsForm()
        {
            InitializeComponent();
            InitializeReportViewer();
            LoadDropdownData();
            SetDefaultDates();
        }

        private void InitializeComponent()
        {
            this.groupFilters = new System.Windows.Forms.GroupBox();
            this.cmbReportType = new System.Windows.Forms.ComboBox();
            this.dtpFromDate = new System.Windows.Forms.DateTimePicker();
            this.dtpToDate = new System.Windows.Forms.DateTimePicker();
            this.cmbCustomer = new System.Windows.Forms.ComboBox();
            this.cmbSupplier = new System.Windows.Forms.ComboBox();
            this.cmbPaymentMode = new System.Windows.Forms.ComboBox();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.groupFilters.SuspendLayout();
            this.SuspendLayout();
            
            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.Controls.Add(this.groupFilters);
            this.Name = "DailyReportsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Daily Reports - Retail Management System";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            
            // groupFilters
            this.groupFilters.Controls.Add(this.btnPrint);
            this.groupFilters.Controls.Add(this.btnExport);
            this.groupFilters.Controls.Add(this.btnGenerate);
            this.groupFilters.Controls.Add(this.cmbPaymentMode);
            this.groupFilters.Controls.Add(this.cmbSupplier);
            this.groupFilters.Controls.Add(this.cmbCustomer);
            this.groupFilters.Controls.Add(this.dtpToDate);
            this.groupFilters.Controls.Add(this.dtpFromDate);
            this.groupFilters.Controls.Add(this.cmbReportType);
            this.groupFilters.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupFilters.Location = new System.Drawing.Point(0, 0);
            this.groupFilters.Name = "groupFilters";
            this.groupFilters.Size = new System.Drawing.Size(1200, 80);
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
            
            // From Date
            this.dtpFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFromDate.Location = new System.Drawing.Point(180, 25);
            this.dtpFromDate.Name = "dtpFromDate";
            this.dtpFromDate.Size = new System.Drawing.Size(100, 20);
            this.dtpFromDate.TabIndex = 1;
            
            // To Date
            this.dtpToDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpToDate.Location = new System.Drawing.Point(290, 25);
            this.dtpToDate.Name = "dtpToDate";
            this.dtpToDate.Size = new System.Drawing.Size(100, 20);
            this.dtpToDate.TabIndex = 2;
            
            // Customer
            this.cmbCustomer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCustomer.FormattingEnabled = true;
            this.cmbCustomer.Location = new System.Drawing.Point(405, 25);
            this.cmbCustomer.Name = "cmbCustomer";
            this.cmbCustomer.Size = new System.Drawing.Size(150, 21);
            this.cmbCustomer.TabIndex = 3;
            this.cmbCustomer.Visible = false;
            
            // Supplier
            this.cmbSupplier.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSupplier.FormattingEnabled = true;
            this.cmbSupplier.Location = new System.Drawing.Point(405, 25);
            this.cmbSupplier.Name = "cmbSupplier";
            this.cmbSupplier.Size = new System.Drawing.Size(150, 21);
            this.cmbSupplier.TabIndex = 4;
            this.cmbSupplier.Visible = false;
            
            // Payment Mode
            this.cmbPaymentMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPaymentMode.FormattingEnabled = true;
            this.cmbPaymentMode.Location = new System.Drawing.Point(570, 25);
            this.cmbPaymentMode.Name = "cmbPaymentMode";
            this.cmbPaymentMode.Size = new System.Drawing.Size(100, 21);
            this.cmbPaymentMode.TabIndex = 5;
            this.cmbPaymentMode.Visible = false;
            
            // Generate Button
            this.btnGenerate.BackColor = System.Drawing.Color.Green;
            this.btnGenerate.ForeColor = System.Drawing.Color.White;
            this.btnGenerate.Location = new System.Drawing.Point(690, 23);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(80, 25);
            this.btnGenerate.TabIndex = 6;
            this.btnGenerate.Text = "Generate";
            this.btnGenerate.UseVisualStyleBackColor = false;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            
            // Export Button
            this.btnExport.BackColor = System.Drawing.Color.Blue;
            this.btnExport.ForeColor = System.Drawing.Color.White;
            this.btnExport.Location = new System.Drawing.Point(780, 23);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(80, 25);
            this.btnExport.TabIndex = 7;
            this.btnExport.Text = "Export PDF";
            this.btnExport.UseVisualStyleBackColor = false;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            
            // Print Button
            this.btnPrint.BackColor = System.Drawing.Color.DarkBlue;
            this.btnPrint.ForeColor = System.Drawing.Color.White;
            this.btnPrint.Location = new System.Drawing.Point(870, 23);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(80, 25);
            this.btnPrint.TabIndex = 8;
            this.btnPrint.Text = "Print";
            this.btnPrint.UseVisualStyleBackColor = false;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            
            this.groupFilters.ResumeLayout(false);
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
            
            this.Controls.Add(reportViewer);
            reportViewer.BringToFront();
            groupFilters.BringToFront();
        }

        private void LoadDropdownData()
        {
            try
            {
                // Load Report Types
                cmbReportType.Items.Clear();
                cmbReportType.Items.Add(new ComboBoxItem { Text = "Daily Sales Summary", Value = "DailySales" });
                cmbReportType.Items.Add(new ComboBoxItem { Text = "Daily Purchase Summary", Value = "DailyPurchase" });
                cmbReportType.Items.Add(new ComboBoxItem { Text = "Sales by Customer", Value = "SalesByCustomer" });
                cmbReportType.Items.Add(new ComboBoxItem { Text = "Purchase by Supplier", Value = "PurchaseBySupplier" });
                cmbReportType.Items.Add(new ComboBoxItem { Text = "Payment Mode Analysis", Value = "PaymentMode" });
                cmbReportType.Items.Add(new ComboBoxItem { Text = "Top Selling Items", Value = "TopSelling" });
                cmbReportType.Items.Add(new ComboBoxItem { Text = "Cash Flow Summary", Value = "CashFlow" });
                cmbReportType.Items.Add(new ComboBoxItem { Text = "GST Summary", Value = "GSTSummary" });
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
                
                // Load Payment Modes
                cmbPaymentMode.Items.Clear();
                cmbPaymentMode.Items.Add(new ComboBoxItem { Text = "All Modes", Value = "All" });
                cmbPaymentMode.Items.Add(new ComboBoxItem { Text = "Cash", Value = "Cash" });
                cmbPaymentMode.Items.Add(new ComboBoxItem { Text = "Card", Value = "Card" });
                cmbPaymentMode.Items.Add(new ComboBoxItem { Text = "UPI", Value = "UPI" });
                cmbPaymentMode.Items.Add(new ComboBoxItem { Text = "Wallet", Value = "Wallet" });
                cmbPaymentMode.Items.Add(new ComboBoxItem { Text = "Credit", Value = "Credit" });
                cmbPaymentMode.DisplayMember = "Text";
                cmbPaymentMode.ValueMember = "Value";
                cmbPaymentMode.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading dropdown data: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetDefaultDates()
        {
            dtpFromDate.Value = DateTime.Today;
            dtpToDate.Value = DateTime.Today;
        }

        private void cmbReportType_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Show/hide relevant filters based on report type
            ComboBoxItem selectedReport = (ComboBoxItem)cmbReportType.SelectedItem;
            if (selectedReport != null)
            {
                string reportType = selectedReport.Value.ToString();
                
                // Hide all filters first
                cmbCustomer.Visible = false;
                cmbSupplier.Visible = false;
                cmbPaymentMode.Visible = false;
                
                // Show relevant filters
                switch (reportType)
                {
                    case "SalesByCustomer":
                        cmbCustomer.Visible = true;
                        break;
                    case "PurchaseBySupplier":
                        cmbSupplier.Visible = true;
                        break;
                    case "PaymentMode":
                        cmbPaymentMode.Visible = true;
                        break;
                }
            }
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            try
            {
                ComboBoxItem selectedReport = (ComboBoxItem)cmbReportType.SelectedItem;
                if (selectedReport == null)
                {
                    MessageBox.Show("Please select a report type.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string reportType = selectedReport.Value.ToString();
                GenerateReport(reportType);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating report: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerateReport(string reportType)
        {
            try
            {
                // Clear previous report
                reportViewer.Reset();
                
                string reportPath = "";
                string dataSetName = "";
                DataTable reportData = null;
                
                switch (reportType)
                {
                    case "DailySales":
                        reportPath = "Reports/DailySummaryReport.rdlc";
                        dataSetName = "DailySalesDataSet";
                        reportData = GetDailySalesData();
                        break;
                    case "DailyPurchase":
                        reportPath = "Reports/DailyPurchaseReport.rdlc";
                        dataSetName = "DailyPurchaseDataSet";
                        reportData = GetDailyPurchaseData();
                        break;
                    case "SalesByCustomer":
                        reportPath = "Reports/CustomerSalesReport.rdlc";
                        dataSetName = "CustomerSalesDataSet";
                        reportData = GetSalesByCustomerData();
                        break;
                    case "PurchaseBySupplier":
                        reportPath = "Reports/SupplierPurchaseReport.rdlc";
                        dataSetName = "SupplierPurchaseDataSet";
                        reportData = GetPurchaseBySupplierData();
                        break;
                    case "PaymentMode":
                        reportPath = "Reports/PaymentModeReport.rdlc";
                        dataSetName = "PaymentModeDataSet";
                        reportData = GetPaymentModeData();
                        break;
                    case "TopSelling":
                        reportPath = "Reports/TopSellingReport.rdlc";
                        dataSetName = "TopSellingDataSet";
                        reportData = GetTopSellingItemsData();
                        break;
                    case "CashFlow":
                        reportPath = "Reports/CashFlowReport.rdlc";
                        dataSetName = "CashFlowDataSet";
                        reportData = GetCashFlowData();
                        break;
                    case "GSTSummary":
                        reportPath = "Reports/GSTReport.rdlc";
                        dataSetName = "GSTDataSet";
                        reportData = GetGSTSummaryData();
                        break;
                }
                
                if (reportData != null && reportData.Rows.Count > 0)
                {
                    currentReportData = reportData;
                    
                    // Set report path
                    reportViewer.LocalReport.ReportPath = reportPath;
                    
                    // Set data source
                    ReportDataSource rds = new ReportDataSource(dataSetName, reportData);
                    reportViewer.LocalReport.DataSources.Clear();
                    reportViewer.LocalReport.DataSources.Add(rds);
                    
                    // Set report parameters
                    SetReportParameters(reportType);
                    
                    // Refresh the report
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
                MessageBox.Show("Error generating report: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetReportParameters(string reportType)
        {
            try
            {
                List<ReportParameter> parameters = new List<ReportParameter>();
                
                // Common parameters
                parameters.Add(new ReportParameter("FromDate", dtpFromDate.Value.ToString("dd/MM/yyyy")));
                parameters.Add(new ReportParameter("ToDate", dtpToDate.Value.ToString("dd/MM/yyyy")));
                parameters.Add(new ReportParameter("GeneratedDate", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")));
                parameters.Add(new ReportParameter("CompanyName", "Retail Management System"));
                parameters.Add(new ReportParameter("ReportTitle", GetReportTitle(reportType)));
                
                // Report-specific parameters
                switch (reportType)
                {
                    case "SalesByCustomer":
                        ComboBoxItem selectedCustomer = (ComboBoxItem)cmbCustomer.SelectedItem;
                        parameters.Add(new ReportParameter("CustomerFilter", selectedCustomer?.Text ?? "All Customers"));
                        break;
                    case "PurchaseBySupplier":
                        ComboBoxItem selectedSupplier = (ComboBoxItem)cmbSupplier.SelectedItem;
                        parameters.Add(new ReportParameter("SupplierFilter", selectedSupplier?.Text ?? "All Suppliers"));
                        break;
                    case "PaymentMode":
                        ComboBoxItem selectedPayment = (ComboBoxItem)cmbPaymentMode.SelectedItem;
                        parameters.Add(new ReportParameter("PaymentFilter", selectedPayment?.Text ?? "All Modes"));
                        break;
                }
                
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
                case "DailySales": return "Daily Sales Summary Report";
                case "DailyPurchase": return "Daily Purchase Summary Report";
                case "SalesByCustomer": return "Customer-wise Sales Report";
                case "PurchaseBySupplier": return "Supplier-wise Purchase Report";
                case "PaymentMode": return "Payment Mode Analysis Report";
                case "TopSelling": return "Top Selling Items Report";
                case "CashFlow": return "Cash Flow Summary Report";
                case "GSTSummary": return "GST Summary Report";
                default: return "Daily Report";
            }
        }

        private DataTable GetDailySalesData()
        {
            string query = @"EXEC sp_GetDailySalesReport @FromDate, @ToDate";
            SqlParameter[] parameters = {
                new SqlParameter("@FromDate", dtpFromDate.Value.Date),
                new SqlParameter("@ToDate", dtpToDate.Value.Date.AddDays(1).AddTicks(-1))
            };
            return DatabaseConnection.ExecuteQuery(query, parameters);
        }

        private DataTable GetDailyPurchaseData()
        {
            string query = @"EXEC sp_GetDailyPurchaseReport @FromDate, @ToDate";
            SqlParameter[] parameters = {
                new SqlParameter("@FromDate", dtpFromDate.Value.Date),
                new SqlParameter("@ToDate", dtpToDate.Value.Date.AddDays(1).AddTicks(-1))
            };
            return DatabaseConnection.ExecuteQuery(query, parameters);
        }

        private DataTable GetSalesByCustomerData()
        {
            ComboBoxItem selectedCustomer = (ComboBoxItem)cmbCustomer.SelectedItem;
            int customerID = selectedCustomer != null ? (int)selectedCustomer.Value : 0;
            
            string query = @"EXEC sp_GetCustomerSalesReport @FromDate, @ToDate, @CustomerID";
            SqlParameter[] parameters = {
                new SqlParameter("@FromDate", dtpFromDate.Value.Date),
                new SqlParameter("@ToDate", dtpToDate.Value.Date.AddDays(1).AddTicks(-1)),
                new SqlParameter("@CustomerID", customerID == 0 ? (object)DBNull.Value : customerID)
            };
            return DatabaseConnection.ExecuteQuery(query, parameters);
        }

        private DataTable GetPurchaseBySupplierData()
        {
            ComboBoxItem selectedSupplier = (ComboBoxItem)cmbSupplier.SelectedItem;
            int supplierID = selectedSupplier != null ? (int)selectedSupplier.Value : 0;
            
            string query = @"EXEC sp_GetSupplierPurchaseReport @FromDate, @ToDate, @SupplierID";
            SqlParameter[] parameters = {
                new SqlParameter("@FromDate", dtpFromDate.Value.Date),
                new SqlParameter("@ToDate", dtpToDate.Value.Date.AddDays(1).AddTicks(-1)),
                new SqlParameter("@SupplierID", supplierID == 0 ? (object)DBNull.Value : supplierID)
            };
            return DatabaseConnection.ExecuteQuery(query, parameters);
        }

        private DataTable GetPaymentModeData()
        {
            ComboBoxItem selectedPayment = (ComboBoxItem)cmbPaymentMode.SelectedItem;
            string paymentMode = selectedPayment != null ? selectedPayment.Value.ToString() : "All";
            
            string query = @"EXEC sp_GetPaymentModeReport @FromDate, @ToDate, @PaymentMode";
            SqlParameter[] parameters = {
                new SqlParameter("@FromDate", dtpFromDate.Value.Date),
                new SqlParameter("@ToDate", dtpToDate.Value.Date.AddDays(1).AddTicks(-1)),
                new SqlParameter("@PaymentMode", paymentMode == "All" ? (object)DBNull.Value : paymentMode)
            };
            return DatabaseConnection.ExecuteQuery(query, parameters);
        }

        private DataTable GetTopSellingItemsData()
        {
            string query = @"EXEC sp_GetTopSellingItemsReport @FromDate, @ToDate, @TopCount = 20";
            SqlParameter[] parameters = {
                new SqlParameter("@FromDate", dtpFromDate.Value.Date),
                new SqlParameter("@ToDate", dtpToDate.Value.Date.AddDays(1).AddTicks(-1))
            };
            return DatabaseConnection.ExecuteQuery(query, parameters);
        }

        private DataTable GetCashFlowData()
        {
            string query = @"EXEC sp_GetCashFlowReport @FromDate, @ToDate";
            SqlParameter[] parameters = {
                new SqlParameter("@FromDate", dtpFromDate.Value.Date),
                new SqlParameter("@ToDate", dtpToDate.Value.Date.AddDays(1).AddTicks(-1))
            };
            return DatabaseConnection.ExecuteQuery(query, parameters);
        }

        private DataTable GetGSTSummaryData()
        {
            string query = @"EXEC sp_GetGSTSummaryReport @FromDate, @ToDate";
            SqlParameter[] parameters = {
                new SqlParameter("@FromDate", dtpFromDate.Value.Date),
                new SqlParameter("@ToDate", dtpToDate.Value.Date.AddDays(1).AddTicks(-1))
            };
            return DatabaseConnection.ExecuteQuery(query, parameters);
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
                saveDialog.Title = "Export Report";
                saveDialog.FileName = GetExportFileName();

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
                                        "<PageWidth>8.5in</PageWidth>" +
                                        "<PageHeight>11in</PageHeight>" +
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

        private string GetExportFileName()
        {
            ComboBoxItem selectedReport = (ComboBoxItem)cmbReportType.SelectedItem;
            string reportName = selectedReport?.Text ?? "Report";
            string dateRange = dtpFromDate.Value.ToString("yyyyMMdd") + "_" + dtpToDate.Value.ToString("yyyyMMdd");
            return $"{reportName}_{dateRange}";
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

                // Use the ReportViewer's built-in print functionality
                reportViewer.PrintDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error printing report: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    
}
