using Discord.Commands;
using MMBot.Discord.Enums;

namespace MMBot.Discord.Modules.Interfaces;

public interface IMemberModule
{
    Task<RuntimeResult> List(SortBy sortBy);
    Task<RuntimeResult> Sort();
    Task<RuntimeResult> Changes(string compact = null, bool useCurrent = false);
    Task<RuntimeResult> Profile(string name = null);
    Task<RuntimeResult> ShowAll(string propertyName, [Remainder] string value);
    Task<RuntimeResult> Delete([Remainder] string name);
    Task<RuntimeResult> Set(string name, string propertyName, [Remainder] string value);
    Task<RuntimeResult> Create([Remainder] string name);
}
