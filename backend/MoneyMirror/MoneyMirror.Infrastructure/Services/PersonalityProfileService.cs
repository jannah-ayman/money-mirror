using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoneyMirror.Core.Interfaces;
using MoneyMirror.Core.Models;
using MoneyMirror.Infrastructure.Data;

namespace MoneyMirror.Infrastructure.Services
{
    /// <summary>
    /// Service implementing personality profile assignment logic.
    /// Currently uses placeholder/temporary profiles while AI team develops real logic.
    /// This is the ONLY place where personality assignment happens - makes it easy to update later.
    /// </summary>
    public class PersonalityProfileService : IPersonalityProfileService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PersonalityProfileService> _logger;

        // Constant for the temporary personality type ID
        // This makes it easy to find and update if needed
        private const int PENDING_ANALYSIS_TYPE_ID = 1;

        public PersonalityProfileService(
            ApplicationDbContext context,
            ILogger<PersonalityProfileService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Assigns temporary "Pending Analysis" profile to a child.
        /// This is what we use NOW while AI is being developed.
        /// </summary>
        public async Task<(bool success, PersonalityType? assignedType)> AssignTemporaryProfileAsync(int childId)
        {
            try
            {
                // STEP 1: Find the child
                var child = await _context.Children.FindAsync(childId);

                if (child == null)
                {
                    _logger.LogWarning($"Attempted to assign temporary profile to non-existent child ID: {childId}");
                    return (false, null);
                }

                // STEP 2: Get the "Pending Analysis" personality type
                var pendingType = await GetPendingAnalysisTypeAsync();

                if (pendingType == null)
                {
                    _logger.LogError("Pending Analysis personality type not found in database! Please run database seed.");
                    return (false, null);
                }

                // STEP 3: Assign the temporary type to the child
                child.TypeID = pendingType.TypeID;
                child.IsPersonalityFinalized = false; // Mark as temporary

                _context.Children.Update(child);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Assigned temporary profile to child ID {childId}: {pendingType.ParentName}");

                return (true, pendingType);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error assigning temporary profile to child ID {childId}: {ex.Message}");
                return (false, null);
            }
        }

        /// <summary>
        /// Assigns real AI-analyzed profile based on questionnaire.
        /// STUB METHOD - to be implemented when AI team is ready.
        /// </summary>
        public async Task<(bool success, PersonalityType? assignedType)> AssignRealProfileAsync(int childId, int questionnaireId)
        {
            // TODO: Implement when AI team delivers the personality classification logic
            // Steps will be:
            // 1. Load questionnaire answers from database
            // 2. Run AI scoring/classification algorithm
            // 3. Determine which PersonalityType matches best
            // 4. Update child.TypeID and set IsPersonalityFinalized = true
            // 5. Return the assigned type

            _logger.LogWarning($"AssignRealProfileAsync called but not yet implemented. ChildID: {childId}, QuestionnaireID: {questionnaireId}");
            
            // For now, just return failure
            // When AI is ready, replace this entire method body
            return await Task.FromResult((false, (PersonalityType?)null));
        }

        /// <summary>
        /// Batch update all pending profiles to real ones.
        /// STUB METHOD - to be run once when AI team is ready.
        /// </summary>
        public async Task<int> UpdateAllPendingProfilesAsync()
        {
            // TODO: Implement when AI team delivers the personality classification logic
            // Steps will be:
            // 1. Find all children where IsPersonalityFinalized = false
            // 2. For each child:
            //    - Load their questionnaire
            //    - Call AssignRealProfileAsync()
            // 3. Return count of successfully updated profiles

            _logger.LogWarning("UpdateAllPendingProfilesAsync called but not yet implemented.");
            
            // For now, return 0
            // When AI is ready, replace this entire method body
            return await Task.FromResult(0);
        }

        /// <summary>
        /// Helper method to get the "Pending Analysis" personality type.
        /// </summary>
        public async Task<PersonalityType?> GetPendingAnalysisTypeAsync()
        {
            try
            {
                // Try to find by the constant ID first (most efficient)
                var pendingType = await _context.PersonalityTypes
                    .FirstOrDefaultAsync(pt => pt.TypeID == PENDING_ANALYSIS_TYPE_ID);

                if (pendingType != null)
                {
                    return pendingType;
                }

                // Fallback: search by name if ID doesn't match
                // (in case someone changes the seed data)
                pendingType = await _context.PersonalityTypes
                    .FirstOrDefaultAsync(pt => pt.ParentName == "Pending Analysis");

                if (pendingType == null)
                {
                    _logger.LogError("Could not find 'Pending Analysis' personality type. Database may not be seeded.");
                }

                return pendingType;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving Pending Analysis type: {ex.Message}");
                return null;
            }
        }
    }
}
