namespace UniT.Logging
{
    using System;
    using UnityEngine;

    public interface ILogger
    {
        public void Debug(string message, Color? color = null);

        public void Info(string message, Color? color = null);

        public void Warning(string message, Color? color = null);

        public void Error(string message, Color? color = null);

        public void Critical(string message, Color? color = null);

        public void Exception(Exception exception);
    }
}