CREATE TABLE [dbo].[ItemUnits] (
    [Id]             INT             IDENTITY (1, 1) NOT NULL,
    [ItemId]         INT             NOT NULL,
    [UnitId]         INT             NOT NULL,
    [QtyPerBaseUnit] DECIMAL (18, 2) NOT NULL,
    [Rate]           DECIMAL (18, 2) NOT NULL,
    [PurchaseRate]   DECIMAL (18, 2) NOT NULL,
    [WholesaleRate]  DECIMAL (18, 2) NOT NULL,
    CONSTRAINT [PK_ItemUnits] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ItemUnits_Items_ItemId] FOREIGN KEY ([ItemId]) REFERENCES [dbo].[Items] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ItemUnits_Units_UnitId] FOREIGN KEY ([UnitId]) REFERENCES [dbo].[Units] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_ItemUnits_ItemId]
    ON [dbo].[ItemUnits]([ItemId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ItemUnits_UnitId]
    ON [dbo].[ItemUnits]([UnitId] ASC);

