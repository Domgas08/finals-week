namespace FinalsWeek.Core.Exceptions;

public class InvalidRoomStateException(string roomCode, RoomState state, string? action)
    : RoomException(roomCode, $"Cannot call '{action}' while room '{roomCode}' is in state '{state}");
