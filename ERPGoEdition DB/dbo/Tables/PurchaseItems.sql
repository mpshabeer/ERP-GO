CREATE TABLE [dbo].[PurchaseItems] (
    [Id]              INT             IDENTITY (1, 1) NOT NULL,
    [PurchaseId]      INT             NOT NULL,
    [ItemId]          INT             NOT NULL,
    [UnitId]          INT             NOT NULL,
    [Qty]             DECIMAL (18, 3) NOT NULL,
    [QtyInBaseUnit]   DECIMAL (18, 3) NOT NULL,
    [Rate]            DECIMAL (18, 2) NOT NULL,
    [Amount]          DECIMAL (18, 2) NOT NULL,
    [ItemUnitId]      INT             NULL,
    [DiscountPercent] DECIMAL (5, 2)  DEFAULT ((0)) NOT NULL,
    [DiscountAmount]  DECIMAL (18, 2) DEFAULT ((0)) NOT NULL,
    [QtyPerBaseUnit]  DECIMAL (18, 3) DEFAULT ((1.0)) NOT NULL,
    CONSTRAINT [PK_PurchaseItems] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PurchaseItems_Items_ItemId] FOREIGN KEY ([ItemId]) REFERENCES [dbo].[Items] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_PurchaseItems_ItemUnits_ItemUnitId] FOREIGN KEY ([ItemUnitId]) REFERENCES [dbo].[ItemUnits] ([Id]),
    CONSTRAINT [FK_PurchaseItems_Purchases_PurchaseId] FOREIGN KEY ([PurchaseId]) REFERENCES [dbo].[Purchases] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_PurchaseItems_Units_UnitId] FOREIGN KEY ([UnitId]) REFERENCES [dbo].[Units] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_PurchaseItems_ItemId]
    ON [dbo].[PurchaseItems]([ItemId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_PurchaseItems_PurchaseId]
    ON [dbo].[PurchaseItems]([PurchaseId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_PurchaseItems_UnitId]
    ON [dbo].[PurchaseItems]([UnitId] ASC);

