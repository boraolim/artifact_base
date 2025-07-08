using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Moq;
using Xunit;
using FluentValidation;
using Microsoft.AspNetCore.Http;

using Hogar.Core.Shared.Services;
using Hogar.Core.Shared.Wrappers;
using Hogar.Core.Shared.Exceptions;
using Hogar.Core.Shared.Tests.Records;
using Hogar.Core.Shared.Tests.Handlers;

using MainConstantsCore = Hogar.Core.Shared.Constants.MainConstants;


namespace Hogar.Core.Shared.Tests
{
    public class CommandDispatcherTests
    {
        private readonly Mock<IServiceProvider> _serviceProviderMock = new();
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock = new();
        private readonly DefaultHttpContext _httpContext = new();

        private CommandDispatcher CreateDispatcher()
        {
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(_httpContext);
            return new CommandDispatcher(_serviceProviderMock.Object, _httpContextAccessorMock.Object);
        }

        [Fact]
        public void Dispatch_Should_Handle_Command_Successfully()
        {
            var command = new TestCommand("Test");

            _serviceProviderMock.Setup(x => x.GetService(typeof(ICommandHandler<TestCommand, Result<string>>)))
                                .Returns(new TestCommandHandler());

            var dispatcher = CreateDispatcher();

            var result = dispatcher.Dispatch<TestCommand, string>(command);

            Assert.True(result.Succeded);
            Assert.Equal("Handled: Test", result.MessageDescription);
        }

        [Fact]
        public async Task DispatchAsync_Should_Handle_Command_Successfully()
        {
            var command = new TestCommand("Test");

            _serviceProviderMock.Setup(x => x.GetService(typeof(ICommandHandler<TestCommand, Result<string>>)))
                    .Returns(new TestCommandHandler());

            var dispatcher = CreateDispatcher();

            var result = await dispatcher.DispatchAsync<TestCommand, string>(command);

            Assert.True(result.Succeded);
            Assert.Equal("HandledAsync: Test", result.MessageDescription);
        }

        [Fact]
        public void Dispatch_Should_Set_TraceId_When_Header_Exists()
        {
            var command = new TestCommand("Test");

            _serviceProviderMock.Setup(x => x.GetService(typeof(ICommandHandler<TestCommand, Result<string>>)))
                    .Returns(new TestCommandHandler());

            _httpContext.Request.Headers[MainConstantsCore.CFG_TRACE_ID_HEADER] = "trace-123";

            var dispatcher = CreateDispatcher();

            var result = dispatcher.Dispatch<TestCommand, string>(command);

            Assert.Equal("trace-123", result.TraceId);
        }

        [Fact]
        public void Dispatch_Should_Pass_Validation_When_Valid()
        {
            var command = new TestCommand("Test");

            var validatorMock = new Mock<IValidator<TestCommand>>();

            validatorMock.Setup(x => x.Validate(command))
                         .Returns(new FluentValidation.Results.ValidationResult());

            _serviceProviderMock.Setup(x => x.GetService(typeof(IValidator<TestCommand>)))
                                .Returns(validatorMock.Object);
            _serviceProviderMock.Setup(x => x.GetService(typeof(ICommandHandler<TestCommand, Result<string>>)))
                                .Returns(new TestCommandHandler());

            var dispatcher = CreateDispatcher();

            var result = dispatcher.Dispatch<TestCommand, string>(command);

            Assert.True(result.Succeded);
        }

        [Fact]
        public void Dispatch_Should_Throw_CommonValidationException_When_Validation_Fails()
        {
            var command = new TestCommand("Test");

            var validatorMock = new Mock<IValidator<TestCommand>>();

            validatorMock.Setup(x => x.Validate(command))
                         .Returns(new FluentValidation.Results.ValidationResult(
                             new[] { new FluentValidation.Results.ValidationFailure("Data", "Error") }));

            _serviceProviderMock.Setup(x => x.GetService(typeof(IValidator<TestCommand>)))
                                .Returns(validatorMock.Object);

            var dispatcher = CreateDispatcher();

            Assert.Throws<CommonValidationException>(() => dispatcher.Dispatch<TestCommand, string>(command));
        }

