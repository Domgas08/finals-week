namespace FinalsWeek.Core.Tests;

using FinalsWeek.Core;
using Xunit;

public class LiveRoomTests
{
    private const string HostToken = "host-secret";
    private const string Code = "ABC123";

    private static readonly RoomSettings DefaultSettings = new(
        TimeLimitSeconds: 30,
        Shuffle: false,
        AllowLateJoin: false,
        MaxPlayers: 10);

    [Fact]
    public void Constructor_SetsInitialState()
    {
        var deckId = Guid.NewGuid();
        var questions = CreateQuestions(2);

        var room = new LiveRoom(Code, deckId, HostToken, DefaultSettings, questions);

        Assert.Equal(Code, room.Code);
        Assert.Equal(deckId, room.DeckId);
        Assert.Equal(DefaultSettings, room.Settings);
        Assert.Equal(RoomState.Lobby, room.State);
        Assert.Equal(-1, room.CurrentQuestionIndex);
        Assert.Null(room.CurrentQuestion);
        Assert.Equal(questions, room.Questions);
        Assert.Empty(room.Players);
    }

    [Fact]
    public void Constructor_CopiesQuestionList()
    {
        var questions = CreateQuestions(2);
        var room = new LiveRoom(Code, Guid.NewGuid(), HostToken, DefaultSettings, questions);

        questions.Add(CreateQuestion());

        Assert.Equal(2, room.Questions.Count);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_BlankCode_Throws(string? code)
    {
        Assert.ThrowsAny<ArgumentException>(() =>
            new LiveRoom(code!, Guid.NewGuid(), HostToken, DefaultSettings, CreateQuestions(1)));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_BlankHostToken_Throws(string? token)
    {
        Assert.ThrowsAny<ArgumentException>(() =>
            new LiveRoom(Code, Guid.NewGuid(), token!, DefaultSettings, CreateQuestions(1)));
    }

    [Fact]
    public void Constructor_EmptyDeckId_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            new LiveRoom(Code, Guid.Empty, HostToken, DefaultSettings, CreateQuestions(1)));
    }

