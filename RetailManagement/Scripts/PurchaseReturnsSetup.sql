-- =============================================
-- Purchase Returns Database Setup Script
-- Creates tables, stored procedures, and seed data
-- Compatible with SQL Server Express
-- =============================================

USE [RetailManagementDB]
GO

-- =============================================
-- 1. CREATE PURCHASE RETURNS TABLES
-- =============================================

-- Drop existing tables if they exist (for clean setup)
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PurchaseReturnItems]') AND type in (N'U'))
DROP TABLE [dbo].[PurchaseReturnItems]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PurchaseReturns]') AND type in (N'U'))
DROP TABLE [dbo].[PurchaseReturns]
GO

-- Create PurchaseReturns table
CREATE TABLE [dbo].[PurchaseReturns](
    [ReturnID] [int] IDENTITY(1,1) NOT NULL,
    [ReturnNumber] [varchar](50) NOT NULL,
    [ReturnDate] [datetime] NOT NULL,
    [OriginalPurchaseID] [int] NOT NULL,
    [CompanyID] [int] NOT NULL,
    [TotalAmount] [decimal](18, 2) NOT NULL DEFAULT(0.00),
    [ReturnReason] [varchar](255) NULL,
    [Remarks] [varchar](500) NULL,
    [ProcessedBy] [int] NOT NULL, -- UserID who processed the return
    [CreatedDate] [datetime] NOT NULL DEFAULT(GETDATE()),
    [IsActive] [bit] NOT NULL DEFAULT(1),
    CONSTRAINT [PK_PurchaseReturns] PRIMARY KEY CLUSTERED ([ReturnID] ASC),
    CONSTRAINT [FK_PurchaseReturns_Purchases] FOREIGN KEY([OriginalPurchaseID]) REFERENCES [dbo].[Purchases] ([PurchaseID]),
    CONSTRAINT [FK_PurchaseReturns_Companies] FOREIGN KEY([CompanyID]) REFERENCES [dbo].[Companies] ([CompanyID]),
    CONSTRAINT [FK_PurchaseReturns_Users] FOREIGN KEY([ProcessedBy]) REFERENCES [dbo].[Users] ([UserID])
)
GO

-- Create PurchaseReturnItems table
CREATE TABLE [dbo].[PurchaseReturnItems](
    [ReturnItemID] [int] IDENTITY(1,1) NOT NULL,
    [ReturnID] [int] NOT NULL,
    [ItemID] [int] NOT NULL,
    [ReturnQuantity] [int] NOT NULL,
    [UnitPrice] [decimal](18, 2) NOT NULL,
    [TotalAmount] [decimal](18, 2) NOT NULL,
    [ReturnReason] [varchar](255) NULL,
    [BatchNumber] [varchar](50) NULL,
    [ExpiryDate] [datetime] NULL,
    [CreatedDate] [datetime] NOT NULL DEFAULT(GETDATE()),
    CONSTRAINT [PK_PurchaseReturnItems] PRIMARY KEY CLUSTERED ([ReturnItemID] ASC),
    CONSTRAINT [FK_PurchaseReturnItems_PurchaseReturns] FOREIGN KEY([ReturnID]) REFERENCES [dbo].[PurchaseReturns] ([ReturnID]),
    CONSTRAINT [FK_PurchaseReturnItems_Items] FOREIGN KEY([ItemID]) REFERENCES [dbo].[Items] ([ItemID])
)
GO

-- =============================================
-- 2. CREATE INDEXES FOR PERFORMANCE
-- =============================================

CREATE NONCLUSTERED INDEX [IX_PurchaseReturns_ReturnDate] ON [dbo].[PurchaseReturns] ([ReturnDate])
GO

CREATE NONCLUSTERED INDEX [IX_PurchaseReturns_ReturnNumber] ON [dbo].[PurchaseReturns] ([ReturnNumber])
GO

CREATE NONCLUSTERED INDEX [IX_PurchaseReturns_CompanyID] ON [dbo].[PurchaseReturns] ([CompanyID])
GO

CREATE NONCLUSTERED INDEX [IX_PurchaseReturnItems_ReturnID] ON [dbo].[PurchaseReturnItems] ([ReturnID])
GO

-- =============================================
-- 3. CREATE STORED PROCEDURES
-- =============================================

-- Stored Procedure: Get Purchase Returns Report Data
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetPurchaseReturnsReport]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_GetPurchaseReturnsReport]
GO

CREATE PROCEDURE [dbo].[sp_GetPurchaseReturnsReport]
    @FromDate DATETIME,
    @ToDate DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        pr.ReturnID,
        pr.ReturnNumber as OriginalPurchaseID,
        pr.ReturnDate,
        c.CompanyName as SupplierName,
        i.ItemName,
        pri.ReturnQuantity as ReturnQuantity,
        pri.ReturnReason,
        pri.UnitPrice as RefundAmount,
        'Processed' as ReturnStatus,
        u.FullName as ProcessedBy
    FROM PurchaseReturns pr
    INNER JOIN Companies c ON pr.CompanyID = c.CompanyID
    INNER JOIN PurchaseReturnItems pri ON pr.ReturnID = pri.ReturnID
    INNER JOIN Items i ON pri.ItemID = i.ItemID
    INNER JOIN Users u ON pr.ProcessedBy = u.UserID
    WHERE pr.ReturnDate BETWEEN @FromDate AND @ToDate
        AND pr.IsActive = 1
    ORDER BY pr.ReturnDate DESC, pr.ReturnNumber
