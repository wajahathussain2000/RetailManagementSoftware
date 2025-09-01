-- =============================================================================================================
-- RETAIL MANAGEMENT SYSTEM - TEST DATA SEEDING SCRIPT
-- This script adds comprehensive sample data for testing reports and functionality
-- =============================================================================================================

USE RetailManagementDB;
GO

PRINT 'Starting data seeding for Retail Management System...';

-- =============================================================================================================
-- 1. USERS - Sample users with different roles
-- =============================================================================================================
INSERT INTO Users (Username, Password, FullName, Email, Role, IsActive, CreatedDate, LastLoginDate) VALUES
('admin', 'admin123', 'System Administrator', 'admin@pharmacy.com', 'Admin', 1, '2024-01-01', '2024-12-26'),
('pharmacist1', 'pharm123', 'Dr. Sarah Ahmed', 'sarah@pharmacy.com', 'Pharmacist', 1, '2024-01-15', '2024-12-25'),
('salesman1', 'sales123', 'Ahmed Khan', 'ahmed@pharmacy.com', 'Salesman', 1, '2024-02-01', '2024-12-24'),
('pharmacist2', 'pharm456', 'Dr. Ali Hassan', 'ali@pharmacy.com', 'Pharmacist', 1, '2024-01-20', '2024-12-23'),
('salesman2', 'sales456', 'Fatima Sheikh', 'fatima@pharmacy.com', 'Salesman', 1, '2024-02-15', '2024-12-22');

-- =============================================================================================================
-- 2. COMPANIES - Sample pharmaceutical companies
-- =============================================================================================================
INSERT INTO Companies (CompanyName, ContactPerson, Phone, Email, Address, City, Country, IsActive) VALUES
('GSK Pakistan', 'Muhammad Ali', '+92-21-111-475-111', 'contact@gsk.pk', 'Plot 1-9, Sector 30, Korangi Industrial Area', 'Karachi', 'Pakistan', 1),
('Searle Pakistan', 'Sara Khan', '+92-21-111-010-111', 'info@searle.com.pk', 'Plot 23-25, Sector 74, SITE', 'Karachi', 'Pakistan', 1),
('Abbott Laboratories', 'Dr. Ahmed Shah', '+92-21-111-111-050', 'pakistan@abbott.com', 'Plot 38, Sector 30, Korangi Industrial Area', 'Karachi', 'Pakistan', 1),
('Novartis Pharma', 'Ayesha Malik', '+92-21-111-669-111', 'contact@novartis.pk', 'Plot 47-48, Sector 28, Korangi Industrial Area', 'Karachi', 'Pakistan', 1),
('Pfizer Pakistan', 'Hassan Sheikh', '+92-21-111-734-937', 'info@pfizer.pk', 'Plot 23, Sector 15, Korangi Industrial Area', 'Karachi', 'Pakistan', 1),
('Martin Dow', 'Sana Ahmed', '+92-21-111-627-846', 'contact@martindow.com', 'Plot 20-21, Sector 6A, Korangi Industrial Area', 'Karachi', 'Pakistan', 1);

-- =============================================================================================================
-- 3. CUSTOMERS - Sample customers with different profiles
-- =============================================================================================================
INSERT INTO Customers (CustomerName, Phone, Email, Address, City, State, PostalCode, GST_Number, CreditLimit, CreditDays, CurrentBalance, IsActive, CreatedDate) VALUES
('Walk-in Customer', '', '', '', '', '', '', '', 0.00, 0, 0.00, 1, '2024-01-01'),
('City Hospital', '+92-21-111-222-333', 'procurement@cityhospital.pk', 'Main University Road', 'Karachi', 'Sindh', '75270', 'GST-001-234-567', 500000.00, 30, 25000.00, 1, '2024-01-05'),
('Green Clinic', '+92-21-444-555-666', 'orders@greenclinic.pk', 'Block A, Gulshan-e-Iqbal', 'Karachi', 'Sindh', '75300', 'GST-002-345-678', 100000.00, 15, 5000.00, 1, '2024-01-10'),
('Ahmed Medical Store', '+92-21-777-888-999', 'ahmed@medstore.pk', 'Tariq Road', 'Karachi', 'Sindh', '75400', 'GST-003-456-789', 200000.00, 20, 15000.00, 1, '2024-01-15'),
('Liaquat National Hospital', '+92-21-111-456-789', 'pharmacy@lnh.edu.pk', 'Stadium Road', 'Karachi', 'Sindh', '74800', 'GST-004-567-890', 750000.00, 45, 45000.00, 1, '2024-01-20'),
('Dr. Fatima Clinic', '+92-300-123-4567', 'fatima.clinic@gmail.com', 'Defence Phase 2', 'Karachi', 'Sindh', '75500', '', 50000.00, 10, 2500.00, 1, '2024-02-01'),
('Medicare Pharmacy', '+92-21-987-654-321', 'info@medicare.pk', 'Nazimabad Block 2', 'Karachi', 'Sindh', '74600', 'GST-005-678-901', 150000.00, 25, 8000.00, 1, '2024-02-10'),
('Express Pharmacy Network', '+92-21-123-987-456', 'orders@express-pharma.pk', 'Clifton Block 5', 'Karachi', 'Sindh', '75600', 'GST-006-789-012', 300000.00, 30, 18000.00, 1, '2024-02-15');

