using System;
using UnityEngine;

namespace Undebugger.Services.Log
{
    internal class LogStorageService
    {
        public const int BufferSize = 1024;

        public delegate void MessageAddedDelegate(LogMessage message);

        public static LogStorageService Instance
        {
            get
            {
                return instance;
            }
        }

        private static LogStorageService instance;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void Initialize()
        {
            if (instance != null)
            {
                return;
            }

            instance = new LogStorageService();
        }

        public event MessageAddedDelegate MessageAdded;

        public int Count
        {
            get
            {
                return head > tail ? messages.Length - head + tail : tail - head;
            }
        }

        private LogMessage[] messages = new LogMessage[BufferSize];
        private int head;
        private int tail;

        public LogStorageService()
        {
            Application.logMessageReceived += AddMessage;
        }

        public LogMessage GetMessage(int index)
        {
            return messages[(head + index) % messages.Length];
        }

        private void AddMessage(string condition, string stackTrace, LogType type)
        {
            var index = tail;

            messages[index] = new LogMessage()
            {
                Message = condition,
                StackTrace = stackTrace,
                Type = type,
                Time = DateTimeOffset.Now
            };

            tail = (tail + 1) % messages.Length;
            head = tail <= head ? tail + 1 : head;

            MessageAdded?.Invoke(messages[index]);
        }
    }
}
