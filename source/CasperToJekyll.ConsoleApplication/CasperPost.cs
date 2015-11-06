using System;
using System.IO;
using System.Threading.Tasks;

namespace CasperToJekyll.ConsoleApplication
{
    internal class CasperPost
    {
        private CasperPost(string slug, string title, DateTime published, string content)
        {
            Slug = slug;
            Title = title;
            Published = published;
            Content = content;
        }

        public string Content { get; }
        public string Title { get; }
        public DateTime Published { get; }
        public string Slug { get; }

        public static async Task<CasperPost> ParseAsync(string source)
        {
            using (var reader = File.OpenText(source))
            {
                // ignore leading ---
                await reader.ReadLineAsync();

                var slug = Path.GetFileNameWithoutExtension(source)?.Trim('-');
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

                return new CasperPost(slug, title, published, content);
            }
        }

        private static string GetFrontMatterValue(string line)
        {
            return line.Substring(line.IndexOf(":", StringComparison.Ordinal) + 1).Trim();
        }
    }
}