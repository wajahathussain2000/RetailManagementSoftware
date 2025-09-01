# PowerShell script to update all form show methods in UserDashboard.cs
# This will replace all the form creation patterns with the safe ShowFormSafely<T>() pattern

$filePath = "RetailManagement\UserDashboard.cs"

# Read the file content
$content = Get-Content $filePath -Raw

# Define the replacements
$replacements = @{
    "public void CustomerLedgerFormShow()
        {
            CustomerLedger customerLedger = new CustomerLedger();
            customerLedger.Show();
        }" = "public void CustomerLedgerFormShow()
        {
            ShowFormSafely<CustomerLedger>();
        }"
    
    "public void PasswordFormShow()
        {
            PasswordForm password = new PasswordForm();
            password.Show();
        }" = "public void PasswordFormShow()
        {
            ShowFormSafely<PasswordForm>();
        }"
    
    "public void StockInHandFormShow()
        {
            StockInHand stockInHand = new StockInHand();
            stockInHand.Show();
        }" = "public void StockInHandFormShow()
        {
            ShowFormSafely<StockInHand>();
        }"
    
    "public void SaleReturnFormShow()
        {
            SaleReturn saleReturn = new SaleReturn();
            saleReturn.Show();
        }" = "public void SaleReturnFormShow()
        {
            ShowFormSafely<SaleReturn>();
        }"
    
    "public void CustomerBalanceFormShow()
        {
            CustomerBalance customerBalance = new CustomerBalance();
            customerBalance.Show();
        }" = "public void CustomerBalanceFormShow()
        {
            ShowFormSafely<CustomerBalance>();
        }"
    
    "public void SalesReportFormShow()
        {
            SalesReport salesReport = new SalesReport();
            salesReport.Show();
        }" = "public void SalesReportFormShow()
        {
            ShowFormSafely<SalesReport>();
        }"
    
    "public void PurchaseReturnFormShow()
        {
            PurchaseReturn purchaseReturn = new PurchaseReturn();
            purchaseReturn.Show();
        }" = "public void PurchaseReturnFormShow()
        {
            ShowFormSafely<PurchaseReturn>();
        }"
    
    "public void ExpenseEntryFormShow()
        {
            ExpenseEntry expenseEntry = new ExpenseEntry();
            expenseEntry.Show();
        }" = "public void ExpenseEntryFormShow()
        {
            ShowFormSafely<ExpenseEntry>();
        }"
}

# Apply all replacements
foreach ($find in $replacements.Keys) {
    $replace = $replacements[$find]
    $content = $content -replace [regex]::Escape($find), $replace
}

# Write the updated content back to the file
Set-Content $filePath $content -Encoding UTF8

Write-Host "Successfully updated all form show methods in UserDashboard.cs"
Write-Host "All forms now use the safe ShowFormSafely<T>() pattern"
