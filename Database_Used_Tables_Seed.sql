-- =============================================================================================================
-- RETAIL MANAGEMENT SYSTEM - FOCUSED TEST DATA SEED
-- This script seeds ONLY the tables that are actively used in the application
-- Based on code analysis of the C# application
-- =============================================================================================================

USE RetailManagementDB;
GO

PRINT 'Starting focused data seeding for tables used in application...';

-- =============================================================================================================
-- 1. USERS - Required for login and user tracking
-- =============================================================================================================
IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = 'admin')
INSERT INTO Users (Username, Password, FullName, Role, IsActive, CreatedDate) VALUES
('admin', 'admin123', 'System Administrator', 'Admin', 1, GETDATE());

IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = 'pharmacist1')
INSERT INTO Users (Username, Password, FullName, Role, IsActive, CreatedDate) VALUES
('pharmacist1', 'pharm123', 'Dr. Sarah Ahmed', 'Pharmacist', 1, GETDATE());

IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = 'salesman1')
INSERT INTO Users (Username, Password, FullName, Role, IsActive, CreatedDate) VALUES
('salesman1', 'sales123', 'Ahmed Khan', 'Salesman', 1, GETDATE());

-- =============================================================================================================
-- 2. COMPANIES - Used in NewPurchase.cs for supplier selection
-- =============================================================================================================
IF NOT EXISTS (SELECT 1 FROM Companies WHERE CompanyName = 'GSK Pakistan')
INSERT INTO Companies (CompanyName, ContactPerson, Phone, Email, Address, IsActive, CreatedDate) VALUES
('GSK Pakistan', 'Muhammad Ali', '+92-21-111-475-111', 'contact@gsk.pk', 'Karachi Industrial Area', 1, GETDATE());

IF NOT EXISTS (SELECT 1 FROM Companies WHERE CompanyName = 'Searle Pakistan')
INSERT INTO Companies (CompanyName, ContactPerson, Phone, Email, Address, IsActive, CreatedDate) VALUES
('Searle Pakistan', 'Sara Khan', '+92-21-111-010-111', 'info@searle.com.pk', 'Karachi SITE Area', 1, GETDATE());

IF NOT EXISTS (SELECT 1 FROM Companies WHERE CompanyName = 'Abbott Laboratories')
INSERT INTO Companies (CompanyName, ContactPerson, Phone, Email, Address, IsActive, CreatedDate) VALUES
('Abbott Laboratories', 'Dr. Ahmed Shah', '+92-21-111-111-050', 'pakistan@abbott.com', 'Karachi Industrial Area', 1, GETDATE());

-- =============================================================================================================
-- 3. CUSTOMERS - Used in NewBill.cs and CustomerManagement.cs
-- =============================================================================================================
IF NOT EXISTS (SELECT 1 FROM Customers WHERE CustomerName = 'Walk-in Customer')
INSERT INTO Customers (CustomerName, Phone, Email, Address, City, State, PostalCode, CreditLimit, CreditDays, CurrentBalance, IsActive, CreatedDate) VALUES
('Walk-in Customer', '', '', '', 'Karachi', 'Sindh', '', 0.00, 0, 0.00, 1, GETDATE());

IF NOT EXISTS (SELECT 1 FROM Customers WHERE CustomerName = 'City Hospital')
INSERT INTO Customers (CustomerName, Phone, Email, Address, City, State, PostalCode, CreditLimit, CreditDays, CurrentBalance, IsActive, CreatedDate) VALUES
('City Hospital', '+92-21-111-222-333', 'procurement@cityhospital.pk', 'University Road', 'Karachi', 'Sindh', '75270', 500000.00, 30, 0.00, 1, GETDATE());

