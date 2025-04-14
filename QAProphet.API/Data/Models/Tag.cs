namespace QAProphet.API.Data.Models;

public class Tag : BaseEntity
{
    public required string Title { get; set; }
    public int QuestionsCount { get; set; } = 0;
    public ICollection<QuestionTags> Questions { get; set; } = [];
}