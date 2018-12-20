using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace GlamlyBackweb.Library
{
    /// <summary>
    /// Hashing class is help to implement the MD5.
    /// </summary>
    public class Hashing
    {
        /// <summary>
        /// MD5 Hashing with salt
        /// </summary>
        /// <param name="dataToHash"></param>
        /// <param name="saltKey"></param>
        /// <returns></returns>
        public static string MD5Hash(string dataToHash, string saltKey)
        {
            HMACMD5 objHMACMD5 = null;
            Byte[] byteSalt = System.Text.Encoding.UTF8.GetBytes(saltKey);
            Byte[] bytePassword = System.Text.Encoding.UTF8.GetBytes(dataToHash);

            if (byteSalt.Length > 0)
                objHMACMD5 = new HMACMD5(byteSalt);
            else
                objHMACMD5 = new HMACMD5();

            if (bytePassword.Length > 0)
                objHMACMD5.ComputeHash(bytePassword);

            return Convert.ToBase64String(objHMACMD5.Hash);
        }

        public static byte[] GetMD5Hash(string Data)
        {
            HashAlgorithm algorithm = MD5.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(Data));
        }

        public static string GetMD5HashString(string Data)
        {
            return GetMD5HashString(Data, "x2");
        }

        public static string GetMD5HashString(string Data, string Format)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetMD5Hash(Data))
                sb.Append(b.ToString(Format));
            return sb.ToString();
        }
    }
}