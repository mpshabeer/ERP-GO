-- Add ItemUnitId to StockLedger
ALTER TABLE StockLedger ADD ItemUnitId int NULL;
GO

-- Add Foreign Key constraint (Assuming ItemUnits table exists and Id is PK)
-- Check if FK already exists or creating new one.
ALTER TABLE StockLedger ADD CONSTRAINT FK_StockLedger_ItemUnits_ItemUnitId FOREIGN KEY (ItemUnitId) REFERENCES ItemUnits (Id) ON DELETE NO ACTION;
GO

-- Create Index for performance
CREATE INDEX IX_StockLedger_ItemUnitId ON StockLedger (ItemUnitId);
GO
