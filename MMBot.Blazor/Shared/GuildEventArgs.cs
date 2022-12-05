namespace MMBot.Blazor.Shared;

public class GuildEventArgs : EventArgs
{
    public GuildEventArgs(string newValue) 
        => NewValue = newValue;

    public string NewValue { get; set; }
}
