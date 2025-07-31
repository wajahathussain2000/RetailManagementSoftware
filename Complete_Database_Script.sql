-- =============================================
-- Retail Management System - Complete Database Script
-- =============================================

-- Create Database
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'RetailManagementDB')
BEGIN
    CREATE DATABASE RetailManagementDB;
END
GO

USE RetailManagementDB;
GO

-- =============================================
-- Create Tables
-- =============================================

-- Users Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Users](
        [UserID] [int] IDENTITY(1,1) NOT NULL,
        [Username] [nvarchar](50) NOT NULL,
        [Password] [nvarchar](100) NOT NULL,
        [FullName] [nvarchar](100) NOT NULL,
        [Email] [nvarchar](100) NULL,
        [IsActive] [bit] NOT NULL DEFAULT(1),
        [CreatedDate] [datetime] NOT NULL DEFAULT(GETDATE()),
        CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([UserID] ASC)
    );
END
GO

-- Items Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Items]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Items](
        [ItemID] [int] IDENTITY(1,1) NOT NULL,
        [ItemName] [nvarchar](100) NOT NULL,
        [Description] [nvarchar](500) NULL,
        [Price] [decimal](18, 2) NOT NULL,
        [StockQuantity] [int] NOT NULL DEFAULT(0),
        [Category] [nvarchar](50) NULL,
        [CreatedDate] [datetime] NOT NULL DEFAULT(GETDATE()),
        [IsActive] [bit] NOT NULL DEFAULT(1),
        CONSTRAINT [PK_Items] PRIMARY KEY CLUSTERED ([ItemID] ASC)
    );
END
GO

-- Companies Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Companies]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Companies](
        [CompanyID] [int] IDENTITY(1,1) NOT NULL,
        [CompanyName] [nvarchar](100) NOT NULL,
        [ContactPerson] [nvarchar](100) NULL,
        [Phone] [nvarchar](20) NULL,
        [Email] [nvarchar](100) NULL,
        [Address] [nvarchar](500) NULL,
        [CreatedDate] [datetime] NOT NULL DEFAULT(GETDATE()),
        [IsActive] [bit] NOT NULL DEFAULT(1),
        CONSTRAINT [PK_Companies] PRIMARY KEY CLUSTERED ([CompanyID] ASC)
    );
END
GO

-- Customers Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Customers]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Customers](
        [CustomerID] [int] IDENTITY(1,1) NOT NULL,
        [CustomerName] [nvarchar](100) NOT NULL,
        [Phone] [nvarchar](20) NULL,
        [Email] [nvarchar](100) NULL,
        [Address] [nvarchar](500) NULL,
        [Balance] [decimal](18, 2) NOT NULL DEFAULT(0),
        [CreatedDate] [datetime] NOT NULL DEFAULT(GETDATE()),
        [IsActive] [bit] NOT NULL DEFAULT(1),
        CONSTRAINT [PK_Customers] PRIMARY KEY CLUSTERED ([CustomerID] ASC)
    );
END
GO

-- Sales Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Sales]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Sales](
        [SaleID] [int] IDENTITY(1,1) NOT NULL,
        [BillNumber] [nvarchar](20) NOT NULL,
        [CustomerID] [int] NOT NULL,
        [SaleDate] [datetime] NOT NULL DEFAULT(GETDATE()),
        [TotalAmount] [decimal](18, 2) NOT NULL,
        [Discount] [decimal](18, 2) NOT NULL DEFAULT(0),
        [NetAmount] [decimal](18, 2) NOT NULL,
        [PaymentMethod] [nvarchar](50) NULL,
        [IsCredit] [bit] NOT NULL DEFAULT(0),
        [Remarks] [nvarchar](500) NULL,
        [CreatedDate] [datetime] NOT NULL DEFAULT(GETDATE()),
        [IsActive] [bit] NOT NULL DEFAULT(1),
        CONSTRAINT [PK_Sales] PRIMARY KEY CLUSTERED ([SaleID] ASC),
        CONSTRAINT [FK_Sales_Customers] FOREIGN KEY([CustomerID]) REFERENCES [dbo].[Customers] ([CustomerID])
    );
END
GO

-- SaleItems Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SaleItems]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[SaleItems](
        [SaleItemID] [int] IDENTITY(1,1) NOT NULL,
        [SaleID] [int] NOT NULL,
        [ItemID] [int] NOT NULL,
        [Quantity] [int] NOT NULL,
        [Price] [decimal](18, 2) NOT NULL,
        [TotalAmount] [decimal](18, 2) NOT NULL,
        CONSTRAINT [PK_SaleItems] PRIMARY KEY CLUSTERED ([SaleItemID] ASC),
        CONSTRAINT [FK_SaleItems_Sales] FOREIGN KEY([SaleID]) REFERENCES [dbo].[Sales] ([SaleID]),
        CONSTRAINT [FK_SaleItems_Items] FOREIGN KEY([ItemID]) REFERENCES [dbo].[Items] ([ItemID])
    );
