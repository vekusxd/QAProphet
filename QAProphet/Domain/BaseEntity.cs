namespace QAProphet.Domain;

public abstract class BaseEntity
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public bool IsDeleted { get; set; } 
    public DateTime CreatedAt { get; init; } 
}