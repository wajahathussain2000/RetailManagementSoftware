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
    public partial class NewPurchase : Form
    {
        private DataTable purchaseItems;
        private int selectedCompanyID = 0;
        private decimal totalAmount = 0;

        public NewPurchase()
        {
            InitializeComponent();
            InitializeDataTable();
            LoadCompanies();
            LoadItems();
            SetupDataGridView();
            GeneratePurchaseNumber();
        }

        private void InitializeDataTable()
        {
            purchaseItems = new DataTable();
            purchaseItems.Columns.Add("ItemID", typeof(int));
            purchaseItems.Columns.Add("ItemName", typeof(string));
            purchaseItems.Columns.Add("Quantity", typeof(int));
            purchaseItems.Columns.Add("Price", typeof(decimal));
            purchaseItems.Columns.Add("TotalAmount", typeof(decimal));
        }

        private void SetupDataGridView()
        {
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.Columns.Clear();

            dataGridView1.Columns.Add("ItemName", "Item Name");
            dataGridView1.Columns.Add("Quantity", "Quantity");
            dataGridView1.Columns.Add("Price", "Price");
            dataGridView1.Columns.Add("TotalAmount", "Total Amount");

            dataGridView1.Columns["ItemName"].DataPropertyName = "ItemName";
            dataGridView1.Columns["Quantity"].DataPropertyName = "Quantity";
            dataGridView1.Columns["Price"].DataPropertyName = "Price";
            dataGridView1.Columns["TotalAmount"].DataPropertyName = "TotalAmount";
        }

        private void GeneratePurchaseNumber()
        {
            try
            {
                string query = "SELECT ISNULL(MAX(CAST(SUBSTRING(PurchaseNumber, 4, LEN(PurchaseNumber)) AS INT)), 0) + 1 FROM Purchases";
                object result = DatabaseConnection.ExecuteScalar(query);
                int nextNumber = Convert.ToInt32(result);
                textBox11.Text = $"PO{nextNumber:D6}";
            }
            catch (Exception ex)
            {
                textBox11.Text = $"PO{DateTime.Now:yyyyMMdd}001";
            }
        }

        private void LoadCompanies()
        {
            try
            {
                string query = "SELECT CompanyID, CompanyName, ContactPerson FROM Companies WHERE IsActive = 1 ORDER BY CompanyName";
                DataTable dt = DatabaseConnection.ExecuteQuery(query);
                comboBox1.DataSource = dt;
                comboBox1.DisplayMember = "CompanyName";
                comboBox1.ValueMember = "CompanyID";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading companies: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadItems()
        {
            try
            {
                string query = "SELECT ItemID, ItemName, Price FROM Items WHERE IsActive = 1 ORDER BY ItemName";
                DataTable dt = DatabaseConnection.ExecuteQuery(query);
                // Note: comboItem doesn't exist in Designer, using textBox4 for item name input
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading items: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAddItem_Click(object sender, EventArgs e)
        {
            if (ValidateItemInput())
            {
                AddItemToPurchase();
                CalculateTotals();
                ClearItemInputs();
            }
        }

        private bool ValidateItemInput()
        {
            // Using textBox4 for item name (Item Name field)
            if (string.IsNullOrWhiteSpace(textBox4.Text))
            {
                MessageBox.Show("Please enter an item name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Using textBox7 for quantity (Qty field)
            if (string.IsNullOrWhiteSpace(textBox7.Text) || !int.TryParse(textBox7.Text, out int qty) || qty <= 0)
            {
                MessageBox.Show("Please enter a valid quantity.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Using textBox6 for rate (Rate field)
            if (string.IsNullOrWhiteSpace(textBox6.Text) || !decimal.TryParse(textBox6.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Please enter a valid rate.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void AddItemToPurchase()
        {
            // Using actual form fields based on design
            string itemName = textBox4.Text.Trim(); // Item Name
            int quantity = int.Parse(textBox7.Text); // Qty
            decimal price = decimal.Parse(textBox6.Text); // Rate
            decimal totalItemAmount = quantity * price;

            DataRow newRow = purchaseItems.NewRow();
            newRow["ItemID"] = 0; // Placeholder since we don't have item selection
            newRow["ItemName"] = itemName;
            newRow["Quantity"] = quantity;
            newRow["Price"] = price;
            newRow["TotalAmount"] = totalItemAmount;
            purchaseItems.Rows.Add(newRow);

            dataGridView1.DataSource = purchaseItems;
        }

        private void CalculateTotals()
        {
            totalAmount = 0;
            foreach (DataRow row in purchaseItems.Rows)
            {
                totalAmount += Convert.ToDecimal(row["TotalAmount"]);
            }

            textBox12.Text = totalAmount.ToString("N2"); // Using textBox12 for total amount
        }

        private void ClearItemInputs()
        {
            textBox4.Text = ""; // Item Name
            textBox7.Text = ""; // Qty
            textBox6.Text = ""; // Rate
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (ValidatePurchase())
            {
                try
                {
                    SavePurchase();
                    MessageBox.Show("Purchase saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearForm();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error saving purchase: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private bool ValidatePurchase()
        {
            if (comboBox1.SelectedValue == null)
            {
                MessageBox.Show("Please select a company.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (purchaseItems.Rows.Count == 0)
            {
                MessageBox.Show("Please add at least one item to the purchase.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(textBox11.Text))
            {
                MessageBox.Show("Purchase number is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void SavePurchase()
        {
            int companyID = Convert.ToInt32(comboBox1.SelectedValue);

            // Insert purchase header
            string purchaseQuery = @"INSERT INTO Purchases (PurchaseNumber, CompanyID, PurchaseDate, TotalAmount, Remarks, CreatedDate, IsActive) 
                                   VALUES (@PurchaseNumber, @CompanyID, @PurchaseDate, @TotalAmount, @Remarks, @CreatedDate, 1);
                                   SELECT SCOPE_IDENTITY();";

            SqlParameter[] purchaseParams = {
                new SqlParameter("@PurchaseNumber", textBox11.Text.Trim()),
                new SqlParameter("@CompanyID", companyID),
                new SqlParameter("@PurchaseDate", DateTime.Now),
                new SqlParameter("@TotalAmount", totalAmount),
                new SqlParameter("@Remarks", ""), // No remarks field available
                new SqlParameter("@CreatedDate", DateTime.Now)
            };

            int purchaseID = Convert.ToInt32(DatabaseConnection.ExecuteScalar(purchaseQuery, purchaseParams));

            // Insert purchase items and update stock
            foreach (DataRow row in purchaseItems.Rows)
            {
                int itemID = Convert.ToInt32(row["ItemID"]);
                int quantity = Convert.ToInt32(row["Quantity"]);
                decimal price = Convert.ToDecimal(row["Price"]);
                decimal totalItemAmount = Convert.ToDecimal(row["TotalAmount"]);

                // Insert purchase item
                string purchaseItemQuery = @"INSERT INTO PurchaseItems (PurchaseID, ItemID, Quantity, Price, TotalAmount) 
                                           VALUES (@PurchaseID, @ItemID, @Quantity, @Price, @TotalAmount)";

                SqlParameter[] purchaseItemParams = {
                    new SqlParameter("@PurchaseID", purchaseID),
                    new SqlParameter("@ItemID", itemID),
                    new SqlParameter("@Quantity", quantity),
                    new SqlParameter("@Price", price),
                    new SqlParameter("@TotalAmount", totalItemAmount)
                };

                DatabaseConnection.ExecuteNonQuery(purchaseItemQuery, purchaseItemParams);

                // Update stock
                string updateStockQuery = "UPDATE Items SET StockQuantity = StockQuantity + @Quantity WHERE ItemID = @ItemID";
                SqlParameter[] stockParams = {
                    new SqlParameter("@Quantity", quantity),
                    new SqlParameter("@ItemID", itemID)
                };

                DatabaseConnection.ExecuteNonQuery(updateStockQuery, stockParams);
            }
        }

        private void ClearForm()
        {
            textBox11.Text = "";
            comboBox1.SelectedIndex = -1;
            purchaseItems.Clear();
            dataGridView1.DataSource = null;
            textBox12.Text = "";
            totalAmount = 0;
            GeneratePurchaseNumber();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void btnNewItem_Click(object sender, EventArgs e)
        {
            Items items = new Items();
            items.Show();
            LoadItems(); // Reload items after adding new one
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
