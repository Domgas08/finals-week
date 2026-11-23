namespace FinalsWeek.Core.Exceptions;

public class InvalidRoomStateException(string roomCode, RoomState state, string? action)
    : RoomException(roomCode, $"Room '{roomCode}' had an invalid state ({state}) in action ({action}).");
