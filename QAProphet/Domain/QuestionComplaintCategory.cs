namespace QAProphet.Domain;

public class QuestionComplaintCategory : BaseEntity
{
    public required string Title { get; set; }
    public ICollection<QuestionComplaint> Complaints { get; set; } = [];
}