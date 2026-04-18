CREATE TABLE [dbo].[Purchases] (
    [Id]              INT             IDENTITY (1, 1) NOT NULL,
    [Date]            DATETIME2 (7)   NOT NULL,
    [SupplierId]      INT             NOT NULL,
    [InvoiceNo]       NVARCHAR (MAX)  NOT NULL,
    [Notes]           NVARCHAR (MAX)  NOT NULL,
    [TotalAmount]     DECIMAL (18, 2) NOT NULL,
    [PurchaseNo]      NVARCHAR (50)   NULL,
    [DueDate]         DATETIME2 (7)   NULL,
    [PaymentTerms]    NVARCHAR (50)   NULL,
    [SubTotal]        DECIMAL (18, 2) DEFAULT ((0)) NOT NULL,
    [DiscountPercent] DECIMAL (5, 2)  DEFAULT ((0)) NOT NULL,
    [DiscountAmount]  DECIMAL (18, 2) DEFAULT ((0)) NOT NULL,
    [TaxPercent]      DECIMAL (5, 2)  DEFAULT ((0)) NOT NULL,
    [TaxAmount]       DECIMAL (18, 2) DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Purchases] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Purchases_Suppliers_SupplierId] FOREIGN KEY ([SupplierId]) REFERENCES [dbo].[Suppliers] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_Purchases_SupplierId]
    ON [dbo].[Purchases]([SupplierId] ASC);

