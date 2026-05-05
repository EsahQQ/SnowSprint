using System;
using System.Security.Cryptography;
using System.Text;

namespace _Project.Scripts.Features.Utils
{
    public static class StringExtensions
    {
        public static Guid ToGuid(this string id)
        {
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(Encoding.Default.GetBytes(id));
                return new Guid(hash);
            }
        }
    }
}