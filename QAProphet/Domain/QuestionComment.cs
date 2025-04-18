namespace QAProphet.Domain;

public class QuestionComment : BaseComment
{
    public required Guid QuestionId { get; init; }
    public Question Question { get; init; } = null!;
}