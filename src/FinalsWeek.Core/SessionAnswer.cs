namespace FinalsWeek.Core;

public record SessionAnswer(int QuestionIndex, int? SelectedOptionIndex, AnswerOutcome Outcome, int PointsAwarded);