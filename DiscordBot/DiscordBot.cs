using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace DiscordServer
{
    public sealed class DiscordBot : IDisposable
    {
        private const ulong GuildId = 811535481690259518;
        private const ulong CategoryId = 811717556938735638;
        private const ulong RoleId = 811540397847347220;
        
        private readonly DiscordSocketClient _bot = new DiscordSocketClient(new DiscordSocketConfig(){AlwaysDownloadUsers = true});
        private SocketGuild _guild;
        private Player[] _players;
        private SocketCategoryChannel _categoryChannel;
        private Dictionary<ulong, Player> _channelIdToPlayer = new Dictionary<ulong, Player>();

        public DiscordBot(string key)
        {
            _bot.MessageReceived += OnMessageReceived;
            
            _bot.LoginAsync(TokenType.Bot, key);
            _bot.StartAsync();

            while (_bot.LoginState == LoginState.LoggingIn) Thread.Sleep(1);
            SetStatus(UserStatus.DoNotDisturb, "Setting everything up.");
            
            while((_guild = _bot.GetGuild(GuildId)) == null) Thread.Sleep(1);
            
            while ((_categoryChannel = _guild.GetCategoryChannel(CategoryId)) == null) Thread.Sleep(1);

            IReadOnlyCollection<SocketGuildUser> sockets;
            while ((sockets = _guild.Users).Count <= 1) Thread.Sleep(1);

            _players = sockets.Where(user => user.Roles.Any(role => role.Id == RoleId))
                .Select(user =>
                {
                    var channelName = user.Nickname.ToLower() + "s-tattoo";
                    if (_categoryChannel.Channels.FirstOrDefault(c => c.Name == channelName) is not ITextChannel channel)
                    {
                        channel = _guild.CreateTextChannelAsync(channelName, 
                            properties => properties.CategoryId = CategoryId).Result;
                        channel.AddPermissionOverwriteAsync(user, new OverwritePermissions(viewChannel: PermValue.Allow, sendMessages: PermValue.Allow));
                    }
                    var player = new Player(user, channel);
                    _channelIdToPlayer.Add(channel.Id, player);
                    return player;
                }).ToArray();
            SetStatus(UserStatus.Online, "Working");
        }

        public IPlayer[] GetPlayers()
        {
            return _players;
        }

        public void SetStatus(UserStatus status, string message)
        {
            _bot.SetStatusAsync(status);
            _bot.SetGameAsync(message);
        }

        private Task OnMessageReceived(SocketMessage socketMessage)
        {
            if (socketMessage is SocketSystemMessage systemMessage)
            {
                if (systemMessage.Type == MessageType.ChannelPinnedMessage)
                {
                    systemMessage.DeleteAsync();
                }
            }
            else if (socketMessage is SocketUserMessage userMessage && userMessage.Author.Id != _bot.CurrentUser.Id)
            {
                if (_channelIdToPlayer.TryGetValue(userMessage.Channel.Id, out var player))
                {
                    if (player.MessageRecieved(userMessage.Content))
                    {
                        userMessage.DeleteAsync();
                    }
                }
            }
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _bot.LogoutAsync();
            while (_bot.LoginState == LoginState.LoggingOut) Thread.Sleep(1);
            _bot.StopAsync();
            _bot.Dispose();
        }
    }
}