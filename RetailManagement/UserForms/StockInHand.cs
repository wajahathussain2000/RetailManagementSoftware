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
    public partial class StockInHand : Form
    {
        private DataTable reportData;

        public StockInHand()
        {
            InitializeComponent();
            LoadDistributors();
            LoadPrincipals();
            InitializeForm();
        }

        private void InitializeForm()
        {
            // Set default radio button
            radioButton1.Checked = true;
            
            // Initialize report data table
            reportData = new DataTable();
        }

        private void LoadDistributors()
        {
            try
            {
                string query = "SELECT DISTINCT Category FROM Items WHERE IsActive = 1 ORDER BY Category";
                DataTable dt = DatabaseConnection.ExecuteQuery(query);
                comboBox1.DataSource = dt;
                comboBox1.DisplayMember = "Category";
                comboBox1.ValueMember = "Category";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading distributors: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadPrincipals()
        {
            try
            {
                string query = "SELECT DISTINCT Category FROM Items WHERE IsActive = 1 ORDER BY Category";
                DataTable dt = DatabaseConnection.ExecuteQuery(query);
                comboBox2.DataSource = dt;
                comboBox2.DisplayMember = "Category";
                comboBox2.ValueMember = "Category";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading principals: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerateReport()
        {
            try
            {
                string query = "";
                string reportTitle = "";

                if (radioButton1.Checked) // Stock Summary
                {
                    query = @"SELECT 
                                i.ItemID,
                                i.ItemName,
                                i.Category,
                                i.Price,
                                i.StockQuantity,
                                i.Price * i.StockQuantity as StockValue
                               FROM Items i
                               WHERE i.IsActive = 1";
                    reportTitle = "Stock Summary Report";
                }
                else if (radioButton2.Checked) // Stock Value Wise
                {
                    query = @"SELECT 
                                i.ItemID,
                                i.ItemName,
                                i.Category,
                                i.Price,
                                i.StockQuantity,
                                i.Price * i.StockQuantity as StockValue
                               FROM Items i
                               WHERE i.IsActive = 1
                               ORDER BY i.Price * i.StockQuantity DESC";
                    reportTitle = "Stock Value Wise Report";
                }
                else if (radioButton3.Checked) // Re-Order Level Report
                {
                    query = @"SELECT 
                                i.ItemID,
                                i.ItemName,
                                i.Category,
                                i.Price,
                                i.StockQuantity,
                                i.Price * i.StockQuantity as StockValue
                               FROM Items i
                               WHERE i.IsActive = 1 AND i.StockQuantity <= 10";
                    reportTitle = "Re-Order Level Report";
                }
                else if (radioButton4.Checked) // Near Expiry Report
                {
                    // For now, using a placeholder since we don't have expiry dates
                    query = @"SELECT 
                                i.ItemID,
                                i.ItemName,
                                i.Category,
                                i.Price,
                                i.StockQuantity,
                                i.Price * i.StockQuantity as StockValue
                               FROM Items i
                               WHERE i.IsActive = 1";
                    reportTitle = "Near Expiry Report";
                }

                // Add distributor/principal filter if selected
                if (comboBox1.SelectedValue != null && comboBox1.SelectedValue.ToString() != "")
                {
                    query += " AND i.Category = @Distributor";
                }

                query += " ORDER BY i.Category, i.ItemName";

                SqlParameter[] parameters = null;
                if (comboBox1.SelectedValue != null && comboBox1.SelectedValue.ToString() != "")
                {
                    parameters = new SqlParameter[] { new SqlParameter("@Distributor", comboBox1.SelectedValue.ToString()) };
                }

                reportData = DatabaseConnection.ExecuteQuery(query, parameters);
                
                // Show report in a new form or print directly
                ShowReport(reportTitle, reportData);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating report: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowReport(string title, DataTable data)
        {
            // Create a simple report display form
            Form reportForm = new Form();
            reportForm.Text = title;
            reportForm.Size = new Size(800, 600);
            reportForm.StartPosition = FormStartPosition.CenterScreen;

            DataGridView dgv = new DataGridView();
            dgv.Dock = DockStyle.Fill;
            dgv.AutoGenerateColumns = true;
            dgv.DataSource = data;

            reportForm.Controls.Add(dgv);
            reportForm.Show();
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            GenerateReport();
        }

        private void PrintReport(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Font titleFont = new Font("Arial", 16, FontStyle.Bold);
            Font headerFont = new Font("Arial", 10, FontStyle.Bold);
            Font dataFont = new Font("Arial", 9);

            int yPos = 50;
            g.DrawString("Stock In Hand Report", titleFont, Brushes.Black, 50, yPos);
            yPos += 30;
            g.DrawString("Generated on: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"), dataFont, Brushes.Black, 50, yPos);
            yPos += 30;

            // Print headers
            g.DrawString("Item Name", headerFont, Brushes.Black, 50, yPos);
            g.DrawString("Category", headerFont, Brushes.Black, 200, yPos);
            g.DrawString("Price", headerFont, Brushes.Black, 300, yPos);
            g.DrawString("Quantity", headerFont, Brushes.Black, 400, yPos);
            g.DrawString("Value", headerFont, Brushes.Black, 500, yPos);
            yPos += 20;

            // Print data
            if (reportData != null)
            {
                foreach (DataRow row in reportData.Rows)
                {
                    if (yPos > e.PageBounds.Height - 100) break;

                    g.DrawString(row["ItemName"].ToString(), dataFont, Brushes.Black, 50, yPos);
                    g.DrawString(row["Category"].ToString(), dataFont, Brushes.Black, 200, yPos);
                    g.DrawString(Convert.ToDecimal(row["Price"]).ToString("N2"), dataFont, Brushes.Black, 300, yPos);
                    g.DrawString(row["StockQuantity"].ToString(), dataFont, Brushes.Black, 400, yPos);
                    g.DrawString(Convert.ToDecimal(row["StockValue"]).ToString("N2"), dataFont, Brushes.Black, 500, yPos);
                    yPos += 15;
                }
            }
        }



        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
