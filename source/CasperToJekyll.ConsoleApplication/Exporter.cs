using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CasperToJekyll.ConsoleApplication
{
    internal class Exporter
    {
        private readonly string _destination;
        private readonly string _source;
        private readonly string[] _excludedFiles;

        public Exporter(string source, string destination)
        {
            ValidateDirectoryExists(source, nameof(source));
            ValidateDirectoryExists(destination, nameof(destination));

            _source = source;
            _destination = destination;
            _excludedFiles = new[] {$"{source}\\README.md", $"{source}\\LICENSE.txt" };
        }

        public async Task ExportAsync()
        {
            await ExportDirectoryAsync(_source);
        }

        private Task ExportDirectoryAsync(string directory)
        {
            Console.WriteLine($"Exporting from {directory}...");

            var tasks = new[]
            {
                ExportSubDirectoriesAsync(directory),
                ExportFilesAsync(directory)
            };
            var task = Task.WhenAll(tasks);

            return task;
        }

        private Task ExportFilesAsync(string directory)
        {
            var files = Directory.EnumerateFiles(directory);
            var tasks = files.Where(IsNotExcludedFile).Select(ExportFileAsync);
            var task = Task.WhenAll(tasks);

            return task;
        }

        private bool IsNotExcludedFile(string file)
        {
            return !_excludedFiles.Contains(file, StringComparer.OrdinalIgnoreCase);
        }

        private Task ExportSubDirectoriesAsync(string directory)
        {
            var directories = Directory.EnumerateDirectories(directory);
            var tasks = directories.Select(ExportDirectoryAsync);
            var task = Task.WhenAll(tasks);

            return task;
        }

        private Task ExportFileAsync(string source)
        {
            Console.WriteLine($"Exporting {source}...");

            switch (Path.GetExtension(source)?.ToLowerInvariant())
            {
                case ".md":
                    return ExportMarkdownFileAsync(source);

                default:
                    return Task.Run(() => ExportFile(source));
            }
        }

        private async Task ExportMarkdownFileAsync(string source)
        {
            var casperPost = await CasperPost.ParseAsync(_source, source);

            await JekyllPost.WriteAsync(_destination, casperPost);
        }

        private void ExportFile(string source)
        {
            var destination = source.Replace(_source, _destination);

            new FileInfo(destination).Directory.CreateDirectoryStructure();

            File.Copy(source, destination, true);
        }

        private static void ValidateDirectoryExists(string directory, string paramName)
        {
            if (Directory.Exists(directory))
            {
                return;
            }
            throw new ArgumentException($"Cannot find {paramName} directory '{directory}'.");
        }
    }
}