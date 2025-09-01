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
    public partial class ProfitAndLoss : Form
    {
        private DataGridView dgvProfitLoss;
        private DataGridView dgvDetailedAnalysis;
        private DataGridView dgvTrendAnalysis;
        private Panel chartProfitTrend;
        private Panel chartCategoryAnalysis;
        private TabControl tabControl;
        private Label lblTotalRevenue, lblTotalCOGS, lblGrossProfit, lblOperatingExp, lblNetProfit;
        private Label lblGrossProfitMargin, lblNetProfitMargin, lblROI;

        public ProfitAndLoss()
        {
            InitializeComponent();
            SetDefaultDateRange();
            InitializeEnhancedControls();
            GenerateEnhancedProfitAndLoss();
        }

        private void SetDefaultDateRange()
        {
            dateTimePicker1.Value = DateTime.Now.AddMonths(-1); // Last month
            dateTimePicker2.Value = DateTime.Now;
        }

        private void InitializeEnhancedControls()
        {
            try
            {
                // Resize the form for comprehensive analysis
                this.Size = new Size(1200, 800);
                this.StartPosition = FormStartPosition.CenterScreen;
                this.Text = "Enhanced Profit & Loss Analysis";

                // Create TabControl for different analysis views
                tabControl = new TabControl
                {
                    Location = new Point(10, 110),
                    Size = new Size(1170, 640),
                    Font = new Font("Microsoft Sans Serif", 9, FontStyle.Bold)
                };

                // Create Summary Tab
                TabPage summaryTab = new TabPage("Summary Dashboard");
                CreateSummaryTab(summaryTab);
                tabControl.TabPages.Add(summaryTab);

                // Create Detailed Analysis Tab
                TabPage detailTab = new TabPage("Detailed Analysis");
                CreateDetailedAnalysisTab(detailTab);
                tabControl.TabPages.Add(detailTab);

                // Create Trend Analysis Tab
                TabPage trendTab = new TabPage("Trend Analysis");
                CreateTrendAnalysisTab(trendTab);
                tabControl.TabPages.Add(trendTab);

                // Create Category Analysis Tab
                TabPage categoryTab = new TabPage("Category Analysis");
                CreateCategoryAnalysisTab(categoryTab);
                tabControl.TabPages.Add(categoryTab);

                this.Controls.Add(tabControl);

                // Update button event to use enhanced generation
                btnProfit.Click += BtnGenerate_Click;
                btnProfit.Text = "Generate Analysis";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error initializing enhanced controls: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            if (dateTimePicker1.Value > dateTimePicker2.Value)
            {
                MessageBox.Show("From date cannot be greater than To date.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            GenerateEnhancedProfitAndLoss();
        }

        private void CreateSummaryTab(TabPage tab)
        {
            // Create summary labels panel
            Panel summaryPanel = new Panel
            {
                Location = new Point(10, 10),
                Size = new Size(1140, 100),
                BackColor = Color.LightGray,
                BorderStyle = BorderStyle.Fixed3D
            };

            // Revenue Label
            lblTotalRevenue = new Label
            {
                Text = "Total Revenue: $0.00",
                Location = new Point(20, 20),
                Size = new Size(180, 25),
                Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold),
                ForeColor = Color.DarkGreen
            };
            summaryPanel.Controls.Add(lblTotalRevenue);

            // COGS Label
            lblTotalCOGS = new Label
            {
                Text = "Total COGS: $0.00",
                Location = new Point(220, 20),
                Size = new Size(180, 25),
                Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold),
                ForeColor = Color.DarkRed
            };
            summaryPanel.Controls.Add(lblTotalCOGS);

            // Gross Profit Label
            lblGrossProfit = new Label
            {
                Text = "Gross Profit: $0.00",
                Location = new Point(420, 20),
                Size = new Size(180, 25),
                Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold),
                ForeColor = Color.Blue
            };
            summaryPanel.Controls.Add(lblGrossProfit);

            // Operating Expenses Label
            lblOperatingExp = new Label
            {
                Text = "Operating Exp: $0.00",
                Location = new Point(620, 20),
                Size = new Size(180, 25),
                Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold),
                ForeColor = Color.DarkOrange
            };
            summaryPanel.Controls.Add(lblOperatingExp);

            // Net Profit Label
            lblNetProfit = new Label
            {
                Text = "Net Profit: $0.00",
                Location = new Point(820, 20),
                Size = new Size(180, 25),
                Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold),
                ForeColor = Color.DarkGreen
            };
            summaryPanel.Controls.Add(lblNetProfit);

            // Margin Labels
            lblGrossProfitMargin = new Label
            {
                Text = "Gross Margin: 0.0%",
                Location = new Point(220, 50),
                Size = new Size(180, 25),
                Font = new Font("Microsoft Sans Serif", 9, FontStyle.Regular)
            };
            summaryPanel.Controls.Add(lblGrossProfitMargin);

            lblNetProfitMargin = new Label
            {
                Text = "Net Margin: 0.0%",
                Location = new Point(420, 50),
                Size = new Size(180, 25),
                Font = new Font("Microsoft Sans Serif", 9, FontStyle.Regular)
            };
            summaryPanel.Controls.Add(lblNetProfitMargin);

            lblROI = new Label
            {
                Text = "ROI: 0.0%",
                Location = new Point(620, 50),
                Size = new Size(180, 25),
                Font = new Font("Microsoft Sans Serif", 9, FontStyle.Regular)
            };
            summaryPanel.Controls.Add(lblROI);

            tab.Controls.Add(summaryPanel);

            // Create main P&L DataGridView
            dgvProfitLoss = new DataGridView
            {
                Location = new Point(10, 120),
                Size = new Size(1140, 480),
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                BackgroundColor = Color.White,
                GridColor = Color.LightGray
            };

            // Add columns for P&L statement
            dgvProfitLoss.Columns.Add(new DataGridViewTextBoxColumn { Name = "Description", HeaderText = "Description", Width = 300 });
            dgvProfitLoss.Columns.Add(new DataGridViewTextBoxColumn { Name = "CurrentPeriod", HeaderText = "Current Period", Width = 150, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });
            dgvProfitLoss.Columns.Add(new DataGridViewTextBoxColumn { Name = "PreviousPeriod", HeaderText = "Previous Period", Width = 150, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });
            dgvProfitLoss.Columns.Add(new DataGridViewTextBoxColumn { Name = "Variance", HeaderText = "Variance", Width = 150, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });
            dgvProfitLoss.Columns.Add(new DataGridViewTextBoxColumn { Name = "VariancePercent", HeaderText = "Variance %", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "P2" } });

            tab.Controls.Add(dgvProfitLoss);
        }

        private void CreateDetailedAnalysisTab(TabPage tab)
        {
            dgvDetailedAnalysis = new DataGridView
            {
                Location = new Point(10, 10),
                Size = new Size(1140, 580),
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                BackgroundColor = Color.White,
                GridColor = Color.LightGray
            };

            // Add columns for detailed analysis
            dgvDetailedAnalysis.Columns.Add(new DataGridViewTextBoxColumn { Name = "Category", HeaderText = "Category", Width = 200 });
            dgvDetailedAnalysis.Columns.Add(new DataGridViewTextBoxColumn { Name = "Revenue", HeaderText = "Revenue", Width = 120, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });
            dgvDetailedAnalysis.Columns.Add(new DataGridViewTextBoxColumn { Name = "COGS", HeaderText = "COGS", Width = 120, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });
            dgvDetailedAnalysis.Columns.Add(new DataGridViewTextBoxColumn { Name = "GrossProfit", HeaderText = "Gross Profit", Width = 120, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });
            dgvDetailedAnalysis.Columns.Add(new DataGridViewTextBoxColumn { Name = "GrossMargin", HeaderText = "Gross Margin %", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "P2" } });
            dgvDetailedAnalysis.Columns.Add(new DataGridViewTextBoxColumn { Name = "QuantitySold", HeaderText = "Qty Sold", Width = 80 });
            dgvDetailedAnalysis.Columns.Add(new DataGridViewTextBoxColumn { Name = "AvgSellingPrice", HeaderText = "Avg Price", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });
            dgvDetailedAnalysis.Columns.Add(new DataGridViewTextBoxColumn { Name = "RevenuePercent", HeaderText = "Revenue %", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "P2" } });

            tab.Controls.Add(dgvDetailedAnalysis);
        }

        private void CreateTrendAnalysisTab(TabPage tab)
        {
            dgvTrendAnalysis = new DataGridView
            {
                Location = new Point(10, 10),
                Size = new Size(560, 580),
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                BackgroundColor = Color.White,
                GridColor = Color.LightGray
            };

            // Add columns for trend analysis
            dgvTrendAnalysis.Columns.Add(new DataGridViewTextBoxColumn { Name = "Period", HeaderText = "Period", Width = 100 });
            dgvTrendAnalysis.Columns.Add(new DataGridViewTextBoxColumn { Name = "Revenue", HeaderText = "Revenue", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "C0" } });
            dgvTrendAnalysis.Columns.Add(new DataGridViewTextBoxColumn { Name = "COGS", HeaderText = "COGS", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "C0" } });
            dgvTrendAnalysis.Columns.Add(new DataGridViewTextBoxColumn { Name = "NetProfit", HeaderText = "Net Profit", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "C0" } });
            dgvTrendAnalysis.Columns.Add(new DataGridViewTextBoxColumn { Name = "NetMargin", HeaderText = "Net Margin %", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "P1" } });

            tab.Controls.Add(dgvTrendAnalysis);

            // Create trend chart panel
            chartProfitTrend = new Panel
            {
                Location = new Point(580, 10),
                Size = new Size(560, 580),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Add chart title
            Label chartTitle = new Label
            {
                Text = "Profit Trend Analysis",
                Location = new Point(200, 10),
                Size = new Size(200, 25),
                Font = new Font("Microsoft Sans Serif", 12, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };
            chartProfitTrend.Controls.Add(chartTitle);

            // Add legend
            Panel legendPanel = new Panel
            {
                Location = new Point(10, 40),
                Size = new Size(540, 30),
                BackColor = Color.LightGray
            };

            Label legendRevenue = new Label { Text = "■ Revenue", ForeColor = Color.Green, Location = new Point(10, 5), Size = new Size(80, 20) };
            Label legendCOGS = new Label { Text = "■ COGS", ForeColor = Color.Red, Location = new Point(100, 5), Size = new Size(80, 20) };
            Label legendProfit = new Label { Text = "■ Net Profit", ForeColor = Color.Blue, Location = new Point(190, 5), Size = new Size(80, 20) };

            legendPanel.Controls.Add(legendRevenue);
            legendPanel.Controls.Add(legendCOGS);
            legendPanel.Controls.Add(legendProfit);
            chartProfitTrend.Controls.Add(legendPanel);

            // Add simple bar chart area
            Panel chartArea = new Panel
            {
                Location = new Point(10, 80),
                Size = new Size(540, 480),
                BackColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D
            };
            chartArea.Paint += ChartArea_Paint;
            chartProfitTrend.Controls.Add(chartArea);

            tab.Controls.Add(chartProfitTrend);
        }

        private void CreateCategoryAnalysisTab(TabPage tab)
        {
            // Create category chart panel
            chartCategoryAnalysis = new Panel
            {
                Location = new Point(10, 10),
                Size = new Size(1140, 580),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Add chart title
            Label chartTitle = new Label
            {
                Text = "Category Performance Analysis",
                Location = new Point(450, 10),
                Size = new Size(250, 25),
                Font = new Font("Microsoft Sans Serif", 12, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };
            chartCategoryAnalysis.Controls.Add(chartTitle);

            // Add legend
            Panel legendPanel = new Panel
            {
                Location = new Point(10, 40),
                Size = new Size(1120, 30),
                BackColor = Color.LightGray
            };

            Label legendRevenue = new Label { Text = "■ Revenue", ForeColor = Color.SteelBlue, Location = new Point(10, 5), Size = new Size(100, 20) };
            Label legendProfit = new Label { Text = "■ Gross Profit", ForeColor = Color.Orange, Location = new Point(120, 5), Size = new Size(100, 20) };

            legendPanel.Controls.Add(legendRevenue);
            legendPanel.Controls.Add(legendProfit);
            chartCategoryAnalysis.Controls.Add(legendPanel);

            // Add category summary data grid
            DataGridView dgvCategoryChart = new DataGridView
            {
                Location = new Point(10, 80),
                Size = new Size(1120, 480),
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                BackgroundColor = Color.White,
                GridColor = Color.LightGray
            };

            // Add columns for category chart data
            dgvCategoryChart.Columns.Add(new DataGridViewTextBoxColumn { Name = "Category", HeaderText = "Category", Width = 200 });
            dgvCategoryChart.Columns.Add(new DataGridViewTextBoxColumn { Name = "Revenue", HeaderText = "Revenue", Width = 150, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });
            dgvCategoryChart.Columns.Add(new DataGridViewTextBoxColumn { Name = "GrossProfit", HeaderText = "Gross Profit", Width = 150, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });
            dgvCategoryChart.Columns.Add(new DataGridViewTextBoxColumn { Name = "GrossMargin", HeaderText = "Gross Margin %", Width = 120, DefaultCellStyle = new DataGridViewCellStyle { Format = "P2" } });
            dgvCategoryChart.Columns.Add(new DataGridViewTextBoxColumn { Name = "RevenueBar", HeaderText = "Revenue Visual", Width = 200 });
            dgvCategoryChart.Columns.Add(new DataGridViewTextBoxColumn { Name = "ProfitBar", HeaderText = "Profit Visual", Width = 200 });

            dgvCategoryChart.Name = "dgvCategoryChart";
            chartCategoryAnalysis.Controls.Add(dgvCategoryChart);

            tab.Controls.Add(chartCategoryAnalysis);
        }

        private void GenerateEnhancedProfitAndLoss()
        {
            try
            {
                DateTime fromDate = dateTimePicker1.Value.Date;
                DateTime toDate = dateTimePicker2.Value.Date.AddDays(1).AddSeconds(-1);

                // Generate comprehensive analysis
                LoadSummaryData(fromDate, toDate);
                LoadDetailedAnalysis(fromDate, toDate);
                LoadTrendAnalysis(fromDate, toDate);
                LoadCategoryAnalysis(fromDate, toDate);
                
                MessageBox.Show("Enhanced Profit & Loss analysis generated successfully!", "Analysis Complete", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating enhanced analysis: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSummaryData(DateTime fromDate, DateTime toDate)
        {
            try
            {
                // Calculate comprehensive financial metrics
                decimal salesRevenue = CalculateEnhancedSalesRevenue(fromDate, toDate);
                decimal costOfGoodsSold = CalculateEnhancedCOGS(fromDate, toDate);
                decimal grossProfit = salesRevenue - costOfGoodsSold;
                decimal operatingExpenses = CalculateEnhancedOperatingExpenses(fromDate, toDate);
                decimal netProfit = grossProfit - operatingExpenses;

                // Calculate previous period for comparison
                DateTime prevFromDate = fromDate.AddMonths(-1);
                DateTime prevToDate = fromDate.AddDays(-1);
                decimal prevSalesRevenue = CalculateEnhancedSalesRevenue(prevFromDate, prevToDate);
                decimal prevCOGS = CalculateEnhancedCOGS(prevFromDate, prevToDate);
                decimal prevGrossProfit = prevSalesRevenue - prevCOGS;
                decimal prevOperatingExp = CalculateEnhancedOperatingExpenses(prevFromDate, prevToDate);
                decimal prevNetProfit = prevGrossProfit - prevOperatingExp;

                // Update summary labels
                lblTotalRevenue.Text = $"Total Revenue: {salesRevenue:C2}";
                lblTotalCOGS.Text = $"Total COGS: {costOfGoodsSold:C2}";
                lblGrossProfit.Text = $"Gross Profit: {grossProfit:C2}";
                lblOperatingExp.Text = $"Operating Exp: {operatingExpenses:C2}";
                lblNetProfit.Text = $"Net Profit: {netProfit:C2}";

                // Calculate and update margins
                decimal grossMargin = salesRevenue != 0 ? (grossProfit / salesRevenue) : 0;
                decimal netMargin = salesRevenue != 0 ? (netProfit / salesRevenue) : 0;
                decimal roi = costOfGoodsSold != 0 ? (netProfit / costOfGoodsSold) : 0;

                lblGrossProfitMargin.Text = $"Gross Margin: {grossMargin:P1}";
                lblNetProfitMargin.Text = $"Net Margin: {netMargin:P1}";
                lblROI.Text = $"ROI: {roi:P1}";

                // Update P&L DataGridView with variance analysis
                UpdateProfitLossGrid(salesRevenue, costOfGoodsSold, grossProfit, operatingExpenses, netProfit,
                                   prevSalesRevenue, prevCOGS, prevGrossProfit, prevOperatingExp, prevNetProfit);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading summary data: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateProfitLossGrid(decimal revenue, decimal cogs, decimal grossProfit, decimal opExp, decimal netProfit,
                                        decimal prevRevenue, decimal prevCOGS, decimal prevGrossProfit, decimal prevOpExp, decimal prevNetProfit)
        {
            dgvProfitLoss.Rows.Clear();

            // Revenue row
            decimal revenueVariance = revenue - prevRevenue;
            decimal revenueVariancePercent = prevRevenue != 0 ? revenueVariance / prevRevenue : 0;
            dgvProfitLoss.Rows.Add("Sales Revenue", revenue, prevRevenue, revenueVariance, revenueVariancePercent);

            // COGS row
            decimal cogsVariance = cogs - prevCOGS;
            decimal cogsVariancePercent = prevCOGS != 0 ? cogsVariance / prevCOGS : 0;
            dgvProfitLoss.Rows.Add("Cost of Goods Sold", -cogs, -prevCOGS, -cogsVariance, cogsVariancePercent);

            // Gross Profit row
            decimal grossVariance = grossProfit - prevGrossProfit;
            decimal grossVariancePercent = prevGrossProfit != 0 ? grossVariance / prevGrossProfit : 0;
            dgvProfitLoss.Rows.Add("Gross Profit", grossProfit, prevGrossProfit, grossVariance, grossVariancePercent);

            // Operating Expenses row
            decimal opExpVariance = opExp - prevOpExp;
            decimal opExpVariancePercent = prevOpExp != 0 ? opExpVariance / prevOpExp : 0;
            dgvProfitLoss.Rows.Add("Operating Expenses", -opExp, -prevOpExp, -opExpVariance, opExpVariancePercent);

            // Net Profit row
            decimal netVariance = netProfit - prevNetProfit;
            decimal netVariancePercent = prevNetProfit != 0 ? netVariance / prevNetProfit : 0;
            dgvProfitLoss.Rows.Add("Net Profit", netProfit, prevNetProfit, netVariance, netVariancePercent);

            // Style the grid
            foreach (DataGridViewRow row in dgvProfitLoss.Rows)
            {
                if (row.Cells["Description"].Value?.ToString().Contains("Profit") == true)
                {
                    row.DefaultCellStyle.BackColor = Color.LightGreen;
                    row.DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 9, FontStyle.Bold);
                }
                else if (row.Cells["Description"].Value?.ToString().Contains("Cost") == true ||
                        row.Cells["Description"].Value?.ToString().Contains("Expenses") == true)
                {
                    row.DefaultCellStyle.BackColor = Color.LightCoral;
                }
            }
        }

        private void GenerateProfitAndLoss()
        {
            try
            {
                DateTime fromDate = dateTimePicker1.Value.Date;
                DateTime toDate = dateTimePicker2.Value.Date.AddDays(1).AddSeconds(-1);

                // Calculate Sales Revenue
                decimal salesRevenue = CalculateSalesRevenue(fromDate, toDate);

                // Calculate Cost of Goods Sold
                decimal costOfGoodsSold = CalculateCostOfGoodsSold(fromDate, toDate);

                // Calculate Gross Profit
                decimal grossProfit = salesRevenue - costOfGoodsSold;

                // Calculate Operating Expenses (simplified)
                decimal operatingExpenses = CalculateOperatingExpenses(fromDate, toDate);

                // Calculate Net Profit
                decimal netProfit = grossProfit - operatingExpenses;

                // Display results
                DisplayResults(salesRevenue, costOfGoodsSold, grossProfit, operatingExpenses, netProfit);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating Profit & Loss: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private decimal CalculateSalesRevenue(DateTime fromDate, DateTime toDate)
        {
            string query = @"SELECT ISNULL(SUM(NetAmount), 0) 
                           FROM Sales 
                           WHERE SaleDate BETWEEN @FromDate AND @ToDate 
                           AND IsActive = 1";

            SqlParameter[] parameters = {
                new SqlParameter("@FromDate", fromDate),
                new SqlParameter("@ToDate", toDate)
            };

            object result = DatabaseConnection.ExecuteScalar(query, parameters);
            return Convert.ToDecimal(result);
        }

        private decimal CalculateCostOfGoodsSold(DateTime fromDate, DateTime toDate)
        {
            // This is a simplified calculation
            // In a real system, you would track actual cost of goods sold
            string query = @"SELECT ISNULL(SUM(si.Quantity * i.Price * 0.7), 0) 
                           FROM Sales s
                           INNER JOIN SaleItems si ON s.SaleID = si.SaleID
                           INNER JOIN Items i ON si.ItemID = i.ItemID
                           WHERE s.SaleDate BETWEEN @FromDate AND @ToDate 
                           AND s.IsActive = 1";

            SqlParameter[] parameters = {
                new SqlParameter("@FromDate", fromDate),
                new SqlParameter("@ToDate", toDate)
            };

            object result = DatabaseConnection.ExecuteScalar(query, parameters);
            return Convert.ToDecimal(result);
        }

        private decimal CalculateOperatingExpenses(DateTime fromDate, DateTime toDate)
        {
            // This is a simplified calculation
            // In a real system, you would have an expenses table
            // For now, we'll use a percentage of sales as operating expenses
            decimal salesRevenue = CalculateSalesRevenue(fromDate, toDate);
            return salesRevenue * 0.15m; // 15% of sales as operating expenses
        }

        private void DisplayResults(decimal salesRevenue, decimal costOfGoodsSold, decimal grossProfit, decimal operatingExpenses, decimal netProfit)
        {
            // Update labels with results
            // You can add labels to display these values
            string report = $"Profit & Loss Statement\n" +
                          $"Period: {dateTimePicker1.Value:dd/MM/yyyy} - {dateTimePicker2.Value:dd/MM/yyyy}\n\n" +
                          $"Sales Revenue: ${salesRevenue:N2}\n" +
                          $"Cost of Goods Sold: ${costOfGoodsSold:N2}\n" +
                          $"Gross Profit: ${grossProfit:N2}\n" +
                          $"Operating Expenses: ${operatingExpenses:N2}\n" +
                          $"Net Profit: ${netProfit:N2}\n\n" +
                          $"Gross Profit Margin: {(grossProfit > 0 ? (grossProfit / salesRevenue * 100) : 0):N1}%\n" +
                          $"Net Profit Margin: {(netProfit > 0 ? (netProfit / salesRevenue * 100) : 0):N1}%";

            MessageBox.Show(report, "Profit & Loss Statement", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                PrintDocument pd = new PrintDocument();
                pd.PrintPage += PrintProfitAndLoss;
                pd.Print();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error printing: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PrintProfitAndLoss(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Font titleFont = new Font("Arial", 16, FontStyle.Bold);
            Font headerFont = new Font("Arial", 12, FontStyle.Bold);
            Font dataFont = new Font("Arial", 10);

            int yPos = 50;
            g.DrawString("Profit & Loss Statement", titleFont, Brushes.Black, 50, yPos);
            yPos += 30;
            g.DrawString($"Period: {dateTimePicker1.Value:dd/MM/yyyy} - {dateTimePicker2.Value:dd/MM/yyyy}", dataFont, Brushes.Black, 50, yPos);
            yPos += 20;
            g.DrawString($"Generated on: {DateTime.Now:dd/MM/yyyy HH:mm}", dataFont, Brushes.Black, 50, yPos);
            yPos += 40;

            // Calculate values for printing
            DateTime fromDate = dateTimePicker1.Value.Date;
            DateTime toDate = dateTimePicker2.Value.Date.AddDays(1).AddSeconds(-1);
            decimal salesRevenue = CalculateSalesRevenue(fromDate, toDate);
            decimal costOfGoodsSold = CalculateCostOfGoodsSold(fromDate, toDate);
            decimal grossProfit = salesRevenue - costOfGoodsSold;
            decimal operatingExpenses = CalculateOperatingExpenses(fromDate, toDate);
            decimal netProfit = grossProfit - operatingExpenses;

            // Print statement
            g.DrawString("Sales Revenue", headerFont, Brushes.Black, 50, yPos);
            g.DrawString(salesRevenue.ToString("N2"), dataFont, Brushes.Black, 300, yPos);
            yPos += 25;

            g.DrawString("Cost of Goods Sold", headerFont, Brushes.Black, 50, yPos);
            g.DrawString(costOfGoodsSold.ToString("N2"), dataFont, Brushes.Black, 300, yPos);
            yPos += 25;

            g.DrawString("Gross Profit", headerFont, Brushes.Black, 50, yPos);
            g.DrawString(grossProfit.ToString("N2"), dataFont, Brushes.Black, 300, yPos);
            yPos += 30;

            g.DrawString("Operating Expenses", headerFont, Brushes.Black, 50, yPos);
            g.DrawString(operatingExpenses.ToString("N2"), dataFont, Brushes.Black, 300, yPos);
            yPos += 30;

            g.DrawString("Net Profit", headerFont, Brushes.Black, 50, yPos);
            g.DrawString(netProfit.ToString("N2"), dataFont, Brushes.Black, 300, yPos);
            yPos += 40;

            // Print margins
            g.DrawString($"Gross Profit Margin: {(grossProfit > 0 ? (grossProfit / salesRevenue * 100) : 0):N1}%", dataFont, Brushes.Black, 50, yPos);
            yPos += 20;
            g.DrawString($"Net Profit Margin: {(netProfit > 0 ? (netProfit / salesRevenue * 100) : 0):N1}%", dataFont, Brushes.Black, 50, yPos);
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                saveDialog.FileName = $"ProfitAndLoss_{DateTime.Now:yyyyMMdd}.csv";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    ExportToCSV(saveDialog.FileName);
                    MessageBox.Show("Profit & Loss report exported successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exporting: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportToCSV(string fileName)
        {
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fileName))
            {
                DateTime fromDate = dateTimePicker1.Value.Date;
                DateTime toDate = dateTimePicker2.Value.Date.AddDays(1).AddSeconds(-1);
                decimal salesRevenue = CalculateSalesRevenue(fromDate, toDate);
                decimal costOfGoodsSold = CalculateCostOfGoodsSold(fromDate, toDate);
                decimal grossProfit = salesRevenue - costOfGoodsSold;
                decimal operatingExpenses = CalculateOperatingExpenses(fromDate, toDate);
                decimal netProfit = grossProfit - operatingExpenses;

                sw.WriteLine("Profit & Loss Statement");
                sw.WriteLine($"Period: {dateTimePicker1.Value:dd/MM/yyyy} - {dateTimePicker2.Value:dd/MM/yyyy}");
                sw.WriteLine($"Generated on: {DateTime.Now:dd/MM/yyyy HH:mm}");
                sw.WriteLine();
                sw.WriteLine("Item,Amount");
                sw.WriteLine($"Sales Revenue,{salesRevenue:N2}");
                sw.WriteLine($"Cost of Goods Sold,{costOfGoodsSold:N2}");
                sw.WriteLine($"Gross Profit,{grossProfit:N2}");
                sw.WriteLine($"Operating Expenses,{operatingExpenses:N2}");
                sw.WriteLine($"Net Profit,{netProfit:N2}");
                sw.WriteLine();
                sw.WriteLine($"Gross Profit Margin,{(grossProfit > 0 ? (grossProfit / salesRevenue * 100) : 0):N1}%");
                sw.WriteLine($"Net Profit Margin,{(netProfit > 0 ? (netProfit / salesRevenue * 100) : 0):N1}%");
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        // Enhanced Calculation Methods
        private decimal CalculateEnhancedSalesRevenue(DateTime fromDate, DateTime toDate)
        {
            try
            {
                string query = @"SELECT ISNULL(SUM(NetAmount), 0) 
                               FROM Sales 
                               WHERE SaleDate BETWEEN @FromDate AND @ToDate 
                               AND IsActive = 1";

                SqlParameter[] parameters = {
                    new SqlParameter("@FromDate", fromDate),
                    new SqlParameter("@ToDate", toDate)
                };

                object result = DatabaseConnection.ExecuteScalar(query, parameters);
                return Convert.ToDecimal(result);
            }
            catch
            {
                return 0;
            }
        }

        private decimal CalculateEnhancedCOGS(DateTime fromDate, DateTime toDate)
        {
            try
            {
                // More accurate COGS calculation using purchase prices
                string query = @"SELECT ISNULL(SUM(si.Quantity * ISNULL(i.PurchasePrice, i.Price * 0.7)), 0) 
                               FROM Sales s
                               INNER JOIN SaleItems si ON s.SaleID = si.SaleID
                               INNER JOIN Items i ON si.ItemID = i.ItemID
                               WHERE s.SaleDate BETWEEN @FromDate AND @ToDate 
                               AND s.IsActive = 1";

                SqlParameter[] parameters = {
                    new SqlParameter("@FromDate", fromDate),
                    new SqlParameter("@ToDate", toDate)
                };

                object result = DatabaseConnection.ExecuteScalar(query, parameters);
                return Convert.ToDecimal(result);
            }
            catch
            {
                return 0;
            }
        }

        private decimal CalculateEnhancedOperatingExpenses(DateTime fromDate, DateTime toDate)
        {
            try
            {
                // Try to get actual expenses if Expenses table exists
                string query = @"SELECT ISNULL(SUM(Amount), 0) FROM Expenses 
                               WHERE ExpenseDate BETWEEN @FromDate AND @ToDate 
                               AND IsActive = 1";

                SqlParameter[] parameters = {
                    new SqlParameter("@FromDate", fromDate),
                    new SqlParameter("@ToDate", toDate)
                };

                decimal actualExpenses = 0;
                try
                {
                    object result = DatabaseConnection.ExecuteScalar(query, parameters);
                    actualExpenses = Convert.ToDecimal(result);
                }
                catch
                {
                    // If no expenses table, estimate based on revenue
                    decimal revenue = CalculateEnhancedSalesRevenue(fromDate, toDate);
                    actualExpenses = revenue * 0.15m; // 15% estimate
                }

                return actualExpenses;
            }
            catch
            {
                return 0;
            }
        }

        private void LoadDetailedAnalysis(DateTime fromDate, DateTime toDate)
        {
            try
            {
                string query = @"SELECT 
                                   i.Category,
                                   SUM(si.Quantity * si.Price) as Revenue,
                                   SUM(si.Quantity * ISNULL(i.PurchasePrice, i.Price * 0.7)) as COGS,
                                   SUM(si.Quantity * si.Price) - SUM(si.Quantity * ISNULL(i.PurchasePrice, i.Price * 0.7)) as GrossProfit,
                                   SUM(si.Quantity) as QuantitySold,
                                   AVG(si.Price) as AvgSellingPrice
                               FROM Sales s
                               INNER JOIN SaleItems si ON s.SaleID = si.SaleID
                               INNER JOIN Items i ON si.ItemID = i.ItemID
                               WHERE s.SaleDate BETWEEN @FromDate AND @ToDate 
                               AND s.IsActive = 1
                               GROUP BY i.Category
                               ORDER BY SUM(si.Quantity * si.Price) DESC";

                SqlParameter[] parameters = {
                    new SqlParameter("@FromDate", fromDate),
                    new SqlParameter("@ToDate", toDate)
                };

                DataTable categoryData = DatabaseConnection.ExecuteQuery(query, parameters);
                decimal totalRevenue = categoryData.AsEnumerable().Sum(r => r.Field<decimal>("Revenue"));

                dgvDetailedAnalysis.Rows.Clear();
                foreach (DataRow row in categoryData.Rows)
                {
                    decimal revenue = Convert.ToDecimal(row["Revenue"]);
                    decimal cogs = Convert.ToDecimal(row["COGS"]);
                    decimal grossProfit = Convert.ToDecimal(row["GrossProfit"]);
                    decimal grossMargin = revenue != 0 ? grossProfit / revenue : 0;
                    decimal revenuePercent = totalRevenue != 0 ? revenue / totalRevenue : 0;

                    dgvDetailedAnalysis.Rows.Add(
                        row["Category"],
                        revenue,
                        cogs,
                        grossProfit,
                        grossMargin,
                        row["QuantitySold"],
                        row["AvgSellingPrice"],
                        revenuePercent
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading detailed analysis: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadTrendAnalysis(DateTime fromDate, DateTime toDate)
        {
            try
            {
                dgvTrendAnalysis.Rows.Clear();

                // Generate 12 months of trend data
                for (int i = 11; i >= 0; i--)
                {
                    DateTime periodStart = fromDate.AddMonths(-i);
                    DateTime periodEnd = periodStart.AddMonths(1).AddDays(-1);

                    decimal revenue = CalculateEnhancedSalesRevenue(periodStart, periodEnd);
                    decimal cogs = CalculateEnhancedCOGS(periodStart, periodEnd);
                    decimal opExp = CalculateEnhancedOperatingExpenses(periodStart, periodEnd);
                    decimal netProfit = revenue - cogs - opExp;
                    decimal netMargin = revenue != 0 ? netProfit / revenue : 0;

                    string periodLabel = periodStart.ToString("MMM yyyy");
                    dgvTrendAnalysis.Rows.Add(periodLabel, revenue, cogs, netProfit, netMargin);
                }

                // Trigger chart refresh
                Panel chartArea = chartProfitTrend.Controls.OfType<Panel>().FirstOrDefault(p => p.BackColor == Color.White && p.BorderStyle == BorderStyle.Fixed3D);
                if (chartArea != null)
                {
                    chartArea.Invalidate();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading trend analysis: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadCategoryAnalysis(DateTime fromDate, DateTime toDate)
        {
            try
            {
                string query = @"SELECT 
                                   i.Category,
                                   SUM(si.Quantity * si.Price) as Revenue,
                                   SUM(si.Quantity * si.Price) - SUM(si.Quantity * ISNULL(i.PurchasePrice, i.Price * 0.7)) as GrossProfit
                               FROM Sales s
                               INNER JOIN SaleItems si ON s.SaleID = si.SaleID
                               INNER JOIN Items i ON si.ItemID = i.ItemID
                               WHERE s.SaleDate BETWEEN @FromDate AND @ToDate 
                               AND s.IsActive = 1
                               GROUP BY i.Category
                               ORDER BY SUM(si.Quantity * si.Price) DESC";

                SqlParameter[] parameters = {
                    new SqlParameter("@FromDate", fromDate),
                    new SqlParameter("@ToDate", toDate)
                };

                DataTable categoryData = DatabaseConnection.ExecuteQuery(query, parameters);

                // Find the category chart data grid
                DataGridView dgvCategoryChart = chartCategoryAnalysis.Controls.OfType<DataGridView>().FirstOrDefault(d => d.Name == "dgvCategoryChart");
                if (dgvCategoryChart != null)
                {
                    dgvCategoryChart.Rows.Clear();

                    // Find max values for scaling visual bars
                    decimal maxRevenue = categoryData.AsEnumerable().DefaultIfEmpty().Max(r => r?.Field<decimal>("Revenue") ?? 0);

                    foreach (DataRow row in categoryData.Rows)
                    {
                        string category = row["Category"].ToString();
                        decimal revenue = Convert.ToDecimal(row["Revenue"]);
                        decimal grossProfit = Convert.ToDecimal(row["GrossProfit"]);
                        decimal grossMargin = revenue != 0 ? grossProfit / revenue : 0;

                        // Create visual bars using text characters
                        int revenueBarLength = maxRevenue != 0 ? (int)((revenue / maxRevenue) * 20) : 0;
                        int profitBarLength = maxRevenue != 0 ? (int)((grossProfit / maxRevenue) * 20) : 0;
                        string revenueBar = new string('█', revenueBarLength) + new string('░', 20 - revenueBarLength);
                        string profitBar = new string('█', profitBarLength) + new string('░', 20 - profitBarLength);

                        dgvCategoryChart.Rows.Add(category, revenue, grossProfit, grossMargin, revenueBar, profitBar);
                    }

                    // Color code the visual bars
                    foreach (DataGridViewRow row in dgvCategoryChart.Rows)
                    {
                        row.Cells["RevenueBar"].Style.ForeColor = Color.SteelBlue;
                        row.Cells["ProfitBar"].Style.ForeColor = Color.Orange;
                        row.Cells["RevenueBar"].Style.Font = new Font("Consolas", 8);
                        row.Cells["ProfitBar"].Style.Font = new Font("Consolas", 8);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading category analysis: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Custom chart drawing for trend analysis
        private void ChartArea_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                if (dgvTrendAnalysis == null || dgvTrendAnalysis.Rows.Count == 0)
                    return;

                Graphics g = e.Graphics;
                Panel panel = sender as Panel;
                int width = panel.Width - 20;
                int height = panel.Height - 40;
                int startX = 10;
                int startY = 10;

                // Draw axes
                g.DrawLine(Pens.Black, startX, startY + height, startX + width, startY + height); // X-axis
                g.DrawLine(Pens.Black, startX, startY, startX, startY + height); // Y-axis

                // Find max values for scaling
                decimal maxValue = 0;
                for (int i = 0; i < dgvTrendAnalysis.Rows.Count; i++)
                {
                    decimal revenue = Convert.ToDecimal(dgvTrendAnalysis.Rows[i].Cells["Revenue"].Value ?? 0);
                    decimal cogs = Convert.ToDecimal(dgvTrendAnalysis.Rows[i].Cells["COGS"].Value ?? 0);
                    decimal netProfit = Convert.ToDecimal(dgvTrendAnalysis.Rows[i].Cells["NetProfit"].Value ?? 0);
                    maxValue = Math.Max(maxValue, Math.Max(revenue, Math.Max(cogs, netProfit)));
                }

                if (maxValue == 0) return;

                // Draw data points and lines
                Point[] revenuePoints = new Point[dgvTrendAnalysis.Rows.Count];
                Point[] cogsPoints = new Point[dgvTrendAnalysis.Rows.Count];
                Point[] profitPoints = new Point[dgvTrendAnalysis.Rows.Count];

                for (int i = 0; i < dgvTrendAnalysis.Rows.Count; i++)
                {
                    decimal revenue = Convert.ToDecimal(dgvTrendAnalysis.Rows[i].Cells["Revenue"].Value ?? 0);
                    decimal cogs = Convert.ToDecimal(dgvTrendAnalysis.Rows[i].Cells["COGS"].Value ?? 0);
                    decimal netProfit = Convert.ToDecimal(dgvTrendAnalysis.Rows[i].Cells["NetProfit"].Value ?? 0);

                    int x = startX + (i * width / Math.Max(dgvTrendAnalysis.Rows.Count - 1, 1));
                    int yRevenue = startY + height - (int)((double)(revenue / maxValue) * height);
                    int yCogs = startY + height - (int)((double)(cogs / maxValue) * height);
                    int yProfit = startY + height - (int)((double)(netProfit / maxValue) * height);

                    revenuePoints[i] = new Point(x, yRevenue);
                    cogsPoints[i] = new Point(x, yCogs);
                    profitPoints[i] = new Point(x, yProfit);

                    // Draw points
                    g.FillEllipse(Brushes.Green, x - 2, yRevenue - 2, 4, 4);
                    g.FillEllipse(Brushes.Red, x - 2, yCogs - 2, 4, 4);
                    g.FillEllipse(Brushes.Blue, x - 2, yProfit - 2, 4, 4);
                }

                // Draw lines
                if (revenuePoints.Length > 1)
                {
                    g.DrawLines(new Pen(Color.Green, 2), revenuePoints);
                    g.DrawLines(new Pen(Color.Red, 2), cogsPoints);
                    g.DrawLines(new Pen(Color.Blue, 2), profitPoints);
                }
            }
            catch (Exception ex)
            {
                // Handle painting errors silently
            }
        }
    }
}