-- =============================================================================================================
-- 4. ITEMS - Sample medicines and healthcare products
-- =============================================================================================================
INSERT INTO Items (ItemName, Description, Category, CompanyID, Barcode, Price, MRP, GST_Rate, StockQuantity, ReorderLevel, MinStock, MaxStock, UnitType, HSN_Code, PackSize, IsPrescriptionRequired, IsActive, CreatedDate) VALUES
-- Antibiotics
('Augmentin 625mg Tab', 'Amoxicillin + Clavulanic Acid Tablets', 'Antibiotics', 1, '123456789001', 25.50, 30.00, 17.0, 500, 50, 20, 1000, 'Tablet', '30049099', 10, 1, 1, '2024-01-01'),
('Azithromycin 500mg Tab', 'Azithromycin Tablets', 'Antibiotics', 2, '123456789002', 18.75, 22.00, 17.0, 300, 30, 15, 600, 'Tablet', '30049099', 6, 1, 1, '2024-01-01'),
('Cephalexin 500mg Cap', 'Cephalexin Capsules', 'Antibiotics', 3, '123456789003', 12.50, 15.00, 17.0, 250, 25, 10, 500, 'Capsule', '30049099', 8, 1, 1, '2024-01-01'),

-- Pain Relief
('Panadol Extra Tab', 'Paracetamol + Caffeine Tablets', 'Pain Relief', 1, '123456789004', 3.50, 4.00, 17.0, 1000, 100, 50, 2000, 'Tablet', '30049099', 20, 0, 1, '2024-01-01'),
('Brufen 400mg Tab', 'Ibuprofen Tablets', 'Pain Relief', 4, '123456789005', 2.75, 3.25, 17.0, 800, 80, 40, 1600, 'Tablet', '30049099', 20, 0, 1, '2024-01-01'),
('Disprin Tab', 'Aspirin Tablets', 'Pain Relief', 5, '123456789006', 1.50, 2.00, 17.0, 1200, 120, 60, 2400, 'Tablet', '30049099', 20, 0, 1, '2024-01-01'),

-- Cardiovascular
('Concor 5mg Tab', 'Bisoprolol Tablets', 'Cardiovascular', 6, '123456789007', 8.50, 10.00, 17.0, 200, 20, 10, 400, 'Tablet', '30049099', 30, 1, 1, '2024-01-01'),
('Lipitor 20mg Tab', 'Atorvastatin Tablets', 'Cardiovascular', 4, '123456789008', 15.75, 18.50, 17.0, 150, 15, 8, 300, 'Tablet', '30049099', 30, 1, 1, '2024-01-01'),

-- Diabetes
('Glucophage 500mg Tab', 'Metformin Tablets', 'Diabetes', 2, '123456789009', 4.25, 5.00, 17.0, 400, 40, 20, 800, 'Tablet', '30049099', 60, 1, 1, '2024-01-01'),
('Januvia 100mg Tab', 'Sitagliptin Tablets', 'Diabetes', 6, '123456789010', 32.50, 38.00, 17.0, 100, 10, 5, 200, 'Tablet', '30049099', 28, 1, 1, '2024-01-01'),

