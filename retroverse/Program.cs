using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RetroVm.Server.Services;

namespace RetroVm.Server
{
    class Program
    {
        private static IServiceProvider ConfigureServices()
        {
            var serviceCollection = new ServiceCollection()
                .AddLogging(opt =>
                {
                    opt.AddConsole();
                })
                .AddTransient<RepositoryBuilder>();

            return serviceCollection.BuildServiceProvider();
        }

        static void Main(string[] args)
        {
            var sp = ConfigureServices();
            var myClass = sp.GetService<RepositoryBuilder>();

            myClass.BuildUsingAggregators("E:\\Prog\\C#\\retrovm").Wait();
        }
    }
}
