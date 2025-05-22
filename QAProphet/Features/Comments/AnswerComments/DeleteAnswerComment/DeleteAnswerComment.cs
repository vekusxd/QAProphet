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

namespace QAProphet.Features.Comments.AnswerComments.DeleteAnswerComment;

public class DeleteAnswerComment : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/answers/comments/{commentId:guid}", Handle)
            .WithTags(nameof(AnswerComment))
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict);
    }

    private static async Task<IResult> Handle(
        Guid commentId,
        ClaimsPrincipal claimsPrincipal,
        IMediator mediator,
        CancellationToken cancellationToken = default)
    {
        var userId = claimsPrincipal.GetUserId();

        var deleteCommand = new DeleteAnswerCommentCommand(commentId, Guid.Parse(userId!));

        var result = await mediator.Send(deleteCommand, cancellationToken);

        if (result.IsError)
        {
            return result.Errors.ToProblem();
        }

        return Results.NoContent();
    }
}

internal sealed record DeleteAnswerCommentCommand(
    Guid CommentId,
    Guid UserId
) : IRequest<ErrorOr<bool>>;

internal sealed class DeleteAnswerCommentHandler : IRequestHandler<DeleteAnswerCommentCommand, ErrorOr<bool>>
{
    private readonly AppDbContext _dbContext;
    private readonly TimeProvider _timeProvider;
    private readonly IOptions<AnswerTimeoutOptions> _options;
    private readonly ILogger<DeleteAnswerCommentHandler> _logger;

    public DeleteAnswerCommentHandler(
        AppDbContext dbContext,
        TimeProvider timeProvider,
        IOptions<AnswerTimeoutOptions> options, 
        ILogger<DeleteAnswerCommentHandler> logger)
    {
        _dbContext = dbContext;
        _timeProvider = timeProvider;
        _options = options;
        _logger = logger;
    }

    public async Task<ErrorOr<bool>> Handle(DeleteAnswerCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = await _dbContext.AnswerComments
            .SingleOrDefaultAsync(q => q.Id == request.CommentId, cancellationToken);

        if (comment is null)
        {
            return Error.NotFound("CommentNotFound", "Comment not found");
        }

        if (comment.AuthorId != request.UserId)
        {
            _logger.LogError("Requested delete answer not from author, {@authorId}, {@requestUserId}", comment.AuthorId,
                request.UserId);
            return Error.Forbidden("NotAuthor", "Not Author");
        }


        var currentTime = _timeProvider.GetUtcNow().UtcDateTime;
        if (currentTime - comment.CreatedAt >
            TimeSpan.FromMinutes(_options.Value.DeleteCommentInMinutes))
        {
            _logger.LogError(
                "Time for delete expired, {@requestDateTime}, {@answerCreatedAtTime}, {@deleteAnswerTimeout}",
                currentTime, comment.CreatedAt, _options.Value.DeleteAnswerInMinutes);
            return Error.Conflict("Time expired", "Time for delete expired");
        }

        comment.IsDeleted = true;
        _dbContext.AnswerComments.Update(comment);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}