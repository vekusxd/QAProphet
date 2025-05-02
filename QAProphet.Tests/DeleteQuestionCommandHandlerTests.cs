using ErrorOr;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;
using QAProphet.Domain;
using QAProphet.Features.Questions.DeleteQuestion;
using QAProphet.Options;

namespace QAProphet.Tests;

public sealed class DeleteQuestionCommandHandlerTests : IDisposable
{
    private readonly DbContextWrapper _dbContextWrapper;
    private readonly IOptions<QuestionTimeoutOptions> _options;

    public DeleteQuestionCommandHandlerTests()
    {
        _dbContextWrapper = new DbContextWrapper();
        _options = Microsoft.Extensions.Options.Options.Create(new QuestionTimeoutOptions
        {
            DeleteQuestionInMinutes = 60
        });
    }

    [Fact]
    public async Task Handle_ReturnsNotFoundErrorResult_WhenQuestionDoesNotExist()
    {
        //arrange
        var command = new DeleteQuestionCommand(Guid.NewGuid(), Guid.NewGuid());

        await using var dbContext = _dbContextWrapper.DbContext;
        var timeProvider = new FakeTimeProvider();

        var handler = new DeleteQuestionHandler(dbContext, timeProvider, _options);

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
        var timeProvider = new FakeTimeProvider();

        var question = new Question
        {
            Id = questionId,
            AuthorName = "Author",
            QuestionerId = authorId,
            Title = "Title",
            Content = "Content",
            CreatedAt = timeProvider.GetUtcNow().UtcDateTime,
        };

        dbContext.Questions.Add(question);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var command = new DeleteQuestionCommand(questionId, Guid.NewGuid());
        var handler = new DeleteQuestionHandler(dbContext, timeProvider, _options);

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
        var timeProvider = new FakeTimeProvider();
        
        var question = new Question
        {
            Id = questionId,
            AuthorName = "Author",
            QuestionerId = authorId,
            Title = "Title",
            Content = "Content",
            CreatedAt = timeProvider.GetUtcNow().UtcDateTime,
        };
        
        timeProvider.SetUtcNow(timeProvider.GetUtcNow() + TimeSpan.FromHours(3));

        dbContext.Questions.Add(question);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var command = new DeleteQuestionCommand(questionId, authorId);
        var handler = new DeleteQuestionHandler(dbContext, timeProvider, _options);

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
        var timeProvider = new FakeTimeProvider();

        var question = new Question
        {
            Id = questionId,
            AuthorName = "Author",
            QuestionerId = authorId,
            Title = "Title",
            Content = "Content",
            CreatedAt = timeProvider.GetUtcNow().UtcDateTime,
        };

        dbContext.Questions.Add(question);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var command = new DeleteQuestionCommand(questionId, authorId);
        var handler = new DeleteQuestionHandler(dbContext, timeProvider, _options);

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