using System.Threading.Tasks;
using DiscordUwuBot.Bot;
using DiscordUwuBot.UwU;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DiscordUwuBot.Main
{
    internal static class Program
    {
        public static async Task Main(string[] args)
        {
            // Create environment
            using var host = CreateHost(args);

            // Run application
            await host.RunAsync();
        }

        private static IHost CreateHost(string[] args) =>
            // Apply default host settings
            Host.CreateDefaultBuilder(args)
                
            // Configure DI
            .ConfigureServices(
                (ctx, services) =>
                {
                    // Inject config
                    services.AddOptions<DiscordAuthOptions>()
                        .Bind(ctx.Configuration.GetSection("DiscordAuth"))
                        .ValidateDataAnnotations();
                    services.AddOptions<BotOptions>()
                        .Bind(ctx.Configuration.GetSection("BotOptions"))
                        .ValidateDataAnnotations();
                    services.AddOptions<UwuOptions>()
                        .Bind(ctx.Configuration.GetSection("UwuOptions"))
                        .ValidateDataAnnotations();

                    // Inject UwU logic
                    services.AddScoped<ITextUwuifier, TextUwuifier>();
                        
                    // Inject discord bot logic
                    services.AddScoped<IUwuRepeater, UwuRepeater>();
                    services.AddScoped<BotMain>();
                    services.AddHostedService<BotService>();
                }
            )
            
            // Create the host
            .Build();
    }
}