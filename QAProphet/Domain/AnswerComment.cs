namespace QAProphet.Domain;

public class AnswerComment : BaseComment
{
    public required Guid AnswerId { get; init; }
    public Answer Answer { get; init; } = null!;
}