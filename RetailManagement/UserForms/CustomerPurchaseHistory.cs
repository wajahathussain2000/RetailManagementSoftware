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
    public partial class CustomerPurchaseHistory : Form
    {
        private DataTable historyData;

        public CustomerPurchaseHistory()
        {
            InitializeComponent();
            InitializeForm();
            LoadCustomerPurchaseHistory();
        }

        private void InitializeForm()
        {
            // Load customers
            LoadCustomers();
            
            // Initialize data table
            historyData = new DataTable();
        }

        private void LoadCustomers()
        {
            try
            {
                string query = "SELECT CustomerID, CustomerName FROM Customers WHERE IsActive = 1 ORDER BY CustomerName";
                DataTable customers = DatabaseConnection.ExecuteQuery(query);
                
                cmbCustomer.Items.Clear();
                cmbCustomer.Items.Add("All Customers");
                
                foreach (DataRow row in customers.Rows)
                {
                    cmbCustomer.Items.Add(new ComboBoxItem 
                    { 
                        Text = row["CustomerName"].ToString(), 
                        Value = Convert.ToInt32(row["CustomerID"]) 
                    });
                }
                
                cmbCustomer.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading customers: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadCustomerPurchaseHistory()
        {
            try
            {
                DateTime fromDate = dtpFromDate.Value.Date;
                DateTime toDate = dtpToDate.Value.Date;
                
                string query = @"SELECT 
                                    s.SaleID,
                                    s.BillNumber,
                                    s.SaleDate,
                                    c.CustomerName,
                                    s.TotalAmount,
                                    s.PaymentMethod,
                                    s.Remarks,
                                    COUNT(si.ItemID) as ItemCount
                               FROM Sales s
                               INNER JOIN Customers c ON s.CustomerID = c.CustomerID
                               LEFT JOIN SaleItems si ON s.SaleID = si.SaleID
                               WHERE s.SaleDate BETWEEN @FromDate AND @ToDate
                               AND s.IsActive = 1";

                // Add customer filter
                if (cmbCustomer.SelectedIndex > 0)
                {
                    ComboBoxItem selectedItem = (ComboBoxItem)cmbCustomer.SelectedItem;
                    query += " AND s.CustomerID = @CustomerID";
                }

                query += " GROUP BY s.SaleID, s.BillNumber, s.SaleDate, c.CustomerName, s.TotalAmount, s.PaymentMethod, s.Remarks";
                query += " ORDER BY s.SaleDate DESC";

                SqlParameter[] parameters = null;
                if (cmbCustomer.SelectedIndex > 0)
                {
                    ComboBoxItem selectedItem = (ComboBoxItem)cmbCustomer.SelectedItem;
                    parameters = new SqlParameter[]
                    {
                        new SqlParameter("@FromDate", fromDate),
                        new SqlParameter("@ToDate", toDate),
                        new SqlParameter("@CustomerID", selectedItem.Value)
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

                historyData = DatabaseConnection.ExecuteQuery(query, parameters);
                ShowHistoryData();
                CalculateTotals();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading customer purchase history: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowHistoryData()
        {
            SetupDataGridView();
            
            dgvHistory.Rows.Clear();

            if (historyData != null && historyData.Rows.Count > 0)
            {
                foreach (DataRow row in historyData.Rows)
                {
                    int rowIndex = dgvHistory.Rows.Add();
                    dgvHistory.Rows[rowIndex].Cells["SaleID"].Value = row["SaleID"];
                    dgvHistory.Rows[rowIndex].Cells["BillNumber"].Value = row["BillNumber"];
                    dgvHistory.Rows[rowIndex].Cells["SaleDate"].Value = Convert.ToDateTime(row["SaleDate"]).ToString("dd/MM/yyyy");
                    dgvHistory.Rows[rowIndex].Cells["CustomerName"].Value = row["CustomerName"];
                    dgvHistory.Rows[rowIndex].Cells["TotalAmount"].Value = Convert.ToDecimal(row["TotalAmount"]).ToString("N2");
                    dgvHistory.Rows[rowIndex].Cells["PaymentMethod"].Value = row["PaymentMethod"];
                    dgvHistory.Rows[rowIndex].Cells["ItemCount"].Value = row["ItemCount"];
                    dgvHistory.Rows[rowIndex].Cells["Remarks"].Value = row["Remarks"];
                }
            }
        }

        private void SetupDataGridView()
        {
            dgvHistory.Columns.Clear();
            
            dgvHistory.Columns.Add("SaleID", "Sale ID");
            dgvHistory.Columns.Add("BillNumber", "Bill #");
            dgvHistory.Columns.Add("SaleDate", "Date");
            dgvHistory.Columns.Add("CustomerName", "Customer");
            dgvHistory.Columns.Add("TotalAmount", "Total Amount");
            dgvHistory.Columns.Add("PaymentMethod", "Payment Method");
            dgvHistory.Columns.Add("ItemCount", "Items");
            dgvHistory.Columns.Add("Remarks", "Remarks");

            // Configure columns
            dgvHistory.Columns["SaleID"].Width = 80;
            dgvHistory.Columns["BillNumber"].Width = 120;
            dgvHistory.Columns["SaleDate"].Width = 100;
            dgvHistory.Columns["CustomerName"].Width = 150;
            dgvHistory.Columns["TotalAmount"].Width = 120;
            dgvHistory.Columns["PaymentMethod"].Width = 100;
            dgvHistory.Columns["ItemCount"].Width = 60;
            dgvHistory.Columns["Remarks"].Width = 150;

            // Set alignment
            dgvHistory.Columns["SaleID"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvHistory.Columns["TotalAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvHistory.Columns["ItemCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Set header alignment
            dgvHistory.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvHistory.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
        }

        private void CalculateTotals()
        {
            int totalSales = 0;
            decimal totalAmount = 0;
            int totalItems = 0;

            if (historyData != null && historyData.Rows.Count > 0)
            {
                totalSales = historyData.Rows.Count;
                foreach (DataRow row in historyData.Rows)
                {
                    totalAmount += Convert.ToDecimal(row["TotalAmount"]);
                    totalItems += Convert.ToInt32(row["ItemCount"]);
                }
            }

            lblTotalSales.Text = "Total Sales: " + totalSales.ToString();
            lblTotalAmount.Text = "Total Amount: " + totalAmount.ToString("N2");
            lblTotalItems.Text = "Total Items: " + totalItems.ToString();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadCustomerPurchaseHistory();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (historyData.Rows.Count == 0)
            {
                MessageBox.Show("No data to print.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            PrintDocument pd = new PrintDocument();
            pd.PrintPage += new PrintPageEventHandler(PrintCustomerHistory);
            
            PrintDialog printDialog = new PrintDialog();
            printDialog.Document = pd;
            
            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                pd.Print();
            }
        }

        private void PrintCustomerHistory(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Font titleFont = new Font("Arial", 16, FontStyle.Bold);
            Font headerFont = new Font("Arial", 10, FontStyle.Bold);
            Font dataFont = new Font("Arial", 9);
            Font summaryFont = new Font("Arial", 10, FontStyle.Bold);

            int yPos = 50;
            int leftMargin = 50;

            // Print title
            string title = "Customer Purchase History";
            if (cmbCustomer.SelectedIndex > 0)
            {
                ComboBoxItem selectedItem = (ComboBoxItem)cmbCustomer.SelectedItem;
                title += $" - {selectedItem.Text}";
            }
            title += $" ({dtpFromDate.Value:dd/MM/yyyy} to {dtpToDate.Value:dd/MM/yyyy})";
            g.DrawString(title, titleFont, Brushes.Black, leftMargin, yPos);
            yPos += 30;

            // Print date
            g.DrawString($"Generated on: {DateTime.Now:dd/MM/yyyy HH:mm}", dataFont, Brushes.Black, leftMargin, yPos);
            yPos += 30;

            // Print headers
            string[] headers = { "Bill #", "Date", "Customer", "Amount", "Payment", "Items" };
            int[] columnWidths = { 100, 80, 150, 100, 80, 60 };
            int xPos = leftMargin;

            for (int i = 0; i < headers.Length; i++)
            {
                g.DrawString(headers[i], headerFont, Brushes.Black, xPos, yPos);
                xPos += columnWidths[i];
            }
            yPos += 20;

            // Print data
            foreach (DataRow row in historyData.Rows)
            {
                if (yPos > e.MarginBounds.Bottom - 100)
                {
                    e.HasMorePages = true;
                    return;
                }

                xPos = leftMargin;
                
                // Bill Number
                g.DrawString(row["BillNumber"].ToString(), dataFont, Brushes.Black, xPos, yPos);
                xPos += columnWidths[0];

                // Date
                g.DrawString(Convert.ToDateTime(row["SaleDate"]).ToString("dd/MM/yyyy"), dataFont, Brushes.Black, xPos, yPos);
                xPos += columnWidths[1];

                // Customer
                g.DrawString(row["CustomerName"].ToString(), dataFont, Brushes.Black, xPos, yPos);
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
            g.DrawString($"Total Sales: {historyData.Rows.Count}", summaryFont, Brushes.Black, leftMargin, yPos);
            yPos += 15;
            
            decimal totalAmount = 0;
            foreach (DataRow row in historyData.Rows)
            {
                totalAmount += Convert.ToDecimal(row["TotalAmount"]);
            }
            g.DrawString($"Total Amount: {totalAmount:N2}", summaryFont, Brushes.Black, leftMargin, yPos);
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (historyData.Rows.Count == 0)
            {
                MessageBox.Show("No data to export.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "CSV files (*.csv)|*.csv";
            saveDialog.FileName = $"CustomerPurchaseHistory_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

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
                csv.AppendLine("Bill Number,Date,Customer,Total Amount,Payment Method,Items,Remarks");
                
                // Add data
                foreach (DataRow row in historyData.Rows)
                {
                    string billNumber = row["BillNumber"].ToString().Replace(",", ";");
                    string date = Convert.ToDateTime(row["SaleDate"]).ToString("dd/MM/yyyy");
                    string customer = row["CustomerName"].ToString().Replace(",", ";");
                    string amount = Convert.ToDecimal(row["TotalAmount"]).ToString("N2");
                    string paymentMethod = row["PaymentMethod"].ToString().Replace(",", ";");
                    string items = row["ItemCount"].ToString();
                    string remarks = row["Remarks"].ToString().Replace(",", ";");

                    csv.AppendLine($"{billNumber},{date},{customer},{amount},{paymentMethod},{items},{remarks}");
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

        private void cmbCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadCustomerPurchaseHistory();
        }

        private void dtpFromDate_ValueChanged(object sender, EventArgs e)
        {
            LoadCustomerPurchaseHistory();
        }

        private void dtpToDate_ValueChanged(object sender, EventArgs e)
        {
            LoadCustomerPurchaseHistory();
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