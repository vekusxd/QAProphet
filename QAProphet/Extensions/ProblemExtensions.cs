using ErrorOr;

namespace QAProphet.Extensions;

public static class EndpointResultsExtensions
{
    public static IResult ToProblem(this List<Error> errors)
    {
        if (errors.Count is 0)
        {
            return Results.Problem();
        }

        return CreateProblem(errors);
    }

    private static IResult CreateProblem(List<Error> errors)
    {
        var statusCode = errors.First().Type switch
        {
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError
        };

        return statusCode switch
        {
            StatusCodes.Status404NotFound => Results.NotFound(),
            StatusCodes.Status409Conflict => Results.Conflict(),
            StatusCodes.Status403Forbidden => Results.Forbid(),
            _ => Results.ValidationProblem(errors.ToDictionary(k => k.Code, v => new[] { v.Description }),
                statusCode: statusCode)
        };
    }

    public static IResult ToProblem(this Error error)
    {
        return CreateProblem([error]);
    }
}