namespace Hogar.Core.Shared.Dictionary;

public static class ExceptionDictionary
{
    public static Dictionary<ushort, Exception> GetStatusHTTPCodes() => new Dictionary<ushort, Exception>()
    {
        {
            HttpStatusCodeConstantsCore.CFG_BAD_REQUEST,
            new BadRequestException(MessageConstantsCore.MSG_BAD_REQUEST)
        },
        {
            HttpStatusCodeConstantsCore.CFG_UNAUTHORIZED,
            new UnauthorizedException(MessageConstantsCore.MSG_UNAUTHORIZED)
        },
        {
            HttpStatusCodeConstantsCore.CFG_PAYMENT_REQUIRED,
            new PaymentRequiredException(MessageConstantsCore.MSG_PAYMENT_REQUIRED)
        },
        {
            HttpStatusCodeConstantsCore.CFG_FORBIDDEN,
            new ForbiddenException(MessageConstantsCore.MSG_FORBIDDEN)
        },
        {
            HttpStatusCodeConstantsCore.CFG_NOT_FOUND,
            new NotFoundSourceException(MessageConstantsCore.MSG_NOT_FOUND)
        },
        {
            HttpStatusCodeConstantsCore.CFG_METHOD_NOT_ALLOWED,
            new MethodNotAllowedException(MessageConstantsCore.MSG_METHOD_NOT_ALLOWED)
        },
        {
            HttpStatusCodeConstantsCore.CFG_NOT_ACCEPTABLE,
            new NotAcceptableException(MessageConstantsCore.MSG_NOT_ACCEPTABLE)
        },
        {
            HttpStatusCodeConstantsCore.CFG_PROXY_AUTHENTICATION_REQUIRED,
            new ProxyAuthenticationRequiredException(MessageConstantsCore.MSG_PROXY_AUTHENTICATION_REQUIRED)
        },
        {
            HttpStatusCodeConstantsCore.CFG_REQUEST_TIMEOUT,
            new RequestTimeoutException(MessageConstantsCore.MSG_REQUEST_TIMEOUT)
        },
        {
            HttpStatusCodeConstantsCore.CFG_CONFLICT,
            new ConflictException(MessageConstantsCore.MSG_CONFLICT)
        },
        {
            HttpStatusCodeConstantsCore.CFG_GONE,
            new GoneException(MessageConstantsCore.MSG_GONE)
        },
        {
            HttpStatusCodeConstantsCore.CFG_LENGTH_REQUIRED,
            new LengthRequiredException(MessageConstantsCore.MSG_LENGTH_REQUIRED)
        },
        {
            HttpStatusCodeConstantsCore.CFG_PRECONDITION_FAILED,
            new PreconditionFailedException(MessageConstantsCore.MSG_PRECONDITION_FAILED)
        },
        {
            HttpStatusCodeConstantsCore.CFG_REQUEST_ENTITY_TOO_LARGE,
            new RequestEntityTooLargeException(MessageConstantsCore.MSG_REQUEST_ENTITY_TOO_LARGE)
        },
        {
            HttpStatusCodeConstantsCore.CFG_REQUEST_URI_TOO_LONG,
            new RequestUriTooLongException(MessageConstantsCore.MSG_REQUEST_URI_TOO_LONG)
        },
        {
            HttpStatusCodeConstantsCore.CFG_UNSUPPORTED_MEDIA_TYPE,
            new UnsupportedMediaTypeException(MessageConstantsCore.MSG_UNSUPPORTED_MEDIA_TYPE)
        },
        {
            HttpStatusCodeConstantsCore.CFG_REQUESTED_RANGE_NOT_SATISFIABLE,
            new RequestedRangeNotSatisfiableException(MessageConstantsCore.MSG_REQUESTED_RANGE_NOT_SATISFIABLE)
        },
        {
            HttpStatusCodeConstantsCore.CFG_EXPECTATION_FAILED,
            new ExpectationFailedException(MessageConstantsCore.MSG_EXPECTATION_FAILED)
        },
        {
            HttpStatusCodeConstantsCore.CFG_IM_A_TEAPOT,
            new ImATeapotException(MessageConstantsCore.MSG_IM_A_TEAPOT)
        },
        {
            HttpStatusCodeConstantsCore.CFG_METHOD_FAILURE,
            new MethodFailureException(MessageConstantsCore.MSG_METHOD_FAILURE)
        },
        {
            HttpStatusCodeConstantsCore.CFG_UNPROCESSABLE_ENTITY,
            new UnprocessableEntityException(MessageConstantsCore.MSG_UNPROCESSABLE_ENTITY)
        },
        {
            HttpStatusCodeConstantsCore.CFG_LOCKED,
            new LockedException(MessageConstantsCore.MSG_LOCKED)
        },
        {
            HttpStatusCodeConstantsCore.CFG_FAILED_DEPENDENCY,
            new FailedDependencyException(MessageConstantsCore.MSG_FAILED_DEPENDENCY)
        },
        {
            HttpStatusCodeConstantsCore.CFG_TOO_EARLY,
            new TooEarlyException(MessageConstantsCore.MSG_TOO_EARLY)
        },
        {
            HttpStatusCodeConstantsCore.CFG_UPGRADE_REQUIRED,
            new UpgradeRequiredException(MessageConstantsCore.MSG_UPGRADE_REQUIRED)
        },
        {
            HttpStatusCodeConstantsCore.CFG_PRECONDITION_REQUIRED,
            new PreconditionRequiredException(MessageConstantsCore.MSG_PRECONDITION_REQUIRED)
        },
        {
            HttpStatusCodeConstantsCore.CFG_TOO_MANY_REQUESTS,
            new TooManyRequestsException(MessageConstantsCore.MSG_TOO_MANY_REQUESTS)
        },
        {
            HttpStatusCodeConstantsCore.CFG_REQUEST_HEADER_FIELDS_TOO_LARGE,
            new RequestHeaderFieldsTooLargeException(MessageConstantsCore.MSG_REQUEST_HEADER_FIELDS_TOO_LARGE)
        },
        {
            HttpStatusCodeConstantsCore.CFG_NO_RESPONSE_NGINX,
            new NoResponseException(MessageConstantsCore.MSG_NO_RESPONSE_NGINX)
        },
        {
            HttpStatusCodeConstantsCore.CFG_RETRY_WITH_IIS,
            new RetryWithException(MessageConstantsCore.MSG_RETRY_WITH_IIS)
        },
        {
            HttpStatusCodeConstantsCore.CFG_BLOCKED_BY_WP_CTRLS,
            new BlockedByWPControlsException(MessageConstantsCore.MSG_BLOCKED_BY_WP_CTRLS)
        },
        {
            HttpStatusCodeConstantsCore.CFG_UNAVAILABLE_FOR_LEGAL_REASONS,
            new UnavailableForLegalReasonsException(MessageConstantsCore.MSG_UNAVAILABLE_FOR_LEGAL_REASONS)
        },
        {
            HttpStatusCodeConstantsCore.CFG_CLIENT_CLOSED_REQUEST_NGINX,
            new ClientClosedRequestException(MessageConstantsCore.MSG_CLIENT_CLOSED_REQUEST_NGINX)
        },
        {
            HttpStatusCodeConstantsCore.CFG_INTERNAL_SERVER_ERROR,
            new InternalServerErrorException(MessageConstantsCore.MSG_INTERNAL_SERVER_ERROR)
        },
        {
            HttpStatusCodeConstantsCore.CFG_NOT_IMPLEMENTED,
            new NotImplementedFormException(MessageConstantsCore.MSG_NOT_IMPLEMENTED)
        },
        {
            HttpStatusCodeConstantsCore.CFG_BAD_GATEWAY,
            new BadGatewayException(MessageConstantsCore.MSG_BAD_GATEWAY)
        },
        {
            HttpStatusCodeConstantsCore.CFG_SERVICE_UNAVAILABLE,
            new ServiceUnavailableException(MessageConstantsCore.MSG_SERVICE_UNAVAILABLE)
        },
        {
            HttpStatusCodeConstantsCore.CFG_GATEWAY_TIMEOUT,
            new GatewayTimeoutException(MessageConstantsCore.MSG_GATEWAY_TIMEOUT)
        },
        {
            HttpStatusCodeConstantsCore.CFG_HTTP_VERSION_NOT_SUPPORTED,
            new HttpVersionNotSupportedException(MessageConstantsCore.MSG_HTTP_VERSION_NOT_SUPPORTED)
        },
        {
            HttpStatusCodeConstantsCore.CFG_VARIANT_ALSO_NEGOTIATES,
            new VariantAlsoNegotiatesException(MessageConstantsCore.MSG_VARIANT_ALSO_NEGOTIATES)
        },
        {
            HttpStatusCodeConstantsCore.CFG_INSUFFICIENT_STORAGE,
            new InsufficientStorageException(MessageConstantsCore.MSG_INSUFFICIENT_STORAGE)
        },
        {
            HttpStatusCodeConstantsCore.CFG_LOOP_DETECTED,
            new LoopDetectedException(MessageConstantsCore.MSG_LOOP_DETECTED)
        },
        {
            HttpStatusCodeConstantsCore.CFG_BANDWIDTH_LIMIT_EXCEEDED,
            new BandwidthLimitExceededException(MessageConstantsCore.MSG_BANDWIDTH_LIMIT_EXCEEDED)
        },
        {
            HttpStatusCodeConstantsCore.CFG_NOT_EXTENDED,
            new NotExtendedException(MessageConstantsCore.MSG_NOT_EXTENDED)
        },
        {
            HttpStatusCodeConstantsCore.CFG_NETWORK_AUTHENTICATION_REQUIRED,
            new NetworkAuthenticationRequiredException(MessageConstantsCore.MSG_NETWORK_AUTHENTICATION_REQUIRED)
        }
    };
}