END
GO

-- CustomerPayments Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CustomerPayments]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[CustomerPayments](
        [PaymentID] [int] IDENTITY(1,1) NOT NULL,
        [CustomerID] [int] NOT NULL,
        [SaleID] [int] NULL,
        [Amount] [decimal](18, 2) NOT NULL,
        [PaymentDate] [datetime] NOT NULL DEFAULT(GETDATE()),
        [PaymentMethod] [nvarchar](50) NULL,
        [Remarks] [nvarchar](500) NULL,
        [CreatedDate] [datetime] NOT NULL DEFAULT(GETDATE()),
        CONSTRAINT [PK_CustomerPayments] PRIMARY KEY CLUSTERED ([PaymentID] ASC),
        CONSTRAINT [FK_CustomerPayments_Customers] FOREIGN KEY([CustomerID]) REFERENCES [dbo].[Customers] ([CustomerID]),
        CONSTRAINT [FK_CustomerPayments_Sales] FOREIGN KEY([SaleID]) REFERENCES [dbo].[Sales] ([SaleID])
    );
END
GO

-- SaleReturns Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SaleReturns]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[SaleReturns](
        [ReturnID] [int] IDENTITY(1,1) NOT NULL,
        [OriginalSaleID] [int] NOT NULL,
        [ReturnDate] [datetime] NOT NULL DEFAULT(GETDATE()),
        [TotalAmount] [decimal](18, 2) NOT NULL,
        [Remarks] [nvarchar](500) NULL,
        [CreatedDate] [datetime] NOT NULL DEFAULT(GETDATE()),
        CONSTRAINT [PK_SaleReturns] PRIMARY KEY CLUSTERED ([ReturnID] ASC),
        CONSTRAINT [FK_SaleReturns_Sales] FOREIGN KEY([OriginalSaleID]) REFERENCES [dbo].[Sales] ([SaleID])
    );
END
GO

-- SaleReturnItems Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SaleReturnItems]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[SaleReturnItems](
        [ReturnItemID] [int] IDENTITY(1,1) NOT NULL,
        [ReturnID] [int] NOT NULL,
        [ItemID] [int] NOT NULL,
        [ReturnQuantity] [int] NOT NULL,
        [Price] [decimal](18, 2) NOT NULL,
        [TotalAmount] [decimal](18, 2) NOT NULL,
        CONSTRAINT [PK_SaleReturnItems] PRIMARY KEY CLUSTERED ([ReturnItemID] ASC),
        CONSTRAINT [FK_SaleReturnItems_SaleReturns] FOREIGN KEY([ReturnID]) REFERENCES [dbo].[SaleReturns] ([ReturnID]),
        CONSTRAINT [FK_SaleReturnItems_Items] FOREIGN KEY([ItemID]) REFERENCES [dbo].[Items] ([ItemID])
    );
END
GO

-- Purchases Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Purchases]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Purchases](
        [PurchaseID] [int] IDENTITY(1,1) NOT NULL,
        [PurchaseNumber] [nvarchar](20) NOT NULL,
        [CompanyID] [int] NOT NULL,
        [PurchaseDate] [datetime] NOT NULL DEFAULT(GETDATE()),
        [TotalAmount] [decimal](18, 2) NOT NULL,
        [Remarks] [nvarchar](500) NULL,
        [CreatedDate] [datetime] NOT NULL DEFAULT(GETDATE()),
        [IsActive] [bit] NOT NULL DEFAULT(1),
        CONSTRAINT [PK_Purchases] PRIMARY KEY CLUSTERED ([PurchaseID] ASC),
        CONSTRAINT [FK_Purchases_Companies] FOREIGN KEY([CompanyID]) REFERENCES [dbo].[Companies] ([CompanyID])
    );
END
GO

