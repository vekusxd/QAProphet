using QAProphet.Domain;
using QAProphet.Features.Answers.Shared.Responses;

namespace QAProphet.Features.Answers.Shared.Mapping;

internal static  class AnswerMappingExtensions
{
    public static AnswerUpdateResponse MapToUpdateResponse(this Answer answer)
        => new(answer.Id, answer.AuthorId, answer.AuthorName, answer.Content, answer.Likes, answer.IsBest);
}