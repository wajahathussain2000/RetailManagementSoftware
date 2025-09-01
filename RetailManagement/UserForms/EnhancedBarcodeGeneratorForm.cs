using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RetailManagement.Database;
using RetailManagement.Models;

namespace RetailManagement.UserForms
{
    public partial class EnhancedBarcodeGeneratorForm : Form
    {
        private ComboBox cmbBarcodeType;
        private ComboBox cmbItems;
        private ComboBox cmbCategory;
        private ComboBox cmbSupplier;
        private TextBox txtCustomBarcode;
        private TextBox txtItemSearch;
        private CheckBox chkIncludePrice;
        private CheckBox chkIncludeBatch;
        private CheckBox chkIncludeExpiry;
        private CheckBox chkAutoGenerate;
        private Button btnGenerate;
        private Button btnGenerateBulk;
        private Button btnSave;
        private Button btnPrint;
        private Button btnExport;
        private DataGridView dgvItems;
        private Panel pnlBarcodePreview;
        private PictureBox picBarcode;
        private Label lblBarcodeText;
        private GroupBox groupFilters;
        private GroupBox groupOptions;
        private GroupBox groupPreview;
        private Panel summaryPanel;
        private Label lblTotalItems;
        private Label lblGeneratedBarcodes;
        private Label lblPendingItems;
        private NumericUpDown nudWidth;
        private NumericUpDown nudHeight;
        private NumericUpDown nudCopies;
        private ComboBox cmbFormat;
        private DataTable itemsData;
        private Dictionary<int, Bitmap> generatedBarcodes;

        public EnhancedBarcodeGeneratorForm()
        {
            InitializeComponent();
            InitializeBarcodeGenerator();
            LoadInitialData();
        }

        private void InitializeComponent()
        {
            this.dgvItems = new System.Windows.Forms.DataGridView();
            this.groupFilters = new System.Windows.Forms.GroupBox();
            this.groupOptions = new System.Windows.Forms.GroupBox();
            this.groupPreview = new System.Windows.Forms.GroupBox();
            this.cmbBarcodeType = new System.Windows.Forms.ComboBox();
            this.cmbItems = new System.Windows.Forms.ComboBox();
            this.cmbCategory = new System.Windows.Forms.ComboBox();
            this.cmbSupplier = new System.Windows.Forms.ComboBox();
            this.txtCustomBarcode = new System.Windows.Forms.TextBox();
            this.txtItemSearch = new System.Windows.Forms.TextBox();
            this.chkIncludePrice = new System.Windows.Forms.CheckBox();
            this.chkIncludeBatch = new System.Windows.Forms.CheckBox();
            this.chkIncludeExpiry = new System.Windows.Forms.CheckBox();
            this.chkAutoGenerate = new System.Windows.Forms.CheckBox();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.btnGenerateBulk = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.pnlBarcodePreview = new System.Windows.Forms.Panel();
            this.picBarcode = new System.Windows.Forms.PictureBox();
            this.lblBarcodeText = new System.Windows.Forms.Label();
            this.summaryPanel = new System.Windows.Forms.Panel();
            this.lblTotalItems = new System.Windows.Forms.Label();
            this.lblGeneratedBarcodes = new System.Windows.Forms.Label();
            this.lblPendingItems = new System.Windows.Forms.Label();
            this.nudWidth = new System.Windows.Forms.NumericUpDown();
            this.nudHeight = new System.Windows.Forms.NumericUpDown();
            this.nudCopies = new System.Windows.Forms.NumericUpDown();
            this.cmbFormat = new System.Windows.Forms.ComboBox();

            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBarcode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCopies)).BeginInit();
            this.SuspendLayout();

            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.Controls.Add(this.summaryPanel);
            this.Controls.Add(this.dgvItems);
            this.Controls.Add(this.groupPreview);
            this.Controls.Add(this.groupOptions);
            this.Controls.Add(this.groupFilters);
            this.Name = "EnhancedBarcodeGeneratorForm";
            this.Text = "Enhanced Barcode Generator for Medicines";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;

