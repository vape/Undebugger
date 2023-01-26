using System;
using UnityEngine;

namespace Undebugger.Services.Log
{
    public struct LogMessage
    {
        public int Id;
        public DateTimeOffset Time;
        public string Message;
        public string StackTrace;
        public LogType Type;
    }
}
