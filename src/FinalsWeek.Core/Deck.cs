namespace FinalsWeek.Core;

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

    public Deck(string title, string? courseCode = null, string? description = null, Visibility visibility = Visibility.Private)
    {
        

        this.Title = ValidateTitle(title);
        this.CourseCode = courseCode;
        this.Description = description;
        this.Visibility = visibility;
    }

        public void Rename(string newTitle)
    {
        
        this.Title = ValidateTitle(newTitle);
    }


    private static string ValidateTitle(string title)
    {
        title = title.Trim();
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title cannot be empty");
        }
        if ((title.Length > 120))
        {
            throw new ArgumentException("Title cannot be longer than 120 characters");
        }
        return title;
    }


}
