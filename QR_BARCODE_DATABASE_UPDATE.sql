-- Add QR Code and Barcode columns to Sales and Purchases tables
-- This script adds columns to store QR code data and barcode data for bills

USE RetailManagementDB;
GO

-- Add columns to Sales table for QR and Barcode data
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Sales' AND COLUMN_NAME = 'QRCodeData')
BEGIN
    ALTER TABLE Sales ADD QRCodeData NVARCHAR(500) NULL;
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Sales' AND COLUMN_NAME = 'BarcodeData')
BEGIN
    ALTER TABLE Sales ADD BarcodeData NVARCHAR(200) NULL;
END

-- Add columns to Purchases table for QR and Barcode data
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Purchases' AND COLUMN_NAME = 'QRCodeData')
BEGIN
    ALTER TABLE Purchases ADD QRCodeData NVARCHAR(500) NULL;
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Purchases' AND COLUMN_NAME = 'BarcodeData')
BEGIN
    ALTER TABLE Purchases ADD BarcodeData NVARCHAR(200) NULL;
END

-- Create indexes for better performance when searching by QR or Barcode
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Sales_QRCodeData')
BEGIN
    CREATE INDEX IX_Sales_QRCodeData ON Sales(QRCodeData);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Sales_BarcodeData')
BEGIN
    CREATE INDEX IX_Sales_BarcodeData ON Sales(BarcodeData);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Purchases_QRCodeData')
BEGIN
    CREATE INDEX IX_Purchases_QRCodeData ON Purchases(QRCodeData);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Purchases_BarcodeData')
BEGIN
    CREATE INDEX IX_Purchases_BarcodeData ON Purchases(BarcodeData);
END

PRINT 'QR Code and Barcode columns added successfully to Sales and Purchases tables.';

