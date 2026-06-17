using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MoneyMirror.Core.DTOs.Child;
using MoneyMirror.Core.Enums;
using MoneyMirror.Core.Helpers;
using MoneyMirror.Core.Interfaces;
using MoneyMirror.Core.Models;
using MoneyMirror.Infrastructure.Data;
using System.Text;
using System.Text.Json;

namespace MoneyMirror.Infrastructure.Services
{
   
    public class ChildService : IChildService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPersonalityProfileService _personalityService;
        private readonly IJwtService _jwtService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ChildService> _logger;
        private readonly IAIPersonalityService _aiPersonalityService;
        public ChildService(
            ApplicationDbContext context,
            IPersonalityProfileService personalityService,
            IJwtService jwtService,
            IConfiguration configuration,
            ILogger<ChildService> logger,
            IAIPersonalityService aiPersonalityService )
        {
            _context = context;
            _personalityService = personalityService;
            _jwtService = jwtService;
            _configuration = configuration;
            _logger = logger;
            _aiPersonalityService = aiPersonalityService;
        }

        /// Completes initial onboarding in ONE request.
        /// The client does NOT send ChildID.
        public async Task<(bool success, QuestionnaireCompletionResponseDto? response, string errorMessage)>
        CompleteInitialProfilingAsync(int parentId, CompleteInitialProfilingDto dto)
        {
            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    // STEP 1: CREATE CHILD 
                    var child = new Child
                    {
                        FName = dto.ChildFirstName,
                        LName = dto.ChildLastName,
                        DOB = dto.DOB,
                        Age = AgeHelper.CalculateAge(dto.DOB),
                        Gender = string.IsNullOrWhiteSpace(dto.Gender) ? null : dto.Gender.Trim(),
                        LoginCode = await GenerateUniqueLoginCodeAsync(),
                        CreatedAt = DateTime.UtcNow,
                        IsPersonalityFinalized = false,
                        CharacterID = 1 // Default: Nova
                    };

                    _context.Children.Add(child);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation(
                        "Created child {FirstName} {LastName} (ID: {ChildID})",
                        child.FName, child.LName, child.ChildID);

                    // STEP 2: LINK PARENT ↔ CHILD (same as before)
                    _context.ParentChildren.Add(new ParentChild
                    {
                        ParentID = parentId,
                        ChildID = child.ChildID
                    });

                    // STEP 3: SAVE QUESTIONNAIRE (same as before)
                    string spendingCategoriesJson = JsonSerializer.Serialize(dto.SpendingCategories);
                    ChildAgeGroup calculatedAgeGroup = AgeHelper.CalculateAgeGroup(dto.DOB);

                    var questionnaire = new InitialProfilingQuestionnaire
                    {
                        ChildID = child.ChildID,
                        ChildAgeGroup = calculatedAgeGroup,
                        HasAllowance = dto.HasAllowance,
                        SpendingPace = dto.SpendingPace,
                        SpendingCategories = spendingCategoriesJson,
                        OutOfMoneyBehavior = dto.OutOfMoneyBehavior,
                        TriesToSave = dto.TriesToSave,
                        MoneyMindset = dto.MoneyMindset,
                        FeelingAfterSpending = dto.FeelingAfterSpending,
                        FeelingWhenSavingGrows = dto.FeelingWhenSavingGrows,
                        ReactionTo100 = dto.ReactionTo100,
                        IsCompleted = true,
                        CompletedDate = DateTime.UtcNow
                    };

                    _context.InitialProfilingQuestionnaires.Add(questionnaire);
                    await _context.SaveChangesAsync(); // ✅ Save to get QuestionnaireID

                    // ✅ AFTER SAVING QUESTIONNAIRE, CALL PYTHON AI
                    _logger.LogInformation($"Calling Python AI service for questionnaire {questionnaire.QuestionnaireID}");

                    var (aiSuccess, parentPersonalityName, aiError) =
                        await _aiPersonalityService.CalculatePersonalityFromQuestionnaireAsync(
                            questionnaire.QuestionnaireID);

                    PersonalityType assignedType;

                    if (aiSuccess && !string.IsNullOrEmpty(parentPersonalityName))
                    {
                        // ✅ PYTHON AI SUCCESS - USE THE REAL TYPE
                        _logger.LogInformation($"Python AI returned: {parentPersonalityName}");

                        // Find the personality type in database by name
                        var personalityType = await _context.PersonalityTypes
                            .FirstOrDefaultAsync(pt => pt.ParentName == parentPersonalityName);

                        if (personalityType != null)
                        {
                            // Update questionnaire
                            questionnaire.CalculatedTypeID = personalityType.TypeID;
                            _context.InitialProfilingQuestionnaires.Update(questionnaire);

                            // Assign to child
                            child.TypeID = personalityType.TypeID;
                            child.IsPersonalityFinalized = true; // ✅ REAL AI ANALYSIS!
                            _context.Children.Update(child);

                            assignedType = personalityType;

                            _logger.LogInformation($"✅ Assigned REAL personality: {assignedType.ParentName}");
                        }
                        else
                        {
                            _logger.LogWarning($"Personality type '{parentPersonalityName}' not found in database");
                            throw new InvalidOperationException($"Personality type '{parentPersonalityName}' not in database");
                        }
                    }
                    else
                    {
                        // ✅ PYTHON AI FAILED - USE PENDING ANALYSIS
                        _logger.LogWarning($"Python AI failed: {aiError}. Using Pending Analysis fallback.");

                        var (fallbackSuccess, fallbackType) =
                            await _personalityService.AssignTemporaryProfileAsync(child.ChildID);

                        if (!fallbackSuccess || fallbackType == null)
                        {
                            throw new InvalidOperationException("Failed to assign personality profile");
                        }

                        assignedType = fallbackType;
                    }

                    // STEP 5: SAVE + COMMIT
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    _logger.LogInformation($"Onboarding completed for child ID {child.ChildID}");

                    // STEP 6: BUILD RESPONSE
                    var response = new QuestionnaireCompletionResponseDto
                    {
                        ChildID = child.ChildID,
                        ChildFirstName = child.FName,
                        Age = child.Age,
                        Gender = child.Gender,
                        ChildLoginCode = child.LoginCode,
                        IsPersonalityFinalized = child.IsPersonalityFinalized, // ✅ Will be true if AI succeeded
                        PersonalityProfile = new PersonalityProfileDto
                        {
                            TypeID = assignedType.TypeID,
                            ParentName = assignedType.ParentName,
                            ChildName = assignedType.ChildName,
                            Description = assignedType.Desc,
                            FunFacts = assignedType.FunFacts
                        }
                    };

                    return (true, response, string.Empty);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error during child onboarding");
                    return (false, null, "An error occurred while onboarding the child");
                }
            });
        }
        /// Authenticates a child using their unique login code.
        /// Generates JWT tokens similar to parent login.
        /// Stores refresh token in database.
        public async Task<(bool success, ChildAuthResponseDto? authResponse, string errorMessage)>
            LoginWithCodeAsync(string code)
        {
            try
            {
                // Normalize code (uppercase, trim)
                var normalizedCode = code.ToUpper().Trim();

                // STEP 1: Find child by login code
                var child = await _context.Children
                    .Include(c => c.PersonalityType) // Load personality info
                    .FirstOrDefaultAsync(c => c.LoginCode == normalizedCode);

                if (child == null)
                {
                    _logger.LogWarning($"Login attempt with invalid code: {normalizedCode}");
                    return (false, null, "Invalid or expired code");
                }

                // STEP 2: Generate JWT tokens
                string accessToken = _jwtService.GenerateAccessToken(child);
                string refreshToken = _jwtService.GenerateRefreshToken();

                int accessTokenExpirationMinutes = int.Parse(
                    _configuration["Jwt:AccessTokenExpirationMinutes"] ?? "15");
                int refreshTokenExpirationDays = int.Parse(
                    _configuration["Jwt:RefreshTokenExpirationDays"] ?? "7");

                DateTime accessTokenExpiry = DateTime.UtcNow.AddMinutes(accessTokenExpirationMinutes);
                DateTime refreshTokenExpiry = DateTime.UtcNow.AddDays(refreshTokenExpirationDays);

                // STEP 3: Store refresh token in database (hashed)
                child.RefreshToken = BCrypt.Net.BCrypt.HashPassword(refreshToken);
                child.RefreshTokenExpiry = refreshTokenExpiry;

                _context.Children.Update(child);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Child logged in successfully: {child.FName} (ID: {child.ChildID})");

                // STEP 4: Build response
                var authResponse = new ChildAuthResponseDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    AccessTokenExpiration = accessTokenExpiry,
                    RefreshTokenExpiration = refreshTokenExpiry,
                    ChildId = child.ChildID,
                    ChildFirstName = child.FName,
                    Age = child.Age,
                    IsPersonalityFinalized = child.IsPersonalityFinalized,
                    PersonalityProfile = child.PersonalityType != null ? new PersonalityProfileDto
                    {
                        TypeID = child.PersonalityType.TypeID,
                        ParentName = child.PersonalityType.ParentName,
                        ChildName = child.PersonalityType.ChildName,
                        Description = child.PersonalityType.Desc,
                        FunFacts = child.PersonalityType.FunFacts
                    } : null
                };

                return (true, authResponse, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during child login: {ex.Message}");
                return (false, null, "An error occurred during login");
            }
        }

        /// Refreshes JWT tokens using a valid refresh token.
        /// Allows child to stay logged in without re-entering code.
        public async Task<(bool success, ChildAuthResponseDto? authResponse, string errorMessage)>
            RefreshTokenAsync(ChildRefreshTokenDto refreshTokenDto)
        {
            try
            {
                // STEP 1: Extract claims from expired access token
                var principal = _jwtService.GetPrincipalFromToken(refreshTokenDto.AccessToken);

                if (principal == null)
                {
                    _logger.LogWarning("Refresh token attempt with invalid access token");
                    return (false, null, "Invalid access token");
                }

                // Get child ID from token claims
                var childIdClaim = principal.FindFirst("ChildId")?.Value;
                if (childIdClaim == null || !int.TryParse(childIdClaim, out int childId))
                {
                    _logger.LogWarning("Refresh token attempt with missing ChildId claim");
                    return (false, null, "Invalid token claims");
                }

                // STEP 2: Find child in database
                var child = await _context.Children
                    .Include(c => c.PersonalityType)
                    .FirstOrDefaultAsync(c => c.ChildID == childId);

                if (child == null)
                {
                    _logger.LogWarning($"Refresh token attempt for non-existent child ID: {childId}");
                    return (false, null, "Child not found");
                }

                // STEP 3: Verify refresh token
                if (string.IsNullOrEmpty(child.RefreshToken))
                {
                    _logger.LogWarning($"Refresh token attempt but child has no stored token: {child.FName}");
                    return (false, null, "No refresh token found. Please log in again.");
                }

                // Verify refresh token matches database (compare hashed)
                bool refreshTokenValid = BCrypt.Net.BCrypt.Verify(
                    refreshTokenDto.RefreshToken,
                    child.RefreshToken);

                if (!refreshTokenValid)
                {
                    _logger.LogWarning($"Refresh token attempt with invalid token for: {child.FName}");
                    return (false, null, "Invalid refresh token. Please log in again.");
                }

                // Check if refresh token has expired
                if (child.RefreshTokenExpiry == null || child.RefreshTokenExpiry < DateTime.UtcNow)
                {
                    _logger.LogWarning($"Refresh token attempt with expired token for: {child.FName}");
                    return (false, null, "Refresh token has expired. Please log in again.");
                }

                // STEP 4: Generate new tokens
                string newAccessToken = _jwtService.GenerateAccessToken(child);
                string newRefreshToken = _jwtService.GenerateRefreshToken();

                // Calculate new expiration times
                int accessTokenExpirationMinutes = int.Parse(
                    _configuration["Jwt:AccessTokenExpirationMinutes"] ?? "15");
                int refreshTokenExpirationDays = int.Parse(
                    _configuration["Jwt:RefreshTokenExpirationDays"] ?? "7");

                DateTime newAccessTokenExpiry = DateTime.UtcNow.AddMinutes(accessTokenExpirationMinutes);
                DateTime newRefreshTokenExpiry = DateTime.UtcNow.AddDays(refreshTokenExpirationDays);

                // STEP 5: Update refresh token in database
                child.RefreshToken = BCrypt.Net.BCrypt.HashPassword(newRefreshToken);
                child.RefreshTokenExpiry = newRefreshTokenExpiry;

                _context.Children.Update(child);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Tokens refreshed successfully for child: {child.FName} (ID: {child.ChildID})");

                // STEP 6: Build response
                var authResponse = new ChildAuthResponseDto
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken,
                    AccessTokenExpiration = newAccessTokenExpiry,
                    RefreshTokenExpiration = newRefreshTokenExpiry,
                    ChildId = child.ChildID,
                    ChildFirstName = child.FName,
                    Age = child.Age,
                    IsPersonalityFinalized = child.IsPersonalityFinalized,
                    PersonalityProfile = child.PersonalityType != null ? new PersonalityProfileDto
                    {
                        TypeID = child.PersonalityType.TypeID,
                        ParentName = child.PersonalityType.ParentName,
                        ChildName = child.PersonalityType.ChildName,
                        Description = child.PersonalityType.Desc,
                        FunFacts = child.PersonalityType.FunFacts
                    } : null
                };

                return (true, authResponse, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during token refresh: {ex.Message}");
                return (false, null, "An error occurred during token refresh");
            }
        }

        /// Revokes a child's refresh token (logout functionality).
        /// Clears the stored refresh token so it can't be used again.
        public async Task<bool> RevokeRefreshTokenAsync(int childId)
        {
            try
            {
                var child = await _context.Children.FindAsync(childId);

                if (child == null)
                {
                    _logger.LogWarning($"Revoke token attempt for non-existent child ID: {childId}");
                    return false;
                }

                // Clear refresh token
                child.RefreshToken = null;
                child.RefreshTokenExpiry = null;

                _context.Children.Update(child);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Refresh token revoked for child: {child.FName} (ID: {child.ChildID})");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error revoking refresh token: {ex.Message}");
                return false;
            }
        }

        /// Adds an existing child to a parent's account using the child's login code.
        /// Supports shared custody - multiple parents can manage the same child.
        public async Task<(bool success, string message, string errorMessage)>
            AddExistingChildAsync(int parentId, string code)
        {
            try
            {
                // Normalize code (uppercase, trim)
                var normalizedCode = code.ToUpper().Trim();

                // STEP 1: Find child by login code
                var child = await _context.Children
                    .FirstOrDefaultAsync(c => c.LoginCode == normalizedCode);

                if (child == null)
                {
                    _logger.LogWarning($"Add child attempt with invalid code: {normalizedCode}");
                    return (false, string.Empty, "Invalid code");
                }

                // STEP 2: Check if this parent-child relationship already exists
                var existingRelationship = await _context.ParentChildren
                    .AnyAsync(pc => pc.ParentID == parentId && pc.ChildID == child.ChildID);

                if (existingRelationship)
                {
                    _logger.LogWarning($"Parent {parentId} attempted to add child {child.ChildID} again");
                    return (false, string.Empty, "This child is already linked to your account");
                }

                // STEP 3: Create parent-child relationship
                _context.ParentChildren.Add(new ParentChild
                {
                    ParentID = parentId,
                    ChildID = child.ChildID
                });

                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    $"Parent {parentId} successfully added existing child {child.ChildID} ({child.FName})");

                return (true, $"Child {child.FName} added successfully!", string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding existing child: {ex.Message}");
                return (false, string.Empty, "An error occurred while adding the child");
            }
        }

        public async Task<(bool success, string message, string errorMessage)>
    UnlinkChildAsync(int parentId, int childId)
        {
            try
            {
                var link = await _context.ParentChildren
                    .FirstOrDefaultAsync(pc => pc.ParentID == parentId && pc.ChildID == childId);

                if (link == null)
                    return (false, string.Empty, "This child is not linked to your account");

                _context.ParentChildren.Remove(link);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Parent {ParentId} unlinked child {ChildId}", parentId, childId);
                return (true, "Child unlinked successfully", string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unlinking child {ChildId} from parent {ParentId}", childId, parentId);
                return (false, string.Empty, "An error occurred while unlinking the child");
            }
        }
        /// Gets all children linked to a specific parent.
        /// Returns basic information needed for the "Manage Children" tab.
        public async Task<List<ChildSummaryDto>> GetMyChildrenAsync(int parentId)
        {
            try
            {
                var children = await _context.ParentChildren
                    .Where(pc => pc.ParentID == parentId)
                    .Include(pc => pc.Child)
                        .ThenInclude(c => c.PersonalityType)
                    .OrderByDescending(pc => pc.Child.CreatedAt) // Newest first
                    .Select(pc => new ChildSummaryDto
                    {
                        ChildID = pc.Child.ChildID,
                        ChildName = $"{pc.Child.FName} {pc.Child.LName}",
                        DOB = pc.Child.DOB,
                        Age = pc.Child.Age,
                        Gender = pc.Child.Gender,
                        LoginCode = pc.Child.LoginCode,
                        CreatedAt = pc.Child.CreatedAt,
                        IsPersonalityFinalized = pc.Child.IsPersonalityFinalized,
                        PersonalityTypeName = pc.Child.PersonalityType != null
                            ? pc.Child.PersonalityType.ParentName
                            : "Pending Analysis"
                    })
                    .ToListAsync();

                _logger.LogInformation(
                    $"Retrieved {children.Count} children for parent {parentId}");

                return children;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving children for parent {parentId}: {ex.Message}");
                return new List<ChildSummaryDto>();
            }
        }


        public async Task<string> GenerateUniqueLoginCodeAsync()
        {
            const string letters = "ABCDEFGHJKLMNPQRSTUVWXYZ";
            const string numbers = "0123456789";
            const int maxAttempts = 100;

            var random = new Random();

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                var code = new StringBuilder();

                for (int i = 0; i < 3; i++)
                    code.Append(letters[random.Next(letters.Length)]);

                for (int i = 0; i < 3; i++)
                    code.Append(numbers[random.Next(numbers.Length)]);

                var loginCode = code.ToString();

                var exists = await _context.Children
                    .AnyAsync(c => c.LoginCode == loginCode);

                if (!exists)
                    return loginCode;
            }

            throw new InvalidOperationException(
                "Unable to generate unique login code");
        }
        public async Task<int> UpdateAllChildrenAgesAsync()
        {
            try
            {
                var children = await _context.Children.ToListAsync();
                int updatedCount = 0;

                foreach (var child in children)
                {
                    int currentAge = AgeHelper.CalculateAge(child.DOB);

                    if (child.Age != currentAge)
                    {
                        child.Age = currentAge;
                        updatedCount++;
                    }
                }

                if (updatedCount > 0)
                {
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Updated ages for {updatedCount} children");
                }

                return updatedCount;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating children ages: {ex.Message}");
                return 0;
            }
        }

        // ==================== CHILD DASHBOARD & PROFILE ====================

        /// Gets the child's full profile information for "My Profile" screen.
        /// This is simple - just fetch the child and their personality type.
        public async Task<(bool success, ChildProfileResponseDto? profile, string errorMessage)>
            GetMyProfileAsync(int childId)
        {
            try
            {
                // STEP 1: Find child with their personality type
                var child = await _context.Children
                    .Include(c => c.PersonalityType) // Load personality info
                    .FirstOrDefaultAsync(c => c.ChildID == childId);

                if (child == null)
                {
                    _logger.LogWarning($"Profile request for non-existent child {childId}");
                    return (false, null, "Child not found");
                }

                // STEP 2: Build the response
                var profile = new ChildProfileResponseDto
                {
                    ChildID = child.ChildID,
                    FirstName = child.FName,
                    LastName = child.LName,
                    Age = child.Age,
                    Gender = child.Gender,
                    CurrentBalance = child.CurrentBalance,
                    AvatarUrl = null, // TODO: implement avatar selection later
                    PersonalityInfo = new PersonalityInfoDto
                    {
                        ChildName = child.PersonalityType?.ChildName ?? "Little Learner",
                        FunFacts = child.PersonalityType?.FunFacts,
                        IsFinalized = child.IsPersonalityFinalized
                    }
                };

                _logger.LogInformation($"Profile loaded for child {childId}");

                return (true, profile, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting profile for child {childId}: {ex.Message}");
                return (false, null, "An error occurred while loading your profile");
            }
        }

        /// Gets the child's dashboard data for main home screen.
        /// Shows balance, personality, and counts for goals/expenses.
        public async Task<(bool success, ChildDashboardDto? dashboard, string errorMessage)>
     GetMyDashboardAsync(int childId)
        {
            try
            {
                var child = await _context.Children
                    .Include(c => c.PersonalityType)
                    .FirstOrDefaultAsync(c => c.ChildID == childId);

                if (child == null)
                {
                    _logger.LogWarning($"Dashboard request for non-existent child {childId}");
                    return (false, null, "Child not found");
                }

                int activeGoalsCount = await _context.SavingsGoals
                    .CountAsync(g => g.ChildID == childId && g.Status == "Active");

                var allowance = await _context.Allowances
                    .FirstOrDefaultAsync(a => a.ChildID == childId && a.IsRecurring && a.IsActive);

                bool lowBalanceAlert = false;

                if (allowance != null && allowance.Amount > 0)
                {
                    var now = DateTime.UtcNow;
                    decimal spentRatio = 1 - (child.CurrentBalance / allowance.Amount);

                    if (allowance.Type == "Weekly")
                    {
                        int daysRemainingInWeek = 7 - (int)now.DayOfWeek;
                        lowBalanceAlert = spentRatio >= 0.75m && daysRemainingInWeek >= 3;
                    }
                    else if (allowance.Type == "Monthly")
                    {
                        int daysRemainingInMonth = DateTime.DaysInMonth(now.Year, now.Month) - now.Day;
                        lowBalanceAlert = spentRatio >= 0.75m && daysRemainingInMonth >= 10;
                    }
                }

                var dashboard = new ChildDashboardDto
                {
                    FirstName = child.FName,
                    CurrentBalance = child.CurrentBalance,
                    AvatarUrl = null,
                    PersonalityName = child.PersonalityType?.ChildName ?? "Little Learner",
                    FunFacts = child.PersonalityType?.FunFacts,
                    ActiveGoalsCount = activeGoalsCount,
                    LowBalanceAlert = lowBalanceAlert
                };

                _logger.LogInformation($"Dashboard loaded for child {childId}");
                return (true, dashboard, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting dashboard for child {childId}: {ex.Message}");
                return (false, null, "An error occurred while loading your dashboard");
            }
        }
        // ==================== PARENT MANAGEMENT OF CHILDREN ====================

        /// Helper method to verify parent-child relationship.
        /// Returns true only if the parent is linked to this child.
        private async Task<bool> IsParentLinkedToChildAsync(int parentId, int childId)
        {
            return await _context.ParentChildren
                .AnyAsync(pc => pc.ParentID == parentId && pc.ChildID == childId);
        }

        /// Updates a child's basic information.
        /// Parent can change first name, last name, and date of birth.
        /// Age and age group are recalculated automatically.
        public async Task<(bool success, UpdateChildResponseDto? updatedChild, string errorMessage)>
            UpdateChildAsync(int parentId, int childId, UpdateChildDto dto)
        {
            try
            {
                // STEP 1: Verify parent-child relationship
                bool isLinked = await IsParentLinkedToChildAsync(parentId, childId);

                if (!isLinked)
                {
                    _logger.LogWarning($"Parent {parentId} attempted to update non-linked child {childId}");
                    return (false, null, "You are not authorized to update this child");
                }

                // STEP 2: Find the child
                var child = await _context.Children.FindAsync(childId);

                if (child == null)
                {
                    _logger.LogWarning($"Update attempt for non-existent child {childId}");
                    return (false, null, "Child not found");
                }

                // STEP 3: Update the fields
                child.FName = dto.FirstName.Trim();
                child.LName = dto.LastName.Trim();
                child.DOB = dto.DateOfBirth;

                // STEP 4: Recalculate age and age group automatically
                child.Age = AgeHelper.CalculateAge(dto.DateOfBirth);
                var newAgeGroup = AgeHelper.CalculateAgeGroup(dto.DateOfBirth);

                // STEP 5: Update questionnaire age group if it exists
                var questionnaire = await _context.InitialProfilingQuestionnaires
                    .FirstOrDefaultAsync(q => q.ChildID == childId);

                if (questionnaire != null)
                {
                    questionnaire.ChildAgeGroup = newAgeGroup;
                    _context.InitialProfilingQuestionnaires.Update(questionnaire);
                }

                // STEP 6: Save changes
                _context.Children.Update(child);
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    $"Parent {parentId} updated child {childId}: {child.FName} {child.LName}, Age: {child.Age}");

                // STEP 7: Build response
                var response = new UpdateChildResponseDto
                {
                    ChildID = child.ChildID,
                    FirstName = child.FName,
                    LastName = child.LName,
                    DateOfBirth = child.DOB,
                    Age = child.Age,
                    AgeGroup = AgeHelper.GetAgeGroupDisplayName(newAgeGroup)
                };

                return (true, response, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating child {childId}: {ex.Message}");
                return (false, null, "An error occurred while updating the child");
            }
        }

        /// Regenerates a new login code for a child.
        /// Old code becomes invalid immediately.
        /// This is useful if the code is lost or compromised.
        public async Task<(bool success, RegenerateCodeResponseDto? codeInfo, string errorMessage)>
            RegenerateLoginCodeAsync(int parentId, int childId)
        {
            try
            {
                // STEP 1: Verify parent-child relationship
                bool isLinked = await IsParentLinkedToChildAsync(parentId, childId);

                if (!isLinked)
                {
                    _logger.LogWarning($"Parent {parentId} attempted to regenerate code for non-linked child {childId}");
                    return (false, null, "You are not authorized to manage this child");
                }

                // STEP 2: Find the child
                var child = await _context.Children.FindAsync(childId);

                if (child == null)
                {
                    _logger.LogWarning($"Code regeneration attempt for non-existent child {childId}");
                    return (false, null, "Child not found");
                }

                // STEP 3: Generate new unique code
                string oldCode = child.LoginCode;
                string newCode = await GenerateUniqueLoginCodeAsync();

                // STEP 4: Update the child's login code
                child.LoginCode = newCode;

                // STEP 5: Revoke any active refresh tokens (force re-login with new code)
                child.RefreshToken = null;
                child.RefreshTokenExpiry = null;

                _context.Children.Update(child);
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    $"Parent {parentId} regenerated login code for child {childId} ({child.FName}). Old: {oldCode}, New: {newCode}");

                // STEP 6: Build response
                var response = new RegenerateCodeResponseDto
                {
                    NewLoginCode = newCode,
                    ChildName = $"{child.FName} {child.LName}"
                };

                return (true, response, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error regenerating code for child {childId}: {ex.Message}");
                return (false, null, "An error occurred while generating a new code");
            }
        }

        /// <summary>
        /// Permanently deletes a child and ALL their data.
        /// This is a HARD DELETE - cannot be undone.
        /// 
        /// Why hard delete for children:
        /// - Privacy: Parent explicitly wants child's data removed
        /// - Compliance: Child's data should not be used for training
        /// - Clear intent: Different from parent account deletion
        /// 
        /// What gets deleted:
        /// - Child record
        /// - All expenses
        /// - All allowances
        /// - All transactions
        /// - All goals
        /// - All achievements
        /// - Questionnaire
        /// - Everything linked to this child
        /// 
        /// What does NOT get deleted:
        /// - Parent account (remains intact)
        /// - Other children under same parent (unaffected)
        /// </summary>
        public async Task<(bool success, string message, string errorMessage)>
            DeleteChildAsync(int parentId, int childId)
        {
            // ✅ Use execution strategy for transaction safety
            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    // STEP 1: Verify parent-child relationship
                    bool isLinked = await IsParentLinkedToChildAsync(parentId, childId);

                    if (!isLinked)
                    {
                        _logger.LogWarning($"Parent {parentId} attempted to delete non-linked child {childId}");
                        return (false, string.Empty, "You are not authorized to delete this child");
                    }

                    // STEP 2: Find the child
                    var child = await _context.Children
                        .Include(c => c.ParentChildren)
                        .FirstOrDefaultAsync(c => c.ChildID == childId);

                    if (child == null)
                    {
                        _logger.LogWarning($"Delete attempt for non-existent child {childId}");
                        return (false, string.Empty, "Child not found");
                    }

                    string childName = $"{child.FName} {child.LName}";

                    // STEP 3: Delete all related data
                    // Entity Framework will handle cascading deletes based on your
                    // OnDelete settings in ApplicationDbContext.cs

                    // Remove ParentChild relationships first
                    _context.ParentChildren.RemoveRange(child.ParentChildren);

                    // Remove the child (cascading deletes handle the rest)
                    _context.Children.Remove(child);

                    // STEP 4: Save and commit
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    _logger.LogInformation(
                        $"Parent {parentId} permanently deleted child {childId} ({childName}) and all related data");

                    return (true, $"Child {childName} and all their data have been permanently deleted", string.Empty);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError($"Error deleting child {childId}: {ex.Message}");
                    return (false, string.Empty, "An error occurred while deleting the child");
                }
            });
        }
    }
}