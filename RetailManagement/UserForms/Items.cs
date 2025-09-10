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
using RetailManagement.Utils;
using System.Text.RegularExpressions;

namespace RetailManagement.UserForms
{
    public partial class Items : Form
    {
        private bool isEditMode = false;
        private int selectedItemID = 0;
        private DataTable itemsData;

        private bool isDisposing = false;
        private int currentUserID = 1; // Default admin user
        private PictureBox picBarcode; // Barcode image display

        public Items()
        {
            InitializeComponent();
            InitializeForm();
            LoadCompanies();
            LoadItems();
            LoadCategories();
            LoadUnitTypes();
            SetupDataGridView();
            SetupEventHandlers();
            LoadExpiryAlerts();
            
            // Add FormClosing event to ensure cleanup
            this.FormClosing += Items_FormClosing;
        }

        public Items(int userID) : this()
        {
            currentUserID = userID;
        }

        private void Items_FormClosing(object sender, FormClosingEventArgs e)
        {
            CleanupResources();
        }



        private void InitializeForm()
        {
            // Set form properties
            this.Text = "Items Management";
            this.StartPosition = FormStartPosition.CenterScreen;

            // Initialize data table
            itemsData = new DataTable();

            // Add missing controls
            AddMissingControls();
            
            // Try to add new columns if they don't exist
            AddNewColumnsIfNotExist();
        }

