using BackendOperacionesFroward.Settings.Objects;
using BackendOperacionesFroward.Shared.Utilities;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BackendOperacionesFroward
{
    public class Program
    {
        public static void Main()
        {
            string[] args = Environment.GetCommandLineArgs();
            var host = CreateWebHostBuilder(args).Build();
            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            KestrelStringsConfiguration kestrelStrings = AppSettings.GetConfigurationOptions().Kestrel;

            List<string> httpBase = kestrelStrings.HTTP.Split(';').ToList();
            httpBase.AddRange(kestrelStrings.HTTPS.Split(';').ToList());

            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseUrls(httpBase.ToArray<string>());

        }

    }
}
