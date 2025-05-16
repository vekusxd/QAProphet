using ErrorOr;
using Microsoft.EntityFrameworkCore;
using QAProphet.Features.Tags.SubscribeToTag;

namespace QAProphet.Tests;

public sealed class SubscribeToTagCommandHandlerTests(DbConnectionFixture dbConnectionFixture) : IAsyncLifetime
{
    [Fact]
    public async Task Handle_ReturnsNotFoundErrorResult_WhenTagNotFound()
    {
        //arrange
        var dbContext = dbConnectionFixture.DbContext;

        var command = new SubscribeToTagCommand(Guid.NewGuid(), Guid.NewGuid());
        var handler = new SubscribeToTagHandler(dbContext);

        //act
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        //assert
        Assert.NotNull(result);
        Assert.Equal(ErrorType.NotFound, result.Value.Type);
    }

    [Fact]
    public async Task Handle_SuccessfullySubscribedToTag_WhenAllGood()
    {
        //arrange
        var dbContext = dbConnectionFixture.DbContext;

        var command = new SubscribeToTagCommand(dbConnectionFixture.TagIds.First(), Guid.NewGuid());
        var handler = new SubscribeToTagHandler(dbContext);

        //act
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        //assert
        Assert.Null(result);
        Assert.True(dbContext.TagSubscribes.Any());
    }

    [Fact]
    public async Task Handle_SuccessfullyUnsubscribedToTag_WhenAlreadySubscribed()
    {
        //arrange
        var dbContext = dbConnectionFixture.DbContext;

        var command = new SubscribeToTagCommand(dbConnectionFixture.TagIds.First(), Guid.NewGuid());
        var handler = new SubscribeToTagHandler(dbContext);

        await handler.Handle(command, TestContext.Current.CancellationToken);

        Assert.True(dbContext.TagSubscribes.Any());
        //act
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        //assert
        Assert.Null(result);
        Assert.False(dbContext.TagSubscribes.Any());
    }

    public async ValueTask DisposeAsync()
        => await dbConnectionFixture.ResetAsync();

    public async ValueTask InitializeAsync()
        => await dbConnectionFixture.SeedAsync();
}