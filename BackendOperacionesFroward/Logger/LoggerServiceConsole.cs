using BackendOperacionesFroward.Shared.Utilities;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace BackendOperacionesFroward.Logger
{
    public enum LOG_TYPES
    {
        INFO,
        ERROR,
        ALERT,
        EXCEPTION
    }

    public class LoggerServiceConsole : ILogger
    {
        private static LoggerServiceConsole _instance = null;

        private string Application { set; get; }

        private string Id { set; get; }

        private string ClassSender { set; get; }

        private StreamWriter File { get; set; }

        private string PathFile { get; }

        public LoggerServiceConsole(string application)
        {
            PathFile = AppSettings.GetConfigurationOptions().PATH_LOG_FILE;

            Application = application;
            Id = Guid.NewGuid().ToString();

            AppendIntoFile($"LOGGER CREATED {Id}");
            Console.WriteLine("LOGGER CREATED {0}", Id);
        }

        private void AppendIntoFile(string line)
        {
            if (!string.IsNullOrEmpty(PathFile))
            {
                try
                {
                    File = new(PathFile, true);
                    File.WriteLine(DateTime.Now.ToString() + '\t' + line);
                    File.Close();
                }
                catch (Exception) { }
            }
        }

        public static LoggerServiceConsole CreateInstanceLoger(string application = "")
        {
            if (_instance == null)
                _instance = new LoggerServiceConsole(application);
            return _instance;
        }

        public ILogger WithApplication(string application)
        {
            Application = application;
            return this;
        }

        public ILogger WithClass(string className)
        {
            ClassSender = className;
            return this;
        }

        public void Write(LOG_TYPES type, string data)
        {
            AppendIntoFile(Application + " - " + $"{ClassSender}" + " - " + GetString(type) + ": " + "\t" + data);
            Console.Write(Application + " - " + $"{ClassSender}" + " - " + GetString(type) + ": " + "\t" + data);
        }

        public void WriteLine(LOG_TYPES type, string data)
        {
            AppendIntoFile(Application + " - " + $"{ClassSender}" + " - " + GetString(type) + ": " + "\t" + data);
            Console.WriteLine(Application + " - " + $"{ClassSender}" + " - " + GetString(type) + ": " + "\t" + data);
        }

        public void WriteLineException(Exception e, string data = "")
        {
            AppendIntoFile(Application + " - " + $"{ClassSender}" + " - " + GetString(LOG_TYPES.EXCEPTION) + ": " + data + "\n\t" + e.Message + "\n\t" + e.GetBaseException());
            Console.WriteLine(Application + " - " + $"{ClassSender}" + " - " + GetString(LOG_TYPES.EXCEPTION) + ": " + data + "\n\t" + e.Message + "\n\t" + e.GetBaseException());
        }

        public string GetString(LOG_TYPES log)
        {
            return log switch
            {
                LOG_TYPES.INFO => "INFO",
                LOG_TYPES.ERROR => "ERROR",
                LOG_TYPES.ALERT => "ALERT",
                LOG_TYPES.EXCEPTION => "EXCEPTION",
                _ => "LEVEL ERROR",
            };
        }
    }
}
