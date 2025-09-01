using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using RetailManagement.Database;

namespace RetailManagement.UserForms
{
    public partial class CashCounterManagement : Form
    {
        private int currentUserID;
        // Note: These fields are assigned for future functionality
        private DateTime currentShift = DateTime.Now;
        private decimal openingBalance = 0; // TODO: Implement opening balance functionality
        private decimal closingBalance = 0; // TODO: Implement closing balance functionality

        public CashCounterManagement()
        {
            InitializeComponent();
            LoadCurrentShiftData();
        }

        public CashCounterManagement(int userID) : this()
        {
            currentUserID = userID;
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panel1 = new Panel();
            this.panel2 = new Panel();
            this.groupBox1 = new GroupBox();
            this.groupBox2 = new GroupBox();
            this.groupBox3 = new GroupBox();
            
            // Labels and TextBoxes
            this.lblShiftDate = new Label();
            this.lblOpeningBalance = new Label();
            this.lblClosingBalance = new Label();
            this.lblCashSales = new Label();
            this.lblCardSales = new Label();
            this.lblUPISales = new Label();
            this.lblExpenses = new Label();
            this.lblNetCash = new Label();
            
            this.txtOpeningBalance = new TextBox();
            this.txtClosingBalance = new TextBox();
            this.txtCashSales = new TextBox();
            this.txtCardSales = new TextBox();
            this.txtUPISales = new TextBox();
            this.txtExpenses = new TextBox();
            this.txtNetCash = new TextBox();
            this.txtRemarks = new TextBox();
            
            // Buttons
            this.btnOpenShift = new Button();
            this.btnCloseShift = new Button();
            this.btnAddExpense = new Button();
            this.btnViewReport = new Button();
            this.btnRefresh = new Button();
            
            // DataGridView for cash transactions
            this.dgvTransactions = new DataGridView();
            
            // Form properties
            this.Text = "Cash Counter Management";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;
            
            SetupControls();
            SetupEvents();
        }

