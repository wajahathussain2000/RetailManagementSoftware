using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Drawing.Printing;
using RetailManagement.Database;

namespace RetailManagement.UserForms
{
    public partial class TrialBalance : Form
    {
        private DataTable trialBalanceData;

        public TrialBalance()
        {
            InitializeComponent();
            SetupDataGridView();
            LoadTrialBalance();
        }

        private void SetupDataGridView()
        {
            dgvTrialBalance.Columns.Clear();
            
            dgvTrialBalance.Columns.Add("AccountName", "Account Name");
            dgvTrialBalance.Columns.Add("AccountType", "Account Type");
            dgvTrialBalance.Columns.Add("NormalBalance", "Normal Balance");
            dgvTrialBalance.Columns.Add("DebitAmount", "Debit Amount");
            dgvTrialBalance.Columns.Add("CreditAmount", "Credit Amount");
            dgvTrialBalance.Columns.Add("Balance", "Balance");

            // Configure columns
            dgvTrialBalance.Columns["AccountName"].Width = 200;
            dgvTrialBalance.Columns["AccountType"].Width = 100;
            dgvTrialBalance.Columns["NormalBalance"].Width = 100;
            dgvTrialBalance.Columns["DebitAmount"].Width = 120;
            dgvTrialBalance.Columns["CreditAmount"].Width = 120;
            dgvTrialBalance.Columns["Balance"].Width = 120;

            // Set alignment
            dgvTrialBalance.Columns["DebitAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvTrialBalance.Columns["CreditAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvTrialBalance.Columns["Balance"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            // Set header alignment
            dgvTrialBalance.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvTrialBalance.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
        }

        private void LoadTrialBalance()
        {
            try
            {
                // Get date range for trial balance
                DateTime fromDate = dtpFromDate.Value;
                DateTime toDate = dtpToDate.Value;

                // Build comprehensive trial balance query
                string query = @"
                    WITH AccountBalances AS (
                        -- Sales Account (Credit)
                        SELECT 'Sales' as AccountName, 'Income' as AccountType, 'Credit' as NormalBalance,
                               SUM(TotalAmount) as CreditAmount, 0 as DebitAmount
                        FROM Sales 
                        WHERE SaleDate BETWEEN @FromDate AND @ToDate AND IsActive = 1
                        
                        UNION ALL
                        
                        -- Purchase Account (Debit)
                        SELECT 'Purchases' as AccountName, 'Expense' as AccountType, 'Debit' as NormalBalance,
                               0 as CreditAmount, SUM(TotalAmount) as DebitAmount
                        FROM Purchases 
                        WHERE PurchaseDate BETWEEN @FromDate AND @ToDate AND IsActive = 1
                        
                        UNION ALL
                        
                        -- Customer Payments (Debit - Cash/Bank)
                        SELECT 'Customer Payments' as AccountName, 'Asset' as AccountType, 'Debit' as NormalBalance,
                               0 as CreditAmount, SUM(Amount) as DebitAmount
                        FROM CustomerPayments 
                        WHERE PaymentDate BETWEEN @FromDate AND @ToDate AND IsActive = 1
                        
                        UNION ALL
                        
                        -- Customer Credit Bills (Credit - Accounts Receivable)
                        SELECT 'Accounts Receivable' as AccountName, 'Asset' as AccountType, 'Debit' as NormalBalance,
                               0 as CreditAmount, SUM(TotalAmount) as DebitAmount
                        FROM Sales 
                        WHERE SaleDate BETWEEN @FromDate AND @ToDate AND IsActive = 1 AND PaymentMethod = 'Credit'
                        
                        UNION ALL
                        
                        -- Purchase Returns (Credit)
                        SELECT 'Purchase Returns' as AccountName, 'Income' as AccountType, 'Credit' as NormalBalance,
                               SUM(TotalAmount) as CreditAmount, 0 as DebitAmount
                        FROM PurchaseReturns 
                        WHERE ReturnDate BETWEEN @FromDate AND @ToDate AND IsActive = 1
                        
                        UNION ALL
                        
                        -- Sale Returns (Debit)
                        SELECT 'Sale Returns' as AccountName, 'Expense' as AccountType, 'Debit' as NormalBalance,
                               0 as CreditAmount, SUM(TotalAmount) as DebitAmount
                        FROM SaleReturns 
                        WHERE ReturnDate BETWEEN @FromDate AND @ToDate AND IsActive = 1
                        
                        UNION ALL
                        
                        -- Expenses (Debit)
                        SELECT 'Expenses' as AccountName, 'Expense' as AccountType, 'Debit' as NormalBalance,
                               0 as CreditAmount, SUM(Amount) as DebitAmount
                        FROM Expenses 
                        WHERE ExpenseDate BETWEEN @FromDate AND @ToDate AND IsActive = 1
                        
                        UNION ALL
                        
                        -- Bank Deposits (Debit)
                        SELECT 'Bank Deposits' as AccountName, 'Asset' as AccountType, 'Debit' as NormalBalance,
                               0 as CreditAmount, SUM(Amount) as DebitAmount
                        FROM BankTransactions 
                        WHERE TransactionDate BETWEEN @FromDate AND @ToDate AND TransactionType = 'Deposit' AND IsActive = 1
                        
                        UNION ALL
                        
                        -- Bank Withdrawals (Credit)
                        SELECT 'Bank Withdrawals' as AccountName, 'Asset' as AccountType, 'Debit' as NormalBalance,
                               SUM(Amount) as CreditAmount, 0 as DebitAmount
                        FROM BankTransactions 
                        WHERE TransactionDate BETWEEN @FromDate AND @ToDate AND TransactionType = 'Withdrawal' AND IsActive = 1
                    )
                    SELECT 
                        AccountName,
                        AccountType,
                        NormalBalance,
                        DebitAmount,
                        CreditAmount,
                        CASE 
                            WHEN NormalBalance = 'Debit' THEN DebitAmount - CreditAmount
                            ELSE CreditAmount - DebitAmount
                        END as Balance
                    FROM AccountBalances
                    WHERE DebitAmount > 0 OR CreditAmount > 0
                    ORDER BY AccountType, AccountName";

                SqlParameter[] parameters = {
                    new SqlParameter("@FromDate", fromDate),
                    new SqlParameter("@ToDate", toDate)
                };

                trialBalanceData = DatabaseConnection.ExecuteQuery(query, parameters);
                ShowTrialBalanceData();
                CalculateTotals();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading trial balance: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowTrialBalanceData()
        {
            dgvTrialBalance.Rows.Clear();

            if (trialBalanceData != null && trialBalanceData.Rows.Count > 0)
            {
                foreach (DataRow row in trialBalanceData.Rows)
                {
                    int rowIndex = dgvTrialBalance.Rows.Add();
                    dgvTrialBalance.Rows[rowIndex].Cells["AccountName"].Value = row["AccountName"];
                    dgvTrialBalance.Rows[rowIndex].Cells["AccountType"].Value = row["AccountType"];
                    dgvTrialBalance.Rows[rowIndex].Cells["NormalBalance"].Value = row["NormalBalance"];
                    dgvTrialBalance.Rows[rowIndex].Cells["DebitAmount"].Value = Convert.ToDecimal(row["DebitAmount"]).ToString("N2");
                    dgvTrialBalance.Rows[rowIndex].Cells["CreditAmount"].Value = Convert.ToDecimal(row["CreditAmount"]).ToString("N2");
                    dgvTrialBalance.Rows[rowIndex].Cells["Balance"].Value = Convert.ToDecimal(row["Balance"]).ToString("N2");

                    // Color coding based on account type
                    if (row["AccountType"].ToString() == "Asset")
                    {
                        dgvTrialBalance.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightBlue;
                    }
                    else if (row["AccountType"].ToString() == "Income")
                    {
                        dgvTrialBalance.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightGreen;
                    }
                    else if (row["AccountType"].ToString() == "Expense")
                    {
                        dgvTrialBalance.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightCoral;
                    }
                }
            }
        }

        private void CalculateTotals()
        {
            decimal totalDebit = 0;
            decimal totalCredit = 0;

            if (trialBalanceData != null && trialBalanceData.Rows.Count > 0)
            {
                foreach (DataRow row in trialBalanceData.Rows)
                {
                    totalDebit += Convert.ToDecimal(row["DebitAmount"]);
                    totalCredit += Convert.ToDecimal(row["CreditAmount"]);
                }
            }

            lblTotalDebit.Text = "Total Debit: " + totalDebit.ToString("N2");
            lblTotalCredit.Text = "Total Credit: " + totalCredit.ToString("N2");

            // Check if trial balance is balanced
            if (Math.Abs(totalDebit - totalCredit) < 0.01m)
            {
                lblBalanceStatus.Text = "Trial Balance is Balanced ✓";
                lblBalanceStatus.ForeColor = Color.Green;
            }
            else
            {
                lblBalanceStatus.Text = "Trial Balance is NOT Balanced ✗";
                lblBalanceStatus.ForeColor = Color.Red;
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadTrialBalance();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                PrintDialog printDialog = new PrintDialog();
                PrintDocument printDocument = new PrintDocument();
                printDocument.PrintPage += PrintTrialBalance;
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

        private void PrintTrialBalance(object sender, PrintPageEventArgs e)
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
                g.DrawString("TRIAL BALANCE", titleFont, brush, leftMargin, yPos);
                yPos += 30;

                // Print date range
                g.DrawString($"Period: {dtpFromDate.Value.ToString("dd/MM/yyyy")} to {dtpToDate.Value.ToString("dd/MM/yyyy")}", normalFont, brush, leftMargin, yPos);
                yPos += 30;

                // Print headers
                g.DrawString("Account Name", headerFont, brush, leftMargin, yPos);
                g.DrawString("Type", headerFont, brush, leftMargin + 200, yPos);
                g.DrawString("Normal", headerFont, brush, leftMargin + 280, yPos);
                g.DrawString("Debit", headerFont, brush, leftMargin + 350, yPos);
                g.DrawString("Credit", headerFont, brush, leftMargin + 450, yPos);
                g.DrawString("Balance", headerFont, brush, leftMargin + 550, yPos);
                yPos += 20;

                // Print data
                if (trialBalanceData != null && trialBalanceData.Rows.Count > 0)
                {
                    foreach (DataRow row in trialBalanceData.Rows)
                    {
                        if (yPos > e.MarginBounds.Bottom - 100) // Check if we need a new page
                        {
                            e.HasMorePages = true;
                            return;
                        }

                        g.DrawString(row["AccountName"].ToString(), normalFont, brush, leftMargin, yPos);
                        g.DrawString(row["AccountType"].ToString(), normalFont, brush, leftMargin + 200, yPos);
                        g.DrawString(row["NormalBalance"].ToString(), normalFont, brush, leftMargin + 280, yPos);
                        g.DrawString(Convert.ToDecimal(row["DebitAmount"]).ToString("N2"), normalFont, brush, leftMargin + 350, yPos);
                        g.DrawString(Convert.ToDecimal(row["CreditAmount"]).ToString("N2"), normalFont, brush, leftMargin + 450, yPos);
                        g.DrawString(Convert.ToDecimal(row["Balance"]).ToString("N2"), normalFont, brush, leftMargin + 550, yPos);
                        yPos += 15;
                    }
                }

                // Print totals
                yPos += 10;
                g.DrawLine(Pens.Black, leftMargin, yPos, leftMargin + 650, yPos);
                yPos += 15;

                decimal totalDebit = 0;
                decimal totalCredit = 0;
                if (trialBalanceData != null && trialBalanceData.Rows.Count > 0)
                {
                    foreach (DataRow row in trialBalanceData.Rows)
                    {
                        totalDebit += Convert.ToDecimal(row["DebitAmount"]);
                        totalCredit += Convert.ToDecimal(row["CreditAmount"]);
                    }
                }

                g.DrawString("TOTAL", headerFont, brush, leftMargin + 280, yPos);
                g.DrawString(totalDebit.ToString("N2"), headerFont, brush, leftMargin + 350, yPos);
                g.DrawString(totalCredit.ToString("N2"), headerFont, brush, leftMargin + 450, yPos);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error printing trial balance: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "CSV files (*.csv)|*.csv";
                saveDialog.FileName = $"TrialBalance_{DateTime.Now:yyyyMMdd}.csv";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    using (System.IO.StreamWriter writer = new System.IO.StreamWriter(saveDialog.FileName))
                    {
                        // Write header
                        writer.WriteLine("Account Name,Account Type,Normal Balance,Debit Amount,Credit Amount,Balance");

                        // Write data
                        if (trialBalanceData != null && trialBalanceData.Rows.Count > 0)
                        {
                            foreach (DataRow row in trialBalanceData.Rows)
                            {
                                writer.WriteLine($"\"{row["AccountName"]}\",\"{row["AccountType"]}\",\"{row["NormalBalance"]}\"," +
                                               $"{Convert.ToDecimal(row["DebitAmount"]):N2},{Convert.ToDecimal(row["CreditAmount"]):N2}," +
                                               $"{Convert.ToDecimal(row["Balance"]):N2}");
                            }
                        }

                        // Write totals
                        decimal totalDebit = 0;
                        decimal totalCredit = 0;
                        if (trialBalanceData != null && trialBalanceData.Rows.Count > 0)
                        {
                            foreach (DataRow row in trialBalanceData.Rows)
                            {
                                totalDebit += Convert.ToDecimal(row["DebitAmount"]);
                                totalCredit += Convert.ToDecimal(row["CreditAmount"]);
                            }
                        }
                        writer.WriteLine($"\"TOTAL\",\"\",\"\",{totalDebit:N2},{totalCredit:N2},");
                    }

                    MessageBox.Show("Trial balance exported successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exporting trial balance: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
} 