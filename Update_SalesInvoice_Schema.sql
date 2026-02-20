-- Add Discount Columns to SalesInvoiceItems
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[SalesInvoiceItems]') AND name = 'DiscountPercent')
BEGIN
    ALTER TABLE [dbo].[SalesInvoiceItems] ADD [DiscountPercent] decimal(5,2) NOT NULL DEFAULT 0;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[SalesInvoiceItems]') AND name = 'DiscountAmount')
BEGIN
    ALTER TABLE [dbo].[SalesInvoiceItems] ADD [DiscountAmount] decimal(18,2) NOT NULL DEFAULT 0;
END
GO
