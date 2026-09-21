namespace FinalsWeek.Core.Tests;

using FinalsWeek.Core;
using Xunit;

public class AnswerGradingTests
{
    [Fact]
    public void NullQuestion_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => AnswerGrader.Grade(null!, 1));
    }

    [Fact]
    public void QuestionWithCorrectAnswer_ReturnsCorrectGrading()
    {
        var question = new Question("What is 2 + 2?", new List<string> { "3", "4", "5" }, 1);

        var result = AnswerGrader.Grade(question, 1);

        Assert.Equal(100, result.PointsAwarded);

        Assert.True(result.IsCorrect);

        Assert.True(result.IsValidSelection);

        Assert.Equal(question.Options[1], result.CorrectOption);
    }

    [Fact]
    public void QuestionWithIncorrectAnswer_ReturnsCorrectGrading()
    {
        var question = new Question("What is 2 + 2?", new List<string> { "3", "4", "5" }, 1);
        var result = AnswerGrader.Grade(question, 0);
        Assert.Equal(0, result.PointsAwarded);
        Assert.False(result.IsCorrect);
        Assert.True(result.IsValidSelection);
        Assert.Equal(question.Options[1], result.CorrectOption);
    }

    [Fact]
    public void QuestionWithAnOptionThatDoesNotExist_ReturnsInvalidSelection()
    {
        var question = new Question("What is 2 + 2?", new List<string> { "3", "4", "5" }, 1);
        var result = AnswerGrader.Grade(question, 5);
        Assert.Equal(0, result.PointsAwarded);
        Assert.False(result.IsCorrect);
        Assert.False(result.IsValidSelection);
        Assert.Equal(question.Options[1], result.CorrectOption);
    }

    [Fact]
    public void SameInputProducesSameResult()
    {
        var question = new Question("What is 5*5?", new List<string> { "10", "20", "25" }, 2);
        var result1 = AnswerGrader.Grade(question, 2);
        var result2 = AnswerGrader.Grade(question, 2);
        Assert.Equal(result1.PointsAwarded, result2.PointsAwarded);
        Assert.Equal(result1.IsCorrect, result2.IsCorrect);
        Assert.Equal(result1.IsValidSelection, result2.IsValidSelection);
        Assert.Equal(result1.CorrectOption, result2.CorrectOption);
    }
}