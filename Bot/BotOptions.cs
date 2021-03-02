using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DiscordUwuBot.Bot
{
    public class BotOptions
    {
        [Required]
        [NotNull]
        public string DiscordToken { get; set; }
    }
}