-- PurchaseItems Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PurchaseItems]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[PurchaseItems](
        [PurchaseItemID] [int] IDENTITY(1,1) NOT NULL,
        [PurchaseID] [int] NOT NULL,
        [ItemID] [int] NOT NULL,
        [Quantity] [int] NOT NULL,
        [Price] [decimal](18, 2) NOT NULL,
        [TotalAmount] [decimal](18, 2) NOT NULL,
        CONSTRAINT [PK_PurchaseItems] PRIMARY KEY CLUSTERED ([PurchaseItemID] ASC),
        CONSTRAINT [FK_PurchaseItems_Purchases] FOREIGN KEY([PurchaseID]) REFERENCES [dbo].[Purchases] ([PurchaseID]),
        CONSTRAINT [FK_PurchaseItems_Items] FOREIGN KEY([ItemID]) REFERENCES [dbo].[Items] ([ItemID])
    );
END
GO

-- =============================================
-- Create Indexes
-- =============================================

-- Items Indexes
CREATE INDEX [IX_Items_Category] ON [dbo].[Items] ([Category]) WHERE [IsActive] = 1;
CREATE INDEX [IX_Items_ItemName] ON [dbo].[Items] ([ItemName]) WHERE [IsActive] = 1;

-- Sales Indexes
CREATE INDEX [IX_Sales_BillNumber] ON [dbo].[Sales] ([BillNumber]) WHERE [IsActive] = 1;
CREATE INDEX [IX_Sales_CustomerID] ON [dbo].[Sales] ([CustomerID]) WHERE [IsActive] = 1;
CREATE INDEX [IX_Sales_SaleDate] ON [dbo].[Sales] ([SaleDate]) WHERE [IsActive] = 1;
CREATE INDEX [IX_Sales_IsCredit] ON [dbo].[Sales] ([IsCredit]) WHERE [IsActive] = 1;

-- Customers Indexes
CREATE INDEX [IX_Customers_CustomerName] ON [dbo].[Customers] ([CustomerName]) WHERE [IsActive] = 1;
CREATE INDEX [IX_Customers_Phone] ON [dbo].[Customers] ([Phone]) WHERE [IsActive] = 1;

-- Companies Indexes
CREATE INDEX [IX_Companies_CompanyName] ON [dbo].[Companies] ([CompanyName]) WHERE [IsActive] = 1;

-- Purchases Indexes
CREATE INDEX [IX_Purchases_PurchaseNumber] ON [dbo].[Purchases] ([PurchaseNumber]) WHERE [IsActive] = 1;
CREATE INDEX [IX_Purchases_CompanyID] ON [dbo].[Purchases] ([CompanyID]) WHERE [IsActive] = 1;

-- =============================================
-- Create Stored Procedures
-- =============================================

-- Get All Active Items
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetAllItems]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_GetAllItems]
GO

CREATE PROCEDURE [dbo].[sp_GetAllItems]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ItemID, ItemName, Description, Price, StockQuantity, Category, CreatedDate
    FROM Items 
    WHERE IsActive = 1
    ORDER BY ItemName;
END
GO

-- Get Item by ID
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetItemByID]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_GetItemByID]
GO

CREATE PROCEDURE [dbo].[sp_GetItemByID]
    @ItemID int
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ItemID, ItemName, Description, Price, StockQuantity, Category, CreatedDate
    FROM Items 
    WHERE ItemID = @ItemID AND IsActive = 1;
END
GO

-- Insert Item
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_InsertItem]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_InsertItem]
GO

CREATE PROCEDURE [dbo].[sp_InsertItem]
    @ItemName nvarchar(100),
    @Description nvarchar(500) = NULL,
    @Price decimal(18,2),
    @StockQuantity int,
    @Category nvarchar(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Items (ItemName, Description, Price, StockQuantity, Category, CreatedDate, IsActive)
    VALUES (@ItemName, @Description, @Price, @StockQuantity, @Category, GETDATE(), 1);
    
    SELECT SCOPE_IDENTITY() AS ItemID;
END
GO

-- Update Item
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_UpdateItem]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_UpdateItem]
GO

CREATE PROCEDURE [dbo].[sp_UpdateItem]
    @ItemID int,
    @ItemName nvarchar(100),
    @Description nvarchar(500) = NULL,
    @Price decimal(18,2),
    @StockQuantity int,
    @Category nvarchar(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Items 
    SET ItemName = @ItemName,
        Description = @Description,
        Price = @Price,
        StockQuantity = @StockQuantity,
        Category = @Category
    WHERE ItemID = @ItemID;
END
GO

-- Delete Item (Soft Delete)
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_DeleteItem]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_DeleteItem]
GO

CREATE PROCEDURE [dbo].[sp_DeleteItem]
    @ItemID int
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Items 
    SET IsActive = 0
    WHERE ItemID = @ItemID;
END
GO

-- Get Customer Balance
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetCustomerBalance]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_GetCustomerBalance]
GO

