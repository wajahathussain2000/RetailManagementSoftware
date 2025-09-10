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
    public partial class NewBillForm : Form
    {
        private DataTable billItems;
        private int selectedCustomerID = 0;
        private decimal totalAmount = 0;
        private int selectedItemID = 0;
        private string currentPackLooseMode = "P"; // P for Pack, L for Loose
        private decimal baseRate = 0;
        private int packSize = 1;
        private DateTimePicker dtpExpiry; // Expiry date picker
        private TextBox txtSearchItems;
        private Label lblSearchItems;
        private DataTable allItemsData; // Store all items for filtering
        private int selectedRowIndex = -1; // Track selected row for editing

        public NewBillForm()
        {
            InitializeComponent();
            InitializeDataTable();
            LoadCustomers();
            LoadItems();
            SetupDataGridView();
            GenerateBillNumber();
            CreateExpiryDatePicker();
            SetupEventHandlers();
            
            // Create search functionality
            CreateSearchControls();
            SetupKeyboardNavigation();
            
            // Wire up button events
            button1.Click += btnAddItem_Click;
            btnSave.Click += btnSave_Click;
            btnCancel.Click += btnClear_Click;
            btnPrint.Click += btnPrint_Click;
            
            // Enable buttons
            btnSave.Enabled = true;
            btnCancel.Enabled = true;
            btnNewItem.Enabled = true;
            
            // Initialize QR code and barcode - generate them on load
            GenerateInitialQRAndBarcode();
            btnRefresh.Enabled = true;
            
            // Adjust layout to prevent overlapping
            AdjustLayoutPositions();
        }

        public void LoadInvoice(int saleID)
        {
            try
            {
                // Load sale header with all necessary fields
                string saleQuery = @"SELECT s.*, c.CustomerName, c.CustomerID
                                   FROM Sales s
                                   INNER JOIN Customers c ON s.CustomerID = c.CustomerID
                                   WHERE s.SaleID = @SaleID";

                SqlParameter[] saleParams = { new SqlParameter("@SaleID", saleID) };
                DataTable saleDt = DatabaseConnection.ExecuteQuery(saleQuery, saleParams);

                if (saleDt.Rows.Count > 0)
                {
                    DataRow saleRow = saleDt.Rows[0];
                    
                    // Populate ALL header fields dynamically with safe NULL handling
                    textBox11.Text = saleRow["BillNumber"]?.ToString() ?? ""; // Bill Number
                    dateTimePicker1.Value = SafeDataHelper.SafeToDateTime(saleRow["SaleDate"], DateTime.Now); // Sale Date
                    
                    // Set customer in combo box properly
                    int customerID = SafeDataHelper.SafeToInt32(saleRow["CustomerID"]);
                    comboBox1.SelectedValue = customerID;
                    comboBox1.Text = saleRow["CustomerName"]?.ToString() ?? "";
                    
                    // Set all summary fields
                    decimal netAmount = SafeDataHelper.SafeToDecimal(saleRow["NetAmount"], 0);
                    label15.Text = netAmount.ToString("N2"); // Net Amount
                    textBox12.Text = "0.00"; // Discount (will be calculated from items)
                    textBox13.Text = "0.00"; // Sales Tax (will be calculated from items)
                    textBox14.Text = "0.00"; // Other Charges (not used)
                    
                    // Load sale items first
                    LoadSaleItems(saleID);
                    
                    // Recalculate totals after loading items
                    CalculateTotals();
                    
                    // Update form title to indicate editing
                    this.Text = $"Edit Sales Invoice - {saleRow["BillNumber"]}";
                    
                    // Clear any existing item entry fields
                    ClearItemEntryFields();
                    
                    MessageBox.Show($"Sales invoice {saleRow["BillNumber"]} loaded successfully!\n\n" +
                                  $"Customer: {saleRow["CustomerName"]}\n" +
                                  $"Date: {Convert.ToDateTime(saleRow["SaleDate"]):dd/MM/yyyy}\n" +
                                  $"Total: {netAmount:N2}\n\n" +
                                  "You can now edit items, quantities, and prices.", 
                                  "Invoice Loaded", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Invoice not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading invoice: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearItemEntryFields()
        {
            // Clear item entry fields for new item entry
            textBox1.Text = ""; // Item Name
            textBox2.Text = "P"; // P/L
            textBox3.Text = ""; // Rate
            textBox4.Text = ""; // Qty
            textBox5.Text = "0"; // Bonus
            textBox6.Text = "0"; // Dis1
            textBox7.Text = "0"; // Dis2
            textBox8.Text = "0"; // Tax
            textBox10.Text = "0.00"; // Total
            dtpExpiry.Value = DateTime.Now.AddYears(2); // Expiry Date
        }

        private void EnsureRateColumnExists()
        {
            try
            {
                // Check if Rate column exists in SaleItems table
                string checkQuery = @"SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS 
                                    WHERE TABLE_NAME = 'SaleItems' AND COLUMN_NAME = 'Rate'";
                
                DataTable result = DatabaseConnection.ExecuteQuery(checkQuery, null);
                int columnExists = Convert.ToInt32(result.Rows[0][0]);
                
                if (columnExists == 0)
                {
                    // Rate column doesn't exist, add it
                    string addColumnQuery = @"ALTER TABLE SaleItems ADD Rate DECIMAL(18,2) NOT NULL DEFAULT 0";
                    DatabaseConnection.ExecuteNonQuery(addColumnQuery, null);
                    
                    System.Diagnostics.Debug.WriteLine("Rate column added to SaleItems table");
                    
                    // Also add to PurchaseItems table
                    string checkPurchaseQuery = @"SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS 
                                               WHERE TABLE_NAME = 'PurchaseItems' AND COLUMN_NAME = 'Rate'";
                    DataTable purchaseResult = DatabaseConnection.ExecuteQuery(checkPurchaseQuery, null);
                    int purchaseColumnExists = Convert.ToInt32(purchaseResult.Rows[0][0]);
                    
                    if (purchaseColumnExists == 0)
                    {
                        string addPurchaseColumnQuery = @"ALTER TABLE PurchaseItems ADD Rate DECIMAL(18,2) NOT NULL DEFAULT 0";
                        DatabaseConnection.ExecuteNonQuery(addPurchaseColumnQuery, null);
                        System.Diagnostics.Debug.WriteLine("Rate column added to PurchaseItems table");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error ensuring Rate column exists: {ex.Message}");
                // Don't throw exception, just log it
            }
        }


        private void LoadSaleItems(int saleID)
        {
            try
            {
                // First, ensure the Rate column exists in SaleItems table
                EnsureRateColumnExists();
                
                // Enhanced query with better NULL handling and debugging
                string itemsQuery = @"SELECT si.SaleItemID, si.SaleID, si.ItemID, si.Quantity, 
                                    ISNULL(si.Rate, 0) as Rate, 
                                    ISNULL(si.Bonus, 0) as Bonus, 
                                    ISNULL(si.Discount1, 0) as Discount1, 
                                    ISNULL(si.Discount2, 0) as Discount2, 
                                    ISNULL(si.Tax, 0) as Tax, 
                                    ISNULL(si.TotalAmount, 0) as TotalAmount,
                                    ISNULL(si.PackLoose, 'P') as PackLoose,
                                    ISNULL(si.ExpiryDate, DATEADD(YEAR, 2, GETDATE())) as ExpiryDate,
                                    i.ItemName,
                                    i.Price as ItemPrice  -- Get the item's base price as fallback
                                    FROM SaleItems si
                                    INNER JOIN Items i ON si.ItemID = i.ItemID
                                    WHERE si.SaleID = @SaleID";

                SqlParameter[] itemsParams = { new SqlParameter("@SaleID", saleID) };
                DataTable itemsDt = DatabaseConnection.ExecuteQuery(itemsQuery, itemsParams);
                
                // Debug: Show what we got from the database
                System.Diagnostics.Debug.WriteLine($"Loaded {itemsDt.Rows.Count} items for SaleID {saleID}");
                foreach (DataRow row in itemsDt.Rows)
                {
                    System.Diagnostics.Debug.WriteLine($"Item: {row["ItemName"]}, Rate: {row["Rate"]}, ItemPrice: {row["ItemPrice"]}");
                }
                
                // Clear existing items
                billItems.Rows.Clear();
                
                // Add items to the grid with safe NULL handling
                foreach (DataRow row in itemsDt.Rows)
                {
                    DataRow newRow = billItems.NewRow();
                    
                    // Populate all fields with safe NULL handling
                    newRow["ItemID"] = SafeDataHelper.SafeToInt32(row["ItemID"]);
                    newRow["ItemName"] = row["ItemName"]?.ToString() ?? "";
                    newRow["ExpiryDate"] = SafeDataHelper.SafeToDateTime(row["ExpiryDate"], DateTime.Now.AddYears(2));
                    newRow["PackLoose"] = row["PackLoose"]?.ToString() ?? "P";
                    newRow["Quantity"] = SafeDataHelper.SafeToDecimal(row["Quantity"], 0);
                    
                    // Use ItemPrice as fallback if Rate is 0 or NULL
                    decimal rate = SafeDataHelper.SafeToDecimal(row["Rate"], 0);
                    decimal itemPrice = SafeDataHelper.SafeToDecimal(row["ItemPrice"], 0);
                    
                    if (rate == 0 && itemPrice > 0)
                    {
                        rate = itemPrice; // Use item's base price as fallback
                        System.Diagnostics.Debug.WriteLine($"Using ItemPrice {itemPrice} as fallback for Rate for item {row["ItemName"]}");
                    }
                    
                    newRow["Rate"] = rate;
                    newRow["Bonus"] = SafeDataHelper.SafeToDecimal(row["Bonus"], 0);
                    newRow["Discount1"] = SafeDataHelper.SafeToDecimal(row["Discount1"], 0);
                    newRow["Discount2"] = SafeDataHelper.SafeToDecimal(row["Discount2"], 0);
                    newRow["Tax"] = SafeDataHelper.SafeToDecimal(row["Tax"], 0);
                    newRow["TotalAmount"] = SafeDataHelper.SafeToDecimal(row["TotalAmount"], 0);
                    
                    billItems.Rows.Add(newRow);
                }
                
            // Refresh the grid
            dataGridView1.DataSource = billItems;
            CalculateTotals();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading sale items: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetupEventHandlers()
        {
            // Add event handler for customer selection
            comboBox1.SelectedIndexChanged += ComboBox1_SelectedIndexChanged;
            
            // Add P/L toggle functionality
            textBox2.DoubleClick += TextBox2_DoubleClick;
            textBox2.Text = "P"; // Default to Pack
            textBox2.ReadOnly = true; // Make it read-only so user can't type, only double-click
            textBox2.BackColor = Color.LightBlue; // Visual indicator that it's clickable
            
            // Add tooltips for input fields
            ToolTip toolTip = new ToolTip();
            toolTip.SetToolTip(textBox2, "Double-click to toggle between Pack (P) and Loose (L) mode");
            toolTip.SetToolTip(textBox5, "Enter bonus quantity in units (e.g., 10 for 10 extra units)");
            toolTip.SetToolTip(textBox6, "Enter discount amount in rupees (e.g., 50 for ₹50 discount)");
            toolTip.SetToolTip(textBox7, "Enter discount amount in rupees (e.g., 25 for ₹25 discount)");
            toolTip.SetToolTip(textBox8, "Enter sales tax as percentage (e.g., 17 for 17% tax)");
            
            // Add automatic calculation handlers for real-time calculation
            textBox4.TextChanged += CalculateItemTotal; // Qty
            textBox3.TextChanged += CalculateItemTotal; // Rate
            textBox5.TextChanged += CalculateItemTotal; // Bonus
            textBox6.TextChanged += CalculateItemTotal; // Dis1
            textBox7.TextChanged += CalculateItemTotal; // Dis2
            textBox8.TextChanged += CalculateItemTotal; // Tax
            
            // Add bill number change handler to update barcode
            textBox11.TextChanged += TextBox11_TextChanged;
        }

        // Search and Keyboard Navigation Methods (aligned like NewPurchase form)
        private void CreateSearchControls()
        {
            try
            {
                // First, find the listBoxItems control
                Control listBox = this.Controls.Find("listBoxItems", true).FirstOrDefault();
                if (listBox == null)
                {
                    MessageBox.Show("ListBox control not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Create search label with fixed coordinates (like NewPurchase)
                lblSearchItems = new Label();
                lblSearchItems.Text = "🔍 Search Items (Name/Barcode):";
                lblSearchItems.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
                lblSearchItems.Location = new Point(780, 170); // Same as NewPurchase
                lblSearchItems.Size = new Size(180, 15);
                lblSearchItems.ForeColor = Color.DarkBlue;
                this.Controls.Add(lblSearchItems);

                // Create search textbox with fixed coordinates (like NewPurchase)
                txtSearchItems = new TextBox();
                txtSearchItems.Location = new Point(780, 180); // Same as NewPurchase - 10px below label
                txtSearchItems.Size = new Size(280, 20); // Same width as NewPurchase
                txtSearchItems.Font = new Font("Microsoft Sans Serif", 9F);
                txtSearchItems.TabIndex = 0; // Make it first in tab order
                
                // Custom placeholder implementation for older .NET Framework
                txtSearchItems.ForeColor = Color.Gray;
                txtSearchItems.Text = "Type to search items...";
                
                // Add focus handlers for custom placeholder
                txtSearchItems.GotFocus += TxtSearchItems_GotFocus;
                txtSearchItems.LostFocus += TxtSearchItems_LostFocus;
                
                // Add event handlers
                txtSearchItems.TextChanged += TxtSearchItems_TextChanged;
                txtSearchItems.KeyDown += TxtSearchItems_KeyDown;
                
                this.Controls.Add(txtSearchItems);

                // Position listbox like NewPurchase form (60px left, 18px below search)
                listBox.Location = new Point(720, 198); // Same as NewPurchase
                listBox.Size = new Size(280, 318); // Same size as NewPurchase
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
                this.KeyDown += NewBillForm_KeyDown;
                
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
                
                // If empty or placeholder text, show all items
                if (string.IsNullOrEmpty(searchText) || searchText == "Type to search items...")
                {
                    listBoxItems.DataSource = allItemsData;
                    listBoxItems.DisplayMember = "ItemName";
                    listBoxItems.ValueMember = "ItemID";
                    return;
                }

                // Filter items by name or barcode
                DataTable filteredData = allItemsData.Clone();
                foreach (DataRow row in allItemsData.Rows)
                {
                    string itemName = row["ItemName"].ToString().ToLower();
                    string barcode = "";
                    
                    // Check if Barcode column exists and has data
                    if (allItemsData.Columns.Contains("Barcode") && row["Barcode"] != DBNull.Value)
                    {
                        barcode = row["Barcode"].ToString().ToLower();
                    }
                    
                    if (itemName.Contains(searchText.ToLower()) || barcode.Contains(searchText.ToLower()))
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
                MessageBox.Show("Error filtering items: " + ex.Message, "Search Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void TxtSearchItems_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter && listBoxItems.Items.Count > 0)
                {
                    // Auto-select first item when Enter is pressed
                    listBoxItems.SelectedIndex = 0;
                    LoadSelectedItemDetails();
                    txtSearchItems.Clear();
                    
                    // Focus on quantity field for quick entry
                    textBox4.Focus(); // textBox4 is the quantity field
                    textBox4.SelectAll();
                }
                else if (e.KeyCode == Keys.Down && listBoxItems.Items.Count > 0)
                {
                    // Navigate to list box
                    listBoxItems.Focus();
                    if (listBoxItems.SelectedIndex < 0) listBoxItems.SelectedIndex = 0;
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    // Clear search text
                    txtSearchItems.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error handling key press: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TxtSearchItems_GotFocus(object sender, EventArgs e)
        {
            if (txtSearchItems.Text == "Type to search items...")
            {
                txtSearchItems.Text = "";
                txtSearchItems.ForeColor = Color.Black;
            }
        }

        private void TxtSearchItems_LostFocus(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearchItems.Text))
            {
                txtSearchItems.Text = "Type to search items...";
                txtSearchItems.ForeColor = Color.Gray;
            }
        }

        private void ListBoxItems_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter && listBoxItems.SelectedIndex >= 0)
                {
                    LoadSelectedItemDetails();
                    textBox3.Focus();
                    textBox3.SelectAll();
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    txtSearchItems.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error handling list box key press: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void NewBillForm_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                // Global keyboard shortcuts
                if (e.Control && e.KeyCode == Keys.F)
                {
                    txtSearchItems.Focus();
                    txtSearchItems.SelectAll();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.F3)
                {
                    txtSearchItems.Focus();
                    txtSearchItems.SelectAll();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error handling global key press: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AdjustLayoutPositions()
        {
            try
            {
                // Adjust positions to prevent overlapping with search controls
                // This method ensures proper spacing between form elements
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adjusting layout: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cmb = sender as ComboBox;
            if (cmb.SelectedValue != null && int.TryParse(cmb.SelectedValue.ToString(), out int customerId))
            {
                selectedCustomerID = customerId;
            }
        }

        private void InitializeDataTable()
        {
            billItems = new DataTable();
            billItems.Columns.Add("ItemID", typeof(int));
            billItems.Columns.Add("ItemName", typeof(string));
            billItems.Columns.Add("ExpiryDate", typeof(DateTime));
            billItems.Columns.Add("PackLoose", typeof(string));
            billItems.Columns.Add("Quantity", typeof(decimal));
            billItems.Columns.Add("Rate", typeof(decimal));
            billItems.Columns.Add("Bonus", typeof(decimal));
            billItems.Columns.Add("Discount1", typeof(decimal));
            billItems.Columns.Add("Discount2", typeof(decimal));
            billItems.Columns.Add("Tax", typeof(decimal));
            billItems.Columns.Add("TotalAmount", typeof(decimal));
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
            dataGridView1.Columns.Add("Discount1", "Dis-1");
            dataGridView1.Columns.Add("Discount2", "Dis-2");
            dataGridView1.Columns.Add("Tax", "S.Tax");
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
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Add event handlers for editing
            dataGridView1.CellDoubleClick += DataGridView1_CellDoubleClick;
            dataGridView1.CellValueChanged += DataGridView1_CellValueChanged;
            dataGridView1.CellEndEdit += DataGridView1_CellEndEdit;
            dataGridView1.CellClick += DataGridView1_CellClick;
            
            // Add event handlers for keyboard deletion (like NewPurchase form)
            dataGridView1.KeyDown += DataGridView1_KeyDown;
            dataGridView1.UserDeletingRow += DataGridView1_UserDeletingRow;

            // Bind the DataTable to the DataGridView
            dataGridView1.DataSource = billItems;

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

        private void GenerateBillNumber()
        {
            try
            {
                // First ensure BillNumber column exists
                EnsureBillNumberColumnExists();
                
                // Try to get the next bill number from existing sales
                string query = @"SELECT ISNULL(MAX(CAST(SUBSTRING(BillNumber, 5, LEN(BillNumber)) AS INT)), 0) + 1 
                                FROM Sales 
                                WHERE BillNumber IS NOT NULL AND BillNumber LIKE 'BILL%'";
                object result = DatabaseConnection.ExecuteScalar(query);
                int nextNumber = Convert.ToInt32(result);
                
                // If no existing bills, start from 1
                if (nextNumber == 1)
                {
                    nextNumber = 1;
                }
                
                textBox11.Text = $"BILL{nextNumber:D6}";
                
                // Generate barcode immediately after setting bill number
                GenerateBarcode(textBox11.Text.Trim());
                
                System.Diagnostics.Debug.WriteLine($"Generated bill number: {textBox11.Text}");
            }
            catch (Exception ex)
            {
                // Fallback to timestamp-based number
                textBox11.Text = $"BILL{DateTime.Now:yyyyMMddHHmmss}";
                GenerateBarcode(textBox11.Text.Trim());
                System.Diagnostics.Debug.WriteLine($"Fallback bill number generated: {textBox11.Text}, Error: {ex.Message}");
            }
        }

        private void EnsureBillNumberColumnExists()
        {
            try
            {
                // Check if BillNumber column exists in Sales table
                string checkQuery = @"SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS 
                                    WHERE TABLE_NAME = 'Sales' AND COLUMN_NAME = 'BillNumber'";
                
                DataTable result = DatabaseConnection.ExecuteQuery(checkQuery, null);
                int columnExists = Convert.ToInt32(result.Rows[0][0]);
                
                if (columnExists == 0)
                {
                    // BillNumber column doesn't exist, add it
                    string addColumnQuery = @"ALTER TABLE Sales ADD BillNumber VARCHAR(50) NULL";
                    DatabaseConnection.ExecuteNonQuery(addColumnQuery, null);
                    
                    System.Diagnostics.Debug.WriteLine("BillNumber column added to Sales table");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error ensuring BillNumber column exists: {ex.Message}");
                // Don't throw exception, just log it
            }
        }

        private void LoadCustomers()
        {
            try
            {
                string query = "SELECT CustomerID, CustomerName, Phone FROM Customers WHERE IsActive = 1 ORDER BY CustomerName";
                DataTable dt = DatabaseConnection.ExecuteQuery(query);
                comboBox1.DataSource = dt;
                comboBox1.DisplayMember = "CustomerName";
                comboBox1.ValueMember = "CustomerID";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading customers: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadItems()
        {
            try
            {
                string stockColumnName = DatabaseConnection.GetStockColumnName();
                
                // Check if Barcode column exists
                string barcodeColumn = "";
                try
                {
                    string checkColumnQuery = "SELECT COUNT(*) FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Items]') AND name = 'Barcode'";
                    int columnExists = Convert.ToInt32(DatabaseConnection.ExecuteScalar(checkColumnQuery));
                    if (columnExists > 0)
                    {
                        barcodeColumn = ", ISNULL(Barcode, '') as Barcode";
                    }
                }
                catch
                {
                    barcodeColumn = ", '' as Barcode"; // Default empty barcode if column doesn't exist
                }
                
                string query = $"SELECT ItemID, ItemName, ISNULL(Price, 0) as Price, ISNULL(MRP, 0) as MRP, {stockColumnName} as StockQuantity{barcodeColumn} FROM Items WHERE IsActive = 1 ORDER BY ItemName";
                DataTable dt = DatabaseConnection.ExecuteQuery(query);
                
                // Store all items data for filtering
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
                
                // Handle ExpiryDate column editing
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
                else
                {
                    // Handle item selection for editing (any other column click)
                    try
                    {
                        DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];
                        
                        // Populate item entry fields with selected item data
                        textBox1.Text = selectedRow.Cells["ItemName"].Value?.ToString() ?? ""; // Item Name
                        textBox2.Text = selectedRow.Cells["PackLoose"].Value?.ToString() ?? "P"; // P/L
                        textBox3.Text = selectedRow.Cells["Rate"].Value?.ToString() ?? ""; // Rate
                        textBox4.Text = selectedRow.Cells["Quantity"].Value?.ToString() ?? ""; // Qty
                        textBox5.Text = selectedRow.Cells["Bonus"].Value?.ToString() ?? "0"; // Bonus
                        textBox6.Text = selectedRow.Cells["Discount1"].Value?.ToString() ?? "0"; // Dis1
                        textBox7.Text = selectedRow.Cells["Discount2"].Value?.ToString() ?? "0"; // Dis2
                        textBox8.Text = selectedRow.Cells["Tax"].Value?.ToString() ?? "0"; // Tax
                        textBox10.Text = selectedRow.Cells["TotalAmount"].Value?.ToString() ?? "0.00"; // Total
                        
                        // Set expiry date
                        if (selectedRow.Cells["ExpiryDate"].Value != null && selectedRow.Cells["ExpiryDate"].Value != DBNull.Value)
                        {
                            dtpExpiry.Value = Convert.ToDateTime(selectedRow.Cells["ExpiryDate"].Value);
                        }
                        else
                        {
                            dtpExpiry.Value = DateTime.Now.AddYears(2);
                        }
                        
                        // Store the selected row index for updating
                        selectedRowIndex = e.RowIndex;
                        
                        // Show user feedback
                        MessageBox.Show($"Item '{textBox1.Text}' selected for editing.\n\n" +
                                      $"Rate: {textBox3.Text}\n" +
                                      $"Quantity: {textBox4.Text}\n" +
                                      $"Total: {textBox10.Text}\n\n" +
                                      "You can now edit the values and click 'Add Item' to save changes.", 
                                      "Item Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error selecting item: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
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
            
            // Debug information
            System.Diagnostics.Debug.WriteLine($"P/L Toggle: Current mode = {currentPackLooseMode}, BaseRate = {baseRate}, PackSize = {packSize}");
            
            // Toggle between P and L
            if (currentPackLooseMode == "P")
            {
                currentPackLooseMode = "L";
                textBox2.Text = "L";
                System.Diagnostics.Debug.WriteLine("Switched to Loose mode");
            }
            else
            {
                currentPackLooseMode = "P";
                textBox2.Text = "P";
                System.Diagnostics.Debug.WriteLine("Switched to Pack mode");
            }
            
            // Update rate based on new mode
            UpdateRateBasedOnMode();
            
            // Show user what happened
            string rateText = textBox3.Text;
            string modeText = currentPackLooseMode == "P" ? "Pack" : "Loose";
            
            // If pack size is 1, offer to set a proper pack size for testing
            if (packSize == 1)
            {
                DialogResult result = MessageBox.Show($"Switched to {modeText} mode\nRate: {rateText}\nBase Rate: {baseRate}\nPack Size: {packSize}\n\nPack Size is 1, so loose rate = pack rate.\n\nWould you like to set Pack Size to 10 for testing?", 
                              "P/L Toggle", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
                if (result == DialogResult.Yes)
                {
                    // Update pack size to 10 for this item
                    try
                    {
                        string updateQuery = "UPDATE Items SET PackSize = '10' WHERE ItemID = @ItemID";
                        SqlParameter[] updateParams = { new SqlParameter("@ItemID", selectedItemID) };
                        DatabaseConnection.ExecuteNonQuery(updateQuery, updateParams);
                        
                        // Reload the item details
                        LoadSelectedItemDetails();
                        
                        MessageBox.Show("Pack Size updated to 10. Try toggling P/L again!", 
                                      "Pack Size Updated", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error updating pack size: " + ex.Message, 
                                      "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show($"Switched to {modeText} mode\nRate: {rateText}\nBase Rate: {baseRate}\nPack Size: {packSize}", 
                              "P/L Toggle", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            
            // Recalculate total
            CalculateTotal(null, null);
        }
        
        /// <summary>
        /// Update rate based on Pack/Loose mode
        /// </summary>
        private void UpdateRateBasedOnMode()
        {
            System.Diagnostics.Debug.WriteLine($"UpdateRateBasedOnMode: Mode = {currentPackLooseMode}, BaseRate = {baseRate}, PackSize = {packSize}");
            
            if (currentPackLooseMode == "P")
            {
                // Pack mode - use base rate (MRP for sales)
                textBox3.Text = baseRate.ToString("F2");
                System.Diagnostics.Debug.WriteLine($"Pack mode: Rate set to {baseRate.ToString("F2")}");
            }
            else
            {
                // Loose mode - divide by pack size
                if (packSize > 0)
                {
                    decimal looseRate = baseRate / packSize;
                    textBox3.Text = looseRate.ToString("F2"); // Use same decimal places as pack mode
                    System.Diagnostics.Debug.WriteLine($"Loose mode: Rate set to {looseRate.ToString("F2")} (BaseRate {baseRate} ÷ PackSize {packSize})");
                }
                else
                {
                    // If pack size is 0 or invalid, use base rate
                    textBox3.Text = baseRate.ToString("F2");
                    System.Diagnostics.Debug.WriteLine($"Loose mode: PackSize is 0, using BaseRate {baseRate.ToString("F2")}");
                }
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
                
                decimal subtotal = qty * rate;  // Only quantity, not quantity + bonus
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
                    string query = @"SELECT ItemID, ItemName, 
                                    ISNULL(Price, 0) as Price,
                                    ISNULL(MRP, 0) as MRP, 
                                    PackSize, 
                                    ISNULL(PackSize, '10') as PackSizeValue
                                    FROM Items WHERE ItemID = @ItemID";
                    SqlParameter[] parameters = { new SqlParameter("@ItemID", selectedItemID) };
                    DataTable dt = DatabaseConnection.ExecuteQuery(query, parameters);
                    
                    if (dt.Rows.Count > 0)
                    {
                        DataRow row = dt.Rows[0];
                        
                        // 1. Item name appears in textbox1 (Item Name field)
                        textBox1.Text = row["ItemName"].ToString();
                        
                        // Store base rate and pack size - use MRP for sales
                        baseRate = Convert.ToDecimal(row["MRP"]);
                        decimal priceValue = Convert.ToDecimal(row["Price"]);
                        System.Diagnostics.Debug.WriteLine($"LoadSelectedItemDetails: MRP = {baseRate}, Price = {priceValue}");
                        
                        // If MRP is 0, use Price as fallback
                        if (baseRate == 0 && priceValue > 0)
                        {
                            baseRate = priceValue;
                            System.Diagnostics.Debug.WriteLine($"LoadSelectedItemDetails: MRP was 0, using Price = {baseRate}");
                        }
                        else if (baseRate == 0 && priceValue == 0)
                        {
                            // If both MRP and Price are 0, set a default rate for testing
                            baseRate = 100; // Default rate for testing
                            System.Diagnostics.Debug.WriteLine($"LoadSelectedItemDetails: Both MRP and Price are 0, using default rate = {baseRate}");
                        }
                        
                        // Parse pack size - handle both numeric and text values
                        string packSizeStr = row["PackSizeValue"].ToString();
                        string originalPackSize = row["PackSize"].ToString();
                        System.Diagnostics.Debug.WriteLine($"LoadSelectedItemDetails: Original PackSize = '{originalPackSize}', PackSizeValue = '{packSizeStr}'");
                        
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
                                packSize = 10; // Default to 10 if can't parse (better for loose calculation)
                            }
                        }
                        
                        // Ensure pack size is at least 1 to avoid division by zero
                        if (packSize <= 0)
                        {
                            packSize = 10; // Default to 10 for better loose calculation
                        }
                        
                        System.Diagnostics.Debug.WriteLine($"LoadSelectedItemDetails: PackSize set to {packSize}");
                        
                        // 2. Default to P (Pack) mode
                        currentPackLooseMode = "P";
                        textBox2.Text = "P";
                        
                        // 3. Set rate based on P/L mode
                        System.Diagnostics.Debug.WriteLine($"About to call UpdateRateBasedOnMode: BaseRate = {baseRate}, PackSize = {packSize}, Mode = {currentPackLooseMode}");
                        UpdateRateBasedOnMode();
                        System.Diagnostics.Debug.WriteLine($"After UpdateRateBasedOnMode: Rate field = {textBox3.Text}");
                        
                        // Show user what was loaded
                        MessageBox.Show($"Item loaded successfully!\nItem: {textBox1.Text}\nMRP: {baseRate}\nPrice: {priceValue}\nPack Size: {packSize}\nRate: {textBox3.Text}", 
                                      "Item Details", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        
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
                if (selectedRowIndex >= 0)
                {
                    // Update existing item
                    UpdateSelectedItem();
                    selectedRowIndex = -1; // Reset selection
                }
                else
                {
                    // Add new item
                    AddItemToBill();
                }
                CalculateTotals();
                ClearItemInputs();
            }
        }

        private void UpdateSelectedItem()
        {
            try
            {
                if (selectedRowIndex >= 0 && selectedRowIndex < dataGridView1.Rows.Count)
                {
                    DataGridViewRow row = dataGridView1.Rows[selectedRowIndex];
                    
                    // Update the row with new values
                    row.Cells["ItemName"].Value = textBox1.Text;
                    row.Cells["PackLoose"].Value = textBox2.Text;
                    row.Cells["Rate"].Value = decimal.Parse(textBox3.Text);
                    row.Cells["Quantity"].Value = decimal.Parse(textBox4.Text);
                    row.Cells["Bonus"].Value = decimal.Parse(textBox5.Text);
                    row.Cells["Discount1"].Value = decimal.Parse(textBox6.Text);
                    row.Cells["Discount2"].Value = decimal.Parse(textBox7.Text);
                    row.Cells["Tax"].Value = decimal.Parse(textBox8.Text);
                    row.Cells["ExpiryDate"].Value = dtpExpiry.Value;
                    
                    // Calculate new total amount - bonus does NOT affect billing amount
                    decimal quantity = decimal.Parse(textBox4.Text);
                    decimal rate = decimal.Parse(textBox3.Text);
                    decimal bonus = decimal.Parse(textBox5.Text);
                    decimal discount1 = decimal.Parse(textBox6.Text);
                    decimal discount2 = decimal.Parse(textBox7.Text);
                    decimal tax = decimal.Parse(textBox8.Text);
                    
                    decimal subtotal = quantity * rate;  // Only quantity, not quantity + bonus
                    decimal discountAmount = discount1 + discount2;
                    decimal taxableAmount = subtotal - discountAmount;
                    decimal taxAmount = taxableAmount * (tax / 100);
                    decimal totalAmount = taxableAmount + taxAmount;
                    
                    row.Cells["TotalAmount"].Value = totalAmount;
                    textBox10.Text = totalAmount.ToString("F2");
                    
                    // Refresh the grid
                    dataGridView1.Refresh();
                    
                    MessageBox.Show($"Item '{textBox1.Text}' updated successfully!\n\n" +
                                  $"New Total: {totalAmount:N2}", 
                                  "Item Updated", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating item: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CalculateItemTotal(object sender, EventArgs e)
        {
            try
            {
                // Only calculate if we have valid numeric inputs
                if (decimal.TryParse(textBox3.Text, out decimal rate) && 
                    decimal.TryParse(textBox4.Text, out decimal quantity) &&
                    decimal.TryParse(textBox5.Text, out decimal bonus) &&
                    decimal.TryParse(textBox6.Text, out decimal discount1) &&
                    decimal.TryParse(textBox7.Text, out decimal discount2) &&
                    decimal.TryParse(textBox8.Text, out decimal tax))
                {
                    // Calculate total amount - bonus does NOT affect billing amount
                    decimal subtotal = quantity * rate;  // Only quantity, not quantity + bonus
                    decimal discountAmount = discount1 + discount2;
                    decimal taxableAmount = subtotal - discountAmount;
                    decimal taxAmount = taxableAmount * (tax / 100);
                    decimal totalAmount = taxableAmount + taxAmount;
                    
                    // Update total field
                    textBox10.Text = totalAmount.ToString("F2");
                }
                else
                {
                    // If parsing fails, set total to 0
                    textBox10.Text = "0.00";
                }
            }
            catch (Exception ex)
            {
                // If calculation fails, set total to 0
                textBox10.Text = "0.00";
            }
        }

        private void TextBox11_TextChanged(object sender, EventArgs e)
        {
            try
            {
                // Update barcode when bill number changes
                if (!string.IsNullOrWhiteSpace(textBox11.Text))
                {
                    GenerateBarcode(textBox11.Text.Trim());
                    System.Diagnostics.Debug.WriteLine($"Barcode updated for bill number: {textBox11.Text}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating barcode: {ex.Message}");
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

            // Check stock availability
            try
            {
                string stockColumnName = DatabaseConnection.GetStockColumnName();
                string query = $"SELECT {stockColumnName} as StockQuantity FROM Items WHERE ItemID = @ItemID";
                SqlParameter[] parameters = { new SqlParameter("@ItemID", selectedItemID) };
                DataTable dt = DatabaseConnection.ExecuteQuery(query, parameters);
                
                if (dt.Rows.Count > 0)
                {
                    decimal stockQuantity = Convert.ToDecimal(dt.Rows[0]["StockQuantity"]);
                    decimal requestedQty = decimal.Parse(textBox4.Text);
                    
                    // Calculate actual stock requirement based on Pack/Loose
                    decimal requiredStock = requestedQty;
                    if (currentPackLooseMode == "P")
                    {
                        requiredStock = requestedQty * packSize;
                    }
                    
                    if (stockQuantity < requiredStock)
                    {
                        DialogResult result = MessageBox.Show(
                            $"Insufficient stock! Available: {stockQuantity}, Required: {requiredStock}\n\nDo you want to continue anyway?", 
                            "Stock Warning", 
                            MessageBoxButtons.YesNo, 
                            MessageBoxIcon.Warning);
                        
                        if (result == DialogResult.No)
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error checking stock: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void AddItemToBill()
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

            DataRow newRow = billItems.NewRow();
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
            billItems.Rows.Add(newRow);

            dataGridView1.DataSource = billItems;
        }

        private void CalculateTotals()
        {
            decimal grossTotal = 0;
            decimal totalDiscounts = 0;
            decimal totalTax = 0;
            decimal taxableAmount = 0;
            
            foreach (DataRow row in billItems.Rows)
            {
                // Get item values
                decimal quantity = Convert.ToDecimal(row["Quantity"]);
                decimal rate = Convert.ToDecimal(row["Rate"]);
                decimal bonus = Convert.ToDecimal(row["Bonus"]);
                decimal discount1 = Convert.ToDecimal(row["Discount1"]);
                decimal discount2 = Convert.ToDecimal(row["Discount2"]);
                decimal tax = Convert.ToDecimal(row["Tax"]);
                
                // Calculate item totals - bonus does NOT affect billing amount
                decimal itemSubtotal = quantity * rate;  // Only quantity, not quantity + bonus
                decimal itemDiscounts = discount1 + discount2;
                decimal itemTaxableAmount = itemSubtotal - itemDiscounts;
                decimal itemTaxAmount = itemTaxableAmount * (tax / 100);
                decimal itemTotal = itemTaxableAmount + itemTaxAmount;
                
                // Add to totals
                grossTotal += itemSubtotal;
                totalDiscounts += itemDiscounts;
                totalTax += itemTaxAmount;
                taxableAmount += itemTaxableAmount;
                
                // Update the row's total amount
                row["TotalAmount"] = itemTotal;
            }
            
            // Calculate final totals
            decimal netTotal = taxableAmount + totalTax;
            
            // Update all total fields
            textBox12.Text = totalDiscounts.ToString("N2"); // Discount field
            textBox13.Text = totalTax.ToString("N2"); // Sales Tax field
            textBox14.Text = "0.00"; // Other Charges (not used)
            label15.Text = grossTotal.ToString("N2"); // Gross Total (display)
            label20.Text = netTotal.ToString("N2"); // Net Total
            
            // Store the net total for saving
            totalAmount = netTotal;
            
            // Refresh the DataGridView to show updated totals
            dataGridView1.Refresh();
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
            if (ValidateBill())
            {
                try
                {
                    SaveBill();
                    MessageBox.Show("Bill saved successfully!\n\nClick Print to generate receipt.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Show print button after successful save
                    btnPrint.Visible = true;
                    btnPrint.Enabled = true;
                    
                    // Focus on print button for user convenience
                    btnPrint.Focus();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error saving bill: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private bool ValidateBill()
        {
            if (comboBox1.SelectedValue == null)
            {
                MessageBox.Show("Please select a customer.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBox1.Focus();
                return false;
            }

            if (billItems.Rows.Count == 0)
            {
                MessageBox.Show("Please add at least one item to the bill.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(textBox11.Text))
            {
                MessageBox.Show("Bill number is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox11.Focus();
                return false;
            }

            // Validate each item in the grid
            foreach (DataRow row in billItems.Rows)
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

        private void SaveBill()
        {
            if (comboBox1.SelectedValue == null)
            {
                MessageBox.Show("Please select a customer.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            int customerID = Convert.ToInt32(comboBox1.SelectedValue);
            DateTime saleDate = dateTimePicker1.Value;

            // Calculate totals for header
            decimal grossAmount = 0;
            decimal totalDiscount = 0;
            decimal totalTax = 0;
            
            foreach (DataRow row in billItems.Rows)
            {
                decimal qty = Convert.ToDecimal(row["Quantity"]);
                decimal rate = Convert.ToDecimal(row["Rate"]);
                decimal bonus = Convert.ToDecimal(row["Bonus"]);
                decimal dis1 = Convert.ToDecimal(row["Discount1"]);
                decimal dis2 = Convert.ToDecimal(row["Discount2"]);
                decimal taxPercent = Convert.ToDecimal(row["Tax"]);
                
                decimal itemGross = qty * rate;  // Only quantity, not quantity + bonus
                decimal itemDiscounts = dis1 + dis2;
                decimal itemTaxableAmount = itemGross - itemDiscounts;
                decimal itemTaxAmount = itemTaxableAmount * (taxPercent / 100);
                
                grossAmount += itemGross;
                totalDiscount += itemDiscounts;
                totalTax += itemTaxAmount;
            }
            
            decimal netAmount = grossAmount - totalDiscount + totalTax;
            totalAmount = netAmount;

            // Prepare QR and Barcode data strings
            string customerName = comboBox1.Text;
            string qrData = $"BILL#{textBox11.Text.Trim()}|CUSTOMER:{customerName}|TOTAL:{netAmount:F2}|DATE:{saleDate:yyyy-MM-dd HH:mm:ss}|SHOP:Retail Management System";
            string barcodeData = textBox11.Text.Trim();

            // Save to Sales table (equivalent to Purchases table)
            string salesQuery = @"INSERT INTO Sales (
                BillNumber, CustomerID, SaleDate, TotalAmount, Discount, NetAmount,
                PaymentMethod, IsCredit, Remarks, IsActive, CreatedDate, QRCodeData, BarcodeData
            ) VALUES (
                @BillNumber, @CustomerID, @SaleDate, @TotalAmount, @Discount, @NetAmount,
                @PaymentMethod, @IsCredit, @Remarks, 1, @CreatedDate, @QRCodeData, @BarcodeData
            ); SELECT SCOPE_IDENTITY();";

            SqlParameter[] salesParams = {
                new SqlParameter("@BillNumber", textBox11.Text.Trim()),
                new SqlParameter("@CustomerID", customerID),
                new SqlParameter("@SaleDate", saleDate),
                new SqlParameter("@TotalAmount", grossAmount),
                new SqlParameter("@Discount", totalDiscount),
                new SqlParameter("@NetAmount", netAmount),
                new SqlParameter("@PaymentMethod", "Cash"), // Default payment method
                new SqlParameter("@IsCredit", false), // Default to cash sale
                new SqlParameter("@Remarks", "Bill Entry"),
                new SqlParameter("@CreatedDate", DateTime.Now),
                new SqlParameter("@QRCodeData", qrData),
                new SqlParameter("@BarcodeData", barcodeData)
            };

            int saleID;
            try
            {
                saleID = Convert.ToInt32(DatabaseConnection.ExecuteScalar(salesQuery, salesParams));
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to save bill header: {ex.Message}");
            }

            // Insert sale items and update stock
            foreach (DataRow row in billItems.Rows)
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

                // Insert sale item - use SaleItems table (equivalent to PurchaseItems)
                string saleItemQuery = @"INSERT INTO SaleItems (
                    SaleID, ItemID, Quantity, Price, TotalAmount
                ) VALUES (
                    @SaleID, @ItemID, @Quantity, @Price, @TotalAmount
                )";

                SqlParameter[] saleItemParams = {
                    new SqlParameter("@SaleID", saleID),
                    new SqlParameter("@ItemID", itemID),
                    new SqlParameter("@Quantity", (int)quantity),
                    new SqlParameter("@Price", rate),
                    new SqlParameter("@TotalAmount", totalItemAmount)
                };

                try {
                    DatabaseConnection.ExecuteNonQuery(saleItemQuery, saleItemParams);
                } catch (Exception ex) {
                    throw new Exception($"Failed to save sale item: {ex.Message}");
                }

                // Calculate actual stock reduction based on Pack/Loose
                decimal stockReduction = quantity;
                if (packLoose == "P")
                {
                    // If Pack mode, multiply by pack size for stock reduction
                    string packSizeQuery = "SELECT ISNULL(PackSize, '1') as PackSize FROM Items WHERE ItemID = @ItemID";
                    SqlParameter[] packSizeParams = { new SqlParameter("@ItemID", itemID) };
                    DataTable packSizeResult = DatabaseConnection.ExecuteQuery(packSizeQuery, packSizeParams);
                    
                    if (packSizeResult.Rows.Count > 0)
                    {
                        string packSizeStr = packSizeResult.Rows[0]["PackSize"].ToString();
                        if (int.TryParse(packSizeStr, out int packSizeInt))
                        {
                            stockReduction = quantity * packSizeInt;
                        }
                        else
                        {
                            // Parse pack size like "10x10" or "10*5"
                            string[] parts = packSizeStr.Split('x', 'X', '*');
                            if (parts.Length > 0 && int.TryParse(parts[0], out int firstPart))
                            {
                                stockReduction = quantity * firstPart;
                            }
                        }
                    }
                }
                // If Loose mode, use quantity as-is

                // Reduce stock (opposite of purchase which increases stock)
                string stockColumnName = DatabaseConnection.GetStockColumnName();
                string updateStockQuery = $"UPDATE Items SET {stockColumnName} = {stockColumnName} - @Quantity WHERE ItemID = @ItemID";
                SqlParameter[] stockParams = {
                    new SqlParameter("@Quantity", stockReduction),
                    new SqlParameter("@ItemID", itemID)
                };

                DatabaseConnection.ExecuteNonQuery(updateStockQuery, stockParams);
            }
            
            // Generate QR code and barcode for this bill
            GenerateQRCode(textBox11.Text.Trim(), customerName, netAmount, saleDate);
            GenerateBarcode(textBox11.Text.Trim());
            
            // Update form totals display
            CalculateTotals();
        }

        private void ClearForm()
        {
            // Clear header fields
            comboBox1.SelectedIndex = -1;
            dateTimePicker1.Value = DateTime.Now;
            
            // Clear bill items
            billItems.Clear();
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = billItems; // Rebind empty table
            
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
            
            // Generate new bill number
            GenerateBillNumber();
            
            // Generate new QR and barcode for the new bill
            GenerateInitialQRAndBarcode();
            
            MessageBox.Show("Form cleared successfully!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ResetFormForNewBill()
        {
            try
            {
                // Clear header fields
                comboBox1.SelectedIndex = -1;
                dateTimePicker1.Value = DateTime.Now;
                
                // Clear bill items
                billItems.Clear();
                dataGridView1.DataSource = null;
                dataGridView1.DataSource = billItems; // Rebind empty table
                
                // Clear item input fields
                ClearItemInputs();
                
                // Clear totals
                textBox12.Text = "0.00";
                textBox13.Text = "0.00";
                textBox14.Text = "0.00";
                label15.Text = "0.00";
                label20.Text = "0.00";
                totalAmount = 0;
                
                // Reset selected row index
                selectedRowIndex = -1;
                
                // Clear QR code and barcode
                ClearAllCodes();
                
                // Generate new bill number
                GenerateBillNumber();
                
                // Generate new QR and barcode for the new bill
                GenerateInitialQRAndBarcode();
                
                // Hide print button until next save
                btnPrint.Visible = false;
                btnPrint.Enabled = false;
                
                // Focus on customer selection for new bill
                comboBox1.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error resetting form: {ex.Message}", "Reset Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

        private void GenerateQRCode(string billNumber, string customerName, decimal totalAmount, DateTime billDate)
        {
            try
            {
                // Create QR code data with bill information
                string qrData = $"BILL#{billNumber}|CUSTOMER:{customerName}|TOTAL:{totalAmount:F2}|DATE:{billDate:yyyy-MM-dd HH:mm:ss}|SHOP:Retail Management System";
                
                // Generate QR code
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrData, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);
                
                // Generate bitmap with custom colors
                Bitmap qrCodeImage = qrCode.GetGraphic(20, Color.Black, Color.White, true);
                
                // Display in PictureBox
                pictureBoxQR.Image = qrCodeImage;
                pictureBoxQR.Visible = true;
                
                // Optional: Save QR code to file for record keeping
                // string qrFileName = $"QR_{billNumber}_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                // qrCodeImage.Save(Path.Combine(Application.StartupPath, "QRCodes", qrFileName));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating QR code: {ex.Message}", "QR Generation Error", 
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                pictureBoxQR.Visible = false;
            }
        }

        private void GenerateBarcode(string billNumber)
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
                    Random rand = new Random(billNumber.GetHashCode()); // Consistent pattern for same bill number
                    for (int i = 0; i < 120; i += 2)
                    {
                        int height = rand.Next(30, 45);
                        int width = rand.Next(1, 3);
                        g.FillRectangle(Brushes.Black, 10 + i, 5, width, height);
                    }
                    
                    // Add text below barcode
                    using (Font font = new Font("Arial", 8, FontStyle.Bold))
                    {
                        string displayText = billNumber;
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
                // Generate initial QR code with bill number and basic info
                string initialQRData = $"BILL#{textBox11.Text.Trim()}|DATE:{DateTime.Now:yyyy-MM-dd}|STATUS:DRAFT|SHOP:Retail Management System";
                
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(initialQRData, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);
                Bitmap qrCodeImage = qrCode.GetGraphic(20, Color.Black, Color.White, true);
                
                pictureBoxQR.Image = qrCodeImage;
                pictureBoxQR.Visible = true;
                
                // Generate initial barcode with bill number
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
                
                // Ask user if they want to reset form for new bill
                DialogResult result = MessageBox.Show("Receipt generated successfully!\n\nDo you want to reset the form for a new bill?", 
                    "Print Success", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
                if (result == DialogResult.Yes)
                {
                    // Reset form for new bill
                    ResetFormForNewBill();
                }
                else
                {
                    // Hide print button but keep form as is
                    btnPrint.Visible = false;
                    btnPrint.Enabled = false;
                }
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
                
                // Create Bill Header table
                DataTable billHeaderTable = new DataTable("BillHeader");
                billHeaderTable.Columns.Add("BillNumber", typeof(string));
                billHeaderTable.Columns.Add("CustomerName", typeof(string));
                billHeaderTable.Columns.Add("BillDate", typeof(DateTime));
                billHeaderTable.Columns.Add("GrossTotal", typeof(decimal));
                billHeaderTable.Columns.Add("TotalDiscount", typeof(decimal));
                billHeaderTable.Columns.Add("SalesTax", typeof(decimal));
                billHeaderTable.Columns.Add("NetAmount", typeof(decimal));
                billHeaderTable.Columns.Add("QRCodeData", typeof(string));
                billHeaderTable.Columns.Add("BarcodeData", typeof(string));
                billHeaderTable.Columns.Add("QRCodeImage", typeof(byte[]));
                billHeaderTable.Columns.Add("BarcodeImage", typeof(byte[]));
                
                // Add bill header data
                DataRow headerRow = billHeaderTable.NewRow();
                headerRow["BillNumber"] = textBox11.Text.Trim();
                headerRow["CustomerName"] = comboBox1.Text;
                headerRow["BillDate"] = dateTimePicker1.Value;
                headerRow["GrossTotal"] = decimal.Parse(label15.Text); // Gross Total
                headerRow["TotalDiscount"] = decimal.Parse(textBox12.Text); // Total Discounts
                headerRow["SalesTax"] = decimal.Parse(textBox13.Text); // Sales Tax
                headerRow["NetAmount"] = decimal.Parse(label20.Text); // Net Total
                
                string customerName = comboBox1.Text;
                string qrData = $"BILL#{textBox11.Text.Trim()}|CUSTOMER:{customerName}|TOTAL:{decimal.Parse(label20.Text):F2}|DATE:{dateTimePicker1.Value:yyyy-MM-dd HH:mm:ss}|SHOP:Retail Management System";
                string barcodeData = textBox11.Text.Trim();
                
                headerRow["QRCodeData"] = qrData;
                headerRow["BarcodeData"] = barcodeData;
                headerRow["QRCodeImage"] = GenerateQRCodeImageBytes(qrData);
                headerRow["BarcodeImage"] = GenerateBarcodeImageBytes(barcodeData);
                billHeaderTable.Rows.Add(headerRow);
                
                // Create Bill Items table
                DataTable billItemsTable = new DataTable("BillItems");
                billItemsTable.Columns.Add("ItemName", typeof(string));
                billItemsTable.Columns.Add("Quantity", typeof(decimal));
                billItemsTable.Columns.Add("Bonus", typeof(decimal));
                billItemsTable.Columns.Add("Price", typeof(decimal)); // Changed from "Rate" to "Price" to match report
                billItemsTable.Columns.Add("Subtotal", typeof(decimal));
                billItemsTable.Columns.Add("Discount1", typeof(decimal));
                billItemsTable.Columns.Add("Discount2", typeof(decimal));
                billItemsTable.Columns.Add("TotalDiscount", typeof(decimal));
                billItemsTable.Columns.Add("TaxableAmount", typeof(decimal));
                billItemsTable.Columns.Add("TaxPercent", typeof(decimal));
                billItemsTable.Columns.Add("TaxAmount", typeof(decimal));
                billItemsTable.Columns.Add("TotalAmount", typeof(decimal));
                
                // Add bill items data
                foreach (DataRow row in billItems.Rows)
                {
                    DataRow itemRow = billItemsTable.NewRow();
                    
                    // Get item values
                    decimal quantity = Convert.ToDecimal(row["Quantity"]);
                    decimal bonus = Convert.ToDecimal(row["Bonus"]);
                    decimal rate = Convert.ToDecimal(row["Rate"]);
                    decimal discount1 = Convert.ToDecimal(row["Discount1"]);
                    decimal discount2 = Convert.ToDecimal(row["Discount2"]);
                    decimal tax = Convert.ToDecimal(row["Tax"]);
                    decimal totalAmount = Convert.ToDecimal(row["TotalAmount"]);
                    
                    // Calculate detailed amounts
                    decimal subtotal = quantity * rate; // Only quantity, not quantity + bonus
                    decimal totalDiscount = discount1 + discount2;
                    decimal taxableAmount = subtotal - totalDiscount;
                    decimal taxAmount = taxableAmount * (tax / 100);
                    
                    // Populate item row
                    itemRow["ItemName"] = row["ItemName"];
                    itemRow["Quantity"] = quantity;
                    itemRow["Bonus"] = bonus;
                    itemRow["Price"] = rate; // Changed from "Rate" to "Price" to match report
                    itemRow["Subtotal"] = subtotal;
                    itemRow["Discount1"] = discount1;
                    itemRow["Discount2"] = discount2;
                    itemRow["TotalDiscount"] = totalDiscount;
                    itemRow["TaxableAmount"] = taxableAmount;
                    itemRow["TaxPercent"] = tax;
                    itemRow["TaxAmount"] = taxAmount;
                    itemRow["TotalAmount"] = totalAmount;
                    
                    billItemsTable.Rows.Add(itemRow);
                }
                
                // Add tables to dataset
                reportDataSet.Tables.Add(billHeaderTable);
                reportDataSet.Tables.Add(billItemsTable);
                
                // Load and display the thermal receipt report
                LoadThermalBillReport(reportDataSet);
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

        private void LoadThermalBillReport(DataSet reportDataSet)
        {
            try
            {
                // Create a new form to display the report
                Form reportForm = new Form();
                reportForm.Text = "Thermal Bill Receipt";
                reportForm.WindowState = FormWindowState.Maximized;
                reportForm.StartPosition = FormStartPosition.CenterScreen;
                
                // Create ReportViewer control
                Microsoft.Reporting.WinForms.ReportViewer reportViewer = new Microsoft.Reporting.WinForms.ReportViewer();
                reportViewer.Dock = DockStyle.Fill;
                
                // Set the RDLC report path
                string reportPath = Path.Combine(Application.StartupPath, "Reports", "ThermalBillReceipt.rdlc");
                if (!File.Exists(reportPath))
                {
                    throw new FileNotFoundException($"Report file not found: {reportPath}");
                }
                
                reportViewer.LocalReport.ReportPath = reportPath;
                
                // Set data sources
                Microsoft.Reporting.WinForms.ReportDataSource headerDataSource = 
                    new Microsoft.Reporting.WinForms.ReportDataSource("BillHeader", reportDataSet.Tables["BillHeader"]);
                Microsoft.Reporting.WinForms.ReportDataSource itemsDataSource = 
                    new Microsoft.Reporting.WinForms.ReportDataSource("BillItems", reportDataSet.Tables["BillItems"]);
                
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

        // Keyboard deletion functionality (same as NewPurchase form)
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
            
            if (MessageBox.Show($"Are you sure you want to delete '{itemName}' from this bill?", 
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

        private void DeleteSelectedRow()
        {
            try
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
                    string itemName = selectedRow.Cells["ItemName"].Value?.ToString() ?? "Unknown Item";
                    
                    if (MessageBox.Show($"Are you sure you want to delete '{itemName}' from this bill?", 
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
                        
                        MessageBox.Show("Item removed from bill successfully.", "Item Removed", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Please select a row to delete.", "No Selection", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting row: {ex.Message}", "Delete Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

