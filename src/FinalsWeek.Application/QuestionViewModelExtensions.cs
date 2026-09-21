namespace FinalsWeek.Application;

using FinalsWeek.Core;

public static class QuestionViewModelExtensions
{
    public static StudyQuestionViewModel ToPreSubmitViewModel(
        this Question question,
        int questionNumber)
    {
        return new StudyQuestionViewModel(
            question.Prompt,
            question.Options,
            questionNumber);
    }

    public static StudyQuestionResultViewModel ToPostSubmitViewModel(
        this Question question,
        int questionNumber)
    {
        return new StudyQuestionResultViewModel(
            question.Prompt,
            question.Options,
            questionNumber,
            question.CorrectIndex);
    }
}