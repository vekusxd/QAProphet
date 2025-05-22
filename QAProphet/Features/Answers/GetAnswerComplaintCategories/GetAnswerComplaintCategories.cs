using Carter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QAProphet.Data;
using QAProphet.Data.EntityFramework;
using QAProphet.Domain;
using QAProphet.Features.Shared.Responses;

namespace QAProphet.Features.Answers.GetAnswerComplaintCategories;

public class GetAnswerComplaintCategories : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/answers/complaintCategories", Handle)
            .WithTags(nameof(Answer))
            .RequireAuthorization()
            .Produces<List<ComplaintCategoryResponse>>()
            .Produces(StatusCodes.Status401Unauthorized);
    }

    private static async Task<IResult> Handle(
        IMediator mediator,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetAnswerComplaintCategoriesQuery(), cancellationToken);
        
        return Results.Ok(result);
    }
}

internal sealed record GetAnswerComplaintCategoriesQuery :
    IRequest<List<ComplaintCategoryResponse>>;

internal sealed class
    GetAnswerComplaintCategoriesHandler : IRequestHandler<GetAnswerComplaintCategoriesQuery,
    List<ComplaintCategoryResponse>>
{
    private readonly AppDbContext _dbContext;

    public GetAnswerComplaintCategoriesHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<List<ComplaintCategoryResponse>> Handle(GetAnswerComplaintCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _dbContext.AnswerComplaintCategories
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        
        return categories.MapToResponse();
    }
}