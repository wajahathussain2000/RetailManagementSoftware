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
    public partial class StockInHand : Form
    {
        private DataTable reportData;
        private ReportViewer reportViewer;

        public StockInHand()
        {
            InitializeComponent();
            InitializeRDLCControls();
            LoadCategories();
            SetDefaultReportType();
        }

        private void InitializeRDLCControls()
        {
            // Remove old controls and replace with ReportViewer
            this.Controls.Clear();
            
            // Create main panel
            Panel mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            // Create control panel
            Panel controlPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 200,
                BackColor = Color.LightGray
            };

            // Create labels
            Label lblCategory = new Label
            {
                Text = "Category:",
                Location = new Point(20, 20),
                Size = new Size(80, 20),
                Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold)
            };

            Label lblReportType = new Label
            {
                Text = "Report Type:",
                Location = new Point(20, 50),
                Size = new Size(80, 20),
                Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold)
            };

            Label lblSearch = new Label
            {
                Text = "Search:",
                Location = new Point(20, 80),
                Size = new Size(80, 20),
                Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold)
            };

            // Create dropdowns
            comboBox1 = new ComboBox
            {
                Location = new Point(110, 20),
                Size = new Size(200, 20),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            comboBox2 = new ComboBox
            {
                Location = new Point(110, 50),
                Size = new Size(200, 20),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            comboBox2.Items.AddRange(new object[] { "All Items", "Low Stock (≤10)", "Out of Stock", "By Value (High to Low)" });

            // Create search textbox
            textBox1 = new TextBox
            {
                Location = new Point(110, 80),
                Size = new Size(200, 20)
            };

            // Create radio buttons for stock status
            radioButton1 = new RadioButton
            {
                Text = "All Items",
                Location = new Point(350, 20),
                Size = new Size(100, 20),
                Checked = true,
                Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold)
            };

            radioButton2 = new RadioButton
            {
                Text = "Low Stock (≤10)",
                Location = new Point(350, 40),
                Size = new Size(120, 20),
                Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold)
            };

            radioButton3 = new RadioButton
            {
                Text = "Out of Stock",
                Location = new Point(350, 60),
                Size = new Size(100, 20),
                Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold)
            };

            // Update button texts and add event handlers
            btnReport.Text = "Generate Report";
            btnReport.Size = new Size(120, 32);
            btnReport.Location = new Point(500, 20);
            btnReport.Click += btnReport_Click;

            btnExit.Text = "Exit";
            btnExit.Size = new Size(75, 32);
            btnExit.Location = new Point(640, 20);
            btnExit.Click += btnExit_Click;

            // Add export and print buttons
            Button btnExport = new Button
            {
                Text = "Export PDF",
                Location = new Point(500, 60),
                Size = new Size(120, 32),
                BackColor = Color.LightGreen,
                Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold)
            };
            btnExport.Click += btnExport_Click;

            Button btnPrint = new Button
            {
                Text = "Print",
                Location = new Point(640, 60),
                Size = new Size(75, 32),
                BackColor = Color.LightYellow,
                Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold)
            };
            btnPrint.Click += btnPrint_Click;

            // Add controls to control panel
            controlPanel.Controls.AddRange(new Control[] {
                lblCategory, lblReportType, lblSearch,
                comboBox1, comboBox2, textBox1,
                radioButton1, radioButton2, radioButton3,
                btnReport, btnExit, btnExport, btnPrint
            });

            // Create report viewer
            reportViewer = new ReportViewer
            {
                Dock = DockStyle.Fill,
                ProcessingMode = ProcessingMode.Local
            };

            // Add panels to main panel
            mainPanel.Controls.Add(reportViewer);
            mainPanel.Controls.Add(controlPanel);

            // Add main panel to form
            this.Controls.Add(mainPanel);

            // Set report path
            reportViewer.LocalReport.ReportPath = "Reports/StockInHandReport.rdlc";

            // Set form properties
            this.Text = "Stock In Hand Report";
            this.WindowState = FormWindowState.Maximized;
        }

        private void LoadCategories()
        {
            try
            {
                string query = "SELECT DISTINCT Category FROM Items WHERE IsActive = 1 AND Category IS NOT NULL ORDER BY Category";
                DataTable categoryData = DatabaseConnection.ExecuteQuery(query);
                
                comboBox1.Items.Clear();
                comboBox1.Items.Add("All Categories");
                
                foreach (DataRow row in categoryData.Rows)
                {
                    comboBox1.Items.Add(row["Category"].ToString());
                }
                
                comboBox1.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading categories: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetDefaultReportType()
        {
            comboBox2.SelectedIndex = 0;
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            GenerateStockReport();
        }

        private void GenerateStockReport()
        {
            try
            {
                string query = @"SELECT 
                                i.ItemID,
                                i.ItemName,
                                i.Category,
                                i.Price,
                                i.StockQuantity,
                                i.Price * i.StockQuantity as StockValue
                               FROM Items i
                               WHERE i.IsActive = 1";

                List<SqlParameter> parameters = new List<SqlParameter>();

                // Add category filter
                if (comboBox1.SelectedIndex > 0)
                {
                    query += " AND i.Category = @Category";
                    parameters.Add(new SqlParameter("@Category", comboBox1.SelectedItem.ToString()));
                }

                // Add stock status filter
                if (radioButton2.Checked)
                {
                    query += " AND i.StockQuantity <= 10";
                }
                else if (radioButton3.Checked)
                {
                    query += " AND i.StockQuantity = 0";
                }

                // Add sorting based on report type
                switch (comboBox2.SelectedIndex)
                {
                    case 1: // Low Stock
                        query += " AND i.StockQuantity <= 10";
                        break;
                    case 2: // Out of Stock
                        query += " AND i.StockQuantity = 0";
                        break;
                    case 3: // By Value (High to Low)
                        query += " ORDER BY i.Price * i.StockQuantity DESC";
                        break;
                    default:
                        query += " ORDER BY i.Category, i.ItemName";
                        break;
                }

                // If no specific sorting is applied, use default
                if (!query.Contains("ORDER BY"))
                {
                    query += " ORDER BY i.Category, i.ItemName";
                }

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
            decimal totalStockValue = 0;
            int totalItems = reportData.Rows.Count;
            int lowStockItems = 0;
            int outOfStockItems = 0;

            foreach (DataRow row in reportData.Rows)
            {
                totalStockValue += Convert.ToDecimal(row["StockValue"]);
                int stockQty = Convert.ToInt32(row["StockQuantity"]);
                if (stockQty <= 10) lowStockItems++;
                if (stockQty == 0) outOfStockItems++;
            }

            string summary = $"Stock Summary:\n" +
                           $"Total Items: {totalItems}\n" +
                           $"Total Stock Value: ${totalStockValue:N2}\n" +
                           $"Low Stock Items (≤10): {lowStockItems}\n" +
                           $"Out of Stock Items: {outOfStockItems}";

            MessageBox.Show(summary, "Stock Report Summary", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                        FileName = $"StockInHandReport_{DateTime.Now:yyyyMMdd}.pdf"
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
    }
}
