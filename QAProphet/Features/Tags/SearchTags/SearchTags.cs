using Carter;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QAProphet.Data;
using QAProphet.Data.EntityFramework;
using QAProphet.Domain;
using QAProphet.Features.Shared.Responses;

namespace QAProphet.Features.Tags.SearchTags;


public class SearchTags : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/tags", Handle)
            .WithTags(nameof(Tag))
            .Produces<List<TagResponse>>();
    }

    private static async Task<Ok<List<TagResponse>>> Handle(
        [FromQuery] string? term,
        [FromQuery] int? pageSize,
        [FromQuery] int? pageNumber,
        IMediator mediator,
        CancellationToken cancellationToken = default)
    {
        var query = new GetTagsQuery(term ?? string.Empty, pageSize ?? 10, pageNumber ?? 1);
        var response = await mediator.Send(query, cancellationToken);
        
        return TypedResults.Ok(response);
    }
}

internal sealed record GetTagsQuery(
    string Term,
    int PageSize,
    int PageNumber) :
    IRequest<List<TagResponse>>;

internal sealed class GetTagsHandler : IRequestHandler<GetTagsQuery, List<TagResponse>>
{
    private readonly AppDbContext _db;

    public GetTagsHandler(AppDbContext db)
    {
        _db = db;
    }
    
    public async Task<List<TagResponse>> Handle(GetTagsQuery request, CancellationToken cancellationToken)
    {
        var offset = (request.PageNumber - 1) * request.PageSize;
        
        var tags = await _db.Tags
            .AsNoTracking()
            .Where(t => t.Title.ToLower().StartsWith(request.Term.ToLower()))
            .OrderBy(t => t.Title)
            .Skip(offset)
            .Take(request.PageSize)
            .Select(t =>t.MapToResponse())
            .ToListAsync(cancellationToken: cancellationToken);

        return tags;
    }
}