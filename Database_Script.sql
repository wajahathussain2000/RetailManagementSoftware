-- =====================================================
-- Retail Management System - Complete Database Script
-- =====================================================

-- Create Database
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'RetailManagementDB')
BEGIN
    CREATE DATABASE RetailManagementDB;
END
GO

USE RetailManagementDB;
GO

-- =====================================================
-- CREATE TABLES
-- =====================================================

-- Users Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
BEGIN
    CREATE TABLE Users (
        UserID INT IDENTITY(1,1) PRIMARY KEY,
        Username NVARCHAR(50) NOT NULL UNIQUE,
        Password NVARCHAR(100) NOT NULL,
        FullName NVARCHAR(100) NOT NULL,
        Email NVARCHAR(100),
        Role NVARCHAR(20) DEFAULT 'User',
        IsActive BIT DEFAULT 1,
        CreatedDate DATETIME DEFAULT GETDATE(),
        LastLoginDate DATETIME
    );
END
GO

-- Companies Table (Suppliers)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Companies]') AND type in (N'U'))
BEGIN
    CREATE TABLE Companies (
        CompanyID INT IDENTITY(1,1) PRIMARY KEY,
        CompanyName NVARCHAR(100) NOT NULL,
        ContactPerson NVARCHAR(100),
        Phone NVARCHAR(20),
        Email NVARCHAR(100),
        Address NVARCHAR(200),
        City NVARCHAR(50),
        State NVARCHAR(50),
        PostalCode NVARCHAR(20),
        Country NVARCHAR(50),
        IsActive BIT DEFAULT 1,
        CreatedDate DATETIME DEFAULT GETDATE()
    );
END
GO

-- Customers Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Customers]') AND type in (N'U'))
BEGIN
    CREATE TABLE Customers (
        CustomerID INT IDENTITY(1,1) PRIMARY KEY,
        CustomerName NVARCHAR(100) NOT NULL,
        Phone NVARCHAR(20),
        Email NVARCHAR(100),
        Address NVARCHAR(200),
        City NVARCHAR(50),
        State NVARCHAR(50),
        PostalCode NVARCHAR(20),
        Country NVARCHAR(50),
        CreditLimit DECIMAL(10,2) DEFAULT 0,
        CurrentBalance DECIMAL(10,2) DEFAULT 0,
        IsActive BIT DEFAULT 1,
        CreatedDate DATETIME DEFAULT GETDATE()
    );
END
GO

-- Items Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Items]') AND type in (N'U'))
BEGIN
    CREATE TABLE Items (
        ItemID INT IDENTITY(1,1) PRIMARY KEY,
        ItemName NVARCHAR(100) NOT NULL,
        GenericName NVARCHAR(100),
        Packing NVARCHAR(50),
        Location NVARCHAR(50),
        RetailPrice DECIMAL(10,2) NOT NULL,
        PurchasePrice DECIMAL(10,2) NOT NULL,
        PackSize NVARCHAR(20),
        ReOrderLevel INT DEFAULT 0,
        CurrentStock INT DEFAULT 0,
        DistributionDisc DECIMAL(5,2) DEFAULT 0,
        SalesTax DECIMAL(5,2) DEFAULT 0,
        CompanyID INT,
        IsBlocked BIT DEFAULT 0,
        IsActive BIT DEFAULT 1,
        CreatedDate DATETIME DEFAULT GETDATE(),
        FOREIGN KEY (CompanyID) REFERENCES Companies(CompanyID)
    );
END
GO

