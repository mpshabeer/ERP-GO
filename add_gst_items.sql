ALTER TABLE [Items] ADD 
    [HSNCode] varchar(8) NULL,
    [IsGSTApplicable] bit NOT NULL DEFAULT 1,
    [TaxType] varchar(20) NOT NULL DEFAULT 'GST';
GO

-- Update existing items to default properties
UPDATE [Items] SET [IsGSTApplicable] = 1, [TaxType] = 'GST';
GO
