namespace FinalsWeek.Application;

using FinalsWeek.Core;

public record RoomSnapshot(
    string Code,
    RoomState State,
    int CurrentQuestionIndex,
    int QuestionCount,
    Question? CurrentQuestion,
    IReadOnlyList<RoomPlayer> Players)
{
    public static RoomSnapshot From(LiveRoom room) => new(
        room.Code,
        room.State,
        room.CurrentQuestionIndex,
        room.Questions.Count,
        room.CurrentQuestion,
        room.Players.ToList());
}