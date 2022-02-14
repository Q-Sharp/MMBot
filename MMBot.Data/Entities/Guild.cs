namespace MMBot.Data.Entities;

public record Guild
{
    public Guild(ulong id, string name)
    {
        Id = id;
        Name = name;
    }

    public ulong Id { get; }
    public string Name { get; }
}
