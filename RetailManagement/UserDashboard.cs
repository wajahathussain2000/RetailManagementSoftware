using RetailManagement.Database;
using RetailManagement.UserForms;
using RetailManagement.Utils;
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
        private List<Form> openForms;
        private bool isDisposing = false;

        public UserDashboard()
        {
            try
            {
                InitializeComponent();
                openForms = new List<Form>();
                InitializeUserSession();
                InitializeDynamicDate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing dashboard: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}", 
                    "Dashboard Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw; // Re-throw to let MainScreen handle it
            }
        }



        internal void CloseAllManagedForms()
        {
            try
            {
                // Create a copy of the list to avoid modification during iteration
                var formsToClose = openForms.ToList();
                openForms.Clear();
                
                foreach (var form in formsToClose)
                {
                    if (form != null && !form.IsDisposed)
                    {
                        form.Close();
                        form.Dispose();
                    }
                }
            }
            catch
            {
                // Ignore disposal errors
            }
        }

        private void InitializeUserSessionMinimal()
        {
            try
            {
                // Check if user is properly logged in
                if (!UserSession.IsLoggedIn)
                {
                    MessageBox.Show("Unauthorized access detected. Please login first.", "Security Alert", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    CancelClicked?.Invoke();
                    return;
                }

                // Minimal user display without database calls
                UpdateUserDisplayMinimal();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in InitializeUserSessionMinimal: {ex.Message}");
                MessageBox.Show($"Error initializing user session: {ex.Message}", "Session Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                CancelClicked?.Invoke();
            }
        }

        private void InitializeUserSession()
        {
            try
            {
                // Check if user is properly logged in
                if (!UserSession.IsLoggedIn)
                {
                    MessageBox.Show("Unauthorized access detected. Please login first.", "Security Alert", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    CancelClicked?.Invoke();
                    return;
                }

                // Display welcome message and user info
                UpdateUserDisplay();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in InitializeUserSession: {ex.Message}");
                MessageBox.Show($"Error initializing user session: {ex.Message}", "Session Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                CancelClicked?.Invoke();
            }
        }

        private void UpdateUserDisplayMinimal()
        {
            try
            {
                // Ensure user session properties are not null
                string fullName = UserSession.FullName ?? "Unknown User";
                string role = UserSession.Role ?? "User";
                
                // Update any user-specific labels or controls
                this.Text = $"Retail Management System - Welcome {fullName}";
                
                // Don't show welcome popup for minimal version
                // SetupRoleBasedAccess(); // Skip database-heavy operations
                
                System.Diagnostics.Debug.WriteLine($"Dashboard loaded for user: {fullName} with role: {role}");
            }
            catch (Exception ex)
            {
                // Handle any display update errors
                System.Diagnostics.Debug.WriteLine($"Error updating user display minimal: {ex.Message}");
            }
        }

        private void UpdateUserDisplay()
        {
            try
            {
                // Ensure user session properties are not null
                string fullName = UserSession.FullName ?? "Unknown User";
                string role = UserSession.Role ?? "User";
                
                // Update any user-specific labels or controls
                this.Text = $"Retail Management System - Welcome {fullName}";
                
                // Show role-specific welcome message
                string roleMessage = GetRoleWelcomeMessage();
                MessageBox.Show(roleMessage, "Welcome", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                // Setup role-based menu access
                SetupRoleBasedAccess();
                
                // You can add more user-specific customizations here
                // For example, if there's a status label:
                // lblUserName.Text = $"Logged in as: {fullName} ({role})";
            }
            catch (Exception ex)
            {
                // Handle any display update errors
                System.Diagnostics.Debug.WriteLine($"Error updating user display: {ex.Message}");
            }
        }

        private string GetRoleWelcomeMessage()
        {
            string fullName = UserSession.FullName ?? "Unknown User";
            string role = UserSession.Role ?? "User";
            
            switch (role)
            {
                case "Admin":
                    return $"Welcome {fullName}!\n\nYou are logged in as Administrator.\nYou have full access to all modules and features.";
                case "Pharmacist":
                    return $"Welcome {fullName}!\n\nYou are logged in as Pharmacist.\nYou can manage inventory, sales, and view reports.";
                case "Salesman":
                    return $"Welcome {fullName}!\n\nYou are logged in as Salesman.\nYou can process sales and manage customers.";
                default:
                    return $"Welcome {fullName}!\n\nRole: {role}";
            }
        }

        private void SetupRoleBasedAccess()
        {
            try
            {
                // Set up role-based access control for dashboard buttons and menu items
                string userRole = UserSession.Role ?? "Salesman";
                
                switch (userRole)
                {
                    case "Admin":
                        // Admin has access to everything - no restrictions
                        EnableAllControls(true);
                        ShowAdminOnlyFeatures(true);
                        break;
                        
                    case "Pharmacist":
                        // Pharmacist can manage inventory, sales, and view reports
                        EnableAllControls(true);
                        
                        // Restrict admin-only features
                        ShowAdminOnlyFeatures(false);
                        
                        // Pharmacist restrictions (if any specific ones needed)
                        RestrictSensitiveFinancialReports(false);
                        break;
                        
                    case "Salesman":
                        // Salesman can only process sales and basic customer management
                        EnableAllControls(false);
                        
                        // Enable sales-related functions
                        EnableSalesControls(true);
                        
                        // Enable basic customer management
                        EnableCustomerControls(true);
                        
                        // Hide admin features completely
                        ShowAdminOnlyFeatures(false);
                        RestrictSensitiveFinancialReports(false);
                        RestrictInventoryManagement(false);
                        break;
                        
                    default:
                        // Unknown role - minimal access
                        EnableAllControls(false);
                        EnableSalesControls(true);  // Basic sales only
                        ShowAdminOnlyFeatures(false);
                        break;
                }
                
                // Log the role-based setup
                UserSession.LogActivity("Dashboard Access", "System", $"Role-based dashboard configured for {UserSession.Role}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting up role-based access: {ex.Message}");
                // On error, default to minimal access
                EnableAllControls(false);
                EnableSalesControls(true);
            }
        }
        
        private void EnableAllControls(bool enabled)
        {
            // Main dashboard buttons
            btnNewBill.Enabled = enabled;
            btnNewPurchase.Enabled = enabled;
            btnItems.Enabled = enabled;
            btnCompanies.Enabled = enabled;
            btnEditBill.Enabled = enabled;
            btnCreditBill.Enabled = enabled;
            btnSaleReturn.Enabled = enabled;
            btnCustomerBalance.Enabled = enabled;
            btnCustomerLedger.Enabled = enabled;
            btnCustomerPayment.Enabled = enabled;
            btnSalesReport.Enabled = enabled;
            btnStockInHand.Enabled = enabled;
            
            // Menu items - Sales
            newBillToolStripMenuItem.Enabled = enabled;
            editBillToolStripMenuItem.Enabled = enabled;
            salesReportToolStripMenuItem.Enabled = enabled;
            creditSaleReporToolStripMenuItem.Enabled = enabled;
            saleReturnsToolStripMenuItem.Enabled = enabled;
            
            // Menu items - Purchases  
            newPurchaseToolStripMenuItem.Enabled = enabled;
            purchaseHistoryToolStripMenuItem.Enabled = enabled;
            purchaseReportToolStripMenuItem.Enabled = enabled;
            supplierManagementToolStripMenuItem.Enabled = enabled;
            
            // Menu items - Accounts
            customerAccountToolStripMenuItem.Enabled = enabled;
            customerPaymentToolStripMenuItem.Enabled = enabled;
            customerLedgerToolStripMenuItem.Enabled = enabled;
            customersBalancesToolStripMenuItem.Enabled = enabled;
            bankTransactionsToolStripMenuItem.Enabled = enabled;
            bankLedgerToolStripMenuItem.Enabled = enabled;
            suppliersPaymentsToolStripMenuItem.Enabled = enabled;
            supplierLedgerToolStripMenuItem.Enabled = enabled;
            supplierBalancesToolStripMenuItem.Enabled = enabled;
            expenseTransactionsToolStripMenuItem.Enabled = enabled;
            expenseReportToolStripMenuItem.Enabled = enabled;
            
            // Menu items - Reports
            updateStockToolStripMenuItem.Enabled = enabled;
            stockInHandToolStripMenuItem.Enabled = enabled;
            comprehensiveReportsToolStripMenuItem.Enabled = enabled;
            genericSearchToolStripMenuItem.Enabled = enabled;
            productSearchToolStripMenuItem.Enabled = enabled;
            profitToolStripMenuItem.Enabled = enabled;
            userActivityToolStripMenuItem.Enabled = enabled;
            
            // Setup menu
            customerManagementToolStripMenuItem.Enabled = enabled;
            setupSupplierManagementToolStripMenuItem.Enabled = enabled;
        }
        
        private void EnableSalesControls(bool enabled)
        {
            // Sales-related buttons
            btnNewBill.Enabled = enabled;
            btnEditBill.Enabled = enabled;
            btnCreditBill.Enabled = enabled;
            btnSaleReturn.Enabled = enabled;
            btnSalesReport.Enabled = enabled;
            
            // Sales menu items
            newBillToolStripMenuItem.Enabled = enabled;
            editBillToolStripMenuItem.Enabled = enabled;
            salesReportToolStripMenuItem.Enabled = enabled;
            saleReturnsToolStripMenuItem.Enabled = enabled;
            enhancedPOSToolStripMenuItem.Enabled = enabled;
        }
        
        private void EnableCustomerControls(bool enabled)
        {
            // Customer-related functions
            btnCustomerBalance.Enabled = enabled;
            btnCustomerLedger.Enabled = enabled;
            btnCustomerPayment.Enabled = enabled;
            
            // Customer menu items
            customerAccountToolStripMenuItem.Enabled = enabled;
            customerPaymentToolStripMenuItem.Enabled = enabled;
            customerLedgerToolStripMenuItem.Enabled = enabled;
            customersBalancesToolStripMenuItem.Enabled = enabled;
            customerManagementToolStripMenuItem.Enabled = enabled;
        }
        
        private void ShowAdminOnlyFeatures(bool show)
        {
            // Admin-only features (these would be hidden for non-admin users)
            userActivityToolStripMenuItem.Visible = show;
            userManagementToolStripMenuItem.Visible = show;  // User Management is Admin-only
            backupToolStripMenuItem.Visible = show;
            databaseBackupToolStripMenuItem.Visible = show;
            databaseRestoreToolStripMenuItem.Visible = show;
            dataExportToolStripMenuItem.Visible = show;
            
            // If there are specific admin buttons, hide them here
            // Example: btnAdminPanel.Visible = show;
        }
        
        private void RestrictSensitiveFinancialReports(bool allow)
        {
            // Restrict access to sensitive financial information
            profitToolStripMenuItem.Enabled = allow;
            bankTransactionsToolStripMenuItem.Enabled = allow;
            bankLedgerToolStripMenuItem.Enabled = allow;
            expenseTransactionsToolStripMenuItem.Enabled = allow;
            expenseReportToolStripMenuItem.Enabled = allow;
            
            // Supplier payment information
            suppliersPaymentsToolStripMenuItem.Enabled = allow;
            supplierLedgerToolStripMenuItem.Enabled = allow;
            supplierBalancesToolStripMenuItem.Enabled = allow;
        }
        
        private void RestrictInventoryManagement(bool allow)
        {
            // Restrict inventory management functions
            btnNewPurchase.Enabled = allow;
            btnItems.Enabled = allow;
            btnCompanies.Enabled = allow;
            btnStockInHand.Enabled = allow;
            
            // Purchase menu items
            newPurchaseToolStripMenuItem.Enabled = allow;
            purchaseHistoryToolStripMenuItem.Enabled = allow;
            purchaseReportToolStripMenuItem.Enabled = allow;
            supplierManagementToolStripMenuItem.Enabled = allow;
            setupSupplierManagementToolStripMenuItem.Enabled = allow;
            
            // Stock management
            updateStockToolStripMenuItem.Enabled = allow;
            stockInHandToolStripMenuItem.Enabled = allow;
        }

        #region Forms Show Methods

        private void ShowFormSafely<T>(string moduleName = "") where T : Form, new()
        {
            if (!CheckAuthentication() || isDisposing) return;
            
            // Check permissions if module name is provided
            if (!string.IsNullOrEmpty(moduleName) && !UserSession.HasPermission(moduleName, "view"))
            {
                MessageBox.Show($"Access Denied!\n\nYou don't have permission to access {moduleName} module.\n\nContact your administrator for access.", 
                    "Permission Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                UserSession.LogActivity("Access Denied", moduleName, $"User tried to access {moduleName} without permission");
                return;
            }
            
            try
            {
                // Check if form of this type is already open
                T existingForm = openForms.OfType<T>().FirstOrDefault();
                if (existingForm != null && !existingForm.IsDisposed)
                {
                    existingForm.BringToFront();
                    existingForm.Focus();
                    UserSession.LogActivity("Form Accessed", moduleName, $"Existing {typeof(T).Name} form brought to front");
                    return;
                }
                
                // Create new form
                T newForm = new T();
                
                // Add to tracking list
                openForms.Add(newForm);
                
                // Add event handler to remove from list when closed
                newForm.FormClosed += (sender, e) => {
                    if (!isDisposing && openForms.Contains(newForm))
                    {
                        openForms.Remove(newForm);
                    }
                };
                
                // Show the form
                newForm.Show();
                
                // Log the activity
                UserSession.LogActivity("Form Opened", moduleName, $"Opened {typeof(T).Name} form");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening form: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                UserSession.LogActivity("Error", moduleName, $"Failed to open {typeof(T).Name}: {ex.Message}");
            }
        }

        public void CompaniesFormShow()
        {
            ShowFormSafely<Companies>("Companies");
        }

        public void ItemsFormShow()
        {
            ShowFormSafely<Items>("Items");
        }

        public void NewPurchaseFormShow()
        {
            ShowFormSafely<NewPurchase>("Purchase");
        }

        public void NewBillFormShow()
        {
            ShowFormSafely<NewBillForm>();
        }

        public void CreditBillFormShow()
        {
            ShowFormSafely<CreditBill>("Sales");
        }

        public void EditBillFormShow()
        {
            ShowFormSafely<EditBill>("Sales");
        }

        public void CustomerPaymentFormShow()
        {
            ShowFormSafely<CustomerPayment>("Customers");
        }

        public void CustomerLedgerFormShow()
        {
            ShowFormSafely<CustomerLedger>();
        }

        public void PasswordFormShow()
        {
            try
            {
                // Show change password form
                ChangePassword changePasswordForm = new ChangePassword();
                DialogResult result = changePasswordForm.ShowDialog();
                
                if (result == DialogResult.OK)
                {
                    MessageBox.Show("Password changed successfully!\n\nYour new password is now active.", 
                        "Password Changed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening password change form: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void StockInHandFormShow()
        {
            ShowFormSafely<StockInHand>();
        }

        public void SaleReturnFormShow()
        {
            ShowFormSafely<SaleReturn>();
        }

        public void CustomerBalanceFormShow()
        {
            ShowFormSafely<CustomerBalance>();
        }

        public void SalesReportFormShow()
        {
            ShowFormSafely<SalesReport>();
        }

        public void PurchaseReturnFormShow()
        {
            ShowFormSafely<PurchaseReturn>();
        }

        public void ExpenseEntryFormShow()
        {
            ShowFormSafely<ExpenseEntry>();
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
            LogoutUser();
        }

        private void pbExit_Click(object sender, EventArgs e)
        {
            LogoutUser();
        }

        private void LogoutUser()
        {
            string fullName = UserSession.FullName ?? "User";
            if (MessageBox.Show($"Are you sure you want to logout, {fullName}?", "Confirm Logout", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                // Close all open forms first
                CloseAllManagedForms();
                
                // Clear user session
                UserSession.Logout();
                
                // Show logout message
                MessageBox.Show("You have been successfully logged out.", "Logout Complete", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                // Return to login screen
                CancelClicked?.Invoke();
            }
        }

        private bool CheckAuthentication()
        {
            if (!UserSession.IsLoggedIn)
            {
                MessageBox.Show("Session expired. Please login again.", "Authentication Required", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                CancelClicked?.Invoke();
                return false;
            }
            return true;
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
            ShowFormSafely<UserManagement>("UserManagement");
        }

        public void CustomerManagementFormShow()
        {
            ShowFormSafely<CustomerManagement>("Customers");
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

        public void SupplierManagementFormShow()
        {
            ShowFormSafely<SupplierManagementForm>();
        }

        public void EnhancedBillingFormShow()
        {
            ShowFormSafely<EnhancedBillingForm>();
        }

        public void EnhancedPurchaseFormShow()
        {
            ShowFormSafely<EnhancedPurchaseForm>();
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

        public void ComprehensiveReportsFormShow()
        {
            ComprehensiveReportsForm comprehensiveReportsForm = new ComprehensiveReportsForm();
            comprehensiveReportsForm.Show();
        }



        public void EnhancedPOSFormShow()
        {
            ShowFormSafely<EnhancedPOSForm>("Enhanced POS");
        }

        private void changePasswordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PasswordFormShow();
        }

        // Add missing menu item click events
        private void newBillToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewBillFormShow();
        }

        private void editBillToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditBillFormShow();
        }

        private void salesReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SalesReportFormShow();
        }

        private void creditSaleReporToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreditBillFormShow();
        }

        private void saleReturnsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaleReturnFormShow();
        }

        private void returnReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SalesReturnReportFormShow();
        }

        private void customerAccountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CustomerBalanceFormShow();
        }

        private void customersBalancesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CustomerBalanceFormShow();
        }

        private void bankTransactionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BankTransactionsFormShow();
        }

        private void supplierLedgerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SupplierLedgerFormShow();
        }

        private void supplierBalancesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SupplierLedgerFormShow();
        }

        private void expenseTransactionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExpenseEntryFormShow();
        }

        private void expenseReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExpenseReportFormShow();
        }

        private void stockInHandToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StockInHandFormShow();
        }

        private void gSTReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GSTReportFormShow();
        }

        private void expiredStockReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExpiredStockReportFormShow();
        }

        private void barcodeGeneratorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BarcodeGeneratorFormShow();
        }

        private void purchaseReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PurchaseReportFormShow();
        }

        private void purchaseReturnReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PurchaseReturnReportFormShow();
        }

        private void customerOutstandingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CustomerOutstandingFormShow();
        }

        private void customerPurchaseHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CustomerPurchaseHistoryFormShow();
        }

        private void dailySummaryReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DailySummaryReportFormShow();
        }

        private void barcodeLabelPrintToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BarcodeLabelPrintFormShow();
        }

        private void inventoryReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InventoryReportFormShow();
        }

        private void lowStockAlertReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LowStockAlertReportFormShow();
        }

        private void trialBalanceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TrialBalanceFormShow();
        }

        private void cashFlowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CashFlowFormShow();
        }

        private void userManagementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UserManagementFormShow();
        }

        private void comprehensiveReportsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ComprehensiveReportsFormShow();
        }



        private void enhancedPOSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EnhancedPOSFormShow();
        }

        private void expiryReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExpiryReportFormShow();
        }

        private void customerManagementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CustomerManagementFormShow();
        }

        private void setupSupplierManagementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SupplierManagementFormShow();
        }

        #region Dynamic Date and Time Management

        private Timer dateTimeTimer;
        private Panel statusPanel;
        private Label digitalClock;

        private void InitializeDynamicDate()
        {
            try
            {
                // Initialize and start the timer for updating date and time
                dateTimeTimer = new Timer();
                dateTimeTimer.Interval = 1000; // Update every second
                dateTimeTimer.Tick += DateTimeTimer_Tick;
                dateTimeTimer.Start();
                
                // Create status panel and digital clock
                CreateDashboardEnhancements();
                
                // Update immediately
                UpdateDateTimeDisplay();
                UpdateMainDashboardLabels();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing dynamic date: {ex.Message}");
                // Don't throw - dashboard should still work without dynamic features
                MessageBox.Show($"Warning: Some dashboard features may not work properly.\nError: {ex.Message}", 
                    "Dashboard Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void DateTimeTimer_Tick(object sender, EventArgs e)
        {
            UpdateDateTimeDisplay();
            
            // Update status panel every 5 seconds (to reduce database load)
            if (DateTime.Now.Second % 5 == 0)
            {
                DashboardStatusHelper.UpdateStatusPanel(statusPanel);
            }
            
            // Update digital clock every second
            DashboardStatusHelper.UpdateDigitalClock(digitalClock);
            
            // Update main dashboard label6 (Last Invoice) every 10 seconds
            if (DateTime.Now.Second % 10 == 0)
            {
                UpdateMainDashboardLabels();
            }
        }

        private void CreateDashboardEnhancements()
        {
            try
            {
                // Create status panel
                statusPanel = DashboardStatusHelper.CreateStatusPanel(this);
                
                // Create digital clock in a good position
                Point clockLocation = new Point(this.Width - 250, this.Height - 80);
                digitalClock = DashboardStatusHelper.CreateDigitalClock(this, clockLocation);
                
                // Handle resize events to reposition elements
                this.Resize += UserDashboard_Resize;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating dashboard enhancements: {ex.Message}");
            }
        }

        private void UserDashboard_Resize(object sender, EventArgs e)
        {
            try
            {
                // Reposition status panel
                if (statusPanel != null)
                {
                    statusPanel.Location = new Point(this.Width - 320, 20);
                }
                
                // Reposition digital clock
                if (digitalClock != null)
                {
                    digitalClock.Location = new Point(this.Width - 250, this.Height - 80);
                }
            }
            catch
            {
                // Ignore resize errors
            }
        }

        private void UpdateMainDashboardLabels()
        {
            try
            {
                // Run in background to avoid blocking UI
                System.Threading.Tasks.Task.Run(() =>
                {
                    try
                    {
                        // Get the latest invoice number
                        string query = @"SELECT TOP 1 BillNumber 
                                       FROM Sales 
                                       WHERE IsActive = 1 
                                       ORDER BY SaleID DESC";
                        
                        DataTable result = DatabaseConnection.ExecuteQuery(query);
                        
                        if (result.Rows.Count > 0)
                        {
                            string lastBillNumber = SafeDataHelper.SafeToString(result.Rows[0]["BillNumber"]);
                            
                            // Extract numeric part for display
                            string displayNumber = lastBillNumber;
                            if (lastBillNumber.StartsWith("BILL"))
                            {
                                string numericPart = lastBillNumber.Substring(4);
                                if (int.TryParse(numericPart, out int billNum))
                                {
                                    displayNumber = billNum.ToString("N0"); // Format with commas
                                }
                            }
                            
                            // Update UI on main thread
                            if (this.InvokeRequired)
                            {
                                this.Invoke(new Action(() =>
                                {
                                    // Find label6 (the invoice number label)
                                    var label6 = this.Controls.Find("label6", true).FirstOrDefault() as Label;
                                    if (label6 != null)
                                    {
                                        label6.Text = displayNumber;
                                        label6.ForeColor = Color.Lime; // Make it green to show it's dynamic
                                        label6.Font = new Font(label6.Font, FontStyle.Bold);
                                    }
                                }));
                            }
                        }
                        else
                        {
                            // No invoices found
                            if (this.InvokeRequired)
                            {
                                this.Invoke(new Action(() =>
                                {
                                    var label6 = this.Controls.Find("label6", true).FirstOrDefault() as Label;
                                    if (label6 != null)
                                    {
                                        label6.Text = "0";
                                        label6.ForeColor = Color.Gray;
                                    }
                                }));
                            }
                        }
                    }
                    catch
                    {
                        // Ignore database errors
                    }
                });
            }
            catch
            {
                // Ignore any errors
            }
        }

        private void UpdateDateTimeDisplay()
        {
            try
            {
                // Update the main date label with current date and time
                DateTime now = DateTime.Now;
                
                // Format: "Monday, December 25, 2024 | 2:30:45 PM"
                string dayName = now.ToString("dddd");
                string monthName = now.ToString("MMMM");
                string formattedDate = $"{dayName}, {monthName} {now.Day}, {now.Year}";
                string formattedTime = now.ToString("h:mm:ss tt");
                
                // Update the main date label (label4) with attractive formatting
                var dateLabel = this.Controls.Find("label4", true).FirstOrDefault() as Label;
                if (dateLabel != null)
                {
                    dateLabel.Text = $"{formattedDate} | {formattedTime}";
                    
                    // Make the label more attractive
                    dateLabel.Font = new Font("Segoe UI", 12, FontStyle.Bold);
                    dateLabel.ForeColor = Color.White;
                    
                    // Add a subtle shadow effect by creating a background label
                    var parentControl = dateLabel.Parent;
                    var shadowLabel = parentControl.Controls.Find("label4_shadow", true).FirstOrDefault() as Label;
                    if (shadowLabel == null)
                    {
                        shadowLabel = new Label();
                        shadowLabel.Name = "label4_shadow";
                        shadowLabel.AutoSize = false;
                        shadowLabel.Size = dateLabel.Size;
                        shadowLabel.Location = new Point(dateLabel.Location.X + 2, dateLabel.Location.Y + 2);
                        shadowLabel.Text = dateLabel.Text;
                        shadowLabel.Font = dateLabel.Font;
                        shadowLabel.ForeColor = Color.FromArgb(100, 0, 0, 0); // Semi-transparent black
                        shadowLabel.BackColor = Color.Transparent;
                        shadowLabel.SendToBack();
                        parentControl.Controls.Add(shadowLabel);
                    }
                    else
                    {
                        shadowLabel.Text = dateLabel.Text;
                        shadowLabel.Size = dateLabel.Size;
                        shadowLabel.Location = new Point(dateLabel.Location.X + 2, dateLabel.Location.Y + 2);
                    }
                }
                
                // Also update the window title with current time
                if (UserSession.IsLoggedIn && this.ParentForm != null)
                {
                    string fullName = UserSession.FullName ?? "User";
                    this.ParentForm.Text = $"Pharmacy Management System - {fullName} | {formattedTime}";
                }
            }
            catch (Exception ex)
            {
                // Ignore any errors in date/time update to avoid disrupting the application
                System.Diagnostics.Debug.WriteLine($"Error updating date/time: {ex.Message}");
            }
        }



        // Purchase menu event handlers
        private void newPurchaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewPurchaseFormShow();
        }

        private void purchaseHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Show purchase history form - placeholder for now
            MessageBox.Show("Purchase History functionality will be implemented here.", "Purchase History", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void supplierManagementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SupplierManagementFormShow();
        }

        // Backup menu event handlers
        private void databaseBackupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show("Database backup functionality will be implemented here.", "Database Backup", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                // Future implementation: ShowFormSafely<DatabaseBackupForm>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error accessing backup: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void databaseRestoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show("Database restore functionality will be implemented here.", "Database Restore", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                // Future implementation: ShowFormSafely<DatabaseRestoreForm>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error accessing restore: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show("Data export functionality will be implemented here.", "Data Export", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                // Future implementation: ShowFormSafely<DataExportForm>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error accessing export: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // New menu event handlers (only for ones that don't already exist)
        private void printBillToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Print Bill (Laser) functionality will be implemented here.", "Print Bill", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void printSaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Print Sale Return functionality will be implemented here.", "Print Sale Return", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void bankLedgerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Bank Ledger functionality will be implemented here.", "Bank Ledger", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void suppliersPaymentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Supplier's Payments functionality will be implemented here.", "Supplier Payments", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void supplierToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Supplier Payments Report functionality will be implemented here.", "Supplier Payments Report", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void updateStockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Update Stock functionality will be implemented here.", "Update Stock", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void genericSearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Generic Search functionality will be implemented here.", "Generic Search", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void productSearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Product Search functionality will be implemented here.", "Product Search", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void singleProductDetailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Single Product Detail functionality will be implemented here.", "Product Detail", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void userActivityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("User Activity functionality will be implemented here.", "User Activity", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void customerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CustomerPurchaseHistoryFormShow();
        }

        #endregion
    }
}
