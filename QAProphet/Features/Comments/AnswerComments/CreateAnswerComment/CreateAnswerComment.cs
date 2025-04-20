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

namespace QAProphet.Features.Comments.AnswerComments.CreateAnswerComment;

public class CreateAnswerComment : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/answers/comments/{answerId:guid}", Handle)
            .WithTags(nameof(Answer))
            .RequireAuthorization()
            .Produces<CommentResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem();
    }

    private static async Task<IResult> Handle(
        Guid answerId,
        CreateCommentRequest request,
        IValidator<CreateCommentRequest> validator,
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
        
        var command = new CreateAnswerCommentCommand(Guid.Parse(userId!), answerId, username!, request.Content);
        
        var result = await mediator.Send(command, token);

        if (result.IsError)
        {
            return result.Errors.ToProblem();
        }

        return Results.Ok(result.Value);
    }
}

internal sealed record CreateAnswerCommentCommand(
    Guid AuthorId,
    Guid AnswerId,
    string AuthorName,
    string Content)
    : IRequest<ErrorOr<CommentResponse>>;

internal sealed class CreateAnswerCommentHandler : IRequestHandler<CreateAnswerCommentCommand, ErrorOr<CommentResponse>>
{
    private readonly AppDbContext _dbContext;

    public CreateAnswerCommentHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<CommentResponse>> Handle(CreateAnswerCommentCommand request,
        CancellationToken cancellationToken)
    {
        var answer = await _dbContext.Answers.AnyAsync(a => a.Id == request.AnswerId, cancellationToken);

        if (!answer)
        {
            return Error.NotFound("AnswerNotFound", "Answer not found");
        }

        var comment = new AnswerComment
        {
            AuthorId = request.AuthorId,
            AuthorName = request.AuthorName,
            Content = request.Content,
            CreatedAt = DateTime.UtcNow,
            AnswerId = request.AnswerId,
            IsDeleted = false
        };
        
        await _dbContext.AnswerComments.AddAsync(comment, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return comment.MapToResponse();
    }
}