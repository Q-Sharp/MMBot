using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;
using TTMMBot.Data.Entities;
using TTMMBot.Services.Interfaces;

namespace TTMMBot.TypeReaders
{
    public class MemberTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            try
            {
                var m = new Member();
                foreach (var i in input.Split(" "))
                {
                    if (int.TryParse(i, out var id))
                        m.MemberID = id;
                    else if (!string.IsNullOrWhiteSpace(i) && string.IsNullOrWhiteSpace(m.Name) && i.Contains("#") && m.DiscordID == 0)
                    {
                        m.DiscordID = context.Channel.GetUsersAsync(CacheMode.AllowDownload).FlattenAsync().GetAwaiter().GetResult().FirstOrDefault(u => u.Discriminator == i).Id;
                    }
                    else if (!string.IsNullOrWhiteSpace(i) && string.IsNullOrWhiteSpace(m.Name))
                        m.Name = i;
                }

                return Task.FromResult(TypeReaderResult.FromSuccess(m));
            }
            catch
            {
                return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Input could not be parsed as a boolean."));
            }
        }
    }
}
