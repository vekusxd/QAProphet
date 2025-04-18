namespace QAProphet.Domain;

public class Tag : BaseEntity
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public int QuestionsCount { get; set; } = 0;
    public ICollection<QuestionTags> Questions { get; set; } = [];
}