IF NOT EXISTS (SELECT 1 FROM Customers WHERE CustomerName = 'Green Clinic')
INSERT INTO Customers (CustomerName, Phone, Email, Address, City, State, PostalCode, CreditLimit, CreditDays, CurrentBalance, IsActive, CreatedDate) VALUES
('Green Clinic', '+92-21-444-555-666', 'orders@greenclinic.pk', 'Gulshan-e-Iqbal', 'Karachi', 'Sindh', '75300', 100000.00, 15, 0.00, 1, GETDATE());

-- =============================================================================================================
-- 4. ITEMS - Used extensively in NewBill.cs, NewPurchase.cs, and DashboardStatusHelper.cs
-- =============================================================================================================
IF NOT EXISTS (SELECT 1 FROM Items WHERE ItemName = 'Panadol Extra Tab')
INSERT INTO Items (ItemName, Description, Category, CompanyID, Barcode, Price, MRP, StockQuantity, ReorderLevel, PackSize, IsActive, CreatedDate) VALUES
('Panadol Extra Tab', 'Paracetamol + Caffeine Tablets', 'Pain Relief', 1, '8964000561157', 3.50, 4.00, 1000, 100, 20, 1, GETDATE());

IF NOT EXISTS (SELECT 1 FROM Items WHERE ItemName = 'Brufen 400mg Tab')
INSERT INTO Items (ItemName, Description, Category, CompanyID, Barcode, Price, MRP, StockQuantity, ReorderLevel, PackSize, IsActive, CreatedDate) VALUES
('Brufen 400mg Tab', 'Ibuprofen Tablets', 'Pain Relief', 2, '8964000561158', 2.75, 3.25, 800, 80, 20, 1, GETDATE());

IF NOT EXISTS (SELECT 1 FROM Items WHERE ItemName = 'Augmentin 625mg Tab')
INSERT INTO Items (ItemName, Description, Category, CompanyID, Barcode, Price, MRP, StockQuantity, ReorderLevel, PackSize, IsActive, CreatedDate) VALUES
('Augmentin 625mg Tab', 'Amoxicillin + Clavulanic Acid', 'Antibiotics', 3, '8964000561159', 25.50, 30.00, 500, 50, 10, 1, GETDATE());

IF NOT EXISTS (SELECT 1 FROM Items WHERE ItemName = 'Glucophage 500mg Tab')
INSERT INTO Items (ItemName, Description, Category, CompanyID, Barcode, Price, MRP, StockQuantity, ReorderLevel, PackSize, IsActive, CreatedDate) VALUES
('Glucophage 500mg Tab', 'Metformin Tablets', 'Diabetes', 1, '8964000561160', 4.25, 5.00, 400, 40, 60, 1, GETDATE());

IF NOT EXISTS (SELECT 1 FROM Items WHERE ItemName = 'Centrum Multivitamin')
INSERT INTO Items (ItemName, Description, Category, CompanyID, Barcode, Price, MRP, StockQuantity, ReorderLevel, PackSize, IsActive, CreatedDate) VALUES
('Centrum Multivitamin', 'Complete Multivitamin', 'Vitamins', 2, '8964000561161', 12.00, 14.50, 300, 30, 30, 1, GETDATE());

-- =============================================================================================================
-- 5. PURCHASES - Used in NewPurchase.cs (main table for purchase records)
-- =============================================================================================================
INSERT INTO Purchases (PurchaseNumber, CompanyID, PurchaseDate, TotalAmount, CreatedDate, IsActive) VALUES
('PUR001', 1, '2024-12-01', 15000.00, '2024-12-01', 1),
('PUR002', 2, '2024-12-05', 22000.00, '2024-12-05', 1),
('PUR003', 3, '2024-12-10', 18500.00, '2024-12-10', 1),
('PUR004', 1, '2024-12-15', 25000.00, '2024-12-15', 1),
('PUR005', 2, '2024-12-20', 20000.00, '2024-12-20', 1);

