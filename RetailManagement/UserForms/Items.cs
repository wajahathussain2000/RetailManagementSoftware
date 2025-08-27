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
        private DataTable itemsData;

        public Items()
        {
            InitializeComponent();
            InitializeForm();
            LoadItems();
            LoadCategories();
            SetupDataGridView();
            SetupEventHandlers();
        }

        private void InitializeForm()
        {
            // Set form properties
            this.Text = "Items Management";
            this.StartPosition = FormStartPosition.CenterScreen;

            // Initialize data table
            itemsData = new DataTable();

            // Add missing controls
            AddMissingControls();
        }

        private void AddMissingControls()
        {
            // Add status label
            Label lblStatus = new Label
            {
                Text = "Total Items: 0",
                Location = new Point(604, 470),
                Size = new Size(200, 20),
                Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold),
                ForeColor = Color.Black
            };
            this.Controls.Add(lblStatus);

            // Add delete button to panel2
            Button btnDelete = new Button
            {
                Text = "Delete",
                Location = new Point(410, 32),
                Size = new Size(75, 36),
                Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold),
                Enabled = false
            };
            btnDelete.Click += btnDelete_Click;
            panel2.Controls.Add(btnDelete);

            // Add category filter label
            Label lblCategoryFilter = new Label
            {
                Text = "Filter by Category:",
                Location = new Point(604, 15),
                Size = new Size(100, 20),
                Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold)
            };
            this.Controls.Add(lblCategoryFilter);

            // Add category filter combo box
            ComboBox comboBoxCategory = new ComboBox
            {
                Location = new Point(710, 12),
                Size = new Size(150, 20),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            comboBoxCategory.SelectedIndexChanged += comboBoxCategory_SelectedIndexChanged;
            this.Controls.Add(comboBoxCategory);

            // Update search textbox properties
            txtSearch.Text = "Search items...";
            txtSearch.ForeColor = Color.Gray;
            txtSearch.TextChanged += txtSearch_TextChanged;
            
            // Add focus events to handle placeholder behavior
            txtSearch.GotFocus += (s, e) => {
                if (txtSearch.Text == "Search items...")
                {
                    txtSearch.Text = "";
                    txtSearch.ForeColor = Color.Black;
                }
            };
            
            txtSearch.LostFocus += (s, e) => {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    txtSearch.Text = "Search items...";
                    txtSearch.ForeColor = Color.Gray;
                }
            };
        }

        private void SetupEventHandlers()
        {
            // Add missing event handlers
            btnSave.Click += btnSave_Click;
            btnCancel.Click += btnCancel_Click;
            btnPrint.Click += btnPrint_Click;
            gridViewProducts.CellClick += gridViewProducts_CellClick;
        }

        private void SetupDataGridView()
        {
            gridViewProducts.AutoGenerateColumns = false;
            gridViewProducts.Columns.Clear();

            // Add columns with proper formatting
            DataGridViewTextBoxColumn colItemID = new DataGridViewTextBoxColumn
            {
                Name = "ItemID",
                HeaderText = "Item ID",
                DataPropertyName = "ItemID",
                Width = 80,
                ReadOnly = true
            };

            DataGridViewTextBoxColumn colItemName = new DataGridViewTextBoxColumn
            {
                Name = "ItemName",
                HeaderText = "Item Name",
                DataPropertyName = "ItemName",
                Width = 200
            };

            DataGridViewTextBoxColumn colDescription = new DataGridViewTextBoxColumn
            {
                Name = "Description",
                HeaderText = "Description",
                DataPropertyName = "Description",
                Width = 250
            };

            DataGridViewTextBoxColumn colPrice = new DataGridViewTextBoxColumn
            {
                Name = "Price",
                HeaderText = "Price",
                DataPropertyName = "Price",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }
            };

            DataGridViewTextBoxColumn colStockQuantity = new DataGridViewTextBoxColumn
            {
                Name = "StockQuantity",
                HeaderText = "Stock",
                DataPropertyName = "StockQuantity",
                Width = 80
            };

            DataGridViewTextBoxColumn colCategory = new DataGridViewTextBoxColumn
            {
                Name = "Category",
                HeaderText = "Category",
                DataPropertyName = "Category",
                Width = 150
            };

            DataGridViewTextBoxColumn colCreatedDate = new DataGridViewTextBoxColumn
            {
                Name = "CreatedDate",
                HeaderText = "Created Date",
                DataPropertyName = "CreatedDate",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" }
            };

            gridViewProducts.Columns.AddRange(new DataGridViewColumn[] {
                colItemID, colItemName, colDescription, colPrice, 
                colStockQuantity, colCategory, colCreatedDate
            });

            // Set grid properties
            gridViewProducts.AllowUserToAddRows = false;
            gridViewProducts.AllowUserToDeleteRows = false;
            gridViewProducts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridViewProducts.MultiSelect = false;
            gridViewProducts.ReadOnly = true;
            gridViewProducts.RowHeadersVisible = false;
            gridViewProducts.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;
        }

        private void LoadItems()
        {
            try
            {
                string query = @"SELECT ItemID, ItemName, Description, Price, StockQuantity, 
                               Category, CreatedDate FROM Items WHERE IsActive = 1 
                               ORDER BY ItemName";
                
                itemsData = DatabaseConnection.ExecuteQuery(query);
                gridViewProducts.DataSource = itemsData;
                
                // Update status
                UpdateStatusLabel();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading items: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadCategories()
        {
            try
            {
                string query = "SELECT DISTINCT Category FROM Items WHERE IsActive = 1 AND Category IS NOT NULL ORDER BY Category";
                DataTable categoryData = DatabaseConnection.ExecuteQuery(query);
                
                // Load categories into comboBox1 (Company dropdown)
                comboBox1.Items.Clear();
                comboBox1.Items.Add("All Companies");
                
                foreach (DataRow row in categoryData.Rows)
                {
                    comboBox1.Items.Add(row["Category"].ToString());
                }
                
                comboBox1.SelectedIndex = 0;

                // Load categories into category filter combo box
                ComboBox comboBoxCategory = this.Controls.OfType<ComboBox>().FirstOrDefault(c => c.Location.X == 710);
                if (comboBoxCategory != null)
                {
                    comboBoxCategory.Items.Clear();
                    comboBoxCategory.Items.Add("All Categories");
                    
                    foreach (DataRow row in categoryData.Rows)
                    {
                        comboBoxCategory.Items.Add(row["Category"].ToString());
                    }
                    
                    comboBoxCategory.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading categories: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateStatusLabel()
        {
            Label lblStatus = this.Controls.OfType<Label>().FirstOrDefault(l => l.Text.StartsWith("Total Items:"));
            if (lblStatus != null && itemsData != null)
            {
                lblStatus.Text = $"Total Items: {itemsData.Rows.Count}";
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
            
            // Update button states
            btnSave.Text = "Save";
            btnSave.Enabled = false;
            btnCancel.Enabled = false;
            
            // Find and update delete button
            Button btnDelete = panel2.Controls.OfType<Button>().FirstOrDefault(b => b.Text == "Delete");
            if (btnDelete != null)
            {
                btnDelete.Enabled = false;
            }
            
            // Clear selection in grid
            gridViewProducts.ClearSelection();
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
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error saving item: " + ex.Message, "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void InsertItem()
        {
            string query = @"INSERT INTO Items (ItemName, Description, Price, StockQuantity, Category, CreatedDate, IsActive) 
                           VALUES (@ItemName, @Description, @Price, @StockQuantity, @Category, @CreatedDate, 1)";

            SqlParameter[] parameters = {
                new SqlParameter("@ItemName", textBox1.Text.Trim()),
                new SqlParameter("@Description", textBox8.Text.Trim()),
                new SqlParameter("@Price", decimal.Parse(textBox4.Text)),
                new SqlParameter("@StockQuantity", string.IsNullOrEmpty(textBox6.Text) ? 0 : int.Parse(textBox6.Text)),
                new SqlParameter("@Category", textBox3.Text.Trim()),
                new SqlParameter("@CreatedDate", DateTime.Now)
            };

            DatabaseConnection.ExecuteNonQuery(query, parameters);
            MessageBox.Show("Item added successfully!", "Success", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void UpdateItem()
        {
            string query = @"UPDATE Items SET ItemName = @ItemName, Description = @Description, 
                           Price = @Price, StockQuantity = @StockQuantity, Category = @Category 
                           WHERE ItemID = @ItemID";

            SqlParameter[] parameters = {
                new SqlParameter("@ItemID", selectedItemID),
                new SqlParameter("@ItemName", textBox1.Text.Trim()),
                new SqlParameter("@Description", textBox8.Text.Trim()),
                new SqlParameter("@Price", decimal.Parse(textBox4.Text)),
                new SqlParameter("@StockQuantity", string.IsNullOrEmpty(textBox6.Text) ? 0 : int.Parse(textBox6.Text)),
                new SqlParameter("@Category", textBox3.Text.Trim())
            };

            DatabaseConnection.ExecuteNonQuery(query, parameters);
            MessageBox.Show("Item updated successfully!", "Success", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private bool ValidateForm()
        {
            // Validate Item Name
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Please enter item name.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Focus();
                return false;
            }

            // Validate Price
            if (string.IsNullOrWhiteSpace(textBox4.Text) || !decimal.TryParse(textBox4.Text, out decimal price) || price < 0)
            {
                MessageBox.Show("Please enter a valid price (must be a positive number).", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox4.Focus();
                return false;
            }

            // Validate Stock Quantity (if provided)
            if (!string.IsNullOrWhiteSpace(textBox6.Text))
            {
                if (!int.TryParse(textBox6.Text, out int stock) || stock < 0)
                {
                    MessageBox.Show("Please enter a valid stock quantity (must be a positive number).", "Validation Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox6.Focus();
                    return false;
                }
            }

            // Check for duplicate item name (case insensitive)
            string checkQuery = "SELECT COUNT(*) FROM Items WHERE LOWER(ItemName) = LOWER(@ItemName) AND IsActive = 1";
            if (isEditMode)
            {
                checkQuery += " AND ItemID != @ItemID";
            }

            SqlParameter[] checkParams = isEditMode ? 
                new SqlParameter[] { 
                    new SqlParameter("@ItemName", textBox1.Text.Trim()),
                    new SqlParameter("@ItemID", selectedItemID)
                } :
                new SqlParameter[] { 
                    new SqlParameter("@ItemName", textBox1.Text.Trim())
                };

            int count = Convert.ToInt32(DatabaseConnection.ExecuteScalar(checkQuery, checkParams));
            if (count > 0)
            {
                MessageBox.Show("An item with this name already exists.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Focus();
                return false;
            }

            return true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearForm();
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
                        // Check if item is used in any sales
                        string checkQuery = "SELECT COUNT(*) FROM SaleItems WHERE ItemID = @ItemID";
                        SqlParameter[] checkParams = { new SqlParameter("@ItemID", selectedItemID) };
                        int saleCount = Convert.ToInt32(DatabaseConnection.ExecuteScalar(checkQuery, checkParams));

                        if (saleCount > 0)
                        {
                            MessageBox.Show("Cannot delete this item as it is used in sales transactions.", 
                                "Delete Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        string query = "UPDATE Items SET IsActive = 0 WHERE ItemID = @ItemID";
                        SqlParameter[] parameters = { new SqlParameter("@ItemID", selectedItemID) };
                        DatabaseConnection.ExecuteNonQuery(query, parameters);
                        
                        LoadItems();
                        ClearForm();
                        MessageBox.Show("Item deleted successfully!", "Success", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error deleting item: " + ex.Message, "Error", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select an item to delete.", "Information", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (itemsData != null && itemsData.Rows.Count > 0)
                {
                    // Create a simple print functionality
                    PrintDialog printDialog = new PrintDialog();
                    if (printDialog.ShowDialog() == DialogResult.OK)
                    {
                        MessageBox.Show("Print functionality would be implemented here.", "Print", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("No data to print.", "Information", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error printing: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void gridViewProducts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = gridViewProducts.Rows[e.RowIndex];
                selectedItemID = Convert.ToInt32(row.Cells["ItemID"].Value);
                
                textBox1.Text = row.Cells["ItemName"].Value.ToString();
                textBox8.Text = row.Cells["Description"].Value.ToString();
                textBox4.Text = Convert.ToDecimal(row.Cells["Price"].Value).ToString("F2");
                textBox6.Text = row.Cells["StockQuantity"].Value.ToString();
                textBox3.Text = row.Cells["Category"].Value.ToString();
                
                isEditMode = true;
                btnSave.Text = "Update";
                btnSave.Enabled = true;
                btnCancel.Enabled = true;
                
                // Enable delete button
                Button btnDelete = panel2.Controls.OfType<Button>().FirstOrDefault(b => b.Text == "Delete");
                if (btnDelete != null)
                {
                    btnDelete.Enabled = true;
                }
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void comboBoxCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterItems();
        }

        private void FilterItems()
        {
            try
            {
                ComboBox comboBoxCategory = this.Controls.OfType<ComboBox>().FirstOrDefault(c => c.Location.X == 710);
                if (comboBoxCategory != null && comboBoxCategory.SelectedIndex > 0)
                {
                    string category = comboBoxCategory.SelectedItem.ToString();
                    DataView dv = itemsData.DefaultView;
                    dv.RowFilter = $"Category = '{category.Replace("'", "''")}'";
                    gridViewProducts.DataSource = dv;
                }
                else
                {
                    gridViewProducts.DataSource = itemsData;
                }
                
                UpdateStatusLabel();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error filtering items: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            SearchItems();
        }

        private void SearchItems()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(txtSearch.Text) && txtSearch.Text != "Search items...")
                {
                    DataView dv = itemsData.DefaultView;
                    dv.RowFilter = $"ItemName LIKE '%{txtSearch.Text.Replace("'", "''")}%' OR Description LIKE '%{txtSearch.Text.Replace("'", "''")}%'";
                    gridViewProducts.DataSource = dv;
                }
                else
                {
                    gridViewProducts.DataSource = itemsData;
                }
                
                UpdateStatusLabel();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching items: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
