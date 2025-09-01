using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using RetailManagement.Database;

namespace RetailManagement.UserForms
{
    public partial class UserManagement : Form
    {
        private DataTable usersData;
        private int selectedUserId = 0;

        public UserManagement()
        {
            InitializeComponent();
            SetupDataGridView();
            LoadUsers();
        }

        private void SetupDataGridView()
        {
            dgvUsers.Columns.Clear();
            
            dgvUsers.Columns.Add("UserID", "User ID");
            dgvUsers.Columns.Add("Username", "Username");
            dgvUsers.Columns.Add("FullName", "Full Name");
            dgvUsers.Columns.Add("Role", "Role");
            dgvUsers.Columns.Add("Email", "Email");
            dgvUsers.Columns.Add("Phone", "Phone");
            dgvUsers.Columns.Add("IsActive", "Status");
            dgvUsers.Columns.Add("LastLogin", "Last Login");

            // Configure columns
            dgvUsers.Columns["UserID"].Width = 80;
            dgvUsers.Columns["Username"].Width = 120;
            dgvUsers.Columns["FullName"].Width = 150;
            dgvUsers.Columns["Role"].Width = 100;
            dgvUsers.Columns["Email"].Width = 180;
            dgvUsers.Columns["Phone"].Width = 120;
            dgvUsers.Columns["IsActive"].Width = 80;
            dgvUsers.Columns["LastLogin"].Width = 120;

            // Hide UserID column
            dgvUsers.Columns["UserID"].Visible = false;

            // Set alignment
            dgvUsers.Columns["IsActive"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Set header alignment
            dgvUsers.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvUsers.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
        }

        private void LoadUsers()
        {
            try
            {
                string query = @"SELECT 
                                    UserID,
                                    Username,
                                    FullName,
                                    Role,
                                    Email,
                                    Phone,
                                    IsActive,
                                    LastLogin,
                                    CreatedDate
                                FROM Users 
                                ORDER BY Username";

                usersData = DatabaseConnection.ExecuteQuery(query);
                ShowUsersData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading users: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowUsersData()
        {
            dgvUsers.Rows.Clear();

            if (usersData != null && usersData.Rows.Count > 0)
            {
                foreach (DataRow row in usersData.Rows)
                {
                    int rowIndex = dgvUsers.Rows.Add();
                    dgvUsers.Rows[rowIndex].Cells["UserID"].Value = row["UserID"];
                    dgvUsers.Rows[rowIndex].Cells["Username"].Value = row["Username"];
                    dgvUsers.Rows[rowIndex].Cells["FullName"].Value = row["FullName"];
                    dgvUsers.Rows[rowIndex].Cells["Role"].Value = row["Role"];
                    dgvUsers.Rows[rowIndex].Cells["Email"].Value = row["Email"];
                    dgvUsers.Rows[rowIndex].Cells["Phone"].Value = row["Phone"];
                    dgvUsers.Rows[rowIndex].Cells["IsActive"].Value = Convert.ToBoolean(row["IsActive"]) ? "Active" : "Inactive";
                    dgvUsers.Rows[rowIndex].Cells["LastLogin"].Value = row["LastLogin"] != DBNull.Value ? 
                        Convert.ToDateTime(row["LastLogin"]).ToString("dd/MM/yyyy HH:mm") : "Never";

                    // Color coding based on status
                    if (Convert.ToBoolean(row["IsActive"]))
                    {
                        dgvUsers.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightGreen;
                        dgvUsers.Rows[rowIndex].Cells["IsActive"].Style.ForeColor = Color.Green;
                    }
                    else
                    {
                        dgvUsers.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightCoral;
                        dgvUsers.Rows[rowIndex].Cells["IsActive"].Style.ForeColor = Color.Red;
                    }
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            ClearForm();
            EnableForm(true);
            btnAdd.Enabled = false;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
            btnSave.Enabled = true;
            btnCancel.Enabled = true;
            selectedUserId = 0;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedUserId > 0)
            {
                EnableForm(true);
                btnAdd.Enabled = false;
                btnUpdate.Enabled = false;
                btnDelete.Enabled = false;
                btnSave.Enabled = true;
                btnCancel.Enabled = true;
            }
            else
            {
                MessageBox.Show("Please select a user to update.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedUserId > 0)
            {
                DialogResult result = MessageBox.Show("Are you sure you want to delete this user?", "Confirm Delete", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        string query = "UPDATE Users SET IsActive = 0 WHERE UserID = @UserID";
                        SqlParameter[] parameters = {
                            new SqlParameter("@UserID", selectedUserId)
                        };

                        DatabaseConnection.ExecuteNonQuery(query, parameters);
                        MessageBox.Show("User deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadUsers();
                        ClearForm();
                        EnableForm(false);
                        btnAdd.Enabled = true;
                        btnUpdate.Enabled = false;
                        btnDelete.Enabled = false;
                        btnSave.Enabled = false;
                        btnCancel.Enabled = false;
                        selectedUserId = 0;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error deleting user: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a user to delete.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                try
                {
                    if (selectedUserId == 0)
                    {
                        // Add new user
                        string query = @"INSERT INTO Users 
                                        (Username, Password, FullName, Role, Email, Phone, IsActive, CreatedDate) 
                                        VALUES (@Username, @Password, @FullName, @Role, @Email, @Phone, 1, GETDATE())";

                        SqlParameter[] parameters = {
                            new SqlParameter("@Username", txtUsername.Text.Trim()),
                            new SqlParameter("@Password", txtPassword.Text), // In production, hash the password
                            new SqlParameter("@FullName", txtFullName.Text.Trim()),
                            new SqlParameter("@Role", cmbRole.Text),
                            new SqlParameter("@Email", txtEmail.Text.Trim()),
                            new SqlParameter("@Phone", txtPhone.Text.Trim())
                        };

                        DatabaseConnection.ExecuteNonQuery(query, parameters);
                        MessageBox.Show("User added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        // Update existing user
                        string query = @"UPDATE Users 
                                        SET Username = @Username, 
                                            FullName = @FullName, 
                                            Role = @Role, 
                                            Email = @Email, 
                                            Phone = @Phone 
                                        WHERE UserID = @UserID";

                        SqlParameter[] parameters = {
                            new SqlParameter("@UserID", selectedUserId),
                            new SqlParameter("@Username", txtUsername.Text.Trim()),
                            new SqlParameter("@FullName", txtFullName.Text.Trim()),
                            new SqlParameter("@Role", cmbRole.Text),
                            new SqlParameter("@Email", txtEmail.Text.Trim()),
                            new SqlParameter("@Phone", txtPhone.Text.Trim())
                        };

                        DatabaseConnection.ExecuteNonQuery(query, parameters);
                        MessageBox.Show("User updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    LoadUsers();
                    ClearForm();
                    EnableForm(false);
                    btnAdd.Enabled = true;
                    btnUpdate.Enabled = false;
                    btnDelete.Enabled = false;
                    btnSave.Enabled = false;
                    btnCancel.Enabled = false;
                    selectedUserId = 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error saving user: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearForm();
            EnableForm(false);
            btnAdd.Enabled = true;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
            btnSave.Enabled = false;
            btnCancel.Enabled = false;
            selectedUserId = 0;
        }

        private void btnChangePassword_Click(object sender, EventArgs e)
        {
            if (selectedUserId > 0)
            {
                // Open password change form
                PasswordForm passwordForm = new PasswordForm(selectedUserId);
                passwordForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please select a user to change password.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ClearForm()
        {
            txtUsername.Text = "";
            txtPassword.Text = "";
            txtFullName.Text = "";
            cmbRole.SelectedIndex = 0;
            txtEmail.Text = "";
            txtPhone.Text = "";
        }

        private void EnableForm(bool enable)
        {
            txtUsername.Enabled = enable;
            txtPassword.Enabled = enable && selectedUserId == 0; // Only enable password for new users
            txtFullName.Enabled = enable;
            cmbRole.Enabled = enable;
            txtEmail.Enabled = enable;
            txtPhone.Enabled = enable;
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Please enter username.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsername.Focus();
                return false;
            }

            if (selectedUserId == 0 && string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Please enter password.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Please enter full name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFullName.Focus();
                return false;
            }

            if (cmbRole.SelectedIndex == 0)
            {
                MessageBox.Show("Please select a role.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbRole.Focus();
                return false;
            }

            if (!string.IsNullOrWhiteSpace(txtEmail.Text) && !IsValidEmail(txtEmail.Text))
            {
                MessageBox.Show("Please enter a valid email address.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return false;
            }

            return true;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void dgvUsers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvUsers.Rows[e.RowIndex];
                selectedUserId = SafeDataHelper.SafeGetCellInt32(row, "UserID");
                
                txtUsername.Text = SafeDataHelper.SafeGetCellString(row, "Username");
                txtFullName.Text = SafeDataHelper.SafeGetCellString(row, "FullName");
                cmbRole.Text = SafeDataHelper.SafeGetCellString(row, "Role");
                txtEmail.Text = SafeDataHelper.SafeGetCellString(row, "Email");
                txtPhone.Text = SafeDataHelper.SafeGetCellString(row, "Phone");
                
                btnUpdate.Enabled = true;
                btnDelete.Enabled = true;
                btnChangePassword.Enabled = true;
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            FilterUsers();
        }

        private void FilterUsers()
        {
            if (usersData != null)
            {
                string searchText = txtSearch.Text.ToLower();
                
                dgvUsers.Rows.Clear();
                
                foreach (DataRow row in usersData.Rows)
                {
                    bool match = false;
                    
                    if (row["Username"].ToString().ToLower().Contains(searchText) ||
                        row["FullName"].ToString().ToLower().Contains(searchText) ||
                        row["Role"].ToString().ToLower().Contains(searchText) ||
                        row["Email"].ToString().ToLower().Contains(searchText))
                    {
                        match = true;
                    }
                    
                    if (match)
                    {
                        int rowIndex = dgvUsers.Rows.Add();
                        dgvUsers.Rows[rowIndex].Cells["UserID"].Value = row["UserID"];
                        dgvUsers.Rows[rowIndex].Cells["Username"].Value = row["Username"];
                        dgvUsers.Rows[rowIndex].Cells["FullName"].Value = row["FullName"];
                        dgvUsers.Rows[rowIndex].Cells["Role"].Value = row["Role"];
                        dgvUsers.Rows[rowIndex].Cells["Email"].Value = row["Email"];
                        dgvUsers.Rows[rowIndex].Cells["Phone"].Value = row["Phone"];
                        dgvUsers.Rows[rowIndex].Cells["IsActive"].Value = Convert.ToBoolean(row["IsActive"]) ? "Active" : "Inactive";
                        dgvUsers.Rows[rowIndex].Cells["LastLogin"].Value = row["LastLogin"] != DBNull.Value ? 
                            Convert.ToDateTime(row["LastLogin"]).ToString("dd/MM/yyyy HH:mm") : "Never";

                        // Color coding based on status
                        if (Convert.ToBoolean(row["IsActive"]))
                        {
                            dgvUsers.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightGreen;
                            dgvUsers.Rows[rowIndex].Cells["IsActive"].Style.ForeColor = Color.Green;
                        }
                        else
                        {
                            dgvUsers.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightCoral;
                            dgvUsers.Rows[rowIndex].Cells["IsActive"].Style.ForeColor = Color.Red;
                        }
                    }
                }
            }
        }
    }
} 