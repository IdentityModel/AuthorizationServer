using System.Diagnostics;

namespace Thinktecture.AuthorizationServer
{
    /// <summary>
    /// Helper class for Tracing
    /// </summary>
    public static class Tracing
    {
        [DebuggerStepThrough]
        public static void Start(string message)
        {
            TraceEvent(TraceEventType.Start, message);
        }

        [DebuggerStepThrough]
        public static void Stop(string message)
        {
            TraceEvent(TraceEventType.Stop, message);
        }

        [DebuggerStepThrough]
        public static void Information(string message)
        {
            TraceEvent(TraceEventType.Information, message);
        }

        [DebuggerStepThrough]
        public static void InformationFormat(string message, params object[] objects)
        {
            TraceEventFormat(TraceEventType.Information, message, objects);
        }

        [DebuggerStepThrough]
        public static void Warning(string message)
        {
            TraceEvent(TraceEventType.Warning, message);
        }

        [DebuggerStepThrough]
        public static void WarningFormat(string message, params object[] objects)
        {
            TraceEventFormat(TraceEventType.Warning, message, objects);
        }

        [DebuggerStepThrough]
        public static void Error(string message)
        {
            TraceEvent(TraceEventType.Error, message);
        }

        [DebuggerStepThrough]
        public static void ErrorFormat(string message, params object[] objects)
        {
            TraceEventFormat(TraceEventType.Error, message, objects);
        }

        [DebuggerStepThrough]
        public static void Verbose(string message)
        {
            TraceEvent(TraceEventType.Verbose, message);
        }

        [DebuggerStepThrough]
        public static void VerboseFormat(string message, params object[] objects)
        {
            TraceEventFormat(TraceEventType.Verbose, message, objects);
        }

        [DebuggerStepThrough]
        public static void TraceEventFormat(TraceEventType type, string message, params object[] objects)
        {
            var format = string.Format(message, objects);
            TraceEvent(type, format);
        }

        [DebuggerStepThrough]
        public static void TraceEvent(TraceEventType type, string message)
        {
            TraceSource ts = new TraceSource("Thinktecture.IdentityServer");
            ts.TraceEvent(type, 0, message);
        }
    }
}
