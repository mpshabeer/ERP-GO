-- Create ItemOpeningStocks Table
CREATE TABLE ItemOpeningStocks (
    Id int IDENTITY(1,1) NOT NULL PRIMARY KEY,
    ItemId int NOT NULL,
    Qty decimal(18, 3) NOT NULL,
    Rate decimal(18, 2) NOT NULL,
    ItemUnitId int NULL,
    Date datetime2 NOT NULL,
    TotalValue decimal(18, 2) NOT NULL,
    Notes nvarchar(max) NOT NULL DEFAULT N'',
    CONSTRAINT FK_ItemOpeningStocks_Items_ItemId FOREIGN KEY (ItemId) REFERENCES Items (Id) ON DELETE CASCADE,
    CONSTRAINT FK_ItemOpeningStocks_ItemUnits_ItemUnitId FOREIGN KEY (ItemUnitId) REFERENCES ItemUnits (Id) ON DELETE NO ACTION
);
GO

CREATE INDEX IX_ItemOpeningStocks_ItemId ON ItemOpeningStocks (ItemId);
GO

-- Create OpeningStockHistories Table
CREATE TABLE OpeningStockHistories (
    Id int IDENTITY(1,1) NOT NULL PRIMARY KEY,
    ItemId int NOT NULL,
    Qty decimal(18, 3) NOT NULL,
    Rate decimal(18, 2) NOT NULL,
    ItemUnitId int NULL,
    Date datetime2 NOT NULL,
    Action nvarchar(max) NOT NULL DEFAULT N'',
    Notes nvarchar(max) NOT NULL DEFAULT N'',
    UserId nvarchar(max) NOT NULL DEFAULT N''
);
GO

CREATE INDEX IX_OpeningStockHistories_ItemId ON OpeningStockHistories (ItemId);
GO
