namespace deck;

public enum Visibility
{
    Public,
    Private
}
public class Deck
{
    public string Title {get; private set;}
    public string? CourseCode {get; set;}
    public string? Description {get; set;}
    public Visibility Visibility {get; set;}

    private static void ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title cannot be empty");
        }
        if ((title.Length > 120))
        {
            throw new ArgumentException("Title cannot be longer than 120 characters");
        }
    }

    public Deck(string title, string? courseCode = null, string? description = null, Visibility visibility = Visibility.Private)
    {
        ValidateTitle(title);

        this.Title = title;
        this.CourseCode = courseCode;
        this.Description = description;
        this.Visibility = visibility;
    }

    

    public void Rename(string newTitle)
    {
        ValidateTitle(newTitle);
        this.Title = newTitle;
    }

    

}