            // Group Filters
            this.groupFilters.Controls.Add(new Label { Text = "Search Item:", Location = new Point(15, 25), Size = new Size(70, 13) });
            this.groupFilters.Controls.Add(this.txtItemSearch);
            this.groupFilters.Controls.Add(new Label { Text = "Select Item:", Location = new Point(300, 25), Size = new Size(70, 13) });
            this.groupFilters.Controls.Add(this.cmbItems);
            this.groupFilters.Controls.Add(new Label { Text = "Category:", Location = new Point(15, 55), Size = new Size(60, 13) });
            this.groupFilters.Controls.Add(this.cmbCategory);
            this.groupFilters.Controls.Add(new Label { Text = "Supplier:", Location = new Point(300, 55), Size = new Size(55, 13) });
            this.groupFilters.Controls.Add(this.cmbSupplier);
            this.groupFilters.Location = new System.Drawing.Point(12, 12);
            this.groupFilters.Name = "groupFilters";
            this.groupFilters.Size = new System.Drawing.Size(580, 90);
            this.groupFilters.TabIndex = 0;
            this.groupFilters.TabStop = false;
            this.groupFilters.Text = "Item Selection & Search";

            // Group Options
            this.groupOptions.Controls.Add(new Label { Text = "Barcode Type:", Location = new Point(15, 25), Size = new Size(85, 13) });
            this.groupOptions.Controls.Add(this.cmbBarcodeType);
            this.groupOptions.Controls.Add(new Label { Text = "Custom Code:", Location = new Point(15, 55), Size = new Size(80, 13) });
            this.groupOptions.Controls.Add(this.txtCustomBarcode);
            this.groupOptions.Controls.Add(new Label { Text = "Width:", Location = new Point(15, 85), Size = new Size(40, 13) });
            this.groupOptions.Controls.Add(this.nudWidth);
            this.groupOptions.Controls.Add(new Label { Text = "Height:", Location = new Point(120, 85), Size = new Size(45, 13) });
            this.groupOptions.Controls.Add(this.nudHeight);
            this.groupOptions.Controls.Add(new Label { Text = "Format:", Location = new Point(15, 115), Size = new Size(45, 13) });
            this.groupOptions.Controls.Add(this.cmbFormat);
            this.groupOptions.Controls.Add(new Label { Text = "Copies:", Location = new Point(120, 115), Size = new Size(45, 13) });
            this.groupOptions.Controls.Add(this.nudCopies);
            this.groupOptions.Controls.Add(this.chkIncludePrice);
            this.groupOptions.Controls.Add(this.chkIncludeBatch);
            this.groupOptions.Controls.Add(this.chkIncludeExpiry);
            this.groupOptions.Controls.Add(this.chkAutoGenerate);
            this.groupOptions.Controls.Add(this.btnGenerate);
            this.groupOptions.Controls.Add(this.btnGenerateBulk);
            this.groupOptions.Controls.Add(this.btnSave);
            this.groupOptions.Controls.Add(this.btnPrint);
            this.groupOptions.Controls.Add(this.btnExport);
            this.groupOptions.Location = new System.Drawing.Point(600, 12);
            this.groupOptions.Name = "groupOptions";
            this.groupOptions.Size = new System.Drawing.Size(580, 180);
            this.groupOptions.TabIndex = 1;
            this.groupOptions.TabStop = false;
            this.groupOptions.Text = "Barcode Generation Options";

            // Group Preview
            this.groupPreview.Controls.Add(this.pnlBarcodePreview);
            this.groupPreview.Location = new System.Drawing.Point(12, 110);
            this.groupPreview.Name = "groupPreview";
            this.groupPreview.Size = new System.Drawing.Size(580, 180);
            this.groupPreview.TabIndex = 2;
            this.groupPreview.TabStop = false;
            this.groupPreview.Text = "Barcode Preview";

