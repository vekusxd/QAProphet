using QAProphet.Domain;
using QAProphet.Features.Shared.Responses;

namespace QAProphet.Features.Answers.GetAnswerComplaintCategories;

internal static class GetAnswerComplaintCategoriesMappingExtensions
{
    public static List<ComplaintCategoryResponse> MapToResponse(this List<AnswerComplaintCategory> categories)
        => categories.Select(c => new ComplaintCategoryResponse(c.Id, c.Title)).ToList();
}