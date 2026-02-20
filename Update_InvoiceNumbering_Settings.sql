-- Initialize Custom Invoice Numbering Settings
IF NOT EXISTS (SELECT * FROM [dbo].[AppSettings] WHERE [Key] = 'SalesInvoicePrefix')
BEGIN
    INSERT INTO [dbo].[AppSettings] ([Key], [Value]) VALUES ('SalesInvoicePrefix', 'INV');
END

IF NOT EXISTS (SELECT * FROM [dbo].[AppSettings] WHERE [Key] = 'SalesInvoiceNextNumber')
BEGIN
    INSERT INTO [dbo].[AppSettings] ([Key], [Value]) VALUES ('SalesInvoiceNextNumber', '1');
END

IF NOT EXISTS (SELECT * FROM [dbo].[AppSettings] WHERE [Key] = 'SalesInvoicePadding')
BEGIN
    INSERT INTO [dbo].[AppSettings] ([Key], [Value]) VALUES ('SalesInvoicePadding', '3');
END
GO
