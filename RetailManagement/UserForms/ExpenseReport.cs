using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using RetailManagement.Database;
using System.Drawing.Printing;

namespace RetailManagement.UserForms
{
    public partial class ExpenseReport : Form
    {
        private DataTable expenseData;

        public ExpenseReport()
        {
            InitializeComponent();
            InitializeForm();
        }

        private void InitializeForm()
        {
            // Set default date range (current month)
            dtpFromDate.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dtpToDate.Value = DateTime.Now;

            // Load expense categories
            LoadExpenseCategories();

            // Load initial data
            LoadExpenseReport();
        }

        private void LoadExpenseCategories()
        {
            try
            {
                string query = "SELECT DISTINCT Category FROM Expenses WHERE IsActive = 1 ORDER BY Category";
                DataTable categories = DatabaseConnection.ExecuteQuery(query);

                cmbCategory.Items.Clear();
                cmbCategory.Items.Add("All Categories");

                foreach (DataRow row in categories.Rows)
                {
                    cmbCategory.Items.Add(row["Category"].ToString());
                }

                cmbCategory.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading categories: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadExpenseReport()
        {
            try
            {
                string query = @"SELECT 
                                    ExpenseID,
                                    ExpenseDate,
                                    Category,
                                    Description,
                                    Amount,
                                    PaymentMethod,
                                    Remarks
                                FROM Expenses 
                                WHERE IsActive = 1 
                                AND ExpenseDate BETWEEN @FromDate AND @ToDate";

                // Add category filter if not "All Categories"
                if (cmbCategory.SelectedIndex > 0)
                {
                    query += " AND Category = @Category";
                }

                query += " ORDER BY ExpenseDate DESC";

                SqlParameter[] parameters = {
                    new SqlParameter("@FromDate", dtpFromDate.Value.Date),
                    new SqlParameter("@ToDate", dtpToDate.Value.Date)
                };

                if (cmbCategory.SelectedIndex > 0)
                {
                    var categoryParam = new SqlParameter("@Category", cmbCategory.Text);
                    var newParams = new SqlParameter[parameters.Length + 1];
                    parameters.CopyTo(newParams, 0);
                    newParams[parameters.Length] = categoryParam;
                    parameters = newParams;
                }

                expenseData = DatabaseConnection.ExecuteQuery(query, parameters);
                ShowExpenseData();
                CalculateTotals();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading expense report: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowExpenseData()
        {
            dataGridView1.DataSource = expenseData;

            // Format columns
            if (dataGridView1.Columns.Count > 0)
            {
                dataGridView1.Columns["ExpenseID"].HeaderText = "ID";
                dataGridView1.Columns["ExpenseID"].Width = 60;

                dataGridView1.Columns["ExpenseDate"].HeaderText = "Date";
                dataGridView1.Columns["ExpenseDate"].Width = 100;
                dataGridView1.Columns["ExpenseDate"].DefaultCellStyle.Format = "dd/MM/yyyy";

                dataGridView1.Columns["Category"].HeaderText = "Category";
                dataGridView1.Columns["Category"].Width = 120;

                dataGridView1.Columns["Description"].HeaderText = "Description";
                dataGridView1.Columns["Description"].Width = 200;

                dataGridView1.Columns["Amount"].HeaderText = "Amount";
                dataGridView1.Columns["Amount"].Width = 100;
                dataGridView1.Columns["Amount"].DefaultCellStyle.Format = "N2";
                dataGridView1.Columns["Amount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                dataGridView1.Columns["PaymentMethod"].HeaderText = "Payment Method";
                dataGridView1.Columns["PaymentMethod"].Width = 120;

                dataGridView1.Columns["Remarks"].HeaderText = "Remarks";
                dataGridView1.Columns["Remarks"].Width = 150;
            }
        }

        private void CalculateTotals()
        {
            decimal totalAmount = 0;
            int totalCount = 0;

            if (expenseData != null && expenseData.Rows.Count > 0)
            {
                foreach (DataRow row in expenseData.Rows)
                {
                    totalAmount += Convert.ToDecimal(row["Amount"]);
                }
                totalCount = expenseData.Rows.Count;
            }

            lblTotalAmount.Text = $"Total Amount: ₹{totalAmount:N2}";
            lblTotalCount.Text = $"Total Expenses: {totalCount}";
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadExpenseReport();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            // Reset to current month
            dtpFromDate.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dtpToDate.Value = DateTime.Now;
            cmbCategory.SelectedIndex = 0;
            LoadExpenseReport();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            PrintExpenseReport();
        }

        private void PrintExpenseReport()
        {
            try
            {
                PrintDialog printDialog = new PrintDialog();
                PrintDocument printDocument = new PrintDocument();

                printDocument.PrintPage += (sender, e) =>
                {
                    Graphics graphics = e.Graphics;
                    Font titleFont = new Font("Arial", 16, FontStyle.Bold);
                    Font headerFont = new Font("Arial", 10, FontStyle.Bold);
                    Font normalFont = new Font("Arial", 9);
                    Font smallFont = new Font("Arial", 8);

                    int yPos = 50;
                    int leftMargin = 50;
                    int topMargin = 50;

                    // Print title
                    string title = "EXPENSE REPORT";
                    graphics.DrawString(title, titleFont, Brushes.Black, leftMargin, yPos);
                    yPos += 30;

                    // Print date range
                    string dateRange = $"Period: {dtpFromDate.Value:dd/MM/yyyy} to {dtpToDate.Value:dd/MM/yyyy}";
                    graphics.DrawString(dateRange, normalFont, Brushes.Black, leftMargin, yPos);
                    yPos += 20;

                    // Print category filter
                    if (cmbCategory.SelectedIndex > 0)
                    {
                        string categoryFilter = $"Category: {cmbCategory.Text}";
                        graphics.DrawString(categoryFilter, normalFont, Brushes.Black, leftMargin, yPos);
                        yPos += 20;
                    }

                    // Print generated date
                    string generatedDate = $"Generated on: {DateTime.Now:dd/MM/yyyy HH:mm}";
                    graphics.DrawString(generatedDate, smallFont, Brushes.Black, leftMargin, yPos);
                    yPos += 30;

                    // Print headers
                    string[] headers = { "Date", "Category", "Description", "Amount", "Payment Method" };
                    int[] columnWidths = { 80, 100, 200, 80, 100 };
                    int xPos = leftMargin;

                    for (int i = 0; i < headers.Length; i++)
                    {
                        graphics.DrawString(headers[i], headerFont, Brushes.Black, xPos, yPos);
                        xPos += columnWidths[i];
                    }
                    yPos += 20;

                    // Print data
                    if (expenseData != null && expenseData.Rows.Count > 0)
                    {
                        decimal totalAmount = 0;

                        foreach (DataRow row in expenseData.Rows)
                        {
                            if (yPos > e.MarginBounds.Bottom - 100) // Check if page break needed
                            {
                                e.HasMorePages = true;
                                return;
                            }

                            xPos = leftMargin;
                            graphics.DrawString(Convert.ToDateTime(row["ExpenseDate"]).ToString("dd/MM/yyyy"), normalFont, Brushes.Black, xPos, yPos);
                            xPos += columnWidths[0];

                            graphics.DrawString(row["Category"].ToString(), normalFont, Brushes.Black, xPos, yPos);
                            xPos += columnWidths[1];

                            graphics.DrawString(row["Description"].ToString(), normalFont, Brushes.Black, xPos, yPos);
                            xPos += columnWidths[2];

                            decimal amount = Convert.ToDecimal(row["Amount"]);
                            totalAmount += amount;
                            graphics.DrawString(amount.ToString("N2"), normalFont, Brushes.Black, xPos, yPos);
                            xPos += columnWidths[3];

                            graphics.DrawString(row["PaymentMethod"].ToString(), normalFont, Brushes.Black, xPos, yPos);

                            yPos += 15;
                        }

                        // Print total
                        yPos += 10;
                        graphics.DrawString("─".PadRight(50, '─'), headerFont, Brushes.Black, leftMargin, yPos);
                        yPos += 20;
                        graphics.DrawString($"Total Amount: ₹{totalAmount:N2}", headerFont, Brushes.Black, leftMargin, yPos);
                        yPos += 15;
                        graphics.DrawString($"Total Expenses: {expenseData.Rows.Count}", normalFont, Brushes.Black, leftMargin, yPos);
                    }
                    else
                    {
                        graphics.DrawString("No expenses found for the selected criteria.", normalFont, Brushes.Black, leftMargin, yPos);
                    }
                };

                printDialog.Document = printDocument;
                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    printDocument.Print();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error printing report: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            ExportToCSV();
        }

        private void ExportToCSV()
        {
            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "CSV files (*.csv)|*.csv";
                saveDialog.FileName = $"ExpenseReport_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    using (StreamWriter writer = new StreamWriter(saveDialog.FileName))
                    {
                        // Write header
                        writer.WriteLine("Expense Report");
                        writer.WriteLine($"Period: {dtpFromDate.Value:dd/MM/yyyy} to {dtpToDate.Value:dd/MM/yyyy}");
                        if (cmbCategory.SelectedIndex > 0)
                        {
                            writer.WriteLine($"Category: {cmbCategory.Text}");
                        }
                        writer.WriteLine($"Generated on: {DateTime.Now:dd/MM/yyyy HH:mm}");
                        writer.WriteLine();

                        // Write column headers
                        writer.WriteLine("Date,Category,Description,Amount,Payment Method,Remarks");

                        // Write data
                        if (expenseData != null && expenseData.Rows.Count > 0)
                        {
                            decimal totalAmount = 0;

                            foreach (DataRow row in expenseData.Rows)
                            {
                                string date = Convert.ToDateTime(row["ExpenseDate"]).ToString("dd/MM/yyyy");
                                string category = row["Category"].ToString().Replace(",", ";");
                                string description = row["Description"].ToString().Replace(",", ";");
                                decimal amount = Convert.ToDecimal(row["Amount"]);
                                string paymentMethod = row["PaymentMethod"].ToString().Replace(",", ";");
                                string remarks = row["Remarks"].ToString().Replace(",", ";");

                                writer.WriteLine($"{date},{category},{description},{amount:N2},{paymentMethod},{remarks}");
                                totalAmount += amount;
                            }

                            writer.WriteLine();
                            writer.WriteLine($"Total Amount,{totalAmount:N2}");
                            writer.WriteLine($"Total Expenses,{expenseData.Rows.Count}");
                        }
                    }

                    MessageBox.Show("Report exported successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exporting report: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cmbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadExpenseReport();
        }
    }
} 