-- Sales Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Sales]') AND type in (N'U'))
BEGIN
    CREATE TABLE Sales (
        SaleID INT IDENTITY(1,1) PRIMARY KEY,
        BillNumber NVARCHAR(20) NOT NULL UNIQUE,
        CustomerID INT NOT NULL,
        SaleDate DATETIME DEFAULT GETDATE(),
        GrossAmount DECIMAL(10,2) NOT NULL,
        Discount DECIMAL(10,2) DEFAULT 0,
        NetAmount DECIMAL(10,2) NOT NULL,
        PaymentMethod NVARCHAR(20) DEFAULT 'Cash',
        Remarks NVARCHAR(200),
        IsActive BIT DEFAULT 1,
        CreatedDate DATETIME DEFAULT GETDATE(),
        FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID)
    );
END
GO

-- SaleItems Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SaleItems]') AND type in (N'U'))
BEGIN
    CREATE TABLE SaleItems (
        SaleItemID INT IDENTITY(1,1) PRIMARY KEY,
        SaleID INT NOT NULL,
        ItemID INT NOT NULL,
        Quantity INT NOT NULL,
        UnitPrice DECIMAL(10,2) NOT NULL,
        TotalAmount DECIMAL(10,2) NOT NULL,
        FOREIGN KEY (SaleID) REFERENCES Sales(SaleID),
        FOREIGN KEY (ItemID) REFERENCES Items(ItemID)
    );
END
GO

-- Purchases Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Purchases]') AND type in (N'U'))
BEGIN
    CREATE TABLE Purchases (
        PurchaseID INT IDENTITY(1,1) PRIMARY KEY,
        PurchaseNumber NVARCHAR(20) NOT NULL UNIQUE,
        CompanyID INT NOT NULL,
        PurchaseDate DATETIME DEFAULT GETDATE(),
        GrossAmount DECIMAL(10,2) NOT NULL,
        Discount DECIMAL(10,2) DEFAULT 0,
        SalesTax DECIMAL(10,2) DEFAULT 0,
        NetAmount DECIMAL(10,2) NOT NULL,
        Remarks NVARCHAR(200),
        IsActive BIT DEFAULT 1,
        CreatedDate DATETIME DEFAULT GETDATE(),
        FOREIGN KEY (CompanyID) REFERENCES Companies(CompanyID)
    );
END
GO

-- PurchaseItems Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PurchaseItems]') AND type in (N'U'))
BEGIN
    CREATE TABLE PurchaseItems (
        PurchaseItemID INT IDENTITY(1,1) PRIMARY KEY,
        PurchaseID INT NOT NULL,
        ItemID INT NOT NULL,
        Quantity INT NOT NULL,
        UnitPrice DECIMAL(10,2) NOT NULL,
        TotalAmount DECIMAL(10,2) NOT NULL,
        FOREIGN KEY (PurchaseID) REFERENCES Purchases(PurchaseID),
        FOREIGN KEY (ItemID) REFERENCES Items(ItemID)
    );
END
GO

-- CustomerPayments Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CustomerPayments]') AND type in (N'U'))
BEGIN
    CREATE TABLE CustomerPayments (
        PaymentID INT IDENTITY(1,1) PRIMARY KEY,
        CustomerID INT NOT NULL,
        SaleID INT,
        Amount DECIMAL(10,2) NOT NULL,
        PaymentDate DATETIME DEFAULT GETDATE(),
        PaymentMethod NVARCHAR(20) DEFAULT 'Cash',
        Remarks NVARCHAR(200),
        IsActive BIT DEFAULT 1,
        CreatedDate DATETIME DEFAULT GETDATE(),
        FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID),
        FOREIGN KEY (SaleID) REFERENCES Sales(SaleID)
    );
END
GO

-- SaleReturns Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SaleReturns]') AND type in (N'U'))
BEGIN
    CREATE TABLE SaleReturns (
        ReturnID INT IDENTITY(1,1) PRIMARY KEY,
        ReturnNumber NVARCHAR(20) NOT NULL UNIQUE,
        SaleID INT NOT NULL,
        CustomerID INT NOT NULL,
        ReturnDate DATETIME DEFAULT GETDATE(),
        TotalAmount DECIMAL(10,2) NOT NULL,
        Reason NVARCHAR(200),
        IsActive BIT DEFAULT 1,
        CreatedDate DATETIME DEFAULT GETDATE(),
        FOREIGN KEY (SaleID) REFERENCES Sales(SaleID),
        FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID)
    );
