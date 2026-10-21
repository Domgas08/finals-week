namespace FinalsWeek.Core.Tests;

using System.Text;
using FinalsWeek.Application;
using Xunit;

public class CsvImporterTests
{
    [Fact]
    public void Import_ValidRow_ReturnsQuestion()
    {
        const string csv =
            "Prompt,Option1,Option2,Option3,Option4,Option5,Option6,CorrectIndex,Topic,TimeLimitSeconds\n" +
            "What is 2+2?,3,4,5,,,,1,Math,20";

        using var stream = CreateStream(csv);

        var importer = new CsvImporter();

        var result = importer.Import(stream);

        Assert.True(result.Success);
        Assert.Single(result.Questions);
        Assert.Empty(result.FailedRows);

        Assert.Equal("What is 2+2?", result.Questions[0].Prompt);
        Assert.Equal("Math", result.Questions[0].Topic);
    }

    [Fact]
    public void Import_BadRow_ReportsFailure()
    {
        const string csv =
            "Prompt,Option1,Option2,Option3,Option4,Option5,Option6,CorrectIndex,Topic,TimeLimitSeconds\n" +
            "Bad question,Only one option,,,,,,0,Test,20";

        using var stream = CreateStream(csv);

        var importer = new CsvImporter();

        var result = importer.Import(stream);

        Assert.True(result.Success);
        Assert.Empty(result.Questions);
        Assert.Single(result.FailedRows);
        Assert.Equal(2, result.FailedRows[0].LineNumber);
    }

    [Fact]
    public void Import_MixedRows_ContinuesAfterBadRow()
    {
        const string csv =
            "Prompt,Option1,Option2,Option3,Option4,Option5,Option6,CorrectIndex,Topic,TimeLimitSeconds\n" +
            "First question,A,B,,,,,0,Topic,20\n" +
            "Broken question,Only one,,,,,,0,Topic,20\n" +
            "Third question,Yes,No,,,,,1,Topic,30";

        using var stream = CreateStream(csv);

        var importer = new CsvImporter();

        var result = importer.Import(stream);

        Assert.True(result.Success);
        Assert.Equal(2, result.Questions.Count);
        Assert.Single(result.FailedRows);
        Assert.Equal(3, result.FailedRows[0].LineNumber);
    }

    [Fact]
    public void Import_EmptyFile_ReturnsFailure()
    {
        using var stream = CreateStream(string.Empty);

        var importer = new CsvImporter();

        var result = importer.Import(stream);

        Assert.False(result.Success);
        Assert.Equal("The CSV file is empty.", result.FailureReason);
    }

    [Fact]
    public void Import_InvalidHeader_ReturnsFailure()
    {
        const string csv =
            "Wrong,Header\n" +
            "Question,A,B";

        using var stream = CreateStream(csv);

        var importer = new CsvImporter();

        var result = importer.Import(stream);

        Assert.False(result.Success);
        Assert.Equal("The CSV header is invalid.", result.FailureReason);
    }

    private static MemoryStream CreateStream(string text)
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(text));
    }
}