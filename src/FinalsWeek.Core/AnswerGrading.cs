namespace FinalsWeek.Core;

public record AnswerGrading
{
    public AnswerGrading(bool isCorrect, bool isValidSelection, int pointsAwarded, int correctIndex, string correctOption)
    {
        IsCorrect = isCorrect;
        IsValidSelection = isValidSelection;
        PointsAwarded = pointsAwarded;
        CorrectIndex = correctIndex;
        CorrectOption = correctOption;
    }

    public bool IsCorrect { get; private init; }

    public bool IsValidSelection { get; private init; }

    public int PointsAwarded { get; private init; }

    public int CorrectIndex { get; private init; }

    public string CorrectOption { get; private init; }
}