            // Barcode Preview Panel
            this.pnlBarcodePreview.Controls.Add(this.picBarcode);
            this.pnlBarcodePreview.Controls.Add(this.lblBarcodeText);
            this.pnlBarcodePreview.Location = new System.Drawing.Point(15, 25);
            this.pnlBarcodePreview.Size = new System.Drawing.Size(550, 140);
            this.pnlBarcodePreview.BorderStyle = BorderStyle.FixedSingle;

            // Summary Panel
            this.summaryPanel.BackColor = System.Drawing.Color.LightCyan;
            this.summaryPanel.Controls.Add(this.lblTotalItems);
            this.summaryPanel.Controls.Add(this.lblGeneratedBarcodes);
            this.summaryPanel.Controls.Add(this.lblPendingItems);
            this.summaryPanel.Location = new System.Drawing.Point(12, 300);
            this.summaryPanel.Name = "summaryPanel";
            this.summaryPanel.Size = new System.Drawing.Size(1168, 50);
            this.summaryPanel.TabIndex = 3;

            // Setup all controls
            SetupControls();
            SetupDataGridView();

            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBarcode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCopies)).EndInit();
            this.ResumeLayout(false);
        }

        private void SetupControls()
        {
            // Search TextBox
            this.txtItemSearch.Location = new System.Drawing.Point(90, 22);
            this.txtItemSearch.Name = "txtItemSearch";
            this.txtItemSearch.Size = new System.Drawing.Size(180, 20);
            this.txtItemSearch.TabIndex = 1;
            this.txtItemSearch.TextChanged += TxtItemSearch_TextChanged;

            // Items ComboBox
            this.cmbItems.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbItems.Location = new System.Drawing.Point(375, 22);
            this.cmbItems.Name = "cmbItems";
            this.cmbItems.Size = new System.Drawing.Size(180, 21);
            this.cmbItems.TabIndex = 2;
            this.cmbItems.SelectedIndexChanged += CmbItems_SelectedIndexChanged;

            // Category ComboBox
            this.cmbCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCategory.Location = new System.Drawing.Point(80, 52);
            this.cmbCategory.Name = "cmbCategory";
            this.cmbCategory.Size = new System.Drawing.Size(150, 21);
            this.cmbCategory.TabIndex = 3;
            this.cmbCategory.SelectedIndexChanged += CmbCategory_SelectedIndexChanged;

            // Supplier ComboBox
            this.cmbSupplier.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSupplier.Location = new System.Drawing.Point(360, 52);
            this.cmbSupplier.Name = "cmbSupplier";
            this.cmbSupplier.Size = new System.Drawing.Size(180, 21);
            this.cmbSupplier.TabIndex = 4;
            this.cmbSupplier.SelectedIndexChanged += CmbSupplier_SelectedIndexChanged;

            // Barcode Type ComboBox
            this.cmbBarcodeType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBarcodeType.Location = new System.Drawing.Point(105, 22);
            this.cmbBarcodeType.Name = "cmbBarcodeType";
            this.cmbBarcodeType.Size = new System.Drawing.Size(120, 21);
            this.cmbBarcodeType.TabIndex = 5;

            // Custom Barcode TextBox
            this.txtCustomBarcode.Location = new System.Drawing.Point(105, 52);
            this.txtCustomBarcode.Name = "txtCustomBarcode";
            this.txtCustomBarcode.Size = new System.Drawing.Size(150, 20);
            this.txtCustomBarcode.TabIndex = 6;

            // Numeric Controls
            this.nudWidth.Location = new System.Drawing.Point(60, 82);
            this.nudWidth.Minimum = 50;
            this.nudWidth.Maximum = 500;
            this.nudWidth.Value = 200;
            this.nudWidth.Size = new System.Drawing.Size(50, 20);

            this.nudHeight.Location = new System.Drawing.Point(170, 82);
            this.nudHeight.Minimum = 30;
            this.nudHeight.Maximum = 200;
            this.nudHeight.Value = 80;
            this.nudHeight.Size = new System.Drawing.Size(50, 20);

            this.nudCopies.Location = new System.Drawing.Point(170, 112);
            this.nudCopies.Minimum = 1;
            this.nudCopies.Maximum = 100;
            this.nudCopies.Value = 1;
            this.nudCopies.Size = new System.Drawing.Size(50, 20);

            // Format ComboBox
            this.cmbFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFormat.Location = new System.Drawing.Point(65, 112);
            this.cmbFormat.Size = new System.Drawing.Size(50, 21);
            this.cmbFormat.Items.AddRange(new object[] { "PNG", "JPG", "BMP", "GIF" });
            this.cmbFormat.SelectedIndex = 0;

            // Checkboxes
            this.chkIncludePrice.Location = new System.Drawing.Point(280, 25);
            this.chkIncludePrice.Size = new System.Drawing.Size(100, 17);
            this.chkIncludePrice.Text = "Include Price";
            this.chkIncludePrice.Checked = true;

            this.chkIncludeBatch.Location = new System.Drawing.Point(280, 45);
            this.chkIncludeBatch.Size = new System.Drawing.Size(100, 17);
            this.chkIncludeBatch.Text = "Include Batch";
            this.chkIncludeBatch.Checked = true;

            this.chkIncludeExpiry.Location = new System.Drawing.Point(280, 65);
            this.chkIncludeExpiry.Size = new System.Drawing.Size(100, 17);
            this.chkIncludeExpiry.Text = "Include Expiry";

            this.chkAutoGenerate.Location = new System.Drawing.Point(280, 85);
            this.chkAutoGenerate.Size = new System.Drawing.Size(120, 17);
            this.chkAutoGenerate.Text = "Auto Generate";
            this.chkAutoGenerate.Checked = true;

            // Buttons
            this.btnGenerate.BackColor = System.Drawing.Color.Green;
            this.btnGenerate.ForeColor = System.Drawing.Color.White;
            this.btnGenerate.Location = new System.Drawing.Point(420, 20);
            this.btnGenerate.Size = new System.Drawing.Size(100, 30);
            this.btnGenerate.Text = "Generate";
            this.btnGenerate.Click += BtnGenerate_Click;

            this.btnGenerateBulk.BackColor = System.Drawing.Color.Blue;
            this.btnGenerateBulk.ForeColor = System.Drawing.Color.White;
            this.btnGenerateBulk.Location = new System.Drawing.Point(420, 55);
            this.btnGenerateBulk.Size = new System.Drawing.Size(100, 30);
            this.btnGenerateBulk.Text = "Bulk Generate";
            this.btnGenerateBulk.Click += BtnGenerateBulk_Click;

            this.btnSave.BackColor = System.Drawing.Color.Orange;
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(420, 90);
            this.btnSave.Size = new System.Drawing.Size(100, 30);
            this.btnSave.Text = "Save Barcode";
            this.btnSave.Click += BtnSave_Click;

            this.btnPrint.BackColor = System.Drawing.Color.Purple;
            this.btnPrint.ForeColor = System.Drawing.Color.White;
            this.btnPrint.Location = new System.Drawing.Point(420, 125);
            this.btnPrint.Size = new System.Drawing.Size(100, 30);
            this.btnPrint.Text = "Print Labels";
            this.btnPrint.Click += BtnPrint_Click;

            this.btnExport.BackColor = System.Drawing.Color.Teal;
            this.btnExport.ForeColor = System.Drawing.Color.White;
            this.btnExport.Location = new System.Drawing.Point(280, 125);
            this.btnExport.Size = new System.Drawing.Size(100, 30);
            this.btnExport.Text = "Export";
            this.btnExport.Click += BtnExport_Click;

            // Picture Box
            this.picBarcode.Location = new System.Drawing.Point(10, 10);
            this.picBarcode.Size = new System.Drawing.Size(300, 100);
            this.picBarcode.SizeMode = PictureBoxSizeMode.CenterImage;
            this.picBarcode.BorderStyle = BorderStyle.FixedSingle;

            // Barcode Text Label
            this.lblBarcodeText.Location = new System.Drawing.Point(10, 120);
            this.lblBarcodeText.Size = new System.Drawing.Size(300, 15);
            this.lblBarcodeText.TextAlign = ContentAlignment.MiddleCenter;
            this.lblBarcodeText.Font = new Font("Arial", 9F, FontStyle.Bold);

            // Summary Labels
            this.lblTotalItems.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblTotalItems.Location = new System.Drawing.Point(15, 10);
            this.lblTotalItems.Size = new System.Drawing.Size(150, 15);
            this.lblTotalItems.Text = "Total Items: 0";

            this.lblGeneratedBarcodes.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblGeneratedBarcodes.Location = new System.Drawing.Point(15, 30);
            this.lblGeneratedBarcodes.Size = new System.Drawing.Size(180, 15);
            this.lblGeneratedBarcodes.Text = "Generated Barcodes: 0";

            this.lblPendingItems.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblPendingItems.Location = new System.Drawing.Point(250, 10);
            this.lblPendingItems.Size = new System.Drawing.Size(150, 15);
            this.lblPendingItems.Text = "Pending Items: 0";
        }

        private void SetupDataGridView()
        {
            this.dgvItems.Location = new System.Drawing.Point(12, 360);
            this.dgvItems.Name = "dgvItems";
            this.dgvItems.Size = new System.Drawing.Size(1168, 290);
            this.dgvItems.TabIndex = 4;
            this.dgvItems.AllowUserToAddRows = false;
            this.dgvItems.AllowUserToDeleteRows = false;
            this.dgvItems.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvItems.MultiSelect = true;
            this.dgvItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvItems.SelectionChanged += DgvItems_SelectionChanged;

            // Add checkbox column for bulk selection
            DataGridViewCheckBoxColumn chkColumn = new DataGridViewCheckBoxColumn();
            chkColumn.Name = "Select";
            chkColumn.HeaderText = "Select";
            chkColumn.Width = 60;
            chkColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            this.dgvItems.Columns.Add(chkColumn);

            // Add barcode preview column
            DataGridViewImageColumn imgColumn = new DataGridViewImageColumn();
            imgColumn.Name = "BarcodePreview";
            imgColumn.HeaderText = "Barcode";
            imgColumn.Width = 120;
            imgColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            imgColumn.ImageLayout = DataGridViewImageCellLayout.Zoom;
            this.dgvItems.Columns.Add(imgColumn);
        }

        private void InitializeBarcodeGenerator()
        {
            generatedBarcodes = new Dictionary<int, Bitmap>();
            
            // Load barcode types
            cmbBarcodeType.Items.AddRange(new object[] { 
                "Code 128", "Code 39", "EAN-13", "EAN-8", "UPC-A", "UPC-E", "Code 93", "Codabar" 
            });
            cmbBarcodeType.SelectedIndex = 0; // Default to Code 128
        }

        private void LoadInitialData()
        {
            try
            {
                LoadCategories();
                LoadSuppliers();
                LoadItems();
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

        private void LoadItems()
        {
            try
            {
                string searchTerm = txtItemSearch.Text.Trim();
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
                        i.MRP,
                        i.StockInHand,
                        c.CompanyName as SupplierName,
                        CASE 
                            WHEN i.Barcode IS NOT NULL AND i.Barcode != '' THEN 'Generated'
                            ELSE 'Pending'
                        END as BarcodeStatus
                    FROM Items i
                    LEFT JOIN Companies c ON i.CompanyID = c.CompanyID
                    WHERE i.IsActive = 1" +
                    (!string.IsNullOrEmpty(searchTerm) ? " AND i.ItemName LIKE @SearchTerm" : "") +
                    (!string.IsNullOrEmpty(category) ? " AND i.Category = @Category" : "") +
                    (supplierID > 0 ? " AND i.CompanyID = @SupplierID" : "") + @"
                    ORDER BY i.ItemName";

                List<SqlParameter> parameters = new List<SqlParameter>();

                if (!string.IsNullOrEmpty(searchTerm))
                    parameters.Add(new SqlParameter("@SearchTerm", "%" + searchTerm + "%"));
                if (!string.IsNullOrEmpty(category))
                    parameters.Add(new SqlParameter("@Category", category));
                if (supplierID > 0)
                    parameters.Add(new SqlParameter("@SupplierID", supplierID));

                itemsData = DatabaseConnection.ExecuteQuery(query, parameters.ToArray());

                if (itemsData != null)
                {
                    // Load items into ComboBox
                    cmbItems.Items.Clear();
                    cmbItems.Items.Add(new ComboBoxItem { Text = "Select Item", Value = 0 });
                    
                    foreach (DataRow row in itemsData.Rows)
                    {
                        cmbItems.Items.Add(new ComboBoxItem 
                        { 
                            Text = row["ItemName"].ToString(), 
                            Value = Convert.ToInt32(row["ItemID"]) 
                        });
                    }
                    
                    cmbItems.DisplayMember = "Text";
                    cmbItems.ValueMember = "Value";
                    cmbItems.SelectedIndex = 0;

                    // Populate DataGridView
                    PopulateItemsGrid();
                    UpdateSummary();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading items: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PopulateItemsGrid()
        {
            if (itemsData == null) return;

            // Clear existing columns except checkbox and barcode preview
            while (dgvItems.Columns.Count > 2)
            {
                dgvItems.Columns.RemoveAt(2);
            }

            // Add data columns
            dgvItems.Columns.Add("ItemName", "Item Name");
            dgvItems.Columns.Add("Category", "Category");
            dgvItems.Columns.Add("CurrentBarcode", "Current Barcode");
            dgvItems.Columns.Add("MRP", "MRP");
            dgvItems.Columns.Add("StockInHand", "Stock");
            dgvItems.Columns.Add("SupplierName", "Supplier");
            dgvItems.Columns.Add("BarcodeStatus", "Status");

            // Populate rows
            dgvItems.Rows.Clear();
            foreach (DataRow row in itemsData.Rows)
            {
                int rowIndex = dgvItems.Rows.Add();
                dgvItems.Rows[rowIndex].Cells["Select"].Value = false;
                
                // Generate barcode preview if auto-generate is enabled
                if (chkAutoGenerate.Checked)
                {
                    Bitmap barcode = GenerateBarcodeImage(row["ItemID"].ToString(), 100, 30);
                    dgvItems.Rows[rowIndex].Cells["BarcodePreview"].Value = barcode;
                }
                
                dgvItems.Rows[rowIndex].Cells["ItemName"].Value = row["ItemName"];
                dgvItems.Rows[rowIndex].Cells["Category"].Value = row["Category"];
                dgvItems.Rows[rowIndex].Cells["CurrentBarcode"].Value = row["Barcode"];
                dgvItems.Rows[rowIndex].Cells["MRP"].Value = Convert.ToDecimal(row["MRP"]).ToString("N2");
                dgvItems.Rows[rowIndex].Cells["StockInHand"].Value = row["StockInHand"];
                dgvItems.Rows[rowIndex].Cells["SupplierName"].Value = row["SupplierName"];
                dgvItems.Rows[rowIndex].Cells["BarcodeStatus"].Value = row["BarcodeStatus"];
                
                // Store row data for later use
                dgvItems.Rows[rowIndex].Tag = row;

                // Color code based on barcode status
                if (row["BarcodeStatus"].ToString() == "Pending")
                {
                    dgvItems.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightYellow;
                }
                else
                {
                    dgvItems.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightGreen;
                }
            }
        }

        private Bitmap GenerateBarcodeImage(string data, int width, int height)
        {
            try
            {
                // Simplified barcode generation - In a real implementation, you would use a barcode library like ZXing.Net
                Bitmap barcode = new Bitmap(width, height);
                using (Graphics g = Graphics.FromImage(barcode))
                {
                    g.Clear(Color.White);
                    
                    // Draw simple bars to simulate a barcode
                    using (Brush blackBrush = new SolidBrush(Color.Black))
                    {
                        int barWidth = 2;
                        int x = 5;
                        
                        // Generate bars based on data hash for uniqueness
                        int hash = data.GetHashCode();
                        for (int i = 0; i < width - 10; i += barWidth * 2)
                        {
                            if ((hash >> (i % 32)) % 2 == 0)
                            {
                                g.FillRectangle(blackBrush, x + i, 5, barWidth, height - 10);
                            }
                        }
                    }
                    
                    // Add text below barcode
                    using (Font font = new Font("Arial", 8))
                    {
                        StringFormat sf = new StringFormat();
                        sf.Alignment = StringAlignment.Center;
                        g.DrawString(data, font, Brushes.Black, new RectangleF(0, height - 15, width, 15), sf);
                    }
                }
                
                return barcode;
            }
            catch
            {
                // Return a placeholder image if generation fails
                Bitmap placeholder = new Bitmap(width, height);
                using (Graphics g = Graphics.FromImage(placeholder))
                {
                    g.Clear(Color.LightGray);
                    g.DrawString("Error", SystemFonts.DefaultFont, Brushes.Red, 10, 10);
                }
                return placeholder;
            }
        }

        private void UpdateSummary()
        {
            if (itemsData != null)
            {
                int totalItems = itemsData.Rows.Count;
                int generatedCount = 0;
                int pendingCount = 0;

                foreach (DataRow row in itemsData.Rows)
                {
                    if (row["BarcodeStatus"].ToString() == "Generated")
                        generatedCount++;
                    else
                        pendingCount++;
                }

                lblTotalItems.Text = $"Total Items: {totalItems}";
                lblGeneratedBarcodes.Text = $"Generated Barcodes: {generatedCount}";
                lblPendingItems.Text = $"Pending Items: {pendingCount}";
            }
        }

        private void TxtItemSearch_TextChanged(object sender, EventArgs e)
        {
            if (txtItemSearch.Text.Length > 2 || string.IsNullOrEmpty(txtItemSearch.Text))
            {
                LoadItems();
            }
        }

        private void CmbItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)cmbItems.SelectedItem;
            if (selectedItem != null && (int)selectedItem.Value > 0)
            {
                GeneratePreviewBarcode((int)selectedItem.Value);
            }
        }

        private void CmbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadItems();
        }

        private void CmbSupplier_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadItems();
        }

        private void DgvItems_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvItems.SelectedRows.Count > 0)
            {
                DataRow selectedRow = (DataRow)dgvItems.SelectedRows[0].Tag;
                if (selectedRow != null)
                {
                    GeneratePreviewBarcode(Convert.ToInt32(selectedRow["ItemID"]));
                }
            }
        }

        private void GeneratePreviewBarcode(int itemID)
        {
            try
            {
                // Find item data
                DataRow[] rows = itemsData.Select($"ItemID = {itemID}");
                if (rows.Length > 0)
                {
                    DataRow item = rows[0];
                    string barcodeData = !string.IsNullOrEmpty(txtCustomBarcode.Text) ? 
                        txtCustomBarcode.Text : itemID.ToString().PadLeft(8, '0');
                    
                    // Generate barcode image
                    int width = (int)nudWidth.Value;
                    int height = (int)nudHeight.Value;
                    Bitmap barcode = GenerateBarcodeImage(barcodeData, width, height);
                    
                    picBarcode.Image = barcode;
                    
                    // Update barcode text with additional info
                    string barcodeText = barcodeData;
                    if (chkIncludePrice.Checked)
                        barcodeText += $" | ₹{item["MRP"]}";
                    if (chkIncludeBatch.Checked)
                        barcodeText += $" | Batch: {DateTime.Now:yyyyMM}";
                    if (chkIncludeExpiry.Checked)
                        barcodeText += $" | Exp: {DateTime.Now.AddYears(2):MM/yy}";
                    
                    lblBarcodeText.Text = barcodeText;
                    
                    // Store generated barcode
                    generatedBarcodes[itemID] = barcode;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating barcode preview: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)cmbItems.SelectedItem;
            if (selectedItem != null && (int)selectedItem.Value > 0)
            {
                GenerateAndSaveBarcode((int)selectedItem.Value);
            }
            else
            {
                MessageBox.Show("Please select an item to generate barcode.", "Selection Required", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnGenerateBulk_Click(object sender, EventArgs e)
        {
            List<int> selectedItems = new List<int>();
            
            foreach (DataGridViewRow row in dgvItems.Rows)
            {
                if (Convert.ToBoolean(row.Cells["Select"].Value))
                {
                    DataRow dataRow = (DataRow)row.Tag;
                    selectedItems.Add(Convert.ToInt32(dataRow["ItemID"]));
                }
            }

            if (selectedItems.Count == 0)
            {
                MessageBox.Show("Please select at least one item for bulk barcode generation.", "Selection Required", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show($"Generate barcodes for {selectedItems.Count} selected item(s)?", "Confirm Bulk Generation", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                int successCount = 0;
                foreach (int itemID in selectedItems)
                {
                    try
                    {
                        GenerateAndSaveBarcode(itemID);
                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error generating barcode for item ID {itemID}: {ex.Message}", 
                            "Generation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }

                MessageBox.Show($"Successfully generated {successCount} out of {selectedItems.Count} barcodes.", 
                    "Bulk Generation Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                LoadItems(); // Refresh the grid
            }
        }

        private void GenerateAndSaveBarcode(int itemID)
        {
            try
            {
                string barcodeData = !string.IsNullOrEmpty(txtCustomBarcode.Text) ? 
                    txtCustomBarcode.Text : itemID.ToString().PadLeft(12, '0');

                // Update item barcode in database
                string updateQuery = "UPDATE Items SET Barcode = @Barcode WHERE ItemID = @ItemID";
                SqlParameter[] parameters = {
                    new SqlParameter("@Barcode", barcodeData),
                    new SqlParameter("@ItemID", itemID)
                };

                DatabaseConnection.ExecuteNonQuery(updateQuery, parameters);

                // Generate and store barcode image
                int width = (int)nudWidth.Value;
                int height = (int)nudHeight.Value;
                Bitmap barcode = GenerateBarcodeImage(barcodeData, width, height);
                generatedBarcodes[itemID] = barcode;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving barcode: {ex.Message}");
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (picBarcode.Image != null)
                {
                    SaveFileDialog saveDialog = new SaveFileDialog();
                    saveDialog.Filter = $"{cmbFormat.SelectedItem} Files|*.{cmbFormat.SelectedItem.ToString().ToLower()}";
                    saveDialog.FileName = $"Barcode_{DateTime.Now:yyyyMMdd_HHmmss}";

                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        ImageFormat format = ImageFormat.Png;
                        switch (cmbFormat.SelectedItem.ToString())
                        {
                            case "JPG": format = ImageFormat.Jpeg; break;
                            case "BMP": format = ImageFormat.Bmp; break;
                            case "GIF": format = ImageFormat.Gif; break;
                        }

                        picBarcode.Image.Save(saveDialog.FileName, format);
                        MessageBox.Show("Barcode saved successfully!", "Save Complete", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("No barcode to save. Please generate a barcode first.", "Save Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving barcode: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show("Label Printing Features:\n\n" +
                    "• Multiple barcode sizes\n" +
                    "• Batch printing capabilities\n" +
                    "• Thermal printer support\n" +
                    "• Custom label templates\n" +
                    "• Price and expiry integration\n\n" +
                    "This would integrate with thermal label printers for pharmacy operations.", 
                    "Label Printing", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error printing labels: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            try
            {
                if (generatedBarcodes.Count == 0)
                {
                    MessageBox.Show("No barcodes to export. Please generate barcodes first.", "Export Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                FolderBrowserDialog folderDialog = new FolderBrowserDialog();
                folderDialog.Description = "Select folder to export barcodes";

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    int exportCount = 0;
                    foreach (var kvp in generatedBarcodes)
                    {
                        string fileName = Path.Combine(folderDialog.SelectedPath, $"Barcode_{kvp.Key}.png");
                        kvp.Value.Save(fileName, ImageFormat.Png);
                        exportCount++;
                    }

                    MessageBox.Show($"Successfully exported {exportCount} barcodes to {folderDialog.SelectedPath}", 
                        "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exporting barcodes: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
