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
using System.IO;
using RetailManagement.Database;
using QRCoder;

namespace RetailManagement.UserForms
{
    public partial class NewPurchase : Form
    {
        private DataTable purchaseItems;
        private int selectedCompanyID = 0;
        private decimal totalAmount = 0;
        private int selectedItemID = 0;
        private string currentPackLooseMode = "P"; // P for Pack, L for Loose
        private decimal baseRate = 0;
        private int packSize = 1;
        private DateTimePicker dtpExpiry; // Expiry date picker
        private TextBox txtSearchItems;
        private Label lblSearchItems;
        private DataTable allItemsData; // Store all items for filtering

        public NewPurchase()
        {
            InitializeComponent();
            InitializeDataTable();
            LoadCompanies();
            LoadItems();
            SetupDataGridView();
            GeneratePurchaseNumber();
            CreateExpiryDatePicker();
            SetupEventHandlers();
            
            // Wire up button events
            button1.Click += btnAddItem_Click;
            btnSave.Click += btnSave_Click;
            btnCancel.Click += btnClear_Click;
            btnPrint.Click += btnPrint_Click;
            
            // Add delete button for data grid rows
            AddDeleteRowButton();
            
            // Enable buttons
            btnSave.Enabled = true;
            btnCancel.Enabled = true;
            btnNewItem.Enabled = true;
            
            // Initialize QR code and barcode - generate them on load
            GenerateInitialQRAndBarcode();
            btnRefresh.Enabled = true;
            
            // Add search and keyboard functionality
            CreateSearchControls();
            SetupKeyboardNavigation();
            
            // Adjust layout to prevent overlapping
            AdjustLayoutPositions();
        }

        private void SetupEventHandlers()
        {
            // Add event handler for company selection
            comboBox1.SelectedIndexChanged += ComboBox1_SelectedIndexChanged;
            
            // Add P/L toggle functionality
            textBox2.DoubleClick += TextBox2_DoubleClick;
            textBox2.Text = "P"; // Default to Pack
            
            // Add automatic calculation handlers
            textBox4.TextChanged += CalculateTotal; // Qty
            textBox3.TextChanged += CalculateTotal; // Rate
            textBox5.TextChanged += CalculateTotal; // Bonus
            textBox6.TextChanged += CalculateTotal; // Dis1
            textBox7.TextChanged += CalculateTotal; // Dis2
            textBox8.TextChanged += CalculateTotal; // Tax
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cmb = sender as ComboBox;
            if (cmb.SelectedValue != null && int.TryParse(cmb.SelectedValue.ToString(), out int companyId))
            {
                selectedCompanyID = companyId;
            }
        }

        private void InitializeDataTable()
        {
            purchaseItems = new DataTable();
            purchaseItems.Columns.Add("ItemID", typeof(int));
            purchaseItems.Columns.Add("ItemName", typeof(string));
            purchaseItems.Columns.Add("ExpiryDate", typeof(DateTime));
            purchaseItems.Columns.Add("PackLoose", typeof(string));
            purchaseItems.Columns.Add("Quantity", typeof(decimal));
            purchaseItems.Columns.Add("Rate", typeof(decimal));
            purchaseItems.Columns.Add("Bonus", typeof(decimal));
            purchaseItems.Columns.Add("Discount1", typeof(decimal));
            purchaseItems.Columns.Add("Discount2", typeof(decimal));
            purchaseItems.Columns.Add("Tax", typeof(decimal));
            purchaseItems.Columns.Add("TotalAmount", typeof(decimal));
        }

        private void SetupDataGridView()
        {
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.Columns.Clear();

            // Add columns with proper mapping
            dataGridView1.Columns.Add("ItemName", "Item Name");
            
            // Add DateTimePicker column for Expiry Date
            DataGridViewTextBoxColumn expiryColumn = new DataGridViewTextBoxColumn();
            expiryColumn.Name = "ExpiryDate";
            expiryColumn.HeaderText = "Expiry Date";
            expiryColumn.DataPropertyName = "ExpiryDate";
            expiryColumn.DefaultCellStyle.Format = "dd/MM/yyyy";
            expiryColumn.Width = 100;
            dataGridView1.Columns.Add(expiryColumn);
            
            dataGridView1.Columns.Add("PackLoose", "P/L");
            dataGridView1.Columns.Add("Quantity", "Qty");
            dataGridView1.Columns.Add("Rate", "Rate");
            dataGridView1.Columns.Add("Bonus", "Bonus");
            dataGridView1.Columns.Add("Discount1", "Dis1");
            dataGridView1.Columns.Add("Discount2", "Dis2");
            dataGridView1.Columns.Add("Tax", "Tax");
            dataGridView1.Columns.Add("TotalAmount", "Total");

            // Set data property names
            dataGridView1.Columns["ItemName"].DataPropertyName = "ItemName";
            dataGridView1.Columns["PackLoose"].DataPropertyName = "PackLoose";
            dataGridView1.Columns["Quantity"].DataPropertyName = "Quantity";
            dataGridView1.Columns["Rate"].DataPropertyName = "Rate";
            dataGridView1.Columns["Bonus"].DataPropertyName = "Bonus";
            dataGridView1.Columns["Discount1"].DataPropertyName = "Discount1";
            dataGridView1.Columns["Discount2"].DataPropertyName = "Discount2";
            dataGridView1.Columns["Tax"].DataPropertyName = "Tax";
            dataGridView1.Columns["TotalAmount"].DataPropertyName = "TotalAmount";

            // Make all columns editable except ItemName (which should be read-only)
            dataGridView1.Columns["ItemName"].ReadOnly = true;
            dataGridView1.Columns["ExpiryDate"].ReadOnly = false;
            dataGridView1.Columns["PackLoose"].ReadOnly = false;
            dataGridView1.Columns["Quantity"].ReadOnly = false;
            dataGridView1.Columns["Rate"].ReadOnly = false;
            dataGridView1.Columns["Bonus"].ReadOnly = false;
            dataGridView1.Columns["Discount1"].ReadOnly = false;
            dataGridView1.Columns["Discount2"].ReadOnly = false;
            dataGridView1.Columns["Tax"].ReadOnly = false;
            dataGridView1.Columns["TotalAmount"].ReadOnly = true; // Auto-calculated, so read-only

            // Enable editing on the DataGridView
            dataGridView1.EditMode = DataGridViewEditMode.EditOnEnter;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = true;
            dataGridView1.MultiSelect = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.CellSelect;

            // Add event handlers for editing
            dataGridView1.CellDoubleClick += DataGridView1_CellDoubleClick;
            dataGridView1.CellValueChanged += DataGridView1_CellValueChanged;
            dataGridView1.CellEndEdit += DataGridView1_CellEndEdit;
            dataGridView1.CellClick += DataGridView1_CellClick;
            dataGridView1.KeyDown += DataGridView1_KeyDown;
            dataGridView1.UserDeletingRow += DataGridView1_UserDeletingRow;

            // Bind the DataTable to the DataGridView
            dataGridView1.DataSource = purchaseItems;

            // Set column widths
            dataGridView1.Columns["ItemName"].Width = 180;
            dataGridView1.Columns["PackLoose"].Width = 35;
            dataGridView1.Columns["Quantity"].Width = 50;
            dataGridView1.Columns["Rate"].Width = 60;
            dataGridView1.Columns["Bonus"].Width = 50;
            dataGridView1.Columns["Discount1"].Width = 50;
            dataGridView1.Columns["Discount2"].Width = 50;
            dataGridView1.Columns["Tax"].Width = 50;
            dataGridView1.Columns["TotalAmount"].Width = 70;
        }

