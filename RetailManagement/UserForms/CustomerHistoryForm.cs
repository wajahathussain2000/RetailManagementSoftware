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
    public partial class CustomerHistoryForm : Form
    {
        private ReportViewer reportViewer;
        private ComboBox cmbCustomer;
        private DateTimePicker dtpFromDate;
        private DateTimePicker dtpToDate;
        private ComboBox cmbViewType;
        private Button btnSearch;
        private Button btnExport;
        private Button btnPrint;
        private DataGridView dgvHistory;
        private GroupBox groupFilters;
        private GroupBox groupSummary;
        private Label lblTotalPurchases;
        private Label lblTotalAmount;
        private Label lblLastPurchase;
        private Label lblAverageAmount;
        private TabControl tabControl;
        private TabPage tabHistory;
        private TabPage tabSummary;
        private DataTable historyData;
        private int selectedCustomerID = 0;

        public CustomerHistoryForm()
        {
            InitializeComponent();
            InitializeReportViewer();
            LoadCustomers();
            SetDefaultDates();
        }

        public CustomerHistoryForm(int customerID) : this()
        {
            selectedCustomerID = customerID;
            if (cmbCustomer.Items.Count > 0)
            {
                foreach (ComboBoxItem item in cmbCustomer.Items)
                {
                    if ((int)item.Value == customerID)
                    {
                        cmbCustomer.SelectedItem = item;
                        break;
                    }
                }
            }
            LoadCustomerHistory();
        }

        private void InitializeComponent()
        {
            this.groupFilters = new System.Windows.Forms.GroupBox();
            this.cmbCustomer = new System.Windows.Forms.ComboBox();
            this.dtpFromDate = new System.Windows.Forms.DateTimePicker();
            this.dtpToDate = new System.Windows.Forms.DateTimePicker();
            this.cmbViewType = new System.Windows.Forms.ComboBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabHistory = new System.Windows.Forms.TabPage();
            this.tabSummary = new System.Windows.Forms.TabPage();
            this.dgvHistory = new System.Windows.Forms.DataGridView();
            this.groupSummary = new System.Windows.Forms.GroupBox();
            this.lblTotalPurchases = new System.Windows.Forms.Label();
            this.lblTotalAmount = new System.Windows.Forms.Label();
            this.lblLastPurchase = new System.Windows.Forms.Label();
            this.lblAverageAmount = new System.Windows.Forms.Label();
            
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistory)).BeginInit();
            this.groupFilters.SuspendLayout();
            this.groupSummary.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabHistory.SuspendLayout();
            this.tabSummary.SuspendLayout();
            this.SuspendLayout();
            
            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.groupFilters);
            this.Name = "CustomerHistoryForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Customer Purchase History - Retail Management System";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            
            // groupFilters
            this.groupFilters.Controls.Add(this.btnPrint);
            this.groupFilters.Controls.Add(this.btnExport);
            this.groupFilters.Controls.Add(this.btnSearch);
            this.groupFilters.Controls.Add(this.cmbViewType);
            this.groupFilters.Controls.Add(this.dtpToDate);
            this.groupFilters.Controls.Add(this.dtpFromDate);
            this.groupFilters.Controls.Add(this.cmbCustomer);
            this.groupFilters.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupFilters.Location = new System.Drawing.Point(0, 0);
            this.groupFilters.Name = "groupFilters";
            this.groupFilters.Size = new System.Drawing.Size(1200, 80);
            this.groupFilters.TabIndex = 0;
            this.groupFilters.TabStop = false;
            this.groupFilters.Text = "Search Filters";
            this.groupFilters.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            
            // Customer ComboBox
            this.cmbCustomer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCustomer.FormattingEnabled = true;
            this.cmbCustomer.Location = new System.Drawing.Point(15, 25);
            this.cmbCustomer.Name = "cmbCustomer";
            this.cmbCustomer.Size = new System.Drawing.Size(200, 21);
            this.cmbCustomer.TabIndex = 0;
            this.cmbCustomer.SelectedIndexChanged += new System.EventHandler(this.cmbCustomer_SelectedIndexChanged);
            
            // From Date
            this.dtpFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFromDate.Location = new System.Drawing.Point(230, 25);
            this.dtpFromDate.Name = "dtpFromDate";
            this.dtpFromDate.Size = new System.Drawing.Size(100, 20);
            this.dtpFromDate.TabIndex = 1;
            
            // To Date
            this.dtpToDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpToDate.Location = new System.Drawing.Point(340, 25);
            this.dtpToDate.Name = "dtpToDate";
            this.dtpToDate.Size = new System.Drawing.Size(100, 20);
            this.dtpToDate.TabIndex = 2;
            
            // View Type
            this.cmbViewType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbViewType.FormattingEnabled = true;
            this.cmbViewType.Location = new System.Drawing.Point(455, 25);
            this.cmbViewType.Name = "cmbViewType";
            this.cmbViewType.Size = new System.Drawing.Size(120, 21);
            this.cmbViewType.TabIndex = 3;
            
            // Search Button
            this.btnSearch.BackColor = System.Drawing.Color.Green;
            this.btnSearch.ForeColor = System.Drawing.Color.White;
            this.btnSearch.Location = new System.Drawing.Point(590, 23);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(80, 25);
            this.btnSearch.TabIndex = 4;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            
            // Export Button
            this.btnExport.BackColor = System.Drawing.Color.Blue;
            this.btnExport.ForeColor = System.Drawing.Color.White;
            this.btnExport.Location = new System.Drawing.Point(680, 23);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(80, 25);
            this.btnExport.TabIndex = 5;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = false;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            
            // Print Button
            this.btnPrint.BackColor = System.Drawing.Color.DarkBlue;
            this.btnPrint.ForeColor = System.Drawing.Color.White;
            this.btnPrint.Location = new System.Drawing.Point(770, 23);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(80, 25);
            this.btnPrint.TabIndex = 6;
            this.btnPrint.Text = "Print";
            this.btnPrint.UseVisualStyleBackColor = false;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            
            // Tab Control
            this.tabControl.Controls.Add(this.tabHistory);
            this.tabControl.Controls.Add(this.tabSummary);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 80);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(1200, 620);
            this.tabControl.TabIndex = 1;
            
            // Tab History
            this.tabHistory.Controls.Add(this.dgvHistory);
            this.tabHistory.Location = new System.Drawing.Point(4, 22);
            this.tabHistory.Name = "tabHistory";
            this.tabHistory.Padding = new System.Windows.Forms.Padding(3);
            this.tabHistory.Size = new System.Drawing.Size(1192, 594);
            this.tabHistory.TabIndex = 0;
            this.tabHistory.Text = "Purchase History";
            this.tabHistory.UseVisualStyleBackColor = true;
            
            // Tab Summary
            this.tabSummary.Controls.Add(this.groupSummary);
            this.tabSummary.Location = new System.Drawing.Point(4, 22);
            this.tabSummary.Name = "tabSummary";
            this.tabSummary.Padding = new System.Windows.Forms.Padding(3);
            this.tabSummary.Size = new System.Drawing.Size(1192, 594);
            this.tabSummary.TabIndex = 1;
            this.tabSummary.Text = "Summary Report";
            this.tabSummary.UseVisualStyleBackColor = true;
            
            // DataGridView
            this.dgvHistory.AllowUserToAddRows = false;
            this.dgvHistory.AllowUserToDeleteRows = false;
            this.dgvHistory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvHistory.Location = new System.Drawing.Point(3, 3);
            this.dgvHistory.Name = "dgvHistory";
            this.dgvHistory.ReadOnly = true;
            this.dgvHistory.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvHistory.Size = new System.Drawing.Size(1186, 588);
            this.dgvHistory.TabIndex = 0;
            this.dgvHistory.DoubleClick += new System.EventHandler(this.dgvHistory_DoubleClick);
            
            // Group Summary
            this.groupSummary.Controls.Add(this.lblAverageAmount);
            this.groupSummary.Controls.Add(this.lblLastPurchase);
            this.groupSummary.Controls.Add(this.lblTotalAmount);
            this.groupSummary.Controls.Add(this.lblTotalPurchases);
            this.groupSummary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupSummary.Location = new System.Drawing.Point(3, 3);
            this.groupSummary.Name = "groupSummary";
            this.groupSummary.Size = new System.Drawing.Size(1186, 588);
            this.groupSummary.TabIndex = 0;
            this.groupSummary.TabStop = false;
            this.groupSummary.Text = "Customer Summary";
            
            // Summary Labels
            this.lblTotalPurchases.AutoSize = true;
            this.lblTotalPurchases.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lblTotalPurchases.Location = new System.Drawing.Point(20, 30);
            this.lblTotalPurchases.Name = "lblTotalPurchases";
            this.lblTotalPurchases.Size = new System.Drawing.Size(150, 20);
            this.lblTotalPurchases.TabIndex = 0;
            this.lblTotalPurchases.Text = "Total Purchases: 0";
            
            this.lblTotalAmount.AutoSize = true;
            this.lblTotalAmount.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lblTotalAmount.ForeColor = System.Drawing.Color.Green;
            this.lblTotalAmount.Location = new System.Drawing.Point(20, 60);
            this.lblTotalAmount.Name = "lblTotalAmount";
            this.lblTotalAmount.Size = new System.Drawing.Size(150, 20);
            this.lblTotalAmount.TabIndex = 1;
            this.lblTotalAmount.Text = "Total Amount: ₹0.00";
            
            this.lblLastPurchase.AutoSize = true;
            this.lblLastPurchase.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.lblLastPurchase.Location = new System.Drawing.Point(20, 90);
            this.lblLastPurchase.Name = "lblLastPurchase";
            this.lblLastPurchase.Size = new System.Drawing.Size(120, 17);
            this.lblLastPurchase.TabIndex = 2;
            this.lblLastPurchase.Text = "Last Purchase: N/A";
            
            this.lblAverageAmount.AutoSize = true;
            this.lblAverageAmount.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.lblAverageAmount.Location = new System.Drawing.Point(20, 120);
            this.lblAverageAmount.Name = "lblAverageAmount";
            this.lblAverageAmount.Size = new System.Drawing.Size(150, 17);
            this.lblAverageAmount.TabIndex = 3;
            this.lblAverageAmount.Text = "Average Amount: ₹0.00";
            
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistory)).EndInit();
            this.groupFilters.ResumeLayout(false);
            this.groupSummary.ResumeLayout(false);
            this.groupSummary.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.tabHistory.ResumeLayout(false);
            this.tabSummary.ResumeLayout(false);
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
            
            // Add report viewer to summary tab
            tabSummary.Controls.Add(reportViewer);
            reportViewer.BringToFront();
        }

        private void LoadCustomers()
        {
            try
            {
                string query = @"SELECT CustomerID, CustomerName, MobileNumber, TotalPurchases, LastPurchaseDate
                               FROM Customers WHERE IsActive = 1 ORDER BY CustomerName";
                DataTable customers = DatabaseConnection.ExecuteQuery(query);
                
                cmbCustomer.Items.Clear();
                cmbCustomer.Items.Add(new ComboBoxItem { Text = "Select Customer", Value = 0 });
                
                foreach (DataRow row in customers.Rows)
                {
                    string displayText = $"{row["CustomerName"]} - {row["MobileNumber"]}";
                    cmbCustomer.Items.Add(new ComboBoxItem 
                    { 
                        Text = displayText, 
                        Value = Convert.ToInt32(row["CustomerID"]) 
                    });
                }
                
                cmbCustomer.DisplayMember = "Text";
                cmbCustomer.ValueMember = "Value";
                cmbCustomer.SelectedIndex = 0;
                
                // Load view types
                cmbViewType.Items.Clear();
                cmbViewType.Items.Add(new ComboBoxItem { Text = "All Purchases", Value = "ALL" });
                cmbViewType.Items.Add(new ComboBoxItem { Text = "Cash Sales", Value = "CASH" });
                cmbViewType.Items.Add(new ComboBoxItem { Text = "Credit Sales", Value = "CREDIT" });
                cmbViewType.Items.Add(new ComboBoxItem { Text = "Returns", Value = "RETURN" });
                cmbViewType.DisplayMember = "Text";
                cmbViewType.ValueMember = "Value";
                cmbViewType.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading customers: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetDefaultDates()
        {
            dtpFromDate.Value = DateTime.Today.AddMonths(-3); // Last 3 months
            dtpToDate.Value = DateTime.Today;
        }

        private void cmbCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxItem selectedCustomer = (ComboBoxItem)cmbCustomer.SelectedItem;
            if (selectedCustomer != null && (int)selectedCustomer.Value > 0)
            {
                selectedCustomerID = (int)selectedCustomer.Value;
                LoadCustomerHistory();
            }
            else
            {
                ClearHistory();
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadCustomerHistory();
        }

        private void LoadCustomerHistory()
        {
            if (selectedCustomerID == 0)
            {
                MessageBox.Show("Please select a customer first.", "Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                ComboBoxItem selectedView = (ComboBoxItem)cmbViewType.SelectedItem;
                string viewType = selectedView?.Value.ToString() ?? "ALL";
                
                string query = @"EXEC sp_GetCustomerPurchaseHistory @CustomerID, @FromDate, @ToDate, @ViewType";
                SqlParameter[] parameters = {
                    new SqlParameter("@CustomerID", selectedCustomerID),
                    new SqlParameter("@FromDate", dtpFromDate.Value.Date),
                    new SqlParameter("@ToDate", dtpToDate.Value.Date.AddDays(1).AddTicks(-1)),
                    new SqlParameter("@ViewType", viewType)
                };
                
                historyData = DatabaseConnection.ExecuteQuery(query, parameters);
                
                if (historyData.Rows.Count > 0)
                {
                    dgvHistory.DataSource = historyData;
                    FormatDataGridView();
                    UpdateSummary();
                    GenerateReport();
                }
                else
                {
                    ClearHistory();
                    MessageBox.Show("No purchase history found for the selected criteria.", "Information",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading customer history: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormatDataGridView()
        {
            if (dgvHistory.Columns.Count > 0)
            {
                // Hide ID columns
                if (dgvHistory.Columns.Contains("SaleID"))
                    dgvHistory.Columns["SaleID"].Visible = false;
                if (dgvHistory.Columns.Contains("CustomerID"))
                    dgvHistory.Columns["CustomerID"].Visible = false;

                // Set column headers and formatting
                var columnMappings = new Dictionary<string, string>
                {
                    { "SaleDate", "Date" },
                    { "BillNumber", "Bill No" },
                    { "TotalItems", "Items" },
                    { "GrossAmount", "Gross Amount" },
                    { "TotalDiscount", "Discount" },
                    { "TotalTax", "Tax" },
                    { "NetAmount", "Net Amount" },
                    { "PaymentMode", "Payment" },
                    { "AmountPaid", "Paid" },
                    { "BalanceAmount", "Balance" },
                    { "Status", "Status" }
                };

                foreach (var mapping in columnMappings)
                {
                    if (dgvHistory.Columns.Contains(mapping.Key))
                    {
                        dgvHistory.Columns[mapping.Key].HeaderText = mapping.Value;
                        
                        // Format currency columns
                        if (mapping.Key.Contains("Amount") || mapping.Key.Contains("Paid") || mapping.Key.Contains("Balance") || mapping.Key.Contains("Discount") || mapping.Key.Contains("Tax"))
                        {
                            dgvHistory.Columns[mapping.Key].DefaultCellStyle.Format = "N2";
                            dgvHistory.Columns[mapping.Key].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        }
                        
                        // Format date columns
                        if (mapping.Key.Contains("Date"))
                        {
                            dgvHistory.Columns[mapping.Key].DefaultCellStyle.Format = "dd/MM/yyyy";
                        }
                    }
                }

                // Color coding for status
                foreach (DataGridViewRow row in dgvHistory.Rows)
                {
                    if (row.Cells["Status"].Value != null)
                    {
                        string status = row.Cells["Status"].Value.ToString();
                        switch (status.ToUpper())
                        {
                            case "PAID":
                                row.DefaultCellStyle.ForeColor = Color.Green;
                                break;
                            case "PENDING":
                            case "PARTIAL":
                                row.DefaultCellStyle.ForeColor = Color.Orange;
                                break;
                            case "OVERDUE":
                                row.DefaultCellStyle.ForeColor = Color.Red;
                                break;
                            case "RETURNED":
                                row.DefaultCellStyle.ForeColor = Color.Blue;
                                break;
                        }
                    }
                }

                // Auto-resize columns
                dgvHistory.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
            }
        }

        private void UpdateSummary()
        {
            if (historyData != null && historyData.Rows.Count > 0)
            {
                int totalPurchases = historyData.Rows.Count;
                decimal totalAmount = historyData.AsEnumerable().Sum(row => row.Field<decimal>("NetAmount"));
                decimal averageAmount = totalAmount / totalPurchases;
                
                DateTime? lastPurchaseDate = historyData.AsEnumerable()
                    .Max(row => row.Field<DateTime?>("SaleDate"));

                lblTotalPurchases.Text = $"Total Purchases: {totalPurchases}";
                lblTotalAmount.Text = $"Total Amount: ₹{totalAmount:N2}";
                lblAverageAmount.Text = $"Average Amount: ₹{averageAmount:N2}";
                lblLastPurchase.Text = lastPurchaseDate.HasValue ? 
                    $"Last Purchase: {lastPurchaseDate.Value:dd/MM/yyyy}" : 
                    "Last Purchase: N/A";
            }
            else
            {
                lblTotalPurchases.Text = "Total Purchases: 0";
                lblTotalAmount.Text = "Total Amount: ₹0.00";
                lblAverageAmount.Text = "Average Amount: ₹0.00";
                lblLastPurchase.Text = "Last Purchase: N/A";
            }
        }

        private void GenerateReport()
        {
            try
            {
                if (historyData == null || historyData.Rows.Count == 0)
                    return;

                // Clear previous report
                reportViewer.Reset();
                
                // Smart RDLC path detection
                string simplePath = System.IO.Path.Combine(Application.StartupPath, "Reports", "CustomerPurchaseHistoryReport_Simple.rdlc");
                string originalPath = System.IO.Path.Combine(Application.StartupPath, "Reports", "CustomerPurchaseHistoryReport.rdlc");
                
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
                    // Fallback paths
                    if (System.IO.File.Exists("Reports/CustomerPurchaseHistoryReport_Simple.rdlc"))
                        reportViewer.LocalReport.ReportPath = "Reports/CustomerPurchaseHistoryReport_Simple.rdlc";
                    else
                        reportViewer.LocalReport.ReportPath = "Reports/CustomerPurchaseHistoryReport.rdlc";
                }
                
                // Set data source
                ReportDataSource rds = new ReportDataSource("CustomerHistoryDataSet", historyData);
                reportViewer.LocalReport.DataSources.Clear();
                reportViewer.LocalReport.DataSources.Add(rds);
                
                // Set report parameters
                List<ReportParameter> parameters = new List<ReportParameter>();
                ComboBoxItem selectedCustomer = (ComboBoxItem)cmbCustomer.SelectedItem;
                
                parameters.Add(new ReportParameter("CustomerName", selectedCustomer?.Text ?? "Unknown Customer"));
                parameters.Add(new ReportParameter("FromDate", dtpFromDate.Value.ToString("dd/MM/yyyy")));
                parameters.Add(new ReportParameter("ToDate", dtpToDate.Value.ToString("dd/MM/yyyy")));
                parameters.Add(new ReportParameter("GeneratedDate", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")));
                parameters.Add(new ReportParameter("CompanyName", "Retail Management System"));
                parameters.Add(new ReportParameter("ReportTitle", "Customer Purchase History Report"));
                
                reportViewer.LocalReport.SetParameters(parameters);
                
                // Refresh the report
                reportViewer.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating report: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearHistory()
        {
            dgvHistory.DataSource = null;
            historyData = null;
            UpdateSummary();
            reportViewer.Reset();
        }

        private void dgvHistory_DoubleClick(object sender, EventArgs e)
        {
            if (dgvHistory.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dgvHistory.SelectedRows[0];
                if (selectedRow.Cells["SaleID"].Value != null)
                {
                    int saleID = Convert.ToInt32(selectedRow.Cells["SaleID"].Value);
                    string billNumber = selectedRow.Cells["BillNumber"].Value.ToString();
                    
                    // Open bill details form
                    MessageBox.Show($"Opening bill details for {billNumber}\nSale ID: {saleID}", "Bill Details",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Here you would open the actual bill details form
                    // BillDetailsForm billForm = new BillDetailsForm(saleID);
                    // billForm.ShowDialog();
                }
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                if (historyData == null || historyData.Rows.Count == 0)
                {
                    MessageBox.Show("No data to export. Please search for customer history first.", "Information",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "PDF Files|*.pdf|Excel Files|*.xlsx|CSV Files|*.csv";
                saveDialog.Title = "Export Customer History";
                
                ComboBoxItem selectedCustomer = (ComboBoxItem)cmbCustomer.SelectedItem;
                string customerName = selectedCustomer?.Text?.Split('-')[0].Trim() ?? "Customer";
                saveDialog.FileName = $"CustomerHistory_{customerName}_{DateTime.Now:yyyyMMdd}";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    string extension = Path.GetExtension(saveDialog.FileName).ToLower();
                    
                    if (extension == ".csv")
                    {
                        ExportToCSV(saveDialog.FileName);
                    }
                    else
                    {
                        // Export using ReportViewer
                        Warning[] warnings;
                        string[] streamids;
                        string mimeType;
                        string encoding;
                        string deviceInfo = "";
                        
                        byte[] bytes = null;
                        
                        switch (extension)
                        {
                            case ".pdf":
                                bytes = reportViewer.LocalReport.Render("PDF", deviceInfo, out mimeType, out encoding, out extension, out streamids, out warnings);
                                break;
                            case ".xlsx":
                                bytes = reportViewer.LocalReport.Render("EXCELOPENXML", deviceInfo, out mimeType, out encoding, out extension, out streamids, out warnings);
                                break;
                        }
                        
                        if (bytes != null)
                        {
                            using (FileStream fs = new FileStream(saveDialog.FileName, FileMode.Create))
                            {
                                fs.Write(bytes, 0, bytes.Length);
                            }
                        }
                    }
                    
                    MessageBox.Show("Customer history exported successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Ask if user wants to open the file
                    if (MessageBox.Show("Do you want to open the exported file?", "Open File",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(saveDialog.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exporting customer history: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportToCSV(string fileName)
        {
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(fileName))
            {
                // Write headers
                string[] headers = new string[dgvHistory.Columns.Count];
                for (int i = 0; i < dgvHistory.Columns.Count; i++)
                {
                    if (dgvHistory.Columns[i].Visible)
                        headers[i] = dgvHistory.Columns[i].HeaderText;
                }
                writer.WriteLine(string.Join(",", headers.Where(h => !string.IsNullOrEmpty(h))));

                // Write data
                foreach (DataGridViewRow row in dgvHistory.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        string[] values = new string[dgvHistory.Columns.Count];
                        for (int i = 0; i < dgvHistory.Columns.Count; i++)
                        {
                            if (dgvHistory.Columns[i].Visible)
                                values[i] = row.Cells[i].Value?.ToString()?.Replace(",", ";") ?? "";
                        }
                        writer.WriteLine(string.Join(",", values.Where((v, index) => dgvHistory.Columns[index].Visible)));
                    }
                }
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (historyData == null || historyData.Rows.Count == 0)
                {
                    MessageBox.Show("No data to print. Please search for customer history first.", "Information",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Switch to summary tab if not already there
                tabControl.SelectedTab = tabSummary;
                
                // Use the ReportViewer's built-in print functionality
                reportViewer.PrintDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error printing customer history: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    
}
