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
    /// <summary>
    /// Implements DSharpPlus command handlers for all uwu* commands
    /// </summary>
    public class UwuCommandModule : BaseCommandModule
    {
        private readonly ILogger<UwuCommandModule> _logger;
        private readonly ITextUwuifier _textUwuifier;
        
        public UwuCommandModule(ILogger<UwuCommandModule> logger, ITextUwuifier textUwuifier)
        {
            _logger = logger;
            _textUwuifier = textUwuifier;
        }
        
        [Command("this")]
        [Description("Uwuifies text")]
        public async Task ThisCommand(CommandContext ctx, [Description("Text to uwuify")][RemainingText] string text)
        {
            // Setup logging context
            using (_logger.BeginScope($"ThisCommand@{ctx.Message.Id.ToString()}"))
            {
                try
                {
                    _logger.LogDebug("Invoked by [{user}]", ctx.User);
                    
                    // Skip if no text provided
                    if (string.IsNullOrWhiteSpace(text))
                    {
                        _logger.LogDebug("Skipping empty input");
                        return;
                    }
                    
                    // Uwuify it
                    var uwuText = _textUwuifier.UwuifyText(text);

                    // Send reply
                    await ctx.RespondAsync(uwuText);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Uncaught exception");
                }
            }
        }

        [Command("that")]
        [Description("Uwuifies a message in chat")]
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
                        _logger.LogDebug("Skipping empty input");
                        _logger.LogDebug("Skipping message loop");
                        return;
                    }
                    
                    // Get original message from reference
                    var text = ctx.Message.ReferencedMessage.Content;
                    
                    // Skip if no text provided
                    if (string.IsNullOrWhiteSpace(text))
                    {
                        return;
                    }
                    
                    // Uwuify it
                    var uwuText = _textUwuifier.UwuifyText(text);

                    // Send reply
                    await ctx.RespondAsync(uwuText);
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