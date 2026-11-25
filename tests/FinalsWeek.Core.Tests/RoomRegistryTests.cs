namespace FinalsWeek.Core.Tests;

using System.Collections.Concurrent;
using FinalsWeek.Core;
using FinalsWeek.Core.Exceptions;
using FinalsWeek.Application;
using Xunit;

public class RoomRegistryTests
{
    private const string Name = "Tom";

    [Fact]
    public void CreateRoom_ReturnsSixCharacterCodeAndHostToken()
    {
        var registry = new RoomRegistry();

        var room = CreateRoom(registry);

        Assert.Equal(6, room.Code.Length);
        Assert.False(string.IsNullOrWhiteSpace(room.HostToken));
        Assert.Equal(RoomState.Lobby, registry.GetSnapshot(room.Code).State);
    }

    [Fact]
    public void CreateRoom_ManyRoomsAtOnce_AllCodesAreUnique()
    {
        var registry = new RoomRegistry();
        var codes = new ConcurrentBag<string>();

        Parallel.For(0, 500, _ => codes.Add(CreateRoom(registry).Code));

        Assert.Equal(500, codes.Distinct().Count());
    }

    [Fact]
    public void CreateRoom_WithShuffle_KeepsAllQuestions()
    {
        var registry = new RoomRegistry();

        var room = registry.CreateRoom(CreateDeck(questionCount: 10), Settings(shuffle: true));

        Assert.Equal(10, registry.GetSnapshot(room.Code).QuestionCount);
    }

    [Fact]
    public void Join_FiftyPlayersAtOnce_AllGetInWithUniqueNames()
    {
        var registry = new RoomRegistry();
        var room = CreateRoom(registry, maxPlayers: 100);

        Parallel.For(0, 50, _ => registry.Join(room.Code, Name));

        var players = registry.GetSnapshot(room.Code).Players;
        Assert.Equal(50, players.Count);
        Assert.Equal(50, players.Select(p => p.DisplayName).Distinct().Count());
    }

    [Fact]
    public void Join_FortyIntoThirty_ExactlyThirtyGetInAndTenAreRejected()
    {
        var registry = new RoomRegistry();
        var room = CreateRoom(registry, maxPlayers: 30);
        var rejected = 0;
        var changes = 0;
        registry.RoomChanged += _ => Interlocked.Increment(ref changes);

        Parallel.For(0, 40, _ =>
        {
            try
            {
                registry.Join(room.Code, Name);
            }
            catch (RoomFullException)
            {
                Interlocked.Increment(ref rejected);
            }
        });

        Assert.Equal(30, registry.GetSnapshot(room.Code).Players.Count);
        Assert.Equal(10, rejected);
        Assert.Equal(30, changes);
    }

    [Fact]
    public void Join_UnknownCode_ThrowsRoomNotFound()
    {
        var registry = new RoomRegistry();

        Assert.Throws<RoomNotFoundException>(() => registry.Join("ZZZZZZ", Name));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Join_EmptyCode_ThrowsArgumentException(string code)
    {
        var registry = new RoomRegistry();

        Assert.ThrowsAny<ArgumentException>(() => registry.Join(code, Name));
    }

    [Fact]
    public void Join_CodeWithSpacesAndLowerCase_FindsRoom()
    {
        var registry = new RoomRegistry();
        var room = CreateRoom(registry);

        registry.Join($"  {room.Code.ToLowerInvariant()}  ", Name);

        Assert.Single(registry.GetSnapshot(room.Code).Players);
    }

    [Fact]
    public void Join_AfterStart_ThrowsRoomAlreadyStarted()
    {
        var registry = new RoomRegistry();
        var room = CreateRoom(registry);
        registry.Start(room.Code, room.HostToken);

        Assert.Throws<RoomAlreadyStartedException>(() => registry.Join(room.Code, Name));
    }

    [Fact]
    public void Join_AfterEnd_ThrowsRoomAlreadyFinished()
    {
        var registry = new RoomRegistry();
        var room = CreateRoom(registry);
        registry.End(room.Code, room.HostToken);

        Assert.Throws<RoomAlreadyFinishedException>(() => registry.Join(room.Code, Name));
    }

    [Fact]
    public void Join_RaisesRoomChangedWithRoomCode()
    {
        var registry = new RoomRegistry();
        var room = CreateRoom(registry);
        string? changedCode = null;
        registry.RoomChanged += code => changedCode = code;

        registry.Join(room.Code, Name);

        Assert.Equal(room.Code, changedCode);
    }

    [Fact]
    public void Join_WhenRoomIsFull_DoesNotRaiseRoomChanged()
    {
        var registry = new RoomRegistry();
        var room = CreateRoom(registry, maxPlayers: 1);
        registry.Join(room.Code, Name);
        var changes = 0;
        registry.RoomChanged += _ => changes++;

        Assert.Throws<RoomFullException>(() => registry.Join(room.Code, "Ann"));
        Assert.Equal(0, changes);
    }

    [Fact]
    public void Start_WithWrongHostToken_ThrowsAndRoomStaysInLobby()
    {
        var registry = new RoomRegistry();
        var room = CreateRoom(registry);

        Assert.Throws<InvalidHostTokenException>(() => registry.Start(room.Code, "wrong-token"));
        Assert.Equal(RoomState.Lobby, registry.GetSnapshot(room.Code).State);
    }

    [Fact]
    public void Start_WithHostToken_ShowsFirstQuestion()
    {
        var registry = new RoomRegistry();
        var room = CreateRoom(registry);

        registry.Start(room.Code, room.HostToken);

        var snapshot = registry.GetSnapshot(room.Code);
        Assert.Equal(RoomState.InProgress, snapshot.State);
        Assert.Equal(0, snapshot.CurrentQuestionIndex);
        Assert.NotNull(snapshot.CurrentQuestion);
    }

    [Fact]
    public void Next_OnLastQuestion_FinishesRoom()
    {
        var registry = new RoomRegistry();
        var room = CreateRoom(registry);
        registry.Start(room.Code, room.HostToken);

        registry.Next(room.Code, room.HostToken);

        Assert.Equal(RoomState.Finished, registry.GetSnapshot(room.Code).State);
    }

    [Fact]
    public void GetSnapshot_IsACopy_LaterJoinsDoNotChangeIt()
    {
        var registry = new RoomRegistry();
        var room = CreateRoom(registry);
        registry.Join(room.Code, Name);

        var snapshot = registry.GetSnapshot(room.Code);
        registry.Join(room.Code, "Ann");

        Assert.Single(snapshot.Players);
    }

    private static CreatedRoom CreateRoom(RoomRegistry registry, int maxPlayers = 10)
    {
        return registry.CreateRoom(CreateDeck(questionCount: 1), Settings(maxPlayers: maxPlayers));
    }

    private static Deck CreateDeck(int questionCount)
    {
        var deck = new Deck("Test deck");

        for (var i = 0; i < questionCount; i++)
        {
            deck.AddQuestion(new Question($"Question {i + 1}?", ["A", "B"], 0));
        }

        return deck;
    }

    private static RoomSettings Settings(int maxPlayers = 10, bool shuffle = false)
    {
        return new RoomSettings(
            TimeLimitSeconds: 20,
            Shuffle: shuffle,
            AllowLateJoin: false,
            MaxPlayers: maxPlayers);
    }
}