namespace FinalsWeek.Core.Tests;

using System.Text;
using FinalsWeek.Application;
using Xunit;

public class JsonImporterTests
{
    [Fact]
    public void Import_ValidEntry_ReturnsQuestion()
    {
        const string json = """
        [
          {
            "prompt": "What is 2+2?",
            "options": ["3", "4", "5"],
            "correctIndex": 1,
            "topic": "Math",
            "timeLimitSeconds": 20
          }
        ]
        """;

        using var stream = CreateStream(json);

        var importer = new JsonImporter();

        var result = importer.Import(stream);

        Assert.True(result.Success);
        Assert.Single(result.Questions);
        Assert.Empty(result.FailedRows);

        Assert.Equal("What is 2+2?", result.Questions[0].Prompt);
        Assert.Equal("Math", result.Questions[0].Topic);
    }

    [Fact]
    public void Import_BadEntry_ReportsFailure()
    {
        const string json = """
        [
          { "prompt": "Bad question", "options": ["Only one"], "correctIndex": 0 }
        ]
        """;

        using var stream = CreateStream(json);

        var importer = new JsonImporter();

        var result = importer.Import(stream);

        Assert.True(result.Success);
        Assert.Empty(result.Questions);
        Assert.Single(result.FailedRows);
        Assert.Equal(1, result.FailedRows[0].LineNumber);
    }

    [Fact]
    public void Import_MixedEntries_ContinuesAfterBadEntry()
    {
        const string json = """
        [
          { "prompt": "First question", "options": ["A", "B"], "correctIndex": 0 },
          { "prompt": "Broken question", "options": ["Only one"], "correctIndex": 0 },
          { "prompt": "Third question", "options": ["Yes", "No"], "correctIndex": 1 }
        ]
        """;

        using var stream = CreateStream(json);

        var importer = new JsonImporter();

        var result = importer.Import(stream);

        Assert.True(result.Success);
        Assert.Equal(2, result.Questions.Count);
        Assert.Single(result.FailedRows);
        Assert.Equal(2, result.FailedRows[0].LineNumber);
    }

    [Fact]
    public void Import_MalformedDocument_ReturnsFailure()
    {
        const string json = """
        [ { "prompt": "Never closed"
        """;

        using var stream = CreateStream(json);

        var importer = new JsonImporter();

        var result = importer.Import(stream);

        Assert.False(result.Success);
        Assert.Contains("Invalid JSON", result.FailureReason);
    }

    [Fact]
    public void Import_RootIsNotArray_ReturnsFailure()
    {
        using var stream = CreateStream("{}");

        var importer = new JsonImporter();

        var result = importer.Import(stream);

        Assert.False(result.Success);
        Assert.Contains("Invalid JSON", result.FailureReason);
    }

    private static MemoryStream CreateStream(string text)
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(text));
    }
}