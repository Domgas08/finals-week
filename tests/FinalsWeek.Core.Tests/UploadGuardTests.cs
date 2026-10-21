namespace FinalsWeek.Core.Tests;

using FinalsWeek.Application;
using Xunit;

public class UploadGuardTests
{
    [Fact]
    public void Check_FileOverLimit_IsRejected()
    {
        var guard = new UploadGuard(100);
        var result = guard.Check("test.csv", 200);

        Assert.False(result.Success);
        Assert.Contains("File size exceeds the maximum allowed limit", result.RejectionReason);
    }

    [Fact]
    public void Check_FileUnderLimit_IsAccepted()
    {
        var guard = new UploadGuard(100);
        var result = guard.Check("test.csv", 100);

        Assert.True(result.Success);
    }

    [Fact]
    public void Check_DifferentFileType_IsRejected()
    {
        var guard = new UploadGuard(100);
        var result = guard.Check("test.pdf", 95);

        Assert.False(result.Success);
        Assert.Contains("Only JSON and CSV files are", result.RejectionReason);
    }

    [Fact]
    public void Check_CorrectFileTypeAndUppercase_IsAccepted()
    {
        var guard = new UploadGuard(100);
        var result = guard.Check("FILE.JSON", 95);

        Assert.True(result.Success);
    }
}