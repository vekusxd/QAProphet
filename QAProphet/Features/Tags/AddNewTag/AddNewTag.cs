using Carter;
using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QAProphet.Data.ElasticSearch;
using QAProphet.Data.EntityFramework;
using QAProphet.Domain;
using QAProphet.Extensions;
using QAProphet.Features.Shared.Responses;

namespace QAProphet.Features.Tags.AddNewTag;

public sealed record AddTagRequest(string Title, string Description);

public sealed record AddTagResponse(Guid Id, string Title, string Description);

public class AddNewTag : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/tags", Handle)
            .WithTags(nameof(Tag))
            .RequireAuthorization()
            .Produces<AddTagResponse>()
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);
    }

    private static async Task<IResult> Handle(
        AddTagRequest request,
        IValidator<AddTagRequest> validator,
        IMediator mediator,
        CancellationToken token = default)
    {
        var validationResult = await validator.ValidateAsync(request, token);

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var command = request.MapToCommand();

        var result = await mediator.Send(command, token);

        if (result.IsError)
        {
            return result.Errors.ToProblem();
        }

        return Results.Ok(result.Value);
    }
}

internal sealed record AddTagCommand(string Title, string Description)
    : IRequest<ErrorOr<AddTagResponse>>;

internal sealed class AddTagCommandHandler : IRequestHandler<AddTagCommand, ErrorOr<AddTagResponse>>
{
    private readonly AppDbContext _dbContext;
    private readonly TimeProvider _timeProvider;
    private readonly ISearchService _searchService;
    private readonly ILogger<AddTagCommandHandler> _logger;
    private readonly LinkGenerator _linkGenerator;

    public AddTagCommandHandler(
        AppDbContext dbContext,
        TimeProvider timeProvider,
        ISearchService searchService,
        ILogger<AddTagCommandHandler> logger,
        LinkGenerator linkGenerator)
    {
        _dbContext = dbContext;
        _timeProvider = timeProvider;
        _searchService = searchService;
        _logger = logger;
        _linkGenerator = linkGenerator;
    }

    public async Task<ErrorOr<AddTagResponse>> Handle(AddTagCommand request, CancellationToken cancellationToken)
    {
        var exists = await _dbContext.Tags.AnyAsync(t => t.Title == request.Title, cancellationToken);

        if (exists)
        {
            return Error.Conflict("Tag already exists", "Tag already exists");
        }

        var tag = new Tag
        {
            Title = request.Title,
            Description = request.Description,
            CreatedAt = _timeProvider.GetUtcNow().UtcDateTime
        };

        await _dbContext.Tags.AddAsync(tag, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        try
        {
            var link = _linkGenerator.GetPathByName(nameof(GetTagDetails), new { id = tag.Id });
            await _searchService.AddOrUpdateEntry(tag.Id, tag.Title, link, nameof(Tag));
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Error indexing tag with id {@tagId}", tag.Id);
        }

        return tag.MapToCreateResponse();
    }
}