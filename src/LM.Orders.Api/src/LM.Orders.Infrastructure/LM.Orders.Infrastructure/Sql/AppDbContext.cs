using LM.Orders.Domain;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace LM.Orders.Infrastructure.Sql;

public sealed class AppDbContext : DbContext
{
 
    public DbSet<Order> Orders => Set<Order>();
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder b)
    {
        var e = b.Entity<Order>();

        e.HasKey(x => x.Id);

        e.Property(x => x.CustomerId).HasMaxLength(100).IsRequired();

        e.Property(x => x.Status).HasConversion<int>().IsRequired();

        e.Property(x => x.CreatedAt).IsRequired();

        e.Ignore(x => x.Items);

        e.Ignore(x => x.TotalAmount);
    }
}
