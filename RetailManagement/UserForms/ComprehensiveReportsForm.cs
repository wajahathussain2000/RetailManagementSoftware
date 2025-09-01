using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using RetailManagement.Database;

namespace RetailManagement.UserForms
{
    public partial class ComprehensiveReportsForm : Form
    {
        private ReportViewer reportViewer;
        private Panel controlsPanel;
        private ComboBox cmbReportType;
        private DateTimePicker dtpFromDate;
        private DateTimePicker dtpToDate;
        private ComboBox cmbCustomer;
        private ComboBox cmbSupplier;
        private ComboBox cmbCategory;
        private Button btnGenerateReport;
        private Button btnExportPDF;
        private Button btnExportExcel;
        private Label lblRecordCount;
        private DataTable currentReportData;

        public ComprehensiveReportsForm()
        {
            InitializeComponent();
            InitializeControls();
            LoadDropdownData();
            SetDefaultValues();
        }

        private void InitializeComponent()
        {
            this.Text = "📊 Comprehensive Reports Center";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.FromArgb(240, 248, 255);
        }

        private void InitializeControls()
        {
            // Main layout
            TableLayoutPanel mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = Color.Transparent
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 120));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            // Controls Panel
            controlsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(230, 240, 250),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Report Type Selection
            var lblReportType = new Label
            {
                Text = "📋 Report Type:",
                Font = new Font("Arial", 10, FontStyle.Bold),
                Location = new Point(20, 20),
                Size = new Size(100, 25),
                ForeColor = Color.DarkBlue
            };

