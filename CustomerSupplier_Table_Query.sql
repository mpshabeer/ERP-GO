-- SQL Script for Customer and Supplier Masters

-- Create Customers Table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Customers' and xtype='U')
BEGIN
    CREATE TABLE [Customers] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [Name] NVARCHAR(MAX) NOT NULL,
        [Code] NVARCHAR(MAX) NOT NULL DEFAULT N'',
        [Address] NVARCHAR(MAX) NOT NULL DEFAULT N'',
        [City] NVARCHAR(MAX) NOT NULL DEFAULT N'',
        [Country] NVARCHAR(MAX) NOT NULL DEFAULT N'',
        [Mobile] NVARCHAR(MAX) NOT NULL DEFAULT N'',
        [Email] NVARCHAR(MAX) NOT NULL DEFAULT N'',
        [TaxNumber] NVARCHAR(MAX) NOT NULL DEFAULT N'',
        [CreditLimit] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [IsActive] BIT NOT NULL DEFAULT 1
    );
END

-- Create Suppliers Table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Suppliers' and xtype='U')
BEGIN
    CREATE TABLE [Suppliers] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [Name] NVARCHAR(MAX) NOT NULL,
        [Code] NVARCHAR(MAX) NOT NULL DEFAULT N'',
        [Address] NVARCHAR(MAX) NOT NULL DEFAULT N'',
        [City] NVARCHAR(MAX) NOT NULL DEFAULT N'',
        [Country] NVARCHAR(MAX) NOT NULL DEFAULT N'',
        [Mobile] NVARCHAR(MAX) NOT NULL DEFAULT N'',
        [Email] NVARCHAR(MAX) NOT NULL DEFAULT N'',
        [TaxNumber] NVARCHAR(MAX) NOT NULL DEFAULT N'',
        [ContactPerson] NVARCHAR(MAX) NOT NULL DEFAULT N'',
        [IsActive] BIT NOT NULL DEFAULT 1
    );
END
