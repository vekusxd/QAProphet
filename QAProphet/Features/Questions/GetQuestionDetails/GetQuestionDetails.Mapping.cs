using QAProphet.Domain;
using QAProphet.Features.Shared.Mappings;
using QAProphet.Features.Shared.Responses;
using QAProphet.Features.Tags.SearchTags;

namespace QAProphet.Features.Questions.GetQuestionDetails;

internal static class GetQuestionDetailsMappingExtensions
{
    public static QuestionDetailsResponse MapToDetailsResponse(this Question question)
        => new(question.Id, question.Title, question.Content, question.QuestionerId, question.AuthorName,
            question.CreatedAt, question.UpdateTime,
            question.Tags.Select(t => new TagResponse(t.TagId, t.Tag.Title)).ToList(),
            question.Comments.Select(c => c.MapToResponse()).ToList(),
            question.Answers.Select(a => a.MapToAnswerResponse()).ToList());
}