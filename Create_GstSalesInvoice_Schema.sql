-- ============================================================
-- GST Invoice Migration Script
-- Run this against your SQL Server database
-- Date: 2026-03-01
-- ============================================================

-- STEP 1: Add GstPercent to Items table
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Items' AND COLUMN_NAME = 'GstPercent')
BEGIN
    ALTER TABLE Items ADD GstPercent decimal(5,2) NOT NULL DEFAULT 0;
    PRINT 'Added GstPercent to Items table.';
END
ELSE
BEGIN
    PRINT 'GstPercent already exists in Items.';
END

-- STEP 2: Create GstSalesInvoices table
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'GstSalesInvoices')
BEGIN
    CREATE TABLE GstSalesInvoices (
        Id               int            NOT NULL IDENTITY(1,1),
        InvoiceNo        nvarchar(50)   NOT NULL,
        [Date]           datetime2      NOT NULL,
        CustomerId       int            NOT NULL,
        DueDate          datetime2      NULL,
        PaymentTerms     nvarchar(100)  NOT NULL DEFAULT '',
        SubTotal         decimal(18,2)  NOT NULL DEFAULT 0,
        DiscountPercent  decimal(5,2)   NOT NULL DEFAULT 0,
        DiscountAmount   decimal(18,2)  NOT NULL DEFAULT 0,
        TotalGstAmount   decimal(18,2)  NOT NULL DEFAULT 0,
        TotalAmount      decimal(18,2)  NOT NULL DEFAULT 0,
        Notes            nvarchar(500)  NOT NULL DEFAULT '',
        CONSTRAINT PK_GstSalesInvoices PRIMARY KEY (Id),
        CONSTRAINT FK_GstSalesInvoices_Customers FOREIGN KEY (CustomerId) REFERENCES Customers (Id)
    );
    PRINT 'Created GstSalesInvoices table.';
END
ELSE
BEGIN
    PRINT 'GstSalesInvoices already exists.';
END

-- STEP 3: Create GstSalesInvoiceItems table
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'GstSalesInvoiceItems')
BEGIN
    CREATE TABLE GstSalesInvoiceItems (
        Id                   int            NOT NULL IDENTITY(1,1),
        GstSalesInvoiceId    int            NOT NULL,
        ItemId               int            NOT NULL,
        UnitId               int            NOT NULL,
        ItemUnitId           int            NULL,
        QtyPerBaseUnit       decimal(18,3)  NOT NULL DEFAULT 1,
        Qty                  decimal(18,3)  NOT NULL DEFAULT 0,
        QtyInBaseUnit        decimal(18,3)  NOT NULL DEFAULT 0,
        Rate                 decimal(18,2)  NOT NULL DEFAULT 0,
        DiscountPercent      decimal(5,2)   NOT NULL DEFAULT 0,
        DiscountAmount       decimal(18,2)  NOT NULL DEFAULT 0,
        Amount               decimal(18,2)  NOT NULL DEFAULT 0,
        GstPercent           decimal(5,2)   NOT NULL DEFAULT 0,
        GstAmount            decimal(18,2)  NOT NULL DEFAULT 0,
        CONSTRAINT PK_GstSalesInvoiceItems PRIMARY KEY (Id),
        CONSTRAINT FK_GstSalesInvoiceItems_Invoice  FOREIGN KEY (GstSalesInvoiceId) REFERENCES GstSalesInvoices (Id) ON DELETE CASCADE,
        CONSTRAINT FK_GstSalesInvoiceItems_Item     FOREIGN KEY (ItemId) REFERENCES Items (Id),
        CONSTRAINT FK_GstSalesInvoiceItems_Unit     FOREIGN KEY (UnitId) REFERENCES Units (Id)
    );
    PRINT 'Created GstSalesInvoiceItems table.';
END
ELSE
BEGIN
    PRINT 'GstSalesInvoiceItems already exists.';
END

-- STEP 4: Seed AppSettings for GST Invoice Numbering
IF NOT EXISTS (SELECT 1 FROM AppSettings WHERE [Key] = 'GstSalesInvoicePrefix')
    INSERT INTO AppSettings ([Key], [Value]) VALUES ('GstSalesInvoicePrefix', 'GINV');

IF NOT EXISTS (SELECT 1 FROM AppSettings WHERE [Key] = 'GstSalesInvoiceNextNumber')
    INSERT INTO AppSettings ([Key], [Value]) VALUES ('GstSalesInvoiceNextNumber', '1');

IF NOT EXISTS (SELECT 1 FROM AppSettings WHERE [Key] = 'GstSalesInvoicePadding')
    INSERT INTO AppSettings ([Key], [Value]) VALUES ('GstSalesInvoicePadding', '3');

PRINT 'AppSettings for GST invoice numbering seeded successfully.';

-- DONE
PRINT 'GST Invoice Migration completed successfully.';
