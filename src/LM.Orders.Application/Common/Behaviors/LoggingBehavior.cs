using MediatR;
using Microsoft.Extensions.Logging;

namespace LM.Orders.Application.Common.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse>: IPipelineBehavior<TRequest, TResponse>where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        => _logger = logger;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        _logger.LogInformation("🟢 Iniciando execução de {RequestType} com dados {@Request}", typeof(TRequest).Name, request);
        var response = await next();
        _logger.LogInformation("✅ Finalizada execução de {RequestType}", typeof(TRequest).Name);
        return response;
    }
}
