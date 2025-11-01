using System;
using System.Collections.Generic;

using Utilities.Core.Shared.Exceptions;
using Xunit;

namespace Utilities.Core.Shared.Tests.Exceptions
{
    public class MyException : Exception { }
    public class OtherException : Exception { }
    public class NotException { }

    public static class ExceptionData
    {
        public static TheoryData<Type, int, object> ExceptionList => new TheoryData<Type, int, object>
        {
             { typeof(CustomException), -1, null },
             { typeof(CommonValidationException), -2, new List<FluentValidation.Results.ValidationFailure>() },
             { typeof(BadRequestException), -3, null },
             { typeof(UnauthorizedException), -4, null },
             { typeof(PaymentRequiredException), -5, null },
             { typeof(ForbiddenException), -6, null },
             { typeof(NotFoundSourceException), -7, null },
             { typeof(MethodNotAllowedException), -8, null },
             { typeof(NotAcceptableException), -9, null },
             { typeof(ProxyAuthenticationRequiredException), -10, null },
             { typeof(RequestTimeoutException), -11, null },
             { typeof(ConflictException), -12, null },
             { typeof(GoneException), -13, null },
             { typeof(LengthRequiredException), -14, null },
             { typeof(PreconditionFailedException), -15, null },
             { typeof(RequestEntityTooLargeException), -16, null },
             { typeof(RequestUriTooLongException), -17, null },
             { typeof(UnsupportedMediaTypeException), -18, null },
             { typeof(RequestedRangeNotSatisfiableException), -19, null },
             { typeof(ExpectationFailedException), -20, null },
             { typeof(ImATeapotException), -21, null },
             { typeof(MethodFailureException), -22, null },
             { typeof(UnprocessableEntityException), -23, null },
             { typeof(LockedException), -24, null },
             { typeof(FailedDependencyException), -25, null },
             { typeof(TooEarlyException), -26, null },
             { typeof(UpgradeRequiredException), -27, null },
             { typeof(PreconditionRequiredException), -28, null },
             { typeof(TooManyRequestsException), -29, null },
             { typeof(RequestHeaderFieldsTooLargeException), -30, null },
             { typeof(NoResponseException), -31, null },
             { typeof(RetryWithException), -32, null },
             { typeof(BlockedByWPControlsException), -33, null },
             { typeof(UnavailableForLegalReasonsException), -34, null },
             { typeof(ClientClosedRequestException), -35, null },
             { typeof(InternalServerErrorException), -36, null },
             { typeof(NotImplementedFormException), -37, null },
             { typeof(BadGatewayException), -38, null },
             { typeof(ServiceUnavailableException), -39, null },
             { typeof(GatewayTimeoutException), -40, null },
             { typeof(HttpVersionNotSupportedException), -41, null },
             { typeof(VariantAlsoNegotiatesException), -42, null },
             { typeof(InsufficientStorageException), -43, null },
             { typeof(LoopDetectedException), -44, null },
             { typeof(BandwidthLimitExceededException), -45, null },
             { typeof(NotExtendedException), -46, null },
             { typeof(NetworkAuthenticationRequiredException), -47, null },
             { typeof(JwtValidationException), -48, null },
             { typeof(DbFactoryException), -49, null },
             { typeof(EntityAlreadyExistsException), -50, null },
             { typeof(EntityNotFoundException), -51, null },
             { typeof(TraceIdNotFoundException), -52, null },
             { typeof(UnhandledException), -53, null }
        };
    }
}
