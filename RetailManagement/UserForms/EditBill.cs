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
using RetailManagement.Utils;

namespace RetailManagement.UserForms
{
    public partial class EditBill : Form
    {
        private string currentBillType = "";
        private string currentInvoiceNumber = "";
        private int selectedTransactionID = 0;
        private DataTable transactionItems;
        private bool isFormLoaded = false; // TODO: Use for form state validation
        private int selectedSaleID = 0; // For legacy sale editing compatibility
        private DataTable saleItems; // For legacy sale items display

        public EditBill()
        {
            InitializeComponent();
            
            // First, ask user to select bill type and invoice number
            if (SelectBillTypeAndInvoice())
            {
                SetupForSelectedBillType();
                isFormLoaded = true;
            }
            else
            {
                // User cancelled, close the form
                this.Load += (s, e) => this.Close();
            }
        }

        private bool SelectBillTypeAndInvoice()
        {
            try
            {
                EditBillTypeSelector selector = new EditBillTypeSelector();
                if (selector.ShowDialog() == DialogResult.OK)
                {
                    currentBillType = selector.SelectedBillType;
                    currentInvoiceNumber = selector.InvoiceNumber;
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening bill type selector: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void SetupForSelectedBillType()
        {
            try
            {
                // Update form title to show current bill type
                this.Text = $"Edit {currentBillType} - {currentInvoiceNumber}";
                
                // Load the specific transaction
                LoadSpecificTransaction();
                
                // Setup data grid view based on bill type
                SetupDataGridView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error setting up form: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSpecificTransaction()
        {
            try
            {
                string tableName = GetTableNameForBillType();
                string idColumn = GetIdColumnForBillType();
                
                // Load transaction header
                string query = $@"SELECT t.*, c.CustomerName 
                                FROM {tableName} t
                                INNER JOIN Customers c ON t.CustomerID = c.CustomerID
                                WHERE t.BillNumber = @BillNumber AND t.IsActive = 1";

                SqlParameter[] parameters = { new SqlParameter("@BillNumber", currentInvoiceNumber) };
                DataTable dt = DatabaseConnection.ExecuteQuery(query, parameters);

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show($"Invoice {currentInvoiceNumber} not found in {currentBillType}.", 
                        "Invoice Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DataRow row = dt.Rows[0];
                selectedTransactionID = Convert.ToInt32(row[idColumn]);
                
                // Populate form fields - PRESERVE QR code and barcode data
                PopulateFormFields(row);
                
                // Load transaction items
                LoadTransactionItems();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading transaction: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetTableNameForBillType()
        {
            switch (currentBillType)
            {
                case "New Bill (Sales)":
                case "Credit Bill":
                    return "Sales";
                case "New Purchase":
                    return "Purchases";
                default:
                    return "Sales";
            }
        }

        private string GetIdColumnForBillType()
        {
            switch (currentBillType)
            {
                case "New Bill (Sales)":
                case "Credit Bill":
                    return "SaleID";
                case "New Purchase":
                    return "PurchaseID";
                default:
                    return "SaleID";
            }
        }

        private string GetItemsTableNameForBillType()
        {
            switch (currentBillType)
            {
                case "New Bill (Sales)":
                case "Credit Bill":
                    return "SaleItems";
                case "New Purchase":
                    return "PurchaseItems";
                default:
                    return "SaleItems";
            }
        }

        private void PopulateFormFields(DataRow row)
        {
            try
            {
                // Populate header fields - NOTE: DO NOT allow editing of customer, date, invoice number, QR, barcode
                txtBillNumber.Text = row["BillNumber"].ToString();
                txtBillNumber.ReadOnly = true; // Cannot edit invoice number
                
                txtCustomerName.Text = row["CustomerName"].ToString();
                txtCustomerName.ReadOnly = true; // Cannot edit customer name
                
                txtSaleDate.Value = Convert.ToDateTime(row[GetDateColumnForBillType()]);
                txtSaleDate.Enabled = false; // Cannot edit date
                
                txtNetAmount.Text = SafeDataHelper.SafeToDecimal(row["NetAmount"]).ToString("N2");
                
                // Show warning about what cannot be edited
                MessageBox.Show($"Editing {currentBillType}\n\n" +
                    "IMPORTANT: You can only edit:\n" +
                    "• Item quantities and prices\n" +
                    "• Discount amounts\n" +
                    "• Remarks\n\n" +
                    "You CANNOT edit:\n" +
                    "• Customer name\n" +
                    "• Invoice number\n" +
                    "• Date\n" +
                    "• QR code and barcode (they remain the same)", 
                    "Edit Restrictions", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error populating fields: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetDateColumnForBillType()
        {
            switch (currentBillType)
            {
                case "New Bill (Sales)":
                case "Credit Bill":
                    return "SaleDate";
                case "New Purchase":
                    return "PurchaseDate";
                default:
                    return "SaleDate";
            }
        }

        private void LoadTransactionItems()
        {
            try
            {
                string itemsTable = GetItemsTableNameForBillType();
                string idColumn = GetIdColumnForBillType();
                
                string query = $@"SELECT si.*, i.ItemName 
                                FROM {itemsTable} si
                                INNER JOIN Items i ON si.ItemID = i.ItemID
                                WHERE si.{idColumn} = @TransactionID";

                SqlParameter[] parameters = { new SqlParameter("@TransactionID", selectedTransactionID) };
                transactionItems = DatabaseConnection.ExecuteQuery(query, parameters);
                
                // Display items in the data grid
                dataGridView1.DataSource = transactionItems;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading transaction items: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                selectedSaleID = SafeDataHelper.SafeGetCellInt32(row, "SaleID");
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
            if (selectedTransactionID == 0)
            {
                MessageBox.Show($"No {currentBillType} loaded to edit.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (ValidateUpdate())
            {
                try
                {
                    UpdateSale();
                    // Reload the transaction to show updated data
                    LoadSpecificTransaction();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating {currentBillType}: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            try
            {
                decimal netAmount = decimal.Parse(txtNetAmount.Text);
                string tableName = GetTableNameForBillType();
                string idColumn = GetIdColumnForBillType();

                // IMPORTANT: Update only editable fields, preserve QR code and barcode
                string updateQuery = $@"UPDATE {tableName} 
                                      SET NetAmount = @NetAmount
                                      WHERE {idColumn} = @TransactionID";

                SqlParameter[] parameters = {
                    new SqlParameter("@NetAmount", netAmount),
                    new SqlParameter("@TransactionID", selectedTransactionID)
                };

                DatabaseConnection.ExecuteNonQuery(updateQuery, parameters);
                
                MessageBox.Show($"{currentBillType} updated successfully!\n\nNote: QR code and barcode data remain unchanged as required.", 
                    "Update Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating {currentBillType}: {ex.Message}", "Update Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

