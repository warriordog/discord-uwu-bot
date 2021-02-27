using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DiscordBotTemplate
{
    public class BotMain
    {
        private readonly DiscordClient        _discord;
        private readonly ILogger<BotMain>     _logger;

        public BotMain(ILoggerFactory loggerFactory, IOptions<BotOptions> botOptions, ILogger<BotMain> logger)
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

            // Register event handler
            _discord.MessageCreated += HandleMessage;
        }

        private async Task HandleMessage(DiscordClient sender, MessageCreateEventArgs e)
        {
            try
            {
                // Don't reply to ourself
                if (e.Author.Equals(sender.CurrentUser))
                {
                    return;
                }
                
                // Check permissions
                if (!e.Channel.IsPrivate)
                {
                    // Get current member (user from current channel)
                    var currentMember = e.Channel.Users.FirstOrDefault(mbr => mbr.Equals(sender.CurrentUser));
                    
                    // Check for chat permissions
                    if (currentMember == null || !e.Channel.PermissionsFor(currentMember).HasPermission(Permissions.SendMessages))
                    {
                        return;
                    }
                }
                
                // Check for trigger message
                if (e.Message.Content.ToLower().Contains("bot!"))
                {
                    // Reply
                    await e.Message.RespondAsync("Hello, world!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception in message handler");
            }
        }
        
        public async Task StartAsync()
        {
            _logger.LogInformation($"Discord bot {GetType().Assembly.GetName().Version} starting");
            await _discord.ConnectAsync();
        }

        public async Task StopAsync()
        {
            _logger.LogInformation("Discord bot stopping");
            await _discord.DisconnectAsync();
        }
    }
}