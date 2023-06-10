namespace UniT.Logging
{
    using System;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public class LogConfig
    {
        public readonly LogLevel logLevel;

        public LogConfig(LogLevel logLevel = LogLevel.All)
        {
            this.logLevel = logLevel;
        }
    }

    [Flags]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum LogLevel
    {
        None      = 0,
        Debug     = 1 << 0,
        Info      = 1 << 1,
        Warning   = 1 << 2,
        Error     = 1 << 3,
        Critical  = 1 << 4,
        Exception = 1 << 5,
        All       = -1,
    }
}