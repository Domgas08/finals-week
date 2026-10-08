namespace FinalsWeek.Application;

using FinalsWeek.Core;

public interface IQuestionImporter
{
    string Extention { get; }

    ImportResult Import(Stream stream);
}