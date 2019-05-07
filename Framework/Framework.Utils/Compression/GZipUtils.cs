namespace Framework.Utils.Compression
{
    using System;
    using System.IO;
    using System.IO.Compression;

    public class GZipUtils
    {
        /// <summary>
        ///     This method will compress a byte array.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] Compress(byte[] data)
        {
            var buffer = data;
            byte[] compressed;

            using (var memoryStream = new MemoryStream())
            {
                using (var zipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
                {
                    zipStream.Write(buffer, 0, buffer.Length);
                }

                memoryStream.Position = 0;
                compressed = new byte[memoryStream.Length];
                memoryStream.Read(compressed, 0, compressed.Length);
            }

            return compressed;
        }

        public static byte[] Decompress(byte[] bytes)
        {
            const int BufferSize = 4096;
            var buffer = new byte[BufferSize];
            byte[] bufferOut;
            using (var streamOut = new MemoryStream())
            {
                using (var streamIn = new MemoryStream(bytes))
                {
                    streamIn.Seek(0, SeekOrigin.Begin);

                    using (var zipStream = new GZipStream(streamIn, CompressionMode.Decompress))
                    {
                        while (true)
                        {
                            var bytesRead = zipStream.Read(buffer, 0, BufferSize);
                            if (bytesRead != 0)
                            {
                                streamOut.Write(buffer, 0, bytesRead);
                            }

                            if (bytesRead != BufferSize)
                            {
                                break;
                            }
                        }
                    }
                }

                bufferOut = new byte[streamOut.Length];
                streamOut.Position = 0;
                streamOut.Read(bufferOut, 0, bufferOut.Length);
            }

            return bufferOut;
        }

        public static string GetBase64Document(string path)
        {
            return !string.IsNullOrEmpty(path)
                ? Convert.ToBase64String(GZipUtils.Compress(File.ReadAllBytes(path)))
                : string.Empty;
        }

        public static byte[] GetCompressedDocumentContentBytes(string path)
        {
            return !string.IsNullOrEmpty(path) ? GZipUtils.Compress(File.ReadAllBytes(path)) : null;
        }
    }
}