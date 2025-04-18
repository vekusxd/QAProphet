using QAProphet.Domain;

namespace QAProphet.Features.Questions.GetQuestions;

internal static class GetQuestionsMappingExtensions
{
    public static QuestionResponse MapToResponse(this Question question)
    => new (question.Id, question.Title, question.CreatedAt, question.AuthorName);
}