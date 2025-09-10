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
using Microsoft.Reporting.WinForms;
using RetailManagement.Database;
using RetailManagement.Models;

namespace RetailManagement.UserForms
{
    public partial class SupplierBalanceReport : Form
    {
        private DataTable balanceData;
        private TabControl tabControl;
        private DataGridView dgvSupplierBalance;
        private DataGridView dgvDetailedAnalysis;
        private DataGridView dgvAgeingAnalysis;
        private Panel summaryPanel;
        private Label lblTotalSuppliers, lblTotalOutstanding, lblOverdueAmount, lblActiveSuppliers;
        private ComboBox cmbSupplier, cmbReportType;
        private DateTimePicker dtpFromDate, dtpToDate;
        private Button btnGenerate, btnPrint, btnExport, btnRefresh, btnExit;

        public SupplierBalanceReport()
        {
            InitializeComponent();
            InitializeControls();
            LoadSuppliers();
            GenerateSupplierBalanceReport();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // Form properties
            this.Text = "Supplier Balance Report";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            
            this.ResumeLayout(false);
        }

        private void InitializeControls()
        {
            try
            {
                // Main Container
                Panel mainPanel = new Panel
                {
                    Dock = DockStyle.Fill,
                    Padding = new Padding(10),
                    BackColor = Color.White
                };
                this.Controls.Add(mainPanel);

                // Title Panel
                Panel titlePanel = new Panel
                {
                    Dock = DockStyle.Top,
                    Height = 60,
                    BackColor = Color.FromArgb(0, 122, 204)
                };
                mainPanel.Controls.Add(titlePanel);

                Label lblTitle = new Label
                {
                    Text = "📊 Supplier Balance Report & Analysis",
                    Font = new Font("Segoe UI", 16, FontStyle.Bold),
                    ForeColor = Color.White,
                    Location = new Point(20, 15),
                    AutoSize = true
                };
                titlePanel.Controls.Add(lblTitle);

                // Filter Panel
                Panel filterPanel = new Panel
                {
                    Dock = DockStyle.Top,
                    Height = 80,
                    BackColor = Color.LightGray,
                    BorderStyle = BorderStyle.FixedSingle
                };
                mainPanel.Controls.Add(filterPanel);

                // Filter Controls
                Label lblSupplier = new Label
                {
                    Text = "Supplier:",
                    Location = new Point(10, 15),
                    Size = new Size(70, 23),
                    Font = new Font("Microsoft Sans Serif", 9, FontStyle.Bold)
                };
                filterPanel.Controls.Add(lblSupplier);

                cmbSupplier = new ComboBox
                {
                    Location = new Point(90, 12),
                    Size = new Size(200, 23),
                    DropDownStyle = ComboBoxStyle.DropDownList
                };
                filterPanel.Controls.Add(cmbSupplier);

                Label lblReportType = new Label
                {
                    Text = "Report Type:",
                    Location = new Point(310, 15),
                    Size = new Size(80, 23),
                    Font = new Font("Microsoft Sans Serif", 9, FontStyle.Bold)
                };
                filterPanel.Controls.Add(lblReportType);

                cmbReportType = new ComboBox
                {
                    Location = new Point(400, 12),
                    Size = new Size(150, 23),
                    DropDownStyle = ComboBoxStyle.DropDownList
                };
                cmbReportType.Items.AddRange(new string[] { "All Balances", "Outstanding Only", "Overdue Only", "Active Suppliers" });
                cmbReportType.SelectedIndex = 0;
                filterPanel.Controls.Add(cmbReportType);

                Label lblFromDate = new Label
                {
                    Text = "From:",
                    Location = new Point(570, 15),
                    Size = new Size(50, 23),
                    Font = new Font("Microsoft Sans Serif", 9, FontStyle.Bold)
                };
                filterPanel.Controls.Add(lblFromDate);

                dtpFromDate = new DateTimePicker
                {
                    Location = new Point(630, 12),
                    Size = new Size(120, 23),
                    Value = DateTime.Now.AddDays(-30)
                };
                filterPanel.Controls.Add(dtpFromDate);

                Label lblToDate = new Label
                {
                    Text = "To:",
                    Location = new Point(760, 15),
                    Size = new Size(30, 23),
                    Font = new Font("Microsoft Sans Serif", 9, FontStyle.Bold)
                };
                filterPanel.Controls.Add(lblToDate);

                dtpToDate = new DateTimePicker
                {
                    Location = new Point(800, 12),
                    Size = new Size(120, 23),
                    Value = DateTime.Now
                };
                filterPanel.Controls.Add(dtpToDate);

                // Buttons
                btnGenerate = new Button
                {
                    Text = "Generate",
                    Location = new Point(10, 45),
                    Size = new Size(80, 30),
                    BackColor = Color.Green,
                    ForeColor = Color.White,
                    Font = new Font("Microsoft Sans Serif", 9, FontStyle.Bold)
                };
                btnGenerate.Click += BtnGenerate_Click;
                filterPanel.Controls.Add(btnGenerate);

                btnPrint = new Button
                {
                    Text = "Print",
                    Location = new Point(100, 45),
                    Size = new Size(80, 30),
                    BackColor = Color.Blue,
                    ForeColor = Color.White,
                    Font = new Font("Microsoft Sans Serif", 9, FontStyle.Bold)
                };
                btnPrint.Click += BtnPrint_Click;
                filterPanel.Controls.Add(btnPrint);

                btnExport = new Button
                {
                    Text = "Export",
                    Location = new Point(190, 45),
                    Size = new Size(80, 30),
                    BackColor = Color.Orange,
                    ForeColor = Color.White,
                    Font = new Font("Microsoft Sans Serif", 9, FontStyle.Bold)
                };
                btnExport.Click += BtnExport_Click;
                filterPanel.Controls.Add(btnExport);

                btnRefresh = new Button
                {
                    Text = "Refresh",
                    Location = new Point(280, 45),
                    Size = new Size(80, 30),
                    BackColor = Color.Purple,
                    ForeColor = Color.White,
                    Font = new Font("Microsoft Sans Serif", 9, FontStyle.Bold)
                };
                btnRefresh.Click += BtnRefresh_Click;
                filterPanel.Controls.Add(btnRefresh);

                btnExit = new Button
                {
                    Text = "Exit",
                    Location = new Point(840, 45),
                    Size = new Size(80, 30),
                    BackColor = Color.Red,
                    ForeColor = Color.White,
                    Font = new Font("Microsoft Sans Serif", 9, FontStyle.Bold)
                };
                btnExit.Click += BtnExit_Click;
                filterPanel.Controls.Add(btnExit);

                // Summary Panel
                summaryPanel = new Panel
                {
                    Dock = DockStyle.Top,
                    Height = 100,
                    BackColor = Color.AliceBlue,
                    BorderStyle = BorderStyle.FixedSingle
                };
                mainPanel.Controls.Add(summaryPanel);

                CreateSummaryLabels();

                // Tab Control for different views
                tabControl = new TabControl
                {
                    Dock = DockStyle.Fill
                };
                mainPanel.Controls.Add(tabControl);

                // Create tabs
                CreateBalanceTab();
                CreateDetailedAnalysisTab();
                CreateAgeingAnalysisTab();

                // Set tab events
                tabControl.SelectedIndexChanged += TabControl_SelectedIndexChanged;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error initializing controls: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CreateSummaryLabels()
        {
            lblTotalSuppliers = new Label
            {
                Text = "Total Suppliers: 0",
                Location = new Point(20, 20),
                Size = new Size(150, 25),
                Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold),
                ForeColor = Color.DarkBlue
            };
            summaryPanel.Controls.Add(lblTotalSuppliers);

            lblActiveSuppliers = new Label
            {
                Text = "Active: 0",
                Location = new Point(20, 50),
                Size = new Size(150, 25),
                Font = new Font("Microsoft Sans Serif", 9, FontStyle.Regular),
                ForeColor = Color.Green
            };
            summaryPanel.Controls.Add(lblActiveSuppliers);

            lblTotalOutstanding = new Label
            {
                Text = "Total Outstanding: $0.00",
                Location = new Point(200, 20),
                Size = new Size(200, 25),
                Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold),
                ForeColor = Color.Red
            };
            summaryPanel.Controls.Add(lblTotalOutstanding);

            lblOverdueAmount = new Label
            {
                Text = "Overdue Amount: $0.00",
                Location = new Point(200, 50),
                Size = new Size(200, 25),
                Font = new Font("Microsoft Sans Serif", 9, FontStyle.Regular),
                ForeColor = Color.OrangeRed
            };
            summaryPanel.Controls.Add(lblOverdueAmount);
        }

