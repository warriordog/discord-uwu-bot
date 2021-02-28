using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DiscordUwuBot.UwU.util;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;

namespace DiscordUwuBot.UwU.command
{
    public class UwuCommandModule : BaseCommandModule
    {
        private const RegexOptions RegexCI = RegexOptions.Compiled | RegexOptions.IgnoreCase;
        
        private readonly ILogger<UwuCommandModule> _logger;

        private readonly (Regex, MatchEvaluator)[] _uwuReplacements = GetUwuReplacements();
        
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
            using (_logger.BeginScope($"ThisCommand@{ctx.Message.Id.ToString()}"))
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
                    var uwuMessage = UwuifyMessage(originalMessage);

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

        private string UwuifyMessage(string message)
        {
            var uwuMessage = ReplaceMany(message, _uwuReplacements);
            uwuMessage += " UwU!";
            return uwuMessage;
        }

        private static string ReplaceMany(string input, IEnumerable<(Regex, MatchEvaluator)> replacements)
        {
            foreach (var (regex, converter) in replacements)
            {
                input = regex.Replace(input, converter);
            }

            return input;
        }

        private static string MatchCase(string casePattern, string textPattern)
        {
            var matchLength = Math.Min(casePattern.Length, textPattern.Length);

            var resultBuilder = new StringBuilder();
            for (var i = 0; i < matchLength; i++)
            {
                var isUpper = char.IsUpper(casePattern[i]);
                resultBuilder.Append(isUpper ? char.ToUpper(textPattern[i]) : char.ToLower(textPattern[i]));
            }

            if (matchLength < textPattern.Length)
            {
                resultBuilder.Append(textPattern.Substring(matchLength));
            }

            return resultBuilder.ToString();
        }

        private static (Regex, MatchEvaluator)[] GetUwuReplacements()
        {
            return new (Regex, MatchEvaluator)[] {
                (new Regex("ll",  RegexCI), m => MatchCase(m.Value, "wl")),
                (new Regex("r",  RegexCI), m => MatchCase(m.Value, "w")),
                (new Regex("th", RegexCI), m => MatchCase(m.Value, "dw")),
                (
                    new Regex("(n)([aeiou])", RegexCI), m =>
                    {
                        var letter = m.Groups[1].Value;
                        var vowel =  m.Groups[2].Value;
                        return letter + MatchCase(vowel, "y") + vowel;
                    }
                ),
                (
                    new Regex("(f)([aeiou])", RegexCI), m =>
                    {
                        var letter = m.Groups[1].Value;
                        var vowel =  m.Groups[2].Value;
                        return letter + MatchCase(vowel, "w") + vowel;
                    }
                ),
                (
                    new Regex("([aeiou])(d)", RegexCI), m =>
                    {
                        var vowel =  m.Groups[1].Value;
                        var letter = m.Groups[2].Value;
                        return vowel + MatchCase(letter, "w") + letter;
                    }
                ),
                (
                    new Regex("(f|b|sh)(uck|itch|it)", RegexCI), m =>
                    {
                        var start =  m.Groups[1].Value;
                        var end = m.Groups[2].Value;
                        return start + MatchCase(end, "w") + end;
                    }
                ),
                (
                    new Regex("(d)(amn)", RegexCI), m =>
                    {
                        var start =  m.Groups[1].Value;
                        var end = m.Groups[2].Value;
                        return start + MatchCase(end, "y") + end;
                    }
                )
            };
        }
    }
}