-- =============================================================================================================
-- 6. PURCHASE ITEMS - Used in NewPurchase.cs for purchase details
-- =============================================================================================================
DECLARE @PurchaseID1 INT = (SELECT PurchaseID FROM Purchases WHERE PurchaseNumber = 'PUR001');
DECLARE @PurchaseID2 INT = (SELECT PurchaseID FROM Purchases WHERE PurchaseNumber = 'PUR002');
DECLARE @PurchaseID3 INT = (SELECT PurchaseID FROM Purchases WHERE PurchaseNumber = 'PUR003');
DECLARE @PurchaseID4 INT = (SELECT PurchaseID FROM Purchases WHERE PurchaseNumber = 'PUR004');
DECLARE @PurchaseID5 INT = (SELECT PurchaseID FROM Purchases WHERE PurchaseNumber = 'PUR005');

INSERT INTO PurchaseItems (PurchaseID, ItemID, Quantity, Price, TotalAmount, ExpiryDate, BatchNumber)
VALUES
(@PurchaseID1, 1, 100, 3.00, 300.00, '2026-12-01', 'BATCH001'),
(@PurchaseID1, 2, 50, 2.50, 125.00, '2026-12-01', 'BATCH002'),
(@PurchaseID2, 3, 75, 22.00, 1650.00, '2026-12-05', 'BATCH003'),
(@PurchaseID2, 4, 60, 3.80, 228.00, '2026-12-05', 'BATCH004'),
(@PurchaseID3, 5, 40, 10.50, 420.00, '2026-12-10', 'BATCH005'),
(@PurchaseID3, 1, 80, 3.00, 240.00, '2026-12-10', 'BATCH006'),
(@PurchaseID4, 2, 100, 2.50, 250.00, '2026-12-15', 'BATCH007'),
(@PurchaseID4, 3, 50, 22.00, 1100.00, '2026-12-15', 'BATCH008'),
(@PurchaseID5, 4, 90, 3.80, 342.00, '2026-12-20', 'BATCH009'),
(@PurchaseID5, 5, 30, 10.50, 315.00, '2026-12-20', 'BATCH010');

-- =============================================================================================================
-- 7. SALES - Used in NewBill.cs and DashboardStatusHelper.cs (main sales table)
-- =============================================================================================================
INSERT INTO Sales (BillNumber, CustomerID, SaleDate, TotalAmount, Discount, NetAmount, PaymentMethod, IsCredit, IsActive, CreatedDate) VALUES
('BILL000001', 1, '2024-12-20 09:30:00', 150.00, 7.50, 142.50, 'Cash', 0, 1, '2024-12-20 09:30:00'),
('BILL000002', 1, '2024-12-20 11:15:00', 85.50, 4.25, 81.25, 'Card', 0, 1, '2024-12-20 11:15:00'),
('BILL000003', 2, '2024-12-20 14:20:00', 1250.00, 62.50, 1187.50, 'Cash', 0, 1, '2024-12-20 14:20:00'),
('BILL000004', 1, '2024-12-21 10:00:00', 95.75, 4.75, 91.00, 'EasyPaisa', 0, 1, '2024-12-21 10:00:00'),
('BILL000005', 3, '2024-12-21 16:30:00', 475.00, 0.00, 475.00, 'Cash', 1, 1, '2024-12-21 16:30:00'),
('BILL000006', 1, '2024-12-22 12:45:00', 67.50, 3.25, 64.25, 'Cash', 0, 1, '2024-12-22 12:45:00'),
('BILL000007', 2, '2024-12-22 15:10:00', 890.00, 44.50, 845.50, 'Card', 0, 1, '2024-12-22 15:10:00'),
('BILL000008', 1, '2024-12-23 09:20:00', 125.00, 6.25, 118.75, 'Cash', 0, 1, '2024-12-23 09:20:00'),
('BILL000009', 3, '2024-12-23 13:40:00', 325.50, 0.00, 325.50, 'JazzCash', 1, 1, '2024-12-23 13:40:00'),
('BILL000010', 1, '2024-12-24 11:30:00', 78.25, 3.90, 74.35, 'Cash', 0, 1, '2024-12-24 11:30:00');

