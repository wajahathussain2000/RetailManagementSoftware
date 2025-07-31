using RetailManagement.UserForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RetailManagement
{
    public partial class UserDashboard : UserControl
    {
        public event Action CancelClicked;
        public UserDashboard()
        {
            InitializeComponent();
        }

        #region Forms Show Methods

        public void CompaniesFormShow()
        {
            Companies companies = new Companies();
            companies.Show();
        }

        public void ItemsFormShow()
        {
            Items items = new Items();
            items.Show();
        }

        public void NewPurchaseFormShow()
        {
            NewPurchase newPurchase = new NewPurchase();
            newPurchase.Show();
        }

        public void NewBillFormShow()
        {
            NewBill newBill = new NewBill();
            newBill.Show();
        }

        public void CreditBillFormShow()
        {
            CreditBill creditBill = new CreditBill();
            creditBill.Show();
        }

        public void EditBillFormShow()
        {
            EditBill editBill = new EditBill();
            editBill.Show();
        }

        public void CustomerPaymentFormShow()
        {
            CustomerPayment customerPayment = new CustomerPayment();
            customerPayment.Show();
        }

        public void CustomerLedgerFormShow()
        {
            CustomerLedger customerLedger = new CustomerLedger();
            customerLedger.Show();
        }

        public void PasswordFormShow()
        {
            PasswordForm password = new PasswordForm();
            password.Show();
        }

        public void StockInHandFormShow()
        {
            StockInHand stockInHand = new StockInHand();
            stockInHand.Show();
        }

        public void SaleReturnFormShow()
        {
            SaleReturn saleReturn = new SaleReturn();
            saleReturn.Show();
        }

        public void CustomerBalanceFormShow()
        {
            CustomerBalance customerBalance = new CustomerBalance();
            customerBalance.Show();
        }

        public void SalesReportFormShow()
        {
            SalesReport salesReport = new SalesReport();
            salesReport.Show();
        }

        #endregion

        private void btnExit_Click(object sender, EventArgs e)
        {
            CancelClicked?.Invoke();
        }

        private void pbExit_Click(object sender, EventArgs e)
        {
            CancelClicked?.Invoke();
        }

        private void btnCompanies_Click(object sender, EventArgs e)
        {
            CompaniesFormShow();
        }

        private void pbCompanies_Click(object sender, EventArgs e)
        {
            CompaniesFormShow();
        }

        private void btnItems_Click(object sender, EventArgs e)
        {
            ItemsFormShow();
        }

        private void pbItems_Click(object sender, EventArgs e)
        {
            ItemsFormShow();
        }

        private void btnNewPurchase_Click(object sender, EventArgs e)
        {
            NewPurchaseFormShow();
        }

        private void pbNewPurchase_Click(object sender, EventArgs e)
        {
            NewPurchaseFormShow();
        }

        private void btnNewBill_Click(object sender, EventArgs e)
        {
            NewBillFormShow();
        }

        private void pbNewBill_Click(object sender, EventArgs e)
        {
            NewBillFormShow();
        }

        private void btnCreditBill_Click(object sender, EventArgs e)
        {
            CreditBillFormShow();
        }

        private void pbCreditBill_Click(object sender, EventArgs e)
        {
            CreditBillFormShow();
        }

        private void btnEditBill_Click(object sender, EventArgs e)
        {
            EditBillFormShow();
        }

        private void pbEditBill_Click(object sender, EventArgs e)
        {
            EditBillFormShow();
        }

        private void btnSaleReturn_Click(object sender, EventArgs e)
        {
            SaleReturnFormShow();
        }

        private void pbSaleReturn_Click(object sender, EventArgs e)
        {
            SaleReturnFormShow();
        }

        private void btnCustomerBalance_Click(object sender, EventArgs e)
        {
            CustomerBalanceFormShow();
        }

        private void pbCustomerBalance_Click(object sender, EventArgs e)
        {
            CustomerBalanceFormShow();
        }

        private void btnCustomerLedger_Click(object sender, EventArgs e)
        {
            CustomerLedgerFormShow();
        }

        private void pbCustomerLedger_Click(object sender, EventArgs e)
        {
            CustomerLedgerFormShow();
        }

        private void btnCustomerPayment_Click(object sender, EventArgs e)
        {
            CustomerPaymentFormShow();
        }

        private void pbCustomerPayment_Click(object sender, EventArgs e)
        {
            CustomerPaymentFormShow();
        }

        private void btnSalesReport_Click(object sender, EventArgs e)
        {
            SalesReportFormShow();
        }

        private void pbSalesReport_Click(object sender, EventArgs e)
        {
            SalesReportFormShow();
        }

        private void btnStockInHand_Click(object sender, EventArgs e)
        {
            StockInHandFormShow();
        }

        private void pbStockInHand_Click(object sender, EventArgs e)
        {
            StockInHandFormShow();
        }

        private void customerPaymentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CustomerPaymentFormShow();
        }

        private void customerLedgerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CustomerLedgerFormShow();
        }

        private void profitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PasswordFormShow();
        }
    }
}
