namespace FinalsWeek.Core;

public abstract class RoomException : Exception
{
    protected RoomException(string roomCode, string message)
        : base(message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(roomCode);
        RoomCode = roomCode;
    }

    public string RoomCode { get; }
}

#pragma warning disable SA1402
public class RoomNotFoundException(string roomCode)
    : RoomException(roomCode, $"No active room was found with code '{roomCode}'.");

public class RoomFullException(string roomCode, int maxPlayers)
    : RoomException(roomCode, $"Room '{roomCode}' is full (maximum players allowed: {maxPlayers}).")
{
    public int MaxPlayers { get; } = maxPlayers;
}

public class RoomAlreadyStartedException(string roomCode)
    : RoomException(roomCode, $"Room '{roomCode}' has already started and does not allow late joining.");

public class RoomAlreadyFinishedException(string roomCode)
    : RoomException(roomCode, $"Room '{roomCode}' has already finished.");

public class InvalidRoomStateException(string roomCode, RoomState state, string? action)
    : RoomException(roomCode, $"Room '{roomCode}' had an invalid state ({state}) in action ({action}).");

public class InvalidHostTokenException(string roomCode)
    : RoomException(roomCode, $"The provided host token does not match the expected token.");