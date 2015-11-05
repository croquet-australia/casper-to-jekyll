using System;

namespace CasperToJekyll.ConsoleApplication
{
    internal class Arguments
    {
        public Arguments(string source, string destination)
        {
            Source = source;
            Destination = destination;
        }

        public string Source { get; }
        public string Destination { get; }

        public static Arguments Parse(string[] args)
        {
            switch (args.Length)
            {
#if DEBUG
                case 0:
                    return new Arguments(
                        @"C:\Users\Tim\Code\croquet-australia\developer-content\source\news",
                        @"C:\Users\Tim\Code\croquet-australia\developer-content\jekyll-template\_posts");
#endif

                case 2:
                    return new Arguments(args[0], args[1]);

                default:
                    throw new ArgumentException("Usage: CasperToJekyll <sourceDirectory> <outputDirectory>");
            }
        }
    }
}