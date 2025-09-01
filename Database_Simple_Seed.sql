-- =============================================================================================================
-- RETAIL MANAGEMENT SYSTEM - SIMPLE TEST DATA SEEDING SCRIPT
-- This script adds basic sample data for core tables only
-- =============================================================================================================

USE RetailManagementDB;
GO

PRINT 'Starting simple data seeding...';

-- =============================================================================================================
-- 1. USERS - Basic users
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
-- 2. COMPANIES - Basic companies
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
-- 3. CUSTOMERS - Basic customers
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
-- 4. ITEMS - Basic medicines (only using existing columns)
-- =============================================================================================================
IF NOT EXISTS (SELECT 1 FROM Items WHERE ItemName = 'Panadol Extra Tab')
INSERT INTO Items (ItemName, Description, Category, CompanyID, Barcode, Price, MRP, StockQuantity, ReorderLevel, PackSize, IsActive, CreatedDate) VALUES
('Panadol Extra Tab', 'Paracetamol + Caffeine Tablets', 'Pain Relief', 1, '123456789001', 3.50, 4.00, 1000, 100, 20, 1, GETDATE());

IF NOT EXISTS (SELECT 1 FROM Items WHERE ItemName = 'Brufen 400mg Tab')
INSERT INTO Items (ItemName, Description, Category, CompanyID, Barcode, Price, MRP, StockQuantity, ReorderLevel, PackSize, IsActive, CreatedDate) VALUES
('Brufen 400mg Tab', 'Ibuprofen Tablets', 'Pain Relief', 2, '123456789002', 2.75, 3.25, 800, 80, 20, 1, GETDATE());

IF NOT EXISTS (SELECT 1 FROM Items WHERE ItemName = 'Augmentin 625mg Tab')
INSERT INTO Items (ItemName, Description, Category, CompanyID, Barcode, Price, MRP, StockQuantity, ReorderLevel, PackSize, IsActive, CreatedDate) VALUES
('Augmentin 625mg Tab', 'Amoxicillin + Clavulanic Acid', 'Antibiotics', 3, '123456789003', 25.50, 30.00, 500, 50, 10, 1, GETDATE());

IF NOT EXISTS (SELECT 1 FROM Items WHERE ItemName = 'Glucophage 500mg Tab')
INSERT INTO Items (ItemName, Description, Category, CompanyID, Barcode, Price, MRP, StockQuantity, ReorderLevel, PackSize, IsActive, CreatedDate) VALUES
('Glucophage 500mg Tab', 'Metformin Tablets', 'Diabetes', 1, '123456789004', 4.25, 5.00, 400, 40, 60, 1, GETDATE());

IF NOT EXISTS (SELECT 1 FROM Items WHERE ItemName = 'Centrum Multivitamin')
INSERT INTO Items (ItemName, Description, Category, CompanyID, Barcode, Price, MRP, StockQuantity, ReorderLevel, PackSize, IsActive, CreatedDate) VALUES
('Centrum Multivitamin', 'Complete Multivitamin', 'Vitamins', 2, '123456789005', 12.00, 14.50, 300, 30, 30, 1, GETDATE());

-- =============================================================================================================
-- 5. PURCHASES - Basic purchase records
-- =============================================================================================================
INSERT INTO Purchases (PurchaseNumber, CompanyID, PurchaseDate, TotalAmount, CreatedDate, IsActive) VALUES
('PUR001', 1, '2024-12-01', 15000.00, '2024-12-01', 1),
('PUR002', 2, '2024-12-05', 22000.00, '2024-12-05', 1),
('PUR003', 3, '2024-12-10', 18500.00, '2024-12-10', 1),
('PUR004', 1, '2024-12-15', 25000.00, '2024-12-15', 1),
('PUR005', 2, '2024-12-20', 20000.00, '2024-12-20', 1);

-- =============================================================================================================
-- 6. PURCHASE ITEMS - Basic purchase item details
-- =============================================================================================================
INSERT INTO PurchaseItems (PurchaseID, ItemID, Quantity, Price, ExpiryDate, BatchNumber)
VALUES
(1, 1, 100, 3.00, '2026-12-01', 'BATCH001'),
(1, 2, 50, 2.50, '2026-12-01', 'BATCH002'),
(2, 3, 75, 22.00, '2026-12-05', 'BATCH003'),
(2, 4, 60, 3.80, '2026-12-05', 'BATCH004'),
(3, 5, 40, 10.50, '2026-12-10', 'BATCH005'),
(3, 1, 80, 3.00, '2026-12-10', 'BATCH006'),
(4, 2, 100, 2.50, '2026-12-15', 'BATCH007'),
(4, 3, 50, 22.00, '2026-12-15', 'BATCH008'),
(5, 4, 90, 3.80, '2026-12-20', 'BATCH009'),
(5, 5, 30, 10.50, '2026-12-20', 'BATCH010');

