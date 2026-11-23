namespace FinalsWeek.Core;

public static class DemoDeck
{
    public static readonly Guid Id = new("3f8a2c1e-7b4d-4e9a-a6c5-1d2e3f4a5b6c");

    public static Deck Create()
    {
        var deck = new Deck(
            "Demo: Software engineering basics",
            courseCode: "DEMO",
            description: "A short sample deck about Git, Scrum, testing and C#. No account needed.",
            visibility: Visibility.Public,
            id: Id);

        deck.AddQuestion(new Question(
            "Which command creates a new branch and switches to it?",
            ["git branch -d feature", "git switch -c feature", "git merge feature", "git push feature"],
            1,
            "Git"));

        deck.AddQuestion(new Question(
            "What does git pull do?",
            ["Fetches new commits and merges them into the current branch", "Only uploads your commits", "Deletes your local changes"],
            0,
            "Git"));

        deck.AddQuestion(new Question(
            "Who orders the product backlog in Scrum?",
            ["The Scrum Master", "The Product Owner", "The Developers", "The customer"],
            1,
            "Scrum"));

        deck.AddQuestion(new Question(
            "What is the main goal of the sprint retrospective?",
            ["To show the product to stakeholders", "To plan the next sprint", "To improve how the team works"],
            2,
            "Scrum"));

        deck.AddQuestion(new Question(
            "In Arrange-Act-Assert, what happens in the Act step?",
            ["Test data is prepared", "The code under test is called", "The result is checked"],
            1,
            "Testing"));

        deck.AddQuestion(new Question(
            "Which kind of test is the smallest and fastest?",
            ["Unit test", "Integration test", "End-to-end test", "Manual test"],
            0,
            "Testing"));

        deck.AddQuestion(new Question(
            "What does ?. do in C#?",
            ["Throws if the value is null", "Converts a value to nullable", "Returns null instead of using a member when the value is null"],
            2,
            "C#"));

        deck.AddQuestion(new Question(
            "Which keyword lets a method use await?",
            ["static", "virtual", "yield", "async"],
            3,
            "C#"));

        return deck;
    }
}