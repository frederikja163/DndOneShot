using System;
using System.Linq;
using Common;
using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace DiscordServer
{
    public class Player : IPlayer
    {
        private readonly SocketGuildUser _user;
        private readonly ITextChannel _channel;
        private readonly IUserMessage _message;
        private PlayerData _data;

        public Player(SocketGuildUser user, ITextChannel channel, IUserMessage message)
        {
            _user = user;
            _channel = channel;
            _message = message;

            
            var json = _message.Embeds.First().Fields[0].Value[8..^3];
            Data = JsonConvert.DeserializeObject<PlayerData>(json);
        }

        public PlayerData Data
        {
            get => _data;
            set
            {
                _data = value;
                
            }
        }

        public event Action<string> OnMessageRecieved;
        
        public void SendMessage(Language language, string message)
        {
            _channel.SendMessageAsync(message);
        }

        internal void MessageRecieved(string message)
        {
            OnMessageRecieved?.Invoke(message);
        }
    }
}