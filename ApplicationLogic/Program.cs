using System;
using System.IO;
using DiscordServer;

namespace ApplicationLogic
{
    class Program
    {
        static void Main(string[] args)
        {
            var key = File.ReadAllText("key.txt");
            using var bot = new DiscordBot(key);
            Console.ReadKey();
        }
    }
}