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
using Microsoft.Reporting.WinForms;

namespace RetailManagement.UserForms
{
    public partial class SalesReport : Form
    {
        private DataTable reportData;
        private ReportViewer reportViewer;
        private ComboBox cmbCustomer;
        private ComboBox cmbPaymentMethod;

        public SalesReport()
        {
            InitializeComponent();
            InitializeRDLCControls();
            LoadDropdownData();
            SetDefaultDateRange();
        }

        private void InitializeRDLCControls()
        {
            // Remove the old DataGridView and replace with ReportViewer
            if (dataGridView1 != null)
            {
                dataGridView1.Dispose();
                groupBox2.Controls.Remove(dataGridView1);
            }

            // Create ReportViewer
            reportViewer = new ReportViewer
            {
                Dock = DockStyle.Fill,
                ProcessingMode = ProcessingMode.Local
            };

            // Add ReportViewer to the groupBox2
            groupBox2.Controls.Add(reportViewer);
            groupBox2.Text = "Sales Report";

            // Set report path
            reportViewer.LocalReport.ReportPath = "Reports/SalesReport.rdlc";

            // Add customer and payment method dropdowns to groupBox1
            AddFilterControls();
        }

        private void AddFilterControls()
        {
            // Create customer label and dropdown
            Label lblCustomer = new Label
            {
                Text = "Customer:",
                Location = new Point(20, 60),
                Size = new Size(80, 20),
                AutoSize = true
            };

            cmbCustomer = new ComboBox
            {
                Location = new Point(110, 60),
                Size = new Size(150, 20),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Create payment method label and dropdown
            Label lblPaymentMethod = new Label
            {
                Text = "Payment Method:",
                Location = new Point(280, 60),
                Size = new Size(100, 20),
                AutoSize = true
            };

            cmbPaymentMethod = new ComboBox
            {
                Location = new Point(390, 60),
                Size = new Size(150, 20),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Add controls to groupBox1
            groupBox1.Controls.Add(lblCustomer);
            groupBox1.Controls.Add(cmbCustomer);
            groupBox1.Controls.Add(lblPaymentMethod);
            groupBox1.Controls.Add(cmbPaymentMethod);

            // Adjust groupBox1 height to accommodate new controls
            groupBox1.Height = 120;
        }

        private void LoadDropdownData()
        {
            try
            {
                // Load customers
                string customerQuery = "SELECT CustomerID, CustomerName FROM Customers WHERE IsActive = 1 ORDER BY CustomerName";
                DataTable customerData = DatabaseConnection.ExecuteQuery(customerQuery);
                
                cmbCustomer.Items.Clear();
                cmbCustomer.Items.Add(new ComboBoxItem { ID = 0, Text = "All Customers" });
                
                foreach (DataRow row in customerData.Rows)
                {
                    cmbCustomer.Items.Add(new ComboBoxItem 
                    { 
                        ID = Convert.ToInt32(row["CustomerID"]), 
                        Text = row["CustomerName"].ToString() 
                    });
                }
                cmbCustomer.SelectedIndex = 0;

                // Load payment methods
                cmbPaymentMethod.Items.Clear();
                cmbPaymentMethod.Items.Add("All Methods");
                cmbPaymentMethod.Items.Add("Cash");
                cmbPaymentMethod.Items.Add("Credit Card");
                cmbPaymentMethod.Items.Add("Debit Card");
                cmbPaymentMethod.Items.Add("Bank Transfer");
                cmbPaymentMethod.Items.Add("Check");
                cmbPaymentMethod.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading dropdown data: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetDefaultDateRange()
        {
            dateTimePicker1.Value = DateTime.Now.AddDays(-30);
            dateTimePicker2.Value = DateTime.Now;
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            if (ValidateDateRange())
            {
                GenerateSalesReport();
            }
        }

        private bool ValidateDateRange()
        {
            if (dateTimePicker1.Value > dateTimePicker2.Value)
            {
                MessageBox.Show("From date cannot be greater than To date.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private void GenerateSalesReport()
        {
            try
            {
                string query = @"SELECT 
                                s.SaleID,
                                s.BillNumber,
                                c.CustomerName,
                                s.SaleDate,
                                s.TotalAmount,
                                s.Discount,
                                s.NetAmount,
                                s.PaymentMethod
                               FROM Sales s
                               INNER JOIN Customers c ON s.CustomerID = c.CustomerID
                               WHERE s.IsActive = 1 
                               AND s.SaleDate BETWEEN @FromDate AND @ToDate";

                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    new SqlParameter("@FromDate", dateTimePicker1.Value.Date),
                    new SqlParameter("@ToDate", dateTimePicker2.Value.Date.AddDays(1).AddSeconds(-1))
                };

                // Add customer filter
                if (cmbCustomer.SelectedItem is ComboBoxItem selectedCustomer && selectedCustomer.ID > 0)
                {
                    query += " AND s.CustomerID = @CustomerID";
                    parameters.Add(new SqlParameter("@CustomerID", selectedCustomer.ID));
                }

                // Add payment method filter
                if (cmbPaymentMethod.SelectedIndex > 0)
                {
                    query += " AND s.PaymentMethod = @PaymentMethod";
                    parameters.Add(new SqlParameter("@PaymentMethod", cmbPaymentMethod.Text));
                }

                query += " ORDER BY s.SaleDate DESC";

                reportData = DatabaseConnection.ExecuteQuery(query, parameters.ToArray());
                
                if (reportData.Rows.Count > 0)
                {
                    LoadReportData();
                    DisplaySummary();
                }
                else
                {
                    MessageBox.Show("No data found for the selected criteria.", "Information", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating report: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadReportData()
        {
            try
            {
                reportViewer.LocalReport.DataSources.Clear();
                ReportDataSource dataSource = new ReportDataSource("DataSet1", reportData);
                reportViewer.LocalReport.DataSources.Add(dataSource);
                reportViewer.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading report data: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisplaySummary()
        {
            decimal totalSales = 0;
            decimal totalDiscount = 0;
            decimal totalNetAmount = 0;
            int totalTransactions = reportData.Rows.Count;

            foreach (DataRow row in reportData.Rows)
            {
                totalSales += Convert.ToDecimal(row["TotalAmount"]);
                totalDiscount += Convert.ToDecimal(row["Discount"]);
                totalNetAmount += Convert.ToDecimal(row["NetAmount"]);
            }

            string summary = $"Report Summary:\n" +
                           $"Total Transactions: {totalTransactions}\n" +
                           $"Total Sales: ${totalSales:N2}\n" +
                           $"Total Discount: ${totalDiscount:N2}\n" +
                           $"Net Amount: ${totalNetAmount:N2}";

            MessageBox.Show(summary, "Sales Report Summary", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                if (reportData != null && reportData.Rows.Count > 0)
                {
                    SaveFileDialog saveDialog = new SaveFileDialog
                    {
                        Filter = "PDF files (*.pdf)|*.pdf",
                        FileName = $"SalesReport_{DateTime.Now:yyyyMMdd}.pdf"
                    };

                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        ExportToPDF(saveDialog.FileName);
                    }
                }
                else
                {
                    MessageBox.Show("No data to export.", "Information", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exporting: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportToPDF(string fileName)
        {
            try
            {
                Warning[] warnings;
                string[] streamIds;
                string mimeType = string.Empty;
                string encoding = string.Empty;
                string extension = string.Empty;

                byte[] bytes = reportViewer.LocalReport.Render("PDF", null, out mimeType, 
                    out encoding, out extension, out streamIds, out warnings);

                System.IO.File.WriteAllBytes(fileName, bytes);
                MessageBox.Show("Report exported successfully!", "Success", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exporting report: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (reportData != null && reportData.Rows.Count > 0)
                {
                    reportViewer.PrintDialog();
                }
                else
                {
                    MessageBox.Show("No data to print.", "Information", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error printing: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void pbExit_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            // Optional: Add any date change logic here
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            // Optional: Add any date change logic here
        }

        // Helper class for ComboBox items
        private class ComboBoxItem
        {
            public int ID { get; set; }
            public string Text { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }
    }
} 