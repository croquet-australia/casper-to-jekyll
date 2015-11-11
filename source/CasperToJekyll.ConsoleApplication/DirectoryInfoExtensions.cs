using System.IO;

namespace CasperToJekyll.ConsoleApplication
{
    internal static class DirectoryInfoExtensions
    {
        internal static void CreateDirectoryStructure(this DirectoryInfo directory)
        {
            if (directory.Exists)
            {
                return;
            }

            CreateDirectoryStructure(directory.Parent);
            directory.Create();
        }
    }
}