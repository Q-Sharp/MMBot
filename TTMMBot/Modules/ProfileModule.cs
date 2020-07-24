using TTMMBot.Services;
using Discord.Commands;
using Discord;
using System.Threading.Tasks;
using Discord.WebSocket;
using System.Linq;

namespace TTMMBot.Modules
{
    [Name("Profile")]
    public class ProfileModule : ModuleBase<SocketCommandContext>
    {
        private readonly IDatabaseService _database;
        public ProfileModule(IDatabaseService database)
        {
            _database = database;
            database.Migrate();
        }

        [Command("say"), Alias("s")]
        [Summary("Make the bot say something")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public Task Say([Remainder] string text) => ReplyAsync(text);

        [Command("nick"), Priority(0)]
        [Summary("Change another user's nickname to the specified text")]
        [RequireUserPermission(GuildPermission.ManageNicknames)]
        public async Task Nick(SocketGuildUser user, [Remainder] string name)
        {
            await user.ModifyAsync(x => x.Nickname = name);
            await ReplyAsync($"{user.Mention} I changed your name to **{name}**");
        }

        [Command("isinteger")]
        [Summary("Check if the input text is a whole number.")]
        public Task IsInteger(int number)
            => ReplyAsync($"The text {number} is a number!");

        [Command("multiply")]
        [Summary("Get the product of two numbers.")]
        public async Task Say(int a, int b)
        {
            int product = a * b;
            await ReplyAsync($"The product of `{a} * {b}` is `{product}`.");
        }

        [Command("addmany")]
        [Summary("Get the sum of many numbers")]
        public async Task Say(params int[] numbers)
        {
            int sum = numbers.Sum();
            await ReplyAsync($"The sum of `{string.Join(", ", numbers)}` is `{sum}`.");


        }
    }
}
