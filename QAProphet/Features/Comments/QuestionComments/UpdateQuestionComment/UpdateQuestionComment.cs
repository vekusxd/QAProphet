using System.Security.Claims;
using Carter;
using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using QAProphet.Data;
using QAProphet.Domain;
using QAProphet.Extensions;
using QAProphet.Features.Comments.Shared.Requests;
using QAProphet.Features.Shared.Mappings;
using QAProphet.Features.Shared.Responses;
using QAProphet.Options;

namespace QAProphet.Features.Comments.QuestionComments.UpdateQuestionComment;

public class UpdateQuestionComment : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/questions/comments/{commentId:guid}", Handle)
            .WithTags(nameof(QuestionComment))
            .RequireAuthorization()
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status204NoContent)
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
        
        var command = new UpdateQuestionCommentCommand(Guid.Parse(userId!), commentId, request.Content);
        
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsError)
        {
            return result.Errors.ToProblem();
        }
        
        return Results.NoContent();
    }
}

internal sealed record UpdateQuestionCommentCommand(
    Guid AuthorId,
    Guid CommentId,
    string Content)
    : IRequest<ErrorOr<CommentResponse>>;

internal sealed class
    UpdateQuestionCommentHandler : IRequestHandler<UpdateQuestionCommentCommand, ErrorOr<CommentResponse>>
{
    private readonly AppDbContext _dbContext;
    private readonly TimeProvider _timeProvider;
    private readonly IOptions<QuestionTimeoutOptions> _options;

    public UpdateQuestionCommentHandler(AppDbContext dbContext, TimeProvider timeProvider, IOptions<QuestionTimeoutOptions> options)
    {
        _dbContext = dbContext;
        _timeProvider = timeProvider;
        _options = options;
    }

    public async Task<ErrorOr<CommentResponse>> Handle(UpdateQuestionCommentCommand request,
        CancellationToken cancellationToken)
    {
        var comment = await _dbContext.QuestionComments
            .FirstOrDefaultAsync(c => c.Id == request.CommentId, cancellationToken);

        if (comment is null)
        {
            return Error.NotFound("CommentNotFound", "Comment not found");
        }

        if (comment.AuthorId != request.AuthorId)
        {
            return Error.Forbidden("Not author", "Not author");
        }

        var currentTime = _timeProvider.GetUtcNow().UtcDateTime;

        if (currentTime - comment.CreatedAt > TimeSpan.FromMinutes(_options.Value.EditCommentInMinutes))
        {
            return Error.Conflict("TimeExpirer", "Time for update expired");
        }

        comment.UpdateTime = currentTime;
        comment.Content = request.Content;
        _dbContext.QuestionComments.Update(comment);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return comment.MapToResponse();
    }
}