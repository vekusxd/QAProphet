namespace QAProphet.Domain;

public class AnswerComplaint : BaseEntity
{
    public required Guid UserId { get; init; }
    public required Guid AnswerId { get; init; }
    public Answer Answer { get; init; } = null!;
    public required Guid CategoryId { get; init; }
    public AnswerComplaintCategory Category { get; init; } = null!;
}