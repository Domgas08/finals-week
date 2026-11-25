namespace FinalsWeek.Application;

using FinalsWeek.Core;

public interface IQuestionImporter
{
    string Extension { get; }

    Task<ImportResult> ImportAsync(Stream stream);
}