CREATE PROCEDURE [dbo].[sp_GetCustomerBalance]
    @CustomerID int = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @CustomerID IS NULL
    BEGIN
        SELECT 
            c.CustomerID,
            c.CustomerName,
            c.Phone,
            ISNULL(SUM(s.NetAmount), 0) as TotalSales,
            ISNULL(SUM(cp.Amount), 0) as TotalPayments,
            ISNULL(SUM(s.NetAmount), 0) - ISNULL(SUM(cp.Amount), 0) as Balance
        FROM Customers c
        LEFT JOIN Sales s ON c.CustomerID = s.CustomerID AND s.IsActive = 1
        LEFT JOIN CustomerPayments cp ON c.CustomerID = cp.CustomerID
        WHERE c.IsActive = 1
        GROUP BY c.CustomerID, c.CustomerName, c.Phone
        ORDER BY c.CustomerName;
    END
    ELSE
    BEGIN
        SELECT 
            c.CustomerID,
            c.CustomerName,
            c.Phone,
            ISNULL(SUM(s.NetAmount), 0) as TotalSales,
            ISNULL(SUM(cp.Amount), 0) as TotalPayments,
            ISNULL(SUM(s.NetAmount), 0) - ISNULL(SUM(cp.Amount), 0) as Balance
        FROM Customers c
        LEFT JOIN Sales s ON c.CustomerID = s.CustomerID AND s.IsActive = 1
        LEFT JOIN CustomerPayments cp ON c.CustomerID = cp.CustomerID
        WHERE c.CustomerID = @CustomerID AND c.IsActive = 1
        GROUP BY c.CustomerID, c.CustomerName, c.Phone;
    END
END
GO

-- Get Sales Report
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetSalesReport]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_GetSalesReport]
GO

CREATE PROCEDURE [dbo].[sp_GetSalesReport]
    @FromDate datetime,
    @ToDate datetime
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        s.SaleID,
        s.BillNumber,
        c.CustomerName,
        s.SaleDate,
        s.TotalAmount,
        s.Discount,
        s.NetAmount,
        s.PaymentMethod
    FROM Sales s
    INNER JOIN Customers c ON s.CustomerID = c.CustomerID
    WHERE s.IsActive = 1 
    AND s.SaleDate BETWEEN @FromDate AND @ToDate
    ORDER BY s.SaleDate DESC;
END
GO

-- Get Stock in Hand
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetStockInHand]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_GetStockInHand]
GO

CREATE PROCEDURE [dbo].[sp_GetStockInHand]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        i.ItemID,
        i.ItemName,
        i.Category,
        i.Price,
        i.StockQuantity,
        i.Price * i.StockQuantity as StockValue
    FROM Items i
    WHERE i.IsActive = 1
    ORDER BY i.Category, i.ItemName;
END
GO

-- Insert Sale
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_InsertSale]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_InsertSale]
GO

CREATE PROCEDURE [dbo].[sp_InsertSale]
    @BillNumber nvarchar(20),
    @CustomerID int,
    @TotalAmount decimal(18,2),
    @Discount decimal(18,2) = 0,
    @NetAmount decimal(18,2),
    @PaymentMethod nvarchar(50) = NULL,
    @IsCredit bit = 0,
    @Remarks nvarchar(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    
    BEGIN TRY
        INSERT INTO Sales (BillNumber, CustomerID, SaleDate, TotalAmount, Discount, NetAmount, PaymentMethod, IsCredit, Remarks, CreatedDate, IsActive)
        VALUES (@BillNumber, @CustomerID, GETDATE(), @TotalAmount, @Discount, @NetAmount, @PaymentMethod, @IsCredit, @Remarks, GETDATE(), 1);
        
        SELECT SCOPE_IDENTITY() AS SaleID;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- Insert Sale Item
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_InsertSaleItem]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_InsertSaleItem]
GO

CREATE PROCEDURE [dbo].[sp_InsertSaleItem]
    @SaleID int,
    @ItemID int,
    @Quantity int,
    @Price decimal(18,2),
    @TotalAmount decimal(18,2)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    
    BEGIN TRY
        INSERT INTO SaleItems (SaleID, ItemID, Quantity, Price, TotalAmount)
        VALUES (@SaleID, @ItemID, @Quantity, @Price, @TotalAmount);
        
        -- Update stock
        UPDATE Items 
        SET StockQuantity = StockQuantity - @Quantity
        WHERE ItemID = @ItemID;
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- Get Credit Sales
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetCreditSales]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_GetCreditSales]
GO

