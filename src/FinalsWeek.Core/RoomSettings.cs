namespace FinalsWeek.Core;

public record RoomSettings(
    int TimeLimitSeconds,
    bool Shuffle,
    bool AllowLateJoin,
    int MaxPlayers);