END
GO

-- SaleReturnItems Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SaleReturnItems]') AND type in (N'U'))
BEGIN
    CREATE TABLE SaleReturnItems (
        ReturnItemID INT IDENTITY(1,1) PRIMARY KEY,
        ReturnID INT NOT NULL,
        ItemID INT NOT NULL,
        Quantity INT NOT NULL,
        UnitPrice DECIMAL(10,2) NOT NULL,
        TotalAmount DECIMAL(10,2) NOT NULL,
        FOREIGN KEY (ReturnID) REFERENCES SaleReturns(ReturnID),
        FOREIGN KEY (ItemID) REFERENCES Items(ItemID)
    );
END
GO

-- AuditLog Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AuditLog]') AND type in (N'U'))
BEGIN
    CREATE TABLE AuditLog (
        LogID INT IDENTITY(1,1) PRIMARY KEY,
        UserID INT,
        TableName NVARCHAR(50) NOT NULL,
        Action NVARCHAR(20) NOT NULL,
        RecordID INT,
        OldValues NVARCHAR(MAX),
        NewValues NVARCHAR(MAX),
        LogDate DATETIME DEFAULT GETDATE(),
        FOREIGN KEY (UserID) REFERENCES Users(UserID)
    );
END
GO

-- =====================================================
-- CREATE INDEXES
-- =====================================================

-- Sales Indexes
CREATE INDEX IX_Sales_CustomerID ON Sales(CustomerID);
CREATE INDEX IX_Sales_SaleDate ON Sales(SaleDate);
CREATE INDEX IX_Sales_BillNumber ON Sales(BillNumber);

-- SaleItems Indexes
CREATE INDEX IX_SaleItems_SaleID ON SaleItems(SaleID);
CREATE INDEX IX_SaleItems_ItemID ON SaleItems(ItemID);

-- Purchases Indexes
CREATE INDEX IX_Purchases_CompanyID ON Purchases(CompanyID);
CREATE INDEX IX_Purchases_PurchaseDate ON Purchases(PurchaseDate);

-- PurchaseItems Indexes
CREATE INDEX IX_PurchaseItems_PurchaseID ON PurchaseItems(PurchaseID);
CREATE INDEX IX_PurchaseItems_ItemID ON PurchaseItems(ItemID);

-- Items Indexes
CREATE INDEX IX_Items_CompanyID ON Items(CompanyID);
CREATE INDEX IX_Items_ItemName ON Items(ItemName);

-- CustomerPayments Indexes
CREATE INDEX IX_CustomerPayments_CustomerID ON CustomerPayments(CustomerID);
CREATE INDEX IX_CustomerPayments_PaymentDate ON CustomerPayments(PaymentDate);

-- SaleReturns Indexes
CREATE INDEX IX_SaleReturns_SaleID ON SaleReturns(SaleID);
CREATE INDEX IX_SaleReturns_CustomerID ON SaleReturns(CustomerID);

-- =====================================================
-- CREATE TRIGGERS
-- =====================================================

-- Trigger to update customer balance after sale
IF EXISTS (SELECT * FROM sys.triggers WHERE name = 'TR_Sales_UpdateCustomerBalance')
    DROP TRIGGER TR_Sales_UpdateCustomerBalance;
GO

CREATE TRIGGER TR_Sales_UpdateCustomerBalance
ON Sales
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE c
    SET CurrentBalance = c.CurrentBalance + i.NetAmount
    FROM Customers c
    INNER JOIN inserted i ON c.CustomerID = i.CustomerID
    WHERE i.IsActive = 1;
END
GO

