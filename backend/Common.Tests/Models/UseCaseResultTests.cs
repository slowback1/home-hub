using Common.Models;

namespace Common.Tests.Models;

[TestFixture]
public class UseCaseResultTests
{
    [Test]
    public void Success_ShouldSetStatusToSuccess_AndSetResult()
    {
        var expected = 42;
        var result = UseCaseResult<int>.Success(expected);
        Assert.That(result.Status, Is.EqualTo(UseCaseStatus.Success));
        Assert.That(result.Result, Is.EqualTo(expected));
        Assert.That(result.ErrorMessage, Is.Null);
        Assert.That(result.Exception, Is.Null);
    }

    [Test]
    public void Success_WithNoResult_ShouldSetStatusToSuccess_AndResultToDefault()
    {
        var result = UseCaseResult<string>.Success();
        Assert.That(result.Status, Is.EqualTo(UseCaseStatus.Success));
        Assert.That(result.Result, Is.Null);
        Assert.That(result.ErrorMessage, Is.Null);
        Assert.That(result.Exception, Is.Null);
    }

    [Test]
    public void Failure_ShouldSetStatusToFailure_AndSetErrorMessageAndException()
    {
        var errorMessage = "Something went wrong";
        var exception = new InvalidOperationException("fail");
        var result = UseCaseResult<object>.Failure(errorMessage, exception);
        Assert.That(result.Status, Is.EqualTo(UseCaseStatus.Failure));
        Assert.That(result.ErrorMessage, Is.EqualTo(errorMessage));
        Assert.That(result.Exception, Is.EqualTo(exception));
        Assert.That(result.Result, Is.Null);
    }

    [Test]
    public void Failure_WithNoArguments_ShouldSetStatusToFailure_AndNullFields()
    {
        var result = UseCaseResult<int>.Failure();
        Assert.That(result.Status, Is.EqualTo(UseCaseStatus.Failure));
        Assert.That(result.ErrorMessage, Is.Null);
        Assert.That(result.Exception, Is.Null);
    }
}