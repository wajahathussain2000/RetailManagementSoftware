-- =============================================================================================================
-- RETAIL MANAGEMENT SYSTEM - CORRECTED TEST DATA SEEDING SCRIPT
-- This script adds sample data based on the actual database schema
-- =============================================================================================================

USE RetailManagementDB;
GO

PRINT 'Starting corrected data seeding for Retail Management System...';

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
-- 2. COMPANIES - Sample pharmaceutical companies (using actual schema)
-- =============================================================================================================
INSERT INTO Companies (CompanyName, ContactPerson, Phone, Email, Address, IsActive, CreatedDate) VALUES
('GSK Pakistan', 'Muhammad Ali', '+92-21-111-475-111', 'contact@gsk.pk', 'Plot 1-9, Sector 30, Korangi Industrial Area, Karachi', 1, '2024-01-01'),
('Searle Pakistan', 'Sara Khan', '+92-21-111-010-111', 'info@searle.com.pk', 'Plot 23-25, Sector 74, SITE, Karachi', 1, '2024-01-01'),
('Abbott Laboratories', 'Dr. Ahmed Shah', '+92-21-111-111-050', 'pakistan@abbott.com', 'Plot 38, Sector 30, Korangi Industrial Area, Karachi', 1, '2024-01-01'),
('Novartis Pharma', 'Ayesha Malik', '+92-21-111-669-111', 'contact@novartis.pk', 'Plot 47-48, Sector 28, Korangi Industrial Area, Karachi', 1, '2024-01-01'),
('Pfizer Pakistan', 'Hassan Sheikh', '+92-21-111-734-937', 'info@pfizer.pk', 'Plot 23, Sector 15, Korangi Industrial Area, Karachi', 1, '2024-01-01'),
('Martin Dow', 'Sana Ahmed', '+92-21-111-627-846', 'contact@martindow.com', 'Plot 20-21, Sector 6A, Korangi Industrial Area, Karachi', 1, '2024-01-01');

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
-- 5. PURCHASES - Sample purchase records (using actual schema)
-- =============================================================================================================
INSERT INTO Purchases (PurchaseNumber, CompanyID, PurchaseDate, TotalAmount, CreatedDate, IsActive) VALUES
('PUR0001', 1, '2024-11-01', 15000.00, '2024-11-01', 1),
('PUR0002', 2, '2024-11-03', 22000.00, '2024-11-03', 1),
('PUR0003', 3, '2024-11-05', 18500.00, '2024-11-05', 1),
('PUR0004', 4, '2024-11-08', 25000.00, '2024-11-08', 1),
('PUR0005', 5, '2024-11-10', 20000.00, '2024-11-10', 1),
('PUR0006', 6, '2024-11-12', 19500.00, '2024-11-12', 1),
('PUR0007', 1, '2024-11-15', 16000.00, '2024-11-15', 1),
('PUR0008', 2, '2024-11-18', 23500.00, '2024-11-18', 1),
('PUR0009', 3, '2024-11-20', 21000.00, '2024-11-20', 1),
('PUR0010', 4, '2024-11-22', 24000.00, '2024-11-22', 1),
('PUR0011', 5, '2024-11-25', 18000.00, '2024-11-25', 1),
('PUR0012', 6, '2024-11-28', 22500.00, '2024-11-28', 1),
('PUR0013', 1, '2024-12-01', 17500.00, '2024-12-01', 1),
('PUR0014', 2, '2024-12-04', 26000.00, '2024-12-04', 1),
('PUR0015', 3, '2024-12-07', 19000.00, '2024-12-07', 1);

-- =============================================================================================================
-- 6. PURCHASE ITEMS - Details for the purchases (using actual schema)
-- =============================================================================================================
INSERT INTO PurchaseItems (PurchaseID, ItemID, Quantity, Price, Amount, ExpiryDate, BatchNumber)
SELECT 
    p.PurchaseID,
    ((p.PurchaseID % 16) + 1), -- Rotate through items 1-16
    (10 + (p.PurchaseID % 20)), -- Quantity between 10-30
    (5.00 + (p.PurchaseID % 25)), -- Unit cost varies
    ((10 + (p.PurchaseID % 20)) * (5.00 + (p.PurchaseID % 25))), -- Total cost
    DATEADD(YEAR, 2, p.PurchaseDate), -- 2 years expiry
    'BATCH' + RIGHT('000' + CAST(p.PurchaseID AS VARCHAR(3)), 3)
FROM Purchases p;

-- =============================================================================================================
-- 7. SALES - Sample sales transactions (using actual schema)
-- =============================================================================================================
-- Generate sales data for the last month
DECLARE @SalesStartDate DATE = '2024-11-01';
DECLARE @SalesCurrentDate DATE = GETDATE();
DECLARE @SalesCounter INT = 1;

