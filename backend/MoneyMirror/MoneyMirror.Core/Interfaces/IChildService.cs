using MoneyMirror.Core.DTOs.Child;

namespace MoneyMirror.Core.Interfaces
{
    /// Interface for child account management operations.
    /// Handles questionnaire completion, login code generation, authentication, and child profile management.
    public interface IChildService
    {
        /// Completes the initial profiling questionnaire for a child.
        /// Saves all questionnaire answers, assigns personality profile, and generates login code.
        /// <param name="parentId">ID of the parent creating the child</param>
        /// <param name="dto">Questionnaire answers from parent</param>
        /// <returns>Tuple: (success flag, response with login code and profile, error message)</returns>
        Task<(bool success, QuestionnaireCompletionResponseDto? response, string errorMessage)>
        CompleteInitialProfilingAsync(int parentId, CompleteInitialProfilingDto dto);

        /// Authenticates a child using their unique login code and generates JWT tokens.
        /// <param name="code">The 6-character login code</param>
        /// <returns>Tuple: (success flag, auth response with tokens and child info, error message)</returns>
        Task<(bool success, ChildAuthResponseDto? authResponse, string errorMessage)>
        LoginWithCodeAsync(string code);

        /// Refreshes JWT tokens using a valid refresh token.
        /// Allows child to stay logged in without re-entering code.
        /// <param name="refreshTokenDto">Current tokens from client</param>
        /// <returns>New auth response with fresh tokens if successful, or error message</returns>
        Task<(bool success, ChildAuthResponseDto? authResponse, string errorMessage)>
        RefreshTokenAsync(ChildRefreshTokenDto refreshTokenDto);

        /// Revokes a child's refresh token (logout functionality).
        /// Marks the refresh token as invalid in the database.
        /// <param name="childId">ID of the child logging out</param>
        /// <returns>True if successfully revoked, False otherwise</returns>
        Task<bool> RevokeRefreshTokenAsync(int childId);

        /// Adds an existing child to a parent's account using the child's login code.
        /// Supports shared custody - multiple parents can manage the same child.
        /// <param name="parentId">ID of the parent adding the child</param>
        /// <param name="code">The child's 6-character login code</param>
        /// <returns>Tuple: (success flag, success message, error message)</returns>
        Task<(bool success, string message, string errorMessage)>
        AddExistingChildAsync(int parentId, string code);

        /// Gets all children linked to a specific parent.
        /// Returns basic information needed for the "Manage Children" tab.
        /// <param name="parentId">ID of the parent</param>
        /// <returns>List of child summary DTOs ordered by creation date (newest first)</returns>
        Task<List<ChildSummaryDto>> GetMyChildrenAsync(int parentId);

        /// Generates a unique login code for a child.
        /// <returns>Unique login code string</returns>
        Task<string> GenerateUniqueLoginCodeAsync();

        /// Updates all children's ages based on current date.
        /// Called by background job daily.
        /// <returns>Number of children whose ages were updated</returns>
        Task<int> UpdateAllChildrenAgesAsync();

        // ==================== NEW METHODS FOR PART 2 ====================

        /// Gets the child's full profile information.
        /// Used for the "My Profile" screen.
        /// <param name="childId">ID of the child (from JWT token)</param>
        /// <returns>Tuple: (success flag, profile data, error message)</returns>
        Task<(bool success, ChildProfileResponseDto? profile, string errorMessage)>
        GetMyProfileAsync(int childId);

        /// Gets the child's dashboard data.
        /// Used for the main home screen.
        /// <param name="childId">ID of the child (from JWT token)</param>
        /// <returns>Tuple: (success flag, dashboard data, error message)</returns>
        Task<(bool success, ChildDashboardDto? dashboard, string errorMessage)>
        GetMyDashboardAsync(int childId);

        // ==================== PARENT MANAGEMENT OF CHILDREN ====================

        /// Updates a child's basic information (name, DOB).
        /// Only the linked parent can update.
        /// Age and age group are recalculated automatically.
        /// <param name="parentId">ID of the parent (from JWT token)</param>
        /// <param name="childId">ID of the child to update</param>
        /// <param name="dto">Updated child information</param>
        /// <returns>Tuple: (success flag, updated child data, error message)</returns>
        Task<(bool success, UpdateChildResponseDto? updatedChild, string errorMessage)>
        UpdateChildAsync(int parentId, int childId, UpdateChildDto dto);

        /// Regenerates a new login code for a child.
        /// Old code becomes invalid immediately.
        /// Only the linked parent can regenerate.
        /// <param name="parentId">ID of the parent (from JWT token)</param>
        /// <param name="childId">ID of the child</param>
        /// <returns>Tuple: (success flag, new login code, error message)</returns>
        Task<(bool success, RegenerateCodeResponseDto? codeInfo, string errorMessage)>
        RegenerateLoginCodeAsync(int parentId, int childId);

        /// Permanently deletes a child and all their data.
        /// This is a HARD DELETE - data cannot be recovered.
        /// Only the linked parent can delete.
        /// <param name="parentId">ID of the parent (from JWT token)</param>
        /// <param name="childId">ID of the child to delete</param>
        /// <returns>Tuple: (success flag, confirmation message, error message)</returns>
        Task<(bool success, string message, string errorMessage)>
        DeleteChildAsync(int parentId, int childId);
    }
}