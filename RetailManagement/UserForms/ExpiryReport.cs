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
    public partial class ExpiryReport : Form
    {
        private DataTable expiryData;

        public ExpiryReport()
        {
            InitializeComponent();
            InitializeForm();
            LoadExpiryReport();
        }

        private void InitializeForm()
        {
            // Load categories
            LoadCategories();
            
            // Initialize data table
            expiryData = new DataTable();
        }

        private void LoadCategories()
        {
            try
            {
                string query = "SELECT DISTINCT Category FROM Items WHERE Category IS NOT NULL AND Category != '' ORDER BY Category";
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

        private void LoadExpiryReport()
        {
            try
            {
                int daysThreshold = (int)numDaysThreshold.Value;
                
                string query = @"SELECT 
                                    i.ItemID,
                                    i.ItemName,
                                    i.Category,
                                    i.StockQuantity,
                                    i.ExpiryDate,
                                    i.PurchasePrice,
                                    (i.StockQuantity * i.PurchasePrice) as TotalValue,
                                    CASE 
                                        WHEN i.ExpiryDate < GETDATE() THEN DATEDIFF(day, i.ExpiryDate, GETDATE())
                                        ELSE DATEDIFF(day, GETDATE(), i.ExpiryDate)
                                    END as DaysDifference
                               FROM Items i
                               WHERE i.ExpiryDate IS NOT NULL 
                               AND i.IsActive = 1";

                // Add category filter
                if (cmbCategory.SelectedIndex > 0)
                {
                    query += " AND i.Category = @Category";
                }

                // Add expiry filter based on report type
                if (rbNearExpiry.Checked)
                {
                    query += @" AND (
                                    i.ExpiryDate > GETDATE() AND 
                                    i.ExpiryDate <= DATEADD(day, @DaysThreshold, GETDATE())
                                )";
                }
                else if (rbExpired.Checked)
                {
                    query += " AND i.ExpiryDate < GETDATE()";
                }
                else if (rbAll.Checked)
                {
                    query += @" AND (
                                    (i.ExpiryDate < GETDATE()) OR 
                                    (i.ExpiryDate BETWEEN GETDATE() AND DATEADD(day, @DaysThreshold, GETDATE()))
                                )";
                }

                query += " ORDER BY i.ExpiryDate ASC";

                SqlParameter[] parameters = null;
                if (cmbCategory.SelectedIndex > 0)
                {
                    parameters = new SqlParameter[]
                    {
                        new SqlParameter("@Category", cmbCategory.Text),
                        new SqlParameter("@DaysThreshold", daysThreshold)
                    };
                }
                else
                {
                    parameters = new SqlParameter[]
                    {
                        new SqlParameter("@DaysThreshold", daysThreshold)
                    };
                }

                expiryData = DatabaseConnection.ExecuteQuery(query, parameters);
                ShowExpiryData();
                CalculateTotals();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading expiry report: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowExpiryData()
        {
            SetupDataGridView();
            
            dgvExpiryReport.Rows.Clear();

            if (expiryData != null && expiryData.Rows.Count > 0)
            {
                foreach (DataRow row in expiryData.Rows)
                {
                    int rowIndex = dgvExpiryReport.Rows.Add();
                    dgvExpiryReport.Rows[rowIndex].Cells["ItemID"].Value = row["ItemID"];
                    dgvExpiryReport.Rows[rowIndex].Cells["ItemName"].Value = row["ItemName"];
                    dgvExpiryReport.Rows[rowIndex].Cells["Category"].Value = row["Category"];
                    dgvExpiryReport.Rows[rowIndex].Cells["StockQuantity"].Value = row["StockQuantity"];
                    dgvExpiryReport.Rows[rowIndex].Cells["ExpiryDate"].Value = Convert.ToDateTime(row["ExpiryDate"]).ToString("dd/MM/yyyy");
                    dgvExpiryReport.Rows[rowIndex].Cells["PurchasePrice"].Value = Convert.ToDecimal(row["PurchasePrice"]).ToString("N2");
                    dgvExpiryReport.Rows[rowIndex].Cells["TotalValue"].Value = Convert.ToDecimal(row["TotalValue"]).ToString("N2");
                    dgvExpiryReport.Rows[rowIndex].Cells["DaysDifference"].Value = row["DaysDifference"];

                    // Color coding based on expiry status
                    DateTime expiryDate = Convert.ToDateTime(row["ExpiryDate"]);
                    int daysDifference = Convert.ToInt32(row["DaysDifference"]);
                    
                    if (expiryDate < DateTime.Now)
                    {
                        dgvExpiryReport.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightCoral;
                        dgvExpiryReport.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.DarkRed;
                    }
                    else if (daysDifference <= 7)
                    {
                        dgvExpiryReport.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightCoral;
                        dgvExpiryReport.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.DarkRed;
                    }
                    else if (daysDifference <= 30)
                    {
                        dgvExpiryReport.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightYellow;
                        dgvExpiryReport.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.DarkOrange;
                    }
                }
            }
        }

        private void SetupDataGridView()
        {
            dgvExpiryReport.Columns.Clear();
            
            dgvExpiryReport.Columns.Add("ItemID", "Item ID");
            dgvExpiryReport.Columns.Add("ItemName", "Item Name");
            dgvExpiryReport.Columns.Add("Category", "Category");
            dgvExpiryReport.Columns.Add("StockQuantity", "Stock Qty");
            dgvExpiryReport.Columns.Add("ExpiryDate", "Expiry Date");
            dgvExpiryReport.Columns.Add("PurchasePrice", "Purchase Price");
            dgvExpiryReport.Columns.Add("TotalValue", "Total Value");
            dgvExpiryReport.Columns.Add("DaysDifference", "Days Difference");

            // Configure columns
            dgvExpiryReport.Columns["ItemID"].Width = 80;
            dgvExpiryReport.Columns["ItemName"].Width = 200;
            dgvExpiryReport.Columns["Category"].Width = 120;
            dgvExpiryReport.Columns["StockQuantity"].Width = 100;
            dgvExpiryReport.Columns["ExpiryDate"].Width = 100;
            dgvExpiryReport.Columns["PurchasePrice"].Width = 120;
            dgvExpiryReport.Columns["TotalValue"].Width = 120;
            dgvExpiryReport.Columns["DaysDifference"].Width = 120;

            // Set alignment
            dgvExpiryReport.Columns["StockQuantity"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvExpiryReport.Columns["PurchasePrice"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvExpiryReport.Columns["TotalValue"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvExpiryReport.Columns["DaysDifference"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Set header alignment
            dgvExpiryReport.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvExpiryReport.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
        }

        private void CalculateTotals()
        {
            int totalItems = 0;
            decimal totalValue = 0;
            int expiredItems = 0;
            int nearExpiryItems = 0;

            if (expiryData != null && expiryData.Rows.Count > 0)
            {
                totalItems = expiryData.Rows.Count;
                foreach (DataRow row in expiryData.Rows)
                {
                    totalValue += Convert.ToDecimal(row["TotalValue"]);
                    
                    DateTime expiryDate = Convert.ToDateTime(row["ExpiryDate"]);
                    if (expiryDate < DateTime.Now)
                    {
                        expiredItems++;
                    }
                    else
                    {
                        nearExpiryItems++;
                    }
                }
            }

            lblTotalItems.Text = "Total Items: " + totalItems.ToString();
            lblTotalValue.Text = "Total Value: " + totalValue.ToString("N2");
            lblExpiredItems.Text = "Expired: " + expiredItems.ToString();
            lblNearExpiryItems.Text = "Near Expiry: " + nearExpiryItems.ToString();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadExpiryReport();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (expiryData.Rows.Count == 0)
            {
                MessageBox.Show("No data to print.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            PrintDocument pd = new PrintDocument();
            pd.PrintPage += new PrintPageEventHandler(PrintExpiryReport);
            
            PrintDialog printDialog = new PrintDialog();
            printDialog.Document = pd;
            
            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                pd.Print();
            }
        }

        private void PrintExpiryReport(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Font titleFont = new Font("Arial", 16, FontStyle.Bold);
            Font headerFont = new Font("Arial", 10, FontStyle.Bold);
            Font dataFont = new Font("Arial", 9);
            Font summaryFont = new Font("Arial", 10, FontStyle.Bold);

            int yPos = 50;
            int leftMargin = 50;

            // Print title
            string title = "Expiry Report";
            if (cmbCategory.SelectedIndex > 0)
            {
                title += $" - {cmbCategory.Text}";
            }
            
            if (rbNearExpiry.Checked)
                title += " (Near Expiry)";
            else if (rbExpired.Checked)
                title += " (Expired Items)";
            else
                title += " (All Items)";
                
            title += $" (Next {(int)numDaysThreshold.Value} days)";
            g.DrawString(title, titleFont, Brushes.Black, leftMargin, yPos);
            yPos += 30;

            // Print date
            g.DrawString($"Generated on: {DateTime.Now:dd/MM/yyyy HH:mm}", dataFont, Brushes.Black, leftMargin, yPos);
            yPos += 30;

            // Print headers
            string[] headers = { "Item Name", "Category", "Stock Qty", "Expiry Date", "Value", "Status" };
            int[] columnWidths = { 200, 120, 80, 100, 100, 80 };
            int xPos = leftMargin;

            for (int i = 0; i < headers.Length; i++)
            {
                g.DrawString(headers[i], headerFont, Brushes.Black, xPos, yPos);
                xPos += columnWidths[i];
            }
            yPos += 20;

            // Print data
            foreach (DataRow row in expiryData.Rows)
            {
                if (yPos > e.MarginBounds.Bottom - 100)
                {
                    e.HasMorePages = true;
                    return;
                }

                xPos = leftMargin;
                
                // Item Name
                g.DrawString(row["ItemName"].ToString(), dataFont, Brushes.Black, xPos, yPos);
                xPos += columnWidths[0];

                // Category
                g.DrawString(row["Category"].ToString(), dataFont, Brushes.Black, xPos, yPos);
                xPos += columnWidths[1];

                // Stock Quantity
                g.DrawString(row["StockQuantity"].ToString(), dataFont, Brushes.Black, xPos, yPos);
                xPos += columnWidths[2];

                // Expiry Date
                g.DrawString(Convert.ToDateTime(row["ExpiryDate"]).ToString("dd/MM/yyyy"), dataFont, Brushes.Black, xPos, yPos);
                xPos += columnWidths[3];

                // Value
                g.DrawString(Convert.ToDecimal(row["TotalValue"]).ToString("N2"), dataFont, Brushes.Black, xPos, yPos);
                xPos += columnWidths[4];

                // Status
                DateTime expiryDate = Convert.ToDateTime(row["ExpiryDate"]);
                if (expiryDate < DateTime.Now)
                {
                    g.DrawString("EXPIRED", dataFont, Brushes.Red, xPos, yPos);
                }
                else
                {
                    g.DrawString("NEAR EXPIRY", dataFont, Brushes.Orange, xPos, yPos);
                }

                yPos += 15;
            }

            // Print summary
            yPos += 20;
            g.DrawString($"Total Items: {expiryData.Rows.Count}", summaryFont, Brushes.Black, leftMargin, yPos);
            yPos += 15;
            
            decimal totalValue = 0;
            foreach (DataRow row in expiryData.Rows)
            {
                totalValue += Convert.ToDecimal(row["TotalValue"]);
            }
            g.DrawString($"Total Value: {totalValue:N2}", summaryFont, Brushes.Black, leftMargin, yPos);
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (expiryData.Rows.Count == 0)
            {
                MessageBox.Show("No data to export.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "CSV files (*.csv)|*.csv";
            saveDialog.FileName = $"ExpiryReport_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

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
                csv.AppendLine("Item Name,Category,Stock Quantity,Expiry Date,Purchase Price,Total Value,Status");
                
                // Add data
                foreach (DataRow row in expiryData.Rows)
                {
                    string itemName = row["ItemName"].ToString().Replace(",", ";");
                    string category = row["Category"].ToString().Replace(",", ";");
                    string stockQty = row["StockQuantity"].ToString();
                    string expiryDate = Convert.ToDateTime(row["ExpiryDate"]).ToString("dd/MM/yyyy");
                    string purchasePrice = Convert.ToDecimal(row["PurchasePrice"]).ToString("N2");
                    string totalValue = Convert.ToDecimal(row["TotalValue"]).ToString("N2");
                    
                    DateTime expDate = Convert.ToDateTime(row["ExpiryDate"]);
                    string status = expDate < DateTime.Now ? "EXPIRED" : "NEAR EXPIRY";

                    csv.AppendLine($"{itemName},{category},{stockQty},{expiryDate},{purchasePrice},{totalValue},{status}");
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

        private void cmbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadExpiryReport();
        }

        private void numDaysThreshold_ValueChanged(object sender, EventArgs e)
        {
            LoadExpiryReport();
        }

        private void rbNearExpiry_CheckedChanged(object sender, EventArgs e)
        {
            LoadExpiryReport();
        }

        private void rbExpired_CheckedChanged(object sender, EventArgs e)
        {
            LoadExpiryReport();
        }

        private void rbAll_CheckedChanged(object sender, EventArgs e)
        {
            LoadExpiryReport();
        }
    }
} 