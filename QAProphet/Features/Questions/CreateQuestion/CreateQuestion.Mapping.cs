using QAProphet.Domain;

namespace QAProphet.Features.Questions.CreateQuestion;

internal static class CreateQuestionMappingExtensions
{
    public static CreateQuestionCommand MapToCommand(this CreateQuestionRequest request, string userName, string userId)
        => new (request.Title, request.Content, userId, userName);

    public static QuestionResponse MapToResponse(this Question question)
        => new (question.Id, question.Title, question.Content, question.CreatedAt, question.AuthorName, question.QuestionerId.ToString());
}