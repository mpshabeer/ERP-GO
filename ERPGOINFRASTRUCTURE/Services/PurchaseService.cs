using ERPGOAPPLICATION.Interfaces;
using ERPGODomain.DTOs;
using ERPGODomain.Entities;
using ERPGOINFRASTRUCTURE.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ERPGOINFRASTRUCTURE.Services;

public class PurchaseService : IPurchaseService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public PurchaseService(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<string> GetNextPurchaseNumber()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var prefix = (await context.AppSettings.FindAsync("PurchasePrefix"))?.Value ?? "PUR";
        var nextNumStr = (await context.AppSettings.FindAsync("PurchaseNextNumber"))?.Value ?? "1";
        var paddingStr = (await context.AppSettings.FindAsync("PurchasePadding"))?.Value ?? "3";

        int nextNum = int.TryParse(nextNumStr, out var n) ? n : 1;
        int padding = int.TryParse(paddingStr, out var p) ? p : 3;

        return $"{prefix}{nextNum.ToString("D" + padding)}";
    }

    public async Task<Purchase> CreatePurchase(Purchase purchase)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var strategy = context.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                // 1. Generate Purchase Number (Transactional)
                var prefixSetting = await context.AppSettings.FindAsync("PurchasePrefix");
                var nextNumSetting = await context.AppSettings.FindAsync("PurchaseNextNumber");
                var paddingSetting = await context.AppSettings.FindAsync("PurchasePadding");

                string prefix = prefixSetting?.Value ?? "PUR";
                int nextNum = int.Parse(nextNumSetting?.Value ?? "1");
                int padding = int.Parse(paddingSetting?.Value ?? "3");

                purchase.PurchaseNo = $"{prefix}{nextNum.ToString("D" + padding)}";

                // Increment Sequence
                if (nextNumSetting == null)
                {
                    context.AppSettings.Add(new AppSetting { Key = "PurchaseNextNumber", Value = (nextNum + 1).ToString() });
                }
                else
                {
                    nextNumSetting.Value = (nextNum + 1).ToString();
                    context.AppSettings.Update(nextNumSetting);
                }

                // 2. Clear Nav Properties to prevent re-insertion
                purchase.Supplier = null;
                foreach (var line in purchase.PurchaseItems)
                {
                    line.Item = null;
                    line.Unit = null;
                    line.ItemUnit = null;
                }

                // 3. Add Header
                context.Purchases.Add(purchase);
                await context.SaveChangesAsync();

                // 4. Stock Impact
                foreach (var line in purchase.PurchaseItems)
                {
                    // Add Stock Ledger
                    var stockEntry = new StockLedger
                    {
                        Date = purchase.Date,
                        ItemId = line.ItemId,
                        Qty = line.QtyInBaseUnit > 0 ? line.QtyInBaseUnit : line.Qty, // Positive for Purchase
                        TransactionType = "PURCHASE",
                        RefId = purchase.PurchaseNo ?? purchase.InvoiceNo, 
                        Notes = $"Supplier Inv: {purchase.InvoiceNo}"
                    };
                    context.StockLedger.Add(stockEntry);

                    // Update Item Stock
                    var item = await context.Items.FindAsync(line.ItemId);
                    if (item != null)
                    {
                        item.CurrentStock += stockEntry.Qty;
                        // Optional: Update Last Purchase Price?
                        // item.PurchaseRate = line.Rate;
                        context.Items.Update(item);
                    }
                }

                // 5. Post Accounting Journal Entry
                var supplier = await context.Suppliers.FindAsync(purchase.SupplierId);
                var purchaseAccount = await context.Accounts.FirstOrDefaultAsync(a => a.Name == "Purchase Account" && a.IsDefault);
                
                if (supplier?.AccountId != null && purchaseAccount != null)
                {
                    var je = new JournalEntry
                    {
                        VoucherDate = purchase.Date,
                        VoucherNo = purchase.PurchaseNo ?? purchase.InvoiceNo,
                        VoucherType = "Purchase",
                        ReferenceId = purchase.Id,
                        Narration = $"Supplier Inv: {purchase.InvoiceNo}",
                        JournalEntryLines = new List<JournalEntryLine>
                        {
                            new JournalEntryLine { AccountId = purchaseAccount.Id, Debit = purchase.TotalAmount, Credit = 0 },
                            new JournalEntryLine { AccountId = supplier.AccountId.Value, Debit = 0, Credit = purchase.TotalAmount }
                        }
                    };
                    context.JournalEntries.Add(je);
                }

                await context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });

        return purchase;
    }

    public async Task<PagedResult<Purchase>> GetPurchasesAsync(PurchaseSearchRequest request)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var query = context.Purchases
            .Include(p => p.Supplier)
            .Include(p => p.PurchaseItems)
                .ThenInclude(pi => pi.Item)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.PurchaseNo))
            query = query.Where(p => p.PurchaseNo.Contains(request.PurchaseNo));
        
        if (!string.IsNullOrWhiteSpace(request.InvoiceNo))
            query = query.Where(p => p.InvoiceNo.Contains(request.InvoiceNo));

        if (!string.IsNullOrWhiteSpace(request.SupplierName))
            query = query.Where(p => p.Supplier != null && p.Supplier.Name.Contains(request.SupplierName));

        if (request.DateFrom.HasValue)
            query = query.Where(p => p.Date >= request.DateFrom.Value.Date);

        if (request.DateTo.HasValue)
            query = query.Where(p => p.Date <= request.DateTo.Value.Date.AddDays(1).AddTicks(-1));

        var totalCount = await query.CountAsync();
        
        var items = await query
            .OrderByDescending(p => p.Date)
            .ThenByDescending(p => p.Id)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return new PagedResult<Purchase>
        {
            Items = items,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    public async Task<Purchase?> GetPurchaseByIdAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Purchases
            .Include(p => p.Supplier)
            .Include(p => p.PurchaseItems)
                .ThenInclude(pi => pi.Unit)
            .Include(p => p.PurchaseItems)
                .ThenInclude(pi => pi.Item)
                    .ThenInclude(i => i.BaseUnit)
            .Include(p => p.PurchaseItems)
                .ThenInclude(pi => pi.Item)
                    .ThenInclude(i => i.ItemUnits)
                        .ThenInclude(iu => iu.Unit)
            .Include(p => p.PurchaseItems)
                .ThenInclude(pi => pi.ItemUnit)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Purchase> UpdatePurchase(Purchase purchase)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        
        // 1. Fetch Existing
        var existingPurchase = await context.Purchases
            .Include(p => p.PurchaseItems)
            .FirstOrDefaultAsync(p => p.Id == purchase.Id);

        if (existingPurchase == null) throw new Exception("Purchase not found");

        // 2. Reverse Old Stock (Deduct what was added)
        foreach (var oldLine in existingPurchase.PurchaseItems)
        {
            var item = await context.Items.FindAsync(oldLine.ItemId);
            if (item != null)
            {
                var qtyReversed = oldLine.QtyInBaseUnit > 0 ? oldLine.QtyInBaseUnit : oldLine.Qty;
                item.CurrentStock -= qtyReversed; // Deduct logic for Purchase Reversal
                context.Items.Update(item);
            }
        }

        // 3. Remove Old Lines
        context.PurchaseItems.RemoveRange(existingPurchase.PurchaseItems);

        // 4. Update Header
        existingPurchase.Date = purchase.Date;
        existingPurchase.DueDate = purchase.DueDate;
        existingPurchase.SupplierId = purchase.SupplierId;
        existingPurchase.InvoiceNo = purchase.InvoiceNo;
        existingPurchase.PaymentTerms = purchase.PaymentTerms;
        existingPurchase.Notes = purchase.Notes;
        
        existingPurchase.SubTotal = purchase.SubTotal;
        existingPurchase.DiscountPercent = purchase.DiscountPercent;
        existingPurchase.DiscountAmount = purchase.DiscountAmount;
        existingPurchase.TaxPercent = purchase.TaxPercent;
        existingPurchase.TaxAmount = purchase.TaxAmount;
        existingPurchase.TotalAmount = purchase.TotalAmount;

        // 5. Add New Lines & Apple New Stock
        foreach (var newLine in purchase.PurchaseItems)
        {
            var lineEntity = new PurchaseItem
            {
                PurchaseId = existingPurchase.Id,
                ItemId = newLine.ItemId,
                UnitId = newLine.UnitId,
                ItemUnitId = newLine.ItemUnitId,
                Qty = newLine.Qty,
                QtyInBaseUnit = newLine.QtyInBaseUnit,
                Rate = newLine.Rate,
                DiscountPercent = newLine.DiscountPercent,
                DiscountAmount = newLine.DiscountAmount,
                Amount = newLine.Amount,
                QtyPerBaseUnit = newLine.QtyPerBaseUnit
            };

            context.PurchaseItems.Add(lineEntity);

            // Add Stock
            var item = await context.Items.FindAsync(newLine.ItemId);
            if (item != null)
            {
                var qtyAdded = newLine.QtyInBaseUnit > 0 ? newLine.QtyInBaseUnit : newLine.Qty;
                item.CurrentStock += qtyAdded;
                context.Items.Update(item);
            }
        }

        // 6. Update Accounting Journal Entry
        var oldJe = await context.JournalEntries.Include(j => j.JournalEntryLines)
            .FirstOrDefaultAsync(j => j.ReferenceId == existingPurchase.Id && j.VoucherType == "Purchase");
        if (oldJe != null)
        {
            context.JournalEntries.Remove(oldJe);
        }

        var supplier = await context.Suppliers.FindAsync(purchase.SupplierId);
        var purchaseAccount = await context.Accounts.FirstOrDefaultAsync(a => a.Name == "Purchase Account" && a.IsDefault);
        
        if (supplier?.AccountId != null && purchaseAccount != null)
        {
            var je = new JournalEntry
            {
                VoucherDate = purchase.Date,
                VoucherNo = existingPurchase.PurchaseNo ?? existingPurchase.InvoiceNo,
                VoucherType = "Purchase",
                ReferenceId = existingPurchase.Id,
                Narration = $"Supplier Inv: {existingPurchase.InvoiceNo} (Updated)",
                JournalEntryLines = new List<JournalEntryLine>
                {
                    new JournalEntryLine { AccountId = purchaseAccount.Id, Debit = purchase.TotalAmount, Credit = 0 },
                    new JournalEntryLine { AccountId = supplier.AccountId.Value, Debit = 0, Credit = purchase.TotalAmount }
                }
            };
            context.JournalEntries.Add(je);
        }

        await context.SaveChangesAsync();
        return existingPurchase;
    }

    public async Task DeletePurchaseAsync(int id)
    {
         using var context = await _contextFactory.CreateDbContextAsync();
         var purchase = await context.Purchases.Include(p => p.PurchaseItems).FirstOrDefaultAsync(p => p.Id == id);
         if (purchase != null)
         {
             // Reverse Stock
             foreach (var line in purchase.PurchaseItems)
             {
                 var item = await context.Items.FindAsync(line.ItemId);
                 if (item != null)
                 {
                     var qty = line.QtyInBaseUnit > 0 ? line.QtyInBaseUnit : line.Qty;
                     item.CurrentStock -= qty;
                     context.Items.Update(item);
                 }
             }

             // Delete related Journal Entry
             var oldJe = await context.JournalEntries.Include(j => j.JournalEntryLines)
                 .FirstOrDefaultAsync(j => j.ReferenceId == purchase.Id && j.VoucherType == "Purchase");
             if (oldJe != null)
             {
                 context.JournalEntries.Remove(oldJe);
             }

             context.Purchases.Remove(purchase);
             await context.SaveChangesAsync();
         }
    }

    public async Task<List<ItemWiseReportLine>> GetPurchaseReportAsync(PurchaseReportRequest request)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var query = context.PurchaseItems
            .Include(i => i.Purchase)
                .ThenInclude(p => p.Supplier)
            .Include(i => i.Item)
                .ThenInclude(it => it.Category)
            .Include(i => i.Unit)
            .AsQueryable();

        if (request.SupplierId.HasValue)
            query = query.Where(i => i.Purchase != null && i.Purchase.SupplierId == request.SupplierId.Value);
            
        if (request.CategoryId.HasValue)
            query = query.Where(i => i.Item != null && i.Item.CategoryId == request.CategoryId.Value);

        if (request.ItemId.HasValue)
            query = query.Where(i => i.ItemId == request.ItemId.Value);

        query = query.Where(i => i.Purchase != null && i.Purchase.Date >= request.FromDate.Date);
        query = query.Where(i => i.Purchase != null && i.Purchase.Date <= request.ToDate.Date.AddDays(1).AddTicks(-1));

        var items = await query
            .OrderByDescending(i => i.Purchase!.Date)
            .ThenByDescending(i => i.PurchaseId)
            .ToListAsync();

        return items.Select(i => new ItemWiseReportLine
        {
            Date = i.Purchase!.Date,
            DocumentNo = !string.IsNullOrWhiteSpace(i.Purchase.InvoiceNo) ? i.Purchase.InvoiceNo : (i.Purchase.PurchaseNo ?? ""),
            PartyName = i.Purchase.Supplier?.Name ?? "Unknown Supplier",
            ItemId = i.ItemId,
            ItemCode = i.Item?.ItemCode ?? "",
            ItemName = i.Item?.Name ?? "",
            CategoryName = i.Item?.Category?.Name ?? "Uncategorized",
            UnitName = i.Unit?.Name ?? "Unit",
            Qty = i.Qty,
            QtyPerBaseUnit = i.QtyPerBaseUnit,
            Rate = i.Rate,
            Amount = i.Amount
        }).ToList();
    }
}
