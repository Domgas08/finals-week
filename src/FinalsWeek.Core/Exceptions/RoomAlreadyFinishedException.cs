namespace FinalsWeek.Core.Exceptions;

public class RoomAlreadyFinishedException(string roomCode)
    : RoomException(roomCode, $"Room '{roomCode}' has already finished.");