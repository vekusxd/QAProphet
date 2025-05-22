using FluentValidation;

namespace QAProphet.Features.Tags.AddNewTag;

public class AddNewTagRequestValidator : AbstractValidator<AddTagRequest>
{
    public AddNewTagRequestValidator()
    {
        RuleFor(r => r.Title)
            .NotEmpty()
            .WithMessage("Название обязательно")
            .NotNull()
            .WithMessage("Название обязательно")
            .MaximumLength(96)
            .WithMessage("Максимальная длина названия 96 символов");
        
        RuleFor(r => r.Title)
            .NotEmpty()
            .WithMessage("Описание обязательно")
            .NotNull()
            .WithMessage("Описание обязательно");
    }
}