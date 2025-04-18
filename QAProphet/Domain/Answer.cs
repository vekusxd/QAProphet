﻿namespace QAProphet.Domain;

public class Answer : BaseEntity
{
    public required string Content { get; set; }
    public Guid AuthorId { get; init; }
    public required string AuthorName { get; set; }
    public Guid QuestionId { get; init; }
    public Question Question { get; init; } = null!;
    public int Likes { get; set; } = 0;
    public int Dislikes { get; set; } = 0;
    public bool IsBest { get; set; } = false;
    public ICollection<AnswerComment> Comments { get; set; } = [];
}