namespace Hogar.Core.Shared.Tests;

public class ResultTests
{
    [Fact]
    public void Success_Should_Return_Successful_Result_With_Data()
    {
        var result = Result<string>.Success("Hello");

        Assert.True(result.Succeded);
        Assert.Equal("Hello", result.MessageDescription);
    }

    [Fact]
    public void Success_Should_Return_Result_With_Expected_Properties()
    {
        // Act
        var result = Result<string>.Success("Hello");

        // Assert
        Assert.True(result.Succeded);
        Assert.NotNull(result.MessageDescription);  // Como no pasaste mensaje, debe ser null
        Assert.Equal("Hello", result.MessageDescription);
        Assert.Null(result.ExceptionInfo);
        Assert.Null(result.Data);
    }


    [Fact]
    public void Failure_ShouldSetProperties()
    {
        var result = Result<string>.Failure("Error Message", 400);
        Assert.False(result.Succeded);
        Assert.Equal("Error Message", result.MessageDescription);
        Assert.Equal(400U, result.StatusCode);
    }

    [Fact]
    public void Failure_WithException_ShouldStoreException()
    {
        var ex = new InvalidOperationException("Error");
        var result = Result<string>.Failure(ex);
        Assert.False(result.Succeded);
        Assert.Equal(ex, result.ExceptionInfo);
    }

    [Fact]
    public async Task SuccessAsync_ShouldReturnSuccessResult()
    {
        var result = await Result<string>.SuccessAsync("Async Success");
        Assert.True(result.Succeded);
        Assert.Equal("Async Success", result.MessageDescription);
    }

    [Fact]
    public async Task FailureAsync_ShouldReturnFailureResult()
    {
        var result = await Result<string>.FailureAsync("Async Failure");
        Assert.False(result.Succeded);
        Assert.Equal("Async Failure", result.MessageDescription);
    }

    [Fact]
    public void Bind_ShouldChainResultsCorrectly()
    {
        var result = Result<int>.Success(5)
            .Bind(value => Result<int>.Success(value * 2));
        Assert.True(result.Succeded);
        Assert.Equal(10, result.Data);
    }

    [Fact]
    public async Task BindAsync_ShouldChainResultsCorrectly()
    {
        var resultOK = await Result<int>.SuccessAsync(5);
            
        var result = await resultOK.BindAsync(async value =>
            {
                await Task.Delay(10);
                return await Result<int>.SuccessAsync(value + 10);
            });

        Assert.True(result.Succeded);
        Assert.Equal(15, result.Data);
    }

    [Fact]
    public void Map_ShouldTransformData()
    {
        var result = Result<int>.Success(7).Map(x => $"Value: {x}");
        Assert.True(result.Succeded);
        Assert.Equal("Value: 7", result.Data);
    }

    [Fact]
    public async Task MapAsync_ShouldTransformDataAsync()
    {
        var resultOK = await Result<int>.SuccessAsync(3);

        var result = await resultOK.MapAsync(async x =>
            {
                await Task.Delay(5);
                return x * 4;
            });
        Assert.True(result.Succeded);
        Assert.Equal(12, result.Data);
    }

    [Fact]
    public void TryCatch_ShouldReturnSuccess()
    {
        var result = Result<int>.TryCatch(() => 100);
        Assert.True(result.Succeded);
        Assert.Equal(100, result.Data);
    }

    [Fact]
    public void TryCatch_ShouldReturnFailureOnException()
    {
        var result = Result<int>.TryCatch(() => throw new Exception("Fail"), "Custom Error");
        Assert.False(result.Succeded);
        Assert.Contains("Custom Error", result.MessageDescription);
    }

    [Fact]
    public async Task TryCatchAsync_ShouldReturnSuccess()
    {
        var result = await Result<int>.TryCatchAsync(async () =>
        {
            await Task.Delay(10);
            return 42;
        });
        Assert.True(result.Succeded);
        Assert.Equal(42, result.Data);
    }

    [Fact]
    public async Task TryCatchAsync_ShouldReturnFailureOnException()
    {
        var result = await Result<int>.TryCatchAsync(async () =>
        {
            await Task.Delay(5);
            throw new InvalidOperationException("Async Fail");
        }, "Async Error");
        Assert.False(result.Succeded);
        Assert.Contains("Async Error", result.MessageDescription);
    }

    [Fact]
    public void Match_ShouldInvokeCorrectAction()
    {
        bool successCalled = false;
        bool failureCalled = false;

        var successResult = Result<int>.Success(99);
        successResult.Match(
            onSuccess: _ => successCalled = true,
            onFailure: _ => failureCalled = true
        );

        Assert.True(successCalled);
        Assert.False(failureCalled);
    }

