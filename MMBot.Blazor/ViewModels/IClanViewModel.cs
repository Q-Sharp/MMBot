using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MMBot.Data.Entities;

namespace MMBot.Blazor.ViewModels
{
    public interface IClanViewModel : ICRUDViewModel
    {
        IList<Clan> Clans { get; set; }
    }
}
