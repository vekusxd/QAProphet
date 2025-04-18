using QAProphet.Domain;
using QAProphet.Features.Tags.SearchTags;

namespace QAProphet.Features.Questions.CreateQuestion;

internal static class CreateQuestionMappingExtensions
{
    public static CreateQuestionCommand MapToCommand(this CreateQuestionRequest request, string userName, string userId)
        => new(request.Title, request.Content, userId, userName, request.Tags);

    public static QuestionDetailsResponse MapToDetailsResponse(this Question question, List<Tag> tags)
        => new(question.Id, question.Title, question.Content, question.CreatedAt, question.AuthorName,
            question.QuestionerId.ToString(),
            tags
                .Select(t => t.MapToResponse())
                .ToList());
}