-- Trigger to update customer balance after payment
IF EXISTS (SELECT * FROM sys.triggers WHERE name = 'TR_CustomerPayments_UpdateCustomerBalance')
    DROP TRIGGER TR_CustomerPayments_UpdateCustomerBalance;
GO

CREATE TRIGGER TR_CustomerPayments_UpdateCustomerBalance
ON CustomerPayments
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE c
    SET CurrentBalance = c.CurrentBalance - i.Amount
    FROM Customers c
    INNER JOIN inserted i ON c.CustomerID = i.CustomerID
    WHERE i.IsActive = 1;
END
GO

-- Trigger to update item stock after sale
IF EXISTS (SELECT * FROM sys.triggers WHERE name = 'TR_SaleItems_UpdateStock')
    DROP TRIGGER TR_SaleItems_UpdateStock;
GO

CREATE TRIGGER TR_SaleItems_UpdateStock
ON SaleItems
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE i
    SET CurrentStock = i.CurrentStock - si.Quantity
    FROM Items i
    INNER JOIN inserted si ON i.ItemID = si.ItemID;
END
GO

-- Trigger to update item stock after purchase
IF EXISTS (SELECT * FROM sys.triggers WHERE name = 'TR_PurchaseItems_UpdateStock')
    DROP TRIGGER TR_PurchaseItems_UpdateStock;
GO

CREATE TRIGGER TR_PurchaseItems_UpdateStock
ON PurchaseItems
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE i
    SET CurrentStock = i.CurrentStock + pi.Quantity
    FROM Items i
    INNER JOIN inserted pi ON i.ItemID = pi.ItemID;
END
GO

-- Trigger to update item stock after sale return
IF EXISTS (SELECT * FROM sys.triggers WHERE name = 'TR_SaleReturnItems_UpdateStock')
    DROP TRIGGER TR_SaleReturnItems_UpdateStock;
GO

CREATE TRIGGER TR_SaleReturnItems_UpdateStock
ON SaleReturnItems
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE i
    SET CurrentStock = i.CurrentStock + sri.Quantity
    FROM Items i
    INNER JOIN inserted sri ON i.ItemID = sri.ItemID;
END
GO

-- =====================================================
-- CREATE STORED PROCEDURES
-- =====================================================

-- Get Customer Balance
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetCustomerBalance]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_GetCustomerBalance];
GO

CREATE PROCEDURE sp_GetCustomerBalance
    @CustomerID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        c.CustomerID,
        c.CustomerName,
        c.CurrentBalance,
        ISNULL(SUM(s.NetAmount), 0) as TotalSales,
        ISNULL(SUM(cp.Amount), 0) as TotalPayments,
        ISNULL(SUM(s.NetAmount), 0) - ISNULL(SUM(cp.Amount), 0) as OutstandingBalance
    FROM Customers c
    LEFT JOIN Sales s ON c.CustomerID = s.CustomerID AND s.IsActive = 1
    LEFT JOIN CustomerPayments cp ON c.CustomerID = cp.CustomerID AND cp.IsActive = 1
    WHERE c.CustomerID = @CustomerID
    GROUP BY c.CustomerID, c.CustomerName, c.CurrentBalance;
END
GO

-- Get Sales Report
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetSalesReport]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_GetSalesReport];
GO

CREATE PROCEDURE sp_GetSalesReport
    @FromDate DATETIME,
    @ToDate DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        s.BillNumber,
        c.CustomerName,
        s.SaleDate,
        s.GrossAmount,
        s.Discount,
        s.NetAmount,
        s.PaymentMethod
    FROM Sales s
    INNER JOIN Customers c ON s.CustomerID = c.CustomerID
    WHERE s.SaleDate BETWEEN @FromDate AND @ToDate
    AND s.IsActive = 1
    ORDER BY s.SaleDate DESC;
END
GO

-- Get Stock Report
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetStockReport]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_GetStockReport];
GO

