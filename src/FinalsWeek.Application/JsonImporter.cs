namespace FinalsWeek.Application;

using System.Text.Json;
using FinalsWeek.Core;

public class JsonImporter : IQuestionImporter
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public string Extension => ".json";

    public ImportResult Import(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        var questions = new List<Question>();
        var failedRows = new List<ImportRowFailure>();

        var index = 0;
        try
        {
            var transfers = JsonSerializer.Deserialize<List<QuestionTransfer?>>(stream, JsonOptions);
            foreach (var transfer in transfers ?? [])
            {
                ++index;

                if (transfer is null)
                {
                    failedRows.Add(
                        new ImportRowFailure(index, "Entry is null"));
                    continue;
                }

                if (transfer.Prompt is null)
                {
                    failedRows.Add(new ImportRowFailure(index, "Prompt is missing."));
                    continue;
                }

                if (transfer.Options is null)
                {
                    failedRows.Add(new ImportRowFailure(index, "Options are missing."));
                    continue;
                }

                if (transfer.CorrectIndex is null)
                {
                    failedRows.Add(new ImportRowFailure(index, "CorrectIndex is missing."));
                    continue;
                }

                try
                {
                    var question = new Question(transfer.Prompt, transfer.Options, transfer.CorrectIndex.Value, transfer.Topic, transfer.TimeLimitSeconds ?? 20);
                    questions.Add(question);
                }
                catch (ArgumentException ex)
                {
                    failedRows.Add(new ImportRowFailure(index, ex.Message));
                }
            }
        }
        catch (JsonException ex)
        {
            return new ImportResult(
                new List<Question>(),
                new List<ImportRowFailure>(),
                $"Invalid JSON: {ex.Message}");
        }

        return new ImportResult(questions, failedRows);
    }
}