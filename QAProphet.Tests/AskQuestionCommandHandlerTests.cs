using ErrorOr;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Time.Testing;
using Moq;
using QAProphet.Data.ElasticSearch;
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


        var httpContext = new Mock<HttpContext>();
        var linkGenerator = new Mock<LinkGenerator>();

        linkGenerator.Setup(l => l.GetPathByAddress(
                httpContext.Object,
                It.IsAny<RouteValuesAddress>(),
                It.IsAny<RouteValueDictionary>(),
                It.IsAny<RouteValueDictionary>(),
                It.IsAny<PathString?>(),
                It.IsAny<FragmentString>(),
                It.IsAny<LinkOptions>()))
            .Returns("/");

        var logger = Microsoft.Extensions.Logging.Abstractions.NullLogger<AskQuestionHandler>.Instance;

        var searchService = new Mock<ISearchService>();
        searchService.Setup(s => s.IndexEntry(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        var handler = new AskQuestionHandler(dbContext, timeProvider, searchService.Object, linkGenerator.Object,
            logger);

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

        var httpContext = new Mock<HttpContext>();
        var linkGenerator = new Mock<LinkGenerator>();

        linkGenerator.Setup(l => l.GetPathByAddress(
                httpContext.Object,
                It.IsAny<RouteValuesAddress>(),
                It.IsAny<RouteValueDictionary>(),
                It.IsAny<RouteValueDictionary>(),
                It.IsAny<PathString?>(),
                It.IsAny<FragmentString>(),
                It.IsAny<LinkOptions>()))
            .Returns("/");

        var searchService = new Mock<ISearchService>();
        searchService.Setup(s => s.IndexEntry(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        var logger = Microsoft.Extensions.Logging.Abstractions.NullLogger<AskQuestionHandler>.Instance;

        var handler = new AskQuestionHandler(dbContext, timeProvider, searchService.Object, linkGenerator.Object,
            logger);

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