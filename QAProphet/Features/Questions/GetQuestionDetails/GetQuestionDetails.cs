using Carter;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QAProphet.Data;
using QAProphet.Data.EntityFramework;
using QAProphet.Domain;
using QAProphet.Extensions;
using QAProphet.Features.Shared.Responses;
using QAProphet.Features.Tags.SearchTags;

namespace QAProphet.Features.Questions.GetQuestionDetails;

public sealed record QuestionDetailsResponse(
    Guid Id,
    string Title,
    string Content,
    Guid AuthorId,
    string AuthorName,
    DateTime Created,
    DateTime? Updated,
    List<TagResponse> Tags,
    List<AnswerResponse> Answers,
    int AnswersCount);

public class GetQuestionDetails : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/questions/{id:guid}", Handle)
            .WithName(nameof(GetQuestionDetails))
            .WithTags(nameof(Question))
            .Produces<QuestionDetailsResponse>()
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> Handle(
        Guid id,
        [FromQuery] int? answersCount,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetQuestionDetailsQuery(id, answersCount ?? 10);
        var result = await mediator.Send(query, cancellationToken);

        if (result.IsError)
        {
            return result.Errors.ToProblem();
        }

        return Results.Ok(result.Value);
    }
}

internal sealed record GetQuestionDetailsQuery(
    Guid QuestionId,
    int AnswersCount)
    : IRequest<ErrorOr<QuestionDetailsResponse>>;

internal sealed class
    GetQuestionDetailsHandler : IRequestHandler<GetQuestionDetailsQuery, ErrorOr<QuestionDetailsResponse>>
{
    private readonly AppDbContext _dbContext;

    public GetQuestionDetailsHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<QuestionDetailsResponse>> Handle(GetQuestionDetailsQuery request,
        CancellationToken cancellationToken)
    {
        var question = await _dbContext.Questions
            .Include(q => q.Answers
                .OrderByDescending(a => a.CreatedAt)
                .ThenByDescending(a => a.IsBest)
                .Take(request.AnswersCount))
            .Include(q => q.Tags)
            .ThenInclude(t => t.Tag)
            .AsSplitQuery()
            .AsNoTracking()
            .FirstOrDefaultAsync(q => q.Id == request.QuestionId, cancellationToken);


        if (question is null)
        {
            return Error.NotFound("QuestionNotFound", "Question not found");
        }

        var answersCount = await _dbContext.Answers.CountAsync(q => q.QuestionId == request.QuestionId,
            cancellationToken: cancellationToken);

        return question.MapToDetailsResponse(answersCount);
    }
}