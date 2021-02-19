using System;
using System.IO;
using System.Linq;
using Common;
using DiscordServer;
using Newtonsoft.Json;

namespace ApplicationLogic
{
    class Program
    {
        static void Main(string[] args)
        {
            var key = File.ReadAllText("key.txt");
            using var bot = new DiscordBot(key);
            var players = bot.GetPlayers();

            foreach (var player in players)
            {
                player.OnMessageRecieved += OnMessageRecieved;
            }
            players.SendMessage(Language.Dwarvish, "Halløjsa med dig123");
            
            Console.ReadKey();
        }

        static bool OnMessageRecieved(IPlayer player, string message)
        {
            if (message.StartsWith("```json"))
            {
                message = message[7..^3];
                player.Data = JsonConvert.DeserializeObject<PlayerData>(message);
                return true;
            }
            if (message.StartsWith("{"))
            {
                player.Data = JsonConvert.DeserializeObject<PlayerData>(message);
                return true;
            }

            player.SendMessage(Language.Dwarvish, player.Data.Name);

            return false;
        }
    }
}