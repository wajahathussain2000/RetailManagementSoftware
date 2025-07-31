using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RetailManagement.Database;

namespace RetailManagement.UserForms
{
    public partial class ProfitAndLoss : Form
    {
        public ProfitAndLoss()
        {
            InitializeComponent();
            SetDefaultDateRange();
            GenerateProfitAndLoss();
        }

        private void SetDefaultDateRange()
        {
            dateTimePicker1.Value = DateTime.Now.AddMonths(-1); // Last month
            dateTimePicker2.Value = DateTime.Now;
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            if (dateTimePicker1.Value > dateTimePicker2.Value)
            {
                MessageBox.Show("From date cannot be greater than To date.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            GenerateProfitAndLoss();
        }

        private void GenerateProfitAndLoss()
        {
            try
            {
                DateTime fromDate = dateTimePicker1.Value.Date;
                DateTime toDate = dateTimePicker2.Value.Date.AddDays(1).AddSeconds(-1);

                // Calculate Sales Revenue
                decimal salesRevenue = CalculateSalesRevenue(fromDate, toDate);

                // Calculate Cost of Goods Sold
                decimal costOfGoodsSold = CalculateCostOfGoodsSold(fromDate, toDate);

                // Calculate Gross Profit
                decimal grossProfit = salesRevenue - costOfGoodsSold;

                // Calculate Operating Expenses (simplified)
                decimal operatingExpenses = CalculateOperatingExpenses(fromDate, toDate);

                // Calculate Net Profit
                decimal netProfit = grossProfit - operatingExpenses;

                // Display results
                DisplayResults(salesRevenue, costOfGoodsSold, grossProfit, operatingExpenses, netProfit);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating Profit & Loss: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private decimal CalculateSalesRevenue(DateTime fromDate, DateTime toDate)
        {
            string query = @"SELECT ISNULL(SUM(NetAmount), 0) 
                           FROM Sales 
                           WHERE SaleDate BETWEEN @FromDate AND @ToDate 
                           AND IsActive = 1";

            SqlParameter[] parameters = {
                new SqlParameter("@FromDate", fromDate),
                new SqlParameter("@ToDate", toDate)
            };

            object result = DatabaseConnection.ExecuteScalar(query, parameters);
            return Convert.ToDecimal(result);
        }

        private decimal CalculateCostOfGoodsSold(DateTime fromDate, DateTime toDate)
        {
            // This is a simplified calculation
            // In a real system, you would track actual cost of goods sold
            string query = @"SELECT ISNULL(SUM(si.Quantity * i.Price * 0.7), 0) 
                           FROM Sales s
                           INNER JOIN SaleItems si ON s.SaleID = si.SaleID
                           INNER JOIN Items i ON si.ItemID = i.ItemID
                           WHERE s.SaleDate BETWEEN @FromDate AND @ToDate 
                           AND s.IsActive = 1";

            SqlParameter[] parameters = {
                new SqlParameter("@FromDate", fromDate),
                new SqlParameter("@ToDate", toDate)
            };

            object result = DatabaseConnection.ExecuteScalar(query, parameters);
            return Convert.ToDecimal(result);
        }

        private decimal CalculateOperatingExpenses(DateTime fromDate, DateTime toDate)
        {
            // This is a simplified calculation
            // In a real system, you would have an expenses table
            // For now, we'll use a percentage of sales as operating expenses
            decimal salesRevenue = CalculateSalesRevenue(fromDate, toDate);
            return salesRevenue * 0.15m; // 15% of sales as operating expenses
        }

        private void DisplayResults(decimal salesRevenue, decimal costOfGoodsSold, decimal grossProfit, decimal operatingExpenses, decimal netProfit)
        {
            // Update labels with results
            // You can add labels to display these values
            string report = $"Profit & Loss Statement\n" +
                          $"Period: {dateTimePicker1.Value:dd/MM/yyyy} - {dateTimePicker2.Value:dd/MM/yyyy}\n\n" +
                          $"Sales Revenue: ${salesRevenue:N2}\n" +
                          $"Cost of Goods Sold: ${costOfGoodsSold:N2}\n" +
                          $"Gross Profit: ${grossProfit:N2}\n" +
                          $"Operating Expenses: ${operatingExpenses:N2}\n" +
                          $"Net Profit: ${netProfit:N2}\n\n" +
                          $"Gross Profit Margin: {(grossProfit > 0 ? (grossProfit / salesRevenue * 100) : 0):N1}%\n" +
                          $"Net Profit Margin: {(netProfit > 0 ? (netProfit / salesRevenue * 100) : 0):N1}%";

            MessageBox.Show(report, "Profit & Loss Statement", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                PrintDocument pd = new PrintDocument();
                pd.PrintPage += PrintProfitAndLoss;
                pd.Print();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error printing: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PrintProfitAndLoss(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Font titleFont = new Font("Arial", 16, FontStyle.Bold);
            Font headerFont = new Font("Arial", 12, FontStyle.Bold);
            Font dataFont = new Font("Arial", 10);

            int yPos = 50;
            g.DrawString("Profit & Loss Statement", titleFont, Brushes.Black, 50, yPos);
            yPos += 30;
            g.DrawString($"Period: {dateTimePicker1.Value:dd/MM/yyyy} - {dateTimePicker2.Value:dd/MM/yyyy}", dataFont, Brushes.Black, 50, yPos);
            yPos += 20;
            g.DrawString($"Generated on: {DateTime.Now:dd/MM/yyyy HH:mm}", dataFont, Brushes.Black, 50, yPos);
            yPos += 40;

            // Calculate values for printing
            DateTime fromDate = dateTimePicker1.Value.Date;
            DateTime toDate = dateTimePicker2.Value.Date.AddDays(1).AddSeconds(-1);
            decimal salesRevenue = CalculateSalesRevenue(fromDate, toDate);
            decimal costOfGoodsSold = CalculateCostOfGoodsSold(fromDate, toDate);
            decimal grossProfit = salesRevenue - costOfGoodsSold;
            decimal operatingExpenses = CalculateOperatingExpenses(fromDate, toDate);
            decimal netProfit = grossProfit - operatingExpenses;

            // Print statement
            g.DrawString("Sales Revenue", headerFont, Brushes.Black, 50, yPos);
            g.DrawString(salesRevenue.ToString("N2"), dataFont, Brushes.Black, 300, yPos);
            yPos += 25;

            g.DrawString("Cost of Goods Sold", headerFont, Brushes.Black, 50, yPos);
            g.DrawString(costOfGoodsSold.ToString("N2"), dataFont, Brushes.Black, 300, yPos);
            yPos += 25;

            g.DrawString("Gross Profit", headerFont, Brushes.Black, 50, yPos);
            g.DrawString(grossProfit.ToString("N2"), dataFont, Brushes.Black, 300, yPos);
            yPos += 30;

            g.DrawString("Operating Expenses", headerFont, Brushes.Black, 50, yPos);
            g.DrawString(operatingExpenses.ToString("N2"), dataFont, Brushes.Black, 300, yPos);
            yPos += 30;

            g.DrawString("Net Profit", headerFont, Brushes.Black, 50, yPos);
            g.DrawString(netProfit.ToString("N2"), dataFont, Brushes.Black, 300, yPos);
            yPos += 40;

            // Print margins
            g.DrawString($"Gross Profit Margin: {(grossProfit > 0 ? (grossProfit / salesRevenue * 100) : 0):N1}%", dataFont, Brushes.Black, 50, yPos);
            yPos += 20;
            g.DrawString($"Net Profit Margin: {(netProfit > 0 ? (netProfit / salesRevenue * 100) : 0):N1}%", dataFont, Brushes.Black, 50, yPos);
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                saveDialog.FileName = $"ProfitAndLoss_{DateTime.Now:yyyyMMdd}.csv";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    ExportToCSV(saveDialog.FileName);
                    MessageBox.Show("Profit & Loss report exported successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exporting: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportToCSV(string fileName)
        {
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fileName))
            {
                DateTime fromDate = dateTimePicker1.Value.Date;
                DateTime toDate = dateTimePicker2.Value.Date.AddDays(1).AddSeconds(-1);
                decimal salesRevenue = CalculateSalesRevenue(fromDate, toDate);
                decimal costOfGoodsSold = CalculateCostOfGoodsSold(fromDate, toDate);
                decimal grossProfit = salesRevenue - costOfGoodsSold;
                decimal operatingExpenses = CalculateOperatingExpenses(fromDate, toDate);
                decimal netProfit = grossProfit - operatingExpenses;

                sw.WriteLine("Profit & Loss Statement");
                sw.WriteLine($"Period: {dateTimePicker1.Value:dd/MM/yyyy} - {dateTimePicker2.Value:dd/MM/yyyy}");
                sw.WriteLine($"Generated on: {DateTime.Now:dd/MM/yyyy HH:mm}");
                sw.WriteLine();
                sw.WriteLine("Item,Amount");
                sw.WriteLine($"Sales Revenue,{salesRevenue:N2}");
                sw.WriteLine($"Cost of Goods Sold,{costOfGoodsSold:N2}");
                sw.WriteLine($"Gross Profit,{grossProfit:N2}");
                sw.WriteLine($"Operating Expenses,{operatingExpenses:N2}");
                sw.WriteLine($"Net Profit,{netProfit:N2}");
                sw.WriteLine();
                sw.WriteLine($"Gross Profit Margin,{(grossProfit > 0 ? (grossProfit / salesRevenue * 100) : 0):N1}%");
                sw.WriteLine($"Net Profit Margin,{(netProfit > 0 ? (netProfit / salesRevenue * 100) : 0):N1}%");
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
