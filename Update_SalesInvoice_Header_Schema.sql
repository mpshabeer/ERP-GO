-- Add Missing Columns to SalesInvoices Header Table

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[SalesInvoices]') AND name = 'DueDate')
BEGIN
    ALTER TABLE [dbo].[SalesInvoices] ADD [DueDate] datetime2 NULL;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[SalesInvoices]') AND name = 'PaymentTerms')
BEGIN
    ALTER TABLE [dbo].[SalesInvoices] ADD [PaymentTerms] nvarchar(max) NULL DEFAULT N'';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[SalesInvoices]') AND name = 'SubTotal')
BEGIN
    ALTER TABLE [dbo].[SalesInvoices] ADD [SubTotal] decimal(18,2) NOT NULL DEFAULT 0;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[SalesInvoices]') AND name = 'DiscountPercent')
BEGIN
    ALTER TABLE [dbo].[SalesInvoices] ADD [DiscountPercent] decimal(5,2) NOT NULL DEFAULT 0;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[SalesInvoices]') AND name = 'DiscountAmount')
BEGIN
    ALTER TABLE [dbo].[SalesInvoices] ADD [DiscountAmount] decimal(18,2) NOT NULL DEFAULT 0;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[SalesInvoices]') AND name = 'TaxPercent')
BEGIN
    ALTER TABLE [dbo].[SalesInvoices] ADD [TaxPercent] decimal(5,2) NOT NULL DEFAULT 0;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[SalesInvoices]') AND name = 'TaxAmount')
BEGIN
    ALTER TABLE [dbo].[SalesInvoices] ADD [TaxAmount] decimal(18,2) NOT NULL DEFAULT 0;
END
GO
