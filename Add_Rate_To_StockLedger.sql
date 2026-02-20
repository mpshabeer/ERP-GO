-- Add Rate column to StockLedger
ALTER TABLE StockLedger ADD Rate decimal(18, 2) NOT NULL DEFAULT 0;
GO
