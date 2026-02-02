using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MoneyMirror.Core.DTOs.Child;
using MoneyMirror.Core.Interfaces;
using MoneyMirror.Core.Models;
using MoneyMirror.Infrastructure.Data;
using System.Text;
using System.Text.Json;

namespace MoneyMirror.Infrastructure.Services
{
    /// <summary>
    /// Handles full child onboarding flow, authentication, and management:
    /// - Creates child
    /// - Links parent-child
    /// - Saves questionnaire (with JSON serialization for multi-select fields)
    /// - Assigns personality profile
    /// - Generates login code
    /// - Authenticates child with code
    /// - Manages refresh tokens
    /// - Handles logout
    /// - Manages parent-child relationships
    /// All steps run inside database transactions where appropriate.
    /// </summary>
    public class ChildService : IChildService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPersonalityProfileService _personalityService;
        private readonly IJwtService _jwtService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ChildService> _logger;

        public ChildService(
            ApplicationDbContext context,
            IPersonalityProfileService personalityService,
            IJwtService jwtService,
            IConfiguration configuration,
            ILogger<ChildService> logger)
        {
            _context = context;
            _personalityService = personalityService;
            _jwtService = jwtService;
            _configuration = configuration;
            _logger = logger;
        }

        /// Completes initial onboarding in ONE request.
        /// The client does NOT send ChildID.
        /// The backend creates everything safely.
        public async Task<(bool success, QuestionnaireCompletionResponseDto? response, string errorMessage)>
            CompleteInitialProfilingAsync(int parentId, CompleteInitialProfilingDto dto)
        {
            // ✅ REQUIRED when EnableRetryOnFailure is enabled
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
                        Age = CalculateAge(dto.DOB),
                        LoginCode = await GenerateUniqueLoginCodeAsync(),
                        CreatedAt = DateTime.UtcNow,
                        IsPersonalityFinalized = false
                    };

                    _context.Children.Add(child);
                    await _context.SaveChangesAsync(); // Generates ChildID

                    _logger.LogInformation(
                        "Created child {FirstName} {LastName} (ID: {ChildID})",
                        child.FName, child.LName, child.ChildID);

                    // STEP 2: LINK PARENT ↔ CHILD
                    _context.ParentChildren.Add(new ParentChild
                    {
                        ParentID = parentId,
                        ChildID = child.ChildID
                    });

                    // STEP 3: SAVE QUESTIONNAIRE
  
                    // Serialize SpendingCategories list to JSON
                    string spendingCategoriesJson = JsonSerializer.Serialize(dto.SpendingCategories);

                    var questionnaire = new InitialProfilingQuestionnaire
                    {
                        ChildID = child.ChildID,

                        // Question 1: Age Group
                        ChildAgeGroup = dto.ChildAgeGroup,

                        // Question 2: Has Allowance
                        HasAllowance = dto.HasAllowance,

                        // Question 3: Spending Pace
                        SpendingPace = dto.SpendingPace,

                        // Question 4: Spending Categories (JSON)
                        SpendingCategories = spendingCategoriesJson,

                        // Question 5: Out of Money Behavior
                        OutOfMoneyBehavior = dto.OutOfMoneyBehavior,

                        // Question 6: Tries to Save
                        TriesToSave = dto.TriesToSave,

                        // Question 7: Money Mindset
                        MoneyMindset = dto.MoneyMindset,

                        // Question 8: Feeling After Spending
                        FeelingAfterSpending = dto.FeelingAfterSpending,

                        // Question 9: Feeling When Saving Grows
                        FeelingWhenSavingGrows = dto.FeelingWhenSavingGrows,

                        // Question 10: Reaction to 100 EGP
                        ReactionTo100 = dto.ReactionTo100,

                        IsCompleted = true,
                        CompletedDate = DateTime.UtcNow
                    };

                    _context.InitialProfilingQuestionnaires.Add(questionnaire);

                    // STEP 4: ASSIGN TEMP PERSONALITY PROFILE

                    var (profileSuccess, assignedType) =
                        await _personalityService.AssignTemporaryProfileAsync(child.ChildID);

                    if (!profileSuccess || assignedType == null)
                    {
                        throw new InvalidOperationException(
                            "Failed to assign personality profile");
                    }

                    // STEP 5: SAVE + COMMIT
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    _logger.LogInformation(
                        "Onboarding completed for child ID {ChildID}",
                        child.ChildID);

                    // STEP 6: BUILD RESPONSE
                    var response = new QuestionnaireCompletionResponseDto
                    {
                        ChildID = child.ChildID,
                        ChildFirstName = child.FName,
                        ChildLoginCode = child.LoginCode,
                        IsPersonalityFinalized = child.IsPersonalityFinalized,
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

        // HELPER METHODS
        private int CalculateAge(DateTime dob)
        {
            var today = DateTime.UtcNow;
            var age = today.Year - dob.Year;

            if (dob.Date > today.AddYears(-age))
                age--;

            return age;
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
    }
}