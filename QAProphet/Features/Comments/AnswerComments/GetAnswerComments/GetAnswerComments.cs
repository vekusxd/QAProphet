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

namespace QAProphet.Features.Comments.AnswerComments.GetAnswerComments;

public record GetAnswerCommentsResponse(int Total, ICollection<CommentResponse> Comments);

public class GetAnswerComments : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/answers/comments/{answerId:guid}", Handle)
            .WithTags(nameof(AnswerComment))
            .Produces<GetAnswerCommentsResponse>();
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
    : IRequest<ErrorOr<GetAnswerCommentsResponse>>;

internal sealed class GetAnswerCommentsHandler
    : IRequestHandler<GetAnswerCommentsQuery, ErrorOr<GetAnswerCommentsResponse>>
{
    private readonly AppDbContext _context;

    public GetAnswerCommentsHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<GetAnswerCommentsResponse>> Handle(GetAnswerCommentsQuery request,
        CancellationToken cancellationToken)
    {
        var exists =
            await _context.Answers.AnyAsync(a => a.Id == request.AnswerId, cancellationToken);

        if (!exists)
        {
            return Error.NotFound("Answer not found", "Answer not found");
        }

        var offset = (request.PageNumber - 1) * request.PageSize;

        var comments = await _context.AnswerComments
            .AsNoTracking()
            .Where(a => a.AnswerId == request.AnswerId)
            .Skip(offset)
            .Take(request.PageSize)
            .OrderByDescending(ac => ac.CreatedAt)
            .ToListAsync(cancellationToken);

        var total = await _context.AnswerComments.CountAsync(a => a.AnswerId == request.AnswerId, cancellationToken);
        
        return new GetAnswerCommentsResponse(total, comments.Select(c => c.MapToResponse()).ToList());
    }
}