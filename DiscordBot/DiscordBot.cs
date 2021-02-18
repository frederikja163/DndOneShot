using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;

namespace DiscordServer
{
    public sealed class DiscordBot : IDisposable
    {
        private readonly DiscordSocketClient _bot = new DiscordSocketClient(new DiscordSocketConfig(){AlwaysDownloadUsers = true});
        private SocketGuild _guild;
        private Player[] _players;
        private SocketCategoryChannel _categoryChannel;

        public DiscordBot(string key)
        {
            _bot.MessageReceived += OnMessageReceived;
            
            _bot.LoginAsync(TokenType.Bot, key);
            _bot.StartAsync();

            while (_bot.LoginState == LoginState.LoggingIn) Thread.Sleep(1);
            SetStatus(UserStatus.DoNotDisturb, "Setting everything up.");
            
            while((_guild = _bot.GetGuild(811535481690259518)) == null) Thread.Sleep(1);
            
            while ((_categoryChannel = _guild.GetCategoryChannel(811717556938735638)) == null) Thread.Sleep(1);

            IReadOnlyCollection<SocketGuildUser> sockets;
            while ((sockets = _guild.Users).Count <= 1) Thread.Sleep(1);
            _players = sockets.Where(user => user.Roles.Any(role => role.Id == 811540397847347220))
                .Select(user =>
                {
                    var channelName = user.Nickname.ToLower() + "s-tattoo";
                    if (!(_categoryChannel.Channels.FirstOrDefault(c => c.Name == channelName) is ITextChannel channel))
                    {
                        channel = _guild.CreateTextChannelAsync(channelName, 
                            properties => properties.CategoryId = 811717556938735638).Result;
                        channel.AddPermissionOverwriteAsync(user, new OverwritePermissions(viewChannel: PermValue.Allow, sendMessages: PermValue.Allow));
                    }
                    return new Player(user, channel);
                }).ToArray();
            SetStatus(UserStatus.Online, "Working");
        }

        public Player[] GetPlayers()
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
            if (socketMessage is SocketUserMessage userMessage)
            {
                userMessage.PinAsync();
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