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
    public partial class SalesReport : Form
    {
        public SalesReport()
        {
            InitializeComponent();
            SetupDataGridView();
            SetDefaultDateRange();
        }

        private void SetDefaultDateRange()
        {
            dateTimePicker1.Value = DateTime.Now.AddDays(-30); // Last 30 days
            dateTimePicker2.Value = DateTime.Now;
        }

        private void SetupDataGridView()
        {
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.Columns.Clear();

            dataGridView1.Columns.Add("SaleID", "Sale ID");
            dataGridView1.Columns.Add("BillNumber", "Bill Number");
            dataGridView1.Columns.Add("CustomerName", "Customer Name");
            dataGridView1.Columns.Add("SaleDate", "Sale Date");
            dataGridView1.Columns.Add("TotalAmount", "Total Amount");
            dataGridView1.Columns.Add("Discount", "Discount");
            dataGridView1.Columns.Add("NetAmount", "Net Amount");
            dataGridView1.Columns.Add("PaymentMethod", "Payment Method");

            dataGridView1.Columns["SaleID"].DataPropertyName = "SaleID";
            dataGridView1.Columns["BillNumber"].DataPropertyName = "BillNumber";
            dataGridView1.Columns["CustomerName"].DataPropertyName = "CustomerName";
            dataGridView1.Columns["SaleDate"].DataPropertyName = "SaleDate";
            dataGridView1.Columns["TotalAmount"].DataPropertyName = "TotalAmount";
            dataGridView1.Columns["Discount"].DataPropertyName = "Discount";
            dataGridView1.Columns["NetAmount"].DataPropertyName = "NetAmount";
            dataGridView1.Columns["PaymentMethod"].DataPropertyName = "PaymentMethod";
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            if (dateTimePicker1.Value > dateTimePicker2.Value)
            {
                MessageBox.Show("From date cannot be greater than To date.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                GenerateSalesReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating report: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerateSalesReport()
        {
            string query = @"SELECT 
                            s.SaleID,
                            s.BillNumber,
                            c.CustomerName,
                            s.SaleDate,
                            s.TotalAmount,
                            s.Discount,
                            s.NetAmount,
                            s.PaymentMethod
                           FROM Sales s
                           INNER JOIN Customers c ON s.CustomerID = c.CustomerID
                           WHERE s.IsActive = 1 
                           AND s.SaleDate BETWEEN @FromDate AND @ToDate
                           ORDER BY s.SaleDate DESC";

            SqlParameter[] parameters = {
                new SqlParameter("@FromDate", dateTimePicker1.Value.Date),
                new SqlParameter("@ToDate", dateTimePicker2.Value.Date.AddDays(1).AddSeconds(-1))
            };

            DataTable dt = DatabaseConnection.ExecuteQuery(query, parameters);
            dataGridView1.DataSource = dt;

            // Display summary
            DisplaySummary(dt);
        }

        private void DisplaySummary(DataTable dt)
        {
            decimal totalSales = 0;
            decimal totalDiscount = 0;
            decimal totalNetAmount = 0;
            int totalTransactions = dt.Rows.Count;

            foreach (DataRow row in dt.Rows)
            {
                totalSales += Convert.ToDecimal(row["TotalAmount"]);
                totalDiscount += Convert.ToDecimal(row["Discount"]);
                totalNetAmount += Convert.ToDecimal(row["NetAmount"]);
            }

            // You can add labels to display these summary values
            // For now, we'll show them in a message box
            string summary = $"Report Summary:\n" +
                           $"Total Transactions: {totalTransactions}\n" +
                           $"Total Sales: ${totalSales:N2}\n" +
                           $"Total Discount: ${totalDiscount:N2}\n" +
                           $"Net Amount: ${totalNetAmount:N2}";

            MessageBox.Show(summary, "Sales Report Summary", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.Rows.Count > 0)
                {
                    PrintDocument pd = new PrintDocument();
                    pd.PrintPage += PrintSalesReport;
                    pd.Print();
                }
                else
                {
                    MessageBox.Show("No data to print.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error printing: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PrintSalesReport(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Font titleFont = new Font("Arial", 16, FontStyle.Bold);
            Font headerFont = new Font("Arial", 10, FontStyle.Bold);
            Font dataFont = new Font("Arial", 9);

            int yPos = 50;
            g.DrawString("Sales Report", titleFont, Brushes.Black, 50, yPos);
            yPos += 30;
            g.DrawString($"Period: {dateTimePicker1.Value:dd/MM/yyyy} - {dateTimePicker2.Value:dd/MM/yyyy}", dataFont, Brushes.Black, 50, yPos);
            yPos += 20;
            g.DrawString($"Generated on: {DateTime.Now:dd/MM/yyyy HH:mm}", dataFont, Brushes.Black, 50, yPos);
            yPos += 30;

            // Print headers
            g.DrawString("Bill No", headerFont, Brushes.Black, 50, yPos);
            g.DrawString("Customer", headerFont, Brushes.Black, 120, yPos);
            g.DrawString("Date", headerFont, Brushes.Black, 250, yPos);
            g.DrawString("Net Amount", headerFont, Brushes.Black, 350, yPos);
            yPos += 20;

            // Print data
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (yPos > e.PageBounds.Height - 100) break;

                g.DrawString(row.Cells["BillNumber"].Value.ToString(), dataFont, Brushes.Black, 50, yPos);
                g.DrawString(row.Cells["CustomerName"].Value.ToString(), dataFont, Brushes.Black, 120, yPos);
                g.DrawString(Convert.ToDateTime(row.Cells["SaleDate"].Value).ToString("dd/MM/yyyy"), dataFont, Brushes.Black, 250, yPos);
                g.DrawString(Convert.ToDecimal(row.Cells["NetAmount"].Value).ToString("N2"), dataFont, Brushes.Black, 350, yPos);
                yPos += 15;
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.Rows.Count > 0)
                {
                    SaveFileDialog saveDialog = new SaveFileDialog();
                    saveDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                    saveDialog.FileName = $"SalesReport_{DateTime.Now:yyyyMMdd}.csv";

                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        ExportToCSV(saveDialog.FileName);
                        MessageBox.Show("Report exported successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("No data to export.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                // Write headers
                for (int i = 0; i < dataGridView1.Columns.Count; i++)
                {
                    sw.Write(dataGridView1.Columns[i].HeaderText);
                    if (i < dataGridView1.Columns.Count - 1)
                        sw.Write(",");
                }
                sw.WriteLine();

                // Write data
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    for (int i = 0; i < dataGridView1.Columns.Count; i++)
                    {
                        sw.Write(row.Cells[i].Value?.ToString() ?? "");
                        if (i < dataGridView1.Columns.Count - 1)
                            sw.Write(",");
                    }
                    sw.WriteLine();
                }
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            // Date range validation
            if (dateTimePicker1.Value > dateTimePicker2.Value)
            {
                dateTimePicker2.Value = dateTimePicker1.Value;
            }
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            // Date range validation
            if (dateTimePicker2.Value < dateTimePicker1.Value)
            {
                dateTimePicker1.Value = dateTimePicker2.Value;
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void pbExit_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
} 