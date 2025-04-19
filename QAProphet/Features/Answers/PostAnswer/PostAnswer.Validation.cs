using FluentValidation;

namespace QAProphet.Features.Answers.PostAnswer;

public class PostAnswerRequestValidator : AbstractValidator<PostAnswerRequest>
{
    public PostAnswerRequestValidator()
    {
        RuleFor(r => r.Content)
            .NotEmpty()
            .NotNull();
        
        RuleFor(r => r.QuestionId)
            .NotEmpty()
            .NotNull()
            .Must(BeValidGuid);
    }

    private static bool BeValidGuid(string id)
    {
        return Guid.TryParse(id, out var guid);
    }
}