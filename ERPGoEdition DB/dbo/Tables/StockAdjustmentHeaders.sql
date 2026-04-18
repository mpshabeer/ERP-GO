CREATE TABLE [dbo].[StockAdjustmentHeaders] (
    [Id]           INT             IDENTITY (1, 1) NOT NULL,
    [AdjustmentNo] NVARCHAR (50)   NOT NULL,
    [Date]         DATETIME2 (7)   NOT NULL,
    [AdjustedBy]   NVARCHAR (100)  NOT NULL,
    [Notes]        NVARCHAR (500)  DEFAULT ('') NOT NULL,
    [TotalQty]     DECIMAL (18, 3) DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

