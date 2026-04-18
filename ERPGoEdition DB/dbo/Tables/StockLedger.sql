CREATE TABLE [dbo].[StockLedger] (
    [Id]              BIGINT          IDENTITY (1, 1) NOT NULL,
    [Date]            DATETIME2 (7)   NOT NULL,
    [ItemId]          INT             NOT NULL,
    [Qty]             DECIMAL (18, 3) NOT NULL,
    [TransactionType] NVARCHAR (MAX)  NOT NULL,
    [RefId]           NVARCHAR (MAX)  NOT NULL,
    [Notes]           NVARCHAR (MAX)  NOT NULL,
    [Rate]            DECIMAL (18, 2) DEFAULT ((0)) NOT NULL,
    [ItemUnitId]      INT             NULL,
    CONSTRAINT [PK_StockLedger] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_StockLedger_Items_ItemId] FOREIGN KEY ([ItemId]) REFERENCES [dbo].[Items] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_StockLedger_ItemUnits_ItemUnitId] FOREIGN KEY ([ItemUnitId]) REFERENCES [dbo].[ItemUnits] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_StockLedger_ItemId]
    ON [dbo].[StockLedger]([ItemId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_StockLedger_ItemUnitId]
    ON [dbo].[StockLedger]([ItemUnitId] ASC);

