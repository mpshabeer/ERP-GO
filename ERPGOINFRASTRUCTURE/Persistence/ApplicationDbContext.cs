
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
    }
}
