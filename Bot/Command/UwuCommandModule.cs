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
                        await new DiscordMessageBuilder()
                            .WithContent($"I'm already following you in this channel. Use {Formatter.InlineCode("uwu*stop")} to make me stop.")
                            .WithReply(ctx.Message.Id)
                            .SendAsync(ctx.Channel);
                    }

                    // Start following
                    _uwuRepeater.FollowUser(ctx.User, ctx.Channel);
                    _logger.LogDebug("Started following user.");

                    // Send response
                    await new DiscordMessageBuilder()
                        .WithContent($"I'm now following you and will translate everything you say in this channel. Use {Formatter.InlineCode("uwu*stop")} to make me stop.")
                        .WithReply(ctx.Message.Id)
                        .SendAsync(ctx.Channel);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Uncaught exception");
                }
            }
        }

        [Command("stop")]
        [Description("Stop following you")]
        public async Task StopUserCommand(CommandContext ctx, string destination = "here")
        {
            // Setup logging context
            using (_logger.BeginScope($"StopUserCommand@{ctx.Message.Id.ToString()}"))
            {
                try
                {
                    _logger.LogDebug("Invoked by [{user}]", ctx.User);

                    // Stop in current channel
                    switch (destination.ToLower())
                    {
                        case "":
                        case "here":
                        case "channel":
                            await StopUserChannel(ctx);
                            break;
                            
                        case "server":
                            await StopUserServer(ctx);
                            break;
                            
                        case "everywhere":
                        case "global":
                        case "discord":
                            await StopUserGlobal(ctx);
                            break;
                            
                        default:
                            await new DiscordMessageBuilder()
                                .WithContent($"Unknown parameter '{destination}' - valid values are 'here', 'server', and 'everywhere'.")
                                .WithReply(ctx.Message.Id)
                                .SendAsync(ctx.Channel);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Uncaught exception");
                }
            }
        }

        private async Task StopUserChannel(CommandContext ctx)
        {
            _logger.LogDebug("In StopUserHere");
            
            // Check if user is already being followed
            if (!_uwuRepeater.IsUserFollowed(ctx.User, ctx.Channel))
            {
                _logger.LogDebug("Skipping - user is not followed.");
                await new DiscordMessageBuilder()
                    .WithContent("I'm not following you in this channel.")
                    .WithReply(ctx.Message.Id)
                    .SendAsync(ctx.Channel);
            }
            else
            {
                // Stop following
                _uwuRepeater.UnfollowUser(ctx.User, ctx.Channel);
                _logger.LogDebug("Stopped following user.");

                // Send response
                await new DiscordMessageBuilder()
                    .WithContent("I'm no longer following you in this channel.")
                    .WithReply(ctx.Message.Id)
                    .SendAsync(ctx.Channel);
            }
        }

        private async Task StopUserServer(CommandContext ctx)
        {
            _logger.LogDebug("Stopped following in server");
            _uwuRepeater.ClearFollowsForUserGuild(ctx.User, ctx.Guild);
            await new DiscordMessageBuilder()
                .WithContent("I'm no longer following you in this server.")
                .WithReply(ctx.Message.Id)
                .SendAsync(ctx.Channel);
        }

        private async Task StopUserGlobal(CommandContext ctx)
        {
            _logger.LogDebug("Stopped following globally");
            _uwuRepeater.ClearFollowsForUser(ctx.User);
            await new DiscordMessageBuilder()
                .WithContent("I'm no longer following you anywhere on Discord.")
                .WithReply(ctx.Message.Id)
                .SendAsync(ctx.Channel);
        }

        [Command("stop_everyone")]
        [RequireUserPermissions(Permissions.ManageMessages)]
        [Description("Stop following everyone")]
        public async Task StopEveryoneCommand(CommandContext ctx, string destination = "here")
        {
            // Setup logging context
            using (_logger.BeginScope($"StopEveryoneCommand@{ctx.Message.Id.ToString()}"))
            {
                try
                {
                    _logger.LogDebug("Invoked by [{user}]", ctx.User);

                    // Stop in current channel
                    switch (destination.ToLower())
                    {
                        case "":
                        case "here":
                        case "channel":
                            await StopEveryoneChannel(ctx);
                            break;
                            
                        case "server":
                            await StopEveryoneServer(ctx);
                            break;
                            
                        default:
                            await new DiscordMessageBuilder()
                                .WithContent($"Unknown parameter '{destination}' - valid values are 'here' and 'server'.")
                                .WithReply(ctx.Message.Id)
                                .SendAsync(ctx.Channel);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Uncaught exception");
                }
            }
        }

        private async Task StopEveryoneChannel(CommandContext ctx)
        {
            _uwuRepeater.ClearFollowsForChannel(ctx.Channel);
            _logger.LogDebug("Stopped following everyone in channel.");
            await new DiscordMessageBuilder()
                .WithContent("I'm no longer following anyone in this channel.")
                .WithReply(ctx.Message.Id)
                .SendAsync(ctx.Channel);
        }

        private async Task StopEveryoneServer(CommandContext ctx)
        {
            _uwuRepeater.ClearFollowsForGuild(ctx.Guild);
            _logger.LogDebug("Stopped following everyone in server.");
            await new DiscordMessageBuilder()
                .WithContent("I'm no longer following anyone in this server.")
                .WithReply(ctx.Message.Id)
                .SendAsync(ctx.Channel);
        }

        [Command("them")]
        [RequireUserPermissions(Permissions.ManageMessages)]
        [Description("Follow someone else")]
        public async Task ThemCommand(CommandContext ctx, DiscordUser user)
        {
            // Setup logging context
            using (_logger.BeginScope($"ThemCommand@{ctx.Message.Id.ToString()}"))
            {
                try
                {
                    _logger.LogDebug("Invoked by [{user}]", ctx.User);

                    // Check if following
                    if (_uwuRepeater.IsUserFollowed(user, ctx.Channel))
                    {
                        // Stop following user
                        _logger.LogDebug("Unfollowing [{target}]", user);
                        _uwuRepeater.UnfollowUser(user, ctx.Channel);
                        await new DiscordMessageBuilder()
                            .WithContent($"I'm no longer following { Formatter.Mention(user) } in this channel.")
                            .WithReply(ctx.Message.Id)
                            .SendAsync(ctx.Channel);  
                    }
                    else
                    {
                        // Follow user
                        _logger.LogDebug("Following [{target}]", user);
                        _uwuRepeater.FollowUser(user, ctx.Channel);
                        await new DiscordMessageBuilder()
                            .WithContent($"I'm now following { Formatter.Mention(user) } in this channel. Use this command again to make me stop.")
                            .WithReply(ctx.Message.Id)
                            .SendAsync(ctx.Channel);   
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Uncaught exception");
                }
            }
        }
    }
}