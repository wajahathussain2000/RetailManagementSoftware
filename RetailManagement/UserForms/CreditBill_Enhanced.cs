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
        }

        private void SetupEventHandlers()
        {
            // Setup event handlers for the form
            listBoxItems.SelectedIndexChanged += listBoxItems_SelectedIndexChanged;
            textBox2.TextChanged += textBox2_TextChanged;
            textBox9.TextChanged += textBox9_TextChanged;
            button14.Click += button14_Click;
        }

        private void CreateExpiryDatePicker()
        {
            // Create expiry date picker if needed (similar to NewBillForm)
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
                // Update display fields - credit bill uses same structure
                currentPackLooseMode = "P";
                UpdateRateBasedOnMode();
            }
        }

        private void UpdateRateBasedOnMode()
        {
            if (currentPackLooseMode == "P")
            {
                // Pack mode - use base rate
                textBox6.Text = baseRate.ToString("F2");
            }
            else
            {
                // Loose mode - divide by pack size
                decimal looseRate = baseRate / packSize;
                textBox6.Text = looseRate.ToString("F2");
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
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
                if (decimal.TryParse(textBox2.Text, out decimal quantity) && 
                    decimal.TryParse(textBox6.Text, out decimal rate))
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

            if (string.IsNullOrWhiteSpace(textBox2.Text) || !decimal.TryParse(textBox2.Text, out decimal qty) || qty <= 0)
            {
                MessageBox.Show("Please enter a valid quantity.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(textBox6.Text) || !decimal.TryParse(textBox6.Text, out decimal rate) || rate <= 0)
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
                string expDate = textBox3.Text;
                decimal quantity = decimal.Parse(textBox2.Text);
                decimal rate = decimal.Parse(textBox6.Text);
                int bonus = string.IsNullOrWhiteSpace(textBox4.Text) ? 0 : int.Parse(textBox4.Text);
                decimal dis1 = string.IsNullOrWhiteSpace(textBox5.Text) ? 0 : decimal.Parse(textBox5.Text);
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
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
            textBox7.Text = "";
            textBox8.Text = "";
            textBox15.Text = "";
            selectedItemID = 0;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (ValidateBill())
            {
                try
                {
                    SaveBill();
                    MessageBox.Show("Credit bill saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
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
    }
}

