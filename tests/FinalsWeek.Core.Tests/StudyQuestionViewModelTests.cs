namespace FinalsWeek.Core.Tests;

using FinalsWeek.Application;
using FinalsWeek.Core;
using Xunit;

public class StudyQuestionViewModelTests
{
    [Fact]
    public void PreSubmitViewModelDoesNotContainCorrectIndex()
    {
        var question = new Question(
            "What is 2 + 2?",
            new List<string> { "3", "4", "5" },
            1);

        var viewModel = question.ToPreSubmitViewModel(1);

        Assert.Equal("What is 2 + 2?", viewModel.Prompt);
        Assert.Equal(3, viewModel.Options.Count);
        Assert.Equal(1, viewModel.QuestionNumber);

        var correctIndexProperty =
            viewModel.GetType().GetProperty("CorrectIndex");

        Assert.Null(correctIndexProperty);
    }

    [Fact]
    public void PostSubmitViewModelContainsCorrectIndex()
    {
        var question = new Question(
            "What is 2 + 2?",
            new List<string> { "3", "4", "5" },
            1);

        var viewModel = question.ToPostSubmitViewModel(1);

        Assert.Equal(1, viewModel.CorrectIndex);
    }
}