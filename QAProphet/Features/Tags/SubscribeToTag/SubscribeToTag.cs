using System.Security.Claims;
using Carter;
using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QAProphet.Data;
using QAProphet.Data.EntityFramework;
using QAProphet.Domain;
using QAProphet.Extensions;

namespace QAProphet.Features.Tags.SubscribeToTag;

public class SubscribeToTag : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/tags/subscribe/{tagId:guid}", Handle)
            .WithTags(nameof(Tag))
            .RequireAuthorization()
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);
    }

    private static async Task<IResult> Handle(
        Guid tagId,
        IMediator mediator,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken = default)
    {
        var userId = claimsPrincipal.GetUserId();

        var command = new SubscribeToTagCommand(tagId, Guid.Parse(userId!));

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
        if (tag is null) return Error.NotFound("Tag not found", "Tag not found");

        var subscribe = await _dbContext.TagSubscribes
            .SingleOrDefaultAsync(s => s.TagId == request.TagId && s.UserId == request.UserId, cancellationToken);

        if (subscribe is not null)
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
            await _dbContext.TagSubscribes.AddAsync(subscribe, cancellationToken);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return null;
    }
}