using FiboForm.Api.Components.Definition;
using FiboForm.Api.Components.Implementation;
using FiboForm.Api.Repositories.Definition;
using FiboForm.Api.Repositories.Implementation;
using FiboForm.Common;
using FiboForm.Common.Components.Definition;
using FiboForm.Common.Components.Implementation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Formatting.Compact;
using System.Text;

namespace FiboForm.Api
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();

            Log.Logger = new LoggerConfiguration()
              .WriteTo.Console(new CompactJsonFormatter())
              .ReadFrom.Configuration(Configuration)
              .CreateLogger();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", corsBuilder => corsBuilder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });

            services.AddSingleton<IEnvironmentSettings, EnvironmentSettings>();
            services.AddSingleton<IFiboComponent, FiboComponent>();
            services.AddSingleton<IFiboRepository, FiboRepository>();

            services.AddHttpClient();

            services.AddMvc().AddNewtonsoftJson();

            services.AddLogging(loggingbuilder =>
            {
                loggingbuilder.AddConfiguration(Configuration.GetSection("Logging"));
                loggingbuilder.AddDebug();
                loggingbuilder.AddSerilog(dispose: true);
            });

            SetupRedis(services);


        }


        private static void SetupRedis(IServiceCollection services)
        {
            using var scope = services.BuildServiceProvider().CreateScope();
            var _envSettings = scope.ServiceProvider.GetRequiredService<IEnvironmentSettings>();

            var redisAddress = _envSettings.GetEnvironmentVariable("REDIS_HOST");
            var redisPort = _envSettings.GetEnvironmentVariable("REDIS_PORT");
            var instanceName = _envSettings.GetEnvironmentVariable("REDIS_INSTANCE", "master");


            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = string.Format("{0}:{1}", redisAddress, redisPort);
                options.InstanceName = instanceName;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("CorsPolicy");

            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";

                    var error = context.Features.Get<IExceptionHandlerFeature>();
                    if (error != null)
                    {
                        var ex = error.Error;

                        var logger = loggerFactory.CreateLogger<Startup>();
                        logger.LogError(ex, "ERROR. An error has ocurred.");

                        await context.Response.WriteAsync(new
                        {
                            Code = 500,
                            ex.Message
                        }.ToString(), Encoding.UTF8);
                    }
                });
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