        private void SetupControls()
        {
            // Panel 1 - Header
            panel1.Dock = DockStyle.Top;
            panel1.Height = 60;
            panel1.BackColor = Color.FromArgb(52, 152, 219);
            this.Controls.Add(panel1);

            Label titleLabel = new Label();
            titleLabel.Text = "Cash Counter Management";
            titleLabel.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            titleLabel.ForeColor = Color.White;
            titleLabel.Location = new Point(20, 15);
            titleLabel.AutoSize = true;
            panel1.Controls.Add(titleLabel);

            // Panel 2 - Main content
            panel2.Dock = DockStyle.Fill;
            panel2.Padding = new Padding(20);
            this.Controls.Add(panel2);

            // GroupBox 1 - Shift Information
            groupBox1.Text = "Current Shift Information";
            groupBox1.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            groupBox1.Location = new Point(20, 20);
            groupBox1.Size = new Size(400, 150);
            panel2.Controls.Add(groupBox1);

            // Shift Date
            lblShiftDate.Text = $"Shift Date: {DateTime.Now:dd/MM/yyyy}";
            lblShiftDate.Location = new Point(20, 30);
            lblShiftDate.AutoSize = true;
            groupBox1.Controls.Add(lblShiftDate);

            // Opening Balance
            lblOpeningBalance.Text = "Opening Balance:";
            lblOpeningBalance.Location = new Point(20, 60);
            lblOpeningBalance.AutoSize = true;
            groupBox1.Controls.Add(lblOpeningBalance);

            txtOpeningBalance.Location = new Point(150, 57);
            txtOpeningBalance.Size = new Size(100, 25);
            txtOpeningBalance.ReadOnly = true;
            groupBox1.Controls.Add(txtOpeningBalance);

            // Closing Balance
            lblClosingBalance.Text = "Closing Balance:";
            lblClosingBalance.Location = new Point(20, 90);
            lblClosingBalance.AutoSize = true;
            groupBox1.Controls.Add(lblClosingBalance);

            txtClosingBalance.Location = new Point(150, 87);
            txtClosingBalance.Size = new Size(100, 25);
            groupBox1.Controls.Add(txtClosingBalance);

            // Buttons
            btnOpenShift.Text = "Open Shift";
            btnOpenShift.Location = new Point(270, 57);
            btnOpenShift.Size = new Size(100, 30);
            btnOpenShift.BackColor = Color.FromArgb(46, 204, 113);
            btnOpenShift.ForeColor = Color.White;
            groupBox1.Controls.Add(btnOpenShift);

            btnCloseShift.Text = "Close Shift";
            btnCloseShift.Location = new Point(270, 97);
            btnCloseShift.Size = new Size(100, 30);
            btnCloseShift.BackColor = Color.FromArgb(231, 76, 60);
            btnCloseShift.ForeColor = Color.White;
            groupBox1.Controls.Add(btnCloseShift);

            // GroupBox 2 - Sales Summary
            groupBox2.Text = "Today's Sales Summary";
            groupBox2.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            groupBox2.Location = new Point(440, 20);
            groupBox2.Size = new Size(400, 150);
            panel2.Controls.Add(groupBox2);

            // Sales fields
            int yPos = 30;
            AddSalesField(groupBox2, "Cash Sales:", txtCashSales, yPos);
            AddSalesField(groupBox2, "Card Sales:", txtCardSales, yPos + 30);
            AddSalesField(groupBox2, "UPI Sales:", txtUPISales, yPos + 60);
            AddSalesField(groupBox2, "Expenses:", txtExpenses, yPos + 90);

            // GroupBox 3 - Transactions
            groupBox3.Text = "Cash Transactions";
            groupBox3.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            groupBox3.Location = new Point(20, 190);
            groupBox3.Size = new Size(820, 300);
            panel2.Controls.Add(groupBox3);

            // DataGridView
            dgvTransactions.Location = new Point(20, 30);
            dgvTransactions.Size = new Size(780, 220);
            dgvTransactions.AutoGenerateColumns = false;
            SetupDataGridView();
            groupBox3.Controls.Add(dgvTransactions);

            // Bottom buttons
            btnAddExpense.Text = "Add Expense";
            btnAddExpense.Location = new Point(20, 260);
            btnAddExpense.Size = new Size(100, 30);
            btnAddExpense.BackColor = Color.FromArgb(52, 152, 219);
            btnAddExpense.ForeColor = Color.White;
            groupBox3.Controls.Add(btnAddExpense);

            btnViewReport.Text = "View Report";
            btnViewReport.Location = new Point(130, 260);
            btnViewReport.Size = new Size(100, 30);
            btnViewReport.BackColor = Color.FromArgb(155, 89, 182);
            btnViewReport.ForeColor = Color.White;
            groupBox3.Controls.Add(btnViewReport);

            btnRefresh.Text = "Refresh";
            btnRefresh.Location = new Point(240, 260);
            btnRefresh.Size = new Size(100, 30);
            btnRefresh.BackColor = Color.FromArgb(52, 73, 94);
            btnRefresh.ForeColor = Color.White;
            groupBox3.Controls.Add(btnRefresh);
        }

        private void AddSalesField(GroupBox parent, string labelText, TextBox textBox, int yPosition)
        {
            Label label = new Label();
            label.Text = labelText;
            label.Location = new Point(20, yPosition);
            label.AutoSize = true;
            parent.Controls.Add(label);

            textBox.Location = new Point(150, yPosition - 3);
            textBox.Size = new Size(100, 25);
            textBox.ReadOnly = true;
            parent.Controls.Add(textBox);
        }

