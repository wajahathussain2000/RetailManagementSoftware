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
using RetailManagement.Models;

namespace RetailManagement.UserForms
{
    public partial class SubstituteManagementForm : Form
    {
        private int itemID;
        private string itemName;

        public SubstituteManagementForm(int itemID, string itemName)
        {
            this.itemID = itemID;
            this.itemName = itemName;
            InitializeComponent();
            LoadSubstitutes();
            LoadAvailableItems();
        }

        private void LoadSubstitutes()
        {
            try
            {
                string query = @"SELECT s.SubstituteID, i.ItemName as SubstituteName, s.Reason, s.CreatedDate 
                               FROM ItemSubstitutes s 
                               INNER JOIN Items i ON s.SubstituteItemID = i.ItemID 
                               WHERE s.ItemID = @ItemID";
                
                SqlParameter[] parameters = { new SqlParameter("@ItemID", itemID) };
                DataTable substituteData = DatabaseConnection.ExecuteQuery(query, parameters);
                
                dgvSubstitutes.DataSource = substituteData;
                
                if (dgvSubstitutes.Columns.Count > 0)
                {
                    dgvSubstitutes.Columns["SubstituteID"].Visible = false;
                    dgvSubstitutes.Columns["SubstituteName"].HeaderText = "Substitute Item";
                    dgvSubstitutes.Columns["Reason"].HeaderText = "Reason";
                    dgvSubstitutes.Columns["CreatedDate"].HeaderText = "Added On";
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Error loading substitutes.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadAvailableItems()
        {
            try
            {
                string query = @"SELECT ItemID, ItemName FROM Items WHERE ItemID != @ItemID AND IsActive = 1 ORDER BY ItemName";
                SqlParameter[] parameters = { new SqlParameter("@ItemID", itemID) };
                DataTable itemsData = DatabaseConnection.ExecuteQuery(query, parameters);
                
                cmbSubstituteItem.DataSource = itemsData;
                cmbSubstituteItem.DisplayMember = "ItemName";
                cmbSubstituteItem.ValueMember = "ItemID";
                cmbSubstituteItem.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading items: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbSubstituteItem.SelectedValue == null)
                {
                    MessageBox.Show("Please select a substitute item.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtSubstituteReason.Text))
                {
                    MessageBox.Show("Please enter a reason for the substitute.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string query = @"INSERT INTO ItemSubstitutes (ItemID, SubstituteItemID, Reason, CreatedDate) 
                               VALUES (@ItemID, @SubstituteItemID, @Reason, @CreatedDate)";
                
                SqlParameter[] parameters = {
                    new SqlParameter("@ItemID", itemID),
                    new SqlParameter("@SubstituteItemID", cmbSubstituteItem.SelectedValue),
                    new SqlParameter("@Reason", txtSubstituteReason.Text.Trim()),
                    new SqlParameter("@CreatedDate", DateTime.Now)
                };

                int result = DatabaseConnection.ExecuteNonQuery(query, parameters);
                
                if (result > 0)
                {
                    MessageBox.Show("Substitute added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadSubstitutes();
                    ClearForm();
                }
                else
                {
                    MessageBox.Show("Failed to add substitute.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding substitute: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvSubstitutes.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Please select a substitute to remove.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var result = MessageBox.Show("Are you sure you want to remove this substitute?", "Confirm Removal", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    int substituteID = Convert.ToInt32(dgvSubstitutes.SelectedRows[0].Cells["SubstituteID"].Value);
                    
                    string query = "DELETE FROM ItemSubstitutes WHERE SubstituteID = @SubstituteID";
                    SqlParameter[] parameters = { new SqlParameter("@SubstituteID", substituteID) };

                    int deleteResult = DatabaseConnection.ExecuteNonQuery(query, parameters);
                    
                    if (deleteResult > 0)
                    {
                        MessageBox.Show("Substitute removed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadSubstitutes();
                    }
                    else
                    {
                        MessageBox.Show("Failed to remove substitute.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error removing substitute: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvSubstitutes_SelectionChanged(object sender, EventArgs e)
        {
            // Update UI based on selection if needed
        }

        private void ClearForm()
        {
            cmbSubstituteItem.SelectedIndex = -1;
            txtSubstituteReason.Clear();
        }
    }
}
