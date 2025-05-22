using Carter;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QAProphet.Data.EntityFramework;
using QAProphet.Domain;
using QAProphet.Extensions;
using QAProphet.Features.Questions.AskQuestion;
using QAProphet.Features.Questions.GetQuestions;
using QAProphet.Features.Shared.Responses;

namespace QAProphet.Features.Tags.GetTagDetails;

public record GetTagDetailsResponse(
    Guid Id,
    string Title,
    string Description,
    int QuestionsCount,
    ICollection<QuestionResponse> Questions);

public class GetTagDetails : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/tags/{id:guid}", Handle)
            .WithTags(nameof(Tag))
            .WithName(nameof(GetTagDetails))
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> Handle(
        Guid id,
        [FromQuery] int? pageSize,
        [FromQuery] int? pageNumber,
        IMediator mediator,
        CancellationToken cancellationToken = default)
    {
        var query = new GetTagDetailsQuery(id, pageNumber ?? 1, pageSize ?? 10);

        var result = await mediator.Send(query, cancellationToken);

        if (result.IsError)
        {
            return result.Errors.ToProblem();
        }

        return Results.Ok(result.Value);
    }
}

internal sealed record GetTagDetailsQuery(Guid TagId, int Page, int PageSize)
    : IRequest<ErrorOr<GetTagDetailsResponse>>;

internal sealed class GetTagDetailsQueryHandler : IRequestHandler<GetTagDetailsQuery, ErrorOr<GetTagDetailsResponse>>
{
    private readonly AppDbContext _context;

    public GetTagDetailsQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<GetTagDetailsResponse>> Handle(GetTagDetailsQuery request,
        CancellationToken cancellationToken)
    {
        var offset = (request.Page - 1) * request.PageSize;

        var tag = await _context.Tags
            .AsNoTracking()
            .Include(t => t.Questions
                .OrderByDescending(q => q.CreatedAt)
                .Skip(offset)
                .Take(request.PageSize))
            .ThenInclude(q => q.Question)
            .SingleOrDefaultAsync(t => t.Id == request.TagId, cancellationToken);
        
        if (tag is null)
        {
            return Error.NotFound("TagNotFound", "Tag not found");
        }
        
        var questionsCount = await _context.QuestionTags
            .Where(q => q.TagId == request.TagId)
            .CountAsync(cancellationToken);

        return tag.MapToDetailsResponse(questionsCount);
    }
}