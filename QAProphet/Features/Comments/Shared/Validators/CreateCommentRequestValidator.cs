using FluentValidation;
using QAProphet.Features.Comments.QuestionComments.CreateQuestionComment;
using QAProphet.Features.Comments.Shared.Requests;

namespace QAProphet.Features.Comments.Shared.Validators;

public class CreateCommentRequestValidator : AbstractValidator<CommentRequest>
{
    public CreateCommentRequestValidator()
    {
        RuleFor(c => c.Content)
            .NotNull()
            .WithMessage("Содержание обязательно")
            .NotEmpty()
            .WithMessage("Содержание обязательно");
    }
}