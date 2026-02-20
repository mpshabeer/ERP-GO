CREATE TABLE [dbo].[StockAdjustmentHeaders] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [AdjustmentNo] NVARCHAR(50) NOT NULL,
    [Date] DATETIME2 NOT NULL,
    [AdjustedBy] NVARCHAR(100) NOT NULL,
    [Notes] NVARCHAR(500) NOT NULL DEFAULT '',
    [TotalQty] DECIMAL(18,3) NOT NULL DEFAULT 0
);

CREATE TABLE [dbo].[StockAdjustmentDetails] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [HeaderId] INT NOT NULL,
    [ItemId] INT NOT NULL,
    [ItemUnitId] INT NULL,
    [SystemQty] DECIMAL(18,3) NOT NULL,
    [NewQty] DECIMAL(18,3) NOT NULL,
    [AdjustedQty] DECIMAL(18,3) NOT NULL,
    [Notes] NVARCHAR(500) NOT NULL DEFAULT '',
    CONSTRAINT [FK_StockAdjustmentDetails_Headers] FOREIGN KEY ([HeaderId]) REFERENCES [dbo].[StockAdjustmentHeaders] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_StockAdjustmentDetails_Items] FOREIGN KEY ([ItemId]) REFERENCES [dbo].[Items] ([Id]),
    CONSTRAINT [FK_StockAdjustmentDetails_ItemUnits] FOREIGN KEY ([ItemUnitId]) REFERENCES [dbo].[ItemUnits] ([Id])
);
