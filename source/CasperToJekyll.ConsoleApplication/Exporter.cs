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

        public Exporter(string source, string destination)
        {
            ValidateDirectoryExists(source, nameof(source));
            ValidateDirectoryExists(destination, nameof(destination));

            _source = source;
            _destination = destination;
        }

        public Task ExportAsync()
        {
            Console.WriteLine($"Exporting blog posts from {_source}...");

            var directories = Directory.EnumerateDirectories(_source);
            var tasks = directories.Select(ExportDirectoryAsync).ToArray();
            var task = Task.WhenAll(tasks);

            return task;
        }

        private Task ExportDirectoryAsync(string directory)
        {
            Console.WriteLine($"Exporting blog posts from {directory}...");

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
            var tasks = files.Select(ExportFileAsync);
            var task = Task.WhenAll(tasks);

            return task;
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
                    return Task.Run(() => ExportMarkdownFile(source));

                default:
                    return Task.Run(() => ExportFile(source));
            }
        }

        private void ExportMarkdownFile(string source)
        {
            // todo Thread.Sleep(TimeSpan.FromSeconds(5));
        }

        private void ExportFile(string source)
        {
            var fileName = Path.GetFileName(source) ?? "";
            var destination = Path.Combine(_destination, fileName);

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