            cmbReportType = new ComboBox
            {
                Location = new Point(130, 18),
                Size = new Size(250, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Arial", 9)
            };
            cmbReportType.Items.AddRange(new string[]
            {
                // Sales & Customer Reports
                "📈 Daily Sales Report",
                "📊 Monthly Sales Summary", 
                "👥 Customer-wise Sales",
                "🔄 Sales Return Report",
                "📅 Custom Date Range Report",
                "💳 Payment Method Analysis",
                
                // Purchase & Supplier Reports
                "🏭 Supplier Details Report",
                "📦 Supplier Purchase History",
                "💰 Supplier Payables & Payments",
                "📋 Supplier Contact & Balance Ledger",
                "🏪 Supplier-wise Purchases",
                "🔄 Purchase Return Report",
                
                // Stock & Inventory Reports
                "📋 Current Stock Report",
                "📊 Stock Valuation Report",
                "⚠️ Low Stock Alert Report",
                "🚨 Expired Medicines Report",
                "📈 Stock Movement Report",
                
                // Financial Reports
                "💰 Enhanced Profit & Loss Report",
                "💸 Enhanced Expense Report",
                "📊 Daily Sales & Purchase Combined",
                "🏦 Bank Transactions Report",
                "📑 Trial Balance Report",
                "💹 Cash Flow Statement",
                
                // GST & Tax Reports
                "🧾 GST Sales Report",
                "📋 GST Purchase Report",
                "📊 GSTR-1 Report",
                "📈 GSTR-2 Report",
                "🏷️ HSN/SAC Code Report",
                "📄 GST Return Summary",
                
                // Advanced Analytics
                "📊 ABC Analysis Report",
                "👥 Customer Outstanding Report",
                "🏭 Supplier Outstanding Report",
                "📅 Batch Expiry Analysis",
                "💹 Profit Margin Analysis"
            });

            // Date Range
            var lblFromDate = new Label
            {
                Text = "From Date:",
                Location = new Point(400, 20),
                Size = new Size(80, 25),
                Font = new Font("Arial", 9),
                ForeColor = Color.DarkBlue
            };

            dtpFromDate = new DateTimePicker
            {
                Location = new Point(485, 18),
                Size = new Size(120, 25),
                Format = DateTimePickerFormat.Short
            };

            var lblToDate = new Label
            {
                Text = "To Date:",
                Location = new Point(620, 20),
                Size = new Size(60, 25),
                Font = new Font("Arial", 9),
                ForeColor = Color.DarkBlue
            };

            dtpToDate = new DateTimePicker
            {
                Location = new Point(685, 18),
                Size = new Size(120, 25),
                Format = DateTimePickerFormat.Short
            };

            // Customer/Supplier filters
            var lblCustomer = new Label
            {
                Text = "Customer:",
                Location = new Point(20, 55),
                Size = new Size(70, 25),
                Font = new Font("Arial", 9),
                ForeColor = Color.DarkBlue
            };

            cmbCustomer = new ComboBox
            {
                Location = new Point(95, 53),
                Size = new Size(180, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Arial", 9)
            };

            var lblSupplier = new Label
            {
                Text = "Supplier:",
                Location = new Point(290, 55),
                Size = new Size(70, 25),
                Font = new Font("Arial", 9),
                ForeColor = Color.DarkBlue
            };

            cmbSupplier = new ComboBox
            {
                Location = new Point(365, 53),
                Size = new Size(180, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Arial", 9)
            };

            var lblCategory = new Label
            {
                Text = "Category:",
                Location = new Point(560, 55),
                Size = new Size(70, 25),
                Font = new Font("Arial", 9),
                ForeColor = Color.DarkBlue
            };

            cmbCategory = new ComboBox
            {
                Location = new Point(635, 53),
                Size = new Size(150, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Arial", 9)
            };

            // Action Buttons
            btnGenerateReport = new Button
            {
                Text = "🔍 Generate Report",
                Location = new Point(20, 85),
                Size = new Size(140, 30),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                Font = new Font("Arial", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnGenerateReport.Click += BtnGenerateReport_Click;

            btnExportPDF = new Button
            {
                Text = "📄 Export PDF",
                Location = new Point(170, 85),
                Size = new Size(120, 30),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                Font = new Font("Arial", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnExportPDF.Click += BtnExportPDF_Click;

            btnExportExcel = new Button
            {
                Text = "📊 Export Excel",
                Location = new Point(300, 85),
                Size = new Size(120, 30),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                Font = new Font("Arial", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnExportExcel.Click += BtnExportExcel_Click;

            lblRecordCount = new Label
            {
                Text = "Ready to generate reports...",
                Location = new Point(450, 92),
                Size = new Size(300, 20),
                Font = new Font("Arial", 9, FontStyle.Italic),
                ForeColor = Color.Gray
            };

            // Add controls to panel
            controlsPanel.Controls.AddRange(new Control[]
            {
                lblReportType, cmbReportType,
                lblFromDate, dtpFromDate, lblToDate, dtpToDate,
                lblCustomer, cmbCustomer, lblSupplier, cmbSupplier, lblCategory, cmbCategory,
                btnGenerateReport, btnExportPDF, btnExportExcel, lblRecordCount
            });

            // Report Viewer
            try
            {
                reportViewer = new ReportViewer
                {
                    Dock = DockStyle.Fill,
                    ProcessingMode = ProcessingMode.Local,
                    BackColor = Color.White
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ReportViewer initialization failed: {ex.Message}", 
                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                reportViewer = null;
            }

            // Add to main layout
            mainLayout.Controls.Add(controlsPanel, 0, 0);
            if (reportViewer != null)
                mainLayout.Controls.Add(reportViewer, 0, 1);
            else
            {
                var placeholder = new Label
                {
                    Text = "Reports will be displayed in table format\n(RDLC viewer not available)",
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill,
                    Font = new Font("Arial", 12),
                    ForeColor = Color.Gray
                };
                mainLayout.Controls.Add(placeholder, 0, 1);
            }

            this.Controls.Add(mainLayout);
        }

        private void LoadDropdownData()
        {
            try
            {
                // Load Customers
                string customerQuery = "SELECT CustomerID, CustomerName FROM Customers WHERE IsActive = 1 ORDER BY CustomerName";
                DataTable customers = DatabaseConnection.ExecuteQuery(customerQuery);
                
                cmbCustomer.Items.Add("All Customers");
                foreach (DataRow row in customers.Rows)
                {
                    cmbCustomer.Items.Add($"{row["CustomerName"]} (ID: {row["CustomerID"]})");
                }

                // Load Suppliers
                string supplierQuery = "SELECT CompanyID, CompanyName FROM Companies WHERE IsActive = 1 ORDER BY CompanyName";
                DataTable suppliers = DatabaseConnection.ExecuteQuery(supplierQuery);
                
                cmbSupplier.Items.Add("All Suppliers");
                foreach (DataRow row in suppliers.Rows)
                {
                    cmbSupplier.Items.Add($"{row["CompanyName"]} (ID: {row["CompanyID"]})");
                }

                // Load Categories
                string categoryQuery = "SELECT DISTINCT Category FROM Items WHERE Category IS NOT NULL ORDER BY Category";
                DataTable categories = DatabaseConnection.ExecuteQuery(categoryQuery);
                
                cmbCategory.Items.Add("All Categories");
                foreach (DataRow row in categories.Rows)
                {
                    cmbCategory.Items.Add(row["Category"].ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading dropdown data: {ex.Message}", 
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void SetDefaultValues()
        {
            cmbReportType.SelectedIndex = 0;
            cmbCustomer.SelectedIndex = 0;
            cmbSupplier.SelectedIndex = 0;
            cmbCategory.SelectedIndex = 0;
            
            // Set date range to current month
            dtpFromDate.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dtpToDate.Value = DateTime.Now;
        }

        private void BtnGenerateReport_Click(object sender, EventArgs e)
        {
            try
            {
                string selectedReport = cmbReportType.SelectedItem?.ToString() ?? "";
                
                if (string.IsNullOrEmpty(selectedReport))
                {
                    MessageBox.Show("Please select a report type.", "Report Selection", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                
                switch (selectedReport)
                {
                    // Original Sales & Customer Reports
                    case "📈 Daily Sales Report":
                        GenerateDailySalesReport();
                        break;
                    case "📊 Monthly Sales Summary":
                        GenerateMonthlySalesReport();
                        break;
                    case "👥 Customer-wise Sales":
                        GenerateCustomerWiseSalesReport();
                        break;
                    case "🔄 Sales Return Report":
                        GenerateSalesReturnReport();
                        break;
                    case "📅 Custom Date Range Report":
                        GenerateDailySalesReport(); // Use daily sales with custom date range
                        break;
                    case "💳 Payment Method Analysis":
                        GeneratePaymentMethodAnalysisReport();
                        break;
                        
                    // Purchase & Supplier Reports
                    case "🏭 Supplier Details Report":
                        GenerateEnhancedSupplierDetailsReport();
                        break;
                    case "📦 Supplier Purchase History":
                        GenerateSupplierPurchaseHistoryReport();
                        break;
                    case "💰 Supplier Payables & Payments":
                        GenerateSupplierPayablesReport();
                        break;
                    case "📋 Supplier Contact & Balance Ledger":
                        GenerateSupplierLedgerReport();
                        break;
                    case "🏪 Supplier-wise Purchases":
                        GenerateSupplierWisePurchasesReport();
                        break;
                    case "🔄 Purchase Return Report":
                        GeneratePurchaseReturnReport();
                        break;
                        
                    // Stock & Inventory Reports
                    case "📋 Current Stock Report":
                        GenerateEnhancedCurrentStockReport();
                        break;
                    case "📊 Stock Valuation Report":
                        GenerateStockValuationReport();
                        break;
                    case "⚠️ Low Stock Alert Report":
                        GenerateLowStockAlertReport();
                        break;
                    case "🚨 Expired Medicines Report":
                        GenerateEnhancedExpiredMedicinesReport();
                        break;
                    case "📈 Stock Movement Report":
                        GenerateStockMovementReport();
                        break;
                        
                    // Financial Reports
                    case "💰 Enhanced Profit & Loss Report":
                        GenerateEnhancedProfitLossReport();
                        break;
                    case "💸 Enhanced Expense Report":
                        GenerateEnhancedExpenseReport();
                        break;
                    case "📊 Daily Sales & Purchase Combined":
                        GenerateDailySalesPurchaseReport();
                        break;
                    case "🏦 Bank Transactions Report":
                        MessageBox.Show("Bank Transactions Report framework is ready. Needs bank transaction data structure.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    case "📑 Trial Balance Report":
                        GenerateTrialBalanceReport();
                        break;
                    case "💹 Cash Flow Statement":
                        MessageBox.Show("Cash Flow Statement framework is ready. Needs cash management setup.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                        
                    // GST & Tax Reports
                    case "🧾 GST Sales Report":
                        GenerateGSTSalesReport();
                        break;
                    case "📋 GST Purchase Report":
                        GenerateGSTPurchaseReport();
                        break;
                    case "📊 GSTR-1 Report":
                        GenerateGSTR1Report();
                        break;
                    case "📈 GSTR-2 Report":
                        MessageBox.Show("GSTR-2 Report framework is ready. Based on purchase data structure.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    case "🏷️ HSN/SAC Code Report":
                        GenerateHSNSACReport();
                        break;
                    case "📄 GST Return Summary":
                        GenerateGSTR1Report(); // Use GSTR-1 as base for summary
                        break;
                        
                    // Advanced Analytics
                    case "📊 ABC Analysis Report":
                        GenerateABCAnalysisReport();
                        break;
                    case "👥 Customer Outstanding Report":
                        GenerateCustomerOutstandingReport();
                        break;
                    case "🏭 Supplier Outstanding Report":
                        GenerateSupplierOutstandingReport();
                        break;
                    case "📅 Batch Expiry Analysis":
                        GenerateEnhancedExpiredMedicinesReport(); // Use enhanced expiry report
                        break;
                    case "💹 Profit Margin Analysis":
                        GenerateEnhancedProfitLossReport(); // Use enhanced P&L
                        break;
                        
                    // Legacy Reports (maintaining backward compatibility)
                    case "📋 Stock/Inventory Report":
                        GenerateStockReport();
                        break;
                    case "⚠️ Expired Medicines Alert":
                        GenerateExpiredMedicinesReport();
                        break;
                    case "💰 Profit & Loss Analysis":
                        GenerateProfitReport();
                        break;
                    case "🏭 Supplier-wise Purchases":
                        GenerateSupplierWisePurchasesReport();
                        break;
                    case "💸 Expense Report":
                        GenerateExpenseReport();
                        break;
                        
                    default:
                        MessageBox.Show($"Report type '{selectedReport}' is not yet implemented or not recognized. Please select a different report.", "Report Not Available", 
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating report: {ex.Message}\n\nReport Type: {cmbReportType.SelectedItem?.ToString() ?? "None Selected"}", 
                    "Report Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerateDailySalesReport()
        {
            string query = @"
                SELECT 
                    s.SaleDate,
                    s.BillNumber,
                    c.CustomerName,
                    i.ItemName,
                    si.Quantity,
                    si.Price,
                    si.TotalAmount,
                    s.PaymentMethod,
                    s.NetAmount
                FROM Sales s
                INNER JOIN SaleItems si ON s.SaleID = si.SaleID
                INNER JOIN Items i ON si.ItemID = i.ItemID
                LEFT JOIN Customers c ON s.CustomerID = c.CustomerID
                WHERE s.SaleDate BETWEEN @FromDate AND @ToDate
                ORDER BY s.SaleDate DESC, s.SaleID, si.SaleItemID";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@FromDate", dtpFromDate.Value.Date),
                new SqlParameter("@ToDate", dtpToDate.Value.Date.AddDays(1).AddSeconds(-1))
            };

            currentReportData = DatabaseConnection.ExecuteQuery(query, parameters);
            LoadRDLCReport("DailySalesReport_Simple.rdlc", "DataSet1");
            lblRecordCount.Text = $"📊 Total Records: {currentReportData.Rows.Count}";
        }

        private void GenerateMonthlySalesReport()
        {
            string query = @"
                SELECT 
                    DATENAME(MONTH, s.SaleDate) as Month,
                    YEAR(s.SaleDate) as Year,
                    COUNT(DISTINCT s.SaleID) as TotalSales,
                    SUM(s.TotalAmount) as TotalAmount,
                    SUM(s.Discount) as TotalDiscount,
                    SUM(s.NetAmount) as NetAmount,
                    SUM(CASE WHEN s.PaymentMethod = 'Cash' THEN s.NetAmount ELSE 0 END) as CashSales,
                    SUM(CASE WHEN s.PaymentMethod = 'Card' THEN s.NetAmount ELSE 0 END) as CardSales,
                    SUM(CASE WHEN s.PaymentMethod = 'UPI' THEN s.NetAmount ELSE 0 END) as UPISales
                FROM Sales s
                WHERE s.SaleDate BETWEEN @FromDate AND @ToDate
                GROUP BY YEAR(s.SaleDate), MONTH(s.SaleDate), DATENAME(MONTH, s.SaleDate)
                ORDER BY YEAR(s.SaleDate), MONTH(s.SaleDate)";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@FromDate", dtpFromDate.Value.Date),
                new SqlParameter("@ToDate", dtpToDate.Value.Date.AddDays(1).AddSeconds(-1))
            };

            currentReportData = DatabaseConnection.ExecuteQuery(query, parameters);
            LoadRDLCReport("MonthlySalesReport_Simple.rdlc", "DataSet1");
            lblRecordCount.Text = $"📊 Total Months: {currentReportData.Rows.Count}";
        }

        private void GenerateStockReport()
        {
            string categoryFilter = cmbCategory.SelectedIndex > 0 ? 
                "AND i.Category = @Category" : "";

            string query = $@"
                SELECT 
                    i.ItemName,
                    i.Category,
                    i.StockQuantity,
                    i.ReorderLevel,
                    i.Price,
                    i.MRP,
                    (i.StockQuantity * i.Price) as StockValue
                FROM Items i
                WHERE i.ItemName IS NOT NULL {categoryFilter}
                ORDER BY i.Category, i.ItemName";

            var parameters = cmbCategory.SelectedIndex > 0 ? 
                new SqlParameter[] { new SqlParameter("@Category", cmbCategory.SelectedItem.ToString()) } :
                new SqlParameter[0];

            currentReportData = DatabaseConnection.ExecuteQuery(query, parameters);
            LoadRDLCReport("InventoryReport_Simple.rdlc", "DataSet1");
            lblRecordCount.Text = $"📊 Total Items: {currentReportData.Rows.Count}";
        }

        private void GenerateExpiredMedicinesReport()
        {
            string query = @"
                SELECT 
                    i.ItemName,
                    pi.BatchNumber,
                    pi.ExpiryDate,
                    pi.Quantity,
                    pi.UnitPrice,
                    (pi.Quantity * pi.UnitPrice) as TotalValue,
                    DATEDIFF(DAY, pi.ExpiryDate, GETDATE()) as DaysExpired,
                    c.CompanyName,
                    CASE 
                        WHEN pi.ExpiryDate < GETDATE() THEN 'EXPIRED'
                        WHEN pi.ExpiryDate <= DATEADD(DAY, 30, GETDATE()) THEN 'EXPIRING SOON'
                        ELSE 'VALID'
                    END as Status
                FROM PurchaseItems pi
                INNER JOIN Items i ON pi.ItemID = i.ItemID
                INNER JOIN Purchases p ON pi.PurchaseID = p.PurchaseID
                INNER JOIN Companies c ON p.CompanyID = c.CompanyID
                WHERE pi.ExpiryDate <= DATEADD(DAY, 90, GETDATE())
                ORDER BY pi.ExpiryDate ASC";

            currentReportData = DatabaseConnection.ExecuteQuery(query);
            LoadRDLCReport("ExpiredMedicinesReport_Simple.rdlc", "DataSet1");
            lblRecordCount.Text = $"⚠️ Total Expired/Expiring: {currentReportData.Rows.Count}";
        }

        private void GenerateProfitReport()
        {
            string query = @"
                SELECT 
                    i.ItemName,
                    i.Category,
                    SUM(si.Quantity) as SalesQuantity,
                    SUM(si.TotalAmount) as SalesAmount,
                    SUM(si.Quantity * i.Price) as CostAmount,
                    SUM(si.TotalAmount - (si.Quantity * i.Price)) as ProfitAmount,
                    CASE WHEN SUM(si.TotalAmount) > 0 
                        THEN (SUM(si.TotalAmount - (si.Quantity * i.Price)) / SUM(si.TotalAmount))
                        ELSE 0 END as ProfitMargin,
                    COALESCE(expenses.TotalExpenses, 0) as TotalExpenses,
                    SUM(si.TotalAmount - (si.Quantity * i.Price)) - COALESCE(expenses.TotalExpenses, 0) as NetProfit
                FROM SaleItems si
                INNER JOIN Items i ON si.ItemID = i.ItemID
                INNER JOIN Sales s ON si.SaleID = s.SaleID
                LEFT JOIN (
                    SELECT SUM(Amount) as TotalExpenses 
                    FROM Expenses 
                    WHERE ExpenseDate BETWEEN @FromDate AND @ToDate
                ) expenses ON 1=1
                WHERE s.SaleDate BETWEEN @FromDate AND @ToDate
                GROUP BY i.ItemName, i.Category, expenses.TotalExpenses
                ORDER BY NetProfit DESC";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@FromDate", dtpFromDate.Value.Date),
                new SqlParameter("@ToDate", dtpToDate.Value.Date.AddDays(1).AddSeconds(-1))
            };

            currentReportData = DatabaseConnection.ExecuteQuery(query, parameters);
            LoadRDLCReport("ProfitReport_Simple.rdlc", "DataSet1");
            lblRecordCount.Text = $"💰 Total Items Analyzed: {currentReportData.Rows.Count}";
        }

        private void GenerateCustomerWiseSalesReport()
        {
            string customerFilter = cmbCustomer.SelectedIndex > 0 ? 
                "AND c.CustomerID = @CustomerID" : "";

            string query = $@"
                SELECT 
                    c.CustomerName,
                    c.Phone,
                    COUNT(DISTINCT s.SaleID) as TotalSales,
                    SUM(s.TotalAmount) as TotalAmount,
                    SUM(s.Discount) as TotalDiscount,
                    SUM(s.NetAmount) as NetAmount,
                    MAX(s.SaleDate) as LastSaleDate,
                    c.CreditLimit,
                    c.CurrentBalance as OutstandingBalance
                FROM Customers c
                LEFT JOIN Sales s ON c.CustomerID = s.CustomerID 
                    AND s.SaleDate BETWEEN @FromDate AND @ToDate
                WHERE c.IsActive = 1 {customerFilter}
                GROUP BY c.CustomerID, c.CustomerName, c.Phone, c.CreditLimit, c.CurrentBalance
                ORDER BY SUM(s.NetAmount) DESC";

            var parameters = cmbCustomer.SelectedIndex > 0 ? 
                new[]
                {
                    new SqlParameter("@FromDate", dtpFromDate.Value.Date),
                    new SqlParameter("@ToDate", dtpToDate.Value.Date.AddDays(1).AddSeconds(-1)),
                    new SqlParameter("@CustomerID", ExtractIDFromComboBox(cmbCustomer))
                } :
                new[]
                {
                    new SqlParameter("@FromDate", dtpFromDate.Value.Date),
                    new SqlParameter("@ToDate", dtpToDate.Value.Date.AddDays(1).AddSeconds(-1))
                };

            currentReportData = DatabaseConnection.ExecuteQuery(query, parameters);
            LoadRDLCReport("CustomerWiseSalesReport_Simple.rdlc", "DataSet1");
            lblRecordCount.Text = $"👥 Total Customers: {currentReportData.Rows.Count}";
        }

        private void GenerateSupplierWisePurchasesReport()
        {
            string query = @"
                SELECT 
                    c.CompanyName as SupplierName,
                    p.PurchaseNumber,
                    p.PurchaseDate,
                    i.ItemName,
                    pi.Quantity,
                    pi.UnitPrice,
                    pi.TotalAmount,
                    'Cash' as PaymentStatus,
                    ISNULL(pi.CGST, 0) + ISNULL(pi.SGST, 0) + ISNULL(pi.IGST, 0) as TaxAmount
                FROM Purchases p
                INNER JOIN Companies c ON p.CompanyID = c.CompanyID
                INNER JOIN PurchaseItems pi ON p.PurchaseID = pi.PurchaseID
                INNER JOIN Items i ON pi.ItemID = i.ItemID
                WHERE p.PurchaseDate BETWEEN @FromDate AND @ToDate
                    AND p.IsActive = 1
                ORDER BY c.CompanyName, p.PurchaseDate DESC, p.PurchaseNumber";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@FromDate", dtpFromDate.Value.Date),
                new SqlParameter("@ToDate", dtpToDate.Value.Date.AddDays(1).AddSeconds(-1))
            };

            currentReportData = DatabaseConnection.ExecuteQuery(query, parameters);
            LoadRDLCReport("SupplierPurchaseHistoryReport_Simple.rdlc", "DataSet1");
            lblRecordCount.Text = $"📊 Supplier Purchases: {currentReportData.Rows.Count} transactions";
        }

        private void GeneratePurchaseReturnReport()
        {
            string query = @"
                SELECT 
                    pr.ReturnID,
                    pr.ReturnNumber as OriginalPurchaseID,
                    pr.ReturnDate,
                    c.CompanyName as SupplierName,
                    i.ItemName,
                    pri.ReturnQuantity as ReturnQuantity,
                    pri.ReturnReason,
                    pri.UnitPrice as RefundAmount,
                    'Processed' as ReturnStatus,
                    u.FullName as ProcessedBy
                FROM PurchaseReturns pr
                INNER JOIN Companies c ON pr.CompanyID = c.CompanyID
                INNER JOIN PurchaseReturnItems pri ON pr.ReturnID = pri.ReturnID
                INNER JOIN Items i ON pri.ItemID = i.ItemID
                INNER JOIN Users u ON pr.ProcessedBy = u.UserID
                WHERE pr.ReturnDate BETWEEN @FromDate AND @ToDate
                    AND pr.IsActive = 1
                ORDER BY pr.ReturnDate DESC, pr.ReturnNumber";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@FromDate", dtpFromDate.Value.Date),
                new SqlParameter("@ToDate", dtpToDate.Value.Date.AddDays(1).AddSeconds(-1))
            };

            currentReportData = DatabaseConnection.ExecuteQuery(query, parameters);
            LoadRDLCReport("PurchaseReturnReport.rdlc", "DataSet1");
            lblRecordCount.Text = $"📊 Purchase Returns: {currentReportData.Rows.Count} items returned";
        }

        private void GenerateExpenseReport()
        {
            string query = @"
                SELECT 
                    e.ExpenseID,
                    e.ExpenseDate,
                    e.ExpenseCategory,
                    e.ExpenseDescription,
                    e.Amount,
                    e.PaymentMethod
                FROM Expenses e
                WHERE e.ExpenseDate BETWEEN @FromDate AND @ToDate
                ORDER BY e.ExpenseDate DESC";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@FromDate", dtpFromDate.Value.Date),
                new SqlParameter("@ToDate", dtpToDate.Value.Date.AddDays(1).AddSeconds(-1))
            };

            currentReportData = DatabaseConnection.ExecuteQuery(query, parameters);
            LoadRDLCReport("ExpenseReport_Simple.rdlc", "DataSet1");
            lblRecordCount.Text = $"💸 Total Expenses: {currentReportData.Rows.Count}";
        }

        // Enhanced Supplier Reports Implementation
        private void GenerateEnhancedSupplierDetailsReport()
        {
            string query = @"
                SELECT 
                    c.CompanyID,
                    c.CompanyName,
                    ISNULL(c.ContactPerson, '') as ContactPerson,
                    ISNULL(c.Phone, '') as Phone,
                    ISNULL(c.Email, '') as Email,
                    ISNULL(c.Address, '') as Address,
                    'N/A' as City,
                    'N/A' as State,
                    'N/A' as PostalCode,
                    'N/A' as GSTNumber,
                    'N/A' as PANNumber,
                    COUNT(DISTINCT p.PurchaseID) as TotalPurchases,
                    ISNULL(SUM(p.TotalAmount), 0) as TotalPurchaseAmount,
                    ISNULL(MAX(p.PurchaseDate), c.CreatedDate) as LastPurchaseDate
                FROM Companies c
                LEFT JOIN Purchases p ON c.CompanyID = p.CompanyID 
                    AND p.PurchaseDate BETWEEN @FromDate AND @ToDate
                WHERE c.IsActive = 1 
                GROUP BY c.CompanyID, c.CompanyName, c.ContactPerson, c.Phone, c.Email, 
                         c.Address, c.CreatedDate
                ORDER BY c.CompanyName";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@FromDate", dtpFromDate.Value.Date),
                new SqlParameter("@ToDate", dtpToDate.Value.Date.AddDays(1).AddSeconds(-1))
            };

            currentReportData = DatabaseConnection.ExecuteQuery(query, parameters);
            LoadRDLCReport("SupplierDetailsReport.rdlc", "SupplierDetailsDataSet");
            lblRecordCount.Text = $"📊 Total Suppliers: {currentReportData.Rows.Count}";
        }

        private void GenerateSupplierPurchaseHistoryReport()
        {
            string query = @"
                SELECT 
                    c.CompanyName as SupplierName,
                    p.PurchaseNumber,
                    p.PurchaseDate,
                    p.TotalAmount,
                    ISNULL(p.Remarks, '') as ItemDetails
                FROM Purchases p
                INNER JOIN Companies c ON p.CompanyID = c.CompanyID
                WHERE p.PurchaseDate BETWEEN @FromDate AND @ToDate
                    AND p.IsActive = 1
                ORDER BY p.PurchaseDate DESC";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@FromDate", dtpFromDate.Value.Date),
                new SqlParameter("@ToDate", dtpToDate.Value.Date.AddDays(1).AddSeconds(-1))
            };

            currentReportData = DatabaseConnection.ExecuteQuery(query, parameters);
            LoadRDLCReport("SupplierPurchaseHistoryReport.rdlc", "SupplierPurchaseHistoryDataSet");
            lblRecordCount.Text = $"📊 Total Purchase Records: {currentReportData.Rows.Count}";
        }

        private void GenerateSupplierPayablesReport()
        {
            string query = @"
                SELECT 
                    c.CompanyName as SupplierName,
                    ISNULL(c.ContactPerson, '') as ContactPerson,
                    ISNULL(c.Phone, '') as Phone,
                    ISNULL(c.Email, '') as Email,
                    SUM(p.TotalAmount) as TotalPurchaseAmount,
                    COUNT(p.PurchaseID) as TotalInvoices,
                    MAX(p.PurchaseDate) as LastPurchaseDate
                FROM Companies c
                INNER JOIN Purchases p ON c.CompanyID = p.CompanyID
                WHERE p.PurchaseDate BETWEEN @FromDate AND @ToDate
                    AND p.IsActive = 1
                GROUP BY c.CompanyName, c.ContactPerson, c.Phone, c.Email
                ORDER BY SUM(p.TotalAmount) DESC";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@FromDate", dtpFromDate.Value.Date),
                new SqlParameter("@ToDate", dtpToDate.Value.Date.AddDays(1).AddSeconds(-1))
            };

            currentReportData = DatabaseConnection.ExecuteQuery(query, parameters);
            LoadRDLCReport("SupplierPayablesReport.rdlc", "SupplierPayablesDataSet");
            lblRecordCount.Text = $"📊 Suppliers with Transactions: {currentReportData.Rows.Count}";
        }

        private void GenerateSupplierLedgerReport()
        {
            string query = @"
                SELECT 
                    c.CompanyName as SupplierName,
                    'Purchase' as TransactionType,
                    p.PurchaseNumber as DocumentNumber,
                    p.PurchaseDate as TransactionDate,
                    p.TotalAmount as DebitAmount,
                    0 as CreditAmount,
                    ISNULL(p.Remarks, '') as Remarks
                FROM Companies c
                INNER JOIN Purchases p ON c.CompanyID = p.CompanyID
                WHERE p.PurchaseDate BETWEEN @FromDate AND @ToDate
                    AND p.IsActive = 1
                ORDER BY c.CompanyName, p.PurchaseDate";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@FromDate", dtpFromDate.Value.Date),
                new SqlParameter("@ToDate", dtpToDate.Value.Date.AddDays(1).AddSeconds(-1))
            };

            currentReportData = DatabaseConnection.ExecuteQuery(query, parameters);
            LoadRDLCReport("SupplierLedgerReport.rdlc", "SupplierLedgerDataSet");
            lblRecordCount.Text = $"📊 Total Ledger Entries: {currentReportData.Rows.Count}";
        }

        // Enhanced Stock Reports
        private void GenerateEnhancedCurrentStockReport()
        {
            string categoryFilter = cmbCategory.SelectedIndex > 0 ? 
                "AND i.Category = @Category" : "";

            string query = $@"
                SELECT 
                    i.ItemID,
                    i.ItemName,
                    ISNULL(i.Category, 'Uncategorized') as Category,
                    ISNULL(i.Description, '') as Description,
                    ISNULL(c.CompanyName, 'Unknown Supplier') as SupplierName,
                    i.StockQuantity,
                    ISNULL(i.ReorderLevel, 0) as ReorderLevel,
                    ISNULL(i.Price, 0) as Price,
                    ISNULL(i.MRP, 0) as MRP,
                    ISNULL(i.PurchasePrice, 0) as PurchasePrice,
                    (i.StockQuantity * ISNULL(i.PurchasePrice, 0)) as StockValue,
                    CASE 
                        WHEN i.StockQuantity <= 0 THEN 'Out of Stock'
                        WHEN i.StockQuantity <= ISNULL(i.ReorderLevel, 10) THEN 'Low Stock'
                        ELSE 'In Stock'
                    END as StockStatus,
                    i.CreatedDate
                FROM Items i
                LEFT JOIN Companies c ON i.CompanyID = c.CompanyID
                WHERE i.IsActive = 1 {categoryFilter}
                ORDER BY i.Category, i.ItemName";

            var parameters = new List<SqlParameter>();
            if (cmbCategory.SelectedIndex > 0)
            {
                parameters.Add(new SqlParameter("@Category", cmbCategory.SelectedItem.ToString()));
            }

            currentReportData = DatabaseConnection.ExecuteQuery(query, parameters.ToArray());
            LoadRDLCReport("CurrentStockReport_Enhanced.rdlc", "CurrentStockDataSet");
            lblRecordCount.Text = $"📊 Total Items: {currentReportData.Rows.Count}";
        }

        private void GenerateStockValuationReport()
        {
            string query = @"
                SELECT 
                    i.ItemName,
                    ISNULL(i.Category, 'Uncategorized') as Category,
                    i.StockQuantity,
                    ISNULL(i.PurchasePrice, 0) as PurchasePrice,
                    ISNULL(i.Price, 0) as SellingPrice,
                    ISNULL(i.MRP, 0) as MRP,
                    (i.StockQuantity * ISNULL(i.PurchasePrice, 0)) as PurchaseValue,
                    (i.StockQuantity * ISNULL(i.Price, 0)) as SellingValue,
                    (i.StockQuantity * ISNULL(i.MRP, 0)) as MRPValue,
                    ((ISNULL(i.Price, 0) - ISNULL(i.PurchasePrice, 0)) * i.StockQuantity) as PotentialProfit
                FROM Items i
                WHERE i.IsActive = 1 AND i.StockQuantity > 0
                ORDER BY (i.StockQuantity * ISNULL(i.PurchasePrice, 0)) DESC";

            currentReportData = DatabaseConnection.ExecuteQuery(query);
            LoadRDLCReport("StockValuationReport.rdlc", "StockValuationDataSet");
            lblRecordCount.Text = $"📊 Items in Stock: {currentReportData.Rows.Count}";
        }

        private void GenerateLowStockAlertReport()
        {
            string query = @"
                SELECT 
                    i.ItemID,
                    i.ItemName,
                    ISNULL(i.Category, 'Uncategorized') as Category,
                    ISNULL(i.Price, 0) as Price,
                    i.StockQuantity,
                    (i.StockQuantity * ISNULL(i.Price, 0)) as StockValue,
                    ISNULL(i.ReorderLevel, 10) as AlertLevel,
                    CASE 
                        WHEN i.StockQuantity = 0 THEN 'OUT OF STOCK'
                        WHEN i.StockQuantity <= (ISNULL(i.ReorderLevel, 10) * 0.5) THEN 'CRITICAL'
                        ELSE 'LOW STOCK'
                    END as Status
                FROM Items i
                LEFT JOIN Companies c ON i.CompanyID = c.CompanyID
                WHERE i.IsActive = 1 
                    AND i.StockQuantity <= ISNULL(i.ReorderLevel, 10)
                ORDER BY 
                    CASE 
                        WHEN i.StockQuantity = 0 THEN 1
                        WHEN i.StockQuantity <= (ISNULL(i.ReorderLevel, 10) * 0.5) THEN 2
                        ELSE 3
                    END,
                    i.StockQuantity";

            currentReportData = DatabaseConnection.ExecuteQuery(query);
            LoadRDLCReport("LowStockAlertReport.rdlc", "DataSet1");
            lblRecordCount.Text = $"📊 Low Stock Items: {currentReportData.Rows.Count}";
        }

        private void GenerateEnhancedExpiredMedicinesReport()
        {
            string query = @"
                SELECT 
                    i.ItemName,
                    ISNULL(i.Category, 'Uncategorized') as Category,
                    ISNULL(i.Description, '') as Description,
                    ISNULL(c.CompanyName, 'Unknown') as SupplierName,
                    ib.BatchNumber,
                    ib.ExpiryDate,
                    ib.QuantityAvailable,
                    ISNULL(ib.PurchasePrice, i.PurchasePrice) as PurchasePrice,
                    (ib.QuantityAvailable * ISNULL(ib.PurchasePrice, i.PurchasePrice)) as LossValue,
                    DATEDIFF(day, GETDATE(), ib.ExpiryDate) as DaysToExpiry,
                    CASE 
                        WHEN ib.ExpiryDate <= GETDATE() THEN 'EXPIRED'
                        WHEN DATEDIFF(day, GETDATE(), ib.ExpiryDate) <= 7 THEN 'CRITICAL'
                        WHEN DATEDIFF(day, GETDATE(), ib.ExpiryDate) <= 30 THEN 'WARNING'
                        WHEN DATEDIFF(day, GETDATE(), ib.ExpiryDate) <= 90 THEN 'MONITOR'
                        ELSE 'GOOD'
                    END as AlertLevel
                FROM Items i
                INNER JOIN ItemBatches ib ON i.ItemID = ib.ItemID
                LEFT JOIN Companies c ON i.CompanyID = c.CompanyID
                WHERE i.IsActive = 1 AND ib.IsActive = 1
                    AND ib.QuantityAvailable > 0
                    AND ib.ExpiryDate <= DATEADD(day, 90, GETDATE())
                ORDER BY ib.ExpiryDate, i.ItemName";

            currentReportData = DatabaseConnection.ExecuteQuery(query);
            LoadRDLCReport("ExpiredMedicinesReport_Enhanced.rdlc", "ExpiredMedicinesDataSet");
            lblRecordCount.Text = $"📊 Items Near/Past Expiry: {currentReportData.Rows.Count}";
        }

        private void GenerateStockMovementReport()
        {
            string query = @"
                SELECT 
                    i.ItemName,
                    'Sale' as TransactionType,
                    s.SaleDate as TransactionDate,
                    s.BillNumber as DocumentNumber,
                    -si.Quantity as Quantity,
                    si.Price as Rate,
                    'Customer: ' + ISNULL(c.CustomerName, 'Walk-in') as Reference
                FROM SaleItems si
                INNER JOIN Sales s ON si.SaleID = s.SaleID
                INNER JOIN Items i ON si.ItemID = i.ItemID
                LEFT JOIN Customers c ON s.CustomerID = c.CustomerID
                WHERE s.SaleDate BETWEEN @FromDate AND @ToDate
                    AND s.IsActive = 1
                
                UNION ALL
                
                SELECT 
                    i.ItemName,
                    'Purchase' as TransactionType,
                    p.PurchaseDate as TransactionDate,
                    p.PurchaseNumber as DocumentNumber,
                    pi.Quantity as Quantity,
                    pi.Price as Rate,
                    'Supplier: ' + ISNULL(comp.CompanyName, 'Unknown') as Reference
                FROM PurchaseItems pi
                INNER JOIN Purchases p ON pi.PurchaseID = p.PurchaseID
                INNER JOIN Items i ON pi.ItemID = i.ItemID
                LEFT JOIN Companies comp ON p.CompanyID = comp.CompanyID
                WHERE p.PurchaseDate BETWEEN @FromDate AND @ToDate
                    AND p.IsActive = 1
                
                ORDER BY TransactionDate DESC, ItemName";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@FromDate", dtpFromDate.Value.Date),
                new SqlParameter("@ToDate", dtpToDate.Value.Date.AddDays(1).AddSeconds(-1))
            };

            currentReportData = DatabaseConnection.ExecuteQuery(query, parameters);
            LoadRDLCReport("StockMovementReport.rdlc", "StockMovementDataSet");
            lblRecordCount.Text = $"📊 Total Movements: {currentReportData.Rows.Count}";
        }

        // Enhanced Financial Reports
        private void GenerateEnhancedProfitLossReport()
        {
            string query = @"
                -- Standard Profit & Loss Report Structure
                WITH RevenueData AS (
                    SELECT 
                        ISNULL(SUM(s.NetAmount), 0) as Revenue
                    FROM Sales s
                    WHERE s.SaleDate BETWEEN @FromDate AND @ToDate AND s.IsActive = 1
                ),
                COGSData AS (
                    SELECT 
                        ISNULL(SUM(si.Quantity * ISNULL(i.PurchasePrice, 0)), 0) as COGS
                    FROM Sales s
                    INNER JOIN SaleItems si ON s.SaleID = si.SaleID
                    INNER JOIN Items i ON si.ItemID = i.ItemID
                    WHERE s.SaleDate BETWEEN @FromDate AND @ToDate AND s.IsActive = 1
                ),
                OperatingExpenses AS (
                    SELECT 
                        ISNULL(SUM(Amount), 0) as TotalOperatingExpenses
                    FROM Expenses
                    WHERE ExpenseDate BETWEEN @FromDate AND @ToDate AND IsActive = 1
                )
                SELECT 
                    FORMAT(@FromDate, 'MMM dd, yyyy') + ' - ' + FORMAT(@ToDate, 'MMM dd, yyyy') as Period,
                    
                    -- Core P&L Structure
                    rd.Revenue,
                    cd.COGS,
                    (rd.Revenue - cd.COGS) as GrossProfit,
                    oe.TotalOperatingExpenses,
                    (rd.Revenue - cd.COGS - oe.TotalOperatingExpenses) as NetProfit,
                    
                    -- Key Percentages for Analysis
                    CASE WHEN rd.Revenue > 0 THEN 
                        ((rd.Revenue - cd.COGS) / rd.Revenue * 100) 
                        ELSE 0 END as GrossProfitMargin,
                    CASE WHEN rd.Revenue > 0 THEN 
                        ((rd.Revenue - cd.COGS - oe.TotalOperatingExpenses) / rd.Revenue * 100) 
                        ELSE 0 END as NetProfitMargin
                        
                FROM RevenueData rd, COGSData cd, OperatingExpenses oe";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@FromDate", dtpFromDate.Value.Date),
                new SqlParameter("@ToDate", dtpToDate.Value.Date.AddDays(1).AddSeconds(-1))
            };

