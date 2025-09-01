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
    public partial class ChangePassword : Form
    {
        public ChangePassword()
        {
            InitializeComponent();
            InitializeForm();
        }

        private void InitializeForm()
        {
            this.Text = "Change Password";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Size = new Size(400, 300);

            // Create controls
            CreateControls();
        }

        private void CreateControls()
        {
            // Main panel
            Panel mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                BackColor = Color.White
            };

            // Title label
            Label lblTitle = new Label
            {
                Text = "Change Password",
                Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold),
                Location = new Point(20, 20),
                Size = new Size(200, 25),
                ForeColor = Color.DarkBlue
            };

            // Current user info
            Label lblUser = new Label
            {
                Text = $"User: {UserSession.FullName ?? "Unknown"} ({UserSession.Username ?? "Unknown"})",
                Location = new Point(20, 50),
                Size = new Size(300, 20),
                Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular)
            };

            // Current password
            Label lblCurrentPassword = new Label
            {
                Text = "Current Password:",
                Location = new Point(20, 80),
                Size = new Size(120, 20),
                Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold)
            };

            TextBox txtCurrentPassword = new TextBox
            {
                Location = new Point(150, 78),
                Size = new Size(200, 20),
                UseSystemPasswordChar = true,
                Name = "txtCurrentPassword"
            };

            // New password
            Label lblNewPassword = new Label
            {
                Text = "New Password:",
                Location = new Point(20, 110),
                Size = new Size(120, 20),
                Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold)
            };

            TextBox txtNewPassword = new TextBox
            {
                Location = new Point(150, 108),
                Size = new Size(200, 20),
                UseSystemPasswordChar = true,
                Name = "txtNewPassword"
            };

            // Confirm password
            Label lblConfirmPassword = new Label
            {
                Text = "Confirm Password:",
                Location = new Point(20, 140),
                Size = new Size(120, 20),
                Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold)
            };

            TextBox txtConfirmPassword = new TextBox
            {
                Location = new Point(150, 138),
                Size = new Size(200, 20),
                UseSystemPasswordChar = true,
                Name = "txtConfirmPassword"
            };

            // Password requirements
            Label lblRequirements = new Label
            {
                Text = "Password Requirements:\n• Minimum 6 characters\n• Must be different from current password",
                Location = new Point(20, 170),
                Size = new Size(330, 50),
                Font = new Font("Microsoft Sans Serif", 8F, FontStyle.Italic),
                ForeColor = Color.Gray
            };

            // Buttons
            Button btnChange = new Button
            {
                Text = "Change Password",
                Location = new Point(150, 230),
                Size = new Size(120, 30),
                BackColor = Color.Green,
                ForeColor = Color.White,
                Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold)
            };
            btnChange.Click += BtnChange_Click;

            Button btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(280, 230),
                Size = new Size(70, 30),
                BackColor = Color.Gray,
                ForeColor = Color.White,
                Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold)
            };
            btnCancel.Click += BtnCancel_Click;

            // Add controls to panel
            mainPanel.Controls.AddRange(new Control[] {
                lblTitle, lblUser, lblCurrentPassword, txtCurrentPassword,
                lblNewPassword, txtNewPassword, lblConfirmPassword, txtConfirmPassword,
                lblRequirements, btnChange, btnCancel
            });

            this.Controls.Add(mainPanel);
        }

        private void BtnChange_Click(object sender, EventArgs e)
        {
            try
            {
                TextBox txtCurrentPassword = this.Controls.Find("txtCurrentPassword", true)[0] as TextBox;
                TextBox txtNewPassword = this.Controls.Find("txtNewPassword", true)[0] as TextBox;
                TextBox txtConfirmPassword = this.Controls.Find("txtConfirmPassword", true)[0] as TextBox;

                string currentPassword = txtCurrentPassword.Text.Trim();
                string newPassword = txtNewPassword.Text.Trim();
                string confirmPassword = txtConfirmPassword.Text.Trim();

                // Validate inputs
                if (string.IsNullOrEmpty(currentPassword))
                {
                    MessageBox.Show("Please enter your current password.", "Validation Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCurrentPassword.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(newPassword))
                {
                    MessageBox.Show("Please enter a new password.", "Validation Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtNewPassword.Focus();
                    return;
                }

                if (newPassword.Length < 6)
                {
                    MessageBox.Show("Password must be at least 6 characters long.", "Validation Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtNewPassword.Focus();
                    return;
                }

                if (newPassword != confirmPassword)
                {
                    MessageBox.Show("New password and confirm password do not match.", "Validation Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtConfirmPassword.Focus();
                    return;
                }

                if (currentPassword == newPassword)
                {
                    MessageBox.Show("New password must be different from current password.", "Validation Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtNewPassword.Focus();
                    return;
                }

                // Verify current password
                if (!VerifyCurrentPassword(currentPassword))
                {
                    MessageBox.Show("Current password is incorrect.", "Authentication Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtCurrentPassword.Focus();
                    return;
                }

                // Change password
                if (ChangeUserPassword(newPassword))
                {
                    MessageBox.Show("Password changed successfully!", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Log activity
                    UserSession.LogActivity("Password Changed", "Users", "User changed their password");
                    
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error changing password: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool VerifyCurrentPassword(string currentPassword)
        {
            try
            {
                string query = "SELECT COUNT(*) FROM Users WHERE UserID = @UserID AND Password = @Password";
                SqlParameter[] parameters = {
                    new SqlParameter("@UserID", UserSession.UserID),
                    new SqlParameter("@Password", currentPassword)
                };

                int count = Convert.ToInt32(DatabaseConnection.ExecuteScalar(query, parameters));
                return count > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error verifying current password: {ex.Message}");
            }
        }

        private bool ChangeUserPassword(string newPassword)
        {
            try
            {
                string query = @"UPDATE Users SET Password = @NewPassword, LastPasswordChangeDate = @ChangeDate, 
                               PasswordChangeRequired = 0 WHERE UserID = @UserID";
                
                SqlParameter[] parameters = {
                    new SqlParameter("@NewPassword", newPassword),
                    new SqlParameter("@ChangeDate", DateTime.Now),
                    new SqlParameter("@UserID", UserSession.UserID)
                };

                int rowsAffected = DatabaseConnection.ExecuteNonQuery(query, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating password: {ex.Message}");
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
