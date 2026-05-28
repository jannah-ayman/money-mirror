namespace MoneyMirror.Core.DTOs.Quiz
{
    public class QuizAnswerOptionDto
    {
        public int AnswerID { get; set; }
        public string AnswerText { get; set; }
    }

    public class NextQuizQuestionDto
    {
        public int StoryID { get; set; }
        public string Title { get; set; }
        public string QuestionText { get; set; }
        public List<QuizAnswerOptionDto> Answers { get; set; }
    }

    public class SubmitQuizAnswerDto
    {
        public int AnswerID { get; set; }
    }

    public class SubmitQuizAnswerResponseDto
    {
        public string FeedbackMessage { get; set; }
    }

    // Used internally by the weekly Hangfire job
    public class ChildQuizSummaryDto
    {
        public int ChildID { get; set; }
        public int Impulsive { get; set; }
        public int Saver { get; set; }
        public int Planner { get; set; }
        public int Bargain { get; set; }
    }
}