using System.Security.Claims;
using Carter;
using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using QAProphet.Data;
using QAProphet.Data.EntityFramework;
using QAProphet.Domain;
using QAProphet.Extensions;
using QAProphet.Features.Comments.Shared.Requests;
using QAProphet.Features.Shared.Mappings;
using QAProphet.Features.Shared.Responses;
using QAProphet.Options;

namespace QAProphet.Features.Comments.AnswerComments.UpdateAnswerComment;

public class UpdateAnswerComment : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/answers/comments/{commentId:guid}", Handle)
            .WithTags(nameof(AnswerComment))
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict);
    }

    private static async Task<IResult> Handle(
        Guid commentId,
        CommentRequest request,
        IValidator<CommentRequest> validator,
        IMediator mediator,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var userId = claimsPrincipal.GetUserId();

        var command = new UpdateAnswerCommentCommand(Guid.Parse(userId!), commentId, request.Content);

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsError)
        {
            return result.Errors.ToProblem();
        }

        return Results.NoContent();
    }
}

internal sealed record UpdateAnswerCommentCommand(
    Guid UserId,
    Guid CommentId,
    string Content)
    : IRequest<ErrorOr<CommentResponse>>;

internal sealed class UpdateAnswerCommentHandler : IRequestHandler<UpdateAnswerCommentCommand, ErrorOr<CommentResponse>>
{
    private readonly AppDbContext _dbContext;
    private readonly TimeProvider _timeProvider;
    private readonly IOptions<AnswerTimeoutOptions> _options;
    private readonly ILogger<UpdateAnswerCommentHandler> _logger;

    public UpdateAnswerCommentHandler(
        AppDbContext dbContext,
        TimeProvider timeProvider,
        IOptions<AnswerTimeoutOptions> options,
        ILogger<UpdateAnswerCommentHandler> logger)
    {
        _dbContext = dbContext;
        _timeProvider = timeProvider;
        _options = options;
        _logger = logger;
    }

    public async Task<ErrorOr<CommentResponse>> Handle(UpdateAnswerCommentCommand request,
        CancellationToken cancellationToken)
    {
        var comment =
            await _dbContext.AnswerComments.SingleOrDefaultAsync(c => c.Id == request.CommentId, cancellationToken);

        if (comment is null)
        {
            return Error.NotFound("CommentNotFound", "Comment not found");
        }

        if (comment.AuthorId != request.UserId)
        {
            _logger.LogError("Requested delete answer comment not from author, {@authorId}, {@requestUserId}",
                comment.AuthorId, request.UserId);
            return Error.Forbidden("Not author", "Not author");
        }

        var currentTime = _timeProvider.GetUtcNow().UtcDateTime;

        if (currentTime - comment.CreatedAt > TimeSpan.FromMinutes(_options.Value.EditCommentInMinutes))
        {
            _logger.LogError(
                "Time for delete expired, {@requestDateTime}, {@answerCreatedAtTime}, {@deleteAnswerTimeout}",
                currentTime, comment.CreatedAt, _options.Value.DeleteAnswerInMinutes);
            return Error.Conflict("TimeExpirer", "Time for update expired");
        }

        comment.UpdateTime = currentTime;
        comment.Content = request.Content;
        _dbContext.AnswerComments.Update(comment);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return comment.MapToResponse();
    }
}