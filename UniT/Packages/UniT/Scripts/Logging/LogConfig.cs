namespace UniT.Logging
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public class LogConfig
    {
        public LogLevel Level { get; set; } = LogLevel.Info;
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