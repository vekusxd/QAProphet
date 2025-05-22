namespace QAProphet.Features.Shared.Responses;

public sealed record QuestionResponse(
    Guid Id,
    string Title,
    DateTime CreatedAt,
    string AuthorName, 
    List<TagResponse> Tags);