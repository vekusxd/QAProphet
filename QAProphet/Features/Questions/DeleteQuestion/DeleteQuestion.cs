using System.Security.Claims;
using Carter;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QAProphet.Data;
using QAProphet.Domain;
using QAProphet.Extensions;

namespace QAProphet.Features.Questions.DeleteQuestion;

public class DeleteQuestion : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/questions/{id:guid}", Handle)
            .WithTags(nameof(Question))
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> Handle(
        Guid id,
        IMediator mediator,
        ClaimsPrincipal principal,
        CancellationToken cancellationToken = default)
    {
        var userId = principal.GetUserId();

        var deleteCommand = new DeleteQuestionCommand(id, Guid.Parse(userId!));

        var result = await mediator.Send(deleteCommand, cancellationToken);

        if (result.IsError)
        {
            return result.Errors.ToProblem();
        }

        return Results.NoContent();
    }
}

internal sealed record DeleteQuestionCommand(
    Guid QuestionId,
    Guid UserId)
    : IRequest<ErrorOr<bool>>;

internal sealed class DeleteQuestionHandler : IRequestHandler<DeleteQuestionCommand, ErrorOr<bool>>
{
    private readonly AppDbContext _dbContext;

    public DeleteQuestionHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<bool>> Handle(DeleteQuestionCommand request, CancellationToken cancellationToken)
    {
        var question =
            await _dbContext.Questions.FirstOrDefaultAsync(q => q.Id == request.QuestionId, cancellationToken);

        if (question is null)
        {
            return Error.NotFound("QuestionNotFound", "Question not found");
        }

        if (question.QuestionerId != request.UserId)
        {
            return Error.Forbidden("NotAuthor", "Not Author");
        }

        if (DateTime.UtcNow - question.CreatedAt > TimeSpan.FromHours(1))
        {
            return Error.Conflict("Time expired", "time for delete expired");
        }
        
        question.IsDeleted = true;
        _dbContext.Questions.Update(question);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}