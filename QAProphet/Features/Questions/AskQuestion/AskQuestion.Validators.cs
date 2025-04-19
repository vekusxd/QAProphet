using FluentValidation;

namespace QAProphet.Features.Questions.AskQuestion;

public class AskQuestionRequestValidator : AbstractValidator<AskQuestionRequest>
{
    public AskQuestionRequestValidator()
    {
        RuleFor(q => q.Title).NotEmpty().NotNull().MaximumLength(96);
        RuleFor(q => q.Content).NotEmpty().NotNull();
        RuleFor(q => q.Tags).Must(tags => tags.Count <= 5);
    }
}