namespace QAProphet.Domain;

public class Answer : BaseEntity
{
    public required string Content { get; set; }
    public Guid AuthorId { get; init; }
    public required string AuthorName { get; set; }
    public Guid QuestionId { get; init; }
    public Question Question { get; init; } = null!;
    public int Likes { get; set; }
    public bool IsBest { get; set; } 
    public DateTime? UpdatedAt { get; set; }
    public ICollection<AnswerComment> Comments { get; set; } = [];
    public ICollection<AnswerLike> AnswerLikes { get; set; } = [];
    public ICollection<AnswerComplaint> Complaints { get; set; } = [];
}