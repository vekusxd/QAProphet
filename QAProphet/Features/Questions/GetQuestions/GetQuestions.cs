﻿using Carter;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QAProphet.Data;
using QAProphet.Data.EntityFramework;
using QAProphet.Domain;
using QAProphet.Features.Shared.Responses;

namespace QAProphet.Features.Questions.GetQuestions;

public class GetQuestions : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/questions", Handle)
            .WithTags(nameof(Question))
            .Produces<List<QuestionResponse>>();
    }

    private static async Task<IResult> Handle(
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        IMediator mediator,
        CancellationToken cancellationToken = default)
    {
        page ??= 1;
        pageSize ??= 10;
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 10;

        var query = new GetQuestionsQuery(page.Value, pageSize.Value);

        var response = await mediator.Send(query, cancellationToken);

        return Results.Ok(response);
    }
}

internal sealed record GetQuestionsQuery(
    int Page,
    int PageSize)
    : IRequest<List<QuestionResponse>>;

internal sealed class GetQuestionsHandler : IRequestHandler<GetQuestionsQuery, List<QuestionResponse>>
{
    private readonly AppDbContext _dbContext;

    public GetQuestionsHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<QuestionResponse>> Handle(GetQuestionsQuery request, CancellationToken cancellationToken)
    {
        var skip = (request.Page - 1) * request.PageSize;

        var questions = await _dbContext.Questions
            .AsNoTracking()
            .Include(q => q.Tags)
            .ThenInclude(q => q.Tag)
            .OrderByDescending(x => x.CreatedAt)
            .Skip(skip)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return questions
            .Select(q => q.MapToResponse())
            .ToList();
    }
}