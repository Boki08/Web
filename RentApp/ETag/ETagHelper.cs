using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace RentApp.ETag
{
    public class ETagHelper
    {
        public const string ETAG_HEADER = "ETag";
        public const string MATCH_HEADER = "If-Match";
        public static string GetETag(byte[] contentBytes)
        {
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(contentBytes);
                string hex = BitConverter.ToString(hash);
                return hex.Replace("-", "");
            }
        }
    }
}