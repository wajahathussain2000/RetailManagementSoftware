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
using RetailManagement.Models;

namespace RetailManagement.UserForms
{
    public partial class CustomerOutstanding : Form
    {
        private DataTable outstandingData;

        public CustomerOutstanding()
        {
            InitializeComponent();
        }

        private void CustomerOutstanding_Load(object sender, EventArgs e)
        {
            InitializeForm();
            LoadCustomerOutstanding();
        }

        private void InitializeForm()
        {
            // Load customers
            LoadCustomers();
            
            // Initialize status combo box
            InitializeStatusComboBox();
            
            // Initialize data table
            outstandingData = new DataTable();
        }

        private void LoadCustomers()
        {
            try
            {
                string query = "SELECT CustomerID, CustomerName FROM Customers WHERE IsActive = 1 ORDER BY CustomerName";
                DataTable customers = DatabaseConnection.ExecuteQuery(query);
                
                cmbCustomer.Items.Clear();
                cmbCustomer.Items.Add("All Customers");
                
                foreach (DataRow row in customers.Rows)
                {
                    cmbCustomer.Items.Add(new ComboBoxItem 
                    { 
                        Text = row["CustomerName"].ToString(), 
                        Value = Convert.ToInt32(row["CustomerID"]) 
                    });
                }
                
                cmbCustomer.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading customers: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeStatusComboBox()
        {
            cmbStatus.Items.Clear();
            cmbStatus.Items.Add("All Status");
            cmbStatus.Items.Add("Outstanding");
            cmbStatus.Items.Add("Advance");
            cmbStatus.Items.Add("Settled");
            cmbStatus.SelectedIndex = 0;
        }

        private void LoadCustomerOutstanding()
        {
            try
            {
                string query = @"SELECT 
                                    c.CustomerID,
                                    c.CustomerName,
                                    c.Phone,
                                    c.Email,
                                    ISNULL(TotalSales, 0) as TotalSales,
                                    ISNULL(TotalPayments, 0) as TotalPayments,
                                    ISNULL(TotalSales, 0) - ISNULL(TotalPayments, 0) as OutstandingAmount,
                                    CASE 
                                        WHEN ISNULL(TotalSales, 0) - ISNULL(TotalPayments, 0) > 0 THEN 'Outstanding'
                                        WHEN ISNULL(TotalSales, 0) - ISNULL(TotalPayments, 0) < 0 THEN 'Advance'
                                        ELSE 'Settled'
                                    END as Status
                               FROM Customers c
                               LEFT JOIN (
                                   SELECT CustomerID, SUM(TotalAmount) as TotalSales
                                   FROM Sales 
                                   WHERE IsActive = 1
                                   GROUP BY CustomerID
                               ) s ON c.CustomerID = s.CustomerID
                               LEFT JOIN (
                                   SELECT CustomerID, SUM(Amount) as TotalPayments
                                   FROM CustomerPayments 
                                   WHERE IsActive = 1
                                   GROUP BY CustomerID
                               ) cp ON c.CustomerID = cp.CustomerID
                               WHERE c.IsActive = 1";

                // Add customer filter
                if (cmbCustomer.SelectedIndex > 0)
                {
                    ComboBoxItem selectedItem = (ComboBoxItem)cmbCustomer.SelectedItem;
                    query += " AND c.CustomerID = @CustomerID";
                }

                // Add status filter
                if (cmbStatus.SelectedIndex > 0)
                {
                    string selectedStatus = cmbStatus.SelectedItem.ToString();
                    if (selectedStatus == "Outstanding")
                    {
                        query += " AND (ISNULL(TotalSales, 0) - ISNULL(TotalPayments, 0)) > 0";
                    }
                    else if (selectedStatus == "Advance")
                    {
                        query += " AND (ISNULL(TotalSales, 0) - ISNULL(TotalPayments, 0)) < 0";
                    }
                    else if (selectedStatus == "Settled")
                    {
                        query += " AND (ISNULL(TotalSales, 0) - ISNULL(TotalPayments, 0)) = 0";
                    }
                }

                query += " ORDER BY OutstandingAmount DESC";

                SqlParameter[] parameters = null;
                if (cmbCustomer.SelectedIndex > 0)
                {
                    ComboBoxItem selectedItem = (ComboBoxItem)cmbCustomer.SelectedItem;
                    parameters = new SqlParameter[]
                    {
                        new SqlParameter("@CustomerID", selectedItem.Value)
                    };
                }

                outstandingData = DatabaseConnection.ExecuteQuery(query, parameters);
                ShowOutstandingData();
                CalculateTotals();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading customer outstanding: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowOutstandingData()
        {
            SetupDataGridView();
            
            dgvCustomerOutstanding.Rows.Clear();

            if (outstandingData != null && outstandingData.Rows.Count > 0)
            {
                foreach (DataRow row in outstandingData.Rows)
                {
                    int rowIndex = dgvCustomerOutstanding.Rows.Add();
                    dgvCustomerOutstanding.Rows[rowIndex].Cells["CustomerID"].Value = row["CustomerID"];
                    dgvCustomerOutstanding.Rows[rowIndex].Cells["CustomerName"].Value = row["CustomerName"];
                    dgvCustomerOutstanding.Rows[rowIndex].Cells["Phone"].Value = row["Phone"];
                    dgvCustomerOutstanding.Rows[rowIndex].Cells["Email"].Value = row["Email"];
                    dgvCustomerOutstanding.Rows[rowIndex].Cells["TotalSales"].Value = Convert.ToDecimal(row["TotalSales"]).ToString("N2");
                    dgvCustomerOutstanding.Rows[rowIndex].Cells["TotalPayments"].Value = Convert.ToDecimal(row["TotalPayments"]).ToString("N2");
                    dgvCustomerOutstanding.Rows[rowIndex].Cells["OutstandingAmount"].Value = Convert.ToDecimal(row["OutstandingAmount"]).ToString("N2");
                    dgvCustomerOutstanding.Rows[rowIndex].Cells["Status"].Value = row["Status"];

                    // Color coding based on status
                    string status = row["Status"].ToString();
                    decimal outstandingAmount = Convert.ToDecimal(row["OutstandingAmount"]);
                    
                    if (status == "Outstanding")
                    {
                        dgvCustomerOutstanding.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightCoral;
                        dgvCustomerOutstanding.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.DarkRed;
                    }
                    else if (status == "Advance")
                    {
                        dgvCustomerOutstanding.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightGreen;
                        dgvCustomerOutstanding.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.DarkGreen;
                    }
                    else if (status == "Settled")
                    {
                        dgvCustomerOutstanding.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightBlue;
                        dgvCustomerOutstanding.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.DarkBlue;
                    }
                }
            }
        }

        private void SetupDataGridView()
        {
            dgvCustomerOutstanding.Columns.Clear();
            
            dgvCustomerOutstanding.Columns.Add("CustomerID", "Customer ID");
            dgvCustomerOutstanding.Columns.Add("CustomerName", "Customer Name");
            dgvCustomerOutstanding.Columns.Add("Phone", "Phone");
            dgvCustomerOutstanding.Columns.Add("Email", "Email");
            dgvCustomerOutstanding.Columns.Add("TotalSales", "Total Sales");
            dgvCustomerOutstanding.Columns.Add("TotalPayments", "Total Payments");
            dgvCustomerOutstanding.Columns.Add("OutstandingAmount", "Outstanding Amount");
            dgvCustomerOutstanding.Columns.Add("Status", "Status");

            // Configure columns
            dgvCustomerOutstanding.Columns["CustomerID"].Width = 80;
            dgvCustomerOutstanding.Columns["CustomerName"].Width = 150;
            dgvCustomerOutstanding.Columns["Phone"].Width = 120;
            dgvCustomerOutstanding.Columns["Email"].Width = 150;
            dgvCustomerOutstanding.Columns["TotalSales"].Width = 120;
            dgvCustomerOutstanding.Columns["TotalPayments"].Width = 120;
            dgvCustomerOutstanding.Columns["OutstandingAmount"].Width = 140;
            dgvCustomerOutstanding.Columns["Status"].Width = 100;

            // Set alignment
            dgvCustomerOutstanding.Columns["CustomerID"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvCustomerOutstanding.Columns["TotalSales"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvCustomerOutstanding.Columns["TotalPayments"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvCustomerOutstanding.Columns["OutstandingAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvCustomerOutstanding.Columns["Status"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Set header alignment
            dgvCustomerOutstanding.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvCustomerOutstanding.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
        }

        private void CalculateTotals()
        {
            decimal totalOutstanding = 0;
            int outstandingCustomers = 0;
            int advanceCustomers = 0;
            int settledCustomers = 0;

            if (outstandingData != null && outstandingData.Rows.Count > 0)
            {
                foreach (DataRow row in outstandingData.Rows)
                {
                    totalOutstanding += Convert.ToDecimal(row["OutstandingAmount"]);
                    
                    string status = row["Status"].ToString();
                    if (status == "Outstanding")
                        outstandingCustomers++;
                    else if (status == "Advance")
                        advanceCustomers++;
                    else if (status == "Settled")
                        settledCustomers++;
                }
            }

            lblTotalOutstanding.Text = "Total Outstanding: " + totalOutstanding.ToString("N2");
            lblTotalAdvance.Text = "Total Advance: " + advanceCustomers.ToString();
            lblTotalSettled.Text = "Total Settled: " + settledCustomers.ToString();
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            LoadCustomerOutstanding();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadCustomerOutstanding();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (outstandingData.Rows.Count == 0)
            {
                MessageBox.Show("No data to print.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            PrintDocument pd = new PrintDocument();
            pd.PrintPage += new PrintPageEventHandler(PrintCustomerOutstanding);
            
            PrintDialog printDialog = new PrintDialog();
            printDialog.Document = pd;
            
            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                pd.Print();
            }
        }

        private void PrintCustomerOutstanding(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Font titleFont = new Font("Arial", 16, FontStyle.Bold);
            Font headerFont = new Font("Arial", 10, FontStyle.Bold);
            Font dataFont = new Font("Arial", 9);
            Font summaryFont = new Font("Arial", 10, FontStyle.Bold);

            int yPos = 50;
            int leftMargin = 50;

            // Print title
            string title = "Customer Outstanding Report";
            if (cmbCustomer.SelectedIndex > 0)
            {
                ComboBoxItem selectedItem = (ComboBoxItem)cmbCustomer.SelectedItem;
                title += $" - {selectedItem.Text}";
            }
            g.DrawString(title, titleFont, Brushes.Black, leftMargin, yPos);
            yPos += 30;

            // Print date
            g.DrawString($"Generated on: {DateTime.Now:dd/MM/yyyy HH:mm}", dataFont, Brushes.Black, leftMargin, yPos);
            yPos += 30;

            // Print headers
            string[] headers = { "Customer", "Phone", "Sales", "Payments", "Outstanding", "Status" };
            int[] columnWidths = { 150, 120, 100, 100, 120, 80 };
            int xPos = leftMargin;

            for (int i = 0; i < headers.Length; i++)
            {
                g.DrawString(headers[i], headerFont, Brushes.Black, xPos, yPos);
                xPos += columnWidths[i];
            }
            yPos += 20;

            // Print data
            foreach (DataRow row in outstandingData.Rows)
            {
                if (yPos > e.MarginBounds.Bottom - 100)
                {
                    e.HasMorePages = true;
                    return;
                }

                xPos = leftMargin;
                
                // Customer Name
                g.DrawString(row["CustomerName"].ToString(), dataFont, Brushes.Black, xPos, yPos);
                xPos += columnWidths[0];

                // Phone
                g.DrawString(row["Phone"].ToString(), dataFont, Brushes.Black, xPos, yPos);
                xPos += columnWidths[1];

                // Sales
                g.DrawString(Convert.ToDecimal(row["TotalSales"]).ToString("N2"), dataFont, Brushes.Black, xPos, yPos);
                xPos += columnWidths[2];

                // Payments
                g.DrawString(Convert.ToDecimal(row["TotalPayments"]).ToString("N2"), dataFont, Brushes.Black, xPos, yPos);
                xPos += columnWidths[3];

                // Outstanding
                g.DrawString(Convert.ToDecimal(row["OutstandingAmount"]).ToString("N2"), dataFont, Brushes.Black, xPos, yPos);
                xPos += columnWidths[4];

                // Status
                g.DrawString(row["Status"].ToString(), dataFont, Brushes.Black, xPos, yPos);

                yPos += 15;
            }

            // Print summary
            yPos += 20;
            g.DrawString($"Total Customers: {outstandingData.Rows.Count}", summaryFont, Brushes.Black, leftMargin, yPos);
            yPos += 15;
            
            decimal totalOutstanding = 0;
            foreach (DataRow row in outstandingData.Rows)
            {
                totalOutstanding += Convert.ToDecimal(row["OutstandingAmount"]);
            }
            g.DrawString($"Total Outstanding: {totalOutstanding:N2}", summaryFont, Brushes.Black, leftMargin, yPos);
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (outstandingData.Rows.Count == 0)
            {
                MessageBox.Show("No data to export.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "CSV files (*.csv)|*.csv";
            saveDialog.FileName = $"CustomerOutstanding_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                ExportToCSV(saveDialog.FileName);
            }
        }

        private void ExportToCSV(string fileName)
        {
            try
            {
                StringBuilder csv = new StringBuilder();
                
                // Add headers
                csv.AppendLine("Customer Name,Phone,Email,Total Sales,Total Payments,Outstanding Amount,Status");
                
                // Add data
                foreach (DataRow row in outstandingData.Rows)
                {
                    string customerName = row["CustomerName"].ToString().Replace(",", ";");
                    string phone = row["Phone"].ToString().Replace(",", ";");
                    string email = row["Email"].ToString().Replace(",", ";");
                    string totalSales = Convert.ToDecimal(row["TotalSales"]).ToString("N2");
                    string totalPayments = Convert.ToDecimal(row["TotalPayments"]).ToString("N2");
                    string outstandingAmount = Convert.ToDecimal(row["OutstandingAmount"]).ToString("N2");
                    string status = row["Status"].ToString();

                    csv.AppendLine($"{customerName},{phone},{email},{totalSales},{totalPayments},{outstandingAmount},{status}");
                }

                System.IO.File.WriteAllText(fileName, csv.ToString());
                MessageBox.Show("Data exported successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exporting data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmbCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadCustomerOutstanding();
        }

        private void cmbStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadCustomerOutstanding();
        }

        // Using shared ComboBoxItem from Models namespace
    }
} 