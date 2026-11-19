namespace FinalsWeek.Application;

using System.Net.NetworkInformation;
using FinalsWeek.Core;

public class CsvImporter : IQuestionImporter
{
    private const string ExpectedHeader =
        "Prompt,Option1,Option2,Option3,Option4,Option5,Option6,CorrectIndex,Topic,TimeLimitSeconds";

    public string Extension => ".csv";

    private static async IAsyncEnumerable<NumberedRow<string>> ReadRowsAsync(StreamReader reader)
    {
        string? line;
        int lineNumber = 1;

        while ((line = await reader.ReadLineAsync()) is not null)
        {
            lineNumber++;

            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            yield return new NumberedRow<string>(lineNumber, line);
        }
    }

    private static Question ToQuestion(string line)
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

        if (options.Count < 2)
        {
            throw new ArgumentException("A question must have at least 2 options.");
        }

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

        return question;
    }

    public async Task<ImportResult> ImportAsync(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        using var reader = new StreamReader(stream, leaveOpen: true);

        var header = await reader.ReadLineAsync();

        if (header is null)
        {
            return new ImportResult(
                [],
                [],
                "The CSV file is empty.");
        }

        if (!string.Equals(header.Trim(), ExpectedHeader, StringComparison.OrdinalIgnoreCase))
        {
            return new ImportResult(
                [],
                [],
                "The CSV header is invalid.");
        }

        return await ImportPipeline.RunAsync(ReadRowsAsync(reader), ToQuestion);
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