namespace QAProphet.Domain;

public class AnswerComplaintCategory : BaseEntity
{
    public required string Title { get; set; }
    public ICollection<AnswerComplaint> Complaints { get; set; } = [];
}