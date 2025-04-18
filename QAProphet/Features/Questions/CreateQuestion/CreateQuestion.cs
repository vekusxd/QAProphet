using System.Security.Claims;
using Carter;
using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using QAProphet.Data;
using QAProphet.Domain;

namespace QAProphet.Features.Questions.CreateQuestion;

public sealed record CreateQuestionRequest(
    string Title,
    string Content);

public sealed record QuestionResponse(
    Guid Id,
    string Title,
    string Content,
    DateTime CreatedAt,
    string AuthorName,
    string AuthorId);

public class CreateQuestion : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/questions", Handler)
            .RequireAuthorization()
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces<QuestionResponse>();
    }

    private static async Task<IResult> Handler(
        [FromBody] CreateQuestionRequest request,
        IValidator<CreateQuestionRequest> validator,
        IMediator mediator,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var username = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;
        var userId = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "jti")?.Value.Split(':')[1];

        var command = request.MapToCommand(username!, userId!);
        
        var response = await mediator.Send(command, cancellationToken);

        return TypedResults.Ok(response.Value);
    }
}

internal sealed record CreateQuestionCommand(
    string Title, 
    string Description,
    string AuthorId,
    string AuthorName)
    : IRequest<ErrorOr<QuestionResponse>>;

internal sealed class CreateQuestionCommandHandler : IRequestHandler<CreateQuestionCommand, ErrorOr<QuestionResponse>>
{
    private readonly AppDbContext _dbContext;

    public CreateQuestionCommandHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<ErrorOr<QuestionResponse>> Handle(
        CreateQuestionCommand request, 

        CancellationToken cancellationToken)
    {
        var question = new Question
        {
            Title = request.Title,
            CreatedAt = DateTime.UtcNow,
            Content = request.Description,
            AuthorName = request.AuthorName,
            QuestionerId = Guid.Parse(request.AuthorId)
        };
        
        await _dbContext.Questions.AddAsync(question, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return question.MapToResponse();
    }
}