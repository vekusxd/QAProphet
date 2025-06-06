﻿using System.Security.Claims;
using Carter;
using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QAProphet.Data;
using QAProphet.Data.ElasticSearch;
using QAProphet.Data.EntityFramework;
using QAProphet.Domain;
using QAProphet.Extensions;

namespace QAProphet.Features.Questions.AskQuestion;

public sealed record AskQuestionRequest(
    string Title,
    string Content,
    List<string> Tags);

public sealed record AskQuestionResponse(
    Guid Id,
    string Title,
    string Content,
    DateTime CreatedAt,
    string AuthorName,
    string AuthorId);

public class AskQuestion : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/questions", Handle)
            .WithTags(nameof(Question))
            .RequireAuthorization()
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces<AskQuestionResponse>();
    }

    private static async Task<IResult> Handle(
        AskQuestionRequest request,
        IValidator<AskQuestionRequest> validator,
        IMediator mediator,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var username = claimsPrincipal.GetUserName();
        var userId = claimsPrincipal.GetUserId();

        var command = request.MapToCommand(username!, userId!);

        var response = await mediator.Send(command, cancellationToken);

        if (response.IsError)
        {
            return response.Errors.ToProblem();
        }

        return TypedResults.Ok(response.Value);
    }
}

internal sealed record AskQuestionCommand(
    string Title,
    string Description,
    string AuthorId,
    string AuthorName,
    List<Guid> Tags)
    : IRequest<ErrorOr<AskQuestionResponse>>;

internal sealed class AskQuestionHandler : IRequestHandler<AskQuestionCommand, ErrorOr<AskQuestionResponse>>
{
    private readonly AppDbContext _dbContext;
    private readonly TimeProvider _timeProvider;
    private readonly ISearchService _searchService;
    private readonly LinkGenerator _linkGenerator;
    private readonly ILogger<AskQuestionHandler> _logger;

    public AskQuestionHandler(
        AppDbContext dbContext,
        TimeProvider timeProvider,
        ISearchService searchService,
        LinkGenerator linkGenerator,
        ILogger<AskQuestionHandler> logger)
    {
        _dbContext = dbContext;
        _timeProvider = timeProvider;
        _searchService = searchService;
        _linkGenerator = linkGenerator;
        _logger = logger;
    }

    public async Task<ErrorOr<AskQuestionResponse>> Handle(
        AskQuestionCommand request,
        CancellationToken cancellationToken)
    {
        var tags = await _dbContext.Tags
            .AsNoTracking()
            .Where(t => request.Tags.Contains(t.Id))
            .ToListAsync(cancellationToken);

        if (tags.Count != request.Tags.Count)
        {
            return Error.Validation("TagNotExists", "Some tag not exists");
        }

        var question = new Question
        {
            Title = request.Title,
            CreatedAt = _timeProvider.GetUtcNow().UtcDateTime,
            Content = request.Description,
            AuthorName = request.AuthorName,
            QuestionerId = Guid.Parse(request.AuthorId)
        };

        await _dbContext.Questions.AddAsync(question, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var questionTags = tags.Select(t => new QuestionTags
        {
            QuestionId = question.Id,
            TagId = t.Id
        });

        await _dbContext.QuestionTags.AddRangeAsync(questionTags, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        try
        {
            const string path = nameof(GetQuestionDetails);

            var url = _linkGenerator.GetPathByName(path, new { id = question.Id });

            await _searchService.AddOrUpdateEntry(question.Id, question.Title, url, nameof(Question));
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Error indexing question with id {@questionId}", question.Id);
        }

        return question.MapToAskResponse();
    }
}