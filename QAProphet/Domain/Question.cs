namespace QAProphet.Domain;

public class Question : BaseEntity
{
    public required string Title { get; set; }
    public required string Content { get; set; }
    public DateTime? UpdateTime { get; set; }
    public required  Guid QuestionerId { get; init; }
    public required string AuthorName { get; set; }
    public ICollection<Answer> Answers { get; set; } = [];
    public ICollection<QuestionComment> Comments { get; set; } = [];
    public ICollection<QuestionTags> Tags { get; set; } = [];
    public ICollection<QuestionSubscribe> Subscribers { get; set; } = [];
    public ICollection<QuestionComplaint> Complaints { get; set; } = [];
}