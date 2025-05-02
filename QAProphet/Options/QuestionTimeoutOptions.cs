namespace QAProphet.Options;

public class QuestionTimeoutOptions
{
    public const string Section = "QuestionTimeout";
    public int EditQuestionInMinutes { get; set; }
    public int DeleteQuestionInMinutes { get; set; }
    public int EditCommentInMinutes { get; set; }
    public int DeleteCommentInMinutes { get; set; }
}