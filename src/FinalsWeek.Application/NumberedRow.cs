namespace FinalsWeek.Application;

public record NumberedRow<TRow>(int RowNumber, TRow Row);
