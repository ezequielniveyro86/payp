using Microsoft.EntityFrameworkCore;
using WebAppTOC.Models;

namespace WebAppTOC.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuraciones adicionales para la tabla Wallet
            modelBuilder.Entity<Wallet>()
                .Property(w => w.Balance)
                .HasPrecision(18, 2);

            // Asegurar que CreatedAt se establezca automáticamente
            modelBuilder.Entity<Wallet>()
                .Property(w => w.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            // Configuraciones para la tabla Transaction
            modelBuilder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasPrecision(18, 2);

            // Asegurar que CreatedAt se establezca automáticamente
            modelBuilder.Entity<Transaction>()
                .Property(t => t.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            // Configurar la relación entre Wallet y Transaction
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Wallet)
                .WithMany(w => w.Transactions)
                .HasForeignKey(t => t.WalletId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
} 