using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;

namespace AzureDatabaseReplica
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);

            var serviceProvider = services.BuildServiceProvider();
            serviceProvider.GetService<Execute>();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(builder =>
                builder
                    .AddDebug()
                    .AddConsole()
            );

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false)
                .AddEnvironmentVariables()
                .Build();

            services.AddOptions();
            services.Configure<AppSettings>(configuration.GetSection("App"));
            services.AddScoped<Execute>();
            services.AddScoped<IDatabase, Database>();
        }
    }
}
