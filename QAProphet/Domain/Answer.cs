namespace QAProphet.Domain;

public class Answer : BaseEntity
{
    public required string Content { get; set; }
    public Guid AnswererId { get; init; }
    public Guid QuestionId { get; init; }
    public Question Question { get; init; } = null!;
    public int Likes { get; set; } = 0;
    public int Dislikes { get; set; } = 0;
    public bool IsBest { get; set; } = false;
    public ICollection<AnswerComment> Comments { get; set; } = [];
}