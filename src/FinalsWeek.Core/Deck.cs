namespace FinalsWeek.Core;

public enum Visibility
{
    Public,
    Private,
}

public class Deck
{
    private readonly List<Question> questions = new();

    public Deck(string title, string? courseCode = null, string? description = null, Visibility visibility = Visibility.Private)
    {
        this.Title = ValidateTitle(title);
        this.CourseCode = courseCode;
        this.Description = description;
        this.Visibility = visibility;
        this.Id = Guid.NewGuid();
    }

    public string Title { get; private set; }

    public string? CourseCode { get; set; }

    public string? Description { get; set; }

    public Visibility Visibility { get; set; }

    public Guid Id { get; }

    public IReadOnlyList<Question> Questions => questions.AsReadOnly();

    public void Rename(string newTitle)
    {
        this.Title = ValidateTitle(newTitle);
    }

    public void AddQuestion(Question question)
    {
        ArgumentNullException.ThrowIfNull(question);
        questions.Add(question);
    }

    private static string ValidateTitle(string title)
    {
        title = title.Trim();
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title cannot be empty");
        }

        if (title.Length > 120)
        {
            throw new ArgumentException("Title cannot be longer than 120 characters");
        }

        return title;
    }
}
