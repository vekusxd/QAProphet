using Carter;
using Microsoft.AspNetCore.Mvc;
using QAProphet.Data.ElasticSearch;

namespace QAProphet.Features.Shared.Search;

public class Search : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/search", Handle)
            .WithTags(nameof(Search))
            .Produces<ICollection<IndexEntry>>();
    }

    private static async Task<IResult> Handle(
        ISearchService service,
        [FromQuery] string? s,
        [FromQuery] int? pageSize,
        [FromQuery] int? pageNumber)
    {
        var result = await service.SearchEntries(s ?? string.Empty, pageSize ?? 10, pageNumber ?? 1);
        return Results.Ok(result);
    }
}