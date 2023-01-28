namespace Undebugger.Utility
{
    internal static class Formatter
    {
        public static string Format(long bytes)
        {
            const long kilobyte = 1024;
            const long megabyte = kilobyte * 1024;
            const long gigabyte = megabyte * 1024;

            if (bytes > gigabyte)
            {
                return $"{bytes / (float)gigabyte:0.0}GB";
            }
            else if (bytes > megabyte)
            {
                return $"{bytes / (float)megabyte:0.0}MB";
            }
            else if (bytes > kilobyte)
            {
                return $"{bytes / (float)kilobyte:0.0}KB";
            }

            return $"{bytes}B";
        }
    }
}
