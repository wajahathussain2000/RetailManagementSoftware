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

namespace RetailManagement
{
    public partial class Login : UserControl
    {
        public event Action LoginSuccessful;
        private int loginAttempts = 0;
        private const int MAX_LOGIN_ATTEMPTS = 3;
        
        public Login()
        {
            InitializeComponent();
            InitializeLoginForm();
        }

        private void InitializeLoginForm()
        {
            // Set placeholder text
            if (textBox1 != null)
            {
                textBox1.Text = "Enter username";
                textBox1.ForeColor = Color.Gray;
                textBox1.GotFocus += Username_GotFocus;
                textBox1.LostFocus += Username_LostFocus;
                textBox1.Focus();
            }
            
            if (textBox2 != null)
            {
                textBox2.Text = "Enter password";
                textBox2.ForeColor = Color.Gray;
                textBox2.GotFocus += Password_GotFocus;
                textBox2.LostFocus += Password_LostFocus;
            }

            // Add Enter key support
            textBox1.KeyPress += Login_KeyPress;
            textBox2.KeyPress += Login_KeyPress;
        }

        private void Username_GotFocus(object sender, EventArgs e)
        {
            if (textBox1.Text == "Enter username")
            {
                textBox1.Text = "";
                textBox1.ForeColor = Color.Black;
            }
        }

        private void Username_LostFocus(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                textBox1.Text = "Enter username";
                textBox1.ForeColor = Color.Gray;
            }
        }

        private void Password_GotFocus(object sender, EventArgs e)
        {
            if (textBox2.Text == "Enter password")
            {
                textBox2.Text = "";
                textBox2.ForeColor = Color.Black;
                textBox2.UseSystemPasswordChar = true;
            }
        }

