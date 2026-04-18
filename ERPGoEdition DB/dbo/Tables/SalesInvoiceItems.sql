CREATE TABLE [dbo].[SalesInvoiceItems] (
    [Id]              INT             IDENTITY (1, 1) NOT NULL,
    [SalesInvoiceId]  INT             NOT NULL,
    [ItemId]          INT             NOT NULL,
    [UnitId]          INT             NOT NULL,
    [Qty]             DECIMAL (18, 3) NOT NULL,
    [Rate]            DECIMAL (18, 2) NOT NULL,
    [Amount]          DECIMAL (18, 2) NOT NULL,
    [QtyInBaseUnit]   DECIMAL (18, 3) NOT NULL,
    [DiscountPercent] DECIMAL (5, 2)  DEFAULT ((0)) NOT NULL,
    [DiscountAmount]  DECIMAL (18, 2) DEFAULT ((0)) NOT NULL,
    [ItemUnitId]      INT             NULL,
    [QtyPerBaseUnit]  DECIMAL (18, 3) DEFAULT ((1.0)) NOT NULL,
    CONSTRAINT [PK_SalesInvoiceItems] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_SalesInvoiceItems_Items_ItemId] FOREIGN KEY ([ItemId]) REFERENCES [dbo].[Items] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_SalesInvoiceItems_ItemUnits_ItemUnitId] FOREIGN KEY ([ItemUnitId]) REFERENCES [dbo].[ItemUnits] ([Id]),
    CONSTRAINT [FK_SalesInvoiceItems_SalesInvoices_SalesInvoiceId] FOREIGN KEY ([SalesInvoiceId]) REFERENCES [dbo].[SalesInvoices] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_SalesInvoiceItems_Units_UnitId] FOREIGN KEY ([UnitId]) REFERENCES [dbo].[Units] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_SalesInvoiceItems_ItemId]
    ON [dbo].[SalesInvoiceItems]([ItemId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_SalesInvoiceItems_SalesInvoiceId]
    ON [dbo].[SalesInvoiceItems]([SalesInvoiceId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_SalesInvoiceItems_UnitId]
    ON [dbo].[SalesInvoiceItems]([UnitId] ASC);

