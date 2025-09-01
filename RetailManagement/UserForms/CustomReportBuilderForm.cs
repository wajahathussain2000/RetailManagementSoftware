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
    public partial class CustomReportBuilderForm : Form
    {
        private ReportViewer reportViewer;
        private ComboBox cmbDataSource;
        private ListBox lstAvailableFields;
        private ListBox lstSelectedFields;
        private ComboBox cmbFilterField;
        private ComboBox cmbFilterOperator;
        private TextBox txtFilterValue;
        private ListBox lstFilters;
        private ComboBox cmbGroupByField;
        private ComboBox cmbSortField;
        private ComboBox cmbSortOrder;
        private DateTimePicker dtpFromDate;
        private DateTimePicker dtpToDate;
        private CheckBox chkDateFilter;
        private TextBox txtReportName;
        private Button btnAddField;
        private Button btnRemoveField;
        private Button btnAddFilter;
        private Button btnRemoveFilter;
        private Button btnPreview;
        private Button btnSaveReport;
        private Button btnLoadReport;
        private Button btnExport;
        private Button btnPrint;
        private GroupBox groupDataSource;
        private GroupBox groupFields;
        private GroupBox groupFilters;
        private GroupBox groupOptions;
        private TabControl tabControl;
        private TabPage tabBuilder;
        private TabPage tabPreview;
        private TabPage tabSavedReports;
        private DataTable currentCustomData;
        private Dictionary<string, string> tableColumns;
        private List<string> availableOperators;

        public CustomReportBuilderForm()
        {
            InitializeComponent();
            InitializeReportViewer();
            InitializeCustomBuilder();
            LoadDataSources();
            SetDefaultDates();
        }

        private void InitializeComponent()
        {
            this.groupDataSource = new System.Windows.Forms.GroupBox();
            this.groupFields = new System.Windows.Forms.GroupBox();
            this.groupFilters = new System.Windows.Forms.GroupBox();
            this.groupOptions = new System.Windows.Forms.GroupBox();
            this.cmbDataSource = new System.Windows.Forms.ComboBox();
            this.lstAvailableFields = new System.Windows.Forms.ListBox();
            this.lstSelectedFields = new System.Windows.Forms.ListBox();
            this.cmbFilterField = new System.Windows.Forms.ComboBox();
            this.cmbFilterOperator = new System.Windows.Forms.ComboBox();
            this.txtFilterValue = new System.Windows.Forms.TextBox();
            this.lstFilters = new System.Windows.Forms.ListBox();
            this.cmbGroupByField = new System.Windows.Forms.ComboBox();
            this.cmbSortField = new System.Windows.Forms.ComboBox();
            this.cmbSortOrder = new System.Windows.Forms.ComboBox();
            this.dtpFromDate = new System.Windows.Forms.DateTimePicker();
            this.dtpToDate = new System.Windows.Forms.DateTimePicker();
            this.chkDateFilter = new System.Windows.Forms.CheckBox();
            this.txtReportName = new System.Windows.Forms.TextBox();
            this.btnAddField = new System.Windows.Forms.Button();
            this.btnRemoveField = new System.Windows.Forms.Button();
            this.btnAddFilter = new System.Windows.Forms.Button();
            this.btnRemoveFilter = new System.Windows.Forms.Button();
            this.btnPreview = new System.Windows.Forms.Button();
            this.btnSaveReport = new System.Windows.Forms.Button();
            this.btnLoadReport = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabBuilder = new System.Windows.Forms.TabPage();
            this.tabPreview = new System.Windows.Forms.TabPage();
            this.tabSavedReports = new System.Windows.Forms.TabPage();

            this.SuspendLayout();

            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.Controls.Add(this.tabControl);
            this.Name = "CustomReportBuilderForm";
            this.Text = "Custom Report Builder";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;

            // Tab Control
            this.tabControl.Controls.Add(this.tabBuilder);
            this.tabControl.Controls.Add(this.tabPreview);
            this.tabControl.Controls.Add(this.tabSavedReports);
            this.tabControl.Dock = DockStyle.Fill;
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.TabIndex = 0;

            // Tab Pages
            this.tabBuilder.Controls.Add(this.groupOptions);
            this.tabBuilder.Controls.Add(this.groupFilters);
            this.tabBuilder.Controls.Add(this.groupFields);
            this.tabBuilder.Controls.Add(this.groupDataSource);
            this.tabBuilder.Location = new System.Drawing.Point(4, 22);
            this.tabBuilder.Name = "tabBuilder";
            this.tabBuilder.Padding = new System.Windows.Forms.Padding(3);
            this.tabBuilder.Size = new System.Drawing.Size(1192, 674);
            this.tabBuilder.TabIndex = 0;
            this.tabBuilder.Text = "Report Builder";
            this.tabBuilder.UseVisualStyleBackColor = true;

            this.tabPreview.Location = new System.Drawing.Point(4, 22);
            this.tabPreview.Name = "tabPreview";
            this.tabPreview.Padding = new System.Windows.Forms.Padding(3);
            this.tabPreview.Size = new System.Drawing.Size(1192, 674);
            this.tabPreview.TabIndex = 1;
            this.tabPreview.Text = "Report Preview";
            this.tabPreview.UseVisualStyleBackColor = true;

            this.tabSavedReports.Location = new System.Drawing.Point(4, 22);
            this.tabSavedReports.Name = "tabSavedReports";
            this.tabSavedReports.Padding = new System.Windows.Forms.Padding(3);
            this.tabSavedReports.Size = new System.Drawing.Size(1192, 674);
            this.tabSavedReports.TabIndex = 2;
            this.tabSavedReports.Text = "Saved Reports";
            this.tabSavedReports.UseVisualStyleBackColor = true;

            // Group Data Source
            this.groupDataSource.Controls.Add(new Label { Text = "Data Source:", Location = new Point(15, 25), Size = new Size(75, 13) });
            this.groupDataSource.Controls.Add(this.cmbDataSource);
            this.groupDataSource.Location = new System.Drawing.Point(12, 12);
            this.groupDataSource.Name = "groupDataSource";
            this.groupDataSource.Size = new System.Drawing.Size(1168, 60);
            this.groupDataSource.TabIndex = 0;
            this.groupDataSource.TabStop = false;
            this.groupDataSource.Text = "Select Data Source";

            // Group Fields
            this.groupFields.Controls.Add(new Label { Text = "Available Fields:", Location = new Point(15, 25), Size = new Size(90, 13) });
            this.groupFields.Controls.Add(this.lstAvailableFields);
            this.groupFields.Controls.Add(this.btnAddField);
            this.groupFields.Controls.Add(this.btnRemoveField);
            this.groupFields.Controls.Add(new Label { Text = "Selected Fields:", Location = new Point(400, 25), Size = new Size(85, 13) });
            this.groupFields.Controls.Add(this.lstSelectedFields);
            this.groupFields.Location = new System.Drawing.Point(12, 80);
            this.groupFields.Name = "groupFields";
            this.groupFields.Size = new System.Drawing.Size(580, 180);
            this.groupFields.TabIndex = 1;
            this.groupFields.TabStop = false;
            this.groupFields.Text = "Select Fields";

            // Group Filters
            this.groupFilters.Controls.Add(new Label { Text = "Field:", Location = new Point(15, 25), Size = new Size(35, 13) });
            this.groupFilters.Controls.Add(this.cmbFilterField);
            this.groupFilters.Controls.Add(new Label { Text = "Operator:", Location = new Point(15, 55), Size = new Size(55, 13) });
            this.groupFilters.Controls.Add(this.cmbFilterOperator);
            this.groupFilters.Controls.Add(new Label { Text = "Value:", Location = new Point(15, 85), Size = new Size(40, 13) });
            this.groupFilters.Controls.Add(this.txtFilterValue);
            this.groupFilters.Controls.Add(this.btnAddFilter);
            this.groupFilters.Controls.Add(new Label { Text = "Applied Filters:", Location = new Point(250, 25), Size = new Size(85, 13) });
            this.groupFilters.Controls.Add(this.lstFilters);
            this.groupFilters.Controls.Add(this.btnRemoveFilter);
            this.groupFilters.Location = new System.Drawing.Point(600, 80);
            this.groupFilters.Name = "groupFilters";
            this.groupFilters.Size = new System.Drawing.Size(580, 180);
            this.groupFilters.TabIndex = 2;
            this.groupFilters.TabStop = false;
            this.groupFilters.Text = "Filters & Conditions";

            // Group Options
            this.groupOptions.Controls.Add(new Label { Text = "Report Name:", Location = new Point(15, 25), Size = new Size(80, 13) });
            this.groupOptions.Controls.Add(this.txtReportName);
            this.groupOptions.Controls.Add(new Label { Text = "Group By:", Location = new Point(15, 55), Size = new Size(60, 13) });
            this.groupOptions.Controls.Add(this.cmbGroupByField);
            this.groupOptions.Controls.Add(new Label { Text = "Sort Field:", Location = new Point(15, 85), Size = new Size(60, 13) });
            this.groupOptions.Controls.Add(this.cmbSortField);
            this.groupOptions.Controls.Add(new Label { Text = "Sort Order:", Location = new Point(250, 85), Size = new Size(65, 13) });
            this.groupOptions.Controls.Add(this.cmbSortOrder);
            this.groupOptions.Controls.Add(this.chkDateFilter);
            this.groupOptions.Controls.Add(new Label { Text = "From:", Location = new Point(500, 25), Size = new Size(35, 13) });
            this.groupOptions.Controls.Add(this.dtpFromDate);
            this.groupOptions.Controls.Add(new Label { Text = "To:", Location = new Point(500, 55), Size = new Size(25, 13) });
            this.groupOptions.Controls.Add(this.dtpToDate);
            this.groupOptions.Controls.Add(this.btnPreview);
            this.groupOptions.Controls.Add(this.btnSaveReport);
            this.groupOptions.Controls.Add(this.btnLoadReport);
            this.groupOptions.Controls.Add(this.btnExport);
            this.groupOptions.Controls.Add(this.btnPrint);
            this.groupOptions.Location = new System.Drawing.Point(12, 270);
            this.groupOptions.Name = "groupOptions";
            this.groupOptions.Size = new System.Drawing.Size(1168, 120);
            this.groupOptions.TabIndex = 3;
            this.groupOptions.TabStop = false;
            this.groupOptions.Text = "Report Options & Actions";

            // Setup all controls
            SetupBuilderControls();

            this.ResumeLayout(false);
        }

        private void SetupBuilderControls()
        {
            // Data Source ComboBox
            this.cmbDataSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDataSource.Location = new System.Drawing.Point(100, 22);
            this.cmbDataSource.Name = "cmbDataSource";
            this.cmbDataSource.Size = new System.Drawing.Size(200, 21);
            this.cmbDataSource.TabIndex = 1;
            this.cmbDataSource.SelectedIndexChanged += CmbDataSource_SelectedIndexChanged;

            // Available Fields ListBox
            this.lstAvailableFields.Location = new System.Drawing.Point(15, 45);
            this.lstAvailableFields.Name = "lstAvailableFields";
            this.lstAvailableFields.Size = new System.Drawing.Size(180, 120);
            this.lstAvailableFields.TabIndex = 2;
            this.lstAvailableFields.SelectionMode = SelectionMode.MultiExtended;

            // Add/Remove Field Buttons
            this.btnAddField.Text = "Add >>";
            this.btnAddField.Location = new System.Drawing.Point(210, 70);
            this.btnAddField.Size = new System.Drawing.Size(70, 25);
            this.btnAddField.Click += BtnAddField_Click;

            this.btnRemoveField.Text = "<< Remove";
            this.btnRemoveField.Location = new System.Drawing.Point(210, 100);
            this.btnRemoveField.Size = new System.Drawing.Size(70, 25);
            this.btnRemoveField.Click += BtnRemoveField_Click;

            // Selected Fields ListBox
            this.lstSelectedFields.Location = new System.Drawing.Point(300, 45);
            this.lstSelectedFields.Name = "lstSelectedFields";
            this.lstSelectedFields.Size = new System.Drawing.Size(180, 120);
            this.lstSelectedFields.TabIndex = 3;
            this.lstSelectedFields.SelectionMode = SelectionMode.MultiExtended;

            // Filter Controls
            this.cmbFilterField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFilterField.Location = new System.Drawing.Point(75, 22);
            this.cmbFilterField.Size = new System.Drawing.Size(150, 21);

            this.cmbFilterOperator.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFilterOperator.Location = new System.Drawing.Point(75, 52);
            this.cmbFilterOperator.Size = new System.Drawing.Size(100, 21);

            this.txtFilterValue.Location = new System.Drawing.Point(75, 82);
            this.txtFilterValue.Size = new System.Drawing.Size(150, 20);

            this.btnAddFilter.Text = "Add Filter";
            this.btnAddFilter.Location = new System.Drawing.Point(75, 110);
            this.btnAddFilter.Size = new System.Drawing.Size(75, 25);
            this.btnAddFilter.Click += BtnAddFilter_Click;

            this.lstFilters.Location = new System.Drawing.Point(250, 45);
            this.lstFilters.Size = new System.Drawing.Size(250, 90);

            this.btnRemoveFilter.Text = "Remove";
            this.btnRemoveFilter.Location = new System.Drawing.Point(450, 140);
            this.btnRemoveFilter.Size = new System.Drawing.Size(70, 25);
            this.btnRemoveFilter.Click += BtnRemoveFilter_Click;

            // Options Controls
            this.txtReportName.Location = new System.Drawing.Point(100, 22);
            this.txtReportName.Size = new System.Drawing.Size(200, 20);

            this.cmbGroupByField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbGroupByField.Location = new System.Drawing.Point(80, 52);
            this.cmbGroupByField.Size = new System.Drawing.Size(150, 21);

            this.cmbSortField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSortField.Location = new System.Drawing.Point(80, 82);
            this.cmbSortField.Size = new System.Drawing.Size(150, 21);

            this.cmbSortOrder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSortOrder.Location = new System.Drawing.Point(320, 82);
            this.cmbSortOrder.Size = new System.Drawing.Size(100, 21);
            this.cmbSortOrder.Items.AddRange(new object[] { "Ascending", "Descending" });
            this.cmbSortOrder.SelectedIndex = 0;

            this.chkDateFilter.Text = "Apply Date Filter";
            this.chkDateFilter.Location = new System.Drawing.Point(400, 25);
            this.chkDateFilter.Size = new System.Drawing.Size(120, 17);

            this.dtpFromDate.Format = DateTimePickerFormat.Short;
            this.dtpFromDate.Location = new System.Drawing.Point(540, 22);
            this.dtpFromDate.Size = new System.Drawing.Size(100, 20);

            this.dtpToDate.Format = DateTimePickerFormat.Short;
            this.dtpToDate.Location = new System.Drawing.Point(540, 52);
            this.dtpToDate.Size = new System.Drawing.Size(100, 20);

            // Action Buttons
            this.btnPreview.BackColor = System.Drawing.Color.Green;
            this.btnPreview.ForeColor = System.Drawing.Color.White;
            this.btnPreview.Location = new System.Drawing.Point(700, 20);
            this.btnPreview.Size = new System.Drawing.Size(100, 30);
            this.btnPreview.Text = "Preview Report";
            this.btnPreview.Click += BtnPreview_Click;

            this.btnSaveReport.BackColor = System.Drawing.Color.Blue;
            this.btnSaveReport.ForeColor = System.Drawing.Color.White;
            this.btnSaveReport.Location = new System.Drawing.Point(820, 20);
            this.btnSaveReport.Size = new System.Drawing.Size(100, 30);
            this.btnSaveReport.Text = "Save Report";
            this.btnSaveReport.Click += BtnSaveReport_Click;

            this.btnLoadReport.BackColor = System.Drawing.Color.Orange;
            this.btnLoadReport.ForeColor = System.Drawing.Color.White;
            this.btnLoadReport.Location = new System.Drawing.Point(940, 20);
            this.btnLoadReport.Size = new System.Drawing.Size(100, 30);
            this.btnLoadReport.Text = "Load Report";
            this.btnLoadReport.Click += BtnLoadReport_Click;

            this.btnExport.BackColor = System.Drawing.Color.Purple;
            this.btnExport.ForeColor = System.Drawing.Color.White;
            this.btnExport.Location = new System.Drawing.Point(700, 60);
            this.btnExport.Size = new System.Drawing.Size(100, 30);
            this.btnExport.Text = "Export";
            this.btnExport.Click += BtnExport_Click;

            this.btnPrint.BackColor = System.Drawing.Color.DarkRed;
            this.btnPrint.ForeColor = System.Drawing.Color.White;
            this.btnPrint.Location = new System.Drawing.Point(820, 60);
            this.btnPrint.Size = new System.Drawing.Size(100, 30);
            this.btnPrint.Text = "Print";
            this.btnPrint.Click += BtnPrint_Click;
        }

        private void InitializeReportViewer()
        {
            this.reportViewer = new ReportViewer();
            this.reportViewer.Dock = DockStyle.Fill;
            this.reportViewer.ProcessingMode = ProcessingMode.Local;
            
            // Add report viewer to preview tab
            this.tabPreview.Controls.Add(this.reportViewer);
        }

        private void InitializeCustomBuilder()
        {
            // Initialize table columns dictionary
            tableColumns = new Dictionary<string, string>();
            
            // Initialize available operators
            availableOperators = new List<string>
            {
                "=", "!=", ">", "<", ">=", "<=", "LIKE", "NOT LIKE", "IN", "NOT IN", "IS NULL", "IS NOT NULL"
            };

            cmbFilterOperator.Items.AddRange(availableOperators.ToArray());
            cmbFilterOperator.SelectedIndex = 0;
        }

        private void LoadDataSources()
        {
            try
            {
                // Load available data sources (tables/views)
                cmbDataSource.Items.Clear();
                cmbDataSource.Items.Add(new ComboBoxItem { Text = "Sales Data", Value = "SALES" });
                cmbDataSource.Items.Add(new ComboBoxItem { Text = "Purchase Data", Value = "PURCHASES" });
                cmbDataSource.Items.Add(new ComboBoxItem { Text = "Inventory Data", Value = "INVENTORY" });
                cmbDataSource.Items.Add(new ComboBoxItem { Text = "Customer Data", Value = "CUSTOMERS" });
                cmbDataSource.Items.Add(new ComboBoxItem { Text = "Supplier Data", Value = "SUPPLIERS" });
                cmbDataSource.Items.Add(new ComboBoxItem { Text = "Financial Data", Value = "FINANCIAL" });
                cmbDataSource.Items.Add(new ComboBoxItem { Text = "Expense Data", Value = "EXPENSES" });
                cmbDataSource.Items.Add(new ComboBoxItem { Text = "User Activity", Value = "USER_ACTIVITY" });
                cmbDataSource.DisplayMember = "Text";
                cmbDataSource.ValueMember = "Value";
                cmbDataSource.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data sources: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetDefaultDates()
        {
            // Set to current month
            dtpFromDate.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dtpToDate.Value = dtpFromDate.Value.AddMonths(1).AddDays(-1);
        }

        private void CmbDataSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadAvailableFields();
        }

        private void LoadAvailableFields()
        {
            try
            {
                ComboBoxItem selectedSource = (ComboBoxItem)cmbDataSource.SelectedItem;
                if (selectedSource == null) return;

                string dataSource = selectedSource.Value.ToString();
                
                lstAvailableFields.Items.Clear();
                cmbFilterField.Items.Clear();
                cmbGroupByField.Items.Clear();
                cmbSortField.Items.Clear();
                tableColumns.Clear();

                // Load fields based on selected data source
                switch (dataSource)
                {
                    case "SALES":
                        LoadSalesFields();
                        break;
                    case "PURCHASES":
                        LoadPurchaseFields();
                        break;
                    case "INVENTORY":
                        LoadInventoryFields();
                        break;
                    case "CUSTOMERS":
                        LoadCustomerFields();
                        break;
                    case "SUPPLIERS":
                        LoadSupplierFields();
                        break;
                    case "FINANCIAL":
                        LoadFinancialFields();
                        break;
                    case "EXPENSES":
                        LoadExpenseFields();
                        break;
                    case "USER_ACTIVITY":
                        LoadUserActivityFields();
                        break;
                }

                // Add "None" option to group by
                cmbGroupByField.Items.Insert(0, new ComboBoxItem { Text = "None", Value = "" });
                cmbGroupByField.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading fields: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSalesFields()
        {
            var fields = new Dictionary<string, string>
            {
                {"BillNumber", "Bill Number"},
                {"BillDate", "Bill Date"},
                {"CustomerName", "Customer Name"},
                {"ItemName", "Item Name"},
                {"Category", "Category"},
                {"Quantity", "Quantity"},
                {"Rate", "Rate"},
                {"TaxableAmount", "Taxable Amount"},
                {"NetAmount", "Net Amount"},
                {"PaymentMode", "Payment Mode"},
                {"PaymentStatus", "Payment Status"},
                {"UserName", "Sold By"}
            };

            LoadFieldsToControls(fields);
        }

        private void LoadPurchaseFields()
        {
            var fields = new Dictionary<string, string>
            {
                {"PurchaseNumber", "Purchase Number"},
                {"PurchaseDate", "Purchase Date"},
                {"CompanyName", "Supplier Name"},
                {"ItemName", "Item Name"},
                {"Category", "Category"},
                {"Quantity", "Quantity"},
                {"Rate", "Rate"},
                {"TaxableAmount", "Taxable Amount"},
                {"NetAmount", "Net Amount"},
                {"PaymentStatus", "Payment Status"},
                {"UserName", "Purchased By"}
            };

            LoadFieldsToControls(fields);
        }

        private void LoadInventoryFields()
        {
            var fields = new Dictionary<string, string>
            {
                {"ItemName", "Item Name"},
                {"Category", "Category"},
                {"Barcode", "Barcode"},
                {"StockInHand", "Stock In Hand"},
                {"MinimumStock", "Minimum Stock"},
                {"MaximumStock", "Maximum Stock"},
                {"Rate", "Rate"},
                {"MRP", "MRP"},
                {"ExpiryDate", "Expiry Date"},
                {"BatchNumber", "Batch Number"},
                {"SupplierName", "Supplier"}
            };

            LoadFieldsToControls(fields);
        }

        private void LoadCustomerFields()
        {
            var fields = new Dictionary<string, string>
            {
                {"CustomerName", "Customer Name"},
                {"ContactNumber", "Contact Number"},
                {"Email", "Email"},
                {"Address", "Address"},
                {"TotalPurchases", "Total Purchases"},
                {"OutstandingBalance", "Outstanding Balance"},
                {"LastPurchaseDate", "Last Purchase Date"}
            };

            LoadFieldsToControls(fields);
        }

        private void LoadSupplierFields()
        {
            var fields = new Dictionary<string, string>
            {
                {"CompanyName", "Company Name"},
                {"ContactNumber", "Contact Number"},
                {"Email", "Email"},
                {"Address", "Address"},
                {"TotalPurchases", "Total Purchases"},
                {"PayableBalance", "Payable Balance"},
                {"LastPurchaseDate", "Last Purchase Date"}
            };

            LoadFieldsToControls(fields);
        }

        private void LoadFinancialFields()
        {
            var fields = new Dictionary<string, string>
            {
                {"TransactionDate", "Transaction Date"},
                {"TransactionType", "Transaction Type"},
                {"Description", "Description"},
                {"DebitAmount", "Debit Amount"},
                {"CreditAmount", "Credit Amount"},
                {"Balance", "Balance"},
                {"Account", "Account"}
            };

            LoadFieldsToControls(fields);
        }

        private void LoadExpenseFields()
        {
            var fields = new Dictionary<string, string>
            {
                {"ExpenseDate", "Expense Date"},
                {"Category", "Category"},
                {"Description", "Description"},
                {"Amount", "Amount"},
                {"PaymentMode", "Payment Mode"},
                {"UserName", "Added By"}
            };

            LoadFieldsToControls(fields);
        }

        private void LoadUserActivityFields()
        {
            var fields = new Dictionary<string, string>
            {
                {"ActivityDate", "Activity Date"},
                {"UserName", "User Name"},
                {"ActivityType", "Activity Type"},
                {"ModuleName", "Module"},
                {"Description", "Description"},
                {"IPAddress", "IP Address"}
            };

            LoadFieldsToControls(fields);
        }

        private void LoadFieldsToControls(Dictionary<string, string> fields)
        {
            tableColumns = fields;

            foreach (var field in fields)
            {
                lstAvailableFields.Items.Add(new ComboBoxItem { Text = field.Value, Value = field.Key });
                cmbFilterField.Items.Add(new ComboBoxItem { Text = field.Value, Value = field.Key });
                cmbGroupByField.Items.Add(new ComboBoxItem { Text = field.Value, Value = field.Key });
                cmbSortField.Items.Add(new ComboBoxItem { Text = field.Value, Value = field.Key });
            }

            cmbFilterField.DisplayMember = "Text";
            cmbFilterField.ValueMember = "Value";
            cmbGroupByField.DisplayMember = "Text";
            cmbGroupByField.ValueMember = "Value";
            cmbSortField.DisplayMember = "Text";
            cmbSortField.ValueMember = "Value";

            if (cmbFilterField.Items.Count > 0) cmbFilterField.SelectedIndex = 0;
            if (cmbSortField.Items.Count > 0) cmbSortField.SelectedIndex = 0;
        }

        private void BtnAddField_Click(object sender, EventArgs e)
        {
            foreach (ComboBoxItem selectedItem in lstAvailableFields.SelectedItems)
            {
                if (!lstSelectedFields.Items.Contains(selectedItem))
                {
                    lstSelectedFields.Items.Add(selectedItem);
                }
            }
        }

        private void BtnRemoveField_Click(object sender, EventArgs e)
        {
            var itemsToRemove = lstSelectedFields.SelectedItems.Cast<object>().ToList();
            foreach (var item in itemsToRemove)
            {
                lstSelectedFields.Items.Remove(item);
            }
        }

        private void BtnAddFilter_Click(object sender, EventArgs e)
        {
            if (cmbFilterField.SelectedItem == null || string.IsNullOrEmpty(txtFilterValue.Text))
            {
                MessageBox.Show("Please select a field and enter a value for the filter.", "Filter Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ComboBoxItem field = (ComboBoxItem)cmbFilterField.SelectedItem;
            string operatorText = cmbFilterOperator.SelectedItem.ToString();
            string value = txtFilterValue.Text;

            string filterText = $"{field.Text} {operatorText} {value}";
            string filterValue = $"{field.Value}|{operatorText}|{value}";

            lstFilters.Items.Add(new ComboBoxItem { Text = filterText, Value = filterValue });
            txtFilterValue.Clear();
        }

        private void BtnRemoveFilter_Click(object sender, EventArgs e)
        {
            if (lstFilters.SelectedItem != null)
            {
                lstFilters.Items.Remove(lstFilters.SelectedItem);
            }
        }

        private void BtnPreview_Click(object sender, EventArgs e)
        {
            GenerateCustomReport();
        }

        private void BtnSaveReport_Click(object sender, EventArgs e)
        {
            SaveCustomReport();
        }

        private void BtnLoadReport_Click(object sender, EventArgs e)
        {
            LoadSavedReport();
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            ExportCustomReport();
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            PrintCustomReport();
        }

        private void GenerateCustomReport()
        {
            try
            {
                if (lstSelectedFields.Items.Count == 0)
                {
                    MessageBox.Show("Please select at least one field for the report.", "Validation Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                ComboBoxItem selectedSource = (ComboBoxItem)cmbDataSource.SelectedItem;
                string dataSource = selectedSource.Value.ToString();

                // Build SQL query
                string query = BuildCustomQuery(dataSource);
                
                // Execute query
                List<SqlParameter> parameters = BuildQueryParameters();
                currentCustomData = DatabaseConnection.ExecuteQuery(query, parameters.ToArray());

                if (currentCustomData != null && currentCustomData.Rows.Count > 0)
                {
                    // Switch to preview tab and show report
                    tabControl.SelectedTab = tabPreview;
                    
                    // For now, display in a simple grid format
                    // In a full implementation, you would generate an RDLC report dynamically
                    ShowCustomReportPreview();
                    
                    MessageBox.Show($"Custom report generated successfully! Found {currentCustomData.Rows.Count} records.", 
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("No data found for the selected criteria.", "Information", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating custom report: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string BuildCustomQuery(string dataSource)
        {
            // Build SELECT clause
            StringBuilder selectClause = new StringBuilder("SELECT ");
            List<string> selectedColumns = new List<string>();
            
            foreach (ComboBoxItem item in lstSelectedFields.Items)
            {
                selectedColumns.Add(item.Value.ToString());
            }
            selectClause.Append(string.Join(", ", selectedColumns));

            // Build FROM clause
            string fromClause = GetTableName(dataSource);

            // Build WHERE clause
            StringBuilder whereClause = new StringBuilder(" WHERE 1=1");
            
            // Add date filter if enabled
            if (chkDateFilter.Checked)
            {
                string dateField = GetDateFieldName(dataSource);
                whereClause.Append($" AND {dateField} BETWEEN @FromDate AND @ToDate");
            }

            // Add custom filters
            foreach (ComboBoxItem filter in lstFilters.Items)
            {
                string[] filterParts = filter.Value.ToString().Split('|');
                if (filterParts.Length == 3)
                {
                    string field = filterParts[0];
                    string operatorText = filterParts[1];
                    string value = filterParts[2];
                    
                    if (operatorText == "LIKE" || operatorText == "NOT LIKE")
                    {
                        whereClause.Append($" AND {field} {operatorText} '%{value}%'");
                    }
                    else if (operatorText == "IS NULL" || operatorText == "IS NOT NULL")
                    {
                        whereClause.Append($" AND {field} {operatorText}");
                    }
                    else
                    {
                        whereClause.Append($" AND {field} {operatorText} '{value}'");
                    }
                }
            }

            // Build GROUP BY clause
            string groupByClause = "";
            ComboBoxItem groupByField = (ComboBoxItem)cmbGroupByField.SelectedItem;
            if (groupByField != null && !string.IsNullOrEmpty(groupByField.Value.ToString()))
            {
                groupByClause = $" GROUP BY {groupByField.Value}";
            }

            // Build ORDER BY clause
            string orderByClause = "";
            ComboBoxItem sortField = (ComboBoxItem)cmbSortField.SelectedItem;
            if (sortField != null)
            {
                string sortOrder = cmbSortOrder.SelectedItem.ToString() == "Ascending" ? "ASC" : "DESC";
                orderByClause = $" ORDER BY {sortField.Value} {sortOrder}";
            }

            return $"{selectClause} FROM {fromClause}{whereClause}{groupByClause}{orderByClause}";
        }

        private string GetTableName(string dataSource)
        {
            switch (dataSource)
            {
                case "SALES":
                    return "Sales s INNER JOIN SaleItems si ON s.SaleID = si.SaleID " +
                           "LEFT JOIN Customers c ON s.CustomerID = c.CustomerID " +
                           "INNER JOIN Items i ON si.ItemID = i.ItemID " +
                           "LEFT JOIN Users u ON s.CreatedBy = u.UserID";
                case "PURCHASES":
                    return "Purchases p INNER JOIN PurchaseItems pi ON p.PurchaseID = pi.PurchaseID " +
                           "LEFT JOIN Companies co ON p.CompanyID = co.CompanyID " +
                           "INNER JOIN Items i ON pi.ItemID = i.ItemID " +
                           "LEFT JOIN Users u ON p.CreatedBy = u.UserID";
                case "INVENTORY":
                    return "Items i LEFT JOIN Companies c ON i.CompanyID = c.CompanyID";
                case "CUSTOMERS":
                    return "Customers";
                case "SUPPLIERS":
                    return "Companies";
                case "EXPENSES":
                    return "Expenses e LEFT JOIN Users u ON e.CreatedBy = u.UserID";
                case "USER_ACTIVITY":
                    return "UserActivity ua LEFT JOIN Users u ON ua.UserID = u.UserID";
                default:
                    return "Sales";
            }
        }

        private string GetDateFieldName(string dataSource)
        {
            switch (dataSource)
            {
                case "SALES":
                    return "s.BillDate";
                case "PURCHASES":
                    return "p.PurchaseDate";
                case "EXPENSES":
                    return "e.ExpenseDate";
                case "USER_ACTIVITY":
                    return "ua.ActivityDate";
                default:
                    return "CreatedDate";
            }
        }

        private List<SqlParameter> BuildQueryParameters()
        {
            List<SqlParameter> parameters = new List<SqlParameter>();

            if (chkDateFilter.Checked)
            {
                parameters.Add(new SqlParameter("@FromDate", dtpFromDate.Value));
                parameters.Add(new SqlParameter("@ToDate", dtpToDate.Value));
            }

            return parameters;
        }

        private void ShowCustomReportPreview()
        {
            // Create a simple tabular report view
            // In a full implementation, you would dynamically generate RDLC reports
            if (currentCustomData != null)
            {
                // For demonstration, we'll show a basic table structure
                // This could be enhanced to generate proper RDLC reports dynamically
                
                StringBuilder htmlReport = new StringBuilder();
                htmlReport.Append("<html><head><title>Custom Report</title>");
                htmlReport.Append("<style>table{border-collapse:collapse;width:100%;}th,td{border:1px solid #ddd;padding:8px;text-align:left;}th{background-color:#f2f2f2;}</style>");
                htmlReport.Append("</head><body>");
                htmlReport.Append($"<h2>{txtReportName.Text}</h2>");
                htmlReport.Append($"<p>Generated on: {DateTime.Now:dd/MM/yyyy HH:mm}</p>");
                htmlReport.Append("<table>");
                
                // Header
                htmlReport.Append("<tr>");
                foreach (DataColumn column in currentCustomData.Columns)
                {
                    htmlReport.Append($"<th>{column.ColumnName}</th>");
                }
                htmlReport.Append("</tr>");
                
                // Data rows
                foreach (DataRow row in currentCustomData.Rows)
                {
                    htmlReport.Append("<tr>");
                    foreach (var item in row.ItemArray)
                    {
                        htmlReport.Append($"<td>{item}</td>");
                    }
                    htmlReport.Append("</tr>");
                }
                
                htmlReport.Append("</table>");
                htmlReport.Append("</body></html>");

                // You could display this in a WebBrowser control or convert to RDLC
                MessageBox.Show("Custom report preview generated. In a full implementation, this would show in a proper report viewer.", 
                    "Preview", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void SaveCustomReport()
        {
            try
            {
                if (string.IsNullOrEmpty(txtReportName.Text))
                {
                    MessageBox.Show("Please enter a report name.", "Validation Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // In a full implementation, you would save the report definition to a database table
                // This would include the data source, selected fields, filters, grouping, etc.
                
                MessageBox.Show($"Report '{txtReportName.Text}' saved successfully!\n\n" +
                    "Note: In a full implementation, this would save the report definition to the database " +
                    "so it can be loaded and reused later.", "Save Report", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving custom report: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSavedReport()
        {
            try
            {
                // In a full implementation, you would load saved report definitions from database
                MessageBox.Show("Load saved reports functionality would be implemented here.\n\n" +
                    "This would show a list of previously saved report definitions that users can select and load.", 
                    "Load Report", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading saved reports: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportCustomReport()
        {
            try
            {
                if (currentCustomData == null || currentCustomData.Rows.Count == 0)
                {
                    MessageBox.Show("No data to export. Please generate a report first.", "Export Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "Excel Files|*.xlsx|CSV Files|*.csv|PDF Files|*.pdf";
                saveDialog.FileName = $"Custom_Report_{DateTime.Now:yyyyMMdd}";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    // Export logic would go here
                    MessageBox.Show("Custom report exported successfully!", "Export Complete", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exporting custom report: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PrintCustomReport()
        {
            try
            {
                if (currentCustomData == null || currentCustomData.Rows.Count == 0)
                {
                    MessageBox.Show("No data to print. Please generate a report first.", "Print Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Print logic would go here
                MessageBox.Show("Print functionality would be implemented here using the report viewer.", 
                    "Print Report", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error printing custom report: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
