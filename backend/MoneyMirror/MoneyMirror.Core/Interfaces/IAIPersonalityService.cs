namespace MoneyMirror.Core.Interfaces
{
    /// <summary>
    /// Interface for calling the Python AI personality scoring service.
    /// This makes HTTP calls to the Flask API running on port 5000.
    /// </summary>
    public interface IAIPersonalityService
    {
        /// <summary>
        /// Calculates personality type by calling the Python AI service.
        /// </summary>
        /// <param name="questionnaireId">ID of the completed questionnaire</param>
        /// <returns>Tuple: (success flag, parent personality name, error message)</returns>
        Task<(bool success, string? parentPersonalityName, string errorMessage)>
            CalculatePersonalityFromQuestionnaireAsync(int questionnaireId);
    }
}