using System.Globalization;
using UnityEngine;

namespace Undebugger.UI
{
    public static class UIUtility
    {
        private static readonly string[] fileSizeUnits = { "B", "KB", "MB", "GB", "TB", "PB" };
        private static readonly CultureInfo culture = CultureInfo.CurrentCulture;

        public static void Expand(this RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.one;
        }

        public static void Float(this RectTransform rect)
        {
            var size = rect.rect.size;

            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.offsetMin = size * -rect.pivot;
            rect.offsetMax = size * (Vector2.one - rect.pivot);
        }

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
