using System.Security.Claims;
using Carter;
using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QAProphet.Data;
using QAProphet.Domain;
using QAProphet.Extensions;
using QAProphet.Features.Shared.Mappings;
using QAProphet.Features.Shared.Responses;

namespace QAProphet.Features.Comments.QuestionComments.CreateQuestionComment;

public sealed record CreateQuestionCommentRequest(string Content);

public class CreateQuestionComment : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/questions/comments/{id:guid}", Handle)
            .WithTags(nameof(Question))
            .RequireAuthorization()
            .Produces(StatusCodes.Status404NotFound)
            .Produces<CommentResponse>()
            .ProducesValidationProblem();
    }

    private static async Task<IResult> Handle(
        Guid id,
        CreateQuestionCommentRequest request,
        IValidator<CreateQuestionCommentRequest> validator,
        IMediator mediator,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken token = default)
    {
        var validationResult = await validator.ValidateAsync(request, token);

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var username = claimsPrincipal.GetUserName();
        var userId = claimsPrincipal.GetUserId();

        var command = new CreateQuestionCommentCommand(Guid.Parse(userId!), id, username!, request.Content);
        
        var result = await mediator.Send(command, token);

        if (result.IsError)
        {
            return result.Errors.ToProblem();
        }
        
        return Results.Ok(result.Value);
    }
}

internal sealed record CreateQuestionCommentCommand(
    Guid UserId,
    Guid QuestionId,
    string UserName,
    string Content)
    : IRequest<ErrorOr<CommentResponse>>;

internal sealed class
    CreateQuestionCommentHandler : IRequestHandler<CreateQuestionCommentCommand, ErrorOr<CommentResponse>>
{
    private readonly AppDbContext _dbContext;

    public CreateQuestionCommentHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<CommentResponse>> Handle(CreateQuestionCommentCommand request,
        CancellationToken cancellationToken)
    {
        var question = await _dbContext.Questions
            .AnyAsync(q => q.Id == request.QuestionId, cancellationToken);

        if (!question)
        {
            return Error.NotFound("QuestionNotFound", "Question not found");
        }

        var comment = new QuestionComment
        {
            AuthorId = request.UserId,
            AuthorName = request.UserName,
            Content = request.Content,
            CreatedAt = DateTime.UtcNow,
            QuestionId = request.QuestionId,
            IsDeleted = false
        };

        await _dbContext.QuestionComments.AddAsync(comment, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return comment.MapToResponse();
    }
}