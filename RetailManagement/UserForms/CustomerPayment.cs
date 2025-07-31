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
    public partial class CustomerPayment : Form
    {
        private int selectedCustomerID = 0;
        private decimal customerBalance = 0;

        public CustomerPayment()
        {
            InitializeComponent();
            LoadCustomers();
            SetupDataGridView();
            SetDefaultDate();
        }

        private void SetupDataGridView()
        {
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.Columns.Clear();

            // Using the actual columns defined in Designer
            dataGridView1.Columns.Add("BillNo", "Bill No");
            dataGridView1.Columns.Add("Description", "Description");
            dataGridView1.Columns.Add("Bank", "Bank");
            dataGridView1.Columns.Add("Payment", "Payment");
            dataGridView1.Columns.Add("Discount", "Discount");

            dataGridView1.Columns["BillNo"].DataPropertyName = "BillNo";
            dataGridView1.Columns["Description"].DataPropertyName = "Description";
            dataGridView1.Columns["Bank"].DataPropertyName = "Bank";
            dataGridView1.Columns["Payment"].DataPropertyName = "Payment";
            dataGridView1.Columns["Discount"].DataPropertyName = "Discount";
        }

        private void LoadCustomers()
        {
            try
            {
                string query = "SELECT CustomerID, CustomerName, Phone FROM Customers WHERE IsActive = 1 ORDER BY CustomerName";
                DataTable dt = DatabaseConnection.ExecuteQuery(query);
                comboBox1.DataSource = dt;
                comboBox1.DisplayMember = "CustomerName";
                comboBox1.ValueMember = "CustomerID";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading customers: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadPayments()
        {
            try
            {
                if (selectedCustomerID == 0)
                {
                    dataGridView1.DataSource = null;
                    return;
                }

                string query = @"SELECT 
                                s.BillNumber as BillNo,
                                'Sale - ' + s.BillNumber as Description,
                                'Cash' as Bank,
                                cp.Amount as Payment,
                                s.Discount as Discount
                               FROM Sales s
                               LEFT JOIN CustomerPayments cp ON s.SaleID = cp.SaleID
                               WHERE s.CustomerID = @CustomerID AND s.IsActive = 1
                               ORDER BY s.SaleDate DESC";

                SqlParameter[] parameters = { new SqlParameter("@CustomerID", selectedCustomerID) };
                DataTable dt = DatabaseConnection.ExecuteQuery(query, parameters);
                dataGridView1.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading payments: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedValue != null)
            {
                selectedCustomerID = Convert.ToInt32(comboBox1.SelectedValue);
                LoadCustomerBalance();
            }
        }

        private void LoadCustomerBalance()
        {
            try
            {
                string query = @"SELECT 
                                ISNULL(SUM(s.NetAmount), 0) - ISNULL(SUM(cp.Amount), 0) as Balance
                               FROM Customers c
                               LEFT JOIN Sales s ON c.CustomerID = s.CustomerID AND s.IsActive = 1
                               LEFT JOIN CustomerPayments cp ON c.CustomerID = cp.CustomerID
                               WHERE c.CustomerID = @CustomerID
                               GROUP BY c.CustomerID";

                SqlParameter[] parameters = { new SqlParameter("@CustomerID", selectedCustomerID) };
                object result = DatabaseConnection.ExecuteScalar(query, parameters);
                customerBalance = Convert.ToDecimal(result);
                textBox1.Text = customerBalance.ToString("N2");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading customer balance: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetDefaultDate()
        {
            dateTimePicker1.Value = DateTime.Now;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedValue != null)
            {
                LoadCustomerBalance();
                LoadPayments();
            }
            else
            {
                MessageBox.Show("Please select a customer first.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // This form is for display only, not for saving payments
            MessageBox.Show("This form is for viewing customer payment history only.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
