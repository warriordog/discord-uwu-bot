using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DiscordBotTemplate
{
    public class BotOptions
    {
        [Required]
        [NotNull]
        public string DiscordToken { get; set; }
    }
}