using RetailManagement.Database;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace RetailManagement.UserForms
{
    public partial class BankTransactions : Form
    {
        private DataTable transactionsData;
        private int selectedTransactionId = 0;

        public BankTransactions()
        {
            InitializeComponent();
            LoadTransactions();
        }

        private void LoadTransactions()
        {
            try
            {
                string query = @"SELECT 
                                    TransactionID,
                                    TransactionDate,
                                    TransactionType,
                                    Amount,
                                    Description,
                                    BankName,
                                    AccountNumber,
                                    Remarks,
                                    IsActive
                                FROM BankTransactions 
                                WHERE IsActive = 1 
                                ORDER BY TransactionDate DESC";

                transactionsData = DatabaseConnection.ExecuteQuery(query);
                ShowTransactionsData();
                CalculateTotals();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading transactions: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowTransactionsData()
        {
            dgvTransactions.DataSource = transactionsData;
            dgvTransactions.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
        }

        private void CalculateTotals()
        {
            if (transactionsData != null && transactionsData.Rows.Count > 0)
            {
                decimal totalDeposits = 0;
                decimal totalWithdrawals = 0;

                foreach (DataRow row in transactionsData.Rows)
                {
                    string transactionType = row["TransactionType"].ToString();
                    decimal amount = Convert.ToDecimal(row["Amount"]);

                    if (transactionType == "Deposit")
                    {
                        totalDeposits += amount;
                    }
                    else if (transactionType == "Withdrawal")
                    {
                        totalWithdrawals += amount;
                    }
                }

                decimal balance = totalDeposits - totalWithdrawals;

                lblTotalDeposits.Text = $"Total Deposits: ₹{totalDeposits:N2}";
                lblTotalWithdrawals.Text = $"Total Withdrawals: ₹{totalWithdrawals:N2}";
                lblBalance.Text = $"Current Balance: ₹{balance:N2}";

                // Color code balance
                if (balance >= 0)
                {
                    lblBalance.ForeColor = System.Drawing.Color.Green;
                }
                else
                {
                    lblBalance.ForeColor = System.Drawing.Color.Red;
                }
            }
            else
            {
                lblTotalDeposits.Text = "Total Deposits: ₹0.00";
                lblTotalWithdrawals.Text = "Total Withdrawals: ₹0.00";
                lblBalance.Text = "Current Balance: ₹0.00";
                lblBalance.ForeColor = System.Drawing.Color.Black;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            ClearForm();
            EnableForm(true);
            btnAdd.Enabled = false;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
            btnSave.Enabled = true;
            btnCancel.Enabled = true;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedTransactionId == 0)
            {
                MessageBox.Show("Please select a transaction to update.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            EnableForm(true);
            btnAdd.Enabled = false;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
            btnSave.Enabled = true;
            btnCancel.Enabled = true;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedTransactionId == 0)
            {
                MessageBox.Show("Please select a transaction to delete.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show("Are you sure you want to delete this transaction?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    string query = "UPDATE BankTransactions SET IsActive = 0 WHERE TransactionID = @TransactionID";
                    SqlParameter[] parameters = {
                        new SqlParameter("@TransactionID", selectedTransactionId)
                    };

                    DatabaseConnection.ExecuteNonQuery(query, parameters);
                    MessageBox.Show("Transaction deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadTransactions();
                    ClearForm();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting transaction: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                try
                {
                    if (selectedTransactionId == 0)
                    {
                        // Add new transaction
                        string query = @"INSERT INTO BankTransactions 
                                        (TransactionDate, TransactionType, Amount, Description, BankName, AccountNumber, Remarks, IsActive, CreatedDate) 
                                        VALUES (@TransactionDate, @TransactionType, @Amount, @Description, @BankName, @AccountNumber, @Remarks, 1, GETDATE())";

                        SqlParameter[] parameters = {
                            new SqlParameter("@TransactionDate", dtpTransactionDate.Value),
                            new SqlParameter("@TransactionType", cmbTransactionType.Text),
                            new SqlParameter("@Amount", numAmount.Value),
                            new SqlParameter("@Description", txtDescription.Text.Trim()),
                            new SqlParameter("@BankName", txtBankName.Text.Trim()),
                            new SqlParameter("@AccountNumber", txtAccountNumber.Text.Trim()),
                            new SqlParameter("@Remarks", txtRemarks.Text.Trim())
                        };

                        DatabaseConnection.ExecuteNonQuery(query, parameters);
                        MessageBox.Show("Transaction added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        // Update existing transaction
                        string query = @"UPDATE BankTransactions 
                                        SET TransactionDate = @TransactionDate, 
                                            TransactionType = @TransactionType, 
                                            Amount = @Amount, 
                                            Description = @Description, 
                                            BankName = @BankName, 
                                            AccountNumber = @AccountNumber, 
                                            Remarks = @Remarks 
                                        WHERE TransactionID = @TransactionID";

                        SqlParameter[] parameters = {
                            new SqlParameter("@TransactionID", selectedTransactionId),
                            new SqlParameter("@TransactionDate", dtpTransactionDate.Value),
                            new SqlParameter("@TransactionType", cmbTransactionType.Text),
                            new SqlParameter("@Amount", numAmount.Value),
                            new SqlParameter("@Description", txtDescription.Text.Trim()),
                            new SqlParameter("@BankName", txtBankName.Text.Trim()),
                            new SqlParameter("@AccountNumber", txtAccountNumber.Text.Trim()),
                            new SqlParameter("@Remarks", txtRemarks.Text.Trim())
                        };

                        DatabaseConnection.ExecuteNonQuery(query, parameters);
                        MessageBox.Show("Transaction updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    LoadTransactions();
                    ClearForm();
                    EnableForm(false);
                    btnAdd.Enabled = true;
                    btnUpdate.Enabled = false;
                    btnDelete.Enabled = false;
                    btnSave.Enabled = false;
                    btnCancel.Enabled = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error saving transaction: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearForm();
            EnableForm(false);
            btnAdd.Enabled = true;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
            btnSave.Enabled = false;
            btnCancel.Enabled = false;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadTransactions();
        }

        private void ClearForm()
        {
            selectedTransactionId = 0;
            dtpTransactionDate.Value = DateTime.Now;
            cmbTransactionType.SelectedIndex = 0;
            numAmount.Value = 0;
            txtDescription.Clear();
            txtBankName.Clear();
            txtAccountNumber.Clear();
            txtRemarks.Clear();
        }

        private void EnableForm(bool enable)
        {
            dtpTransactionDate.Enabled = enable;
            cmbTransactionType.Enabled = enable;
            numAmount.Enabled = enable;
            txtDescription.Enabled = enable;
            txtBankName.Enabled = enable;
            txtAccountNumber.Enabled = enable;
            txtRemarks.Enabled = enable;
        }

        private bool ValidateForm()
        {
            if (cmbTransactionType.SelectedIndex == 0)
            {
                MessageBox.Show("Please select a transaction type.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbTransactionType.Focus();
                return false;
            }

            if (numAmount.Value <= 0)
            {
                MessageBox.Show("Please enter a valid amount.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                numAmount.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtDescription.Text))
            {
                MessageBox.Show("Please enter a description.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDescription.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtBankName.Text))
            {
                MessageBox.Show("Please enter a bank name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBankName.Focus();
                return false;
            }

            return true;
        }

        private void dgvTransactions_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvTransactions.Rows[e.RowIndex];
                selectedTransactionId = SafeDataHelper.SafeGetCellInt32(row, "TransactionID");
                
                dtpTransactionDate.Value = Convert.ToDateTime(row.Cells["TransactionDate"].Value);
                cmbTransactionType.Text = SafeDataHelper.SafeGetCellString(row, "TransactionType");
                numAmount.Value = Convert.ToDecimal(row.Cells["Amount"].Value);
                txtDescription.Text = SafeDataHelper.SafeGetCellString(row, "Description");
                txtBankName.Text = SafeDataHelper.SafeGetCellString(row, "BankName");
                txtAccountNumber.Text = SafeDataHelper.SafeGetCellString(row, "AccountNumber");
                txtRemarks.Text = SafeDataHelper.SafeGetCellString(row, "Remarks");

                btnUpdate.Enabled = true;
                btnDelete.Enabled = true;
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                string query = @"SELECT 
                                    TransactionID,
                                    TransactionDate,
                                    TransactionType,
                                    Amount,
                                    Description,
                                    BankName,
                                    AccountNumber,
                                    Remarks,
                                    IsActive
                                FROM BankTransactions 
                                WHERE IsActive = 1";

                if (dtpFromDate.Value <= dtpToDate.Value)
                {
                    query += " AND TransactionDate BETWEEN @FromDate AND @ToDate";
                }

                if (cmbSearchType.SelectedIndex > 0)
                {
                    query += " AND TransactionType = @TransactionType";
                }

                query += " ORDER BY TransactionDate DESC";

                SqlParameter[] parameters = null;
                if (dtpFromDate.Value <= dtpToDate.Value && cmbSearchType.SelectedIndex > 0)
                {
                    parameters = new SqlParameter[]
                    {
                        new SqlParameter("@FromDate", dtpFromDate.Value.Date),
                        new SqlParameter("@ToDate", dtpToDate.Value.Date),
                        new SqlParameter("@TransactionType", cmbSearchType.Text)
                    };
                }
                else if (dtpFromDate.Value <= dtpToDate.Value)
                {
                    parameters = new SqlParameter[]
                    {
                        new SqlParameter("@FromDate", dtpFromDate.Value.Date),
                        new SqlParameter("@ToDate", dtpToDate.Value.Date)
                    };
                }
                else if (cmbSearchType.SelectedIndex > 0)
                {
                    parameters = new SqlParameter[]
                    {
                        new SqlParameter("@TransactionType", cmbSearchType.Text)
                    };
                }

                transactionsData = DatabaseConnection.ExecuteQuery(query, parameters);
                ShowTransactionsData();
                CalculateTotals();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching transactions: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
} 
