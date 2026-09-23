namespace FinalsWeek.Core;

public readonly struct SessionProgress
{
    public SessionProgress(int answered, int total)
    {
        if (total < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(total), "Total questions cannot be negative.");
        }

        if (answered < 0 || answered > total)
        {
            throw new ArgumentOutOfRangeException(nameof(answered), "Answered count must be between 0 and total questions.");
        }

        Answered = answered;
        Total = total;
    }

    public int Answered { get; }

    public int Total { get; }

    public double Percentage => Total == 0 ? 0.0 : Math.Round((double)Answered / Total * 100, 2);
}