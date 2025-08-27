using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RetailManagement.Database;
using RetailManagement.Models;

namespace RetailManagement.UserForms
{
    public partial class SupplierLedger : Form
    {
        private DataTable ledgerData;

        public SupplierLedger()
        {
            InitializeComponent();
            InitializeForm();
        }

        private void InitializeForm()
        {
            // Set default date range (current month)
            dtpFromDate.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dtpToDate.Value = DateTime.Now;

            // Load companies (suppliers)
            LoadCompanies();

            // Load initial data
            LoadSupplierLedger();
        }

        private void LoadCompanies()
        {
            try
            {
                string query = "SELECT CompanyID, CompanyName FROM Companies WHERE IsActive = 1 ORDER BY CompanyName";
                DataTable companies = DatabaseConnection.ExecuteQuery(query);

                cmbSupplier.Items.Clear();
                cmbSupplier.Items.Add("All Suppliers");

                foreach (DataRow row in companies.Rows)
                {
                    cmbSupplier.Items.Add(new ComboBoxItem
                    {
                        Text = row["CompanyName"].ToString(),
                        Value = row["CompanyID"]
                    });
                }

                cmbSupplier.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading suppliers: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSupplierLedger()
        {
            try
            {
                string query = @"SELECT 
                                    'Purchase' as TransactionType,
                                    p.PurchaseID as TransactionID,
                                    p.PurchaseNumber as ReferenceNumber,
                                    p.PurchaseDate as TransactionDate,
                                    c.CompanyName as SupplierName,
                                    p.TotalAmount as Debit,
                                    0 as Credit,
                                    p.Remarks as Description
                                FROM Purchases p
                                INNER JOIN Companies c ON p.CompanyID = c.CompanyID
                                WHERE p.IsActive = 1 
                                AND p.PurchaseDate BETWEEN @FromDate AND @ToDate";

                // Add supplier filter if not "All Suppliers"
                if (cmbSupplier.SelectedIndex > 0)
                {
                    query += " AND p.CompanyID = @CompanyID";
                }

                query += @" UNION ALL
                            SELECT 
                                'Payment' as TransactionType,
                                cp.PaymentID as TransactionID,
                                cp.PaymentNumber as ReferenceNumber,
                                cp.PaymentDate as TransactionDate,
                                c.CompanyName as SupplierName,
                                0 as Debit,
                                cp.Amount as Credit,
                                cp.Remarks as Description
                            FROM CompanyPayments cp
                            INNER JOIN Companies c ON cp.CompanyID = c.CompanyID
                            WHERE cp.IsActive = 1 
                            AND cp.PaymentDate BETWEEN @FromDate AND @ToDate";

                // Add supplier filter for payments
                if (cmbSupplier.SelectedIndex > 0)
                {
                    query += " AND cp.CompanyID = @CompanyID";
                }

                query += @" UNION ALL
                            SELECT 
                                'Return' as TransactionType,
                                pr.ReturnID as TransactionID,
                                pr.ReturnNumber as ReferenceNumber,
                                pr.ReturnDate as TransactionDate,
                                c.CompanyName as SupplierName,
                                0 as Debit,
                                pr.TotalAmount as Credit,
                                pr.Remarks as Description
                            FROM PurchaseReturns pr
                            INNER JOIN Companies c ON pr.CompanyID = c.CompanyID
                            WHERE pr.IsActive = 1 
                            AND pr.ReturnDate BETWEEN @FromDate AND @ToDate";

                // Add supplier filter for returns
                if (cmbSupplier.SelectedIndex > 0)
                {
                    query += " AND pr.CompanyID = @CompanyID";
                }

                query += " ORDER BY TransactionDate DESC, TransactionID DESC";

                SqlParameter[] parameters = {
                    new SqlParameter("@FromDate", dtpFromDate.Value.Date),
                    new SqlParameter("@ToDate", dtpToDate.Value.Date)
                };

                if (cmbSupplier.SelectedIndex > 0)
                {
                    var companyParam = new SqlParameter("@CompanyID", ((ComboBoxItem)cmbSupplier.SelectedItem).Value);
                    var newParams = new SqlParameter[parameters.Length + 1];
                    parameters.CopyTo(newParams, 0);
                    newParams[parameters.Length] = companyParam;
                    parameters = newParams;
                }

                ledgerData = DatabaseConnection.ExecuteQuery(query, parameters);
                ShowLedgerData();
                CalculateTotals();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading supplier ledger: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowLedgerData()
        {
            dataGridView1.DataSource = ledgerData;

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

                dataGridView1.Columns["SupplierName"].HeaderText = "Supplier";
                dataGridView1.Columns["SupplierName"].Width = 150;

                dataGridView1.Columns["Debit"].HeaderText = "Debit";
                dataGridView1.Columns["Debit"].Width = 100;
                dataGridView1.Columns["Debit"].DefaultCellStyle.Format = "N2";
                dataGridView1.Columns["Debit"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                dataGridView1.Columns["Credit"].HeaderText = "Credit";
                dataGridView1.Columns["Credit"].Width = 100;
                dataGridView1.Columns["Credit"].DefaultCellStyle.Format = "N2";
                dataGridView1.Columns["Credit"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                dataGridView1.Columns["Description"].HeaderText = "Description";
                dataGridView1.Columns["Description"].Width = 200;

                // Color coding for transaction types
                dataGridView1.CellFormatting += (sender, e) =>
                {
                    if (e.Value != null && e.ColumnIndex == dataGridView1.Columns["TransactionType"].Index)
                    {
                        string transactionType = e.Value.ToString();
                        switch (transactionType)
                        {
                            case "Purchase":
                                e.CellStyle.ForeColor = Color.DarkBlue;
                                break;
                            case "Payment":
                                e.CellStyle.ForeColor = Color.DarkGreen;
                                break;
                            case "Return":
                                e.CellStyle.ForeColor = Color.DarkRed;
                                break;
                        }
                    }
                };
            }
        }

        private void CalculateTotals()
        {
            decimal totalDebit = 0;
            decimal totalCredit = 0;
            decimal balance = 0;

            if (ledgerData != null && ledgerData.Rows.Count > 0)
            {
                foreach (DataRow row in ledgerData.Rows)
                {
                    totalDebit += Convert.ToDecimal(row["Debit"]);
                    totalCredit += Convert.ToDecimal(row["Credit"]);
                }
                balance = totalDebit - totalCredit;
            }

            lblTotalDebit.Text = $"Total Debit: ₹{totalDebit:N2}";
            lblTotalCredit.Text = $"Total Credit: ₹{totalCredit:N2}";
            lblBalance.Text = $"Balance: ₹{balance:N2}";

            // Color code the balance
            if (balance > 0)
            {
                lblBalance.ForeColor = Color.DarkRed; // We owe money
            }
            else if (balance < 0)
            {
                lblBalance.ForeColor = Color.DarkGreen; // We have credit
            }
            else
            {
                lblBalance.ForeColor = Color.Black; // Balanced
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadSupplierLedger();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            // Reset to current month
            dtpFromDate.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dtpToDate.Value = DateTime.Now;
            cmbSupplier.SelectedIndex = 0;
            LoadSupplierLedger();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            PrintSupplierLedger();
        }

        private void PrintSupplierLedger()
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
                    string title = "SUPPLIER LEDGER";
                    graphics.DrawString(title, titleFont, Brushes.Black, leftMargin, yPos);
                    yPos += 30;

                    // Print date range
                    string dateRange = $"Period: {dtpFromDate.Value:dd/MM/yyyy} to {dtpToDate.Value:dd/MM/yyyy}";
                    graphics.DrawString(dateRange, normalFont, Brushes.Black, leftMargin, yPos);
                    yPos += 20;

                    // Print supplier filter
                    if (cmbSupplier.SelectedIndex > 0)
                    {
                        string supplierFilter = $"Supplier: {cmbSupplier.Text}";
                        graphics.DrawString(supplierFilter, normalFont, Brushes.Black, leftMargin, yPos);
                        yPos += 20;
                    }

                    // Print generated date
                    string generatedDate = $"Generated on: {DateTime.Now:dd/MM/yyyy HH:mm}";
                    graphics.DrawString(generatedDate, smallFont, Brushes.Black, leftMargin, yPos);
                    yPos += 30;

                    // Print headers
                    string[] headers = { "Date", "Type", "Reference", "Supplier", "Debit", "Credit", "Description" };
                    int[] columnWidths = { 80, 60, 100, 120, 80, 80, 150 };
                    int xPos = leftMargin;

                    for (int i = 0; i < headers.Length; i++)
                    {
                        graphics.DrawString(headers[i], headerFont, Brushes.Black, xPos, yPos);
                        xPos += columnWidths[i];
                    }
                    yPos += 20;

                    // Print data
                    if (ledgerData != null && ledgerData.Rows.Count > 0)
                    {
                        decimal totalDebit = 0;
                        decimal totalCredit = 0;

                        foreach (DataRow row in ledgerData.Rows)
                        {
                            if (yPos > e.MarginBounds.Bottom - 100) // Check if page break needed
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

                            graphics.DrawString(row["SupplierName"].ToString(), normalFont, Brushes.Black, xPos, yPos);
                            xPos += columnWidths[3];

                            decimal debit = Convert.ToDecimal(row["Debit"]);
                            totalDebit += debit;
                            graphics.DrawString(debit.ToString("N2"), normalFont, Brushes.Black, xPos, yPos);
                            xPos += columnWidths[4];

                            decimal credit = Convert.ToDecimal(row["Credit"]);
                            totalCredit += credit;
                            graphics.DrawString(credit.ToString("N2"), normalFont, Brushes.Black, xPos, yPos);
                            xPos += columnWidths[5];

                            graphics.DrawString(row["Description"].ToString(), normalFont, Brushes.Black, xPos, yPos);

                            yPos += 15;
                        }

                        // Print totals
                        yPos += 10;
                        graphics.DrawString("─".PadRight(50, '─'), headerFont, Brushes.Black, leftMargin, yPos);
                        yPos += 20;
                        graphics.DrawString($"Total Debit: ₹{totalDebit:N2}", headerFont, Brushes.Black, leftMargin, yPos);
                        yPos += 15;
                        graphics.DrawString($"Total Credit: ₹{totalCredit:N2}", headerFont, Brushes.Black, leftMargin, yPos);
                        yPos += 15;
                        graphics.DrawString($"Balance: ₹{(totalDebit - totalCredit):N2}", headerFont, Brushes.Black, leftMargin, yPos);
                    }
                    else
                    {
                        graphics.DrawString("No transactions found for the selected criteria.", normalFont, Brushes.Black, leftMargin, yPos);
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
                MessageBox.Show("Error printing ledger: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                saveDialog.FileName = $"SupplierLedger_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    using (StreamWriter writer = new StreamWriter(saveDialog.FileName))
                    {
                        // Write header
                        writer.WriteLine("Supplier Ledger");
                        writer.WriteLine($"Period: {dtpFromDate.Value:dd/MM/yyyy} to {dtpToDate.Value:dd/MM/yyyy}");
                        if (cmbSupplier.SelectedIndex > 0)
                        {
                            writer.WriteLine($"Supplier: {cmbSupplier.Text}");
                        }
                        writer.WriteLine($"Generated on: {DateTime.Now:dd/MM/yyyy HH:mm}");
                        writer.WriteLine();

                        // Write column headers
                        writer.WriteLine("Date,Type,Reference,Supplier,Debit,Credit,Description");

                        // Write data
                        if (ledgerData != null && ledgerData.Rows.Count > 0)
                        {
                            decimal totalDebit = 0;
                            decimal totalCredit = 0;

                            foreach (DataRow row in ledgerData.Rows)
                            {
                                string date = Convert.ToDateTime(row["TransactionDate"]).ToString("dd/MM/yyyy");
                                string type = row["TransactionType"].ToString().Replace(",", ";");
                                string reference = row["ReferenceNumber"].ToString().Replace(",", ";");
                                string supplier = row["SupplierName"].ToString().Replace(",", ";");
                                decimal debit = Convert.ToDecimal(row["Debit"]);
                                decimal credit = Convert.ToDecimal(row["Credit"]);
                                string description = row["Description"].ToString().Replace(",", ";");

                                writer.WriteLine($"{date},{type},{reference},{supplier},{debit:N2},{credit:N2},{description}");
                                totalDebit += debit;
                                totalCredit += credit;
                            }

                            writer.WriteLine();
                            writer.WriteLine($"Total Debit,{totalDebit:N2}");
                            writer.WriteLine($"Total Credit,{totalCredit:N2}");
                            writer.WriteLine($"Balance,{(totalDebit - totalCredit):N2}");
                        }
                    }

                    MessageBox.Show("Ledger exported successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exporting ledger: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cmbSupplier_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadSupplierLedger();
        }
    }
} 