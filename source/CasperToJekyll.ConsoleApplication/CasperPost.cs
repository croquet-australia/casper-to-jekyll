using System;
using System.IO;
using System.Threading.Tasks;
using static System.IO.Path;

namespace CasperToJekyll.ConsoleApplication
{
    internal class CasperPost
    {
        private CasperPost(string slug, string title, DateTime published, string content, string relativePath)
        {
            Slug = slug;
            Title = title;
            Published = published;
            Content = content;
            RelativePath = relativePath;
        }

        public string Content { get; }
        public string RelativePath { get; }
        public string Title { get; }
        public DateTime Published { get; }
        public string Slug { get; }

        public static async Task<CasperPost> ParseAsync(string sourceDirectory, string source)
        {
            try
            {
                using (var reader = File.OpenText(source))
                {
                    // ignore leading ---
                    await reader.ReadLineAsync();

                    var slug = GetFileNameWithoutExtension(source)?.Trim('-');
                    var title = GetFrontMatterValue(await reader.ReadLineAsync());
                    var published = DateTime.Parse(GetFrontMatterValue(await reader.ReadLineAsync()));

                    // ignore author lines
                    await reader.ReadLineAsync();
                    await reader.ReadLineAsync();
                    await reader.ReadLineAsync();
                    await reader.ReadLineAsync();

                    // ignore blank line ---
                    await reader.ReadLineAsync();

                    // ignore final ---
                    await reader.ReadLineAsync();

                    var content = await reader.ReadToEndAsync();
                    var relativePath = GetRelativePath(sourceDirectory, source);

                    return new CasperPost(slug, title, published, content, relativePath);
                }
            }
            catch (Exception exception)
            {
                throw new Exception($"Parsing {source} failed.", exception);    
            }
        }

        private static string GetRelativePath(string sourceDirectory, string source)
        {
            var relativePath = source.Substring(sourceDirectory.Length).Trim('\\');

            return relativePath;
        }

        private static string GetFrontMatterValue(string line)
        {
            return line.Substring(line.IndexOf(":", StringComparison.Ordinal) + 1).Trim();
        }
    }
}