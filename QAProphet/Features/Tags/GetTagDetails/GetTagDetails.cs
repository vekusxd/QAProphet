using Carter;
using QAProphet.Domain;
using QAProphet.Features.Questions.AskQuestion;

namespace QAProphet.Features.Tags.GetTagDetails;

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

    private static async Task<IResult> Handle(Guid id)
    {
        return Results.Ok();
    }
}