        private void GeneratePurchaseNumber()
        {
            try
            {
                string query = "SELECT ISNULL(MAX(CAST(SUBSTRING(PurchaseNumber, 4, LEN(PurchaseNumber)) AS INT)), 0) + 1 FROM Purchases";
                object result = DatabaseConnection.ExecuteScalar(query);
                int nextNumber = Convert.ToInt32(result);
                textBox11.Text = $"PO{nextNumber:D6}";
            }
            catch (Exception ex)
            {
                textBox11.Text = $"PO{DateTime.Now:yyyyMMdd}001";
            }
        }

        private void LoadCompanies()
        {
            try
            {
                string query = "SELECT CompanyID, CompanyName, ContactPerson FROM Companies WHERE IsActive = 1 ORDER BY CompanyName";
                DataTable dt = DatabaseConnection.ExecuteQuery(query);
                comboBox1.DataSource = dt;
                comboBox1.DisplayMember = "CompanyName";
                comboBox1.ValueMember = "CompanyID";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading companies: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadItems()
        {
            try
            {
                string stockColumnName = DatabaseConnection.GetStockColumnName();
                string query = $@"SELECT ItemID, ItemName, Price, {stockColumnName} as StockQuantity, 
                                 ISNULL(Barcode, '') as Barcode, 
                                 ISNULL(Category, '') as Category
                                 FROM Items WHERE IsActive = 1 ORDER BY ItemName";
                DataTable dt = DatabaseConnection.ExecuteQuery(query);
                
                // Store all items for search filtering
                allItemsData = dt.Copy();
                
                // Bind items to the listBoxItems control
                listBoxItems.DataSource = dt;
                listBoxItems.DisplayMember = "ItemName";
                listBoxItems.ValueMember = "ItemID";
                
                // Add click event handler for item selection
                listBoxItems.Click += ListBoxItems_Click;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading items: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ListBoxItems_Click(object sender, EventArgs e)
        {
            if (listBoxItems.SelectedValue != null)
            {
                LoadSelectedItemDetails();
            }
        }
        
        /// <summary>
        /// Create and position the expiry date picker
        /// </summary>
        private void CreateExpiryDatePicker()
        {
            // Remove textBox9 if it exists and replace with DateTimePicker
            if (groupBox1.Controls.Contains(textBox9))
            {
                groupBox1.Controls.Remove(textBox9);
            }
            
            dtpExpiry = new DateTimePicker();
            dtpExpiry.Format = DateTimePickerFormat.Short;
            dtpExpiry.Location = new Point(738, 57); // Same position as textBox9
            dtpExpiry.Size = new Size(105, 26);
            dtpExpiry.Value = DateTime.Now.AddYears(2); // Default 2 years from now
            dtpExpiry.Font = new Font("Microsoft Sans Serif", 8.25f);
            dtpExpiry.Name = "dtpExpiry";
            
            // Add to the groupBox1 (same container as other input fields)
            groupBox1.Controls.Add(dtpExpiry);
            
            // Update the label to show "Expiry Date" instead of just "Expiry"
            label12.Text = "Expiry Date";
            
            // Make it more visible
            dtpExpiry.BackColor = Color.White;
            dtpExpiry.ForeColor = Color.Black;
            
            // Add event handler for date change
            dtpExpiry.ValueChanged += (sender, e) => {
                // Validate that expiry date is not in the past
                if (dtpExpiry.Value.Date < DateTime.Now.Date)
                {
                    MessageBox.Show("Expiry date cannot be in the past.", "Invalid Date", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dtpExpiry.Value = DateTime.Now.AddYears(2);
                }
            };
        }
        
        /// <summary>
        /// Handle grid cell click to show date picker for expiry date column
        /// </summary>
        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                string columnName = dataGridView1.Columns[e.ColumnIndex].Name;
                if (columnName == "ExpiryDate")
                {
                    // Create a temporary DateTimePicker for grid editing
                    DateTimePicker gridDatePicker = new DateTimePicker();
                    gridDatePicker.Format = DateTimePickerFormat.Short;
                    
                    Rectangle cellRect = dataGridView1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
                    gridDatePicker.Location = new Point(cellRect.X, cellRect.Y);
                    gridDatePicker.Size = cellRect.Size;
                    
                    // Set current value
                    object cellValue = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                    if (cellValue != null && cellValue != DBNull.Value)
                    {
                        gridDatePicker.Value = Convert.ToDateTime(cellValue);
                    }
                    else
                    {
                        gridDatePicker.Value = DateTime.Now.AddYears(2);
                    }
                    
                    // Add to grid
                    dataGridView1.Controls.Add(gridDatePicker);
                    gridDatePicker.BringToFront();
                    
                    // Handle value change
                    gridDatePicker.ValueChanged += (s, args) => {
                        dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = gridDatePicker.Value;
                        dataGridView1.Controls.Remove(gridDatePicker);
                        gridDatePicker.Dispose();
                    };
                    
                    // Handle lost focus
                    gridDatePicker.Leave += (s, args) => {
                        dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = gridDatePicker.Value;
                        dataGridView1.Controls.Remove(gridDatePicker);
                        gridDatePicker.Dispose();
                    };
                    
                    gridDatePicker.Focus();
                }
            }
        }
        
        /// <summary>
        /// P/L Toggle functionality - double click to switch between Pack and Loose
        /// </summary>
        private void TextBox2_DoubleClick(object sender, EventArgs e)
        {
            if (selectedItemID == 0)
            {
                MessageBox.Show("Please select an item first.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            // Toggle between P and L
            if (currentPackLooseMode == "P")
            {
                currentPackLooseMode = "L";
                textBox2.Text = "L";
            }
            else
            {
                currentPackLooseMode = "P";
                textBox2.Text = "P";
            }
            
            // Update rate based on new mode
            UpdateRateBasedOnMode();
            
            // Recalculate total
            CalculateTotal(null, null);
        }
        
        /// <summary>
        /// Update rate based on Pack/Loose mode
        /// </summary>
        private void UpdateRateBasedOnMode()
        {
            if (currentPackLooseMode == "P")
            {
                // Pack mode - use base rate
                textBox3.Text = baseRate.ToString("F2");
            }
            else
            {
                // Loose mode - divide by pack size
                decimal looseRate = packSize > 0 ? baseRate / packSize : baseRate;
                textBox3.Text = looseRate.ToString("F4"); // More decimal places for loose rate
            }
        }
        
        /// <summary>
        /// Automatic total calculation
        /// </summary>
        private void CalculateTotal(object sender, EventArgs e)
        {
            try
            {
                // Get values from textboxes
                decimal qty = string.IsNullOrEmpty(textBox4.Text) ? 0 : decimal.Parse(textBox4.Text);
                decimal rate = string.IsNullOrEmpty(textBox3.Text) ? 0 : decimal.Parse(textBox3.Text);
                decimal bonus = string.IsNullOrEmpty(textBox5.Text) ? 0 : decimal.Parse(textBox5.Text);
                decimal dis1 = string.IsNullOrEmpty(textBox6.Text) ? 0 : decimal.Parse(textBox6.Text);
                decimal dis2 = string.IsNullOrEmpty(textBox7.Text) ? 0 : decimal.Parse(textBox7.Text);
                decimal tax = string.IsNullOrEmpty(textBox8.Text) ? 0 : decimal.Parse(textBox8.Text);
                
                // Calculate: (Qty + Bonus) * Rate - Dis1 - Dis2 + Tax
                decimal totalQty = qty + bonus;
                decimal subtotal = totalQty * rate;
                decimal afterDiscounts = subtotal - dis1 - dis2;
                decimal finalTotal = afterDiscounts + tax;
                
                // Ensure total is not negative
                if (finalTotal < 0) finalTotal = 0;
                
                // Update total textbox
                textBox10.Text = finalTotal.ToString("F2");
            }
            catch
            {
                // If parsing fails, set total to 0
                textBox10.Text = "0.00";
            }
        }
        
        /// <summary>
        /// Grid cell value changed event for real-time calculation
        /// </summary>
        private void DataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                RecalculateGridRowTotal(e.RowIndex);
                CalculateTotals();
            }
        }
        
        /// <summary>
        /// Grid cell end edit event
        /// </summary>
        private void DataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                RecalculateGridRowTotal(e.RowIndex);
                CalculateTotals();
            }
        }
        
        /// <summary>
        /// Recalculate total for a specific grid row
        /// </summary>
        private void RecalculateGridRowTotal(int rowIndex)
        {
            try
            {
                DataGridViewRow row = dataGridView1.Rows[rowIndex];
                
                decimal qty = row.Cells["Quantity"].Value == null ? 0 : Convert.ToDecimal(row.Cells["Quantity"].Value);
                decimal rate = row.Cells["Rate"].Value == null ? 0 : Convert.ToDecimal(row.Cells["Rate"].Value);
                decimal bonus = row.Cells["Bonus"].Value == null ? 0 : Convert.ToDecimal(row.Cells["Bonus"].Value);
                decimal dis1 = row.Cells["Discount1"].Value == null ? 0 : Convert.ToDecimal(row.Cells["Discount1"].Value);
                decimal dis2 = row.Cells["Discount2"].Value == null ? 0 : Convert.ToDecimal(row.Cells["Discount2"].Value);
                decimal tax = row.Cells["Tax"].Value == null ? 0 : Convert.ToDecimal(row.Cells["Tax"].Value);
                
                decimal totalQty = qty + bonus;
                decimal subtotal = totalQty * rate;
                decimal afterDiscounts = subtotal - dis1 - dis2;
                decimal finalTotal = afterDiscounts + tax;
                
                if (finalTotal < 0) finalTotal = 0;
                
                row.Cells["TotalAmount"].Value = finalTotal;
                
                // Update the underlying DataTable
                if (row.DataBoundItem is DataRowView dataRowView)
                {
                    dataRowView.Row["TotalAmount"] = finalTotal;
                    
                    // Ensure expiry date is valid
                    if (row.Cells["ExpiryDate"].Value == null || row.Cells["ExpiryDate"].Value == DBNull.Value)
                    {
                        row.Cells["ExpiryDate"].Value = DateTime.Now.AddYears(2);
                        dataRowView.Row["ExpiryDate"] = DateTime.Now.AddYears(2);
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle calculation errors silently - could log for debugging
                System.Diagnostics.Debug.WriteLine($"Error in RecalculateGridRowTotal: {ex.Message}");
            }
        }

        private void LoadSelectedItemDetails()
        {
            try
            {
                if (listBoxItems.SelectedValue != null)
                {
                    selectedItemID = Convert.ToInt32(listBoxItems.SelectedValue);
                    string query = @"SELECT ItemID, ItemName, PurchasePrice, PackSize, 
                                    ISNULL(PackSize, '1') as PackSizeValue
                                    FROM Items WHERE ItemID = @ItemID";
                    SqlParameter[] parameters = { new SqlParameter("@ItemID", selectedItemID) };
                    DataTable dt = DatabaseConnection.ExecuteQuery(query, parameters);
                    
                    if (dt.Rows.Count > 0)
                    {
                        DataRow row = dt.Rows[0];
                        
                        // 1. Item name appears in textbox1 (Item Name field)
                        textBox1.Text = row["ItemName"].ToString();
                        
                        // Store base rate and pack size
                        baseRate = Convert.ToDecimal(row["PurchasePrice"]);
                        
                        // Parse pack size - handle both numeric and text values
                        string packSizeStr = row["PackSizeValue"].ToString();
                        if (!int.TryParse(packSizeStr, out packSize))
                        {
                            // If pack size is text like "10x10", extract first number
                            string[] parts = packSizeStr.Split('x', 'X', '*');
                            if (parts.Length > 0 && int.TryParse(parts[0], out int firstPart))
                            {
                                packSize = firstPart;
                            }
                            else
                            {
                                packSize = 1; // Default if can't parse
                            }
                        }
                        
                        // 2. Default to P (Pack) mode
                        currentPackLooseMode = "P";
                        textBox2.Text = "P";
                        
                        // 3. Set rate based on P/L mode
                        UpdateRateBasedOnMode();
                        
                        // Set default expiry date (2 years from today)
                        dtpExpiry.Value = DateTime.Now.AddYears(2);
                        
                        // Clear other fields
                        textBox4.Text = ""; // Qty
                        textBox5.Text = "0"; // Bonus
                        textBox6.Text = "0"; // Dis1
                        textBox7.Text = "0"; // Dis2
                        textBox8.Text = "0"; // Tax
                        textBox10.Text = "0.00"; // Total
                        
                        // Focus on quantity field
                        textBox4.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading item details: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAddItem_Click(object sender, EventArgs e)
        {
            if (ValidateItemInput())
            {
                AddItemToPurchase();
                CalculateTotals();
                ClearItemInputs();
            }
        }

        private bool ValidateItemInput()
        {
            // Using textBox1 for item name
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Please select an item first.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Using textBox4 for quantity
            if (string.IsNullOrWhiteSpace(textBox4.Text) || !decimal.TryParse(textBox4.Text, out decimal qty) || qty <= 0)
            {
                MessageBox.Show("Please enter a valid quantity.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Using textBox3 for rate
            if (string.IsNullOrWhiteSpace(textBox3.Text) || !decimal.TryParse(textBox3.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Please enter a valid rate.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void AddItemToPurchase()
        {
            // Get values from form fields
            string itemName = textBox1.Text.Trim(); // Item Name
            DateTime expiryDate = dtpExpiry.Value; // Expiry Date
            string packLoose = textBox2.Text; // P/L
            decimal rate = decimal.Parse(textBox3.Text); // Rate
            decimal quantity = decimal.Parse(textBox4.Text); // Qty
            decimal bonus = string.IsNullOrEmpty(textBox5.Text) ? 0 : decimal.Parse(textBox5.Text); // Bonus
            decimal dis1 = string.IsNullOrEmpty(textBox6.Text) ? 0 : decimal.Parse(textBox6.Text); // Dis1
            decimal dis2 = string.IsNullOrEmpty(textBox7.Text) ? 0 : decimal.Parse(textBox7.Text); // Dis2
            decimal tax = string.IsNullOrEmpty(textBox8.Text) ? 0 : decimal.Parse(textBox8.Text); // Tax
            decimal totalAmount = decimal.Parse(textBox10.Text); // Total

            DataRow newRow = purchaseItems.NewRow();
            newRow["ItemID"] = selectedItemID;
            newRow["ItemName"] = itemName;
            newRow["ExpiryDate"] = expiryDate;
            newRow["PackLoose"] = packLoose;
            newRow["Quantity"] = quantity;
            newRow["Rate"] = rate;
            newRow["Bonus"] = bonus;
            newRow["Discount1"] = dis1;
            newRow["Discount2"] = dis2;
            newRow["Tax"] = tax;
            newRow["TotalAmount"] = totalAmount;
            purchaseItems.Rows.Add(newRow);

            dataGridView1.DataSource = purchaseItems;
        }

        private void CalculateTotals()
        {
            totalAmount = 0;
            foreach (DataRow row in purchaseItems.Rows)
            {
                totalAmount += Convert.ToDecimal(row["TotalAmount"]);
            }

            // Update all total fields
            textBox12.Text = totalAmount.ToString("N2"); // Discount field
            textBox13.Text = "0.00"; // Sales Tax
            textBox14.Text = "0.00"; // Other Charges
            label15.Text = totalAmount.ToString("N2"); // Gross Total
            label20.Text = totalAmount.ToString("N2"); // Net Total
        }

        private void ClearItemInputs()
        {
            textBox1.Text = ""; // Item Name
            dtpExpiry.Value = DateTime.Now.AddYears(2); // Reset expiry to default
            textBox2.Text = "P"; // P/L - reset to Pack
            textBox3.Text = ""; // Rate
            textBox4.Text = ""; // Qty
            textBox5.Text = "0"; // Bonus
            textBox6.Text = "0"; // Dis1
            textBox7.Text = "0"; // Dis2
            textBox8.Text = "0"; // Tax
            textBox10.Text = "0.00"; // Total
            
            // Reset internal variables
            selectedItemID = 0;
            currentPackLooseMode = "P";
            baseRate = 0;
            packSize = 1;
            
            // Clear selection in list
            listBoxItems.ClearSelected();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (ValidatePurchase())
            {
                try
                {
                    SavePurchase();
                    MessageBox.Show("Purchase saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Show print button after successful save
                    btnPrint.Visible = true;
                    btnPrint.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error saving purchase: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private bool ValidatePurchase()
        {
            if (comboBox1.SelectedValue == null)
            {
                MessageBox.Show("Please select a supplier.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBox1.Focus();
                return false;
            }

            if (purchaseItems.Rows.Count == 0)
            {
                MessageBox.Show("Please add at least one item to the purchase.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(textBox11.Text))
            {
                MessageBox.Show("Purchase number is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox11.Focus();
                return false;
            }

            // Validate each item in the grid
            foreach (DataRow row in purchaseItems.Rows)
            {
                decimal quantity = Convert.ToDecimal(row["Quantity"]);
                decimal rate = Convert.ToDecimal(row["Rate"]);
                
                if (quantity <= 0)
                {
                    MessageBox.Show("All items must have valid quantities greater than 0.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                
                if (rate <= 0)
                {
                    MessageBox.Show("All items must have valid rates greater than 0.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            return true;
        }

        private void SavePurchase()
        {
            if (comboBox1.SelectedValue == null)
            {
                MessageBox.Show("Please select a supplier.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            int companyID = Convert.ToInt32(comboBox1.SelectedValue);
            DateTime purchaseDate = dateTimePicker1.Value;

            // Calculate totals for header
            decimal grossAmount = 0;
            decimal totalDiscount = 0;
            decimal totalTax = 0;
            
            foreach (DataRow row in purchaseItems.Rows)
            {
                decimal qty = Convert.ToDecimal(row["Quantity"]);
                decimal rate = Convert.ToDecimal(row["Rate"]);
                decimal bonus = Convert.ToDecimal(row["Bonus"]);
                decimal dis1 = Convert.ToDecimal(row["Discount1"]);
                decimal dis2 = Convert.ToDecimal(row["Discount2"]);
                decimal tax = Convert.ToDecimal(row["Tax"]);
                
                decimal itemGross = (qty + bonus) * rate;
                grossAmount += itemGross;
                totalDiscount += dis1 + dis2;
                totalTax += tax;
            }
            
            decimal taxableAmount = grossAmount - totalDiscount;
            decimal netAmount = taxableAmount + totalTax;
            totalAmount = netAmount;

            // Prepare QR and Barcode data strings
            string supplierName = comboBox1.Text;
            string qrData = $"PURCHASE#{textBox11.Text.Trim()}|SUPPLIER:{supplierName}|TOTAL:{netAmount:F2}|DATE:{purchaseDate:yyyy-MM-dd HH:mm:ss}|SHOP:Retail Management System";
            string barcodeData = textBox11.Text.Trim();

            // Try enhanced purchase header first
            string purchaseQuery = @"INSERT INTO Purchases (
                PurchaseNumber, CompanyID, UserID, PurchaseDate, InvoiceDate,
                GrossAmount, TotalDiscount, TaxableAmount, TotalTax, NetAmount,
                Remarks, IsActive, CreatedDate, QRCodeData, BarcodeData
            ) VALUES (
                @PurchaseNumber, @CompanyID, @UserID, @PurchaseDate, @InvoiceDate,
                @GrossAmount, @TotalDiscount, @TaxableAmount, @TotalTax, @NetAmount,
                @Remarks, 1, @CreatedDate, @QRCodeData, @BarcodeData
            ); SELECT SCOPE_IDENTITY();";

            SqlParameter[] purchaseParams = {
                new SqlParameter("@PurchaseNumber", textBox11.Text.Trim()),
                new SqlParameter("@CompanyID", companyID),
                new SqlParameter("@UserID", 1), // Default user ID
                new SqlParameter("@PurchaseDate", purchaseDate),
                new SqlParameter("@InvoiceDate", purchaseDate),
                new SqlParameter("@GrossAmount", grossAmount),
                new SqlParameter("@TotalDiscount", totalDiscount),
                new SqlParameter("@TaxableAmount", taxableAmount),
                new SqlParameter("@TotalTax", totalTax),
                new SqlParameter("@NetAmount", netAmount),
                new SqlParameter("@Remarks", "Enhanced Purchase Entry"),
                new SqlParameter("@CreatedDate", DateTime.Now),
                new SqlParameter("@QRCodeData", qrData),
                new SqlParameter("@BarcodeData", barcodeData)
            };

            int purchaseID;
            try
            {
                purchaseID = Convert.ToInt32(DatabaseConnection.ExecuteScalar(purchaseQuery, purchaseParams));
            }
            catch
            {
                // Fallback to basic purchase header
                string basicPurchaseQuery = @"INSERT INTO Purchases (PurchaseNumber, CompanyID, PurchaseDate, TotalAmount, Remarks, CreatedDate, IsActive) 
                                             VALUES (@PurchaseNumber, @CompanyID, @PurchaseDate, @TotalAmount, @Remarks, @CreatedDate, 1);
                                             SELECT SCOPE_IDENTITY();";
                
                SqlParameter[] basicParams = {
                    new SqlParameter("@PurchaseNumber", textBox11.Text.Trim()),
                    new SqlParameter("@CompanyID", companyID),
                    new SqlParameter("@PurchaseDate", purchaseDate),
                    new SqlParameter("@TotalAmount", netAmount),
                    new SqlParameter("@Remarks", "Purchase Entry"),
                    new SqlParameter("@CreatedDate", DateTime.Now)
                };
                
                purchaseID = Convert.ToInt32(DatabaseConnection.ExecuteScalar(basicPurchaseQuery, basicParams));
            }

            // Insert purchase items and update stock
            foreach (DataRow row in purchaseItems.Rows)
            {
                int itemID = Convert.ToInt32(row["ItemID"]);
                decimal quantity = Convert.ToDecimal(row["Quantity"]);
                decimal rate = Convert.ToDecimal(row["Rate"]);
                decimal bonus = Convert.ToDecimal(row["Bonus"]);
                decimal discount1 = Convert.ToDecimal(row["Discount1"]);
                decimal discount2 = Convert.ToDecimal(row["Discount2"]);
                decimal tax = Convert.ToDecimal(row["Tax"]);
                decimal totalItemAmount = Convert.ToDecimal(row["TotalAmount"]);
                string packLoose = row["PackLoose"].ToString();
                DateTime expiryDate = Convert.ToDateTime(row["ExpiryDate"]);

                // Insert purchase item using correct database schema
                string purchaseItemQuery = @"INSERT INTO PurchaseItems (
                    PurchaseID, ItemID, BatchNumber, ManufacturingDate, ExpiryDate, 
                    Quantity, FreeQuantity, UnitPrice, MRP, Discount, DiscountPercent,
                    TaxableAmount, GST_Rate, CGST, SGST, IGST, TotalAmount
                ) VALUES (
                    @PurchaseID, @ItemID, @BatchNumber, @ManufacturingDate, @ExpiryDate,
                    @Quantity, @FreeQuantity, @UnitPrice, @MRP, @Discount, @DiscountPercent,
                    @TaxableAmount, @GST_Rate, @CGST, @SGST, @IGST, @TotalAmount
                )";

                // Calculate derived values
                decimal taxableItemAmount = (quantity + bonus) * rate - discount1 - discount2;
                decimal gstRate = tax > 0 ? 12 : 0; // Assume 12% GST if tax is applied
                decimal cgst = tax / 2;
                decimal sgst = tax / 2;

                SqlParameter[] purchaseItemParams = {
                    new SqlParameter("@PurchaseID", purchaseID),
                    new SqlParameter("@ItemID", itemID),
                    new SqlParameter("@BatchNumber", $"BATCH{DateTime.Now:yyyyMMddHHmmss}"),
                    new SqlParameter("@ManufacturingDate", DateTime.Now),
                    new SqlParameter("@ExpiryDate", expiryDate),
                    new SqlParameter("@Quantity", (int)quantity),
                    new SqlParameter("@FreeQuantity", (int)bonus),
                    new SqlParameter("@UnitPrice", rate),
                    new SqlParameter("@MRP", rate * 1.2m), // Assume 20% markup for MRP
                    new SqlParameter("@Discount", discount1 + discount2),
                    new SqlParameter("@DiscountPercent", 0),
                    new SqlParameter("@TaxableAmount", taxableItemAmount),
                    new SqlParameter("@GST_Rate", gstRate),
                    new SqlParameter("@CGST", cgst),
                    new SqlParameter("@SGST", sgst),
                    new SqlParameter("@IGST", 0),
                    new SqlParameter("@TotalAmount", totalItemAmount)
                };

                try {
                    DatabaseConnection.ExecuteNonQuery(purchaseItemQuery, purchaseItemParams);
                } catch (Exception) {
                    // If enhanced insert fails, try basic insert without extra columns
                    string basicQuery = @"INSERT INTO PurchaseItems (PurchaseID, ItemID, Quantity, UnitPrice, TotalAmount) 
                                       VALUES (@PurchaseID, @ItemID, @Quantity, @UnitPrice, @TotalAmount)";
                    SqlParameter[] basicParams = {
                        new SqlParameter("@PurchaseID", purchaseID),
                        new SqlParameter("@ItemID", itemID),
                        new SqlParameter("@Quantity", (int)quantity),
                        new SqlParameter("@UnitPrice", rate),
                        new SqlParameter("@TotalAmount", totalItemAmount)
                    };
                    try {
                        DatabaseConnection.ExecuteNonQuery(basicQuery, basicParams);
                    } catch (Exception basicEx) {
                        throw new Exception($"Failed to save purchase item: {basicEx.Message}");
                    }
                }

                // Calculate actual stock quantity based on Pack/Loose
                decimal stockQuantity = quantity;
                if (packLoose == "P")
                {
                    // If Pack mode, multiply by pack size for stock update
                    // Get pack size for this item
                    string packSizeQuery = "SELECT ISNULL(PackSize, '1') as PackSize FROM Items WHERE ItemID = @ItemID";
                    SqlParameter[] packSizeParams = { new SqlParameter("@ItemID", itemID) };
                    DataTable packSizeResult = DatabaseConnection.ExecuteQuery(packSizeQuery, packSizeParams);
                    
                    if (packSizeResult.Rows.Count > 0)
                    {
                        string packSizeStr = packSizeResult.Rows[0]["PackSize"].ToString();
                        if (int.TryParse(packSizeStr, out int packSizeInt))
                        {
                            stockQuantity = quantity * packSizeInt;
                        }
                        else
                        {
                            // Parse pack size like "10x10" or "10*5"
                            string[] parts = packSizeStr.Split('x', 'X', '*');
                            if (parts.Length > 0 && int.TryParse(parts[0], out int firstPart))
                            {
                                stockQuantity = quantity * firstPart;
                            }
                        }
                    }
                }
                // If Loose mode, use quantity as-is

                // Update stock
                string stockColumnName = DatabaseConnection.GetStockColumnName();
                string updateStockQuery = $"UPDATE Items SET {stockColumnName} = {stockColumnName} + @Quantity WHERE ItemID = @ItemID";
                SqlParameter[] stockParams = {
                    new SqlParameter("@Quantity", stockQuantity),
                    new SqlParameter("@ItemID", itemID)
                };

                DatabaseConnection.ExecuteNonQuery(updateStockQuery, stockParams);
            }
            
            // Generate QR code and barcode for this purchase
            GenerateQRCode(textBox11.Text.Trim(), supplierName, netAmount, purchaseDate);
            GenerateBarcode(textBox11.Text.Trim());
            
            // Update form totals display
            CalculateTotals();
        }
        
        /// <summary>
        /// Calculate stock quantity based on Pack/Loose mode
        /// </summary>
        private decimal CalculateStockQuantity(int itemID, decimal quantity, string packLoose)
        {
            if (packLoose == "L")
            {
                return quantity; // Loose quantity goes directly to stock
            }
            
            // Pack mode - multiply by pack size
            try
            {
                string packSizeQuery = "SELECT ISNULL(PackSize, '1') as PackSize FROM Items WHERE ItemID = @ItemID";
                SqlParameter[] packSizeParams = { new SqlParameter("@ItemID", itemID) };
                DataTable packSizeResult = DatabaseConnection.ExecuteQuery(packSizeQuery, packSizeParams);
                
                if (packSizeResult.Rows.Count > 0)
                {
                    string packSizeStr = packSizeResult.Rows[0]["PackSize"].ToString();
                    
                    // Try to parse as integer first
                    if (int.TryParse(packSizeStr, out int packSizeInt))
                    {
                        return quantity * packSizeInt;
                    }
                    
                    // Handle pack sizes like "10x10", "10*5", etc.
                    string[] separators = { "x", "X", "*", "×" };
                    foreach (string separator in separators)
                    {
                        if (packSizeStr.Contains(separator))
                        {
                            string[] parts = packSizeStr.Split(separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                            if (parts.Length > 0 && int.TryParse(parts[0].Trim(), out int firstPart))
                            {
                                return quantity * firstPart;
                            }
                        }
                    }
                }
            }
            catch
            {
                // If any error, default to 1:1 ratio
            }
            
            return quantity; // Default if can't determine pack size
        }
        
        /// <summary>
        /// Update item stock quantity
        /// </summary>
        private void UpdateItemStock(int itemID, decimal stockQuantity)
        {
            try
            {
                string stockColumnName = DatabaseConnection.GetStockColumnName();
                string updateStockQuery = $"UPDATE Items SET {stockColumnName} = {stockColumnName} + @Quantity WHERE ItemID = @ItemID";
                
                SqlParameter[] stockParams = {
                    new SqlParameter("@Quantity", stockQuantity),
                    new SqlParameter("@ItemID", itemID)
                };
                
                DatabaseConnection.ExecuteNonQuery(updateStockQuery, stockParams);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Warning: Could not update stock for item ID {itemID}. {ex.Message}", "Stock Update Warning", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ClearForm()
        {
            // Clear header fields
            comboBox1.SelectedIndex = -1;
            dateTimePicker1.Value = DateTime.Now;
            
            // Clear purchase items
            purchaseItems.Clear();
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = purchaseItems; // Rebind empty table
            
            // Clear item input fields
            ClearItemInputs();
            
            // Clear totals
            textBox12.Text = "0.00";
            textBox13.Text = "0.00";
            textBox14.Text = "0.00";
            label15.Text = "0.00";
            label20.Text = "0.00";
            totalAmount = 0;
            
            // Clear QR code and barcode
            ClearAllCodes();
            
            // Hide print button
            btnPrint.Visible = false;
            btnPrint.Enabled = false;
            
            // Generate new purchase number
            GeneratePurchaseNumber();
            
            // Generate new QR and barcode for the new purchase
            GenerateInitialQRAndBarcode();
            
            MessageBox.Show("Form cleared successfully!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void btnNewItem_Click(object sender, EventArgs e)
        {
            Items items = new Items();
            items.Show();
            LoadItems(); // Reload items after adding new one
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void pbExit_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        /// <summary>
        /// DataGridView cell double-click event handler for P/L toggle
        /// </summary>
        private void DataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                string columnName = dataGridView1.Columns[e.ColumnIndex].Name;
                
                // Toggle P/L on double-click
                if (columnName == "PackLoose")
                {
                    DataGridViewCell cell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    string currentValue = cell.Value?.ToString() ?? "P";
                    cell.Value = currentValue == "P" ? "L" : "P";
                    
                    // Recalculate total for this row
                    RecalculateGridRowTotal(e.RowIndex);
                    CalculateTotals();
                }
            }
        }

        private void GenerateQRCode(string purchaseNumber, string supplierName, decimal totalAmount, DateTime purchaseDate)
        {
            try
            {
                // Create QR code data with purchase information
                string qrData = $"PURCHASE#{purchaseNumber}|SUPPLIER:{supplierName}|TOTAL:{totalAmount:F2}|DATE:{purchaseDate:yyyy-MM-dd HH:mm:ss}|SHOP:Retail Management System";
                
                // Generate QR code
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrData, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);
                
                // Generate bitmap with custom colors
                Bitmap qrCodeImage = qrCode.GetGraphic(20, Color.Black, Color.White, true);
                
                // Display in PictureBox
                pictureBoxQR.Image = qrCodeImage;
                pictureBoxQR.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating QR code: {ex.Message}", "QR Generation Error", 
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                pictureBoxQR.Visible = false;
            }
        }

        private void GenerateBarcode(string purchaseNumber)
        {
            try
            {
                // Create a simple text-based barcode representation
                Bitmap barcodeImage = new Bitmap(180, 60);
                using (Graphics g = Graphics.FromImage(barcodeImage))
                {
                    // Fill background
                    g.FillRectangle(Brushes.White, 0, 0, 180, 60);
                    
                    // Create barcode-like pattern with vertical lines
                    Random rand = new Random(purchaseNumber.GetHashCode()); // Consistent pattern for same purchase number
                    for (int i = 0; i < 120; i += 2)
                    {
                        int height = rand.Next(30, 45);
                        int width = rand.Next(1, 3);
                        g.FillRectangle(Brushes.Black, 10 + i, 5, width, height);
                    }
                    
                    // Add text below barcode
                    using (Font font = new Font("Arial", 8, FontStyle.Bold))
                    {
                        string displayText = purchaseNumber;
                        SizeF textSize = g.MeasureString(displayText, font);
                        float x = (180 - textSize.Width) / 2;
                        g.DrawString(displayText, font, Brushes.Black, x, 45);
                    }
                }
                
                // Display in PictureBox
                pictureBoxBarcode.Image = barcodeImage;
                pictureBoxBarcode.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating barcode: {ex.Message}", "Barcode Generation Error", 
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                pictureBoxBarcode.Visible = false;
            }
        }

        private void GenerateInitialQRAndBarcode()
        {
            try
            {
                // Generate initial QR code with purchase number and basic info
                string initialQRData = $"PURCHASE#{textBox11.Text.Trim()}|DATE:{DateTime.Now:yyyy-MM-dd}|STATUS:DRAFT|SHOP:Retail Management System";
                
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(initialQRData, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);
                Bitmap qrCodeImage = qrCode.GetGraphic(20, Color.Black, Color.White, true);
                
                pictureBoxQR.Image = qrCodeImage;
                pictureBoxQR.Visible = true;
                
                // Generate initial barcode with purchase number
                GenerateBarcode(textBox11.Text.Trim());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating initial codes: {ex.Message}", "Code Generation Error", 
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                pictureBoxQR.Visible = false;
                pictureBoxBarcode.Visible = false;
            }
        }

        private void ClearQRCode()
        {
            pictureBoxQR.Image?.Dispose();
            pictureBoxQR.Image = null;
            pictureBoxQR.Visible = false;
        }

        private void ClearBarcode()
        {
            pictureBoxBarcode.Image?.Dispose();
            pictureBoxBarcode.Image = null;
            pictureBoxBarcode.Visible = false;
        }

        private void ClearAllCodes()
        {
            ClearQRCode();
            ClearBarcode();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                GenerateThermalReceipt();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating receipt: {ex.Message}", "Print Error", 
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerateThermalReceipt()
        {
            try
            {
                // Create datasets for the report
                DataSet reportDataSet = new DataSet();
                
                // Create Purchase Header table
                DataTable purchaseHeaderTable = new DataTable("PurchaseHeader");
                purchaseHeaderTable.Columns.Add("PurchaseNumber", typeof(string));
                purchaseHeaderTable.Columns.Add("SupplierName", typeof(string));
                purchaseHeaderTable.Columns.Add("PurchaseDate", typeof(DateTime));
                purchaseHeaderTable.Columns.Add("GrossAmount", typeof(decimal));
                purchaseHeaderTable.Columns.Add("TotalDiscount", typeof(decimal));
                purchaseHeaderTable.Columns.Add("NetAmount", typeof(decimal));
                purchaseHeaderTable.Columns.Add("QRCodeData", typeof(string));
                purchaseHeaderTable.Columns.Add("BarcodeData", typeof(string));
                purchaseHeaderTable.Columns.Add("QRCodeImage", typeof(byte[]));
                purchaseHeaderTable.Columns.Add("BarcodeImage", typeof(byte[]));
                
                // Add purchase header data
                DataRow headerRow = purchaseHeaderTable.NewRow();
                headerRow["PurchaseNumber"] = textBox11.Text.Trim();
                headerRow["SupplierName"] = comboBox1.Text;
                headerRow["PurchaseDate"] = dateTimePicker1.Value;
                headerRow["GrossAmount"] = totalAmount;
                headerRow["TotalDiscount"] = decimal.Parse(textBox12.Text);
                headerRow["NetAmount"] = decimal.Parse(label20.Text);
                
                string supplierName = comboBox1.Text;
                string qrData = $"PURCHASE#{textBox11.Text.Trim()}|SUPPLIER:{supplierName}|TOTAL:{decimal.Parse(label20.Text):F2}|DATE:{dateTimePicker1.Value:yyyy-MM-dd HH:mm:ss}|SHOP:Retail Management System";
                string barcodeData = textBox11.Text.Trim();
                
                headerRow["QRCodeData"] = qrData;
                headerRow["BarcodeData"] = barcodeData;
                headerRow["QRCodeImage"] = GenerateQRCodeImageBytes(qrData);
                headerRow["BarcodeImage"] = GenerateBarcodeImageBytes(barcodeData);
                purchaseHeaderTable.Rows.Add(headerRow);
                
                // Create Purchase Items table
                DataTable purchaseItemsTable = new DataTable("PurchaseItems");
                purchaseItemsTable.Columns.Add("ItemName", typeof(string));
                purchaseItemsTable.Columns.Add("Quantity", typeof(int));
                purchaseItemsTable.Columns.Add("Rate", typeof(decimal));
                purchaseItemsTable.Columns.Add("TotalAmount", typeof(decimal));
                
                // Add purchase items data
                foreach (DataRow row in purchaseItems.Rows)
                {
                    DataRow itemRow = purchaseItemsTable.NewRow();
                    itemRow["ItemName"] = row["ItemName"];
                    itemRow["Quantity"] = Convert.ToInt32(row["Quantity"]);
                    itemRow["Rate"] = Convert.ToDecimal(row["Rate"]);
                    itemRow["TotalAmount"] = Convert.ToDecimal(row["TotalAmount"]);
                    purchaseItemsTable.Rows.Add(itemRow);
                }
                
                // Add tables to dataset
                reportDataSet.Tables.Add(purchaseHeaderTable);
                reportDataSet.Tables.Add(purchaseItemsTable);
                
                // Load and display the thermal receipt report
                LoadThermalPurchaseReport(reportDataSet);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to generate thermal receipt: {ex.Message}");
            }
        }

        private byte[] GenerateQRCodeImageBytes(string qrData)
        {
            try
            {
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrData, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);
                
                using (Bitmap qrCodeImage = qrCode.GetGraphic(4))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        qrCodeImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        return ms.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                // Return a small placeholder image if QR generation fails
                using (Bitmap placeholder = new Bitmap(100, 100))
                using (Graphics g = Graphics.FromImage(placeholder))
                {
                    g.FillRectangle(Brushes.White, 0, 0, 100, 100);
                    g.DrawString("QR", new Font("Arial", 8), Brushes.Black, 35, 45);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        placeholder.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        return ms.ToArray();
                    }
                }
            }
        }

        private byte[] GenerateBarcodeImageBytes(string barcodeData)
        {
            try
            {
                // Create a simple barcode-style image
                using (Bitmap barcodeImage = new Bitmap(200, 50))
                using (Graphics g = Graphics.FromImage(barcodeImage))
                {
                    g.FillRectangle(Brushes.White, 0, 0, 200, 50);
                    
                    // Generate barcode-style lines
                    Random rand = new Random(barcodeData.GetHashCode());
                    for (int i = 0; i < 160; i += 2)
                    {
                        int height = rand.Next(25, 35);
                        int width = rand.Next(1, 3);
                        g.FillRectangle(Brushes.Black, 20 + i, 5, width, height);
                    }
                    
                    // Add text below
                    using (Font font = new Font("Arial", 8, FontStyle.Bold))
                    {
                        SizeF textSize = g.MeasureString(barcodeData, font);
                        float x = (200 - textSize.Width) / 2;
                        g.DrawString(barcodeData, font, Brushes.Black, x, 35);
                    }
                    
                    using (MemoryStream ms = new MemoryStream())
                    {
                        barcodeImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        return ms.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                // Return a placeholder if barcode generation fails
                using (Bitmap placeholder = new Bitmap(200, 50))
                using (Graphics g = Graphics.FromImage(placeholder))
                {
                    g.FillRectangle(Brushes.White, 0, 0, 200, 50);
                    g.DrawString(barcodeData, new Font("Arial", 8), Brushes.Black, 10, 20);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        placeholder.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        return ms.ToArray();
                    }
                }
            }
        }

        private void LoadThermalPurchaseReport(DataSet reportDataSet)
        {
            try
            {
                // Create a new form to display the report
                Form reportForm = new Form();
                reportForm.Text = "Thermal Purchase Receipt";
                reportForm.WindowState = FormWindowState.Maximized;
                reportForm.StartPosition = FormStartPosition.CenterScreen;
                
                // Create ReportViewer control
                Microsoft.Reporting.WinForms.ReportViewer reportViewer = new Microsoft.Reporting.WinForms.ReportViewer();
                reportViewer.Dock = DockStyle.Fill;
                
                // Set the RDLC report path
                string reportPath = Path.Combine(Application.StartupPath, "Reports", "ThermalPurchaseReceipt.rdlc");
                if (!File.Exists(reportPath))
                {
                    throw new FileNotFoundException($"Report file not found: {reportPath}");
                }
                
                reportViewer.LocalReport.ReportPath = reportPath;
                
                // Set data sources
                Microsoft.Reporting.WinForms.ReportDataSource headerDataSource = 
                    new Microsoft.Reporting.WinForms.ReportDataSource("PurchaseHeader", reportDataSet.Tables["PurchaseHeader"]);
                Microsoft.Reporting.WinForms.ReportDataSource itemsDataSource = 
                    new Microsoft.Reporting.WinForms.ReportDataSource("PurchaseItems", reportDataSet.Tables["PurchaseItems"]);
                
                reportViewer.LocalReport.DataSources.Clear();
                reportViewer.LocalReport.DataSources.Add(headerDataSource);
                reportViewer.LocalReport.DataSources.Add(itemsDataSource);
                
                // Refresh the report
                reportViewer.RefreshReport();
                
                // Add the ReportViewer to the form and show it
                reportForm.Controls.Add(reportViewer);
                reportForm.ShowDialog();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to load thermal receipt report: {ex.Message}");
            }
        }

        // Delete Row Functionality
        private void AddDeleteRowButton()
        {
            try
            {
                // Add a delete button near the data grid view
                Button btnDeleteRow = new Button
                {
                    Text = "Delete Row",
                    Name = "btnDeleteRow",
                    Size = new Size(90, 30),
                    Location = new Point(dataGridView1.Location.X + dataGridView1.Width - 95, dataGridView1.Location.Y - 35),
                    BackColor = Color.Red,
                    ForeColor = Color.White,
                    Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold),
                    UseVisualStyleBackColor = false
                };
                
                btnDeleteRow.Click += BtnDeleteRow_Click;
                this.Controls.Add(btnDeleteRow);
                
                // Add label for instructions
                Label lblDeleteInstructions = new Label
                {
                    Text = "Select row and click 'Delete Row' or press Del key",
                    Name = "lblDeleteInstructions",
                    Size = new Size(300, 20),
                    Location = new Point(dataGridView1.Location.X, dataGridView1.Location.Y - 35),
                    Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Italic),
                    ForeColor = Color.Gray
                };
                
                this.Controls.Add(lblDeleteInstructions);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding delete button: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDeleteRow_Click(object sender, EventArgs e)
        {
            DeleteSelectedRow();
        }

        private void DeleteSelectedRow()
        {
            try
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
                    string itemName = selectedRow.Cells["ItemName"].Value?.ToString() ?? "Unknown Item";
                    
                    if (MessageBox.Show($"Are you sure you want to delete '{itemName}' from this purchase?", 
                        "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        // Remove from DataTable (this will automatically update the DataGridView)
                        DataRowView rowView = selectedRow.DataBoundItem as DataRowView;
                        if (rowView != null)
                        {
                            rowView.Row.Delete();
                        }
                        
                        // Recalculate totals
                        CalculateTotals();
                        
                        MessageBox.Show("Item removed successfully.", "Item Removed", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else if (dataGridView1.CurrentRow != null)
                {
                    DataGridViewRow currentRow = dataGridView1.CurrentRow;
                    string itemName = currentRow.Cells["ItemName"].Value?.ToString() ?? "Unknown Item";
                    
                    if (MessageBox.Show($"Are you sure you want to delete '{itemName}' from this purchase?", 
                        "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        // Remove from DataTable
                        DataRowView rowView = currentRow.DataBoundItem as DataRowView;
                        if (rowView != null)
                        {
                            rowView.Row.Delete();
                        }
                        
                        // Recalculate totals
                        CalculateTotals();
                        
                        MessageBox.Show("Item removed successfully.", "Item Removed", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Please select a row to delete.", "No Selection", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting row: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DeleteSelectedRow();
                e.Handled = true;
            }
        }

        private void DataGridView1_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            string itemName = e.Row.Cells["ItemName"].Value?.ToString() ?? "Unknown Item";
            
            if (MessageBox.Show($"Are you sure you want to delete '{itemName}' from this purchase?", 
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                // Recalculate totals after row is deleted
                this.BeginInvoke(new Action(() => CalculateTotals()));
            }
        }

        // Search and Keyboard Navigation Methods
        private void CreateSearchControls()
        {
            try
            {
                // Create search label
                lblSearchItems = new Label();
                lblSearchItems.Text = "🔍 Search Items (Name/Barcode):";
                lblSearchItems.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
                lblSearchItems.Location = new Point(780, 170); // Move search label a little more to the right
                lblSearchItems.Size = new Size(180, 15);
                lblSearchItems.ForeColor = Color.DarkBlue;
                this.Controls.Add(lblSearchItems);

                // Create search textbox
                txtSearchItems = new TextBox();
                txtSearchItems.Location = new Point(780, 180); // Move search textbox a little more to the right
                txtSearchItems.Size = new Size(280, 20); // Adjusted width to fit better
                txtSearchItems.Font = new Font("Microsoft Sans Serif", 9F);
                txtSearchItems.ForeColor = Color.Gray;
                txtSearchItems.Text = "Type to search items by name or scan barcode...";
                txtSearchItems.TabIndex = 0; // Make it first in tab order
                
                // Add event handlers
                txtSearchItems.TextChanged += TxtSearchItems_TextChanged;
                txtSearchItems.KeyDown += TxtSearchItems_KeyDown;
                txtSearchItems.GotFocus += TxtSearchItems_GotFocus;
                txtSearchItems.LostFocus += TxtSearchItems_LostFocus;
                
                this.Controls.Add(txtSearchItems);

                // Keep listBoxItems in original position - don't move it
                listBoxItems.Location = new Point(720, 198); // Keep list box where it was
                listBoxItems.Size = new Size(280, 318); // Keep original size
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error creating search controls: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetupKeyboardNavigation()
        {
            try
            {
                // Enable keyboard navigation for the form
                this.KeyPreview = true;
                this.KeyDown += NewPurchase_KeyDown;
                
                // Enable keyboard navigation for listBoxItems
                listBoxItems.KeyDown += ListBoxItems_KeyDown;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error setting up keyboard navigation: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TxtSearchItems_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string searchText = txtSearchItems.Text.Trim();
                
                // If it's the placeholder text or empty, show all items
                if (string.IsNullOrEmpty(searchText) || 
                    searchText == "Type to search items by name or scan barcode...")
                {
                    listBoxItems.DataSource = allItemsData;
                    return;
                }

                // Filter items by name or barcode
                DataTable filteredData = allItemsData.Clone();
                foreach (DataRow row in allItemsData.Rows)
                {
                    string itemName = row["ItemName"].ToString().ToLower();
                    string barcode = row["Barcode"].ToString().ToLower();
                    string category = row["Category"].ToString().ToLower();
                    
                    if (itemName.Contains(searchText.ToLower()) || 
                        barcode.Contains(searchText.ToLower()) ||
                        category.Contains(searchText.ToLower()))
                    {
                        filteredData.ImportRow(row);
                    }
                }

                listBoxItems.DataSource = filteredData;
                listBoxItems.DisplayMember = "ItemName";
                listBoxItems.ValueMember = "ItemID";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Search error: {ex.Message}");
            }
        }

        private void TxtSearchItems_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                // Handle arrow keys to navigate listbox from search box
                if (e.KeyCode == Keys.Down && listBoxItems.Items.Count > 0)
                {
                    listBoxItems.Focus();
                    listBoxItems.SelectedIndex = 0;
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Enter && listBoxItems.Items.Count > 0)
                {
                    // Select first item if Enter is pressed
                    listBoxItems.SelectedIndex = 0;
                    LoadSelectedItemDetails();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Search keydown error: {ex.Message}");
            }
        }

        private void TxtSearchItems_GotFocus(object sender, EventArgs e)
        {
            if (txtSearchItems.Text == "Type to search items by name or scan barcode...")
            {
                txtSearchItems.Text = "";
                txtSearchItems.ForeColor = Color.Black;
            }
        }

        private void TxtSearchItems_LostFocus(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearchItems.Text))
            {
                txtSearchItems.Text = "Type to search items by name or scan barcode...";
                txtSearchItems.ForeColor = Color.Gray;
            }
        }

        private void ListBoxItems_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter && listBoxItems.SelectedValue != null)
                {
                    LoadSelectedItemDetails();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Up && listBoxItems.SelectedIndex == 0)
                {
                    // Go back to search box when at top of list
                    txtSearchItems.Focus();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ListBox keydown error: {ex.Message}");
            }
        }

        private void NewPurchase_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                // Global keyboard shortcuts
                if (e.Control && e.KeyCode == Keys.F)
                {
                    txtSearchItems.Focus();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.F3)
                {
                    txtSearchItems.Focus();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Form keydown error: {ex.Message}");
            }
        }

        private void AdjustLayoutPositions()
        {
            try
            {
                // Move barcode slightly to the right and up to avoid button overlap
                pictureBoxBarcode.Location = new Point(1020, 30); // Moved up from 80 to 50
                pictureBoxBarcode.Size = new Size(160, 50); // Slightly smaller
                
                // Move QR code down and make it smaller to avoid list overlap
                pictureBoxQR.Location = new Point(980, 520); // Keep QR position perfect as requested
                pictureBoxQR.Size = new Size(120, 120); // Smaller from 150x150
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adjusting layout: " + ex.Message, "Layout Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void listBoxItems_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
