using QAProphet.Domain;
using QAProphet.Features.Shared.Mappings;
using QAProphet.Features.Shared.Responses;
using QAProphet.Features.Tags.SearchTags;

namespace QAProphet.Features.Questions.GetQuestionDetails;

internal static class GetQuestionDetailsMappingExtensions
{
    public static QuestionDetailsResponse MapToDetailsResponse(this Question question, int commentsCount,int answersCount)
        => new(
            question.Id,
            question.Title,
            question.Content,
            question.QuestionerId,
            question.AuthorName,
            question.CreatedAt,
            question.UpdateTime,
            question.Tags
                .Select(t => new TagResponse(t.TagId, t.Tag.Title))
                .ToList(),
            commentsCount,
            answersCount);
}