
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Common
{
    public record PlayerData
    {
        
        [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
        public Language[] Languages { get; } = new[] {Language.Common, Language.Elvish, Language.Dwarvish};
        public string Name { get; } = "Name";

        public PlayerData(){}
        
        [JsonConstructor]
        public PlayerData(Language[] languages, string name)
        {
            Languages = languages;
            Name = name;
        }
    }

    public interface IPlayer
    {
        public PlayerData Data { get; set; }
        
        public void SendMessage(Language language, string message);
    }
}