using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using DiscordUwuBot.Bot.Command;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DiscordUwuBot.Bot;

/// <summary>
/// Discord authentication options
/// </summary>
public class DiscordAuthOptions
{

    /// <summary>
    /// Discord API authentication token
    /// </summary>
    [Required]
    public string DiscordToken { get; init; } = "";
}
    
/// <summary>
/// Configuration options for the bot
/// </summary>
public class BotOptions
{
    /// <summary>
    /// List of prefixes for discord commands
    /// </summary>
    [MinLength(1)]
    [Required]
    public IEnumerable<string> CommandPrefixes { get; init; } = new List<string>();
}
    
/// <summary>
/// Main class for the bot
/// </summary>
public class BotMain
{
    private readonly DiscordClient        _discord;
    private readonly ILogger<BotMain>     _logger;

    public BotMain(IServiceProvider serviceProvider, ILoggerFactory loggerFactory, IOptions<DiscordAuthOptions> authOptions, IOptions<BotOptions> botOptions, ILogger<BotMain> logger, IUwuRepeater uwuRepeater)
    {
        _logger = logger;

        // Create discord client
        _discord = new DiscordClient(
            new DiscordConfiguration
            {
                Token = authOptions.Value.DiscordToken,
                TokenType = TokenType.Bot,
                LoggerFactory = loggerFactory,
                Intents = DiscordIntents.DirectMessages | DiscordIntents.GuildMessages | DiscordIntents.Guilds | DiscordIntents.MessageContents
            }
        );

        // Register commands
        _discord.UseCommandsNext(
                new CommandsNextConfiguration
                {
                    StringPrefixes = botOptions.Value.CommandPrefixes,
                    Services = serviceProvider
                }
            )
            .RegisterCommands<UwuCommandModule>();
            
        // Log when bot joins a server.
        _discord.GuildCreated += (_, e) =>
        {
            _logger.LogInformation($"Joined server {e.Guild.Id} ({e.Guild.Name})");
            return Task.CompletedTask;
        };
            
        // Listen to all chat and UwU anyone who has asked to be followed
        _discord.MessageCreated += uwuRepeater.OnMessageCreated;
    }

    public async Task StartAsync()
    {
        _logger.LogInformation("UwU Bot {version} starting", GetType().Assembly.GetName().Version);
        await _discord.ConnectAsync();
    }

    public async Task StopAsync()
    {
        _logger.LogInformation("UwU Bot stopping");
        await _discord.DisconnectAsync();
    }
}