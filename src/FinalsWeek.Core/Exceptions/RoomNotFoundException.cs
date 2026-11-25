namespace FinalsWeek.Core.Exceptions;

public class RoomNotFoundException(string roomCode)
    : RoomException(roomCode, $"No active room was found with code '{roomCode}'.");