-- Add new columns to Purchases table
ALTER TABLE Purchases ADD PurchaseNo nvarchar(50) NULL;
ALTER TABLE Purchases ADD DueDate datetime2 NULL;
ALTER TABLE Purchases ADD PaymentTerms nvarchar(50) NULL;
ALTER TABLE Purchases ADD SubTotal decimal(18,2) NOT NULL DEFAULT 0;
ALTER TABLE Purchases ADD DiscountPercent decimal(5,2) NOT NULL DEFAULT 0;
ALTER TABLE Purchases ADD DiscountAmount decimal(18,2) NOT NULL DEFAULT 0;
ALTER TABLE Purchases ADD TaxPercent decimal(5,2) NOT NULL DEFAULT 0;
ALTER TABLE Purchases ADD TaxAmount decimal(18,2) NOT NULL DEFAULT 0;

-- Add new columns to PurchaseItems table
ALTER TABLE PurchaseItems ADD ItemUnitId int NULL;
ALTER TABLE PurchaseItems ADD DiscountPercent decimal(5,2) NOT NULL DEFAULT 0;
ALTER TABLE PurchaseItems ADD DiscountAmount decimal(18,2) NOT NULL DEFAULT 0;

-- Add Foreign Key for ItemUnitId
ALTER TABLE PurchaseItems ADD CONSTRAINT FK_PurchaseItems_ItemUnits_ItemUnitId FOREIGN KEY (ItemUnitId) REFERENCES ItemUnits (Id);

-- Initialize AppSettings for Purchase Numbering if not exists
IF NOT EXISTS (SELECT 1 FROM AppSettings WHERE [Key] = 'PurchasePrefix')
BEGIN
    INSERT INTO AppSettings ([Key], [Value]) VALUES ('PurchasePrefix', 'PUR');
END

IF NOT EXISTS (SELECT 1 FROM AppSettings WHERE [Key] = 'PurchaseNextNumber')
BEGIN
    INSERT INTO AppSettings ([Key], [Value]) VALUES ('PurchaseNextNumber', '1');
END

IF NOT EXISTS (SELECT 1 FROM AppSettings WHERE [Key] = 'PurchasePadding')
BEGIN
    INSERT INTO AppSettings ([Key], [Value]) VALUES ('PurchasePadding', '3');
END