END
GO

-- Stored Procedure: Create Purchase Return
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_CreatePurchaseReturn]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_CreatePurchaseReturn]
GO

CREATE PROCEDURE [dbo].[sp_CreatePurchaseReturn]
    @ReturnNumber VARCHAR(50),
    @OriginalPurchaseID INT,
    @CompanyID INT,
    @ReturnReason VARCHAR(255),
    @Remarks VARCHAR(500),
    @ProcessedBy INT,
    @ReturnID INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRANSACTION;
    
    TRY
        -- Insert purchase return header
        INSERT INTO PurchaseReturns (ReturnNumber, ReturnDate, OriginalPurchaseID, CompanyID, ReturnReason, Remarks, ProcessedBy)
        VALUES (@ReturnNumber, GETDATE(), @OriginalPurchaseID, @CompanyID, @ReturnReason, @Remarks, @ProcessedBy);
        
        SET @ReturnID = SCOPE_IDENTITY();
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- Stored Procedure: Add Purchase Return Item
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_AddPurchaseReturnItem]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_AddPurchaseReturnItem]
GO

CREATE PROCEDURE [dbo].[sp_AddPurchaseReturnItem]
    @ReturnID INT,
    @ItemID INT,
    @ReturnQuantity INT,
    @UnitPrice DECIMAL(18,2),
    @ReturnReason VARCHAR(255),
    @BatchNumber VARCHAR(50) = NULL,
    @ExpiryDate DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @TotalAmount DECIMAL(18,2) = @ReturnQuantity * @UnitPrice;
    
    BEGIN TRANSACTION;
    
    TRY
        -- Insert return item
        INSERT INTO PurchaseReturnItems (ReturnID, ItemID, ReturnQuantity, UnitPrice, TotalAmount, ReturnReason, BatchNumber, ExpiryDate)
        VALUES (@ReturnID, @ItemID, @ReturnQuantity, @UnitPrice, @TotalAmount, @ReturnReason, @BatchNumber, @ExpiryDate);
        
        -- Update purchase return total
        UPDATE PurchaseReturns 
        SET TotalAmount = (
            SELECT SUM(TotalAmount) 
            FROM PurchaseReturnItems 
            WHERE ReturnID = @ReturnID
        )
        WHERE ReturnID = @ReturnID;
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- =============================================
-- 4. SEED TEST DATA
-- =============================================

-- First, let's get some existing data to reference
DECLARE @CompanyID1 INT, @CompanyID2 INT;
DECLARE @ItemID1 INT, @ItemID2 INT, @ItemID3 INT;
DECLARE @PurchaseID1 INT, @PurchaseID2 INT;
DECLARE @UserID INT;

-- Get existing company IDs
SELECT TOP 1 @CompanyID1 = CompanyID FROM Companies WHERE CompanyName LIKE '%Medical%' OR CompanyName LIKE '%Pharma%';
IF @CompanyID1 IS NULL
    SELECT TOP 1 @CompanyID1 = CompanyID FROM Companies;

SELECT TOP 1 @CompanyID2 = CompanyID FROM Companies WHERE CompanyID != @CompanyID1;
IF @CompanyID2 IS NULL
    SET @CompanyID2 = @CompanyID1;

-- Get existing item IDs
SELECT TOP 1 @ItemID1 = ItemID FROM Items WHERE ItemName LIKE '%Paracetamol%' OR ItemName LIKE '%Aspirin%';
IF @ItemID1 IS NULL
    SELECT TOP 1 @ItemID1 = ItemID FROM Items;

SELECT TOP 1 @ItemID2 = ItemID FROM Items WHERE ItemID != @ItemID1 AND (ItemName LIKE '%Antibiotic%' OR ItemName LIKE '%Tablet%');
IF @ItemID2 IS NULL
    SELECT TOP 1 @ItemID2 = ItemID FROM Items WHERE ItemID != @ItemID1;

SELECT TOP 1 @ItemID3 = ItemID FROM Items WHERE ItemID NOT IN (@ItemID1, @ItemID2);
IF @ItemID3 IS NULL
    SET @ItemID3 = @ItemID1;

-- Get existing purchase IDs
SELECT TOP 1 @PurchaseID1 = PurchaseID FROM Purchases;
SELECT TOP 1 @PurchaseID2 = PurchaseID FROM Purchases WHERE PurchaseID != @PurchaseID1;
IF @PurchaseID2 IS NULL
    SET @PurchaseID2 = @PurchaseID1;

-- Get existing user ID
SELECT TOP 1 @UserID = UserID FROM Users WHERE Role = 'Admin' OR Role = 'Pharmacist';
IF @UserID IS NULL
    SELECT TOP 1 @UserID = UserID FROM Users;

-- Now insert test purchase returns
PRINT 'Inserting test purchase return data...';

