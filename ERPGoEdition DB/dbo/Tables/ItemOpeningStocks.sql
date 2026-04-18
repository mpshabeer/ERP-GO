CREATE TABLE [dbo].[ItemOpeningStocks] (
    [Id]         INT             IDENTITY (1, 1) NOT NULL,
    [ItemId]     INT             NOT NULL,
    [Qty]        DECIMAL (18, 3) NOT NULL,
    [Rate]       DECIMAL (18, 2) NOT NULL,
    [ItemUnitId] INT             NULL,
    [Date]       DATETIME2 (7)   NOT NULL,
    [TotalValue] DECIMAL (18, 2) NOT NULL,
    [Notes]      NVARCHAR (MAX)  DEFAULT (N'') NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ItemOpeningStocks_Items_ItemId] FOREIGN KEY ([ItemId]) REFERENCES [dbo].[Items] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ItemOpeningStocks_ItemUnits_ItemUnitId] FOREIGN KEY ([ItemUnitId]) REFERENCES [dbo].[ItemUnits] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_ItemOpeningStocks_ItemId]
    ON [dbo].[ItemOpeningStocks]([ItemId] ASC);

