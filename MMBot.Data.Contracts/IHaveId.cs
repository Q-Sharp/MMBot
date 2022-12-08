namespace MMBot.Data.Contracts;

public interface IHaveId
{
    int Id { get; set; }
    object Update(object HaveId);
}
