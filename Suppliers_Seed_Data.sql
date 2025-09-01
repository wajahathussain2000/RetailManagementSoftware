-- =============================================================================================================
-- SUPPLIERS TABLE SEED DATA
-- This script adds sample suppliers to the Suppliers table
-- =============================================================================================================

USE RetailManagementDB;
GO

-- Create some sample suppliers
INSERT INTO Suppliers (SupplierCode, SupplierName, ContactPerson, Phone, Email, Address, City, State, OpeningBalance, CurrentBalance, IsActive, CreatedDate)
VALUES 
    ('SUP001', 'Global Pharmaceuticals Ltd', 'Ahmed Khan', '021-1234567', 'ahmed@globalpharm.com', 'Industrial Area Phase 1', 'Karachi', 'Sindh', 50000.00, 45000.00, 1, GETDATE()),
    
    ('SUP002', 'MediCore Supplies', 'Fatima Ali', '042-2345678', 'fatima@medicore.com', 'Gulberg Medical Complex', 'Lahore', 'Punjab', 25000.00, 30000.00, 1, GETDATE()),
    
    ('SUP003', 'Healthcare Solutions Pvt Ltd', 'Dr. Hassan Sheikh', '051-3456789', 'hassan@healthcare.com', 'Blue Area Medical Plaza', 'Islamabad', 'ICT', 75000.00, 65000.00, 1, GETDATE()),
    
    ('SUP004', 'Metro Medical Distributors', 'Aisha Malik', '021-4567890', 'aisha@metromed.com', 'Saddar Medical Market', 'Karachi', 'Sindh', 30000.00, 35000.00, 1, GETDATE()),
    
    ('SUP005', 'Prime Pharma Trading', 'Usman Tariq', '042-5678901', 'usman@primepharma.com', 'Liberty Market Medical', 'Lahore', 'Punjab', 40000.00, 42000.00, 1, GETDATE()),
    
    ('SUP006', 'Allied Healthcare Group', 'Maria Rodriguez', '092-6789012', 'maria@alliedhc.com', 'University Road Medical', 'Peshawar', 'KPK', 20000.00, 18000.00, 1, GETDATE()),
    
    ('SUP007', 'Nationwide Medicines', 'Ali Raza', '061-7890123', 'ali@nationwide.com', 'City Medical Complex', 'Multan', 'Punjab', 35000.00, 38000.00, 1, GETDATE()),
    
    ('SUP008', 'Apex Pharmaceutical Supply', 'Sara Ahmed', '071-8901234', 'sara@apexpharma.com', 'Main Medical Market', 'Quetta', 'Balochistan', 15000.00, 16000.00, 1, GETDATE()),
    
    ('SUP009', 'Elite Medical Traders', 'Mohammad Hussain', '021-9012345', 'hussain@elitemedical.com', 'Clifton Medical Center', 'Karachi', 'Sindh', 60000.00, 55000.00, 1, GETDATE()),
    
    ('SUP010', 'Reliable Pharma Services', 'Nadia Khan', '042-0123456', 'nadia@reliablepharma.com', 'Model Town Medical', 'Lahore', 'Punjab', 45000.00, 47000.00, 1, GETDATE()),
    
    ('SUP011', 'Continental Healthcare', 'Rizwan Sheikh', '051-1234567', 'rizwan@continental.com', 'F-10 Medical Plaza', 'Islamabad', 'ICT', 55000.00, 52000.00, 1, GETDATE()),
    
    ('SUP012', 'Sterling Medical Corp', 'Ayesha Qureshi', '021-2345678', 'ayesha@sterling.com', 'Defense Medical Complex', 'Karachi', 'Sindh', 38000.00, 40000.00, 1, GETDATE()),
    
    ('SUP013', 'Phoenix Drug Distributors', 'Tariq Mahmood', '042-3456789', 'tariq@phoenix.com', 'Johar Town Medical', 'Lahore', 'Punjab', 32000.00, 34000.00, 1, GETDATE()),
    
    ('SUP014', 'Universal Pharma Hub', 'Sana Malik', '051-4567890', 'sana@universal.com', 'G-9 Medical Center', 'Islamabad', 'ICT', 42000.00, 41000.00, 1, GETDATE()),
    
    ('SUP015', 'Premier Medical Supply', 'Kamran Ali', '021-5678901', 'kamran@premier.com', 'Nazimabad Medical', 'Karachi', 'Sindh', 28000.00, 31000.00, 1, GETDATE());

PRINT 'Successfully inserted 15 sample suppliers with realistic data';

-- Display the inserted data
SELECT 
    SupplierID,
    SupplierCode,
    SupplierName,
    ContactPerson,
    Phone,
    City,
    State,
    OpeningBalance,
    CurrentBalance,
    CASE WHEN IsActive = 1 THEN 'Active' ELSE 'Inactive' END as Status
FROM Suppliers
ORDER BY SupplierName;

PRINT 'Suppliers seed data insertion completed successfully!';