-- Vitamins & Supplements
('Centrum Multivitamin', 'Complete Multivitamin Tablets', 'Vitamins', 4, '123456789011', 12.00, 14.50, 17.0, 300, 30, 15, 600, 'Tablet', '30049099', 30, 0, 1, '2024-01-01'),
('Vitamin D3 1000 IU', 'Cholecalciferol Tablets', 'Vitamins', 1, '123456789012', 8.75, 10.25, 17.0, 250, 25, 12, 500, 'Tablet', '30049099', 30, 0, 1, '2024-01-01'),

-- Cough & Cold
('Actifed Plus Syrup', 'Cough & Cold Syrup', 'Cough & Cold', 3, '123456789013', 45.50, 52.00, 17.0, 80, 8, 4, 160, 'Bottle', '30049099', 1, 0, 1, '2024-01-01'),
('Mucolite Syrup', 'Mucolytic Expectorant', 'Cough & Cold', 5, '123456789014', 38.25, 44.00, 17.0, 90, 9, 5, 180, 'Bottle', '30049099', 1, 0, 1, '2024-01-01'),

-- Topical
('Betnovate Cream', 'Betamethasone Cream', 'Topical', 1, '123456789015', 28.50, 33.00, 17.0, 120, 12, 6, 240, 'Tube', '30049099', 1, 1, 1, '2024-01-01'),
('Soframycin Ointment', 'Framycetin Ointment', 'Topical', 2, '123456789016', 22.75, 26.50, 17.0, 100, 10, 5, 200, 'Tube', '30049099', 1, 0, 1, '2024-01-01');

-- =============================================================================================================
-- 5. PURCHASES - Sample purchase records from different companies
-- =============================================================================================================
-- Generate Purchase data for last 3 months
DECLARE @PurchaseStartDate DATE = '2024-10-01';
DECLARE @CurrentDate DATE = GETDATE();
DECLARE @PurchaseCounter INT = 1;

WHILE @PurchaseStartDate <= @CurrentDate
BEGIN
    -- Insert 2-3 purchases per week
    IF DATEPART(WEEKDAY, @PurchaseStartDate) IN (2, 4, 6) -- Monday, Wednesday, Friday
    BEGIN
        INSERT INTO Purchases (PurchaseNumber, CompanyID, UserID, PurchaseDate, TotalAmount, PaidAmount, Status, PaymentStatus, IsActive, CreatedDate)
        VALUES 
        (
            'PUR' + RIGHT('0000' + CAST(@PurchaseCounter AS VARCHAR(4)), 4),
            ((@PurchaseCounter % 6) + 1), -- Rotate through companies 1-6
            ((@PurchaseCounter % 4) + 1), -- Rotate through users 1-4
            @PurchaseStartDate,
            (1000 + (@PurchaseCounter * 150)), -- Varying amounts
            (1000 + (@PurchaseCounter * 150)), -- Paid in full
            'Completed',
            'Paid',
            1,
            @PurchaseStartDate
        );
        SET @PurchaseCounter = @PurchaseCounter + 1;
    END
    SET @PurchaseStartDate = DATEADD(DAY, 1, @PurchaseStartDate);
END

-- =============================================================================================================
-- 6. PURCHASE ITEMS - Details for the purchases
-- =============================================================================================================
INSERT INTO PurchaseItems (PurchaseID, ItemID, Quantity, UnitCost, TotalCost, ExpiryDate, BatchNumber, PackSize)
SELECT 
    p.PurchaseID,
    ((p.PurchaseID % 16) + 1), -- Rotate through items 1-16
    (10 + (p.PurchaseID % 20)), -- Quantity between 10-30
    (5.00 + (p.PurchaseID % 25)), -- Unit cost varies
    ((10 + (p.PurchaseID % 20)) * (5.00 + (p.PurchaseID % 25))), -- Total cost
    DATEADD(YEAR, 2, p.PurchaseDate), -- 2 years expiry
    'BATCH' + RIGHT('000' + CAST(p.PurchaseID AS VARCHAR(3)), 3),
    (p.PurchaseID % 20) + 10 -- Pack size varies
FROM Purchases p;

-- =============================================================================================================
-- 7. SALES - Sample sales transactions
-- =============================================================================================================
DECLARE @SalesStartDate DATE = '2024-11-01';
DECLARE @SalesCurrentDate DATE = GETDATE();
DECLARE @SalesCounter INT = 1;

