using UnityEngine;

namespace Deszz.Undebugger
{
    internal static class L
    {
        private static UndebuggerLogger logger = new UndebuggerLogger();

        public static void Info(string message)
        {
            logger.Info(message);
        }

        public static void Warning(string message)
        {
            logger.Warning(message);
        }

        public static void Error(string message)
        {
            logger.Error(message);
        }
    }

    internal class UndebuggerLogger
    {
        public void Info(string message)
        {
            Debug.Log(message);
        }

        public void Warning(string message)
        {
            Debug.LogWarning(message);
        }

        public void Error(string message)
        {
            Debug.LogError(message);
        }
    }
}
