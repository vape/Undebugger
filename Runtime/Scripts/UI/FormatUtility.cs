using System;
using System.Text;
using UnityEngine;

namespace Undebugger.UI
{
    internal static class FormatUtility
    {
        public static readonly char[] TempBuffer = new char[128];

        private static readonly char[] sizeFormat = new char[2] { 'N', '2' }; 
        private static readonly char[] shortMsFormat = new char[1] { '0' };
        private static readonly char[] longMsFormat = new char[3] { '0', '.', '0' };
        private static readonly string[] fileSizeUnits = { "B", "KB", "MB", "GB", "TB", "PB" };
        private static StringBuilder sharedStringBuilder = new StringBuilder(capacity: 32);

        public static void FrametimeToReadableString(float time, bool shortFormat, char[] buffer, ref int offset)
        {
            const string prefix = "(";
            const string postfix = "ms)";

            WriteInt32(Mathf.RoundToInt(1f / time), buffer, ref offset);
            Copy(prefix, buffer, ref offset);
            WriteFloat(time * 1000, shortFormat ? 0 : 1, buffer, ref offset);
            Copy(postfix, buffer, ref offset);
        }

        public static void BytesToReadableString(long value, char[] buffer, ref int offset)
        {
            if (value < 0)
            {
                buffer[offset++] = '-';
            }

            value = value == long.MinValue ? Math.Abs(value + 1) : Math.Abs(value);

            if (value < 1024)
            {
                WriteInt64(value, buffer, ref offset);
                return;
            }
            else
            {
                var len = (float)value;
                var order = 0;
                while (len >= 1024 && order < fileSizeUnits.Length - 1)
                {
                    order++;
                    len = len / 1024;
                }

                WriteFloat(len, 2, buffer, ref offset);
                Copy(GetFileSizeUnit(order), buffer, ref offset);
            }
        }

