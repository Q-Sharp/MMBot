namespace MMBot.Data.Interfaces
{
    public interface IHaveId
    {
        int Id { get; set; }

        void Update(object guildSettings);
    }
}