    [Fact]
    public async Task MatchAsync_ShouldInvokeCorrectActionAsync()
    {
        bool successCalled = false;
        bool failureCalled = false;

        var failureResult = await Result<int>.FailureAsync("Failure Test");
        await failureResult.MatchAsync(
            onSuccess: async _ => { successCalled = true; await Task.CompletedTask; },
            onFailure: async _ => { failureCalled = true; await Task.CompletedTask; }
        );

        Assert.False(successCalled);
        Assert.True(failureCalled);
    }

    [Fact]
    public void Failure_WithErrorDetails_ShouldStoreDetails()
    {
        var errors = new Dictionary<string, string> { { "Field", "Error Detail" } };
        var result = Result<string>.Failure("Validation failed", errors);
        Assert.False(result.Succeded);
        Assert.Equal("Validation failed", result.MessageDescription);
        Assert.NotNull(result.ErrorDetail);
        Assert.Contains("Field", result.ErrorDetail.Keys);
    }

    [Fact]
    public void Properties_Should_Set_And_Get_Correctly()
    {
        var result = new Result<string>
        {
            Succeded = true,
            TraceId = "trace-123",
            MessageDescription = "Test message",
            StatusCode = 200,
            TimeStamp = DateTime.UtcNow,
            ErrorDetail = new Dictionary<string, string> { { "ErrorKey", "ErrorValue" } },
            Data = "DataValue",
            ExceptionInfo = new InvalidOperationException("Test Exception"),
            UrlPathDetails = "/test/path"
        };

        Assert.True(result.Succeded);
        Assert.Equal("trace-123", result.TraceId);
        Assert.Equal("Test message", result.MessageDescription);
        Assert.Equal(200u, result.StatusCode);
        Assert.NotNull(result.TimeStamp);
        Assert.Equal("ErrorValue", result.ErrorDetail["ErrorKey"]);
        Assert.Equal("DataValue", result.Data);
        Assert.IsType<InvalidOperationException>(result.ExceptionInfo);
        Assert.Equal("/test/path", result.UrlPathDetails);
    }

    [Fact]
    public void Success_Methods_Should_Return_Successful_Result()
    {
        var r1 = Result<string>.Success();
        Assert.True(r1.Succeded);

        var r2 = Result<string>.Success("Success message");
        Assert.True(r2.Succeded);
        Assert.Equal("Success message", r2.MessageDescription);

        var r3 = Result<string>.Success("DataValue");
        Assert.True(r3.Succeded);
        Assert.NotEqual("DataValue", r3.Data);
    }

    [Fact]
    public void Failure_Methods_Should_Return_Failed_Result()
    {
        var r1 = Result<string>.Failure();
        Assert.False(r1.Succeded);

        var r2 = Result<string>.Failure("Fail message");
        Assert.False(r2.Succeded);
        Assert.Equal("Fail message", r2.Data);

        var r3 = Result<string>.Failure(new Dictionary<string, string> { { "key", "value" } });
        Assert.False(r3.Succeded);
        Assert.Equal("value", r3.ErrorDetail["key"]);

        var exception = new InvalidOperationException("Test Exception");
        var r4 = Result<string>.Failure(exception);
        Assert.False(r4.Succeded);
        Assert.Equal(exception, r4.ExceptionInfo);
    }

    [Fact]
    public void TryCatch_Should_Handle_Success_And_Exception()
    {
        var resultSuccess = Result<string>.TryCatch(() => "OK");
        Assert.True(resultSuccess.Succeded);
        Assert.Equal("OK", resultSuccess.Data);

        var resultFailure = Result<string>.TryCatch(() => throw new InvalidOperationException("Boom"));
        Assert.False(resultFailure.Succeded);
        Assert.Contains("Boom", resultFailure.MessageDescription);
    }

    [Fact]
    public void Map_Should_Transform_Data_On_Success()
    {
        var result = Result<int>.Success(5);
        var mapped = result.Map(x => x * 2);

        Assert.True(mapped.Succeded);
        Assert.Equal(10, mapped.Data);
    }

    [Fact]
    public void Bind_Should_Chain_Results()
    {
        var result = Result<int>.Success(3);
        var bound = result.Bind(x => Result<string>.Success($"Value is {x}"));

        Assert.True(bound.Succeded);
        Assert.Equal("Value is 3", bound.MessageDescription);
    }

