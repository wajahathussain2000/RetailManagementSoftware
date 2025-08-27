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
    public partial class CreditBill : Form
    {
        private DataTable billItems;
        private int selectedCustomerID = 0;
        private decimal totalAmount = 0;

        public CreditBill()
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
                textBox12.Text = $"BILL{nextNumber:D6}";
            }
            catch (Exception ex)
            {
                textBox12.Text = $"BILL{DateTime.Now:yyyyMMdd}001";
            }
        }

        private void LoadCustomers()
        {
            try
            {
                string query = "SELECT CustomerID, CustomerName, Phone FROM Customers WHERE IsActive = 1 ORDER BY CustomerName";
                DataTable dt = DatabaseConnection.ExecuteQuery(query);
                comboBox1.DataSource = dt;
                comboBox1.DisplayMember = "CustomerName";
                comboBox1.ValueMember = "CustomerID";
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
                string query = "SELECT ItemID, ItemName, Price FROM Items WHERE IsActive = 1 ORDER BY ItemName";
                DataTable dt = DatabaseConnection.ExecuteQuery(query);
                
                // Create a dropdown for item selection if it doesn't exist
                ComboBox comboItem = this.Controls.OfType<ComboBox>().FirstOrDefault(c => c.Name == "comboItem");
                if (comboItem == null)
                {
                    comboItem = new ComboBox
                    {
                        Name = "comboItem",
                        Location = new Point(110, 110),
                        Size = new Size(200, 20),
                        DropDownStyle = ComboBoxStyle.DropDownList
                    };
                    this.Controls.Add(comboItem);
                }
                
                comboItem.DataSource = dt;
                comboItem.DisplayMember = "ItemName";
                comboItem.ValueMember = "ItemID";
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
            // Using textBox1 for item name (Item Name Reorder field)
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Please enter an item name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Using textBox5 for quantity (Qty field)
            if (string.IsNullOrWhiteSpace(textBox5.Text) || !int.TryParse(textBox5.Text, out int qty) || qty <= 0)
            {
                MessageBox.Show("Please enter a valid quantity.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Using textBox4 for price (Price field)
            if (string.IsNullOrWhiteSpace(textBox4.Text) || !decimal.TryParse(textBox4.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Please enter a valid price.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void AddItemToBill()
        {
            // Using actual form fields based on design
            string itemName = textBox1.Text.Trim(); // Item Name Reorder
            int quantity = int.Parse(textBox5.Text); // Qty
            decimal price = decimal.Parse(textBox4.Text); // Price
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

        private void CalculateTotals()
        {
            totalAmount = 0;
            foreach (DataRow row in billItems.Rows)
            {
                totalAmount += Convert.ToDecimal(row["TotalAmount"]);
            }

            textBox6.Text = totalAmount.ToString("N2"); // Total field
            textBox12.Text = totalAmount.ToString("N2"); // Bill Total field
        }

        private void ClearItemInputs()
        {
            textBox1.Text = ""; // Item Name Reorder
            textBox5.Text = ""; // Qty
            textBox4.Text = ""; // Price
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (ValidateBill())
            {
                try
                {
                    SaveBill();
                    MessageBox.Show("Credit bill saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearForm();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error saving credit bill: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private bool ValidateBill()
        {
            if (comboBox1.SelectedValue == null)
            {
                MessageBox.Show("Please select a customer.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (billItems.Rows.Count == 0)
            {
                MessageBox.Show("Please add at least one item to the bill.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(textBox12.Text))
            {
                MessageBox.Show("Bill number is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void SaveBill()
        {
            int customerID = Convert.ToInt32(comboBox1.SelectedValue);
            string paymentMethod = "Credit"; // This is a credit bill

            // Insert sale header
            string saleQuery = @"INSERT INTO Sales (BillNumber, CustomerID, SaleDate, TotalAmount, Discount, NetAmount, PaymentMethod, IsCredit, Remarks, CreatedDate, IsActive) 
                               VALUES (@BillNumber, @CustomerID, @SaleDate, @TotalAmount, @Discount, @NetAmount, @PaymentMethod, @IsCredit, @Remarks, @CreatedDate, 1);
                               SELECT SCOPE_IDENTITY();";

            SqlParameter[] saleParams = {
                new SqlParameter("@BillNumber", textBox12.Text.Trim()),
                new SqlParameter("@CustomerID", customerID),
                new SqlParameter("@SaleDate", DateTime.Now),
                new SqlParameter("@TotalAmount", totalAmount),
                new SqlParameter("@Discount", 0), // No discount for credit bills
                new SqlParameter("@NetAmount", totalAmount),
                new SqlParameter("@PaymentMethod", paymentMethod),
                new SqlParameter("@IsCredit", true),
                new SqlParameter("@Remarks", "Credit sale"),
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
            textBox12.Text = "";
            comboBox1.SelectedIndex = -1;
            billItems.Clear();
            dataGridView1.DataSource = null;
            textBox6.Text = "";
            totalAmount = 0;
            GenerateBillNumber();
        }


        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadCustomers();
            LoadItems();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (billItems.Rows.Count > 0)
                {
                    PrintDocument pd = new PrintDocument();
                    pd.PrintPage += PrintCreditBill;
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

        private void PrintCreditBill(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Font titleFont = new Font("Arial", 16, FontStyle.Bold);
            Font headerFont = new Font("Arial", 10, FontStyle.Bold);
            Font dataFont = new Font("Arial", 9);

            int yPos = 50;
            g.DrawString("Credit Bill", titleFont, Brushes.Black, 50, yPos);
            yPos += 20;
            g.DrawString("Bill Number: " + textBox12.Text, dataFont, Brushes.Black, 50, yPos);
            yPos += 15;
            g.DrawString("Customer: " + comboBox1.Text, dataFont, Brushes.Black, 50, yPos);
            yPos += 15;
            g.DrawString("Date: " + DateTime.Now.ToString("dd/MM/yyyy"), dataFont, Brushes.Black, 50, yPos);
            yPos += 30;

            // Draw headers
            g.DrawString("Item", headerFont, Brushes.Black, 50, yPos);
            g.DrawString("Qty", headerFont, Brushes.Black, 200, yPos);
            g.DrawString("Price", headerFont, Brushes.Black, 250, yPos);
            g.DrawString("Total", headerFont, Brushes.Black, 350, yPos);
            yPos += 20;

            // Draw data
            foreach (DataRow row in billItems.Rows)
            {
                if (yPos > e.MarginBounds.Bottom - 50) break;

                g.DrawString(row["ItemName"].ToString(), dataFont, Brushes.Black, 50, yPos);
                g.DrawString(row["Quantity"].ToString(), dataFont, Brushes.Black, 200, yPos);
                g.DrawString(Convert.ToDecimal(row["Price"]).ToString("N2"), dataFont, Brushes.Black, 250, yPos);
                g.DrawString(Convert.ToDecimal(row["TotalAmount"]).ToString("N2"), dataFont, Brushes.Black, 350, yPos);
                yPos += 15;
            }

            yPos += 10;
            g.DrawString("Total Amount: " + totalAmount.ToString("N2"), headerFont, Brushes.Black, 250, yPos);
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
