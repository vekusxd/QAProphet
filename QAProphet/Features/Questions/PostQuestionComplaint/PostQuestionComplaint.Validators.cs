using FluentValidation;
using QAProphet.Extensions;

namespace QAProphet.Features.Questions.PostQuestionComplaint;

public class PostQuestionComplaintRequestValidator : AbstractValidator<PostQuestionComplaintRequest>
{
    public PostQuestionComplaintRequestValidator()
    {
        RuleFor(p => p.ComplaintCategoryId)
            .NotEmpty()
            .WithMessage("Категория жалобы обязательна")
            .NotNull()
            .WithMessage("Категория жалобы обязательна")
            .Must(s => s.IsGuid())
            .WithMessage("Категория должна быть в виде UUID");

        RuleFor(p => p.QuestionId)
            .NotEmpty()
            .WithMessage("Ответ обязателен обязателен")
            .NotNull()
            .WithMessage("Ответ обязателен")
            .Must(s => s.IsGuid())
            .WithMessage("Вопрос должен быть в виде UUID"); 
    }
}