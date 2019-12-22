using Fibo.Service.Components.Definition;
using Fibo.Service.Components.Implementation;
using Fibo.Service.Repositories.Definition;
using Fibo.Service.Repositories.Implementation;
using FiboForm.Common;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Threading;

namespace Fibo.Service
{
    static class Program
    {
        const int Port = 50051;
        public static IConfiguration Configuration { get; set; }
        static void Main(string[] args)
        {
            SetConfiguration();

            var serviceProvider = new ServiceCollection()
            .AddLogging(configure =>
            {
                configure.SetMinimumLevel(LogLevel.Warning);
            })
            .AddSingleton<IConfiguration>(Configuration)
            .AddSingleton<IFiboComponent, FiboComponent>()
            .AddSingleton<IFiboRepository, FiboRepository>();

            SetupRedis(serviceProvider);

            var provider = serviceProvider.BuildServiceProvider();

            Server server = new Server
            {
                Services = {
                    Fibo.Definition.Fibo.BindService(new Fibo.Service.Implementation.FiboServiceImplementation(provider))
                },
                Ports = { new ServerPort("0.0.0.0", Port, ServerCredentials.Insecure) }
            };

            Log.Information("Before Server Start");
            server.Start();

            Console.WriteLine("Fibo Service server listening on port " + Port);
            Thread.Sleep(Timeout.Infinite);

            server.ShutdownAsync().Wait();

        }

        private static void SetConfiguration()
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var builder = new ConfigurationBuilder()
               .AddJsonFile($"appsettings.json", optional: false, reloadOnChange: true)
               .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
               .AddJsonFile($"sharedsettings.json", optional: true)
               .AddJsonFile($"/app/settings/sharedsettings.json", optional: true)
               .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        private static void SetupRedis(IServiceCollection services)
        {
            var redisAddress = EnvironmentSettings.GetEnvironmentVariable("REDIS_ADDRESS", "192.168.99.100:6379");
            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = redisAddress;
                options.InstanceName = "master";
            });
        }


    }
}