CREATE PROCEDURE sp_GetStockReport
    @CompanyID INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        i.ItemID,
        i.ItemName,
        i.GenericName,
        i.Packing,
        i.Location,
        i.RetailPrice,
        i.PurchasePrice,
        i.CurrentStock,
        i.ReOrderLevel,
        comp.CompanyName,
        CASE 
            WHEN i.CurrentStock <= i.ReOrderLevel THEN 'Low Stock'
            WHEN i.CurrentStock = 0 THEN 'Out of Stock'
            ELSE 'In Stock'
        END as StockStatus
    FROM Items i
    LEFT JOIN Companies comp ON i.CompanyID = comp.CompanyID
    WHERE i.IsActive = 1
    AND (@CompanyID IS NULL OR i.CompanyID = @CompanyID)
    ORDER BY i.ItemName;
END
GO

-- Get Customer Ledger
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetCustomerLedger]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_GetCustomerLedger];
GO

CREATE PROCEDURE sp_GetCustomerLedger
    @CustomerID INT,
    @FromDate DATETIME,
    @ToDate DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        'Sale' as TransactionType,
        s.BillNumber as Reference,
        s.SaleDate as TransactionDate,
        s.NetAmount as Debit,
        0 as Credit,
        s.NetAmount as Balance
    FROM Sales s
    WHERE s.CustomerID = @CustomerID 
    AND s.SaleDate BETWEEN @FromDate AND @ToDate
    AND s.IsActive = 1
    
    UNION ALL
    
    SELECT 
        'Payment' as TransactionType,
        'Payment' as Reference,
        cp.PaymentDate as TransactionDate,
        0 as Debit,
        cp.Amount as Credit,
        -cp.Amount as Balance
    FROM CustomerPayments cp
    WHERE cp.CustomerID = @CustomerID 
    AND cp.PaymentDate BETWEEN @FromDate AND @ToDate
    AND cp.IsActive = 1
    
    ORDER BY TransactionDate;
END
GO

-- Get Profit and Loss Report
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetProfitAndLoss]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_GetProfitAndLoss];
GO

CREATE PROCEDURE sp_GetProfitAndLoss
    @FromDate DATETIME,
    @ToDate DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @TotalSales DECIMAL(10,2) = 0;
    DECLARE @TotalCost DECIMAL(10,2) = 0;
    DECLARE @TotalDiscount DECIMAL(10,2) = 0;
    
    -- Calculate Total Sales
    SELECT @TotalSales = ISNULL(SUM(s.NetAmount), 0)
    FROM Sales s
    WHERE s.SaleDate BETWEEN @FromDate AND @ToDate
    AND s.IsActive = 1;
    
    -- Calculate Total Cost of Goods Sold
    SELECT @TotalCost = ISNULL(SUM(si.Quantity * i.PurchasePrice), 0)
    FROM SaleItems si
    INNER JOIN Sales s ON si.SaleID = s.SaleID
    INNER JOIN Items i ON si.ItemID = i.ItemID
    WHERE s.SaleDate BETWEEN @FromDate AND @ToDate
    AND s.IsActive = 1;
    
    -- Calculate Total Discount
    SELECT @TotalDiscount = ISNULL(SUM(s.Discount), 0)
    FROM Sales s
    WHERE s.SaleDate BETWEEN @FromDate AND @ToDate
    AND s.IsActive = 1;
    
    SELECT 
        @TotalSales as TotalSales,
        @TotalCost as TotalCost,
        @TotalDiscount as TotalDiscount,
        (@TotalSales - @TotalCost - @TotalDiscount) as GrossProfit,
        (@TotalSales - @TotalCost - @TotalDiscount) / NULLIF(@TotalSales, 0) * 100 as ProfitPercentage;
END
GO

-- Insert Item
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_InsertItem]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_InsertItem];
GO

