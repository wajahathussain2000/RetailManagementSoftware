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
    public partial class SaleReturn : Form
    {
        private DataTable originalSaleItems;
        private DataTable returnItems;
        private int originalSaleID = 0;

        public SaleReturn()
        {
            InitializeComponent();
            InitializeDataTables();
            SetupDataGridView();
        }

        private void InitializeDataTables()
        {
            returnItems = new DataTable();
            returnItems.Columns.Add("ItemID", typeof(int));
            returnItems.Columns.Add("ItemName", typeof(string));
            returnItems.Columns.Add("OriginalQuantity", typeof(int));
            returnItems.Columns.Add("ReturnQuantity", typeof(int));
            returnItems.Columns.Add("Price", typeof(decimal));
            returnItems.Columns.Add("TotalAmount", typeof(decimal));
        }

        private void SetupDataGridView()
        {
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.Columns.Clear();

            dataGridView1.Columns.Add("ItemName", "Item Name");
            dataGridView1.Columns.Add("OriginalQuantity", "Original Qty");
            dataGridView1.Columns.Add("ReturnQuantity", "Return Qty");
            dataGridView1.Columns.Add("Price", "Price");
            dataGridView1.Columns.Add("TotalAmount", "Total Amount");

            dataGridView1.Columns["ItemName"].DataPropertyName = "ItemName";
            dataGridView1.Columns["OriginalQuantity"].DataPropertyName = "OriginalQuantity";
            dataGridView1.Columns["ReturnQuantity"].DataPropertyName = "ReturnQuantity";
            dataGridView1.Columns["Price"].DataPropertyName = "Price";
            dataGridView1.Columns["TotalAmount"].DataPropertyName = "TotalAmount";
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBillNumber.Text))
            {
                MessageBox.Show("Please enter bill number to search.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string query = @"SELECT s.SaleID, s.BillNumber, s.SaleDate, c.CustomerName, si.ItemID, i.ItemName, 
                               si.Quantity as OriginalQuantity, si.Price
                               FROM Sales s
                               INNER JOIN SaleItems si ON s.SaleID = si.SaleID
                               INNER JOIN Items i ON si.ItemID = i.ItemID
                               INNER JOIN Customers c ON s.CustomerID = c.CustomerID
                               WHERE s.BillNumber = @BillNumber AND s.IsActive = 1";

                SqlParameter[] parameters = { new SqlParameter("@BillNumber", txtBillNumber.Text.Trim()) };
                originalSaleItems = DatabaseConnection.ExecuteQuery(query, parameters);

                if (originalSaleItems.Rows.Count > 0)
                {
                    originalSaleID = Convert.ToInt32(originalSaleItems.Rows[0]["SaleID"]);
                    LoadOriginalSaleItems();
                    MessageBox.Show("Original sale found!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("No sale found with this bill number.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching sale: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadOriginalSaleItems()
        {
            returnItems.Clear();
            foreach (DataRow row in originalSaleItems.Rows)
            {
                DataRow newRow = returnItems.NewRow();
                newRow["ItemID"] = row["ItemID"];
                newRow["ItemName"] = row["ItemName"];
                newRow["OriginalQuantity"] = row["OriginalQuantity"];
                newRow["ReturnQuantity"] = 0;
                newRow["Price"] = row["Price"];
                newRow["TotalAmount"] = 0;
                returnItems.Rows.Add(newRow);
            }
            dataGridView1.DataSource = returnItems;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (returnItems.Rows.Count == 0)
            {
                MessageBox.Show("Please search for an original sale first.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // This would typically open a dialog to select items and quantities
            // For now, we'll just enable editing in the grid
            dataGridView1.ReadOnly = false;
            dataGridView1.Columns["ReturnQuantity"].ReadOnly = false;
            MessageBox.Show("You can now edit return quantities in the grid.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (ValidateReturnData())
            {
                try
                {
                    SaveReturnTransaction();
                    MessageBox.Show("Return transaction saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearForm();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error saving return: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private bool ValidateReturnData()
        {
            if (returnItems.Rows.Count == 0)
            {
                MessageBox.Show("No items to return.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            bool hasReturnItems = false;
            foreach (DataRow row in returnItems.Rows)
            {
                int returnQty = Convert.ToInt32(row["ReturnQuantity"]);
                int originalQty = Convert.ToInt32(row["OriginalQuantity"]);
                
                if (returnQty > 0)
                {
                    hasReturnItems = true;
                    if (returnQty > originalQty)
                    {
                        MessageBox.Show($"Return quantity cannot exceed original quantity for {row["ItemName"]}.", 
                            "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }
            }

            if (!hasReturnItems)
            {
                MessageBox.Show("Please specify return quantities for at least one item.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void SaveReturnTransaction()
        {
            // Insert return header
            string returnQuery = @"INSERT INTO SaleReturns (OriginalSaleID, ReturnDate, TotalAmount, Remarks, CreatedDate) 
                                 VALUES (@OriginalSaleID, @ReturnDate, @TotalAmount, @Remarks, @CreatedDate);
                                 SELECT SCOPE_IDENTITY();";

            decimal totalAmount = CalculateTotalReturnAmount();
            SqlParameter[] returnParams = {
                new SqlParameter("@OriginalSaleID", originalSaleID),
                new SqlParameter("@ReturnDate", DateTime.Now),
                new SqlParameter("@TotalAmount", totalAmount),
                new SqlParameter("@Remarks", "Customer return"),
                new SqlParameter("@CreatedDate", DateTime.Now)
            };

            int returnID = Convert.ToInt32(DatabaseConnection.ExecuteScalar(returnQuery, returnParams));

            // Insert return items and update stock
            foreach (DataRow row in returnItems.Rows)
            {
                int returnQty = Convert.ToInt32(row["ReturnQuantity"]);
                if (returnQty > 0)
                {
                    int itemID = Convert.ToInt32(row["ItemID"]);
                    decimal price = Convert.ToDecimal(row["Price"]);
                    decimal totalItemAmount = returnQty * price;

                    // Insert return item
                    string returnItemQuery = @"INSERT INTO SaleReturnItems (ReturnID, ItemID, ReturnQuantity, Price, TotalAmount) 
                                             VALUES (@ReturnID, @ItemID, @ReturnQuantity, @Price, @TotalAmount)";

                    SqlParameter[] returnItemParams = {
                        new SqlParameter("@ReturnID", returnID),
                        new SqlParameter("@ItemID", itemID),
                        new SqlParameter("@ReturnQuantity", returnQty),
                        new SqlParameter("@Price", price),
                        new SqlParameter("@TotalAmount", totalItemAmount)
                    };

                    DatabaseConnection.ExecuteNonQuery(returnItemQuery, returnItemParams);

                    // Update stock
                    string updateStockQuery = "UPDATE Items SET StockQuantity = StockQuantity + @ReturnQuantity WHERE ItemID = @ItemID";
                    SqlParameter[] stockParams = {
                        new SqlParameter("@ReturnQuantity", returnQty),
                        new SqlParameter("@ItemID", itemID)
                    };

                    DatabaseConnection.ExecuteNonQuery(updateStockQuery, stockParams);
                }
            }
        }

        private decimal CalculateTotalReturnAmount()
        {
            decimal total = 0;
            foreach (DataRow row in returnItems.Rows)
            {
                int returnQty = Convert.ToInt32(row["ReturnQuantity"]);
                decimal price = Convert.ToDecimal(row["Price"]);
                total += returnQty * price;
            }
            return total;
        }

        private void ClearForm()
        {
            txtBillNumber.Text = "";
            returnItems.Clear();
            dataGridView1.DataSource = null;
            originalSaleID = 0;
            dataGridView1.ReadOnly = true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearForm();
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