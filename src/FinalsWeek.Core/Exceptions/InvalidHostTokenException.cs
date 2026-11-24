namespace FinalsWeek.Core.Exceptions;

public class InvalidHostTokenException(string roomCode)
    : RoomException(roomCode, "The provided host token does not match the expected token.");