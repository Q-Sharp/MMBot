using System.Collections.Generic;

namespace TTMMBot.Data.Entities
{
    public class Clan
    {
        public int ClanID { get; set; }

        public string Tag { get; set; }

        public string Name { get; set; }

        public ICollection<Member> Members { get; set; }
        public override string ToString() => $"[{Tag}]";
    }
}
