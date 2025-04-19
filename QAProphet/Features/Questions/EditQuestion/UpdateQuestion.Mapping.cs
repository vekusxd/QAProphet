using QAProphet.Features.Questions.AskQuestion;

namespace QAProphet.Features.Questions.EditQuestion;

internal static class UpdateQuestionMappingExtensions
{
    public static UpdateQuestionCommand MapToUpdateCommand(
        this AskQuestionRequest request, 
        Guid questionId,
        Guid userId)
        => new(questionId, request.Title, request.Content, userId, request.Tags);
}