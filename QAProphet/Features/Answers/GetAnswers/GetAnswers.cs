using Carter;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QAProphet.Data.EntityFramework;
using QAProphet.Domain;
using QAProphet.Extensions;
using QAProphet.Features.Shared.Mappings;
using QAProphet.Features.Shared.Responses;

namespace QAProphet.Features.Answers.GetAnswers;

public record GetAnswersResponse(int Total, ICollection<AnswerResponse> Answers);

public class GetAnswers : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/answers/{questionId:guid}", Handle)
            .WithTags(nameof(Answer))
            .Produces<GetAnswersResponse>()
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> Handle(
        Guid questionId,
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        IMediator mediator,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAnswersQuery(questionId, page ?? 1, pageSize ?? 10);
        
        var result = await mediator.Send(query, cancellationToken);

        if (result.IsError)
        {
            return result.Errors.ToProblem();
        }
        
        return Results.Ok(result.Value);
    }
}

internal sealed record GetAnswersQuery(
    Guid QuestionId,
    int Page,
    int PageSize)
    : IRequest<ErrorOr<GetAnswersResponse>>;

internal sealed class GetAnswersHandler : IRequestHandler<GetAnswersQuery, ErrorOr<GetAnswersResponse>>
{
    private readonly AppDbContext _dbContext;

    public GetAnswersHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<GetAnswersResponse>> Handle(GetAnswersQuery request, CancellationToken cancellationToken)
    {
        var exists =
            await _dbContext.Questions.AnyAsync(q => q.Id == request.QuestionId, cancellationToken);

        if (!exists)
        {
            return Error.NotFound("Question not found", "Question not found");
        }

        var offset = (request.Page - 1) * request.PageSize;

        var answers = await _dbContext.Answers
            .Where(a => a.QuestionId == request.QuestionId)
            .Skip(offset)
            .Take(request.PageSize)
            .OrderByDescending(a => a.CreatedAt)
            .ThenByDescending(a => a.IsBest)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var total =
            await _dbContext.Answers.CountAsync(a => a.QuestionId == request.QuestionId,
                cancellationToken: cancellationToken);

        return new GetAnswersResponse(total, answers
            .Select(a => a.MapToAnswerResponse())
            .ToList());
    }
}