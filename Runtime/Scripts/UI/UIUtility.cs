using System.Globalization;

namespace Deszz.Undebugger.UI
{
    public static class UIUtility
    {
        private static readonly string[] fileSizeUnits = { "B", "KB", "MB", "GB", "TB", "PB" };
        private static readonly CultureInfo culture = CultureInfo.CurrentCulture;

        public static string ConvertMegabyteSizeToReadableString(long size)
        {
            return ConvertBytesSizeToReadableString(size * 1024 * 1024);
        }

        public static string ConvertBytesSizeToReadableString(long size, bool tryLocalizeUnit = false)
        {
            var sign = size < 0 ? culture.NumberFormat.NegativeSign : "";
            size = size == long.MinValue ? System.Math.Abs(size + 1) : System.Math.Abs(size);

            if (size < 1024)
            {
                return $"{sign}{size}{GetFileSizeUnit(0)}";
            }

            var len = (double)size;
            var order = 0;
            while (len >= 1024 && order < fileSizeUnits.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            return $"{sign}{len.ToString("N2", culture)}{GetFileSizeUnit(order)}";
        }

        private static string GetFileSizeUnit(int order)
        {
            if (order < 0 || order >= fileSizeUnits.Length)
            {
                return string.Empty;
            }

            return fileSizeUnits[order];
        }
    }
}
