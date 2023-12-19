using System.IO;
using System.Linq;

namespace CommonLibrary.Utils
{
    public class FileUtils
    {
        public static void DeleteDirectoryAndFiles(string path, bool recursive = false)
        {
            if (!Directory.Exists(path))
            {
                return;
            }

            if (recursive)
            {
                Directory.EnumerateDirectories(path).ToList().ForEach(dir => DeleteDirectoryAndFiles(dir, recursive));
            }

            Directory.EnumerateFiles(path).ToList().ForEach(file => File.Delete(file));
            Directory.Delete(path);
        }

        public static string GetTempDirectoryPath() => Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
    }
}
