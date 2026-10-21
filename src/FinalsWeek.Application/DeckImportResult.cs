namespace FinalsWeek.Application;

public class DeckImportResult
{
    public DeckImportResult(
        IReadOnlyList<QuestionRowParsed> successfulRows,
        IReadOnlyList<ImportRowFailure> failedRows,
        string? failureReason = null)
    {
        SuccessfulRows = successfulRows ?? Array.Empty<QuestionRowParsed>();
        FailedRows = failedRows ?? Array.Empty<ImportRowFailure>();
        FailureReason = failureReason;
    }

    public IReadOnlyList<QuestionRowParsed> SuccessfulRows { get; }

    public IReadOnlyList<ImportRowFailure> FailedRows { get; }

    public string? FailureReason { get; }

    public int TotalCount => SuccessfulCount + FailedCount;

    public int SuccessfulCount => SuccessfulRows.Count;

    public int FailedCount => FailedRows.Count;

    public bool Success => FailureReason is null;
}