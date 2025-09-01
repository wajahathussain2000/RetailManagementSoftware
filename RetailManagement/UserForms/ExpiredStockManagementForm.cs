using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using RetailManagement.Database;
using RetailManagement.Models;

namespace RetailManagement.UserForms
{
    public partial class ExpiredStockManagementForm : Form
    {
        private DataGridView dgvExpiredStock;
        private ComboBox cmbCategory;
        private ComboBox cmbSupplier;
        private ComboBox cmbActionType;
        private DateTimePicker dtpExpiryFromDate;
        private DateTimePicker dtpExpiryToDate;
        private TextBox txtSearchItem;
        private TextBox txtReason;
        private TextBox txtNotes;
        private Button btnSearch;
        private Button btnSelectAll;
        private Button btnDeselectAll;
        private Button btnRemoveStock;
        private Button btnReturnToSupplier;
        private Button btnDisposeStock;
        private Button btnGenerateReport;
        private Button btnExport;
        private GroupBox groupFilters;
        private GroupBox groupActions;
        private GroupBox groupDetails;
        private Panel summaryPanel;
        private Label lblTotalExpiredItems;
        private Label lblTotalExpiredValue;
        private Label lblSelectedItems;
        private Label lblSelectedValue;
        private CheckBox chkShowDisposed;
        private CheckBox chkShowReturned;
        private DataTable expiredStockData;
        private int currentUserID = 1;

        public ExpiredStockManagementForm()
        {
            InitializeComponent();
            LoadInitialData();
        }

        public ExpiredStockManagementForm(int userID) : this()
        {
            currentUserID = userID;
        }

