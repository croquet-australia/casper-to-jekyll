using System;
using System.IO;
using System.Threading.Tasks;

namespace CasperToJekyll.ConsoleApplication
{
    internal class JekyllPost
    {
        private readonly CasperPost _casperPost;

        private JekyllPost(string destinationDirectory, CasperPost casperPost)
        {
            _casperPost = casperPost;

            Destination = GetDestinationPath(destinationDirectory);
        }

        public string Content => _casperPost.Content;

        public DateTime Date => _casperPost.Published;

        public string Title => _casperPost.Title.Trim('"');

        public string Destination { get; }

        private string GetDestinationPath(string destinationDirectory)
        {
            var filename = $"{_casperPost.Published:yyyy-MM-dd}-{_casperPost.Slug}.md";
            var path = Path.Combine(destinationDirectory, filename);

            return path;
        }

        public static async Task WriteAsync(string destinationDirectory, CasperPost casperPost)
        {
            var jekyllPost = new JekyllPost(destinationDirectory, casperPost);

            using (var destination = File.CreateText(jekyllPost.Destination))
            {
                await destination.WriteLineAsync("---");
                await destination.WriteLineAsync("layout: post");
                await destination.WriteLineAsync($"title: \"{jekyllPost.Title}\"");
                await destination.WriteLineAsync($"date: {jekyllPost.Date:yyyy-MM-dd HH:mm:ss zzz}");
                await destination.WriteLineAsync("author: Susan Linge");
                await destination.WriteLineAsync("---");
                await destination.WriteLineAsync(jekyllPost.Content);
            }
        }
    }
}