using System.Collections.Generic;

namespace Common
{
    public static class Extensions
    {
        public static void SendMessage(this IEnumerable<IPlayer> playerList, Language language, string message)
        {
            foreach (var player in playerList)
            {
                player.SendMessage(language, message);
            }
        }
    }
}