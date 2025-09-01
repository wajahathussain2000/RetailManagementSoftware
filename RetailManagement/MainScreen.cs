using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RetailManagement
{
    public partial class MainScreen : Form
    {
        private Login currentLoginControl;
        private UserDashboard currentDashboardControl;
        private bool isDisposing = false;

        public MainScreen()
        {
            InitializeComponent();
            LoadLoginControl();
            
            // Add form closing event to cleanup
            this.FormClosing += MainScreen_FormClosing;
        }

        private void MainScreen_FormClosing(object sender, FormClosingEventArgs e)
        {
            CleanupControls();
        }

        internal void CleanupControls()
        {
            if (!isDisposing)
            {
                isDisposing = true;
                
                try
                {
                    // Clean up login control
                    if (currentLoginControl != null)
                    {
                        currentLoginControl.LoginSuccessful -= LoadUserDashboardControl;
                        currentLoginControl.Dispose();
                        currentLoginControl = null;
                    }
                    
                    // Clean up dashboard control
                    if (currentDashboardControl != null)
                    {
                        currentDashboardControl.CancelClicked -= LoadLoginControl;
                        currentDashboardControl.Dispose();
                        currentDashboardControl = null;
                    }
                }
                catch
                {
                    // Ignore cleanup errors
                }
            }
        }

        private void LoadLoginControl()
        {
            if (isDisposing) return;
            
            try
            {
                // Clean up existing dashboard control
                if (currentDashboardControl != null)
                {
                    currentDashboardControl.CancelClicked -= LoadLoginControl;
                    currentDashboardControl.Dispose();
                    currentDashboardControl = null;
                }
                
                panel1.Controls.Clear();
                currentLoginControl = new Login();
                currentLoginControl.LoginSuccessful += LoadUserDashboardControl;
                currentLoginControl.Dock = DockStyle.Fill;
                panel1.Controls.Add(currentLoginControl);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading login: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadUserDashboardControl()
        {
            if (isDisposing) return;
            
            try
            {
                // Clean up existing login control
                if (currentLoginControl != null)
                {
                    currentLoginControl.LoginSuccessful -= LoadUserDashboardControl;
                    currentLoginControl.Dispose();
                    currentLoginControl = null;
                }
                
                panel1.Controls.Clear();
                currentDashboardControl = new UserDashboard();
                currentDashboardControl.CancelClicked += LoadLoginControl;
                currentDashboardControl.Dock = DockStyle.Fill;
                panel1.Controls.Add(currentDashboardControl);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading dashboard: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


    }
}
