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
    public partial class GSTSetup : Form
    {
        private bool isEditMode = false;
        private int selectedGSTID = 0;

        public GSTSetup()
        {
            InitializeComponent();
            LoadGSTCategories();
            LoadGSTData();
            SetupDataGridView();
        }

        private void SetupDataGridView()
        {
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.Columns.Clear();

            dataGridView1.Columns.Add("GSTID", "GST ID");
            dataGridView1.Columns.Add("Category", "Category");
            dataGridView1.Columns.Add("GSTPercentage", "GST %");
            dataGridView1.Columns.Add("HSNCode", "HSN Code");
            dataGridView1.Columns.Add("Description", "Description");
            dataGridView1.Columns.Add("IsActive", "Active");

            dataGridView1.Columns["GSTID"].DataPropertyName = "GSTID";
            dataGridView1.Columns["Category"].DataPropertyName = "Category";
            dataGridView1.Columns["GSTPercentage"].DataPropertyName = "GSTPercentage";
            dataGridView1.Columns["HSNCode"].DataPropertyName = "HSNCode";
            dataGridView1.Columns["Description"].DataPropertyName = "Description";
            dataGridView1.Columns["IsActive"].DataPropertyName = "IsActive";
        }

        private void LoadGSTCategories()
        {
            try
            {
                // Predefined GST categories
                cmbCategory.Items.Clear();
                cmbCategory.Items.AddRange(new string[] {
                    "Medicines",
                    "Medical Devices",
                    "Surgical Items",
                    "Health Supplements",
                    "Cosmetics",
                    "General Items",
                    "Services",
                    "Other"
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading GST categories: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadGSTData()
        {
            try
            {
                string query = @"SELECT GSTID, Category, GSTPercentage, HSNCode, Description, IsActive 
                               FROM GSTSetup 
                               ORDER BY Category, GSTPercentage";
                DataTable dt = DatabaseConnection.ExecuteQuery(query);
                dataGridView1.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading GST data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearForm()
        {
            cmbCategory.SelectedIndex = -1;
            txtGSTPercentage.Text = "";
            txtHSNCode.Text = "";
            txtDescription.Text = "";
            chkActive.Checked = true;
            selectedGSTID = 0;
            isEditMode = false;
            btnSave.Text = "Save";
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            ClearForm();
            btnSave.Enabled = true;
            btnCancel.Enabled = true;
            cmbCategory.Focus();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                try
                {
                    if (isEditMode)
                    {
                        UpdateGST();
                    }
                    else
                    {
                        InsertGST();
                    }
                    LoadGSTData();
                    ClearForm();
                    btnSave.Enabled = false;
                    btnCancel.Enabled = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error saving GST setup: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void InsertGST()
        {
            string query = @"INSERT INTO GSTSetup (Category, GSTPercentage, HSNCode, Description, IsActive, CreatedDate) 
                           VALUES (@Category, @GSTPercentage, @HSNCode, @Description, @IsActive, @CreatedDate)";

            SqlParameter[] parameters = {
                new SqlParameter("@Category", cmbCategory.Text),
                new SqlParameter("@GSTPercentage", Convert.ToDecimal(txtGSTPercentage.Text)),
                new SqlParameter("@HSNCode", txtHSNCode.Text),
                new SqlParameter("@Description", txtDescription.Text),
                new SqlParameter("@IsActive", chkActive.Checked),
                new SqlParameter("@CreatedDate", DateTime.Now)
            };

            DatabaseConnection.ExecuteNonQuery(query, parameters);
            MessageBox.Show("GST setup saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void UpdateGST()
        {
            string query = @"UPDATE GSTSetup 
                           SET Category = @Category, GSTPercentage = @GSTPercentage, 
                               HSNCode = @HSNCode, Description = @Description, IsActive = @IsActive 
                           WHERE GSTID = @GSTID";

            SqlParameter[] parameters = {
                new SqlParameter("@GSTID", selectedGSTID),
                new SqlParameter("@Category", cmbCategory.Text),
                new SqlParameter("@GSTPercentage", Convert.ToDecimal(txtGSTPercentage.Text)),
                new SqlParameter("@HSNCode", txtHSNCode.Text),
                new SqlParameter("@Description", txtDescription.Text),
                new SqlParameter("@IsActive", chkActive.Checked)
            };

            DatabaseConnection.ExecuteNonQuery(query, parameters);
            MessageBox.Show("GST setup updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private bool ValidateForm()
        {
            if (cmbCategory.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a category.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!decimal.TryParse(txtGSTPercentage.Text, out decimal gstPercentage) || gstPercentage < 0 || gstPercentage > 100)
            {
                MessageBox.Show("Please enter a valid GST percentage (0-100).", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtHSNCode.Text))
            {
                MessageBox.Show("Please enter HSN code.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtDescription.Text))
            {
                MessageBox.Show("Please enter description.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            if (selectedGSTID == 0)
            {
                MessageBox.Show("Please select a GST setup to delete.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show("Are you sure you want to delete this GST setup?", "Confirm Delete", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    string query = "UPDATE GSTSetup SET IsActive = 0 WHERE GSTID = @GSTID";
                    SqlParameter[] parameters = { new SqlParameter("@GSTID", selectedGSTID) };
                    DatabaseConnection.ExecuteNonQuery(query, parameters);
                    
                    MessageBox.Show("GST setup deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadGSTData();
                    ClearForm();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting GST setup: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                selectedGSTID = SafeDataHelper.SafeGetCellInt32(row, "GSTID");
                
                cmbCategory.Text = SafeDataHelper.SafeGetCellString(row, "Category");
                txtGSTPercentage.Text = SafeDataHelper.SafeGetCellString(row, "GSTPercentage");
                txtHSNCode.Text = SafeDataHelper.SafeGetCellString(row, "HSNCode");
                txtDescription.Text = SafeDataHelper.SafeGetCellString(row, "Description");
                chkActive.Checked = Convert.ToBoolean(row.Cells["IsActive"].Value);
                
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

        private void txtGSTPercentage_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtHSNCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow only numbers and backspace for HSN code
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
} 
