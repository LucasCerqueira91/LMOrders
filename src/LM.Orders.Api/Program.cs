
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using LM.Orders.Application.Common.Behaviors;
using LM.Orders.Application.Orders.Create;
using LM.Orders.Application.Orders.Get;
using LM.Orders.Application.Abstractions;
using LM.Orders.Infrastructure.Cache;
using LM.Orders.Infrastructure.Messaging;
using LM.Orders.Infrastructure.Mongo;
using LM.Orders.Infrastructure.Repositories;
using LM.Orders.Infrastructure.Sql;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var sqlConn = builder.Configuration.GetConnectionString("Sql") ?? "Server=localhost,1433;Database=OrdersDb;User Id=sa;Password=P@ssw0rd!;TrustServerCertificate=True";
var mongoConn = builder.Configuration.GetConnectionString("Mongo") ?? "mongodb://localhost:27017";
var redisConn = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
var rabbitHost = builder.Configuration.GetValue<string>("RabbitHost") ?? "localhost";

builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlServer(sqlConn));
builder.Services.AddScoped<IOrderSqlRepository, OrderSqlRepository>();

builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoConn));
builder.Services.AddScoped<IMongoDatabase>(sp => sp.GetRequiredService<IMongoClient>().GetDatabase("ordersdb"));
builder.Services.AddScoped<IOrderItemsMongoRepository, OrderItemsMongoRepository>();

builder.Services.AddSingleton<ICacheService>(_ => new RedisCacheService(redisConn));

builder.Services.AddSingleton<IEventBus>(_ => new RabbitEventBus(rabbitHost));

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(
        typeof(CreateOrderCommand).Assembly,
        typeof(GetOrderByIdQuery).Assembly
    );
});

builder.Services.AddValidatorsFromAssembly(typeof(CreateOrderCommand).Assembly);

builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await LM.Orders.Infrastructure.Sql.DbInitializer.InitializeAsync(db);
}


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/orders", async (CreateOrderCommand cmd, ISender sender, CancellationToken ct) =>
{
    try
    {
        var id = await sender.Send(cmd, ct);
        return Results.Created($"/orders/{id}", new { id });
    }
 
    catch (FluentValidation.ValidationException vex)
    {
        var errors = vex.Errors.Select(e => new { field = e.PropertyName, error = e.ErrorMessage });
        return Results.BadRequest(new { message = "Validation failed", errors });
    }
  
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
  
    catch
    {
        return Results.Problem(detail: "An error occurred while creating the order", statusCode: 500);
    }
})
.WithName("CreateOrder")
.WithOpenApi();

app.MapGet("/orders/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
{
    try
    {
        var dto = await sender.Send(new GetOrderByIdQuery(id), ct);
        return Results.Ok(dto);
    }
 
    catch (KeyNotFoundException)
    {
        return Results.NotFound();
    }
 
    catch
    {
        return Results.Problem(detail: "An error occurred while retrieving the order", statusCode: 500);
    }
})
.WithName("GetOrderById")
.WithOpenApi();

app.Run();
