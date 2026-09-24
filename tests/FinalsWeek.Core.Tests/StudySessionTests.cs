namespace FinalsWeek.Core.Tests;

using System;
using System.Collections.Generic;
using FinalsWeek.Core;
using Xunit;

public class StudySessionTests
{
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

    [Fact]
    private void Progress_WithNoQuestions_ReturnsZeroPercentage()
    {
        var session = new StudySession(CreateTestDeckId(), new List<Question>());

        var progress = session.Progress;

        Assert.Equal(0, progress.Answered);
        Assert.Equal(0, progress.Total);
        Assert.Equal(0.0, progress.Percentage);
    }

    [Fact]
    private void Progress_CalculatesPartialAndFullCompletionCorrectly()
    {
        var session = new StudySession(CreateTestDeckId(), CreateTestQuestions(4));

        Assert.Equal(0, session.Progress.Answered);
        Assert.Equal(0.0, session.Progress.Percentage);

        session.AnswerCurrentQuestion(AnswerOutcome.Correct);

        Assert.Equal(1, session.Progress.Answered);
        Assert.Equal(25.0, session.Progress.Percentage);

        session.SkipCurrentQuestion();

        Assert.Equal(2, session.Progress.Answered);
        Assert.Equal(50.0, session.Progress.Percentage);
        Assert.Equal(AnswerOutcome.Skipped, session.Answers[1]);
    }

    [Fact]
    private void Constructor_ValidDeckIdAndQuestions_InitializesCorrectly()
    {
        var deckId = CreateTestDeckId();
        var questions = CreateTestQuestions(2);

        var session = new StudySession(deckId, questions);

        Assert.Equal(deckId, session.DeckId);
        Assert.Equal(questions.Count, session.Questions.Count);
    }

    [Fact]
    private void Constructor_NullQuestions_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new StudySession(CreateTestDeckId(), null!));
    }

    [Fact]
    private void AnswerCurrentQuestion_AfterSessionCompleted_ThrowsInvalidOperationException()
    {
        var session = new StudySession(CreateTestDeckId(), CreateTestQuestions(1));

        session.AnswerCurrentQuestion(AnswerOutcome.Correct);

        Assert.Throws<InvalidOperationException>(() =>
            session.AnswerCurrentQuestion(AnswerOutcome.Correct));
    }

    [Fact]
    private void AnswerCurrentQuestion_InvalidOutcome_ThrowsArgumentOutOfRangeException()
    {
        var session = new StudySession(CreateTestDeckId(), CreateTestQuestions(1));

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            session.AnswerCurrentQuestion((AnswerOutcome)99));
    }

    [Fact]
    private void SessionProgress_NegativeTotal_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new SessionProgress(0, -1));
    }

    [Fact]
    private void SessionProgress_NegativeAnswered_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new SessionProgress(-1, 5));
    }

    [Fact]
    private void SessionProgress_AnsweredGreaterThanTotal_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new SessionProgress(6, 5));
    }
}
