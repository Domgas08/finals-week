namespace FinalsWeek.Core.Tests;

using FinalsWeek.Core.Exceptions;
using Xunit;

public class RoomExceptionTests
{
    [Fact]
    public void RoomNotFoundException_SetsPropertiesAndMessage()
    {
        var exception = new RoomNotFoundException("ABC123");

        Assert.Equal("ABC123", exception.RoomCode);
        Assert.Equal(
            "No active room was found with code 'ABC123'.",
            exception.Message);
    }

    [Fact]
    public void RoomFullException_SetsPropertiesAndMessage()
    {
        var exception = new RoomFullException("ABC123", 4);

        Assert.Equal("ABC123", exception.RoomCode);
        Assert.Equal(4, exception.MaxPlayers);
        Assert.Equal(
            "Room 'ABC123' is full (maximum players allowed: 4).",
            exception.Message);
    }

    [Fact]
    public void RoomAlreadyStartedException_SetsPropertiesAndMessage()
    {
        var exception = new RoomAlreadyStartedException("ABC123");

        Assert.Equal("ABC123", exception.RoomCode);
        Assert.Equal(
            "Room 'ABC123' has already started and does not allow late joining.",
            exception.Message);
    }

    [Fact]
    public void RoomAlreadyFinishedException_SetsPropertiesAndMessage()
    {
        var exception = new RoomAlreadyFinishedException("ABC123");

        Assert.Equal("ABC123", exception.RoomCode);
        Assert.Equal(
            "Room 'ABC123' has already finished.",
            exception.Message);
    }
}
