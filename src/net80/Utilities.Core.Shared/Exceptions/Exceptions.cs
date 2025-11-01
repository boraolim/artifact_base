namespace Utilities.Core.Shared.Exceptions;

public class CustomException : Exception
{
    public CustomException() { HResult = -1; }

    public CustomException(string message) : base(message) { HResult = -1; }

    public CustomException(string message, Exception inner) : base(message, inner) { HResult = -1; }
}

public class CommonValidationException : Exception
{
    public List<ValidationFailure> Errors { get; }

    public CommonValidationException(List<ValidationFailure> failures) : base(MessageConstantsCore.MSG_FAIL_VALIDATION)
        { HResult = -2; Errors = failures.ToList(); }
}

public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message) { HResult = -3; }
}

public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message) : base(message) { HResult = -4; }
}

public class PaymentRequiredException : Exception
{
    public PaymentRequiredException(string message) : base(message) { HResult = -5; }
}

public class ForbiddenException : Exception
{
    public ForbiddenException(string message) : base(message) { HResult = -6; }
}

public class NotFoundSourceException : Exception
{
    public NotFoundSourceException(string message) : base(message) { HResult = -7; }
}

public class MethodNotAllowedException : Exception
{
    public MethodNotAllowedException(string message) : base(message) { HResult = -8; }
}

public class NotAcceptableException : Exception
{
    public NotAcceptableException(string message) : base(message) { HResult = -9; }
}

public class ProxyAuthenticationRequiredException : Exception
{
    public ProxyAuthenticationRequiredException(string message) : base(message) { HResult = -10; }
}

public class RequestTimeoutException : Exception
{
    public RequestTimeoutException(string message) : base(message) { HResult = -11; }
}

public class ConflictException : Exception
{
    public ConflictException(string message) : base(message) { HResult = -12; }
}

public class GoneException : Exception
{
    public GoneException(string message) : base(message) { HResult = -13; }
}

public class LengthRequiredException : Exception
{
    public LengthRequiredException(string message) : base(message) { HResult = -14; }
}

public class PreconditionFailedException : Exception
{
    public PreconditionFailedException(string message) : base(message) { HResult = -15; }
}

public class RequestEntityTooLargeException : Exception
{
    public RequestEntityTooLargeException(string message) : base(message) { HResult = -16; }
}

public class RequestUriTooLongException : Exception
{
    public RequestUriTooLongException(string message) : base(message) { HResult = -17; }
}

public class UnsupportedMediaTypeException : Exception
{
    public UnsupportedMediaTypeException(string message) : base(message) { HResult = -18; }
}

public class RequestedRangeNotSatisfiableException : Exception
{
    public RequestedRangeNotSatisfiableException(string message) : base(message) { HResult = -19; }
}

public class ExpectationFailedException : Exception
{
    public ExpectationFailedException(string message) : base(message) { HResult = -20; }
}

public class ImATeapotException : Exception
{
    public ImATeapotException(string message) : base(message) { HResult = -21; }
}

public class MethodFailureException : Exception
{
    public MethodFailureException(string message) : base(message) { HResult = -22; }
}

public class UnprocessableEntityException : Exception
{
    public UnprocessableEntityException(string message) : base(message) { HResult = -23; }
}

public class LockedException : Exception
{
    public LockedException(string message) : base(message) { HResult = -24; }
}

public class FailedDependencyException : Exception
{
    public FailedDependencyException(string message) : base(message) { HResult = -25; }
}

public class TooEarlyException : Exception
{
    public TooEarlyException(string message) : base(message) { HResult = -26; }
}

public class UpgradeRequiredException : Exception
{
    public UpgradeRequiredException(string message) : base(message) { HResult = -27; }
}

public class PreconditionRequiredException : Exception
{
    public PreconditionRequiredException(string message) : base(message) { HResult = -28; }
}

public class TooManyRequestsException : Exception
{
    public TooManyRequestsException(string message) : base(message) { HResult = -29; }
}

