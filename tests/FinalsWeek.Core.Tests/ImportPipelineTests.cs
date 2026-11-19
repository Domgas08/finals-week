namespace FinalsWeek.Core.Tests;

using FinalsWeek.Application;
using FinalsWeek.Core;
using Xunit;

public class ImportPipelineTests
{
    [Fact]
    public async Task RunAsync_GoodAndBadRows_KeepsGoodAndRecordsBadWithItsNumber()
    {
        var rows = RowsAsync(
            new NumberedRow<string>(2, "First"),
            new NumberedRow<string>(4, "bad"),
            new NumberedRow<string>(5, "Third"));

        var result = await ImportPipeline.RunAsync(rows, ToQuestion);

        Assert.True(result.Success);
        Assert.Equal(new[] { "First", "Third" }, result.Questions.Select(q => q.Prompt));
        Assert.Single(result.FailedRows);
        Assert.Equal(4, result.FailedRows[0].LineNumber);
        Assert.Equal("Bad row.", result.FailedRows[0].Reason);
    }

    [Fact]
    public async Task RunAsync_FormatException_IsRecordedAsFailedRow()
    {
        var rows = RowsAsync(new NumberedRow<string>(1, "format"));

        var result = await ImportPipeline.RunAsync(rows, ToQuestion);

        Assert.Empty(result.Questions);
        Assert.Single(result.FailedRows);
        Assert.Equal(1, result.FailedRows[0].LineNumber);
    }

    [Fact]
    public async Task RunAsync_OtherException_IsNotHidden()
    {
        var rows = RowsAsync(new NumberedRow<string>(1, "crash"));

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => ImportPipeline.RunAsync(rows, ToQuestion));
    }

    [Fact]
    public async Task RunAsync_NoRows_ReturnsEmptySuccess()
    {
        var result = await ImportPipeline.RunAsync(RowsAsync(), ToQuestion);

        Assert.True(result.Success);
        Assert.Empty(result.Questions);
        Assert.Empty(result.FailedRows);
    }

    private static Question ToQuestion(string text)
    {
        return text switch
        {
            "bad" => throw new ArgumentException("Bad row."),
            "format" => throw new FormatException("Bad format."),
            "crash" => throw new InvalidOperationException("A bug, not a bad row."),
            _ => new Question(text, ["A", "B"], 0),
        };
    }

    private static async IAsyncEnumerable<NumberedRow<string>> RowsAsync(params NumberedRow<string>[] rows)
    {
        foreach (var row in rows)
        {
            await Task.Yield();
            yield return row;
        }
    }
}