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
    public partial class CustomerPayment : Form
    {
        private int selectedSupplierID = 0;
        private decimal supplierBalance = 0;
        private DataTable paymentHistory; // TODO: Implement payment history display

        public CustomerPayment()
        {
            InitializeComponent();
            LoadSuppliers();
            SetupDataGridView();
            SetDefaultDate();
            LoadPaymentMethods();
            SetupForm();
        }

        private void SetupForm()
        {
            // Enable payment recording functionality
            btnSave.Text = "Record Payment";
            btnSave.Enabled = false; // Enable only when customer is selected
            
            // Create payment amount input textbox
            CreatePaymentAmountInput();
            
            // Wire up event handlers
            button1.Click += button1_Click;
            btnSave.Click += btnSave_Click;
            btnCancel.Click += btnCancel_Click;
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
        }

        private void CreatePaymentAmountInput()
        {
            // Create payment amount textbox
            TextBox txtPaymentAmount = new TextBox
            {
                Name = "txtPaymentAmount",
                Location = new Point(31, 550),
                Size = new Size(120, 25),
                Font = new Font("Microsoft Sans Serif", 9, FontStyle.Bold),
                TextAlign = HorizontalAlignment.Right
            };
            txtPaymentAmount.TextChanged += TxtPaymentAmount_TextChanged;
            this.Controls.Add(txtPaymentAmount);
            
            // Add label for payment amount
            Label lblPaymentAmount = new Label
            {
                Text = "Payment Amount:",
                Location = new Point(31, 530),
                Size = new Size(100, 20),
                Font = new Font("Microsoft Sans Serif", 9, FontStyle.Bold)
            };
            this.Controls.Add(lblPaymentAmount);
        }

        private void TxtPaymentAmount_TextChanged(object sender, EventArgs e)
        {
            TextBox txtPaymentAmount = sender as TextBox;
            if (decimal.TryParse(txtPaymentAmount?.Text, out decimal amount) && amount > 0 && selectedSupplierID > 0)
            {
                btnSave.Enabled = true;
            }
            else
            {
                btnSave.Enabled = false;
            }
            
            UpdateCalculations();
        }

        private void LoadPaymentMethods()
        {
            try
            {
                // Setup payment method combo if it exists
                ComboBox paymentMethodCombo = this.Controls.OfType<ComboBox>()
                    .FirstOrDefault(c => c.Name == "comboBox2" || c.Name == "cmbPaymentMethod");
                
                if (paymentMethodCombo == null)
                {
                    // Create payment method combo if it doesn't exist
                    paymentMethodCombo = new ComboBox
                    {
                        Name = "cmbPaymentMethod",
                        Location = new Point(150, 80),
                        Size = new Size(150, 25),
                        DropDownStyle = ComboBoxStyle.DropDownList
                    };
                    this.Controls.Add(paymentMethodCombo);
                    
                    Label lblPaymentMethod = new Label
                    {
                        Text = "Payment Method:",
                        Location = new Point(30, 85),
                        Size = new Size(100, 20)
                    };
                    this.Controls.Add(lblPaymentMethod);
                }
                
                paymentMethodCombo.Items.Clear();
                paymentMethodCombo.Items.AddRange(new string[] { "Cash", "Credit Card", "Debit Card", "Bank Transfer", "Check" });
                paymentMethodCombo.SelectedIndex = 0; // Default to Cash
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting up payment methods: {ex.Message}");
            }
        }



        private void SetupDataGridView()
        {
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.Columns.Clear();

            // Using the actual columns defined in Designer
            dataGridView1.Columns.Add("BillNo", "Bill No");
            dataGridView1.Columns.Add("Description", "Description");
            dataGridView1.Columns.Add("Bank", "Bank");
            dataGridView1.Columns.Add("Payment", "Payment");
            dataGridView1.Columns.Add("Discount", "Discount");

            dataGridView1.Columns["BillNo"].DataPropertyName = "BillNo";
            dataGridView1.Columns["Description"].DataPropertyName = "Description";
            dataGridView1.Columns["Bank"].DataPropertyName = "Bank";
            dataGridView1.Columns["Payment"].DataPropertyName = "Payment";
            dataGridView1.Columns["Discount"].DataPropertyName = "Discount";
        }

        private void LoadSuppliers()
        {
            try
            {
                string query = "SELECT CompanyID, CompanyName, Phone FROM Companies WHERE IsActive = 1 ORDER BY CompanyName";
                DataTable dt = DatabaseConnection.ExecuteQuery(query);
                
                System.Diagnostics.Debug.WriteLine($"Loaded {dt.Rows.Count} suppliers");
                
                comboBox1.DataSource = dt;
                comboBox1.DisplayMember = "CompanyName";
                comboBox1.ValueMember = "CompanyID";
                
                if (dt.Rows.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"First supplier: ID={dt.Rows[0]["CompanyID"]}, Name={dt.Rows[0]["CompanyName"]}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading suppliers: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"Error in LoadSuppliers: {ex.Message}");
            }
        }

        private void LoadPayments()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Loading payments for Supplier ID: {selectedSupplierID}");
                
                if (selectedSupplierID == 0)
                {
                    dataGridView1.DataSource = null;
                    System.Diagnostics.Debug.WriteLine("No supplier selected, clearing grid");
                    return;
                }

                string query = @"SELECT 
                                p.PurchaseNumber as BillNo,
                                'Purchase - ' + p.PurchaseNumber as Description,
                                ISNULL(sp.PaymentMode, 'Cash') as Bank,
                                ISNULL(sp.Amount, 0) as Payment,
                                ISNULL(p.TotalDiscount, 0) as Discount
                               FROM Purchases p
                               LEFT JOIN SupplierPayments sp ON p.CompanyID = sp.SupplierID
                               WHERE p.CompanyID = @SupplierID AND p.IsActive = 1
                               ORDER BY p.PurchaseDate DESC";

                SqlParameter[] parameters = { new SqlParameter("@SupplierID", selectedSupplierID) };
                DataTable dt = DatabaseConnection.ExecuteQuery(query, parameters);
                
                System.Diagnostics.Debug.WriteLine($"Payment data loaded: {dt.Rows.Count} rows");
                dataGridView1.DataSource = dt;
                
                if (dt.Rows.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"First row data: BillNo={dt.Rows[0]["BillNo"]}, Description={dt.Rows[0]["Description"]}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading payments: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"Error in LoadPayments: {ex.Message}");
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.SelectedValue != null && comboBox1.SelectedValue != DBNull.Value && 
                    int.TryParse(comboBox1.SelectedValue.ToString(), out int supplierId))
                {
                    selectedSupplierID = supplierId;
                    System.Diagnostics.Debug.WriteLine($"Selected Supplier ID: {selectedSupplierID}");
                    
                    LoadSupplierBalance();
                    LoadPayments();
                    
                    // Enable payment amount field
                    if (textBox1 != null)
                    {
                        textBox1.Enabled = true;
                    }
                    
                    // Check if save button should be enabled
                    TxtPaymentAmount_TextChanged(null, null);
                }
                else
                {
                    // Clear data when no valid selection
                    selectedSupplierID = 0;
                    if (textBox1 != null)
                    {
                        textBox1.Text = "0.00";
                        textBox1.Enabled = false;
                    }
                    dataGridView1.DataSource = null;
                    UpdateCalculations();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in supplier selection: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"Error in comboBox1_SelectedIndexChanged: {ex.Message}");
            }
        }

        private void LoadSupplierBalance()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Loading balance for Supplier ID: {selectedSupplierID}");
                
                // Get total purchases for this supplier
                string purchaseQuery = @"SELECT ISNULL(SUM(TotalAmount), 0) FROM Purchases 
                                        WHERE CompanyID = @SupplierID AND IsActive = 1";
                SqlParameter[] purchaseParams = { new SqlParameter("@SupplierID", selectedSupplierID) };
                decimal totalPurchases = Convert.ToDecimal(DatabaseConnection.ExecuteScalar(purchaseQuery, purchaseParams) ?? 0);
                System.Diagnostics.Debug.WriteLine($"Total Purchases: {totalPurchases}");
                
                // Get total payments for this supplier
                string paymentQuery = @"SELECT ISNULL(SUM(Amount), 0) FROM SupplierPayments 
                                       WHERE SupplierID = @SupplierID AND IsActive = 1";
                SqlParameter[] paymentParams = { new SqlParameter("@SupplierID", selectedSupplierID) };
                decimal totalPayments = Convert.ToDecimal(DatabaseConnection.ExecuteScalar(paymentQuery, paymentParams) ?? 0);
                System.Diagnostics.Debug.WriteLine($"Total Payments: {totalPayments}");
                
                // Calculate outstanding balance (Total Purchases - Total Payments)
                supplierBalance = totalPurchases - totalPayments;
                System.Diagnostics.Debug.WriteLine($"Outstanding Balance: {supplierBalance}");
                
                // Update the Outstanding Balance display (textBox1 in the blue panel)
                if (textBox1 != null)
                {
                    textBox1.Text = supplierBalance.ToString("N2");
                    System.Diagnostics.Debug.WriteLine($"Updated textBox1 with: {textBox1.Text}");
                }
                
                // Update the calculations in the summary section
                UpdateCalculations();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading supplier balance: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"Error in LoadSupplierBalance: {ex.Message}");
            }
        }

        private void SetDefaultDate()
        {
            dateTimePicker1.Value = DateTime.Now;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.SelectedValue != null && selectedSupplierID > 0)
                {
                    System.Diagnostics.Debug.WriteLine("Get Data button clicked - Refreshing supplier data");
                    LoadSupplierBalance();
                    LoadPayments();
                    MessageBox.Show("Supplier data refreshed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Please select a supplier first.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error refreshing data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"Error in button1_Click: {ex.Message}");
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (ValidatePayment())
            {
                try
                {
                    int paymentID = RecordPayment();
                    
                    // Generate payment slip
                    GeneratePaymentSlip(paymentID);
                    
                    MessageBox.Show("Payment recorded successfully! Payment slip has been generated.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Refresh data
                    LoadSupplierBalance();
                    LoadPayments();
                    ClearPaymentForm();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error recording payment: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void UpdateCalculations()
        {
            try
            {
                // Calculate total payments made so far
                decimal totalPayments = 0;
                decimal totalDiscount = 0;
                
                System.Diagnostics.Debug.WriteLine($"UpdateCalculations called for Supplier ID: {selectedSupplierID}");
                
                if (selectedSupplierID > 0)
                {
                    // Get total payments
                    string paymentQuery = @"SELECT ISNULL(SUM(Amount), 0) FROM SupplierPayments 
                                          WHERE SupplierID = @SupplierID AND IsActive = 1";
                    SqlParameter[] paymentParams = { new SqlParameter("@SupplierID", selectedSupplierID) };
                    totalPayments = Convert.ToDecimal(DatabaseConnection.ExecuteScalar(paymentQuery, paymentParams) ?? 0);
                    
                    // Get total discount
                    string discountQuery = @"SELECT ISNULL(SUM(TotalDiscount), 0) FROM Purchases 
                                           WHERE CompanyID = @SupplierID AND IsActive = 1";
                    SqlParameter[] discountParams = { new SqlParameter("@SupplierID", selectedSupplierID) };
                    totalDiscount = Convert.ToDecimal(DatabaseConnection.ExecuteScalar(discountQuery, discountParams) ?? 0);
                    
                    System.Diagnostics.Debug.WriteLine($"Calculations - Total Payments: {totalPayments}, Total Discount: {totalDiscount}");
                }
                
                // Add current payment amount if entered
                TextBox txtPaymentAmount = this.Controls.Find("txtPaymentAmount", true).FirstOrDefault() as TextBox;
                if (txtPaymentAmount != null && decimal.TryParse(txtPaymentAmount.Text, out decimal currentPayment))
                {
                    totalPayments += currentPayment;
                    System.Diagnostics.Debug.WriteLine($"Added current payment: {currentPayment}, New total: {totalPayments}");
                }
                
                // Calculate balance left (Outstanding = Total Purchases - Total Payments)
                decimal balanceLeft = supplierBalance - totalPayments;
                System.Diagnostics.Debug.WriteLine($"Balance calculation: {supplierBalance} - {totalPayments} = {balanceLeft}");
                
                // Update the summary labels with null checks
                Label lblTotalCash = this.Controls.Find("label8", true).FirstOrDefault() as Label;
                Label lblDiscount = this.Controls.Find("label9", true).FirstOrDefault() as Label;
                Label lblBalanceLeft = this.Controls.Find("label10", true).FirstOrDefault() as Label;
                
                if (lblTotalCash != null) 
                {
                    lblTotalCash.Text = totalPayments.ToString("N2");
                    System.Diagnostics.Debug.WriteLine($"Updated Total Cash label: {lblTotalCash.Text}");
                }
                if (lblDiscount != null) 
                {
                    lblDiscount.Text = totalDiscount.ToString("N2");
                    System.Diagnostics.Debug.WriteLine($"Updated Discount label: {lblDiscount.Text}");
                }
                if (lblBalanceLeft != null) 
                {
                    lblBalanceLeft.Text = balanceLeft.ToString("N2");
                    System.Diagnostics.Debug.WriteLine($"Updated Balance Left label: {lblBalanceLeft.Text}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating calculations: {ex.Message}");
                // Don't show message box here as it might cause UI issues
            }
        }

        private int RecordPayment()
        {
            // Get payment amount from the dedicated input textbox
            TextBox txtPaymentAmount = this.Controls.Find("txtPaymentAmount", true).FirstOrDefault() as TextBox;
            if (txtPaymentAmount == null || !decimal.TryParse(txtPaymentAmount.Text, out decimal paymentAmount))
            {
                MessageBox.Show("Please enter a valid payment amount.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return 0;
            }
            
            DateTime paymentDate = dateTimePicker1.Value;
            
            // Get payment method
            ComboBox paymentMethodCombo = this.Controls.OfType<ComboBox>()
                .FirstOrDefault(c => c.Name == "cmbPaymentMethod");
            string paymentMethod = paymentMethodCombo?.SelectedItem?.ToString() ?? "Cash";

            // Insert payment record and get the PaymentID
            string paymentQuery = @"INSERT INTO SupplierPayments 
                                  (SupplierID, Amount, PaymentDate, PaymentMode, Remarks, CreatedDate, IsActive) 
                                  VALUES 
                                  (@SupplierID, @Amount, @PaymentDate, @PaymentMode, @Remarks, @CreatedDate, 1);
                                  SELECT SCOPE_IDENTITY();";

            SqlParameter[] paymentParams = {
                new SqlParameter("@SupplierID", selectedSupplierID),
                new SqlParameter("@Amount", paymentAmount),
                new SqlParameter("@PaymentDate", paymentDate),
                new SqlParameter("@PaymentMode", paymentMethod),
                new SqlParameter("@Remarks", $"Payment recorded via Supplier Payment form"),
                new SqlParameter("@CreatedDate", DateTime.Now)
            };

            object result = DatabaseConnection.ExecuteScalar(paymentQuery, paymentParams);
            return Convert.ToInt32(result);
        }

        private void ClearPaymentForm()
        {
            // Clear payment amount input
            TextBox txtPaymentAmount = this.Controls.Find("txtPaymentAmount", true).FirstOrDefault() as TextBox;
            if (txtPaymentAmount != null)
            {
                txtPaymentAmount.Text = "";
            }
            
            btnSave.Enabled = false;
            
            // Reset payment method to Cash
            ComboBox paymentMethodCombo = this.Controls.OfType<ComboBox>()
                .FirstOrDefault(c => c.Name == "cmbPaymentMethod");
            if (paymentMethodCombo != null)
            {
                paymentMethodCombo.SelectedIndex = 0;
            }
        }

        private bool ValidatePayment()
        {
            if (selectedSupplierID == 0)
            {
                MessageBox.Show("Please select a supplier.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            TextBox txtPaymentAmount = this.Controls.Find("txtPaymentAmount", true).FirstOrDefault() as TextBox;
            if (txtPaymentAmount == null || string.IsNullOrWhiteSpace(txtPaymentAmount.Text) || 
                !decimal.TryParse(txtPaymentAmount.Text, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Please enter a valid payment amount.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void GeneratePaymentSlip(int paymentID)
        {
            try
            {
                // Get payment details
                string query = @"SELECT 
                                sp.PaymentID,
                                sp.Amount,
                                sp.PaymentDate,
                                sp.PaymentMode,
                                sp.Remarks,
                                c.CompanyName,
                                c.Phone,
                                c.Address
                               FROM SupplierPayments sp
                               INNER JOIN Companies c ON sp.SupplierID = c.CompanyID
                               WHERE sp.PaymentID = @PaymentID";

                SqlParameter[] parameters = { new SqlParameter("@PaymentID", paymentID) };
                DataTable paymentData = DatabaseConnection.ExecuteQuery(query, parameters);

                if (paymentData.Rows.Count > 0)
                {
                    DataRow payment = paymentData.Rows[0];
                    
                    // Calculate remaining balance
                    decimal remainingBalance = supplierBalance;
                    TextBox txtPaymentAmount = this.Controls.Find("txtPaymentAmount", true).FirstOrDefault() as TextBox;
                    if (txtPaymentAmount != null && decimal.TryParse(txtPaymentAmount.Text, out decimal paidAmount))
                    {
                        remainingBalance -= paidAmount;
                    }

                    // Create payment slip content
                    string slipContent = GeneratePaymentSlipContent(payment, remainingBalance);
                    
                    // Show print preview dialog
                    ShowPaymentSlipPreview(slipContent);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating payment slip: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GeneratePaymentSlipContent(DataRow payment, decimal remainingBalance)
        {
            string slipContent = $@"
===========================================
           SUPPLIER PAYMENT RECEIPT
===========================================

Receipt No: SPAY{payment["PaymentID"]:D6}
Date: {Convert.ToDateTime(payment["PaymentDate"]):dd/MM/yyyy HH:mm}

===========================================
SUPPLIER DETAILS:
===========================================
Name: {payment["CompanyName"]}
Phone: {payment["Phone"]}
Address: {payment["Address"]}

===========================================
PAYMENT DETAILS:
===========================================
Payment Method: {payment["PaymentMode"]}
Amount Paid: Rs. {Convert.ToDecimal(payment["Amount"]):N2}
Previous Outstanding: Rs. {supplierBalance:N2}
Remaining Balance: Rs. {remainingBalance:N2}

===========================================
Remarks: {payment["Remarks"]}

Thank you for your payment!

===========================================
                  Aziz Hospital Pharmacy
                  Near Eid Gah Katchary Road A.P.E
                  Cell: 0300-0600894
===========================================
";
            return slipContent;
        }

        private void ShowPaymentSlipPreview(string slipContent)
        {
            // Create a simple form to show the payment slip
            Form slipForm = new Form
            {
                Text = "Payment Slip Preview",
                Size = new Size(500, 600),
                StartPosition = FormStartPosition.CenterParent
            };

            TextBox txtSlip = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Dock = DockStyle.Fill,
                Font = new Font("Courier New", 9),
                Text = slipContent,
                ReadOnly = true
            };

            Button btnPrint = new Button
            {
                Text = "Print Slip",
                Dock = DockStyle.Bottom,
                Height = 40
            };

            btnPrint.Click += (sender, e) => {
                PrintSlip(slipContent);
                slipForm.Close();
            };

            slipForm.Controls.Add(txtSlip);
            slipForm.Controls.Add(btnPrint);
            slipForm.ShowDialog();
        }

        private void PrintSlip(string slipContent)
        {
            try
            {
                System.Drawing.Printing.PrintDocument printDoc = new System.Drawing.Printing.PrintDocument();
                printDoc.PrintPage += (sender, e) => {
                    e.Graphics.DrawString(slipContent, new Font("Courier New", 8), Brushes.Black, 10, 10);
                };
                
                printDoc.Print();
                MessageBox.Show("Payment slip printed successfully!", "Print Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error printing slip: {ex.Message}", "Print Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
