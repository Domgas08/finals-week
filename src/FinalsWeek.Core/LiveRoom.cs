namespace FinalsWeek.Core;

using FinalsWeek.Core.Exceptions;

public class LiveRoom
{
    private readonly string hostToken;
    private readonly List<Question> questions;
    private readonly List<RoomPlayer> players = [];

    public LiveRoom(
        string code,
        Guid deckId,
        string hostToken,
        RoomSettings settings,
        List<Question> questionList)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(hostToken);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(questionList);

        ValidateLiveRoom(deckId, settings.MaxPlayers, questionList);

        Code = code;
        DeckId = deckId;
        this.hostToken = hostToken;
        Settings = settings;
        questions = questionList.ToList();

        State = RoomState.Lobby;
        CurrentQuestionIndex = -1;
    }

    public string Code { get; }

    public Guid DeckId { get; }

    public RoomSettings Settings { get; }

    public RoomState State { get; private set; }

    public int CurrentQuestionIndex { get; private set; }

    public IReadOnlyList<Question> Questions => questions.AsReadOnly();

    public IReadOnlyList<RoomPlayer> Players => players.AsReadOnly();

    public Question? CurrentQuestion =>
        State == RoomState.InProgress && CurrentQuestionIndex >= 0 && CurrentQuestionIndex < questions.Count
            ? questions[CurrentQuestionIndex]
            : null;

    public RoomPlayer Join(string displayName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);

        ValidateJoinConditions();

        string uniqueName = MakeDisplayNameUnique(displayName.Trim());
        var player = new RoomPlayer(Guid.NewGuid(), uniqueName);
        players.Add(player);

        return player;
    }

    public void Start(string requestedHostToken)
    {
        ValidateHostToken(requestedHostToken);

        if (State != RoomState.Lobby)
        {
            throw new InvalidRoomStateException(Code, State, "Start()");
        }

        State = RoomState.InProgress;
        CurrentQuestionIndex = 0;
    }

    public void Next(string requestedHostToken)
    {
        ValidateHostToken(requestedHostToken);

        if (State != RoomState.InProgress)
        {
            throw new InvalidRoomStateException(Code, State, "Next()");
        }

        if (CurrentQuestionIndex + 1 >= questions.Count)
        {
            State = RoomState.Finished;
            return;
        }

        CurrentQuestionIndex++;
    }

    public void End(string requestedHostToken)
    {
        ValidateHostToken(requestedHostToken);

        if (State == RoomState.Finished)
        {
            throw new InvalidRoomStateException(Code, State, "End()");
        }

        State = RoomState.Finished;
    }

    private void ValidateLiveRoom(Guid deckId, int maxPlayers, List<Question> questionList)
    {
        if (deckId == Guid.Empty)
        {
            throw new ArgumentException("Deck ID cannot be empty.", nameof(deckId));
        }

        if (maxPlayers < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(maxPlayers), "MaxPlayers must be at least 1.");
        }

        if (questionList.Count == 0)
        {
            throw new ArgumentException("A live room must have at least one question.", nameof(questionList));
        }
    }

    private void ValidateJoinConditions()
    {
        if (State == RoomState.Finished)
        {
            throw new RoomAlreadyFinishedException(Code);
        }

        if (State == RoomState.InProgress && !Settings.AllowLateJoin)
        {
            throw new RoomAlreadyStartedException(Code);
        }

        if (players.Count >= Settings.MaxPlayers)
        {
            throw new RoomFullException(Code, Settings.MaxPlayers);
        }
    }

    private void ValidateHostToken(string requestedHostToken)
    {
        if (string.IsNullOrWhiteSpace(requestedHostToken) ||
            !string.Equals(hostToken, requestedHostToken, StringComparison.Ordinal))
        {
            throw new InvalidHostTokenException(Code);
        }
    }

    private string MakeDisplayNameUnique(string baseName)
    {
        if (!players.Any(p => string.Equals(p.DisplayName, baseName, StringComparison.OrdinalIgnoreCase)))
        {
            return baseName;
        }

        int i = 2;
        while (true)
        {
            string numerized = $"{baseName} ({i})";
            if (!players.Any(p => string.Equals(p.DisplayName, numerized, StringComparison.OrdinalIgnoreCase)))
            {
                return numerized;
            }

            i++;
        }
    }
}