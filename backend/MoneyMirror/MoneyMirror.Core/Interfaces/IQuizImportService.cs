using MoneyMirror.Core.DTOs.Quiz;

namespace MoneyMirror.Core.Interfaces
{
    public interface IQuizImportService
    {
        Task<(bool success, QuizImportResultDto? result, string errorMessage)> ImportFromJsonAsync();
    }
}