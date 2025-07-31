using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RetailManagement.Database;

namespace RetailManagement.UserForms
{
    public partial class EditBill : Form
    {
        private int selectedSaleID = 0;
        private DataTable saleItems;

        public EditBill()
        {
            InitializeComponent();
            LoadSales();
            SetupDataGridView();
        }

        private void SetupDataGridView()
        {
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.Columns.Clear();

            dataGridView1.Columns.Add("SaleID", "Sale ID");
            dataGridView1.Columns.Add("BillNumber", "Bill Number");
            dataGridView1.Columns.Add("CustomerName", "Customer Name");
            dataGridView1.Columns.Add("SaleDate", "Sale Date");
            dataGridView1.Columns.Add("NetAmount", "Amount");

            dataGridView1.Columns["SaleID"].DataPropertyName = "SaleID";
            dataGridView1.Columns["BillNumber"].DataPropertyName = "BillNumber";
            dataGridView1.Columns["CustomerName"].DataPropertyName = "CustomerName";
            dataGridView1.Columns["SaleDate"].DataPropertyName = "SaleDate";
            dataGridView1.Columns["NetAmount"].DataPropertyName = "NetAmount";
        }

        private void LoadSales()
        {
            try
            {
                string query = @"SELECT 
                                s.SaleID,
                                s.BillNumber,
                                c.CustomerName,
                                s.SaleDate,
                                s.NetAmount
                               FROM Sales s
                               INNER JOIN Customers c ON s.CustomerID = c.CustomerID
                               WHERE s.IsActive = 1
                               ORDER BY s.SaleDate DESC";

                DataTable dt = DatabaseConnection.ExecuteQuery(query);
                dataGridView1.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading sales: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                LoadSales();
                return;
            }

            try
            {
                string query = @"SELECT 
                                s.SaleID,
                                s.BillNumber,
                                c.CustomerName,
                                s.SaleDate,
                                s.NetAmount
                               FROM Sales s
                               INNER JOIN Customers c ON s.CustomerID = c.CustomerID
                               WHERE s.IsActive = 1
                               AND (s.BillNumber LIKE @Search OR c.CustomerName LIKE @Search)
                               ORDER BY s.SaleDate DESC";

                SqlParameter[] parameters = { new SqlParameter("@Search", "%" + txtSearch.Text.Trim() + "%") };
                DataTable dt = DatabaseConnection.ExecuteQuery(query, parameters);
                dataGridView1.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching sales: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                selectedSaleID = Convert.ToInt32(row.Cells["SaleID"].Value);
                LoadSaleDetails(selectedSaleID);
            }
        }

        private void LoadSaleDetails(int saleID)
        {
            try
            {
                // Load sale header
                string saleQuery = @"SELECT s.*, c.CustomerName 
                                   FROM Sales s
                                   INNER JOIN Customers c ON s.CustomerID = c.CustomerID
                                   WHERE s.SaleID = @SaleID";

                SqlParameter[] saleParams = { new SqlParameter("@SaleID", saleID) };
                DataTable saleDt = DatabaseConnection.ExecuteQuery(saleQuery, saleParams);

                if (saleDt.Rows.Count > 0)
                {
                    DataRow saleRow = saleDt.Rows[0];
                    txtBillNumber.Text = saleRow["BillNumber"].ToString();
                    txtCustomerName.Text = saleRow["CustomerName"].ToString();
                    txtSaleDate.Value = Convert.ToDateTime(saleRow["SaleDate"]);
                    txtNetAmount.Text = Convert.ToDecimal(saleRow["NetAmount"]).ToString("N2");
                }

                // Load sale items (for reference only, not editable in this simplified form)
                string itemsQuery = @"SELECT si.*, i.ItemName 
                                    FROM SaleItems si
                                    INNER JOIN Items i ON si.ItemID = i.ItemID
                                    WHERE si.SaleID = @SaleID";

                SqlParameter[] itemsParams = { new SqlParameter("@SaleID", saleID) };
                saleItems = DatabaseConnection.ExecuteQuery(itemsQuery, itemsParams);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading sale details: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedSaleID == 0)
            {
                MessageBox.Show("Please select a sale to edit.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (ValidateUpdate())
            {
                try
                {
                    UpdateSale();
                    MessageBox.Show("Sale updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadSales();
                    ClearForm();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating sale: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private bool ValidateUpdate()
        {
            if (string.IsNullOrWhiteSpace(txtNetAmount.Text) || !decimal.TryParse(txtNetAmount.Text, out _))
            {
                MessageBox.Show("Please enter a valid net amount.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtBillNumber.Text))
            {
                MessageBox.Show("Bill number is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void UpdateSale()
        {
            decimal netAmount = decimal.Parse(txtNetAmount.Text);
            DateTime saleDate = txtSaleDate.Value;

            string updateQuery = @"UPDATE Sales 
                                 SET NetAmount = @NetAmount, 
                                     SaleDate = @SaleDate
                                 WHERE SaleID = @SaleID";

            SqlParameter[] parameters = {
                new SqlParameter("@NetAmount", netAmount),
                new SqlParameter("@SaleDate", saleDate),
                new SqlParameter("@SaleID", selectedSaleID)
            };

            DatabaseConnection.ExecuteNonQuery(updateQuery, parameters);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedSaleID == 0)
            {
                MessageBox.Show("Please select a sale to delete.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show("Are you sure you want to delete this sale?", "Confirm Delete", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    DeleteSale();
                    MessageBox.Show("Sale deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadSales();
                    ClearForm();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting sale: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void DeleteSale()
        {
            // Soft delete - mark as inactive
            string deleteQuery = "UPDATE Sales SET IsActive = 0 WHERE SaleID = @SaleID";
            SqlParameter[] parameters = { new SqlParameter("@SaleID", selectedSaleID) };
            DatabaseConnection.ExecuteNonQuery(deleteQuery, parameters);
        }

        private void ClearForm()
        {
            txtBillNumber.Text = "";
            txtCustomerName.Text = "";
            txtSaleDate.Value = DateTime.Now;
            txtNetAmount.Text = "";
            selectedSaleID = 0;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            LoadSales();
            ClearForm();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
