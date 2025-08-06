using RetailManagement.Database;
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;

namespace RetailManagement.UserForms
{
    public partial class ExpenseReport : Form
    {

        public ExpenseReport()
        {
            InitializeComponent();
            LoadExpenseReport();
        }

        private void LoadExpenseReport()
        {
            try
            {
                DateTime fromDate = dtpFromDate.Value.Date;
                DateTime toDate = dtpToDate.Value.Date;

                string query = @"
                    SELECT 
                        ExpenseDate,
                        Category,
                        Description,
                        Amount,
                        PaymentMethod,
                        Remarks
                    FROM Expenses 
                    WHERE ExpenseDate BETWEEN @FromDate AND @ToDate
                    ORDER BY ExpenseDate DESC";

                DataTable dt = DatabaseConnection.ExecuteQuery(query, new System.Data.SqlClient.SqlParameter[] 
                { 
                    new System.Data.SqlClient.SqlParameter("@FromDate", fromDate),
                    new System.Data.SqlClient.SqlParameter("@ToDate", toDate)
                });
                dgvExpenseReport.DataSource = dt;

                // Configure DataGridView
                dgvExpenseReport.Columns["ExpenseDate"].HeaderText = "Date";
                dgvExpenseReport.Columns["ExpenseDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
                dgvExpenseReport.Columns["Amount"].DefaultCellStyle.Format = "N2";
                dgvExpenseReport.Columns["Amount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                // Auto-size columns
                dgvExpenseReport.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

                CalculateTotals(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading expense report: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CalculateTotals(DataTable dt)
        {
            decimal totalAmount = 0;
            foreach (DataRow row in dt.Rows)
            {
                totalAmount += Convert.ToDecimal(row["Amount"]);
            }

            lblTotalExpenses.Text = "Total Expenses: ₹" + totalAmount.ToString("N2");
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            LoadExpenseReport();
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
            string title = "Expense Report";
            if (dtpFromDate.Value.Date == dtpToDate.Value.Date)
            {
                title += " - " + dtpFromDate.Value.ToString("dd/MM/yyyy");
            }
            else
            {
                title += " - " + dtpFromDate.Value.ToString("dd/MM/yyyy") + " to " + dtpToDate.Value.ToString("dd/MM/yyyy");
            }

            g.DrawString(title, titleFont, brush, leftMargin, yPos);
            yPos += 40;

            // Headers
            string[] headers = { "Date", "Category", "Description", "Amount", "Payment Method", "Remarks" };
            int[] columnWidths = { 80, 100, 200, 80, 100, 150 };
            int xPos = leftMargin;

            for (int i = 0; i < headers.Length; i++)
            {
                g.DrawString(headers[i], headerFont, brush, xPos, yPos);
                xPos += columnWidths[i];
            }
            yPos += 20;

            // Data
            DataTable dt = (DataTable)dgvExpenseReport.DataSource;
            decimal totalAmount = 0;

            foreach (DataRow row in dt.Rows)
            {
                if (yPos > e.PageBounds.Height - 100)
                {
                    e.HasMorePages = true;
                    return;
                }

                xPos = leftMargin;
                g.DrawString(Convert.ToDateTime(row["ExpenseDate"]).ToString("dd/MM/yyyy"), normalFont, brush, xPos, yPos);
                xPos += columnWidths[0];

                g.DrawString(row["Category"].ToString(), normalFont, brush, xPos, yPos);
                xPos += columnWidths[1];

                g.DrawString(row["Description"].ToString(), normalFont, brush, xPos, yPos);
                xPos += columnWidths[2];

                decimal amount = Convert.ToDecimal(row["Amount"]);
                totalAmount += amount;
                g.DrawString("₹" + amount.ToString("N2"), normalFont, brush, xPos, yPos);
                xPos += columnWidths[3];

                g.DrawString(row["PaymentMethod"].ToString(), normalFont, brush, xPos, yPos);
                xPos += columnWidths[4];

                g.DrawString(row["Remarks"].ToString(), normalFont, brush, xPos, yPos);

                yPos += 15;
            }

            // Total
            yPos += 10;
            g.DrawString("Total Expenses: ₹" + totalAmount.ToString("N2"), headerFont, brush, leftMargin, yPos);
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "CSV files (*.csv)|*.csv";
                sfd.FileName = "ExpenseReport_" + DateTime.Now.ToString("yyyyMMdd") + ".csv";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    DataTable dt = (DataTable)dgvExpenseReport.DataSource;
                    StringBuilder sb = new StringBuilder();

                    // Headers
                    sb.AppendLine("Date,Category,Description,Amount,Payment Method,Remarks");

                    // Data
                    foreach (DataRow row in dt.Rows)
                    {
                        sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5}",
                            Convert.ToDateTime(row["ExpenseDate"]).ToString("dd/MM/yyyy"),
                            row["Category"].ToString().Replace(",", ";"),
                            row["Description"].ToString().Replace(",", ";"),
                            Convert.ToDecimal(row["Amount"]).ToString("N2"),
                            row["PaymentMethod"].ToString().Replace(",", ";"),
                            row["Remarks"].ToString().Replace(",", ";")));
                    }

                    System.IO.File.WriteAllText(sfd.FileName, sb.ToString());
                    MessageBox.Show("Report exported successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exporting report: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
} 