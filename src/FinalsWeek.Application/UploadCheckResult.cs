namespace FinalsWeek.Application;

using FinalsWeek.Core;

public class UploadCheckResult
{
    public UploadCheckResult(string? rejectionReason = null)
    {
        RejectionReason = rejectionReason;
    }

    public string? RejectionReason { get; }

    public bool Success => RejectionReason == null;
}
