-- Multi-Unit Redesign: Remove Name, ItemUnitCode, Barcode from ItemUnits table
-- Run this script against your ERP database BEFORE deploying the updated application

-- Step 1: Drop the ItemUnitCode column
ALTER TABLE ItemUnits DROP COLUMN ItemUnitCode;

-- Step 2: Drop the Name column
ALTER TABLE ItemUnits DROP COLUMN Name;

-- Step 3: Drop the Barcode column
ALTER TABLE ItemUnits DROP COLUMN Barcode;

-- Verification
SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'ItemUnits' 
ORDER BY ORDINAL_POSITION;