CREATE PROCEDURE [dbo].[sp_GetCreditSales]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        s.SaleID,
        s.BillNumber,
        c.CustomerName,
        s.SaleDate,
        s.NetAmount,
        s.NetAmount - ISNULL(SUM(cp.Amount), 0) as Balance
    FROM Sales s
    INNER JOIN Customers c ON s.CustomerID = c.CustomerID
    LEFT JOIN CustomerPayments cp ON s.SaleID = cp.SaleID
    WHERE s.IsCredit = 1 AND s.IsActive = 1
    GROUP BY s.SaleID, s.BillNumber, c.CustomerName, s.SaleDate, s.NetAmount
    HAVING s.NetAmount - ISNULL(SUM(cp.Amount), 0) > 0
    ORDER BY s.SaleDate DESC;
END
GO

-- Insert Customer Payment
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_InsertCustomerPayment]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_InsertCustomerPayment]
GO

CREATE PROCEDURE [dbo].[sp_InsertCustomerPayment]
    @CustomerID int,
    @SaleID int = NULL,
    @Amount decimal(18,2),
    @PaymentMethod nvarchar(50),
    @Remarks nvarchar(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO CustomerPayments (CustomerID, SaleID, Amount, PaymentDate, PaymentMethod, Remarks, CreatedDate)
    VALUES (@CustomerID, @SaleID, @Amount, GETDATE(), @PaymentMethod, @Remarks, GETDATE());
END
GO

-- Get Customer Ledger
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetCustomerLedger]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_GetCustomerLedger]
GO

CREATE PROCEDURE [dbo].[sp_GetCustomerLedger]
    @CustomerID int
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        Date,
        Description,
        Debit,
        Credit,
        Balance
    FROM (
        SELECT 
            s.SaleDate as Date,
            'Sale - ' + s.BillNumber as Description,
            s.NetAmount as Debit,
            0 as Credit,
            0 as Balance
        FROM Sales s
        WHERE s.CustomerID = @CustomerID AND s.IsActive = 1
        
        UNION ALL
        
        SELECT 
            cp.PaymentDate as Date,
            'Payment - ' + cp.PaymentMethod as Description,
            0 as Debit,
            cp.Amount as Credit,
            0 as Balance
        FROM CustomerPayments cp
        WHERE cp.CustomerID = @CustomerID
    ) as Ledger
    ORDER BY Date;
END
GO

-- Insert Purchase
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_InsertPurchase]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_InsertPurchase]
GO

CREATE PROCEDURE [dbo].[sp_InsertPurchase]
    @PurchaseNumber nvarchar(20),
    @CompanyID int,
    @TotalAmount decimal(18,2),
    @Remarks nvarchar(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    
    BEGIN TRY
        INSERT INTO Purchases (PurchaseNumber, CompanyID, PurchaseDate, TotalAmount, Remarks, CreatedDate, IsActive)
        VALUES (@PurchaseNumber, @CompanyID, GETDATE(), @TotalAmount, @Remarks, GETDATE(), 1);
        
        SELECT SCOPE_IDENTITY() AS PurchaseID;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- Insert Purchase Item
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_InsertPurchaseItem]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_InsertPurchaseItem]
GO

CREATE PROCEDURE [dbo].[sp_InsertPurchaseItem]
    @PurchaseID int,
    @ItemID int,
    @Quantity int,
    @Price decimal(18,2),
    @TotalAmount decimal(18,2)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    
    BEGIN TRY
        INSERT INTO PurchaseItems (PurchaseID, ItemID, Quantity, Price, TotalAmount)
        VALUES (@PurchaseID, @ItemID, @Quantity, @Price, @TotalAmount);
        
        -- Update stock
        UPDATE Items 
        SET StockQuantity = StockQuantity + @Quantity
        WHERE ItemID = @ItemID;
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- =============================================
-- Create Views
-- =============================================

-- Customer Balance View
IF EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vw_CustomerBalance]'))
    DROP VIEW [dbo].[vw_CustomerBalance]
GO

CREATE VIEW [dbo].[vw_CustomerBalance]
AS
SELECT 
    c.CustomerID,
    c.CustomerName,
    c.Phone,
    c.Email,
    ISNULL(SUM(s.NetAmount), 0) as TotalSales,
    ISNULL(SUM(cp.Amount), 0) as TotalPayments,
    ISNULL(SUM(s.NetAmount), 0) - ISNULL(SUM(cp.Amount), 0) as Balance
FROM Customers c
LEFT JOIN Sales s ON c.CustomerID = s.CustomerID AND s.IsActive = 1
LEFT JOIN CustomerPayments cp ON c.CustomerID = cp.CustomerID
WHERE c.IsActive = 1
GROUP BY c.CustomerID, c.CustomerName, c.Phone, c.Email
GO

