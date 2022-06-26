namespace MMBot.Data.Contracts;

public interface IHaveIdentifier
{
    string Identitfier { get; }
    string Name { get; }
    ulong GuildId { get; }
}
