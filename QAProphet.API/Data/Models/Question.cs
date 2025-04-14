namespace QAProphet.API.Data.Models;

public class Question : BaseEntity
{
    public required string Title { get; set; }
    public required string Content { get; set; }
    public Guid QuestionerId { get; init; }
    public ICollection<Answer> Answers { get; set; } = [];
    public ICollection<QuestionComment> Comments { get; set; } = [];
    public ICollection<QuestionTags> Tags { get; set; } = [];
}