        private void SetupDataGridView()
        {
            dgvTransactions.Columns.Clear();
            
            dgvTransactions.Columns.Add("Time", "Time");
            dgvTransactions.Columns.Add("Type", "Type");
            dgvTransactions.Columns.Add("Amount", "Amount");
            dgvTransactions.Columns.Add("PaymentMode", "Payment Mode");
            dgvTransactions.Columns.Add("Description", "Description");
            dgvTransactions.Columns.Add("UserName", "User");

            // Configure columns
            dgvTransactions.Columns["Time"].Width = 100;
            dgvTransactions.Columns["Type"].Width = 80;
            dgvTransactions.Columns["Amount"].Width = 100;
            dgvTransactions.Columns["PaymentMode"].Width = 100;
            dgvTransactions.Columns["Description"].Width = 200;
            dgvTransactions.Columns["UserName"].Width = 120;

            // Formatting
            dgvTransactions.Columns["Amount"].DefaultCellStyle.Format = "C2";
            dgvTransactions.Columns["Amount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private void SetupEvents()
        {
            btnOpenShift.Click += BtnOpenShift_Click;
            btnCloseShift.Click += BtnCloseShift_Click;
            btnAddExpense.Click += BtnAddExpense_Click;
            btnViewReport.Click += BtnViewReport_Click;
            btnRefresh.Click += BtnRefresh_Click;
            this.Load += CashCounterManagement_Load;
        }

        private void CashCounterManagement_Load(object sender, EventArgs e)
        {
            LoadCurrentShiftData();
            LoadTodaysSales();
            LoadCashTransactions();
        }

        private void LoadCurrentShiftData()
        {
            try
            {
                string query = @"SELECT TOP 1 OpeningBalance, ClosingBalance, ShiftStatus 
                               FROM CashCounter 
                               WHERE UserID = @UserID AND CAST(ShiftDate AS DATE) = CAST(GETDATE() AS DATE)
                               ORDER BY ShiftDate DESC";
                
                SqlParameter[] parameters = { new SqlParameter("@UserID", currentUserID) };
                DataTable result = DatabaseConnection.ExecuteQuery(query, parameters);

                if (result.Rows.Count > 0)
                {
                    txtOpeningBalance.Text = result.Rows[0]["OpeningBalance"].ToString();
                    txtClosingBalance.Text = result.Rows[0]["ClosingBalance"].ToString();
                    
                    string status = result.Rows[0]["ShiftStatus"].ToString();
                    btnOpenShift.Enabled = status != "Open";
                    btnCloseShift.Enabled = status == "Open";
                }
                else
                {
                    btnOpenShift.Enabled = true;
                    btnCloseShift.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading shift data: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadTodaysSales()
        {
            try
            {
                string query = @"SELECT 
                                SUM(CASE WHEN PaymentStatus = 'Cash' OR CashAmount > 0 THEN CashAmount ELSE 0 END) as CashSales,
                                SUM(CASE WHEN PaymentStatus = 'Card' OR CardAmount > 0 THEN CardAmount ELSE 0 END) as CardSales,
                                SUM(CASE WHEN PaymentStatus = 'UPI' OR UPIAmount > 0 THEN UPIAmount ELSE 0 END) as UPISales
                                FROM Sales 
                                WHERE CAST(SaleDate AS DATE) = CAST(GETDATE() AS DATE) AND IsActive = 1";

                DataTable result = DatabaseConnection.ExecuteQuery(query);
                
                if (result.Rows.Count > 0)
                {
                    txtCashSales.Text = SafeDataHelper.SafeToDecimal(result.Rows[0]["CashSales"]).ToString("C2");
                    txtCardSales.Text = SafeDataHelper.SafeToDecimal(result.Rows[0]["CardSales"]).ToString("C2");
                    txtUPISales.Text = SafeDataHelper.SafeToDecimal(result.Rows[0]["UPISales"]).ToString("C2");
                }

                // Load expenses
                string expenseQuery = @"SELECT SUM(Amount) as TotalExpenses FROM Expenses 
                                       WHERE CAST(ExpenseDate AS DATE) = CAST(GETDATE() AS DATE) AND IsActive = 1";
                DataTable expenseResult = DatabaseConnection.ExecuteQuery(expenseQuery);
                
                if (expenseResult.Rows.Count > 0)
                {
                    txtExpenses.Text = SafeDataHelper.SafeToDecimal(expenseResult.Rows[0]["TotalExpenses"]).ToString("C2");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading sales data: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadCashTransactions()
        {
            try
            {
                string query = @"SELECT 
                                FORMAT(s.SaleDate, 'HH:mm') as Time,
                                'Sale' as Type,
                                s.NetAmount as Amount,
                                CASE 
                                    WHEN s.CashAmount > 0 THEN 'Cash'
                                    WHEN s.CardAmount > 0 THEN 'Card'
                                    WHEN s.UPIAmount > 0 THEN 'UPI'
                                    ELSE 'Mixed'
                                END as PaymentMode,
                                'Sale #' + s.BillNumber as Description,
                                u.FullName as UserName
                                FROM Sales s
                                INNER JOIN Users u ON s.UserID = u.UserID
                                WHERE CAST(s.SaleDate AS DATE) = CAST(GETDATE() AS DATE) AND s.IsActive = 1
                                
                                UNION ALL
                                
                                SELECT 
                                FORMAT(e.ExpenseDate, 'HH:mm') as Time,
                                'Expense' as Type,
                                e.Amount,
                                'Cash' as PaymentMode,
                                e.Description,
                                u.FullName as UserName
                                FROM Expenses e
                                INNER JOIN Users u ON e.UserID = u.UserID
                                WHERE CAST(e.ExpenseDate AS DATE) = CAST(GETDATE() AS DATE) AND e.IsActive = 1
                                
                                ORDER BY Time DESC";

                DataTable result = DatabaseConnection.ExecuteQuery(query);
                dgvTransactions.DataSource = result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading transactions: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnOpenShift_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtOpeningBalance.Text))
                {
                    MessageBox.Show("Please enter opening balance.", "Validation Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                decimal openingBal = Convert.ToDecimal(txtOpeningBalance.Text);
                
                string query = @"INSERT INTO CashCounter (UserID, ShiftDate, OpeningBalance, ShiftStatus, CreatedDate)
                               VALUES (@UserID, @ShiftDate, @OpeningBalance, 'Open', GETDATE())";
                
                SqlParameter[] parameters = {
                    new SqlParameter("@UserID", currentUserID),
                    new SqlParameter("@ShiftDate", DateTime.Now),
                    new SqlParameter("@OpeningBalance", openingBal)
                };

                DatabaseConnection.ExecuteNonQuery(query, parameters);
                
                MessageBox.Show("Shift opened successfully!", "Success", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                LoadCurrentShiftData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening shift: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCloseShift_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtClosingBalance.Text))
                {
                    MessageBox.Show("Please enter closing balance.", "Validation Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                decimal closingBal = Convert.ToDecimal(txtClosingBalance.Text);
                
                string query = @"UPDATE CashCounter SET 
                               ClosingBalance = @ClosingBalance, 
                               ShiftStatus = 'Closed',
                               ClosedDate = GETDATE()
                               WHERE UserID = @UserID AND CAST(ShiftDate AS DATE) = CAST(GETDATE() AS DATE)
                               AND ShiftStatus = 'Open'";
                
                SqlParameter[] parameters = {
                    new SqlParameter("@ClosingBalance", closingBal),
                    new SqlParameter("@UserID", currentUserID)
                };

                int result = DatabaseConnection.ExecuteNonQuery(query, parameters);
                
                if (result > 0)
                {
                    MessageBox.Show("Shift closed successfully!", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadCurrentShiftData();
                }
                else
                {
                    MessageBox.Show("No open shift found to close.", "Information", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error closing shift: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAddExpense_Click(object sender, EventArgs e)
        {
            // Open expense entry form
            ExpenseEntry expenseForm = new ExpenseEntry();
            if (expenseForm.ShowDialog() == DialogResult.OK)
            {
                LoadTodaysSales();
                LoadCashTransactions();
            }
        }

        private void BtnViewReport_Click(object sender, EventArgs e)
        {
            // Open daily reports form which includes cash flow and sales reports
            DailyReportsForm reportForm = new DailyReportsForm();
            reportForm.Show();
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadCurrentShiftData();
            LoadTodaysSales();
            LoadCashTransactions();
        }

        #region Designer Variables
        private System.ComponentModel.IContainer components = null;
        private Panel panel1;
        private Panel panel2;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private GroupBox groupBox3;
        private Label lblShiftDate;
        private Label lblOpeningBalance;
        private Label lblClosingBalance;
        private Label lblCashSales;
        private Label lblCardSales;
        private Label lblUPISales;
        private Label lblExpenses;
        private Label lblNetCash;
        private TextBox txtOpeningBalance;
        private TextBox txtClosingBalance;
        private TextBox txtCashSales;
        private TextBox txtCardSales;
        private TextBox txtUPISales;
        private TextBox txtExpenses;
        private TextBox txtNetCash;
        private TextBox txtRemarks;
        private Button btnOpenShift;
        private Button btnCloseShift;
        private Button btnAddExpense;
        private Button btnViewReport;
        private Button btnRefresh;
        private DataGridView dgvTransactions;
        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
