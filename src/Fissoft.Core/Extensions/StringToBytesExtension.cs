using System.Text;

namespace Fissoft.Framework.Systems
{
    public static class StringToBytesExtension
    {
        internal static byte[] ToAsciiBytes(this string strKey) {
            return Encoding.ASCII.GetBytes(strKey);
        }
        internal static byte[] ToUtf8Bytes(this string strKey) {
            return Encoding.UTF8 .GetBytes(strKey);
        }
    }
}