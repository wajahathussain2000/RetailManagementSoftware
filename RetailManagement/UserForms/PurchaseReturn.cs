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
    public partial class PurchaseReturn : Form
    {
        private DataTable returnItems;
        private int selectedCompanyID = 0;
        private decimal totalAmount = 0;

        public PurchaseReturn()
        {
            InitializeComponent();
            InitializeDataTable();
            LoadCompanies();
            LoadItems();
            SetupDataGridView();
            GenerateReturnNumber();
        }

        private void InitializeDataTable()
        {
            returnItems = new DataTable();
            returnItems.Columns.Add("ItemID", typeof(int));
            returnItems.Columns.Add("ItemName", typeof(string));
            returnItems.Columns.Add("Quantity", typeof(int));
            returnItems.Columns.Add("Price", typeof(decimal));
            returnItems.Columns.Add("TotalAmount", typeof(decimal));
            returnItems.Columns.Add("ReturnReason", typeof(string));
        }

        private void SetupDataGridView()
        {
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.Columns.Clear();

            dataGridView1.Columns.Add("ItemName", "Item Name");
            dataGridView1.Columns.Add("Quantity", "Quantity");
            dataGridView1.Columns.Add("Price", "Price");
            dataGridView1.Columns.Add("TotalAmount", "Total Amount");
            dataGridView1.Columns.Add("ReturnReason", "Return Reason");

            dataGridView1.Columns["ItemName"].DataPropertyName = "ItemName";
            dataGridView1.Columns["Quantity"].DataPropertyName = "Quantity";
            dataGridView1.Columns["Price"].DataPropertyName = "Price";
            dataGridView1.Columns["TotalAmount"].DataPropertyName = "TotalAmount";
            dataGridView1.Columns["ReturnReason"].DataPropertyName = "ReturnReason";
        }

        private void GenerateReturnNumber()
        {
            try
            {
                string query = "SELECT ISNULL(MAX(CAST(SUBSTRING(ReturnNumber, 4, LEN(ReturnNumber)) AS INT)), 0) + 1 FROM PurchaseReturns";
                object result = DatabaseConnection.ExecuteScalar(query);
                int nextNumber = Convert.ToInt32(result);
                txtReturnNumber.Text = $"PR{nextNumber:D6}";
            }
            catch (Exception ex)
            {
                txtReturnNumber.Text = $"PR{DateTime.Now:yyyyMMdd}001";
            }
        }

        private void LoadCompanies()
        {
            try
            {
                string query = "SELECT CompanyID, CompanyName, ContactPerson FROM Companies WHERE IsActive = 1 ORDER BY CompanyName";
                DataTable dt = DatabaseConnection.ExecuteQuery(query);
                cmbCompany.DataSource = dt;
                cmbCompany.DisplayMember = "CompanyName";
                cmbCompany.ValueMember = "CompanyID";
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
                cmbItem.DataSource = dt;
                cmbItem.DisplayMember = "ItemName";
                cmbItem.ValueMember = "ItemID";
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
                AddItemToReturn();
                CalculateTotals();
                ClearItemInputs();
            }
        }

        private bool ValidateItemInput()
        {
            if (cmbItem.SelectedValue == null)
            {
                MessageBox.Show("Please select an item.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Please enter a valid quantity.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtReturnReason.Text))
            {
                MessageBox.Show("Please enter a return reason.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void AddItemToReturn()
        {
            int itemID = Convert.ToInt32(cmbItem.SelectedValue);
            string itemName = cmbItem.Text;
            int quantity = Convert.ToInt32(txtQuantity.Text);
            decimal price = GetItemPrice(itemID);
            decimal totalAmount = quantity * price;
            string returnReason = txtReturnReason.Text;

            DataRow newRow = returnItems.NewRow();
            newRow["ItemID"] = itemID;
            newRow["ItemName"] = itemName;
            newRow["Quantity"] = quantity;
            newRow["Price"] = price;
            newRow["TotalAmount"] = totalAmount;
            newRow["ReturnReason"] = returnReason;

            returnItems.Rows.Add(newRow);
            dataGridView1.DataSource = returnItems;
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
            catch (Exception ex)
            {
                MessageBox.Show("Error getting item price: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        private void CalculateTotals()
        {
            totalAmount = 0;
            foreach (DataRow row in returnItems.Rows)
            {
                totalAmount += Convert.ToDecimal(row["TotalAmount"]);
            }
            txtTotalAmount.Text = totalAmount.ToString("N2");
        }

        private void ClearItemInputs()
        {
            txtQuantity.Text = "";
            txtReturnReason.Text = "";
            cmbItem.Focus();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (ValidateReturn())
            {
                try
                {
                    SaveReturn();
                    MessageBox.Show("Purchase return saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearForm();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error saving purchase return: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private bool ValidateReturn()
        {
            if (cmbCompany.SelectedValue == null)
            {
                MessageBox.Show("Please select a company.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (returnItems.Rows.Count == 0)
            {
                MessageBox.Show("Please add at least one item to return.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtRemarks.Text))
            {
                MessageBox.Show("Please enter remarks.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void SaveReturn()
        {
            using (SqlConnection connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    // Insert Purchase Return Header
                    string headerQuery = @"INSERT INTO PurchaseReturns (ReturnNumber, CompanyID, ReturnDate, TotalAmount, Remarks, IsActive, CreatedDate) 
                                         VALUES (@ReturnNumber, @CompanyID, @ReturnDate, @TotalAmount, @Remarks, 1, @CreatedDate);
                                         SELECT SCOPE_IDENTITY();";

                    SqlParameter[] headerParams = {
                        new SqlParameter("@ReturnNumber", txtReturnNumber.Text),
                        new SqlParameter("@CompanyID", cmbCompany.SelectedValue),
                        new SqlParameter("@ReturnDate", dtpReturnDate.Value),
                        new SqlParameter("@TotalAmount", totalAmount),
                        new SqlParameter("@Remarks", txtRemarks.Text),
                        new SqlParameter("@CreatedDate", DateTime.Now)
                    };

                    int returnID = Convert.ToInt32(DatabaseConnection.ExecuteScalar(headerQuery, headerParams));

                    // Insert Purchase Return Items
                    foreach (DataRow row in returnItems.Rows)
                    {
                        string itemQuery = @"INSERT INTO PurchaseReturnItems (ReturnID, ItemID, Quantity, UnitPrice, TotalAmount, ReturnReason) 
                                           VALUES (@ReturnID, @ItemID, @Quantity, @UnitPrice, @TotalAmount, @ReturnReason)";

                        SqlParameter[] itemParams = {
                            new SqlParameter("@ReturnID", returnID),
                            new SqlParameter("@ItemID", row["ItemID"]),
                            new SqlParameter("@Quantity", row["Quantity"]),
                            new SqlParameter("@UnitPrice", row["Price"]),
                            new SqlParameter("@TotalAmount", row["TotalAmount"]),
                            new SqlParameter("@ReturnReason", row["ReturnReason"])
                        };

                        DatabaseConnection.ExecuteNonQuery(itemQuery, itemParams);

                        // Update stock quantity
                        string updateStockQuery = "UPDATE Items SET StockQuantity = StockQuantity - @Quantity WHERE ItemID = @ItemID";
                        SqlParameter[] stockParams = {
                            new SqlParameter("@Quantity", row["Quantity"]),
                            new SqlParameter("@ItemID", row["ItemID"])
                        };
                        DatabaseConnection.ExecuteNonQuery(updateStockQuery, stockParams);
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        private void ClearForm()
        {
            txtReturnNumber.Text = "";
            cmbCompany.SelectedIndex = -1;
            dtpReturnDate.Value = DateTime.Now;
            txtRemarks.Text = "";
            returnItems.Clear();
            dataGridView1.DataSource = returnItems;
            txtTotalAmount.Text = "0.00";
            totalAmount = 0;
            GenerateReturnNumber();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmbCompany_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbCompany.SelectedValue != null)
            {
                selectedCompanyID = Convert.ToInt32(cmbCompany.SelectedValue);
            }
        }
    }
} 