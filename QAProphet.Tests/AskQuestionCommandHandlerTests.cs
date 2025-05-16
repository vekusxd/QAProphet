using ErrorOr;
using Microsoft.Extensions.Time.Testing;
using QAProphet.Features.Questions.AskQuestion;

namespace QAProphet.Tests;

public sealed class AskQuestionCommandHandlerTests(DbConnectionFixture dbConnectionFixture) : IAsyncLifetime
{
    [Fact]
    public async Task Handle_ReturnsValidationErrorResult_WhenTagNotFound()
    {
        //arrange   
        var command = new AskQuestionCommand(
            "Question title",
            "Question description",
            Guid.NewGuid().ToString(),
            "User",
            [Guid.NewGuid(), Guid.NewGuid()]);

        var dbContext = dbConnectionFixture.DbContext;

        var timeProvider = new FakeTimeProvider();

        var handler = new AskQuestionHandler(dbContext, timeProvider);

        //act
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        //assert
        Assert.True(result.IsError);
        Assert.Equal(ErrorType.Validation, result.FirstError.Type);
    }

    [Fact]
    public async Task Handle_ReturnsAskQuestionResponse_WhenAllGood()
    {
        //arrange
        var dbContext = dbConnectionFixture.DbContext;
        var timeProvider = new FakeTimeProvider();

        var command = new AskQuestionCommand(
            "Question title",
            "Question description",
            Guid.NewGuid().ToString(),
            "User",
            [dbConnectionFixture.TagIds.First(), dbConnectionFixture.TagIds.Last()]);

        var handler = new AskQuestionHandler(dbContext, timeProvider);

        //act
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        //assert
        Assert.False(result.IsError);
        Assert.Empty(result.ErrorsOrEmptyList);
        Assert.NotNull(result.Value);
    }

    public async ValueTask DisposeAsync()
        => await dbConnectionFixture.ResetAsync();

    public async ValueTask InitializeAsync()
        => await dbConnectionFixture.SeedAsync();
}