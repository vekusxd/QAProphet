using System.Security.Claims;
using Carter;
using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QAProphet.Data;
using QAProphet.Domain;
using QAProphet.Extensions;

namespace QAProphet.Features.Tags.SubscribeToTag;

public record SubscribeToTagRequest(string TagId);

public class SubscribeToTag : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/tags/subscribe", Handle)
            .WithTags(nameof(Tag))
            .RequireAuthorization()
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);
    }

    private static async Task<IResult> Handle(
        SubscribeToTagRequest request,
        IValidator<SubscribeToTagRequest> validator,
        IMediator mediator,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var userId = claimsPrincipal.GetUserId();

        var command = new SubscribeToTagCommand(Guid.Parse(request.TagId), Guid.Parse(userId!));

        var result = await mediator.Send(command, cancellationToken);

        if (result is not null)
        {
            return result.Value.ToProblem();
        }

        return Results.NoContent();
    }
}

internal sealed record SubscribeToTagCommand(
    Guid TagId,
    Guid UserId) :
    IRequest<Error?>;

internal sealed class SubscribeToTagHandler : IRequestHandler<SubscribeToTagCommand, Error?>
{
    private readonly AppDbContext _dbContext;

    public SubscribeToTagHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Error?> Handle(SubscribeToTagCommand request, CancellationToken cancellationToken)
    {
        var tag = await _dbContext.Tags.SingleOrDefaultAsync(s => s.Id == request.TagId, cancellationToken);
        if (tag == null) return Error.NotFound("Tag not found", "Tag not found");

        var subscribe = await _dbContext.TagSubscribes
            .SingleOrDefaultAsync(s => s.TagId == request.TagId && s.UserId == request.UserId, cancellationToken);

        if (subscribe != null)
        {
            _dbContext.Remove(subscribe);
        }
        else
        {
            subscribe = new TagSubscribe
            {
                TagId = request.TagId,
                UserId = request.UserId
            };
            _dbContext.TagSubscribes.Add(subscribe);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return null;
    }
}