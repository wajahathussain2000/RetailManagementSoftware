using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RetailManagement.Database;

namespace RetailManagement.UserForms
{
    public partial class CustomerLedger : Form
    {
        private DataTable ledgerData;

        public CustomerLedger()
        {
            InitializeComponent();
            LoadCustomers();
            SetDefaultDateRange();
        }

        private void SetDefaultDateRange()
        {
            // Set default date range to current month
            txtFrom.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            txtTo.Value = DateTime.Now;
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

        private void GenerateCustomerLedger()
        {
            try
            {
                if (comboBox1.SelectedValue == null)
                {
                    MessageBox.Show("Please select a customer.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int customerID = Convert.ToInt32(comboBox1.SelectedValue);
                DateTime fromDate = txtFrom.Value.Date;
                DateTime toDate = txtTo.Value.Date;

                string query = @"SELECT 
                                Date,
                                Description,
                                Debit,
                                Credit,
                                Balance
                               FROM (
                                   SELECT 
                                       s.SaleDate as Date,
                                       'Sale - ' + s.BillNumber as Description,
                                       s.NetAmount as Debit,
                                       0 as Credit,
                                       0 as Balance
                                   FROM Sales s
                                   WHERE s.CustomerID = @CustomerID AND s.IsActive = 1
                                   AND s.SaleDate BETWEEN @FromDate AND @ToDate
                                   
                                   UNION ALL
                                   
                                   SELECT 
                                       cp.PaymentDate as Date,
                                       'Payment - ' + cp.PaymentMethod as Description,
                                       0 as Debit,
                                       cp.Amount as Credit,
                                       0 as Balance
                                   FROM CustomerPayments cp
                                   WHERE cp.CustomerID = @CustomerID
                                   AND cp.PaymentDate BETWEEN @FromDate AND @ToDate
                               ) as Ledger
                               ORDER BY Date";

                SqlParameter[] parameters = { 
                    new SqlParameter("@CustomerID", customerID),
                    new SqlParameter("@FromDate", fromDate),
                    new SqlParameter("@ToDate", toDate)
                };
                
                ledgerData = DatabaseConnection.ExecuteQuery(query, parameters);

                // Calculate running balance
                CalculateRunningBalance(ledgerData);

                // Show report
                ShowCustomerLedgerReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating customer ledger: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CalculateRunningBalance(DataTable dt)
        {
            decimal runningBalance = 0;
            foreach (DataRow row in dt.Rows)
            {
                decimal debit = Convert.ToDecimal(row["Debit"]);
                decimal credit = Convert.ToDecimal(row["Credit"]);
                runningBalance += debit - credit;
                row["Balance"] = runningBalance;
            }
        }

        private void ShowCustomerLedgerReport()
        {
            if (ledgerData == null || ledgerData.Rows.Count == 0)
            {
                MessageBox.Show("No data found for the selected customer and date range.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Create a simple report display form
            Form reportForm = new Form();
            reportForm.Text = "Customer Ledger Report";
            reportForm.Size = new Size(900, 600);
            reportForm.StartPosition = FormStartPosition.CenterScreen;

            DataGridView dgv = new DataGridView();
            dgv.Dock = DockStyle.Fill;
            dgv.AutoGenerateColumns = false;
            dgv.Columns.Add("Date", "Date");
            dgv.Columns.Add("Description", "Description");
            dgv.Columns.Add("Debit", "Debit");
            dgv.Columns.Add("Credit", "Credit");
            dgv.Columns.Add("Balance", "Balance");

            dgv.Columns["Date"].DataPropertyName = "Date";
            dgv.Columns["Description"].DataPropertyName = "Description";
            dgv.Columns["Debit"].DataPropertyName = "Debit";
            dgv.Columns["Credit"].DataPropertyName = "Credit";
            dgv.Columns["Balance"].DataPropertyName = "Balance";

            dgv.DataSource = ledgerData;

            reportForm.Controls.Add(dgv);
            reportForm.Show();
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            GenerateCustomerLedger();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadCustomers();
            SetDefaultDateRange();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (ledgerData != null && ledgerData.Rows.Count > 0)
                {
                    PrintDocument pd = new PrintDocument();
                    pd.PrintPage += PrintCustomerLedger;
                    pd.Print();
                }
                else
                {
                    MessageBox.Show("No data to print. Please generate a report first.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error printing: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PrintCustomerLedger(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            if (ledgerData == null || ledgerData.Rows.Count == 0)
                return;

            Graphics g = e.Graphics;
            Font titleFont = new Font("Arial", 16, FontStyle.Bold);
            Font headerFont = new Font("Arial", 10, FontStyle.Bold);
            Font dataFont = new Font("Arial", 9);

            int yPos = 50;
            g.DrawString("Customer Ledger", titleFont, Brushes.Black, 50, yPos);
            yPos += 30;
            g.DrawString("Customer: " + comboBox1.Text, dataFont, Brushes.Black, 50, yPos);
            yPos += 15;
            g.DrawString("Period: " + txtFrom.Value.ToString("dd/MM/yyyy") + " to " + txtTo.Value.ToString("dd/MM/yyyy"), dataFont, Brushes.Black, 50, yPos);
            yPos += 15;
            g.DrawString("Generated on: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"), dataFont, Brushes.Black, 50, yPos);
            yPos += 30;

            // Print headers
            g.DrawString("Date", headerFont, Brushes.Black, 50, yPos);
            g.DrawString("Description", headerFont, Brushes.Black, 150, yPos);
            g.DrawString("Debit", headerFont, Brushes.Black, 300, yPos);
            g.DrawString("Credit", headerFont, Brushes.Black, 400, yPos);
            g.DrawString("Balance", headerFont, Brushes.Black, 500, yPos);
            yPos += 20;

            // Print data
            foreach (DataRow row in ledgerData.Rows)
            {
                if (yPos > e.PageBounds.Height - 100) break;

                g.DrawString(Convert.ToDateTime(row["Date"]).ToString("dd/MM/yyyy"), dataFont, Brushes.Black, 50, yPos);
                g.DrawString(row["Description"].ToString(), dataFont, Brushes.Black, 150, yPos);
                g.DrawString(Convert.ToDecimal(row["Debit"]).ToString("N2"), dataFont, Brushes.Black, 300, yPos);
                g.DrawString(Convert.ToDecimal(row["Credit"]).ToString("N2"), dataFont, Brushes.Black, 400, yPos);
                g.DrawString(Convert.ToDecimal(row["Balance"]).ToString("N2"), dataFont, Brushes.Black, 500, yPos);
                yPos += 15;
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
