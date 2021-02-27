using System;
using System.Reflection;
using System.Threading.Tasks;
using DiscordUwuBot.UwU.command;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DiscordUwuBot.UwU
{
    public class BotMain
    {
        private readonly DiscordClient        _discord;
        private readonly ILogger<BotMain>     _logger;

        public BotMain(IServiceProvider serviceProvider, ILoggerFactory loggerFactory, IOptions<BotOptions> botOptions, ILogger<BotMain> logger)
        {
            _logger = logger;
            var options = botOptions.Value;

            // Create discord client
            _discord = new DiscordClient(
                new DiscordConfiguration
                {
                    Token = options.DiscordToken,
                    TokenType = TokenType.Bot,
                    LoggerFactory = loggerFactory
                }
            );

            // Register commands
            _discord.UseCommandsNext(
                    new CommandsNextConfiguration
                    {
                        StringPrefixes = new[] {"uwu*"},
                        Services = serviceProvider
                    }
                )
                .RegisterCommands<UwuCommandModule>();
        }

        public async Task StartAsync()
        {
            _logger.LogInformation($"UwU Bot {GetType().Assembly.GetName().Version} starting");
            await _discord.ConnectAsync();
        }

        public async Task StopAsync()
        {
            _logger.LogInformation("UwU Bot stopping");
            await _discord.DisconnectAsync();
        }
    }
}