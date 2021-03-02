using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace DiscordUwuBot.Bot.Util
{
    /// <summary>
    /// Restrict a command to only be invokable in replies to another message
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class RequireReplyAttribute : CheckBaseAttribute
    {
        public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
        {
            return Task.FromResult(ctx.Message.MessageType == MessageType.Reply);
        }
    }
}