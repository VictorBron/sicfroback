using BackendOperacionesFroward.Settings.Objects;

namespace BackendOperacionesFroward.Settings
{
    public class ConfigurationOptions
    {
        public string PATH_LOG_FILE { get; set; }

        public KestrelStringsConfiguration Kestrel { get; set; } = new();

        public ExpirationTimesConfiguration ExpirationTimes { get; set; } = new();

        public EmailConfiguration Email { get; set; } = new();

        public FrontInformation FrontInformation { get; set; } = new();
    }
}
