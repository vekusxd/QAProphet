using FluentValidation;
using QAProphet.Features.Comments.Shared.Requests;

namespace QAProphet.Features.Comments.Shared.Validators;

public class CommentRequestValidator : AbstractValidator<CommentRequest>
{
    public CommentRequestValidator()
    {
        RuleFor(c => c.Content)
            .NotNull()
            .WithMessage("Содержание обязательно")
            .NotEmpty()
            .WithMessage("Содержание обязательно");
    }
}