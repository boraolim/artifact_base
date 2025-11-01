using System;
using System.Linq;
using System.Collections.Generic;

using Xunit;
using FluentValidation.Results;

using Utilities.Core.Shared.Exceptions;
using Utilities.Core.Shared.Tests.Exceptions;

using MessageConstantsCore = Utilities.Core.Shared.Constants.MessageConstants;

namespace Utilities.Core.Shared.Tests
{
    public class CustomExceptionsTests
    {
        [Fact]
        public void CustomException_DefaultConstructor_ShouldSetHResultAndVerifyMessage()
        {
            var ex = new CustomException();
            Assert.Equal(-1, ex.HResult);
            Assert.False(string.IsNullOrEmpty(ex.Message)); // Verifica que tenga algún mensaje
        }

        [Fact]
        public void CustomException_DefaultConstructor_ShouldSetHResult()
        {
            var ex = new CustomException();
            Assert.Equal(-1, ex.HResult);
        }

        [Fact]
        public void CustomException_MessageConstructor_ShouldSetMessageAndHResult()
        {
            var ex = new CustomException("Mensaje de error");
            Assert.Equal(-1, ex.HResult);
            Assert.Equal("Mensaje de error", ex.Message);
        }

        [Fact]
        public void CustomException_MessageInnerConstructor_ShouldSetAllProperties()
        {
            var inner = new Exception("Inner");
            var ex = new CustomException("Mensaje", inner);
            Assert.Equal(-1, ex.HResult);
            Assert.Equal("Mensaje", ex.Message);
            Assert.Equal(inner, ex.InnerException);
        }

        [Fact]
        public void CommonValidationException_ShouldSetErrorsAndMessage()
        {
            var failures = new List<ValidationFailure>
        {
            new ValidationFailure("Propiedad", "Error")
        };

            var ex = new CommonValidationException(failures);

            Assert.Equal(-2, ex.HResult);
            Assert.Equal(MessageConstantsCore.MSG_FAIL_VALIDATION, ex.Message);
            Assert.NotNull(ex.Errors);
            Assert.Single(ex.Errors);
            Assert.Equal("Propiedad", ex.Errors[0].PropertyName);
            Assert.Equal("Error", ex.Errors[0].ErrorMessage);
        }

