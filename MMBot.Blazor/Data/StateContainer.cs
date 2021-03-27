using System;


public class StateContainer
{
    public string SelectedGuildId { get; set; } = "0";

    public event Action OnChange;

    public void SetSelectedGuildId(string value)
    {
        SelectedGuildId = value;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
