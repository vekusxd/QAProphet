using FluentValidation;

namespace QAProphet.Features.Comments.QuestionComments.CreateQuestionComment;

public class CreateQuestionCommentRequestValidator : AbstractValidator<CreateQuestionCommentRequest>
{
    public CreateQuestionCommentRequestValidator()
    {
        RuleFor(c => c.Content)
            .NotNull()
            .WithMessage("Содержание обязательно")
            .NotEmpty()
            .WithMessage("Содержание обязательно");
    }
}