WHILE @SalesStartDate <= @SalesCurrentDate
BEGIN
    -- Generate 3-5 sales per day
    DECLARE @DailySales INT = 3 + (@SalesCounter % 3);
    DECLARE @SaleTime TIME = '09:00:00';
    
    DECLARE @DaySaleCounter INT = 1;
    WHILE @DaySaleCounter <= @DailySales
    BEGIN
        DECLARE @CustomerID INT = CASE 
            WHEN @DaySaleCounter <= 2 THEN 1 -- Walk-in customers
            ELSE ((@SalesCounter % 7) + 2) -- Other customers
        END;
        
        DECLARE @TotalAmount DECIMAL(10,2) = 100 + (@SalesCounter * 15) + (@DaySaleCounter * 30);
        DECLARE @Discount DECIMAL(10,2) = @TotalAmount * 0.05;
        DECLARE @NetAmount DECIMAL(10,2) = @TotalAmount - @Discount;
        DECLARE @PaymentMethod NVARCHAR(10) = CASE (@SalesCounter % 3)
            WHEN 0 THEN 'Cash'
            WHEN 1 THEN 'Card' 
            ELSE 'EasyPaisa'
        END;
        DECLARE @IsCredit BIT = CASE WHEN @CustomerID > 1 AND (@SalesCounter % 5) = 0 THEN 1 ELSE 0 END;
        
        INSERT INTO Sales (BillNumber, CustomerID, SaleDate, TotalAmount, Discount, NetAmount, PaymentMethod, IsCredit, Remarks, IsActive, CreatedDate)
        VALUES (
            'BILL' + RIGHT('000000' + CAST(@SalesCounter AS VARCHAR(6)), 6),
            @CustomerID,
            CAST(@SalesStartDate AS DATETIME) + CAST(@SaleTime AS DATETIME),
            @TotalAmount,
            @Discount,
            @NetAmount,
            @PaymentMethod,
            @IsCredit,
            CASE WHEN @IsCredit = 1 THEN 'Credit Sale' ELSE 'Cash Sale' END,
            1,
            CAST(@SalesStartDate AS DATETIME) + CAST(@SaleTime AS DATETIME)
        );
        
        SET @SalesCounter = @SalesCounter + 1;
        SET @DaySaleCounter = @DaySaleCounter + 1;
        SET @SaleTime = DATEADD(HOUR, 2, @SaleTime);
    END
    
    SET @SalesStartDate = DATEADD(DAY, 1, @SalesStartDate);
END

-- =============================================================================================================
-- 8. SALE ITEMS - Details for the sales (using actual schema)
-- =============================================================================================================
INSERT INTO SaleItems (SaleID, ItemID, Quantity, Price, Amount, Discount, BatchNumber)
SELECT 
    s.SaleID,
    ((s.SaleID % 16) + 1), -- Rotate through items
    (1 + (s.SaleID % 3)), -- Quantity 1-3
    (8.00 + (s.SaleID % 25)), -- Unit price varies
    ((1 + (s.SaleID % 3)) * (8.00 + (s.SaleID % 25))), -- Total amount
    (s.SaleID % 5), -- Discount amount
    'BATCH' + RIGHT('000' + CAST((s.SaleID % 50) + 1 AS VARCHAR(3)), 3)
FROM Sales s;

-- =============================================================================================================
-- 9. EXPENSES - Daily operational expenses (using actual schema)
-- =============================================================================================================
INSERT INTO Expenses (ExpenseType, Amount, ExpenseDate, CreatedDate, IsActive)
VALUES
('Utilities', 8500.00, '2024-12-01', '2024-12-01', 1),
('Rent', 45000.00, '2024-12-01', '2024-12-01', 1),
('Staff Salary', 35000.00, '2024-11-30', '2024-11-30', 1),
('Staff Salary', 25000.00, '2024-11-30', '2024-11-30', 1),
('Maintenance', 3500.00, '2024-12-15', '2024-12-15', 1),
('Transportation', 2800.00, '2024-12-20', '2024-12-20', 1),
('Office Supplies', 1500.00, '2024-12-10', '2024-12-10', 1),
('Insurance', 12000.00, '2024-12-05', '2024-12-05', 1),
('Marketing', 4500.00, '2024-12-12', '2024-12-12', 1),
('License Fees', 8000.00, '2024-11-15', '2024-11-15', 1);

-- =============================================================================================================
-- 10. Update customer balances for credit sales
-- =============================================================================================================
UPDATE Customers 
SET CurrentBalance = ISNULL(
    (SELECT SUM(s.NetAmount) 
     FROM Sales s 
     WHERE s.CustomerID = Customers.CustomerID 
     AND s.IsCredit = 1 
     AND s.IsActive = 1), 0
);

PRINT 'Corrected database seeding completed successfully!';
PRINT 'Sample data has been added based on actual table schema.';
PRINT '';
PRINT 'Summary of data added:';
PRINT '- 5 Users (Admin, Pharmacists, Salesmen)';
PRINT '- 6 Companies (Major pharmaceutical companies)';
PRINT '- 8 Customers (Hospitals, clinics, pharmacies)';
PRINT '- 16 Items (Various medicines and products)';
PRINT '- 15 Purchases (Last 2 months)';
PRINT '- Multiple Sales (Last month with daily transactions)';
PRINT '- 10 Expense records';
PRINT '- Updated customer balances for credit sales';
PRINT '';
PRINT 'You can now test all reports with this data!';

