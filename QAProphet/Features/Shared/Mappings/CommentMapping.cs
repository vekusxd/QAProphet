using QAProphet.Domain;
using QAProphet.Features.Shared.Responses;

namespace QAProphet.Features.Shared.Mappings;

internal static  class CommentMapping
{
    public static CommentResponse MapToResponse(this BaseComment comment) 
        => new(comment.Id, comment.Content, comment.AuthorId, comment.AuthorName, comment.CreatedAt);
    
    
    // public static CommentResponse MapToResponse(this AnswerComment comment) 
    //     => new(comment.Id, comment.Content, comment.AuthorId, comment.AuthorName, comment.CreatedAt);
}