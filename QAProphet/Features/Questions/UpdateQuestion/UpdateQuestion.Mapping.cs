using QAProphet.Features.Questions.CreateQuestion;

namespace QAProphet.Features.Questions.UpdateQuestion;

internal static class UpdateQuestionMappingExtensions
{
    public static UpdateQuestionCommand MapToUpdateCommand(
        this CreateQuestionRequest request, 
        Guid questionId,
        Guid userId)
        => new(questionId, request.Title, request.Content, userId, request.Tags);
}