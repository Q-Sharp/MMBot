using Discord;

namespace MMBot.Blazor.Services
{
    public class DCChannel
    {
        public string id { get; set; }       
        public string name { get; set; }        
        public bool owner { get; set; }       
        public int permissions { get; set; }
        
        public GuildPermission PermissionFlags => (GuildPermission)permissions;
    }
}
