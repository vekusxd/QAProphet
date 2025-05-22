using ErrorOr;
using MediatR;

namespace QAProphet.Behaviors;

public class LoggingPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingPipelineBehavior<TRequest, TResponse>> _logger;

    public LoggingPipelineBehavior(ILogger<LoggingPipelineBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;


        _logger.LogInformation("Start handling request {RequestName}, {DateTimeUtc}",
            requestName,
            DateTime.UtcNow);

        var result = await next(cancellationToken);

        _logger.LogInformation("Completed request {RequestName}, {DateTimeUtc}",
            requestName,
            DateTime.UtcNow);

        return result;
    }
}