    [Fact]
    public void Map_And_Bind_Should_Return_Failure_If_Original_Fails()
    {
        var failResult = Result<int>.Failure("Fail");

        var mapped = failResult.Map(x => x * 2);
        Assert.False(mapped.Succeded);
        Assert.Equal("Fail", mapped.MessageDescription);

        var bound = failResult.Bind(x => Result<string>.Success("Nope"));
        Assert.False(bound.Succeded);
        Assert.Equal("Fail", bound.MessageDescription);
    }

    [Fact]
    public async Task Async_Success_And_Failure_Should_Work()
    {
        var success = await Result<string>.SuccessAsync("Async OK");
        Assert.True(success.Succeded);
        Assert.Equal("Async OK", success.MessageDescription);

        var fail = await Result<string>.FailureAsync("Async Fail");
        Assert.False(fail.Succeded);
        Assert.Equal("Async Fail", fail.MessageDescription);
    }

    [Fact]
    public void Match_Should_Invoke_Correct_Action()
    {
        var success = Result<int>.Success(42);
        var fail = Result<int>.Failure("Error");

        var successCalled = false;
        var failCalled = false;

        success.Match(
            onSuccess: _ => successCalled = true,
            onFailure: _ => failCalled = true
        );

        Assert.True(successCalled);
        Assert.False(failCalled);

        successCalled = false;
        failCalled = false;

        fail.Match(
            onSuccess: _ => successCalled = true,
            onFailure: _ => failCalled = true
        );

        Assert.False(successCalled);
        Assert.True(failCalled);
    }

    [Fact]
    public async Task MatchAsync_Should_Invoke_Correct_Action()
    {
        var success = await Result<int>.SuccessAsync(42);
        var fail = await Result<int>.FailureAsync("Error");

        var successCalled = false;
        var failCalled = false;

        await success.MatchAsync(
            _ => { successCalled = true; return Task.CompletedTask; },
            _ => { failCalled = true; return Task.CompletedTask; }
        );

        Assert.True(successCalled);
        Assert.False(failCalled);

        successCalled = false;
        failCalled = false;

        await fail.MatchAsync(
            _ => { successCalled = true; return Task.CompletedTask; },
            _ => { failCalled = true; return Task.CompletedTask; }
        );

        Assert.False(successCalled);
        Assert.True(failCalled);
    }

    [Fact]
    public void Success_WithDataMessageAndStatusCode_ShouldSetAllProperties()
    {
        var data = "TestData";
        var message = "Success Message";
        uint statusCode = 201;

        var result = Result<string>.Success(data, message, statusCode);

        Assert.True(result.Succeded);
        Assert.Equal(data, result.Data);
        Assert.Equal(message, result.MessageDescription);
        Assert.Equal(statusCode, result.StatusCode);
    }

    [Fact]
    public void Success_WithDataAndMessage_ShouldSetDefaultStatusCode()
    {
        var data = "TestData";
        var message = "Success Message";

        var result = Result<string>.Success(data, message);

        Assert.True(result.Succeded);
        Assert.Equal(data, result.Data);
        Assert.Equal(message, result.MessageDescription);
        Assert.Equal((uint)HttpStatusCode.Created, result.StatusCode);
    }

    [Fact]
    public async Task SuccessAsync_WithDataAndMessage_ShouldSetAllProperties()
    {
        var data = "TestData";
        var message = "Success Message";

        var result = await Result<string>.SuccessAsync(data, message);

        Assert.True(result.Succeded);
        Assert.Equal(data, result.Data);
        Assert.Equal(message, result.MessageDescription);
        Assert.NotNull(result.TimeStamp);
    }

    [Fact]
    public async Task FailureAsync_WithMessage_ShouldSetProperties()
    {
        var message = "Error message";
        var result = await Result<string>.FailureAsync(message);
        var statusCode = 400;

        Assert.False(result.Succeded);
        Assert.Equal(message, result.MessageDescription);
        Assert.Equal((uint)statusCode, result.StatusCode);
        Assert.Null(result.Data);
        Assert.Null(result.ErrorDetail);
        Assert.Null(result.ExceptionInfo);
    }

    [Fact]
    public async Task FailureAsync_WithErrors_ShouldSetProperties()
    {
        var errors = new Dictionary<string, string>
        {
            { "Field1", "Required" },
            { "Field2", "Invalid" }
        };

        var result = await Result<string>.FailureAsync(errors);

        Assert.False(result.Succeded);
        Assert.Null(result.MessageDescription);
        Assert.Equal(errors, result.ErrorDetail);
    }

    [Fact]
    public async Task FailureAsync_WithData_ShouldSetProperties()
    {
        var data = new { Valor = "ErrorData" };
        var result = await Result<object>.FailureAsync(data);

        Assert.False(result.Succeded);
        Assert.Equal(data, result.Data);
    }

