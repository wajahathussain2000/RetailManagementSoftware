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
    public partial class CustomerBalance : Form
    {
        public CustomerBalance()
        {
            InitializeComponent();
            LoadAllCustomerBalances();
            SetupDataGridView();
        }

        private void SetupDataGridView()
        {
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.Columns.Clear();

            dataGridView1.Columns.Add("CustomerID", "Customer ID");
            dataGridView1.Columns.Add("CustomerName", "Customer Name");
            dataGridView1.Columns.Add("Phone", "Phone");
            dataGridView1.Columns.Add("TotalSales", "Total Sales");
            dataGridView1.Columns.Add("TotalPayments", "Total Payments");
            dataGridView1.Columns.Add("Balance", "Balance");

            dataGridView1.Columns["CustomerID"].DataPropertyName = "CustomerID";
            dataGridView1.Columns["CustomerName"].DataPropertyName = "CustomerName";
            dataGridView1.Columns["Phone"].DataPropertyName = "Phone";
            dataGridView1.Columns["TotalSales"].DataPropertyName = "TotalSales";
            dataGridView1.Columns["TotalPayments"].DataPropertyName = "TotalPayments";
            dataGridView1.Columns["Balance"].DataPropertyName = "Balance";
        }

        private void LoadAllCustomerBalances()
        {
            try
            {
                string query = @"SELECT 
                                c.CustomerID,
                                c.CustomerName,
                                c.Phone,
                                ISNULL(SUM(s.NetAmount), 0) as TotalSales,
                                ISNULL(SUM(cp.Amount), 0) as TotalPayments,
                                ISNULL(SUM(s.NetAmount), 0) - ISNULL(SUM(cp.Amount), 0) as Balance
                               FROM Customers c
                               LEFT JOIN Sales s ON c.CustomerID = s.CustomerID AND s.IsActive = 1
                               LEFT JOIN CustomerPayments cp ON c.CustomerID = cp.CustomerID
                               WHERE c.IsActive = 1
                               GROUP BY c.CustomerID, c.CustomerName, c.Phone
                               ORDER BY c.CustomerName";

                DataTable dt = DatabaseConnection.ExecuteQuery(query);
                dataGridView1.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading customer balances: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCustomerName.Text))
            {
                LoadAllCustomerBalances();
                return;
            }

            try
            {
                string query = @"SELECT 
                                c.CustomerID,
                                c.CustomerName,
                                c.Phone,
                                ISNULL(SUM(s.NetAmount), 0) as TotalSales,
                                ISNULL(SUM(cp.Amount), 0) as TotalPayments,
                                ISNULL(SUM(s.NetAmount), 0) - ISNULL(SUM(cp.Amount), 0) as Balance
                               FROM Customers c
                               LEFT JOIN Sales s ON c.CustomerID = s.CustomerID AND s.IsActive = 1
                               LEFT JOIN CustomerPayments cp ON c.CustomerID = cp.CustomerID
                               WHERE c.IsActive = 1 AND c.CustomerName LIKE @CustomerName
                               GROUP BY c.CustomerID, c.CustomerName, c.Phone
                               ORDER BY c.CustomerName";

                SqlParameter[] parameters = { new SqlParameter("@CustomerName", "%" + txtCustomerName.Text.Trim() + "%") };
                DataTable dt = DatabaseConnection.ExecuteQuery(query, parameters);
                dataGridView1.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching customer: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            txtCustomerName.Text = "";
            LoadAllCustomerBalances();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                // Simple print functionality - you can enhance this with proper reporting
                if (dataGridView1.Rows.Count > 0)
                {
                    PrintDocument pd = new PrintDocument();
                    pd.PrintPage += PrintPage;
                    pd.Print();
                }
                else
                {
                    MessageBox.Show("No data to print.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error printing: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            // Basic print implementation
            Graphics g = e.Graphics;
            Font titleFont = new Font("Arial", 16, FontStyle.Bold);
            Font headerFont = new Font("Arial", 10, FontStyle.Bold);
            Font dataFont = new Font("Arial", 9);

            int yPos = 50;
            g.DrawString("Customer Balance Report", titleFont, Brushes.Black, 50, yPos);
            yPos += 30;
            g.DrawString("Generated on: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"), dataFont, Brushes.Black, 50, yPos);
            yPos += 30;

            // Print headers
            g.DrawString("Customer Name", headerFont, Brushes.Black, 50, yPos);
            g.DrawString("Phone", headerFont, Brushes.Black, 200, yPos);
            g.DrawString("Balance", headerFont, Brushes.Black, 350, yPos);
            yPos += 20;

            // Print data
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (yPos > e.PageBounds.Height - 100) break;

                g.DrawString(row.Cells["CustomerName"].Value.ToString(), dataFont, Brushes.Black, 50, yPos);
                g.DrawString(row.Cells["Phone"].Value.ToString(), dataFont, Brushes.Black, 200, yPos);
                g.DrawString(row.Cells["Balance"].Value.ToString(), dataFont, Brushes.Black, 350, yPos);
                yPos += 15;
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void pbExit_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
} 