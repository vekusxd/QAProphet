namespace QAProphet.Options;

public class AnswerTimeoutOptions
{
    public const string Section = "AnswerTimeout";
    public int EditAnswerInMinutes { get; set; }
    public int DeleteAnswerInMinutes { get; set; }
    public int EditCommentInMinutes { get; set; }
    public int DeleteCommentInMinutes { get; set; }
}