-- Stock Summary View
IF EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vw_StockSummary]'))
    DROP VIEW [dbo].[vw_StockSummary]
GO

CREATE VIEW [dbo].[vw_StockSummary]
AS
SELECT 
    i.ItemID,
    i.ItemName,
    i.Category,
    i.Price,
    i.StockQuantity,
    i.StockQuantity * i.Price as StockValue
FROM Items i
WHERE i.IsActive = 1
GO

-- Sales Summary View
IF EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vw_SalesSummary]'))
    DROP VIEW [dbo].[vw_SalesSummary]
GO

CREATE VIEW [dbo].[vw_SalesSummary]
AS
SELECT 
    s.SaleID,
    s.BillNumber,
    c.CustomerName,
    s.SaleDate,
    s.TotalAmount,
    s.Discount,
    s.NetAmount,
    s.PaymentMethod,
    s.IsCredit
FROM Sales s
INNER JOIN Customers c ON s.CustomerID = c.CustomerID
WHERE s.IsActive = 1
GO

-- =============================================
-- Create Triggers
-- =============================================

-- Update Customer Balance Trigger
IF EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[trg_UpdateCustomerBalance]'))
    DROP TRIGGER [dbo].[trg_UpdateCustomerBalance]
GO

CREATE TRIGGER [dbo].[trg_UpdateCustomerBalance]
ON [dbo].[Sales]
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Update customer balance for inserted/updated sales
    IF EXISTS (SELECT * FROM inserted)
    BEGIN
        UPDATE c
        SET Balance = (
            SELECT ISNULL(SUM(s.NetAmount), 0) - ISNULL(SUM(cp.Amount), 0)
            FROM Customers c2
            LEFT JOIN Sales s ON c2.CustomerID = s.CustomerID AND s.IsActive = 1
            LEFT JOIN CustomerPayments cp ON c2.CustomerID = cp.CustomerID
            WHERE c2.CustomerID = c.CustomerID
        )
        FROM Customers c
        INNER JOIN inserted i ON c.CustomerID = i.CustomerID;
    END
    
    -- Update customer balance for deleted sales
    IF EXISTS (SELECT * FROM deleted)
    BEGIN
        UPDATE c
        SET Balance = (
            SELECT ISNULL(SUM(s.NetAmount), 0) - ISNULL(SUM(cp.Amount), 0)
            FROM Customers c2
            LEFT JOIN Sales s ON c2.CustomerID = s.CustomerID AND s.IsActive = 1
            LEFT JOIN CustomerPayments cp ON c2.CustomerID = cp.CustomerID
            WHERE c2.CustomerID = c.CustomerID
        )
        FROM Customers c
        INNER JOIN deleted d ON c.CustomerID = d.CustomerID;
    END
END
GO

-- Update Customer Balance on Payment Trigger
IF EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[trg_UpdateCustomerBalanceOnPayment]'))
    DROP TRIGGER [dbo].[trg_UpdateCustomerBalanceOnPayment]
GO

CREATE TRIGGER [dbo].[trg_UpdateCustomerBalanceOnPayment]
ON [dbo].[CustomerPayments]
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Update customer balance for inserted/updated payments
    IF EXISTS (SELECT * FROM inserted)
    BEGIN
        UPDATE c
        SET Balance = (
            SELECT ISNULL(SUM(s.NetAmount), 0) - ISNULL(SUM(cp.Amount), 0)
            FROM Customers c2
            LEFT JOIN Sales s ON c2.CustomerID = s.CustomerID AND s.IsActive = 1
            LEFT JOIN CustomerPayments cp ON c2.CustomerID = cp.CustomerID
            WHERE c2.CustomerID = c.CustomerID
        )
        FROM Customers c
        INNER JOIN inserted i ON c.CustomerID = i.CustomerID;
    END
    
    -- Update customer balance for deleted payments
    IF EXISTS (SELECT * FROM deleted)
    BEGIN
        UPDATE c
        SET Balance = (
            SELECT ISNULL(SUM(s.NetAmount), 0) - ISNULL(SUM(cp.Amount), 0)
            FROM Customers c2
            LEFT JOIN Sales s ON c2.CustomerID = s.CustomerID AND s.IsActive = 1
            LEFT JOIN CustomerPayments cp ON c2.CustomerID = cp.CustomerID
            WHERE c2.CustomerID = c.CustomerID
        )
        FROM Customers c
        INNER JOIN deleted d ON c.CustomerID = d.CustomerID;
    END
END
GO

-- =============================================
-- Insert Sample Data
-- =============================================

