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
    public partial class SalesReturnReport : Form
    {
        private DataTable returnData;

        public SalesReturnReport()
        {
            InitializeComponent();
            InitializeForm();
            LoadSalesReturnReport();
        }

        private void InitializeForm()
        {
            // Load customers
            LoadCustomers();
            
            // Initialize data table
            returnData = new DataTable();
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

        private void LoadSalesReturnReport()
        {
            try
            {
                DateTime fromDate = dtpFromDate.Value.Date;
                DateTime toDate = dtpToDate.Value.Date;
                
                string query = @"SELECT 
                                    sr.ReturnID,
                                    sr.ReturnNumber,
                                    sr.ReturnDate,
                                    c.CustomerName,
                                    sr.TotalAmount,
                                    sr.ReturnReason,
                                    sr.Remarks,
                                    COUNT(sri.ItemID) as ItemCount
                               FROM SaleReturns sr
                               INNER JOIN Customers c ON sr.CustomerID = c.CustomerID
                               LEFT JOIN SaleReturnItems sri ON sr.ReturnID = sri.ReturnID
                               WHERE sr.ReturnDate BETWEEN @FromDate AND @ToDate
                               AND sr.IsActive = 1";

                // Add customer filter
                if (cmbCustomer.SelectedIndex > 0)
                {
                    ComboBoxItem selectedItem = (ComboBoxItem)cmbCustomer.SelectedItem;
                    query += " AND sr.CustomerID = @CustomerID";
                }

                query += " GROUP BY sr.ReturnID, sr.ReturnNumber, sr.ReturnDate, c.CustomerName, sr.TotalAmount, sr.ReturnReason, sr.Remarks";
                query += " ORDER BY sr.ReturnDate DESC";

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

                returnData = DatabaseConnection.ExecuteQuery(query, parameters);
                ShowReturnData();
                CalculateTotals();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading sales return report: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowReturnData()
        {
            SetupDataGridView();
            
            dgvSalesReturn.Rows.Clear();

            if (returnData != null && returnData.Rows.Count > 0)
            {
                foreach (DataRow row in returnData.Rows)
                {
                    int rowIndex = dgvSalesReturn.Rows.Add();
                    dgvSalesReturn.Rows[rowIndex].Cells["ReturnID"].Value = row["ReturnID"];
                    dgvSalesReturn.Rows[rowIndex].Cells["ReturnNumber"].Value = row["ReturnNumber"];
                    dgvSalesReturn.Rows[rowIndex].Cells["ReturnDate"].Value = Convert.ToDateTime(row["ReturnDate"]).ToString("dd/MM/yyyy");
                    dgvSalesReturn.Rows[rowIndex].Cells["CustomerName"].Value = row["CustomerName"];
                    dgvSalesReturn.Rows[rowIndex].Cells["TotalAmount"].Value = Convert.ToDecimal(row["TotalAmount"]).ToString("N2");
                    dgvSalesReturn.Rows[rowIndex].Cells["ReturnReason"].Value = row["ReturnReason"];
                    dgvSalesReturn.Rows[rowIndex].Cells["ItemCount"].Value = row["ItemCount"];
                    dgvSalesReturn.Rows[rowIndex].Cells["Remarks"].Value = row["Remarks"];

                    // Color coding for return reasons
                    string returnReason = row["ReturnReason"].ToString().ToLower();
                    if (returnReason.Contains("damaged") || returnReason.Contains("defective"))
                    {
                        dgvSalesReturn.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightCoral;
                        dgvSalesReturn.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.DarkRed;
                    }
                    else if (returnReason.Contains("expired"))
                    {
                        dgvSalesReturn.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightYellow;
                        dgvSalesReturn.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.DarkOrange;
                    }
                }
            }
        }

        private void SetupDataGridView()
        {
            dgvSalesReturn.Columns.Clear();
            
            dgvSalesReturn.Columns.Add("ReturnID", "Return ID");
            dgvSalesReturn.Columns.Add("ReturnNumber", "Return #");
            dgvSalesReturn.Columns.Add("ReturnDate", "Date");
            dgvSalesReturn.Columns.Add("CustomerName", "Customer");
            dgvSalesReturn.Columns.Add("TotalAmount", "Total Amount");
            dgvSalesReturn.Columns.Add("ReturnReason", "Return Reason");
            dgvSalesReturn.Columns.Add("ItemCount", "Items");
            dgvSalesReturn.Columns.Add("Remarks", "Remarks");

            // Configure columns
            dgvSalesReturn.Columns["ReturnID"].Width = 80;
            dgvSalesReturn.Columns["ReturnNumber"].Width = 120;
            dgvSalesReturn.Columns["ReturnDate"].Width = 100;
            dgvSalesReturn.Columns["CustomerName"].Width = 150;
            dgvSalesReturn.Columns["TotalAmount"].Width = 120;
            dgvSalesReturn.Columns["ReturnReason"].Width = 150;
            dgvSalesReturn.Columns["ItemCount"].Width = 60;
            dgvSalesReturn.Columns["Remarks"].Width = 150;

            // Set alignment
            dgvSalesReturn.Columns["ReturnID"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvSalesReturn.Columns["TotalAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvSalesReturn.Columns["ItemCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Set header alignment
            dgvSalesReturn.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvSalesReturn.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
        }

        private void CalculateTotals()
        {
            int totalReturns = 0;
            decimal totalAmount = 0;
            int totalItems = 0;

            if (returnData != null && returnData.Rows.Count > 0)
            {
                totalReturns = returnData.Rows.Count;
                foreach (DataRow row in returnData.Rows)
                {
                    totalAmount += Convert.ToDecimal(row["TotalAmount"]);
                    totalItems += Convert.ToInt32(row["ItemCount"]);
                }
            }

            lblTotalReturns.Text = "Total Returns: " + totalReturns.ToString();
            lblTotalAmount.Text = "Total Amount: " + totalAmount.ToString("N2");
            lblTotalItems.Text = "Total Items: " + totalItems.ToString();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadSalesReturnReport();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (returnData.Rows.Count == 0)
            {
                MessageBox.Show("No data to print.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            PrintDocument pd = new PrintDocument();
            pd.PrintPage += new PrintPageEventHandler(PrintSalesReturnReport);
            
            PrintDialog printDialog = new PrintDialog();
            printDialog.Document = pd;
            
            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                pd.Print();
            }
        }

        private void PrintSalesReturnReport(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Font titleFont = new Font("Arial", 16, FontStyle.Bold);
            Font headerFont = new Font("Arial", 10, FontStyle.Bold);
            Font dataFont = new Font("Arial", 9);
            Font summaryFont = new Font("Arial", 10, FontStyle.Bold);

            int yPos = 50;
            int leftMargin = 50;

            // Print title
            string title = "Sales Return Report";
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
            string[] headers = { "Return #", "Date", "Customer", "Amount", "Reason", "Items" };
            int[] columnWidths = { 100, 80, 150, 100, 120, 60 };
            int xPos = leftMargin;

            for (int i = 0; i < headers.Length; i++)
            {
                g.DrawString(headers[i], headerFont, Brushes.Black, xPos, yPos);
                xPos += columnWidths[i];
            }
            yPos += 20;

            // Print data
            foreach (DataRow row in returnData.Rows)
            {
                if (yPos > e.MarginBounds.Bottom - 100)
                {
                    e.HasMorePages = true;
                    return;
                }

                xPos = leftMargin;
                
                // Return Number
                g.DrawString(row["ReturnNumber"].ToString(), dataFont, Brushes.Black, xPos, yPos);
                xPos += columnWidths[0];

                // Date
                g.DrawString(Convert.ToDateTime(row["ReturnDate"]).ToString("dd/MM/yyyy"), dataFont, Brushes.Black, xPos, yPos);
                xPos += columnWidths[1];

                // Customer
                g.DrawString(row["CustomerName"].ToString(), dataFont, Brushes.Black, xPos, yPos);
                xPos += columnWidths[2];

                // Amount
                g.DrawString(Convert.ToDecimal(row["TotalAmount"]).ToString("N2"), dataFont, Brushes.Black, xPos, yPos);
                xPos += columnWidths[3];

                // Return Reason
                g.DrawString(row["ReturnReason"].ToString(), dataFont, Brushes.Black, xPos, yPos);
                xPos += columnWidths[4];

                // Items
                g.DrawString(row["ItemCount"].ToString(), dataFont, Brushes.Black, xPos, yPos);

                yPos += 15;
            }

            // Print summary
            yPos += 20;
            g.DrawString($"Total Returns: {returnData.Rows.Count}", summaryFont, Brushes.Black, leftMargin, yPos);
            yPos += 15;
            
            decimal totalAmount = 0;
            foreach (DataRow row in returnData.Rows)
            {
                totalAmount += Convert.ToDecimal(row["TotalAmount"]);
            }
            g.DrawString($"Total Amount: {totalAmount:N2}", summaryFont, Brushes.Black, leftMargin, yPos);
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (returnData.Rows.Count == 0)
            {
                MessageBox.Show("No data to export.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "CSV files (*.csv)|*.csv";
            saveDialog.FileName = $"SalesReturnReport_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

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
                csv.AppendLine("Return Number,Date,Customer,Total Amount,Return Reason,Items,Remarks");
                
                // Add data
                foreach (DataRow row in returnData.Rows)
                {
                    string returnNumber = row["ReturnNumber"].ToString().Replace(",", ";");
                    string date = Convert.ToDateTime(row["ReturnDate"]).ToString("dd/MM/yyyy");
                    string customer = row["CustomerName"].ToString().Replace(",", ";");
                    string amount = Convert.ToDecimal(row["TotalAmount"]).ToString("N2");
                    string returnReason = row["ReturnReason"].ToString().Replace(",", ";");
                    string items = row["ItemCount"].ToString();
                    string remarks = row["Remarks"].ToString().Replace(",", ";");

                    csv.AppendLine($"{returnNumber},{date},{customer},{amount},{returnReason},{items},{remarks}");
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
            LoadSalesReturnReport();
        }

        private void dtpFromDate_ValueChanged(object sender, EventArgs e)
        {
            LoadSalesReturnReport();
        }

        private void dtpToDate_ValueChanged(object sender, EventArgs e)
        {
            LoadSalesReturnReport();
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