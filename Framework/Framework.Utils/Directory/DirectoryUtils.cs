using System;
using System.Collections.Generic;
using System.IO;

namespace Framework.Utils.Directory
{
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
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }
        }

        public static bool Exists(string directory)
        {
            return System.IO.Directory.Exists(directory);
        }
    }
}