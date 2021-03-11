using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MMBot.Data.Interfaces;

namespace MMBot.Blazor.ViewModels
{
    public interface ICRUDViewModel<TEntity> where TEntity : new()
    {
        Task Create();
        Task Delete(int? id);
        string Entity { get; }
        ICollection<TEntity> Entities { get; }
    }
}
