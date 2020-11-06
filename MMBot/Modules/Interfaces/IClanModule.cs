﻿using Discord.Commands;
using System.Threading.Tasks;

namespace MMBot.Modules.Interfaces
{
    public interface IClanModule
    {
        Task<RuntimeResult> List(string tag = null);
        Task<RuntimeResult> Delete(string tag);
        Task<RuntimeResult> SetCommand(string tag, string propertyName, [Remainder] string value);
        Task<RuntimeResult> Create(string tag, [Remainder] string name);
        Task<RuntimeResult> AddMember(string tag, string memberName);
    }
}
