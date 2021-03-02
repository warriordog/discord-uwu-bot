using System;
using System.Threading.Tasks;
using DiscordUwuBot.Bot.Util;
using DiscordUwuBot.UwU;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;

namespace DiscordUwuBot.Bot.Command
{
    public class UwuCommandModule : BaseCommandModule
    {
        private readonly ILogger<UwuCommandModule> _logger;
        private readonly ITextUwuifier _textUwuifier;
        
        public UwuCommandModule(ILogger<UwuCommandModule> logger, ITextUwuifier textUwuifier)
        {
            _logger = logger;
            _textUwuifier = textUwuifier;
        }

        [Command("that")]
        [Description("UwU-ifies a message")]
        [RequireReply]
        public async Task ThatCommand(CommandContext ctx)
        {
            // Setup logging context
            using (_logger.BeginScope($"ThatCommand@{ctx.Message.Id.ToString()}"))
            {
                try
                {
                    _logger.LogDebug("Invoked by [{user}]", ctx.User);

                    // Prevent infinite loops
                    if (IsMessageLoop(ctx))
                    {
                        _logger.LogDebug("Skipping message loop");
                        return;
                    }
                    
                    // Get original message from reference
                    var originalMessage = ctx.Message.ReferencedMessage.Content;
                    
                    // Uwu-ify it
                    var uwuMessage = _textUwuifier.UwuifyText(originalMessage);

                    // Send reply
                    await ctx.RespondAsync(uwuMessage);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Uncaught exception");
                }
            }
        }

        private static bool IsMessageLoop(CommandContext ctx)
        {
            var currentUser = ctx.Client.CurrentUser;
            
            bool IsMessageLoopAt(DiscordMessage currentMessage)
            {
                // If we sent the message, then this is a loop
                if (currentUser.Equals(currentMessage.Author))
                {
                    return true;
                }
            
                // If we didn't send the message, and it is the last one, then this is not a loop
                var currentMessageRef = currentMessage.Reference?.Message;
                if (currentMessageRef == null || currentMessageRef.Equals(currentMessage))
                {
                    return false;
                }
            
                // If we didn't send the message, but its not the last one, then we need to recurse
                return IsMessageLoopAt(currentMessageRef);
            }
            
            return IsMessageLoopAt(ctx.Message);
        }
    }
}