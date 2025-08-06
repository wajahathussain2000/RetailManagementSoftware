using RetailManagement.Database;
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;

namespace RetailManagement.UserForms
{
    public partial class LowStockAlertReport : Form
    {
        public LowStockAlertReport()
        {
            InitializeComponent();
            LoadCategories();
            LoadCompanies();
            LoadLowStockAlertReport();
        }

        private void LoadCategories()
        {
            try
            {
                string query = "SELECT DISTINCT Category FROM Items ORDER BY Category";
                DataTable dt = DatabaseConnection.ExecuteQuery(query);
                cmbCategory.Items.Clear();
                cmbCategory.Items.Add("All Categories");
                foreach (DataRow row in dt.Rows)
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

        private void LoadCompanies()
        {
            try
            {
                string query = "SELECT DISTINCT CompanyName FROM Companies ORDER BY CompanyName";
                DataTable dt = DatabaseConnection.ExecuteQuery(query);
                cmbCompany.Items.Clear();
                cmbCompany.Items.Add("All Companies");
                foreach (DataRow row in dt.Rows)
                {
                    cmbCompany.Items.Add(row["CompanyName"].ToString());
                }
                cmbCompany.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading companies: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadLowStockAlertReport()
        {
            try
            {
                string categoryFilter = cmbCategory.SelectedItem.ToString();
                string companyFilter = cmbCompany.SelectedItem.ToString();

                string query = @"
                    SELECT 
                        i.ItemID,
                        i.ItemName,
                        i.Category,
                        c.CompanyName,
                        i.Quantity,
                        i.ReorderLevel,
                        (i.ReorderLevel - i.Quantity) as Shortage,
                        i.PurchasePrice,
                        i.SellingPrice,
                        i.ExpiryDate
                    FROM Items i
                    LEFT JOIN Companies c ON i.CompanyID = c.CompanyID
                    WHERE i.Quantity <= i.ReorderLevel";

                if (categoryFilter != "All Categories")
                {
                    query += " AND i.Category = @Category";
                }

                if (companyFilter != "All Companies")
                {
                    query += " AND c.CompanyName = @Company";
                }

                query += " ORDER BY (i.ReorderLevel - i.Quantity) DESC, i.Category, i.ItemName";

                System.Data.SqlClient.SqlParameter[] parameters = null;
                if (categoryFilter != "All Categories" && companyFilter != "All Companies")
                {
                    parameters = new System.Data.SqlClient.SqlParameter[] 
                    {
                        new System.Data.SqlClient.SqlParameter("@Category", categoryFilter),
                        new System.Data.SqlClient.SqlParameter("@Company", companyFilter)
                    };
                }
                else if (categoryFilter != "All Categories")
                {
                    parameters = new System.Data.SqlClient.SqlParameter[] 
                    {
                        new System.Data.SqlClient.SqlParameter("@Category", categoryFilter)
                    };
                }
                else if (companyFilter != "All Companies")
                {
                    parameters = new System.Data.SqlClient.SqlParameter[] 
                    {
                        new System.Data.SqlClient.SqlParameter("@Company", companyFilter)
                    };
                }

                DataTable dt = DatabaseConnection.ExecuteQuery(query, parameters);
                dgvLowStockAlert.DataSource = dt;

                // Configure DataGridView
                dgvLowStockAlert.Columns["ItemID"].HeaderText = "Item ID";
                dgvLowStockAlert.Columns["ItemName"].HeaderText = "Item Name";
                dgvLowStockAlert.Columns["Category"].HeaderText = "Category";
                dgvLowStockAlert.Columns["CompanyName"].HeaderText = "Company";
                dgvLowStockAlert.Columns["Quantity"].HeaderText = "Current Stock";
                dgvLowStockAlert.Columns["ReorderLevel"].HeaderText = "Reorder Level";
                dgvLowStockAlert.Columns["Shortage"].HeaderText = "Shortage";
                dgvLowStockAlert.Columns["PurchasePrice"].HeaderText = "Purchase Price";
                dgvLowStockAlert.Columns["SellingPrice"].HeaderText = "Selling Price";
                dgvLowStockAlert.Columns["ExpiryDate"].HeaderText = "Expiry Date";

                dgvLowStockAlert.Columns["PurchasePrice"].DefaultCellStyle.Format = "N2";
                dgvLowStockAlert.Columns["SellingPrice"].DefaultCellStyle.Format = "N2";

                dgvLowStockAlert.Columns["PurchasePrice"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvLowStockAlert.Columns["SellingPrice"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvLowStockAlert.Columns["Quantity"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvLowStockAlert.Columns["ReorderLevel"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvLowStockAlert.Columns["Shortage"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvLowStockAlert.Columns["ExpiryDate"].DefaultCellStyle.Format = "dd/MM/yyyy";

                // Color coding for shortage levels
                dgvLowStockAlert.CellFormatting += (sender, e) =>
                {
                    if (e.ColumnIndex == dgvLowStockAlert.Columns["Shortage"].Index && e.Value != null)
                    {
                        int shortage = Convert.ToInt32(e.Value);
                        if (shortage > 10)
                        {
                            e.CellStyle.BackColor = Color.LightCoral;
                            e.CellStyle.ForeColor = Color.DarkRed;
                        }
                        else if (shortage > 5)
                        {
                            e.CellStyle.BackColor = Color.LightYellow;
                            e.CellStyle.ForeColor = Color.DarkOrange;
                        }
                        else
                        {
                            e.CellStyle.BackColor = Color.LightGreen;
                            e.CellStyle.ForeColor = Color.DarkGreen;
                        }
                    }
                };

                // Auto-size columns
                dgvLowStockAlert.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

                CalculateTotals(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading low stock alert report: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CalculateTotals(DataTable dt)
        {
            int totalItems = dt.Rows.Count;
            int totalShortage = 0;
            decimal totalValue = 0;

            foreach (DataRow row in dt.Rows)
            {
                totalShortage += Convert.ToInt32(row["Shortage"]);
                totalValue += Convert.ToDecimal(row["PurchasePrice"]) * Convert.ToInt32(row["Shortage"]);
            }

            lblTotalItems.Text = "Total Items: " + totalItems.ToString();
            lblTotalShortage.Text = "Total Shortage: " + totalShortage.ToString();
            lblTotalValue.Text = "Total Value: ₹" + totalValue.ToString("N2");
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            LoadLowStockAlertReport();
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
            string title = "Low Stock Alert Report";
            g.DrawString(title, titleFont, brush, leftMargin, yPos);
            yPos += 40;

            // Filters
            if (cmbCategory.SelectedItem.ToString() != "All Categories")
            {
                g.DrawString("Category: " + cmbCategory.SelectedItem.ToString(), normalFont, brush, leftMargin, yPos);
                yPos += 15;
            }

            if (cmbCompany.SelectedItem.ToString() != "All Companies")
            {
                g.DrawString("Company: " + cmbCompany.SelectedItem.ToString(), normalFont, brush, leftMargin, yPos);
                yPos += 15;
            }

            yPos += 10;

            // Headers
            string[] headers = { "Item ID", "Item Name", "Category", "Company", "Current Stock", "Reorder Level", "Shortage", "Purchase Price", "Selling Price", "Expiry Date" };
            int[] columnWidths = { 80, 150, 100, 120, 80, 80, 80, 100, 100, 100 };
            int xPos = leftMargin;

            for (int i = 0; i < headers.Length; i++)
            {
                g.DrawString(headers[i], headerFont, brush, xPos, yPos);
                xPos += columnWidths[i];
            }
            yPos += 20;

            // Data
            DataTable dt = (DataTable)dgvLowStockAlert.DataSource;
            decimal totalValue = 0;

            foreach (DataRow row in dt.Rows)
            {
                if (yPos > e.PageBounds.Height - 100)
                {
                    e.HasMorePages = true;
                    return;
                }

                xPos = leftMargin;
                g.DrawString(row["ItemID"].ToString(), normalFont, brush, xPos, yPos);
                xPos += columnWidths[0];

                g.DrawString(row["ItemName"].ToString(), normalFont, brush, xPos, yPos);
                xPos += columnWidths[1];

                g.DrawString(row["Category"].ToString(), normalFont, brush, xPos, yPos);
                xPos += columnWidths[2];

                g.DrawString(row["CompanyName"].ToString(), normalFont, brush, xPos, yPos);
                xPos += columnWidths[3];

                g.DrawString(row["Quantity"].ToString(), normalFont, brush, xPos, yPos);
                xPos += columnWidths[4];

                g.DrawString(row["ReorderLevel"].ToString(), normalFont, brush, xPos, yPos);
                xPos += columnWidths[5];

                int shortage = Convert.ToInt32(row["Shortage"]);
                g.DrawString(shortage.ToString(), normalFont, brush, xPos, yPos);
                xPos += columnWidths[6];

                g.DrawString("₹" + Convert.ToDecimal(row["PurchasePrice"]).ToString("N2"), normalFont, brush, xPos, yPos);
                xPos += columnWidths[7];

                g.DrawString("₹" + Convert.ToDecimal(row["SellingPrice"]).ToString("N2"), normalFont, brush, xPos, yPos);
                xPos += columnWidths[8];

                if (row["ExpiryDate"] != DBNull.Value)
                {
                    g.DrawString(Convert.ToDateTime(row["ExpiryDate"]).ToString("dd/MM/yyyy"), normalFont, brush, xPos, yPos);
                }

                totalValue += Convert.ToDecimal(row["PurchasePrice"]) * shortage;
                yPos += 15;
            }

            // Total
            yPos += 10;
            g.DrawString("Total Value: ₹" + totalValue.ToString("N2"), headerFont, brush, leftMargin, yPos);
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "CSV files (*.csv)|*.csv";
                sfd.FileName = "LowStockAlertReport_" + DateTime.Now.ToString("yyyyMMdd") + ".csv";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    DataTable dt = (DataTable)dgvLowStockAlert.DataSource;
                    StringBuilder sb = new StringBuilder();

                    // Headers
                    sb.AppendLine("Item ID,Item Name,Category,Company,Current Stock,Reorder Level,Shortage,Purchase Price,Selling Price,Expiry Date");

                    // Data
                    foreach (DataRow row in dt.Rows)
                    {
                        sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}",
                            row["ItemID"].ToString().Replace(",", ";"),
                            row["ItemName"].ToString().Replace(",", ";"),
                            row["Category"].ToString().Replace(",", ";"),
                            row["CompanyName"].ToString().Replace(",", ";"),
                            row["Quantity"].ToString(),
                            row["ReorderLevel"].ToString(),
                            row["Shortage"].ToString(),
                            Convert.ToDecimal(row["PurchasePrice"]).ToString("N2"),
                            Convert.ToDecimal(row["SellingPrice"]).ToString("N2"),
                            row["ExpiryDate"] != DBNull.Value ? Convert.ToDateTime(row["ExpiryDate"]).ToString("dd/MM/yyyy") : ""));
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

        private void cmbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadLowStockAlertReport();
        }

        private void cmbCompany_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadLowStockAlertReport();
        }
    }
} 