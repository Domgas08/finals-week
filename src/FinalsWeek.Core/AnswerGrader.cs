namespace FinalsWeek.Core;

public static class AnswerGrader
{
// TODO: when more structure is analyzed - implement Time and Credit into grader
    private const int BasePoints = 100;

    public static AnswerGrading Grade(Question question, int selectedIndex, TimeSpan? responseTime = null, bool allowPartialCredit = false)
    {
        if (question == null)
        {
            throw new ArgumentNullException(nameof(question));
        }

        bool isValidSelection = selectedIndex >= 0 && selectedIndex < question.Options.Count;
        bool isCorrect = isValidSelection && selectedIndex == question.CorrectIndex;
        int pointsAwarded = isCorrect ? BasePoints : 0;

        return new AnswerGrading(isCorrect, isValidSelection, pointsAwarded, question.CorrectIndex, question.Options[question.CorrectIndex]);
    }
}