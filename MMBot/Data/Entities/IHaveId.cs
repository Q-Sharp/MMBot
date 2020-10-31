namespace MMBot.Data.Entities
{
    public interface IHaveId
    {
        int Id { get; set; }

        void Update(object guildSettings);
    }
}