        [Fact]
        public async Task DispatchAsync_Should_Throw_CommonValidationException_When_Validation_Fails()
        {
            var command = new TestCommand("Test");

            var validatorMock = new Mock<IValidator<TestCommand>>();
            validatorMock.Setup(x => x.ValidateAsync(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new FluentValidation.Results.ValidationResult(
                             new[] { new FluentValidation.Results.ValidationFailure("Data", "Error") }));

            _serviceProviderMock.Setup(x => x.GetService(typeof(IValidator<TestCommand>)))
                                .Returns(validatorMock.Object);

            var dispatcher = CreateDispatcher();

            await Assert.ThrowsAsync<CommonValidationException>(() => dispatcher.DispatchAsync<TestCommand, string>(command));
        }

        [Fact]
        public void Dispatch_Should_Handle_Without_Validator()
        {
            var command = new TestCommand("Test");

            _serviceProviderMock.Setup(x => x.GetService(typeof(IValidator<TestCommand>)))
                                .Returns(null); // No hay validador

            _serviceProviderMock.Setup(x => x.GetService(typeof(ICommandHandler<TestCommand, Result<string>>)))
                                .Returns(new TestCommandHandler());

            var dispatcher = CreateDispatcher();

            var result = dispatcher.Dispatch<TestCommand, string>(command);

            Assert.True(result.Succeded);
        }

        [Fact]
        public async Task DispatchAsync_Should_Handle_Without_Validator()
        {
            var command = new TestCommand("Test");
            
            _serviceProviderMock.Setup(x => x.GetService(typeof(IValidator<TestCommand>)))
                                .Returns(null);

            _serviceProviderMock.Setup(x => x.GetService(typeof(ICommandHandler<TestCommand, Result<string>>)))
                                .Returns(new TestCommandHandler());

            var dispatcher = CreateDispatcher();

            var result = await dispatcher.DispatchAsync<TestCommand, string>(command);

            Assert.True(result.Succeded);
        }

        [Fact]
        public async Task DispatchAsync_Should_Pass_Validation_When_Valid()
        {
            var command = new TestCommand("ValidCommand");

            var validatorMock = new Mock<IValidator<TestCommand>>();
            validatorMock.Setup(x => x.ValidateAsync(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _serviceProviderMock.Setup(x => x.GetService(typeof(IValidator<TestCommand>)))
                                .Returns(validatorMock.Object);
            _serviceProviderMock.Setup(x => x.GetService(typeof(ICommandHandler<TestCommand, Result<string>>)))
                                .Returns(new TestCommandHandler());

            var dispatcher = CreateDispatcher();

            var result = await dispatcher.DispatchAsync<TestCommand, string>(command);

            Assert.True(result.Succeded);
            Assert.Equal("HandledAsync: ValidCommand", result.MessageDescription);
        }

        [Fact]
        public async Task DispatchAsync_Should_Throw_CommonValidationException_When_Invalid()
        {
            var command = new TestCommand("InvalidCommand");

            var validationFailures = new List<FluentValidation.Results.ValidationFailure>
            {
                new("Propiedad", "Error de validación")
            };

            var validatorMock = new Mock<IValidator<TestCommand>>();
            validatorMock.Setup(x => x.ValidateAsync(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new FluentValidation.Results.ValidationResult(validationFailures));

            _serviceProviderMock.Setup(x => x.GetService(typeof(IValidator<TestCommand>)))
                                .Returns(validatorMock.Object);

            var dispatcher = CreateDispatcher();

            var exception = await Assert.ThrowsAsync<CommonValidationException>(() =>
                dispatcher.DispatchAsync<TestCommand, string>(command));

            Assert.Single(exception.Errors);
            Assert.Equal("Propiedad", exception.Errors[0].PropertyName);
            Assert.Equal("Error de validación", exception.Errors[0].ErrorMessage);
        }

        [Fact]
        public void TestCommandHandler_Should_Handle_Successfully()
        {
            var handler = new TestCommandHandler();
            var command = new TestCommand("TestData");

            var result = handler.Handle(command);

            Assert.True(result.Succeded);
            Assert.Equal("Handled: TestData", result.MessageDescription);
        }

        [Fact]
        public async Task TestCommandHandler_Should_HandleAsync_Successfully()
        {
            var handler = new TestCommandHandler();
            var command = new TestCommand("TestData");

            var result = await handler.HandleAsync(command, CancellationToken.None);

            Assert.True(result.Succeded);
            Assert.Equal("HandledAsync: TestData", result.MessageDescription);
        }

        [Fact]
        public async Task TestChainToStringHandler_Should_HandleAsync_Successfully()
        {
            var handler = new TestChainToStringHandler();
            var command = new ChainToStringRequest("ChainData");

            var result = await handler.HandleAsync(command, CancellationToken.None);

            Assert.True(result.Succeded);
            Assert.Equal($"HandledAsync: ChainData", result.MessageDescription);
        }

        [Fact]
        public async Task Dispatcher_Should_HandleAsync_TestCommand_Successfully_With_Validation()
        {
            var command = new TestCommand("TestData");

            var validatorMock = new Mock<IValidator<TestCommand>>();
            validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _serviceProviderMock.Setup(x => x.GetService(typeof(IValidator<TestCommand>)))
                                .Returns(validatorMock.Object);
            _serviceProviderMock.Setup(x => x.GetService(typeof(ICommandHandler<TestCommand, Result<string>>)))
                                .Returns(new TestCommandHandler());

            var dispatcher = CreateDispatcher();

            var result = await dispatcher.DispatchAsync<TestCommand, string>(command);

            Assert.True(result.Succeded);
            Assert.Equal("HandledAsync: TestData", result.MessageDescription);
        }

        [Fact]
        public async Task Dispatcher_Should_Throw_CommonValidationException_When_TestCommand_Invalid()
        {
            var command = new TestCommand("TestData");

            var failures = new List<FluentValidation.Results.ValidationFailure>
            {
                new("Data", "Invalid Data")
            };

            var validatorMock = new Mock<IValidator<TestCommand>>();
            validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new FluentValidation.Results.ValidationResult(failures));

            _serviceProviderMock.Setup(x => x.GetService(typeof(IValidator<TestCommand>)))
                                .Returns(validatorMock.Object);

            var dispatcher = CreateDispatcher();

            var exception = await Assert.ThrowsAsync<CommonValidationException>(() =>
                dispatcher.DispatchAsync<TestCommand, string>(command));

            Assert.Single(exception.Errors);
            Assert.Equal("Invalid Data", exception.Errors[0].ErrorMessage);
        }

        [Fact]
        public async Task Dispatcher_Should_HandleAsync_ChainToStringRequest_Successfully_With_Validation()
        {
            var command = new ChainToStringRequest("ChainData");

            var validatorMock = new Mock<IValidator<ChainToStringRequest>>();
            validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _serviceProviderMock.Setup(x => x.GetService(typeof(IValidator<ChainToStringRequest>)))
                                .Returns(validatorMock.Object);
            _serviceProviderMock.Setup(x => x.GetService(typeof(ICommandHandler<ChainToStringRequest, Result<object>>)))
                                .Returns(new TestChainToStringHandler());

            var dispatcher = CreateDispatcher();

            var result = await dispatcher.DispatchAsync<ChainToStringRequest, object>(command);

            Assert.True(result.Succeded);
            Assert.Equal($"HandledAsync: ChainData", result.MessageDescription);
        }

        [Fact]
        public async Task Dispatcher_Should_Throw_CommonValidationException_When_ChainToStringRequest_Invalid()
        {
            var command = new ChainToStringRequest("ChainData");

            var failures = new List<FluentValidation.Results.ValidationFailure>
            {
                new("Payload", "Invalid Payload")
            };

            var validatorMock = new Mock<IValidator<ChainToStringRequest>>();
            validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new FluentValidation.Results.ValidationResult(failures));

            _serviceProviderMock.Setup(x => x.GetService(typeof(IValidator<ChainToStringRequest>)))
                                .Returns(validatorMock.Object);

            var dispatcher = CreateDispatcher();

            var exception = await Assert.ThrowsAsync<CommonValidationException>(() =>
                dispatcher.DispatchAsync<ChainToStringRequest, object>(command));

            Assert.Single(exception.Errors);
            Assert.Equal("Invalid Payload", exception.Errors[0].ErrorMessage);
        }
    }
}
