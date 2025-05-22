using QAProphet.Features.Shared.Responses;

namespace QAProphet.Features.Comments.Shared.Responses;

public record PaginatedCommentResponse(int Total, ICollection<CommentResponse> Comments);
