using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace TTMMBot.Data.Entities
{
    [Table("Clan")]
    public class Clan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ClanID { get; set; }

        [Required]
        public string Tag { get; set; }

        public string Name { get; set; }

        public virtual ICollection<Member> Member { get; set; }
        public override string ToString() => $"[{Tag}]";
    }
}
