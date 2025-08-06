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
    public partial class PurchaseReport : Form
    {
        private DataTable purchaseData;

        public PurchaseReport()
        {
            InitializeComponent();
            InitializeForm();
            LoadPurchaseReport();
        }

        private void InitializeForm()
        {
            // Load companies
            LoadCompanies();
            
            // Initialize data table
            purchaseData = new DataTable();
        }

        private void LoadCompanies()
        {
            try
            {
                string query = "SELECT CompanyID, CompanyName FROM Companies WHERE IsActive = 1 ORDER BY CompanyName";
                DataTable companies = DatabaseConnection.ExecuteQuery(query);
                
                cmbCompany.Items.Clear();
                cmbCompany.Items.Add("All Companies");
                
                foreach (DataRow row in companies.Rows)
                {
                    cmbCompany.Items.Add(new ComboBoxItem 
                    { 
                        Text = row["CompanyName"].ToString(), 
                        Value = Convert.ToInt32(row["CompanyID"]) 
                    });
                }
                
                cmbCompany.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading companies: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadPurchaseReport()
        {
            try
            {
                DateTime fromDate = dtpFromDate.Value.Date;
                DateTime toDate = dtpToDate.Value.Date;
                
                string query = @"SELECT 
                                    p.PurchaseID,
                                    p.PurchaseNumber,
                                    p.PurchaseDate,
                                    c.CompanyName,
                                    p.TotalAmount,
                                    p.PaymentMethod,
                                    p.Remarks,
                                    COUNT(pi.ItemID) as ItemCount
                               FROM Purchases p
                               INNER JOIN Companies c ON p.CompanyID = c.CompanyID
                               LEFT JOIN PurchaseItems pi ON p.PurchaseID = pi.PurchaseID
                               WHERE p.PurchaseDate BETWEEN @FromDate AND @ToDate
                               AND p.IsActive = 1";

                // Add company filter
                if (cmbCompany.SelectedIndex > 0)
                {
                    ComboBoxItem selectedItem = (ComboBoxItem)cmbCompany.SelectedItem;
                    query += " AND p.CompanyID = @CompanyID";
                }

                query += " GROUP BY p.PurchaseID, p.PurchaseNumber, p.PurchaseDate, c.CompanyName, p.TotalAmount, p.PaymentMethod, p.Remarks";
                query += " ORDER BY p.PurchaseDate DESC";

                SqlParameter[] parameters = null;
                if (cmbCompany.SelectedIndex > 0)
                {
                    ComboBoxItem selectedItem = (ComboBoxItem)cmbCompany.SelectedItem;
                    parameters = new SqlParameter[]
                    {
                        new SqlParameter("@FromDate", fromDate),
                        new SqlParameter("@ToDate", toDate),
                        new SqlParameter("@CompanyID", selectedItem.Value)
                    };
                }
                else
                {
                    parameters = new SqlParameter[]
                    {
                        new SqlParameter("@FromDate", fromDate),
                        new SqlParameter("@ToDate", toDate)
                    };
                }

                purchaseData = DatabaseConnection.ExecuteQuery(query, parameters);
                ShowPurchaseData();
                CalculateTotals();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading purchase report: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowPurchaseData()
        {
            SetupDataGridView();
            
            dgvPurchaseReport.Rows.Clear();

            if (purchaseData != null && purchaseData.Rows.Count > 0)
            {
                foreach (DataRow row in purchaseData.Rows)
                {
                    int rowIndex = dgvPurchaseReport.Rows.Add();
                    dgvPurchaseReport.Rows[rowIndex].Cells["PurchaseID"].Value = row["PurchaseID"];
                    dgvPurchaseReport.Rows[rowIndex].Cells["PurchaseNumber"].Value = row["PurchaseNumber"];
                    dgvPurchaseReport.Rows[rowIndex].Cells["PurchaseDate"].Value = Convert.ToDateTime(row["PurchaseDate"]).ToString("dd/MM/yyyy");
                    dgvPurchaseReport.Rows[rowIndex].Cells["CompanyName"].Value = row["CompanyName"];
                    dgvPurchaseReport.Rows[rowIndex].Cells["TotalAmount"].Value = Convert.ToDecimal(row["TotalAmount"]).ToString("N2");
                    dgvPurchaseReport.Rows[rowIndex].Cells["PaymentMethod"].Value = row["PaymentMethod"];
                    dgvPurchaseReport.Rows[rowIndex].Cells["ItemCount"].Value = row["ItemCount"];
                    dgvPurchaseReport.Rows[rowIndex].Cells["Remarks"].Value = row["Remarks"];
                }
            }
        }

        private void SetupDataGridView()
        {
            dgvPurchaseReport.Columns.Clear();
            
            dgvPurchaseReport.Columns.Add("PurchaseID", "Purchase ID");
            dgvPurchaseReport.Columns.Add("PurchaseNumber", "Purchase #");
            dgvPurchaseReport.Columns.Add("PurchaseDate", "Date");
            dgvPurchaseReport.Columns.Add("CompanyName", "Company");
            dgvPurchaseReport.Columns.Add("TotalAmount", "Total Amount");
            dgvPurchaseReport.Columns.Add("PaymentMethod", "Payment Method");
            dgvPurchaseReport.Columns.Add("ItemCount", "Items");
            dgvPurchaseReport.Columns.Add("Remarks", "Remarks");

            // Configure columns
            dgvPurchaseReport.Columns["PurchaseID"].Width = 80;
            dgvPurchaseReport.Columns["PurchaseNumber"].Width = 120;
            dgvPurchaseReport.Columns["PurchaseDate"].Width = 100;
            dgvPurchaseReport.Columns["CompanyName"].Width = 150;
            dgvPurchaseReport.Columns["TotalAmount"].Width = 120;
            dgvPurchaseReport.Columns["PaymentMethod"].Width = 100;
            dgvPurchaseReport.Columns["ItemCount"].Width = 60;
            dgvPurchaseReport.Columns["Remarks"].Width = 150;

            // Set alignment
            dgvPurchaseReport.Columns["PurchaseID"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPurchaseReport.Columns["TotalAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvPurchaseReport.Columns["ItemCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Set header alignment
            dgvPurchaseReport.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPurchaseReport.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
        }

        private void CalculateTotals()
        {
            int totalPurchases = 0;
            decimal totalAmount = 0;
            int totalItems = 0;

            if (purchaseData != null && purchaseData.Rows.Count > 0)
            {
                totalPurchases = purchaseData.Rows.Count;
                foreach (DataRow row in purchaseData.Rows)
                {
                    totalAmount += Convert.ToDecimal(row["TotalAmount"]);
                    totalItems += Convert.ToInt32(row["ItemCount"]);
                }
            }

            lblTotalPurchases.Text = "Total Purchases: " + totalPurchases.ToString();
            lblTotalAmount.Text = "Total Amount: " + totalAmount.ToString("N2");
            lblTotalItems.Text = "Total Items: " + totalItems.ToString();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadPurchaseReport();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (purchaseData.Rows.Count == 0)
            {
                MessageBox.Show("No data to print.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            PrintDocument pd = new PrintDocument();
            pd.PrintPage += new PrintPageEventHandler(PrintPurchaseReport);
            
            PrintDialog printDialog = new PrintDialog();
            printDialog.Document = pd;
            
            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                pd.Print();
            }
        }

        private void PrintPurchaseReport(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Font titleFont = new Font("Arial", 16, FontStyle.Bold);
            Font headerFont = new Font("Arial", 10, FontStyle.Bold);
            Font dataFont = new Font("Arial", 9);
            Font summaryFont = new Font("Arial", 10, FontStyle.Bold);

            int yPos = 50;
            int leftMargin = 50;

            // Print title
            string title = "Purchase Report";
            if (cmbCompany.SelectedIndex > 0)
            {
                ComboBoxItem selectedItem = (ComboBoxItem)cmbCompany.SelectedItem;
                title += $" - {selectedItem.Text}";
            }
            title += $" ({dtpFromDate.Value:dd/MM/yyyy} to {dtpToDate.Value:dd/MM/yyyy})";
            g.DrawString(title, titleFont, Brushes.Black, leftMargin, yPos);
            yPos += 30;

            // Print date
            g.DrawString($"Generated on: {DateTime.Now:dd/MM/yyyy HH:mm}", dataFont, Brushes.Black, leftMargin, yPos);
            yPos += 30;

            // Print headers
            string[] headers = { "Purchase #", "Date", "Company", "Amount", "Payment", "Items" };
            int[] columnWidths = { 100, 80, 150, 100, 80, 60 };
            int xPos = leftMargin;

            for (int i = 0; i < headers.Length; i++)
            {
                g.DrawString(headers[i], headerFont, Brushes.Black, xPos, yPos);
                xPos += columnWidths[i];
            }
            yPos += 20;

            // Print data
            foreach (DataRow row in purchaseData.Rows)
            {
                if (yPos > e.MarginBounds.Bottom - 100)
                {
                    e.HasMorePages = true;
                    return;
                }

                xPos = leftMargin;
                
                // Purchase Number
                g.DrawString(row["PurchaseNumber"].ToString(), dataFont, Brushes.Black, xPos, yPos);
                xPos += columnWidths[0];

                // Date
                g.DrawString(Convert.ToDateTime(row["PurchaseDate"]).ToString("dd/MM/yyyy"), dataFont, Brushes.Black, xPos, yPos);
                xPos += columnWidths[1];

                // Company
                g.DrawString(row["CompanyName"].ToString(), dataFont, Brushes.Black, xPos, yPos);
                xPos += columnWidths[2];

                // Amount
                g.DrawString(Convert.ToDecimal(row["TotalAmount"]).ToString("N2"), dataFont, Brushes.Black, xPos, yPos);
                xPos += columnWidths[3];

                // Payment Method
                g.DrawString(row["PaymentMethod"].ToString(), dataFont, Brushes.Black, xPos, yPos);
                xPos += columnWidths[4];

                // Items
                g.DrawString(row["ItemCount"].ToString(), dataFont, Brushes.Black, xPos, yPos);

                yPos += 15;
            }

            // Print summary
            yPos += 20;
            g.DrawString($"Total Purchases: {purchaseData.Rows.Count}", summaryFont, Brushes.Black, leftMargin, yPos);
            yPos += 15;
            
            decimal totalAmount = 0;
            foreach (DataRow row in purchaseData.Rows)
            {
                totalAmount += Convert.ToDecimal(row["TotalAmount"]);
            }
            g.DrawString($"Total Amount: {totalAmount:N2}", summaryFont, Brushes.Black, leftMargin, yPos);
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (purchaseData.Rows.Count == 0)
            {
                MessageBox.Show("No data to export.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "CSV files (*.csv)|*.csv";
            saveDialog.FileName = $"PurchaseReport_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                ExportToCSV(saveDialog.FileName);
            }
        }

        private void ExportToCSV(string fileName)
        {
            try
            {
                StringBuilder csv = new StringBuilder();
                
                // Add headers
                csv.AppendLine("Purchase Number,Date,Company,Total Amount,Payment Method,Items,Remarks");
                
                // Add data
                foreach (DataRow row in purchaseData.Rows)
                {
                    string purchaseNumber = row["PurchaseNumber"].ToString().Replace(",", ";");
                    string date = Convert.ToDateTime(row["PurchaseDate"]).ToString("dd/MM/yyyy");
                    string company = row["CompanyName"].ToString().Replace(",", ";");
                    string amount = Convert.ToDecimal(row["TotalAmount"]).ToString("N2");
                    string paymentMethod = row["PaymentMethod"].ToString().Replace(",", ";");
                    string items = row["ItemCount"].ToString();
                    string remarks = row["Remarks"].ToString().Replace(",", ";");

                    csv.AppendLine($"{purchaseNumber},{date},{company},{amount},{paymentMethod},{items},{remarks}");
                }

                System.IO.File.WriteAllText(fileName, csv.ToString());
                MessageBox.Show("Data exported successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void cmbCompany_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadPurchaseReport();
        }

        private void dtpFromDate_ValueChanged(object sender, EventArgs e)
        {
            LoadPurchaseReport();
        }

        private void dtpToDate_ValueChanged(object sender, EventArgs e)
        {
            LoadPurchaseReport();
        }

        // Helper class for ComboBox items
        private class ComboBoxItem
        {
            public string Text { get; set; }
            public object Value { get; set; }
            public override string ToString() => Text;
        }
    }
} 