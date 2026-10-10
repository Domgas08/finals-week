namespace FinalsWeek.Core.Tests;

using System;
using System.Collections.Generic;
using FinalsWeek.Core;
using Xunit;

public class StudySessionTests
{
    [Fact]
    public void Progress_WithNoQuestions_ReturnsZeroPercentage()
    {
        var session = new StudySession(CreateTestDeckId(), new List<Question>());

        var progress = session.Progress;

        Assert.Equal(0, progress.Answered);
        Assert.Equal(0, progress.Total);
        Assert.Equal(0.0, progress.Percentage);
    }

    [Fact]
    public void Progress_CalculatesPartialAndFullCompletionCorrectly()
    {
        var session = new StudySession(CreateTestDeckId(), CreateTestQuestions(4));

        Assert.Equal(0, session.Progress.Answered);
        Assert.Equal(0.0, session.Progress.Percentage);

        session.AnswerCurrentQuestion(0, AnswerOutcome.Correct, 10);

        Assert.Equal(1, session.Progress.Answered);
        Assert.Equal(25.0, session.Progress.Percentage);

        session.SkipCurrentQuestion();

        Assert.Equal(2, session.Progress.Answered);
        Assert.Equal(50.0, session.Progress.Percentage);
        Assert.Equal(AnswerOutcome.Skipped, session.Answers[1].Outcome);
        Assert.Null(session.Answers[1].SelectedOptionIndex);
        Assert.Equal(0, session.Answers[1].PointsAwarded);
    }

    [Fact]
    public void Constructor_ValidDeckIdAndQuestions_InitializesCorrectly()
    {
        var deckId = CreateTestDeckId();
        var questions = CreateTestQuestions(2);

        var session = new StudySession(deckId, questions);

        Assert.Equal(deckId, session.DeckId);
        Assert.Equal(questions.Count, session.Questions.Count);
        Assert.False(session.IsCompleted);
        Assert.Null(session.FinishedAt);
        Assert.True(session.StartedAt <= DateTimeOffset.UtcNow);
    }

    [Fact]
    public void Constructor_NullQuestions_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new StudySession(CreateTestDeckId(), null!));
    }

    [Fact]
    public void AnswerCurrentQuestion_AfterSessionCompleted_ThrowsInvalidOperationException()
    {
        var session = new StudySession(CreateTestDeckId(), CreateTestQuestions(1));

        session.AnswerCurrentQuestion(0, AnswerOutcome.Correct, 10);

        Assert.Throws<InvalidOperationException>(() =>
            session.AnswerCurrentQuestion(0, AnswerOutcome.Correct, 10));
    }

    [Fact]
    public void AnswerCurrentQuestion_InvalidOutcome_ThrowsArgumentOutOfRangeException()
    {
        var session = new StudySession(CreateTestDeckId(), CreateTestQuestions(1));

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            session.AnswerCurrentQuestion(0, (AnswerOutcome)99, 0));
    }

    [Fact]
    public void FullSession_WithMixedAnswers_RecordsDetailsAndTimestamps()
    {
        var session = new StudySession(CreateTestDeckId(), CreateTestQuestions(3));

        session.AnswerCurrentQuestion(0, AnswerOutcome.Correct, 5);
        session.AnswerCurrentQuestion(1, AnswerOutcome.Incorrect, 0);
        session.SkipCurrentQuestion();

        Assert.True(session.IsCompleted);
        Assert.NotNull(session.FinishedAt);
        Assert.True(session.FinishedAt >= session.StartedAt);
        Assert.Equal(3, session.Answers.Count);

        Assert.Equal(0, session.Answers[0].SelectedOptionIndex);
        Assert.Equal(AnswerOutcome.Correct, session.Answers[0].Outcome);
        Assert.Equal(5, session.Answers[0].PointsAwarded);

        Assert.Equal(1, session.Answers[1].SelectedOptionIndex);
        Assert.Equal(AnswerOutcome.Incorrect, session.Answers[1].Outcome);
        Assert.Equal(0, session.Answers[1].PointsAwarded);

        Assert.Null(session.Answers[2].SelectedOptionIndex);
        Assert.Equal(AnswerOutcome.Skipped, session.Answers[2].Outcome);
        Assert.Equal(0, session.Answers[2].PointsAwarded);
    }

    [Fact]
    public void SessionProgress_NegativeTotal_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new SessionProgress(0, -1));
    }

    [Fact]
    public void SessionProgress_NegativeAnswered_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new SessionProgress(-1, 5));
    }

    [Fact]
    public void SessionProgress_AnsweredGreaterThanTotal_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new SessionProgress(6, 5));
    }

    private Guid CreateTestDeckId() => Guid.NewGuid();

    private List<Question> CreateTestQuestions(int count)
    {
        var questions = new List<Question>();
        for (int i = 0; i < count; i++)
        {
            questions.Add(new Question($"Question {i + 1}", new[] { "A", "B" }, 0));
        }

        return questions;
    }
}