using System;

namespace BackendOperacionesFroward.Logger
{
    public interface ILogger
    {
        public void Write(LOG_TYPES type, string data);
        public void WriteLine(LOG_TYPES type, string data);
        public void WriteLineException(Exception e, string data = "");
        public string GetString(LOG_TYPES log);
        public ILogger WithApplication(string application);
        public ILogger WithClass(string className);
    }
}
