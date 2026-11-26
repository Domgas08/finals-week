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

    private static async IAsyncEnumerable<NumberedRow<QuestionTransfer?>> ReadRowsAsync(IAsyncEnumerable<QuestionTransfer?> transfers)
    {
        var index = 0;
        await foreach (var transfer in transfers)
        {
            index++;
            yield return new NumberedRow<QuestionTransfer?>(index, transfer);
        }
    }

    private static Question ToQuestion(QuestionTransfer? transfer)
    {
        if (transfer is null)
        {
            throw new ArgumentException("Entry is null.");
        }

        if (transfer.Prompt is null)
        {
            throw new ArgumentException("Prompt is missing.");
        }

        if (transfer.Options is null)
        {
            throw new ArgumentException("Options are missing.");
        }

        if (transfer.CorrectIndex is null)
        {
            throw new ArgumentException("CorrectIndex is missing.");
        }

        return new Question(transfer.Prompt, transfer.Options, transfer.CorrectIndex.Value, transfer.Topic, transfer.TimeLimitSeconds ?? 20);
    }

    public async Task<ImportResult> ImportAsync(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        try
        {
            var transfer = JsonSerializer.DeserializeAsyncEnumerable<QuestionTransfer?>(stream, JsonOptions);
            return await ImportPipeline.RunAsync(ReadRowsAsync(transfer), ToQuestion);
        }
        catch (JsonException ex)
        {
            return new ImportResult(
                new List<Question>(),
                new List<ImportRowFailure>(),
                $"Invalid JSON: {ex.Message}");
        }
    }
}