using System;
using System.Windows.Forms;
using RetailManagement.UserForms;

namespace RetailManagement
{
    /// <summary>
    /// Simple test form to demonstrate the new billing functionality
    /// This can be removed after integration into the main dashboard
    /// </summary>
    public partial class TestNewBillForm : Form
    {
        private Button btnOpenNewBill;
        private Button btnOpenSupplierMgmt;
        private Button btnOpenEnhancedBilling;
        private Label lblTitle;

        public TestNewBillForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.btnOpenNewBill = new Button();
            this.btnOpenSupplierMgmt = new Button();
            this.btnOpenEnhancedBilling = new Button();
            this.lblTitle = new Label();
            this.SuspendLayout();

            // Form
            this.Text = "Test New Forms";
            this.Size = new System.Drawing.Size(500, 300);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Title
            this.lblTitle.Text = "🧪 Test New Enhanced Forms";
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.Navy;
            this.lblTitle.Location = new System.Drawing.Point(50, 30);
            this.lblTitle.Size = new System.Drawing.Size(400, 30);
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // New Bill Form Button
            this.btnOpenNewBill.Text = "🧾 Open New Bill Form\n(Exact UI like Purchase)";
            this.btnOpenNewBill.Location = new System.Drawing.Point(50, 80);
            this.btnOpenNewBill.Size = new System.Drawing.Size(180, 60);
            this.btnOpenNewBill.BackColor = System.Drawing.Color.FromArgb(40, 167, 69);
            this.btnOpenNewBill.ForeColor = System.Drawing.Color.White;
            this.btnOpenNewBill.FlatStyle = FlatStyle.Flat;
            this.btnOpenNewBill.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            this.btnOpenNewBill.Click += BtnOpenNewBill_Click;

            // Enhanced Billing Form Button
            this.btnOpenEnhancedBilling.Text = "💳 Open Enhanced Billing\n(Modern UI with Barcode)";
            this.btnOpenEnhancedBilling.Location = new System.Drawing.Point(250, 80);
            this.btnOpenEnhancedBilling.Size = new System.Drawing.Size(180, 60);
            this.btnOpenEnhancedBilling.BackColor = System.Drawing.Color.FromArgb(0, 123, 255);
            this.btnOpenEnhancedBilling.ForeColor = System.Drawing.Color.White;
            this.btnOpenEnhancedBilling.FlatStyle = FlatStyle.Flat;
            this.btnOpenEnhancedBilling.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            this.btnOpenEnhancedBilling.Click += BtnOpenEnhancedBilling_Click;

            // Supplier Management Button
            this.btnOpenSupplierMgmt.Text = "🏢 Open Supplier Management\n(With Balance Tracking)";
            this.btnOpenSupplierMgmt.Location = new System.Drawing.Point(150, 160);
            this.btnOpenSupplierMgmt.Size = new System.Drawing.Size(180, 60);
            this.btnOpenSupplierMgmt.BackColor = System.Drawing.Color.FromArgb(255, 193, 7);
            this.btnOpenSupplierMgmt.ForeColor = System.Drawing.Color.Black;
            this.btnOpenSupplierMgmt.FlatStyle = FlatStyle.Flat;
            this.btnOpenSupplierMgmt.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            this.btnOpenSupplierMgmt.Click += BtnOpenSupplierMgmt_Click;

            // Add controls to form
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.btnOpenNewBill);
            this.Controls.Add(this.btnOpenEnhancedBilling);
            this.Controls.Add(this.btnOpenSupplierMgmt);

            this.ResumeLayout(false);
        }

        private void BtnOpenNewBill_Click(object sender, EventArgs e)
        {
            try
            {
                NewBillForm newBillForm = new NewBillForm();
                newBillForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening New Bill Form: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnOpenEnhancedBilling_Click(object sender, EventArgs e)
        {
            try
            {
                EnhancedBillingForm enhancedBillingForm = new EnhancedBillingForm();
                enhancedBillingForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening Enhanced Billing Form: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnOpenSupplierMgmt_Click(object sender, EventArgs e)
        {
            try
            {
                SupplierManagementForm supplierMgmtForm = new SupplierManagementForm();
                supplierMgmtForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening Supplier Management Form: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
