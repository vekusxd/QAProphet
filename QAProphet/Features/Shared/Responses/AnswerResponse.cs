﻿namespace QAProphet.Features.Shared.Responses;

public record AnswerResponse(
    Guid Id,
    string Content, 
    DateTime Created,
    Guid AuthorId,
    string AuthorName,
    bool IsSolution,
    int Likes);
