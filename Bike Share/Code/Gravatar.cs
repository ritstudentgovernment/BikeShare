using System.Security.Cryptography;
using System.Text;

namespace BikeShare.Views
{
    /// <summary>
    /// Provides tools to enable gravatar use.
    /// </summary>
    public static class Gravatar
    {
        /// <summary>
        /// Returns a hash of an email to support gravatar.
        /// </summary>
        /// <param name="email">Email to hash.</param>
        /// <returns>String of the hashed email.</returns>
        private static string HashEmail(string email)
        {
            MD5 md5Hasher = MD5.Create();

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(email));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();  // Return the hexadecimal string.
        }

        /// <summary>
        /// Generates a complete gravatar url for an email.
        /// </summary>
        /// <param name="email">Email to retrieve gravatar for.</param>
        /// <returns>String url link to gravatar image.</returns>
        public static string url(string email)
        {
            return string.Format("http://www.gravatar.com/avatar/{0}", HashEmail(email));
        }
    }
}