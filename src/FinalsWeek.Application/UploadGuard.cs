namespace FinalsWeek.Application;

using FinalsWeek.Core;

public class UploadGuard
{
    private readonly long MaxSizeInBytes;

    public UploadGuard(long maxSizeInBytes)
    {
        MaxSizeInBytes = maxSizeInBytes;
    }

    public UploadCheckResult Check(string fileName, long sizeInBytes)
    {
        if (sizeInBytes > MaxSizeInBytes)
        {
            return new UploadCheckResult($"File size exceeds the maximum allowed limit of {MaxSizeInBytes / (1024 * 1024)} MB.");
        }

        if (!fileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase) && !fileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
        {
            return new UploadCheckResult("Only JSON and CSV files are allowed.");
        }

        return new UploadCheckResult();
    }
}