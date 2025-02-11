using Microsoft.EntityFrameworkCore;
using Teste.Domain.Entities;

namespace Teste.Infrastructure.Contexts;

public class TesteDbContext(DbContextOptions<TesteDbContext> options) : DbContext(options)
{
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Wallet> Wallets { get; set; }

    public DbSet<Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Wallet>()
            .Property(a => a.Role)
            .HasConversion<int>();

        modelBuilder.Entity<Transaction>()
            .Property(a => a.Status)
            .HasConversion<int>();
    }
}