using Common;
using Discord;
using Discord.WebSocket;

namespace DiscordServer
{
    public class Player : IPlayer
    {
        private readonly SocketGuildUser _user;
        private readonly ITextChannel _channel;

        public Player(SocketGuildUser user, ITextChannel channel)
        {
            _user = user;
            _channel = channel;
        }

        public void SendMessage(Languages languages, string message)
        {
            _channel.SendMessageAsync(message);
        }
    }
}