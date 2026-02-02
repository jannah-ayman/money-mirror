using MoneyMirror.Core.Models;

namespace MoneyMirror.Core.Interfaces
{
    /// Interface for personality profile assignment operations.
    /// Handles both temporary placeholder profiles and real AI-based analysis.
    /// This separation makes it easy to swap from dummy to real logic later.
    public interface IPersonalityProfileService
    {
        /// <summary>
        /// Assigns a temporary "Pending Analysis" personality type to a child.
        /// Sets IsPersonalityFinalized = false to indicate this is a placeholder.
        /// <param name="childId">ID of the child to assign temporary profile to</param>
        /// <returns>Tuple: (success flag, assigned personality type or null)</returns>
        Task<(bool success, PersonalityType? assignedType)> AssignTemporaryProfileAsync(int childId);

        /// Assigns a real AI-analyzed personality type based on questionnaire answers.
        /// THIS METHOD IS A STUB FOR NOW - will be implemented when AI team is ready.
        /// <param name="childId">ID of the child to analyze</param>
        /// <param name="questionnaireId">ID of the completed questionnaire</param>
        /// <returns>Tuple: (success flag, assigned personality type or null)</returns>
        Task<(bool success, PersonalityType? assignedType)> AssignRealProfileAsync(int childId, int questionnaireId);

        /// Batch updates all children with pending profiles to real AI-analyzed profiles.
        /// Used as a one-time job when AI team delivers the real logic.
        /// Finds all children where IsPersonalityFinalized = false and re-analyzes them.
        /// THIS METHOD IS A STUB FOR NOW.
        /// <returns>Count of profiles successfully updated</returns>
        Task<int> UpdateAllPendingProfilesAsync();

        /// Gets the "Pending Analysis" personality type from database.
        /// Helper method used by AssignTemporaryProfileAsync.

        /// <returns>The temporary placeholder personality type, or null if not found</returns>
        Task<PersonalityType?> GetPendingAnalysisTypeAsync();
    }
}
