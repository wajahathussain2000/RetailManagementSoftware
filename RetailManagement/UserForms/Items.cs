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
using RetailManagement.Models;

namespace RetailManagement.UserForms
{
    public partial class Items : Form
    {
        private bool isEditMode = false;
        private int selectedItemID = 0;

        public Items()
        {
            InitializeComponent();
            LoadItems();
            SetupDataGridView();
        }

        private void SetupDataGridView()
        {
            gridViewProducts.AutoGenerateColumns = false;
            gridViewProducts.Columns.Clear();

            gridViewProducts.Columns.Add("ItemID", "Item ID");
            gridViewProducts.Columns.Add("ItemName", "Name");
            gridViewProducts.Columns.Add("Description", "Generic Name");
            gridViewProducts.Columns.Add("Price", "Retail Price");
            gridViewProducts.Columns.Add("Category", "Location");

            gridViewProducts.Columns["ItemID"].DataPropertyName = "ItemID";
            gridViewProducts.Columns["ItemName"].DataPropertyName = "ItemName";
            gridViewProducts.Columns["Description"].DataPropertyName = "Description";
            gridViewProducts.Columns["Price"].DataPropertyName = "Price";
            gridViewProducts.Columns["Category"].DataPropertyName = "Category";
        }

        private void LoadItems()
        {
            try
            {
                string query = "SELECT ItemID, ItemName, Description, Price, Category FROM Items WHERE IsActive = 1";
                DataTable dt = DatabaseConnection.ExecuteQuery(query);
                gridViewProducts.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading items: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearForm()
        {
            textBox1.Text = ""; // Name
            textBox2.Text = ""; // Packing
            textBox3.Text = ""; // Location
            textBox4.Text = ""; // Retail Price
            textBox5.Text = ""; // Purchase Price
            textBox6.Text = ""; // Pack Size
            textBox7.Text = ""; // Re-Order Level
            textBox8.Text = ""; // Generic Name
            textBox9.Text = ""; // Distribution Disc.
            textBox10.Text = ""; // Sales Tax
            textBox11.Text = ""; // Sales Tax
            selectedItemID = 0;
            isEditMode = false;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            ClearForm();
            btnSave.Enabled = true;
            btnCancel.Enabled = true;
            textBox1.Focus();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                try
                {
                    if (isEditMode)
                    {
                        UpdateItem();
                    }
                    else
                    {
                        InsertItem();
                    }
                    LoadItems();
                    ClearForm();
                    btnSave.Enabled = false;
                    btnCancel.Enabled = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error saving item: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void InsertItem()
        {
            string query = @"INSERT INTO Items (ItemName, Description, Price, StockQuantity, Category, CreatedDate, IsActive) 
                           VALUES (@ItemName, @Description, @Price, @StockQuantity, @Category, @CreatedDate, 1)";

            SqlParameter[] parameters = {
                new SqlParameter("@ItemName", textBox1.Text.Trim()), // Name
                new SqlParameter("@Description", textBox8.Text.Trim()), // Generic Name as Description
                new SqlParameter("@Price", decimal.Parse(textBox4.Text)), // Retail Price
                new SqlParameter("@StockQuantity", 0), // Default stock quantity
                new SqlParameter("@Category", textBox3.Text.Trim()), // Location as Category
                new SqlParameter("@CreatedDate", DateTime.Now)
            };

            DatabaseConnection.ExecuteNonQuery(query, parameters);
            MessageBox.Show("Item added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void UpdateItem()
        {
            string query = @"UPDATE Items SET ItemName = @ItemName, Description = @Description, 
                           Price = @Price, StockQuantity = @StockQuantity, Category = @Category 
                           WHERE ItemID = @ItemID";

            SqlParameter[] parameters = {
                new SqlParameter("@ItemID", selectedItemID),
                new SqlParameter("@ItemName", textBox1.Text.Trim()), // Name
                new SqlParameter("@Description", textBox8.Text.Trim()), // Generic Name as Description
                new SqlParameter("@Price", decimal.Parse(textBox4.Text)), // Retail Price
                new SqlParameter("@StockQuantity", 0), // Default stock quantity
                new SqlParameter("@Category", textBox3.Text.Trim()) // Location as Category
            };

            DatabaseConnection.ExecuteNonQuery(query, parameters);
            MessageBox.Show("Item updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Please enter item name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(textBox4.Text) || !decimal.TryParse(textBox4.Text, out _))
            {
                MessageBox.Show("Please enter a valid retail price.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox4.Focus();
                return false;
            }

            return true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearForm();
            btnSave.Enabled = false;
            btnCancel.Enabled = false;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedItemID > 0)
            {
                if (MessageBox.Show("Are you sure you want to delete this item?", "Confirm Delete", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        string query = "UPDATE Items SET IsActive = 0 WHERE ItemID = @ItemID";
                        SqlParameter[] parameters = { new SqlParameter("@ItemID", selectedItemID) };
                        DatabaseConnection.ExecuteNonQuery(query, parameters);
                        LoadItems();
                        ClearForm();
                        MessageBox.Show("Item deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error deleting item: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select an item to delete.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void gridViewProducts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = gridViewProducts.Rows[e.RowIndex];
                selectedItemID = Convert.ToInt32(row.Cells["ItemID"].Value);
                textBox1.Text = row.Cells["ItemName"].Value.ToString(); // Name
                textBox8.Text = row.Cells["Description"].Value.ToString(); // Generic Name
                textBox4.Text = row.Cells["Price"].Value.ToString(); // Retail Price
                textBox3.Text = row.Cells["Category"].Value.ToString(); // Location
                isEditMode = true;
                btnSave.Enabled = true;
                btnCancel.Enabled = true;
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