        public static string BytesToReadableString(long value)
        {
            sharedStringBuilder.Clear();
            
            if (value < 0)
            {
                sharedStringBuilder.Append('-');
            }

            value = value == long.MinValue ? Math.Abs(value + 1) : Math.Abs(value);

            if (value < 1024)
            {
                sharedStringBuilder.Append(value);
                sharedStringBuilder.Append(GetFileSizeUnit(0));

                return sharedStringBuilder.ToString();
            }

            var len = (double)value;
            var order = 0;
            while (len >= 1024 && order < fileSizeUnits.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            sharedStringBuilder.Append(len.ToString("N2"));
            sharedStringBuilder.Append(GetFileSizeUnit(order));

            return sharedStringBuilder.ToString();
        }

        private static string GetFileSizeUnit(int order)
        {
            if (order < 0 || order >= fileSizeUnits.Length)
            {
                return string.Empty;
            }

            return fileSizeUnits[order];
        }

        public static void Copy(string str, char[] buffer, ref int offset)
        {
            str.CopyTo(0, buffer, offset, str.Length);
            offset += str.Length;
        }

        #region Write Float

        public static void WriteFloat(float value, int decimalPlaces, char[] buffer, ref int offset)
        {
            const string inf = "Infinity";
            const string nan = "NaN";

            if (float.IsNaN(value))
            {
                Copy(nan, buffer, ref offset);
                return;
            }

            if (value < 0)
            {
                buffer[offset++] = '-';
                value = -value;
            }

            if (float.IsInfinity(value))
            {
                Copy(inf, buffer, ref offset);
                return;
            }

            uint integralPart = 0;
            uint decimalPart = 0;
            short exponent = 0;

            SplitFloat(value, ref integralPart, ref decimalPart, ref exponent);
            WriteUInt32(integralPart, buffer, ref offset);

            if (decimalPlaces > 0)
            {
                WriteFloatDecimal(decimalPart, buffer, ref offset, decimalPlaces);
            }

            if (exponent < 0)
            {
                buffer[offset++] = 'e';
                buffer[offset++] = '-';
                WriteInt32(-exponent, buffer, ref offset);
            }

            if (exponent > 0)
            {
                buffer[offset++] = 'e';
                WriteInt32(exponent, buffer, ref offset);
            }
        }

        private static void SplitFloat(double value, ref uint integralPart, ref uint decimalPart, ref short exponent)
        {
            exponent = NormalizeFloat(ref value);

            integralPart = (uint)value;
            double remainder = value - integralPart;

            remainder *= 1e9;
            decimalPart = (uint)remainder;
            remainder -= decimalPart;

            if (remainder >= 0.5)
            {
                decimalPart++;

                if (decimalPart >= 1000000000)
                {
                    decimalPart = 0;
                    integralPart++;

                    if (exponent != 0 && integralPart >= 10)
                    {
                        exponent++;
                        integralPart = 1;
                    }
                }
            }
        }

        private static short NormalizeFloat(ref double value)
        {
            const double positiveExpThreshold = 1e7;
            const double negativeExpThreshold = 1e-5;

            short exponent = 0;

            if (value >= positiveExpThreshold)
            {
                if (value >= 1e256)
                {
                    value /= 1e256;
                    exponent += 256;
                }
                if (value >= 1e128)
                {
                    value /= 1e128;
                    exponent += 128;
                }
                if (value >= 1e64)
                {
                    value /= 1e64;
                    exponent += 64;
                }
                if (value >= 1e32)
                {
                    value /= 1e32;
                    exponent += 32;
                }
                if (value >= 1e16)
                {
                    value /= 1e16;
                    exponent += 16;
                }
                if (value >= 1e8)
                {
                    value /= 1e8;
                    exponent += 8;
                }
                if (value >= 1e4)
                {
                    value /= 1e4;
                    exponent += 4;
                }
                if (value >= 1e2)
                {
                    value /= 1e2;
                    exponent += 2;
                }
                if (value >= 1e1)
                {
                    value /= 1e1;
                    exponent += 1;
                }
            }

            if (value > 0 && value <= negativeExpThreshold)
            {
                if (value < 1e-255)
                {
                    value *= 1e256;
                    exponent -= 256;
                }
                if (value < 1e-127)
                {
                    value *= 1e128;
                    exponent -= 128;
                }
                if (value < 1e-63)
                {
                    value *= 1e64;
                    exponent -= 64;
                }
                if (value < 1e-31)
                {
                    value *= 1e32;
                    exponent -= 32;
                }
                if (value < 1e-15)
                {
                    value *= 1e16;
                    exponent -= 16;
                }
                if (value < 1e-7)
                {
                    value *= 1e8;
                    exponent -= 8;
                }
                if (value < 1e-3)
                {
                    value *= 1e4;
                    exponent -= 4;
                }
                if (value < 1e-1)
                {
                    value *= 1e2;
                    exponent -= 2;
                }
                if (value < 1e0)
                {
                    value *= 1e1;
                    exponent -= 1;
                }
            }

            return exponent;
        }

        private static unsafe void WriteFloatDecimal(uint value, char[] buffer, ref int offset, int length)
        {
            const int exp = 9;

            int width = exp;
            var tmp = stackalloc char[exp];
            var idx = exp - 1;

            while (width-- > 0)
            {
                tmp[idx--] = (char)((value % 10) + '0');
                value /= 10;
            }

            buffer[offset++] = '.';

            while (length-- > 0)
            {
                buffer[offset++] = tmp[++idx];
            }
        }

        #endregion

        public static unsafe void WriteUInt32(uint value, char[] buffer, ref int offset)
        {
            if (value == 0)
            {
                buffer[offset++] = '0';
            }

            var tmp = stackalloc char[10];
            var idx = 0;

            while (value > 0)
            {
                tmp[idx++] = (char)((value % 10) + '0');
                value /= 10;
            }

            while (idx > 0)
            {
                buffer[offset++] = tmp[--idx];
            }
        }

        public static unsafe void WriteUInt64(ulong value, char[] buffer, ref int offset)
        {
            if (value == 0)
            {
                buffer[offset++] = '0';
            }

            var tmp = stackalloc char[20];
            var idx = 0;

            while (value > 0)
            {
                tmp[idx++] = (char)((value % 10) + '0');
                value /= 10;
            }

            while (idx > 0)
            {
                buffer[offset++] = tmp[--idx];
            }
        }

        public static void WriteInt32(int value, char[] buffer, ref int offset)
        {
            if (value == 0)
            {
                buffer[offset++] = '0';
            }

            unchecked
            {
                if (value < 0)
                {
                    buffer[offset++] = '-';
                    value = -value;
                }

                WriteUInt32((uint)value, buffer, ref offset);
            }
        }

        public static void WriteInt64(long value, char[] buffer, ref int offset)
        {
            if (value == 0)
            {
                buffer[offset++] = '0';
            }

            unchecked
            {
                if (value < 0)
                {
                    buffer[offset++] = '-';
                    value = -value;
                }

                WriteUInt64((ulong)value, buffer, ref offset);
            }
        }
    }
}
