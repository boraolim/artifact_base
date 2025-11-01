using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using Xunit;

using Utilities.Core.Shared.Dictionary;
using Utilities.Core.Shared.Exceptions;

using HttpStatusCodeConstantsCore = Utilities.Core.Shared.Constants.HttpStatusCodeConstants;

namespace Utilities.Core.Shared.Tests
{
    public class ExceptionDictionaryTests
    {
        [Theory]
        [InlineData(HttpStatusCodeConstantsCore.CFG_BAD_REQUEST, typeof(BadRequestException))]
        [InlineData(HttpStatusCodeConstantsCore.CFG_UNAUTHORIZED, typeof(UnauthorizedException))]
        [InlineData(HttpStatusCodeConstantsCore.CFG_NOT_FOUND, typeof(NotFoundSourceException))]
        [InlineData(HttpStatusCodeConstantsCore.CFG_INTERNAL_SERVER_ERROR, typeof(InternalServerErrorException))]
        public void GetStatusHTTPCodes_ShouldContainCorrectExceptionType(ushort statusCode, Type expectedExceptionType)
        {
            // Act
            var dictionary = ExceptionDictionary.GetStatusHTTPCodes();

            // Assert
            Assert.True(dictionary.ContainsKey(statusCode), $"Missing HTTP status code: {statusCode}");
            Assert.IsType(expectedExceptionType, dictionary[statusCode]);
        }

        [Fact]
        public void GetStatusHTTPCodes_ShouldNotContainUnknownStatusCode()
        {
            // Act
            var dictionary = ExceptionDictionary.GetStatusHTTPCodes();

            // Assert
            ushort unknownCode = 999;
            Assert.False(dictionary.ContainsKey(unknownCode));
        }

        [Fact]
        public void GetStatusHTTPCodes_ShouldReturnAllExpectedStatusCodes()
        {
            // Act
            var dictionary = ExceptionDictionary.GetStatusHTTPCodes();

            // Assert
            Assert.NotNull(dictionary);
            Assert.NotEmpty(dictionary);

            // Opcional: Validar que todos los códigos esperados están presentes
            var expectedCodes = new ushort[]
            {
                HttpStatusCodeConstantsCore.CFG_BAD_REQUEST,
                HttpStatusCodeConstantsCore.CFG_UNAUTHORIZED,
                HttpStatusCodeConstantsCore.CFG_PAYMENT_REQUIRED,
                HttpStatusCodeConstantsCore.CFG_FORBIDDEN,
                HttpStatusCodeConstantsCore.CFG_NOT_FOUND,
                HttpStatusCodeConstantsCore.CFG_METHOD_NOT_ALLOWED,
                HttpStatusCodeConstantsCore.CFG_NOT_ACCEPTABLE,
                HttpStatusCodeConstantsCore.CFG_PROXY_AUTHENTICATION_REQUIRED,
                HttpStatusCodeConstantsCore.CFG_REQUEST_TIMEOUT,
                HttpStatusCodeConstantsCore.CFG_CONFLICT,
                HttpStatusCodeConstantsCore.CFG_GONE,
                HttpStatusCodeConstantsCore.CFG_LENGTH_REQUIRED,
                HttpStatusCodeConstantsCore.CFG_PRECONDITION_FAILED,
                HttpStatusCodeConstantsCore.CFG_REQUEST_ENTITY_TOO_LARGE,
                HttpStatusCodeConstantsCore.CFG_REQUEST_URI_TOO_LONG,
                HttpStatusCodeConstantsCore.CFG_UNSUPPORTED_MEDIA_TYPE,
                HttpStatusCodeConstantsCore.CFG_REQUESTED_RANGE_NOT_SATISFIABLE,
                HttpStatusCodeConstantsCore.CFG_EXPECTATION_FAILED,
                HttpStatusCodeConstantsCore.CFG_IM_A_TEAPOT,
                HttpStatusCodeConstantsCore.CFG_METHOD_FAILURE,
                HttpStatusCodeConstantsCore.CFG_UNPROCESSABLE_ENTITY,
                HttpStatusCodeConstantsCore.CFG_LOCKED,
                HttpStatusCodeConstantsCore.CFG_FAILED_DEPENDENCY,
                HttpStatusCodeConstantsCore.CFG_TOO_EARLY,
                HttpStatusCodeConstantsCore.CFG_UPGRADE_REQUIRED,
                HttpStatusCodeConstantsCore.CFG_PRECONDITION_REQUIRED,
                HttpStatusCodeConstantsCore.CFG_TOO_MANY_REQUESTS,
                HttpStatusCodeConstantsCore.CFG_REQUEST_HEADER_FIELDS_TOO_LARGE,
                HttpStatusCodeConstantsCore.CFG_NO_RESPONSE_NGINX,
                HttpStatusCodeConstantsCore.CFG_RETRY_WITH_IIS,
                HttpStatusCodeConstantsCore.CFG_BLOCKED_BY_WP_CTRLS,
                HttpStatusCodeConstantsCore.CFG_UNAVAILABLE_FOR_LEGAL_REASONS,
                HttpStatusCodeConstantsCore.CFG_CLIENT_CLOSED_REQUEST_NGINX,
                HttpStatusCodeConstantsCore.CFG_INTERNAL_SERVER_ERROR,
                HttpStatusCodeConstantsCore.CFG_NOT_IMPLEMENTED,
                HttpStatusCodeConstantsCore.CFG_BAD_GATEWAY,
                HttpStatusCodeConstantsCore.CFG_SERVICE_UNAVAILABLE,
                HttpStatusCodeConstantsCore.CFG_GATEWAY_TIMEOUT,
                HttpStatusCodeConstantsCore.CFG_HTTP_VERSION_NOT_SUPPORTED,
                HttpStatusCodeConstantsCore.CFG_VARIANT_ALSO_NEGOTIATES,
                HttpStatusCodeConstantsCore.CFG_INSUFFICIENT_STORAGE,
                HttpStatusCodeConstantsCore.CFG_LOOP_DETECTED,
                HttpStatusCodeConstantsCore.CFG_BANDWIDTH_LIMIT_EXCEEDED,
                HttpStatusCodeConstantsCore.CFG_NOT_EXTENDED,
                HttpStatusCodeConstantsCore.CFG_NETWORK_AUTHENTICATION_REQUIRED,
                HttpStatusCodeConstantsCore.CFG_NETWORK_AUTHENTICATION_REQUIRED
            };

            foreach(var code in expectedCodes)
            {
                Assert.True(dictionary.ContainsKey(code), $"Missing HTTP status code: {code}");
                Assert.NotNull(dictionary[code]);
                Assert.IsAssignableFrom<Exception>(dictionary[code]);
            }
        }
    }
}
