using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Anymate.Dotnet.Template.Worker.Configuration;
using System.IO;
using Anymate.Dotnet.Template.Worker.Handlers;

namespace Anymate.Dotnet.Template.Worker
{
    public class Program
    {
        /*
         * Getting started
         * In order to use this template, modify the AnymateTask.cs and AnymateRules.cs to fit the configuration of the Process in anymate.app.
         * Modify the needed business logic in worker.cs
         */ 

        public static async Task Main(string[] args)
        {
            using IHost host = CreateHostBuilder(args).Build();
            using IServiceScope serviceScope = host.Services.CreateScope();
            IServiceProvider provider = serviceScope.ServiceProvider;

            var main = provider.GetRequiredService<IFlowControl>();
            await main.Run();


        }


        static IHostBuilder CreateHostBuilder(string[] args)
        {
            // Get the Environment
            string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";


            // Load the configuration
            var configurationBuilder = new ConfigurationBuilder()
                                                .AddEnvironmentVariables()
                                                .AddCommandLine(args)
                                                .AddJsonFile("appsettings.json");

            if (environment == "Development")
            {

                configurationBuilder.AddJsonFile(Path.Combine(AppContext.BaseDirectory, string.Format("..{0}..{0}..{0}", Path.DirectorySeparatorChar), $"appsettings.{environment}.json"),
                        optional: true, reloadOnChange: true
                    );
            }
            else
            {
                configurationBuilder.AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true);
            }
            var configuration = configurationBuilder.Build();


            // Build the host
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(configBuilder =>
                {
                    configBuilder.AddConfiguration(configuration);
                })
                .ConfigureServices((builder, services) =>
                {
                    // Add Configuration objects from appsettings.json like this:
                    services.Configure<AnymateConfig>(configuration.GetSection("Anymate"));

                    // Setup dependency injection
                    services.AddScoped<IFlowControl, FlowControl>();
                    services.AddTransient<IWorker, Worker>();
                    /*
                    services.AddTransient<ITransientOperation, DefaultOperation>();
                    services.AddScoped<IScopedOperation, DefaultOperation>();
                    services.AddSingleton<ISingletonOperation, DefaultOperation>();
                    */
                });

            return host;
        }
    }
}
