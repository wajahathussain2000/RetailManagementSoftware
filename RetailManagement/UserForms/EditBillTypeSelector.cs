using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using RetailManagement.Database;

namespace RetailManagement.UserForms
{
    public partial class EditBillTypeSelector : Form
    {
        public string SelectedBillType { get; private set; }
        public string InvoiceNumber { get; private set; }
        public int SelectedInvoiceID { get; private set; }

        private DataTable invoicesData;

        public EditBillTypeSelector()
        {
            InitializeComponent();
            
            // Add items to combo box
            cmbBillType.Items.AddRange(new object[] {
                "Sales",
                "Purchase"
            });
            cmbBillType.SelectedIndex = 0;
            
            // Add event handlers
            cmbBillType.SelectedIndexChanged += new EventHandler(this.cmbBillType_SelectedIndexChanged);
            txtSearch.TextChanged += new EventHandler(this.txtSearch_TextChanged);
            dgvInvoices.CellDoubleClick += new DataGridViewCellEventHandler(this.dgvInvoices_CellDoubleClick);
            btnRefresh.Click += new EventHandler(this.btnRefresh_Click);
            btnOK.Click += new EventHandler(this.btnOK_Click);
            btnCancel.Click += new EventHandler(this.btnCancel_Click);
            
            // Load initial data
            LoadInvoices();
        }

