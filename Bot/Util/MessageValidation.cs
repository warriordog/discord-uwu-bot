using DSharpPlus.Entities;

namespace DiscordUwuBot.Bot.Util;

/// <summary>
/// Contains routines to validate discord messages
/// </summary>
public static class MessageValidation
{
    /// <summary>
    /// Checks if a message is a direct or indirect reply to the current user.
    /// An indirect reply is a message that is a reply of a reply [of a reply [...]] to a message by the current user.
    /// </summary>
    /// <param name="currentUser">User to check for</param>
    /// <param name="message">Root message</param>
    /// <returns>True if the message is a direct or indirect reply</returns>
    public static bool IsDeepReply(DiscordUser currentUser, DiscordMessage message)
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