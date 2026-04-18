CREATE TABLE [dbo].[GstSalesInvoiceItems] (
    [Id]                INT             IDENTITY (1, 1) NOT NULL,
    [GstSalesInvoiceId] INT             NOT NULL,
    [ItemId]            INT             NOT NULL,
    [UnitId]            INT             NOT NULL,
    [ItemUnitId]        INT             NULL,
    [QtyPerBaseUnit]    DECIMAL (18, 3) DEFAULT ((1)) NOT NULL,
    [Qty]               DECIMAL (18, 3) DEFAULT ((0)) NOT NULL,
    [QtyInBaseUnit]     DECIMAL (18, 3) DEFAULT ((0)) NOT NULL,
    [Rate]              DECIMAL (18, 2) DEFAULT ((0)) NOT NULL,
    [DiscountPercent]   DECIMAL (5, 2)  DEFAULT ((0)) NOT NULL,
    [DiscountAmount]    DECIMAL (18, 2) DEFAULT ((0)) NOT NULL,
    [Amount]            DECIMAL (18, 2) DEFAULT ((0)) NOT NULL,
    [GstPercent]        DECIMAL (5, 2)  DEFAULT ((0)) NOT NULL,
    [GstAmount]         DECIMAL (18, 2) DEFAULT ((0)) NOT NULL,
    [HSNCode]           VARCHAR (8)     NULL,
    CONSTRAINT [PK_GstSalesInvoiceItems] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_GstSalesInvoiceItems_Invoice] FOREIGN KEY ([GstSalesInvoiceId]) REFERENCES [dbo].[GstSalesInvoices] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_GstSalesInvoiceItems_Item] FOREIGN KEY ([ItemId]) REFERENCES [dbo].[Items] ([Id]),
    CONSTRAINT [FK_GstSalesInvoiceItems_Unit] FOREIGN KEY ([UnitId]) REFERENCES [dbo].[Units] ([Id])
);

