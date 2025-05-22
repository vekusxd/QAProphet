using Carter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QAProphet.Data;
using QAProphet.Data.EntityFramework;
using QAProphet.Domain;
using QAProphet.Features.Shared.Responses;

namespace QAProphet.Features.Questions.GetQuestionComplaintCategories;

public class GetQuestionComplaintCategories : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/questions/complaintCategories", Handle)
            .WithTags(nameof(Question))
            .RequireAuthorization()
            .Produces<List<ComplaintCategoryResponse>>()
            .Produces(StatusCodes.Status401Unauthorized);
    }

    private static async Task<IResult> Handle(
        IMediator mediator,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetQuestionComplaintCategoriesQuery(), cancellationToken);

        return Results.Ok(result);
    }
}

internal sealed record GetQuestionComplaintCategoriesQuery :
    IRequest<List<ComplaintCategoryResponse>>;

internal sealed class
    GetQuestionComplaintCategoriesHandler : IRequestHandler<GetQuestionComplaintCategoriesQuery,
    List<ComplaintCategoryResponse>>
{
    private readonly AppDbContext _dbContext;

    public GetQuestionComplaintCategoriesHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<ComplaintCategoryResponse>> Handle(GetQuestionComplaintCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var categories = await _dbContext.QuestionComplaintCategories
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return categories.MapToResponse();
    }
}