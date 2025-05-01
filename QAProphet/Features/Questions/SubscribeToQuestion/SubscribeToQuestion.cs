using System.Security.Claims;
using Carter;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QAProphet.Data;
using QAProphet.Domain;
using QAProphet.Extensions;

namespace QAProphet.Features.Questions.SubscribeToQuestion;

public class SubscribeToQuestion : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/questions/subscribe/{questionId:guid}", Handle)
            .WithTags(nameof(Question))
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);
    }

    private static async Task<IResult> Handle(
        Guid questionId,
        IMediator mediator,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken = default)
    {
        var userId = claimsPrincipal.GetUserId();

        var command = new SubscribeToQuestionCommand(questionId, Guid.Parse(userId!));
        
        var result = await mediator.Send(command, cancellationToken);

        if (result is not null)
        {
            return result.Value.ToProblem();
        }

        return Results.NoContent();
    }
}

internal sealed record SubscribeToQuestionCommand(
    Guid QuestionId,
    Guid UserId)
    : IRequest<Error?>;

internal sealed class SubscribeToQuestionHandler : IRequestHandler<SubscribeToQuestionCommand, Error?>
{
    private readonly AppDbContext _dbContext;

    public SubscribeToQuestionHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Error?> Handle(SubscribeToQuestionCommand request, CancellationToken cancellationToken)
    {
        var question = await _dbContext.Questions
            .SingleOrDefaultAsync(q => q.Id == request.QuestionId, cancellationToken);

        if (question is null) return Error.NotFound("Question not found", "Question not found");

        var subscribe = await _dbContext.QuestionSubscribes
            .SingleOrDefaultAsync(q => q.QuestionId == request.QuestionId && q.UserId == request.UserId,
                cancellationToken);

        if (subscribe is not null)
        {
            _dbContext.Remove(subscribe);
        }
        else
        {
            subscribe = new QuestionSubscribe
            {
                QuestionId = request.QuestionId,
                UserId = request.UserId
            };
            await _dbContext.QuestionSubscribes.AddAsync(subscribe, cancellationToken);
        }
        
        await _dbContext.SaveChangesAsync(cancellationToken);
        return null;
    }
}