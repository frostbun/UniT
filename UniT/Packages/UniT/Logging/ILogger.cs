namespace UniT.Logging
{
    using System;
    using UnityEngine;

    public interface ILogger
    {
        public void Log(string message, Color? color = null);

        public void Warning(string message, Color? color = null);

        public void Error(string message, Color? color = null);

        public void Exception(Exception exception);
    }
}