CREATE PROCEDURE sp_InsertItem
    @ItemName NVARCHAR(100),
    @GenericName NVARCHAR(100),
    @Packing NVARCHAR(50),
    @Location NVARCHAR(50),
    @RetailPrice DECIMAL(10,2),
    @PurchasePrice DECIMAL(10,2),
    @PackSize NVARCHAR(20),
    @ReOrderLevel INT,
    @DistributionDisc DECIMAL(5,2),
    @SalesTax DECIMAL(5,2),
    @CompanyID INT,
    @IsBlocked BIT
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO Items (ItemName, GenericName, Packing, Location, RetailPrice, PurchasePrice, 
                      PackSize, ReOrderLevel, DistributionDisc, SalesTax, CompanyID, IsBlocked)
    VALUES (@ItemName, @GenericName, @Packing, @Location, @RetailPrice, @PurchasePrice,
            @PackSize, @ReOrderLevel, @DistributionDisc, @SalesTax, @CompanyID, @IsBlocked);
    
    SELECT SCOPE_IDENTITY() as ItemID;
END
GO

-- Insert Sale
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_InsertSale]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_InsertSale];
GO

CREATE PROCEDURE sp_InsertSale
    @BillNumber NVARCHAR(20),
    @CustomerID INT,
    @GrossAmount DECIMAL(10,2),
    @Discount DECIMAL(10,2),
    @NetAmount DECIMAL(10,2),
    @PaymentMethod NVARCHAR(20),
    @Remarks NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO Sales (BillNumber, CustomerID, GrossAmount, Discount, NetAmount, PaymentMethod, Remarks)
    VALUES (@BillNumber, @CustomerID, @GrossAmount, @Discount, @NetAmount, @PaymentMethod, @Remarks);
    
    SELECT SCOPE_IDENTITY() as SaleID;
END
GO

-- Insert Customer Payment
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_InsertCustomerPayment]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_InsertCustomerPayment];
GO

CREATE PROCEDURE sp_InsertCustomerPayment
    @CustomerID INT,
    @SaleID INT,
    @Amount DECIMAL(10,2),
    @PaymentMethod NVARCHAR(20),
    @Remarks NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO CustomerPayments (CustomerID, SaleID, Amount, PaymentMethod, Remarks)
    VALUES (@CustomerID, @SaleID, @Amount, @PaymentMethod, @Remarks);
    
    SELECT SCOPE_IDENTITY() as PaymentID;
END
GO

-- =====================================================
-- CREATE VIEWS
-- =====================================================

-- Customer Balance View
IF EXISTS (SELECT * FROM sys.views WHERE name = 'vw_CustomerBalance')
    DROP VIEW vw_CustomerBalance;
GO

CREATE VIEW vw_CustomerBalance AS
SELECT 
    c.CustomerID,
    c.CustomerName,
    c.Phone,
    c.CurrentBalance,
    ISNULL(SUM(s.NetAmount), 0) as TotalSales,
    ISNULL(SUM(cp.Amount), 0) as TotalPayments,
    ISNULL(SUM(s.NetAmount), 0) - ISNULL(SUM(cp.Amount), 0) as OutstandingBalance
FROM Customers c
LEFT JOIN Sales s ON c.CustomerID = s.CustomerID AND s.IsActive = 1
LEFT JOIN CustomerPayments cp ON c.CustomerID = cp.CustomerID AND cp.IsActive = 1
WHERE c.IsActive = 1
GROUP BY c.CustomerID, c.CustomerName, c.Phone, c.CurrentBalance;
GO

-- Stock Summary View
IF EXISTS (SELECT * FROM sys.views WHERE name = 'vw_StockSummary')
    DROP VIEW vw_StockSummary;
GO

