using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WIRS.Shared.Extensions
{
    public static class SSOExtensions
    {
        public static bool sso_sunburstconnect(string value, string key, string digest_value)
        {
            bool isvalid = false;
            try
            {
                // Convert the input string to a byte array and compute the hash.
                byte[] data = Encoding.UTF8.GetBytes(value);

                byte[] keyByte = Encoding.UTF8.GetBytes(key);

                // Initialize a SHA512 hash object. with the key
                HMACSHA512 hmac = new HMACSHA512(keyByte);
                byte[] computedHash = hmac.ComputeHash(data);
                string returnValue = Convert.ToBase64String(computedHash);

                if (returnValue.Equals(digest_value))
                {
                    isvalid = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return isvalid;
        }
    }
}