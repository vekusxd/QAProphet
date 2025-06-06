﻿using System.Security.Claims;
using Carter;
using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using QAProphet.Data;
using QAProphet.Data.ElasticSearch;
using QAProphet.Data.EntityFramework;
using QAProphet.Domain;
using QAProphet.Extensions;
using QAProphet.Features.Questions.AskQuestion;
using QAProphet.Options;

namespace QAProphet.Features.Questions.EditQuestion;

public class EditQuestion : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/questions/{id:guid}", Handle)
            .WithTags(nameof(Question))
            .RequireAuthorization()
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict);
    }

    private static async Task<IResult> Handle(
        Guid id,
        AskQuestionRequest request,
        IValidator<AskQuestionRequest> validator,
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

        var command = request.MapToUpdateCommand(id, Guid.Parse(userId!));

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsError)
        {
            return result.Errors.ToProblem();
        }

        return Results.NoContent();
    }
}

internal sealed record EditQuestionCommand(
    Guid QuestionId,
    string Title,
    string Content,
    Guid AuthorId,
    List<Guid> Tags)
    : IRequest<ErrorOr<bool>>;

internal sealed class EditQuestionHandler : IRequestHandler<EditQuestionCommand, ErrorOr<bool>>
{
    private readonly AppDbContext _dbContext;
    private readonly TimeProvider _timeProvider;
    private readonly IOptions<QuestionTimeoutOptions> _options;
    private readonly ISearchService _searchService;
    private readonly LinkGenerator _linkGenerator;
    private readonly ILogger<EditQuestionHandler> _logger;

    public EditQuestionHandler(
        AppDbContext dbContext,
        TimeProvider timeProvider,
        IOptions<QuestionTimeoutOptions> options,
        ISearchService searchService,
        LinkGenerator linkGenerator,
        ILogger<EditQuestionHandler> logger)
    {
        _dbContext = dbContext;
        _timeProvider = timeProvider;
        _options = options;
        _searchService = searchService;
        _linkGenerator = linkGenerator;
        _logger = logger;
    }

    public async Task<ErrorOr<bool>> Handle(EditQuestionCommand request, CancellationToken cancellationToken)
    {
        var question =
            await _dbContext.Questions.FirstOrDefaultAsync(q => q.Id == request.QuestionId, cancellationToken);

        if (question is null)
        {
            return Error.NotFound("QuestionNotFound", "Question not found");
        }

        if (question.QuestionerId != request.AuthorId)
        {
            return Error.Forbidden("NotAuthor", "Not Author");
        }

        var currentTime = _timeProvider.GetUtcNow().UtcDateTime;

        if (currentTime - question.CreatedAt > TimeSpan.FromMinutes(_options.Value.EditQuestionInMinutes))
        {
            return Error.Conflict("Time expired", "time for update expired");
        }

        var tags = await _dbContext.Tags
            .AsNoTracking()
            .Where(t => request.Tags.Contains(t.Id))
            .ToListAsync(cancellationToken);

        if (tags.Count != request.Tags.Count)
        {
            return Error.Validation("TagNotExists", "Some tag not exists");
        }

        question.Title = request.Title;
        question.Content = request.Content;
        question.UpdateTime = currentTime;

        var questionTags = tags
            .Select(t => new QuestionTags { QuestionId = request.QuestionId, TagId = t.Id })
            .ToList();

        await _dbContext.QuestionTags
            .Where(t => t.QuestionId == request.QuestionId)
            .ExecuteDeleteAsync(cancellationToken);
        await _dbContext.QuestionTags.AddRangeAsync(questionTags, cancellationToken);

        _dbContext.Update(question);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var link = _linkGenerator.GetPathByName(nameof(GetQuestionDetails), new { id = question.Id });

        try
        {
            await _searchService.AddOrUpdateEntry(question.Id, question.Title, link, nameof(Question));
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "Error indexing question with id {@questionId}", question.Id);
        }

        return true;
    }
}