using System;
using UnityEngine;

namespace Deszz.Undebugger
{
    public struct LogMessage
    {
        public DateTimeOffset Time;
        public string Message;
        public string StackTrace;
        public LogType Type;
    }

    public class UndebuggerLogsStorage : IDisposable
    {
        public delegate void MessageAddedDelegate(LogMessage message);

        public static UndebuggerLogsStorage Instance
        {
            get
            {
                Initialize();
                return instance;
            }
        }

        private static UndebuggerLogsStorage instance;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void Initialize()
        {
            if (instance != null)
            {
                return;
            }

            instance = new UndebuggerLogsStorage(1024);
        }

        public event MessageAddedDelegate MessageAdded;

        public int Count
        {
            get
            {
                return head > tail ? size - head + tail : tail - head;
            }
        }

        public int BufferSize => size;

        private LogMessage[] messages;

        private int size;
        private int head;
        private int tail;
        private bool disposed;

        public UndebuggerLogsStorage(int bufferSize)
        {
            size = bufferSize;
            messages = new LogMessage[bufferSize];
            
            Application.logMessageReceived += AddMessage;
        }

        public LogMessage GetMessage(int index)
        {
            return messages[(head + index) % size];
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            Application.logMessageReceived -= AddMessage;
            disposed = true;
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

            tail = (tail + 1) % size;
            head = tail <= head ? tail + 1 : head;

            MessageAdded?.Invoke(messages[index]);
        }
    }
}
