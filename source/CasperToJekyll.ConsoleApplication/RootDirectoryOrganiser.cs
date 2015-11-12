using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CasperToJekyll.ConsoleApplication
{
    /// <summary>
    ///     Move files (pdf, jpg, etc) from website's root directory to /files
    /// </summary>
    internal class RootDirectoryOrganiser
    {
        private readonly string[] _extensionsToMove;
        private readonly string _filesDirectory;
        private readonly string _rootDirectory;
        private IEnumerable<string> _documents;

        public RootDirectoryOrganiser(string rootDirectory)
        {
            _rootDirectory = rootDirectory;
            _filesDirectory = Path.Combine(_rootDirectory, "files");
            _extensionsToMove = new[] {".pdf", ".jpg", ".png", ".gif"};
            new DirectoryInfo(_filesDirectory).CreateDirectoryStructure();
        }

        public async Task OrganiseAsync()
        {
            Console.WriteLine("Get documents that might need updating...");
            _documents = GetDocuments(_rootDirectory);

            Console.WriteLine("Getting files that will need moving...");
            var files = Directory.EnumerateFiles(_rootDirectory).Where(IsFileToMove);

            foreach (var file in files)
            {
                await OrganiseFileAsync(file);
            }
        }

        private IEnumerable<string> GetDocuments(string directory)
        {
            return Directory.EnumerateFiles(directory, "*.md", SearchOption.AllDirectories);
        }

        private async Task OrganiseFileAsync(string fileToMove)
        {
            Console.WriteLine($"Moving {fileToMove}...");

            var destination = fileToMove.Replace(_rootDirectory, _filesDirectory);

            // Destination file might exist because this program will be run several times
            if (File.Exists(destination))
            {
                File.Delete(destination);
            }

            File.Move(fileToMove, destination);

            await UpdateDocumentsAsync(Path.GetFileName(fileToMove));
        }

        private Task UpdateDocumentsAsync(string movedFileName)
        {
            var tasks = _documents.Select(document => UpdateDocumentAsyc(document, movedFileName));

            return Task.WhenAll(tasks);
        }

        private static async Task UpdateDocumentAsyc(string document, string movedFileName)
        {
            var movedToFileName = $"/files/{movedFileName}";
            var contents = await ReadContentsAsync(document);
            var newContents = contents.Replace(movedFileName, movedToFileName).Replace("//files", "/files");

            // Following replace is needed because this program might be run multiple times
            newContents = newContents.Replace("/files/files/", "/files/");

            if (contents != newContents)
            {
                await WriteContentsAsync(document, newContents);
            }
        }

        private static async Task WriteContentsAsync(string document, string newContents)
        {
            Console.WriteLine($"Updating {document}...");

            using (var stream = File.CreateText(document))
            {
                await stream.WriteAsync(newContents);
            }
        }

        private static async Task<string> ReadContentsAsync(string document)
        {
            // Console.WriteLine($"Reading {document}...");
            using (var stream = File.OpenText(document))
            {
                return await stream.ReadToEndAsync();
            }
        }

        private bool IsFileToMove(string path)
        {
            var extension = Path.GetExtension(path) ?? "";
            var moveFile = _extensionsToMove.Any(e => e.Equals(extension, StringComparison.OrdinalIgnoreCase));

            return moveFile;
        }
    }
}