            currentReportData = DatabaseConnection.ExecuteQuery(query, parameters);
            LoadRDLCReport("ProfitLossReport_Simple.rdlc", "ProfitLossDataSet");
            lblRecordCount.Text = $"📊 P&L Analysis Generated for Selected Period";
        }

        private void GenerateEnhancedExpenseReport()
        {
            string query = @"
                SELECT 
                    e.ExpenseID,
                    e.ExpenseDate,
                    ISNULL(e.ExpenseType, 'General') as ExpenseType,
                    ISNULL(e.Description, '') as Description,
                    e.Amount,
                    ISNULL(e.PaymentMethod, 'Cash') as PaymentMethod,
                    ISNULL(e.VoucherNumber, '') as VoucherNumber,
                    ISNULL(e.Remarks, '') as Remarks,
                    ISNULL(u.FullName, 'System') as CreatedBy
                FROM Expenses e
                LEFT JOIN Users u ON e.CreatedBy = u.UserID
                WHERE e.ExpenseDate BETWEEN @FromDate AND @ToDate
                    AND e.IsActive = 1
                ORDER BY e.ExpenseDate DESC";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@FromDate", dtpFromDate.Value.Date),
                new SqlParameter("@ToDate", dtpToDate.Value.Date.AddDays(1).AddSeconds(-1))
            };

            currentReportData = DatabaseConnection.ExecuteQuery(query, parameters);
            LoadRDLCReport("ExpenseReport_Enhanced.rdlc", "ExpenseDataSet");
            lblRecordCount.Text = $"📊 Total Expenses: {currentReportData.Rows.Count}";
        }

        private void GenerateDailySalesPurchaseReport()
        {
            string query = @"
                WITH DailySales AS (
                    SELECT 
                        CAST(SaleDate AS DATE) as TransactionDate,
                        COUNT(*) as SalesCount,
                        SUM(NetAmount) as SalesAmount,
                        SUM(ISNULL(Discount, 0)) as SalesDiscount,
                        AVG(NetAmount) as AvgSaleAmount
                    FROM Sales 
                    WHERE SaleDate BETWEEN @FromDate AND @ToDate AND IsActive = 1
                    GROUP BY CAST(SaleDate AS DATE)
                ),
                DailyPurchases AS (
                    SELECT 
                        CAST(PurchaseDate AS DATE) as TransactionDate,
                        COUNT(*) as PurchaseCount,
                        SUM(TotalAmount) as PurchaseAmount,
                        AVG(TotalAmount) as AvgPurchaseAmount
                    FROM Purchases 
                    WHERE PurchaseDate BETWEEN @FromDate AND @ToDate AND IsActive = 1
                    GROUP BY CAST(PurchaseDate AS DATE)
                )
                SELECT 
                    COALESCE(ds.TransactionDate, dp.TransactionDate) as TransactionDate,
                    ISNULL(ds.SalesCount, 0) as SalesCount,
                    ISNULL(ds.SalesAmount, 0) as SalesAmount,
                    ISNULL(ds.SalesDiscount, 0) as SalesDiscount,
                    ISNULL(ds.AvgSaleAmount, 0) as AvgSaleAmount,
                    ISNULL(dp.PurchaseCount, 0) as PurchaseCount,
                    ISNULL(dp.PurchaseAmount, 0) as PurchaseAmount,
                    ISNULL(dp.AvgPurchaseAmount, 0) as AvgPurchaseAmount,
                    (ISNULL(ds.SalesAmount, 0) - ISNULL(dp.PurchaseAmount, 0)) as NetAmount
                FROM DailySales ds
                FULL OUTER JOIN DailyPurchases dp ON ds.TransactionDate = dp.TransactionDate
                ORDER BY COALESCE(ds.TransactionDate, dp.TransactionDate)";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@FromDate", dtpFromDate.Value.Date),
                new SqlParameter("@ToDate", dtpToDate.Value.Date.AddDays(1).AddSeconds(-1))
            };

            currentReportData = DatabaseConnection.ExecuteQuery(query, parameters);
            LoadRDLCReport("DailySalesPurchaseReport.rdlc", "DailySalesPurchaseDataSet");
            lblRecordCount.Text = $"📊 Daily Analysis: {currentReportData.Rows.Count} days";
        }

        // GST Reports
        private void GenerateGSTSalesReport()
        {
            string query = @"
                SELECT 
                    s.BillNumber,
                    s.SaleDate,
                    ISNULL(c.CustomerName, 'Walk-in Customer') as CustomerName,
                    ISNULL(c.GSTNumber, 'UNREGISTERED') as CustomerGST,
                    s.TotalAmount as TaxableAmount,
                    0 as GSTAmount,
                    s.NetAmount,
                    CASE WHEN s.TotalAmount > 0 THEN 'GST Sale' ELSE 'Non-GST Sale' END as SaleType,
                    ISNULL(s.Remarks, '') as ItemDetails
                FROM Sales s
                LEFT JOIN Customers c ON s.CustomerID = c.CustomerID
                WHERE s.SaleDate BETWEEN @FromDate AND @ToDate
                    AND s.IsActive = 1
                ORDER BY s.SaleDate DESC";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@FromDate", dtpFromDate.Value.Date),
                new SqlParameter("@ToDate", dtpToDate.Value.Date.AddDays(1).AddSeconds(-1))
            };

            currentReportData = DatabaseConnection.ExecuteQuery(query, parameters);
            LoadRDLCReport("GSTSalesReport_Enhanced.rdlc", "GSTSalesDataSet");
            lblRecordCount.Text = $"📊 GST Sales Records: {currentReportData.Rows.Count}";
        }

        private void GenerateGSTPurchaseReport()
        {
            string query = @"
                SELECT 
                    p.PurchaseNumber,
                    p.PurchaseDate,
                    c.CompanyName as SupplierName,
                    ISNULL(c.GSTNumber, 'NOT AVAILABLE') as SupplierGST,
                    p.TotalAmount as TaxableAmount,
                    0 as GSTAmount,
                    p.TotalAmount as NetAmount,
                    'GST Purchase' as PurchaseType,
                    ISNULL(p.Remarks, '') as ItemDetails
                FROM Purchases p
                INNER JOIN Companies c ON p.CompanyID = c.CompanyID
                WHERE p.PurchaseDate BETWEEN @FromDate AND @ToDate
                    AND p.IsActive = 1
                ORDER BY p.PurchaseDate DESC";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@FromDate", dtpFromDate.Value.Date),
                new SqlParameter("@ToDate", dtpToDate.Value.Date.AddDays(1).AddSeconds(-1))
            };

            currentReportData = DatabaseConnection.ExecuteQuery(query, parameters);
            LoadRDLCReport("GSTPurchaseReport_Enhanced.rdlc", "GSTPurchaseDataSet");
            lblRecordCount.Text = $"📊 GST Purchase Records: {currentReportData.Rows.Count}";
        }

        private void GenerateGSTR1Report()
        {
            string query = @"
                SELECT 
                    'B2C' as ReportSection,
                    s.BillNumber,
                    s.SaleDate,
                    ISNULL(c.CustomerName, 'Walk-in Customer') as CustomerName,
                    ISNULL(c.GSTNumber, '') as GSTIN,
                    ISNULL(c.State, 'Local') as PlaceOfSupply,
                    s.TotalAmount as TaxableValue,
                    0 as CGSTAmount,
                    0 as SGSTAmount,
                    0 as IGSTAmount,
                    0 as TotalTax,
                    s.NetAmount as InvoiceValue,
                    ISNULL(s.Remarks, '') as HSNDetails
                FROM Sales s
                LEFT JOIN Customers c ON s.CustomerID = c.CustomerID
                WHERE s.SaleDate BETWEEN @FromDate AND @ToDate
                    AND s.IsActive = 1
                ORDER BY s.SaleDate";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@FromDate", dtpFromDate.Value.Date),
                new SqlParameter("@ToDate", dtpToDate.Value.Date.AddDays(1).AddSeconds(-1))
            };

            currentReportData = DatabaseConnection.ExecuteQuery(query, parameters);
            LoadRDLCReport("GSTR1Report_Enhanced.rdlc", "GSTR1DataSet");
            lblRecordCount.Text = $"📊 GSTR-1 Records: {currentReportData.Rows.Count}";
        }

        private void GenerateHSNSACReport()
        {
            string query = @"
                SELECT 
                    ISNULL(i.HSNCode, 'N/A') as HSNCode,
                    i.ItemName,
                    ISNULL(i.Category, 'Uncategorized') as Category,
                    i.StockQuantity,
                    ISNULL(i.Price, 0) as SellingPrice,
                    ISNULL(i.MRP, 0) as MRP,
                    'Goods' as ItemType,
                    CASE 
                        WHEN ISNULL(i.HSNCode, '') = '' THEN 'Missing HSN'
                        WHEN LEN(ISNULL(i.HSNCode, '')) < 4 THEN 'Invalid HSN'
                        ELSE 'Valid HSN'
                    END as HSNStatus
                FROM Items i
                WHERE i.IsActive = 1
                ORDER BY ISNULL(i.HSNCode, 'ZZZZ'), i.ItemName";

            currentReportData = DatabaseConnection.ExecuteQuery(query);
            LoadRDLCReport("HSNSACReport_Enhanced.rdlc", "HSNSACDataSet");
            lblRecordCount.Text = $"📊 Total Items: {currentReportData.Rows.Count}";
        }

        // Analytics Reports
        private void GenerateABCAnalysisReport()
        {
            string query = @"
                WITH SalesAnalysis AS (
                    SELECT 
                        i.ItemName,
                        i.Category,
                        SUM(si.Quantity) as TotalQuantitySold,
                        SUM(si.TotalAmount) as TotalSalesValue,
                        COUNT(DISTINCT s.SaleID) as TransactionCount,
                        AVG(si.Price) as AvgSellingPrice
                    FROM Sales s
                    INNER JOIN SaleItems si ON s.SaleID = si.SaleID
                    INNER JOIN Items i ON si.ItemID = i.ItemID
                    WHERE s.SaleDate BETWEEN @FromDate AND @ToDate
                        AND s.IsActive = 1
                    GROUP BY i.ItemName, i.Category
                ),
                RankedItems AS (
                    SELECT *,
                        ROW_NUMBER() OVER (ORDER BY TotalSalesValue DESC) as SalesRank,
                        NTILE(3) OVER (ORDER BY TotalSalesValue DESC) as ABCCategory
                    FROM SalesAnalysis
                )
                SELECT 
                    ItemName,
                    Category,
                    TotalQuantitySold,
                    TotalSalesValue,
                    TransactionCount,
                    AvgSellingPrice,
                    SalesRank,
                    CASE ABCCategory
                        WHEN 1 THEN 'A - High Value'
                        WHEN 2 THEN 'B - Medium Value'
                        ELSE 'C - Low Value'
                    END as ABCClassification
                FROM RankedItems
                ORDER BY SalesRank";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@FromDate", dtpFromDate.Value.Date),
                new SqlParameter("@ToDate", dtpToDate.Value.Date.AddDays(1).AddSeconds(-1))
            };

            currentReportData = DatabaseConnection.ExecuteQuery(query, parameters);
            LoadRDLCReport("ABCAnalysisReport.rdlc", "ABCAnalysisDataSet");
            lblRecordCount.Text = $"📊 Items Analyzed: {currentReportData.Rows.Count}";
        }

        private void GenerateCustomerOutstandingReport()
        {
            string query = @"
                SELECT 
                    c.CustomerName,
                    ISNULL(c.Phone, '') as Phone,
                    ISNULL(c.Email, '') as Email,
                    ISNULL(c.Address, '') as Address,
                    COUNT(s.SaleID) as TotalSales,
                    SUM(s.TotalAmount) as TotalSalesAmount,
                    SUM(CASE WHEN s.IsCredit = 1 THEN s.NetAmount ELSE 0 END) as CreditSales,
                    MAX(s.SaleDate) as LastSaleDate,
                    DATEDIFF(day, MAX(s.SaleDate), GETDATE()) as DaysSinceLastSale
                FROM Customers c
                LEFT JOIN Sales s ON c.CustomerID = s.CustomerID 
                    AND s.SaleDate BETWEEN @FromDate AND @ToDate
                    AND s.IsActive = 1
                WHERE c.IsActive = 1
                GROUP BY c.CustomerName, c.Phone, c.Email, c.Address
                HAVING COUNT(s.SaleID) > 0
                ORDER BY SUM(CASE WHEN s.IsCredit = 1 THEN s.NetAmount ELSE 0 END) DESC";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@FromDate", dtpFromDate.Value.Date),
                new SqlParameter("@ToDate", dtpToDate.Value.Date.AddDays(1).AddSeconds(-1))
            };

            currentReportData = DatabaseConnection.ExecuteQuery(query, parameters);
            LoadRDLCReport("CustomerOutstandingReport.rdlc", "CustomerOutstandingDataSet");
            lblRecordCount.Text = $"📊 Customers with Outstanding: {currentReportData.Rows.Count}";
        }

        private void GenerateSupplierOutstandingReport()
        {
            string query = @"
                SELECT 
                    c.CompanyName as SupplierName,
                    ISNULL(c.ContactPerson, '') as ContactPerson,
                    ISNULL(c.Phone, '') as Phone,
                    ISNULL(c.Email, '') as Email,
                    COUNT(p.PurchaseID) as TotalPurchases,
                    SUM(p.TotalAmount) as TotalPurchaseAmount,
                    MAX(p.PurchaseDate) as LastPurchaseDate,
                    DATEDIFF(day, MAX(p.PurchaseDate), GETDATE()) as DaysSinceLastPurchase
                FROM Companies c
                LEFT JOIN Purchases p ON c.CompanyID = p.CompanyID 
                    AND p.PurchaseDate BETWEEN @FromDate AND @ToDate
                    AND p.IsActive = 1
                WHERE c.IsActive = 1
                GROUP BY c.CompanyName, c.ContactPerson, c.Phone, c.Email
                HAVING COUNT(p.PurchaseID) > 0
                ORDER BY SUM(p.TotalAmount) DESC";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@FromDate", dtpFromDate.Value.Date),
                new SqlParameter("@ToDate", dtpToDate.Value.Date.AddDays(1).AddSeconds(-1))
            };

            currentReportData = DatabaseConnection.ExecuteQuery(query, parameters);
            LoadRDLCReport("SupplierOutstandingReport.rdlc", "SupplierOutstandingDataSet");
            lblRecordCount.Text = $"📊 Active Suppliers: {currentReportData.Rows.Count}";
        }

        private void GenerateSalesReturnReport()
        {
            string query = @"
                SELECT 
                    sr.ReturnID,
                    'SR' + RIGHT('000' + CAST(sr.ReturnID as VARCHAR), 3) as ReturnNumber,
                    sr.ReturnDate,
                    'Customer' as CustomerName,
                    i.ItemName,
                    sri.ReturnQuantity as Quantity,
                    sri.Price as UnitPrice,
                    sri.TotalAmount,
                    sr.Remarks as Reason,
                    'Active' as Status
                FROM SaleReturns sr
                INNER JOIN SaleReturnItems sri ON sr.ReturnID = sri.ReturnID
                INNER JOIN Items i ON sri.ItemID = i.ItemID
                WHERE sr.ReturnDate BETWEEN @FromDate AND @ToDate
                ORDER BY sr.ReturnDate DESC, sr.ReturnID";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@FromDate", dtpFromDate.Value.Date),
                new SqlParameter("@ToDate", dtpToDate.Value.Date.AddDays(1).AddSeconds(-1))
            };

            currentReportData = DatabaseConnection.ExecuteQuery(query, parameters);
            LoadRDLCReport("SalesReturnReport_Simple.rdlc", "DataSet1");
            lblRecordCount.Text = $"📊 Sales Returns: {currentReportData.Rows.Count} items returned";
        }

        private void GeneratePaymentMethodAnalysisReport()
        {
            string query = @"
                SELECT 
                    s.PaymentMethod,
                    COUNT(*) as TransactionCount,
                    SUM(s.TotalAmount) as TotalAmount,
                    SUM(s.NetAmount) as NetAmount,
                    AVG(s.NetAmount) as AvgTransactionValue,
                    MIN(s.NetAmount) as MinTransactionValue,
                    MAX(s.NetAmount) as MaxTransactionValue,
                    CAST(COUNT(*) * 100.0 / (SELECT COUNT(*) FROM Sales WHERE SaleDate BETWEEN @FromDate AND @ToDate AND IsActive = 1) AS DECIMAL(10,2)) as PercentageOfTransactions
                FROM Sales s
                WHERE s.SaleDate BETWEEN @FromDate AND @ToDate
                    AND s.IsActive = 1
                GROUP BY s.PaymentMethod
                ORDER BY COUNT(*) DESC";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@FromDate", dtpFromDate.Value.Date),
                new SqlParameter("@ToDate", dtpToDate.Value.Date.AddDays(1).AddSeconds(-1))
            };

            currentReportData = DatabaseConnection.ExecuteQuery(query, parameters);
            LoadRDLCReport("PaymentMethodAnalysisReport.rdlc", "PaymentMethodDataSet");
            lblRecordCount.Text = $"📊 Payment Methods: {currentReportData.Rows.Count}";
        }

        private void LoadRDLCReport(string rdlcFileName, string dataSetName)
        {
            if (reportViewer == null || currentReportData == null)
            {
                ShowDataInGrid();
                return;
            }

            try
            {
                // Smart RDLC path detection
                string simplePath = Path.Combine(Application.StartupPath, "Reports", rdlcFileName);
                string fallbackPath = $"Reports/{rdlcFileName}";
                string selectedPath = "";

                if (File.Exists(simplePath))
                {
                    selectedPath = simplePath;
                }
                else if (File.Exists(fallbackPath))
                {
                    selectedPath = fallbackPath;
                }
                else
                {
                    // Try with _Simple suffix for compatibility
                    string simpleRdlc = rdlcFileName.Replace(".rdlc", "_Simple.rdlc");
                    string simpleFullPath = Path.Combine(Application.StartupPath, "Reports", simpleRdlc);
                    
                    if (File.Exists(simpleFullPath))
                    {
                        selectedPath = simpleFullPath;
                    }
                    else
                    {
                        // RDLC file not found, show data in grid instead
                        ShowDataInGrid();
                        return;
                    }
                }

                reportViewer.LocalReport.ReportPath = selectedPath;

                // Clear and add data source
                reportViewer.LocalReport.DataSources.Clear();
                ReportDataSource dataSource = new ReportDataSource(dataSetName, currentReportData);
                reportViewer.LocalReport.DataSources.Add(dataSource);
                
                // Add report parameters if the report supports them
                try
                {
                    List<ReportParameter> parameters = new List<ReportParameter>();
                    
                    // Check if this is a Purchase Return Report (uses different parameter names)
                    if (rdlcFileName.Contains("PurchaseReturn"))
                    {
                        // Purchase Return Report uses StartDate/EndDate instead of FromDate/ToDate
                        parameters.Add(new ReportParameter("StartDate", dtpFromDate.Value.ToString("dd/MM/yyyy")));
                        parameters.Add(new ReportParameter("EndDate", dtpToDate.Value.ToString("dd/MM/yyyy")));
                        parameters.Add(new ReportParameter("SupplierID", "0")); // Default to 0 (all suppliers)
                        parameters.Add(new ReportParameter("ReturnStatus", "All")); // Default to "All" status
                    }
                    else if (rdlcFileName.Contains("LowStockAlert"))
                    {
                        // Low Stock Alert Report uses specific parameters
                        parameters.Add(new ReportParameter("AlertThreshold", "10")); // Default alert threshold
                        parameters.Add(new ReportParameter("CategoryFilter", "All")); // Default to all categories
                    }
                    else
                    {
                        // Standard parameters used by most reports
                        parameters.Add(new ReportParameter("FromDate", dtpFromDate.Value.ToString("dd/MM/yyyy")));
                        parameters.Add(new ReportParameter("ToDate", dtpToDate.Value.ToString("dd/MM/yyyy")));
                        parameters.Add(new ReportParameter("GeneratedBy", UserSession.FullName ?? "System"));
                        parameters.Add(new ReportParameter("GeneratedDate", DateTime.Now.ToString("dd/MM/yyyy HH:mm")));
                        parameters.Add(new ReportParameter("CompanyName", "Aziz Hospital Pharmacy"));
                    }
                    
                    reportViewer.LocalReport.SetParameters(parameters);
                }
                catch
                {
                    // Parameters are optional, continue without them
                }
                
                // Force refresh
                reportViewer.LocalReport.Refresh();
                reportViewer.RefreshReport();
                
                lblRecordCount.Text += $" | Report: {Path.GetFileName(selectedPath)}";
            }
            catch (Exception ex)
            {
                // Only show error message for critical errors, not missing RDLC files
                if (!(ex is FileNotFoundException))
                {
                    MessageBox.Show($"Error loading report: {ex.Message}\n\nDisplaying data in table format instead.", 
                        "Report Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                ShowDataInGrid();
            }
        }

        private void ShowDataInGrid()
        {
            if (currentReportData == null) return;

            var dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                DataSource = currentReportData,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.LightGray }
            };

            // Clear and add to the second row of main layout
            var mainLayout = this.Controls.OfType<TableLayoutPanel>().FirstOrDefault();
            if (mainLayout != null && mainLayout.Controls.Count > 1)
            {
                mainLayout.Controls.RemoveAt(1);
                mainLayout.Controls.Add(dgv, 0, 1);
            }
        }

        private int ExtractIDFromComboBox(ComboBox comboBox)
        {
            string selectedText = comboBox.SelectedItem?.ToString() ?? "";
            if (selectedText.Contains("(ID: "))
            {
                int startIndex = selectedText.IndexOf("(ID: ") + 5;
                int endIndex = selectedText.IndexOf(")", startIndex);
                if (int.TryParse(selectedText.Substring(startIndex, endIndex - startIndex), out int id))
                {
                    return id;
                }
            }
            return 0;
        }

        private void BtnExportPDF_Click(object sender, EventArgs e)
        {
            try
            {
                if (reportViewer?.LocalReport?.DataSources?.Count > 0)
                {
                    // Export RDLC to PDF
                    Warning[] warnings;
                    string[] streamids;
                    string mimeType;
                    string encoding;
                    string extension;
                    byte[] renderedBytes = reportViewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                    
                    SaveFileDialog saveDialog = new SaveFileDialog
                    {
                        Filter = "PDF files (*.pdf)|*.pdf",
                        FileName = $"Report_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
                    };

                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        File.WriteAllBytes(saveDialog.FileName, renderedBytes);
                        MessageBox.Show("Report exported successfully!", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Please generate a report first.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Export failed: {ex.Message}", "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnExportExcel_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentReportData?.Rows.Count > 0)
                {
                    SaveFileDialog saveDialog = new SaveFileDialog
                    {
                        Filter = "Excel files (*.csv)|*.csv",
                        FileName = $"Report_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
                    };

                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        ExportToCSV(currentReportData, saveDialog.FileName);
                        MessageBox.Show("Report exported successfully!", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Please generate a report first.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Export failed: {ex.Message}", "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerateTrialBalanceReport()
        {
            string query = @"
                WITH AccountBalances AS (
                    -- Sales Account (Credit - Revenue)
                    SELECT 
                        'SALES' as AccountCode,
                        'Sales Revenue' as AccountName, 
                        'Income' as AccountType,
                        0.00 as DebitBalance,
                        ISNULL(SUM(s.TotalAmount), 0) as CreditBalance
                    FROM Sales s
                    WHERE s.SaleDate BETWEEN @FromDate AND @ToDate AND s.IsActive = 1
                    
                    UNION ALL
                    
                    -- Purchase Account (Debit - Cost of Goods Sold)
                    SELECT 
                        'PURCH' as AccountCode,
                        'Cost of Goods Sold' as AccountName, 
                        'Expense' as AccountType,
                        ISNULL(SUM(p.TotalAmount), 0) as DebitBalance,
                        0.00 as CreditBalance
                    FROM Purchases p
                    WHERE p.PurchaseDate BETWEEN @FromDate AND @ToDate AND p.IsActive = 1
                    
                    UNION ALL
                    
                    -- Customer Payments (Debit - Cash/Bank)
                    SELECT 
                        'CASH' as AccountCode,
                        'Cash & Bank Receipts' as AccountName, 
                        'Asset' as AccountType,
                        ISNULL(SUM(cp.Amount), 0) as DebitBalance,
                        0.00 as CreditBalance
                    FROM CustomerPayments cp
                    WHERE cp.PaymentDate BETWEEN @FromDate AND @ToDate
                    
                    UNION ALL
                    
                    -- Supplier Payables (Credit - Accounts Payable)
                    SELECT 
                        'PAYBL' as AccountCode,
                        'Accounts Payable - Suppliers' as AccountName, 
                        'Liability' as AccountType,
                        0.00 as DebitBalance,
                        ISNULL(SUM(p.TotalAmount), 0) as CreditBalance
                    FROM Purchases p
                    WHERE p.PurchaseDate BETWEEN @FromDate AND @ToDate AND p.IsActive = 1
                    
                    UNION ALL
                    
                    -- Operating Expenses (Debit)
                    SELECT 
                        'OPEXP' as AccountCode,
                        'Operating Expenses' as AccountName, 
                        'Expense' as AccountType,
                        ISNULL(SUM(e.Amount), 0) as DebitBalance,
                        0.00 as CreditBalance
                    FROM Expenses e
                    WHERE e.ExpenseDate BETWEEN @FromDate AND @ToDate AND e.IsActive = 1
                    
                    UNION ALL
                    
                    -- Inventory/Stock (Debit - Current Asset)
                    SELECT 
                        'INV' as AccountCode,
                        'Inventory - Stock on Hand' as AccountName, 
                        'Asset' as AccountType,
                        ISNULL(SUM(i.StockQuantity * ISNULL(i.PurchasePrice, i.Price)), 0) as DebitBalance,
                        0.00 as CreditBalance
                    FROM Items i
                    WHERE i.IsActive = 1
                )
                SELECT 
                    AccountCode,
                    AccountName,
                    AccountType,
                    ROUND(DebitBalance, 2) as DebitBalance,
                    ROUND(CreditBalance, 2) as CreditBalance
                FROM AccountBalances
                WHERE DebitBalance > 0 OR CreditBalance > 0
                ORDER BY AccountType DESC, AccountName";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@FromDate", dtpFromDate.Value.Date),
                new SqlParameter("@ToDate", dtpToDate.Value.Date.AddDays(1).AddSeconds(-1))
            };

            currentReportData = DatabaseConnection.ExecuteQuery(query, parameters);
            
            // Calculate totals for validation
            decimal totalDebit = 0, totalCredit = 0;
            if (currentReportData != null && currentReportData.Rows.Count > 0)
            {
                foreach (DataRow row in currentReportData.Rows)
                {
                    totalDebit += Convert.ToDecimal(row["DebitBalance"]);
                    totalCredit += Convert.ToDecimal(row["CreditBalance"]);
                }
            }
            
            // Check if trial balance is balanced
            bool isBalanced = Math.Abs(totalDebit - totalCredit) < 0.01m;
            string balanceStatus = isBalanced ? "✅ BALANCED" : "❌ NOT BALANCED";
            
            LoadRDLCReport("TrialBalanceReport_Simple.rdlc", "DataSet1");
            lblRecordCount.Text = $"📊 Trial Balance: {currentReportData.Rows.Count} accounts | Total Debit: ₹{totalDebit:N2} | Total Credit: ₹{totalCredit:N2} | Status: {balanceStatus}";
        }

        private void ExportToCSV(DataTable dataTable, string filePath)
        {
            using (var writer = new StreamWriter(filePath))
            {
                // Write headers
                writer.WriteLine(string.Join(",", dataTable.Columns.Cast<DataColumn>().Select(column => $"\"{column.ColumnName}\"")));

                // Write data
                foreach (DataRow row in dataTable.Rows)
                {
                    writer.WriteLine(string.Join(",", row.ItemArray.Select(field => $"\"{field}\"")));
                }
            }
        }
    }
}