    [Fact]
    public async Task FailureAsync_WithDataAndMessage_ShouldSetProperties()
    {
        var data = new { Valor = "ErrorData" };
        var message = "Error occurred";

        var result = await Result<object>.FailureAsync(data, message);

        Assert.False(result.Succeded);
        Assert.Equal(data, result.Data);
        Assert.Equal(message, result.MessageDescription);
    }

    [Fact]
    public async Task FailureAsync_WithDataAndErrors_ShouldSetProperties()
    {
        var data = new { Valor = "ErrorData" };
        var errors = new Dictionary<string, string>
        {
            { "Code", "500" }
        };

        var result = await Result<object>.FailureAsync(data, errors);

        Assert.False(result.Succeded);
        Assert.Equal(data, result.Data);
        Assert.Equal(errors, result.ErrorDetail);
    }

    [Fact]
    public async Task FailureAsync_WithMessageAndErrors_ShouldSetProperties()
    {
        var message = "Validation failed";
        var errors = new Dictionary<string, string>
    {
        { "Email", "Invalid" }
    };

        var result = await Result<string>.FailureAsync(message, errors);

        Assert.False(result.Succeded);
        Assert.Equal(message, result.MessageDescription);
        Assert.Equal(errors, result.ErrorDetail);
    }

    [Fact]
    public async Task FailureAsync_WithMessageExceptionAndErrors_ShouldSetProperties()
    {
        var message = "Critical error";
        var exception = new InvalidOperationException("Invalid operation");
        var errors = new Dictionary<string, string>
        {
            { "Field", "NotAllowed" }
        };

        var result = await Result<string>.FailureAsync(message, exception, errors);

        Assert.False(result.Succeded);
        Assert.Equal(message, result.MessageDescription);
        Assert.Equal(exception, result.ExceptionInfo);
        Assert.Equal(errors, result.ErrorDetail);
    }

    [Fact]
    public async Task FailureAsync_WithException_ShouldSetProperties()
    {
        var exception = new Exception("Unexpected error");
        var result = await Result<string>.FailureAsync(exception);

        Assert.False(result.Succeded);
        Assert.Equal(exception, result.ExceptionInfo);
    }

    [Fact]
    public void Failure_WithDataAndMessage_ShouldSetProperties()
    {
        var data = new { Valor = "ErrorData" };
        var message = "Operation failed";

        var result = Result<object>.Failure(data, message);

        Assert.False(result.Succeded);
        Assert.Equal(data, result.Data);
        Assert.Equal(message, result.MessageDescription);
        Assert.Null(result.ErrorDetail);
        Assert.Null(result.ExceptionInfo);
    }

    [Fact]
    public void Failure_WithDataAndErrors_ShouldSetProperties()
    {
        var data = new { Valor = "ErrorData" };
        var errors = new Dictionary<string, string>
        {
            { "Code", "Invalid" },
            { "Detail", "Missing field" }
        };

        var result = Result<object>.Failure(data, errors);

        Assert.False(result.Succeded);
        Assert.Equal(data, result.Data);
        Assert.Equal(errors, result.ErrorDetail);
        Assert.Null(result.MessageDescription);
        Assert.Null(result.ExceptionInfo);
    }

    [Fact]
    public void Failure_WithMessageExceptionAndErrors_ShouldSetProperties()
    {
        var message = "Critical failure occurred";
        var exception = new InvalidOperationException("Invalid operation");
        var errors = new Dictionary<string, string>
        {
            { "ErrorCode", "500" },
            { "ErrorDetail", "Internal Server Error" }
        };

        var result = Result<string>.Failure(message, exception, errors);

        Assert.False(result.Succeded);
        Assert.Equal(message, result.MessageDescription);
        Assert.Equal(exception, result.ExceptionInfo);
        Assert.Equal(errors, result.ErrorDetail);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task FailureAsync_ShouldReturnFailedResult()
    {
        var result = await Result<string>.FailureAsync();

        Assert.False(result.Succeded);
        Assert.Null(result.Data);
        Assert.Null(result.MessageDescription);
        Assert.Null(result.ErrorDetail);
        Assert.Null(result.ExceptionInfo);
    }

    [Fact]
    public async Task SuccessAsync_ShouldReturnSuccessfulResult()
    {
        var result = await Result<string>.SuccessAsync();

        Assert.True(result.Succeded);
        Assert.Null(result.Data);
        Assert.Null(result.MessageDescription);
        Assert.Null(result.ErrorDetail);
        Assert.Null(result.ExceptionInfo);
    }
}
