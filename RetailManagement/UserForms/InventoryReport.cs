using RetailManagement.Database;
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;

namespace RetailManagement.UserForms
{
    public partial class InventoryReport : Form
    {
        public InventoryReport()
        {
            InitializeComponent();
            LoadCategories();
            LoadCompanies();
            LoadInventoryReport();
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

        private void LoadInventoryReport()
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
                        i.PurchasePrice,
                        i.SellingPrice,
                        (i.Quantity * i.PurchasePrice) as StockValue,
                        i.ExpiryDate
                    FROM Items i
                    LEFT JOIN Companies c ON i.CompanyID = c.CompanyID
                    WHERE 1=1";

                if (categoryFilter != "All Categories")
                {
                    query += " AND i.Category = @Category";
                }

                if (companyFilter != "All Companies")
                {
                    query += " AND c.CompanyName = @Company";
                }

                query += " ORDER BY i.Category, i.ItemName";

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
                dgvInventoryReport.DataSource = dt;

                // Configure DataGridView
                dgvInventoryReport.Columns["ItemID"].HeaderText = "Item ID";
                dgvInventoryReport.Columns["ItemName"].HeaderText = "Item Name";
                dgvInventoryReport.Columns["Category"].HeaderText = "Category";
                dgvInventoryReport.Columns["CompanyName"].HeaderText = "Company";
                dgvInventoryReport.Columns["Quantity"].HeaderText = "Stock Qty";
                dgvInventoryReport.Columns["PurchasePrice"].HeaderText = "Purchase Price";
                dgvInventoryReport.Columns["SellingPrice"].HeaderText = "Selling Price";
                dgvInventoryReport.Columns["StockValue"].HeaderText = "Stock Value";
                dgvInventoryReport.Columns["ExpiryDate"].HeaderText = "Expiry Date";

                dgvInventoryReport.Columns["PurchasePrice"].DefaultCellStyle.Format = "N2";
                dgvInventoryReport.Columns["SellingPrice"].DefaultCellStyle.Format = "N2";
                dgvInventoryReport.Columns["StockValue"].DefaultCellStyle.Format = "N2";
                dgvInventoryReport.Columns["ExpiryDate"].DefaultCellStyle.Format = "dd/MM/yyyy";

                dgvInventoryReport.Columns["PurchasePrice"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvInventoryReport.Columns["SellingPrice"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvInventoryReport.Columns["StockValue"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvInventoryReport.Columns["Quantity"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                // Auto-size columns
                dgvInventoryReport.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

                CalculateTotals(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading inventory report: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CalculateTotals(DataTable dt)
        {
            int totalItems = dt.Rows.Count;
            int totalQuantity = 0;
            decimal totalStockValue = 0;

            foreach (DataRow row in dt.Rows)
            {
                totalQuantity += Convert.ToInt32(row["Quantity"]);
                totalStockValue += Convert.ToDecimal(row["StockValue"]);
            }

            lblTotalItems.Text = "Total Items: " + totalItems.ToString();
            lblTotalQuantity.Text = "Total Quantity: " + totalQuantity.ToString();
            lblTotalValue.Text = "Total Stock Value: ₹" + totalStockValue.ToString("N2");
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            LoadInventoryReport();
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
            string title = "Inventory Report";
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
            string[] headers = { "Item ID", "Item Name", "Category", "Company", "Qty", "Purchase Price", "Selling Price", "Stock Value", "Expiry Date" };
            int[] columnWidths = { 80, 150, 100, 120, 60, 100, 100, 100, 100 };
            int xPos = leftMargin;

            for (int i = 0; i < headers.Length; i++)
            {
                g.DrawString(headers[i], headerFont, brush, xPos, yPos);
                xPos += columnWidths[i];
            }
            yPos += 20;

            // Data
            DataTable dt = (DataTable)dgvInventoryReport.DataSource;
            decimal totalStockValue = 0;

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

                g.DrawString("₹" + Convert.ToDecimal(row["PurchasePrice"]).ToString("N2"), normalFont, brush, xPos, yPos);
                xPos += columnWidths[5];

                g.DrawString("₹" + Convert.ToDecimal(row["SellingPrice"]).ToString("N2"), normalFont, brush, xPos, yPos);
                xPos += columnWidths[6];

                decimal stockValue = Convert.ToDecimal(row["StockValue"]);
                totalStockValue += stockValue;
                g.DrawString("₹" + stockValue.ToString("N2"), normalFont, brush, xPos, yPos);
                xPos += columnWidths[7];

                if (row["ExpiryDate"] != DBNull.Value)
                {
                    g.DrawString(Convert.ToDateTime(row["ExpiryDate"]).ToString("dd/MM/yyyy"), normalFont, brush, xPos, yPos);
                }

                yPos += 15;
            }

            // Total
            yPos += 10;
            g.DrawString("Total Stock Value: ₹" + totalStockValue.ToString("N2"), headerFont, brush, leftMargin, yPos);
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "CSV files (*.csv)|*.csv";
                sfd.FileName = "InventoryReport_" + DateTime.Now.ToString("yyyyMMdd") + ".csv";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    DataTable dt = (DataTable)dgvInventoryReport.DataSource;
                    StringBuilder sb = new StringBuilder();

                    // Headers
                    sb.AppendLine("Item ID,Item Name,Category,Company,Quantity,Purchase Price,Selling Price,Stock Value,Expiry Date");

                    // Data
                    foreach (DataRow row in dt.Rows)
                    {
                        sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}",
                            row["ItemID"].ToString().Replace(",", ";"),
                            row["ItemName"].ToString().Replace(",", ";"),
                            row["Category"].ToString().Replace(",", ";"),
                            row["CompanyName"].ToString().Replace(",", ";"),
                            row["Quantity"].ToString(),
                            Convert.ToDecimal(row["PurchasePrice"]).ToString("N2"),
                            Convert.ToDecimal(row["SellingPrice"]).ToString("N2"),
                            Convert.ToDecimal(row["StockValue"]).ToString("N2"),
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
            LoadInventoryReport();
        }

        private void cmbCompany_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadInventoryReport();
        }
    }
} 