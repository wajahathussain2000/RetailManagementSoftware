-- =============================================
-- Purchase Returns Database Setup Script
-- Creates tables, stored procedures, and seed data
-- =============================================

USE [RetailManagementDB]
GO

-- Drop existing tables if they exist
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
    [ProcessedBy] [int] NOT NULL,
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

-- Create indexes
CREATE NONCLUSTERED INDEX [IX_PurchaseReturns_ReturnDate] ON [dbo].[PurchaseReturns] ([ReturnDate])
GO

CREATE NONCLUSTERED INDEX [IX_PurchaseReturnItems_ReturnID] ON [dbo].[PurchaseReturnItems] ([ReturnID])
GO

PRINT 'Purchase Returns tables created successfully!'
GO

-- Seed test data
DECLARE @CompanyID1 INT, @ItemID1 INT, @ItemID2 INT, @ItemID3 INT, @PurchaseID1 INT, @UserID INT

-- Get existing IDs
SELECT TOP 1 @CompanyID1 = CompanyID FROM Companies
SELECT TOP 1 @ItemID1 = ItemID FROM Items
SELECT TOP 1 @ItemID2 = ItemID FROM Items WHERE ItemID != @ItemID1
SELECT TOP 1 @ItemID3 = ItemID FROM Items WHERE ItemID NOT IN (@ItemID1, @ItemID2)
SELECT TOP 1 @PurchaseID1 = PurchaseID FROM Purchases
SELECT TOP 1 @UserID = UserID FROM Users

-- Insert test returns
INSERT INTO PurchaseReturns (ReturnNumber, ReturnDate, OriginalPurchaseID, CompanyID, TotalAmount, ReturnReason, Remarks, ProcessedBy)
VALUES 
('PR001', DATEADD(day, -10, GETDATE()), @PurchaseID1, @CompanyID1, 750.00, 'Expired Product', 'Medicines expired before sale', @UserID),
('PR002', DATEADD(day, -5, GETDATE()), @PurchaseID1, @CompanyID1, 1020.00, 'Damaged Goods', 'Products damaged during transport', @UserID),
('PR003', DATEADD(day, -2, GETDATE()), @PurchaseID1, @CompanyID1, 225.00, 'Wrong Specification', 'Different strength than ordered', @UserID)

-- Get the return IDs
DECLARE @ReturnID1 INT = SCOPE_IDENTITY() - 2
DECLARE @ReturnID2 INT = SCOPE_IDENTITY() - 1  
DECLARE @ReturnID3 INT = SCOPE_IDENTITY()

-- Insert return items
INSERT INTO PurchaseReturnItems (ReturnID, ItemID, ReturnQuantity, UnitPrice, TotalAmount, ReturnReason, BatchNumber, ExpiryDate)
VALUES 
(@ReturnID1, @ItemID1, 50, 15.00, 750.00, 'Expired Product', 'BATCH001', DATEADD(day, -30, GETDATE())),
(@ReturnID2, @ItemID2, 40, 25.50, 1020.00, 'Damaged Goods', 'BATCH002', DATEADD(day, 180, GETDATE())),
(@ReturnID3, @ItemID3, 15, 15.00, 225.00, 'Wrong Specification', 'BATCH003', DATEADD(day, 120, GETDATE()))

PRINT 'Test data inserted successfully!'

-- Verification
SELECT COUNT(*) as TotalReturns FROM PurchaseReturns
SELECT COUNT(*) as TotalReturnItems FROM PurchaseReturnItems

PRINT 'Purchase Returns setup completed!'

