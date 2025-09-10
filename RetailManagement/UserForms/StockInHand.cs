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
            try
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
            comboBox2.Items.AddRange(new object[] { "All Items", "Low Stock (=10)", "Out of Stock", "By Value (High to Low)" });

            // Create search textbox with placeholder
            textBox1 = new TextBox
            {
                Location = new Point(110, 80),
                Size = new Size(200, 20),
                Text = "Search by item name or category...",
                ForeColor = Color.Gray
            };
            
            // Add placeholder text functionality
            textBox1.GotFocus += (s, e) =>
            {
                if (textBox1.Text == "Search by item name or category...")
                {
                    textBox1.Text = "";
                    textBox1.ForeColor = Color.Black;
                }
            };
            
            textBox1.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(textBox1.Text))
                {
                    textBox1.Text = "Search by item name or category...";
                    textBox1.ForeColor = Color.Gray;
                }
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
                Text = "Low Stock (=10)",
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

            // Add event handlers for real-time filtering
            comboBox1.SelectedIndexChanged += Filter_Changed;
            comboBox2.SelectedIndexChanged += Filter_Changed;
            textBox1.TextChanged += TextSearch_Changed;
            radioButton1.CheckedChanged += Filter_Changed;
            radioButton2.CheckedChanged += Filter_Changed;
            radioButton3.CheckedChanged += Filter_Changed;

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

            // Try simple RDLC first (most compatible), then others
            string simplePath = System.IO.Path.Combine(Application.StartupPath, "Reports", "StockInHandReport_Simple.rdlc");
            string originalPath = System.IO.Path.Combine(Application.StartupPath, "Reports", "StockInHandReport.rdlc");
            
            if (System.IO.File.Exists(simplePath))
            {
                reportViewer.LocalReport.ReportPath = simplePath;
            }
            else if (System.IO.File.Exists(originalPath))
            {
                reportViewer.LocalReport.ReportPath = originalPath;
            }
            else
            {
                // Fallback paths
                if (System.IO.File.Exists("Reports/StockInHandReport_Simple.rdlc"))
                    reportViewer.LocalReport.ReportPath = "Reports/StockInHandReport_Simple.rdlc";
                else
                    reportViewer.LocalReport.ReportPath = "Reports/StockInHandReport.rdlc";
            }

            // Set form properties
            this.Text = "Stock In Hand Report";
            this.WindowState = FormWindowState.Maximized;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"RDLC initialization failed, will use table view: {ex.Message}", 
                    "Report Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                reportViewer = null; // Will trigger DataGridView fallback
            }
        }

        private void LoadCategories()
        {
            try
            {
                string query = "SELECT DISTINCT Category FROM Items WHERE IsActive = 1 AND Category IS NOT NULL ORDER BY Category";
                DataTable categoryData = DatabaseConnection.ExecuteQuery(query);
                
                DataTable comboBox1Data = new DataTable();
                comboBox1Data.Columns.Add("Category", typeof(string));
                comboBox1Data.Rows.Add("All Categories");
                
                foreach (DataRow row in categoryData.Rows)
                {
                    comboBox1Data.Rows.Add(row["Category"].ToString());
                }
                
                comboBox1.DataSource = comboBox1Data;
                comboBox1.DisplayMember = "Category";
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
            // Setup ComboBox2 with proper stock report types
            if (comboBox2.Items.Count == 0)
            {
                comboBox2.Items.Clear();
                comboBox2.Items.AddRange(new string[] {
                    "All Items",
                    "Low Stock",
                    "Out of Stock",
                    "By Value (High to Low)"
                });
            }
            comboBox2.SelectedIndex = 0;
        }

        private int GetLowStockThreshold()
        {
            // You can make this configurable or read from settings
            // For now, return a default value of 10
            return 10;
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            GenerateStockReport();
        }

        private void GenerateStockReport()
        {
            try
            {
                // Get the correct stock column name (dynamic based on database structure)
                string stockColumnName = DatabaseConnection.GetStockColumnName();
                
                string query = $@"SELECT 
                                i.ItemID,
                                i.ItemName,
                                i.Category,
                                i.Price,
                                ISNULL({stockColumnName}, 0) as StockQuantity,
                                ISNULL(i.Price * {stockColumnName}, 0) as StockValue,
                                ISNULL(i.PurchasePrice, 0) as PurchasePrice,
                                CASE 
                                    WHEN ISNULL({stockColumnName}, 0) = 0 THEN 'Out of Stock'
                                    WHEN ISNULL({stockColumnName}, 0) <= 10 THEN 'Low Stock'
                                    ELSE 'In Stock'
                                END as StockStatus
                               FROM Items i
                               WHERE i.IsActive = 1";

                List<SqlParameter> parameters = new List<SqlParameter>();

                // Add category filter
                if (comboBox1.SelectedIndex > 0 && comboBox1.SelectedItem != null)
                {
                    DataRowView selectedCategory = comboBox1.SelectedItem as DataRowView;
                    if (selectedCategory != null)
                    {
                        string categoryValue = selectedCategory["Category"].ToString();
                        if (categoryValue != "All Categories")
                        {
                            query += " AND i.Category = @Category";
                            parameters.Add(new SqlParameter("@Category", categoryValue));
                        }
                    }
                }

                // Add text search filter (ignore placeholder text)
                if (!string.IsNullOrWhiteSpace(textBox1.Text) && 
                    textBox1.Text != "Search by item name or category..." &&
                    textBox1.ForeColor != Color.Gray)
                {
                    query += " AND (i.ItemName LIKE @SearchText OR i.Category LIKE @SearchText)";
                    parameters.Add(new SqlParameter("@SearchText", "%" + textBox1.Text.Trim() + "%"));
                }

                // Check radio button selection first, then comboBox2 as fallback
                bool useRadioFilter = false;
                if (radioButton2.Checked) // Low Stock
                {
                    int lowStockThreshold = GetLowStockThreshold();
                    query += $" AND ISNULL({stockColumnName}, 0) <= {lowStockThreshold} AND ISNULL({stockColumnName}, 0) > 0";
                    query += $" ORDER BY {stockColumnName} ASC, i.ItemName";
                    useRadioFilter = true;
                }
                else if (radioButton3.Checked) // Out of Stock
                {
                    query += $" AND ISNULL({stockColumnName}, 0) = 0";
                    query += " ORDER BY i.ItemName";
                    useRadioFilter = true;
                }
                else if (radioButton1.Checked) // All Items
                {
                    // No additional filter for All Items radio button
                    useRadioFilter = true;
                }

                // If no radio button selection or All Items selected, use comboBox2 filter
                if (!useRadioFilter || radioButton1.Checked)
                {
                    switch (comboBox2.SelectedIndex)
                    {
                        case 0: // All Items - no additional filter
                            query += " ORDER BY i.Category, i.ItemName";
                            break;
                        case 1: // Low Stock
                            int lowStockThreshold = GetLowStockThreshold();
                            query += $" AND ISNULL({stockColumnName}, 0) <= {lowStockThreshold} AND ISNULL({stockColumnName}, 0) > 0";
                            query += $" ORDER BY {stockColumnName} ASC, i.ItemName";
                            break;
                        case 2: // Out of Stock
                            query += $" AND ISNULL({stockColumnName}, 0) = 0";
                            query += " ORDER BY i.ItemName";
                            break;
                        case 3: // By Value (High to Low)
                            query += $" ORDER BY ISNULL(i.Price * {stockColumnName}, 0) DESC";
                            break;
                        default:
                            query += " ORDER BY i.Category, i.ItemName";
                            break;
                    }
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
                           $"Total Stock Value: {totalStockValue:N2}\n" +
                           $"Low Stock Items (=10): {lowStockItems}\n" +
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

        // Enhanced Filter Event Handlers
        private void Filter_Changed(object sender, EventArgs e)
        {
            // Auto-generate report when filter changes
            try
            {
                if (reportData != null || (comboBox1.SelectedIndex >= 0 && comboBox2.SelectedIndex >= 0))
                {
                    GenerateStockReport();
                }
            }
            catch (Exception ex)
            {
                // Silent catch for filter changes to avoid constant error popups
                System.Diagnostics.Debug.WriteLine($"Filter change error: {ex.Message}");
            }
        }

        private System.Windows.Forms.Timer searchTimer;
        private void TextSearch_Changed(object sender, EventArgs e)
        {
            // Implement debounced search to avoid too many queries while typing
            if (searchTimer != null)
            {
                searchTimer.Stop();
                searchTimer.Dispose();
            }

            searchTimer = new System.Windows.Forms.Timer();
            searchTimer.Interval = 500; // 500ms delay after user stops typing
            searchTimer.Tick += (s, args) =>
            {
                searchTimer.Stop();
                searchTimer.Dispose();
                searchTimer = null;
                
                try
                {
                    GenerateStockReport();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Search error: {ex.Message}");
                }
            };
            searchTimer.Start();
        }

        // Enhanced error handling and validation
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try
            {
                // Clean up timer
                if (searchTimer != null)
                {
                    searchTimer.Stop();
                    searchTimer.Dispose();
                    searchTimer = null;
                }
            }
            catch
            {
                // Silent cleanup
            }
            base.OnFormClosing(e);
        }
    }
}
