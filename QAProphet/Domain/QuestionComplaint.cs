namespace QAProphet.Domain;

public class QuestionComplaint : BaseEntity
{
    public required Guid UserId { get; init; }
    public required Guid QuestionId { get; init; }
    public Question Question { get; init; } = null!;
}