CREATE VIEW vw_StockSummary AS
SELECT 
    i.ItemID,
    i.ItemName,
    i.GenericName,
    i.Packing,
    i.Location,
    i.RetailPrice,
    i.PurchasePrice,
    i.CurrentStock,
    i.ReOrderLevel,
    comp.CompanyName,
    (i.RetailPrice * i.CurrentStock) as StockValue,
    CASE 
        WHEN i.CurrentStock <= i.ReOrderLevel THEN 'Low Stock'
        WHEN i.CurrentStock = 0 THEN 'Out of Stock'
        ELSE 'In Stock'
    END as StockStatus
FROM Items i
LEFT JOIN Companies comp ON i.CompanyID = comp.CompanyID
WHERE i.IsActive = 1;
GO

-- Sales Summary View
IF EXISTS (SELECT * FROM sys.views WHERE name = 'vw_SalesSummary')
    DROP VIEW vw_SalesSummary;
GO

CREATE VIEW vw_SalesSummary AS
SELECT 
    s.SaleID,
    s.BillNumber,
    c.CustomerName,
    s.SaleDate,
    s.GrossAmount,
    s.Discount,
    s.NetAmount,
    s.PaymentMethod,
    COUNT(si.SaleItemID) as TotalItems
FROM Sales s
INNER JOIN Customers c ON s.CustomerID = c.CustomerID
LEFT JOIN SaleItems si ON s.SaleID = si.SaleID
WHERE s.IsActive = 1
GROUP BY s.SaleID, s.BillNumber, c.CustomerName, s.SaleDate, s.GrossAmount, s.Discount, s.NetAmount, s.PaymentMethod;
GO

-- =====================================================
-- INSERT SAMPLE DATA
-- =====================================================

-- Insert Default User
IF NOT EXISTS (SELECT * FROM Users WHERE Username = 'admin')
BEGIN
    INSERT INTO Users (Username, Password, FullName, Email, Role)
    VALUES ('admin', 'admin123', 'Administrator', 'admin@retail.com', 'Admin');
END

-- Insert Sample Companies
IF NOT EXISTS (SELECT * FROM Companies WHERE CompanyName = 'ABC Pharmaceuticals')
BEGIN
    INSERT INTO Companies (CompanyName, ContactPerson, Phone, Email, Address, City, State, PostalCode, Country)
    VALUES 
    ('ABC Pharmaceuticals', 'John Smith', '123-456-7890', 'john@abc.com', '123 Main St', 'New York', 'NY', '10001', 'USA'),
    ('XYZ Medical Supplies', 'Jane Doe', '098-765-4321', 'jane@xyz.com', '456 Oak Ave', 'Los Angeles', 'CA', '90210', 'USA'),
    ('MediCare Solutions', 'Mike Johnson', '555-123-4567', 'mike@medicare.com', '789 Pine Rd', 'Chicago', 'IL', '60601', 'USA');
END

-- Insert Sample Customers
IF NOT EXISTS (SELECT * FROM Customers WHERE CustomerName = 'City Hospital')
BEGIN
    INSERT INTO Customers (CustomerName, Phone, Email, Address, City, State, PostalCode, Country, CreditLimit)
    VALUES 
    ('City Hospital', '111-222-3333', 'info@cityhospital.com', '100 Hospital Dr', 'New York', 'NY', '10002', 'USA', 50000.00),
    ('Community Clinic', '444-555-6666', 'contact@communityclinic.com', '200 Clinic St', 'Los Angeles', 'CA', '90211', 'USA', 25000.00),
    ('Medical Center', '777-888-9999', 'admin@medicalcenter.com', '300 Medical Blvd', 'Chicago', 'IL', '60602', 'USA', 75000.00);
END

