namespace QAProphet.API.Data.Models;

public class AnswerComment : BaseComment
{
    public required Guid AnswerId { get; init; }
    public Answer Answer { get; init; } = null!;
}