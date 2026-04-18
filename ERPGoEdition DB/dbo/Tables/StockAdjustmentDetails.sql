CREATE TABLE [dbo].[StockAdjustmentDetails] (
    [Id]          INT             IDENTITY (1, 1) NOT NULL,
    [HeaderId]    INT             NOT NULL,
    [ItemId]      INT             NOT NULL,
    [ItemUnitId]  INT             NULL,
    [SystemQty]   DECIMAL (18, 3) NOT NULL,
    [NewQty]      DECIMAL (18, 3) NOT NULL,
    [AdjustedQty] DECIMAL (18, 3) NOT NULL,
    [Notes]       NVARCHAR (500)  DEFAULT ('') NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_StockAdjustmentDetails_Headers] FOREIGN KEY ([HeaderId]) REFERENCES [dbo].[StockAdjustmentHeaders] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_StockAdjustmentDetails_Items] FOREIGN KEY ([ItemId]) REFERENCES [dbo].[Items] ([Id]),
    CONSTRAINT [FK_StockAdjustmentDetails_ItemUnits] FOREIGN KEY ([ItemUnitId]) REFERENCES [dbo].[ItemUnits] ([Id])
);

