namespace QAProphet.Domain;

public class AnswerComplaint : BaseEntity
{
    public required Guid UserId { get; init; }
    public required Guid AnswerId { get; init; }
    public Answer Answer { get; init; } = null!;
}