namespace QAProphet.Domain;

public class QuestionSubscribe : BaseEntity
{
    public required Guid UserId { get; init; }
    public required Guid QuestionId { get; init; }
    public Question Question { get; init; } = null!;
}