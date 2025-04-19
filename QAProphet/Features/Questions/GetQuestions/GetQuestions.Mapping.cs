using QAProphet.Domain;
using QAProphet.Features.Shared.Responses;

namespace QAProphet.Features.Questions.GetQuestions;

internal static class GetQuestionsMappingExtensions
{
    public static QuestionResponse MapToResponse(this Question question)
        => new(question.Id, question.Title, question.CreatedAt, question.AuthorName,
            question.Tags.Select(t => new TagResponse(t.TagId, t.Tag.Title)).ToList());
}