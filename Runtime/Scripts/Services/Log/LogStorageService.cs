#if (UNITY_EDITOR || DEBUG || UNDEBUGGER) && !UNDEBUGGER_DISABLE
#define UNDEBUGGER_ENABLED
#endif

using System;
using Undebugger.Utility;
using UnityEngine;

namespace Undebugger.Services.Log
{
    public delegate void MessageAddedDelegate(in LogMessage message);

#if UNDEBUGGER_ENABLED

    internal class LogStorageService
    {
        public const int Capacity = 1000;
        public const int BuffersCount = (int)LogTypeMask.All;

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

        private int totalInfo;
        private int totalErrors;
        private int totalWarnings;
        private CircularBuffer<LogMessage>[] buffers = new CircularBuffer<LogMessage>[BuffersCount];
        private int idgen;

        public LogStorageService()
        {
            for (int i = 0; i < buffers.Length; i++)
            {
                buffers[i] = new CircularBuffer<LogMessage>(Capacity);
            }

            Application.logMessageReceived += AddMessage;
        }

        public bool TryFindById(int id, out LogMessage message)
        {
            for (int k = BuffersCount - 1; k >= 0; --k)
            {
                var buffer = buffers[k];
                var array = buffer.GetRawArray();
                var count = buffer.Count < array.Length ? buffer.Count : array.Length;

                for (int i = 0; i < count; ++i)
                {
                    if (array[i].Id == id)
                    {
                        message = array[i];
                        return true;
                    }
                }
            }

            message = default;
            return false;
        }

        private CircularBuffer<LogMessage> GetBuffer(LogTypeMask mask)
        {
            return buffers[(int)mask - 1];
        }

        public int GetTotalCount(LogTypeMask mask)
        {
            switch (mask)
            {
                case LogTypeMask.Info:
                    return totalInfo;
                case LogTypeMask.Warning:
                    return totalWarnings;
                case LogTypeMask.Error:
                    return totalErrors;
            }

            return GetCount(mask);
        }

        public int GetCount(LogTypeMask mask)
        {
            if (mask == LogTypeMask.None)
            {
                return 0;
            }

            return GetBuffer(mask).Count;
        }

        public ref LogMessage GetMessage(LogTypeMask mask, int index)
        {
            return ref GetBuffer(mask).Get(index);
        }

        private void AddMessage(string condition, string stackTrace, LogType type)
        {
            var id = ++idgen;
            var message = new LogMessage()
            {
                Id = id,
                Message = condition,
                StackTrace = stackTrace,
                Type = type,
                Time = DateTimeOffset.Now
            };

            var mask = (int)GetMask(in type);

            for (int i = 0; i < BuffersCount; ++i)
            {
                if ((mask & (i + 1)) != 0)
                {
                    buffers[i].PushFront(message);
                }
            }

            switch (mask)
            {
                case (int)LogTypeMask.Error:
                    totalErrors++;
                    break;
                case (int)LogTypeMask.Warning:
                    totalWarnings++;
                    break;
                default:
                    totalInfo++;
                    break;
            }

            MessageAdded?.Invoke(in message);
        }

        public static LogTypeMask GetMask(in LogType type)
        {
            switch (type)
            {
                case LogType.Error:
                case LogType.Exception:
                case LogType.Assert:
                    return LogTypeMask.Error;
                case LogType.Warning:
                    return LogTypeMask.Warning;
                default:
                    return LogTypeMask.Info;
            }
        }

        public void Clear()
        {
            for (int i = 0; i < buffers.Length; ++i)
            {
                buffers[i].Clear();
            }

            totalInfo = 0;
            totalErrors = 0;
            totalWarnings = 0;
        }
    }

#else

    internal class LogStorageService
    {
        public const int Capacity = 1000;
        public const int BuffersCount = (int)LogTypeMask.All;

        public static readonly LogStorageService Instance = new LogStorageService();

        private static LogMessage message;

        public event MessageAddedDelegate MessageAdded;

        public int GetTotalCount(LogTypeMask mask)
        {
            return 0;
        }

        public int GetCount(LogTypeMask mask)
        {
            return 0;
        }

        public ref LogMessage GetMessage(LogTypeMask mask, int index)
        {
            return ref message;
        }

        public bool TryFindById(int id, out LogMessage message)
        {
            message = default;
            return false;
        }

        public void Clear()
        { }
    }

#endif
}

