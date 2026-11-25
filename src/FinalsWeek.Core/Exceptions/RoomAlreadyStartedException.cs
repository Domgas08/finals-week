namespace FinalsWeek.Core.Exceptions;

public class RoomAlreadyStartedException(string roomCode)
    : RoomException(roomCode, $"Room '{roomCode}' has already started and does not allow late joining.");