-- =============================================================================================================
-- 8. SALE ITEMS - Used in NewBill.cs for sale details
-- =============================================================================================================
DECLARE @SaleID1 INT = (SELECT SaleID FROM Sales WHERE BillNumber = 'BILL000001');
DECLARE @SaleID2 INT = (SELECT SaleID FROM Sales WHERE BillNumber = 'BILL000002');
DECLARE @SaleID3 INT = (SELECT SaleID FROM Sales WHERE BillNumber = 'BILL000003');
DECLARE @SaleID4 INT = (SELECT SaleID FROM Sales WHERE BillNumber = 'BILL000004');
DECLARE @SaleID5 INT = (SELECT SaleID FROM Sales WHERE BillNumber = 'BILL000005');
DECLARE @SaleID6 INT = (SELECT SaleID FROM Sales WHERE BillNumber = 'BILL000006');
DECLARE @SaleID7 INT = (SELECT SaleID FROM Sales WHERE BillNumber = 'BILL000007');
DECLARE @SaleID8 INT = (SELECT SaleID FROM Sales WHERE BillNumber = 'BILL000008');
DECLARE @SaleID9 INT = (SELECT SaleID FROM Sales WHERE BillNumber = 'BILL000009');
DECLARE @SaleID10 INT = (SELECT SaleID FROM Sales WHERE BillNumber = 'BILL000010');

INSERT INTO SaleItems (SaleID, ItemID, Quantity, Price, TotalAmount) VALUES
-- Sale 1 items
(@SaleID1, 1, 10, 4.00, 40.00),
(@SaleID1, 2, 5, 3.25, 16.25),
(@SaleID1, 4, 20, 5.00, 100.00),
-- Sale 2 items
(@SaleID2, 1, 5, 4.00, 20.00),
(@SaleID2, 5, 5, 14.50, 72.50),
-- Sale 3 items (hospital bulk order)
(@SaleID3, 3, 20, 30.00, 600.00),
(@SaleID3, 4, 50, 5.00, 250.00),
(@SaleID3, 1, 100, 4.00, 400.00),
-- Sale 4 items
(@SaleID4, 2, 10, 3.25, 32.50),
(@SaleID4, 1, 15, 4.00, 60.00),
-- Sale 5 items (clinic credit)
(@SaleID5, 3, 15, 30.00, 450.00),
(@SaleID5, 5, 1, 14.50, 14.50),
-- Sale 6 items
(@SaleID6, 1, 15, 4.00, 60.00),
(@SaleID6, 2, 2, 3.25, 6.50),
-- Sale 7 items
(@SaleID7, 4, 80, 5.00, 400.00),
(@SaleID7, 3, 10, 30.00, 300.00),
-- Sale 8 items
(@SaleID8, 1, 20, 4.00, 80.00),
(@SaleID8, 2, 5, 3.25, 16.25),
(@SaleID8, 5, 2, 14.50, 29.00),
-- Sale 9 items
(@SaleID9, 3, 10, 30.00, 300.00),
(@SaleID9, 1, 5, 4.00, 20.00),
-- Sale 10 items
(@SaleID10, 1, 10, 4.00, 40.00),
(@SaleID10, 2, 10, 3.25, 32.50);

