using FluentValidation;

namespace QAProphet.Features.Questions.CreateQuestion;

public class CreateQuestionRequestValidator : AbstractValidator<CreateQuestionRequest>
{
    public CreateQuestionRequestValidator()
    {
        RuleFor(q => q.Title).NotEmpty().NotNull().MaximumLength(96);
        RuleFor(q => q.Content).NotEmpty().NotNull();
        RuleFor(q => q.Tags).Must(tags => tags.Count <= 5);
    }
}