using System.Security.Claims;
using Carter;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using QAProphet.Data;
using QAProphet.Data.EntityFramework;
using QAProphet.Domain;
using QAProphet.Extensions;
using QAProphet.Options;

namespace QAProphet.Features.Answers.DeleteAnswer;

public class DeleteAnswer : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/answers/{id:guid}", Handle)
            .WithTags(nameof(Answer))
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status409Conflict);
    }

    private static async Task<IResult> Handle(
        Guid id,
        ClaimsPrincipal claimsPrincipal,
        IMediator mediator,
        CancellationToken cancellationToken = default)
    {
        var userId = claimsPrincipal.GetUserId();

        var deleteCommand = new DeleteAnswerCommand(id, Guid.Parse(userId!));

        var result = await mediator.Send(deleteCommand, cancellationToken);

        if (result.HasValue)
        {
            return result.Value.ToProblem();
        }

        return Results.NoContent();
    }
}

internal sealed record DeleteAnswerCommand(
    Guid AnswerId,
    Guid UserId)
    : IRequest<Error?>;

internal sealed class DeleteAnswerHandler : IRequestHandler<DeleteAnswerCommand, Error?>
{
    private readonly AppDbContext _dbContext;
    private readonly TimeProvider _timeProvider;
    private readonly IOptions<AnswerTimeoutOptions> _options;
    private readonly ILogger<DeleteAnswerHandler> _logger;

    public DeleteAnswerHandler(
        AppDbContext dbContext,
        TimeProvider timeProvider,
        IOptions<AnswerTimeoutOptions> options,
        ILogger<DeleteAnswerHandler> logger)
    {
        _dbContext = dbContext;
        _timeProvider = timeProvider;
        _options = options;
        _logger = logger;
    }

    public async Task<Error?> Handle(DeleteAnswerCommand request, CancellationToken cancellationToken)
    {
        var answer = await _dbContext.Answers
            .SingleOrDefaultAsync(a => a.Id == request.AnswerId, cancellationToken);

        if (answer is null)
        {
            return Error.NotFound("AnswerNotFound", "Answer not found");
        }

        if (answer.AuthorId != request.UserId)
        {
            _logger.LogError("Requested delete answer not from author, {@authorId}, {@requestUserId}", answer.AuthorId,
                request.UserId);
            return Error.Forbidden("NotAuthor", "Not author");
        }

        var currentTime = _timeProvider.GetUtcNow().UtcDateTime;

        if (currentTime - answer.CreatedAt > TimeSpan.FromMinutes(_options.Value.DeleteAnswerInMinutes))
        {
            _logger.LogError(
                "Time for delete expired, {@requestDateTime}, {@answerCreatedAtTime}, {@deleteAnswerTimeout}",
                currentTime, answer.CreatedAt, _options.Value.DeleteAnswerInMinutes);
            return Error.Conflict("TimeExpired", "time for delete expired");
        }

        answer.IsDeleted = true;
        _dbContext.Answers.Update(answer);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return null;
    }
}