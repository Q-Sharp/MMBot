using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;
using TTMMBot.Data.Entities;
using TTMMBot.Services.Interfaces;

namespace TTMMBot.TypeReaders
{
    public class ClanTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            try
            {
                var c = new Clan();
                foreach (var i in input.Split(" "))
                {
                    if (int.TryParse(i, out var id))
                        c.ClanID = id;
                    else if (!string.IsNullOrWhiteSpace(i))
                    {
                        if (i.Length <= 3 && string.IsNullOrWhiteSpace(c.Tag))
                            c.Tag = i;
                        else if (string.IsNullOrWhiteSpace(c.Name))
                            c.Name = i;
                    }
                }

                return Task.FromResult(TypeReaderResult.FromSuccess(c));
            }
            catch
            {
                return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Input could not be parsed as a boolean."));
            }
        }
    }
}
