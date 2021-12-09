using System;

namespace MMBot.Blazor.ViewModels;

public interface IStateChanged
{
    public event Action PropertyChanged;
}