WHILE @SalesStartDate <= @SalesCurrentDate
BEGIN
    -- Generate 5-8 sales per day
    DECLARE @DailySales INT = 5 + (@SalesCounter % 4);
    DECLARE @SaleTime TIME = '09:00:00';
    
    DECLARE @DaySaleCounter INT = 1;
    WHILE @DaySaleCounter <= @DailySales
    BEGIN
        DECLARE @CustomerID INT = CASE 
            WHEN @DaySaleCounter <= 3 THEN 1 -- Walk-in customers
            ELSE ((@SalesCounter % 7) + 2) -- Other customers
        END;
        
        DECLARE @NetAmount DECIMAL(10,2) = 150 + (@SalesCounter * 25) + (@DaySaleCounter * 50);
        DECLARE @PaymentMethod NVARCHAR(10) = CASE (@SalesCounter % 3)
            WHEN 0 THEN 'Cash'
            WHEN 1 THEN 'Card' 
            ELSE 'EasyPaisa'
        END;
        
        INSERT INTO Sales (BillNumber, CustomerID, UserID, SaleDate, GrossAmount, DiscountAmount, TaxableAmount, CGST, SGST, IGST, NetAmount, PaymentStatus, PaymentMethod, CashAmount, CardAmount, EasyPaisaAmount, IsActive, CreatedDate)
        VALUES (
            'BILL' + RIGHT('000000' + CAST(@SalesCounter AS VARCHAR(6)), 6),
            @CustomerID,
            ((@SalesCounter % 4) + 1),
            CAST(@SalesStartDate AS DATETIME) + CAST(@SaleTime AS DATETIME),
            @NetAmount * 0.85, -- Gross before tax
            @NetAmount * 0.05, -- 5% discount
            @NetAmount * 0.80, -- Taxable amount
            @NetAmount * 0.09, -- 9% CGST
            @NetAmount * 0.09, -- 9% SGST
            0.00, -- No IGST
            @NetAmount,
            'Paid',
            @PaymentMethod,
            CASE WHEN @PaymentMethod = 'Cash' THEN @NetAmount ELSE 0.00 END,
            CASE WHEN @PaymentMethod = 'Card' THEN @NetAmount ELSE 0.00 END,
            CASE WHEN @PaymentMethod = 'EasyPaisa' THEN @NetAmount ELSE 0.00 END,
            1,
            CAST(@SalesStartDate AS DATETIME) + CAST(@SaleTime AS DATETIME)
        );
        
        SET @SalesCounter = @SalesCounter + 1;
        SET @DaySaleCounter = @DaySaleCounter + 1;
        SET @SaleTime = DATEADD(HOUR, 1, @SaleTime);
    END
    
    SET @SalesStartDate = DATEADD(DAY, 1, @SalesStartDate);
END

-- =============================================================================================================
-- 8. SALE ITEMS - Details for the sales
-- =============================================================================================================
INSERT INTO SaleItems (SaleID, ItemID, Quantity, UnitPrice, MRP, Discount, DiscountPercent, TaxableAmount, GST_Rate, CGST, SGST, IGST, TotalAmount, FreeQuantity, BatchNumber)
SELECT 
    s.SaleID,
    ((s.SaleID % 16) + 1), -- Rotate through items
    (1 + (s.SaleID % 5)), -- Quantity 1-5
    (10.00 + (s.SaleID % 30)), -- Unit price varies
    (12.00 + (s.SaleID % 35)), -- MRP slightly higher
    (s.SaleID % 10), -- Discount amount
    5.00, -- 5% discount
    ((1 + (s.SaleID % 5)) * (10.00 + (s.SaleID % 30)) * 0.95), -- Taxable amount
    17.0, -- GST Rate
    ((1 + (s.SaleID % 5)) * (10.00 + (s.SaleID % 30)) * 0.95 * 0.085), -- CGST 8.5%
    ((1 + (s.SaleID % 5)) * (10.00 + (s.SaleID % 30)) * 0.95 * 0.085), -- SGST 8.5%
    0.00, -- IGST
    ((1 + (s.SaleID % 5)) * (10.00 + (s.SaleID % 30))), -- Total amount
    CASE WHEN s.SaleID % 10 = 0 THEN 1 ELSE 0 END, -- Free quantity occasionally
    'BATCH' + RIGHT('000' + CAST((s.SaleID % 50) + 1 AS VARCHAR(3)), 3)
