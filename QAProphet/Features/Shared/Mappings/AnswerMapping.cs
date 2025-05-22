using QAProphet.Domain;
using QAProphet.Features.Shared.Responses;

namespace QAProphet.Features.Shared.Mappings;

internal static class AnswerMappingExtensions
{
    public static AnswerResponse MapToAnswerResponse(this Answer answer)
     => new(answer.Id, answer.Content, answer.CreatedAt,answer.AuthorId, answer.AuthorName, answer.IsBest, answer.Likes);
}