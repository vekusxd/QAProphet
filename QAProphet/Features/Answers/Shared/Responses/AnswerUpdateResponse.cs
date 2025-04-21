namespace QAProphet.Features.Answers.Shared.Responses;

public sealed record AnswerUpdateResponse(
    Guid Id,
    Guid AuthorId,
    string AuthorName,
    string Content,
    int Likes,
    bool IsBest);