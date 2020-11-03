using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using MMBot.Data.Entities;
using MMBot.Services.Interfaces;

namespace MMBot.Helpers
{
    public static class MemberHelpers
    {
        public static async Task<Member> FindAndAskForMember(this IEnumerable<Member> members, ulong guildId, string search, IMessageChannel smc, ICommandHandler ch) 
            => await members
                .FindMember(search)
                .AskForMemberAsync(guildId, smc, ch);

        private static IEnumerable<Member> FindMember(this IEnumerable<Member> members, string search) 
            => members.Where(x => x.Name.ToLower().Contains(search.ToLower()));

        private static async Task<Member> AskForMemberAsync(this IEnumerable<Member> members, ulong guildId, IMessageChannel smc, ICommandHandler ch)
        {
            if(members.Count() > 5)
            {
                await smc.SendMessageAsync("Be more precise with your input");
                return default;
            }
                
            if(members.Count() == 0)
            {
                await smc.SendMessageAsync("I couldn't find anyone with that name!");
                return default;
            }

            if(members.Count() == 1)
                return members.FirstOrDefault();

            var emojis = new IEmote[] 
            {
               new Emoji("1️⃣"),
               new Emoji("2️⃣"),
               new Emoji("3️⃣"),
               new Emoji("4️⃣"),
               new Emoji("5️⃣")
            }
            .Take(members.Count())
            .ToArray();

            var qstring = $"```Select the correct member:{Environment.NewLine}";

            var i = 0;
            foreach(var me in members)
                qstring += $"{emojis[++i-1].Name} {me.Name} {Environment.NewLine}";

            qstring = $"{qstring.TrimEnd(Environment.NewLine.ToCharArray())}```";

            var question = await smc.SendMessageAsync(qstring);

            Member m = null;
            await question.AddReactionsAsync(emojis);
            
            await ch.AddToReactionList(guildId, question, async (r, u) =>
            {
                if(emojis.Select(r => r.Name).Contains(r.Name))
                {
                    m = members.ElementAt(emojis.Select((x, i) => new { x, i }).FirstOrDefault(x => x.x.Name == r.Name).i);

                    if(m != null)
                        await question.DeleteAsync();
                }
            }, false);

            return m;
        }
    }
}