FROM Sales s;

-- =============================================================================================================
-- 9. CUSTOMER PAYMENTS - Payment records
-- =============================================================================================================
INSERT INTO CustomerPayments (CustomerID, UserID, PaymentDate, Amount, PaymentMethod, Description, IsActive, CreatedDate)
SELECT 
    (PaymentCounter % 7) + 2, -- Customers 2-8
    ((PaymentCounter % 4) + 1), -- Users 1-4
    DATEADD(DAY, -(PaymentCounter % 30), GETDATE()), -- Last 30 days
    (1000 + (PaymentCounter * 500)), -- Payment amounts
    CASE (PaymentCounter % 3) WHEN 0 THEN 'Cash' WHEN 1 THEN 'Bank Transfer' ELSE 'Cheque' END,
    'Payment against outstanding amount',
    1,
    DATEADD(DAY, -(PaymentCounter % 30), GETDATE())
FROM (SELECT ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS PaymentCounter FROM Sales WHERE SaleID <= 20) AS PaymentData;

-- =============================================================================================================
-- 10. EXPENSES - Daily operational expenses
-- =============================================================================================================
INSERT INTO Expenses (ExpenseType, Description, Amount, ExpenseDate, UserID, IsActive, CreatedDate)
VALUES
('Utilities', 'Electricity bill for December 2024', 8500.00, '2024-12-01', 1, 1, '2024-12-01'),
('Rent', 'Shop rent for December 2024', 45000.00, '2024-12-01', 1, 1, '2024-12-01'),
('Staff Salary', 'Salary for pharmacist - November', 35000.00, '2024-11-30', 1, 1, '2024-11-30'),
('Staff Salary', 'Salary for salesman - November', 25000.00, '2024-11-30', 1, 1, '2024-11-30'),
('Maintenance', 'AC maintenance and repair', 3500.00, '2024-12-15', 2, 1, '2024-12-15'),
('Transportation', 'Fuel and transportation costs', 2800.00, '2024-12-20', 3, 1, '2024-12-20'),
('Office Supplies', 'Stationary and printing materials', 1500.00, '2024-12-10', 2, 1, '2024-12-10'),
('Insurance', 'Shop insurance premium', 12000.00, '2024-12-05', 1, 1, '2024-12-05'),
('Marketing', 'Promotional materials and advertising', 4500.00, '2024-12-12', 1, 1, '2024-12-12'),
('License Fees', 'Pharmacy license renewal', 8000.00, '2024-11-15', 1, 1, '2024-11-15');

-- =============================================================================================================
-- 11. BANK TRANSACTIONS - Banking records
-- =============================================================================================================
INSERT INTO BankTransactions (TransactionType, Description, Amount, TransactionDate, AccountNumber, BankName, UserID, IsActive, CreatedDate)
VALUES
('Deposit', 'Daily sales deposit', 25000.00, '2024-12-25', 'ACC-001-234567', 'HBL Main Branch', 1, 1, '2024-12-25'),
('Deposit', 'Customer payment - City Hospital', 50000.00, '2024-12-24', 'ACC-001-234567', 'HBL Main Branch', 2, 1, '2024-12-24'),
('Withdrawal', 'Cash for daily operations', 15000.00, '2024-12-23', 'ACC-001-234567', 'HBL Main Branch', 1, 1, '2024-12-23'),
('Payment', 'Supplier payment - GSK Pakistan', 75000.00, '2024-12-22', 'ACC-001-234567', 'HBL Main Branch', 1, 1, '2024-12-22'),
('Deposit', 'Weekly sales collection', 180000.00, '2024-12-21', 'ACC-001-234567', 'HBL Main Branch', 1, 1, '2024-12-21'),
('Payment', 'Rent payment via bank transfer', 45000.00, '2024-12-01', 'ACC-001-234567', 'HBL Main Branch', 1, 1, '2024-12-01'),
('Payment', 'Utility bill payment', 8500.00, '2024-12-01', 'ACC-001-234567', 'HBL Main Branch', 1, 1, '2024-12-01');

