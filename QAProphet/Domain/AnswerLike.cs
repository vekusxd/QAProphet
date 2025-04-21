namespace QAProphet.Domain;

public class AnswerLike : BaseEntity
{
    public required Guid AnswerId { get; init; }
    public Answer Answer { get; init; } = null!;
    public required Guid AuthorId { get; init; }
}