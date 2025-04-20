using FluentValidation;
using QAProphet.Extensions;

namespace QAProphet.Features.Questions.AskQuestion;

public class AskQuestionRequestValidator : AbstractValidator<AskQuestionRequest>
{
    public AskQuestionRequestValidator()
    {
        RuleFor(q => q.Title)
            .NotEmpty()
            .WithMessage("Название обязательно")
            .NotNull()
            .WithMessage("Название обязательно")
            .MaximumLength(96)
            .WithMessage("Максимальная длина названия 96 символов");
        
        RuleFor(q => q.Content)
            .NotEmpty()
            .WithMessage("Описание обязательно")
            .NotNull()
            .WithMessage("Описание обязательно");
        
        RuleFor(q => q.Tags)
            .Must(tags => tags.Count != 0)
            .WithMessage("Выберите хотя бы один тег")
            .Must(tags => tags.Count <= 5)
            .WithMessage("Максимальное количество тегов: 5")
            .Must(t => t.TrueForAll(s => s.IsGuid()))
            .WithMessage("Теги должны быть в виде UUID");
    }
}