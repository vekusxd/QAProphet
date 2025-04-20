using FluentValidation;
using QAProphet.Extensions;

namespace QAProphet.Features.Answers.PostAnswer;

public class PostAnswerRequestValidator : AbstractValidator<PostAnswerRequest>
{
    public PostAnswerRequestValidator()
    {
        RuleFor(r => r.Content)
            .NotEmpty()
            .WithMessage("Содержание обязательно")
            .NotNull()
            .WithMessage("Содержание обязательно");
        
        RuleFor(r => r.QuestionId)
            .NotEmpty()
            .WithMessage("ID сообщения обязательно")
            .NotNull()
            .WithMessage("ID сообщения обязательно")
            .Must(s => s.IsGuid())
            .WithMessage("Сообщение должно быть в виде UUID");
    }
}