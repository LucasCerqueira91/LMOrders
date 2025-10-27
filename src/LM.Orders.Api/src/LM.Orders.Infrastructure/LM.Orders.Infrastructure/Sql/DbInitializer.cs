using Microsoft.EntityFrameworkCore;

namespace LM.Orders.Infrastructure.Sql;

public static class DbInitializer
{
    public static async Task InitializeAsync(AppDbContext context)
    {
        await context.Database.EnsureCreatedAsync();
    }
}