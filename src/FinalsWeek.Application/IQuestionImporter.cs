namespace FinalsWeek.Application;

using FinalsWeek.Core;

public interface IQuestionImporter
{
    string Extension { get; }

    ImportResult Import(Stream stream);
}