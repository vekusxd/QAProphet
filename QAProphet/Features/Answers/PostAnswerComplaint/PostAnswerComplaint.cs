using System.Security.Claims;
using Carter;
using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QAProphet.Data;
using QAProphet.Domain;
using QAProphet.Extensions;

namespace QAProphet.Features.Answers.PostAnswerComplaint;

public sealed record PostAnswerComplaintRequest(
    string AnswerId,
    string ComplaintCategoryId);

public class PostAnswerComplaint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/answers/complaints", Handle)
            .WithTags(nameof(Answer))
            .RequireAuthorization()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem();
    }

    private static async Task<IResult> Handle(
        PostAnswerComplaintRequest request,
        IValidator<PostAnswerComplaintRequest> validator,
        IMediator mediator,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());

        var userId = claimsPrincipal.GetUserId();

        var command = new PostAnswerComplaintCommand(Guid.Parse(request.AnswerId),
            Guid.Parse(request.ComplaintCategoryId), Guid.Parse(userId!));

        var result = await mediator.Send(command, cancellationToken);

        if (result is not null)
        {
            return result.Value.ToProblem();
        }

        return Results.Ok();
    }
}

internal sealed record PostAnswerComplaintCommand(
    Guid AnswerId,
    Guid ComplaintCategoryId,
    Guid UserId) : IRequest<Error?>;

internal sealed class PostAnswerComplaintHandler : IRequestHandler<PostAnswerComplaintCommand, Error?>
{
    private readonly AppDbContext _dbContext;
    private readonly TimeProvider _timeProvider;

    public PostAnswerComplaintHandler(AppDbContext dbContext, TimeProvider timeProvider)
    {
        _dbContext = dbContext;
        _timeProvider = timeProvider;
    }

    public async Task<Error?> Handle(PostAnswerComplaintCommand request, CancellationToken cancellationToken)
    {
        var answer = await _dbContext.Answers.SingleOrDefaultAsync(a => a.Id == request.AnswerId, cancellationToken);

        if (answer is null)
        {
            return Error.NotFound("Answer not found", "Answer was not found");
        }

        var category =
            await _dbContext.AnswerComplaintCategories.SingleOrDefaultAsync(c => c.Id == request.ComplaintCategoryId,
                cancellationToken);

        if (category is null)
        {
            return Error.NotFound("ComplaintCategory not found", "ComplaintCategory was not found");
        }

        if (await _dbContext.AnswerComplaints.AnyAsync(
                c => c.AnswerId == request.AnswerId && c.UserId == request.UserId, cancellationToken))
        {
            return Error.Conflict("Complaint already posted", "Complaint already posted");
        }

        var complaint = new AnswerComplaint
        {
            AnswerId = answer.Id,
            CategoryId = category.Id,
            UserId = request.UserId,
            CreatedAt = _timeProvider.GetUtcNow().UtcDateTime
        };

        await _dbContext.AnswerComplaints.AddAsync(complaint, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return null;
    }
}