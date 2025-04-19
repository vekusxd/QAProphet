namespace QAProphet.Features.Shared.Responses;

public record CommentResponse(
    Guid Id,
    string Content,
    Guid UserId,
    string UserName,
    DateTime CreatedAt);
    
