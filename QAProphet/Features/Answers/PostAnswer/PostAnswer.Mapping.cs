using QAProphet.Domain;

namespace QAProphet.Features.Answers.PostAnswer;

internal static class PostAnswerMappingExtensions
{
    public static PostAnswerCommand MapToCommand(this PostAnswerRequest request, Guid authorId, string authorName)
        => new (Guid.Parse(request.QuestionId), authorId, authorName, request.Content);

    public static PostAnswerResponse MapToPostResponse(this Answer answer)
        => new(answer.Id, answer.Content, answer.CreatedAt, answer.AuthorId, answer.AuthorName);
}