-- =============================================================================================================
-- 7. SALES - Basic sales transactions
-- =============================================================================================================
INSERT INTO Sales (BillNumber, CustomerID, SaleDate, TotalAmount, Discount, NetAmount, PaymentMethod, IsCredit, IsActive, CreatedDate) VALUES
('BILL000001', 1, '2024-12-20 09:30:00', 150.00, 7.50, 142.50, 'Cash', 0, 1, '2024-12-20 09:30:00'),
('BILL000002', 1, '2024-12-20 11:15:00', 85.50, 4.25, 81.25, 'Card', 0, 1, '2024-12-20 11:15:00'),
('BILL000003', 2, '2024-12-20 14:20:00', 1250.00, 62.50, 1187.50, 'Cash', 0, 1, '2024-12-20 14:20:00'),
('BILL000004', 1, '2024-12-21 10:00:00', 95.75, 4.75, 91.00, 'UPI', 0, 1, '2024-12-21 10:00:00'),
('BILL000005', 3, '2024-12-21 16:30:00', 475.00, 0.00, 475.00, 'Cash', 1, 1, '2024-12-21 16:30:00'),
('BILL000006', 1, '2024-12-22 12:45:00', 67.50, 3.25, 64.25, 'Cash', 0, 1, '2024-12-22 12:45:00'),
('BILL000007', 2, '2024-12-22 15:10:00', 890.00, 44.50, 845.50, 'Card', 0, 1, '2024-12-22 15:10:00'),
('BILL000008', 1, '2024-12-23 09:20:00', 125.00, 6.25, 118.75, 'Cash', 0, 1, '2024-12-23 09:20:00'),
('BILL000009', 3, '2024-12-23 13:40:00', 325.50, 0.00, 325.50, 'UPI', 1, 1, '2024-12-23 13:40:00'),
('BILL000010', 1, '2024-12-24 11:30:00', 78.25, 3.90, 74.35, 'Cash', 0, 1, '2024-12-24 11:30:00');

-- =============================================================================================================
-- 8. SALE ITEMS - Basic sale item details
-- =============================================================================================================
INSERT INTO SaleItems (SaleID, ItemID, Quantity, Price) VALUES
-- Sale 1 items
(1, 1, 10, 4.00),
(1, 2, 5, 3.25),
(1, 4, 20, 5.00),
-- Sale 2 items
(2, 1, 5, 4.00),
(2, 5, 5, 14.50),
-- Sale 3 items (hospital bulk order)
(3, 3, 20, 30.00),
(3, 4, 50, 5.00),
(3, 1, 100, 4.00),
-- Sale 4 items
(4, 2, 10, 3.25),
(4, 1, 15, 4.00),
-- Sale 5 items (clinic credit)
(5, 3, 15, 30.00),
(5, 5, 1, 14.50),
-- Sale 6 items
(6, 1, 15, 4.00),
(6, 2, 2, 3.25),
-- Sale 7 items
(7, 4, 80, 5.00),
(7, 3, 10, 30.00),
-- Sale 8 items
(8, 1, 20, 4.00),
(8, 2, 5, 3.25),
(8, 5, 2, 14.50),
-- Sale 9 items
(9, 3, 10, 30.00),
(9, 1, 5, 4.00),
-- Sale 10 items
(10, 1, 10, 4.00),
(10, 2, 10, 3.25);

-- =============================================================================================================
-- 9. EXPENSES - Basic operational expenses
-- =============================================================================================================
INSERT INTO Expenses (ExpenseType, Amount, ExpenseDate, CreatedDate, IsActive) VALUES
('Rent', 45000.00, '2024-12-01', '2024-12-01', 1),
('Utilities', 8500.00, '2024-12-01', '2024-12-01', 1),
('Staff Salary', 35000.00, '2024-12-01', '2024-12-01', 1),
('Staff Salary', 25000.00, '2024-12-01', '2024-12-01', 1),
('Transportation', 3500.00, '2024-12-15', '2024-12-15', 1),
('Office Supplies', 2500.00, '2024-12-10', '2024-12-10', 1),
('Maintenance', 4500.00, '2024-12-20', '2024-12-20', 1);

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

-- =============================================================================================================
-- 11. Update item stock based on sales
-- =============================================================================================================
UPDATE Items 
SET StockQuantity = StockQuantity - ISNULL(
    (SELECT SUM(si.Quantity) 
     FROM SaleItems si 
     INNER JOIN Sales s ON si.SaleID = s.SaleID 
     WHERE si.ItemID = Items.ItemID AND s.IsActive = 1), 0
)
WHERE ItemID IN (SELECT DISTINCT ItemID FROM SaleItems);

PRINT 'Simple database seeding completed successfully!';
PRINT '';
PRINT 'Summary of data added:';
PRINT '- 3 Users (Admin, Pharmacist, Salesman)';
PRINT '- 3 Companies (GSK, Searle, Abbott)';
PRINT '- 3 Customers (Walk-in, Hospital, Clinic)';
PRINT '- 5 Items (Common medicines)';
PRINT '- 5 Purchases with 10 purchase items';
PRINT '- 10 Sales with 20 sale items';
PRINT '- 7 Expense records';
PRINT '- Updated customer balances and stock levels';
PRINT '';
PRINT 'You can now test your reports with this basic data!';

