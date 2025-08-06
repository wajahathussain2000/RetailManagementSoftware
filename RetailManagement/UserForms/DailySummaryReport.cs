using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic; // Added for HashSet
using System.Linq;
using RetailManagement.Database; // Added for OrderByDescending and FirstOrDefault

namespace RetailManagement.UserForms
{
    public partial class DailySummaryReport : Form
    {
        private DataTable salesData;
        private DataTable purchaseData;
        private decimal totalSales = 0;
        private decimal totalPurchases = 0;
        private decimal totalProfit = 0;

        public DailySummaryReport()
        {
            InitializeComponent();
            LoadData();
        }

        private void DailySummaryReport_Load(object sender, EventArgs e)
        {
            dtpFromDate.Value = DateTime.Today.AddDays(-30);
            dtpToDate.Value = DateTime.Today;
            LoadDailySummary();
        }

        private void LoadData()
        {
            // Initialize data tables
            salesData = new DataTable();
            purchaseData = new DataTable();
        }

        private void LoadDailySummary()
        {
            try
            {
                string fromDate = dtpFromDate.Value.ToString("yyyy-MM-dd");
                string toDate = dtpToDate.Value.ToString("yyyy-MM-dd");

                // Load sales summary
                string salesQuery = @"
                    SELECT 
                        CAST(SaleDate AS DATE) as Date,
                        COUNT(*) as TotalBills,
                        SUM(TotalAmount) as TotalSales,
                        SUM(PaidAmount) as TotalPaid,
                        SUM(TotalAmount - PaidAmount) as TotalOutstanding
                    FROM Sales 
                    WHERE CAST(SaleDate AS DATE) BETWEEN @FromDate AND @ToDate
                    GROUP BY CAST(SaleDate AS DATE)
                    ORDER BY Date DESC";

                salesData = DatabaseConnection.ExecuteQuery(salesQuery, 
                    new System.Data.SqlClient.SqlParameter[] 
                    {
                        new System.Data.SqlClient.SqlParameter("@FromDate", fromDate),
                        new System.Data.SqlClient.SqlParameter("@ToDate", toDate)
                    });

                // Load purchase summary
                string purchaseQuery = @"
                    SELECT 
                        CAST(PurchaseDate AS DATE) as Date,
                        COUNT(*) as TotalBills,
                        SUM(TotalAmount) as TotalPurchases,
                        SUM(PaidAmount) as TotalPaid,
                        SUM(TotalAmount - PaidAmount) as TotalOutstanding
                    FROM Purchases 
                    WHERE CAST(PurchaseDate AS DATE) BETWEEN @FromDate AND @ToDate
                    GROUP BY CAST(PurchaseDate AS DATE)
                    ORDER BY Date DESC";

                purchaseData = DatabaseConnection.ExecuteQuery(purchaseQuery,
                    new System.Data.SqlClient.SqlParameter[] 
                    {
                        new System.Data.SqlClient.SqlParameter("@FromDate", fromDate),
                        new System.Data.SqlClient.SqlParameter("@ToDate", toDate)
                    });

                DisplaySummaryData();
                CalculateTotals();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading daily summary: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisplaySummaryData()
        {
            dgvDailySummary.Rows.Clear();

            // Create a combined view of sales and purchases
            var combinedData = new DataTable();
            combinedData.Columns.Add("Date", typeof(DateTime));
            combinedData.Columns.Add("SalesBills", typeof(int));
            combinedData.Columns.Add("SalesAmount", typeof(decimal));
            combinedData.Columns.Add("SalesPaid", typeof(decimal));
            combinedData.Columns.Add("SalesOutstanding", typeof(decimal));
            combinedData.Columns.Add("PurchaseBills", typeof(int));
            combinedData.Columns.Add("PurchaseAmount", typeof(decimal));
            combinedData.Columns.Add("PurchasePaid", typeof(decimal));
            combinedData.Columns.Add("PurchaseOutstanding", typeof(decimal));
            combinedData.Columns.Add("NetCashFlow", typeof(decimal));

            // Get all unique dates
            var allDates = new HashSet<DateTime>();
            foreach (DataRow row in salesData.Rows)
                allDates.Add(Convert.ToDateTime(row["Date"]));
            foreach (DataRow row in purchaseData.Rows)
                allDates.Add(Convert.ToDateTime(row["Date"]));

            foreach (var date in allDates.OrderByDescending(d => d))
            {
                var salesRow = salesData.Select($"Date = '{date:yyyy-MM-dd}'").FirstOrDefault();
                var purchaseRow = purchaseData.Select($"Date = '{date:yyyy-MM-dd}'").FirstOrDefault();

                var newRow = combinedData.NewRow();
                newRow["Date"] = date;
                newRow["SalesBills"] = salesRow != null ? Convert.ToInt32(salesRow["TotalBills"]) : 0;
                newRow["SalesAmount"] = salesRow != null ? Convert.ToDecimal(salesRow["TotalSales"]) : 0;
                newRow["SalesPaid"] = salesRow != null ? Convert.ToDecimal(salesRow["TotalPaid"]) : 0;
                newRow["SalesOutstanding"] = salesRow != null ? Convert.ToDecimal(salesRow["TotalOutstanding"]) : 0;
                newRow["PurchaseBills"] = purchaseRow != null ? Convert.ToInt32(purchaseRow["TotalBills"]) : 0;
                newRow["PurchaseAmount"] = purchaseRow != null ? Convert.ToDecimal(purchaseRow["TotalPurchases"]) : 0;
                newRow["PurchasePaid"] = purchaseRow != null ? Convert.ToDecimal(purchaseRow["TotalPaid"]) : 0;
                newRow["PurchaseOutstanding"] = purchaseRow != null ? Convert.ToDecimal(purchaseRow["TotalOutstanding"]) : 0;
                newRow["NetCashFlow"] = Convert.ToDecimal(newRow["SalesPaid"]) - Convert.ToDecimal(newRow["PurchasePaid"]);
                combinedData.Rows.Add(newRow);
            }

            dgvDailySummary.DataSource = combinedData;
            ConfigureDataGridView();
        }

        private void ConfigureDataGridView()
        {
            dgvDailySummary.Columns["Date"].HeaderText = "Date";
            dgvDailySummary.Columns["Date"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvDailySummary.Columns["Date"].Width = 100;

            dgvDailySummary.Columns["SalesBills"].HeaderText = "Sales Bills";
            dgvDailySummary.Columns["SalesBills"].Width = 80;
            dgvDailySummary.Columns["SalesBills"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvDailySummary.Columns["SalesAmount"].HeaderText = "Sales Amount";
            dgvDailySummary.Columns["SalesAmount"].Width = 120;
            dgvDailySummary.Columns["SalesAmount"].DefaultCellStyle.Format = "N2";
            dgvDailySummary.Columns["SalesAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvDailySummary.Columns["SalesPaid"].HeaderText = "Sales Paid";
            dgvDailySummary.Columns["SalesPaid"].Width = 120;
            dgvDailySummary.Columns["SalesPaid"].DefaultCellStyle.Format = "N2";
            dgvDailySummary.Columns["SalesPaid"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvDailySummary.Columns["SalesOutstanding"].HeaderText = "Sales Outstanding";
            dgvDailySummary.Columns["SalesOutstanding"].Width = 120;
            dgvDailySummary.Columns["SalesOutstanding"].DefaultCellStyle.Format = "N2";
            dgvDailySummary.Columns["SalesOutstanding"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvDailySummary.Columns["PurchaseBills"].HeaderText = "Purchase Bills";
            dgvDailySummary.Columns["PurchaseBills"].Width = 100;
            dgvDailySummary.Columns["PurchaseBills"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvDailySummary.Columns["PurchaseAmount"].HeaderText = "Purchase Amount";
            dgvDailySummary.Columns["PurchaseAmount"].Width = 120;
            dgvDailySummary.Columns["PurchaseAmount"].DefaultCellStyle.Format = "N2";
            dgvDailySummary.Columns["PurchaseAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvDailySummary.Columns["PurchasePaid"].HeaderText = "Purchase Paid";
            dgvDailySummary.Columns["PurchasePaid"].Width = 120;
            dgvDailySummary.Columns["PurchasePaid"].DefaultCellStyle.Format = "N2";
            dgvDailySummary.Columns["PurchasePaid"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvDailySummary.Columns["PurchaseOutstanding"].HeaderText = "Purchase Outstanding";
            dgvDailySummary.Columns["PurchaseOutstanding"].Width = 120;
            dgvDailySummary.Columns["PurchaseOutstanding"].DefaultCellStyle.Format = "N2";
            dgvDailySummary.Columns["PurchaseOutstanding"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvDailySummary.Columns["NetCashFlow"].HeaderText = "Net Cash Flow";
            dgvDailySummary.Columns["NetCashFlow"].Width = 120;
            dgvDailySummary.Columns["NetCashFlow"].DefaultCellStyle.Format = "N2";
            dgvDailySummary.Columns["NetCashFlow"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            // Color coding for net cash flow
            dgvDailySummary.CellFormatting += (sender, e) =>
            {
                if (e.ColumnIndex == dgvDailySummary.Columns["NetCashFlow"].Index && e.Value != null)
                {
                    decimal cashFlow = Convert.ToDecimal(e.Value);
                    if (cashFlow > 0)
                        e.CellStyle.ForeColor = Color.Green;
                    else if (cashFlow < 0)
                        e.CellStyle.ForeColor = Color.Red;
                }
            };
        }

        private void CalculateTotals()
        {
            totalSales = 0;
            totalPurchases = 0;
            totalProfit = 0;

            foreach (DataGridViewRow row in dgvDailySummary.Rows)
            {
                if (row.Cells["SalesAmount"].Value != null)
                    totalSales += Convert.ToDecimal(row.Cells["SalesAmount"].Value);
                if (row.Cells["PurchaseAmount"].Value != null)
                    totalPurchases += Convert.ToDecimal(row.Cells["PurchaseAmount"].Value);
            }

            totalProfit = totalSales - totalPurchases;

            lblTotalSales.Text = $"Total Sales: ₹{totalSales:N2}";
            lblTotalPurchases.Text = $"Total Purchases: ₹{totalPurchases:N2}";
            lblTotalProfit.Text = $"Net Profit: ₹{totalProfit:N2}";

            // Color coding for profit
            if (totalProfit > 0)
                lblTotalProfit.ForeColor = Color.Green;
            else if (totalProfit < 0)
                lblTotalProfit.ForeColor = Color.Red;
            else
                lblTotalProfit.ForeColor = Color.Black;
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            LoadDailySummary();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                PrintDocument pd = new PrintDocument();
                pd.PrintPage += PrintPage;
                pd.Print();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error printing: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Font titleFont = new Font("Arial", 16, FontStyle.Bold);
            Font headerFont = new Font("Arial", 10, FontStyle.Bold);
            Font normalFont = new Font("Arial", 9);
            Brush brush = Brushes.Black;

            int yPos = 50;
            int leftMargin = 50;

            // Title
            string title = "Daily Summary Report";
            g.DrawString(title, titleFont, brush, leftMargin, yPos);
            yPos += 30;

            // Date range
            string dateRange = $"Period: {dtpFromDate.Value:dd/MM/yyyy} to {dtpToDate.Value:dd/MM/yyyy}";
            g.DrawString(dateRange, normalFont, brush, leftMargin, yPos);
            yPos += 20;

            // Summary
            g.DrawString($"Total Sales: ₹{totalSales:N2}", normalFont, brush, leftMargin, yPos);
            yPos += 15;
            g.DrawString($"Total Purchases: ₹{totalPurchases:N2}", normalFont, brush, leftMargin, yPos);
            yPos += 15;
            g.DrawString($"Net Profit: ₹{totalProfit:N2}", normalFont, brush, leftMargin, yPos);
            yPos += 25;

            // Print data grid
            if (dgvDailySummary.Rows.Count > 0)
            {
                // Headers
                string[] headers = { "Date", "Sales Bills", "Sales Amount", "Purchase Bills", "Purchase Amount", "Net Cash Flow" };
                int[] widths = { 80, 70, 90, 80, 90, 90 };
                int xPos = leftMargin;

                for (int i = 0; i < headers.Length; i++)
                {
                    g.DrawString(headers[i], headerFont, brush, xPos, yPos);
                    xPos += widths[i];
                }
                yPos += 20;

                // Data rows
                for (int row = 0; row < Math.Min(dgvDailySummary.Rows.Count, 20); row++)
                {
                    xPos = leftMargin;
                    var dataRow = dgvDailySummary.Rows[row];

                    g.DrawString(Convert.ToDateTime(dataRow.Cells["Date"].Value).ToString("dd/MM/yyyy"), normalFont, brush, xPos, yPos);
                    xPos += widths[0];

                    g.DrawString(dataRow.Cells["SalesBills"].Value.ToString(), normalFont, brush, xPos, yPos);
                    xPos += widths[1];

                    g.DrawString(Convert.ToDecimal(dataRow.Cells["SalesAmount"].Value).ToString("N2"), normalFont, brush, xPos, yPos);
                    xPos += widths[2];

                    g.DrawString(dataRow.Cells["PurchaseBills"].Value.ToString(), normalFont, brush, xPos, yPos);
                    xPos += widths[3];

                    g.DrawString(Convert.ToDecimal(dataRow.Cells["PurchaseAmount"].Value).ToString("N2"), normalFont, brush, xPos, yPos);
                    xPos += widths[4];

                    g.DrawString(Convert.ToDecimal(dataRow.Cells["NetCashFlow"].Value).ToString("N2"), normalFont, brush, xPos, yPos);

                    yPos += 15;
                }
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "CSV files (*.csv)|*.csv";
                sfd.FileName = $"DailySummary_{DateTime.Now:yyyyMMdd}.csv";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    StringBuilder sb = new StringBuilder();

                    // Headers
                    sb.AppendLine("Date,Sales Bills,Sales Amount,Sales Paid,Sales Outstanding,Purchase Bills,Purchase Amount,Purchase Paid,Purchase Outstanding,Net Cash Flow");

                    // Data
                    foreach (DataGridViewRow row in dgvDailySummary.Rows)
                    {
                        sb.AppendLine($"{Convert.ToDateTime(row.Cells["Date"].Value):dd/MM/yyyy}," +
                                     $"{row.Cells["SalesBills"].Value}," +
                                     $"{Convert.ToDecimal(row.Cells["SalesAmount"].Value):N2}," +
                                     $"{Convert.ToDecimal(row.Cells["SalesPaid"].Value):N2}," +
                                     $"{Convert.ToDecimal(row.Cells["SalesOutstanding"].Value):N2}," +
                                     $"{row.Cells["PurchaseBills"].Value}," +
                                     $"{Convert.ToDecimal(row.Cells["PurchaseAmount"].Value):N2}," +
                                     $"{Convert.ToDecimal(row.Cells["PurchasePaid"].Value):N2}," +
                                     $"{Convert.ToDecimal(row.Cells["PurchaseOutstanding"].Value):N2}," +
                                     $"{Convert.ToDecimal(row.Cells["NetCashFlow"].Value):N2}");
                    }

                    System.IO.File.WriteAllText(sfd.FileName, sb.ToString());
                    MessageBox.Show("Data exported successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exporting data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
} 