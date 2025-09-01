using System;
using System.Windows.Forms;

namespace RetailManagement.UserForms
{
    public partial class EditBillTypeSelector : Form
    {
        public string SelectedBillType { get; private set; }
        public string InvoiceNumber { get; private set; }

        private ComboBox cmbBillType;
        private TextBox txtInvoiceNumber;
        private Button btnOK;
        private Button btnCancel;
        private Label lblBillType;
        private Label lblInvoiceNumber;

        public EditBillTypeSelector()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.cmbBillType = new ComboBox();
            this.txtInvoiceNumber = new TextBox();
            this.btnOK = new Button();
            this.btnCancel = new Button();
            this.lblBillType = new Label();
            this.lblInvoiceNumber = new Label();
            this.SuspendLayout();

            // Form
            this.Text = "Select Bill Type to Edit";
            this.Size = new System.Drawing.Size(400, 200);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // lblBillType
            this.lblBillType.AutoSize = true;
            this.lblBillType.Location = new System.Drawing.Point(30, 30);
            this.lblBillType.Name = "lblBillType";
            this.lblBillType.Size = new System.Drawing.Size(100, 15);
            this.lblBillType.Text = "Select Bill Type:";

            // cmbBillType
            this.cmbBillType.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbBillType.Location = new System.Drawing.Point(150, 27);
            this.cmbBillType.Name = "cmbBillType";
            this.cmbBillType.Size = new System.Drawing.Size(200, 23);
            this.cmbBillType.Items.AddRange(new object[] {
                "New Bill (Sales)",
                "New Purchase",
                "Credit Bill"
            });
            this.cmbBillType.SelectedIndex = 0;

            // lblInvoiceNumber
            this.lblInvoiceNumber.AutoSize = true;
            this.lblInvoiceNumber.Location = new System.Drawing.Point(30, 70);
            this.lblInvoiceNumber.Name = "lblInvoiceNumber";
            this.lblInvoiceNumber.Size = new System.Drawing.Size(100, 15);
            this.lblInvoiceNumber.Text = "Invoice Number:";

            // txtInvoiceNumber
            this.txtInvoiceNumber.Location = new System.Drawing.Point(150, 67);
            this.txtInvoiceNumber.Name = "txtInvoiceNumber";
            this.txtInvoiceNumber.Size = new System.Drawing.Size(200, 23);
            // Note: PlaceholderText not available in .NET Framework

            // btnOK
            this.btnOK.Location = new System.Drawing.Point(150, 110);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new EventHandler(this.btnOK_Click);

            // btnCancel
            this.btnCancel.Location = new System.Drawing.Point(250, 110);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);

            // Add controls
            this.Controls.Add(this.lblBillType);
            this.Controls.Add(this.cmbBillType);
            this.Controls.Add(this.lblInvoiceNumber);
            this.Controls.Add(this.txtInvoiceNumber);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtInvoiceNumber.Text))
            {
                MessageBox.Show("Please enter an invoice number.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SelectedBillType = cmbBillType.SelectedItem.ToString();
            InvoiceNumber = txtInvoiceNumber.Text.Trim().ToUpper();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