        private void CreateBalanceTab()
        {
            TabPage tabBalance = new TabPage("Supplier Balance Overview");
            
            dgvSupplierBalance = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = true,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D
            };
            
            tabBalance.Controls.Add(dgvSupplierBalance);
            tabControl.TabPages.Add(tabBalance);
        }

        private void CreateDetailedAnalysisTab()
        {
            TabPage tabDetailed = new TabPage("Detailed Analysis");
            
            dgvDetailedAnalysis = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = true,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D
            };
            
            tabDetailed.Controls.Add(dgvDetailedAnalysis);
            tabControl.TabPages.Add(tabDetailed);
        }

        private void CreateAgeingAnalysisTab()
        {
            TabPage tabAgeing = new TabPage("Ageing Analysis");
            
            dgvAgeingAnalysis = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = true,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D
            };
            
            tabAgeing.Controls.Add(dgvAgeingAnalysis);
            tabControl.TabPages.Add(tabAgeing);
        }

        private void LoadSuppliers()
        {
            try
            {
                string query = "SELECT CompanyID, CompanyName FROM Companies WHERE IsActive = 1 ORDER BY CompanyName";
                DataTable dt = DatabaseConnection.ExecuteQuery(query);

                cmbSupplier.Items.Clear();
                cmbSupplier.Items.Add(new ComboBoxItem { Text = "All Suppliers", Value = 0 });

                foreach (DataRow row in dt.Rows)
                {
                    cmbSupplier.Items.Add(new ComboBoxItem 
                    { 
                        Text = row["CompanyName"].ToString(), 
                        Value = Convert.ToInt32(row["CompanyID"]) 
                    });
                }

                cmbSupplier.DisplayMember = "Text";
                cmbSupplier.ValueMember = "Value";
                cmbSupplier.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading suppliers: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerateSupplierBalanceReport()
        {
            try
            {
                LoadSupplierBalanceData();
                LoadDetailedAnalysisData();
                LoadAgeingAnalysisData();
                UpdateSummaryInfo();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating supplier balance report: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSupplierBalanceData()
        {
            try
            {
                string query = @"SELECT 
                                c.CompanyID,
                                c.CompanyName as [Supplier Name],
                                c.ContactPerson as [Contact Person],
                                c.Phone,
                                c.Email,
                                ISNULL(c.CreditLimit, 0) as [Credit Limit],
                                ISNULL(SUM(p.NetAmount), 0) as [Total Purchases],
                                ISNULL(SUM(cp.Amount), 0) as [Total Payments],
                                ISNULL(SUM(pr.TotalAmount), 0) as [Returns],
                                (ISNULL(SUM(p.NetAmount), 0) - ISNULL(SUM(cp.Amount), 0) - ISNULL(SUM(pr.TotalAmount), 0)) as [Outstanding Balance],
                                CASE 
                                    WHEN (ISNULL(SUM(p.NetAmount), 0) - ISNULL(SUM(cp.Amount), 0) - ISNULL(SUM(pr.TotalAmount), 0)) > ISNULL(c.CreditLimit, 0) 
                                    THEN 'Over Limit' 
                                    WHEN (ISNULL(SUM(p.NetAmount), 0) - ISNULL(SUM(cp.Amount), 0) - ISNULL(SUM(pr.TotalAmount), 0)) > 0 
                                    THEN 'Outstanding' 
                                    ELSE 'Clear' 
                                END as [Status],
                                MAX(p.PurchaseDate) as [Last Purchase Date],
                                MAX(cp.PaymentDate) as [Last Payment Date]
                               FROM Companies c
                               LEFT JOIN Purchases p ON c.CompanyID = p.CompanyID AND p.IsActive = 1
                               LEFT JOIN CompanyPayments cp ON c.CompanyID = cp.CompanyID AND cp.IsActive = 1
                               LEFT JOIN PurchaseReturns pr ON c.CompanyID = pr.CompanyID AND pr.IsActive = 1
                               WHERE c.IsActive = 1";

                // Add supplier filter
                if (cmbSupplier.SelectedItem != null && ((ComboBoxItem)cmbSupplier.SelectedItem).Value.ToString() != "0")
                {
                    query += " AND c.CompanyID = @CompanyID";
                }

                // Add report type filter
                string reportType = cmbReportType.SelectedItem?.ToString() ?? "All Balances";
                switch (reportType)
                {
                    case "Outstanding Only":
                        query += " GROUP BY c.CompanyID, c.CompanyName, c.ContactPerson, c.Phone, c.Email, c.CreditLimit " +
                                " HAVING (ISNULL(SUM(p.NetAmount), 0) - ISNULL(SUM(cp.Amount), 0) - ISNULL(SUM(pr.TotalAmount), 0)) > 0";
                        break;
                    case "Overdue Only":
                        query += " GROUP BY c.CompanyID, c.CompanyName, c.ContactPerson, c.Phone, c.Email, c.CreditLimit " +
                                " HAVING (ISNULL(SUM(p.NetAmount), 0) - ISNULL(SUM(cp.Amount), 0) - ISNULL(SUM(pr.TotalAmount), 0)) > ISNULL(c.CreditLimit, 0)";
                        break;
                    default:
                        query += " GROUP BY c.CompanyID, c.CompanyName, c.ContactPerson, c.Phone, c.Email, c.CreditLimit";
                        break;
                }

                query += " ORDER BY [Outstanding Balance] DESC, c.CompanyName";

                SqlParameter[] parameters = {};
                if (cmbSupplier.SelectedItem != null && ((ComboBoxItem)cmbSupplier.SelectedItem).Value.ToString() != "0")
                {
                    parameters = new SqlParameter[] { new SqlParameter("@CompanyID", ((ComboBoxItem)cmbSupplier.SelectedItem).Value) };
                }

                balanceData = DatabaseConnection.ExecuteQuery(query, parameters);
                dgvSupplierBalance.DataSource = balanceData;

                // Format columns
                FormatBalanceGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading supplier balance data: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormatBalanceGrid()
        {
            if (dgvSupplierBalance.Columns.Count > 0)
            {
                // Hide CompanyID column
                if (dgvSupplierBalance.Columns.Contains("CompanyID"))
                    dgvSupplierBalance.Columns["CompanyID"].Visible = false;

                // Format currency columns
                string[] currencyColumns = { "Credit Limit", "Total Purchases", "Total Payments", "Returns", "Outstanding Balance" };
                foreach (string colName in currencyColumns)
                {
                    if (dgvSupplierBalance.Columns.Contains(colName))
                    {
                        dgvSupplierBalance.Columns[colName].DefaultCellStyle.Format = "N2";
                        dgvSupplierBalance.Columns[colName].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    }
                }

                // Format date columns
                string[] dateColumns = { "Last Purchase Date", "Last Payment Date" };
                foreach (string colName in dateColumns)
                {
                    if (dgvSupplierBalance.Columns.Contains(colName))
                    {
                        dgvSupplierBalance.Columns[colName].DefaultCellStyle.Format = "dd/MM/yyyy";
                        dgvSupplierBalance.Columns[colName].Width = 120;
                    }
                }

                // Color code status
                dgvSupplierBalance.CellFormatting += (sender, e) =>
                {
                    if (e.ColumnIndex >= 0 && dgvSupplierBalance.Columns[e.ColumnIndex].Name == "Status")
                    {
                        string status = e.Value?.ToString() ?? "";
                        switch (status)
                        {
                            case "Over Limit":
                                e.CellStyle.BackColor = Color.Red;
                                e.CellStyle.ForeColor = Color.White;
                                break;
                            case "Outstanding":
                                e.CellStyle.BackColor = Color.Orange;
                                e.CellStyle.ForeColor = Color.Black;
                                break;
                            case "Clear":
                                e.CellStyle.BackColor = Color.LightGreen;
                                e.CellStyle.ForeColor = Color.Black;
                                break;
                        }
                    }
                };

                // Auto size columns
                dgvSupplierBalance.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            }
        }

        private void LoadDetailedAnalysisData()
        {
            try
            {
                string query = @"SELECT 
                                p.PurchaseNumber as [Purchase No],
                                p.PurchaseDate as [Date],
                                c.CompanyName as [Supplier],
                                p.NetAmount as [Amount],
                                ISNULL(SUM(cp.Amount), 0) as [Paid],
                                (p.NetAmount - ISNULL(SUM(cp.Amount), 0)) as [Outstanding],
                                DATEDIFF(day, p.PurchaseDate, GETDATE()) as [Days Outstanding],
                                CASE 
                                    WHEN DATEDIFF(day, p.PurchaseDate, GETDATE()) <= 30 THEN '0-30 Days'
                                    WHEN DATEDIFF(day, p.PurchaseDate, GETDATE()) <= 60 THEN '31-60 Days'
                                    WHEN DATEDIFF(day, p.PurchaseDate, GETDATE()) <= 90 THEN '61-90 Days'
                                    ELSE '90+ Days'
                                END as [Age Category],
                                p.Remarks as [Notes]
                               FROM Purchases p
                               INNER JOIN Companies c ON p.CompanyID = c.CompanyID
                               LEFT JOIN CompanyPayments cp ON p.CompanyID = cp.CompanyID
                               WHERE p.IsActive = 1 
                               AND p.PurchaseDate BETWEEN @FromDate AND @ToDate
                               AND (p.NetAmount - ISNULL(SUM(cp.Amount), 0)) > 0";

                if (cmbSupplier.SelectedItem != null && ((ComboBoxItem)cmbSupplier.SelectedItem).Value.ToString() != "0")
                {
                    query += " AND c.CompanyID = @CompanyID";
                }

                query += @" GROUP BY p.PurchaseNumber, p.PurchaseDate, c.CompanyName, p.NetAmount, p.Remarks
                           ORDER BY p.PurchaseDate DESC";

                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    new SqlParameter("@FromDate", dtpFromDate.Value.Date),
                    new SqlParameter("@ToDate", dtpToDate.Value.Date)
                };

                if (cmbSupplier.SelectedItem != null && ((ComboBoxItem)cmbSupplier.SelectedItem).Value.ToString() != "0")
                {
                    parameters.Add(new SqlParameter("@CompanyID", ((ComboBoxItem)cmbSupplier.SelectedItem).Value));
                }

                DataTable detailedData = DatabaseConnection.ExecuteQuery(query, parameters.ToArray());
                dgvDetailedAnalysis.DataSource = detailedData;

                // Format detailed analysis grid
                FormatDetailedGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading detailed analysis: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormatDetailedGrid()
        {
            if (dgvDetailedAnalysis.Columns.Count > 0)
            {
                // Format currency columns
                string[] currencyColumns = { "Amount", "Paid", "Outstanding" };
                foreach (string colName in currencyColumns)
                {
                    if (dgvDetailedAnalysis.Columns.Contains(colName))
                    {
                        dgvDetailedAnalysis.Columns[colName].DefaultCellStyle.Format = "N2";
                        dgvDetailedAnalysis.Columns[colName].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    }
                }

                // Format date column
                if (dgvDetailedAnalysis.Columns.Contains("Date"))
                {
                    dgvDetailedAnalysis.Columns["Date"].DefaultCellStyle.Format = "dd/MM/yyyy";
                }

                // Color code age categories
                dgvDetailedAnalysis.CellFormatting += (sender, e) =>
                {
                    if (e.ColumnIndex >= 0 && dgvDetailedAnalysis.Columns[e.ColumnIndex].Name == "Age Category")
                    {
                        string ageCategory = e.Value?.ToString() ?? "";
                        switch (ageCategory)
                        {
                            case "0-30 Days":
                                e.CellStyle.BackColor = Color.LightGreen;
                                break;
                            case "31-60 Days":
                                e.CellStyle.BackColor = Color.Yellow;
                                break;
                            case "61-90 Days":
                                e.CellStyle.BackColor = Color.Orange;
                                break;
                            case "90+ Days":
                                e.CellStyle.BackColor = Color.Red;
                                e.CellStyle.ForeColor = Color.White;
                                break;
                        }
                    }
                };

                dgvDetailedAnalysis.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            }
        }

        private void LoadAgeingAnalysisData()
        {
            try
            {
                string query = @"SELECT 
                                c.CompanyName as [Supplier],
                                SUM(CASE WHEN DATEDIFF(day, p.PurchaseDate, GETDATE()) <= 30 THEN (p.NetAmount - ISNULL(cp.Amount, 0)) ELSE 0 END) as [0-30 Days],
                                SUM(CASE WHEN DATEDIFF(day, p.PurchaseDate, GETDATE()) BETWEEN 31 AND 60 THEN (p.NetAmount - ISNULL(cp.Amount, 0)) ELSE 0 END) as [31-60 Days],
                                SUM(CASE WHEN DATEDIFF(day, p.PurchaseDate, GETDATE()) BETWEEN 61 AND 90 THEN (p.NetAmount - ISNULL(cp.Amount, 0)) ELSE 0 END) as [61-90 Days],
                                SUM(CASE WHEN DATEDIFF(day, p.PurchaseDate, GETDATE()) > 90 THEN (p.NetAmount - ISNULL(cp.Amount, 0)) ELSE 0 END) as [90+ Days],
                                SUM(p.NetAmount - ISNULL(cp.Amount, 0)) as [Total Outstanding]
                               FROM Companies c
                               INNER JOIN Purchases p ON c.CompanyID = p.CompanyID
                               LEFT JOIN (
                                   SELECT CompanyID, SUM(Amount) as Amount 
                                   FROM CompanyPayments 
                                   WHERE IsActive = 1 
                                   GROUP BY CompanyID
                               ) cp ON c.CompanyID = cp.CompanyID
                               WHERE p.IsActive = 1 
                               AND c.IsActive = 1
                               AND (p.NetAmount - ISNULL(cp.Amount, 0)) > 0";

                if (cmbSupplier.SelectedItem != null && ((ComboBoxItem)cmbSupplier.SelectedItem).Value.ToString() != "0")
                {
                    query += " AND c.CompanyID = @CompanyID";
                }

                query += @" GROUP BY c.CompanyName
                           HAVING SUM(p.NetAmount - ISNULL(cp.Amount, 0)) > 0
                           ORDER BY SUM(p.NetAmount - ISNULL(cp.Amount, 0)) DESC";

                SqlParameter[] parameters = {};
                if (cmbSupplier.SelectedItem != null && ((ComboBoxItem)cmbSupplier.SelectedItem).Value.ToString() != "0")
                {
                    parameters = new SqlParameter[] { new SqlParameter("@CompanyID", ((ComboBoxItem)cmbSupplier.SelectedItem).Value) };
                }

                DataTable ageingData = DatabaseConnection.ExecuteQuery(query, parameters);
                dgvAgeingAnalysis.DataSource = ageingData;

                // Format ageing analysis grid
                FormatAgeingGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading ageing analysis: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormatAgeingGrid()
        {
            if (dgvAgeingAnalysis.Columns.Count > 0)
            {
                // Format all numeric columns as currency
                foreach (DataGridViewColumn col in dgvAgeingAnalysis.Columns)
                {
                    if (col.Name != "Supplier")
                    {
                        col.DefaultCellStyle.Format = "N2";
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    }
                }

                dgvAgeingAnalysis.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            }
        }

        private void UpdateSummaryInfo()
        {
            try
            {
                if (balanceData != null && balanceData.Rows.Count > 0)
                {
                    int totalSuppliers = balanceData.Rows.Count;
                    int activeSuppliers = balanceData.AsEnumerable()
                        .Count(row => row.Field<string>("Status") != "Clear");
                    
                    decimal totalOutstanding = balanceData.AsEnumerable()
                        .Sum(row => row.Field<decimal>("Outstanding Balance"));
                    
                    decimal overdueAmount = balanceData.AsEnumerable()
                        .Where(row => row.Field<string>("Status") == "Over Limit")
                        .Sum(row => row.Field<decimal>("Outstanding Balance"));

                    lblTotalSuppliers.Text = $"Total Suppliers: {totalSuppliers}";
                    lblActiveSuppliers.Text = $"With Outstanding: {activeSuppliers}";
                    lblTotalOutstanding.Text = $"Total Outstanding: {totalOutstanding:C2}";
                    lblOverdueAmount.Text = $"Over Limit: {overdueAmount:C2}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating summary: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            GenerateSupplierBalanceReport();
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                // Simple grid printing functionality
                PrintDocument printDoc = new PrintDocument();
                printDoc.PrintPage += (s, ev) =>
                {
                    // Basic printing implementation
                    Bitmap bmp = new Bitmap(dgvSupplierBalance.Width, dgvSupplierBalance.Height);
                    dgvSupplierBalance.DrawToBitmap(bmp, new Rectangle(0, 0, dgvSupplierBalance.Width, dgvSupplierBalance.Height));
                    ev.Graphics.DrawImage(bmp, 100, 100);
                };

                PrintPreviewDialog printPreview = new PrintPreviewDialog();
                printPreview.Document = printDoc;
                printPreview.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error printing report: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                saveDialog.FileName = $"SupplierBalanceReport_{DateTime.Now:yyyyMMdd}";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    StringBuilder sb = new StringBuilder();
                    
                    // Add headers
                    for (int i = 0; i < dgvSupplierBalance.Columns.Count; i++)
                    {
                        if (dgvSupplierBalance.Columns[i].Visible)
                        {
                            sb.Append(dgvSupplierBalance.Columns[i].HeaderText);
                            if (i < dgvSupplierBalance.Columns.Count - 1) sb.Append(",");
                        }
                    }
                    sb.AppendLine();

                    // Add data
                    foreach (DataGridViewRow row in dgvSupplierBalance.Rows)
                    {
                        for (int i = 0; i < dgvSupplierBalance.Columns.Count; i++)
                        {
                            if (dgvSupplierBalance.Columns[i].Visible)
                            {
                                sb.Append(row.Cells[i].Value?.ToString() ?? "");
                                if (i < dgvSupplierBalance.Columns.Count - 1) sb.Append(",");
                            }
                        }
                        sb.AppendLine();
                    }

                    System.IO.File.WriteAllText(saveDialog.FileName, sb.ToString());
                    MessageBox.Show("Report exported successfully!", "Export Complete", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exporting report: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadSuppliers();
            GenerateSupplierBalanceReport();
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Refresh data when switching tabs
            switch (tabControl.SelectedIndex)
            {
                case 0:
                    LoadSupplierBalanceData();
                    break;
                case 1:
                    LoadDetailedAnalysisData();
                    break;
                case 2:
                    LoadAgeingAnalysisData();
                    break;
            }
        }
    }
}
