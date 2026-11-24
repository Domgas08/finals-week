namespace FinalsWeek.Core;

public class RoomPlayer(Guid id, string displayName)
{
    public Guid Id { get; } = id;

    public string DisplayName { get; } = displayName;
}