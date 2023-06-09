namespace UniT.Core.Logging
{
    using System;

    public class LogConfig
    {
        public readonly LogLevel logLevel;

        public LogConfig(LogLevel logLevel = LogLevel.All)
        {
            this.logLevel = logLevel;
        }
    }

    [Flags]
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