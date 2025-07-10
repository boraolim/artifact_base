using Hogar.Core.Shared.Constants;

namespace Hogar.Core.Shared.Settings
{
    public class UploadSettings
    {
        public long MaxFileSizeBytes { get; set; }
        public int MaxFileCount { get; set; }
        public long MaxTotalFileSizeBytes { get; set; }
        public string UploadDirectory { get; set; }
        public int HeadersLengthLimit { get; set; } = MainConstants.CFG_BUFFER_VALUE;
        public string[] AllowedFileExtensions { get; set; }
    }
}
