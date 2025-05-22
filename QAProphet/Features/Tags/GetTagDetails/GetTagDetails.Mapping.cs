using QAProphet.Domain;
using QAProphet.Features.Questions.GetQuestions;

namespace QAProphet.Features.Tags.GetTagDetails;

internal static class GetTagDetailsMappingExtensions
{
    public static GetTagDetailsResponse MapToDetailsResponse(this Tag tag, int questionsCount)
        => new(
            tag.Id,
            tag.Title,
            tag.Description,
            questionsCount,
            tag.Questions.Select(q => q.Question.MapToResponse()).ToList());
}