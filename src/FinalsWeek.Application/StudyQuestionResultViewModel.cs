namespace FinalsWeek.Application;

public class StudyQuestionResultViewModel
{
    public StudyQuestionResultViewModel(
        string prompt,
        IReadOnlyList<string> options,
        int questionNumber,
        int correctIndex)
    {
        Prompt = prompt;
        Options = options;
        QuestionNumber = questionNumber;
        CorrectIndex = correctIndex;
    }

    public string Prompt { get; }

    public IReadOnlyList<string> Options { get; }

    public int QuestionNumber { get; }

    public int CorrectIndex { get; }
}