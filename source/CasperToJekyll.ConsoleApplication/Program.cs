using System;
using System.Threading.Tasks;

namespace CasperToJekyll.ConsoleApplication
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                ExecuteAsync(args).Wait();
            }
            catch (Exception exception)
            {
                HandleException(exception);
            }
        }

        private static async Task ExecuteAsync(string[] args)
        {
            var arguments = Arguments.Parse(args);
            var exporter = new Exporter(arguments.Source, arguments.Destination);

            await exporter.ExportAsync();
        }

        private static void HandleException(Exception exception)
        {
            var aggregateException = exception as AggregateException;

            if (aggregateException !=null && aggregateException.InnerExceptions.Count == 1)
            {
                exception = aggregateException.InnerException;
            }

            var foregroundColor = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(exception.Message);
            Console.WriteLine();
            Console.WriteLine(exception.StackTrace);
            Console.WriteLine();
            Console.ForegroundColor = foregroundColor;
        }
    }
}