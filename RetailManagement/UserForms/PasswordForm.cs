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
    public partial class PasswordForm : Form
    {
        private int userId;
        private string username;

        // Helper class for ComboBox items
        private class ComboBoxItem
        {
            public string Text { get; set; }
            public object Value { get; set; }
            public override string ToString() => Text;
        }

        public PasswordForm(int userId)
        {
            InitializeComponent();
            this.userId = userId;
            LoadUserInfo();
        }

        public PasswordForm()
        {
            InitializeComponent();
            LoadUserList();
        }

        private void LoadUserInfo()
        {
            try
            {
                string query = "SELECT Username, FullName FROM Users WHERE UserID = @UserID";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@UserID", userId)
                };

                DataTable userData = DatabaseConnection.ExecuteQuery(query, parameters);
                if (userData.Rows.Count > 0)
                {
                    username = userData.Rows[0]["Username"].ToString();
                    string fullName = userData.Rows[0]["FullName"].ToString();
                    lblUserInfo.Text = $"Change Password for: {fullName} ({username})";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading user information: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadUserList()
        {
            try
            {
                string query = "SELECT UserID, Username, FullName FROM Users WHERE IsActive = 1 ORDER BY FullName";
                DataTable users = DatabaseConnection.ExecuteQuery(query);
                
                if (users.Rows.Count > 0)
                {
                    // Create a simple user selection dialog
                    using (Form userSelectForm = new Form())
                    {
                        userSelectForm.Text = "Select User";
                        userSelectForm.Size = new Size(300, 200);
                        userSelectForm.StartPosition = FormStartPosition.CenterParent;
                        userSelectForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                        userSelectForm.MaximizeBox = false;
                        userSelectForm.MinimizeBox = false;

                        ComboBox cmbUsers = new ComboBox();
                        cmbUsers.DropDownStyle = ComboBoxStyle.DropDownList;
                        cmbUsers.Location = new Point(20, 20);
                        cmbUsers.Size = new Size(240, 25);
                        cmbUsers.Font = new Font("Segoe UI", 10);

                        // Add users to combobox
                        foreach (DataRow row in users.Rows)
                        {
                            int userId = Convert.ToInt32(row["UserID"]);
                            string fullName = row["FullName"].ToString();
                            string username = row["Username"].ToString();
                            cmbUsers.Items.Add(new ComboBoxItem { Text = $"{fullName} ({username})", Value = userId });
                        }
                        cmbUsers.SelectedIndex = 0;

                        Button btnOK = new Button();
                        btnOK.Text = "OK";
                        btnOK.Location = new Point(100, 60);
                        btnOK.Size = new Size(80, 30);
                        btnOK.DialogResult = DialogResult.OK;

                        Button btnCancel = new Button();
                        btnCancel.Text = "Cancel";
                        btnCancel.Location = new Point(190, 60);
                        btnCancel.Size = new Size(80, 30);
                        btnCancel.DialogResult = DialogResult.Cancel;

                        Label lblSelect = new Label();
                        lblSelect.Text = "Select user to change password:";
                        lblSelect.Location = new Point(20, 5);
                        lblSelect.Size = new Size(200, 15);
                        lblSelect.Font = new Font("Segoe UI", 9, FontStyle.Bold);

                        userSelectForm.Controls.AddRange(new Control[] { lblSelect, cmbUsers, btnOK, btnCancel });

                        if (userSelectForm.ShowDialog() == DialogResult.OK)
                        {
                            ComboBoxItem selectedItem = (ComboBoxItem)cmbUsers.SelectedItem;
                            this.userId = (int)selectedItem.Value;
                            LoadUserInfo();
                        }
                        else
                        {
                            this.Close();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("No active users found.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading users: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (ValidateInputs())
            {
                try
                {
                    string newPassword = txtNewPassword.Text;
                    // In production, hash the password here
                    // newPassword = HashPassword(newPassword);

                    string query = "UPDATE Users SET Password = @Password WHERE UserID = @UserID";
                    SqlParameter[] parameters = new SqlParameter[]
                    {
                        new SqlParameter("@Password", newPassword),
                        new SqlParameter("@UserID", userId)
                    };

                    int result = DatabaseConnection.ExecuteNonQuery(query, parameters);
                    if (result > 0)
                    {
                        MessageBox.Show("Password updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Failed to update password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating password: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtCurrentPassword.Text))
            {
                MessageBox.Show("Please enter current password.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCurrentPassword.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtNewPassword.Text))
            {
                MessageBox.Show("Please enter new password.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNewPassword.Focus();
                return false;
            }

            if (txtNewPassword.Text.Length < 6)
            {
                MessageBox.Show("New password must be at least 6 characters long.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNewPassword.Focus();
                return false;
            }

            if (txtNewPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show("New password and confirm password do not match.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtConfirmPassword.Focus();
                return false;
            }

            // Verify current password
            try
            {
                string query = "SELECT Password FROM Users WHERE UserID = @UserID";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@UserID", userId)
                };

                object currentPassword = DatabaseConnection.ExecuteScalar(query, parameters);
                if (currentPassword != null && currentPassword.ToString() != txtCurrentPassword.Text)
                {
                    MessageBox.Show("Current password is incorrect.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCurrentPassword.Focus();
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error verifying current password: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
