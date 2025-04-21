using System.Security.Claims;
using Carter;
using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QAProphet.Data;
using QAProphet.Domain;
using QAProphet.Extensions;
using QAProphet.Features.Comments.Shared.Requests;
using QAProphet.Features.Shared.Mappings;
using QAProphet.Features.Shared.Responses;

namespace QAProphet.Features.Comments.QuestionComments.CreateQuestionComment;

public class CreateQuestionComment : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/questions/comments/{questionId:guid}", Handle)
            .WithTags(nameof(QuestionComment))
            .RequireAuthorization()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces<CommentResponse>()
            .ProducesValidationProblem();
    }

    private static async Task<IResult> Handle(
        Guid questionId,
        CommentRequest request,
        IValidator<CommentRequest> validator,
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

        var command = new CreateQuestionCommentCommand(Guid.Parse(userId!), questionId, username!, request.Content);
        
        var result = await mediator.Send(command, token);

        if (result.IsError)
        {
            return result.Errors.ToProblem();
        }
        
        return Results.Ok(result.Value);
    }
}

internal sealed record CreateQuestionCommentCommand(
    Guid AuthorId,
    Guid QuestionId,
    string AuthorName,
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
            AuthorId = request.AuthorId,
            AuthorName = request.AuthorName,
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