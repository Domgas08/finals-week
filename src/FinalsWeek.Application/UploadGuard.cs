namespace FinalsWeek.Application;

using FinalsWeek.Core;

public class UploadGuard
{
    private readonly long maxSizeInBytes;

    public UploadGuard(long maxSizeInBytes)
    {
        this.maxSizeInBytes = maxSizeInBytes;
    }

    public UploadCheckResult Check(string fileName, long sizeInBytes)
    {
        if (sizeInBytes > maxSizeInBytes)
        {
            return new UploadCheckResult($"File size exceeds the maximum allowed limit of {maxSizeInBytes / (1024 * 1024)} MB.");
        }

        if (!fileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase) && !fileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
        {
            return new UploadCheckResult("Only JSON and CSV files are allowed.");
        }

        return new UploadCheckResult();
    }
}