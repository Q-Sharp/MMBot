using System;
using System.Collections.Generic;
using System.Text;

namespace TTMMBot.Data.Entities
{
    public interface IHaveId
    {
        int Id { get; set; }

        void Update(object guildSettings);
    }
}
