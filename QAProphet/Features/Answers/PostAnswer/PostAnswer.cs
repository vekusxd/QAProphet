using System.Security.Claims;
using Carter;
using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QAProphet.Data;
using QAProphet.Domain;
using QAProphet.Extensions;

namespace QAProphet.Features.Answers.PostAnswer;

public sealed record PostAnswerRequest(
    string Content,
    string QuestionId
);

public sealed record PostAnswerResponse(
    Guid Id,
    string Content,
    DateTime Created,
    Guid AuthorId,
    string AuthorName);

public class PostAnswer : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/answers", Handle)
            .WithTags(nameof(Answer))
            .RequireAuthorization()
            .Produces<PostAnswerResponse>()
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> Handle(
        PostAnswerRequest request,
        IValidator<PostAnswerRequest> validator,
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

       var command = request.MapToCommand(Guid.Parse(userId!), username!);
       
       var result  = await mediator.Send(command, token);

       if (result.IsError)
       {
           return result.Errors.ToProblem();
       }
       
       return Results.Ok(result.Value);
    }
}

internal sealed record PostAnswerCommand(
    Guid QuestionId,
    Guid AuthorId,
    string AuthorName,
    string Content
) : IRequest<ErrorOr<PostAnswerResponse>>;

internal sealed record PostAnswerHandler : IRequestHandler<PostAnswerCommand, ErrorOr<PostAnswerResponse>>
{
    private readonly AppDbContext _dbContext;
    private readonly TimeProvider _timeProvider;

    public PostAnswerHandler(AppDbContext dbContext, TimeProvider timeProvider)
    {
        _dbContext = dbContext;
        _timeProvider = timeProvider;
    }

    public async Task<ErrorOr<PostAnswerResponse>> Handle(
        PostAnswerCommand request,
        CancellationToken cancellationToken)
    {
        var question = await _dbContext.Questions
            .FirstOrDefaultAsync(a => a.Id == request.QuestionId, cancellationToken: cancellationToken);

        if (question is null)
        {
            return Error.NotFound("QuestionNotFound", "Question not found");
        }

        var answer = new Answer
        {
            AuthorId = request.AuthorId,
            AuthorName = request.AuthorName,
            Content = request.Content,
            CreatedAt = _timeProvider.GetUtcNow().UtcDateTime,
            QuestionId = request.QuestionId,
            Likes = 0,
            IsBest = false
        };
        
        await _dbContext.Answers.AddAsync(answer, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return answer.MapToPostResponse();
    }
}