-- Insert Sample User
IF NOT EXISTS (SELECT * FROM Users WHERE Username = 'admin')
BEGIN
    INSERT INTO Users (Username, Password, FullName, Email, IsActive, CreatedDate)
    VALUES ('admin', 'admin123', 'Administrator', 'admin@retail.com', 1, GETDATE());
END

-- Insert Sample Items
IF NOT EXISTS (SELECT * FROM Items WHERE ItemName = 'Laptop')
BEGIN
    INSERT INTO Items (ItemName, Description, Price, StockQuantity, Category, CreatedDate, IsActive)
    VALUES 
    ('Laptop', 'High performance laptop', 999.99, 10, 'Electronics', GETDATE(), 1),
    ('Mouse', 'Wireless optical mouse', 25.99, 50, 'Electronics', GETDATE(), 1),
    ('Keyboard', 'Mechanical gaming keyboard', 89.99, 20, 'Electronics', GETDATE(), 1),
    ('Monitor', '24 inch LED monitor', 199.99, 15, 'Electronics', GETDATE(), 1),
    ('Headphones', 'Noise cancelling headphones', 149.99, 30, 'Electronics', GETDATE(), 1),
    ('USB Drive', '32GB USB flash drive', 19.99, 100, 'Accessories', GETDATE(), 1),
    ('Webcam', 'HD webcam for video calls', 79.99, 25, 'Electronics', GETDATE(), 1),
    ('Printer', 'All-in-one inkjet printer', 299.99, 8, 'Electronics', GETDATE(), 1);
END

-- Insert Sample Companies
IF NOT EXISTS (SELECT * FROM Companies WHERE CompanyName = 'Tech Solutions Inc.')
BEGIN
    INSERT INTO Companies (CompanyName, ContactPerson, Phone, Email, Address, CreatedDate, IsActive)
    VALUES 
    ('Tech Solutions Inc.', 'John Smith', '555-0101', 'john@techsolutions.com', '123 Tech Street, City', GETDATE(), 1),
    ('Digital Electronics Ltd.', 'Sarah Johnson', '555-0102', 'sarah@digitalelectronics.com', '456 Digital Ave, Town', GETDATE(), 1),
    ('Computer World', 'Mike Wilson', '555-0103', 'mike@computerworld.com', '789 Computer Blvd, Village', GETDATE(), 1);
END

-- Insert Sample Customers
IF NOT EXISTS (SELECT * FROM Customers WHERE CustomerName = 'Alice Johnson')
BEGIN
    INSERT INTO Customers (CustomerName, Phone, Email, Address, Balance, CreatedDate, IsActive)
    VALUES 
    ('Alice Johnson', '555-0201', 'alice@email.com', '123 Main St, City', 0.00, GETDATE(), 1),
    ('Bob Smith', '555-0202', 'bob@email.com', '456 Oak Ave, Town', 0.00, GETDATE(), 1),
    ('Carol Davis', '555-0203', 'carol@email.com', '789 Pine Rd, Village', 0.00, GETDATE(), 1),
    ('David Wilson', '555-0204', 'david@email.com', '321 Elm St, City', 0.00, GETDATE(), 1),
    ('Eva Brown', '555-0205', 'eva@email.com', '654 Maple Dr, Town', 0.00, GETDATE(), 1);
END

-- =============================================
-- Create Additional Stored Procedures
-- =============================================

-- Get Profit and Loss
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetProfitAndLoss]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_GetProfitAndLoss]
GO

CREATE PROCEDURE [dbo].[sp_GetProfitAndLoss]
    @FromDate datetime,
    @ToDate datetime
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @SalesRevenue decimal(18,2)
    DECLARE @CostOfGoodsSold decimal(18,2)
    DECLARE @GrossProfit decimal(18,2)
    DECLARE @OperatingExpenses decimal(18,2)
    DECLARE @NetProfit decimal(18,2)
    
    -- Calculate Sales Revenue
    SELECT @SalesRevenue = ISNULL(SUM(NetAmount), 0)
    FROM Sales 
    WHERE SaleDate BETWEEN @FromDate AND @ToDate AND IsActive = 1
    
    -- Calculate Cost of Goods Sold (simplified)
    SELECT @CostOfGoodsSold = ISNULL(SUM(si.Quantity * i.Price * 0.7), 0)
    FROM Sales s
    INNER JOIN SaleItems si ON s.SaleID = si.SaleID
    INNER JOIN Items i ON si.ItemID = i.ItemID
    WHERE s.SaleDate BETWEEN @FromDate AND @ToDate AND s.IsActive = 1
    
    -- Calculate Gross Profit
    SET @GrossProfit = @SalesRevenue - @CostOfGoodsSold
    
    -- Calculate Operating Expenses (simplified - 15% of sales)
    SET @OperatingExpenses = @SalesRevenue * 0.15
    
    -- Calculate Net Profit
    SET @NetProfit = @GrossProfit - @OperatingExpenses
    
    -- Return results
    SELECT 
        @SalesRevenue as SalesRevenue,
        @CostOfGoodsSold as CostOfGoodsSold,
        @GrossProfit as GrossProfit,
        @OperatingExpenses as OperatingExpenses,
        @NetProfit as NetProfit,
        CASE WHEN @SalesRevenue > 0 THEN (@GrossProfit / @SalesRevenue * 100) ELSE 0 END as GrossProfitMargin,
        CASE WHEN @SalesRevenue > 0 THEN (@NetProfit / @SalesRevenue * 100) ELSE 0 END as NetProfitMargin
