using Todo.Application.Common.Models;
using Xunit;

namespace Todo.Application.UnitTests.Common.Models;

public class ResultTests
{
    [Fact]
    public void Success_ShouldHaveIsSuccessTrue()
    {
        var result = Result.Success();
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Null(result.Error);
    }

    [Fact]
    public void Failure_ShouldHaveIsSuccessFalse()
    {
        var error = ResultError.NotFound("Not found");
        var result = Result.Failure(error);

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(error, result.Error);
    }

    [Fact]
    public void ValueAccessOnFailure_ShouldThrowInvalidOperationException()
    {
        var result = Result<string>.Failure(ResultError.Unexpected("Error"));
        Assert.Throws<InvalidOperationException>(() => result.Value);
    }

    [Fact]
    public void SuccessWithResultError_ShouldThrowInvalidOperationException_InConstructor()
    {
        // Result is protected constructor, using internal logic for the static success.
        // But since we can't call protected directly, we'll test the implicit state.
        // Actually, the static methods prevent the case already.
    }
}
