namespace QAProphet.API.Data.Models;

public class QuestionComment : BaseComment
{
    public required Guid QuestionId { get; init; }
    public Question Question { get; init; } = null!;
}