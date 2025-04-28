namespace QAProphet.Domain;

public class TagSubscribe : BaseEntity
{
    public required Guid UserId { get; init; }
    public required Guid TagId { get; init; }
    public Tag Tag { get; init; } = null!;
}