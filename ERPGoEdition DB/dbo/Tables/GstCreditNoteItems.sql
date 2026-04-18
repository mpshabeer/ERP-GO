CREATE TABLE [dbo].[GstCreditNoteItems] (
    [Id]                    INT             IDENTITY (1, 1) NOT NULL,
    [GstCreditNoteId]       INT             NOT NULL,
    [ItemId]                INT             NOT NULL,
    [UnitId]                INT             NOT NULL,
    [HSNCode]               VARCHAR (8)     NULL,
    [OriginalInvoiceItemId] INT             NULL,
    [Qty]                   DECIMAL (18, 3) DEFAULT ((0)) NOT NULL,
    [Rate]                  DECIMAL (18, 2) DEFAULT ((0)) NOT NULL,
    [DiscountPercent]       DECIMAL (5, 2)  DEFAULT ((0)) NOT NULL,
    [Amount]                DECIMAL (18, 2) DEFAULT ((0)) NOT NULL,
    [GstPercent]            DECIMAL (5, 2)  DEFAULT ((0)) NOT NULL,
    [GstAmount]             DECIMAL (18, 2) DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CNItems_CN] FOREIGN KEY ([GstCreditNoteId]) REFERENCES [dbo].[GstCreditNotes] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_CNItems_Item] FOREIGN KEY ([ItemId]) REFERENCES [dbo].[Items] ([Id]),
    CONSTRAINT [FK_CNItems_Unit] FOREIGN KEY ([UnitId]) REFERENCES [dbo].[Units] ([Id])
);

