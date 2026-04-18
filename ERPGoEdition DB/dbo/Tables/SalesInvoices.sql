CREATE TABLE [dbo].[SalesInvoices] (
    [Id]              INT             IDENTITY (1, 1) NOT NULL,
    [InvoiceNo]       NVARCHAR (MAX)  NOT NULL,
    [Date]            DATETIME2 (7)   NOT NULL,
    [CustomerId]      INT             NOT NULL,
    [TotalAmount]     DECIMAL (18, 2) NOT NULL,
    [Notes]           NVARCHAR (MAX)  NOT NULL,
    [DueDate]         DATETIME2 (7)   NULL,
    [PaymentTerms]    NVARCHAR (MAX)  DEFAULT (N'') NULL,
    [SubTotal]        DECIMAL (18, 2) DEFAULT ((0)) NOT NULL,
    [DiscountPercent] DECIMAL (5, 2)  DEFAULT ((0)) NOT NULL,
    [DiscountAmount]  DECIMAL (18, 2) DEFAULT ((0)) NOT NULL,
    [TaxPercent]      DECIMAL (5, 2)  DEFAULT ((0)) NOT NULL,
    [TaxAmount]       DECIMAL (18, 2) DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_SalesInvoices] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_SalesInvoices_Customers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_SalesInvoices_CustomerId]
    ON [dbo].[SalesInvoices]([CustomerId] ASC);

