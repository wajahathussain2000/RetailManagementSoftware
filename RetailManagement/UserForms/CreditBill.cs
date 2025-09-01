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
using QRCoder;
using RetailManagement.Database;
using RetailManagement.Utils;

namespace RetailManagement.UserForms
{
    public partial class CreditBill : Form
    {
        private DataTable billItems;
        private int selectedCustomerID = 0;
        private decimal totalAmount = 0;
        private decimal discount = 0;
        private decimal netAmount = 0;
        private int selectedItemID = 0;
        private string currentPackLooseMode = "P"; // P for Pack, L for Loose
        private decimal baseRate = 0;
        private int packSize = 1;
        private DateTimePicker dtpExpiry; // Expiry date picker
        
        // Customer Balance Management
        private Label lblCustomerBalance;
        private Label lblCurrentBalance;
        private Label lblCreditLimit;
        private Label lblAvailableCredit;
        private Button btnViewBalance;
        private decimal customerCurrentBalance = 0;
        private decimal customerCreditLimit = 0;

        public CreditBill()
        {
            InitializeComponent();
            SetupDataGridView();
            GenerateBillNumber();
            CreateExpiryDatePicker();
            SetupEventHandlers();
            
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
            
            // Initialize P/L button text
            button14.Text = "P";
            textBox2.Text = "P";
            
            // Add customer balance functionality
            InitializeCustomerBalance();
        }

        private void SetupEventHandlers()
        {
            // Setup event handlers for the form
            listBoxItems.SelectedIndexChanged += listBoxItems_SelectedIndexChanged;
            
            // Add event handler for customer selection
            comboBox1.SelectedIndexChanged += ComboBox1_SelectedIndexChanged;
            
            // Add P/L toggle functionality
            textBox2.DoubleClick += TextBox2_DoubleClick;
            button14.Click += button14_Click;
            
            // Add automatic calculation handlers for all input fields
            textBox4.TextChanged += CalculateTotal; // Qty
            textBox3.TextChanged += CalculateTotal; // Rate
            textBox5.TextChanged += CalculateTotal; // Bonus
            textBox6.TextChanged += CalculateTotal; // Dis1
            textBox7.TextChanged += CalculateTotal; // Dis2
            textBox8.TextChanged += CalculateTotal; // Tax
            
            // Overall totals calculation
            textBox9.TextChanged += textBox9_TextChanged;
        }