-- =============================================================================================================
-- 9. EXPENSES - Used in CashCounterManagement.cs for expense tracking
-- =============================================================================================================
INSERT INTO Expenses (ExpenseDate, ExpenseCategory, ExpenseDescription, Amount, PaymentMethod, CreatedBy, CreatedDate, IsActive) VALUES
('2024-12-01', 'Rent', 'Monthly shop rent', 45000.00, 'Bank Transfer', 1, '2024-12-01', 1),
('2024-12-01', 'Utilities', 'Electricity bill', 8500.00, 'Cash', 1, '2024-12-01', 1),
('2024-12-01', 'Salaries', 'Pharmacist salary', 35000.00, 'Bank Transfer', 1, '2024-12-01', 1),
('2024-12-01', 'Salaries', 'Salesman salary', 25000.00, 'Bank Transfer', 1, '2024-12-01', 1),
('2024-12-15', 'Transportation', 'Fuel and delivery costs', 3500.00, 'Cash', 1, '2024-12-15', 1),
('2024-12-10', 'Office Supplies', 'Stationery and printing', 2500.00, 'Cash', 1, '2024-12-10', 1),
('2024-12-20', 'Maintenance', 'Equipment maintenance', 4500.00, 'Cash', 1, '2024-12-20', 1);

-- =============================================================================================================
-- 10. SYSTEM SETTINGS - Used in DatabaseBackupManager.cs
-- =============================================================================================================
IF NOT EXISTS (SELECT 1 FROM SystemSettings WHERE SettingKey = 'AutoBackupEnabled')
INSERT INTO SystemSettings (SettingKey, SettingValue, CreatedDate, IsActive) VALUES
('AutoBackupEnabled', 'true', GETDATE(), 1);

IF NOT EXISTS (SELECT 1 FROM SystemSettings WHERE SettingKey = 'BackupFrequency')
INSERT INTO SystemSettings (SettingKey, SettingValue, CreatedDate, IsActive) VALUES
('BackupFrequency', 'Daily', GETDATE(), 1);

IF NOT EXISTS (SELECT 1 FROM SystemSettings WHERE SettingKey = 'BackupTime')
INSERT INTO SystemSettings (SettingKey, SettingValue, CreatedDate, IsActive) VALUES
('BackupTime', '02:00', GETDATE(), 1);

-- =============================================================================================================
-- 11. CASH COUNTER - Used in CashCounterManagement.cs
-- =============================================================================================================
INSERT INTO CashCounter (UserID, ShiftDate, OpeningBalance, ClosingBalance, ShiftStatus, CreatedDate, IsActive) VALUES
(1, '2024-12-26', 10000.00, 25000.00, 'Closed', '2024-12-26 08:00:00', 1),
(2, '2024-12-26', 25000.00, NULL, 'Open', '2024-12-26 16:00:00', 1);

-- =============================================================================================================
-- 12. Update customer balances for credit sales
-- =============================================================================================================
UPDATE Customers 
SET CurrentBalance = ISNULL(
    (SELECT SUM(s.NetAmount) 
     FROM Sales s 
     WHERE s.CustomerID = Customers.CustomerID 
     AND s.IsCredit = 1 
     AND s.IsActive = 1), 0
);

-- =============================================================================================================
-- 13. Update item stock quantities based on sales (realistic stock reduction)
-- =============================================================================================================
UPDATE Items 
SET StockQuantity = StockQuantity - ISNULL(
    (SELECT SUM(si.Quantity) 
     FROM SaleItems si 
     INNER JOIN Sales s ON si.SaleID = s.SaleID 
     WHERE si.ItemID = Items.ItemID AND s.IsActive = 1), 0
)
WHERE ItemID IN (SELECT DISTINCT ItemID FROM SaleItems);

PRINT 'Focused database seeding completed successfully!';
PRINT '';
PRINT 'Summary of data added (only for tables used in application):';
PRINT '- 3 Users (for login functionality)';
PRINT '- 3 Companies (for purchase management)';
PRINT '- 3 Customers (for sales management)';
PRINT '- 5 Items (for inventory and sales)';
PRINT '- 5 Purchases with 10 purchase items (for purchase tracking)';
PRINT '- 10 Sales with 24 sale items (for sales reports)';
PRINT '- 7 Expense records (for expense tracking)';
PRINT '- 3 System settings (for backup management)';
PRINT '- 2 Cash counter records (for shift management)';
PRINT '- Updated customer balances and stock levels';
PRINT '';
PRINT 'All core functionality now has test data for reports!';

