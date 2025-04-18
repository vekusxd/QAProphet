namespace QAProphet.Domain;

public class QuestionTags : BaseEntity
{
    public required Guid QuestionId { get; init; }
    public Question Question { get; init; } = null!;
    public required Guid TagId { get; init; }
    public Tag Tag { get; init; } = null!;
}