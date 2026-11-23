namespace FinalsWeek.Core.Exceptions;

public abstract class RoomException : Exception
{
    protected RoomException(string roomCode, string message)
        : base(message)
    {
        RoomCode = roomCode;
    }

    public string RoomCode { get; }
}
