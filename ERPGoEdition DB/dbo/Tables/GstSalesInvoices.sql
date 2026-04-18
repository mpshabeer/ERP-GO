CREATE TABLE [dbo].[GstSalesInvoices] (
    [Id]              INT             IDENTITY (1, 1) NOT NULL,
    [InvoiceNo]       NVARCHAR (50)   NOT NULL,
    [Date]            DATETIME2 (7)   NOT NULL,
    [CustomerId]      INT             NOT NULL,
    [DueDate]         DATETIME2 (7)   NULL,
    [PaymentTerms]    NVARCHAR (100)  DEFAULT ('') NOT NULL,
    [SubTotal]        DECIMAL (18, 2) DEFAULT ((0)) NOT NULL,
    [DiscountPercent] DECIMAL (5, 2)  DEFAULT ((0)) NOT NULL,
    [DiscountAmount]  DECIMAL (18, 2) DEFAULT ((0)) NOT NULL,
    [TotalGstAmount]  DECIMAL (18, 2) DEFAULT ((0)) NOT NULL,
    [TotalAmount]     DECIMAL (18, 2) DEFAULT ((0)) NOT NULL,
    [Notes]           NVARCHAR (500)  DEFAULT ('') NOT NULL,
    [SupplyType]      NVARCHAR (20)   DEFAULT ('Intra-State') NOT NULL,
    [Status]          INT             DEFAULT ((0)) NOT NULL,
    [PostedAt]        DATETIME2 (7)   NULL,
    [PostedBy]        NVARCHAR (MAX)  NULL,
    [CreatedAt]       DATETIME2 (7)   DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_GstSalesInvoices] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_GstSalesInvoices_Customers] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers] ([Id])
);

