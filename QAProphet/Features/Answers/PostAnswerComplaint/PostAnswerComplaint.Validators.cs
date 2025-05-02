using FluentValidation;
using QAProphet.Extensions;

namespace QAProphet.Features.Answers.PostAnswerComplaint;

public class PostAnswerComplaintRequestValidator : AbstractValidator<PostAnswerComplaintRequest>
{
    public PostAnswerComplaintRequestValidator()
    {
        RuleFor(p => p.ComplaintCategoryId)
            .NotEmpty()
            .WithMessage("Категория жалобы обязательна")
            .NotNull()
            .WithMessage("Категория жалобы обязательна")
            .Must(s => s.IsGuid())
            .WithMessage("Категория должна быть в виде UUID");

        RuleFor(p => p.AnswerId)
            .NotEmpty()
            .WithMessage("Ответ обязателен обязателен")
            .NotNull()
            .WithMessage("Ответ обязателен")
            .Must(s => s.IsGuid())
            .WithMessage("Вопрос должен быть в виде UUID");
        ;
    }
}