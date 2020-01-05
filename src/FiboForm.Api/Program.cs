using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace FiboForm.Api
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel();
                    webBuilder.UseUrls("http://0.0.0.0:81");
                    webBuilder.UseStartup<Startup>();
                });
    }
}
