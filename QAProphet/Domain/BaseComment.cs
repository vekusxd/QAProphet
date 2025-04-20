namespace QAProphet.Domain;

public abstract class BaseComment : BaseEntity
{
    public required string Content { get; set; }
    public required Guid AuthorId { get; init; }
    public DateTime? UpdateTime { get; set; }
    public required string AuthorName { get; init; }
    public int Likes { get; set; } = 0;
    public int Dislikes { get; set; } = 0;
}