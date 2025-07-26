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
        public MainScreen()
        {
            InitializeComponent();
            LoadLoginControl();
        }

        private void LoadLoginControl()
        {
            panel1.Controls.Clear();
            Login loginControl = new Login();
            loginControl.LoginSuccessful += LoadUserDashboardControl; // subscribe to event
            loginControl.Dock = DockStyle.Fill;
            panel1.Controls.Add(loginControl);
        }
        private void LoadUserDashboardControl()
        {
            panel1.Controls.Clear();
            UserDashboard dashboardControl = new UserDashboard();
            dashboardControl.CancelClicked += LoadLoginControl; // subscribe to cancel event
            dashboardControl.Dock = DockStyle.Fill;
            panel1.Controls.Add(dashboardControl);
        }


    }
}