-- Insert Sample Items
IF NOT EXISTS (SELECT * FROM Items WHERE ItemName = 'Paracetamol 500mg')
BEGIN
    INSERT INTO Items (ItemName, GenericName, Packing, Location, RetailPrice, PurchasePrice, PackSize, ReOrderLevel, DistributionDisc, SalesTax, CompanyID, CurrentStock)
    VALUES 
    ('Paracetamol 500mg', 'Acetaminophen', 'Tablet', 'Shelf A1', 5.00, 3.50, '10 tablets', 100, 5.00, 8.00, 1, 500),
    ('Amoxicillin 250mg', 'Amoxicillin', 'Capsule', 'Shelf A2', 8.00, 5.00, '20 capsules', 50, 5.00, 8.00, 1, 200),
    ('Ibuprofen 400mg', 'Ibuprofen', 'Tablet', 'Shelf A3', 6.00, 4.00, '15 tablets', 75, 5.00, 8.00, 2, 300),
    ('Omeprazole 20mg', 'Omeprazole', 'Capsule', 'Shelf B1', 12.00, 8.00, '14 capsules', 30, 5.00, 8.00, 2, 150),
    ('Metformin 500mg', 'Metformin', 'Tablet', 'Shelf B2', 10.00, 6.50, '30 tablets', 40, 5.00, 8.00, 3, 250);
END

-- Insert Sample Sales
IF NOT EXISTS (SELECT * FROM Sales WHERE BillNumber = 'SALE001')
BEGIN
    INSERT INTO Sales (BillNumber, CustomerID, GrossAmount, Discount, NetAmount, PaymentMethod, Remarks)
    VALUES 
    ('SALE001', 1, 150.00, 10.00, 140.00, 'Cash', 'Regular order'),
    ('SALE002', 2, 200.00, 0.00, 200.00, 'Credit', 'Monthly supply'),
    ('SALE003', 3, 300.00, 25.00, 275.00, 'Cash', 'Bulk order');
END

-- Insert Sample Sale Items
IF NOT EXISTS (SELECT * FROM SaleItems WHERE SaleID = 1)
BEGIN
    INSERT INTO SaleItems (SaleID, ItemID, Quantity, UnitPrice, TotalAmount)
    VALUES 
    (1, 1, 20, 5.00, 100.00),
    (1, 2, 5, 8.00, 40.00),
    (1, 3, 10, 6.00, 60.00),
    (2, 4, 10, 12.00, 120.00),
    (2, 5, 8, 10.00, 80.00),
    (3, 1, 30, 5.00, 150.00),
    (3, 3, 15, 6.00, 90.00),
    (3, 4, 5, 12.00, 60.00);
END

-- Insert Sample Customer Payments
IF NOT EXISTS (SELECT * FROM CustomerPayments WHERE CustomerID = 2)
BEGIN
    INSERT INTO CustomerPayments (CustomerID, SaleID, Amount, PaymentMethod, Remarks)
    VALUES 
    (2, 2, 100.00, 'Cash', 'Partial payment'),
    (2, 2, 100.00, 'Bank Transfer', 'Final payment');
END

-- =====================================================
-- UPDATE CUSTOMER BALANCES
-- =====================================================

-- Update customer balances based on sales and payments
UPDATE c
SET CurrentBalance = 
    ISNULL((SELECT SUM(s.NetAmount) FROM Sales s WHERE s.CustomerID = c.CustomerID AND s.IsActive = 1), 0) -
    ISNULL((SELECT SUM(cp.Amount) FROM CustomerPayments cp WHERE cp.CustomerID = c.CustomerID AND cp.IsActive = 1), 0)
FROM Customers c;

-- =====================================================
-- SCRIPT COMPLETION MESSAGE
-- =====================================================

PRINT '=====================================================';
PRINT 'Retail Management Database Created Successfully!';
PRINT '=====================================================';
PRINT '';
PRINT 'Database: RetailManagementDB';
PRINT 'Tables Created: 12';
PRINT 'Stored Procedures Created: 8';
PRINT 'Views Created: 3';
PRINT 'Triggers Created: 5';
PRINT 'Sample Data Inserted: Yes';
PRINT '';
PRINT 'Default Login:';
PRINT 'Username: admin';
PRINT 'Password: admin123';
PRINT '';
PRINT '====================================================='; 