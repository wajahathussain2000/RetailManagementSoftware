using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using RetailManagement.Database;

namespace RetailManagement.UserForms
{
    public partial class ExpiredStockReport : Form
    {
        private DataTable expiredData;

        public ExpiredStockReport()
        {
            InitializeComponent();
            LoadCategories();
            LoadExpiredStockReport();
        }

        private void LoadCategories()
        {
            try
            {
                string query = "SELECT DISTINCT Category FROM Items WHERE IsActive = 1 ORDER BY Category";
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

        private void LoadExpiredStockReport()
        {
            try
            {
                string query = @"SELECT 
                                    i.ItemID,
                                    i.ItemName,
                                    i.Category,
                                    i.ExpiryDate,
                                    i.StockQuantity,
                                    i.PurchasePrice,
                                    (i.StockQuantity * i.PurchasePrice) as TotalValue,
                                    DATEDIFF(day, GETDATE(), i.ExpiryDate) as DaysUntilExpiry
                                FROM Items i
                                WHERE i.IsActive = 1 
                                AND i.ExpiryDate IS NOT NULL
                                AND i.ExpiryDate <= GETDATE()";

                if (cmbCategory.SelectedIndex > 0)
                {
                    query += " AND i.Category = @Category";
                }

                query += " ORDER BY i.ExpiryDate DESC, i.ItemName";

                SqlParameter[] parameters = null;
                if (cmbCategory.SelectedIndex > 0)
                {
                    parameters = new SqlParameter[]
                    {
                        new SqlParameter("@Category", cmbCategory.Text)
                    };
                }

                expiredData = DatabaseConnection.ExecuteQuery(query, parameters);
                ShowExpiredData();
                CalculateTotals();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading expired stock report: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowExpiredData()
        {
            dgvExpiredStock.DataSource = expiredData;
            dgvExpiredStock.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            
            // Color code expired items
            foreach (DataGridViewRow row in dgvExpiredStock.Rows)
            {
                if (row.Cells["DaysUntilExpiry"].Value != DBNull.Value)
                {
                    int daysUntilExpiry = Convert.ToInt32(row.Cells["DaysUntilExpiry"].Value);
                    if (daysUntilExpiry < 0)
                    {
                        row.DefaultCellStyle.BackColor = Color.LightCoral;
                        row.DefaultCellStyle.ForeColor = Color.DarkRed;
                    }
                }
            }
        }

        private void CalculateTotals()
        {
            if (expiredData != null && expiredData.Rows.Count > 0)
            {
                int totalItems = expiredData.Rows.Count;
                decimal totalValue = 0;
                int totalQuantity = 0;

                foreach (DataRow row in expiredData.Rows)
                {
                    totalQuantity += Convert.ToInt32(row["StockQuantity"]);
                    totalValue += Convert.ToDecimal(row["TotalValue"]);
                }

                lblTotalItems.Text = $"Total Items: {totalItems}";
                lblTotalQuantity.Text = $"Total Quantity: {totalQuantity}";
                lblTotalValue.Text = $"Total Value: ₹{totalValue:N2}";
            }
            else
            {
                lblTotalItems.Text = "Total Items: 0";
                lblTotalQuantity.Text = "Total Quantity: 0";
                lblTotalValue.Text = "Total Value: ₹0.00";
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadExpiredStockReport();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            cmbCategory.SelectedIndex = 0;
            LoadExpiredStockReport();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (expiredData == null || expiredData.Rows.Count == 0)
                {
                    MessageBox.Show("No data to print.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    // Create print document
                    System.Drawing.Printing.PrintDocument pd = new System.Drawing.Printing.PrintDocument();
                    pd.PrintPage += PrintPage;
                    pd.Print();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error printing report: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            try
            {
                Graphics g = e.Graphics;
                Font titleFont = new Font("Arial", 16, FontStyle.Bold);
                Font headerFont = new Font("Arial", 10, FontStyle.Bold);
                Font normalFont = new Font("Arial", 9);
                Font smallFont = new Font("Arial", 8);

                int yPos = 50;
                int leftMargin = 50;
                int topMargin = 50;

                // Print title
                string title = "EXPIRED STOCK REPORT";
                g.DrawString(title, titleFont, Brushes.Black, leftMargin, yPos);
                yPos += 30;

                // Print date
                string date = "Generated on: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                g.DrawString(date, normalFont, Brushes.Black, leftMargin, yPos);
                yPos += 20;

                if (cmbCategory.SelectedIndex > 0)
                {
                    string category = "Category: " + cmbCategory.Text;
                    g.DrawString(category, normalFont, Brushes.Black, leftMargin, yPos);
                    yPos += 20;
                }

                yPos += 10;

                // Print headers
                string[] headers = { "Item Name", "Category", "Expiry Date", "Quantity", "Unit Price", "Total Value" };
                int[] columnWidths = { 150, 100, 100, 80, 80, 100 };
                int xPos = leftMargin;

                for (int i = 0; i < headers.Length; i++)
                {
                    g.DrawString(headers[i], headerFont, Brushes.Black, xPos, yPos);
                    xPos += columnWidths[i];
                }

                yPos += 20;

                // Print data
                foreach (DataRow row in expiredData.Rows)
                {
                    if (yPos > e.PageBounds.Height - 100)
                    {
                        e.HasMorePages = true;
                        return;
                    }

                    xPos = leftMargin;
                    g.DrawString(row["ItemName"].ToString(), normalFont, Brushes.Black, xPos, yPos);
                    xPos += columnWidths[0];

                    g.DrawString(row["Category"].ToString(), normalFont, Brushes.Black, xPos, yPos);
                    xPos += columnWidths[1];

                    DateTime expiryDate = Convert.ToDateTime(row["ExpiryDate"]);
                    g.DrawString(expiryDate.ToString("dd/MM/yyyy"), normalFont, Brushes.Black, xPos, yPos);
                    xPos += columnWidths[2];

                    g.DrawString(row["StockQuantity"].ToString(), normalFont, Brushes.Black, xPos, yPos);
                    xPos += columnWidths[3];

                    decimal unitPrice = Convert.ToDecimal(row["PurchasePrice"]);
                    g.DrawString(unitPrice.ToString("N2"), normalFont, Brushes.Black, xPos, yPos);
                    xPos += columnWidths[4];

                    decimal totalValue = Convert.ToDecimal(row["TotalValue"]);
                    g.DrawString(totalValue.ToString("N2"), normalFont, Brushes.Black, xPos, yPos);

                    yPos += 15;
                }

                // Print totals
                yPos += 10;
                g.DrawString("---", normalFont, Brushes.Black, leftMargin, yPos);
                yPos += 15;

                if (expiredData.Rows.Count > 0)
                {
                    int totalItems = expiredData.Rows.Count;
                    int totalQuantity = 0;
                    decimal totalValue = 0;

                    foreach (DataRow row in expiredData.Rows)
                    {
                        totalQuantity += Convert.ToInt32(row["StockQuantity"]);
                        totalValue += Convert.ToDecimal(row["TotalValue"]);
                    }

                    g.DrawString($"Total Items: {totalItems}", headerFont, Brushes.Black, leftMargin, yPos);
                    yPos += 15;
                    g.DrawString($"Total Quantity: {totalQuantity}", headerFont, Brushes.Black, leftMargin, yPos);
                    yPos += 15;
                    g.DrawString($"Total Value: ₹{totalValue:N2}", headerFont, Brushes.Black, leftMargin, yPos);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error printing: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                if (expiredData == null || expiredData.Rows.Count == 0)
                {
                    MessageBox.Show("No data to export.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "CSV files (*.csv)|*.csv";
                saveDialog.FileName = $"ExpiredStockReport_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    using (StreamWriter sw = new StreamWriter(saveDialog.FileName))
                    {
                        // Write headers
                        sw.WriteLine("Item Name,Category,Expiry Date,Quantity,Unit Price,Total Value,Days Until Expiry");

                        // Write data
                        foreach (DataRow row in expiredData.Rows)
                        {
                            string line = string.Format("{0},{1},{2},{3},{4},{5},{6}",
                                EscapeCsvField(row["ItemName"].ToString()),
                                EscapeCsvField(row["Category"].ToString()),
                                Convert.ToDateTime(row["ExpiryDate"]).ToString("dd/MM/yyyy"),
                                row["StockQuantity"],
                                Convert.ToDecimal(row["PurchasePrice"]).ToString("N2"),
                                Convert.ToDecimal(row["TotalValue"]).ToString("N2"),
                                row["DaysUntilExpiry"]);
                            sw.WriteLine(line);
                        }
                    }

                    MessageBox.Show("Report exported successfully to: " + saveDialog.FileName, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exporting report: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string EscapeCsvField(string field)
        {
            if (field.Contains(",") || field.Contains("\"") || field.Contains("\n"))
            {
                return "\"" + field.Replace("\"", "\"\"") + "\"";
            }
            return field;
        }

        private void cmbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadExpiredStockReport();
        }
    }
} 