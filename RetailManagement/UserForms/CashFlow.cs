using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Drawing.Printing;
using RetailManagement.Database;

namespace RetailManagement.UserForms
{
    public partial class CashFlow : Form
    {
        private DataTable cashFlowData;

        public CashFlow()
        {
            InitializeComponent();
            SetupDataGridView();
            LoadCashFlow();
        }

        private void SetupDataGridView()
        {
            dgvCashFlow.Columns.Clear();
            
            dgvCashFlow.Columns.Add("TransactionDate", "Date");
            dgvCashFlow.Columns.Add("Description", "Description");
            dgvCashFlow.Columns.Add("TransactionType", "Type");
            dgvCashFlow.Columns.Add("CashIn", "Cash In");
            dgvCashFlow.Columns.Add("CashOut", "Cash Out");
            dgvCashFlow.Columns.Add("RunningBalance", "Running Balance");

            // Configure columns
            dgvCashFlow.Columns["TransactionDate"].Width = 100;
            dgvCashFlow.Columns["Description"].Width = 250;
            dgvCashFlow.Columns["TransactionType"].Width = 100;
            dgvCashFlow.Columns["CashIn"].Width = 120;
            dgvCashFlow.Columns["CashOut"].Width = 120;
            dgvCashFlow.Columns["RunningBalance"].Width = 130;

            // Set alignment
            dgvCashFlow.Columns["TransactionDate"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvCashFlow.Columns["CashIn"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvCashFlow.Columns["CashOut"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvCashFlow.Columns["RunningBalance"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            // Set header alignment
            dgvCashFlow.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvCashFlow.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
        }

        private void LoadCashFlow()
        {
            try
            {
                DateTime fromDate = dtpFromDate.Value;
                DateTime toDate = dtpToDate.Value;

                // Build comprehensive cash flow query
                string query = @"
                    WITH CashTransactions AS (
                        -- Cash Sales
                        SELECT 
                            SaleDate as TransactionDate,
                            'Cash Sale' as Description,
                            'Cash In' as TransactionType,
                            TotalAmount as CashIn,
                            0 as CashOut
                        FROM Sales 
                        WHERE SaleDate BETWEEN @FromDate AND @ToDate 
                        AND IsActive = 1 
                        AND PaymentMethod = 'Cash'
                        
                        UNION ALL
                        
                        -- Credit Sales (when payment received)
                        SELECT 
                            PaymentDate as TransactionDate,
                            'Customer Payment' as Description,
                            'Cash In' as TransactionType,
                            Amount as CashIn,
                            0 as CashOut
                        FROM CustomerPayments 
                        WHERE PaymentDate BETWEEN @FromDate AND @ToDate 
                        AND IsActive = 1
                        
                        UNION ALL
                        
                        -- Cash Purchases
                        SELECT 
                            PurchaseDate as TransactionDate,
                            'Cash Purchase' as Description,
                            'Cash Out' as TransactionType,
                            0 as CashIn,
                            TotalAmount as CashOut
                        FROM Purchases 
                        WHERE PurchaseDate BETWEEN @FromDate AND @ToDate 
                        AND IsActive = 1 
                        AND PaymentMethod = 'Cash'
                        
                        UNION ALL
                        
                        -- Purchase Returns (Cash received back)
                        SELECT 
                            ReturnDate as TransactionDate,
                            'Purchase Return' as Description,
                            'Cash In' as TransactionType,
                            TotalAmount as CashIn,
                            0 as CashOut
                        FROM PurchaseReturns 
                        WHERE ReturnDate BETWEEN @FromDate AND @ToDate 
                        AND IsActive = 1
                        
                        UNION ALL
                        
                        -- Sale Returns (Cash paid back)
                        SELECT 
                            ReturnDate as TransactionDate,
                            'Sale Return' as Description,
                            'Cash Out' as TransactionType,
                            0 as CashIn,
                            TotalAmount as CashOut
                        FROM SaleReturns 
                        WHERE ReturnDate BETWEEN @FromDate AND @ToDate 
                        AND IsActive = 1
                        
                        UNION ALL
                        
                        -- Expenses
                        SELECT 
                            ExpenseDate as TransactionDate,
                            'Expense: ' + Category + ' - ' + Description as Description,
                            'Cash Out' as TransactionType,
                            0 as CashIn,
                            Amount as CashOut
                        FROM Expenses 
                        WHERE ExpenseDate BETWEEN @FromDate AND @ToDate 
                        AND IsActive = 1
                        AND PaymentMethod = 'Cash'
                        
                        UNION ALL
                        
                        -- Bank Deposits (Cash going out to bank)
                        SELECT 
                            TransactionDate as TransactionDate,
                            'Bank Deposit: ' + Description as Description,
                            'Cash Out' as TransactionType,
                            0 as CashIn,
                            Amount as CashOut
                        FROM BankTransactions 
                        WHERE TransactionDate BETWEEN @FromDate AND @ToDate 
                        AND TransactionType = 'Deposit' 
                        AND IsActive = 1
                        
                        UNION ALL
                        
                        -- Bank Withdrawals (Cash coming in from bank)
                        SELECT 
                            TransactionDate as TransactionDate,
                            'Bank Withdrawal: ' + Description as Description,
                            'Cash In' as TransactionType,
                            Amount as CashIn,
                            0 as CashOut
                        FROM BankTransactions 
                        WHERE TransactionDate BETWEEN @FromDate AND @ToDate 
                        AND TransactionType = 'Withdrawal' 
                        AND IsActive = 1
                    )
                    SELECT 
                        TransactionDate,
                        Description,
                        TransactionType,
                        CashIn,
                        CashOut,
                        0 as RunningBalance
                    FROM CashTransactions
                    ORDER BY TransactionDate, TransactionType";

                SqlParameter[] parameters = {
                    new SqlParameter("@FromDate", fromDate),
                    new SqlParameter("@ToDate", toDate)
                };

                cashFlowData = DatabaseConnection.ExecuteQuery(query, parameters);
                CalculateRunningBalance();
                ShowCashFlowData();
                CalculateTotals();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading cash flow: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CalculateRunningBalance()
        {
            if (cashFlowData != null && cashFlowData.Rows.Count > 0)
            {
                decimal runningBalance = 0;
                
                foreach (DataRow row in cashFlowData.Rows)
                {
                    decimal cashIn = Convert.ToDecimal(row["CashIn"]);
                    decimal cashOut = Convert.ToDecimal(row["CashOut"]);
                    
                    runningBalance += cashIn - cashOut;
                    row["RunningBalance"] = runningBalance;
                }
            }
        }

        private void ShowCashFlowData()
        {
            dgvCashFlow.Rows.Clear();

            if (cashFlowData != null && cashFlowData.Rows.Count > 0)
            {
                foreach (DataRow row in cashFlowData.Rows)
                {
                    int rowIndex = dgvCashFlow.Rows.Add();
                    dgvCashFlow.Rows[rowIndex].Cells["TransactionDate"].Value = Convert.ToDateTime(row["TransactionDate"]).ToString("dd/MM/yyyy");
                    dgvCashFlow.Rows[rowIndex].Cells["Description"].Value = row["Description"];
                    dgvCashFlow.Rows[rowIndex].Cells["TransactionType"].Value = row["TransactionType"];
                    dgvCashFlow.Rows[rowIndex].Cells["CashIn"].Value = Convert.ToDecimal(row["CashIn"]).ToString("N2");
                    dgvCashFlow.Rows[rowIndex].Cells["CashOut"].Value = Convert.ToDecimal(row["CashOut"]).ToString("N2");
                    dgvCashFlow.Rows[rowIndex].Cells["RunningBalance"].Value = Convert.ToDecimal(row["RunningBalance"]).ToString("N2");

                    // Color coding based on transaction type
                    if (row["TransactionType"].ToString() == "Cash In")
                    {
                        dgvCashFlow.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightGreen;
                        dgvCashFlow.Rows[rowIndex].Cells["CashIn"].Style.ForeColor = Color.Green;
                    }
                    else if (row["TransactionType"].ToString() == "Cash Out")
                    {
                        dgvCashFlow.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightCoral;
                        dgvCashFlow.Rows[rowIndex].Cells["CashOut"].Style.ForeColor = Color.Red;
                    }

                    // Color code running balance
                    decimal runningBalance = Convert.ToDecimal(row["RunningBalance"]);
                    if (runningBalance >= 0)
                    {
                        dgvCashFlow.Rows[rowIndex].Cells["RunningBalance"].Style.ForeColor = Color.Green;
                    }
                    else
                    {
                        dgvCashFlow.Rows[rowIndex].Cells["RunningBalance"].Style.ForeColor = Color.Red;
                    }
                }
            }
        }

        private void CalculateTotals()
        {
            decimal totalCashIn = 0;
            decimal totalCashOut = 0;
            decimal finalBalance = 0;

            if (cashFlowData != null && cashFlowData.Rows.Count > 0)
            {
                foreach (DataRow row in cashFlowData.Rows)
                {
                    totalCashIn += Convert.ToDecimal(row["CashIn"]);
                    totalCashOut += Convert.ToDecimal(row["CashOut"]);
                }
                finalBalance = totalCashIn - totalCashOut;
            }

            lblTotalCashIn.Text = "Total Cash In: " + totalCashIn.ToString("N2");
            lblTotalCashOut.Text = "Total Cash Out: " + totalCashOut.ToString("N2");
            lblFinalBalance.Text = "Final Balance: " + finalBalance.ToString("N2");

            // Color code final balance
            if (finalBalance >= 0)
            {
                lblFinalBalance.ForeColor = Color.Green;
            }
            else
            {
                lblFinalBalance.ForeColor = Color.Red;
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadCashFlow();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                PrintDialog printDialog = new PrintDialog();
                PrintDocument printDocument = new PrintDocument();
                printDocument.PrintPage += PrintCashFlow;
                printDialog.Document = printDocument;

                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    printDocument.Print();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error printing: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PrintCashFlow(object sender, PrintPageEventArgs e)
        {
            try
            {
                Graphics g = e.Graphics;
                Font titleFont = new Font("Arial", 16, FontStyle.Bold);
                Font headerFont = new Font("Arial", 10, FontStyle.Bold);
                Font normalFont = new Font("Arial", 9);
                Brush brush = Brushes.Black;

                int yPos = 50;
                int leftMargin = 50;

                // Print title
                g.DrawString("CASH FLOW STATEMENT", titleFont, brush, leftMargin, yPos);
                yPos += 30;

                // Print date range
                g.DrawString($"Period: {dtpFromDate.Value.ToString("dd/MM/yyyy")} to {dtpToDate.Value.ToString("dd/MM/yyyy")}", normalFont, brush, leftMargin, yPos);
                yPos += 30;

                // Print headers
                g.DrawString("Date", headerFont, brush, leftMargin, yPos);
                g.DrawString("Description", headerFont, brush, leftMargin + 100, yPos);
                g.DrawString("Type", headerFont, brush, leftMargin + 350, yPos);
                g.DrawString("Cash In", headerFont, brush, leftMargin + 450, yPos);
                g.DrawString("Cash Out", headerFont, brush, leftMargin + 550, yPos);
                g.DrawString("Balance", headerFont, brush, leftMargin + 650, yPos);
                yPos += 20;

                // Print data
                if (cashFlowData != null && cashFlowData.Rows.Count > 0)
                {
                    foreach (DataRow row in cashFlowData.Rows)
                    {
                        if (yPos > e.MarginBounds.Bottom - 100) // Check if we need a new page
                        {
                            e.HasMorePages = true;
                            return;
                        }

                        g.DrawString(Convert.ToDateTime(row["TransactionDate"]).ToString("dd/MM/yyyy"), normalFont, brush, leftMargin, yPos);
                        g.DrawString(row["Description"].ToString(), normalFont, brush, leftMargin + 100, yPos);
                        g.DrawString(row["TransactionType"].ToString(), normalFont, brush, leftMargin + 350, yPos);
                        g.DrawString(Convert.ToDecimal(row["CashIn"]).ToString("N2"), normalFont, brush, leftMargin + 450, yPos);
                        g.DrawString(Convert.ToDecimal(row["CashOut"]).ToString("N2"), normalFont, brush, leftMargin + 550, yPos);
                        g.DrawString(Convert.ToDecimal(row["RunningBalance"]).ToString("N2"), normalFont, brush, leftMargin + 650, yPos);
                        yPos += 15;
                    }
                }

                // Print totals
                yPos += 10;
                g.DrawLine(Pens.Black, leftMargin, yPos, leftMargin + 750, yPos);
                yPos += 15;

                decimal totalCashIn = 0;
                decimal totalCashOut = 0;
                if (cashFlowData != null && cashFlowData.Rows.Count > 0)
                {
                    foreach (DataRow row in cashFlowData.Rows)
                    {
                        totalCashIn += Convert.ToDecimal(row["CashIn"]);
                        totalCashOut += Convert.ToDecimal(row["CashOut"]);
                    }
                }

                g.DrawString("TOTAL", headerFont, brush, leftMargin + 350, yPos);
                g.DrawString(totalCashIn.ToString("N2"), headerFont, brush, leftMargin + 450, yPos);
                g.DrawString(totalCashOut.ToString("N2"), headerFont, brush, leftMargin + 550, yPos);
                g.DrawString((totalCashIn - totalCashOut).ToString("N2"), headerFont, brush, leftMargin + 650, yPos);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error printing cash flow: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "CSV files (*.csv)|*.csv";
                saveDialog.FileName = $"CashFlow_{DateTime.Now:yyyyMMdd}.csv";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    using (System.IO.StreamWriter writer = new System.IO.StreamWriter(saveDialog.FileName))
                    {
                        // Write header
                        writer.WriteLine("Date,Description,Type,Cash In,Cash Out,Running Balance");

                        // Write data
                        if (cashFlowData != null && cashFlowData.Rows.Count > 0)
                        {
                            foreach (DataRow row in cashFlowData.Rows)
                            {
                                writer.WriteLine($"\"{Convert.ToDateTime(row["TransactionDate"]):dd/MM/yyyy}\",\"{row["Description"]}\",\"{row["TransactionType"]}\"," +
                                               $"{Convert.ToDecimal(row["CashIn"]):N2},{Convert.ToDecimal(row["CashOut"]):N2}," +
                                               $"{Convert.ToDecimal(row["RunningBalance"]):N2}");
                            }
                        }

                        // Write totals
                        decimal totalCashIn = 0;
                        decimal totalCashOut = 0;
                        if (cashFlowData != null && cashFlowData.Rows.Count > 0)
                        {
                            foreach (DataRow row in cashFlowData.Rows)
                            {
                                totalCashIn += Convert.ToDecimal(row["CashIn"]);
                                totalCashOut += Convert.ToDecimal(row["CashOut"]);
                            }
                        }
                        writer.WriteLine($"\"TOTAL\",\"\",\"\",{totalCashIn:N2},{totalCashOut:N2},{(totalCashIn - totalCashOut):N2}");
                    }

                    MessageBox.Show("Cash flow exported successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exporting cash flow: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
} 