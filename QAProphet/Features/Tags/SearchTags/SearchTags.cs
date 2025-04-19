using Carter;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QAProphet.Data;
using QAProphet.Domain;

namespace QAProphet.Features.Tags.SearchTags;

public sealed record TagResponse(
    Guid Id,
    string Title);

public class SearchTags : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/tags", Handle)
            .WithTags(nameof(Tag))
            .Produces<List<TagResponse>>();
    }

    private static async Task<Ok<List<TagResponse>>> Handle(
        [FromQuery] string term,
        IMediator mediator,
        CancellationToken cancellationToken = default)
    {
        var query = new GetTagsQuery(term);
        var response = await mediator.Send(query, cancellationToken);
        
        return TypedResults.Ok(response);
    }
}

internal sealed record GetTagsQuery(
    string Term) :
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
        var tags = await _db.Tags
            .AsNoTracking()
            .Where(t => t.Title.ToLower().StartsWith(request.Term.ToLower()))
            .OrderBy(t => t.Title)
            .Take(10)
            .Select(t =>t.MapToResponse())
            .ToListAsync(cancellationToken: cancellationToken);

        return tags;
    }
}