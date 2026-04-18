CREATE TABLE [dbo].[GstCreditNotes] (
    [Id]                INT             IDENTITY (1, 1) NOT NULL,
    [CreditNoteNo]      NVARCHAR (MAX)  NOT NULL,
    [Date]              DATETIME2 (7)   NOT NULL,
    [OriginalInvoiceId] INT             NOT NULL,
    [CustomerId]        INT             NOT NULL,
    [Reason]            NVARCHAR (MAX)  NOT NULL,
    [Notes]             NVARCHAR (MAX)  NOT NULL,
    [SubTotal]          DECIMAL (18, 2) DEFAULT ((0)) NOT NULL,
    [TotalGstAmount]    DECIMAL (18, 2) DEFAULT ((0)) NOT NULL,
    [TotalAmount]       DECIMAL (18, 2) DEFAULT ((0)) NOT NULL,
    [CreatedAt]         DATETIME2 (7)   DEFAULT (getdate()) NOT NULL,
    [CreatedBy]         NVARCHAR (MAX)  NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CreditNotes_Customer] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers] ([Id]),
    CONSTRAINT [FK_CreditNotes_Invoice] FOREIGN KEY ([OriginalInvoiceId]) REFERENCES [dbo].[GstSalesInvoices] ([Id])
);