-- Purchase Return 1: Expired medicines
INSERT INTO PurchaseReturns (ReturnNumber, ReturnDate, OriginalPurchaseID, CompanyID, TotalAmount, ReturnReason, Remarks, ProcessedBy)
VALUES ('PR001', DATEADD(day, -10, GETDATE()), @PurchaseID1, @CompanyID1, 0.00, 'Expired Product', 'Medicines expired before sale', @UserID);

DECLARE @ReturnID1 INT = SCOPE_IDENTITY();

-- Purchase Return 2: Damaged goods
INSERT INTO PurchaseReturns (ReturnNumber, ReturnDate, OriginalPurchaseID, CompanyID, TotalAmount, ReturnReason, Remarks, ProcessedBy)
VALUES ('PR002', DATEADD(day, -5, GETDATE()), @PurchaseID2, @CompanyID2, 0.00, 'Damaged Goods', 'Products damaged during transport', @UserID);

DECLARE @ReturnID2 INT = SCOPE_IDENTITY();

-- Purchase Return 3: Wrong specification
INSERT INTO PurchaseReturns (ReturnNumber, ReturnDate, OriginalPurchaseID, CompanyID, TotalAmount, ReturnReason, Remarks, ProcessedBy)
VALUES ('PR003', DATEADD(day, -2, GETDATE()), @PurchaseID1, @CompanyID1, 0.00, 'Wrong Specification', 'Different strength than ordered', @UserID);

DECLARE @ReturnID3 INT = SCOPE_IDENTITY();

-- Insert return items for Return 1
INSERT INTO PurchaseReturnItems (ReturnID, ItemID, ReturnQuantity, UnitPrice, TotalAmount, ReturnReason, BatchNumber, ExpiryDate)
VALUES 
(@ReturnID1, @ItemID1, 50, 15.00, 750.00, 'Expired Product', 'BATCH001', DATEADD(day, -30, GETDATE())),
(@ReturnID1, @ItemID2, 25, 25.50, 637.50, 'Expired Product', 'BATCH002', DATEADD(day, -15, GETDATE()));

-- Insert return items for Return 2
INSERT INTO PurchaseReturnItems (ReturnID, ItemID, ReturnQuantity, UnitPrice, TotalAmount, ReturnReason, BatchNumber, ExpiryDate)
VALUES 
(@ReturnID2, @ItemID2, 30, 25.50, 765.00, 'Damaged Goods', 'BATCH003', DATEADD(day, 180, GETDATE())),
(@ReturnID2, @ItemID3, 20, 12.75, 255.00, 'Damaged Goods', 'BATCH004', DATEADD(day, 200, GETDATE()));

-- Insert return items for Return 3
INSERT INTO PurchaseReturnItems (ReturnID, ItemID, ReturnQuantity, UnitPrice, TotalAmount, ReturnReason, BatchNumber, ExpiryDate)
VALUES 
(@ReturnID3, @ItemID1, 15, 15.00, 225.00, 'Wrong Specification', 'BATCH005', DATEADD(day, 120, GETDATE()));

-- Update total amounts for each return
UPDATE PurchaseReturns 
SET TotalAmount = (SELECT SUM(TotalAmount) FROM PurchaseReturnItems WHERE ReturnID = @ReturnID1)
WHERE ReturnID = @ReturnID1;

UPDATE PurchaseReturns 
SET TotalAmount = (SELECT SUM(TotalAmount) FROM PurchaseReturnItems WHERE ReturnID = @ReturnID2)
WHERE ReturnID = @ReturnID2;

UPDATE PurchaseReturns 
SET TotalAmount = (SELECT SUM(TotalAmount) FROM PurchaseReturnItems WHERE ReturnID = @ReturnID3)
WHERE ReturnID = @ReturnID3;

-- =============================================
-- 5. VERIFICATION QUERIES
-- =============================================

PRINT 'Purchase Returns setup completed successfully!';
PRINT '';
PRINT 'Summary of created data:';

SELECT 
    COUNT(*) as TotalReturns,
    SUM(TotalAmount) as TotalReturnValue
FROM PurchaseReturns 
WHERE IsActive = 1;

SELECT 
    COUNT(*) as TotalReturnItems,
    SUM(TotalAmount) as TotalItemValue
FROM PurchaseReturnItems;

PRINT '';
PRINT 'Sample data verification:';

SELECT 
    pr.ReturnNumber,
    pr.ReturnDate,
    c.CompanyName,
    pr.ReturnReason,
    pr.TotalAmount,
    COUNT(pri.ReturnItemID) as ItemCount
FROM PurchaseReturns pr
INNER JOIN Companies c ON pr.CompanyID = c.CompanyID
LEFT JOIN PurchaseReturnItems pri ON pr.ReturnID = pri.ReturnID
WHERE pr.IsActive = 1
GROUP BY pr.ReturnID, pr.ReturnNumber, pr.ReturnDate, c.CompanyName, pr.ReturnReason, pr.TotalAmount
ORDER BY pr.ReturnDate DESC;

PRINT '';
PRINT 'Purchase Returns tables and data are ready for testing!';

GO

