using ERPGODomain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ERPGOINFRASTRUCTURE.Persistence;

public static class AccountingDbSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        //await context.Database.MigrateAsync();

        // 1. Seed Groups
        var groups = new[] { "Assets", "Liabilities", "Equity", "Revenue", "Expenses" };
        foreach (var gName in groups)
        {
            if (!await context.AccountGroups.AnyAsync(g => g.Name == gName))
                context.AccountGroups.Add(new AccountGroup { Name = gName });
        }
        await context.SaveChangesAsync();

        var assetGroup = await context.AccountGroups.FirstAsync(g => g.Name == "Assets");
        var liabGroup = await context.AccountGroups.FirstAsync(g => g.Name == "Liabilities");
        var revGroup = await context.AccountGroups.FirstAsync(g => g.Name == "Revenue");
        var expGroup = await context.AccountGroups.FirstAsync(g => g.Name == "Expenses");

        async Task SeedHeadAsync(string name, int groupId)
        {
            if (!await context.AccountHeads.AnyAsync(h => h.Name == name))
                context.AccountHeads.Add(new AccountHead { Name = name, AccountGroupId = groupId });
        }

        await SeedHeadAsync("Cash & Bank", assetGroup.Id);
        await SeedHeadAsync("Sundry Debtors", assetGroup.Id);
        await SeedHeadAsync("Current Assets", assetGroup.Id);
        await SeedHeadAsync("Sundry Creditors", liabGroup.Id);
        await SeedHeadAsync("Current Liabilities", liabGroup.Id);
        await SeedHeadAsync("Direct Income", revGroup.Id);
        await SeedHeadAsync("Direct Expenses", expGroup.Id);
        await context.SaveChangesAsync();

        var cashHead = await context.AccountHeads.FirstAsync(h => h.Name == "Cash & Bank");
        var debtorsHead = await context.AccountHeads.FirstAsync(h => h.Name == "Sundry Debtors");
        var creditorsHead = await context.AccountHeads.FirstAsync(h => h.Name == "Sundry Creditors");
        var incomeHead = await context.AccountHeads.FirstAsync(h => h.Name == "Direct Income");
        var expHead = await context.AccountHeads.FirstAsync(h => h.Name == "Direct Expenses");

        async Task<Account> SeedAccountAsync(string name, int headId, bool isDefault)
        {
            var acc = await context.Accounts.FirstOrDefaultAsync(a => a.Name == name);
            if (acc == null)
            {
                acc = new Account { Name = name, AccountHeadId = headId, IsDefault = isDefault };
                context.Accounts.Add(acc);
            }
            return acc;
        }

        await SeedAccountAsync("Cash Account", cashHead.Id, true);
        await SeedAccountAsync("Sales Account", incomeHead.Id, true);
        await SeedAccountAsync("Purchase Account", expHead.Id, true);
        
        var c1000 = await SeedAccountAsync("C1000 Cash Customer", debtorsHead.Id, true);
        var s1000 = await SeedAccountAsync("S1000 Cash Supplier", creditorsHead.Id, true);
        
        await context.SaveChangesAsync();

        // Ensure default C1000 Customer gets linked to the ledger
        var c1000Entity = await context.Customers.FirstOrDefaultAsync(c => c.Name == "C1000 Cash Customer" || c.Code == "C1000");
        if (c1000Entity != null && c1000Entity.AccountId == null)
        {
            c1000Entity.AccountId = c1000.Id;
        }

        // Ensure default S1000 Supplier gets linked to the ledger
        var s1000Entity = await context.Suppliers.FirstOrDefaultAsync(s => s.Name == "S1000 Cash Supplier" || s.Code == "S1000");
        if (s1000Entity != null && s1000Entity.AccountId == null)
        {
            s1000Entity.AccountId = s1000.Id;
        }
        
        await context.SaveChangesAsync();
    }
}
