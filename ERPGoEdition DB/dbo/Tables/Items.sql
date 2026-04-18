CREATE TABLE [dbo].[Items] (
    [Id]              INT             IDENTITY (1, 1) NOT NULL,
    [Name]            NVARCHAR (MAX)  NOT NULL,
    [UnitId]          INT             NOT NULL,
    [Rate]            DECIMAL (18, 2) NOT NULL,
    [IsActive]        BIT             NOT NULL,
    [Barcode]         NVARCHAR (MAX)  DEFAULT (N'') NOT NULL,
    [CurrentStock]    DECIMAL (18, 3) DEFAULT ((0.0)) NOT NULL,
    [IsMultiUnit]     BIT             DEFAULT (CONVERT([bit],(0))) NOT NULL,
    [ItemCode]        NVARCHAR (MAX)  DEFAULT (N'') NOT NULL,
    [PurchaseRate]    DECIMAL (18, 2) DEFAULT ((0.0)) NOT NULL,
    [WholesaleRate]   DECIMAL (18, 2) DEFAULT ((0.0)) NOT NULL,
    [CategoryId]      INT             NULL,
    [GstPercent]      DECIMAL (5, 2)  DEFAULT ((0)) NOT NULL,
    [HSNCode]         VARCHAR (8)     NULL,
    [IsGSTApplicable] BIT             DEFAULT ((1)) NOT NULL,
    [TaxType]         VARCHAR (20)    DEFAULT ('GST') NOT NULL,
    CONSTRAINT [PK_Items] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Items_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Categories] ([Id]),
    CONSTRAINT [FK_Items_Units_UnitId] FOREIGN KEY ([UnitId]) REFERENCES [dbo].[Units] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Items_UnitId]
    ON [dbo].[Items]([UnitId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Items_CategoryId]
    ON [dbo].[Items]([CategoryId] ASC);

