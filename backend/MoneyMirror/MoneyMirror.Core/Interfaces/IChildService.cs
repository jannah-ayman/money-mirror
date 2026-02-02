using MoneyMirror.Core.DTOs.Child;

namespace MoneyMirror.Core.Interfaces
{
    /// Interface for child account management operations.
    /// Handles questionnaire completion, login code generation, and child profile management.
    public interface IChildService
    {
        /// Completes the initial profiling questionnaire for a child.
        /// Saves all questionnaire answers, assigns personality profile, and generates login code.
        /// <param name="questionnaireDto">Questionnaire answers from parent</param>
        /// <returns>Tuple: (success flag, response with login code and profile, or null if failed)</returns>
        Task<(bool success, QuestionnaireCompletionResponseDto? response, string errorMessage)>
        CompleteInitialProfilingAsync(int parentId, CompleteInitialProfilingDto dto);


        /// Generates a unique login code for a child.
        /// <returns>Unique login code string</returns>
        Task<string> GenerateUniqueLoginCodeAsync();
    }
}
