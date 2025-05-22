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

namespace QAProphet.Features.Questions.PostQuestionComplaint;

public sealed record PostQuestionComplaintRequest(
    string QuestionId,
    string ComplaintCategoryId);

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
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem();
    }

    private static async Task<IResult> Handle(
        PostQuestionComplaintRequest request,
        IValidator<PostQuestionComplaintRequest> validator,
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

        var command = new PostQuestionComplaintCommand(
            Guid.Parse(request.QuestionId), Guid.Parse(request.ComplaintCategoryId), Guid.Parse(userId!));

        var result = await mediator.Send(command, cancellationToken);

        if (result is not null)
        {
            return result.Value.ToProblem();
        }

        return Results.Ok();
    }
}

internal sealed record PostQuestionComplaintCommand(
    Guid QuestionId,
    Guid ComplaintCategoryId,
    Guid UserId) : IRequest<Error?>;

internal sealed record PostQuestionComplaintHandler : IRequestHandler<PostQuestionComplaintCommand, Error?>
{
    private readonly AppDbContext _dbContext;
    private readonly TimeProvider _timeProvider;

    public PostQuestionComplaintHandler(AppDbContext dbContext, TimeProvider timeProvider)
    {
        _dbContext = dbContext;
        _timeProvider = timeProvider;
    }

    public async Task<Error?> Handle(PostQuestionComplaintCommand request, CancellationToken cancellationToken)
    {
        var question = await _dbContext.Questions
            .SingleOrDefaultAsync(q => q.Id == request.QuestionId, cancellationToken);

        if (question is null)
        {
            return Error.NotFound("Question not found", "Question was not found");
        }

        var category = await _dbContext.QuestionComplaintCategories
            .SingleOrDefaultAsync(c => c.Id == request.ComplaintCategoryId, cancellationToken);

        if (category is null)
        {
            return Error.NotFound("Category not found", "Category was not found");
        }

        if (await _dbContext.QuestionComplaints.AnyAsync(
                qc => qc.QuestionId == request.QuestionId && qc.UserId == request.UserId, cancellationToken))
        {
            return Error.Conflict("Complaint already posted", "Complaint already posted");
        }

        var complaint = new QuestionComplaint
        {
            QuestionId = question.Id,
            UserId = request.UserId,
            CategoryId = category.Id,
            CreatedAt = _timeProvider.GetUtcNow().UtcDateTime,
        };

        await _dbContext.QuestionComplaints.AddAsync(complaint, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return null;
    }
}