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
    public partial class GSTReport : Form
    {
        private DataTable gstData;

        public GSTReport()
        {
            InitializeComponent();
            InitializeForm();
        }

        private void InitializeForm()
        {
            // Set default date range (current month)
            dtpFromDate.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dtpToDate.Value = DateTime.Now;

            // Load GST categories
            LoadGSTCategories();

            // Load initial data
            LoadGSTReport();
        }

        private void LoadGSTCategories()
        {
            try
            {
                string query = "SELECT DISTINCT Category FROM GSTSetup WHERE IsActive = 1 ORDER BY Category";
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
                MessageBox.Show("Error loading GST categories: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadGSTReport()
        {
            try
            {
                string query = @"SELECT 
                                    'Sales' as TransactionType,
                                    s.SaleID as TransactionID,
                                    s.BillNumber as ReferenceNumber,
                                    s.SaleDate as TransactionDate,
                                    c.CustomerName as PartyName,
                                    s.TotalAmount as GrossAmount,
                                    s.GSTAmount as GSTAmount,
                                    s.NetAmount as NetAmount,
                                    gs.Category as GSTCategory,
                                    gs.GSTPercentage as GSTPercentage,
                                    gs.HSNCode as HSNCode
                                FROM Sales s
                                INNER JOIN Customers c ON s.CustomerID = c.CustomerID
                                INNER JOIN GSTSetup gs ON s.GSTCategory = gs.Category
                                WHERE s.IsActive = 1 
                                AND s.SaleDate BETWEEN @FromDate AND @ToDate
                                AND s.GSTAmount > 0";

                // Add category filter if not "All Categories"
                if (cmbCategory.SelectedIndex > 0)
                {
                    query += " AND s.GSTCategory = @Category";
                }

                query += @" UNION ALL
                            SELECT 
                                'Purchase' as TransactionType,
                                p.PurchaseID as TransactionID,
                                p.PurchaseNumber as ReferenceNumber,
                                p.PurchaseDate as TransactionDate,
                                c.CompanyName as PartyName,
                                p.TotalAmount as GrossAmount,
                                p.GSTAmount as GSTAmount,
                                p.NetAmount as NetAmount,
                                gs.Category as GSTCategory,
                                gs.GSTPercentage as GSTPercentage,
                                gs.HSNCode as HSNCode
                            FROM Purchases p
                            INNER JOIN Companies c ON p.CompanyID = c.CompanyID
                            INNER JOIN GSTSetup gs ON p.GSTCategory = gs.Category
                            WHERE p.IsActive = 1 
                            AND p.PurchaseDate BETWEEN @FromDate AND @ToDate
                            AND p.GSTAmount > 0";

                // Add category filter for purchases
                if (cmbCategory.SelectedIndex > 0)
                {
                    query += " AND p.GSTCategory = @Category";
                }

                query += " ORDER BY TransactionDate DESC, TransactionID DESC";

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

                gstData = DatabaseConnection.ExecuteQuery(query, parameters);
                ShowGSTData();
                CalculateTotals();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading GST report: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowGSTData()
        {
            dataGridView1.DataSource = gstData;

            // Format columns
            if (dataGridView1.Columns.Count > 0)
            {
                dataGridView1.Columns["TransactionType"].HeaderText = "Type";
                dataGridView1.Columns["TransactionType"].Width = 80;

                dataGridView1.Columns["TransactionID"].HeaderText = "ID";
                dataGridView1.Columns["TransactionID"].Width = 60;

                dataGridView1.Columns["ReferenceNumber"].HeaderText = "Reference";
                dataGridView1.Columns["ReferenceNumber"].Width = 120;

                dataGridView1.Columns["TransactionDate"].HeaderText = "Date";
                dataGridView1.Columns["TransactionDate"].Width = 100;
                dataGridView1.Columns["TransactionDate"].DefaultCellStyle.Format = "dd/MM/yyyy";

                dataGridView1.Columns["PartyName"].HeaderText = "Party Name";
                dataGridView1.Columns["PartyName"].Width = 150;

                dataGridView1.Columns["GrossAmount"].HeaderText = "Gross Amount";
                dataGridView1.Columns["GrossAmount"].Width = 100;
                dataGridView1.Columns["GrossAmount"].DefaultCellStyle.Format = "N2";
                dataGridView1.Columns["GrossAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                dataGridView1.Columns["GSTAmount"].HeaderText = "GST Amount";
                dataGridView1.Columns["GSTAmount"].Width = 100;
                dataGridView1.Columns["GSTAmount"].DefaultCellStyle.Format = "N2";
                dataGridView1.Columns["GSTAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                dataGridView1.Columns["NetAmount"].HeaderText = "Net Amount";
                dataGridView1.Columns["NetAmount"].Width = 100;
                dataGridView1.Columns["NetAmount"].DefaultCellStyle.Format = "N2";
                dataGridView1.Columns["NetAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                dataGridView1.Columns["GSTCategory"].HeaderText = "GST Category";
                dataGridView1.Columns["GSTCategory"].Width = 100;

                dataGridView1.Columns["GSTPercentage"].HeaderText = "GST %";
                dataGridView1.Columns["GSTPercentage"].Width = 80;
                dataGridView1.Columns["GSTPercentage"].DefaultCellStyle.Format = "N2";
                dataGridView1.Columns["GSTPercentage"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                dataGridView1.Columns["HSNCode"].HeaderText = "HSN Code";
                dataGridView1.Columns["HSNCode"].Width = 100;

                // Color coding for transaction types
                dataGridView1.CellFormatting += (sender, e) =>
                {
                    if (e.Value != null && e.ColumnIndex == dataGridView1.Columns["TransactionType"].Index)
                    {
                        string transactionType = e.Value.ToString();
                        switch (transactionType)
                        {
                            case "Sales":
                                e.CellStyle.ForeColor = Color.DarkGreen;
                                break;
                            case "Purchase":
                                e.CellStyle.ForeColor = Color.DarkBlue;
                                break;
                        }
                    }
                };
            }
        }

        private void CalculateTotals()
        {
            decimal totalSalesGross = 0;
            decimal totalSalesGST = 0;
            decimal totalSalesNet = 0;
            decimal totalPurchaseGross = 0;
            decimal totalPurchaseGST = 0;
            decimal totalPurchaseNet = 0;

            if (gstData != null && gstData.Rows.Count > 0)
            {
                foreach (DataRow row in gstData.Rows)
                {
                    string transactionType = row["TransactionType"].ToString();
                    decimal grossAmount = Convert.ToDecimal(row["GrossAmount"]);
                    decimal gstAmount = Convert.ToDecimal(row["GSTAmount"]);
                    decimal netAmount = Convert.ToDecimal(row["NetAmount"]);

                    if (transactionType == "Sales")
                    {
                        totalSalesGross += grossAmount;
                        totalSalesGST += gstAmount;
                        totalSalesNet += netAmount;
                    }
                    else if (transactionType == "Purchase")
                    {
                        totalPurchaseGross += grossAmount;
                        totalPurchaseGST += gstAmount;
                        totalPurchaseNet += netAmount;
                    }
                }
            }

            // Calculate GST liability
            decimal gstLiability = totalSalesGST - totalPurchaseGST;

            lblSalesGross.Text = $"Sales Gross: ₹{totalSalesGross:N2}";
            lblSalesGST.Text = $"Sales GST: ₹{totalSalesGST:N2}";
            lblSalesNet.Text = $"Sales Net: ₹{totalSalesNet:N2}";
            lblPurchaseGross.Text = $"Purchase Gross: ₹{totalPurchaseGross:N2}";
            lblPurchaseGST.Text = $"Purchase GST: ₹{totalPurchaseGST:N2}";
            lblPurchaseNet.Text = $"Purchase Net: ₹{totalPurchaseNet:N2}";
            lblGSTLiability.Text = $"GST Liability: ₹{gstLiability:N2}";

            // Color code the GST liability
            if (gstLiability > 0)
            {
                lblGSTLiability.ForeColor = Color.DarkRed; // We owe GST
            }
            else if (gstLiability < 0)
            {
                lblGSTLiability.ForeColor = Color.DarkGreen; // We have GST credit
            }
            else
            {
                lblGSTLiability.ForeColor = Color.Black; // Balanced
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadGSTReport();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            // Reset to current month
            dtpFromDate.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dtpToDate.Value = DateTime.Now;
            cmbCategory.SelectedIndex = 0;
            LoadGSTReport();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            PrintGSTReport();
        }

        private void PrintGSTReport()
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

                    // Print title
                    string title = "GST REPORT";
                    graphics.DrawString(title, titleFont, Brushes.Black, leftMargin, yPos);
                    yPos += 30;

                    // Print date range
                    string dateRange = $"Period: {dtpFromDate.Value:dd/MM/yyyy} to {dtpToDate.Value:dd/MM/yyyy}";
                    graphics.DrawString(dateRange, normalFont, Brushes.Black, leftMargin, yPos);
                    yPos += 20;

                    // Print category filter
                    if (cmbCategory.SelectedIndex > 0)
                    {
                        string categoryFilter = $"GST Category: {cmbCategory.Text}";
                        graphics.DrawString(categoryFilter, normalFont, Brushes.Black, leftMargin, yPos);
                        yPos += 20;
                    }

                    // Print generated date
                    string generatedDate = $"Generated on: {DateTime.Now:dd/MM/yyyy HH:mm}";
                    graphics.DrawString(generatedDate, smallFont, Brushes.Black, leftMargin, yPos);
                    yPos += 30;

                    // Print headers
                    string[] headers = { "Date", "Type", "Reference", "Party", "Gross", "GST", "Net", "Category", "GST%", "HSN" };
                    int[] columnWidths = { 70, 50, 90, 120, 70, 70, 70, 80, 60, 80 };
                    int xPos = leftMargin;

                    for (int i = 0; i < headers.Length; i++)
                    {
                        graphics.DrawString(headers[i], headerFont, Brushes.Black, xPos, yPos);
                        xPos += columnWidths[i];
                    }
                    yPos += 20;

                    // Print data
                    if (gstData != null && gstData.Rows.Count > 0)
                    {
                        decimal totalSalesGross = 0;
                        decimal totalSalesGST = 0;
                        decimal totalPurchaseGross = 0;
                        decimal totalPurchaseGST = 0;

                        foreach (DataRow row in gstData.Rows)
                        {
                            if (yPos > e.MarginBounds.Bottom - 150) // Check if page break needed
                            {
                                e.HasMorePages = true;
                                return;
                            }

                            xPos = leftMargin;
                            graphics.DrawString(Convert.ToDateTime(row["TransactionDate"]).ToString("dd/MM/yyyy"), normalFont, Brushes.Black, xPos, yPos);
                            xPos += columnWidths[0];

                            graphics.DrawString(row["TransactionType"].ToString(), normalFont, Brushes.Black, xPos, yPos);
                            xPos += columnWidths[1];

                            graphics.DrawString(row["ReferenceNumber"].ToString(), normalFont, Brushes.Black, xPos, yPos);
                            xPos += columnWidths[2];

                            graphics.DrawString(row["PartyName"].ToString(), normalFont, Brushes.Black, xPos, yPos);
                            xPos += columnWidths[3];

                            decimal grossAmount = Convert.ToDecimal(row["GrossAmount"]);
                            graphics.DrawString(grossAmount.ToString("N2"), normalFont, Brushes.Black, xPos, yPos);
                            xPos += columnWidths[4];

                            decimal gstAmount = Convert.ToDecimal(row["GSTAmount"]);
                            graphics.DrawString(gstAmount.ToString("N2"), normalFont, Brushes.Black, xPos, yPos);
                            xPos += columnWidths[5];

                            decimal netAmount = Convert.ToDecimal(row["NetAmount"]);
                            graphics.DrawString(netAmount.ToString("N2"), normalFont, Brushes.Black, xPos, yPos);
                            xPos += columnWidths[6];

                            graphics.DrawString(row["GSTCategory"].ToString(), normalFont, Brushes.Black, xPos, yPos);
                            xPos += columnWidths[7];

                            decimal gstPercentage = Convert.ToDecimal(row["GSTPercentage"]);
                            graphics.DrawString(gstPercentage.ToString("N2"), normalFont, Brushes.Black, xPos, yPos);
                            xPos += columnWidths[8];

                            graphics.DrawString(row["HSNCode"].ToString(), normalFont, Brushes.Black, xPos, yPos);

                            // Accumulate totals
                            if (row["TransactionType"].ToString() == "Sales")
                            {
                                totalSalesGross += grossAmount;
                                totalSalesGST += gstAmount;
                            }
                            else
                            {
                                totalPurchaseGross += grossAmount;
                                totalPurchaseGST += gstAmount;
                            }

                            yPos += 15;
                        }

                        // Print totals
                        yPos += 10;
                        graphics.DrawString("─".PadRight(50, '─'), headerFont, Brushes.Black, leftMargin, yPos);
                        yPos += 20;
                        graphics.DrawString($"Sales Gross: ₹{totalSalesGross:N2}", headerFont, Brushes.Black, leftMargin, yPos);
                        yPos += 15;
                        graphics.DrawString($"Sales GST: ₹{totalSalesGST:N2}", headerFont, Brushes.Black, leftMargin, yPos);
                        yPos += 15;
                        graphics.DrawString($"Purchase Gross: ₹{totalPurchaseGross:N2}", headerFont, Brushes.Black, leftMargin, yPos);
                        yPos += 15;
                        graphics.DrawString($"Purchase GST: ₹{totalPurchaseGST:N2}", headerFont, Brushes.Black, leftMargin, yPos);
                        yPos += 15;
                        graphics.DrawString($"GST Liability: ₹{(totalSalesGST - totalPurchaseGST):N2}", headerFont, Brushes.Black, leftMargin, yPos);
                    }
                    else
                    {
                        graphics.DrawString("No GST transactions found for the selected criteria.", normalFont, Brushes.Black, leftMargin, yPos);
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
                MessageBox.Show("Error printing GST report: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                saveDialog.FileName = $"GSTReport_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    using (StreamWriter writer = new StreamWriter(saveDialog.FileName))
                    {
                        // Write header
                        writer.WriteLine("GST Report");
                        writer.WriteLine($"Period: {dtpFromDate.Value:dd/MM/yyyy} to {dtpToDate.Value:dd/MM/yyyy}");
                        if (cmbCategory.SelectedIndex > 0)
                        {
                            writer.WriteLine($"GST Category: {cmbCategory.Text}");
                        }
                        writer.WriteLine($"Generated on: {DateTime.Now:dd/MM/yyyy HH:mm}");
                        writer.WriteLine();

                        // Write column headers
                        writer.WriteLine("Date,Type,Reference,Party,Gross Amount,GST Amount,Net Amount,GST Category,GST Percentage,HSN Code");

                        // Write data
                        if (gstData != null && gstData.Rows.Count > 0)
                        {
                            decimal totalSalesGross = 0;
                            decimal totalSalesGST = 0;
                            decimal totalPurchaseGross = 0;
                            decimal totalPurchaseGST = 0;

                            foreach (DataRow row in gstData.Rows)
                            {
                                string date = Convert.ToDateTime(row["TransactionDate"]).ToString("dd/MM/yyyy");
                                string type = row["TransactionType"].ToString().Replace(",", ";");
                                string reference = row["ReferenceNumber"].ToString().Replace(",", ";");
                                string party = row["PartyName"].ToString().Replace(",", ";");
                                decimal grossAmount = Convert.ToDecimal(row["GrossAmount"]);
                                decimal gstAmount = Convert.ToDecimal(row["GSTAmount"]);
                                decimal netAmount = Convert.ToDecimal(row["NetAmount"]);
                                string category = row["GSTCategory"].ToString().Replace(",", ";");
                                decimal gstPercentage = Convert.ToDecimal(row["GSTPercentage"]);
                                string hsnCode = row["HSNCode"].ToString().Replace(",", ";");

                                writer.WriteLine($"{date},{type},{reference},{party},{grossAmount:N2},{gstAmount:N2},{netAmount:N2},{category},{gstPercentage:N2},{hsnCode}");

                                // Accumulate totals
                                if (type == "Sales")
                                {
                                    totalSalesGross += grossAmount;
                                    totalSalesGST += gstAmount;
                                }
                                else
                                {
                                    totalPurchaseGross += grossAmount;
                                    totalPurchaseGST += gstAmount;
                                }
                            }

                            writer.WriteLine();
                            writer.WriteLine($"Sales Gross,{totalSalesGross:N2}");
                            writer.WriteLine($"Sales GST,{totalSalesGST:N2}");
                            writer.WriteLine($"Purchase Gross,{totalPurchaseGross:N2}");
                            writer.WriteLine($"Purchase GST,{totalPurchaseGST:N2}");
                            writer.WriteLine($"GST Liability,{(totalSalesGST - totalPurchaseGST):N2}");
                        }
                    }

                    MessageBox.Show("GST report exported successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exporting GST report: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cmbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadGSTReport();
        }
    }
} 