using MoneyMirror.Core.DTOs.Quiz;

namespace MoneyMirror.Core.Interfaces
{
    public interface IQuizService
    {
        // Returns next question for child, or null with a status message
        Task<(bool success, NextQuizQuestionDto? question, string message)>
            GetNextQuestionAsync(int childId);

        // Saves the child's answer, returns feedback message
        Task<(bool success, SubmitQuizAnswerResponseDto? response, string errorMessage)>
            SubmitAnswerAsync(int childId, SubmitQuizAnswerDto dto);

        // Called by weekly Hangfire job - returns summary of last 7 answers per child
        Task<List<ChildQuizSummaryDto>> GetWeeklyQuizSummariesAsync();
    }
}