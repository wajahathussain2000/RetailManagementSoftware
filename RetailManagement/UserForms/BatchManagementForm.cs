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
    public partial class BatchManagementForm : Form
    {
        private int itemID;
        private string itemName;

        public BatchManagementForm(int itemID, string itemName)
        {
            this.itemID = itemID;
            this.itemName = itemName;
            InitializeComponent();
            SetupForm();
            LoadBatches();
        }

        private void SetupForm()
        {
            // Form properties
            this.Text = "Batch Management";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Set item name label
            this.lblItemName.Text = $"Managing batches for: {itemName}";

            // DataGridView properties
            this.dgvBatches.AllowUserToAddRows = false;
            this.dgvBatches.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Wire up events
            this.dgvBatches.SelectionChanged += dgvBatches_SelectionChanged;
            this.btnAdd.Click += btnAdd_Click;
            this.btnUpdate.Click += btnUpdate_Click;
            this.btnDelete.Click += btnDelete_Click;
            this.btnClose.Click += btnClose_Click;
        }



        private void LoadBatches()
        {
            try
            {
                string query = @"SELECT BatchID, BatchNumber, ManufacturingDate, ExpiryDate, 
                               Quantity, PurchaseRate, MRP, CreatedDate 
                               FROM ItemBatches WHERE ItemID = @ItemID ORDER BY ExpiryDate";
                
                SqlParameter[] parameters = { new SqlParameter("@ItemID", itemID) };
                DataTable batchData = DatabaseConnection.ExecuteQuery(query, parameters);
                
                dgvBatches.DataSource = batchData;
                
                if (dgvBatches.Columns.Count > 0)
                {
                    dgvBatches.Columns["BatchID"].Visible = false;
                    dgvBatches.Columns["BatchNumber"].HeaderText = "Batch Number";
                    dgvBatches.Columns["ManufacturingDate"].HeaderText = "Mfg Date";
                    dgvBatches.Columns["ExpiryDate"].HeaderText = "Expiry Date";
                    dgvBatches.Columns["Quantity"].HeaderText = "Quantity";
                    dgvBatches.Columns["PurchaseRate"].HeaderText = "Purchase Rate";
                    dgvBatches.Columns["MRP"].HeaderText = "MRP";
                    dgvBatches.Columns["CreatedDate"].HeaderText = "Created Date";
                    
                    dgvBatches.AutoResizeColumns();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading batches: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvBatches_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvBatches.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dgvBatches.SelectedRows[0];
                
                txtBatchNumber.Text = selectedRow.Cells["BatchNumber"].Value?.ToString() ?? "";
                
                if (DateTime.TryParse(selectedRow.Cells["ManufacturingDate"].Value?.ToString(), out DateTime mfgDate))
                    dtpManufacturingDate.Value = mfgDate;
                
                if (DateTime.TryParse(selectedRow.Cells["ExpiryDate"].Value?.ToString(), out DateTime expDate))
                    dtpExpiryDate.Value = expDate;
                
                txtQuantity.Text = selectedRow.Cells["Quantity"].Value?.ToString() ?? "";
                txtPurchaseRate.Text = selectedRow.Cells["PurchaseRate"].Value?.ToString() ?? "";
                txtMRP.Text = selectedRow.Cells["MRP"].Value?.ToString() ?? "";
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateInput())
                {
                    string query = @"INSERT INTO ItemBatches (ItemID, BatchNumber, ManufacturingDate, 
                                   ExpiryDate, Quantity, PurchaseRate, MRP, CreatedDate, CreatedBy)
                                   VALUES (@ItemID, @BatchNumber, @ManufacturingDate, @ExpiryDate, 
                                   @Quantity, @PurchaseRate, @MRP, GETDATE(), 1)";
                    
                    SqlParameter[] parameters = {
                        new SqlParameter("@ItemID", itemID),
                        new SqlParameter("@BatchNumber", txtBatchNumber.Text.Trim()),
                        new SqlParameter("@ManufacturingDate", dtpManufacturingDate.Value),
                        new SqlParameter("@ExpiryDate", dtpExpiryDate.Value),
                        new SqlParameter("@Quantity", decimal.Parse(txtQuantity.Text)),
                        new SqlParameter("@PurchaseRate", decimal.Parse(txtPurchaseRate.Text)),
                        new SqlParameter("@MRP", decimal.Parse(txtMRP.Text))
                    };
                    
                    int result = DatabaseConnection.ExecuteNonQuery(query, parameters);
                    
                    if (result > 0)
                    {
                        MessageBox.Show("Batch added successfully!", "Success", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadBatches();
                        ClearForm();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding batch: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvBatches.SelectedRows.Count > 0 && ValidateInput())
                {
                    int batchID = Convert.ToInt32(dgvBatches.SelectedRows[0].Cells["BatchID"].Value);
                    
                    string query = @"UPDATE ItemBatches SET BatchNumber = @BatchNumber, 
                                   ManufacturingDate = @ManufacturingDate, ExpiryDate = @ExpiryDate, 
                                   Quantity = @Quantity, PurchaseRate = @PurchaseRate, MRP = @MRP, 
                                   ModifiedDate = GETDATE(), ModifiedBy = 1
                                   WHERE BatchID = @BatchID";
                    
                    SqlParameter[] parameters = {
                        new SqlParameter("@BatchID", batchID),
                        new SqlParameter("@BatchNumber", txtBatchNumber.Text.Trim()),
                        new SqlParameter("@ManufacturingDate", dtpManufacturingDate.Value),
                        new SqlParameter("@ExpiryDate", dtpExpiryDate.Value),
                        new SqlParameter("@Quantity", decimal.Parse(txtQuantity.Text)),
                        new SqlParameter("@PurchaseRate", decimal.Parse(txtPurchaseRate.Text)),
                        new SqlParameter("@MRP", decimal.Parse(txtMRP.Text))
                    };
                    
                    int result = DatabaseConnection.ExecuteNonQuery(query, parameters);
                    
                    if (result > 0)
                    {
                        MessageBox.Show("Batch updated successfully!", "Success", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadBatches();
                        ClearForm();
                    }
                }
                else
                {
                    MessageBox.Show("Please select a batch to update.", "Information", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating batch: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvBatches.SelectedRows.Count > 0)
                {
                    if (MessageBox.Show("Are you sure you want to delete this batch?", "Confirm Delete", 
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        int batchID = Convert.ToInt32(dgvBatches.SelectedRows[0].Cells["BatchID"].Value);
                        
                        string query = "DELETE FROM ItemBatches WHERE BatchID = @BatchID";
                        SqlParameter[] parameters = { new SqlParameter("@BatchID", batchID) };
                        
                        int result = DatabaseConnection.ExecuteNonQuery(query, parameters);
                        
                        if (result > 0)
                        {
                            MessageBox.Show("Batch deleted successfully!", "Success", 
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadBatches();
                            ClearForm();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Please select a batch to delete.", "Information", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting batch: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtBatchNumber.Text))
            {
                MessageBox.Show("Please enter batch number.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBatchNumber.Focus();
                return false;
            }

            if (dtpExpiryDate.Value <= dtpManufacturingDate.Value)
            {
                MessageBox.Show("Expiry date must be after manufacturing date.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtpExpiryDate.Focus();
                return false;
            }

            if (!decimal.TryParse(txtQuantity.Text, out decimal quantity) || quantity <= 0)
            {
                MessageBox.Show("Please enter valid quantity.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtQuantity.Focus();
                return false;
            }

            if (!decimal.TryParse(txtPurchaseRate.Text, out decimal purchaseRate) || purchaseRate < 0)
            {
                MessageBox.Show("Please enter valid purchase rate.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPurchaseRate.Focus();
                return false;
            }

            if (!decimal.TryParse(txtMRP.Text, out decimal mrp) || mrp < 0)
            {
                MessageBox.Show("Please enter valid MRP.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMRP.Focus();
                return false;
            }

            return true;
        }

        private void ClearForm()
        {
            txtBatchNumber.Clear();
            dtpManufacturingDate.Value = DateTime.Today;
            dtpExpiryDate.Value = DateTime.Today.AddYears(1);
            txtQuantity.Clear();
            txtPurchaseRate.Clear();
            txtMRP.Clear();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}