namespace FinalsWeek.Application;

public class StudyQuestionViewModel
{
    public StudyQuestionViewModel(
        string prompt,
        IReadOnlyList<string> options,
        int questionNumber)
    {
        Prompt = prompt;
        Options = options;
        QuestionNumber = questionNumber;
    }

    public string Prompt { get; }

    public IReadOnlyList<string> Options { get; }

    public int QuestionNumber { get; }
}