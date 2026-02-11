-- 1. Update Items Table
ALTER TABLE Items ADD CurrentStock decimal(18, 3) NOT NULL DEFAULT 0;
GO

-- 2. Create StockLedger Table
CREATE TABLE StockLedger (
    Id bigint IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Date datetime2 NOT NULL,
    ItemId int NOT NULL,
    Qty decimal(18, 3) NOT NULL,
    TransactionType nvarchar(max) NOT NULL DEFAULT N'',
    RefId nvarchar(max) NOT NULL DEFAULT N'',
    Notes nvarchar(max) NOT NULL DEFAULT N'',
    CONSTRAINT FK_StockLedger_Items_ItemId FOREIGN KEY (ItemId) REFERENCES Items (Id) ON DELETE CASCADE
);
GO

CREATE INDEX IX_StockLedger_ItemId ON StockLedger (ItemId);
GO

-- 3. Create SalesInvoices Table
CREATE TABLE SalesInvoices (
    Id int IDENTITY(1,1) NOT NULL PRIMARY KEY,
    InvoiceNo nvarchar(max) NOT NULL,
    Date datetime2 NOT NULL,
    CustomerId int NOT NULL,
    TotalAmount decimal(18, 2) NOT NULL,
    Notes nvarchar(max) NOT NULL DEFAULT N'',
    CONSTRAINT FK_SalesInvoices_Customers_CustomerId FOREIGN KEY (CustomerId) REFERENCES Customers (Id) ON DELETE CASCADE
);
GO

CREATE INDEX IX_SalesInvoices_CustomerId ON SalesInvoices (CustomerId);
GO

-- 4. Create SalesInvoiceItems Table
CREATE TABLE SalesInvoiceItems (
    Id int IDENTITY(1,1) NOT NULL PRIMARY KEY,
    SalesInvoiceId int NOT NULL,
    ItemId int NOT NULL,
    UnitId int NOT NULL,
    Qty decimal(18, 3) NOT NULL,
    Rate decimal(18, 2) NOT NULL,
    Amount decimal(18, 2) NOT NULL,
    QtyInBaseUnit decimal(18, 3) NOT NULL,
    CONSTRAINT FK_SalesInvoiceItems_SalesInvoices_SalesInvoiceId FOREIGN KEY (SalesInvoiceId) REFERENCES SalesInvoices (Id) ON DELETE CASCADE,
    CONSTRAINT FK_SalesInvoiceItems_Items_ItemId FOREIGN KEY (ItemId) REFERENCES Items (Id) ON DELETE CASCADE,
    CONSTRAINT FK_SalesInvoiceItems_Units_UnitId FOREIGN KEY (UnitId) REFERENCES Units (Id) ON DELETE NO ACTION
);
GO

CREATE INDEX IX_SalesInvoiceItems_SalesInvoiceId ON SalesInvoiceItems (SalesInvoiceId);
CREATE INDEX IX_SalesInvoiceItems_ItemId ON SalesInvoiceItems (ItemId);
CREATE INDEX IX_SalesInvoiceItems_UnitId ON SalesInvoiceItems (UnitId);
GO