public class RequestHeaderFieldsTooLargeException : Exception
{
    public RequestHeaderFieldsTooLargeException(string message) : base(message) { HResult = -30; }
}

public class NoResponseException : Exception
{
    public NoResponseException(string message) : base(message) { HResult = -31; }
}

public class RetryWithException : Exception
{
    public RetryWithException(string message) : base(message) { HResult = -32; }
}

public class BlockedByWPControlsException : Exception
{
    public BlockedByWPControlsException(string message) : base(message) { HResult = -33; }
}

public class UnavailableForLegalReasonsException : Exception
{
    public UnavailableForLegalReasonsException(string message) : base(message) { HResult = -34; }
}

public class ClientClosedRequestException : Exception
{
    public ClientClosedRequestException(string message) : base(message) { HResult = -35; }
}

public class InternalServerErrorException : Exception
{
    public InternalServerErrorException(string message) : base(message) { HResult = -36; }
}

public class NotImplementedFormException : Exception
{
    public NotImplementedFormException(string message) : base(message) { HResult = -37; }
}

public class BadGatewayException : Exception
{
    public BadGatewayException(string message) : base(message) { HResult = -38; }
}

public class ServiceUnavailableException : Exception
{
    public ServiceUnavailableException(string message) : base(message) { HResult = -39; }
}

public class GatewayTimeoutException : Exception
{
    public GatewayTimeoutException(string message) : base(message) { HResult = -40; }
}

public class HttpVersionNotSupportedException : Exception
{
    public HttpVersionNotSupportedException(string message) : base(message) { HResult = -41; }
}

public class VariantAlsoNegotiatesException : Exception
{
    public VariantAlsoNegotiatesException(string message) : base(message) { HResult = -42; }
}

public class InsufficientStorageException : Exception
{
    public InsufficientStorageException(string message) : base(message) { HResult = -43; }
}

public class LoopDetectedException : Exception
{
    public LoopDetectedException(string message) : base(message) { HResult = -44; }
}

public class BandwidthLimitExceededException : Exception
{
    public BandwidthLimitExceededException(string message) : base(message) { HResult = -45; }
}

public class NotExtendedException : Exception
{
    public NotExtendedException(string message) : base(message) { HResult = -46; }
}

public class NetworkAuthenticationRequiredException : Exception
{
    public NetworkAuthenticationRequiredException(string message) : base(message) { HResult = -47; }
}

public class JwtValidationException : Exception
{
    public JwtValidationException(string message) : base(message) { HResult = -48; }
}

public class DbFactoryException : Exception
{
    public DbFactoryException(string message) : base(message) { HResult = -49; }
}

public class EntityAlreadyExistsException : Exception
{
    public EntityAlreadyExistsException(string message) : base(message) { HResult = -50; }
}

public class EntityNotFoundException : Exception
{
    public Type EntityType { get; set; }
    public object Id { get; set; }

    public EntityNotFoundException() : base() { HResult = -51; }

    public EntityNotFoundException(Type entityType) : this(entityType, null, null) { }

    public EntityNotFoundException(Type entityType, object idValue) : this(entityType, idValue, null) { }

    public EntityNotFoundException(Type entityType, object idValue, Exception innerException)
        : base(
            idValue == null ? string.Format(MessageConstantsCore.MSG_ENTITY_NOT_FOUND, entityType.FullName.Trim())
                            : string.Format(MessageConstantsCore.MSG_ENTITY_BY_ID_NOT_FOUND, entityType.FullName.Trim(), idValue),
            innerException) { EntityType = entityType; Id = idValue; HResult = -51; }

    public EntityNotFoundException(string message) : base(message) { HResult = -51; }

    public EntityNotFoundException(string message, Exception innerException) : base(message, innerException)
        {  HResult = -51; }
}

public class TraceIdNotFoundException : Exception
{
    public TraceIdNotFoundException(string message) : base(message) { HResult = -52; }
}

public class UnhandledException : Exception
{
    public UnhandledException(string message) : base(message) { HResult = -53; }
}
