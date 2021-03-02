using System.Threading.Tasks;
using DiscordUwuBot.Bot;
using DiscordUwuBot.UwU;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DiscordUwuBot.Main
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

                        // Inject UwU logic
                        services.AddSingleton<ITextUwuifier, TextUwuifier>();
                        
                        // Inject discord bot logic
                        services.AddScoped<BotMain>();
                        services.AddHostedService<BotService>();
                    }
                )
                .Build();
        }
    }
}