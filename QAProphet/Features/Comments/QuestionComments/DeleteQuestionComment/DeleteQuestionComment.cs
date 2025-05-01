using System.Security.Claims;
using Carter;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QAProphet.Data;
using QAProphet.Domain;
using QAProphet.Extensions;

namespace QAProphet.Features.Comments.QuestionComments.DeleteQuestionComment;

public class DeleteQuestionComment : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/questions/comments/{commentId:guid}", Handle)
            .WithTags(nameof(QuestionComment))
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict);
    }

    private static async Task<IResult> Handle(
        Guid commentId,
        IMediator mediator,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken = default)
    {
        var userId = claimsPrincipal.GetUserId();
        
        var deleteCommand = new DeleteQuestionCommentCommand(commentId, Guid.Parse(userId!));
        
        var result = await mediator.Send(deleteCommand, cancellationToken);

        if (result.IsError)
        {
            return result.Errors.ToProblem();
        }
        
        return Results.NoContent();
    }
}

internal sealed record DeleteQuestionCommentCommand(
    Guid CommentId,
    Guid UserId)
    : IRequest<ErrorOr<bool>>;

internal sealed class DeleteQuestionCommentHandler : IRequestHandler<DeleteQuestionCommentCommand, ErrorOr<bool>>
{
    private readonly AppDbContext _dbContext;
    private readonly TimeProvider _timeProvider;

    public DeleteQuestionCommentHandler(AppDbContext dbContext, TimeProvider timeProvider)
    {
        _dbContext = dbContext;
        _timeProvider = timeProvider;
    }

    public async Task<ErrorOr<bool>> Handle(DeleteQuestionCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = await _dbContext.QuestionComments
            .SingleOrDefaultAsync(q => q.Id == request.CommentId, cancellationToken);

        if (comment is null)
        {
            return Error.NotFound("CommentNotFound", "Comment not found");
        }

        if (comment.AuthorId != request.UserId)
        {
            return Error.Forbidden("NotAuthor", "Not Author");
        }

        if (_timeProvider.GetUtcNow().UtcDateTime - comment.CreatedAt > TimeSpan.FromHours(1))
        {
            return Error.Conflict("Time expired", "Time for delete expired");
        }

        comment.IsDeleted = true;
        _dbContext.QuestionComments.Update(comment);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}