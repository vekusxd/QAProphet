using ErrorOr;
using QAProphet.Features.Questions.AskQuestion;

namespace QAProphet.Tests;

public sealed class AskQuestionCommandHandlerTests : IDisposable
{
    private readonly DbContextWrapper _dbContextWrapper;
    
    public AskQuestionCommandHandlerTests()
    {
        _dbContextWrapper = new DbContextWrapper();
    }
    
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

        await using var dbContext = _dbContextWrapper.DbContext;
        
        var handler = new AskQuestionHandler(dbContext);
        
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
        await using var dbContext = _dbContextWrapper.DbContext;
        
        var command = new AskQuestionCommand(
            "Question title",
            "Question description", 
            Guid.NewGuid().ToString(),
            "User", 
            [_dbContextWrapper.TagIds.First(), _dbContextWrapper.TagIds.Last()]);
        
        var handler = new AskQuestionHandler(dbContext);
        
        //act
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);
        
        //assert
        Assert.False(result.IsError);
        Assert.Empty(result.ErrorsOrEmptyList);
        Assert.NotNull(result.Value);
    }

    public void Dispose()
    {
        _dbContextWrapper.Dispose();
    }
}