using System.Security.Claims;
using Carter;
using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QAProphet.Data;
using QAProphet.Domain;
using QAProphet.Extensions;
using QAProphet.Features.Questions.AskQuestion;

namespace QAProphet.Features.Questions.EditQuestion;

public class UpdateQuestion : ICarterModule
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
        
        var command = request.MapToUpdateCommand(id,Guid.Parse(userId!));
        
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsError)
        {
            return result.Errors.ToProblem();
        }
        
        return Results.NoContent();
    }
}

internal sealed record UpdateQuestionCommand(
    Guid QuestionId,
    string Title,
    string Content,
    Guid AuthorId,
    List<Guid> Tags)
    : IRequest<ErrorOr<bool>>;

internal sealed class UpdateQuestionHandler : IRequestHandler<UpdateQuestionCommand, ErrorOr<bool>>
{
    private readonly AppDbContext _dbContext;

    public UpdateQuestionHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<ErrorOr<bool>> Handle(UpdateQuestionCommand request, CancellationToken cancellationToken)
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
        
        if (DateTime.UtcNow - question.CreatedAt > TimeSpan.FromHours(1))
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
        question.UpdateTime = DateTime.UtcNow;
        
        var questionTags = tags
            .Select(t => new QuestionTags{QuestionId = request.QuestionId, TagId = t.Id})
            .ToList();

        await _dbContext.QuestionTags
            .Where(t => t.QuestionId == request.QuestionId)
            .ExecuteDeleteAsync(cancellationToken);
        await _dbContext.QuestionTags.AddRangeAsync(questionTags, cancellationToken);
        
        _dbContext.Update(question);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}