using Carter;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QAProphet.Data.EntityFramework;
using QAProphet.Domain;
using QAProphet.Extensions;
using QAProphet.Features.Comments.Shared.Responses;
using QAProphet.Features.Shared.Mappings;

namespace QAProphet.Features.Comments.QuestionComments.GetQuestionComments;

public class GetQuestionComments : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/questions/comments/{questionId:guid}", Handle)
            .WithTags(nameof(QuestionComment))
            .Produces<PaginatedCommentResponse>();
    }

    private static async Task<IResult> Handle(
        Guid questionId,
        [FromQuery] int? pageNumber,
        [FromQuery] int? pageSize,
        IMediator mediator,
        CancellationToken cancellationToken = default)
    {
        var query = new GetQuestionCommentsQuery(questionId, pageNumber ?? 1, pageSize ?? 10);

        var result = await mediator.Send(query, cancellationToken);

        if (result.IsError)
        {
            return result.Errors.ToProblem();
        }

        return Results.Ok(result.Value);
    }
}

internal sealed record GetQuestionCommentsQuery(
    Guid QuestionId,
    int PageNumber,
    int PageSize)
    : IRequest<ErrorOr<PaginatedCommentResponse>>;

internal sealed class GetQuestionCommentsHandler
    : IRequestHandler<GetQuestionCommentsQuery, ErrorOr<PaginatedCommentResponse>>
{
    private readonly AppDbContext _dbContext;

    public GetQuestionCommentsHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<PaginatedCommentResponse>> Handle(GetQuestionCommentsQuery request,
        CancellationToken cancellationToken)
    {
        var exists =
            await _dbContext.Questions.AnyAsync(q => q.Id == request.QuestionId, cancellationToken);

        if (!exists)
        {
            return Error.NotFound("Question not found", "Question not found");
        }

        var offset = (request.PageNumber - 1) * request.PageSize;

        var comments = await _dbContext.QuestionComments
            .AsNoTracking()
            .Where(q => q.QuestionId == request.QuestionId)
            .Skip(offset)
            .Take(request.PageSize)
            .OrderByDescending(qc => qc.CreatedAt)
            .ToListAsync(cancellationToken);

        var total = await _dbContext.QuestionComments.CountAsync(qc => qc.QuestionId == request.QuestionId,
            cancellationToken: cancellationToken);

        return new PaginatedCommentResponse(total, comments.Select(c => c.MapToResponse()).ToList());
    }
}