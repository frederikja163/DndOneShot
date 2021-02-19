using System;
using System.Collections.Generic;
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

        public Player(SocketGuildUser user, ITextChannel channel)
        {
            _user = user;
            _channel = channel;


            var pins = channel.GetPinnedMessagesAsync().Result;
            if ((_message = pins.FirstOrDefault() as IUserMessage) == null)
            {
                _message = channel.SendMessageAsync(embed: BuildEmbed(new PlayerData())).Result;
                _message.PinAsync();
            }
            
            var json = _message.Embeds.First().Fields[0].Value[8..^3];
            Data = JsonConvert.DeserializeObject<PlayerData>(json);
        }

        public PlayerData Data
        {
            get => _data;
            set
            {
                _data = value;
                _message.ModifyAsync(properties => properties.Embed = BuildEmbed(_data));
            }
        }

        public event Func<IPlayer, string, bool> OnMessageRecieved;
        
        public void SendMessageRaw(string message)
        {
            _channel.SendMessageAsync(message);
        }

        internal bool MessageRecieved(string message)
        {
            return OnMessageRecieved?.Invoke(this, message) ?? false;
        }

        private Embed BuildEmbed(PlayerData data)
        {
            var jsonField = "```json\n" + JsonConvert.SerializeObject(data, Formatting.Indented) + "\n```";
            var builder = new EmbedBuilder()
            {
                Title = "Player Character",
                // Author = new EmbedAuthorBuilder(){Name = "The book"},
                Color = Color.Blue,
                Description = "This is a template to put your character into.",
                Fields = new List<EmbedFieldBuilder>()
                {
                    new EmbedFieldBuilder()
                    {
                        IsInline = false,
                        Name = "Json",
                        Value = jsonField,
                    }
                },
            };
            return builder.Build();
        }
    }
}