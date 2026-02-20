ALTER TABLE SalesInvoiceItems
ADD ItemUnitId int NULL;

ALTER TABLE SalesInvoiceItems
ADD CONSTRAINT FK_SalesInvoiceItems_ItemUnits_ItemUnitId FOREIGN KEY (ItemUnitId) REFERENCES ItemUnits (Id);
