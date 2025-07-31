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
    public partial class NewBill : Form
    {
        private DataTable billItems;
        private int selectedCustomerID = 0;
        private decimal totalAmount = 0;
        private decimal discount = 0;
        private decimal netAmount = 0;

        public NewBill()
        {
            InitializeComponent();
            InitializeDataTable();
            LoadCustomers();
            LoadItems();
            SetupDataGridView();
            GenerateBillNumber();
        }

        private void InitializeDataTable()
        {
            billItems = new DataTable();
            billItems.Columns.Add("ItemID", typeof(int));
            billItems.Columns.Add("ItemName", typeof(string));
            billItems.Columns.Add("Quantity", typeof(int));
            billItems.Columns.Add("Price", typeof(decimal));
            billItems.Columns.Add("TotalAmount", typeof(decimal));
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

        private void GenerateBillNumber()
        {
            try
            {
                string query = "SELECT ISNULL(MAX(CAST(SUBSTRING(BillNumber, 5, LEN(BillNumber)) AS INT)), 0) + 1 FROM Sales";
                object result = DatabaseConnection.ExecuteScalar(query);
                int nextNumber = Convert.ToInt32(result);
                textBox11.Text = $"BILL{nextNumber:D6}";
            }
            catch (Exception ex)
            {
                textBox11.Text = $"BILL{DateTime.Now:yyyyMMdd}001";
            }
        }

        private void LoadCustomers()
        {
            try
            {
                string query = "SELECT CustomerID, CustomerName, Phone FROM Customers WHERE IsActive = 1 ORDER BY CustomerName";
                DataTable dt = DatabaseConnection.ExecuteQuery(query);
                // Using listBoxItems for customer selection based on form design
                listBoxItems.DataSource = dt;
                listBoxItems.DisplayMember = "CustomerName";
                listBoxItems.ValueMember = "CustomerID";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading customers: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadItems()
        {
            try
            {
                string query = "SELECT ItemID, ItemName, Price, StockQuantity FROM Items WHERE IsActive = 1 ORDER BY ItemName";
                DataTable dt = DatabaseConnection.ExecuteQuery(query);
                // Items will be selected from textBox4 (Item Name Reorder) and dataGridView1
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
                AddItemToBill();
                CalculateTotals();
                ClearItemInputs();
            }
        }

        private bool ValidateItemInput()
        {
            // Using textBox4 for item name (Item Name Reorder field)
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

            return true;
        }

        private int GetItemStock(int itemID)
        {
            try
            {
                string query = "SELECT StockQuantity FROM Items WHERE ItemID = @ItemID";
                SqlParameter[] parameters = { new SqlParameter("@ItemID", itemID) };
                object result = DatabaseConnection.ExecuteScalar(query, parameters);
                return Convert.ToInt32(result);
            }
            catch
            {
                return 0;
            }
        }

        private void AddItemToBill()
        {
            // Using actual form fields based on design
            string itemName = textBox4.Text.Trim(); // Item Name Reorder
            int quantity = int.Parse(textBox7.Text); // Qty
            decimal price = decimal.Parse(textBox6.Text); // Rate
            decimal totalItemAmount = quantity * price;

            DataRow newRow = billItems.NewRow();
            newRow["ItemID"] = 0; // Placeholder since we don't have item selection
            newRow["ItemName"] = itemName;
            newRow["Quantity"] = quantity;
            newRow["Price"] = price;
            newRow["TotalAmount"] = totalItemAmount;
            billItems.Rows.Add(newRow);

            dataGridView1.DataSource = billItems;
        }

        private decimal GetItemPrice(int itemID)
        {
            try
            {
                string query = "SELECT Price FROM Items WHERE ItemID = @ItemID";
                SqlParameter[] parameters = { new SqlParameter("@ItemID", itemID) };
                object result = DatabaseConnection.ExecuteScalar(query, parameters);
                return Convert.ToDecimal(result);
            }
            catch
            {
                return 0;
            }
        }

        private void CalculateTotals()
        {
            totalAmount = 0;
            foreach (DataRow row in billItems.Rows)
            {
                totalAmount += Convert.ToDecimal(row["TotalAmount"]);
            }

            discount = string.IsNullOrWhiteSpace(textBox10.Text) ? 0 : decimal.Parse(textBox10.Text);
            netAmount = totalAmount - discount;

            textBox9.Text = totalAmount.ToString("N2");
            textBox12.Text = netAmount.ToString("N2");
        }

        private void ClearItemInputs()
        {
            textBox4.Text = ""; // Item Name Reorder
            textBox7.Text = ""; // Qty
            textBox6.Text = ""; // Rate
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (ValidateBill())
            {
                try
                {
                    SaveBill();
                    MessageBox.Show("Bill saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearForm();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error saving bill: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private bool ValidateBill()
        {
            if (listBoxItems.SelectedValue == null)
            {
                MessageBox.Show("Please select a customer.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (billItems.Rows.Count == 0)
            {
                MessageBox.Show("Please add at least one item to the bill.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(textBox11.Text))
            {
                MessageBox.Show("Bill number is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void SaveBill()
        {
            int customerID = Convert.ToInt32(listBoxItems.SelectedValue);
            string paymentMethod = "Cash"; // Default payment method

            // Insert sale header
            string saleQuery = @"INSERT INTO Sales (BillNumber, CustomerID, SaleDate, TotalAmount, Discount, NetAmount, PaymentMethod, IsCredit, Remarks, CreatedDate, IsActive) 
                               VALUES (@BillNumber, @CustomerID, @SaleDate, @TotalAmount, @Discount, @NetAmount, @PaymentMethod, @IsCredit, @Remarks, @CreatedDate, 1);
                               SELECT SCOPE_IDENTITY();";

            SqlParameter[] saleParams = {
                new SqlParameter("@BillNumber", textBox11.Text.Trim()),
                new SqlParameter("@CustomerID", customerID),
                new SqlParameter("@SaleDate", DateTime.Now),
                new SqlParameter("@TotalAmount", totalAmount),
                new SqlParameter("@Discount", discount),
                new SqlParameter("@NetAmount", netAmount),
                new SqlParameter("@PaymentMethod", paymentMethod),
                new SqlParameter("@IsCredit", paymentMethod == "Credit"),
                new SqlParameter("@Remarks", ""), // No remarks field available
                new SqlParameter("@CreatedDate", DateTime.Now)
            };

            int saleID = Convert.ToInt32(DatabaseConnection.ExecuteScalar(saleQuery, saleParams));

            // Insert sale items and update stock
            foreach (DataRow row in billItems.Rows)
            {
                int itemID = Convert.ToInt32(row["ItemID"]);
                int quantity = Convert.ToInt32(row["Quantity"]);
                decimal price = Convert.ToDecimal(row["Price"]);
                decimal totalItemAmount = Convert.ToDecimal(row["TotalAmount"]);

                // Insert sale item
                string saleItemQuery = @"INSERT INTO SaleItems (SaleID, ItemID, Quantity, Price, TotalAmount) 
                                       VALUES (@SaleID, @ItemID, @Quantity, @Price, @TotalAmount)";

                SqlParameter[] saleItemParams = {
                    new SqlParameter("@SaleID", saleID),
                    new SqlParameter("@ItemID", itemID),
                    new SqlParameter("@Quantity", quantity),
                    new SqlParameter("@Price", price),
                    new SqlParameter("@TotalAmount", totalItemAmount)
                };

                DatabaseConnection.ExecuteNonQuery(saleItemQuery, saleItemParams);

                // Update stock
                string updateStockQuery = "UPDATE Items SET StockQuantity = StockQuantity - @Quantity WHERE ItemID = @ItemID";
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
            listBoxItems.SelectedIndex = -1;
            textBox10.Text = "";
            billItems.Clear();
            dataGridView1.DataSource = null;
            textBox9.Text = "";
            textBox12.Text = "";
            totalAmount = 0;
            discount = 0;
            netAmount = 0;
            GenerateBillNumber();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {
            CalculateTotals();
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
