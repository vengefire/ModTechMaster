namespace Framework.Utils.Extensions.File
{
    using System.IO;

    public static class FileExtensions
    {
        public static bool IsFileLocked(this FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                // the file is unavailable because it is:
                // still being written to
                // or being processed by another thread
                // or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }

            // file is not locked
            return false;
        }

        public static string StripExtensionFromFileName(this FileInfo file)
        {
            return file.Name.Substring(0, file.Name.Length - file.Extension.Length);
        }
    }
}