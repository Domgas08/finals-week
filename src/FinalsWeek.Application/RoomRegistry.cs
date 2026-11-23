namespace FinalsWeek.Application;

using System.Collections.Concurrent;
using FinalsWeek.Core;

public class RoomRegistry
{
    private const string CodeAlphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
    private const int CodeLength = 6;
    private readonly ConcurrentDictionary<string, RoomEntry> rooms = new();

    public event Action<string>? RoomChanged;

    public CreatedRoom CreateRoom(Deck deck, RoomSettings settings)
    {
        ArgumentNullException.ThrowIfNull(deck);
        ArgumentNullException.ThrowIfNull(settings);

        var questions = deck.Questions.ToArray();

        if (settings.Shuffle)
        {
            Random.Shared.Shuffle(questions);
        }

        var hostToken = Guid.NewGuid().ToString("N");

        while (true)
        {
            var code = NewCode();
            var room = new LiveRoom(code, deck.Id, hostToken, settings, questions.ToList());
            if (rooms.TryAdd(code, new RoomEntry(room))) { return new CreatedRoom(code, hostToken); }
        }
    }

    public RoomPlayer Join(string code, string displayName)
    {
        return Change(code, room => room.Join(displayName));
    }

    public void Start(string code, string hostToken)
    {
        Change(code, room => room.Start(hostToken));
    }

    public void End(string code, string hostToken)
    {
        Change(code, room => room.End(hostToken));
    }

    public void Next(string code, string hostToken)
    {
        Change(code, room => room.Next(hostToken));
    }

    public RoomSnapshot GetSnapshot(string code)
    {
        var entry = Find(code);

        lock (entry.Gate)
        {
            return RoomSnapshot.From(entry.Room);
        }
    }

    private static string NewCode()
    {
        return new string(Random.Shared.GetItems<char>(CodeAlphabet, CodeLength));
    }

    private static string Normalize(string code)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        return code.Trim().ToUpperInvariant();
    }

    private T Change<T>(string code, Func<LiveRoom, T> change)
    {
        var entry = Find(code);
        T result;

        lock (entry.Gate)
        {
            result = change(entry.Room);
        }

        RoomChanged?.Invoke(entry.Room.Code);
        return result;
    }

    private void Change(string code, Action<LiveRoom> change)
    {
        Change(code, room =>
        {
            change(room);
            return true;
        });
    }

    private RoomEntry Find(string code)
    {
        var key = Normalize(code);

        if (!rooms.TryGetValue(key, out var entry))
        {
            throw new RoomNotFoundException(key);
        }

        return entry;
    }

    private sealed class RoomEntry(LiveRoom room)
    {
        public LiveRoom Room { get; } = room;

        public Lock Gate { get; } = new();
    }
}