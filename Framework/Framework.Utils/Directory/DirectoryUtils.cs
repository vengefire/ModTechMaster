namespace Framework.Utils.Directory
{
    using System;
    using System.IO;

    public static class DirectoryUtils
    {
        public static string ConstructDateSegmentedDirectoryPath(string baseDirectory)
        {
            var dateSegmentedRelativeDirectory = string.Format(
                                                               "{0}\\{1}\\{2}",
                                                               DateTime.Today.Year,
                                                               DateTime.Today.Month,
                                                               DateTime.Today.Day);
            var finalDirectory = Path.Combine(baseDirectory, dateSegmentedRelativeDirectory);

            return finalDirectory;
        }

        public static void EnsureExists(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        public static bool Exists(string directory)
        {
            return Directory.Exists(directory);
        }
    }
}