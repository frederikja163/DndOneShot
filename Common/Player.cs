namespace Common
{
    public record PlayerData(Languages[] Languages, string Name)
    {
    }

    public interface IPlayer
    {
        public void SendMessage(Languages languages, string message);
    }
}