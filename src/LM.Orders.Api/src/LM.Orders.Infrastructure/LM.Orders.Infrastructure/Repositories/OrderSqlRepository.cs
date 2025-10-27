using Microsoft.EntityFrameworkCore;
using LM.Orders.Domain;
using LM.Orders.Application.Abstractions;
using LM.Orders.Infrastructure.Sql;

namespace LM.Orders.Infrastructure.Repositories;
public sealed class OrderSqlRepository : IOrderSqlRepository
{
    private readonly AppDbContext _db;
    public OrderSqlRepository(AppDbContext db) => _db = db;
    public async Task AddAsync(Order order, CancellationToken ct)
    {
        _db.Orders.Add(order);
        await _db.SaveChangesAsync(ct);
    }
    public Task<Order?> GetByIdAsync(Guid id, CancellationToken ct) =>
        _db.Orders.AsNoTracking().FirstOrDefaultAsync(o => o.Id == id, ct);
}
