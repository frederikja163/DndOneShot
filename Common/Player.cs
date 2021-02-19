
using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Common
{
    public record PlayerData(string Name = "Name")
    {
        
        [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
        public Language[] Languages { get; init; } = new[] {Language.Common, Language.Elvish, Language.Dwarvish};

        [JsonConstructor]
        public PlayerData(Language[] languages) : this()
        {
            Languages = languages;
        }
    }

    public interface IPlayer
    {
        public PlayerData Data { get; set; }

        public event Func<IPlayer, string, bool> OnMessageRecieved;

        public void SendMessage(Language language, string message)
        {
            if (!Data.Languages.Contains(language))
            {
                SendMessageRaw(Utility.ConvertToJibberish(message));
            }
            else
            {
                SendMessageRaw(message);
            }
        }

        public void SendMessageRaw(string message);
    }
}