        private void AddNewColumnsIfNotExist()
        {
            try
            {
                // Check if discount columns exist
                string checkDiscountQuery = "SELECT COUNT(*) FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Items]') AND name IN ('Disc1', 'Disc2', 'SalesTax')";
                int discountColumnCount = SafeDataHelper.SafeToInt32(DatabaseConnection.ExecuteScalar(checkDiscountQuery));
                
                if (discountColumnCount < 3)
                {
                    // Add missing discount columns
                    if (discountColumnCount == 0)
                    {
                        DatabaseConnection.ExecuteNonQuery("ALTER TABLE Items ADD Disc1 DECIMAL(5,2) DEFAULT 0");
                        DatabaseConnection.ExecuteNonQuery("ALTER TABLE Items ADD Disc2 DECIMAL(5,2) DEFAULT 0");
                        DatabaseConnection.ExecuteNonQuery("ALTER TABLE Items ADD SalesTax DECIMAL(5,2) DEFAULT 0");
                    }
                    else
                    {
                        // Add individual discount columns that are missing
                        string[] discountColumns = { "Disc1", "Disc2", "SalesTax" };
                        foreach (string column in discountColumns)
                        {
                            string checkColumnQuery = $"SELECT COUNT(*) FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Items]') AND name = '{column}'";
                            int exists = SafeDataHelper.SafeToInt32(DatabaseConnection.ExecuteScalar(checkColumnQuery));
                            if (exists == 0)
                            {
                                DatabaseConnection.ExecuteNonQuery($"ALTER TABLE Items ADD {column} DECIMAL(5,2) DEFAULT 0");
                            }
                        }
                    }
                }

                // Check if item detail columns exist
                string checkItemDetailQuery = "SELECT COUNT(*) FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Items]') AND name IN ('Packing', 'PurchasePrice', 'ReorderLevel')";
                int itemDetailColumnCount = SafeDataHelper.SafeToInt32(DatabaseConnection.ExecuteScalar(checkItemDetailQuery));
                
                if (itemDetailColumnCount < 3)
                {
                    // Add missing item detail columns
                    if (itemDetailColumnCount == 0)
                    {
                        DatabaseConnection.ExecuteNonQuery("ALTER TABLE Items ADD Packing NVARCHAR(100) DEFAULT ''");
                        DatabaseConnection.ExecuteNonQuery("ALTER TABLE Items ADD PurchasePrice DECIMAL(10,2) DEFAULT 0");
                        DatabaseConnection.ExecuteNonQuery("ALTER TABLE Items ADD ReorderLevel INT DEFAULT 0");
                    }
                    else
                    {
                        // Add individual item detail columns that are missing
                        string[] itemDetailColumns = { "Packing", "PurchasePrice", "ReorderLevel" };
                        foreach (string column in itemDetailColumns)
                        {
                            string checkColumnQuery = $"SELECT COUNT(*) FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Items]') AND name = '{column}'";
                            int exists = SafeDataHelper.SafeToInt32(DatabaseConnection.ExecuteScalar(checkColumnQuery));
                            if (exists == 0)
                            {
                                if (column == "Packing")
                                {
                                    DatabaseConnection.ExecuteNonQuery($"ALTER TABLE Items ADD {column} NVARCHAR(100) DEFAULT ''");
                                }
                                else if (column == "PurchasePrice")
                                {
                                    DatabaseConnection.ExecuteNonQuery($"ALTER TABLE Items ADD {column} DECIMAL(10,2) DEFAULT 0");
                                }
                                else if (column == "ReorderLevel")
                                {
                                    DatabaseConnection.ExecuteNonQuery($"ALTER TABLE Items ADD {column} INT DEFAULT 0");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error but don't show to user - columns might already exist
                System.Diagnostics.Debug.WriteLine($"Error adding columns: {ex.Message}");
            }
        }

        private void AddMissingControls()
        {
            // Add comprehensive medicine management controls
            AddEnhancedMedicineControls();
            
            // Add status label
            Label lblStatus = new Label
            {
                Text = "Total Items: 0",
                Location = new Point(604, 470),
                Size = new Size(200, 20),
                Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold),
                ForeColor = Color.Black,
                Name = "lblStatus"
            };
            this.Controls.Add(lblStatus);

            // Add delete button to panel2
            Button btnDelete = new Button
            {
                Text = "Delete",
                Location = new Point(410, 32),
                Size = new Size(75, 36),
                Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold),
                Enabled = false,
                Name = "btnDelete"
            };
            btnDelete.Click += btnDelete_Click;
            panel2.Controls.Add(btnDelete);

            // Add category filter label
            Label lblCategoryFilter = new Label
            {
                Text = "Filter by Category:",
                Location = new Point(604, 15),
                Size = new Size(100, 20),
                Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold),
                Name = "lblCategoryFilter"
            };
            this.Controls.Add(lblCategoryFilter);

            // Add category filter combo box
            ComboBox comboBoxCategory = new ComboBox
            {
                Location = new Point(710, 12),
                Size = new Size(150, 20),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Name = "comboBoxCategory"
            };
            comboBoxCategory.SelectedIndexChanged += comboBoxCategory_SelectedIndexChanged;
            this.Controls.Add(comboBoxCategory);

            // Update search textbox properties
            InitializeSearchTextBox();
        }

        private void InitializeSearchTextBox()
        {
            // Remove existing event handlers first to prevent duplicates
            txtSearch.TextChanged -= txtSearch_TextChanged;
            txtSearch.GotFocus -= TxtSearch_GotFocus;
            txtSearch.LostFocus -= TxtSearch_LostFocus;

            // Set initial state
            txtSearch.Text = "Search items...";
            txtSearch.ForeColor = Color.Gray;
            
            // Add event handlers
            txtSearch.TextChanged += txtSearch_TextChanged;
            txtSearch.GotFocus += TxtSearch_GotFocus;
            txtSearch.LostFocus += TxtSearch_LostFocus;
        }

        private void TxtSearch_GotFocus(object sender, EventArgs e)
        {
            if (!isDisposing && !this.IsDisposed && txtSearch != null)
            {
                if (txtSearch.Text == "Search items...")
                {
                    txtSearch.Text = "";
                    txtSearch.ForeColor = Color.Black;
                }
            }
        }

        private void TxtSearch_LostFocus(object sender, EventArgs e)
        {
            if (!isDisposing && !this.IsDisposed && txtSearch != null)
            {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    txtSearch.Text = "Search items...";
                    txtSearch.ForeColor = Color.Gray;
                }
            }
        }

        private void SetupEventHandlers()
        {
            // Add missing event handlers
            btnSave.Click += btnSave_Click;
            btnSaveNew.Click += btnSaveNew_Click;
            btnCancel.Click += btnCancel_Click;
            btnPrint.Click += btnPrint_Click;
            btnAdd.Click += btnAdd_Click;
            gridViewProducts.CellClick += gridViewProducts_CellClick;
        }

        private void SetupDataGridView()
        {
            gridViewProducts.AutoGenerateColumns = false;
            gridViewProducts.Columns.Clear();

            // Add columns with proper formatting
            DataGridViewTextBoxColumn colItemID = new DataGridViewTextBoxColumn
            {
                Name = "ItemID",
                HeaderText = "Item ID",
                DataPropertyName = "ItemID",
                Width = 80,
                ReadOnly = true
            };

            DataGridViewTextBoxColumn colItemName = new DataGridViewTextBoxColumn
            {
                Name = "ItemName",
                HeaderText = "Item Name",
                DataPropertyName = "ItemName",
                Width = 200
            };

            DataGridViewTextBoxColumn colDescription = new DataGridViewTextBoxColumn
            {
                Name = "Description",
                HeaderText = "Description",
                DataPropertyName = "Description",
                Width = 250
            };

            DataGridViewTextBoxColumn colPrice = new DataGridViewTextBoxColumn
            {
                Name = "Price",
                HeaderText = "Price",
                DataPropertyName = "Price",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" }
            };

            DataGridViewTextBoxColumn colStockQuantity = new DataGridViewTextBoxColumn
            {
                Name = "StockQuantity",
                HeaderText = "Stock",
                DataPropertyName = "StockQuantity",
                Width = 80
            };

            DataGridViewTextBoxColumn colCategory = new DataGridViewTextBoxColumn
            {
                Name = "Category",
                HeaderText = "Category",
                DataPropertyName = "Category",
                Width = 150
            };

            DataGridViewTextBoxColumn colCompany = new DataGridViewTextBoxColumn
            {
                Name = "CompanyName",
                HeaderText = "Company",
                DataPropertyName = "CompanyName",
                Width = 150
            };

            DataGridViewTextBoxColumn colCreatedDate = new DataGridViewTextBoxColumn
            {
                Name = "CreatedDate",
                HeaderText = "Created Date",
                DataPropertyName = "CreatedDate",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" }
            };

            gridViewProducts.Columns.AddRange(new DataGridViewColumn[] {
                colItemID, colItemName, colDescription, colPrice, 
                colStockQuantity, colCategory, colCompany, colCreatedDate
            });

            // Set grid properties
            gridViewProducts.AllowUserToAddRows = false;
            gridViewProducts.AllowUserToDeleteRows = false;
            gridViewProducts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridViewProducts.MultiSelect = false;
            gridViewProducts.ReadOnly = true;
            gridViewProducts.RowHeadersVisible = false;
            gridViewProducts.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;
        }

        private void LoadItems()
        {
            try
            {
                // Get the correct stock column name
                string stockColumnName = DatabaseConnection.GetStockColumnName();
                
                // Check if Barcode column exists
                string barcodeColumn = "";
                try
                {
                    string checkColumnQuery = "SELECT COUNT(*) FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Items]') AND name = 'Barcode'";
                    int columnExists = SafeDataHelper.SafeToInt32(DatabaseConnection.ExecuteScalar(checkColumnQuery));
                    if (columnExists > 0)
                    {
                        barcodeColumn = ", i.Barcode";
                    }
                }
                catch
                {
                    // If checking fails, assume column doesn't exist
                    barcodeColumn = "";
                }

                // Check if new discount and tax columns exist
                bool hasDiscountColumns = false;
                try
                {
                    string checkDiscountQuery = "SELECT COUNT(*) FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Items]') AND name IN ('Disc1', 'Disc2', 'SalesTax')";
                    int discountColumnsExist = SafeDataHelper.SafeToInt32(DatabaseConnection.ExecuteScalar(checkDiscountQuery));
                    hasDiscountColumns = discountColumnsExist >= 3; // All 3 columns must exist
                }
                catch
                {
                    hasDiscountColumns = false;
                }

                string discountColumns = hasDiscountColumns ? ", i.Disc1, i.Disc2, i.SalesTax" : ", 0 as Disc1, 0 as Disc2, 0 as SalesTax";
                
                string query = $@"SELECT i.ItemID, i.ItemName, i.Description, i.Price, i.{stockColumnName} as StockQuantity, 
                               i.Category{barcodeColumn}, i.CreatedDate, ISNULL(c.CompanyName, 'No Company') as CompanyName, i.CompanyID{discountColumns},
                               i.Packing, i.PurchasePrice, i.ReorderLevel
                               FROM Items i 
                               LEFT JOIN Companies c ON i.CompanyID = c.CompanyID
                               WHERE i.IsActive = 1 
                               ORDER BY i.ItemName";
                
                itemsData = DatabaseConnection.ExecuteQuery(query);
                gridViewProducts.DataSource = itemsData;
                
                // Update status
                UpdateStatusLabel();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading items: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadCategories()
        {
            try
            {
                // Load categories into category filter combo box using DataSource
                string categoryQuery = "SELECT DISTINCT Category FROM Items WHERE IsActive = 1 AND Category IS NOT NULL ORDER BY Category";
                DataTable categoryData = DatabaseConnection.ExecuteQuery(categoryQuery);
                
                ComboBox comboBoxCategory = this.Controls.OfType<ComboBox>().FirstOrDefault(c => c.Location.X == 710);
                if (comboBoxCategory != null)
                {
                    DataTable comboBoxCategoryData = new DataTable();
                    comboBoxCategoryData.Columns.Add("Category", typeof(string));
                    comboBoxCategoryData.Rows.Add("All Categories");
                    
                    foreach (DataRow row in categoryData.Rows)
                    {
                        comboBoxCategoryData.Rows.Add(row["Category"].ToString());
                    }
                    
                    comboBoxCategory.DataSource = comboBoxCategoryData;
                    comboBoxCategory.DisplayMember = "Category";
                    comboBoxCategory.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading categories: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadCompanies()
        {
            try
            {
                // Remove existing event handler first to prevent duplicates
                comboBox1.SelectedIndexChanged -= ComboBox1_SelectedIndexChanged;
                
                // Load companies into comboBox1 using DataSource
                string query = "SELECT CompanyID, CompanyName FROM Companies WHERE IsActive = 1 ORDER BY CompanyName";
                DataTable companiesData = DatabaseConnection.ExecuteQuery(query);
                
                // Create a DataTable for ComboBox binding with additional "All Companies" option
                DataTable comboBox1Data = new DataTable();
                comboBox1Data.Columns.Add("CompanyID", typeof(int));
                comboBox1Data.Columns.Add("CompanyName", typeof(string));
                
                // Add "All Companies" option
                comboBox1Data.Rows.Add(0, "All Companies");
                
                // Add actual companies
                foreach (DataRow row in companiesData.Rows)
                {
                    comboBox1Data.Rows.Add(row["CompanyID"], row["CompanyName"]);
                }
                
                comboBox1.DataSource = comboBox1Data;
                comboBox1.ValueMember = "CompanyID";
                comboBox1.DisplayMember = "CompanyName";
                comboBox1.SelectedIndex = 0;
                
                // Make ComboBox searchable
                comboBox1.DropDownStyle = ComboBoxStyle.DropDown;
                comboBox1.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                comboBox1.AutoCompleteSource = AutoCompleteSource.ListItems;
                
                // Add selection changed event
                comboBox1.SelectedIndexChanged += ComboBox1_SelectedIndexChanged;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading companies: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isDisposing && !this.IsDisposed)
            {
                FilterItemsByCompany();
            }
        }

        private void FilterItemsByCompany()
        {
            try
            {
                if (comboBox1.SelectedValue != null)
                {
                    int selectedCompanyID = Convert.ToInt32(comboBox1.SelectedValue);
                    
                    if (selectedCompanyID == 0) // "All Companies" selected
                    {
                        LoadItems(); // Load all items
                    }
                    else
                    {
                        // Load items for specific company
                        string stockColumnName = DatabaseConnection.GetStockColumnName();
                        // Check if Barcode column exists
                        string barcodeColumn = "";
                        try
                        {
                            string checkColumnQuery = "SELECT COUNT(*) FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Items]') AND name = 'Barcode'";
                            int columnExists = SafeDataHelper.SafeToInt32(DatabaseConnection.ExecuteScalar(checkColumnQuery));
                            if (columnExists > 0)
                            {
                                barcodeColumn = ", i.Barcode";
                            }
                        }
                        catch
                        {
                            barcodeColumn = "";
                        }

                        // Check if new discount and tax columns exist
                        bool hasDiscountColumns = false;
                        try
                        {
                            string checkDiscountQuery = "SELECT COUNT(*) FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Items]') AND name IN ('Disc1', 'Disc2', 'SalesTax')";
                            int discountColumnsExist = SafeDataHelper.SafeToInt32(DatabaseConnection.ExecuteScalar(checkDiscountQuery));
                            hasDiscountColumns = discountColumnsExist >= 3; // All 3 columns must exist
                        }
                        catch
                        {
                            hasDiscountColumns = false;
                        }

                        string discountColumns = hasDiscountColumns ? ", i.Disc1, i.Disc2, i.SalesTax" : ", 0 as Disc1, 0 as Disc2, 0 as SalesTax";
                        
                        string query = $@"SELECT i.ItemID, i.ItemName, i.Description, i.Price, i.{stockColumnName} as StockQuantity, 
                                       i.Category{barcodeColumn}, i.CreatedDate, c.CompanyName, i.CompanyID{discountColumns},
                                       i.Packing, i.PurchasePrice, i.ReorderLevel
                                       FROM Items i 
                                       LEFT JOIN Companies c ON i.CompanyID = c.CompanyID
                                       WHERE i.IsActive = 1 AND i.CompanyID = @CompanyID
                                       ORDER BY i.ItemName";
                        
                        SqlParameter[] parameters = { new SqlParameter("@CompanyID", selectedCompanyID) };
                        itemsData = DatabaseConnection.ExecuteQuery(query, parameters);
                        gridViewProducts.DataSource = itemsData;
                        
                        UpdateStatusLabel();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error filtering items by company: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddEnhancedMedicineControls()
        {
            // Only add minimal essential features without disrupting the existing form layout
            AddBarcodeTextBox();
            AddEssentialButtons();
            AddInfoStatusBox();
        }
        
        private void AddBarcodeTextBox()
        {
            // Just add the editable barcode textbox where the original placeholder was
            TextBox txtBarcode = new TextBox()
            {
                Name = "txtBarcode", 
                Location = new Point(250, 233), // Position where barcode field should be
                Size = new Size(150, 20),
                Text = "Enter or auto-generate",
                ForeColor = Color.Gray,
                BackColor = Color.LightYellow
            };
            
            // Add barcode functionality
            txtBarcode.TextChanged += TxtBarcode_TextChanged;
            txtBarcode.KeyDown += TxtBarcode_KeyDown;
            txtBarcode.GotFocus += TxtBarcode_GotFocus;
            txtBarcode.LostFocus += TxtBarcode_LostFocus;
            
            panel1.Controls.Add(txtBarcode);
            
            // Create the missing PictureBox for barcode image display
            picBarcode = new PictureBox()
            {
                Name = "picBarcode",
                Location = new Point(250, 260), // Position below the textbox
                Size = new Size(200, 80),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                SizeMode = PictureBoxSizeMode.CenterImage
            };
            
            panel1.Controls.Add(picBarcode);
            
            // Add a label for the barcode image
            Label lblBarcodeImage = new Label()
            {
                Name = "lblBarcodeImage",
               
                Location = new Point(250, 245),
                Size = new Size(100, 15),
                Font = new Font("Microsoft Sans Serif", 8f, FontStyle.Bold)
            };
            
            panel1.Controls.Add(lblBarcodeImage);
        }
        
        private void AddEssentialButtons()
        {
            // Add only the essential Manage and Substitutes buttons as requested
            Button btnManageSubstitutes = new Button()
            {
                Name = "btnManageSubstitutes",
                Text = "Substitutes",
                Location = new Point(740, 435), // Position below other form elements
                Size = new Size(100, 30),
                BackColor = Color.LightGreen,
                Font = new Font("Microsoft Sans Serif", 9f, FontStyle.Bold)
            };
            btnManageSubstitutes.Click += BtnManageSubstitutes_Click;
            panel1.Controls.Add(btnManageSubstitutes);
        }
        
        private void AddInfoStatusBox()
        {
            // Create System Status box similar to UserMainScreen
            GroupBox infoBox = new GroupBox()
            {
                Text = "System Status",
                Name = "infoBox",
                Location = new Point(580, 420), // Move to bottom
                Size = new Size(280, 100),
                Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent
            };
            
            // Today label and date
            Label lblToday = new Label()
            {
                Text = "Today: " + DateTime.Now.ToString("dddd, MMMM dd, yyyy"),
                Location = new Point(10, 20),
                Size = new Size(250, 15),
                Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular),
                ForeColor = Color.Gray,
                BackColor = Color.Transparent
            };
            
            // Time label
            Label lblTime = new Label()
            {
                Text = "Time: " + DateTime.Now.ToString("h:mm:ss tt"),
                Name = "lblTime",
                Location = new Point(10, 35),
                Size = new Size(250, 15),
                Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular),
                ForeColor = Color.Gray,
                BackColor = Color.Transparent
            };
            
            // Low Stock Items
            Label lblLowStock = new Label()
            {
                Text = "Low Stock Items: ",
                Name = "lblLowStock",
                Location = new Point(10, 50),
                Size = new Size(200, 15),
                Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular),
                ForeColor = Color.Red,
                BackColor = Color.Transparent
            };
            
            // Session info
            Label lblSession = new Label()
            {
                Text = "Session: " + (UserSession.Role ?? "User") + " | " + DateTime.Now.ToString("h:mm tt"),
                Location = new Point(10, 65),
                Size = new Size(250, 15),
                Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular),
                ForeColor = Color.Gray,
                BackColor = Color.Transparent
            };
            
            // Add all labels to the group box
            infoBox.Controls.Add(lblToday);
            infoBox.Controls.Add(lblTime);
            infoBox.Controls.Add(lblLowStock);
            infoBox.Controls.Add(lblSession);
            
            // Add the group box to panel1
            panel1.Controls.Add(infoBox);
            
            // Load the actual statistics
            LoadItemStatistics();
        }
        
        private void LoadItemStatistics()
        {
            try
            {
                // Get low stock count (items with stock < 10)
                string stockColumnName = DatabaseConnection.GetStockColumnName();
                string lowStockQuery = $"SELECT COUNT(*) FROM Items WHERE IsActive = 1 AND {stockColumnName} < 10";
                int lowStockItems = Convert.ToInt32(DatabaseConnection.ExecuteScalar(lowStockQuery));
                
                // Update the low stock label
                var lblLowStock = panel1.Controls.OfType<GroupBox>()
                    .FirstOrDefault(g => g.Name == "infoBox")?.Controls.OfType<Label>()
                    .FirstOrDefault(l => l.Name == "lblLowStock");
                if (lblLowStock != null) 
                {
                    lblLowStock.Text = $"Low Stock Items: {lowStockItems}";
                    lblLowStock.ForeColor = lowStockItems > 0 ? Color.Red : Color.Green;
                }
                
                // Update time every time this is called
                var lblTime = panel1.Controls.OfType<GroupBox>()
                    .FirstOrDefault(g => g.Name == "infoBox")?.Controls.OfType<Label>()
                    .FirstOrDefault(l => l.Name == "lblTime");
                if (lblTime != null) 
                {
                    lblTime.Text = "Time: " + DateTime.Now.ToString("h:mm:ss tt");
                }
            }
            catch (Exception ex)
            {
                // Handle errors silently or show in status
                System.Diagnostics.Debug.WriteLine($"Error loading item statistics: {ex.Message}");
            }
        }
        
        private void LoadUnitTypes()
        {
            ComboBox cmbUnitType = panel1.Controls.OfType<ComboBox>().FirstOrDefault(c => c.Name == "cmbUnitType");
            if (cmbUnitType != null)
            {
                cmbUnitType.Items.Clear();
                cmbUnitType.Items.AddRange(new string[] {
                    "Tablet", "Capsule", "Syrup", "Injection", "Bottle", "Strip", 
                    "Tube", "Vial", "Drops", "Cream", "Ointment", "Powder", "Sachet"
                });
                cmbUnitType.SelectedIndex = 0; // Default to Tablet
            }
        }
        
        private void LoadExpiryAlerts()
        {
            try
            {
                // Load and display expiry alerts using stored procedure
                int alertCount = 0;
                try
                {
                    // Try using the stored procedure first
                    string spQuery = "EXEC sp_GetExpiryAlerts @DaysAhead = 90";
                    DataTable expiryData = DatabaseConnection.ExecuteQuery(spQuery);
                    alertCount = expiryData.Rows.Count;
                }
                catch
                {
                    // Fallback to direct query
                    string alertQuery = @"SELECT COUNT(*) FROM ExpiryAlerts 
                                        WHERE IsActive = 1 AND IsAcknowledged = 0 
                                        AND AlertType IN ('EXPIRING', 'EXPIRED', 'EXPIRING_SOON')";
                    
                    alertCount = SafeDataHelper.SafeToInt32(DatabaseConnection.ExecuteScalar(alertQuery));
                }
                
                if (alertCount > 0)
                {
                    // Update expiry report button to show alert count
                    Button btnExpiryReport = panel2.Controls.OfType<Button>().FirstOrDefault(b => b.Name == "btnExpiryReport");
                    if (btnExpiryReport != null)
                    {
                        btnExpiryReport.Text = $"Expiry ({alertCount})";
                        btnExpiryReport.BackColor = alertCount > 5 ? Color.DarkRed : Color.Red;
                    }
                }
            }
            catch (Exception)
            {
                // Handle silently as this is not critical
            }
        }

        private void UpdateStatusLabel()
        {
            Label lblStatus = this.Controls.OfType<Label>().FirstOrDefault(l => l.Text.StartsWith("Total Items:"));
            if (lblStatus != null && itemsData != null)
            {
                int lowStockCount = 0;
                int expiredCount = 0;
                
                try
                {
                    // Get low stock count
                    string lowStockQuery = "SELECT COUNT(*) FROM Items WHERE StockQuantity <= ReorderLevel AND IsActive = 1";
                    lowStockCount = Convert.ToInt32(DatabaseConnection.ExecuteScalar(lowStockQuery));
                    
                    // Get expired items count
                    string expiredQuery = @"SELECT COUNT(DISTINCT ib.ItemID) FROM ItemBatches ib 
                                          WHERE ib.ExpiryDate <= GETDATE() AND ib.IsActive = 1 AND ib.QuantityAvailable > 0";
                    expiredCount = Convert.ToInt32(DatabaseConnection.ExecuteScalar(expiredQuery));
                }
                catch
                {
                    // Handle silently
                }
                
                lblStatus.Text = $"Total: {itemsData.Rows.Count} | Low Stock: {lowStockCount} | Expired: {expiredCount}";
                lblStatus.ForeColor = (lowStockCount > 0 || expiredCount > 0) ? Color.Red : Color.Black;
            }
        }

        private void ClearForm()
        {
            textBox1.Text = ""; // Name
            textBox2.Text = ""; // Packing
            textBox3.Text = ""; // Location
            textBox4.Text = ""; // Retail Price
            textBox5.Text = ""; // Purchase Price
            textBox6.Text = ""; // Pack Size
            textBox7.Text = ""; // Re-Order Level
            textBox8.Text = ""; // Generic Name
            txtDisc1.Text = ""; // Discount 1
            txtDisc2.Text = ""; // Discount 2
            txtSalesTax.Text = ""; // Sales Tax
           
           
            
            
            // Clear enhanced medicine fields
            ClearEnhancedFields();
            
            selectedItemID = 0;
            isEditMode = false;
            
            // Reset company selection to "All Companies"
            comboBox1.SelectedIndex = 0;
            
            // Clear selection in grid
            gridViewProducts.ClearSelection();
        }
        
        private void ClearEnhancedFields()
        {
            // Clear enhanced medicine management fields
            var txtBarcode = panel1.Controls.OfType<TextBox>().FirstOrDefault(t => t.Name == "txtBarcode");
            if (txtBarcode != null) 
            {
                txtBarcode.Text = "";
                txtBarcode.BackColor = Color.LightYellow;
            }
            
            // Clear barcode image
            var picBarcode = panel1.Controls.OfType<PictureBox>().FirstOrDefault(p => p.Name == "picBarcode");
            if (picBarcode != null) 
            {
                picBarcode.Image = null;
            }
            
            var txtHSN = panel1.Controls.OfType<TextBox>().FirstOrDefault(t => t.Name == "txtHSN");
            if (txtHSN != null) txtHSN.Text = "";
            
            var cmbUnitType = panel1.Controls.OfType<ComboBox>().FirstOrDefault(c => c.Name == "cmbUnitType");
            if (cmbUnitType != null) cmbUnitType.SelectedIndex = 0;
            
            var txtMRP = panel1.Controls.OfType<TextBox>().FirstOrDefault(t => t.Name == "txtMRP");
            if (txtMRP != null) txtMRP.Text = "";
            
            var txtGSTRate = panel1.Controls.OfType<TextBox>().FirstOrDefault(t => t.Name == "txtGSTRate");
            if (txtGSTRate != null) txtGSTRate.Text = "12.00"; // Default GST rate
            
            var txtMinStock = panel1.Controls.OfType<TextBox>().FirstOrDefault(t => t.Name == "txtMinStock");
            if (txtMinStock != null) txtMinStock.Text = "10";
            
            var txtMaxStock = panel1.Controls.OfType<TextBox>().FirstOrDefault(t => t.Name == "txtMaxStock");
            if (txtMaxStock != null) txtMaxStock.Text = "1000";
            
            var chkPrescription = panel1.Controls.OfType<CheckBox>().FirstOrDefault(c => c.Name == "chkPrescription");
            if (chkPrescription != null) chkPrescription.Checked = false;
            
            checkBox1.Checked = false; // IsBlocked
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            ClearForm();
            btnSaveNew.Enabled = true;
            btnCancel.Enabled = true;
            textBox1.Focus();
            
            // Auto-generate barcode for new item
            int selectedCompanyID = 0;
            if (comboBox1.SelectedValue != null)
            {
                selectedCompanyID = Convert.ToInt32(comboBox1.SelectedValue);
            }
            
            // Generate barcode and display it immediately
            AutoGenerateAndDisplayBarcode("", selectedCompanyID, "NEW");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // This button is now only for updating existing items
            if (ValidateForm())
            {
                try
                {
                    UpdateItem();
                    LoadItems();
                    ClearForm();
                    
                    // Reset form state after successful update
                    isEditMode = false;
                    selectedItemID = 0;
                    btnSave.Enabled = false;
                    btnCancel.Enabled = false;
                    
                    // Disable delete button
                    var btnDelete = panel2.Controls.OfType<Button>().FirstOrDefault(b => b.Name == "btnDelete");
                    if (btnDelete != null) btnDelete.Enabled = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating item: " + ex.Message, "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnSaveNew_Click(object sender, EventArgs e)
        {
            // This button is for adding new items
            if (ValidateForm())
            {
                try
                {
                    InsertItem();
                    LoadItems();
                    ClearForm();
                    
                    // Reset form state after successful save
                    btnSaveNew.Enabled = false;
                    btnCancel.Enabled = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error adding item: " + ex.Message, "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void InsertItem()
        {
            string stockColumnName = DatabaseConnection.GetStockColumnName();
            
            // Check if Barcode column exists
            bool hasBarcodeColumn = false;
            try
            {
                string checkColumnQuery = "SELECT COUNT(*) FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Items]') AND name = 'Barcode'";
                int columnExists = SafeDataHelper.SafeToInt32(DatabaseConnection.ExecuteScalar(checkColumnQuery));
                hasBarcodeColumn = columnExists > 0;
            }
            catch
            {
                hasBarcodeColumn = false;
            }

            // Check if new discount and tax columns exist
            bool hasDiscountColumns = false;
            try
            {
                string checkDiscountQuery = "SELECT COUNT(*) FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Items]') AND name IN ('Disc1', 'Disc2', 'SalesTax')";
                int discountColumnsExist = SafeDataHelper.SafeToInt32(DatabaseConnection.ExecuteScalar(checkDiscountQuery));
                hasDiscountColumns = discountColumnsExist >= 3; // All 3 columns must exist
            }
            catch
            {
                hasDiscountColumns = false;
            }

            // Get selected company ID (0 means no company selected)
            int selectedCompanyID = Convert.ToInt32(comboBox1.SelectedValue);
            object companyIDValue = selectedCompanyID == 0 ? (object)DBNull.Value : selectedCompanyID;

            string barcode = "";
            string barcodeColumn = "";
            string barcodeValue = "";
            
            if (hasBarcodeColumn)
            {
                // Generate barcode automatically
                barcode = GenerateItemBarcode(selectedCompanyID, textBox3.Text.Trim());
                
                // Update the barcode textbox with generated barcode
                var txtBarcode = panel1.Controls.OfType<TextBox>().FirstOrDefault(t => t.Name == "txtBarcode");
                if (txtBarcode != null) 
                {
                    if (string.IsNullOrEmpty(txtBarcode.Text))
                    {
                        txtBarcode.Text = barcode;
                    }
                    else
                    {
                        barcode = txtBarcode.Text.Trim(); // Use user-provided barcode
                    }
                }
                
                barcodeColumn = ", Barcode";
                barcodeValue = ", @Barcode";
            }

            string discountColumns = hasDiscountColumns ? ", Disc1, Disc2, SalesTax" : "";
            string discountValues = hasDiscountColumns ? ", @Disc1, @Disc2, @SalesTax" : "";
            
            string query = $@"INSERT INTO Items (ItemName, Description, Price, {stockColumnName}, Category, CompanyID{barcodeColumn}{discountColumns}, Packing, PurchasePrice, ReorderLevel, CreatedDate, IsActive) 
                           VALUES (@ItemName, @Description, @Price, @StockQuantity, @Category, @CompanyID{barcodeValue}{discountValues}, @Packing, @PurchasePrice, @ReorderLevel, @CreatedDate, 1)";

            var parameterList = new List<SqlParameter>
            {
                new SqlParameter("@ItemName", textBox1.Text.Trim()),
                new SqlParameter("@Description", textBox8.Text.Trim()),
                new SqlParameter("@Price", decimal.Parse(textBox4.Text)),
                new SqlParameter("@StockQuantity", string.IsNullOrEmpty(textBox6.Text) ? 0 : int.Parse(textBox6.Text)),
                new SqlParameter("@Category", textBox3.Text.Trim()),
                new SqlParameter("@CompanyID", companyIDValue),
                new SqlParameter("@Packing", textBox2.Text.Trim()),
                new SqlParameter("@PurchasePrice", string.IsNullOrEmpty(textBox5.Text) ? 0 : decimal.Parse(textBox5.Text)),
                new SqlParameter("@ReorderLevel", string.IsNullOrEmpty(textBox7.Text) ? 0 : int.Parse(textBox7.Text)),
                new SqlParameter("@CreatedDate", DateTime.Now)
            };

            if (hasDiscountColumns)
            {
                parameterList.Add(new SqlParameter("@Disc1", string.IsNullOrEmpty(txtDisc1.Text) ? 0 : decimal.Parse(txtDisc1.Text)));
                parameterList.Add(new SqlParameter("@Disc2", string.IsNullOrEmpty(txtDisc2.Text) ? 0 : decimal.Parse(txtDisc2.Text)));
                parameterList.Add(new SqlParameter("@SalesTax", string.IsNullOrEmpty(txtSalesTax.Text) ? 0 : decimal.Parse(txtSalesTax.Text)));
            }

            if (hasBarcodeColumn)
            {
                parameterList.Add(new SqlParameter("@Barcode", barcode));
            }

            DatabaseConnection.ExecuteNonQuery(query, parameterList.ToArray());
            
            string successMessage = hasBarcodeColumn ? 
                $"Item added successfully!\nBarcode: {barcode}" : 
                "Item added successfully!";
            MessageBox.Show(successMessage, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void UpdateItem()
        {
            string stockColumnName = DatabaseConnection.GetStockColumnName();
            
            // Check if Barcode column exists
            bool hasBarcodeColumn = false;
            try
            {
                string checkColumnQuery = "SELECT COUNT(*) FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Items]') AND name = 'Barcode'";
                int columnExists = SafeDataHelper.SafeToInt32(DatabaseConnection.ExecuteScalar(checkColumnQuery));
                hasBarcodeColumn = columnExists > 0;
            }
            catch
            {
                hasBarcodeColumn = false;
            }

            // Check if new discount and tax columns exist
            bool hasDiscountColumns = false;
            try
            {
                string checkDiscountQuery = "SELECT COUNT(*) FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Items]') AND name IN ('Disc1', 'Disc2', 'SalesTax')";
                int discountColumnsExist = SafeDataHelper.SafeToInt32(DatabaseConnection.ExecuteScalar(checkDiscountQuery));
                hasDiscountColumns = discountColumnsExist >= 3; // All 3 columns must exist
            }
            catch
            {
                hasDiscountColumns = false;
            }

            // Get selected company ID (0 means no company selected)
            int selectedCompanyID = Convert.ToInt32(comboBox1.SelectedValue);
            object companyIDValue = selectedCompanyID == 0 ? (object)DBNull.Value : selectedCompanyID;

            string barcode = "";
            string barcodeUpdate = "";
            
            if (hasBarcodeColumn)
            {
                // Get barcode from textbox (keep existing or allow manual entry)
                var txtBarcode = panel1.Controls.OfType<TextBox>().FirstOrDefault(t => t.Name == "txtBarcode");
                if (txtBarcode != null)
                {
                    barcode = txtBarcode.Text.Trim();
                    if (string.IsNullOrEmpty(barcode))
                    {
                        // Generate new barcode if empty
                        barcode = GenerateItemBarcode(selectedCompanyID, textBox3.Text.Trim());
                        txtBarcode.Text = barcode;
                    }
                }
                barcodeUpdate = ", Barcode = @Barcode";
            }

            string discountUpdate = hasDiscountColumns ? ", Disc1 = @Disc1, Disc2 = @Disc2, SalesTax = @SalesTax" : "";
            
            string query = $@"UPDATE Items SET ItemName = @ItemName, Description = @Description, 
                           Price = @Price, {stockColumnName} = @StockQuantity, Category = @Category, CompanyID = @CompanyID{barcodeUpdate}{discountUpdate},
                           Packing = @Packing, PurchasePrice = @PurchasePrice, ReorderLevel = @ReorderLevel
                           WHERE ItemID = @ItemID";

            var parameterList = new List<SqlParameter>
            {
                new SqlParameter("@ItemID", selectedItemID),
                new SqlParameter("@ItemName", textBox1.Text.Trim()),
                new SqlParameter("@Description", textBox8.Text.Trim()),
                new SqlParameter("@Price", decimal.Parse(textBox4.Text)),
                new SqlParameter("@StockQuantity", string.IsNullOrEmpty(textBox6.Text) ? 0 : int.Parse(textBox6.Text)),
                new SqlParameter("@Category", textBox3.Text.Trim()),
                new SqlParameter("@CompanyID", companyIDValue),
                new SqlParameter("@Packing", textBox2.Text.Trim()),
                new SqlParameter("@PurchasePrice", string.IsNullOrEmpty(textBox5.Text) ? 0 : decimal.Parse(textBox5.Text)),
                new SqlParameter("@ReorderLevel", string.IsNullOrEmpty(textBox7.Text) ? 0 : int.Parse(textBox7.Text))
            };

            if (hasDiscountColumns)
            {
                parameterList.Add(new SqlParameter("@Disc1", string.IsNullOrEmpty(txtDisc1.Text) ? 0 : decimal.Parse(txtDisc1.Text)));
                parameterList.Add(new SqlParameter("@Disc2", string.IsNullOrEmpty(txtDisc2.Text) ? 0 : decimal.Parse(txtDisc2.Text)));
                parameterList.Add(new SqlParameter("@SalesTax", string.IsNullOrEmpty(txtSalesTax.Text) ? 0 : decimal.Parse(txtSalesTax.Text)));
            }

            if (hasBarcodeColumn)
            {
                parameterList.Add(new SqlParameter("@Barcode", barcode));
            }

            DatabaseConnection.ExecuteNonQuery(query, parameterList.ToArray());
            MessageBox.Show("Item updated successfully!", "Success", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Generates a unique barcode for pharmacy items
        /// </summary>
        private string GenerateItemBarcode(int companyId, string category)
        {
            try
            {
                // Use BarcodeHelper to generate unique barcode
                return BarcodeHelper.GenerateUniqueBarcode(companyId > 0 ? companyId : (int?)null, category);
            }
            catch (Exception ex)
            {
                // Fallback barcode generation
                long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                return $"PH{companyId:D2}{(timestamp % 1000000):D6}";
            }
        }

        /// <summary>
        /// Auto generate and display barcode when product is selected
        /// </summary>
        private void AutoGenerateAndDisplayBarcode(string barcodeText, int companyID, string category)
        {
            try
            {
                var txtBarcode = panel1.Controls.OfType<TextBox>().FirstOrDefault(t => t.Name == "txtBarcode");
                var picBarcode = panel1.Controls.OfType<PictureBox>().FirstOrDefault(p => p.Name == "picBarcode");

                // If no barcode provided, generate one
                if (string.IsNullOrEmpty(barcodeText))
                {
                    barcodeText = GenerateItemBarcode(companyID, category);
                    if (txtBarcode != null)
                    {
                        txtBarcode.Text = barcodeText;
                        txtBarcode.BackColor = Color.LightCyan; // Show it's auto-generated
                    }
                }
                else
                {
                    if (txtBarcode != null)
                    {
                        txtBarcode.Text = barcodeText;
                        txtBarcode.BackColor = Color.LightYellow; // Normal existing barcode
                    }
                }

                // Generate and display barcode image automatically
                if (picBarcode != null && !string.IsNullOrEmpty(barcodeText))
                {
                    try
                    {
                        System.Drawing.Image barcodeImage = BarcodeHelper.GenerateCode128Barcode(barcodeText, 200, 50, false);
                        picBarcode.Image = barcodeImage;
                    }
                    catch (Exception ex)
                    {
                        // Show simple error in picture box
                        Bitmap errorImage = new Bitmap(200, 50);
                        using (Graphics g = Graphics.FromImage(errorImage))
                        {
                            g.Clear(Color.White);
                            g.DrawString("Barcode Error", new Font("Arial", 8), Brushes.Red, 5, 5);
                            g.DrawString(barcodeText, new Font("Arial", 8), Brushes.Black, 5, 20);
                        }
                        picBarcode.Image = errorImage;
                    }
                }
            }
            catch (Exception ex)
            {
                // Silent error handling - don't show popup during product selection
                var picBarcode = panel1.Controls.OfType<PictureBox>().FirstOrDefault(p => p.Name == "picBarcode");
                if (picBarcode != null)
                {
                    Bitmap errorImage = new Bitmap(200, 50);
                    using (Graphics g = Graphics.FromImage(errorImage))
                    {
                        g.Clear(Color.White);
                        g.DrawString("Error generating barcode", new Font("Arial", 8), Brushes.Red, 5, 15);
                    }
                    picBarcode.Image = errorImage;
                }
            }
        }

        private bool ValidateForm()
        {
            // Validate Item Name
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Please enter item name.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Focus();
                return false;
            }

            // Validate Price
            if (string.IsNullOrWhiteSpace(textBox4.Text) || !decimal.TryParse(textBox4.Text, out decimal price) || price < 0)
            {
                MessageBox.Show("Please enter a valid price (must be a positive number).", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox4.Focus();
                return false;
            }

            // Validate Stock Quantity (if provided)
            if (!string.IsNullOrWhiteSpace(textBox6.Text))
            {
                if (!int.TryParse(textBox6.Text, out int stock) || stock < 0)
                {
                    MessageBox.Show("Please enter a valid stock quantity (must be a positive number).", "Validation Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox6.Focus();
                    return false;
                }
            }

            // Check for duplicate item name (case insensitive)
            string checkQuery = "SELECT COUNT(*) FROM Items WHERE LOWER(ItemName) = LOWER(@ItemName) AND IsActive = 1";
            if (isEditMode)
            {
                checkQuery += " AND ItemID != @ItemID";
            }

            SqlParameter[] checkParams = isEditMode ? 
                new SqlParameter[] { 
                    new SqlParameter("@ItemName", textBox1.Text.Trim()),
                    new SqlParameter("@ItemID", selectedItemID)
                } :
                new SqlParameter[] { 
                    new SqlParameter("@ItemName", textBox1.Text.Trim())
                };

            int count = Convert.ToInt32(DatabaseConnection.ExecuteScalar(checkQuery, checkParams));
            if (count > 0)
            {
                MessageBox.Show("An item with this name already exists.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Focus();
                return false;
            }

            return true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearForm();
            
            // Disable buttons after cancel
            btnSave.Enabled = false;
            btnSaveNew.Enabled = false;
            btnCancel.Enabled = false;
            
            // Disable delete button
            var btnDelete = panel2.Controls.OfType<Button>().FirstOrDefault(b => b.Name == "btnDelete");
            if (btnDelete != null) btnDelete.Enabled = false;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedItemID > 0)
            {
                if (MessageBox.Show("Are you sure you want to delete this item?", "Confirm Delete", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        // Check if item is used in any sales
                        string checkQuery = "SELECT COUNT(*) FROM SaleItems WHERE ItemID = @ItemID";
                        SqlParameter[] checkParams = { new SqlParameter("@ItemID", selectedItemID) };
                        int saleCount = Convert.ToInt32(DatabaseConnection.ExecuteScalar(checkQuery, checkParams));

                        if (saleCount > 0)
                        {
                            MessageBox.Show("Cannot delete this item as it is used in sales transactions.", 
                                "Delete Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        string query = "UPDATE Items SET IsActive = 0 WHERE ItemID = @ItemID";
                        SqlParameter[] parameters = { new SqlParameter("@ItemID", selectedItemID) };
                        DatabaseConnection.ExecuteNonQuery(query, parameters);
                        
                        LoadItems();
                        ClearForm();
                        
                        // Disable buttons after delete
                        btnSave.Enabled = false;
                        btnSaveNew.Enabled = false;
                        btnCancel.Enabled = false;
                        
                        // Disable delete button
                        var btnDelete = panel2.Controls.OfType<Button>().FirstOrDefault(b => b.Name == "btnDelete");
                        if (btnDelete != null) btnDelete.Enabled = false;
                        
                        MessageBox.Show("Item deleted successfully!", "Success", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error deleting item: " + ex.Message, "Error", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select an item to delete.", "Information", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (itemsData != null && itemsData.Rows.Count > 0)
                {
                    // Create a simple print functionality
                    PrintDialog printDialog = new PrintDialog();
                    if (printDialog.ShowDialog() == DialogResult.OK)
                    {
                        MessageBox.Show("Print functionality would be implemented here.", "Print", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("No data to print.", "Information", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error printing: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void gridViewProducts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = gridViewProducts.Rows[e.RowIndex];
                
                // Safely handle conversions using centralized SafeDataHelper
                selectedItemID = SafeDataHelper.SafeGetCellInt32(row, "ItemID");
                textBox1.Text = SafeDataHelper.SafeGetCellString(row, "ItemName");
                textBox2.Text = SafeDataHelper.SafeGetCellString(row, "Packing");
                textBox3.Text = SafeDataHelper.SafeGetCellString(row, "Category");
                textBox4.Text = SafeDataHelper.SafeGetCellDecimal(row, "Price").ToString("F2");
                textBox5.Text = SafeDataHelper.SafeGetCellDecimal(row, "PurchasePrice").ToString("F2");
                textBox6.Text = SafeDataHelper.SafeGetCellString(row, "StockQuantity");
                textBox7.Text = SafeDataHelper.SafeGetCellString(row, "ReorderLevel");
                textBox8.Text = SafeDataHelper.SafeGetCellString(row, "Description");
                txtDisc1.Text = SafeDataHelper.SafeGetCellDecimal(row, "Disc1").ToString("F2");
                txtDisc2.Text = SafeDataHelper.SafeGetCellDecimal(row, "Disc2").ToString("F2");
                txtSalesTax.Text = SafeDataHelper.SafeGetCellDecimal(row, "SalesTax").ToString("F2");
                
                // Auto-generate and display barcode automatically
                string existingBarcode = SafeDataHelper.SafeGetCellString(row, "Barcode");
                int companyID = SafeDataHelper.SafeGetCellInt32(row, "CompanyID");
                string category = SafeDataHelper.SafeGetCellString(row, "Category");
                
                // This will handle both existing barcodes and auto-generation for missing ones
                AutoGenerateAndDisplayBarcode(existingBarcode, companyID, category);
                
                // Set the company in comboBox1 if CompanyID is available
                int selectedCompanyID = SafeDataHelper.SafeGetCellInt32(row, "CompanyID");
                if (selectedCompanyID > 0)
                {
                    comboBox1.SelectedValue = selectedCompanyID;
                }
                else
                {
                    comboBox1.SelectedIndex = 0; // Select "All Companies"
                }
                
                isEditMode = true;
                btnSave.Text = "Update";
                btnSave.Enabled = true;
                btnCancel.Enabled = true;
                
                // Enable delete button
                Button btnDelete = panel2.Controls.OfType<Button>().FirstOrDefault(b => b.Text == "Delete");
                if (btnDelete != null)
                {
                    btnDelete.Enabled = true;
                }
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            CleanupResources();
            this.Close();
        }

        internal void CleanupResources()
        {
            if (!isDisposing)
            {
                isDisposing = true;
                
                try
                {
                    // Remove FormClosing event to prevent recursive calls
                    this.FormClosing -= Items_FormClosing;
                    
                    // Remove event handlers to prevent garbage collection issues
                    if (comboBox1 != null)
                        comboBox1.SelectedIndexChanged -= ComboBox1_SelectedIndexChanged;
                    
                    if (txtSearch != null)
                    {
                        txtSearch.TextChanged -= txtSearch_TextChanged;
                        txtSearch.GotFocus -= TxtSearch_GotFocus;
                        txtSearch.LostFocus -= TxtSearch_LostFocus;
                    }
                    
                    // Find and remove dynamically created control event handlers
                    ComboBox comboBoxCategory = this.Controls.OfType<ComboBox>().FirstOrDefault(c => c.Name == "comboBoxCategory");
                    if (comboBoxCategory != null)
                        comboBoxCategory.SelectedIndexChanged -= comboBoxCategory_SelectedIndexChanged;
                    
                    Button btnDelete = panel2?.Controls.OfType<Button>().FirstOrDefault(b => b.Name == "btnDelete");
                    if (btnDelete != null)
                        btnDelete.Click -= btnDelete_Click;
                    
                    // Remove barcode event handlers
                    TextBox txtBarcode = panel1?.Controls.OfType<TextBox>().FirstOrDefault(t => t.Name == "txtBarcode");
                    if (txtBarcode != null)
                    {
                        txtBarcode.TextChanged -= TxtBarcode_TextChanged;
                        txtBarcode.KeyDown -= TxtBarcode_KeyDown;
                        txtBarcode.GotFocus -= TxtBarcode_GotFocus;
                        txtBarcode.LostFocus -= TxtBarcode_LostFocus;
                    }
                    
                    Button btnGenerateBarcode = panel1?.Controls.OfType<Button>().FirstOrDefault(b => b.Name == "btnGenerateBarcode");
                    if (btnGenerateBarcode != null)
                        btnGenerateBarcode.Click -= BtnGenerateBarcode_Click;
                    
                    // Dispose data table
                    if (itemsData != null)
                    {
                        itemsData.Dispose();
                        itemsData = null;
                    }
                }
                catch
                {
                    // Ignore disposal errors to prevent crashes during cleanup
                }
            }
        }

        private void comboBoxCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isDisposing && !this.IsDisposed)
            {
                FilterItems();
            }
        }

        private void FilterItems()
        {
            try
            {
                ComboBox comboBoxCategory = this.Controls.OfType<ComboBox>().FirstOrDefault(c => c.Location.X == 710);
                if (comboBoxCategory != null && comboBoxCategory.SelectedIndex > 0)
                {
                    DataRowView selectedRow = comboBoxCategory.SelectedItem as DataRowView;
                    if (selectedRow != null)
                    {
                        string category = selectedRow["Category"].ToString();
                        DataView dv = itemsData.DefaultView;
                        dv.RowFilter = $"Category = '{category.Replace("'", "''")}'";
                        gridViewProducts.DataSource = dv;
                    }
                }
                else
                {
                    gridViewProducts.DataSource = itemsData;
                }
                
                UpdateStatusLabel();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error filtering items: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (!isDisposing && !this.IsDisposed)
            {
                SearchItems();
            }
        }

        private void SearchItems()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(txtSearch.Text) && txtSearch.Text != "Search items...")
                {
                    DataView dv = itemsData.DefaultView;
                    dv.RowFilter = $"ItemName LIKE '%{txtSearch.Text.Replace("'", "''")}%' OR Description LIKE '%{txtSearch.Text.Replace("'", "''")}%'";
                    gridViewProducts.DataSource = dv;
                }
                else
                {
                    gridViewProducts.DataSource = itemsData;
                }
                
                UpdateStatusLabel();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching items: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        // Enhanced Medicine Management Event Handlers
        private void BtnManageBatches_Click(object sender, EventArgs e)
        {
            if (selectedItemID == 0)
            {
                MessageBox.Show("Please select an item first.", "Information", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            // Open Batch Management Form
            BatchManagementForm batchForm = new BatchManagementForm(selectedItemID, textBox1.Text);
            batchForm.ShowDialog();
            
            // Refresh items after batch management
            LoadItems();
        }
        
        private void BtnManageSubstitutes_Click(object sender, EventArgs e)
        {
            if (selectedItemID == 0)
            {
                MessageBox.Show("Please select an item first.", "Information", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            // Open Substitute Management Form
            SubstituteManagementForm substituteForm = new SubstituteManagementForm(selectedItemID, textBox1.Text);
            substituteForm.ShowDialog();
        }
        
        private void BtnLowStockAlert_Click(object sender, EventArgs e)
        {
            try
            {
                // Execute low stock alert procedure
                string query = "EXEC sp_GetLowStockAlert";
                DataTable lowStockData = DatabaseConnection.ExecuteQuery(query);
                
                if (lowStockData.Rows.Count > 0)
                {
                    LowStockReportForm reportForm = new LowStockReportForm(lowStockData);
                    reportForm.Show();
                }
                else
                {
                    MessageBox.Show("No items are currently running low on stock.", "Information", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating low stock report: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void BtnExpiryReport_Click(object sender, EventArgs e)
        {
            try
            {
                // Execute expiry report procedure
                string query = "EXEC sp_GetExpiryReport @DaysAhead = 90, @IncludeExpired = 1";
                DataTable expiryData = DatabaseConnection.ExecuteQuery(query);
                
                if (expiryData.Rows.Count > 0)
                {
                    ExpiryReportForm reportForm = new ExpiryReportForm(expiryData);
                    reportForm.Show();
                }
                else
                {
                    MessageBox.Show("No items are expiring in the next 90 days.", "Information", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating expiry report: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void LogUserActivity(string action, string tableName, string description)
        {
            try
            {
                string query = @"INSERT INTO UserActivityLog (UserID, Activity, ModuleName, Description, LogDate) 
                               VALUES (@UserID, @Activity, @ModuleName, @Description, @LogDate)";
                
                SqlParameter[] parameters = {
                    new SqlParameter("@UserID", currentUserID),
                    new SqlParameter("@Activity", action),
                    new SqlParameter("@ModuleName", "Items"),
                    new SqlParameter("@Description", description),
                    new SqlParameter("@LogDate", DateTime.Now)
                };
                
                DatabaseConnection.ExecuteNonQuery(query, parameters);
            }
            catch
            {
                // Log errors silently - don't interrupt the main operation
            }
        }

        // Enhanced Barcode Functionality Event Handlers
        private void TxtBarcode_GotFocus(object sender, EventArgs e)
        {
            TextBox txtBarcode = sender as TextBox;
            if (txtBarcode != null && txtBarcode.Text == "Enter or auto-generate")
            {
                txtBarcode.Text = "";
                txtBarcode.ForeColor = Color.Black;
            }
        }

        private void TxtBarcode_LostFocus(object sender, EventArgs e)
        {
            TextBox txtBarcode = sender as TextBox;
            if (txtBarcode != null && string.IsNullOrWhiteSpace(txtBarcode.Text))
            {
                txtBarcode.Text = "Enter or auto-generate";
                txtBarcode.ForeColor = Color.Gray;
            }
        }

        private void TxtBarcode_TextChanged(object sender, EventArgs e)
        {
            if (!isDisposing && !this.IsDisposed)
            {
                TextBox txtBarcode = sender as TextBox;
                if (txtBarcode != null && !string.IsNullOrWhiteSpace(txtBarcode.Text) && txtBarcode.Text != "Enter or auto-generate")
                {
                    // Validate barcode format and generate image
                    if (BarcodeHelper.IsValidCode128Text(txtBarcode.Text))
                    {
                        txtBarcode.BackColor = Color.LightGreen; // Valid barcode
                        GenerateBarcodeImageFromText(txtBarcode.Text);
                    }
                    else
                    {
                        txtBarcode.BackColor = Color.LightCoral; // Invalid barcode
                        ClearBarcodeImage();
                    }
                }
                else
                {
                    txtBarcode.BackColor = Color.LightYellow; // Default/empty
                    ClearBarcodeImage();
                }
            }
        }

        private void TxtBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                TextBox txtBarcode = sender as TextBox;
                if (txtBarcode != null && !string.IsNullOrWhiteSpace(txtBarcode.Text))
                {
                    // Generate barcode image on Enter key
                    GenerateBarcodeImageFromText(txtBarcode.Text);
                    MessageBox.Show("Barcode generated successfully!", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void BtnGenerateBarcode_Click(object sender, EventArgs e)
        {
            try
            {
                var txtBarcode = panel1.Controls.OfType<TextBox>().FirstOrDefault(t => t.Name == "txtBarcode");
                if (txtBarcode != null)
                {
                    if (string.IsNullOrWhiteSpace(txtBarcode.Text))
                    {
                        // Auto-generate a new barcode
                        int selectedCompanyID = 0;
                        if (comboBox1.SelectedValue != null)
                        {
                            selectedCompanyID = Convert.ToInt32(comboBox1.SelectedValue);
                        }
                        
                        string newBarcode = GenerateItemBarcode(selectedCompanyID, textBox3.Text.Trim());
                        txtBarcode.Text = newBarcode;
                        txtBarcode.BackColor = Color.LightCyan; // Show it's auto-generated
                    }
                    
                    // Generate and display barcode image
                    GenerateBarcodeImageFromText(txtBarcode.Text);
                    MessageBox.Show($"Barcode generated: {txtBarcode.Text}", "Barcode Generated", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating barcode: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerateBarcodeImageFromText(string barcodeText)
        {
            try
            {
                var picBarcode = panel1.Controls.OfType<PictureBox>().FirstOrDefault(p => p.Name == "picBarcode");
                if (picBarcode != null && !string.IsNullOrWhiteSpace(barcodeText))
                {
                    // Dispose previous image to prevent memory leaks
                    if (picBarcode.Image != null)
                    {
                        picBarcode.Image.Dispose();
                        picBarcode.Image = null;
                    }
                    
                    // Generate new barcode image
                    System.Drawing.Image barcodeImage = BarcodeHelper.GenerateCode128Barcode(barcodeText, 200, 50, true);
                    picBarcode.Image = barcodeImage;
                }
            }
            catch (Exception ex)
            {
                // Show error in picture box instead of popup
                var picBarcode = panel1.Controls.OfType<PictureBox>().FirstOrDefault(p => p.Name == "picBarcode");
                if (picBarcode != null)
                {
                    Bitmap errorImage = new Bitmap(200, 50);
                    using (Graphics g = Graphics.FromImage(errorImage))
                    {
                        g.Clear(Color.White);
                        g.DrawString("Invalid Barcode", new Font("Arial", 8), Brushes.Red, 5, 5);
                        g.DrawString(barcodeText, new Font("Arial", 8), Brushes.Black, 5, 20);
                    }
                    picBarcode.Image = errorImage;
                }
            }
        }

        private void ClearBarcodeImage()
        {
            var picBarcode = panel1.Controls.OfType<PictureBox>().FirstOrDefault(p => p.Name == "picBarcode");
            if (picBarcode != null)
            {
                if (picBarcode.Image != null)
                {
                    picBarcode.Image.Dispose();
                    picBarcode.Image = null;
                }
            }
        }

        private void Button14_Click(object sender, EventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                if (btn != null)
                {
                    // Toggle between Pack (P) and Loose (L) mode
                    if (btn.Text == "P")
                    {
                        btn.Text = "L";
                        btn.BackColor = Color.Orange;
                    }
                    else
                    {
                        btn.Text = "P";
                        btn.BackColor = Color.LightGreen;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error toggling pack/loose mode: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
