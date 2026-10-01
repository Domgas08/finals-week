namespace FinalsWeek.Application;

using FinalsWeek.Core;

public class CsvImportResult
{
    public CsvImportResult(
        IReadOnlyList<Question> questions,
        IReadOnlyList<ImportRowFailure> failedRows,
        string? failureReason = null)
    {
        Questions = questions;
        FailedRows = failedRows;
        FailureReason = failureReason;
    }

    public IReadOnlyList<Question> Questions { get; }

    public IReadOnlyList<ImportRowFailure> FailedRows { get; }

    public string? FailureReason { get; }

    public bool Success => FailureReason is null;
}