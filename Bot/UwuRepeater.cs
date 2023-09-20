using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DiscordUwuBot.Bot.Util;
using DiscordUwuBot.UwU;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;

namespace DiscordUwuBot.Bot;

/// <summary>
/// Module that repeats and uwuifies messages sent by certain followed users.
/// </summary>
public interface IUwuRepeater
{
    /// <summary>
    /// Process an incoming message posted to a channel
    /// </summary>
    /// <param name="discord">Discord client instance</param>
    /// <param name="evt">MessageCreate event</param>
    /// <returns>Returns a task that completes when the message is fully processed</returns>
    public Task OnMessageCreated(DiscordClient discord, MessageCreateEventArgs evt);
        
    /// <summary>
    /// Begin following a user in a specified channel.
    /// Duplicate follows will be ignored.
    /// </summary>
    /// <param name="user">User to follow</param>
    /// <param name="channel">Channel to follow in</param>
    public void FollowUser(DiscordUser user, DiscordChannel channel);
        
    /// <summary>
    /// Stop following a user in a specified channel.
    /// Requests to unfollow a user who is not followed will be ignored.
    /// </summary>
    /// <param name="user">User to stop following</param>
    /// <param name="channel">Channel to stop following in</param>
    public void UnfollowUser(DiscordUser user, DiscordChannel channel);
        
    /// <summary>
    /// Stop following all users in all channels
    /// </summary>
    public void ClearFollows();
        
    /// <summary>
    /// Stop following a user in all channels
    /// </summary>
    /// <param name="user">User to stop following</param>
    public void ClearFollowsForUser(DiscordUser user);
        
    /// <summary>
    /// Stop following a user in all channels in a specified guild
    /// </summary>
    /// <param name="user">User to stop following</param>
    /// <param name="guild">Guild to stop following in</param>
    public void ClearFollowsForUserGuild(DiscordUser user, DiscordGuild guild);
        
    /// <summary>
    /// Stop following all users in a channel
    /// </summary>
    /// <param name="channel">Channel to stop following in</param>
    public void ClearFollowsForChannel(DiscordChannel channel);
        
    /// <summary>
    /// Stop following all users in all channels in a guild
    /// </summary>
    /// <param name="guild">Guild to stop following in</param>
    public void ClearFollowsForGuild(DiscordGuild guild);
        
    /// <summary>
    /// Check if a user is followed in a specified channel
    /// </summary>
    /// <param name="user">User to check</param>
    /// <param name="channel">Channel to check in</param>
    /// <returns>Returns true if the user is followed in the channel</returns>
    public bool IsUserFollowed(DiscordUser user, DiscordChannel channel);
}
    
public class UwuRepeater : IUwuRepeater
{
    private readonly Dictionary<FollowedUser, DateTime> _followedUsers = new();
    private readonly TimeSpan _remindInterval = new(0, 15, 0);
    private readonly ITextUwuifier _textUwuifier;
    private readonly ILogger<UwuRepeater> _logger;

    public UwuRepeater(ITextUwuifier textUwuifier, ILogger<UwuRepeater> logger)
    {
        _textUwuifier = textUwuifier;
        _logger = logger;
    }

    public async Task OnMessageCreated(DiscordClient discord, MessageCreateEventArgs evt)
    {
        // Make sure that we are following the sender
        if (_followedUsers.TryGetValue(new FollowedUser(evt.Author.Id, evt.Channel.Id, evt.Channel.GuildId), out var lastRemindTime))
        {
            // Get the original message and make sure that it isn't empty
            var message = evt.Message.Content;
            if (string.IsNullOrWhiteSpace(message)) return;
                
            // Stop if this is a message loop
            if (MessageValidation.IsDeepReply(discord.CurrentUser, evt.Message)) return;
                
            // Create UwU message
            var uwuText = _textUwuifier.UwuifyText(message);

            // Append stop reminder if enough time has elapsed
            if (DateTime.Now - lastRemindTime > _remindInterval)
            {
                uwuText += $"\nIf you want me to stop, just say { Formatter.InlineCode("uwu*stop") }.";
            }
                
            // Send response
            await new DiscordMessageBuilder()
                .WithContent(uwuText)
                .WithReply(evt.Message.Id)
                .SendAsync(evt.Channel);
                
            _logger.LogDebug("Translating message from followed user [{user}]", evt.Author);
        }
    }

    public void FollowUser(DiscordUser user, DiscordChannel channel)
        => _followedUsers.Add(new FollowedUser(user.Id, channel.Id, channel.GuildId), DateTime.Now);

    public void UnfollowUser(DiscordUser user, DiscordChannel channel)
        => _followedUsers.Remove(new FollowedUser(user.Id, channel.Id, channel.GuildId));

    public void ClearFollows()
        => _followedUsers.Clear();

    public void ClearFollowsForUser(DiscordUser user)
        => _followedUsers.RemoveWhere(entry => entry.Key.UserId == user.Id);

    public void ClearFollowsForUserGuild(DiscordUser user, DiscordGuild guild)
        => _followedUsers.RemoveWhere(entry => entry.Key.UserId == user.Id && entry.Key.GuildId == guild.Id);

    public void ClearFollowsForChannel(DiscordChannel channel)
        => _followedUsers.RemoveWhere(entry => entry.Key.ChannelId == channel.Id);

    public void ClearFollowsForGuild(DiscordGuild guild)
        => _followedUsers.RemoveWhere(entry => entry.Key.GuildId == guild.Id);

    public bool IsUserFollowed(DiscordUser user, DiscordChannel channel)
        => _followedUsers.ContainsKey(new FollowedUser(user.Id, channel.Id, channel.GuildId));
}

internal sealed record FollowedUser(ulong UserId, ulong ChannelId, ulong? GuildId)
{
    public bool Equals(FollowedUser? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return UserId == other.UserId && ChannelId == other.ChannelId;
    }

    public override int GetHashCode() => HashCode.Combine(UserId, ChannelId);
}