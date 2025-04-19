using QAProphet.Domain;

namespace QAProphet.Features.Questions.AskQuestion;

internal static class CreateQuestionMappingExtensions
{
    public static AskQuestionCommand MapToCommand(this AskQuestionRequest request, string userName, string userId)
        => new(request.Title, request.Content, userId, userName, request.Tags);

    public static AskQuestionResponse MapToAskResponse(this Question question)
        => new(question.Id, question.Title, question.Content, question.CreatedAt, question.AuthorName,
            question.QuestionerId.ToString());

}