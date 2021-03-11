using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using MMBot.Data.Interfaces;

namespace MMBot.Blazor.Pages
{
    public partial class DataList<TEntity> where TEntity : new()
    {
        [Parameter] public TEntity Type { get; set; } 
    }
}
