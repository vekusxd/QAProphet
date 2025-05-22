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
using QAProphet.Features.Shared.Responses;

namespace QAProphet.Features.Comments.AnswerComments.GetAnswerComments;


public class GetAnswerComments : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/answers/comments/{answerId:guid}", Handle)
            .WithTags(nameof(AnswerComment))
            .Produces<PaginatedCommentResponse>();
    }

    private static async Task<IResult> Handle(
        Guid answerId,
        [FromQuery] int? pageNumber,
        [FromQuery] int? pageSize,
        IMediator mediator,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAnswerCommentsQuery(answerId, pageNumber ?? 1, pageSize ?? 10);

        var result = await mediator.Send(query, cancellationToken);

        if (result.IsError)
        {
            return result.Errors.ToProblem();
        }

        return Results.Ok(result.Value);
    }
}

internal sealed record GetAnswerCommentsQuery(
    Guid AnswerId,
    int PageNumber,
    int PageSize)
    : IRequest<ErrorOr<PaginatedCommentResponse>>;

internal sealed class GetAnswerCommentsHandler
    : IRequestHandler<GetAnswerCommentsQuery, ErrorOr<PaginatedCommentResponse>>
{
    private readonly AppDbContext _dbContext;

    public GetAnswerCommentsHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<PaginatedCommentResponse>> Handle(GetAnswerCommentsQuery request,
        CancellationToken cancellationToken)
    {
        var exists =
            await _dbContext.Answers.AnyAsync(a => a.Id == request.AnswerId, cancellationToken);

        if (!exists)
        {
            return Error.NotFound("Answer not found", "Answer not found");
        }

        var offset = (request.PageNumber - 1) * request.PageSize;

        var comments = await _dbContext.AnswerComments
            .AsNoTracking()
            .Where(a => a.AnswerId == request.AnswerId)
            .Skip(offset)
            .Take(request.PageSize)
            .OrderByDescending(ac => ac.CreatedAt)
            .ToListAsync(cancellationToken);

        var total = await _dbContext.AnswerComments.CountAsync(a => a.AnswerId == request.AnswerId, cancellationToken);
        
        return new PaginatedCommentResponse(total, comments.Select(c => c.MapToResponse()).ToList());
    }
}