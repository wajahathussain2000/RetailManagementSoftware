namespace RetailManagement
{
    partial class UserDashboard
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Call our custom cleanup method
                CloseAllManagedForms();
                
                // Stop and dispose the timer
                if (dateTimeTimer != null)
                {
                    dateTimeTimer.Stop();
                    dateTimeTimer.Dispose();
                    dateTimeTimer = null;
                }
                
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnExit = new System.Windows.Forms.Button();
            this.btnStockInHand = new System.Windows.Forms.Button();
            this.btnSalesReport = new System.Windows.Forms.Button();
            this.btnCustomerPayment = new System.Windows.Forms.Button();
            this.btnCustomerLedger = new System.Windows.Forms.Button();
            this.btnCustomerBalance = new System.Windows.Forms.Button();
            this.btnSaleReturn = new System.Windows.Forms.Button();
            this.btnEditBill = new System.Windows.Forms.Button();
            this.btnCreditBill = new System.Windows.Forms.Button();
            this.btnNewBill = new System.Windows.Forms.Button();
            this.btnNewPurchase = new System.Windows.Forms.Button();
            this.btnItems = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.btnCompanies = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pbExit = new System.Windows.Forms.PictureBox();
            this.pbStockInHand = new System.Windows.Forms.PictureBox();
            this.pbSalesReport = new System.Windows.Forms.PictureBox();
            this.pbCustomerPayment = new System.Windows.Forms.PictureBox();
            this.pbCustomerLedger = new System.Windows.Forms.PictureBox();
            this.pbCustomerBalance = new System.Windows.Forms.PictureBox();
            this.pbSaleReturn = new System.Windows.Forms.PictureBox();
            this.pbEditBill = new System.Windows.Forms.PictureBox();
            this.pbCreditBill = new System.Windows.Forms.PictureBox();
            this.pbNewBill = new System.Windows.Forms.PictureBox();
            this.pbNewPurchase = new System.Windows.Forms.PictureBox();
            this.pbItems = new System.Windows.Forms.PictureBox();
            this.pbCompanies = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.setupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.customerManagementToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setupSupplierManagementToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.userManagementToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.changePasswordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.purchasesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newPurchaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.purchaseHistoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.purchaseReportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.supplierManagementToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.salesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.enhancedPOSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newBillToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editBillToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.printBillToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.salesReportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.creditSaleReporToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saleReturnsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.printSaleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.returnReportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.accountsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.customerAccountToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.customerPaymentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.customerLedgerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.customerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.customersBalancesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bankTransactionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bankLedgerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.suppliersPaymentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.supplierLedgerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.supplierToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.supplierBalancesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.expenseTransactionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.expenseReportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reportsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.comprehensiveReportsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateStockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stockInHandToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.genericSearchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.productSearchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.singleProductDetailToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.profitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.userActivityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.databaseBackupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.databaseRestoreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataExportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbExit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbStockInHand)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbSalesReport)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbCustomerPayment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbCustomerLedger)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbCustomerBalance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbSaleReturn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbEditBill)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbCreditBill)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbNewBill)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbNewPurchase)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbCompanies)).BeginInit();
            this.panel1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnExit
            // 
            this.btnExit.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExit.Location = new System.Drawing.Point(1827, 29);
            this.btnExit.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(142, 122);
            this.btnExit.TabIndex = 25;
            this.btnExit.Text = "Exit";
            this.btnExit.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnStockInHand
            // 
            this.btnStockInHand.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStockInHand.Location = new System.Drawing.Point(1676, 29);
            this.btnStockInHand.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnStockInHand.Name = "btnStockInHand";
            this.btnStockInHand.Size = new System.Drawing.Size(142, 122);
            this.btnStockInHand.TabIndex = 23;
            this.btnStockInHand.Text = "Stock In Hand";
            this.btnStockInHand.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnStockInHand.UseVisualStyleBackColor = true;
            this.btnStockInHand.Click += new System.EventHandler(this.btnStockInHand_Click);
            // 
            // btnSalesReport
            // 
            this.btnSalesReport.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSalesReport.Location = new System.Drawing.Point(1524, 29);
            this.btnSalesReport.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnSalesReport.Name = "btnSalesReport";
            this.btnSalesReport.Size = new System.Drawing.Size(142, 122);
            this.btnSalesReport.TabIndex = 21;
            this.btnSalesReport.Text = "Sales Report";
            this.btnSalesReport.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnSalesReport.UseVisualStyleBackColor = true;
            this.btnSalesReport.Click += new System.EventHandler(this.btnSalesReport_Click);
            // 
            // btnCustomerPayment
            // 
            this.btnCustomerPayment.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCustomerPayment.Location = new System.Drawing.Point(1372, 29);
            this.btnCustomerPayment.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnCustomerPayment.Name = "btnCustomerPayment";
            this.btnCustomerPayment.Size = new System.Drawing.Size(142, 122);
            this.btnCustomerPayment.TabIndex = 19;
            this.btnCustomerPayment.Text = "Customer Payment";
            this.btnCustomerPayment.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnCustomerPayment.UseVisualStyleBackColor = true;
            this.btnCustomerPayment.Click += new System.EventHandler(this.btnCustomerPayment_Click);
            // 
            // btnCustomerLedger
            // 
            this.btnCustomerLedger.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCustomerLedger.Location = new System.Drawing.Point(1221, 29);
            this.btnCustomerLedger.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnCustomerLedger.Name = "btnCustomerLedger";
            this.btnCustomerLedger.Size = new System.Drawing.Size(142, 122);
            this.btnCustomerLedger.TabIndex = 17;
            this.btnCustomerLedger.Text = "Customer Ledger";
            this.btnCustomerLedger.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnCustomerLedger.UseVisualStyleBackColor = true;
            this.btnCustomerLedger.Click += new System.EventHandler(this.btnCustomerLedger_Click);
            // 
            // btnCustomerBalance
            // 
            this.btnCustomerBalance.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCustomerBalance.Location = new System.Drawing.Point(1070, 29);
            this.btnCustomerBalance.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnCustomerBalance.Name = "btnCustomerBalance";
            this.btnCustomerBalance.Size = new System.Drawing.Size(142, 122);
            this.btnCustomerBalance.TabIndex = 15;
            this.btnCustomerBalance.Text = "Customer Balance";
            this.btnCustomerBalance.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnCustomerBalance.UseVisualStyleBackColor = true;
            this.btnCustomerBalance.Click += new System.EventHandler(this.btnCustomerBalance_Click);
            // 
            // btnSaleReturn
            // 
            this.btnSaleReturn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSaleReturn.Location = new System.Drawing.Point(918, 29);
            this.btnSaleReturn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnSaleReturn.Name = "btnSaleReturn";
            this.btnSaleReturn.Size = new System.Drawing.Size(142, 122);
            this.btnSaleReturn.TabIndex = 13;
            this.btnSaleReturn.Text = "Sale Return";
            this.btnSaleReturn.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnSaleReturn.UseVisualStyleBackColor = true;
            this.btnSaleReturn.Click += new System.EventHandler(this.btnSaleReturn_Click);
            // 
            // btnEditBill
            // 
            this.btnEditBill.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEditBill.Location = new System.Drawing.Point(766, 29);
            this.btnEditBill.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnEditBill.Name = "btnEditBill";
            this.btnEditBill.Size = new System.Drawing.Size(142, 122);
            this.btnEditBill.TabIndex = 11;
            this.btnEditBill.Text = "Edit Bill";
            this.btnEditBill.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnEditBill.UseVisualStyleBackColor = true;
            this.btnEditBill.Click += new System.EventHandler(this.btnEditBill_Click);
            // 
            // btnCreditBill
            // 
            this.btnCreditBill.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCreditBill.Location = new System.Drawing.Point(615, 29);
            this.btnCreditBill.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnCreditBill.Name = "btnCreditBill";
            this.btnCreditBill.Size = new System.Drawing.Size(142, 122);
            this.btnCreditBill.TabIndex = 9;
            this.btnCreditBill.Text = "Credit Bill";
            this.btnCreditBill.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnCreditBill.UseVisualStyleBackColor = true;
            this.btnCreditBill.Click += new System.EventHandler(this.btnCreditBill_Click);
            // 
            // btnNewBill
            // 
            this.btnNewBill.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNewBill.Location = new System.Drawing.Point(464, 29);
            this.btnNewBill.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnNewBill.Name = "btnNewBill";
            this.btnNewBill.Size = new System.Drawing.Size(142, 122);
            this.btnNewBill.TabIndex = 7;
            this.btnNewBill.Text = "New Bill";
            this.btnNewBill.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnNewBill.UseVisualStyleBackColor = true;
            this.btnNewBill.Click += new System.EventHandler(this.btnNewBill_Click);
            // 
            // btnNewPurchase
            // 
            this.btnNewPurchase.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNewPurchase.Location = new System.Drawing.Point(312, 29);
            this.btnNewPurchase.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnNewPurchase.Name = "btnNewPurchase";
            this.btnNewPurchase.Size = new System.Drawing.Size(142, 122);
            this.btnNewPurchase.TabIndex = 5;
            this.btnNewPurchase.Text = "New Purchase";
            this.btnNewPurchase.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnNewPurchase.UseVisualStyleBackColor = true;
            this.btnNewPurchase.Click += new System.EventHandler(this.btnNewPurchase_Click);
            // 
            // btnItems
            // 
            this.btnItems.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnItems.Location = new System.Drawing.Point(160, 29);
            this.btnItems.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnItems.Name = "btnItems";
            this.btnItems.Size = new System.Drawing.Size(142, 122);
            this.btnItems.TabIndex = 3;
            this.btnItems.Text = "Items";
            this.btnItems.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnItems.UseVisualStyleBackColor = true;
            this.btnItems.Click += new System.EventHandler(this.btnItems_Click);
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label4.Location = new System.Drawing.Point(808, 255);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(355, 37);
            this.label4.TabIndex = 1;
            this.label4.Text = "Sunday, July 26, 2025";
            // 
            // btnCompanies
            // 
            this.btnCompanies.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCompanies.Location = new System.Drawing.Point(9, 29);
            this.btnCompanies.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnCompanies.Name = "btnCompanies";
            this.btnCompanies.Size = new System.Drawing.Size(142, 122);
            this.btnCompanies.TabIndex = 1;
            this.btnCompanies.Text = "Companies";
            this.btnCompanies.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnCompanies.UseVisualStyleBackColor = true;
            this.btnCompanies.Click += new System.EventHandler(this.btnCompanies_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.pbExit);
            this.groupBox1.Controls.Add(this.btnExit);
            this.groupBox1.Controls.Add(this.pbStockInHand);
            this.groupBox1.Controls.Add(this.btnStockInHand);
            this.groupBox1.Controls.Add(this.pbSalesReport);
            this.groupBox1.Controls.Add(this.btnSalesReport);
            this.groupBox1.Controls.Add(this.pbCustomerPayment);
            this.groupBox1.Controls.Add(this.btnCustomerPayment);
            this.groupBox1.Controls.Add(this.pbCustomerLedger);
            this.groupBox1.Controls.Add(this.btnCustomerLedger);
            this.groupBox1.Controls.Add(this.pbCustomerBalance);
            this.groupBox1.Controls.Add(this.btnCustomerBalance);
            this.groupBox1.Controls.Add(this.pbSaleReturn);
            this.groupBox1.Controls.Add(this.btnSaleReturn);
            this.groupBox1.Controls.Add(this.pbEditBill);
            this.groupBox1.Controls.Add(this.btnEditBill);
            this.groupBox1.Controls.Add(this.pbCreditBill);
            this.groupBox1.Controls.Add(this.btnCreditBill);
            this.groupBox1.Controls.Add(this.pbNewBill);
            this.groupBox1.Controls.Add(this.btnNewBill);
            this.groupBox1.Controls.Add(this.pbNewPurchase);
            this.groupBox1.Controls.Add(this.btnNewPurchase);
            this.groupBox1.Controls.Add(this.pbItems);
            this.groupBox1.Controls.Add(this.btnItems);
            this.groupBox1.Controls.Add(this.pbCompanies);
            this.groupBox1.Controls.Add(this.btnCompanies);
            this.groupBox1.Location = new System.Drawing.Point(18, 22);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Size = new System.Drawing.Size(1992, 177);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // pbExit
            // 
            this.pbExit.BackColor = System.Drawing.Color.White;
            this.pbExit.Image = global::RetailManagement.Properties.Resources.cancel;
            this.pbExit.Location = new System.Drawing.Point(1866, 42);
            this.pbExit.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pbExit.Name = "pbExit";
            this.pbExit.Size = new System.Drawing.Size(66, 58);
            this.pbExit.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbExit.TabIndex = 24;
            this.pbExit.TabStop = false;
            this.pbExit.Click += new System.EventHandler(this.pbExit_Click);
            // 
            // pbStockInHand
            // 
            this.pbStockInHand.BackColor = System.Drawing.Color.White;
            this.pbStockInHand.Image = global::RetailManagement.Properties.Resources.stock_in_hand;
            this.pbStockInHand.Location = new System.Drawing.Point(1714, 42);
            this.pbStockInHand.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pbStockInHand.Name = "pbStockInHand";
            this.pbStockInHand.Size = new System.Drawing.Size(66, 58);
            this.pbStockInHand.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbStockInHand.TabIndex = 22;
            this.pbStockInHand.TabStop = false;
            this.pbStockInHand.Click += new System.EventHandler(this.pbStockInHand_Click);
            // 
            // pbSalesReport
            // 
            this.pbSalesReport.BackColor = System.Drawing.Color.White;
            this.pbSalesReport.Image = global::RetailManagement.Properties.Resources.items;
            this.pbSalesReport.Location = new System.Drawing.Point(1563, 42);
            this.pbSalesReport.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pbSalesReport.Name = "pbSalesReport";
            this.pbSalesReport.Size = new System.Drawing.Size(66, 58);
            this.pbSalesReport.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbSalesReport.TabIndex = 20;
            this.pbSalesReport.TabStop = false;
            this.pbSalesReport.Click += new System.EventHandler(this.pbSalesReport_Click);
            // 
            // pbCustomerPayment
            // 
            this.pbCustomerPayment.BackColor = System.Drawing.Color.White;
            this.pbCustomerPayment.Image = global::RetailManagement.Properties.Resources.customer_payment;
            this.pbCustomerPayment.Location = new System.Drawing.Point(1412, 42);
            this.pbCustomerPayment.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pbCustomerPayment.Name = "pbCustomerPayment";
            this.pbCustomerPayment.Size = new System.Drawing.Size(66, 58);
            this.pbCustomerPayment.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbCustomerPayment.TabIndex = 18;
            this.pbCustomerPayment.TabStop = false;
            this.pbCustomerPayment.Click += new System.EventHandler(this.pbCustomerPayment_Click);
            // 
            // pbCustomerLedger
            // 
            this.pbCustomerLedger.BackColor = System.Drawing.Color.White;
            this.pbCustomerLedger.Image = global::RetailManagement.Properties.Resources.customer_ledger;
            this.pbCustomerLedger.Location = new System.Drawing.Point(1260, 42);
            this.pbCustomerLedger.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pbCustomerLedger.Name = "pbCustomerLedger";
            this.pbCustomerLedger.Size = new System.Drawing.Size(66, 58);
            this.pbCustomerLedger.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbCustomerLedger.TabIndex = 16;
            this.pbCustomerLedger.TabStop = false;
            this.pbCustomerLedger.Click += new System.EventHandler(this.pbCustomerLedger_Click);
            // 
            // pbCustomerBalance
            // 
            this.pbCustomerBalance.BackColor = System.Drawing.Color.White;
            this.pbCustomerBalance.Image = global::RetailManagement.Properties.Resources.customer_balance;
            this.pbCustomerBalance.Location = new System.Drawing.Point(1108, 42);
            this.pbCustomerBalance.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pbCustomerBalance.Name = "pbCustomerBalance";
            this.pbCustomerBalance.Size = new System.Drawing.Size(66, 58);
            this.pbCustomerBalance.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbCustomerBalance.TabIndex = 14;
            this.pbCustomerBalance.TabStop = false;
            this.pbCustomerBalance.Click += new System.EventHandler(this.pbCustomerBalance_Click);
            // 
            // pbSaleReturn
            // 
            this.pbSaleReturn.BackColor = System.Drawing.Color.White;
            this.pbSaleReturn.Image = global::RetailManagement.Properties.Resources.sale_return;
            this.pbSaleReturn.Location = new System.Drawing.Point(957, 42);
            this.pbSaleReturn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pbSaleReturn.Name = "pbSaleReturn";
            this.pbSaleReturn.Size = new System.Drawing.Size(66, 58);
            this.pbSaleReturn.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbSaleReturn.TabIndex = 12;
            this.pbSaleReturn.TabStop = false;
            this.pbSaleReturn.Click += new System.EventHandler(this.pbSaleReturn_Click);
            // 
            // pbEditBill
            // 
            this.pbEditBill.BackColor = System.Drawing.Color.White;
            this.pbEditBill.Image = global::RetailManagement.Properties.Resources.edit_bill;
            this.pbEditBill.Location = new System.Drawing.Point(806, 42);
            this.pbEditBill.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pbEditBill.Name = "pbEditBill";
            this.pbEditBill.Size = new System.Drawing.Size(66, 58);
            this.pbEditBill.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbEditBill.TabIndex = 10;
            this.pbEditBill.TabStop = false;
            this.pbEditBill.Click += new System.EventHandler(this.pbEditBill_Click);
            // 
            // pbCreditBill
            // 
            this.pbCreditBill.BackColor = System.Drawing.Color.White;
            this.pbCreditBill.Image = global::RetailManagement.Properties.Resources.credit_bill;
            this.pbCreditBill.Location = new System.Drawing.Point(654, 42);
            this.pbCreditBill.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pbCreditBill.Name = "pbCreditBill";
            this.pbCreditBill.Size = new System.Drawing.Size(66, 58);
            this.pbCreditBill.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbCreditBill.TabIndex = 8;
            this.pbCreditBill.TabStop = false;
            this.pbCreditBill.Click += new System.EventHandler(this.pbCreditBill_Click);
            // 
            // pbNewBill
            // 
            this.pbNewBill.BackColor = System.Drawing.Color.White;
            this.pbNewBill.Image = global::RetailManagement.Properties.Resources.bill;
            this.pbNewBill.Location = new System.Drawing.Point(502, 42);
            this.pbNewBill.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pbNewBill.Name = "pbNewBill";
            this.pbNewBill.Size = new System.Drawing.Size(66, 58);
            this.pbNewBill.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbNewBill.TabIndex = 6;
            this.pbNewBill.TabStop = false;
            this.pbNewBill.Click += new System.EventHandler(this.pbNewBill_Click);
            // 
            // pbNewPurchase
            // 
            this.pbNewPurchase.BackColor = System.Drawing.Color.White;
            this.pbNewPurchase.Image = global::RetailManagement.Properties.Resources.purchase;
            this.pbNewPurchase.Location = new System.Drawing.Point(351, 42);
            this.pbNewPurchase.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pbNewPurchase.Name = "pbNewPurchase";
            this.pbNewPurchase.Size = new System.Drawing.Size(66, 58);
            this.pbNewPurchase.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbNewPurchase.TabIndex = 4;
            this.pbNewPurchase.TabStop = false;
            this.pbNewPurchase.Click += new System.EventHandler(this.pbNewPurchase_Click);
            // 
            // pbItems
            // 
            this.pbItems.BackColor = System.Drawing.Color.White;
            this.pbItems.Image = global::RetailManagement.Properties.Resources.items;
            this.pbItems.Location = new System.Drawing.Point(202, 42);
            this.pbItems.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pbItems.Name = "pbItems";
            this.pbItems.Size = new System.Drawing.Size(66, 58);
            this.pbItems.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbItems.TabIndex = 2;
            this.pbItems.TabStop = false;
            this.pbItems.Click += new System.EventHandler(this.pbItems_Click);
            // 
            // pbCompanies
            // 
            this.pbCompanies.BackColor = System.Drawing.Color.White;
            this.pbCompanies.Image = global::RetailManagement.Properties.Resources.office_building;
            this.pbCompanies.Location = new System.Drawing.Point(48, 42);
            this.pbCompanies.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pbCompanies.Name = "pbCompanies";
            this.pbCompanies.Size = new System.Drawing.Size(66, 58);
            this.pbCompanies.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbCompanies.TabIndex = 0;
            this.pbCompanies.TabStop = false;
            this.pbCompanies.Click += new System.EventHandler(this.pbCompanies_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.BackColor = System.Drawing.Color.SeaGreen;
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Location = new System.Drawing.Point(0, 286);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(2055, 546);
            this.panel1.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(885, 126);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(171, 20);
            this.label3.TabIndex = 7;
            this.label3.Text = "Cell: 0300-0600894";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(812, 94);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(326, 22);
            this.label2.TabIndex = 6;
            this.label2.Text = "Near Eid Gah Katchary Road A.P.E";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label1.AutoSize = true;
            this.label1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.SeaGreen;
            this.label1.Location = new System.Drawing.Point(789, 37);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(480, 47);
            this.label1.TabIndex = 5;
            this.label1.Text = "Aziz Hospital Pharmacy";
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.Info;
            this.menuStrip1.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menuStrip1.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setupToolStripMenuItem,
            this.purchasesToolStripMenuItem,
            this.salesToolStripMenuItem,
            this.accountsToolStripMenuItem,
            this.reportsToolStripMenuItem,
            this.backupToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(2055, 35);
            this.menuStrip1.TabIndex = 9;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // setupToolStripMenuItem
            // 
            this.setupToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.customerManagementToolStripMenuItem,
            this.setupSupplierManagementToolStripMenuItem,
            this.userManagementToolStripMenuItem,
            this.changePasswordToolStripMenuItem});
            this.setupToolStripMenuItem.Name = "setupToolStripMenuItem";
            this.setupToolStripMenuItem.Size = new System.Drawing.Size(77, 29);
            this.setupToolStripMenuItem.Text = "Setup";
            // 
            // customerManagementToolStripMenuItem
            // 
            this.customerManagementToolStripMenuItem.Name = "customerManagementToolStripMenuItem";
            this.customerManagementToolStripMenuItem.Size = new System.Drawing.Size(311, 34);
            this.customerManagementToolStripMenuItem.Text = "Customer Management";
            this.customerManagementToolStripMenuItem.Click += new System.EventHandler(this.customerManagementToolStripMenuItem_Click);
            // 
            // setupSupplierManagementToolStripMenuItem
            // 
            this.setupSupplierManagementToolStripMenuItem.Name = "setupSupplierManagementToolStripMenuItem";
            this.setupSupplierManagementToolStripMenuItem.Size = new System.Drawing.Size(311, 34);
            this.setupSupplierManagementToolStripMenuItem.Text = "Supplier Management";
            this.setupSupplierManagementToolStripMenuItem.Click += new System.EventHandler(this.setupSupplierManagementToolStripMenuItem_Click);
            // 
            // userManagementToolStripMenuItem
            // 
            this.userManagementToolStripMenuItem.Name = "userManagementToolStripMenuItem";
            this.userManagementToolStripMenuItem.Size = new System.Drawing.Size(311, 34);
            this.userManagementToolStripMenuItem.Text = "👤 User Management";
            this.userManagementToolStripMenuItem.Click += new System.EventHandler(this.userManagementToolStripMenuItem_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(403, 6);
            // 
            // changePasswordToolStripMenuItem
            // 
            this.changePasswordToolStripMenuItem.Name = "changePasswordToolStripMenuItem";
            this.changePasswordToolStripMenuItem.Size = new System.Drawing.Size(311, 34);
            this.changePasswordToolStripMenuItem.Text = "Change Password";
            this.changePasswordToolStripMenuItem.Click += new System.EventHandler(this.changePasswordToolStripMenuItem_Click);
            // 
            // purchasesToolStripMenuItem
            // 
            this.purchasesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newPurchaseToolStripMenuItem,
            this.purchaseHistoryToolStripMenuItem,
            this.purchaseReportToolStripMenuItem,
            this.toolStripSeparator9,
            this.supplierManagementToolStripMenuItem});
            this.purchasesToolStripMenuItem.Name = "purchasesToolStripMenuItem";
            this.purchasesToolStripMenuItem.Size = new System.Drawing.Size(110, 29);
            this.purchasesToolStripMenuItem.Text = "Purchases";
            // 
            // newPurchaseToolStripMenuItem
            // 
            this.newPurchaseToolStripMenuItem.Name = "newPurchaseToolStripMenuItem";
            this.newPurchaseToolStripMenuItem.Size = new System.Drawing.Size(300, 34);
            this.newPurchaseToolStripMenuItem.Text = "New Purchase";
            this.newPurchaseToolStripMenuItem.Click += new System.EventHandler(this.newPurchaseToolStripMenuItem_Click);
            // 
            // purchaseHistoryToolStripMenuItem
            // 
            this.purchaseHistoryToolStripMenuItem.Name = "purchaseHistoryToolStripMenuItem";
            this.purchaseHistoryToolStripMenuItem.Size = new System.Drawing.Size(300, 34);
            this.purchaseHistoryToolStripMenuItem.Text = "Purchase History";
            this.purchaseHistoryToolStripMenuItem.Click += new System.EventHandler(this.purchaseHistoryToolStripMenuItem_Click);
            // 
            // purchaseReportToolStripMenuItem
            // 
            this.purchaseReportToolStripMenuItem.Name = "purchaseReportToolStripMenuItem";
            this.purchaseReportToolStripMenuItem.Size = new System.Drawing.Size(300, 34);
            this.purchaseReportToolStripMenuItem.Text = "Purchase Report";
            this.purchaseReportToolStripMenuItem.Click += new System.EventHandler(this.purchaseReportToolStripMenuItem_Click);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(297, 6);
            // 
            // supplierManagementToolStripMenuItem
            // 
            this.supplierManagementToolStripMenuItem.Name = "supplierManagementToolStripMenuItem";
            this.supplierManagementToolStripMenuItem.Size = new System.Drawing.Size(300, 34);
            this.supplierManagementToolStripMenuItem.Text = "Supplier Management";
            this.supplierManagementToolStripMenuItem.Click += new System.EventHandler(this.supplierManagementToolStripMenuItem_Click);
            // 
            // salesToolStripMenuItem
            // 
            this.salesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.enhancedPOSToolStripMenuItem,
            this.newBillToolStripMenuItem,
            this.editBillToolStripMenuItem,
            this.toolStripSeparator4,
            this.printBillToolStripMenuItem,
            this.salesReportToolStripMenuItem,
            this.toolStripSeparator5,
            this.creditSaleReporToolStripMenuItem,
            this.saleReturnsToolStripMenuItem,
            this.printSaleToolStripMenuItem,
            this.returnReportToolStripMenuItem});
            this.salesToolStripMenuItem.Name = "salesToolStripMenuItem";
            this.salesToolStripMenuItem.Size = new System.Drawing.Size(70, 29);
            this.salesToolStripMenuItem.Text = "Sales";
            // 
            // enhancedPOSToolStripMenuItem
            // 
            this.enhancedPOSToolStripMenuItem.Name = "enhancedPOSToolStripMenuItem";
            this.enhancedPOSToolStripMenuItem.Size = new System.Drawing.Size(274, 34);
            this.enhancedPOSToolStripMenuItem.Text = "🏪 Enhanced POS";
            this.enhancedPOSToolStripMenuItem.Click += new System.EventHandler(this.enhancedPOSToolStripMenuItem_Click);
            // 
            // newBillToolStripMenuItem
            // 
            this.newBillToolStripMenuItem.Name = "newBillToolStripMenuItem";
            this.newBillToolStripMenuItem.Size = new System.Drawing.Size(274, 34);
            this.newBillToolStripMenuItem.Text = "New Bill";
            this.newBillToolStripMenuItem.Click += new System.EventHandler(this.newBillToolStripMenuItem_Click);
            // 
            // editBillToolStripMenuItem
            // 
            this.editBillToolStripMenuItem.Name = "editBillToolStripMenuItem";
            this.editBillToolStripMenuItem.Size = new System.Drawing.Size(274, 34);
            this.editBillToolStripMenuItem.Text = "Edit Bill";
            this.editBillToolStripMenuItem.Click += new System.EventHandler(this.editBillToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(271, 6);
            // 
            // printBillToolStripMenuItem
            // 
            this.printBillToolStripMenuItem.Name = "printBillToolStripMenuItem";
            this.printBillToolStripMenuItem.Size = new System.Drawing.Size(274, 34);
            this.printBillToolStripMenuItem.Text = "Print Bill (Laser)";
            this.printBillToolStripMenuItem.Click += new System.EventHandler(this.printBillToolStripMenuItem_Click);
            // 
            // salesReportToolStripMenuItem
            // 
            this.salesReportToolStripMenuItem.Name = "salesReportToolStripMenuItem";
            this.salesReportToolStripMenuItem.Size = new System.Drawing.Size(274, 34);
            this.salesReportToolStripMenuItem.Text = "Sales Report";
            this.salesReportToolStripMenuItem.Click += new System.EventHandler(this.salesReportToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(271, 6);
            // 
            // creditSaleReporToolStripMenuItem
            // 
            this.creditSaleReporToolStripMenuItem.Name = "creditSaleReporToolStripMenuItem";
            this.creditSaleReporToolStripMenuItem.Size = new System.Drawing.Size(274, 34);
            this.creditSaleReporToolStripMenuItem.Text = "Credit Sale Returns";
            this.creditSaleReporToolStripMenuItem.Click += new System.EventHandler(this.creditSaleReporToolStripMenuItem_Click);
            // 
            // saleReturnsToolStripMenuItem
            // 
            this.saleReturnsToolStripMenuItem.Name = "saleReturnsToolStripMenuItem";
            this.saleReturnsToolStripMenuItem.Size = new System.Drawing.Size(274, 34);
            this.saleReturnsToolStripMenuItem.Text = "Sale Returns";
            this.saleReturnsToolStripMenuItem.Click += new System.EventHandler(this.saleReturnsToolStripMenuItem_Click);
            // 
            // printSaleToolStripMenuItem
            // 
            this.printSaleToolStripMenuItem.Name = "printSaleToolStripMenuItem";
            this.printSaleToolStripMenuItem.Size = new System.Drawing.Size(274, 34);
            this.printSaleToolStripMenuItem.Text = "Print Sale Return";
            this.printSaleToolStripMenuItem.Click += new System.EventHandler(this.printSaleToolStripMenuItem_Click);
            // 
            // returnReportToolStripMenuItem
            // 
            this.returnReportToolStripMenuItem.Name = "returnReportToolStripMenuItem";
            this.returnReportToolStripMenuItem.Size = new System.Drawing.Size(274, 34);
            this.returnReportToolStripMenuItem.Text = "Return Report";
            this.returnReportToolStripMenuItem.Click += new System.EventHandler(this.returnReportToolStripMenuItem_Click);
            // 
            // accountsToolStripMenuItem
            // 
            this.accountsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.customerAccountToolStripMenuItem,
            this.bankTransactionsToolStripMenuItem,
            this.bankLedgerToolStripMenuItem,
            this.toolStripSeparator2,
            this.suppliersPaymentsToolStripMenuItem,
            this.supplierLedgerToolStripMenuItem,
            this.supplierToolStripMenuItem,
            this.supplierBalancesToolStripMenuItem,
            this.toolStripSeparator3,
            this.expenseTransactionsToolStripMenuItem,
            this.expenseReportToolStripMenuItem});
            this.accountsToolStripMenuItem.Name = "accountsToolStripMenuItem";
            this.accountsToolStripMenuItem.Size = new System.Drawing.Size(104, 29);
            this.accountsToolStripMenuItem.Text = "Accounts";
            // 
            // customerAccountToolStripMenuItem
            // 
            this.customerAccountToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.customerPaymentToolStripMenuItem,
            this.toolStripSeparator1,
            this.customerLedgerToolStripMenuItem,
            this.customerToolStripMenuItem,
            this.customersBalancesToolStripMenuItem});
            this.customerAccountToolStripMenuItem.Name = "customerAccountToolStripMenuItem";
            this.customerAccountToolStripMenuItem.Size = new System.Drawing.Size(332, 34);
            this.customerAccountToolStripMenuItem.Text = "Customer Account";
            // 
            // customerPaymentToolStripMenuItem
            // 
            this.customerPaymentToolStripMenuItem.Name = "customerPaymentToolStripMenuItem";
            this.customerPaymentToolStripMenuItem.Size = new System.Drawing.Size(288, 34);
            this.customerPaymentToolStripMenuItem.Text = "Customer Payment";
            this.customerPaymentToolStripMenuItem.Click += new System.EventHandler(this.customerPaymentToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(285, 6);
            // 
            // customerLedgerToolStripMenuItem
            // 
            this.customerLedgerToolStripMenuItem.Name = "customerLedgerToolStripMenuItem";
            this.customerLedgerToolStripMenuItem.Size = new System.Drawing.Size(288, 34);
            this.customerLedgerToolStripMenuItem.Text = "Customer Ledger";
            this.customerLedgerToolStripMenuItem.Click += new System.EventHandler(this.customerLedgerToolStripMenuItem_Click);
            // 
            // customerToolStripMenuItem
            // 
            this.customerToolStripMenuItem.Name = "customerToolStripMenuItem";
            this.customerToolStripMenuItem.Size = new System.Drawing.Size(288, 34);
            this.customerToolStripMenuItem.Text = "Customer Sale Detail";
            this.customerToolStripMenuItem.Click += new System.EventHandler(this.customerToolStripMenuItem_Click);
            // 
            // customersBalancesToolStripMenuItem
            // 
            this.customersBalancesToolStripMenuItem.Name = "customersBalancesToolStripMenuItem";
            this.customersBalancesToolStripMenuItem.Size = new System.Drawing.Size(288, 34);
            this.customersBalancesToolStripMenuItem.Text = "Customers Balances";
            this.customersBalancesToolStripMenuItem.Click += new System.EventHandler(this.customersBalancesToolStripMenuItem_Click);
            // 
            // bankTransactionsToolStripMenuItem
            // 
            this.bankTransactionsToolStripMenuItem.Name = "bankTransactionsToolStripMenuItem";
            this.bankTransactionsToolStripMenuItem.Size = new System.Drawing.Size(332, 34);
            this.bankTransactionsToolStripMenuItem.Text = "Bank Transactions";
            this.bankTransactionsToolStripMenuItem.Click += new System.EventHandler(this.bankTransactionsToolStripMenuItem_Click);
            // 
            // bankLedgerToolStripMenuItem
            // 
            this.bankLedgerToolStripMenuItem.Name = "bankLedgerToolStripMenuItem";
            this.bankLedgerToolStripMenuItem.Size = new System.Drawing.Size(332, 34);
            this.bankLedgerToolStripMenuItem.Text = "Bank Ledger";
            this.bankLedgerToolStripMenuItem.Click += new System.EventHandler(this.bankLedgerToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(329, 6);
            // 
            // suppliersPaymentsToolStripMenuItem
            // 
            this.suppliersPaymentsToolStripMenuItem.Name = "suppliersPaymentsToolStripMenuItem";
            this.suppliersPaymentsToolStripMenuItem.Size = new System.Drawing.Size(332, 34);
            this.suppliersPaymentsToolStripMenuItem.Text = "Supplier\'s Payments";
            this.suppliersPaymentsToolStripMenuItem.Click += new System.EventHandler(this.suppliersPaymentsToolStripMenuItem_Click);
            // 
            // supplierLedgerToolStripMenuItem
            // 
            this.supplierLedgerToolStripMenuItem.Name = "supplierLedgerToolStripMenuItem";
            this.supplierLedgerToolStripMenuItem.Size = new System.Drawing.Size(332, 34);
            this.supplierLedgerToolStripMenuItem.Text = "Supplier Ledger";
            this.supplierLedgerToolStripMenuItem.Click += new System.EventHandler(this.supplierLedgerToolStripMenuItem_Click);
            // 
            // supplierToolStripMenuItem
            // 
            this.supplierToolStripMenuItem.Name = "supplierToolStripMenuItem";
            this.supplierToolStripMenuItem.Size = new System.Drawing.Size(332, 34);
            this.supplierToolStripMenuItem.Text = "Supplier Payments Report";
            this.supplierToolStripMenuItem.Click += new System.EventHandler(this.supplierToolStripMenuItem_Click);
            // 
            // supplierBalancesToolStripMenuItem
            // 
            this.supplierBalancesToolStripMenuItem.Name = "supplierBalancesToolStripMenuItem";
            this.supplierBalancesToolStripMenuItem.Size = new System.Drawing.Size(332, 34);
            this.supplierBalancesToolStripMenuItem.Text = "Supplier Balances";
            this.supplierBalancesToolStripMenuItem.Click += new System.EventHandler(this.supplierBalancesToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(329, 6);
            // 
            // expenseTransactionsToolStripMenuItem
            // 
            this.expenseTransactionsToolStripMenuItem.Name = "expenseTransactionsToolStripMenuItem";
            this.expenseTransactionsToolStripMenuItem.Size = new System.Drawing.Size(332, 34);
            this.expenseTransactionsToolStripMenuItem.Text = "Expense Transactions";
            this.expenseTransactionsToolStripMenuItem.Click += new System.EventHandler(this.expenseTransactionsToolStripMenuItem_Click);
            // 
            // expenseReportToolStripMenuItem
            // 
            this.expenseReportToolStripMenuItem.Name = "expenseReportToolStripMenuItem";
            this.expenseReportToolStripMenuItem.Size = new System.Drawing.Size(332, 34);
            this.expenseReportToolStripMenuItem.Text = "Expense Report";
            this.expenseReportToolStripMenuItem.Click += new System.EventHandler(this.expenseReportToolStripMenuItem_Click);
            // 
            // reportsToolStripMenuItem
            // 
            this.reportsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.comprehensiveReportsToolStripMenuItem,
            this.toolStripSeparator8,
            this.updateStockToolStripMenuItem,
            this.stockInHandToolStripMenuItem,
            this.genericSearchToolStripMenuItem,
            this.productSearchToolStripMenuItem,
            this.singleProductDetailToolStripMenuItem,
            this.toolStripSeparator6,
            this.profitToolStripMenuItem,
            this.toolStripSeparator7,
            this.userActivityToolStripMenuItem});
            this.reportsToolStripMenuItem.Name = "reportsToolStripMenuItem";
            this.reportsToolStripMenuItem.Size = new System.Drawing.Size(94, 29);
            this.reportsToolStripMenuItem.Text = "Reports";
            // 
            // comprehensiveReportsToolStripMenuItem
            // 
            this.comprehensiveReportsToolStripMenuItem.Name = "comprehensiveReportsToolStripMenuItem";
            this.comprehensiveReportsToolStripMenuItem.Size = new System.Drawing.Size(406, 34);
            this.comprehensiveReportsToolStripMenuItem.Text = "📊 Comprehensive Reports Center";
            this.comprehensiveReportsToolStripMenuItem.Click += new System.EventHandler(this.comprehensiveReportsToolStripMenuItem_Click);
            // 
            // updateStockToolStripMenuItem
            // 
            this.updateStockToolStripMenuItem.Name = "updateStockToolStripMenuItem";
            this.updateStockToolStripMenuItem.Size = new System.Drawing.Size(406, 34);
            this.updateStockToolStripMenuItem.Text = "Update Stock";
            this.updateStockToolStripMenuItem.Click += new System.EventHandler(this.updateStockToolStripMenuItem_Click);
            // 
            // stockInHandToolStripMenuItem
            // 
            this.stockInHandToolStripMenuItem.Name = "stockInHandToolStripMenuItem";
            this.stockInHandToolStripMenuItem.Size = new System.Drawing.Size(406, 34);
            this.stockInHandToolStripMenuItem.Text = "Stock In Hand";
            this.stockInHandToolStripMenuItem.Click += new System.EventHandler(this.stockInHandToolStripMenuItem_Click);
            // 
            // genericSearchToolStripMenuItem
            // 
            this.genericSearchToolStripMenuItem.Name = "genericSearchToolStripMenuItem";
            this.genericSearchToolStripMenuItem.Size = new System.Drawing.Size(406, 34);
            this.genericSearchToolStripMenuItem.Text = "Generic Search";
            this.genericSearchToolStripMenuItem.Click += new System.EventHandler(this.genericSearchToolStripMenuItem_Click);
            // 
            // productSearchToolStripMenuItem
            // 
            this.productSearchToolStripMenuItem.Name = "productSearchToolStripMenuItem";
            this.productSearchToolStripMenuItem.Size = new System.Drawing.Size(406, 34);
            this.productSearchToolStripMenuItem.Text = "Product Search";
            this.productSearchToolStripMenuItem.Click += new System.EventHandler(this.productSearchToolStripMenuItem_Click);
            // 
            // singleProductDetailToolStripMenuItem
            // 
            this.singleProductDetailToolStripMenuItem.Name = "singleProductDetailToolStripMenuItem";
            this.singleProductDetailToolStripMenuItem.Size = new System.Drawing.Size(406, 34);
            this.singleProductDetailToolStripMenuItem.Text = "Single Product Detail";
            this.singleProductDetailToolStripMenuItem.Click += new System.EventHandler(this.singleProductDetailToolStripMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(403, 6);
            // 
            // profitToolStripMenuItem
            // 
            this.profitToolStripMenuItem.Name = "profitToolStripMenuItem";
            this.profitToolStripMenuItem.Size = new System.Drawing.Size(406, 34);
            this.profitToolStripMenuItem.Text = "Profit Loss";
            this.profitToolStripMenuItem.Click += new System.EventHandler(this.profitToolStripMenuItem_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(403, 6);
            // 
            // userActivityToolStripMenuItem
            // 
            this.userActivityToolStripMenuItem.Name = "userActivityToolStripMenuItem";
            this.userActivityToolStripMenuItem.Size = new System.Drawing.Size(406, 34);
            this.userActivityToolStripMenuItem.Text = "User Activity";
            this.userActivityToolStripMenuItem.Click += new System.EventHandler(this.userActivityToolStripMenuItem_Click);
            // 
            // backupToolStripMenuItem
            // 
            this.backupToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.databaseBackupToolStripMenuItem,
            this.databaseRestoreToolStripMenuItem,
            this.dataExportToolStripMenuItem});
            this.backupToolStripMenuItem.Name = "backupToolStripMenuItem";
            this.backupToolStripMenuItem.Size = new System.Drawing.Size(87, 29);
            this.backupToolStripMenuItem.Text = "Backup";
            // 
            // databaseBackupToolStripMenuItem
            // 
            this.databaseBackupToolStripMenuItem.Name = "databaseBackupToolStripMenuItem";
            this.databaseBackupToolStripMenuItem.Size = new System.Drawing.Size(259, 34);
            this.databaseBackupToolStripMenuItem.Text = "Database Backup";
            this.databaseBackupToolStripMenuItem.Click += new System.EventHandler(this.databaseBackupToolStripMenuItem_Click);
            // 
            // databaseRestoreToolStripMenuItem
            // 
            this.databaseRestoreToolStripMenuItem.Name = "databaseRestoreToolStripMenuItem";
            this.databaseRestoreToolStripMenuItem.Size = new System.Drawing.Size(259, 34);
            this.databaseRestoreToolStripMenuItem.Text = "Database Restore";
            this.databaseRestoreToolStripMenuItem.Click += new System.EventHandler(this.databaseRestoreToolStripMenuItem_Click);
            // 
            // dataExportToolStripMenuItem
            // 
            this.dataExportToolStripMenuItem.Name = "dataExportToolStripMenuItem";
            this.dataExportToolStripMenuItem.Size = new System.Drawing.Size(259, 34);
            this.dataExportToolStripMenuItem.Text = "Data Export";
            this.dataExportToolStripMenuItem.Click += new System.EventHandler(this.dataExportToolStripMenuItem_Click);
            // 
            // UserDashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.menuStrip1);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "UserDashboard";
            this.Size = new System.Drawing.Size(2055, 869);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbExit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbStockInHand)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbSalesReport)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbCustomerPayment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbCustomerLedger)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbCustomerBalance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbSaleReturn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbEditBill)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbCreditBill)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbNewBill)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbNewPurchase)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbCompanies)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.PictureBox pbStockInHand;
        private System.Windows.Forms.Button btnStockInHand;
        private System.Windows.Forms.PictureBox pbSalesReport;
        private System.Windows.Forms.Button btnSalesReport;
        private System.Windows.Forms.PictureBox pbCustomerPayment;
        private System.Windows.Forms.Button btnCustomerPayment;
        private System.Windows.Forms.PictureBox pbCustomerLedger;
        private System.Windows.Forms.Button btnCustomerLedger;
        private System.Windows.Forms.PictureBox pbCustomerBalance;
        private System.Windows.Forms.Button btnCustomerBalance;
        private System.Windows.Forms.PictureBox pbSaleReturn;
        private System.Windows.Forms.Button btnSaleReturn;
        private System.Windows.Forms.PictureBox pbEditBill;
        private System.Windows.Forms.Button btnEditBill;
        private System.Windows.Forms.PictureBox pbCreditBill;
        private System.Windows.Forms.Button btnCreditBill;
        private System.Windows.Forms.PictureBox pbNewBill;
        private System.Windows.Forms.Button btnNewBill;
        private System.Windows.Forms.PictureBox pbNewPurchase;
        private System.Windows.Forms.Button btnNewPurchase;
        private System.Windows.Forms.PictureBox pbItems;
        private System.Windows.Forms.Button btnItems;
        private System.Windows.Forms.PictureBox pbCompanies;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnCompanies;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem setupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem purchasesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem salesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem accountsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reportsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem backupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newBillToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editBillToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem printBillToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem salesReportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem creditSaleReporToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saleReturnsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem printSaleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem returnReportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem customerAccountToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bankTransactionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bankLedgerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem customerPaymentToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem customerLedgerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem customerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem customersBalancesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem customerManagementToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem suppliersPaymentsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem supplierLedgerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem supplierToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem supplierBalancesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem expenseTransactionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem expenseReportToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.PictureBox pbExit;
        private System.Windows.Forms.ToolStripMenuItem updateStockToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stockInHandToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem genericSearchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem productSearchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem singleProductDetailToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem profitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem userActivityToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripMenuItem comprehensiveReportsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem changePasswordToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem userManagementToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem enhancedPOSToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newPurchaseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem purchaseHistoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem purchaseReportToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripMenuItem supplierManagementToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem databaseBackupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem databaseRestoreToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dataExportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setupSupplierManagementToolStripMenuItem;
    }
}