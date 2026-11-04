namespace FinalsWeek.Core;

public record QuestionResult
{
    public QuestionResult(string prompt, string correctAnswer, AnswerOutcome outcome, int points, string? topic, string? studentAnswer)
    {
        Prompt = prompt;
        StudentAnswer = studentAnswer;
        CorrectAnswer = correctAnswer;
        Topic = topic;
        Points = points;
        Outcome = outcome;
    }

    public string Prompt { get; private init; }

    public string CorrectAnswer { get; private init; }

    public string? StudentAnswer { get; private init; }

    public string? Topic { get; private init; }

    public int Points { get; private init; }

    public AnswerOutcome Outcome { get; private init; }
}