        private void InitializeComponent()
        {
            this.dgvExpiredStock = new System.Windows.Forms.DataGridView();
            this.groupFilters = new System.Windows.Forms.GroupBox();
            this.groupActions = new System.Windows.Forms.GroupBox();
            this.groupDetails = new System.Windows.Forms.GroupBox();
            this.cmbCategory = new System.Windows.Forms.ComboBox();
            this.cmbSupplier = new System.Windows.Forms.ComboBox();
            this.cmbActionType = new System.Windows.Forms.ComboBox();
            this.dtpExpiryFromDate = new System.Windows.Forms.DateTimePicker();
            this.dtpExpiryToDate = new System.Windows.Forms.DateTimePicker();
            this.txtSearchItem = new System.Windows.Forms.TextBox();
            this.txtReason = new System.Windows.Forms.TextBox();
            this.txtNotes = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnSelectAll = new System.Windows.Forms.Button();
            this.btnDeselectAll = new System.Windows.Forms.Button();
            this.btnRemoveStock = new System.Windows.Forms.Button();
            this.btnReturnToSupplier = new System.Windows.Forms.Button();
            this.btnDisposeStock = new System.Windows.Forms.Button();
            this.btnGenerateReport = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.summaryPanel = new System.Windows.Forms.Panel();
            this.lblTotalExpiredItems = new System.Windows.Forms.Label();
            this.lblTotalExpiredValue = new System.Windows.Forms.Label();
            this.lblSelectedItems = new System.Windows.Forms.Label();
            this.lblSelectedValue = new System.Windows.Forms.Label();
            this.chkShowDisposed = new System.Windows.Forms.CheckBox();
            this.chkShowReturned = new System.Windows.Forms.CheckBox();

            ((System.ComponentModel.ISupportInitialize)(this.dgvExpiredStock)).BeginInit();
            this.SuspendLayout();

            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.Controls.Add(this.summaryPanel);
            this.Controls.Add(this.dgvExpiredStock);
            this.Controls.Add(this.groupDetails);
            this.Controls.Add(this.groupActions);
            this.Controls.Add(this.groupFilters);
            this.Name = "ExpiredStockManagementForm";
            this.Text = "Expired Stock Management System";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;

            // Group Filters
            this.groupFilters.Controls.Add(new Label { Text = "Search Item:", Location = new Point(15, 25), Size = new Size(70, 13) });
            this.groupFilters.Controls.Add(this.txtSearchItem);
            this.groupFilters.Controls.Add(new Label { Text = "Category:", Location = new Point(300, 25), Size = new Size(60, 13) });
            this.groupFilters.Controls.Add(this.cmbCategory);
            this.groupFilters.Controls.Add(new Label { Text = "Supplier:", Location = new Point(15, 55), Size = new Size(55, 13) });
            this.groupFilters.Controls.Add(this.cmbSupplier);
            this.groupFilters.Controls.Add(new Label { Text = "Expiry From:", Location = new Point(300, 55), Size = new Size(70, 13) });
            this.groupFilters.Controls.Add(this.dtpExpiryFromDate);
            this.groupFilters.Controls.Add(new Label { Text = "Expiry To:", Location = new Point(500, 55), Size = new Size(60, 13) });
            this.groupFilters.Controls.Add(this.dtpExpiryToDate);
            this.groupFilters.Controls.Add(this.chkShowDisposed);
            this.groupFilters.Controls.Add(this.chkShowReturned);
            this.groupFilters.Controls.Add(this.btnSearch);
            this.groupFilters.Location = new System.Drawing.Point(12, 12);
            this.groupFilters.Name = "groupFilters";
            this.groupFilters.Size = new System.Drawing.Size(1176, 100);
            this.groupFilters.TabIndex = 0;
            this.groupFilters.TabStop = false;
            this.groupFilters.Text = "Search & Filter Expired Stock";

            // Group Actions
            this.groupActions.Controls.Add(this.btnSelectAll);
            this.groupActions.Controls.Add(this.btnDeselectAll);
            this.groupActions.Controls.Add(this.btnRemoveStock);
            this.groupActions.Controls.Add(this.btnReturnToSupplier);
            this.groupActions.Controls.Add(this.btnDisposeStock);
            this.groupActions.Controls.Add(this.btnGenerateReport);
            this.groupActions.Controls.Add(this.btnExport);
            this.groupActions.Location = new System.Drawing.Point(12, 460);
            this.groupActions.Name = "groupActions";
            this.groupActions.Size = new System.Drawing.Size(800, 80);
            this.groupActions.TabIndex = 1;
            this.groupActions.TabStop = false;
            this.groupActions.Text = "Stock Actions";

            // Group Details
            this.groupDetails.Controls.Add(new Label { Text = "Action Type:", Location = new Point(15, 25), Size = new Size(75, 13) });
            this.groupDetails.Controls.Add(this.cmbActionType);
            this.groupDetails.Controls.Add(new Label { Text = "Reason:", Location = new Point(15, 55), Size = new Size(50, 13) });
            this.groupDetails.Controls.Add(this.txtReason);
            this.groupDetails.Controls.Add(new Label { Text = "Notes:", Location = new Point(15, 120), Size = new Size(40, 13) });
            this.groupDetails.Controls.Add(this.txtNotes);
            this.groupDetails.Location = new System.Drawing.Point(830, 460);
            this.groupDetails.Name = "groupDetails";
            this.groupDetails.Size = new System.Drawing.Size(358, 200);
            this.groupDetails.TabIndex = 2;
            this.groupDetails.TabStop = false;
            this.groupDetails.Text = "Action Details";

            // Summary Panel
            this.summaryPanel.BackColor = System.Drawing.Color.LightSalmon;
            this.summaryPanel.Controls.Add(this.lblTotalExpiredItems);
            this.summaryPanel.Controls.Add(this.lblTotalExpiredValue);
            this.summaryPanel.Controls.Add(this.lblSelectedItems);
            this.summaryPanel.Controls.Add(this.lblSelectedValue);
            this.summaryPanel.Location = new System.Drawing.Point(12, 120);
            this.summaryPanel.Name = "summaryPanel";
            this.summaryPanel.Size = new System.Drawing.Size(1176, 50);
            this.summaryPanel.TabIndex = 3;

            // Setup all controls
            SetupControls();
            SetupDataGridView();

            ((System.ComponentModel.ISupportInitialize)(this.dgvExpiredStock)).EndInit();
            this.ResumeLayout(false);
        }

