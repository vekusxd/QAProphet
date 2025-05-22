using System.Security.Claims;
using Carter;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QAProphet.Data;
using QAProphet.Data.EntityFramework;
using QAProphet.Domain;
using QAProphet.Extensions;
using QAProphet.Features.Answers.Shared.Mapping;
using QAProphet.Features.Answers.Shared.Responses;

namespace QAProphet.Features.Answers.MarkAnswer;

public class MarkAnswer : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/api/answers/{answerId:guid}/mark", Handle)
            .WithTags(nameof(Answer))
            .RequireAuthorization()
            .Produces<AnswerUpdateResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status409Conflict);
    }

    private static async Task<IResult> Handle(
        Guid answerId,
        ClaimsPrincipal claimsPrincipal,
        IMediator mediator,
        CancellationToken token = default)
    {
        var userId = claimsPrincipal.GetUserId();

        var command = new MarkAnswerCommand(answerId, Guid.Parse(userId!));

        var result = await mediator.Send(command, token);

        if (result.IsError)
        {
            return result.Errors.ToProblem();
        }
        
        return Results.Ok(result.Value);
    }
}

internal sealed record MarkAnswerCommand(Guid AnswerId, Guid UserId)
    : IRequest<ErrorOr<AnswerUpdateResponse>>;

internal sealed class MarkAnswerHandler : IRequestHandler<MarkAnswerCommand, ErrorOr<AnswerUpdateResponse>>
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<MarkAnswerHandler> _logger;

    public MarkAnswerHandler(AppDbContext dbContext, ILogger<MarkAnswerHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<ErrorOr<AnswerUpdateResponse>> Handle(MarkAnswerCommand request,
        CancellationToken cancellationToken)
    {
        var answer = await _dbContext.Answers
            .Include(a => a.Question)
            .SingleOrDefaultAsync(a => a.Id == request.AnswerId, cancellationToken: cancellationToken);

        if (answer is null)
        {
            return Error.NotFound("AnswerNotFound", "Answer not found");
        }
        
        if (answer.Question.QuestionerId != request.UserId)
        {
            _logger.LogError("Mark answer request not from author, {@authorId}, {@requestUserId}", answer.AuthorId,
                request.UserId);
            return Error.Forbidden("NotAuthor", "Not author");
        }

        var bestSet = await _dbContext.Answers
            .FirstOrDefaultAsync(a => a.QuestionId == answer.QuestionId && a.IsBest == true, cancellationToken: cancellationToken);
        
        if (bestSet is not null)
        {
            _logger.LogError("Best answer already set {@questionId}, {@bestAnswerId}", answer.QuestionId, bestSet.Id);
            return Error.Conflict("BestAnswerSet", "Best answer already set");
        }

        answer.IsBest = true;
        _dbContext.Answers.Update(answer);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return answer.MapToUpdateResponse();
    }
}