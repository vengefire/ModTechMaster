using System;
using System.Linq;
using System.Security.Cryptography;

namespace Framework.Utils.Compression
{
    public static class CheckSumUtils
    {
        /// <summary>
        ///     this Method will generate a MD5 checksum key, that will validate
        ///     the file that was received vs the file we no decompressed and generated
        ///     as pdf.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>MD5 checksum.</returns>
        /// <exception cref="System.Exception">Failed to compute MD5 checksum:  + ex.Message</exception>
        public static string CalcMd5Checksum(byte[] data)
        {
            var results = string.Empty;
            try
            {
                var objMd5 = MD5.Create("md5");
                var buffer = objMd5.ComputeHash(data);

                results = buffer.Aggregate(results, (current, t) => current + t.ToString("x2"));
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to compute MD5 checksum: " + ex.Message, ex);
            }

            return results.ToUpper();
        }
    }
}