using System.Security.Claims;
using Carter;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QAProphet.Data;
using QAProphet.Domain;
using QAProphet.Extensions;
using QAProphet.Features.Answers.Shared.Mapping;
using QAProphet.Features.Answers.Shared.Responses;

namespace QAProphet.Features.Answers.LikeAnswer;

public class LikeAnswer : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/api/answers/{answerId:guid}/like", Handle)
            .WithTags(nameof(Answer))
            .RequireAuthorization()
            .Produces<AnswerUpdateResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized);
    }

    private static async Task<IResult> Handle(
        Guid answerId,
        ClaimsPrincipal claimsPrincipal,
        IMediator mediator,
        CancellationToken cancellationToken = default)
    {
        var userId = claimsPrincipal.GetUserId();
        
        var command = new LikeAnswerCommand(answerId, Guid.Parse(userId!));
        
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsError)
        {
            return result.Errors.ToProblem();
        }

        return Results.Ok(result.Value);
    }
}

internal sealed record LikeAnswerCommand(Guid AnswerId, Guid UserId)
    : IRequest<ErrorOr<AnswerUpdateResponse>>;

internal sealed class LikeAnswerHandler : IRequestHandler<LikeAnswerCommand, ErrorOr<AnswerUpdateResponse>>
{
    private readonly AppDbContext _dbContext;
    private readonly TimeProvider _timeProvider;

    public LikeAnswerHandler(AppDbContext dbContext, TimeProvider timeProvider)
    {
        _dbContext = dbContext;
        _timeProvider = timeProvider;
    }

    public async Task<ErrorOr<AnswerUpdateResponse>> Handle(LikeAnswerCommand request, CancellationToken cancellationToken)
    {
        var answer = await _dbContext.Answers.SingleOrDefaultAsync(a => a.Id == request.AnswerId, cancellationToken);

        if (answer is null)
        {
            return Error.NotFound("AnswerNotFound", "Answer not found");
        }
        
        var answerLike = await _dbContext.AnswerLikes
            .SingleOrDefaultAsync(a => a.AnswerId == request.AnswerId && a.AuthorId == request.UserId, cancellationToken);

        if (answerLike is not null)
        {
            answerLike.IsDeleted = true;
            _dbContext.AnswerLikes.Update(answerLike);
            answer.Likes--;
        }
        else
        {
            answerLike = new AnswerLike
            {
                AnswerId = answer.Id,
                AuthorId = request.UserId,
                CreatedAt = _timeProvider.GetUtcNow().UtcDateTime,
                IsDeleted = false
            };
            await _dbContext.AnswerLikes.AddAsync(answerLike, cancellationToken);
            answer.Likes++;
        }

        _dbContext.Answers.Update(answer);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return answer.MapToUpdateResponse();
    }
}