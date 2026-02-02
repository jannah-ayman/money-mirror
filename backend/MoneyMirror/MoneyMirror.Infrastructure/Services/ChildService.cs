using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoneyMirror.Core.DTOs.Child;
using MoneyMirror.Core.Interfaces;
using MoneyMirror.Core.Models;
using MoneyMirror.Infrastructure.Data;
using System.Text;

namespace MoneyMirror.Infrastructure.Services
{
    /// <summary>
    /// Handles full child onboarding flow:
    /// - Creates child
    /// - Links parent-child
    /// - Saves questionnaire
    /// - Assigns personality profile
    /// - Generates login code
    /// All steps run inside a single database transaction.
    /// </summary>
    public class ChildService : IChildService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPersonalityProfileService _personalityService;
        private readonly ILogger<ChildService> _logger;

        public ChildService(
            ApplicationDbContext context,
            IPersonalityProfileService personalityService,
            ILogger<ChildService> logger)
        {
            _context = context;
            _personalityService = personalityService;
            _logger = logger;
        }

        /// <summary>
        /// Completes initial onboarding in ONE request.
        /// The client does NOT send ChildID.
        /// The backend creates everything safely.
        /// </summary>
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
                    // ============================================================
                    // STEP 1: CREATE CHILD
                    // ============================================================

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

                    // ============================================================
                    // STEP 2: LINK PARENT ↔ CHILD
                    // ============================================================

                    _context.ParentChildren.Add(new ParentChild
                    {
                        ParentID = parentId,
                        ChildID = child.ChildID
                    });

                    // ============================================================
                    // STEP 3: SAVE QUESTIONNAIRE
                    // ============================================================

                    var questionnaire = new InitialProfilingQuestionnaire
                    {
                        ChildID = child.ChildID,

                        ChildAgeGroup = dto.ChildAgeGroup,
                        ChildGender = dto.ChildGender,

                        AllowanceFrequency = dto.AllowanceFrequency,
                        AllowanceAmount = dto.AllowanceAmount,

                        PrimarySpendingCategory = dto.PrimarySpendingCategory,
                        SpendingPlanning = dto.SpendingPlanning,
                        OutOfMoneyBehavior = dto.OutOfMoneyBehavior,
                        SpendingAffectsSaving = dto.SpendingAffectsSaving,
                        SpendingPace = dto.SpendingPace,

                        SavingGoal = dto.SavingGoal,
                        SavingPercentage = dto.SavingPercentage,
                        SavingSuccessRate = dto.SavingSuccessRate,

                        FeelingAfterSpending = dto.FeelingAfterSpending,
                        SavingFailureReason = dto.SavingFailureReason,
                        SatisfactionPreference = dto.SatisfactionPreference,
                        TalksAboutMoney = dto.TalksAboutMoney,
                        FeelingWhenSavingGrows = dto.FeelingWhenSavingGrows,

                        ReactionTo100 = dto.ReactionTo100,
                        MoneyPriority = dto.MoneyPriority,
                        ReactionToExpensiveItem = dto.ReactionToExpensiveItem,
                        ReactionToMoreAllowance = dto.ReactionToMoreAllowance,
                        MoneyMindset = dto.MoneyMindset,

                        IsCompleted = true,
                        CompletedDate = DateTime.UtcNow
                    };

                    _context.InitialProfilingQuestionnaires.Add(questionnaire);

                    // ============================================================
                    // STEP 4: ASSIGN TEMP PERSONALITY PROFILE
                    // ============================================================

                    var (profileSuccess, assignedType) =
                        await _personalityService.AssignTemporaryProfileAsync(child.ChildID);

                    if (!profileSuccess || assignedType == null)
                    {
                        throw new InvalidOperationException(
                            "Failed to assign personality profile");
                    }

                    // ============================================================
                    // STEP 5: SAVE + COMMIT
                    // ============================================================

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    _logger.LogInformation(
                        "Onboarding completed for child ID {ChildID}",
                        child.ChildID);

                    // ============================================================
                    // STEP 6: BUILD RESPONSE
                    // ============================================================

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

        // ============================================================
        // HELPER METHODS
        // ============================================================

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
