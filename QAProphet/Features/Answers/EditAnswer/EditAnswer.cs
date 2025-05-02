using System.Security.Claims;
using Carter;
using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using QAProphet.Data;
using QAProphet.Domain;
using QAProphet.Extensions;
using QAProphet.Features.Answers.Shared.Mapping;
using QAProphet.Features.Answers.Shared.Responses;
using QAProphet.Options;

namespace QAProphet.Features.Answers.EditAnswer;

public sealed record EditAnswerRequest(string Content);

public class EditAnswer : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/answers/{id:guid}", Handle)
            .WithTags(nameof(Answer))
            .RequireAuthorization()
            .Produces<AnswerUpdateResponse>()
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status409Conflict);
    }

    private static async Task<IResult> Handle(
        Guid id,
        EditAnswerRequest request,
        IValidator<EditAnswerRequest> validator,
        ClaimsPrincipal claimsPrincipal,
        IMediator mediator,
        CancellationToken cancellationToken = default
        )
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var userId = claimsPrincipal.GetUserId();
        
        var command = new EditAnswerCommand(id, Guid.Parse(userId!), request.Content);
        
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsError)
        {
            return result.Errors.ToProblem();
        }

        return Results.Ok(result.Value);
    }
}

internal sealed record EditAnswerCommand(
    Guid AnswerId,
    Guid UserId,
    string Content)
    : IRequest<ErrorOr<AnswerUpdateResponse>>;


internal sealed class EditAnswerHandler : IRequestHandler<EditAnswerCommand, ErrorOr<AnswerUpdateResponse>>
{
    private readonly AppDbContext _dbContext;
    private readonly TimeProvider _timeProvider;
    private readonly IOptions<AnswerTimeoutOptions> _options;

    public EditAnswerHandler(AppDbContext dbContext, TimeProvider timeProvider, IOptions<AnswerTimeoutOptions> options)
    {
        _dbContext = dbContext;
        _timeProvider = timeProvider;
        _options = options;
    }
    
    public async Task<ErrorOr<AnswerUpdateResponse>> Handle(EditAnswerCommand request, CancellationToken cancellationToken)
    {
       var answer = await _dbContext.Answers
           .SingleOrDefaultAsync(x => x.Id == request.AnswerId, cancellationToken);

       if (answer is null)
       {
           return Error.NotFound("AnswerNotFound", "Answer not found");
       }

       if (answer.AuthorId != request.UserId)
       {
           return Error.Forbidden("Not author", "Not author");
       }
       
        var currentTime = _timeProvider.GetUtcNow().UtcDateTime;
       
       if (currentTime - answer.CreatedAt > TimeSpan.FromMinutes(_options.Value.EditAnswerInMinutes))
       {
           return Error.Conflict("Time expired", "time for update expired");
       }
       
       answer.Content = request.Content;
       answer.UpdatedAt = currentTime;
       
       _dbContext.Answers.Update(answer);
       await _dbContext.SaveChangesAsync(cancellationToken);

       return answer.MapToUpdateResponse();
    }
}