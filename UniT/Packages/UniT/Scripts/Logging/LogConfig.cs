namespace UniT.Logging
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public class LogConfig
    {
        public readonly LogLevel logLevel;

        public LogConfig(LogLevel logLevel)
        {
            this.logLevel = logLevel;
        }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error,
        Critical,
        Exception,
    }
}