using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using DiscordUwuBot.Bot.Command;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DiscordUwuBot.Bot
{
    /// <summary>
    /// Configuration options for the bot
    /// </summary>
    public class BotOptions
    {
        /// <summary>
        /// Discord API authentication token
        /// </summary>
        [Required]
        [NotNull]
        public string DiscordToken { get; init; }
    }
    
    /// <summary>
    /// Main class for the bot
    /// </summary>
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
            
            // Log when bot joins a server.
            _discord.GuildCreated += (d, e) =>
            {
                _logger.LogInformation($"Joined server {e.Guild.Id} ({e.Guild.Name})");
                return Task.CompletedTask;
            };
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