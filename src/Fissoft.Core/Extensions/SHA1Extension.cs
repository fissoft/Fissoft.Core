using System.Security.Cryptography;
using System.Text;
using Fissoft.Framework.Systems;
using Newtonsoft.Json;

namespace Fissoft.Framework.Extensions
{
    public static class SHA1Extension
    {
        public static string GenerateSHA1<T>(T obj)
        {
            var sha1Str = JsonConvert.SerializeObject(obj).ToSHA1();
            return sha1Str;
        }

        public static string ToSHA1(this string dataStr)
        {
            if (dataStr == null)
            {
                return null;
            }

            using (var sha1 = SHA1.Create())
            {
                var dataBytes = Encoding.UTF8.GetBytes(dataStr);
                var hashCode = sha1.ComputeHash(dataBytes).ToHexStr();
                return hashCode;
            }
        }

    }
}