    [Fact]
    public void Constructor_NullSettings_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new LiveRoom(Code, Guid.NewGuid(), HostToken, null!, CreateQuestions(1)));
    }

    [Fact]
    public void Constructor_NullQuestions_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new LiveRoom(Code, Guid.NewGuid(), HostToken, DefaultSettings, null!));
    }

    [Fact]
    public void Constructor_NoQuestions_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            new LiveRoom(Code, Guid.NewGuid(), HostToken, DefaultSettings, []));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_MaxPlayersBelowOne_Throws(int maxPlayers)
    {
        var settings = DefaultSettings with { MaxPlayers = maxPlayers };

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new LiveRoom(Code, Guid.NewGuid(), HostToken, settings, CreateQuestions(1)));
    }

    [Fact]
    public void Join_AddsPlayerAndReturnsIt()
    {
        var room = CreateRoom();

        var player = room.Join("Ona");

        Assert.Equal("Ona", player.DisplayName);
        Assert.NotEqual(Guid.Empty, player.Id);
        Assert.Single(room.Players);
        Assert.Same(player, room.Players[0]);
    }

    [Fact]
    public void Join_TrimsDisplayName()
    {
        var room = CreateRoom();

        var player = room.Join("  Ona  ");

        Assert.Equal("Ona", player.DisplayName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Join_BlankName_Throws(string? name)
    {
        var room = CreateRoom();

        Assert.ThrowsAny<ArgumentException>(() => room.Join(name!));
        Assert.Empty(room.Players);
    }

    [Fact]
    public void Join_GivesEachPlayerUniqueId()
    {
        var room = CreateRoom();

        var a = room.Join("Ona");
        var b = room.Join("Jonas");

        Assert.NotEqual(a.Id, b.Id);
    }

    [Fact]
    public void Join_DuplicateName_AddsNumber()
    {
        var room = CreateRoom();

        var first = room.Join("Ona");
        var second = room.Join("Ona");

        Assert.Equal("Ona", first.DisplayName);
        Assert.Equal("Ona (2)", second.DisplayName);
    }

    [Fact]
    public void Join_DuplicateNameThreeTimes_IncrementsNumber()
    {
        var room = CreateRoom();

        room.Join("Ona");
        room.Join("Ona");
        var third = room.Join("Ona");

        Assert.Equal("Ona (3)", third.DisplayName);
    }

    [Fact]
    public void Join_DuplicateNameIsCaseInsensitive()
    {
        var room = CreateRoom();

        room.Join("Ona");
        var second = room.Join("ONA");

        Assert.Equal("ONA (2)", second.DisplayName);
    }

    [Fact]
    public void Join_DuplicateNameAfterTrimming_AddsNumber()
    {
        var room = CreateRoom();

        room.Join("Ona");
        var second = room.Join("  Ona ");

        Assert.Equal("Ona (2)", second.DisplayName);
    }

    [Fact]
    public void Join_NumberedNameAlreadyTaken_SkipsToNextFreeNumber()
    {
        var room = CreateRoom();

        room.Join("Ona (2)");
        room.Join("Ona");
        var third = room.Join("Ona");

        Assert.Equal("Ona (3)", third.DisplayName);
    }

    [Fact]
    public void Join_RoomFull_Throws()
    {
        var room = CreateRoom(DefaultSettings with { MaxPlayers = 2 });
        room.Join("Ona");
        room.Join("Jonas");

        var ex = Assert.Throws<RoomFullException>(() => room.Join("Petras"));

        Assert.Equal(Code, ex.RoomCode);
        Assert.Equal(2, ex.MaxPlayers);
        Assert.Equal(2, room.Players.Count);
    }

    [Fact]
    public void Join_RoomWithOneSlot_AllowsExactlyOnePlayer()
    {
        var room = CreateRoom(DefaultSettings with { MaxPlayers = 1 });

        room.Join("Ona");

        Assert.Throws<RoomFullException>(() => room.Join("Jonas"));
    }

    [Fact]
    public void Join_RoomFinished_Throws()
    {
        var room = CreateRoom();
        room.End(HostToken);

        var ex = Assert.Throws<RoomAlreadyFinishedException>(() => room.Join("Ona"));

        Assert.Equal(Code, ex.RoomCode);
        Assert.Empty(room.Players);
    }

    [Fact]
    public void Join_FinishedTakesPriorityOverLateJoin()
    {
        var room = CreateRoom(DefaultSettings with { AllowLateJoin = true });
        room.End(HostToken);

        Assert.Throws<RoomAlreadyFinishedException>(() => room.Join("Ona"));
    }

    [Fact]
    public void Join_InProgressWithoutLateJoin_Throws()
    {
        var room = CreateStartedRoom(DefaultSettings with { AllowLateJoin = false });

        var ex = Assert.Throws<RoomAlreadyStartedException>(() => room.Join("Ona"));

        Assert.Equal(Code, ex.RoomCode);
        Assert.Empty(room.Players);
    }

    [Fact]
    public void Join_InProgressWithLateJoin_Succeeds()
    {
        var room = CreateStartedRoom(DefaultSettings with { AllowLateJoin = true });

        var player = room.Join("Ona");

        Assert.Equal("Ona", player.DisplayName);
        Assert.Single(room.Players);
    }

    [Fact]
    public void Join_InProgressWithLateJoinButFull_Throws()
    {
        var room = CreateStartedRoom(DefaultSettings with { AllowLateJoin = true, MaxPlayers = 1 });
        room.Join("Ona");

        Assert.Throws<RoomFullException>(() => room.Join("Jonas"));
    }

    [Fact]
    public void Start_FromLobby_MovesToInProgressAtFirstQuestion()
    {
        var room = CreateRoom();

        room.Start(HostToken);

        Assert.Equal(RoomState.InProgress, room.State);
        Assert.Equal(0, room.CurrentQuestionIndex);
        Assert.Same(room.Questions[0], room.CurrentQuestion);
    }

    [Fact]
    public void Start_WhenAlreadyInProgress_Throws()
    {
        var room = CreateStartedRoom();

        var ex = Assert.Throws<InvalidRoomStateException>(() => room.Start(HostToken));

        Assert.Equal(Code, ex.RoomCode);
        Assert.Equal(RoomState.InProgress, room.State);
    }

    [Fact]
    public void Start_WhenFinished_Throws()
    {
        var room = CreateRoom();
        room.End(HostToken);

        Assert.Throws<InvalidRoomStateException>(() => room.Start(HostToken));
        Assert.Equal(RoomState.Finished, room.State);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("wrong-token")]
    [InlineData("HOST-SECRET")]
    public void Start_WrongHostToken_Throws(string? token)
    {
        var room = CreateRoom();

        var ex = Assert.Throws<InvalidHostTokenException>(() => room.Start(token!));

        Assert.Equal(Code, ex.RoomCode);
        Assert.Equal(RoomState.Lobby, room.State);
        Assert.Equal(-1, room.CurrentQuestionIndex);
    }

    [Fact]
    public void Next_AdvancesToFollowingQuestion()
    {
        var room = CreateStartedRoom();

        room.Next(HostToken);

        Assert.Equal(RoomState.InProgress, room.State);
        Assert.Equal(1, room.CurrentQuestionIndex);
        Assert.Same(room.Questions[1], room.CurrentQuestion);
    }

    [Fact]
    public void Next_OnLastQuestion_FinishesRoom()
    {
        var room = CreateStartedRoom(questionCount: 2);
        room.Next(HostToken);

        room.Next(HostToken);

        Assert.Equal(RoomState.Finished, room.State);
        Assert.Null(room.CurrentQuestion);
    }

    [Fact]
    public void Next_OnSingleQuestionRoom_FinishesImmediately()
    {
        var room = CreateStartedRoom(questionCount: 1);

        room.Next(HostToken);

        Assert.Equal(RoomState.Finished, room.State);
    }

    [Fact]
    public void Next_WalksThroughEveryQuestionInOrder()
    {
        var room = CreateStartedRoom(questionCount: 3);
        var seen = new List<Question> { room.CurrentQuestion! };

        room.Next(HostToken);
        seen.Add(room.CurrentQuestion!);
        room.Next(HostToken);
        seen.Add(room.CurrentQuestion!);

        Assert.Equal(room.Questions, seen);
    }

    [Fact]
    public void Next_FromLobby_Throws()
    {
        var room = CreateRoom();

        var ex = Assert.Throws<InvalidRoomStateException>(() => room.Next(HostToken));

        Assert.Equal(Code, ex.RoomCode);
        Assert.Equal(RoomState.Lobby, room.State);
    }

    [Fact]
    public void Next_WhenFinished_Throws()
    {
        var room = CreateStartedRoom(questionCount: 1);
        room.Next(HostToken);

        Assert.Throws<InvalidRoomStateException>(() => room.Next(HostToken));
        Assert.Equal(RoomState.Finished, room.State);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("wrong-token")]
    public void Next_WrongHostToken_Throws(string? token)
    {
        var room = CreateStartedRoom();

        Assert.Throws<InvalidHostTokenException>(() => room.Next(token!));

        Assert.Equal(RoomState.InProgress, room.State);
        Assert.Equal(0, room.CurrentQuestionIndex);
    }

    [Fact]
    public void End_FromLobby_FinishesRoom()
    {
        var room = CreateRoom();

        room.End(HostToken);

        Assert.Equal(RoomState.Finished, room.State);
    }

    [Fact]
    public void End_FromInProgress_FinishesRoom()
    {
        var room = CreateStartedRoom();

        room.End(HostToken);

        Assert.Equal(RoomState.Finished, room.State);
        Assert.Null(room.CurrentQuestion);
    }

    [Fact]
    public void End_WhenAlreadyFinished_Throws()
    {
        var room = CreateRoom();
        room.End(HostToken);

        var ex = Assert.Throws<InvalidRoomStateException>(() => room.End(HostToken));

        Assert.Equal(Code, ex.RoomCode);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("wrong-token")]
    public void End_WrongHostToken_Throws(string? token)
    {
        var room = CreateStartedRoom();

        Assert.Throws<InvalidHostTokenException>(() => room.End(token!));

        Assert.Equal(RoomState.InProgress, room.State);
    }

    [Fact]
    public void WrongHostToken_TakesPriorityOverInvalidState()
    {
        var room = CreateRoom();
        room.End(HostToken);

        Assert.Throws<InvalidHostTokenException>(() => room.End("wrong-token"));
    }

    private static Question CreateQuestion(int number = 1) =>
        new($"Question {number}?", ["A", "B", "C", "D"], correctIndex: 0);

    private static List<Question> CreateQuestions(int count) =>
        Enumerable.Range(1, count).Select(CreateQuestion).ToList();

    private static LiveRoom CreateRoom(
        RoomSettings? settings = null,
        int questionCount = 3) =>
        new(Code, Guid.NewGuid(), HostToken, settings ?? DefaultSettings, CreateQuestions(questionCount));

    private static LiveRoom CreateStartedRoom(RoomSettings? settings = null, int questionCount = 3)
    {
        var room = CreateRoom(settings, questionCount);
        room.Start(HostToken);
        return room;
    }
}