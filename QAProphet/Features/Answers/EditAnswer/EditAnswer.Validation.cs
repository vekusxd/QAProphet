using FluentValidation;

namespace QAProphet.Features.Answers.EditAnswer;

public class EditAnswerRequestValidator : AbstractValidator<EditAnswerRequest>
{
    public EditAnswerRequestValidator()
    {
        RuleFor(r => r.Content)
            .NotEmpty()
            .WithMessage("Описание обязательно")
            .NotNull()
            .WithMessage("Описание обязательно");
    }
}