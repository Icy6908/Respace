using System;
using System.Security.Cryptography;
using System.Text;

namespace Respace.App_Code
{
    public static class Security
    {
        // Simple SHA256 hash (OK for school projects; in real apps use bcrypt/ASP.NET Identity)
        public static string Sha256(string input)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}
