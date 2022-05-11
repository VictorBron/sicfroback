using BackendOperacionesFroward.Settings;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace BackendOperacionesFroward.Shared.Utilities
{
    public class AppSettings
    {
        public static IConfigurationRoot IConfigurationRoot { get; set; }

        public static ConfigurationOptions ConfigurationOptions { get; set; }

        public static IConfigurationRoot GetAppSettings(bool forceReload = false)
        {
            if (forceReload)
                if (IConfigurationRoot != null)
                    return IConfigurationRoot;

            IConfigurationRoot = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(Constants.APP_SETTINGS_NAME, optional: false)
                .Build();
            return IConfigurationRoot;
        }

        public static IConfigurationSection GetConfigurationSection() {
            if (IConfigurationRoot == null)
                GetAppSettings();

            return IConfigurationRoot.GetSection(Constants.CONFIGURATION_OPTIONS);
        }

        public static ConfigurationOptions GetConfigurationOptions(bool forceReload = false) {

            if (forceReload)
            {
                GetAppSettings();
            }
            else { 
                if (ConfigurationOptions != null)
                    return ConfigurationOptions;

                if (IConfigurationRoot == null)
                    GetAppSettings();
            }
            ConfigurationOptions = new();
            IConfigurationRoot.Bind(Constants.CONFIGURATION_OPTIONS, ConfigurationOptions);

            return ConfigurationOptions;
        }
    }
}
