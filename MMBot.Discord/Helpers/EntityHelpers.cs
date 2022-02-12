using Discord;
using MMBot.Data.Interfaces;
using MMBot.Discord.Services.Interfaces;

namespace MMBot.Discord.Helpers;

public static class EntityHelpers
{
    public async static Task<T> FindAndAskForEntity<T>(this IEnumerable<T> entities, ulong guildId, string search, IMessageChannel smc, ICommandHandler ch)
        where T : class, IHaveIdentifier
        => await entities
            .FindEntity(search)
            .AskForEntityAsync(guildId, smc, ch);

    private static IEnumerable<T> FindEntity<T>(this IEnumerable<T> entities, string search)
        where T : class, IHaveIdentifier
        => entities.Where(x => x.Identitfier.ToLower().Contains(search.ToLower()));

    private async static Task<T> AskForEntityAsync<T>(this IEnumerable<T> entities, ulong guildId, IMessageChannel smc, ICommandHandler ch)
        where T : class, IHaveIdentifier
    {
        if (entities.Count() > 5)
        {
            await smc.SendMessageAsync("Be more precise with your input!");
            return default;
        }

        if (!entities.Any())
        {
            await smc.SendMessageAsync($"I couldn't find anyone/anything with your input!");
            return default;
        }

        if (entities.Count() == 1)
            return entities.FirstOrDefault();

        var emojis = new IEmote[]
        {
               new Emoji("1️⃣"),
               new Emoji("2️⃣"),
               new Emoji("3️⃣"),
               new Emoji("4️⃣"),
               new Emoji("5️⃣")
        }
        .Take(entities.Count())
        .ToArray();

        var qstring = $"```Select the correct {entities.FirstOrDefault().GetType()}:{Environment.NewLine}";

        var i = 0;
        foreach (var me in entities)
            qstring += $"{emojis[++i - 1].Name} {me.Identitfier} - {me.Name} {Environment.NewLine}";

        qstring = $"{qstring.TrimEnd(Environment.NewLine.ToCharArray())}```";

        var question = await smc.SendMessageAsync(qstring);

        IHaveIdentifier m = null;

        await question.AddReactionsAsync(emojis);

        await ch.AddToReactionList(guildId, question, async (r, u) =>
        {
            if (emojis.Select(r => r.Name).Contains(r.Name))
            {
                m = entities.ElementAt(emojis.Select((x, i) => new { x, i }).FirstOrDefault(x => x.x.Name == r.Name).i);

                if (m is not null)
                    await question.DeleteAsync();
            }
        }, false);

        return m as T;
    }
}
