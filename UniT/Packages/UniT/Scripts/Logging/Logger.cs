namespace UniT.Logging
{
    using System;
    using UniT.Extensions;
    using UnityEngine;

    public class Logger : BaseLogger
    {
        protected override void Debug(string message, Color? color = null)
        {
            UnityEngine.Debug.Log(message.WithColor(color));
        }

        protected override void Info(string message, Color? color = null)
        {
            UnityEngine.Debug.Log(message.WithColor(color));
        }

        protected override void Warning(string message, Color? color = null)
        {
            UnityEngine.Debug.LogWarning(message.WithColor(color));
        }

        protected override void Error(string message, Color? color = null)
        {
            UnityEngine.Debug.LogError(message.WithColor(color));
        }

        protected override void Critical(string message, Color? color = null)
        {
            UnityEngine.Debug.LogError(message.WithColor(color));
        }

        protected override void Exception(Exception exception)
        {
            UnityEngine.Debug.LogException(exception);
        }
    }
}