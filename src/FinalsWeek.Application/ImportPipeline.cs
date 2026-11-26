namespace FinalsWeek.Application;

using FinalsWeek.Core;

public static class ImportPipeline
{
    public static async Task<ImportResult> RunAsync<TRow> (IAsyncEnumerable<NumberedRow<TRow>> rows, Func<TRow, Question> toQuestion)
    {
        ArgumentNullException.ThrowIfNull(rows);
        ArgumentNullException.ThrowIfNull(toQuestion);

        var questions = new List<Question>();
        var failedRows = new List<ImportRowFailure>();

        await foreach (var row in rows)
        {
            try
            {
                var question = toQuestion(row.Row);
                questions.Add(question);

            }
            catch (Exception ex) when (ex is ArgumentException || ex is FormatException)
            {
                failedRows.Add(new ImportRowFailure(row.RowNumber, ex.Message));
            }
        }

        return new ImportResult (questions, failedRows);
    }
}