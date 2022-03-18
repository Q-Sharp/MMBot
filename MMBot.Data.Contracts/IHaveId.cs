namespace MMBot.Data.Contracts;

public interface IHaveId
{
    int Id { get; set; }
    void Update(object HaveId);
}
