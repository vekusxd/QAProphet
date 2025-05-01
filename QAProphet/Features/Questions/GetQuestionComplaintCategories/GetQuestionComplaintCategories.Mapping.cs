using QAProphet.Domain;
using QAProphet.Features.Questions.GetQuestions;
using QAProphet.Features.Shared.Responses;

namespace QAProphet.Features.Questions.GetQuestionComplaintCategories;

internal static class GetQuestionComplaintCategoriesMappingExtensions
{
    public static List<ComplaintCategoryResponse> MapToResponse(this List<QuestionComplaintCategory> categories)
        => categories.Select(c => new ComplaintCategoryResponse(c.Id, c.Title)).ToList();
}