namespace QAProphet.API.Data.Models;

public abstract class BaseComment : BaseEntity
{
    public required string Content { get; set; }
    public Guid AuthorId { get; init; }
    public int Likes { get; set; } = 0;
    public int Dislikes { get; set; } = 0;
}