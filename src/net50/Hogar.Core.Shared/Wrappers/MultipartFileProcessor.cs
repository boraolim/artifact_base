using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.WebUtilities;

using Utilities.Core.Shared.Services;
using Utilities.Core.Shared.Settings;
using Utilities.Core.Shared.Exceptions;
using Utilities.Core.Shared.Extensions;

using MainConstantsCore = Utilities.Core.Shared.Constants.MainConstants;
using FormatConstantsCore = Utilities.Core.Shared.Constants.FormatConstants;
using MessageConstantsCore = Utilities.Core.Shared.Constants.MessageConstants;

namespace Utilities.Core.Shared.Wrappers
{
    public class MultipartFileProcessor : IMultipartFileProcessor
    {
        private readonly UploadSettings _settings;
        public MultipartFileProcessor(IOptions<UploadSettings> settings) =>
            _settings = Guard.AgainstNull(settings.Value, nameof(settings.Value));

        public async Task<Result<ResultUploadRespose>> ProcessAsync<TResult>(HttpRequest request,
                                                                             Func<string, Stream, Task<TResult>> processor)
        {
            try
            {
                ValidateRequestHeaders(request);

                var boundary = ExtractBoundary(request.ContentType);

                if(request.Body.CanSeek)
                    request.Body.Position = 0;

                var reader = new MultipartReader(boundary, request.Body)
                {
                    HeadersLengthLimit = _settings.HeadersLengthLimit
                };

                var uploadedFiles = new List<SuccessFileResponse>();
                var notUploadedFiles = new List<NotSuccessFileResponse>();

                int fileCount = 0; long totalUploadSize = 0; MultipartSection section;
                while((section = await reader.ReadNextSectionAsync()) is not null)
                {
                    if(IsValidFileSection(section, out _, out var fileName))
                    {
                        if(++fileCount > _settings.MaxFileCount)
                            throw new BadRequestException(string.Format(FormatConstantsCore.FMT_MAX_FILE_COUNT_EXCEEDED, _settings.MaxFileCount));

                        var memoryStream = await CopyToMemoryStreamAsync(section.Body);
                        var fileSize = memoryStream.Length;

                        if(IsFileSizeValid(fileSize, totalUploadSize, fileName, notUploadedFiles))
                        {
                            var result = await processor(fileName, memoryStream);
                            bool isSuccess = result.MapTo<Result<SuccessFileResponse>>().Succeded;

                            if(isSuccess)
                            {
                                uploadedFiles.Add(new SuccessFileResponse(fileName, fileSize));
                                totalUploadSize += fileSize;
                            }
                            else
                            {
                                notUploadedFiles.Add(new NotSuccessFileResponse(fileName));
                            }
                        }
                    }
                }

                if(fileCount == 0)
                    throw new BadRequestException(MessageConstantsCore.MSG_REQUEST_MULTIPART_EMPTY);

                var response = new ResultUploadRespose(
                    new DetailSuccessFileListResponse(uploadedFiles, uploadedFiles.Count, totalUploadSize),
                    notUploadedFiles);

                return await Result<ResultUploadRespose>.SuccessAsync(response);
            }
            catch(UnsupportedMediaTypeException)
            {
                throw;
            }
            catch(Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }
        }

        private static void ValidateRequestHeaders(HttpRequest request)
        {
            if(string.IsNullOrWhiteSpace(request.ContentType) ||
                !request.ContentType.StartsWith(MainConstantsCore.CFG_PREFIX_MULTIPART, StringComparison.OrdinalIgnoreCase))
            {
                throw new UnsupportedMediaTypeException(MessageConstantsCore.MSG_IS_NOT_MULTIPART);
            }
        }

        private static string ExtractBoundary(string contentType)
        {
            var boundary = HeaderUtilities.RemoveQuotes(MediaTypeHeaderValue.Parse(contentType).Boundary).Value;

            if(string.IsNullOrWhiteSpace(boundary))
                throw new BadRequestException(MessageConstantsCore.MSG_BOUNDARY_NOT_FOUND);

            return boundary;
        }

        private static bool IsValidFileSection(MultipartSection section,
                                               out ContentDispositionHeaderValue contentDisposition,
                                               out string fileName)
        {
            fileName = null;
            contentDisposition = null;

            if(!ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out contentDisposition))
                return false;

            fileName = Path.GetFileName(contentDisposition.FileName.Value ?? string.Empty);

            return !string.IsNullOrWhiteSpace(fileName) &&
                   contentDisposition.DispositionType == MainConstantsCore.CFG_PREFIX_FORM_DATA;
        }

        private static async Task<MemoryStream> CopyToMemoryStreamAsync(Stream sectionBody)
        {
            var memoryStream = new MemoryStream();
            await sectionBody.CopyToAsync(memoryStream);
            memoryStream.Position = 0;
            return memoryStream;
        }

        private bool IsFileSizeValid(long fileSize, long currentTotal, string fileName,
                                     List<NotSuccessFileResponse> notUploadedFiles)
        {
            if(fileSize > _settings.MaxTotalFileSizeBytes)
            {
                notUploadedFiles.Add(new NotSuccessFileResponse(fileName));
                return false;
            }

            if(currentTotal + fileSize > _settings.MaxTotalFileSizeBytes)
            {
                throw new BadRequestException(
                    string.Format(FormatConstantsCore.FMT_MAX_FILE_SIZE_EXCEEDED, _settings.MaxTotalFileSizeBytes));
            }

            return true;
        }
    }
}
