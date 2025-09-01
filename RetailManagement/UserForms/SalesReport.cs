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
using RetailManagement.Models;
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
            try
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

                // Try simple RDLC first (most compatible), then others
                string simplePath = System.IO.Path.Combine(Application.StartupPath, "Reports", "SalesReport_Simple.rdlc");
                string compatiblePath = System.IO.Path.Combine(Application.StartupPath, "Reports", "SalesReport_Compatible.rdlc");
                string originalPath = System.IO.Path.Combine(Application.StartupPath, "Reports", "SalesReport.rdlc");
                
                if (System.IO.File.Exists(simplePath))
                {
                    reportViewer.LocalReport.ReportPath = simplePath;
                }
                else if (System.IO.File.Exists(compatiblePath))
                {
                    reportViewer.LocalReport.ReportPath = compatiblePath;
                }
                else if (System.IO.File.Exists(originalPath))
                {
                    reportViewer.LocalReport.ReportPath = originalPath;
                }
                else
                {
                    // Fallback paths
                    if (System.IO.File.Exists("Reports/SalesReport_Simple.rdlc"))
                        reportViewer.LocalReport.ReportPath = "Reports/SalesReport_Simple.rdlc";
                    else if (System.IO.File.Exists("Reports/SalesReport_Compatible.rdlc"))
                        reportViewer.LocalReport.ReportPath = "Reports/SalesReport_Compatible.rdlc";
                    else
                        reportViewer.LocalReport.ReportPath = "Reports/SalesReport.rdlc";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"RDLC initialization failed, will use table view: {ex.Message}", 
                    "Report Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                reportViewer = null; // Will trigger DataGridView fallback
            }

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
                cmbCustomer.Items.Add(new ComboBoxItem { Value = 0, Text = "All Customers" });
                
                foreach (DataRow row in customerData.Rows)
                {
                    cmbCustomer.Items.Add(new ComboBoxItem 
                    { 
                        Value = Convert.ToInt32(row["CustomerID"]), 
                        Text = row["CustomerName"].ToString() 
                    });
                }
                cmbCustomer.SelectedIndex = 0;

                // Load payment methods from actual seeded data
                cmbPaymentMethod.Items.Clear();
                cmbPaymentMethod.Items.Add("All Methods");
                cmbPaymentMethod.Items.Add("Cash");
                cmbPaymentMethod.Items.Add("Card");
                cmbPaymentMethod.Items.Add("UPI");
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
            // Set to August 2025 to match our seeded test data
            dateTimePicker1.Value = new DateTime(2025, 8, 1);
            dateTimePicker2.Value = new DateTime(2025, 8, 31);
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
                if (cmbCustomer.SelectedItem is ComboBoxItem selectedCustomer && selectedCustomer.Value != null && (int)selectedCustomer.Value > 0)
                {
                    query += " AND s.CustomerID = @CustomerID";
                    parameters.Add(new SqlParameter("@CustomerID", selectedCustomer.Value));
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
                // Check if we have a report viewer and if the RDLC file exists
                string reportPath = System.IO.Path.Combine(Application.StartupPath, "Reports", "SalesReport.rdlc");
                bool useRDLC = reportViewer != null && (System.IO.File.Exists(reportPath) || System.IO.File.Exists("Reports/SalesReport.rdlc"));
                
                if (useRDLC)
                {
                    // Try RDLC approach
                    reportViewer.LocalReport.DataSources.Clear();
                    ReportDataSource dataSource = new ReportDataSource("DataSet1", reportData);
                    reportViewer.LocalReport.DataSources.Add(dataSource);
                    reportViewer.RefreshReport();
                }
                else
                {
                    // Fallback to DataGridView
                    ShowDataInGrid();
                }
            }
            catch (Exception ex)
            {
                // On any RDLC error, fallback to DataGridView
                try
                {
                    ShowDataInGrid();
                    MessageBox.Show($"Report loaded in table format due to RDLC issue.\nTotal records: {reportData.Rows.Count}", 
                        "Sales Report", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception fallbackEx)
                {
                    MessageBox.Show($"Error loading report: {ex.Message}\nFallback error: {fallbackEx.Message}", 
                        "Report Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        
        private void ShowDataInGrid()
        {
            // Create DataGridView if it doesn't exist or if we need to replace ReportViewer
            var dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                DataSource = reportData,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.LightGray }
            };
            
            // Clear and add to groupBox2
            groupBox2.Controls.Clear();
            groupBox2.Controls.Add(dgv);
            groupBox2.Text = "Sales Report (Table View)";
            
            // Format currency columns
            if (dgv.Columns["TotalAmount"] != null)
                dgv.Columns["TotalAmount"].DefaultCellStyle.Format = "C2";
            if (dgv.Columns["Discount"] != null)
                dgv.Columns["Discount"].DefaultCellStyle.Format = "C2";
            if (dgv.Columns["NetAmount"] != null)
                dgv.Columns["NetAmount"].DefaultCellStyle.Format = "C2";
                
            // Make columns more readable
            foreach (DataGridViewColumn column in dgv.Columns)
            {
                if (column.Name == "SaleDate")
                {
                    column.HeaderText = "Sale Date";
                    column.DefaultCellStyle.Format = "MMM dd, yyyy";
                }
                else if (column.Name == "BillNumber")
                    column.HeaderText = "Bill Number";
                else if (column.Name == "CustomerName")
                    column.HeaderText = "Customer";
                else if (column.Name == "PaymentMethod")
                    column.HeaderText = "Payment Method";
                else if (column.Name == "TotalAmount")
                    column.HeaderText = "Total Amount";
                else if (column.Name == "NetAmount")
                    column.HeaderText = "Net Amount";
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

        // Using shared ComboBoxItem from Models namespace
    }
} 