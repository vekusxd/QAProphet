using ErrorOr;
using QAProphet.Domain;
using QAProphet.Features.Questions.DeleteQuestion;

namespace QAProphet.Tests;

public sealed class DeleteQuestionCommandHandlerTests : IDisposable
{
    private readonly DbContextWrapper _dbContextWrapper;

    public DeleteQuestionCommandHandlerTests()
    {
        _dbContextWrapper = new DbContextWrapper();
    }

    [Fact]
    public async Task Handle_ReturnsNotFoundErrorResult_WhenQuestionDoesNotExist()
    {
        //arrange
        var command = new DeleteQuestionCommand(Guid.NewGuid(), Guid.NewGuid());

        await using var dbContext = _dbContextWrapper.DbContext;

        var handler = new DeleteQuestionHandler(dbContext);

        //act
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        //assert
        Assert.True(result.IsError);
        Assert.Equal(ErrorType.NotFound, result.FirstError.Type);
    }

    [Fact]
    public async Task Handle_ReturnsForbiddenErrorResult_WhenRequestNotByAuthor()
    {
        //arrange
        var authorId = Guid.NewGuid();
        var questionId = Guid.NewGuid();

        await using var dbContext = _dbContextWrapper.DbContext;

        var question = new Question
        {
            Id = questionId,
            AuthorName = "Author",
            QuestionerId = authorId,
            Title = "Title",
            Content = "Content",
            CreatedAt = DateTime.UtcNow,
        };

        dbContext.Questions.Add(question);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var command = new DeleteQuestionCommand(questionId, Guid.NewGuid());
        var handler = new DeleteQuestionHandler(dbContext);

        //act
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        //assert
        Assert.True(result.IsError);
        Assert.Equal(ErrorType.Forbidden, result.FirstError.Type);
    }

    [Fact]
    public async Task Handle_ReturnsConflictErrorResult_WhenDeleteTimeExpired()
    {
        //arrange
        var authorId = Guid.NewGuid();
        var questionId = Guid.NewGuid();

        await using var dbContext = _dbContextWrapper.DbContext;

        var question = new Question
        {
            Id = questionId,
            AuthorName = "Author",
            QuestionerId = authorId,
            Title = "Title",
            Content = "Content",
            CreatedAt = DateTime.UtcNow - TimeSpan.FromHours(2),
        };

        dbContext.Questions.Add(question);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var command = new DeleteQuestionCommand(questionId, authorId);
        var handler = new DeleteQuestionHandler(dbContext);

        //act
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        //assert
        Assert.True(result.IsError);
        Assert.Equal(ErrorType.Conflict, result.FirstError.Type);
    }

    [Fact]
    public async Task Handle_ReturnsTrue_WhenQuestionSuccessfullyDeleted()
    {
        //arrange
        var authorId = Guid.NewGuid();
        var questionId = Guid.NewGuid();

        await using var dbContext = _dbContextWrapper.DbContext;

        var question = new Question
        {
            Id = questionId,
            AuthorName = "Author",
            QuestionerId = authorId,
            Title = "Title",
            Content = "Content",
            CreatedAt = DateTime.UtcNow,
        };

        dbContext.Questions.Add(question);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var command = new DeleteQuestionCommand(questionId, authorId);
        var handler = new DeleteQuestionHandler(dbContext);

        //act
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        //assert
        Assert.False(result.IsError);
        Assert.Empty(result.ErrorsOrEmptyList);
        Assert.True(result.Value);
    }

    public void Dispose()
    {
        _dbContextWrapper.Dispose();
    }
}