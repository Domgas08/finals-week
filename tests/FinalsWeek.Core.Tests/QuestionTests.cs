namespace FinalsWeek.Core.Tests;

using FinalsWeek.Core;

using Xunit;

public class QuestionTests
{
    [Fact]
    public void Constructor_WithValidOptionsAndOneCorrect_CreatesQuestionSuccessfully()
    {
        string prompt = "What is the capital of Lithuania?";
        var options = new List<string> { "Vilnius", "Kaunas", "Klaipeda" };
        int correctIndex = 0;

        var question = new Question(prompt, options, correctIndex);

        Assert.NotNull(question);
        Assert.Equal(prompt, question.Prompt);
        Assert.Equal(options, question.Options);
        Assert.Equal(correctIndex, question.CorrectIndex);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(3)]
    [InlineData(5)]
    public void Constructor_WithInvalidCorrectIndex_ThrowsArgumentOutOfRangeException(int invalidIndex)
    {
        string prompt = "What is 2 + 2?";
        var options = new List<string> { "3", "4", "5" };

        Assert.Throws<ArgumentOutOfRangeException>(() => new Question(prompt, options, invalidIndex));
    }

    [Fact]
    public void Constructor_WithOptionsCountLessThanTwo_ThrowsArgumentException()
    {
        string prompt = "Select true:";
        var options = new List<string> { "Only Option" };

        Assert.Throws<ArgumentException>(() => new Question(prompt, options, 0));
    }

    [Fact]
    public void Constructor_WithOptionsCountGreaterThanSix_ThrowsArgumentException()
    {
        string prompt = "Pick one number:";
        var options = new List<string> { "1", "2", "3", "4", "5", "6", "7" };

        Assert.Throws<ArgumentException>(() => new Question(prompt, options, 0));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(4)]
    public void Constructor_WithInvalidTimeLimit_ThrowsArgumentOutOfRangeException(int invalidTimeLimit)
    {
        string prompt = "What is 2 + 2?";
        var options = new List<string> { "3", "4", "5" };

        Assert.Throws<ArgumentOutOfRangeException>(() => new Question(prompt, options, 1, timeLimitSeconds: invalidTimeLimit));
    }

    [Fact]
    public void Question_IsImmutable_ModificationCreatesNewInstanceAndOriginalOptionsCannotBeMutated()
    {
        var options = new List<string> { "Vilnius", "Kaunas", "Klaipeda" };
        var question = new Question("Capital of Lithuania?", options, 0, "Geography", 10);

        options.Add("Siauliai");

        Assert.Equal(3, question.Options.Count);
        Assert.DoesNotContain("Siauliai", question.Options);
    }
}