        private void Password_LostFocus(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                textBox2.Text = "Enter password";
                textBox2.ForeColor = Color.Gray;
                textBox2.UseSystemPasswordChar = false;
            }
        }

        private void Login_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnOK_Click(sender, e);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                string username = textBox1.Text.Trim();
                string password = textBox2.Text.Trim();

                // Check for placeholder text
                if (username == "Enter username") username = "";
                if (password == "Enter password") password = "";

                // Validate input
                if (string.IsNullOrEmpty(username))
                {
                    MessageBox.Show("Please enter username.", "Login Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox1.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(password))
                {
                    MessageBox.Show("Please enter password.", "Login Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox2.Focus();
                    return;
                }

                // Disable login button during authentication
                btnOK.Enabled = false;
                btnOK.Text = "Authenticating...";

                // Authenticate user
                if (AuthenticateUser(username, password))
                {
                    // Clear sensitive data
                    ClearForm();
                    
                    // Fire successful login event
            LoginSuccessful?.Invoke();
                }
                else
                {
                    loginAttempts++;
                    
                    if (loginAttempts >= MAX_LOGIN_ATTEMPTS)
                    {
                        MessageBox.Show($"Maximum login attempts ({MAX_LOGIN_ATTEMPTS}) exceeded.\nApplication will exit for security.", 
                            "Security Alert", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Application.Exit();
                        return;
                    }

                    MessageBox.Show($"Invalid username or password.\nAttempts remaining: {MAX_LOGIN_ATTEMPTS - loginAttempts}", 
                        "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    
                    // Clear password field
                    textBox2.Text = "";
                    textBox1.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Login error: {ex.Message}\n\nPlease check your database connection.", 
                    "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Re-enable login button
                btnOK.Enabled = true;
                btnOK.Text = "Login";
            }
        }

        private bool AuthenticateUser(string username, string password)
        {
            try
            {
                // Check if we can connect to database first
                using (var connection = DatabaseConnection.GetConnection())
                {
                    connection.Open();
                    connection.Close();
                }

                // Query to authenticate user and get role
                string query = @"SELECT UserID, Username, FullName, Role, IsActive, LastLoginDate 
                               FROM Users WHERE Username = @Username AND Password = @Password AND IsActive = 1";
                
                SqlParameter[] parameters = {
                    new SqlParameter("@Username", username),
                    new SqlParameter("@Password", password)
                };

                DataTable result = DatabaseConnection.ExecuteQuery(query, parameters);

                if (result != null && result.Rows.Count > 0)
                {
                    DataRow userRow = result.Rows[0];
                    
                    // Store user session information with null-safe assignments
                    UserSession.UserID = Convert.ToInt32(userRow["UserID"]);
                    UserSession.Username = userRow["Username"]?.ToString() ?? "Unknown";
                    UserSession.FullName = userRow["FullName"]?.ToString() ?? "Unknown User";
                    UserSession.Role = userRow["Role"]?.ToString() ?? "Salesman"; // Default to Salesman if null
                    UserSession.LoginTime = DateTime.Now;
                    
                    // Load user permissions based on role
                    UserSession.LoadUserPermissions();
                    
                    // Update last login date
                    UpdateLastLoginDate(UserSession.UserID);
                    
                    // Log login activity
                    UserSession.LogActivity("Login", "System", $"User {username} logged in with role {UserSession.Role}");

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw new Exception($"Authentication failed: {ex.Message}");
            }
        }

        private void ClearForm()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            loginAttempts = 0;
        }

        private void UpdateLastLoginDate(int userID)
        {
            try
            {
                string query = "UPDATE Users SET LastLoginDate = @LoginDate WHERE UserID = @UserID";
                SqlParameter[] parameters = {
                    new SqlParameter("@LoginDate", DateTime.Now),
                    new SqlParameter("@UserID", userID)
                };
                DatabaseConnection.ExecuteNonQuery(query, parameters);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating last login date: {ex.Message}");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to exit?", "Confirm Exit", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
    }

    // User session management class
    public static class UserSession
    {
        public static int UserID { get; set; } = 0;
        public static string Username { get; set; } = string.Empty;
        public static string FullName { get; set; } = string.Empty;
        public static string Role { get; set; } = string.Empty;
        public static DateTime LoginTime { get; set; } = DateTime.MinValue;
        private static Dictionary<string, UserPermission> _permissions = new Dictionary<string, UserPermission>();
        
        public static bool IsLoggedIn => UserID > 0;
        
        public static bool IsAdmin => Role == "Admin";
        public static bool IsPharmacist => Role == "Pharmacist";
        public static bool IsSalesman => Role == "Salesman";
        
        public static void LoadUserPermissions()
        {
            try
            {
                _permissions.Clear();
                string query = @"SELECT ModuleName, CanView, CanAdd, CanEdit, CanDelete, CanPrint 
                               FROM UserRolePermissions WHERE Role = @Role";
                
                SqlParameter[] parameters = { new SqlParameter("@Role", Role) };
                DataTable permissionsData = DatabaseConnection.ExecuteQuery(query, parameters);
                
                foreach (DataRow row in permissionsData.Rows)
                {
                    string moduleName = row["ModuleName"].ToString();
                    _permissions[moduleName] = new UserPermission
                    {
                        CanView = Convert.ToBoolean(row["CanView"]),
                        CanAdd = Convert.ToBoolean(row["CanAdd"]),
                        CanEdit = Convert.ToBoolean(row["CanEdit"]),
                        CanDelete = Convert.ToBoolean(row["CanDelete"]),
                        CanPrint = Convert.ToBoolean(row["CanPrint"])
                    };
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading permissions: {ex.Message}");
            }
        }
        
        public static bool HasPermission(string moduleName, string action)
        {
            if (IsAdmin) return true; // Admin has all permissions
            
            if (!_permissions.ContainsKey(moduleName))
                return false;
                
            var permission = _permissions[moduleName];
            
            switch (action.ToLower())
            {
                case "view": return permission.CanView;
                case "add": return permission.CanAdd;
                case "edit": return permission.CanEdit;
                case "delete": return permission.CanDelete;
                case "print": return permission.CanPrint;
                default: return false;
            }
        }
        
        public static void LogActivity(string activity, string moduleName, string description = "")
        {
            try
            {
                string query = @"INSERT INTO UserActivityLog (UserID, Activity, ModuleName, Description, LogDate) 
                               VALUES (@UserID, @Activity, @ModuleName, @Description, @LogDate)";
                
                SqlParameter[] parameters = {
                    new SqlParameter("@UserID", UserID),
                    new SqlParameter("@Activity", activity),
                    new SqlParameter("@ModuleName", moduleName),
                    new SqlParameter("@Description", description),
                    new SqlParameter("@LogDate", DateTime.Now)
                };
                
                DatabaseConnection.ExecuteNonQuery(query, parameters);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error logging activity: {ex.Message}");
            }
        }
        
        public static void Logout()
        {
            if (IsLoggedIn)
            {
                LogActivity("Logout", "System", $"User {Username} logged out");
            }
            
            UserID = 0;
            Username = "";
            FullName = "";
            Role = "";
            LoginTime = DateTime.MinValue;
            _permissions.Clear();
        }
    }
    
    // User permission class
    public class UserPermission
    {
        public bool CanView { get; set; }
        public bool CanAdd { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public bool CanPrint { get; set; }
    }
}