        [Theory]
        [InlineData(typeof(BadRequestException), -3)]
        [InlineData(typeof(UnauthorizedException), -4)]
        [InlineData(typeof(PaymentRequiredException), -5)]
        [InlineData(typeof(ForbiddenException), -6)]
        [InlineData(typeof(NotFoundSourceException), -7)]
        [InlineData(typeof(MethodNotAllowedException), -8)]
        [InlineData(typeof(NotAcceptableException), -9)]
        [InlineData(typeof(ProxyAuthenticationRequiredException), -10)]
        [InlineData(typeof(RequestTimeoutException), -11)]
        [InlineData(typeof(ConflictException), -12)]
        [InlineData(typeof(GoneException), -13)]
        [InlineData(typeof(LengthRequiredException), -14)]
        [InlineData(typeof(PreconditionFailedException), -15)]
        [InlineData(typeof(RequestEntityTooLargeException), -16)]
        [InlineData(typeof(RequestUriTooLongException), -17)]
        [InlineData(typeof(UnsupportedMediaTypeException), -18)]
        [InlineData(typeof(RequestedRangeNotSatisfiableException), -19)]
        [InlineData(typeof(ExpectationFailedException), -20)]
        [InlineData(typeof(ImATeapotException), -21)]
        [InlineData(typeof(MethodFailureException), -22)]
        [InlineData(typeof(UnprocessableEntityException), -23)]
        [InlineData(typeof(LockedException), -24)]
        [InlineData(typeof(FailedDependencyException), -25)]
        [InlineData(typeof(TooEarlyException), -26)]
        [InlineData(typeof(UpgradeRequiredException), -27)]
        [InlineData(typeof(PreconditionRequiredException), -28)]
        [InlineData(typeof(TooManyRequestsException), -29)]
        [InlineData(typeof(RequestHeaderFieldsTooLargeException), -30)]
        [InlineData(typeof(NoResponseException), -31)]
        [InlineData(typeof(RetryWithException), -32)]
        [InlineData(typeof(BlockedByWPControlsException), -33)]
        [InlineData(typeof(UnavailableForLegalReasonsException), -34)]
        [InlineData(typeof(ClientClosedRequestException), -35)]
        [InlineData(typeof(InternalServerErrorException), -36)]
        [InlineData(typeof(NotImplementedFormException), -37)]
        [InlineData(typeof(BadGatewayException), -38)]
        [InlineData(typeof(ServiceUnavailableException), -39)]
        [InlineData(typeof(GatewayTimeoutException), -40)]
        [InlineData(typeof(HttpVersionNotSupportedException), -41)]
        [InlineData(typeof(VariantAlsoNegotiatesException), -42)]
        [InlineData(typeof(InsufficientStorageException), -43)]
        [InlineData(typeof(LoopDetectedException), -44)]
        [InlineData(typeof(BandwidthLimitExceededException), -45)]
        [InlineData(typeof(NotExtendedException), -46)]
        [InlineData(typeof(NetworkAuthenticationRequiredException), -47)]
        [InlineData(typeof(JwtValidationException), -48)]
        [InlineData(typeof(DbFactoryException), -49)]
        [InlineData(typeof(EntityAlreadyExistsException), -50)]
        [InlineData(typeof(TraceIdNotFoundException), -52)]
        [InlineData(typeof(UnhandledException), -53)]
        public void Exception_WithMessage_ShouldSetMessageAndHResult(Type exceptionType, int expectedHResult)
        {
            var ctor = exceptionType.GetConstructor(new[] { typeof(string) });
            var ex = (Exception)ctor.Invoke(new object[] { "Mensaje prueba" });

            Assert.Equal(expectedHResult, ex.HResult);
            Assert.Equal("Mensaje prueba", ex.Message);
        }

        [Fact]
        public void EntityNotFoundException_Constructor_ShouldSetProperties()
        {
            var type = typeof(string);
            var idValue = 123;

            var ex1 = new EntityNotFoundException();
            Assert.Equal(-51, ex1.HResult);

            var ex2 = new EntityNotFoundException(type);
            Assert.Equal(-51, ex2.HResult);
            Assert.Equal(type, ex2.EntityType);

            var ex3 = new EntityNotFoundException(type, idValue);
            Assert.Equal(-51, ex3.HResult);
            Assert.Equal(type, ex3.EntityType);
            Assert.Equal(idValue, ex3.Id);

            var inner = new Exception("inner");
            var ex4 = new EntityNotFoundException(type, idValue, inner);
            Assert.Equal(-51, ex4.HResult);
            Assert.Equal(type, ex4.EntityType);
            Assert.Equal(idValue, ex4.Id);
            Assert.Equal(inner, ex4.InnerException);

            var message = "Mensaje personalizado";
            var ex5 = new EntityNotFoundException(message);
            Assert.Equal(-51, ex5.HResult);
            Assert.Equal(message, ex5.Message);

            var ex6 = new EntityNotFoundException(message, inner);
            Assert.Equal(-51, ex6.HResult);
            Assert.Equal(message, ex6.Message);
            Assert.Equal(inner, ex6.InnerException);
        }

