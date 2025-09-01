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

namespace RetailManagement.UserForms
{
    public partial class ExpenseEntry : Form
    {
        private bool isEditMode = false;
        private int selectedExpenseID = 0;

        public ExpenseEntry()
        {
            InitializeComponent();
            LoadExpenseCategories();
            LoadExpenses();
            SetupDataGridView();
            SetDefaultDate();
        }

        private void SetupDataGridView()
        {
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.Columns.Clear();

            dataGridView1.Columns.Add("ExpenseID", "Expense ID");
            dataGridView1.Columns.Add("ExpenseDate", "Date");
            dataGridView1.Columns.Add("Category", "Category");
            dataGridView1.Columns.Add("Description", "Description");
            dataGridView1.Columns.Add("Amount", "Amount");
            dataGridView1.Columns.Add("PaymentMethod", "Payment Method");

            dataGridView1.Columns["ExpenseID"].DataPropertyName = "ExpenseID";
            dataGridView1.Columns["ExpenseDate"].DataPropertyName = "ExpenseDate";
            dataGridView1.Columns["Category"].DataPropertyName = "Category";
            dataGridView1.Columns["Description"].DataPropertyName = "Description";
            dataGridView1.Columns["Amount"].DataPropertyName = "Amount";
            dataGridView1.Columns["PaymentMethod"].DataPropertyName = "PaymentMethod";
        }

        private void LoadExpenseCategories()
        {
            try
            {
                // Predefined expense categories
                cmbCategory.Items.Clear();
                cmbCategory.Items.AddRange(new string[] {
                    "Rent",
                    "Salary",
                    "Utilities",
                    "Office Supplies",
                    "Marketing",
                    "Transportation",
                    "Insurance",
                    "Maintenance",
                    "Professional Services",
                    "Other"
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading expense categories: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadExpenses()
        {
            try
            {
                string query = @"SELECT ExpenseID, ExpenseDate, Category, Description, Amount, PaymentMethod 
                               FROM Expenses 
                               WHERE IsActive = 1 
                               ORDER BY ExpenseDate DESC";
                DataTable dt = DatabaseConnection.ExecuteQuery(query);
                dataGridView1.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading expenses: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetDefaultDate()
        {
            dtpExpenseDate.Value = DateTime.Now;
        }

        private void ClearForm()
        {
            txtDescription.Text = "";
            txtAmount.Text = "";
            cmbCategory.SelectedIndex = -1;
            cmbPaymentMethod.SelectedIndex = -1;
            dtpExpenseDate.Value = DateTime.Now;
            txtRemarks.Text = "";
            selectedExpenseID = 0;
            isEditMode = false;
            btnSave.Text = "Save";
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            ClearForm();
            btnSave.Enabled = true;
            btnCancel.Enabled = true;
            txtDescription.Focus();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                try
                {
                    if (isEditMode)
                    {
                        UpdateExpense();
                    }
                    else
                    {
                        InsertExpense();
                    }
                    LoadExpenses();
                    ClearForm();
                    btnSave.Enabled = false;
                    btnCancel.Enabled = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error saving expense: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void InsertExpense()
        {
            string query = @"INSERT INTO Expenses (ExpenseDate, Category, Description, Amount, PaymentMethod, Remarks, IsActive, CreatedDate) 
                           VALUES (@ExpenseDate, @Category, @Description, @Amount, @PaymentMethod, @Remarks, 1, @CreatedDate)";

            SqlParameter[] parameters = {
                new SqlParameter("@ExpenseDate", dtpExpenseDate.Value.Date),
                new SqlParameter("@Category", cmbCategory.Text),
                new SqlParameter("@Description", txtDescription.Text),
                new SqlParameter("@Amount", Convert.ToDecimal(txtAmount.Text)),
                new SqlParameter("@PaymentMethod", cmbPaymentMethod.Text),
                new SqlParameter("@Remarks", txtRemarks.Text),
                new SqlParameter("@CreatedDate", DateTime.Now)
            };

            DatabaseConnection.ExecuteNonQuery(query, parameters);
            MessageBox.Show("Expense saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void UpdateExpense()
        {
            string query = @"UPDATE Expenses 
                           SET ExpenseDate = @ExpenseDate, Category = @Category, Description = @Description, 
                               Amount = @Amount, PaymentMethod = @PaymentMethod, Remarks = @Remarks 
                           WHERE ExpenseID = @ExpenseID";

            SqlParameter[] parameters = {
                new SqlParameter("@ExpenseID", selectedExpenseID),
                new SqlParameter("@ExpenseDate", dtpExpenseDate.Value.Date),
                new SqlParameter("@Category", cmbCategory.Text),
                new SqlParameter("@Description", txtDescription.Text),
                new SqlParameter("@Amount", Convert.ToDecimal(txtAmount.Text)),
                new SqlParameter("@PaymentMethod", cmbPaymentMethod.Text),
                new SqlParameter("@Remarks", txtRemarks.Text)
            };

            DatabaseConnection.ExecuteNonQuery(query, parameters);
            MessageBox.Show("Expense updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private bool ValidateForm()
        {
            if (cmbCategory.SelectedIndex == -1)
            {
                MessageBox.Show("Please select an expense category.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtDescription.Text))
            {
                MessageBox.Show("Please enter expense description.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!decimal.TryParse(txtAmount.Text, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Please enter a valid amount.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (cmbPaymentMethod.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a payment method.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearForm();
            btnSave.Enabled = false;
            btnCancel.Enabled = false;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedExpenseID == 0)
            {
                MessageBox.Show("Please select an expense to delete.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show("Are you sure you want to delete this expense?", "Confirm Delete", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    string query = "UPDATE Expenses SET IsActive = 0 WHERE ExpenseID = @ExpenseID";
                    SqlParameter[] parameters = { new SqlParameter("@ExpenseID", selectedExpenseID) };
                    DatabaseConnection.ExecuteNonQuery(query, parameters);
                    
                    MessageBox.Show("Expense deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadExpenses();
                    ClearForm();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting expense: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                selectedExpenseID = SafeDataHelper.SafeGetCellInt32(row, "ExpenseID");
                
                dtpExpenseDate.Value = Convert.ToDateTime(row.Cells["ExpenseDate"].Value);
                cmbCategory.Text = SafeDataHelper.SafeGetCellString(row, "Category");
                txtDescription.Text = SafeDataHelper.SafeGetCellString(row, "Description");
                txtAmount.Text = SafeDataHelper.SafeGetCellString(row, "Amount");
                cmbPaymentMethod.Text = SafeDataHelper.SafeGetCellString(row, "PaymentMethod");
                
                isEditMode = true;
                btnSave.Text = "Update";
                btnSave.Enabled = true;
                btnCancel.Enabled = true;
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow only numbers, decimal point, and backspace
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            // Allow only one decimal point
            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
        }
    }
} 