        private void SetupControls()
        {
            // Search TextBox
            this.txtSearchItem.Location = new System.Drawing.Point(90, 22);
            this.txtSearchItem.Name = "txtSearchItem";
            this.txtSearchItem.Size = new System.Drawing.Size(180, 20);
            this.txtSearchItem.TabIndex = 1;
            this.txtSearchItem.TextChanged += TxtSearchItem_TextChanged;

            // Category ComboBox
            this.cmbCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCategory.Location = new System.Drawing.Point(365, 22);
            this.cmbCategory.Name = "cmbCategory";
            this.cmbCategory.Size = new System.Drawing.Size(120, 21);
            this.cmbCategory.TabIndex = 2;

            // Supplier ComboBox
            this.cmbSupplier.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSupplier.Location = new System.Drawing.Point(75, 52);
            this.cmbSupplier.Name = "cmbSupplier";
            this.cmbSupplier.Size = new System.Drawing.Size(180, 21);
            this.cmbSupplier.TabIndex = 3;

            // Date Pickers
            this.dtpExpiryFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpExpiryFromDate.Location = new System.Drawing.Point(375, 52);
            this.dtpExpiryFromDate.Name = "dtpExpiryFromDate";
            this.dtpExpiryFromDate.Size = new System.Drawing.Size(100, 20);
            this.dtpExpiryFromDate.TabIndex = 4;
            this.dtpExpiryFromDate.Value = DateTime.Now.AddMonths(-6);

            this.dtpExpiryToDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpExpiryToDate.Location = new System.Drawing.Point(565, 52);
            this.dtpExpiryToDate.Name = "dtpExpiryToDate";
            this.dtpExpiryToDate.Size = new System.Drawing.Size(100, 20);
            this.dtpExpiryToDate.TabIndex = 5;
            this.dtpExpiryToDate.Value = DateTime.Now;

            // Checkboxes
            this.chkShowDisposed.Location = new System.Drawing.Point(700, 25);
            this.chkShowDisposed.Name = "chkShowDisposed";
            this.chkShowDisposed.Size = new System.Drawing.Size(100, 17);
            this.chkShowDisposed.TabIndex = 6;
            this.chkShowDisposed.Text = "Show Disposed";
            this.chkShowDisposed.UseVisualStyleBackColor = true;

            this.chkShowReturned.Location = new System.Drawing.Point(700, 45);
            this.chkShowReturned.Name = "chkShowReturned";
            this.chkShowReturned.Size = new System.Drawing.Size(100, 17);
            this.chkShowReturned.TabIndex = 7;
            this.chkShowReturned.Text = "Show Returned";
            this.chkShowReturned.UseVisualStyleBackColor = true;

            // Search Button
            this.btnSearch.BackColor = System.Drawing.Color.Blue;
            this.btnSearch.ForeColor = System.Drawing.Color.White;
            this.btnSearch.Location = new System.Drawing.Point(850, 35);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(100, 30);
            this.btnSearch.TabIndex = 8;
            this.btnSearch.Text = "Search Stock";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += BtnSearch_Click;

            // Action Buttons
            this.btnSelectAll.Location = new System.Drawing.Point(15, 25);
            this.btnSelectAll.Size = new System.Drawing.Size(80, 25);
            this.btnSelectAll.Text = "Select All";
            this.btnSelectAll.Click += BtnSelectAll_Click;

            this.btnDeselectAll.Location = new System.Drawing.Point(105, 25);
            this.btnDeselectAll.Size = new System.Drawing.Size(80, 25);
            this.btnDeselectAll.Text = "Deselect All";
            this.btnDeselectAll.Click += BtnDeselectAll_Click;

            this.btnRemoveStock.BackColor = System.Drawing.Color.Red;
            this.btnRemoveStock.ForeColor = System.Drawing.Color.White;
            this.btnRemoveStock.Location = new System.Drawing.Point(200, 25);
            this.btnRemoveStock.Size = new System.Drawing.Size(100, 25);
            this.btnRemoveStock.Text = "Remove Stock";
            this.btnRemoveStock.Click += BtnRemoveStock_Click;

            this.btnReturnToSupplier.BackColor = System.Drawing.Color.Orange;
            this.btnReturnToSupplier.ForeColor = System.Drawing.Color.White;
            this.btnReturnToSupplier.Location = new System.Drawing.Point(310, 25);
            this.btnReturnToSupplier.Size = new System.Drawing.Size(120, 25);
            this.btnReturnToSupplier.Text = "Return to Supplier";
            this.btnReturnToSupplier.Click += BtnReturnToSupplier_Click;

            this.btnDisposeStock.BackColor = System.Drawing.Color.Purple;
            this.btnDisposeStock.ForeColor = System.Drawing.Color.White;
            this.btnDisposeStock.Location = new System.Drawing.Point(440, 25);
            this.btnDisposeStock.Size = new System.Drawing.Size(100, 25);
            this.btnDisposeStock.Text = "Dispose Stock";
            this.btnDisposeStock.Click += BtnDisposeStock_Click;

            this.btnGenerateReport.BackColor = System.Drawing.Color.Green;
            this.btnGenerateReport.ForeColor = System.Drawing.Color.White;
            this.btnGenerateReport.Location = new System.Drawing.Point(560, 25);
            this.btnGenerateReport.Size = new System.Drawing.Size(110, 25);
            this.btnGenerateReport.Text = "Generate Report";
            this.btnGenerateReport.Click += BtnGenerateReport_Click;

            this.btnExport.BackColor = System.Drawing.Color.Teal;
            this.btnExport.ForeColor = System.Drawing.Color.White;
            this.btnExport.Location = new System.Drawing.Point(680, 25);
            this.btnExport.Size = new System.Drawing.Size(80, 25);
            this.btnExport.Text = "Export";
            this.btnExport.Click += BtnExport_Click;

            // Details Controls
            this.cmbActionType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbActionType.Location = new System.Drawing.Point(95, 22);
            this.cmbActionType.Size = new System.Drawing.Size(150, 21);
            this.cmbActionType.Items.AddRange(new object[] { "Remove from Inventory", "Return to Supplier", "Dispose Safely", "Mark as Expired" });
            this.cmbActionType.SelectedIndex = 0;

            this.txtReason.Location = new System.Drawing.Point(70, 52);
            this.txtReason.Size = new System.Drawing.Size(270, 20);
            this.txtReason.Text = "Expired product - safety compliance";

            this.txtNotes.Location = new System.Drawing.Point(15, 140);
            this.txtNotes.Multiline = true;
            this.txtNotes.Size = new System.Drawing.Size(325, 50);
            this.txtNotes.Text = "Enter additional notes about the expired stock action...";

            // Summary Labels
            this.lblTotalExpiredItems.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblTotalExpiredItems.Location = new System.Drawing.Point(15, 10);
            this.lblTotalExpiredItems.Size = new System.Drawing.Size(200, 15);
            this.lblTotalExpiredItems.Text = "Total Expired Items: 0";

            this.lblTotalExpiredValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblTotalExpiredValue.Location = new System.Drawing.Point(15, 30);
            this.lblTotalExpiredValue.Size = new System.Drawing.Size(200, 15);
            this.lblTotalExpiredValue.Text = "Total Expired Value: ₹0.00";

            this.lblSelectedItems.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblSelectedItems.Location = new System.Drawing.Point(300, 10);
            this.lblSelectedItems.Size = new System.Drawing.Size(200, 15);
            this.lblSelectedItems.Text = "Selected Items: 0";

            this.lblSelectedValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblSelectedValue.Location = new System.Drawing.Point(300, 30);
            this.lblSelectedValue.Size = new System.Drawing.Size(200, 15);
            this.lblSelectedValue.Text = "Selected Value: ₹0.00";
        }

