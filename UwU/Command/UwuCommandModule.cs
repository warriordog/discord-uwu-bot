using System;
using System.Threading.Tasks;
using DiscordUwuBot.UwU.util;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Microsoft.Extensions.Logging;

namespace DiscordUwuBot.UwU.command
{
    public class UwuCommandModule : BaseCommandModule
    {
        private readonly ILogger<UwuCommandModule> _logger;

        public UwuCommandModule(ILogger<UwuCommandModule> logger)
        {
            _logger = logger;
        }

        [Command("this")]
        [Description("UwU-ifies a message")]
        [RequireReply]
        public async Task ThisCommand(CommandContext ctx)
        {
            // Setup logging context
            using (_logger.BeginScope($"UwuCommandModule.ThisCommand@{ctx.Message.Id.ToString()}"))
            {
                try
                {
                    _logger.LogDebug("Invoked by [{user}]", ctx.User);

                    var message = ctx.Message.ReferencedMessage.Content;

                    await ctx.RespondAsync(message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Uncaught exception");
                }
            }
        }
    }
}