namespace QAProphet.Domain;

public abstract class BaseEntity
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}