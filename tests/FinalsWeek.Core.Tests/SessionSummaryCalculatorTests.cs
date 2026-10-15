namespace FinalsWeek.Core.Tests;

using FinalsWeek.Core;
using Xunit;

public class SessionSummaryCalculatorTests
{
    [Fact]
    public void Summarise_AllCorrect_ScoresFullMarks()
    {
        var session = CreateSession(CreateQuestion("Q1"), CreateQuestion("Q2"));
        session.AnswerCurrentQuestion(0, AnswerOutcome.Correct, 100);
        session.AnswerCurrentQuestion(0, AnswerOutcome.Correct, 100);

        var summary = SessionSummaryCalculator.Summarise(session);

        Assert.Equal(200, summary.TotalScore);
        Assert.Equal(2, summary.CorrectCount);
        Assert.Equal(100.0, summary.PercentageCorrect);
    }

    [Fact]
    public void Summarise_MixedSession_CountsOnlyCorrectAnswers()
    {
        var session = CreateSession(CreateQuestion("Q1"), CreateQuestion("Q2"), CreateQuestion("Q3"));
        session.AnswerCurrentQuestion(0, AnswerOutcome.Correct, 100);
        session.AnswerCurrentQuestion(1, AnswerOutcome.Incorrect, 0);
        session.SkipCurrentQuestion();

        var summary = SessionSummaryCalculator.Summarise(session);

        Assert.Equal(100, summary.TotalScore);
        Assert.Equal(1, summary.CorrectCount);
        Assert.Equal(3, summary.TotalQuestions);
    }

    [Fact]
    public void Summarise_SkippedQuestion_HasNoStudentAnswerAndNoPoints()
    {
        var session = CreateSession(CreateQuestion("Q1"));
        session.SkipCurrentQuestion();

        var summary = SessionSummaryCalculator.Summarise(session);
        var result = summary.Topics.SelectMany(t => t.Results).Single();

        Assert.Null(result.StudentAnswer);
        Assert.Equal(0, result.Points);
        Assert.Equal(AnswerOutcome.Skipped, result.Outcome);
    }

    [Fact]
    public void Summarise_GroupsQuestionsByTopic()
    {
        var session = CreateSession(
            CreateQuestion("Q1", "Math"),
            CreateQuestion("Q2", "Math"),
            CreateQuestion("Q3", "History"));

        var summary = SessionSummaryCalculator.Summarise(session);

        Assert.Equal(2, summary.Topics.Count);
        Assert.Equal(2, summary.Topics.Single(t => t.Topic == "Math").TotalCount);
        Assert.Equal(1, summary.Topics.Single(t => t.Topic == "History").TotalCount);
    }

    [Fact]
    public void Summarise_UntaggedQuestions_AreGroupedLast()
    {
        var session = CreateSession(
            CreateQuestion("Q1"),
            CreateQuestion("Q2", "Math"));

        var summary = SessionSummaryCalculator.Summarise(session);

        Assert.Equal("Math", summary.Topics[0].Topic);
        Assert.Equal("Untagged", summary.Topics[1].Topic);
    }

    [Fact]
    public void Summarise_NullSession_ThrowsArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => SessionSummaryCalculator.Summarise(null!));

        Assert.Equal("session", ex.ParamName);
    }

    private static Question CreateQuestion(string prompt, string? topic = null)
    {
        return new Question(prompt, new List<string> { "A", "B" }, 0, topic);
    }

    private static StudySession CreateSession(params Question[] questions)
    {
        return new StudySession(Guid.NewGuid(), questions);
    }
}