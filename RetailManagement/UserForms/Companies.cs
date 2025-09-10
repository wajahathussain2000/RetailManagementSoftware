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
    public partial class Companies : Form
    {
        private bool isEditMode = false;
        private int selectedCompanyID = 0;
        private DataTable originalData;

        public Companies()
        {
            InitializeComponent();
            LoadCompanies();
            SetupDataGridView();
            LoadCompaniesDropdown();
            InitializeForm();
        }

        private void SetupDataGridView()
        {
            gridViewCompanies.AutoGenerateColumns = false;
            gridViewCompanies.Columns.Clear();

            gridViewCompanies.Columns.Add("CompanyID", "Computer ID");
            gridViewCompanies.Columns.Add("CompanyName", "Computer Name");

            gridViewCompanies.Columns["CompanyID"].DataPropertyName = "CompanyID";
            gridViewCompanies.Columns["CompanyName"].DataPropertyName = "CompanyName";
        }

        private void LoadCompanies()
        {
            try
            {
                string query = "SELECT CompanyID, CompanyName FROM Companies WHERE IsActive = 1";
                originalData = DatabaseConnection.ExecuteQuery(query);
                gridViewCompanies.DataSource = originalData;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading computers: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeForm()
        {
            // Enable text boxes
            txtComputerName.Enabled = true;
            
            // Set initial state
            btnSave.Enabled = false;
            btnCancel.Enabled = false;
            
            // Set focus
            txtComputerName.Focus();
        }



        private void LoadCompaniesDropdown()
        {
            // Add a dropdown to select existing companies
            try
            {
                string query = "SELECT CompanyID, CompanyName FROM Companies WHERE IsActive = 1 ORDER BY CompanyName";
                DataTable dt = DatabaseConnection.ExecuteQuery(query);
                
                // Create and add dropdown if it doesn't exist
                if (this.Controls.Find("cmbCompanies", true).Length == 0)
                {
                    ComboBox cmbCompanies = new ComboBox
                    {
                        Name = "cmbCompanies",
                        Location = new Point(442, 30),
                        Size = new Size(240, 25),
                        DropDownStyle = ComboBoxStyle.DropDownList,
                        DataSource = dt,
                        DisplayMember = "CompanyName",
                        ValueMember = "CompanyID"
                    };
                    
                    cmbCompanies.SelectedIndexChanged += CmbCompanies_SelectedIndexChanged;
                    this.Controls.Add(cmbCompanies);
                    
                    // Add label for dropdown
                    Label lblCompanies = new Label
                    {
                        Text = "Select Company:",
                        Location = new Point(442, 10),
                        Size = new Size(100, 20),
                        Font = new Font("Microsoft Sans Serif", 9, FontStyle.Bold),
                        ForeColor = Color.White
                    };
                    this.Controls.Add(lblCompanies);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading companies dropdown: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CmbCompanies_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cmb = sender as ComboBox;
            if (cmb.SelectedValue != null && int.TryParse(cmb.SelectedValue.ToString(), out int companyId))
            {
                LoadCompanyDetails(companyId);
            }
        }

        private void LoadCompanyDetails(int companyId)
        {
            try
            {
                string query = "SELECT * FROM Companies WHERE CompanyID = @CompanyID";
                SqlParameter[] parameters = { new SqlParameter("@CompanyID", companyId) };
                DataTable dt = DatabaseConnection.ExecuteQuery(query, parameters);
                
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    txtComputerID.Text = row["CompanyID"].ToString();
                    txtComputerName.Text = row["CompanyName"].ToString();
                    selectedCompanyID = companyId;
                    isEditMode = true;
                    btnSave.Enabled = true;
                    btnCancel.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading company details: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearForm()
        {
            txtComputerID.Text = "";
            txtComputerName.Text = "";
            selectedCompanyID = 0;
            isEditMode = false;
            
            // Clear dropdown selection
            ComboBox cmbCompanies = this.Controls.Find("cmbCompanies", true).FirstOrDefault() as ComboBox;
            if (cmbCompanies != null)
            {
                cmbCompanies.SelectedIndex = -1;
            }
            
            // Clear search box
            txtSearch.Clear();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            ClearForm();
            btnSave.Enabled = true;
            btnCancel.Enabled = true;
            txtComputerName.Focus();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                try
                {
                    if (isEditMode)
                    {
                        UpdateCompany();
                    }
                    else
                    {
                        InsertCompany();
                    }
                    LoadCompanies();
                    LoadCompaniesDropdown(); // Refresh dropdown
                    ClearForm();
                    btnSave.Enabled = false;
                    btnCancel.Enabled = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error saving company: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void InsertCompany()
        {
            string query = @"INSERT INTO Companies (CompanyName, ContactPerson, Phone, Email, Address, CreatedDate, IsActive) 
                           VALUES (@CompanyName, @ContactPerson, @Phone, @Email, @Address, @CreatedDate, 1)";

            SqlParameter[] parameters = {
                new SqlParameter("@CompanyName", txtComputerName.Text.Trim()),
                new SqlParameter("@ContactPerson", ""), // Not available in form
                new SqlParameter("@Phone", ""), // Not available in form
                new SqlParameter("@Email", ""), // Not available in form
                new SqlParameter("@Address", ""), // Not available in form
                new SqlParameter("@CreatedDate", DateTime.Now)
            };

            DatabaseConnection.ExecuteNonQuery(query, parameters);
            MessageBox.Show("Computer added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void UpdateCompany()
        {
            string query = @"UPDATE Companies SET CompanyName = @CompanyName, ContactPerson = @ContactPerson, 
                           Phone = @Phone, Email = @Email, Address = @Address 
                           WHERE CompanyID = @CompanyID";

            SqlParameter[] parameters = {
                new SqlParameter("@CompanyID", selectedCompanyID),
                new SqlParameter("@CompanyName", txtComputerName.Text.Trim()),
                new SqlParameter("@ContactPerson", ""), // Not available in form
                new SqlParameter("@Phone", ""), // Not available in form
                new SqlParameter("@Email", ""), // Not available in form
                new SqlParameter("@Address", "") // Not available in form
            };

            DatabaseConnection.ExecuteNonQuery(query, parameters);
            MessageBox.Show("Computer updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(txtComputerName.Text))
            {
                MessageBox.Show("Please enter computer name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtComputerName.Focus();
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
            if (selectedCompanyID > 0)
            {
                // Get company name for confirmation
                string companyName = txtComputerName.Text;
                
                if (MessageBox.Show($"Are you sure you want to delete '{companyName}'?\n\nThis action cannot be undone.", 
                    "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    try
                    {
                        // Check if company is being used in other tables
                        string checkQuery = @"
                            SELECT COUNT(*) FROM Sales WHERE CustomerID = @CompanyID
                            UNION ALL
                            SELECT COUNT(*) FROM Purchases WHERE CompanyID = @CompanyID";
                        
                        SqlParameter[] checkParams = { new SqlParameter("@CompanyID", selectedCompanyID) };
                        DataTable checkResult = DatabaseConnection.ExecuteQuery(checkQuery, checkParams);
                        
                        bool hasReferences = false;
                        foreach (DataRow row in checkResult.Rows)
                        {
                            if (Convert.ToInt32(row[0]) > 0)
                            {
                                hasReferences = true;
                                break;
                            }
                        }
                        
                        if (hasReferences)
                        {
                            MessageBox.Show($"Cannot delete '{companyName}' because it is being used in transactions.\n\nPlease contact administrator for assistance.", 
                                "Cannot Delete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        
                        // Soft delete by setting IsActive = 0
                        string query = "UPDATE Companies SET IsActive = 0 WHERE CompanyID = @CompanyID";
                        SqlParameter[] parameters = { new SqlParameter("@CompanyID", selectedCompanyID) };
                        DatabaseConnection.ExecuteNonQuery(query, parameters);
                        
                        LoadCompanies();
                        LoadCompaniesDropdown(); // Refresh dropdown
                        ClearForm();
                        txtSearch.Clear(); // Clear search box
                        
                        MessageBox.Show($"'{companyName}' has been deleted successfully!", "Success", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error deleting company: " + ex.Message, "Error", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a company to delete.\n\nClick on a row in the list to select it.", 
                    "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void gridViewCompanies_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = gridViewCompanies.Rows[e.RowIndex];
                selectedCompanyID = SafeDataHelper.SafeGetCellInt32(row, "CompanyID");
                txtComputerID.Text = SafeDataHelper.SafeGetCellString(row, "CompanyID");
                txtComputerName.Text = SafeDataHelper.SafeGetCellString(row, "CompanyName");
                isEditMode = true;
                btnSave.Enabled = true;
                btnCancel.Enabled = true;
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (originalData != null)
            {
                string searchText = txtSearch.Text.Trim().ToLower();
                
                if (string.IsNullOrEmpty(searchText))
                {
                    // Show all data if search is empty
                    gridViewCompanies.DataSource = originalData;
                }
                else
                {
                    // Filter data based on search text
                    DataTable filteredData = originalData.Clone();
                    
                    foreach (DataRow row in originalData.Rows)
                    {
                        string companyName = row["CompanyName"].ToString().ToLower();
                        string companyID = row["CompanyID"].ToString().ToLower();
                        
                        if (companyName.Contains(searchText) || companyID.Contains(searchText))
                        {
                            filteredData.ImportRow(row);
                        }
                    }
                    
                    gridViewCompanies.DataSource = filteredData;
                }
            }
        }

        private void txtComputerName_KeyDown(object sender, KeyEventArgs e)
        {
            // Allow Enter key to add new line
            if (e.KeyCode == Keys.Enter)
            {
                // Move cursor to end and add new line
                TextBox textBox = sender as TextBox;
                int selectionStart = textBox.SelectionStart;
                textBox.Text = textBox.Text.Insert(selectionStart, Environment.NewLine);
                textBox.SelectionStart = selectionStart + Environment.NewLine.Length;
                e.Handled = true;
            }
        }
    }
}