        private void CreateExpiryDatePicker()
        {
            // Remove existing textBox9 if it exists (replace with date picker)
            if (groupBox1.Controls.Contains(textBox9))
            {
                groupBox1.Controls.Remove(textBox9);
            }
            
            dtpExpiry = new DateTimePicker();
            dtpExpiry.Format = DateTimePickerFormat.Short;
            dtpExpiry.Location = new Point(703, 57); // Same position as textBox9
            dtpExpiry.Size = new Size(79, 26);
            dtpExpiry.Value = DateTime.Now.AddYears(2); // Default 2 years from now
            dtpExpiry.Font = new Font("Microsoft Sans Serif", 8.25f);
            dtpExpiry.Name = "dtpExpiry";
            
            // Add to the groupBox1 (same container as other input fields)
            groupBox1.Controls.Add(dtpExpiry);
            
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

        private void SetupDataGridView()
        {
            try
            {
                // Initialize DataTable for bill items
                billItems = new DataTable();
                billItems.Columns.Add("ItemName", typeof(string));
                billItems.Columns.Add("ExpDate", typeof(string));
                billItems.Columns.Add("PackLoose", typeof(string));
                billItems.Columns.Add("Quantity", typeof(int));
                billItems.Columns.Add("Rate", typeof(decimal));
                billItems.Columns.Add("Bonus", typeof(int));
                billItems.Columns.Add("Dis-1", typeof(decimal));
                billItems.Columns.Add("Dis-2", typeof(decimal));
                billItems.Columns.Add("S.Tax", typeof(decimal));
                billItems.Columns.Add("TotalAmount", typeof(decimal));

                // Bind to DataGridView
                dataGridView1.DataSource = billItems;

                // Setup columns
                dataGridView1.AutoGenerateColumns = true;
                dataGridView1.AllowUserToAddRows = false;
                dataGridView1.ReadOnly = true;
                dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

                // Load customers and items
                LoadCustomers();
                LoadItems();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error setting up data grid: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerateBillNumber()
        {
            try
            {
                string query = "SELECT ISNULL(MAX(CAST(SUBSTRING(BillNumber, 5, LEN(BillNumber)) AS INT)), 0) + 1 FROM Sales WHERE BillNumber LIKE 'CRDT%'";
                object result = DatabaseConnection.ExecuteScalar(query);
                int nextNumber = Convert.ToInt32(result);
                textBox11.Text = $"CRDT{nextNumber:D6}";
            }
            catch
            {
                textBox11.Text = $"CRDT{DateTime.Now:yyyyMMdd}001";
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
                comboBox1.SelectedIndexChanged += ComboBox1_SelectedIndexChanged;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading customers: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedValue != null && comboBox1.SelectedValue != DBNull.Value)
            {
                selectedCustomerID = Convert.ToInt32(comboBox1.SelectedValue);
                LoadCustomerBalance(selectedCustomerID);
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
            
            // Toggle P/L mode
            currentPackLooseMode = currentPackLooseMode == "P" ? "L" : "P";
            textBox2.Text = currentPackLooseMode;
            button14.Text = currentPackLooseMode;
            UpdateRateBasedOnMode();
        }

        /// <summary>
        /// Auto-calculate item total when any input field changes
        /// </summary>
        private void CalculateTotal(object sender, EventArgs e)
        {
            try
            {
                // Debug: Check values
                System.Diagnostics.Debug.WriteLine($"CalculateTotal called - textBox4: '{textBox4.Text}', textBox3: '{textBox3.Text}'");
                
                if (decimal.TryParse(textBox4.Text, out decimal quantity) && 
                    decimal.TryParse(textBox3.Text, out decimal rate))
                {
                    decimal bonus = decimal.TryParse(textBox5.Text, out decimal b) ? b : 0;
                    decimal dis1 = decimal.TryParse(textBox6.Text, out decimal d1) ? d1 : 0;
                    decimal dis2 = decimal.TryParse(textBox7.Text, out decimal d2) ? d2 : 0;
                    decimal tax = decimal.TryParse(textBox8.Text, out decimal t) ? t : 0;
                    
                    // Calculate total: (quantity * rate) - bonus - dis1 - dis2 + tax
                    decimal total = (quantity * rate) - bonus - dis1 - dis2 + tax;
                    
                    System.Diagnostics.Debug.WriteLine($"Calculation: {quantity} * {rate} - {bonus} - {dis1} - {dis2} + {tax} = {total}");
                    
                    textBox15.Text = total.ToString("F2");
                }
                else
                {
                    textBox15.Text = "0.00";
                    System.Diagnostics.Debug.WriteLine($"Parse failed - quantity: '{textBox4.Text}', rate: '{textBox3.Text}'");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error calculating total: {ex.Message}");
                textBox15.Text = "0.00";
            }
        }

        private void LoadItems()
        {
            try
            {
                string query = "SELECT ItemID, ItemName, Price FROM Items WHERE IsActive = 1 ORDER BY ItemName";
                DataTable dt = DatabaseConnection.ExecuteQuery(query);
                listBoxItems.DataSource = dt;
                listBoxItems.DisplayMember = "ItemName";
                listBoxItems.ValueMember = "ItemID";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading items: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void listBoxItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (listBoxItems.SelectedValue != null)
                {
                    selectedItemID = Convert.ToInt32(listBoxItems.SelectedValue);
                    string stockColumnName = DatabaseConnection.GetStockColumnName();
                    
                    string query = $@"SELECT ItemID, ItemName, Price, 
                                      ISNULL(PurchasePrice, Price * 0.8) as PurchasePrice, 
                                      (Price * 1.2) as MRP, 
                                      ISNULL(PackSize, 1) as PackSize, 
                                      {stockColumnName} as StockQuantity, 
                                      ISNULL(Barcode, '') as Barcode
                                      FROM Items WHERE ItemID = @ItemID";
                    
                    SqlParameter[] parameters = { new SqlParameter("@ItemID", selectedItemID) };
                    DataTable dt = DatabaseConnection.ExecuteQuery(query, parameters);
                    
                    if (dt.Rows.Count > 0)
                    {
                        DataRow row = dt.Rows[0];
                        
                        baseRate = SafeDataHelper.SafeToDecimal(row["Price"]);
                        packSize = SafeDataHelper.SafeToInt32(row["PackSize"]);
                        if (packSize <= 0) packSize = 1;
                        
                        // Populate item fields (credit bill shows same fields as regular bill)
                        UpdateCurrentItemDisplay();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading item details: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateCurrentItemDisplay()
        {
            if (listBoxItems.SelectedItem != null)
            {
                DataRowView drv = (DataRowView)listBoxItems.SelectedItem;
                
                // Auto-fill item name in textBox1 (Item Name field)
                textBox1.Text = drv["ItemName"].ToString();
                
                // Set default Pack/Loose mode to Pack
                currentPackLooseMode = "P";
                textBox2.Text = "P";
                button14.Text = "P";
                
                // Auto-fill the rate based on Pack mode
                UpdateRateBasedOnMode();
                
                // Clear other fields for user input
                textBox4.Text = "1"; // Default quantity to 1
                textBox5.Text = "0"; // Default bonus to 0
                textBox6.Text = "0"; // Default discount1 to 0
                textBox7.Text = "0"; // Default discount2 to 0
                textBox8.Text = "0"; // Default tax to 0
                
                // Force calculation after setting values
                CalculateTotal(null, null);
                
                // Set focus to quantity field for easy entry
                textBox4.Focus();
                textBox4.SelectAll();
            }
        }

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
                decimal looseRate = baseRate / packSize;
                textBox3.Text = looseRate.ToString("F2");
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            CalculateItemTotal();
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            CalculateOverallTotals();
        }

        private void CalculateItemTotal()
        {
            try
            {
                if (decimal.TryParse(textBox4.Text, out decimal quantity) && 
                    decimal.TryParse(textBox3.Text, out decimal rate))
                {
                    decimal total = quantity * rate;
                    textBox15.Text = total.ToString("F2");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error calculating item total: {ex.Message}");
            }
        }

        private void CalculateOverallTotals()
        {
            try
            {
                totalAmount = 0;
                foreach (DataRow row in billItems.Rows)
                {
                    totalAmount += Convert.ToDecimal(row["TotalAmount"]);
                }

                discount = string.IsNullOrWhiteSpace(textBox12.Text) ? 0 : decimal.Parse(textBox12.Text);
                netAmount = totalAmount - discount;

                label15.Text = totalAmount.ToString("N2");
                label20.Text = netAmount.ToString("N2");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error calculating totals: {ex.Message}");
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            // Toggle P/L mode
            currentPackLooseMode = currentPackLooseMode == "P" ? "L" : "P";
            button14.Text = currentPackLooseMode;
            UpdateRateBasedOnMode();
        }

        private void btnAddItem_Click(object sender, EventArgs e)
        {
            if (ValidateItemInput())
            {
                AddItemToBill();
                CalculateOverallTotals();
                ClearItemInputs();
            }
        }

        private bool ValidateItemInput()
        {
            if (selectedItemID == 0)
            {
                MessageBox.Show("Please select an item from the list.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(textBox4.Text) || !decimal.TryParse(textBox4.Text, out decimal qty) || qty <= 0)
            {
                MessageBox.Show("Please enter a valid quantity.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(textBox3.Text) || !decimal.TryParse(textBox3.Text, out decimal rate) || rate <= 0)
            {
                MessageBox.Show("Please enter a valid rate.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void AddItemToBill()
        {
            try
            {
                string itemName = listBoxItems.Text;
                string expDate = dtpExpiry.Value.ToString("yyyy-MM-dd");
                string packLoose = textBox2.Text; // P/L
                decimal rate = decimal.Parse(textBox3.Text); // Rate  
                decimal quantity = decimal.Parse(textBox4.Text); // Qty
                decimal bonus = string.IsNullOrWhiteSpace(textBox5.Text) ? 0 : decimal.Parse(textBox5.Text);
                decimal dis1 = string.IsNullOrWhiteSpace(textBox6.Text) ? 0 : decimal.Parse(textBox6.Text);
                decimal dis2 = string.IsNullOrWhiteSpace(textBox7.Text) ? 0 : decimal.Parse(textBox7.Text);
                decimal stax = string.IsNullOrWhiteSpace(textBox8.Text) ? 0 : decimal.Parse(textBox8.Text);

                decimal totalItemAmount = quantity * rate;
                totalItemAmount -= dis1 + dis2;
                totalItemAmount += stax;

                DataRow newRow = billItems.NewRow();
                newRow["ItemName"] = itemName;
                newRow["ExpDate"] = expDate;
                newRow["PackLoose"] = currentPackLooseMode;
                newRow["Quantity"] = quantity;
                newRow["Rate"] = rate;
                newRow["Bonus"] = bonus;
                newRow["Dis-1"] = dis1;
                newRow["Dis-2"] = dis2;
                newRow["S.Tax"] = stax;
                newRow["TotalAmount"] = totalItemAmount;

                billItems.Rows.Add(newRow);
                dataGridView1.DataSource = billItems;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding item: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearItemInputs()
        {
            textBox2.Text = "P"; // Reset P/L to Pack
            textBox3.Text = ""; // Rate
            textBox4.Text = ""; // Quantity  
            textBox5.Text = ""; // Bonus
            textBox6.Text = ""; // Dis1
            textBox7.Text = ""; // Dis2
            textBox8.Text = ""; // STax
            textBox15.Text = ""; // Total
            if (dtpExpiry != null)
            {
                dtpExpiry.Value = DateTime.Now.AddYears(2); // Reset expiry to default
            }
            selectedItemID = 0;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (ValidateBill())
            {
                try
                {
                    // Use enhanced save method with customer balance management
                    SaveCreditBill();
                    
                    // Show print button after successful save
                    btnPrint.Visible = true;
                    btnPrint.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error saving credit bill: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private bool ValidateBill()
        {
            if (selectedCustomerID == 0)
            {
                MessageBox.Show("Please select a customer.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                return false;
            }

            return true;
        }

        private void SaveBill()
        {
            string customerName = comboBox1.Text;
            string qrData = $"CREDIT#{textBox11.Text.Trim()}|CUSTOMER:{customerName}|TOTAL:{decimal.Parse(label20.Text):F2}|DATE:{dateTimePicker1.Value:yyyy-MM-dd HH:mm:ss}|SHOP:Retail Management System";
            string barcodeData = textBox11.Text.Trim();

            string salesQuery = @"INSERT INTO Sales (
                BillNumber, CustomerID, SaleDate, TotalAmount, Discount, NetAmount,
                PaymentMethod, IsCredit, Remarks, IsActive, CreatedDate, QRCodeData, BarcodeData
            ) VALUES (
                @BillNumber, @CustomerID, @SaleDate, @TotalAmount, @Discount, @NetAmount,
                @PaymentMethod, @IsCredit, @Remarks, 1, @CreatedDate, @QRCodeData, @BarcodeData
            ); SELECT SCOPE_IDENTITY();";

            SqlParameter[] salesParams = {
                new SqlParameter("@BillNumber", textBox11.Text.Trim()),
                new SqlParameter("@CustomerID", selectedCustomerID),
                new SqlParameter("@SaleDate", dateTimePicker1.Value),
                new SqlParameter("@TotalAmount", totalAmount),
                new SqlParameter("@Discount", decimal.Parse(textBox12.Text)),
                new SqlParameter("@NetAmount", decimal.Parse(label20.Text)),
                new SqlParameter("@PaymentMethod", "Credit"),
                new SqlParameter("@IsCredit", true),
                new SqlParameter("@Remarks", "Credit Sale"),
                new SqlParameter("@CreatedDate", DateTime.Now),
                new SqlParameter("@QRCodeData", qrData),
                new SqlParameter("@BarcodeData", barcodeData)
            };

            int saleID = Convert.ToInt32(DatabaseConnection.ExecuteScalar(salesQuery, salesParams));

            foreach (DataRow row in billItems.Rows)
            {
                string saleItemQuery = @"INSERT INTO SaleItems (SaleID, ItemID, Quantity, Price, TotalAmount) 
                                       VALUES (@SaleID, @ItemID, @Quantity, @Price, @TotalAmount)";

                SqlParameter[] saleItemParams = {
                    new SqlParameter("@SaleID", saleID),
                    new SqlParameter("@ItemID", selectedItemID),
                    new SqlParameter("@Quantity", Convert.ToInt32(row["Quantity"])),
                    new SqlParameter("@Price", Convert.ToDecimal(row["Rate"])),
                    new SqlParameter("@TotalAmount", Convert.ToDecimal(row["TotalAmount"]))
                };

                DatabaseConnection.ExecuteNonQuery(saleItemQuery, saleItemParams);
            }

            // Generate QR code and barcode after save
            GenerateQRCode(qrData);
            GenerateBarcode(barcodeData);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            // Clear all form fields
            comboBox1.SelectedIndex = -1;
            selectedCustomerID = 0;
            billItems.Clear();
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = billItems;
            
            // Clear totals
            textBox12.Text = "0.00";
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
        }

        #region QR Code and Barcode Methods

        private void GenerateInitialQRAndBarcode()
        {
            try
            {
                string initialQR = $"CREDIT#{textBox11.Text}|INITIAL|DATE:{DateTime.Now:yyyy-MM-dd}";
                string initialBarcode = textBox11.Text;
                
                GenerateQRCode(initialQR);
                GenerateBarcode(initialBarcode);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error generating initial codes: {ex.Message}");
            }
        }

        private void GenerateQRCode(string qrData)
        {
            try
            {
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrData, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);
                
                using (Bitmap qrCodeImage = qrCode.GetGraphic(4))
                {
                    if (pictureBoxQR.Image != null)
                    {
                        pictureBoxQR.Image.Dispose();
                    }
                    pictureBoxQR.Image = new Bitmap(qrCodeImage);
                    pictureBoxQR.Visible = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error generating QR code: {ex.Message}");
                pictureBoxQR.Visible = false;
            }
        }

        private void GenerateBarcode(string barcodeData)
        {
            try
            {
                using (Bitmap barcodeImage = new Bitmap(180, 60))
                using (Graphics g = Graphics.FromImage(barcodeImage))
                {
                    g.FillRectangle(Brushes.White, 0, 0, 180, 60);
                    Random rand = new Random(barcodeData.GetHashCode());
                    for (int i = 0; i < 120; i += 2)
                    {
                        int height = rand.Next(30, 45);
                        int width = rand.Next(1, 3);
                        g.FillRectangle(Brushes.Black, 10 + i, 5, width, height);
                    }
                    using (Font font = new Font("Arial", 8, FontStyle.Bold))
                    {
                        string displayText = barcodeData;
                        SizeF textSize = g.MeasureString(displayText, font);
                        float x = (180 - textSize.Width) / 2;
                        g.DrawString(displayText, font, Brushes.Black, x, 45);
                    }
                    
                    if (pictureBoxBarcode.Image != null)
                    {
                        pictureBoxBarcode.Image.Dispose();
                    }
                    pictureBoxBarcode.Image = new Bitmap(barcodeImage);
                    pictureBoxBarcode.Visible = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error generating barcode: {ex.Message}");
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

        #endregion

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show("Credit bill thermal receipt will be generated here.", "Print", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                // Future: Implement thermal receipt printing similar to NewBillForm
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error printing: {ex.Message}", "Print Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadCustomers();
            LoadItems();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pbExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Customer Balance Management Methods
        private void InitializeCustomerBalance()
        {
            try
            {
                // Create customer balance display panel
                Panel balancePanel = new Panel
                {
                    Location = new Point(500, 10),
                    Size = new Size(350, 120),
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = Color.LightBlue
                };

                // Title label
                Label lblTitle = new Label
                {
                    Text = "Customer Balance Information",
                    Location = new Point(10, 5),
                    Size = new Size(200, 20),
                    Font = new Font("Microsoft Sans Serif", 9, FontStyle.Bold),
                    ForeColor = Color.DarkBlue
                };
                balancePanel.Controls.Add(lblTitle);

                // Current Balance
                lblCustomerBalance = new Label
                {
                    Text = "Current Balance:",
                    Location = new Point(10, 30),
                    Size = new Size(100, 20),
                    Font = new Font("Microsoft Sans Serif", 8, FontStyle.Regular)
                };
                balancePanel.Controls.Add(lblCustomerBalance);

                lblCurrentBalance = new Label
                {
                    Text = "$0.00",
                    Location = new Point(120, 30),
                    Size = new Size(80, 20),
                    Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold),
                    ForeColor = Color.Red
                };
                balancePanel.Controls.Add(lblCurrentBalance);

                // Credit Limit
                lblCreditLimit = new Label
                {
                    Text = "Credit Limit: $0.00",
                    Location = new Point(10, 55),
                    Size = new Size(150, 20),
                    Font = new Font("Microsoft Sans Serif", 8, FontStyle.Regular)
                };
                balancePanel.Controls.Add(lblCreditLimit);

                // Available Credit
                lblAvailableCredit = new Label
                {
                    Text = "Available: $0.00",
                    Location = new Point(10, 80),
                    Size = new Size(150, 20),
                    Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold),
                    ForeColor = Color.Green
                };
                balancePanel.Controls.Add(lblAvailableCredit);

                // View Balance Button
                btnViewBalance = new Button
                {
                    Text = "View Details",
                    Location = new Point(220, 30),
                    Size = new Size(100, 30),
                    BackColor = Color.SteelBlue,
                    ForeColor = Color.White,
                    Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold)
                };
                btnViewBalance.Click += BtnViewBalance_Click;
                balancePanel.Controls.Add(btnViewBalance);

                // Warning Label
                Label lblWarning = new Label
                {
                    Text = "⚠️ Check credit limit before proceeding",
                    Location = new Point(220, 65),
                    Size = new Size(120, 30),
                    Font = new Font("Microsoft Sans Serif", 7, FontStyle.Italic),
                    ForeColor = Color.Orange,
                    TextAlign = ContentAlignment.MiddleCenter
                };
                balancePanel.Controls.Add(lblWarning);

                this.Controls.Add(balancePanel);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error initializing customer balance display: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadCustomerBalance(int customerID)
        {
            try
            {
                if (customerID <= 0)
                {
                    // Reset balance display
                    lblCurrentBalance.Text = "$0.00";
                    lblCreditLimit.Text = "Credit Limit: $0.00";
                    lblAvailableCredit.Text = "Available: $0.00";
                    lblCurrentBalance.ForeColor = Color.Black;
                    return;
                }

                string query = @"SELECT 
                                c.CustomerName,
                                ISNULL(c.CreditLimit, 5000) as CreditLimit,
                                ISNULL(SUM(s.NetAmount), 0) as TotalSales,
                                ISNULL(SUM(cp.Amount), 0) as TotalPayments,
                                (ISNULL(SUM(s.NetAmount), 0) - ISNULL(SUM(cp.Amount), 0)) as CurrentBalance
                               FROM Customers c
                               LEFT JOIN Sales s ON c.CustomerID = s.CustomerID AND s.IsActive = 1 AND s.BillType = 'Credit'
                               LEFT JOIN CustomerPayments cp ON c.CustomerID = cp.CustomerID
                               WHERE c.CustomerID = @CustomerID
                               GROUP BY c.CustomerID, c.CustomerName, c.CreditLimit";

                SqlParameter[] parameters = { new SqlParameter("@CustomerID", customerID) };
                DataTable dt = DatabaseConnection.ExecuteQuery(query, parameters);

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    customerCurrentBalance = Convert.ToDecimal(row["CurrentBalance"]);
                    customerCreditLimit = Convert.ToDecimal(row["CreditLimit"]);
                    decimal availableCredit = customerCreditLimit - customerCurrentBalance;

                    // Update UI
                    lblCurrentBalance.Text = customerCurrentBalance.ToString("C2");
                    lblCreditLimit.Text = $"Credit Limit: {customerCreditLimit:C2}";
                    lblAvailableCredit.Text = $"Available: {availableCredit:C2}";

                    // Color coding based on balance status
                    if (customerCurrentBalance > customerCreditLimit)
                    {
                        lblCurrentBalance.ForeColor = Color.Red;
                        lblAvailableCredit.ForeColor = Color.Red;
                        lblAvailableCredit.Text = "⚠️ OVER LIMIT!";
                    }
                    else if (availableCredit < customerCreditLimit * 0.1m) // Less than 10% available
                    {
                        lblCurrentBalance.ForeColor = Color.Orange;
                        lblAvailableCredit.ForeColor = Color.Orange;
                    }
                    else
                    {
                        lblCurrentBalance.ForeColor = Color.Green;
                        lblAvailableCredit.ForeColor = Color.Green;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading customer balance: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnViewBalance_Click(object sender, EventArgs e)
        {
            if (selectedCustomerID <= 0)
            {
                MessageBox.Show("Please select a customer first.", "Information", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                // Open detailed customer balance form
                CustomerBalance balanceForm = new CustomerBalance();
                balanceForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening customer balance form: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateCreditLimit()
        {
            try
            {
                if (selectedCustomerID <= 0)
                    return true; // No customer selected, allow transaction

                decimal billTotal = 0;
                if (decimal.TryParse(label20.Text.Replace("$", "").Replace(",", ""), out billTotal))
                {
                    decimal newBalance = customerCurrentBalance + billTotal;
                    
                    if (newBalance > customerCreditLimit)
                    {
                        decimal overLimit = newBalance - customerCreditLimit;
                        DialogResult result = MessageBox.Show(
                            $"WARNING: This transaction will exceed the customer's credit limit!\n\n" +
                            $"Current Balance: {customerCurrentBalance:C2}\n" +
                            $"Bill Amount: {billTotal:C2}\n" +
                            $"New Balance: {newBalance:C2}\n" +
                            $"Credit Limit: {customerCreditLimit:C2}\n" +
                            $"Over Limit: {overLimit:C2}\n\n" +
                            $"Do you want to proceed anyway?",
                            "Credit Limit Exceeded", 
                            MessageBoxButtons.YesNo, 
                            MessageBoxIcon.Warning);
                        
                        return result == DialogResult.Yes;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error validating credit limit: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void SaveCreditBill()
        {
            try
            {
                // Validate credit limit before saving
                if (!ValidateCreditLimit())
                {
                    return;
                }

                // Insert into Sales table with BillType = 'Credit'
                string insertSaleQuery = @"INSERT INTO Sales (BillNumber, CustomerID, SaleDate, TotalAmount, Discount, NetAmount, BillType, IsActive) 
                                         VALUES (@BillNumber, @CustomerID, @SaleDate, @TotalAmount, @Discount, @NetAmount, 'Credit', 1);
                                         SELECT SCOPE_IDENTITY();";

                SqlParameter[] saleParams = {
                    new SqlParameter("@BillNumber", textBox11.Text),
                    new SqlParameter("@CustomerID", selectedCustomerID),
                    new SqlParameter("@SaleDate", DateTime.Now),
                    new SqlParameter("@TotalAmount", totalAmount),
                    new SqlParameter("@Discount", discount),
                    new SqlParameter("@NetAmount", netAmount)
                };

                object saleIdObj = DatabaseConnection.ExecuteScalar(insertSaleQuery, saleParams);
                int saleId = Convert.ToInt32(saleIdObj);

                // Insert sale items
                foreach (DataRow row in billItems.Rows)
                {
                    string insertItemQuery = @"INSERT INTO SaleItems (SaleID, ItemID, Quantity, Price, TotalAmount) 
                                             VALUES (@SaleID, @ItemID, @Quantity, @Price, @TotalAmount)";

                    SqlParameter[] itemParams = {
                        new SqlParameter("@SaleID", saleId),
                        new SqlParameter("@ItemID", GetItemIDByName(row["ItemName"].ToString())),
                        new SqlParameter("@Quantity", row["Quantity"]),
                        new SqlParameter("@Price", row["Rate"]),
                        new SqlParameter("@TotalAmount", row["TotalAmount"])
                    };

                    DatabaseConnection.ExecuteNonQuery(insertItemQuery, itemParams);
                }

                MessageBox.Show("Credit bill saved successfully!\nCustomer balance has been updated.", "Success", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Refresh customer balance display
                LoadCustomerBalance(selectedCustomerID);
                
                // Clear form for next transaction
                ClearFormEnhanced();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving credit bill: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int GetItemIDByName(string itemName)
        {
            try
            {
                string query = "SELECT ItemID FROM Items WHERE ItemName = @ItemName";
                SqlParameter[] parameters = { new SqlParameter("@ItemName", itemName) };
                object result = DatabaseConnection.ExecuteScalar(query, parameters);
                return Convert.ToInt32(result);
            }
            catch
            {
                return 0;
            }
        }

        private void ClearFormEnhanced()
        {
            // Clear all form fields
            billItems.Clear();
            totalAmount = 0;
            discount = 0;
            netAmount = 0;
            selectedItemID = 0;
            selectedCustomerID = 0;
            
            // Reset UI
            comboBox1.SelectedIndex = -1;
            listBoxItems.ClearSelected();
            
            // Clear balance display
            if (lblCurrentBalance != null)
            {
                lblCurrentBalance.Text = "$0.00";
                lblCreditLimit.Text = "Credit Limit: $0.00";
                lblAvailableCredit.Text = "Available: $0.00";
            }
            
            // Generate new bill number
            GenerateBillNumber();
        }
    }
}
