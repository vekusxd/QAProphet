namespace QAProphet.Domain;

public class Tag : BaseEntity
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public ICollection<QuestionTags> Questions { get; set; } = [];
    public ICollection<TagSubscribe> Subscribers { get; set; } = [];
}