        [Theory]
        [MemberData(nameof(ExceptionData.ExceptionList), MemberType = typeof(ExceptionData))]
        public void Exception_ShouldSetHResult_And_Message(Type exceptionType, int expectedHResult, object additionalParam = null)
        {
            Exception ex;

            if(exceptionType == typeof(CommonValidationException))
            {
                var failures = additionalParam as List<FluentValidation.Results.ValidationFailure> ?? new List<FluentValidation.Results.ValidationFailure>();
                ex = (Exception)Activator.CreateInstance(exceptionType, failures);
            }
            else if(exceptionType == typeof(EntityNotFoundException))
            {
                var entityType = typeof(string);
                var idValue = 123;
                var innerEx = new Exception("inner");

                var ex1 = new EntityNotFoundException();
                Assert.Equal(expectedHResult, ex1.HResult);

                var ex2 = new EntityNotFoundException(entityType);
                Assert.Equal(expectedHResult, ex2.HResult);
                Assert.Equal(entityType, ex2.EntityType);

                var ex3 = new EntityNotFoundException(entityType, idValue);
                Assert.Equal(expectedHResult, ex3.HResult);
                Assert.Equal(entityType, ex3.EntityType);
                Assert.Equal(idValue, ex3.Id);

                var ex4 = new EntityNotFoundException(entityType, idValue, innerEx);
                Assert.Equal(expectedHResult, ex4.HResult);
                Assert.Equal(entityType, ex4.EntityType);
                Assert.Equal(idValue, ex4.Id);
                Assert.Equal(innerEx, ex4.InnerException);

                var message = "Mensaje personalizado";
                var ex5 = new EntityNotFoundException(message);
                Assert.Equal(expectedHResult, ex5.HResult);
                Assert.Equal(message, ex5.Message);

                var ex6 = new EntityNotFoundException(message, innerEx);
                Assert.Equal(expectedHResult, ex6.HResult);
                Assert.Equal(message, ex6.Message);
                Assert.Equal(innerEx, ex6.InnerException);

                return;
            }
            else
            {
                if(additionalParam != null)
                {
                    ex = (Exception)Activator.CreateInstance(exceptionType, additionalParam);
                }
                else
                {
                    // Si el constructor con string no existe, crear con default constructor
                    var constructor = exceptionType.GetConstructor(new[] { typeof(string) });
                    if(constructor != null)
                    {
                        ex = (Exception)Activator.CreateInstance(exceptionType, "Mensaje de prueba");
                    }
                    else
                    {
                        ex = (Exception)Activator.CreateInstance(exceptionType);
                    }
                }
            }

            Assert.Equal(expectedHResult, ex.HResult);

            // Validar mensaje
            if(ex is CommonValidationException)
            {
                // Compara con el mensaje constante esperado para esta excepción
                Assert.Equal(MessageConstantsCore.MSG_FAIL_VALIDATION, ex.Message);
            }
            else if(!string.IsNullOrEmpty(ex.Message))
            {
                if(additionalParam is string s)
                    Assert.Equal(s, ex.Message);
                else
                    Assert.Equal("Mensaje de prueba", ex.Message);
            }
            else
            {
                Assert.Null(ex.Message);
            }

            Assert.Null(ex.InnerException);
        }


        [Theory]
        [MemberData(nameof(ExceptionData.ExceptionList), MemberType = typeof(ExceptionData))]
        public void Exception_ShouldInheritFromException(Type exceptionType, int _, object __ = null)
        {
            Assert.True(typeof(Exception).IsAssignableFrom(exceptionType));

            if(exceptionType == typeof(CommonValidationException))
            {
                var constructor = exceptionType.GetConstructor(new[] { typeof(List<FluentValidation.Results.ValidationFailure>) });
                Assert.NotNull(constructor);
                Assert.Single(constructor.GetParameters());
                Assert.Equal(typeof(List<FluentValidation.Results.ValidationFailure>), constructor.GetParameters()[0].ParameterType);
            }
            else if(exceptionType == typeof(EntityNotFoundException))
            {
                var ctors = exceptionType.GetConstructors();
                Assert.Contains(ctors, c => c.GetParameters().Length == 0); // sin parámetros
                Assert.Contains(ctors, c => c.GetParameters().Any(p => p.ParameterType == typeof(Type)));
                // etc...
            }
        }
    }
}
