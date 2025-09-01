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
    public partial class ExpiryAlert : Form
    {
        private DataTable alertData;

        public ExpiryAlert()
        {
            InitializeComponent();
            InitializeForm();
            LoadExpiryAlerts();
        }

        private void InitializeForm()
        {
            // Load categories
            LoadCategories();
            
            // Initialize alert data table
            alertData = new DataTable();
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

        private void LoadExpiryAlerts()
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

                // Add expiry filter
                query += @" AND (
                                (i.ExpiryDate < GETDATE()) OR 
                                (i.ExpiryDate BETWEEN GETDATE() AND DATEADD(day, @DaysThreshold, GETDATE()))
                            )";

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

                alertData = DatabaseConnection.ExecuteQuery(query, parameters);
                ShowAlertData();
                CalculateTotals();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading expiry alerts: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowAlertData()
        {
            SetupDataGridView();
            
            dgvExpiryAlert.Rows.Clear();

            if (alertData != null && alertData.Rows.Count > 0)
            {
                foreach (DataRow row in alertData.Rows)
                {
                    int rowIndex = dgvExpiryAlert.Rows.Add();
                    dgvExpiryAlert.Rows[rowIndex].Cells["ItemID"].Value = row["ItemID"];
                    dgvExpiryAlert.Rows[rowIndex].Cells["ItemName"].Value = row["ItemName"];
                    dgvExpiryAlert.Rows[rowIndex].Cells["Category"].Value = row["Category"];
                    dgvExpiryAlert.Rows[rowIndex].Cells["StockQuantity"].Value = row["StockQuantity"];
                    dgvExpiryAlert.Rows[rowIndex].Cells["ExpiryDate"].Value = Convert.ToDateTime(row["ExpiryDate"]).ToString("dd/MM/yyyy");
                    dgvExpiryAlert.Rows[rowIndex].Cells["PurchasePrice"].Value = Convert.ToDecimal(row["PurchasePrice"]).ToString("N2");
                    dgvExpiryAlert.Rows[rowIndex].Cells["TotalValue"].Value = Convert.ToDecimal(row["TotalValue"]).ToString("N2");
                    dgvExpiryAlert.Rows[rowIndex].Cells["DaysDifference"].Value = row["DaysDifference"];

                    // Color coding based on expiry status
                    DateTime expiryDate = Convert.ToDateTime(row["ExpiryDate"]);
                    int daysDifference = Convert.ToInt32(row["DaysDifference"]);
                    
                    if (expiryDate < DateTime.Now)
                    {
                        dgvExpiryAlert.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightCoral;
                        dgvExpiryAlert.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.DarkRed;
                    }
                    else if (daysDifference <= 7)
                    {
                        dgvExpiryAlert.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightCoral;
                        dgvExpiryAlert.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.DarkRed;
                    }
                    else if (daysDifference <= 30)
                    {
                        dgvExpiryAlert.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightYellow;
                        dgvExpiryAlert.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.DarkOrange;
                    }
                }
            }
        }

        private void SetupDataGridView()
        {
            dgvExpiryAlert.Columns.Clear();
            
            dgvExpiryAlert.Columns.Add("ItemID", "Item ID");
            dgvExpiryAlert.Columns.Add("ItemName", "Item Name");
            dgvExpiryAlert.Columns.Add("Category", "Category");
            dgvExpiryAlert.Columns.Add("StockQuantity", "Stock Qty");
            dgvExpiryAlert.Columns.Add("ExpiryDate", "Expiry Date");
            dgvExpiryAlert.Columns.Add("PurchasePrice", "Purchase Price");
            dgvExpiryAlert.Columns.Add("TotalValue", "Total Value");
            dgvExpiryAlert.Columns.Add("DaysDifference", "Days Difference");

            // Configure columns
            dgvExpiryAlert.Columns["ItemID"].Width = 80;
            dgvExpiryAlert.Columns["ItemName"].Width = 200;
            dgvExpiryAlert.Columns["Category"].Width = 120;
            dgvExpiryAlert.Columns["StockQuantity"].Width = 100;
            dgvExpiryAlert.Columns["ExpiryDate"].Width = 100;
            dgvExpiryAlert.Columns["PurchasePrice"].Width = 120;
            dgvExpiryAlert.Columns["TotalValue"].Width = 120;
            dgvExpiryAlert.Columns["DaysDifference"].Width = 120;

            // Set alignment
            dgvExpiryAlert.Columns["StockQuantity"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvExpiryAlert.Columns["PurchasePrice"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvExpiryAlert.Columns["TotalValue"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvExpiryAlert.Columns["DaysDifference"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Set header alignment
            dgvExpiryAlert.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvExpiryAlert.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
        }

        private void CalculateTotals()
        {
            int totalItems = 0;
            decimal totalValue = 0;

            if (alertData != null && alertData.Rows.Count > 0)
            {
                totalItems = alertData.Rows.Count;
                foreach (DataRow row in alertData.Rows)
                {
                    totalValue += Convert.ToDecimal(row["TotalValue"]);
                }
            }

            lblTotalItems.Text = "Total Items: " + totalItems.ToString();
            lblTotalValue.Text = "Total Value: " + totalValue.ToString("N2");
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadExpiryAlerts();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (alertData.Rows.Count == 0)
            {
                MessageBox.Show("No data to print.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            PrintDocument pd = new PrintDocument();
            pd.PrintPage += new PrintPageEventHandler(PrintExpiryAlert);
            
            PrintDialog printDialog = new PrintDialog();
            printDialog.Document = pd;
            
            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                pd.Print();
            }
        }

        private void PrintExpiryAlert(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Font titleFont = new Font("Arial", 16, FontStyle.Bold);
            Font headerFont = new Font("Arial", 10, FontStyle.Bold);
            Font dataFont = new Font("Arial", 9);
            Font summaryFont = new Font("Arial", 10, FontStyle.Bold);

            int yPos = 50;
            int leftMargin = 50;

            // Print title
            string title = "Expiry Alert Report";
            if (cmbCategory.SelectedIndex > 0)
            {
                title += $" - {cmbCategory.Text}";
            }
            title += $" (Next {(int)numDaysThreshold.Value} days)";
            g.DrawString(title, titleFont, Brushes.Black, leftMargin, yPos);
            yPos += 30;

            // Print date
            g.DrawString($"Generated on: {DateTime.Now:dd/MM/yyyy HH:mm}", dataFont, Brushes.Black, leftMargin, yPos);
            yPos += 30;

            // Print headers
            string[] headers = { "Item Name", "Category", "Stock Qty", "Expiry Date", "Status" };
            int[] columnWidths = { 200, 120, 80, 100, 100 };
            int xPos = leftMargin;

            for (int i = 0; i < headers.Length; i++)
            {
                g.DrawString(headers[i], headerFont, Brushes.Black, xPos, yPos);
                xPos += columnWidths[i];
            }
            yPos += 20;

            // Print data
            foreach (DataRow row in alertData.Rows)
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
                if (row["ExpiryDate"] != DBNull.Value)
                {
                    DateTime expiryDate = Convert.ToDateTime(row["ExpiryDate"]);
                    g.DrawString(expiryDate.ToString("dd/MM/yyyy"), dataFont, Brushes.Black, xPos, yPos);
                }
                xPos += columnWidths[3];

                // Status
                if (row["ExpiryDate"] != DBNull.Value)
                {
                    DateTime expiryDate = Convert.ToDateTime(row["ExpiryDate"]);
                    if (expiryDate < DateTime.Now)
                    {
                        g.DrawString("EXPIRED", dataFont, Brushes.Red, xPos, yPos);
                    }
                    else if (expiryDate <= DateTime.Now.AddDays(30))
                    {
                        g.DrawString("NEAR EXPIRY", dataFont, Brushes.Orange, xPos, yPos);
                    }
                    else
                    {
                        g.DrawString("OK", dataFont, Brushes.Green, xPos, yPos);
                    }
                }

                yPos += 15;
            }

            // Print summary
            yPos += 20;
            g.DrawString($"Total Items: {alertData.Rows.Count}", summaryFont, Brushes.Black, leftMargin, yPos);
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (alertData.Rows.Count == 0)
            {
                MessageBox.Show("No data to export.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "CSV files (*.csv)|*.csv";
            saveDialog.FileName = $"ExpiryAlert_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

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
                csv.AppendLine("Item Name,Category,Stock Quantity,Expiry Date,Status");
                
                // Add data
                foreach (DataRow row in alertData.Rows)
                {
                    string itemName = row["ItemName"].ToString().Replace(",", ";");
                    string category = row["Category"].ToString().Replace(",", ";");
                    string stockQty = row["StockQuantity"].ToString();
                    string expiryDate = "";
                    string status = "";

                    if (row["ExpiryDate"] != DBNull.Value)
                    {
                        DateTime expDate = Convert.ToDateTime(row["ExpiryDate"]);
                        expiryDate = expDate.ToString("dd/MM/yyyy");
                        
                        if (expDate < DateTime.Now)
                        {
                            status = "EXPIRED";
                        }
                        else if (expDate <= DateTime.Now.AddDays(30))
                        {
                            status = "NEAR EXPIRY";
                        }
                        else
                        {
                            status = "OK";
                        }
                    }

                    csv.AppendLine($"{itemName},{category},{stockQty},{expiryDate},{status}");
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
            LoadExpiryAlerts();
        }

        private void numDaysThreshold_ValueChanged(object sender, EventArgs e)
        {
            LoadExpiryAlerts();
        }
    }
} 