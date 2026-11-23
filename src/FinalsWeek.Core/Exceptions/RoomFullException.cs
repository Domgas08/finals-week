namespace FinalsWeek.Core.Exceptions;

public class RoomFullException(string roomCode, int maxPlayers)
    : RoomException(roomCode, $"Room '{roomCode}' is full (maximum players allowed: {maxPlayers}).")
{
    public int MaxPlayers { get; } = maxPlayers;
}