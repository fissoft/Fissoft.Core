using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Fissoft
{
    public static class DESEntension
    {
        /// <summary>
        /// 用于补位的密钥
        /// </summary>
        private const string Replacecryptorkey = "EA16704A";
        /// <summary>
        /// 使用DES加密 chsword 2005-2-12
        /// </summary>
        /// <param name="originalValue">待加密的字符串</param>
        /// <param name="key">密钥(最大长度8)</param>
        /// <param name="iv">初始化向量(最大长度8)</param>
        /// <returns>加密后的字符串</returns>
        public static string DESEncrypt(this string originalValue, string key, string iv)
        {
            //将key和IV处理成8个字符
            var key1 =Encoding.UTF8.GetBytes( (key + Replacecryptorkey).Substring(0, 8));
            var iv1 = Encoding.UTF8.GetBytes((iv + Replacecryptorkey).Substring(0, 8));
            
            using (SymmetricAlgorithm sa
                = Aes.Create())
            {
                sa.IV = iv1;
                sa.Key = key1;
                using (ICryptoTransform ct = sa.CreateEncryptor())
                {
                    byte[] byt = originalValue.ToUtf8Bytes();
                    using (var ms = new MemoryStream())
                    {
                        using (var cs = new CryptoStream(ms, ct,
                                                         CryptoStreamMode.Write))
                        {
                            cs.Write(byt, 0, byt.Length);
                            cs.FlushFinalBlock();
                        }
                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
        }

        /// <summary>
        /// 使用DES加密（Added by niehl 2005-4-6）
        /// </summary>
        /// <param name="originalValue">待加密的字符串</param>
        /// <param name="key">密钥(最大长度8)</param>
        /// <returns>加密后的字符串</returns>
        public static string DESEncrypt(this string originalValue, string key) {
            return originalValue.DESEncrypt( key, key);
        }

        /// <summary>
        /// 使用DES解密（Added by chsword 2005-2-12）
        /// </summary>
        /// <param name="encryptedValue">待解密的字符串</param>
        /// <param name="key">密钥(最大长度8)</param>
        /// <param name="iv">m初始化向量(最大长度8)</param>
        /// <returns>解密后的字符串</returns>
        public static string DESDecrypt(this string encryptedValue, string key, string iv)
        {

            //将key和IV处理成8个字符
            var key1 = Encoding.UTF8.GetBytes((key + Replacecryptorkey).Substring(0, 8));
            var iv1 = Encoding.UTF8.GetBytes((iv + Replacecryptorkey).Substring(0, 8));
        
            using (SymmetricAlgorithm sa =
                 Aes.Create())
            {
                sa.IV = iv1;
                sa.Key = key1;
                using (ICryptoTransform ct = sa.CreateDecryptor())
                {

                    byte[] byt = Convert.FromBase64String(encryptedValue);

                    using (var ms = new MemoryStream())
                    {
                        using (var cs = new CryptoStream(ms, ct, CryptoStreamMode.Write))
                        {
                            cs.Write(byt, 0, byt.Length);
                            cs.FlushFinalBlock();
                        }
                        return Encoding.UTF8.GetString(ms.ToArray());
                    }
                }
            }
        }

        /// <summary>
        /// 使用DES解密（Added by niehl 2005-4-6）
        /// </summary>
        /// <param name="encryptedValue">待解密的字符串</param>
        /// <param name="key">密钥(最大长度8)</param>
        /// <returns>解密后的字符串</returns>
        public static string DESDecrypt(this string encryptedValue, string key) {
            return encryptedValue.DESDecrypt(key, key);
        }
    }
}