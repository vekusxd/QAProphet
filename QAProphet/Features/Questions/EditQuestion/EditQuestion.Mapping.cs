using QAProphet.Features.Questions.AskQuestion;

namespace QAProphet.Features.Questions.EditQuestion;

internal static class EditQuestionMappingExtensions
{
    public static EditQuestionCommand MapToUpdateCommand(
        this AskQuestionRequest request, 
        Guid questionId,
        Guid userId)
        => new(questionId, request.Title, request.Content, userId, request.Tags.Select(Guid.Parse).ToList());
}