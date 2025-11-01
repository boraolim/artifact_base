using System.Text.Json;

using Xunit;

using Utilities.Core.Shared.Settings;

namespace Utilities.Core.Shared.Tests
{
    public class SettingsTests
    {
        [Fact]
        public void PerformanceSettings_ShouldAssignProperties()
        {
            var settings = new PerformanceSettings
            {
                WarningThresholdMiliseconds = 500,
                TimeoutMiliseconds = 3000
            };

            Assert.Equal(500, settings.WarningThresholdMiliseconds);
            Assert.Equal(3000, settings.TimeoutMiliseconds);
        }

        [Fact]
        public void UploadSettings_ShouldAssignProperties()
        {
            var settings = new UploadSettings
            {
                MaxFileSizeBytes = 1024 * 1024,
                MaxFileCount = 10,
                MaxTotalFileSizeBytes = 10 * 1024 * 1024,
                UploadDirectory = "/uploads",
                HeadersLengthLimit = 1024,
                AllowedFileExtensions = new[] { ".jpg", ".png" }
            };

            Assert.Equal(1024 * 1024, settings.MaxFileSizeBytes);
            Assert.Equal(10, settings.MaxFileCount);
            Assert.Equal(10 * 1024 * 1024, settings.MaxTotalFileSizeBytes);
            Assert.Equal("/uploads", settings.UploadDirectory);
            Assert.Equal(1024, settings.HeadersLengthLimit);
            Assert.Equal(new[] { ".jpg", ".png" }, settings.AllowedFileExtensions);
        }

        [Fact]
        public void PerformanceSettings_ShouldSerializeDeserializeJson()
        {
            var original = new PerformanceSettings
            {
                WarningThresholdMiliseconds = 700,
                TimeoutMiliseconds = 5000
            };

            var json = JsonSerializer.Serialize(original);
            var deserialized = JsonSerializer.Deserialize<PerformanceSettings>(json);

            Assert.Equal(original.WarningThresholdMiliseconds, deserialized.WarningThresholdMiliseconds);
            Assert.Equal(original.TimeoutMiliseconds, deserialized.TimeoutMiliseconds);
        }

        [Fact]
        public void UploadSettings_ShouldAllowNullProperties()
        {
            var settings = new UploadSettings
            {
                UploadDirectory = null,
                AllowedFileExtensions = null
            };

            Assert.Null(settings.UploadDirectory);
            Assert.Null(settings.AllowedFileExtensions);
        }
    }
}
