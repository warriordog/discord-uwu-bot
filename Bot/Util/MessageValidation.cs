using DSharpPlus.Entities;

namespace DiscordUwuBot.Bot.Util
{
    public static class MessageValidation
    {
        public static bool IsMessageLoop(DiscordUser currentUser, DiscordMessage message)
        {
            for (var currentMessage = message; currentMessage != null; currentMessage = currentMessage.Reference?.Message)
            {
                // If we sent the message, then this is a loop
                if (currentUser.Equals(currentMessage.Author))
                {
                    return true;
                }
            }

            // If we get to the end without a match, then this is not a loop
            return false;
        }
    }
}