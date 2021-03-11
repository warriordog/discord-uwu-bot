using System;
using System.Threading.Tasks;
using DiscordUwuBot.Bot.Util;
using DiscordUwuBot.UwU;
using DSharpPlus;
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
        private readonly IUwuRepeater _uwuRepeater;

        public UwuCommandModule(ILogger<UwuCommandModule> logger, ITextUwuifier textUwuifier, IUwuRepeater uwuRepeater)
        {
            _logger = logger;
            _textUwuifier = textUwuifier;
            _uwuRepeater = uwuRepeater;
        }

        [Command("this")]
        [Description("Uwuifies text")]
        public async Task ThisCommand(CommandContext ctx, [Description("Text to uwuify")] [RemainingText]
            string text)
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
                    await new DiscordMessageBuilder()
                        .WithContent(uwuText)
                        .WithReply(ctx.Message.Id)
                        .SendAsync(ctx.Channel);
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
                    if (MessageValidation.IsMessageLoop(ctx.Client.CurrentUser, ctx.Message))
                    {
                        _logger.LogDebug("Skipping message loop");
                        return;
                    }

                    // Get original message from reference
                    var text = ctx.Message.ReferencedMessage.Content;

                    // Skip if no text provided
                    if (string.IsNullOrWhiteSpace(text))
                    {
                        _logger.LogDebug("Skipping empty input");
                        return;
                    }

                    // Uwuify it
                    var uwuText = _textUwuifier.UwuifyText(text);

                    // Send reply
                    await new DiscordMessageBuilder()
                        .WithContent(uwuText)
                        .WithReply(ctx.Message.ReferencedMessage.Id)
                        .SendAsync(ctx.Channel);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Uncaught exception");
                }
            }
        }

        [Command("me")]
        [Description("Follow you and UwUify everything you say")]
        public async Task MeCommand(CommandContext ctx)
        {
            // Setup logging context
            using (_logger.BeginScope($"MeCommand@{ctx.Message.Id.ToString()}"))
            {
                try
                {
                    _logger.LogDebug("Invoked by [{user}]", ctx.User);

                    // Check if user is already being followed
                    if (_uwuRepeater.IsUserFollowed(ctx.User, ctx.Channel))
                    {
                        _logger.LogDebug("Skipping - user is already followed.");
                        await ctx.RespondAsync($"I'm already following you in this channel. Use {Formatter.InlineCode("uwu*stop")} to make me stop.");
                    }

                    // Start following
                    _uwuRepeater.FollowUser(ctx.User, ctx.Channel);
                    _logger.LogDebug("Started following user.");

                    // Send response
                    await ctx.RespondAsync($"I'm now following you and will translate everything you say in this channel. Use {Formatter.InlineCode("uwu*stop")} to make me stop.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Uncaught exception");
                }
            }
        }

        [Command("stop")]
        [Description("Stop following you")]
        public async Task StopCommand(CommandContext ctx)
        {
            // Setup logging context
            using (_logger.BeginScope($"StopCommand@{ctx.Message.Id.ToString()}"))
            {
                try
                {
                    _logger.LogDebug("Invoked by [{user}]", ctx.User);

                    // Check if user is already being followed
                    if (!_uwuRepeater.IsUserFollowed(ctx.User, ctx.Channel))
                    {
                        _logger.LogDebug("Skipping - user is not followed.");
                        await ctx.RespondAsync($"I'm not following you in this channel.");
                    }

                    // Start following
                    _uwuRepeater.UnfollowUser(ctx.User, ctx.Channel);
                    _logger.LogDebug("Stopped following user.");

                    // Send response
                    await ctx.RespondAsync($"I'm no longer following you in this channel.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Uncaught exception");
                }
            }
        }
    }
}