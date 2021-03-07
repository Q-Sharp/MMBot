using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MMBot.Blazor.ViewModels
{
    public interface ICRUDViewModel
    {
        Task Create();
        Task Delete(int? id);
        string Entity { get; set; }
        IList<object> Entities { get; }
    }
}