END
GO

-- Get Sales by Date Range
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetSalesByDateRange]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_GetSalesByDateRange]
GO

CREATE PROCEDURE [dbo].[sp_GetSalesByDateRange]
    @FromDate datetime,
    @ToDate datetime
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        s.SaleID,
        s.BillNumber,
        c.CustomerName,
        s.SaleDate,
        s.TotalAmount,
        s.Discount,
        s.NetAmount,
        s.PaymentMethod,
        s.IsCredit,
        s.Remarks
    FROM Sales s
    INNER JOIN Customers c ON s.CustomerID = c.CustomerID
    WHERE s.SaleDate BETWEEN @FromDate AND @ToDate AND s.IsActive = 1
    ORDER BY s.SaleDate DESC;
END
GO

-- Get Sale Details by SaleID
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetSaleDetails]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_GetSaleDetails]
GO

CREATE PROCEDURE [dbo].[sp_GetSaleDetails]
    @SaleID int
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get sale header
    SELECT 
        s.*,
        c.CustomerName,
        c.Phone,
        c.Email
    FROM Sales s
    INNER JOIN Customers c ON s.CustomerID = c.CustomerID
    WHERE s.SaleID = @SaleID AND s.IsActive = 1;
    
    -- Get sale items
    SELECT 
        si.*,
        i.ItemName,
        i.Category
    FROM SaleItems si
    INNER JOIN Items i ON si.ItemID = i.ItemID
    WHERE si.SaleID = @SaleID;
END
GO

-- Update Sale
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_UpdateSale]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_UpdateSale]
GO

CREATE PROCEDURE [dbo].[sp_UpdateSale]
    @SaleID int,
    @Discount decimal(18,2),
    @NetAmount decimal(18,2),
    @Remarks nvarchar(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Sales 
    SET Discount = @Discount,
        NetAmount = @NetAmount,
        Remarks = @Remarks
    WHERE SaleID = @SaleID;
END
GO

-- Delete Sale (Soft Delete)
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_DeleteSale]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_DeleteSale]
GO

CREATE PROCEDURE [dbo].[sp_DeleteSale]
    @SaleID int
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Sales 
    SET IsActive = 0
    WHERE SaleID = @SaleID;
END
GO

-- Get Next Bill Number
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetNextBillNumber]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_GetNextBillNumber]
GO

CREATE PROCEDURE [dbo].[sp_GetNextBillNumber]
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @NextNumber int;
    
    SELECT @NextNumber = ISNULL(MAX(CAST(SUBSTRING(BillNumber, 5, LEN(BillNumber)) AS INT)), 0) + 1
    FROM Sales;
    
    SELECT 'BILL' + RIGHT('000000' + CAST(@NextNumber AS VARCHAR(10)), 6) AS NextBillNumber;
END
GO

-- Get Next Purchase Number
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetNextPurchaseNumber]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_GetNextPurchaseNumber]
GO

CREATE PROCEDURE [dbo].[sp_GetNextPurchaseNumber]
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @NextNumber int;
    
    SELECT @NextNumber = ISNULL(MAX(CAST(SUBSTRING(PurchaseNumber, 3, LEN(PurchaseNumber)) AS INT)), 0) + 1
    FROM Purchases;
    
    SELECT 'PO' + RIGHT('000000' + CAST(@NextNumber AS VARCHAR(10)), 6) AS NextPurchaseNumber;
END
GO

PRINT 'Complete database script executed successfully!';
PRINT 'Retail Management System database is ready for use.';
PRINT 'Default admin user: admin / admin123';
PRINT 'Sample data has been inserted for testing.'; 