-- =============================================================================================================
-- 12. SALE RETURNS - Return transactions
-- =============================================================================================================
INSERT INTO SaleReturns (OriginalSaleID, CustomerID, UserID, ReturnDate, ReturnReason, TotalAmount, RefundAmount, Status, IsActive, CreatedDate)
SELECT 
    s.SaleID,
    s.CustomerID,
    s.UserID,
    DATEADD(DAY, 2, s.SaleDate), -- Return after 2 days
    CASE (s.SaleID % 4) 
        WHEN 0 THEN 'Damaged product'
        WHEN 1 THEN 'Wrong medicine'
        WHEN 2 THEN 'Customer changed mind'
        ELSE 'Expired product'
    END,
    s.NetAmount * 0.3, -- 30% of original amount
    s.NetAmount * 0.3,
    'Completed',
    1,
    DATEADD(DAY, 2, s.SaleDate)
FROM Sales s 
WHERE s.SaleID % 15 = 0 -- Only 1 in 15 sales have returns
AND s.SaleID <= 30; -- Limit to first 30 sales

-- =============================================================================================================
-- 13. EXPIRY ALERTS - Products nearing expiry
-- =============================================================================================================
INSERT INTO ExpiryAlerts (ItemID, BatchNumber, ExpiryDate, DaysToExpiry, AlertLevel, IsActive, CreatedDate)
SELECT 
    i.ItemID,
    'BATCH' + RIGHT('000' + CAST(i.ItemID AS VARCHAR(3)), 3),
    DATEADD(MONTH, 6, GETDATE()), -- 6 months from now
    180, -- Days to expiry
    CASE 
        WHEN i.ItemID % 3 = 0 THEN 'High'
        WHEN i.ItemID % 3 = 1 THEN 'Medium'
        ELSE 'Low'
    END,
    1,
    GETDATE()
FROM Items i
WHERE i.ItemID % 4 = 0; -- 1 in 4 items have expiry alerts

-- =============================================================================================================
-- 14. UPDATE ITEM STOCK QUANTITIES BASED ON SALES
-- =============================================================================================================
UPDATE Items 
SET StockQuantity = StockQuantity - ISNULL(
    (SELECT SUM(si.Quantity) 
     FROM SaleItems si 
     INNER JOIN Sales s ON si.SaleID = s.SaleID 
     WHERE si.ItemID = Items.ItemID AND s.IsActive = 1), 0
)
WHERE ItemID IN (SELECT DISTINCT ItemID FROM SaleItems);

-- Add stock from purchases
UPDATE Items 
SET StockQuantity = StockQuantity + ISNULL(
    (SELECT SUM(pi.Quantity) 
     FROM PurchaseItems pi 
     INNER JOIN Purchases p ON pi.PurchaseID = p.PurchaseID 
     WHERE pi.ItemID = Items.ItemID AND p.IsActive = 1), 0
)
WHERE ItemID IN (SELECT DISTINCT ItemID FROM PurchaseItems);

-- =============================================================================================================
-- 15. UPDATE CUSTOMER BALANCES
-- =============================================================================================================
UPDATE Customers 
SET CurrentBalance = ISNULL(
    (SELECT SUM(s.NetAmount) 
     FROM Sales s 
     WHERE s.CustomerID = Customers.CustomerID 
     AND s.PaymentStatus = 'Pending' 
     AND s.IsActive = 1), 0
) - ISNULL(
    (SELECT SUM(cp.Amount) 
     FROM CustomerPayments cp 
     WHERE cp.CustomerID = Customers.CustomerID 
     AND cp.IsActive = 1), 0
);

PRINT 'Database seeding completed successfully!';
PRINT 'Sample data has been added to all major tables for testing reports.';
PRINT '';
PRINT 'Summary of data added:';
PRINT '- 5 Users (Admin, Pharmacists, Salesmen)';
PRINT '- 6 Companies (Major pharmaceutical companies)';
PRINT '- 8 Customers (Hospitals, clinics, pharmacies)';
PRINT '- 16 Items (Various medicines and products)';
PRINT '- Multiple Purchases (Last 3 months)';
PRINT '- Multiple Sales (Last month with daily transactions)';
PRINT '- Customer Payments';
PRINT '- Expenses (Monthly operational costs)';
PRINT '- Bank Transactions';
PRINT '- Sale Returns';
PRINT '- Expiry Alerts';
PRINT '';
PRINT 'You can now test all reports with this comprehensive data!';
