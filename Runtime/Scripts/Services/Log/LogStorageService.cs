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

        public int TotalInfo
        { get; private set; }
        public int TotalErrors
        { get; private set; }
        public int TotalWarnings
        { get; private set; }

        private LogMessage[] messages = new LogMessage[BufferSize];
        private int head;
        private int tail;
        private int idgen;

        public LogStorageService()
        {
            Application.logMessageReceived += AddMessage;
        }

        public bool TryFindById(int id, out LogMessage message)
        {
            for (int i = 0; i < messages.Length; ++i)
            {
                if (messages[i].Id == id)
                {
                    message = messages[i];
                    return true;
                }
            }

            message = default;
            return false;
        }

        public ref LogMessage GetMessage(int index)
        {
            return ref messages[(head + index) % messages.Length];
        }

        private void AddMessage(string condition, string stackTrace, LogType type)
        {
            var index = tail;
            var id = ++idgen;

            messages[index] = new LogMessage()
            {
                Id = id,
                Message = condition,
                StackTrace = stackTrace,
                Type = type,
                Time = DateTimeOffset.Now
            };

            switch (type)
            {
                case LogType.Exception:
                case LogType.Error:
                    TotalErrors++;
                    break;
                case LogType.Warning:
                    TotalWarnings++;
                    break;
                default:
                    TotalInfo++;
                    break;
            }

            tail = (tail + 1) % messages.Length;
            head = tail <= head ? tail + 1 : head;

            MessageAdded?.Invoke(messages[index]);
        }
    }
}
