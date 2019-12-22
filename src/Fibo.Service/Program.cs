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
using Serilog.Formatting.Compact;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Fibo.Service
{
    static class Program
    {
        const int Port = 50051;
        public static IConfiguration Configuration { get; set; }
        static void Main(string[] args)
        {
            SetConfiguration();
            CreateLogger();

            var serviceProvider = new ServiceCollection()
            .AddLogging(configure =>
            {
                configure.SetMinimumLevel(LogLevel.Warning);
            })
            .AddSingleton<IConfiguration>(Configuration)
            .AddSingleton<IFiboComponent, FiboComponent>()
            .AddSingleton<IFiboRepository, FiboRepository>();

            SetupRedis(serviceProvider);

            serviceProvider.AddLogging(loggingbuilder =>
            {
                loggingbuilder.AddConfiguration(Configuration.GetSection("Logging"));
                loggingbuilder.AddDebug();
                loggingbuilder.AddSerilog(dispose: true);
            });

            var provider = serviceProvider.BuildServiceProvider();

            SetupPostgres(provider);

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

        private static void SetupPostgres(ServiceProvider provider)
        {
            var scope = provider.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IFiboRepository>();
            Task.Run(async () => { await repo.CreateVisitValuesTable(); });
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
            var redisAddress = EnvironmentSettings.GetEnvironmentVariable("REDIS_HOST", "192.168.99.100:6379");
            var instanceName = EnvironmentSettings.GetEnvironmentVariable("REDIS_INSTANCE", "master");
            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = redisAddress;
                options.InstanceName = instanceName;
            });
        }

        private static void CreateLogger()
        {
            Log.Logger = new LoggerConfiguration()
              .WriteTo.Console(new CompactJsonFormatter())
              .ReadFrom.Configuration(Configuration)
              .CreateLogger();
        }


    }
}
