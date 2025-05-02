using Carter;
using QAProphet.Domain;

namespace QAProphet.Features.Questions.PostQuestionComplaint;

public class PostQuestionComplaint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/questions/complaints", Handle)
            .WithTags(nameof(Question))
            .RequireAuthorization()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status409Conflict);
    }

    private static async Task<IResult> Handle()
    {
        return Results.Ok();
    }
}