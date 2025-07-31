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

        public Companies()
        {
            InitializeComponent();
            LoadCompanies();
            SetupDataGridView();
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
                DataTable dt = DatabaseConnection.ExecuteQuery(query);
                gridViewCompanies.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading computers: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearForm()
        {
            txtComputerID.Text = "";
            txtComputerName.Text = "";
            selectedCompanyID = 0;
            isEditMode = false;
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
                if (MessageBox.Show("Are you sure you want to delete this computer?", "Confirm Delete", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        string query = "UPDATE Companies SET IsActive = 0 WHERE CompanyID = @CompanyID";
                        SqlParameter[] parameters = { new SqlParameter("@CompanyID", selectedCompanyID) };
                        DatabaseConnection.ExecuteNonQuery(query, parameters);
                        LoadCompanies();
                        ClearForm();
                        MessageBox.Show("Computer deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error deleting computer: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a computer to delete.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void gridViewCompanies_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = gridViewCompanies.Rows[e.RowIndex];
                selectedCompanyID = Convert.ToInt32(row.Cells["CompanyID"].Value);
                txtComputerID.Text = row.Cells["CompanyID"].Value.ToString();
                txtComputerName.Text = row.Cells["CompanyName"].Value.ToString();
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
