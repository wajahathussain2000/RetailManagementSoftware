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

        public void PurchaseReturnFormShow()
        {
            PurchaseReturn purchaseReturn = new PurchaseReturn();
            purchaseReturn.Show();
        }

        public void ExpenseEntryFormShow()
        {
            ExpenseEntry expenseEntry = new ExpenseEntry();
            expenseEntry.Show();
        }

        public void GSTSetupFormShow()
        {
            GSTSetup gstSetup = new GSTSetup();
            gstSetup.Show();
        }

        public void ExpiryAlertFormShow()
        {
            ExpiryAlert expiryAlert = new ExpiryAlert();
            expiryAlert.Show();
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
            ProfitAndLoss profitAndLoss = new ProfitAndLoss();
            profitAndLoss.Show();
        }

        public void ExpenseReportFormShow()
        {
            ExpenseReport expenseReport = new ExpenseReport();
            expenseReport.Show();
        }

        public void SupplierLedgerFormShow()
        {
            SupplierLedger supplierLedger = new SupplierLedger();
            supplierLedger.Show();
        }

        public void GSTReportFormShow()
        {
            GSTReport gstReport = new GSTReport();
            gstReport.Show();
        }

        public void ExpiredStockReportFormShow()
        {
            ExpiredStockReport expiredStockReport = new ExpiredStockReport();
            expiredStockReport.Show();
        }

        public void BarcodeGeneratorFormShow()
        {
            BarcodeGenerator barcodeGenerator = new BarcodeGenerator();
            barcodeGenerator.Show();
        }

        public void BankTransactionsFormShow()
        {
            BankTransactions bankTransactions = new BankTransactions();
            bankTransactions.Show();
        }

        public void TrialBalanceFormShow()
        {
            TrialBalance trialBalance = new TrialBalance();
            trialBalance.Show();
        }

        public void CashFlowFormShow()
        {
            CashFlow cashFlow = new CashFlow();
            cashFlow.Show();
        }

        public void UserManagementFormShow()
        {
            UserManagement userManagement = new UserManagement();
            userManagement.Show();
        }

        public void PurchaseReportFormShow()
        {
            PurchaseReport purchaseReport = new PurchaseReport();
            purchaseReport.Show();
        }

        public void ExpiryReportFormShow()
        {
            ExpiryReport expiryReport = new ExpiryReport();
            expiryReport.Show();
        }

        public void CustomerPurchaseHistoryFormShow()
        {
            CustomerPurchaseHistory customerPurchaseHistory = new CustomerPurchaseHistory();
            customerPurchaseHistory.Show();
        }

        public void SalesReturnReportFormShow()
        {
            SalesReturnReport salesReturnReport = new SalesReturnReport();
            salesReturnReport.Show();
        }

        public void PurchaseReturnReportFormShow()
        {
            PurchaseReturnReport purchaseReturnReport = new PurchaseReturnReport();
            purchaseReturnReport.Show();
        }

        public void CustomerOutstandingFormShow()
        {
            CustomerOutstanding customerOutstanding = new CustomerOutstanding();
            customerOutstanding.Show();
        }

        public void DailySummaryReportFormShow()
        {
            DailySummaryReport dailySummaryReport = new DailySummaryReport();
            dailySummaryReport.Show();
        }

        public void BarcodeLabelPrintFormShow()
        {
            BarcodeLabelPrint barcodeLabelPrint = new BarcodeLabelPrint();
            barcodeLabelPrint.Show();
        }

        public void InventoryReportFormShow()
        {
            InventoryReport inventoryReport = new InventoryReport();
            inventoryReport.Show();
        }

        public void LowStockAlertReportFormShow()
        {
            LowStockAlertReport lowStockAlertReport = new LowStockAlertReport();
            lowStockAlertReport.Show();
        }

        private void changePasswordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PasswordFormShow();
        }
    }
}
