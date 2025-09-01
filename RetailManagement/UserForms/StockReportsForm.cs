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
    public partial class StockReportsForm : Form
    {
        private ReportViewer reportViewer;
        private ComboBox cmbReportType;
        private ComboBox cmbCategory;
        private ComboBox cmbSupplier;
        private ComboBox cmbStockLevel;
        private ComboBox cmbValueRange;
        private DateTimePicker dtpExpiryFrom;
        private DateTimePicker dtpExpiryTo;
        private CheckBox chkShowZeroStock;
        private CheckBox chkShowExpired;
        private CheckBox chkShowNearExpiry;
        private CheckBox chkGroupByCategory;
        private CheckBox chkIncludeBatches;
        private Button btnGenerate;
        private Button btnExport;
        private Button btnPrint;
        private Button btnRefresh;
        private GroupBox groupFilters;
        private GroupBox groupOptions;
        private TabControl tabControl;
        private TabPage tabReports;
        private TabPage tabSummary;
        private DataTable currentStockData;
        private Panel summaryPanel;
        private Label lblTotalItems;
        private Label lblTotalStockValue;
        private Label lblLowStockItems;
        private Label lblExpiringItems;
        private Label lblZeroStockItems;
        private Label lblTopCategory;

        public StockReportsForm()
        {
            InitializeComponent();
            InitializeReportViewer();
            LoadDropdownData();
            SetDefaultDates();
        }

        private void InitializeComponent()
        {
            this.groupFilters = new System.Windows.Forms.GroupBox();
            this.groupOptions = new System.Windows.Forms.GroupBox();
            this.cmbReportType = new System.Windows.Forms.ComboBox();
            this.cmbCategory = new System.Windows.Forms.ComboBox();
            this.cmbSupplier = new System.Windows.Forms.ComboBox();
            this.cmbStockLevel = new System.Windows.Forms.ComboBox();
            this.cmbValueRange = new System.Windows.Forms.ComboBox();
            this.dtpExpiryFrom = new System.Windows.Forms.DateTimePicker();
            this.dtpExpiryTo = new System.Windows.Forms.DateTimePicker();
            this.chkShowZeroStock = new System.Windows.Forms.CheckBox();
            this.chkShowExpired = new System.Windows.Forms.CheckBox();
            this.chkShowNearExpiry = new System.Windows.Forms.CheckBox();
            this.chkGroupByCategory = new System.Windows.Forms.CheckBox();
            this.chkIncludeBatches = new System.Windows.Forms.CheckBox();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabReports = new System.Windows.Forms.TabPage();
            this.tabSummary = new System.Windows.Forms.TabPage();
            this.summaryPanel = new System.Windows.Forms.Panel();
            this.lblTotalItems = new System.Windows.Forms.Label();
            this.lblTotalStockValue = new System.Windows.Forms.Label();
            this.lblLowStockItems = new System.Windows.Forms.Label();
            this.lblExpiringItems = new System.Windows.Forms.Label();
            this.lblZeroStockItems = new System.Windows.Forms.Label();
            this.lblTopCategory = new System.Windows.Forms.Label();
            
            this.groupFilters.SuspendLayout();
            this.groupOptions.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabReports.SuspendLayout();
            this.tabSummary.SuspendLayout();
            this.summaryPanel.SuspendLayout();
            this.SuspendLayout();
            
            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1400, 800);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.groupOptions);
            this.Controls.Add(this.groupFilters);
            this.Name = "StockReportsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Stock Reports & Analytics - Retail Management System";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            
            // groupFilters
            this.groupFilters.Controls.Add(this.dtpExpiryTo);
            this.groupFilters.Controls.Add(this.dtpExpiryFrom);
            this.groupFilters.Controls.Add(this.cmbValueRange);
            this.groupFilters.Controls.Add(this.cmbStockLevel);
            this.groupFilters.Controls.Add(this.cmbSupplier);
            this.groupFilters.Controls.Add(this.cmbCategory);
            this.groupFilters.Controls.Add(this.cmbReportType);
            this.groupFilters.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupFilters.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.groupFilters.Location = new System.Drawing.Point(0, 0);
            this.groupFilters.Name = "groupFilters";
            this.groupFilters.Size = new System.Drawing.Size(1400, 80);
            this.groupFilters.TabIndex = 0;
            this.groupFilters.TabStop = false;
            this.groupFilters.Text = "Stock Report Filters";
            
            // Report Type
            this.cmbReportType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbReportType.FormattingEnabled = true;
            this.cmbReportType.Location = new System.Drawing.Point(15, 25);
            this.cmbReportType.Name = "cmbReportType";
            this.cmbReportType.Size = new System.Drawing.Size(180, 21);
            this.cmbReportType.TabIndex = 0;
            this.cmbReportType.SelectedIndexChanged += new System.EventHandler(this.cmbReportType_SelectedIndexChanged);
            
            // Category
            this.cmbCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCategory.FormattingEnabled = true;
            this.cmbCategory.Location = new System.Drawing.Point(210, 25);
            this.cmbCategory.Name = "cmbCategory";
            this.cmbCategory.Size = new System.Drawing.Size(140, 21);
            this.cmbCategory.TabIndex = 1;
            
            // Supplier
            this.cmbSupplier.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSupplier.FormattingEnabled = true;
            this.cmbSupplier.Location = new System.Drawing.Point(365, 25);
            this.cmbSupplier.Name = "cmbSupplier";
            this.cmbSupplier.Size = new System.Drawing.Size(140, 21);
            this.cmbSupplier.TabIndex = 2;
            
            // Stock Level
            this.cmbStockLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStockLevel.FormattingEnabled = true;
            this.cmbStockLevel.Location = new System.Drawing.Point(520, 25);
            this.cmbStockLevel.Name = "cmbStockLevel";
            this.cmbStockLevel.Size = new System.Drawing.Size(120, 21);
            this.cmbStockLevel.TabIndex = 3;
            
            // Value Range
            this.cmbValueRange.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbValueRange.FormattingEnabled = true;
            this.cmbValueRange.Location = new System.Drawing.Point(655, 25);
            this.cmbValueRange.Name = "cmbValueRange";
            this.cmbValueRange.Size = new System.Drawing.Size(120, 21);
            this.cmbValueRange.TabIndex = 4;
            
            // Expiry From Date
            this.dtpExpiryFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpExpiryFrom.Location = new System.Drawing.Point(790, 25);
            this.dtpExpiryFrom.Name = "dtpExpiryFrom";
            this.dtpExpiryFrom.Size = new System.Drawing.Size(100, 20);
            this.dtpExpiryFrom.TabIndex = 5;
            
            // Expiry To Date
            this.dtpExpiryTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpExpiryTo.Location = new System.Drawing.Point(905, 25);
            this.dtpExpiryTo.Name = "dtpExpiryTo";
            this.dtpExpiryTo.Size = new System.Drawing.Size(100, 20);
            this.dtpExpiryTo.TabIndex = 6;
            
            // groupOptions
            this.groupOptions.Controls.Add(this.btnRefresh);
            this.groupOptions.Controls.Add(this.btnPrint);
            this.groupOptions.Controls.Add(this.btnExport);
            this.groupOptions.Controls.Add(this.btnGenerate);
            this.groupOptions.Controls.Add(this.chkIncludeBatches);
            this.groupOptions.Controls.Add(this.chkGroupByCategory);
            this.groupOptions.Controls.Add(this.chkShowNearExpiry);
            this.groupOptions.Controls.Add(this.chkShowExpired);
            this.groupOptions.Controls.Add(this.chkShowZeroStock);
            this.groupOptions.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupOptions.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.groupOptions.Location = new System.Drawing.Point(0, 80);
            this.groupOptions.Name = "groupOptions";
            this.groupOptions.Size = new System.Drawing.Size(1400, 70);
            this.groupOptions.TabIndex = 1;
            this.groupOptions.TabStop = false;
            this.groupOptions.Text = "Report Options";
            
            // Show Zero Stock
            this.chkShowZeroStock.AutoSize = true;
            this.chkShowZeroStock.Location = new System.Drawing.Point(15, 25);
            this.chkShowZeroStock.Name = "chkShowZeroStock";
            this.chkShowZeroStock.Size = new System.Drawing.Size(125, 19);
            this.chkShowZeroStock.TabIndex = 0;
            this.chkShowZeroStock.Text = "Show Zero Stock";
            this.chkShowZeroStock.UseVisualStyleBackColor = true;
            
            // Show Expired
            this.chkShowExpired.AutoSize = true;
            this.chkShowExpired.Location = new System.Drawing.Point(155, 25);
            this.chkShowExpired.Name = "chkShowExpired";
            this.chkShowExpired.Size = new System.Drawing.Size(115, 19);
            this.chkShowExpired.TabIndex = 1;
            this.chkShowExpired.Text = "Show Expired";
            this.chkShowExpired.UseVisualStyleBackColor = true;
            
            // Show Near Expiry
            this.chkShowNearExpiry.AutoSize = true;
            this.chkShowNearExpiry.Checked = true;
            this.chkShowNearExpiry.Location = new System.Drawing.Point(285, 25);
            this.chkShowNearExpiry.Name = "chkShowNearExpiry";
            this.chkShowNearExpiry.Size = new System.Drawing.Size(140, 19);
            this.chkShowNearExpiry.TabIndex = 2;
            this.chkShowNearExpiry.Text = "Show Near Expiry";
            this.chkShowNearExpiry.UseVisualStyleBackColor = true;
            
            // Group By Category
            this.chkGroupByCategory.AutoSize = true;
            this.chkGroupByCategory.Location = new System.Drawing.Point(15, 50);
            this.chkGroupByCategory.Name = "chkGroupByCategory";
            this.chkGroupByCategory.Size = new System.Drawing.Size(140, 19);
            this.chkGroupByCategory.TabIndex = 3;
            this.chkGroupByCategory.Text = "Group by Category";
            this.chkGroupByCategory.UseVisualStyleBackColor = true;
            
            // Include Batches
            this.chkIncludeBatches.AutoSize = true;
            this.chkIncludeBatches.Checked = true;
            this.chkIncludeBatches.Location = new System.Drawing.Point(170, 50);
            this.chkIncludeBatches.Name = "chkIncludeBatches";
            this.chkIncludeBatches.Size = new System.Drawing.Size(125, 19);
            this.chkIncludeBatches.TabIndex = 4;
            this.chkIncludeBatches.Text = "Include Batches";
            this.chkIncludeBatches.UseVisualStyleBackColor = true;
            
            // Generate Button
            this.btnGenerate.BackColor = System.Drawing.Color.Green;
            this.btnGenerate.ForeColor = System.Drawing.Color.White;
            this.btnGenerate.Location = new System.Drawing.Point(320, 35);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(90, 25);
            this.btnGenerate.TabIndex = 5;
            this.btnGenerate.Text = "Generate";
            this.btnGenerate.UseVisualStyleBackColor = false;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            
            // Export Button
            this.btnExport.BackColor = System.Drawing.Color.Blue;
            this.btnExport.ForeColor = System.Drawing.Color.White;
            this.btnExport.Location = new System.Drawing.Point(420, 35);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(90, 25);
            this.btnExport.TabIndex = 6;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = false;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            
            // Print Button
            this.btnPrint.BackColor = System.Drawing.Color.DarkBlue;
            this.btnPrint.ForeColor = System.Drawing.Color.White;
            this.btnPrint.Location = new System.Drawing.Point(520, 35);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(90, 25);
            this.btnPrint.TabIndex = 7;
            this.btnPrint.Text = "Print";
            this.btnPrint.UseVisualStyleBackColor = false;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            
            // Refresh Button
            this.btnRefresh.BackColor = System.Drawing.Color.Orange;
            this.btnRefresh.ForeColor = System.Drawing.Color.White;
            this.btnRefresh.Location = new System.Drawing.Point(620, 35);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(90, 25);
            this.btnRefresh.TabIndex = 8;
            this.btnRefresh.Text = "Refresh Data";
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            
            // Tab Control
            this.tabControl.Controls.Add(this.tabReports);
            this.tabControl.Controls.Add(this.tabSummary);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 150);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(1400, 650);
            this.tabControl.TabIndex = 2;
            
            // Tab Reports
            this.tabReports.Location = new System.Drawing.Point(4, 22);
            this.tabReports.Name = "tabReports";
            this.tabReports.Padding = new System.Windows.Forms.Padding(3);
            this.tabReports.Size = new System.Drawing.Size(1392, 624);
            this.tabReports.TabIndex = 0;
            this.tabReports.Text = "Detailed Stock Reports";
            this.tabReports.UseVisualStyleBackColor = true;
            
            // Tab Summary
            this.tabSummary.Controls.Add(this.summaryPanel);
            this.tabSummary.Location = new System.Drawing.Point(4, 22);
            this.tabSummary.Name = "tabSummary";
            this.tabSummary.Padding = new System.Windows.Forms.Padding(3);
            this.tabSummary.Size = new System.Drawing.Size(1392, 624);
            this.tabSummary.TabIndex = 1;
            this.tabSummary.Text = "Stock Analytics & Summary";
            this.tabSummary.UseVisualStyleBackColor = true;
            
            // Summary Panel
            this.summaryPanel.Controls.Add(this.lblTopCategory);
            this.summaryPanel.Controls.Add(this.lblZeroStockItems);
            this.summaryPanel.Controls.Add(this.lblExpiringItems);
            this.summaryPanel.Controls.Add(this.lblLowStockItems);
            this.summaryPanel.Controls.Add(this.lblTotalStockValue);
            this.summaryPanel.Controls.Add(this.lblTotalItems);
            this.summaryPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.summaryPanel.Location = new System.Drawing.Point(3, 3);
            this.summaryPanel.Name = "summaryPanel";
            this.summaryPanel.Size = new System.Drawing.Size(1386, 618);
            this.summaryPanel.TabIndex = 0;
            
            // Summary Labels
            this.lblTotalItems.AutoSize = true;
            this.lblTotalItems.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold);
            this.lblTotalItems.ForeColor = System.Drawing.Color.DarkBlue;
            this.lblTotalItems.Location = new System.Drawing.Point(30, 30);
            this.lblTotalItems.Name = "lblTotalItems";
            this.lblTotalItems.Size = new System.Drawing.Size(200, 29);
            this.lblTotalItems.TabIndex = 0;
            this.lblTotalItems.Text = "Total Items: 0";
            
            this.lblTotalStockValue.AutoSize = true;
            this.lblTotalStockValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold);
            this.lblTotalStockValue.ForeColor = System.Drawing.Color.Green;
            this.lblTotalStockValue.Location = new System.Drawing.Point(30, 80);
            this.lblTotalStockValue.Name = "lblTotalStockValue";
            this.lblTotalStockValue.Size = new System.Drawing.Size(250, 26);
            this.lblTotalStockValue.TabIndex = 1;
            this.lblTotalStockValue.Text = "Total Stock Value: ₹0.00";
            
            this.lblLowStockItems.AutoSize = true;
            this.lblLowStockItems.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold);
            this.lblLowStockItems.ForeColor = System.Drawing.Color.Orange;
            this.lblLowStockItems.Location = new System.Drawing.Point(30, 130);
            this.lblLowStockItems.Name = "lblLowStockItems";
            this.lblLowStockItems.Size = new System.Drawing.Size(200, 24);
            this.lblLowStockItems.TabIndex = 2;
            this.lblLowStockItems.Text = "Low Stock Items: 0";
            
            this.lblExpiringItems.AutoSize = true;
            this.lblExpiringItems.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold);
            this.lblExpiringItems.ForeColor = System.Drawing.Color.Red;
            this.lblExpiringItems.Location = new System.Drawing.Point(30, 180);
            this.lblExpiringItems.Name = "lblExpiringItems";
            this.lblExpiringItems.Size = new System.Drawing.Size(250, 24);
            this.lblExpiringItems.TabIndex = 3;
            this.lblExpiringItems.Text = "Expiring Soon Items: 0";
            
            this.lblZeroStockItems.AutoSize = true;
            this.lblZeroStockItems.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lblZeroStockItems.ForeColor = System.Drawing.Color.DarkRed;
            this.lblZeroStockItems.Location = new System.Drawing.Point(30, 230);
            this.lblZeroStockItems.Name = "lblZeroStockItems";
            this.lblZeroStockItems.Size = new System.Drawing.Size(200, 20);
            this.lblZeroStockItems.TabIndex = 4;
            this.lblZeroStockItems.Text = "Zero Stock Items: 0";
            
            this.lblTopCategory.AutoSize = true;
            this.lblTopCategory.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lblTopCategory.ForeColor = System.Drawing.Color.Purple;
            this.lblTopCategory.Location = new System.Drawing.Point(30, 280);
            this.lblTopCategory.Name = "lblTopCategory";
            this.lblTopCategory.Size = new System.Drawing.Size(250, 20);
            this.lblTopCategory.TabIndex = 5;
            this.lblTopCategory.Text = "Top Category by Value: N/A";
            
            this.groupFilters.ResumeLayout(false);
            this.groupOptions.ResumeLayout(false);
            this.groupOptions.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.tabReports.ResumeLayout(false);
            this.tabSummary.ResumeLayout(false);
            this.summaryPanel.ResumeLayout(false);
            this.summaryPanel.PerformLayout();
            this.ResumeLayout(false);
        }

        private void InitializeReportViewer()
        {
            reportViewer = new ReportViewer
            {
                Dock = DockStyle.Fill,
                ProcessingMode = ProcessingMode.Local,
                ShowBackButton = false,
                ShowCredentialPrompts = false,
                ShowDocumentMapButton = false,
                ShowFindControls = true,
                ShowPageNavigationControls = true,
                ShowParameterPrompts = false,
                ShowPrintButton = true,
                ShowProgress = true,
                ShowRefreshButton = true,
                ShowStopButton = true,
                ShowToolBar = true,
                ShowZoomControl = true
            };
            
            tabReports.Controls.Add(reportViewer);
        }

        private void LoadDropdownData()
        {
            try
            {
                // Load Report Types
                cmbReportType.Items.Clear();
                cmbReportType.Items.Add(new ComboBoxItem { Text = "Current Stock Report", Value = "CURRENT_STOCK" });
                cmbReportType.Items.Add(new ComboBoxItem { Text = "Low Stock Alert", Value = "LOW_STOCK" });
                cmbReportType.Items.Add(new ComboBoxItem { Text = "Expiry Alert Report", Value = "EXPIRY_ALERT" });
                cmbReportType.Items.Add(new ComboBoxItem { Text = "Zero Stock Report", Value = "ZERO_STOCK" });
                cmbReportType.Items.Add(new ComboBoxItem { Text = "Stock Valuation", Value = "STOCK_VALUATION" });
                cmbReportType.Items.Add(new ComboBoxItem { Text = "Category-wise Stock", Value = "CATEGORY_STOCK" });
                cmbReportType.Items.Add(new ComboBoxItem { Text = "Batch-wise Stock", Value = "BATCH_STOCK" });
                cmbReportType.Items.Add(new ComboBoxItem { Text = "Supplier-wise Stock", Value = "SUPPLIER_STOCK" });
                cmbReportType.Items.Add(new ComboBoxItem { Text = "Stock Movement", Value = "STOCK_MOVEMENT" });
                cmbReportType.Items.Add(new ComboBoxItem { Text = "ABC Analysis", Value = "ABC_ANALYSIS" });
                cmbReportType.DisplayMember = "Text";
                cmbReportType.ValueMember = "Value";
                cmbReportType.SelectedIndex = 0;
                
                // Load Categories
                string categoryQuery = "SELECT DISTINCT Category FROM Items WHERE Category IS NOT NULL AND Category != '' ORDER BY Category";
                DataTable categories = DatabaseConnection.ExecuteQuery(categoryQuery);
                
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
                
                // Load Suppliers
                string supplierQuery = "SELECT CompanyID, CompanyName FROM Companies WHERE IsActive = 1 ORDER BY CompanyName";
                DataTable suppliers = DatabaseConnection.ExecuteQuery(supplierQuery);
                
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
                
                // Load Stock Levels
                cmbStockLevel.Items.Clear();
                cmbStockLevel.Items.Add(new ComboBoxItem { Text = "All Levels", Value = "ALL" });
                cmbStockLevel.Items.Add(new ComboBoxItem { Text = "In Stock", Value = "IN_STOCK" });
                cmbStockLevel.Items.Add(new ComboBoxItem { Text = "Low Stock", Value = "LOW_STOCK" });
                cmbStockLevel.Items.Add(new ComboBoxItem { Text = "Out of Stock", Value = "OUT_OF_STOCK" });
                cmbStockLevel.Items.Add(new ComboBoxItem { Text = "Overstock", Value = "OVERSTOCK" });
                cmbStockLevel.DisplayMember = "Text";
                cmbStockLevel.ValueMember = "Value";
                cmbStockLevel.SelectedIndex = 0;
                
                // Load Value Ranges
                cmbValueRange.Items.Clear();
                cmbValueRange.Items.Add(new ComboBoxItem { Text = "All Values", Value = "ALL" });
                cmbValueRange.Items.Add(new ComboBoxItem { Text = "₹0 - ₹1,000", Value = "0-1000" });
                cmbValueRange.Items.Add(new ComboBoxItem { Text = "₹1,001 - ₹5,000", Value = "1001-5000" });
                cmbValueRange.Items.Add(new ComboBoxItem { Text = "₹5,001 - ₹10,000", Value = "5001-10000" });
                cmbValueRange.Items.Add(new ComboBoxItem { Text = "₹10,001 - ₹25,000", Value = "10001-25000" });
                cmbValueRange.Items.Add(new ComboBoxItem { Text = "₹25,001 - ₹50,000", Value = "25001-50000" });
                cmbValueRange.Items.Add(new ComboBoxItem { Text = "Above ₹50,000", Value = "50001-999999999" });
                cmbValueRange.DisplayMember = "Text";
                cmbValueRange.ValueMember = "Value";
                cmbValueRange.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading dropdown data: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetDefaultDates()
        {
            dtpExpiryFrom.Value = DateTime.Today;
            dtpExpiryTo.Value = DateTime.Today.AddMonths(6); // Next 6 months
        }

        private void cmbReportType_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Enable/disable relevant options based on report type
            ComboBoxItem selectedType = (ComboBoxItem)cmbReportType.SelectedItem;
            if (selectedType != null)
            {
                string reportType = selectedType.Value.ToString();
                
                // Show/hide expiry date filters
                bool showExpiryDates = (reportType == "EXPIRY_ALERT" || reportType == "BATCH_STOCK");
                dtpExpiryFrom.Visible = showExpiryDates;
                dtpExpiryTo.Visible = showExpiryDates;
                
                // Enable/disable checkboxes based on report type
                chkShowZeroStock.Enabled = (reportType != "ZERO_STOCK");
                chkShowExpired.Enabled = (reportType != "EXPIRY_ALERT");
                chkIncludeBatches.Enabled = (reportType != "BATCH_STOCK");
                
                // Set default values
                switch (reportType)
                {
                    case "LOW_STOCK":
                        chkShowZeroStock.Checked = false;
                        cmbStockLevel.SelectedValue = "LOW_STOCK";
                        break;
                    case "ZERO_STOCK":
                        chkShowZeroStock.Checked = true;
                        cmbStockLevel.SelectedValue = "OUT_OF_STOCK";
                        break;
                    case "EXPIRY_ALERT":
                        chkShowNearExpiry.Checked = true;
                        chkShowExpired.Checked = true;
                        break;
                }
            }
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            try
            {
                GenerateStockReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating stock report: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerateStockReport()
        {
            try
            {
                // Get filter values
                ComboBoxItem selectedType = (ComboBoxItem)cmbReportType.SelectedItem;
                ComboBoxItem selectedCategory = (ComboBoxItem)cmbCategory.SelectedItem;
                ComboBoxItem selectedSupplier = (ComboBoxItem)cmbSupplier.SelectedItem;
                ComboBoxItem selectedStockLevel = (ComboBoxItem)cmbStockLevel.SelectedItem;
                ComboBoxItem selectedValueRange = (ComboBoxItem)cmbValueRange.SelectedItem;
                
                string reportType = selectedType?.Value.ToString() ?? "CURRENT_STOCK";
                string category = selectedCategory?.Value.ToString() ?? "";
                int supplierID = selectedSupplier != null ? (int)selectedSupplier.Value : 0;
                string stockLevel = selectedStockLevel?.Value.ToString() ?? "ALL";
                string valueRange = selectedValueRange?.Value.ToString() ?? "ALL";
                
                // Call appropriate stored procedure based on report type
                string storedProcedure = GetStockReportStoredProcedure(reportType);
                currentStockData = ExecuteStockReportQuery(storedProcedure, reportType, category, supplierID, 
                    stockLevel, valueRange, dtpExpiryFrom.Value, dtpExpiryTo.Value);
                
                if (currentStockData != null && currentStockData.Rows.Count > 0)
                {
                    // Generate the report
                    string reportPath = GetStockReportPath(reportType);
                    GenerateRDLCStockReport(reportPath, reportType);
                    
                    // Update analytics
                    UpdateStockAnalytics();
                }
                else
                {
                    MessageBox.Show("No stock data found for the selected criteria.", "Information",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearStockReport();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating stock report: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetStockReportStoredProcedure(string reportType)
        {
            switch (reportType)
            {
                case "CURRENT_STOCK": return "sp_GetCurrentStockReport";
                case "LOW_STOCK": return "sp_GetLowStockReport";
                case "EXPIRY_ALERT": return "sp_GetExpiryAlertReport";
                case "ZERO_STOCK": return "sp_GetZeroStockReport";
                case "STOCK_VALUATION": return "sp_GetStockValuationReport";
                case "CATEGORY_STOCK": return "sp_GetCategoryStockReport";
                case "BATCH_STOCK": return "sp_GetBatchStockReport";
                case "SUPPLIER_STOCK": return "sp_GetSupplierStockReport";
                case "STOCK_MOVEMENT": return "sp_GetStockMovementReport";
                case "ABC_ANALYSIS": return "sp_GetABCAnalysisReport";
                default: return "sp_GetCurrentStockReport";
            }
        }

        private string GetStockReportPath(string reportType)
        {
            switch (reportType)
            {
                case "CURRENT_STOCK": return "Reports/CurrentStockReport.rdlc";
                case "LOW_STOCK": return "Reports/LowStockReport.rdlc";
                case "EXPIRY_ALERT": return "Reports/ExpiryReport.rdlc";
                case "ZERO_STOCK": return "Reports/ZeroStockReport.rdlc";
                case "STOCK_VALUATION": return "Reports/StockValuationReport.rdlc";
                case "CATEGORY_STOCK": return "Reports/CategoryStockReport.rdlc";
                case "BATCH_STOCK": return "Reports/BatchStockReport.rdlc";
                case "SUPPLIER_STOCK": return "Reports/SupplierStockReport.rdlc";
                case "STOCK_MOVEMENT": return "Reports/StockMovementReport.rdlc";
                case "ABC_ANALYSIS": return "Reports/ABCAnalysisReport.rdlc";
                default: return "Reports/StockInHandReport.rdlc";
            }
        }

        private DataTable ExecuteStockReportQuery(string storedProcedure, string reportType, string category, 
            int supplierID, string stockLevel, string valueRange, DateTime expiryFrom, DateTime expiryTo)
        {
            try
            {
                // Parse value range
                decimal minValue = 0, maxValue = 999999999;
                if (valueRange != "ALL" && valueRange.Contains("-"))
                {
                    string[] parts = valueRange.Split('-');
                    if (parts.Length == 2)
                    {
                        decimal.TryParse(parts[0], out minValue);
                        decimal.TryParse(parts[1], out maxValue);
                    }
                }

                SqlParameter[] parameters = {
                    new SqlParameter("@Category", string.IsNullOrEmpty(category) ? (object)DBNull.Value : category),
                    new SqlParameter("@SupplierID", supplierID == 0 ? (object)DBNull.Value : supplierID),
                    new SqlParameter("@StockLevel", stockLevel == "ALL" ? (object)DBNull.Value : stockLevel),
                    new SqlParameter("@MinValue", minValue),
                    new SqlParameter("@MaxValue", maxValue),
                    new SqlParameter("@ExpiryFromDate", expiryFrom),
                    new SqlParameter("@ExpiryToDate", expiryTo),
                    new SqlParameter("@ShowZeroStock", chkShowZeroStock.Checked),
                    new SqlParameter("@ShowExpired", chkShowExpired.Checked),
                    new SqlParameter("@ShowNearExpiry", chkShowNearExpiry.Checked),
                    new SqlParameter("@GroupByCategory", chkGroupByCategory.Checked),
                    new SqlParameter("@IncludeBatches", chkIncludeBatches.Checked)
                };
                
                return DatabaseConnection.ExecuteQuery($"EXEC {storedProcedure} @Category, @SupplierID, @StockLevel, @MinValue, @MaxValue, @ExpiryFromDate, @ExpiryToDate, @ShowZeroStock, @ShowExpired, @ShowNearExpiry, @GroupByCategory, @IncludeBatches", parameters);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error executing stock report query: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        private void GenerateRDLCStockReport(string reportPath, string reportType)
        {
            try
            {
                // Clear previous report
                reportViewer.Reset();
                
                // Set report path
                reportViewer.LocalReport.ReportPath = reportPath;
                
                // Set data source
                string dataSetName = GetStockDataSetName(reportType);
                ReportDataSource rds = new ReportDataSource(dataSetName, currentStockData);
                reportViewer.LocalReport.DataSources.Clear();
                reportViewer.LocalReport.DataSources.Add(rds);
                
                // Set report parameters
                SetStockReportParameters(reportType);
                
                // Refresh the report
                reportViewer.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating RDLC stock report: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetStockDataSetName(string reportType)
        {
            switch (reportType)
            {
                case "CURRENT_STOCK": return "CurrentStockDataSet";
                case "LOW_STOCK": return "LowStockDataSet";
                case "EXPIRY_ALERT": return "ExpiryDataSet";
                case "ZERO_STOCK": return "ZeroStockDataSet";
                case "STOCK_VALUATION": return "StockValuationDataSet";
                case "CATEGORY_STOCK": return "CategoryStockDataSet";
                case "BATCH_STOCK": return "BatchStockDataSet";
                case "SUPPLIER_STOCK": return "SupplierStockDataSet";
                case "STOCK_MOVEMENT": return "StockMovementDataSet";
                case "ABC_ANALYSIS": return "ABCAnalysisDataSet";
                default: return "StockDataSet";
            }
        }

        private void SetStockReportParameters(string reportType)
        {
            try
            {
                List<ReportParameter> parameters = new List<ReportParameter>();
                
                // Common parameters
                parameters.Add(new ReportParameter("GeneratedDate", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")));
                parameters.Add(new ReportParameter("CompanyName", "Retail Management System"));
                parameters.Add(new ReportParameter("ReportTitle", GetStockReportTitle(reportType)));
                
                // Report-specific parameters
                ComboBoxItem selectedCategory = (ComboBoxItem)cmbCategory.SelectedItem;
                ComboBoxItem selectedSupplier = (ComboBoxItem)cmbSupplier.SelectedItem;
                ComboBoxItem selectedStockLevel = (ComboBoxItem)cmbStockLevel.SelectedItem;
                ComboBoxItem selectedValueRange = (ComboBoxItem)cmbValueRange.SelectedItem;
                
                if (selectedCategory != null)
                    parameters.Add(new ReportParameter("CategoryFilter", selectedCategory.Text));
                if (selectedSupplier != null)
                    parameters.Add(new ReportParameter("SupplierFilter", selectedSupplier.Text));
                if (selectedStockLevel != null)
                    parameters.Add(new ReportParameter("StockLevelFilter", selectedStockLevel.Text));
                if (selectedValueRange != null)
                    parameters.Add(new ReportParameter("ValueRangeFilter", selectedValueRange.Text));
                
                parameters.Add(new ReportParameter("ExpiryFromDate", dtpExpiryFrom.Value.ToString("dd/MM/yyyy")));
                parameters.Add(new ReportParameter("ExpiryToDate", dtpExpiryTo.Value.ToString("dd/MM/yyyy")));
                parameters.Add(new ReportParameter("ShowZeroStock", chkShowZeroStock.Checked.ToString()));
                parameters.Add(new ReportParameter("IncludeBatches", chkIncludeBatches.Checked.ToString()));
                
                reportViewer.LocalReport.SetParameters(parameters);
            }
            catch (Exception)
            {
                // Handle parameter errors silently
            }
        }

        private string GetStockReportTitle(string reportType)
        {
            switch (reportType)
            {
                case "CURRENT_STOCK": return "Current Stock Report";
                case "LOW_STOCK": return "Low Stock Alert Report";
                case "EXPIRY_ALERT": return "Expiry Alert Report";
                case "ZERO_STOCK": return "Zero Stock Report";
                case "STOCK_VALUATION": return "Stock Valuation Report";
                case "CATEGORY_STOCK": return "Category-wise Stock Report";
                case "BATCH_STOCK": return "Batch-wise Stock Report";
                case "SUPPLIER_STOCK": return "Supplier-wise Stock Report";
                case "STOCK_MOVEMENT": return "Stock Movement Report";
                case "ABC_ANALYSIS": return "ABC Analysis Report";
                default: return "Stock Report";
            }
        }

        private void UpdateStockAnalytics()
        {
            if (currentStockData != null && currentStockData.Rows.Count > 0)
            {
                try
                {
                    // Calculate analytics based on available columns
                    int totalItems = currentStockData.Rows.Count;
                    decimal totalStockValue = 0;
                    int lowStockItems = 0;
                    int expiringItems = 0;
                    int zeroStockItems = 0;
                    string topCategory = "N/A";
                    
                    // Try to get stock value from different possible column names
                    string[] valueColumns = { "StockValue", "TotalValue", "Value", "NetAmount" };
                    string[] quantityColumns = { "CurrentStock", "StockQuantity", "Quantity", "QtyInHand" };
                    string valueColumn = null;
                    string quantityColumn = null;
                    
                    foreach (string col in valueColumns)
                    {
                        if (currentStockData.Columns.Contains(col))
                        {
                            valueColumn = col;
                            break;
                        }
                    }
                    
                    foreach (string col in quantityColumns)
                    {
                        if (currentStockData.Columns.Contains(col))
                        {
                            quantityColumn = col;
                            break;
                        }
                    }
                    
                    // Calculate totals
                    if (valueColumn != null)
                    {
                        totalStockValue = currentStockData.AsEnumerable()
                            .Sum(row => row.Field<decimal?>(valueColumn) ?? 0);
                    }
                    
                    if (quantityColumn != null)
                    {
                        foreach (DataRow row in currentStockData.Rows)
                        {
                            decimal qty = row.Field<decimal?>(quantityColumn) ?? 0;
                            decimal minStock = row.Field<decimal?>("MinStock") ?? 0;
                            
                            if (qty == 0)
                                zeroStockItems++;
                            else if (qty <= minStock && minStock > 0)
                                lowStockItems++;
                        }
                    }
                    
                    // Check for expiring items
                    if (currentStockData.Columns.Contains("ExpiryDate"))
                    {
                        DateTime alertDate = DateTime.Today.AddDays(30); // 30 days from now
                        expiringItems = currentStockData.AsEnumerable()
                            .Count(row => {
                                DateTime? expiryDate = row.Field<DateTime?>("ExpiryDate");
                                return expiryDate.HasValue && expiryDate.Value <= alertDate;
                            });
                    }
                    
                    // Get top category by value
                    if (currentStockData.Columns.Contains("Category") && valueColumn != null)
                    {
                        var topCategoryData = currentStockData.AsEnumerable()
                            .Where(row => !string.IsNullOrEmpty(row.Field<string>("Category")))
                            .GroupBy(row => row.Field<string>("Category"))
                            .OrderByDescending(g => g.Sum(row => row.Field<decimal?>(valueColumn) ?? 0))
                            .FirstOrDefault();
                        
                        if (topCategoryData != null)
                            topCategory = topCategoryData.Key;
                    }
                    
                    // Update labels
                    lblTotalItems.Text = $"Total Items: {totalItems:N0}";
                    lblTotalStockValue.Text = $"Total Stock Value: ₹{totalStockValue:N2}";
                    lblLowStockItems.Text = $"Low Stock Items: {lowStockItems:N0}";
                    lblExpiringItems.Text = $"Expiring Soon Items: {expiringItems:N0}";
                    lblZeroStockItems.Text = $"Zero Stock Items: {zeroStockItems:N0}";
                    lblTopCategory.Text = $"Top Category by Value: {topCategory}";
                }
                catch (Exception)
                {
                    // Handle analytics calculation errors silently
                }
            }
            else
            {
                lblTotalItems.Text = "Total Items: 0";
                lblTotalStockValue.Text = "Total Stock Value: ₹0.00";
                lblLowStockItems.Text = "Low Stock Items: 0";
                lblExpiringItems.Text = "Expiring Soon Items: 0";
                lblZeroStockItems.Text = "Zero Stock Items: 0";
                lblTopCategory.Text = "Top Category by Value: N/A";
            }
        }

        private void ClearStockReport()
        {
            reportViewer.Reset();
            currentStockData = null;
            UpdateStockAnalytics();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentStockData == null || currentStockData.Rows.Count == 0)
                {
                    MessageBox.Show("Please generate a stock report first.", "Information",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "PDF Files|*.pdf|Excel Files|*.xlsx|Word Documents|*.docx";
                saveDialog.Title = "Export Stock Report";
                
                ComboBoxItem selectedType = (ComboBoxItem)cmbReportType.SelectedItem;
                string reportName = selectedType?.Text?.Replace(" ", "_") ?? "StockReport";
                saveDialog.FileName = $"{reportName}_{DateTime.Now:yyyyMMdd_HHmmss}";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    string extension = Path.GetExtension(saveDialog.FileName).ToLower();
                    
                    Warning[] warnings;
                    string[] streamids;
                    string mimeType;
                    string encoding;
                    string deviceInfo = "";
                    
                    byte[] bytes = null;
                    
                    switch (extension)
                    {
                        case ".pdf":
                            deviceInfo = "<DeviceInfo>" +
                                        "<OutputFormat>PDF</OutputFormat>" +
                                        "<PageWidth>11in</PageWidth>" +
                                        "<PageHeight>8.5in</PageHeight>" +
                                        "<MarginTop>0.5in</MarginTop>" +
                                        "<MarginLeft>0.5in</MarginLeft>" +
                                        "<MarginRight>0.5in</MarginRight>" +
                                        "<MarginBottom>0.5in</MarginBottom>" +
                                        "</DeviceInfo>";
                            bytes = reportViewer.LocalReport.Render("PDF", deviceInfo, out mimeType, out encoding, out extension, out streamids, out warnings);
                            break;
                        case ".xlsx":
                            bytes = reportViewer.LocalReport.Render("EXCELOPENXML", deviceInfo, out mimeType, out encoding, out extension, out streamids, out warnings);
                            break;
                        case ".docx":
                            bytes = reportViewer.LocalReport.Render("WORDOPENXML", deviceInfo, out mimeType, out encoding, out extension, out streamids, out warnings);
                            break;
                    }
                    
                    if (bytes != null)
                    {
                        using (FileStream fs = new FileStream(saveDialog.FileName, FileMode.Create))
                        {
                            fs.Write(bytes, 0, bytes.Length);
                        }
                        
                        MessageBox.Show("Stock report exported successfully!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        
                        // Ask if user wants to open the file
                        if (MessageBox.Show("Do you want to open the exported file?", "Open File",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start(saveDialog.FileName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exporting stock report: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentStockData == null || currentStockData.Rows.Count == 0)
                {
                    MessageBox.Show("Please generate a stock report first.", "Information",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Switch to reports tab if not already there
                tabControl.SelectedTab = tabReports;
                
                // Use the ReportViewer's built-in print functionality
                reportViewer.PrintDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error printing stock report: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                LoadDropdownData();
                MessageBox.Show("Data refreshed successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error refreshing data: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    
}
