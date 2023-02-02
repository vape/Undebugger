using System;
using UnityEngine;

namespace Undebugger.Services.Log
{
    [Flags]
    public enum LogTypeMask
    {
        None = 0,
        Info = 1,
        Warning = 2,
        Error = 4,
        All = Info | Warning | Error
    }

    public struct LogMessage
    {
        public int Id;
        public DateTimeOffset Time;
        public string Message;
        public string StackTrace;
        public LogType Type;
    }
}
