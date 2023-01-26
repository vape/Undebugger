using System;
using UnityEngine;

namespace Undebugger.Services.Log
{
    public struct LogMessage
    {
        public DateTimeOffset Time;
        public string Message;
        public string StackTrace;
        public LogType Type;
    }
}
