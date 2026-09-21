namespace FinalsWeek.Core;

public static class AnswerGrader
{
    public static AnswerGrading Grade(Question question, int selectedIndex, int? responseTimeSeconds = null, bool allowPartialCredit = false)
    {
        if (question == null)
        {
            throw new ArgumentNullException(nameof(question));
        }

        bool isValidSelection = selectedIndex >= 0 && selectedIndex < question.Options.Count;
        bool isCorrect = isValidSelection && selectedIndex == question.CorrectIndex;
        int pointsAwarded = isCorrect ? 100 : 0;

        return new AnswerGrading(isCorrect, isValidSelection, pointsAwarded, question.CorrectIndex, question.Options[question.CorrectIndex]);
    }
}