        private void EnsureRequiredColumnsExist()
        {
            try
            {
                // Check and add BillNumber column to Sales table
                string checkSalesQuery = @"SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS 
                                         WHERE TABLE_NAME = 'Sales' AND COLUMN_NAME = 'BillNumber'";
                DataTable salesResult = DatabaseConnection.ExecuteQuery(checkSalesQuery, null);
                int salesColumnExists = Convert.ToInt32(salesResult.Rows[0][0]);
                
                if (salesColumnExists == 0)
                {
                    string addSalesColumnQuery = @"ALTER TABLE Sales ADD BillNumber VARCHAR(50) NULL";
                    DatabaseConnection.ExecuteNonQuery(addSalesColumnQuery, null);
                    System.Diagnostics.Debug.WriteLine("BillNumber column added to Sales table");
                }
                
                // Check and add BillNumber column to Purchases table
                string checkPurchasesQuery = @"SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS 
                                             WHERE TABLE_NAME = 'Purchases' AND COLUMN_NAME = 'BillNumber'";
                DataTable purchasesResult = DatabaseConnection.ExecuteQuery(checkPurchasesQuery, null);
                int purchasesColumnExists = Convert.ToInt32(purchasesResult.Rows[0][0]);
                
                if (purchasesColumnExists == 0)
                {
                    string addPurchasesColumnQuery = @"ALTER TABLE Purchases ADD BillNumber VARCHAR(50) NULL";
                    DatabaseConnection.ExecuteNonQuery(addPurchasesColumnQuery, null);
                    System.Diagnostics.Debug.WriteLine("BillNumber column added to Purchases table");
                }
                
                // Check and add IsActive column to Sales table
                string checkSalesActiveQuery = @"SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS 
                                              WHERE TABLE_NAME = 'Sales' AND COLUMN_NAME = 'IsActive'";
                DataTable salesActiveResult = DatabaseConnection.ExecuteQuery(checkSalesActiveQuery, null);
                int salesActiveColumnExists = Convert.ToInt32(salesActiveResult.Rows[0][0]);
                
                if (salesActiveColumnExists == 0)
                {
                    string addSalesActiveColumnQuery = @"ALTER TABLE Sales ADD IsActive BIT NOT NULL DEFAULT 1";
                    DatabaseConnection.ExecuteNonQuery(addSalesActiveColumnQuery, null);
                    System.Diagnostics.Debug.WriteLine("IsActive column added to Sales table");
                }
                
                // Check and add IsActive column to Purchases table
                string checkPurchasesActiveQuery = @"SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS 
                                                  WHERE TABLE_NAME = 'Purchases' AND COLUMN_NAME = 'IsActive'";
                DataTable purchasesActiveResult = DatabaseConnection.ExecuteQuery(checkPurchasesActiveQuery, null);
                int purchasesActiveColumnExists = Convert.ToInt32(purchasesActiveResult.Rows[0][0]);
                
                if (purchasesActiveColumnExists == 0)
                {
                    string addPurchasesActiveColumnQuery = @"ALTER TABLE Purchases ADD IsActive BIT NOT NULL DEFAULT 1";
                    DatabaseConnection.ExecuteNonQuery(addPurchasesActiveColumnQuery, null);
                    System.Diagnostics.Debug.WriteLine("IsActive column added to Purchases table");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error ensuring required columns exist: {ex.Message}");
                // Don't throw exception, just log it
            }
        }


        private void LoadInvoices()
        {
            try
            {
                // First ensure required columns exist
                EnsureRequiredColumnsExist();
                
                string billType = cmbBillType.SelectedItem.ToString();
                string query = "";
                
                if (billType == "Sales")
                {
                    query = @"SELECT s.SaleID as InvoiceID, 
                             ISNULL(s.BillNumber, 'BILL' + RIGHT('000000' + CAST(s.SaleID AS VARCHAR), 6)) as InvoiceNo,
                             c.CustomerName as CustomerSupplier, s.SaleDate as InvoiceDate,
                             ISNULL(s.NetAmount, 0) as TotalAmount, 
                             ISNULL(s.IsActive, 1) as IsActive
                             FROM Sales s
                             INNER JOIN Customers c ON s.CustomerID = c.CustomerID
                             WHERE ISNULL(s.IsActive, 1) = 1
                             ORDER BY s.SaleDate DESC";
                }
                else if (billType == "Purchase")
                {
                    query = @"SELECT p.PurchaseID as InvoiceID, 
                             ISNULL(p.BillNumber, 'PURCH' + RIGHT('000000' + CAST(p.PurchaseID AS VARCHAR), 6)) as InvoiceNo,
                             c.CompanyName as CustomerSupplier, p.PurchaseDate as InvoiceDate,
                             ISNULL(p.TotalAmount, 0) as TotalAmount,
                             ISNULL(p.IsActive, 1) as IsActive
                             FROM Purchases p
                             INNER JOIN Companies c ON p.CompanyID = c.CompanyID
                             WHERE ISNULL(p.IsActive, 1) = 1
                             ORDER BY p.PurchaseDate DESC";
                }

                invoicesData = DatabaseConnection.ExecuteQuery(query);
                dgvInvoices.DataSource = invoicesData;
                
                // Set column headers
                if (dgvInvoices.Columns.Count > 0)
                {
                    dgvInvoices.Columns["InvoiceID"].Visible = false; // Hide ID column
                    dgvInvoices.Columns["InvoiceNo"].HeaderText = "Invoice No";
                    dgvInvoices.Columns["CustomerSupplier"].HeaderText = billType == "Sales" ? "Customer" : "Supplier";
                    dgvInvoices.Columns["InvoiceDate"].HeaderText = "Date";
                    dgvInvoices.Columns["TotalAmount"].HeaderText = "Amount";
                    dgvInvoices.Columns["IsActive"].Visible = false; // Hide IsActive column
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading invoices: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cmbBillType_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadInvoices();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (invoicesData != null)
            {
                string searchText = txtSearch.Text.ToLower();
                if (string.IsNullOrWhiteSpace(searchText))
                {
                    dgvInvoices.DataSource = invoicesData;
                }
                else
                {
                    DataTable filteredData = invoicesData.Clone();
                    foreach (DataRow row in invoicesData.Rows)
                    {
                        if (row["InvoiceNo"].ToString().ToLower().Contains(searchText) ||
                            row["CustomerSupplier"].ToString().ToLower().Contains(searchText))
                        {
                            filteredData.ImportRow(row);
                        }
                    }
                    dgvInvoices.DataSource = filteredData;
                }
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadInvoices();
        }

        private void dgvInvoices_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                btnOK_Click(sender, e);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (dgvInvoices.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an invoice to edit.", "Selection Required", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow selectedRow = dgvInvoices.SelectedRows[0];
            SelectedBillType = cmbBillType.SelectedItem.ToString();
            SelectedInvoiceID = Convert.ToInt32(selectedRow.Cells["InvoiceID"].Value);
            InvoiceNumber = selectedRow.Cells["InvoiceNo"].Value.ToString();
            
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
