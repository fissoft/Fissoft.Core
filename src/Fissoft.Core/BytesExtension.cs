/*
 zou jian
 */

using System;
using System.Text;

namespace Fissoft
{
    public static class BytesExtension
    {
        public static long ToLong(this byte[] x, int offset)
        {
            long result = 0;
            result |= (long) x[offset + 0] << 56;
            result |= (long) x[offset + 1] << 48;
            result |= (long) x[offset + 2] << 40;
            result |= (long) x[offset + 3] << 32;
            result |= (long) x[offset + 4] << 24;
            result |= (long) x[offset + 5] << 16;
            result |= (long) x[offset + 6] << 8;
            result |= x[offset + 7];
            return result;
        }

        public static Int64 ToLong(this byte[] x)
        {
            return x.ToLong(0);
        }

        public static Int32 ToInt(this byte[] x, int offset)
        {
            int result = 0;
            result |= x[offset + 0] << 24;
            result |= x[offset + 1] << 16;
            result |= x[offset + 2] << 8;
            result |= x[offset + 3];
            return result;
        }

        public static Int32 ToInt(this byte[] x)
        {
            return x.ToInt(0);
        }

        public static void ToBytes(this int x, byte[] resultbuf, int bufoffset)
        {
            resultbuf[bufoffset + 0] = (byte) (x >> 24);
            resultbuf[bufoffset + 1] = (byte) (x >> 16);
            resultbuf[bufoffset + 2] = (byte) (x >> 8);
            resultbuf[bufoffset + 3] = (byte) x;
        }

        public static byte[] ToBytes(this int x)
        {
            var bytes = new byte[4];
            x.ToBytes(bytes, 0);
            return bytes;
        }

        public static void ToBytes(this long x, byte[] resultbuf, int bufoffset)
        {
            resultbuf[bufoffset + 0] = (byte) (x >> 56);
            resultbuf[bufoffset + 1] = (byte) (x >> 48);
            resultbuf[bufoffset + 2] = (byte) (x >> 40);
            resultbuf[bufoffset + 3] = (byte) (x >> 32);
            resultbuf[bufoffset + 4] = (byte) (x >> 24);
            resultbuf[bufoffset + 5] = (byte) (x >> 16);
            resultbuf[bufoffset + 6] = (byte) (x >> 8);
            resultbuf[bufoffset + 7] = (byte) x;
        }

        public static byte[] ToBytes(this long x)
        {
            var bytes = new byte[8];
            x.ToBytes(bytes, 0);
            return bytes;
        }

        public static string ToHexStr(this byte[] data, bool isUpperCase = false)
        {
            if (data == null)
            {
                return null;
            }

            var format = isUpperCase ? "{0:X2}" : "{0:x2}";
            var stringBuilder = new StringBuilder();
            foreach (var perByte in data)
            {
                stringBuilder.AppendFormat(format, perByte);
            }

            return stringBuilder.ToString();
        }
    }
}