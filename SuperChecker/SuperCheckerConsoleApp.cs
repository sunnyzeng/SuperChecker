using Microsoft.Extensions.DependencyInjection;
using SuperChecker.Data;
using SuperChecker.Service;
using System;
using System.IO;
using System.Reflection;

namespace SuperChecker
{
    class SuperCheckerConsoleApp
    {
        private static ServiceProvider _serviceProvider;

        static void Main(string[] args)
        {
            SetupDIServiceProvider();

            var outPutDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Substring(0, AppContext.BaseDirectory.IndexOf("bin"));
            var filePath = Path.Combine(outPutDirectory, "Data\\Sample Super Data.xlsx");

            if (args.Length > 0 && File.Exists(args[0]))
                filePath = args[0];

            var request = _serviceProvider.GetService<ISuperDataReader>().GetSuperCheckerRequest(filePath);
            var service = _serviceProvider.GetService<ISuperCheckerService>();
            service.SetSuperCheckerRequest(request);

            var results = service.GetCheckResults();

            Console.WriteLine($"input file path:{filePath}");
            foreach (var result in results)
            {
                Console.WriteLine(result.ToString());
            }

            Console.ReadLine();
        }

        private static void SetupDIServiceProvider()
        {
            _serviceProvider = new ServiceCollection()
                .AddSingleton<ISuperCheckerService, SuperCheckerService>()
                .AddSingleton<ISuperDataReader, SuperDataReader>()
                .BuildServiceProvider(); ;
        }
    }
}
