-- SQL Script to Create Item Master and Item Units tables

-- Ensure Unit table exists
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Units' and xtype='U')
BEGIN
CREATE TABLE [Units] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Name] NVARCHAR(MAX) NOT NULL,
    [Code] NVARCHAR(MAX) NOT NULL,
    [DecimalPlaces] INT NOT NULL,
    [IsActive] BIT NOT NULL
);
END

-- Create Items Table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Items' and xtype='U')
BEGIN
    CREATE TABLE [Items] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Name] NVARCHAR(MAX) NOT NULL,
        [ItemCode] NVARCHAR(MAX) NOT NULL DEFAULT N'',
        [Barcode] NVARCHAR(MAX) NOT NULL DEFAULT N'',
        [UnitId] INT NOT NULL,
        [Rate] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [PurchaseRate] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [WholesaleRate] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [IsMultiUnit] BIT NOT NULL DEFAULT 0,
        [IsActive] BIT NOT NULL DEFAULT 1,
        CONSTRAINT [FK_Items_Units_UnitId] FOREIGN KEY ([UnitId]) REFERENCES [Units] ([Id])
    );
END
ELSE
BEGIN
    -- If table exists, add missing columns (Idempotent approach)
    IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'ItemCode' AND Object_ID = Object_ID(N'Items'))
        ALTER TABLE [Items] ADD [ItemCode] NVARCHAR(MAX) NOT NULL DEFAULT N'';
        
    IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'Barcode' AND Object_ID = Object_ID(N'Items'))
        ALTER TABLE [Items] ADD [Barcode] NVARCHAR(MAX) NOT NULL DEFAULT N'';
        
    IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'PurchaseRate' AND Object_ID = Object_ID(N'Items'))
        ALTER TABLE [Items] ADD [PurchaseRate] DECIMAL(18,2) NOT NULL DEFAULT 0;
        
    IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'WholesaleRate' AND Object_ID = Object_ID(N'Items'))
        ALTER TABLE [Items] ADD [WholesaleRate] DECIMAL(18,2) NOT NULL DEFAULT 0;
        
    IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'IsMultiUnit' AND Object_ID = Object_ID(N'Items'))
        ALTER TABLE [Items] ADD [IsMultiUnit] BIT NOT NULL DEFAULT 0;
END

-- Create ItemUnits Table (Multi-Unit Storage)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ItemUnits' and xtype='U')
BEGIN
    CREATE TABLE [ItemUnits] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [ItemId] INT NOT NULL,
        [UnitId] INT NOT NULL,
        [Name] NVARCHAR(MAX) NOT NULL DEFAULT N'',
        [Barcode] NVARCHAR(MAX) NOT NULL DEFAULT N'',
        [ItemUnitCode] NVARCHAR(MAX) NOT NULL DEFAULT N'',
        [QtyPerBaseUnit] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [Rate] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [PurchaseRate] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [WholesaleRate] DECIMAL(18,2) NOT NULL DEFAULT 0,
        CONSTRAINT [FK_ItemUnits_Items_ItemId] FOREIGN KEY ([ItemId]) REFERENCES [Items] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ItemUnits_Units_UnitId] FOREIGN KEY ([UnitId]) REFERENCES [Units] ([Id])
    );
END
ELSE
BEGIN
     IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'Name' AND Object_ID = Object_ID(N'ItemUnits'))
        ALTER TABLE [ItemUnits] ADD [Name] NVARCHAR(MAX) NOT NULL DEFAULT N'';
END
