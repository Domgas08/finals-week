namespace FinalsWeek.Application;

using FinalsWeek.Core;

public class CsvImporter : IQuestionImporter
{
    private const string ExpectedHeader =
        "Prompt,Option1,Option2,Option3,Option4,Option5,Option6,CorrectIndex,Topic,TimeLimitSeconds";

    public string Extention => ".csv";

    public ImportResult Import(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        var questions = new List<Question>();
        var failedRows = new List<ImportRowFailure>();

        using var reader = new StreamReader(stream, leaveOpen: true);

        var header = reader.ReadLine();

        if (header is null)
        {
            return new ImportResult(
                questions,
                failedRows,
                "The CSV file is empty.");
        }

        if (!string.Equals(header.Trim(), ExpectedHeader, StringComparison.OrdinalIgnoreCase))
        {
            return new ImportResult(
                questions,
                failedRows,
                "The CSV header is invalid.");
        }

        var lineNumber = 1;
        string? line;

        while ((line = reader.ReadLine()) is not null)
        {
            lineNumber++;

            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            try
            {
                var columns = ParseCsvLine(line);

                if (columns.Count != 10)
                {
                    throw new ArgumentException(
                        $"Expected 10 columns but found {columns.Count}.");
                }

                var prompt = columns[0];

                var options = columns
                    .Skip(1)
                    .Take(6)
                    .Where(option => !string.IsNullOrWhiteSpace(option))
                    .ToList();

                if (!int.TryParse(columns[7], out var correctIndex))
                {
                    throw new ArgumentException("CorrectIndex must be a number.");
                }

                var topic = string.IsNullOrWhiteSpace(columns[8])
                    ? null
                    : columns[8].Trim();

                var timeLimitSeconds = 20;

                if (!string.IsNullOrWhiteSpace(columns[9])
                    && !int.TryParse(columns[9], out timeLimitSeconds))
                {
                    throw new ArgumentException("TimeLimitSeconds must be a number.");
                }

                var question = new Question(
                    prompt,
                    options,
                    correctIndex,
                    topic,
                    timeLimitSeconds);

                questions.Add(question);
            }
            catch (Exception ex) when (
                ex is ArgumentException
                || ex is FormatException)
            {
                failedRows.Add(
                    new ImportRowFailure(lineNumber, ex.Message));
            }
        }

        return new ImportResult(questions, failedRows);
    }

    private static List<string> ParseCsvLine(string line)
    {
        var values = new List<string>();
        var current = new System.Text.StringBuilder();
        var insideQuotes = false;

        for (var i = 0; i < line.Length; i++)
        {
            var character = line[i];

            if (character == '"')
            {
                if (insideQuotes
                    && i + 1 < line.Length
                    && line[i + 1] == '"')
                {
                    current.Append('"');
                    i++;
                }
                else
                {
                    insideQuotes = !insideQuotes;
                }
            }
            else if (character == ',' && !insideQuotes)
            {
                values.Add(current.ToString());
                current.Clear();
            }
            else
            {
                current.Append(character);
            }
        }

        if (insideQuotes)
        {
            throw new FormatException("CSV row contains an unclosed quote.");
        }

        values.Add(current.ToString());

        return values;
    }
}