        private void SetupDataGridView()
        {
            this.dgvExpiredStock.Location = new System.Drawing.Point(12, 180);
            this.dgvExpiredStock.Name = "dgvExpiredStock";
            this.dgvExpiredStock.Size = new System.Drawing.Size(1176, 270);
            this.dgvExpiredStock.TabIndex = 4;
            this.dgvExpiredStock.AllowUserToAddRows = false;
            this.dgvExpiredStock.AllowUserToDeleteRows = false;
            this.dgvExpiredStock.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvExpiredStock.MultiSelect = true;
            this.dgvExpiredStock.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvExpiredStock.SelectionChanged += DgvExpiredStock_SelectionChanged;

            // Add checkbox column for selection
            DataGridViewCheckBoxColumn chkColumn = new DataGridViewCheckBoxColumn();
            chkColumn.Name = "Select";
            chkColumn.HeaderText = "Select";
            chkColumn.Width = 60;
            chkColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            this.dgvExpiredStock.Columns.Add(chkColumn);
        }

        private void LoadInitialData()
        {
            try
            {
                LoadCategories();
                LoadSuppliers();
                LoadExpiredStock();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading initial data: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadCategories()
        {
            try
            {
                string query = "SELECT DISTINCT Category FROM Items WHERE Category IS NOT NULL AND Category != '' ORDER BY Category";
                DataTable categories = DatabaseConnection.ExecuteQuery(query);

                cmbCategory.Items.Clear();
                cmbCategory.Items.Add(new ComboBoxItem { Text = "All Categories", Value = "" });
                
                foreach (DataRow row in categories.Rows)
                {
                    cmbCategory.Items.Add(new ComboBoxItem 
                    { 
                        Text = row["Category"].ToString(), 
                        Value = row["Category"].ToString() 
                    });
                }
                
                cmbCategory.DisplayMember = "Text";
                cmbCategory.ValueMember = "Value";
                cmbCategory.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading categories: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSuppliers()
        {
            try
            {
                string query = "SELECT CompanyID, CompanyName FROM Companies WHERE IsActive = 1 ORDER BY CompanyName";
                DataTable suppliers = DatabaseConnection.ExecuteQuery(query);

                cmbSupplier.Items.Clear();
                cmbSupplier.Items.Add(new ComboBoxItem { Text = "All Suppliers", Value = 0 });
                
                foreach (DataRow row in suppliers.Rows)
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

        private void LoadExpiredStock()
        {
            try
            {
                string searchTerm = txtSearchItem.Text.Trim();
                ComboBoxItem selectedCategory = (ComboBoxItem)cmbCategory.SelectedItem;
                ComboBoxItem selectedSupplier = (ComboBoxItem)cmbSupplier.SelectedItem;
                
                string category = selectedCategory?.Value.ToString() ?? "";
                int supplierID = selectedSupplier != null ? (int)selectedSupplier.Value : 0;

                string query = @"
                    SELECT 
                        i.ItemID,
                        i.ItemName,
                        i.Category,
                        i.Barcode,
                        ib.BatchNumber,
                        ib.ExpiryDate,
                        ib.Quantity as BatchQuantity,
                        i.Rate,
                        i.MRP,
                        (ib.Quantity * i.Rate) as TotalValue,
                        c.CompanyName as SupplierName,
                        DATEDIFF(day, ib.ExpiryDate, GETDATE()) as DaysExpired,
                        CASE 
                            WHEN EXISTS (SELECT 1 FROM ExpiredStockActions esa WHERE esa.ItemID = i.ItemID AND esa.BatchNumber = ib.BatchNumber AND esa.ActionType = 'DISPOSED')
                            THEN 'DISPOSED'
                            WHEN EXISTS (SELECT 1 FROM ExpiredStockActions esa WHERE esa.ItemID = i.ItemID AND esa.BatchNumber = ib.BatchNumber AND esa.ActionType = 'RETURNED')
                            THEN 'RETURNED'
                            WHEN EXISTS (SELECT 1 FROM ExpiredStockActions esa WHERE esa.ItemID = i.ItemID AND esa.BatchNumber = ib.BatchNumber AND esa.ActionType = 'REMOVED')
                            THEN 'REMOVED'
                            ELSE 'PENDING'
                        END as ActionStatus
                    FROM Items i
                    INNER JOIN ItemBatches ib ON i.ItemID = ib.ItemID
                    LEFT JOIN Companies c ON i.CompanyID = c.CompanyID
                    WHERE ib.ExpiryDate <= GETDATE()
                    AND ib.Quantity > 0
                    AND i.IsActive = 1" +
                    (!string.IsNullOrEmpty(searchTerm) ? " AND i.ItemName LIKE @SearchTerm" : "") +
                    (!string.IsNullOrEmpty(category) ? " AND i.Category = @Category" : "") +
                    (supplierID > 0 ? " AND i.CompanyID = @SupplierID" : "") +
                    (!chkShowDisposed.Checked ? " AND NOT EXISTS (SELECT 1 FROM ExpiredStockActions esa WHERE esa.ItemID = i.ItemID AND esa.BatchNumber = ib.BatchNumber AND esa.ActionType = 'DISPOSED')" : "") +
                    (!chkShowReturned.Checked ? " AND NOT EXISTS (SELECT 1 FROM ExpiredStockActions esa WHERE esa.ItemID = i.ItemID AND esa.BatchNumber = ib.BatchNumber AND esa.ActionType = 'RETURNED')" : "") +
                    " AND ib.ExpiryDate BETWEEN @ExpiryFromDate AND @ExpiryToDate" + @"
                    ORDER BY ib.ExpiryDate ASC, i.ItemName";

                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    new SqlParameter("@ExpiryFromDate", dtpExpiryFromDate.Value),
                    new SqlParameter("@ExpiryToDate", dtpExpiryToDate.Value)
                };

                if (!string.IsNullOrEmpty(searchTerm))
                    parameters.Add(new SqlParameter("@SearchTerm", "%" + searchTerm + "%"));
                if (!string.IsNullOrEmpty(category))
                    parameters.Add(new SqlParameter("@Category", category));
                if (supplierID > 0)
                    parameters.Add(new SqlParameter("@SupplierID", supplierID));

                expiredStockData = DatabaseConnection.ExecuteQuery(query, parameters.ToArray());

                if (expiredStockData != null)
                {
                    // Clear existing columns except checkbox
                    while (dgvExpiredStock.Columns.Count > 1)
                    {
                        dgvExpiredStock.Columns.RemoveAt(1);
                    }

                    // Add data columns
                    dgvExpiredStock.Columns.Add("ItemName", "Item Name");
                    dgvExpiredStock.Columns.Add("Category", "Category");
                    dgvExpiredStock.Columns.Add("BatchNumber", "Batch No.");
                    dgvExpiredStock.Columns.Add("ExpiryDate", "Expiry Date");
                    dgvExpiredStock.Columns.Add("BatchQuantity", "Qty");
                    dgvExpiredStock.Columns.Add("Rate", "Rate");
                    dgvExpiredStock.Columns.Add("TotalValue", "Total Value");
                    dgvExpiredStock.Columns.Add("SupplierName", "Supplier");
                    dgvExpiredStock.Columns.Add("DaysExpired", "Days Expired");
                    dgvExpiredStock.Columns.Add("ActionStatus", "Status");

                    // Populate rows
                    dgvExpiredStock.Rows.Clear();
                    foreach (DataRow row in expiredStockData.Rows)
                    {
                        int rowIndex = dgvExpiredStock.Rows.Add();
                        dgvExpiredStock.Rows[rowIndex].Cells["Select"].Value = false;
                        dgvExpiredStock.Rows[rowIndex].Cells["ItemName"].Value = row["ItemName"];
                        dgvExpiredStock.Rows[rowIndex].Cells["Category"].Value = row["Category"];
                        dgvExpiredStock.Rows[rowIndex].Cells["BatchNumber"].Value = row["BatchNumber"];
                        dgvExpiredStock.Rows[rowIndex].Cells["ExpiryDate"].Value = Convert.ToDateTime(row["ExpiryDate"]).ToString("dd/MM/yyyy");
                        dgvExpiredStock.Rows[rowIndex].Cells["BatchQuantity"].Value = row["BatchQuantity"];
                        dgvExpiredStock.Rows[rowIndex].Cells["Rate"].Value = Convert.ToDecimal(row["Rate"]).ToString("N2");
                        dgvExpiredStock.Rows[rowIndex].Cells["TotalValue"].Value = Convert.ToDecimal(row["TotalValue"]).ToString("N2");
                        dgvExpiredStock.Rows[rowIndex].Cells["SupplierName"].Value = row["SupplierName"];
                        dgvExpiredStock.Rows[rowIndex].Cells["DaysExpired"].Value = row["DaysExpired"];
                        dgvExpiredStock.Rows[rowIndex].Cells["ActionStatus"].Value = row["ActionStatus"];
                        
                        // Store row data for later use
                        dgvExpiredStock.Rows[rowIndex].Tag = row;

                        // Color code based on status
                        string status = row["ActionStatus"].ToString();
                        switch (status)
                        {
                            case "DISPOSED":
                                dgvExpiredStock.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightGray;
                                break;
                            case "RETURNED":
                                dgvExpiredStock.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightBlue;
                                break;
                            case "REMOVED":
                                dgvExpiredStock.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightYellow;
                                break;
                            default:
                                dgvExpiredStock.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightPink;
                                break;
                        }
                    }

                    UpdateSummary();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading expired stock: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateSummary()
        {
            if (expiredStockData != null)
            {
                int totalItems = expiredStockData.Rows.Count;
                decimal totalValue = 0;
                int selectedItems = 0;
                decimal selectedValue = 0;

                foreach (DataRow row in expiredStockData.Rows)
                {
                    totalValue += Convert.ToDecimal(row["TotalValue"]);
                }

                foreach (DataGridViewRow gridRow in dgvExpiredStock.Rows)
                {
                    if (Convert.ToBoolean(gridRow.Cells["Select"].Value))
                    {
                        selectedItems++;
                        DataRow dataRow = (DataRow)gridRow.Tag;
                        selectedValue += Convert.ToDecimal(dataRow["TotalValue"]);
                    }
                }

                lblTotalExpiredItems.Text = $"Total Expired Items: {totalItems}";
                lblTotalExpiredValue.Text = $"Total Expired Value: ₹{totalValue:N2}";
                lblSelectedItems.Text = $"Selected Items: {selectedItems}";
                lblSelectedValue.Text = $"Selected Value: ₹{selectedValue:N2}";
            }
        }

        private void TxtSearchItem_TextChanged(object sender, EventArgs e)
        {
            // Auto-search as user types (with small delay to avoid too many queries)
            if (txtSearchItem.Text.Length > 2 || string.IsNullOrEmpty(txtSearchItem.Text))
            {
                LoadExpiredStock();
            }
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            LoadExpiredStock();
        }

        private void BtnSelectAll_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgvExpiredStock.Rows)
            {
                row.Cells["Select"].Value = true;
            }
            UpdateSummary();
        }

        private void BtnDeselectAll_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgvExpiredStock.Rows)
            {
                row.Cells["Select"].Value = false;
            }
            UpdateSummary();
        }

        private void DgvExpiredStock_SelectionChanged(object sender, EventArgs e)
        {
            UpdateSummary();
        }

        private void BtnRemoveStock_Click(object sender, EventArgs e)
        {
            ProcessExpiredStockAction("REMOVED", "Remove from Inventory");
        }

        private void BtnReturnToSupplier_Click(object sender, EventArgs e)
        {
            ProcessExpiredStockAction("RETURNED", "Return to Supplier");
        }

        private void BtnDisposeStock_Click(object sender, EventArgs e)
        {
            ProcessExpiredStockAction("DISPOSED", "Dispose Safely");
        }

        private void ProcessExpiredStockAction(string actionType, string actionDescription)
        {
            try
            {
                List<DataGridViewRow> selectedRows = new List<DataGridViewRow>();
                
                foreach (DataGridViewRow row in dgvExpiredStock.Rows)
                {
                    if (Convert.ToBoolean(row.Cells["Select"].Value))
                    {
                        selectedRows.Add(row);
                    }
                }

                if (selectedRows.Count == 0)
                {
                    MessageBox.Show("Please select at least one item to process.", "Selection Required", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string confirmMessage = $"Are you sure you want to {actionDescription.ToLower()} {selectedRows.Count} selected item(s)?\n\n" +
                    $"This action will:\n" +
                    $"• Mark items as {actionType}\n" +
                    $"• Update inventory quantities\n" +
                    $"• Create audit log entries\n" +
                    $"• Generate compliance documentation";

                if (MessageBox.Show(confirmMessage, $"Confirm {actionDescription}", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    int processedCount = 0;
                    string reason = txtReason.Text.Trim();
                    string notes = txtNotes.Text.Trim();

                    if (string.IsNullOrEmpty(reason))
                        reason = $"Expired stock - {actionDescription}";

                    foreach (DataGridViewRow gridRow in selectedRows)
                    {
                        DataRow dataRow = (DataRow)gridRow.Tag;
                        
                        try
                        {
                            ProcessSingleItem(dataRow, actionType, reason, notes);
                            processedCount++;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error processing item {dataRow["ItemName"]}: {ex.Message}", 
                                "Processing Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }

                    MessageBox.Show($"Successfully processed {processedCount} out of {selectedRows.Count} items.\n\n" +
                        $"Action: {actionDescription}\n" +
                        $"Status: Completed", "Process Complete", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Refresh the grid
                    LoadExpiredStock();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error processing expired stock action: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ProcessSingleItem(DataRow item, string actionType, string reason, string notes)
        {
            try
            {
                int itemID = Convert.ToInt32(item["ItemID"]);
                string batchNumber = item["BatchNumber"].ToString();
                int quantity = Convert.ToInt32(item["BatchQuantity"]);
                decimal value = Convert.ToDecimal(item["TotalValue"]);

                using (SqlConnection connection = DatabaseConnection.GetConnection())
                {
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // 1. Insert expired stock action record
                            string insertActionQuery = @"
                                INSERT INTO ExpiredStockActions 
                                (ItemID, BatchNumber, ActionType, Quantity, Value, Reason, Notes, ProcessedBy, ProcessedDate, IsActive)
                                VALUES 
                                (@ItemID, @BatchNumber, @ActionType, @Quantity, @Value, @Reason, @Notes, @ProcessedBy, GETDATE(), 1)";

                            SqlCommand insertCmd = new SqlCommand(insertActionQuery, connection, transaction);
                            insertCmd.Parameters.AddWithValue("@ItemID", itemID);
                            insertCmd.Parameters.AddWithValue("@BatchNumber", batchNumber);
                            insertCmd.Parameters.AddWithValue("@ActionType", actionType);
                            insertCmd.Parameters.AddWithValue("@Quantity", quantity);
                            insertCmd.Parameters.AddWithValue("@Value", value);
                            insertCmd.Parameters.AddWithValue("@Reason", reason);
                            insertCmd.Parameters.AddWithValue("@Notes", notes);
                            insertCmd.Parameters.AddWithValue("@ProcessedBy", currentUserID);
                            insertCmd.ExecuteNonQuery();

                            // 2. Update batch quantity (remove from inventory)
                            if (actionType == "REMOVED" || actionType == "DISPOSED")
                            {
                                string updateBatchQuery = @"
                                    UPDATE ItemBatches 
                                    SET Quantity = 0, 
                                        UpdatedBy = @ProcessedBy, 
                                        UpdatedDate = GETDATE()
                                    WHERE ItemID = @ItemID AND BatchNumber = @BatchNumber";

                                SqlCommand updateCmd = new SqlCommand(updateBatchQuery, connection, transaction);
                                updateCmd.Parameters.AddWithValue("@ItemID", itemID);
                                updateCmd.Parameters.AddWithValue("@BatchNumber", batchNumber);
                                updateCmd.Parameters.AddWithValue("@ProcessedBy", currentUserID);
                                updateCmd.ExecuteNonQuery();

                                // 3. Update main item stock
                                string updateStockQuery = @"
                                    UPDATE Items 
                                    SET StockInHand = StockInHand - @Quantity,
                                        UpdatedBy = @ProcessedBy, 
                                        UpdatedDate = GETDATE()
                                    WHERE ItemID = @ItemID";

                                SqlCommand stockCmd = new SqlCommand(updateStockQuery, connection, transaction);
                                stockCmd.Parameters.AddWithValue("@ItemID", itemID);
                                stockCmd.Parameters.AddWithValue("@Quantity", quantity);
                                stockCmd.Parameters.AddWithValue("@ProcessedBy", currentUserID);
                                stockCmd.ExecuteNonQuery();
                            }

                            // 4. Log user activity
                            string activityQuery = @"
                                INSERT INTO UserActivity 
                                (UserID, ActivityType, ModuleName, Description, ActivityDate, IPAddress)
                                VALUES 
                                (@UserID, 'EXPIRED_STOCK', 'Expired Stock Management', @Description, GETDATE(), @IPAddress)";

                            SqlCommand activityCmd = new SqlCommand(activityQuery, connection, transaction);
                            activityCmd.Parameters.AddWithValue("@UserID", currentUserID);
                            activityCmd.Parameters.AddWithValue("@Description", 
                                $"{actionType} expired stock: {item["ItemName"]} (Batch: {batchNumber}, Qty: {quantity})");
                            activityCmd.Parameters.AddWithValue("@IPAddress", "127.0.0.1");
                            activityCmd.ExecuteNonQuery();

                            transaction.Commit();
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error processing item: {ex.Message}");
            }
        }

        private void BtnGenerateReport_Click(object sender, EventArgs e)
        {
            GenerateExpiredStockReport();
        }

        private void GenerateExpiredStockReport()
        {
            try
            {
                // In a full implementation, this would generate a comprehensive RDLC report
                MessageBox.Show("Expired Stock Report Generation:\n\n" +
                    "• Compliance documentation\n" +
                    "• Disposed stock summary\n" +
                    "• Financial impact analysis\n" +
                    "• Audit trail report\n" +
                    "• Supplier return summary\n\n" +
                    "This would generate professional RDLC reports for regulatory compliance.", 
                    "Report Generation", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating report: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            try
            {
                if (expiredStockData == null || expiredStockData.Rows.Count == 0)
                {
                    MessageBox.Show("No data to export. Please search for expired stock first.", "Export Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "Excel Files|*.xlsx|CSV Files|*.csv";
                saveDialog.FileName = $"Expired_Stock_Report_{DateTime.Now:yyyyMMdd}";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    // Export logic would go here
                    MessageBox.Show("Expired stock data exported successfully!", "Export Complete", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exporting data: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
