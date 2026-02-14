using Microsoft.EntityFrameworkCore;
using net_api.Models;

namespace net_api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<o> os { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<TransactionDetail> TransactionDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure o
            modelBuilder.Entity<o>(entity =>
            {
                entity.HasIndex(e => e.oname).IsUnique();
                entity.Property(e => e.Username).IsRequired();
            });

            // Configure Product
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasIndex(e => e.SKU).IsUnique();
                entity.Property(e => e.Price).HasPrecision(18, 2);
            });

            // Configure Transaction
            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasIndex(e => e.ReferenceNumber).IsUnique();
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure TransactionDetail
            modelBuilder.Entity<TransactionDetail>(entity =>
            {
                entity.HasOne(e => e.Transaction)
                    .WithMany(t => t.TransactionDetails)
                    .HasForeignKey(e => e.TransactionId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Product)
                    .WithMany(p => p.TransactionDetails)
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
                entity.Property(e => e.SubTotal).HasPrecision(18, 2);
            });
        }
    }
}
