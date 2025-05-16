using System.Runtime.CompilerServices;
using ErrorOr;
using QAProphet.Domain;
using QAProphet.Features.Questions.SubscribeToQuestion;

namespace QAProphet.Tests;

public sealed class SubscribeToQuestionCommandHandlerTests(DbConnectionFixture dbConnectionFixture) : IAsyncLifetime
{
    [Fact]
    public async Task Handle_ReturnsNotFoundErrorResult_WhenQuestionNotFound()
    {
        //arrange
        var dbContext = dbConnectionFixture.DbContext;

        var command = new SubscribeToQuestionCommand(Guid.NewGuid(), Guid.NewGuid());
        var handler = new SubscribeToQuestionHandler(dbContext);

        //act
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        //assert
        Assert.NotNull(result);
        Assert.Equal(ErrorType.NotFound, result.Value.Type);
    }

    [Fact]
    public async Task Handle_ReturnsNull_WhenAllGood()
    {
        //arrange
        var dbContext = dbConnectionFixture.DbContext;

        var authorId = Guid.NewGuid();

        var question = new Question
        {
            Content = "content",
            AuthorName = "Author",
            QuestionerId = authorId,
            Title = "Title"
        };

        await dbContext.Questions.AddAsync(question, TestContext.Current.CancellationToken);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var command = new SubscribeToQuestionCommand(question.Id, authorId);
        var handler = new SubscribeToQuestionHandler(dbContext);

        //act
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        //assert
        Assert.Null(result);
        Assert.True(dbContext.QuestionSubscribes.Any());
    }

    [Fact]
    public async Task Handle_ReturnsNullAndClearRow_WhenAlreadySubscribed()
    {
        //arrange
        var dbContext = dbConnectionFixture.DbContext;

        var authorId = Guid.NewGuid();

        var question = new Question
        {
            Content = "content",
            AuthorName = "Author",
            QuestionerId = authorId,
            Title = "Title"
        };

        await dbContext.Questions.AddAsync(question, TestContext.Current.CancellationToken);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var command = new SubscribeToQuestionCommand(question.Id, authorId);
        var handler = new SubscribeToQuestionHandler(dbContext);

        await handler.Handle(command, TestContext.Current.CancellationToken);

        Assert.True(dbContext.QuestionSubscribes.Any());

        //act
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        //assert
        Assert.Null(result);
        Assert.False(dbContext.QuestionSubscribes.Any());
    }

    public async ValueTask DisposeAsync()
        => await dbConnectionFixture.ResetAsync();

    public async ValueTask InitializeAsync()
        => await dbConnectionFixture.SeedAsync();
}