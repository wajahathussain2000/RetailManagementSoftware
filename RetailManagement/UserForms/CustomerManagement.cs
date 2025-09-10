using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using RetailManagement.Database;

namespace RetailManagement.UserForms
{
    public partial class CustomerManagement : Form
    {
        private DataTable customersData;
        private int selectedCustomerId = 0;

        public CustomerManagement()
        {
            InitializeComponent();
            SetupDataGridView();
            LoadCustomers();
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panel1 = new Panel();
            this.panel2 = new Panel();
            this.groupBox1 = new GroupBox();
            this.groupBox2 = new GroupBox();
            
            // Form Controls
            this.dgvCustomers = new DataGridView();
            this.txtCustomerName = new TextBox();
            this.txtPhone = new TextBox();
            this.txtEmail = new TextBox();
            this.txtAddress = new TextBox();
            this.txtCity = new TextBox();
            this.txtState = new TextBox();
            this.txtPostalCode = new TextBox();
            this.txtGSTNumber = new TextBox();
            this.txtCreditLimit = new TextBox();
            this.txtCreditDays = new TextBox();
            this.chkIsActive = new CheckBox();
            
            // Labels
            this.lblCustomerName = new Label();
            this.lblPhone = new Label();
            this.lblEmail = new Label();
            this.lblAddress = new Label();
            this.lblCity = new Label();
            this.lblState = new Label();
            this.lblPostalCode = new Label();
            this.lblGSTNumber = new Label();
            this.lblCreditLimit = new Label();
            this.lblCreditDays = new Label();
            
            // Buttons
            this.btnAdd = new Button();
            this.btnUpdate = new Button();
            this.btnDelete = new Button();
            this.btnSave = new Button();
            this.btnCancel = new Button();
            this.btnRefresh = new Button();
            this.btnSearch = new Button();
            this.txtSearch = new TextBox();
            
            // Form properties
            this.Text = "Customer Management";
            this.Size = new Size(1400, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;
            this.WindowState = FormWindowState.Maximized;
            
            SetupControls();
            SetupEvents();
        }

        private void SetupControls()
        {
            // Header Panel
            panel1.Dock = DockStyle.Top;
            panel1.Height = 60;
            panel1.BackColor = Color.FromArgb(52, 152, 219);
            this.Controls.Add(panel1);

            Label titleLabel = new Label();
            titleLabel.Text = "Customer Management";
            titleLabel.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            titleLabel.ForeColor = Color.White;
            titleLabel.Location = new Point(20, 15);
            titleLabel.AutoSize = true;
            panel1.Controls.Add(titleLabel);

            // Main Panel
            panel2.Dock = DockStyle.Fill;
            panel2.Padding = new Padding(20);
            this.Controls.Add(panel2);

            // Customer List GroupBox
            groupBox1.Text = "Customer List";
            groupBox1.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            groupBox1.Location = new Point(20, 20);
            groupBox1.Size = new Size(800, 500);
            panel2.Controls.Add(groupBox1);

            // Search
            Label lblSearch = new Label();
            lblSearch.Text = "Search:";
            lblSearch.Location = new Point(20, 30);
            lblSearch.AutoSize = true;
            groupBox1.Controls.Add(lblSearch);

            txtSearch.Location = new Point(80, 27);
            txtSearch.Size = new Size(200, 25);
            groupBox1.Controls.Add(txtSearch);

            btnSearch.Text = "Search";
            btnSearch.Location = new Point(290, 27);
            btnSearch.Size = new Size(80, 25);
            btnSearch.BackColor = Color.FromArgb(52, 152, 219);
            btnSearch.ForeColor = Color.White;
            groupBox1.Controls.Add(btnSearch);

            btnRefresh.Text = "Refresh";
            btnRefresh.Location = new Point(380, 27);
            btnRefresh.Size = new Size(80, 25);
            btnRefresh.BackColor = Color.FromArgb(52, 73, 94);
            btnRefresh.ForeColor = Color.White;
            groupBox1.Controls.Add(btnRefresh);

            // DataGridView
            dgvCustomers.Location = new Point(20, 60);
            dgvCustomers.Size = new Size(760, 420);
            dgvCustomers.ReadOnly = true;
            dgvCustomers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCustomers.MultiSelect = false;
            dgvCustomers.AllowUserToAddRows = false;
            dgvCustomers.AllowUserToDeleteRows = false;
            groupBox1.Controls.Add(dgvCustomers);

            // Customer Details GroupBox
            groupBox2.Text = "Customer Details";
            groupBox2.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            groupBox2.Location = new Point(840, 20);
            groupBox2.Size = new Size(500, 500);
            panel2.Controls.Add(groupBox2);

            // Customer Details Fields
            int yPos = 30;
            int spacing = 35;

            AddFormField(groupBox2, "Customer Name:", lblCustomerName, txtCustomerName, yPos);
            yPos += spacing;
            AddFormField(groupBox2, "Phone:", lblPhone, txtPhone, yPos);
            yPos += spacing;
            AddFormField(groupBox2, "Email:", lblEmail, txtEmail, yPos);
            yPos += spacing;
            AddFormField(groupBox2, "Address:", lblAddress, txtAddress, yPos);
            yPos += spacing;
            AddFormField(groupBox2, "City:", lblCity, txtCity, yPos);
            yPos += spacing;
            AddFormField(groupBox2, "State:", lblState, txtState, yPos);
            yPos += spacing;
            AddFormField(groupBox2, "Postal Code:", lblPostalCode, txtPostalCode, yPos);
            yPos += spacing;
            AddFormField(groupBox2, "GST Number:", lblGSTNumber, txtGSTNumber, yPos);
            yPos += spacing;
            AddFormField(groupBox2, "Credit Limit:", lblCreditLimit, txtCreditLimit, yPos);
            yPos += spacing;
            AddFormField(groupBox2, "Credit Days:", lblCreditDays, txtCreditDays, yPos);
            yPos += spacing;

            chkIsActive.Text = "Active";
            chkIsActive.Location = new Point(150, yPos);
            chkIsActive.Checked = true;
            groupBox2.Controls.Add(chkIsActive);

            // Buttons Panel
            yPos += 40;
            
            // First row of buttons
            btnAdd.Text = "Add New";
            btnAdd.Location = new Point(20, yPos);
            btnAdd.Size = new Size(100, 35);
            btnAdd.BackColor = Color.FromArgb(46, 204, 113);
            btnAdd.ForeColor = Color.White;
            btnAdd.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            groupBox2.Controls.Add(btnAdd);

            btnUpdate.Text = "Update";
            btnUpdate.Location = new Point(130, yPos);
            btnUpdate.Size = new Size(100, 35);
            btnUpdate.BackColor = Color.FromArgb(52, 152, 219);
            btnUpdate.ForeColor = Color.White;
            btnUpdate.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnUpdate.Enabled = false;
            groupBox2.Controls.Add(btnUpdate);

            btnDelete.Text = "Delete";
            btnDelete.Location = new Point(240, yPos);
            btnDelete.Size = new Size(100, 35);
            btnDelete.BackColor = Color.FromArgb(231, 76, 60);
            btnDelete.ForeColor = Color.White;
            btnDelete.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnDelete.Enabled = false;
            groupBox2.Controls.Add(btnDelete);

            btnSave.Text = "Save";
            btnSave.Location = new Point(350, yPos);
            btnSave.Size = new Size(100, 35);
            btnSave.BackColor = Color.FromArgb(155, 89, 182);
            btnSave.ForeColor = Color.White;
            btnSave.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnSave.Enabled = false;
            groupBox2.Controls.Add(btnSave);

            // Second row of buttons
            btnCancel.Text = "Cancel";
            btnCancel.Location = new Point(20, yPos + 45);
            btnCancel.Size = new Size(100, 35);
            btnCancel.BackColor = Color.FromArgb(149, 165, 166);
            btnCancel.ForeColor = Color.White;
            btnCancel.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnCancel.Enabled = false;
            groupBox2.Controls.Add(btnCancel);

            // Add instruction label
            Label lblInstructions = new Label();
            lblInstructions.Text = "Click 'Add New' to create a customer or select a customer from the list to edit.";
            lblInstructions.Location = new Point(20, yPos + 100);
            lblInstructions.Size = new Size(450, 40);
            lblInstructions.Font = new Font("Segoe UI", 10, FontStyle.Italic);
            lblInstructions.ForeColor = Color.Gray;
            groupBox2.Controls.Add(lblInstructions);

            // Initially disable form but keep Add button enabled
            EnableForm(false);
            btnAdd.Enabled = true;
        }

        private void AddFormField(GroupBox parent, string labelText, Label label, TextBox textBox, int yPosition)
        {
            label.Text = labelText;
            label.Location = new Point(20, yPosition);
            label.Size = new Size(120, 20);
            parent.Controls.Add(label);

            textBox.Location = new Point(150, yPosition - 3);
            textBox.Size = new Size(320, 25);
            parent.Controls.Add(textBox);
        }

        private void SetupDataGridView()
        {
            // DataGridView will be configured automatically when data is loaded
            dgvCustomers.AutoGenerateColumns = true;
            dgvCustomers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCustomers.MultiSelect = false;
            dgvCustomers.AllowUserToAddRows = false;
            dgvCustomers.AllowUserToDeleteRows = false;
            dgvCustomers.ReadOnly = true;
            dgvCustomers.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvCustomers.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void SetupEvents()
        {
            btnAdd.Click += BtnAdd_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnDelete.Click += BtnDelete_Click;
            btnSave.Click += BtnSave_Click;
            btnCancel.Click += BtnCancel_Click;
            btnRefresh.Click += BtnRefresh_Click;
            btnSearch.Click += BtnSearch_Click;
            txtSearch.KeyPress += TxtSearch_KeyPress;
            dgvCustomers.CellClick += DgvCustomers_CellClick;
        }

        private void LoadCustomers()
        {
            try
            {
                string query = @"SELECT CustomerID, CustomerName, Phone, Email, City, 
                               CreditLimit, CurrentBalance, 
                               CASE WHEN IsActive = 1 THEN 'Active' ELSE 'Inactive' END as Status
                               FROM Customers 
                               ORDER BY CustomerName";
                
                customersData = DatabaseConnection.ExecuteQuery(query);
                dgvCustomers.DataSource = customersData;
                
                // Hide ID column
                if (dgvCustomers.Columns["CustomerID"] != null)
                    dgvCustomers.Columns["CustomerID"].Visible = false;
                
                // Format currency columns
                if (dgvCustomers.Columns["CreditLimit"] != null)
                {
                    dgvCustomers.Columns["CreditLimit"].DefaultCellStyle.Format = "N2";
                    dgvCustomers.Columns["CreditLimit"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                
                if (dgvCustomers.Columns["CurrentBalance"] != null)
                {
                    dgvCustomers.Columns["CurrentBalance"].DefaultCellStyle.Format = "N2";
                    dgvCustomers.Columns["CurrentBalance"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                
                // Set column headers
                if (dgvCustomers.Columns["CustomerName"] != null)
                    dgvCustomers.Columns["CustomerName"].HeaderText = "Customer Name";
                if (dgvCustomers.Columns["CreditLimit"] != null)
                    dgvCustomers.Columns["CreditLimit"].HeaderText = "Credit Limit";
                if (dgvCustomers.Columns["CurrentBalance"] != null)
                    dgvCustomers.Columns["CurrentBalance"].HeaderText = "Balance";
                
                // Auto-size columns
                dgvCustomers.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading customers: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvCustomers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvCustomers.Rows[e.RowIndex];
                selectedCustomerId = SafeDataHelper.SafeGetCellInt32(row, "CustomerID");
                
                // Load complete customer details from database
                try
                {
                    string query = @"SELECT CustomerID, CustomerName, Phone, Email, Address, City, State, 
                                   PostalCode, GST_Number, CreditLimit, CreditDays, CurrentBalance, IsActive
                                   FROM Customers WHERE CustomerID = @CustomerID";
                    
                    var parameters = new[] { new System.Data.SqlClient.SqlParameter("@CustomerID", selectedCustomerId) };
                    DataTable customerDetails = DatabaseConnection.ExecuteQuery(query, parameters);
                    
                    if (customerDetails.Rows.Count > 0)
                    {
                        DataRow customerRow = customerDetails.Rows[0];
                        
                        // Populate form fields
                        txtCustomerName.Text = SafeDataHelper.SafeToString(customerRow["CustomerName"]);
                        txtPhone.Text = SafeDataHelper.SafeToString(customerRow["Phone"]);
                        txtEmail.Text = SafeDataHelper.SafeToString(customerRow["Email"]);
                        txtAddress.Text = SafeDataHelper.SafeToString(customerRow["Address"]);
                        txtCity.Text = SafeDataHelper.SafeToString(customerRow["City"]);
                        txtState.Text = SafeDataHelper.SafeToString(customerRow["State"]);
                        txtPostalCode.Text = SafeDataHelper.SafeToString(customerRow["PostalCode"]);
                        txtGSTNumber.Text = SafeDataHelper.SafeToString(customerRow["GST_Number"]);
                        txtCreditLimit.Text = SafeDataHelper.SafeToDecimal(customerRow["CreditLimit"]).ToString("F2");
                        txtCreditDays.Text = SafeDataHelper.SafeToInt32(customerRow["CreditDays"]).ToString();
                        chkIsActive.Checked = SafeDataHelper.SafeToInt32(customerRow["IsActive"]) == 1;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading customer details: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                EnableForm(false);
                btnAdd.Enabled = true;
                btnUpdate.Enabled = true;
                btnDelete.Enabled = true;
                btnSave.Enabled = false;
                btnCancel.Enabled = false;
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            ClearForm();
            EnableForm(true);
            selectedCustomerId = 0;
            btnAdd.Enabled = false;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
            btnSave.Enabled = true;
            btnCancel.Enabled = true;
            txtCustomerName.Focus();
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedCustomerId > 0)
            {
                EnableForm(true);
                btnAdd.Enabled = false;
                btnUpdate.Enabled = false;
                btnDelete.Enabled = false;
                btnSave.Enabled = true;
                btnCancel.Enabled = true;
                txtCustomerName.Focus();
            }
            else
            {
                MessageBox.Show("Please select a customer to update.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (selectedCustomerId > 0)
            {
                DialogResult result = MessageBox.Show("Are you sure you want to delete this customer?", "Confirm Delete", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        string query = "UPDATE Customers SET IsActive = 0 WHERE CustomerID = @CustomerID";
                        SqlParameter[] parameters = { new SqlParameter("@CustomerID", selectedCustomerId) };

                        DatabaseConnection.ExecuteNonQuery(query, parameters);
                        MessageBox.Show("Customer deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadCustomers();
                        ClearForm();
                        EnableForm(false);
                        btnAdd.Enabled = true;
                        btnUpdate.Enabled = false;
                        btnDelete.Enabled = false;
                        btnSave.Enabled = false;
                        btnCancel.Enabled = false;
                        selectedCustomerId = 0;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error deleting customer: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a customer to delete.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                try
                {
                    if (selectedCustomerId == 0)
                    {
                        // Add new customer
                        string query = @"INSERT INTO Customers 
                                        (CustomerName, Phone, Email, Address, City, State, PostalCode, 
                                         GST_Number, CreditLimit, CreditDays, IsActive, CreatedDate) 
                                        VALUES (@CustomerName, @Phone, @Email, @Address, @City, @State, @PostalCode, 
                                               @GST_Number, @CreditLimit, @CreditDays, @IsActive, GETDATE())";

                        SqlParameter[] parameters = GetCustomerParameters();
                        DatabaseConnection.ExecuteNonQuery(query, parameters);
                        MessageBox.Show("Customer added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        // Update existing customer
                        string query = @"UPDATE Customers 
                                        SET CustomerName = @CustomerName, Phone = @Phone, Email = @Email, 
                                            Address = @Address, City = @City, State = @State, PostalCode = @PostalCode,
                                            GST_Number = @GST_Number, CreditLimit = @CreditLimit, CreditDays = @CreditDays,
                                            IsActive = @IsActive 
                                        WHERE CustomerID = @CustomerID";

                        SqlParameter[] parameters = GetCustomerParameters();
                        Array.Resize(ref parameters, parameters.Length + 1);
                        parameters[parameters.Length - 1] = new SqlParameter("@CustomerID", selectedCustomerId);

                        DatabaseConnection.ExecuteNonQuery(query, parameters);
                        MessageBox.Show("Customer updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    LoadCustomers();
                    ClearForm();
                    EnableForm(false);
                    btnAdd.Enabled = true;
                    btnUpdate.Enabled = false;
                    btnDelete.Enabled = false;
                    btnSave.Enabled = false;
                    btnCancel.Enabled = false;
                    selectedCustomerId = 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error saving customer: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private SqlParameter[] GetCustomerParameters()
        {
            return new SqlParameter[]
            {
                new SqlParameter("@CustomerName", txtCustomerName.Text.Trim()),
                new SqlParameter("@Phone", txtPhone.Text.Trim()),
                new SqlParameter("@Email", txtEmail.Text.Trim()),
                new SqlParameter("@Address", txtAddress.Text.Trim()),
                new SqlParameter("@City", txtCity.Text.Trim()),
                new SqlParameter("@State", txtState.Text.Trim()),
                new SqlParameter("@PostalCode", txtPostalCode.Text.Trim()),
                new SqlParameter("@GST_Number", txtGSTNumber.Text.Trim()),
                new SqlParameter("@CreditLimit", decimal.TryParse(txtCreditLimit.Text, out decimal credit) ? credit : 0),
                new SqlParameter("@CreditDays", int.TryParse(txtCreditDays.Text, out int days) ? days : 30),
                new SqlParameter("@IsActive", chkIsActive.Checked)
            };
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(txtCustomerName.Text))
            {
                MessageBox.Show("Customer name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCustomerName.Focus();
                return false;
            }

            if (!string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                try
                {
                    var addr = new System.Net.Mail.MailAddress(txtEmail.Text);
                    if (addr.Address != txtEmail.Text)
                        throw new FormatException();
                }
                catch
                {
                    MessageBox.Show("Please enter a valid email address.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtEmail.Focus();
                    return false;
                }
            }

            return true;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            ClearForm();
            EnableForm(false);
            btnAdd.Enabled = true;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
            btnSave.Enabled = false;
            btnCancel.Enabled = false;
            selectedCustomerId = 0;
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            LoadCustomers();
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            SearchCustomers();
        }

        private void TxtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                SearchCustomers();
            }
        }

        private void SearchCustomers()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    LoadCustomers();
                    return;
                }

                string searchText = txtSearch.Text.Trim();
                DataView dv = customersData.DefaultView;
                dv.RowFilter = $"CustomerName LIKE '%{searchText}%' OR Phone LIKE '%{searchText}%' OR Email LIKE '%{searchText}%'";
                dgvCustomers.DataSource = dv;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching customers: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearForm()
        {
            txtCustomerName.Clear();
            txtPhone.Clear();
            txtEmail.Clear();
            txtAddress.Clear();
            txtCity.Clear();
            txtState.Clear();
            txtPostalCode.Clear();
            txtGSTNumber.Clear();
            txtCreditLimit.Text = "0";
            txtCreditDays.Text = "30";
            chkIsActive.Checked = true;
        }

        private void EnableForm(bool enabled)
        {
            txtCustomerName.Enabled = enabled;
            txtPhone.Enabled = enabled;
            txtEmail.Enabled = enabled;
            txtAddress.Enabled = enabled;
            txtCity.Enabled = enabled;
            txtState.Enabled = enabled;
            txtPostalCode.Enabled = enabled;
            txtGSTNumber.Enabled = enabled;
            txtCreditLimit.Enabled = enabled;
            txtCreditDays.Enabled = enabled;
            chkIsActive.Enabled = enabled;
        }

        #region Designer Variables
        private System.ComponentModel.IContainer components = null;
        private Panel panel1;
        private Panel panel2;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private DataGridView dgvCustomers;
        private TextBox txtCustomerName;
        private TextBox txtPhone;
        private TextBox txtEmail;
        private TextBox txtAddress;
        private TextBox txtCity;
        private TextBox txtState;
        private TextBox txtPostalCode;
        private TextBox txtGSTNumber;
        private TextBox txtCreditLimit;
        private TextBox txtCreditDays;
        private CheckBox chkIsActive;
        private Label lblCustomerName;
        private Label lblPhone;
        private Label lblEmail;
        private Label lblAddress;
        private Label lblCity;
        private Label lblState;
        private Label lblPostalCode;
        private Label lblGSTNumber;
        private Label lblCreditLimit;
        private Label lblCreditDays;
        private Button btnAdd;
        private Button btnUpdate;
        private Button btnDelete;
        private Button btnSave;
        private Button btnCancel;
        private Button btnRefresh;
        private Button btnSearch;
        private TextBox txtSearch;
        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
