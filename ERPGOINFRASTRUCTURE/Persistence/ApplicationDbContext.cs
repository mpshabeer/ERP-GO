
using ERPGODomain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ERPGOINFRASTRUCTURE.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Unit> Units { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<ItemUnit> ItemUnits { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<StockLedger> StockLedger { get; set; }
    public DbSet<SalesInvoice> SalesInvoices { get; set; }
    public DbSet<SalesInvoiceItem> SalesInvoiceItems { get; set; }
    public DbSet<Purchase> Purchases { get; set; }
    public DbSet<PurchaseItem> PurchaseItems { get; set; }
    public DbSet<Category> Categories { get; set; }
    
    public DbSet<ItemOpeningStock> ItemOpeningStocks { get; set; }
    public DbSet<OpeningStockHistory> OpeningStockHistories { get; set; }
    
    public DbSet<StockAdjustmentHeader> StockAdjustmentHeaders { get; set; }
    public DbSet<StockAdjustmentDetail> StockAdjustmentDetails { get; set; }

    public DbSet<AppSetting> AppSettings { get; set; }

    // Accounting
    public DbSet<AccountGroup> AccountGroups { get; set; }
    public DbSet<AccountHead> AccountHeads { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<JournalEntry> JournalEntries { get; set; }
    public DbSet<JournalEntryLine> JournalEntryLines { get; set; }
    
    // GST Sales Invoice
    public DbSet<GstSalesInvoice> GstSalesInvoices { get; set; }
    public DbSet<GstSalesInvoiceItem> GstSalesInvoiceItems { get; set; }

    // GST Credit Notes
    public DbSet<GstCreditNote> GstCreditNotes { get; set; }
    public DbSet<GstCreditNoteItem> GstCreditNoteItems { get; set; }

    // Vouchers
    public DbSet<Receipt> Receipts { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Expense> Expenses { get; set; }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure Item -> Unit relationship (Base Unit)
        modelBuilder.Entity<Item>()
            .HasOne(i => i.BaseUnit)
            .WithMany()
            .HasForeignKey(i => i.UnitId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure Item -> ItemUnits relationship
        modelBuilder.Entity<Item>()
            .HasMany(i => i.ItemUnits)
            .WithOne(iu => iu.Item)
            .HasForeignKey(iu => iu.ItemId)
            .OnDelete(DeleteBehavior.Cascade);
        // Configure Item -> Category relationship
        modelBuilder.Entity<Item>()
            .HasOne(i => i.Category)
            .WithMany()
            .HasForeignKey(i => i.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure Accounting Relationships
        modelBuilder.Entity<JournalEntry>()
            .HasMany(j => j.JournalEntryLines)
            .WithOne(l => l.JournalEntry)
            .HasForeignKey(l => l.JournalEntryId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Customer>()
            .HasOne(c => c.Account)
            .WithMany()
            .HasForeignKey(c => c.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Supplier>()
            .HasOne(s => s.Account)
            .WithMany()
            .HasForeignKey(s => s.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        // Define default precision and scale for decimal properties
        foreach (var property in modelBuilder.Model.GetEntityTypes().SelectMany(t => t.GetProperties()).Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
        {
            property.SetColumnType("decimal(18, 2)");
        }

        // --- Seed Data ---

        // 1. Seed Default Admin User
        // Note: In real production, do not keep fixed plain passwords. 
        // We will seed "admin" with "admin" password hash using BCrypt for easy testing
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Username = "admin",
                Email = "admin@erpgo.com",
                FullName = "System Administrator",
                PasswordHash = "$2a$11$nNfRid0iG4ZITKxcMOQsaODRQEekR4GIbQBsxciEChA8cHwliJVJy", // Valid BCrypt hash for "admin"
                Role = "Admin",
                IsActive = true,
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        // Configure Voucher Relationships
        modelBuilder.Entity<Receipt>()
            .HasOne(r => r.CashBankAccount)
            .WithMany()
            .HasForeignKey(r => r.CashBankAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Receipt>()
            .HasOne(r => r.PartyAccount)
            .WithMany()
            .HasForeignKey(r => r.PartyAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Payment>()
            .HasOne(p => p.CashBankAccount)
            .WithMany()
            .HasForeignKey(p => p.CashBankAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Payment>()
            .HasOne(p => p.PartyAccount)
            .WithMany()
            .HasForeignKey(p => p.PartyAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Expense>()
            .HasOne(e => e.CashBankAccount)
            .WithMany()
            .HasForeignKey(e => e.CashBankAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Expense>()
            .HasOne(e => e.ExpenseAccount)
            .WithMany()
            .HasForeignKey(e => e.ExpenseAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure GST Sales Invoice Relationships
        modelBuilder.Entity<GstSalesInvoice>()
            .HasMany(g => g.GstSalesInvoiceItems)
            .WithOne(gi => gi.GstSalesInvoice)
            .HasForeignKey(gi => gi.GstSalesInvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<GstSalesInvoiceItem>()
            .HasOne(gi => gi.Item)
            .WithMany()
            .HasForeignKey(gi => gi.ItemId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<GstSalesInvoiceItem>()
            .HasOne(gi => gi.Unit)
            .WithMany()
            .HasForeignKey(gi => gi.UnitId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure GstCreditNote relationships
        modelBuilder.Entity<GstCreditNote>()
            .HasOne(cn => cn.OriginalInvoice)
            .WithMany(i => i.CreditNotes)
            .HasForeignKey(cn => cn.OriginalInvoiceId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<GstCreditNote>()
            .HasOne(cn => cn.Customer)
            .WithMany()
            .HasForeignKey(cn => cn.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<GstCreditNote>()
            .HasMany(cn => cn.Items)
            .WithOne(i => i.GstCreditNote)
            .HasForeignKey(i => i.GstCreditNoteId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<GstCreditNoteItem>()
            .HasOne(i => i.Item)
            .WithMany()
            .HasForeignKey(i => i.ItemId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<GstCreditNoteItem>()
            .HasOne(i => i.Unit)
            .WithMany()
            .HasForeignKey(i => i.UnitId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
