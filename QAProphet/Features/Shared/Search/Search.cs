using Carter;
using QAProphet.Data.ElasticSearch;

namespace QAProphet.Features.Shared.Search;

public class Search : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/search", Handle)
            .WithTags(nameof(Search));
    }

    private static async Task<IResult> Handle(ISearchService service)
    {
        return Results.Ok();
    }
}