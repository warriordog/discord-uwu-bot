using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DiscordUwuBot.Bot.Util;
using DiscordUwuBot.UwU;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;

namespace DiscordUwuBot.Bot
{
    public interface IUwuRepeater
    {
        public Task OnMessageCreated(DiscordClient discord, MessageCreateEventArgs evt);
        public void FollowUser(DiscordUser user, DiscordChannel channel);
        public void UnfollowUser(DiscordUser user, DiscordChannel channel);
        public void ClearFollows();
        public void ClearFollowsForUser(DiscordUser user);
        public void ClearFollowsForUserGuild(DiscordUser user, DiscordGuild guild);
        public void ClearFollowsForChannel(DiscordChannel channel);
        public void ClearFollowsForGuild(DiscordGuild guild);
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
                if (MessageValidation.IsMessageLoop(discord.CurrentUser, evt.Message)) return;
                
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
        {
            _followedUsers.Add(new FollowedUser(user.Id, channel.Id, channel.GuildId), DateTime.Now);
        }

        public void UnfollowUser(DiscordUser user, DiscordChannel channel)
        {
            _followedUsers.Remove(new FollowedUser(user.Id, channel.Id, channel.GuildId));
        }

        public void ClearFollows()
        {
            _followedUsers.Clear();
        }

        public void ClearFollowsForUser(DiscordUser user)
        {
            _followedUsers.RemoveWhere(entry => entry.Key.UserId == user.Id);
        }

        public void ClearFollowsForUserGuild(DiscordUser user, DiscordGuild guild)
        {
            _followedUsers.RemoveWhere(entry => entry.Key.UserId == user.Id && entry.Key.GuildId == guild.Id);
        }

        public void ClearFollowsForChannel(DiscordChannel channel)
        {
            _followedUsers.RemoveWhere(entry => entry.Key.ChannelId == channel.Id);
        }

        public void ClearFollowsForGuild(DiscordGuild guild)
        {
            _followedUsers.RemoveWhere(entry => entry.Key.GuildId == guild.Id);
        }

        public bool IsUserFollowed(DiscordUser user, DiscordChannel channel)
        {
            return _followedUsers.ContainsKey(new FollowedUser(user.Id, channel.Id, channel.GuildId));
        }
    }

    internal sealed record FollowedUser(ulong UserId, ulong ChannelId, ulong GuildId)
    {
        public bool Equals(FollowedUser other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return UserId == other.UserId && ChannelId == other.ChannelId;
        }

        public override int GetHashCode() => HashCode.Combine(UserId, ChannelId);
    }
}