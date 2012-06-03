using System;
using System.Text;
#if NETFX_CORE
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Security.Cryptography.DataProtection;
#else
using System.Security.Cryptography;
#endif

namespace RestSharp.Authenticators.OAuth.Extensions
{
    internal static class OAuthExtensions
    {
        public static string ToRequestValue(this OAuthSignatureMethod signatureMethod)
        {
            var value = signatureMethod.ToString().ToUpper();
            var shaIndex = value.IndexOf("SHA1");
            return shaIndex > -1 ? value.Insert(shaIndex, "-") : value;
        }

        public static OAuthSignatureMethod FromRequestValue(this string signatureMethod)
        {
            switch (signatureMethod)
            {
                case "HMAC-SHA1":
                    return OAuthSignatureMethod.HmacSha1;
                case "RSA-SHA1":
                    return OAuthSignatureMethod.RsaSha1;
                default:
                    return OAuthSignatureMethod.PlainText;
            }
        }

#if NETFX_CORE
        public static string HashWith(this string input, HashAlgorithmProvider algorithm)
        {
            throw new NotImplementedException("got to figure out how to do this");

 //           var data = Encoding.UTF8.GetBytes(input);
 //           var hash = algorithm.HashData(CryptographicBuffer.ConvertStringToBinary(data));
 //           //var hash = algorithm.CreateHash()(data);
 //           return Convert.ToBase64String(CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, hash));
        }
#else
        public static string HashWith(this string input, HashAlgorithm algorithm)
        {
            var data = Encoding.UTF8.GetBytes(input);
            var hash = algorithm.ComputeHash(data);
            return Convert.ToBase64String(hash);
        }
#endif
    }
}