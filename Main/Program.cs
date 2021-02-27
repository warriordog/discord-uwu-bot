using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DiscordBotTemplate
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            // Create environment
            using var host = CreateHost(args);

            // Run application
            await host.RunAsync();
        }

        private static IHost CreateHost(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices(
                    (ctx, services) =>
                    {
                        // Inject config
                        services.AddOptions<BotOptions>()
                            .Bind(ctx.Configuration.GetSection(nameof(BotOptions)))
                            .ValidateDataAnnotations();

                        // Inject main app logic
                        services.AddScoped<BotMain>();
                        services.AddHostedService<BotService>();
                    }
                )
                .Build();
        }
    }
}