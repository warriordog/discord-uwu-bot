using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

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
        public string DiscordToken { get; set; }
    }
}