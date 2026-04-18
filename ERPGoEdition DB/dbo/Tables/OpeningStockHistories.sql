CREATE TABLE [dbo].[OpeningStockHistories] (
    [Id]         INT             IDENTITY (1, 1) NOT NULL,
    [ItemId]     INT             NOT NULL,
    [Qty]        DECIMAL (18, 3) NOT NULL,
    [Rate]       DECIMAL (18, 2) NOT NULL,
    [ItemUnitId] INT             NULL,
    [Date]       DATETIME2 (7)   NOT NULL,
    [Action]     NVARCHAR (MAX)  DEFAULT (N'') NOT NULL,
    [Notes]      NVARCHAR (MAX)  DEFAULT (N'') NOT NULL,
    [UserId]     NVARCHAR (MAX)  DEFAULT (N'') NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_OpeningStockHistories_ItemId]
    ON [dbo].[OpeningStockHistories]([ItemId] ASC);

