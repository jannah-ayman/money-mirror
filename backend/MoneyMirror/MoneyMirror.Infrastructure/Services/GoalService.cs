using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoneyMirror.Core.DTOs.Goals;
using MoneyMirror.Core.Interfaces;
using MoneyMirror.Core.Models;
using MoneyMirror.Infrastructure.Data;
using MoneyMirror.Core.Helpers;


namespace MoneyMirror.Infrastructure.Services
{
    public class GoalService : IGoalService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GoalService> _logger;

        private readonly IAchievementService _achievementService;
        private readonly INotificationService _notificationService;

        public GoalService(ApplicationDbContext context, ILogger<GoalService> logger, IAchievementService achievementService, INotificationService notificationService  )
        {
            _context = context;
            _logger = logger;
            _achievementService = achievementService;
            _notificationService = notificationService;
        }

        private async Task<bool> IsParentLinkedToChildAsync(int parentId, int childId) =>
            await _context.ParentChildren.AnyAsync(pc => pc.ParentID == parentId && pc.ChildID == childId);

        private static GoalResponseDto MapToDto(SavingsGoal g) => new()
        {
            GoalID = g.GoalID,
            Title = g.Title,
            TargetAmount = g.TargetAmount,
            CurrentAmount = g.CurrentAmount,
            ProgressPercent = g.TargetAmount > 0
                ? Math.Round((g.CurrentAmount / g.TargetAmount) * 100, 1)
                : 0,
            StartDate = g.StartDate,
            EndDate = g.EndDate,
            IsChallenge = g.IsChallenge,
            Status = g.Status,
            RewardValue = g.RewardValue
        };

        public async Task<(bool success, GoalResponseDto? goal, string errorMessage)>
            CreatePersonalGoalAsync(int childId, CreatePersonalGoalDto dto)
        {
            try
            {
                var activeCount = await _context.SavingsGoals
                    .CountAsync(g => g.ChildID == childId && !g.IsChallenge && g.Status == "Active");

                if (activeCount >= 2)
                    return (false, null, "You can only have 2 active personal goals at a time.");

                var goal = new SavingsGoal
                {
                    Title = dto.Title.Trim(),
                    TargetAmount = dto.TargetAmount,
                    CurrentAmount = 0,
                    EndDate = dto.EndDate,
                    IsChallenge = false,
                    Status = "Active",
                    ChildID = childId,
                    StartDate = DateTime.UtcNow
                };
                var child = await _context.Children.FindAsync(childId);
                if (child == null)
                    return (false, null, "Child not found.");
                _context.SavingsGoals.Add(goal);
                await _context.SaveChangesAsync();
                await _notificationService.NotifyAllParentsOfChildAsync(
                childId,
                "New Goal Created! 🎯",
                $"{child.FName} just created a new savings goal: \"{goal.Title}\".",
                $"/children/{childId}/goals"
            );
              _logger.LogInformation("Child {ChildId} created personal goal: {Title}", childId, goal.Title);

                return (true, MapToDto(goal), string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating personal goal for child {ChildId}", childId);
                return (false, null, "An error occurred while creating your goal.");
            }
        }

        public async Task<(bool success, GoalResponseDto? goal, string errorMessage)>
            CreateChallengeAsync(int parentId, int childId, CreateChallengeDto dto)
        {
            try
            {
                if (!await IsParentLinkedToChildAsync(parentId, childId))
                    return (false, null, "You are not authorized to create challenges for this child.");

                var existingChallenge = await _context.SavingsGoals
                    .AnyAsync(g => g.ChildID == childId && g.ParentID == parentId
                                && g.IsChallenge && g.Status == "Active");

                if (existingChallenge)
                    return (false, null, "You already have an active challenge for this child. Complete or wait for it to end first.");

                var goal = new SavingsGoal
                {
                    Title = dto.Title.Trim(),
                    TargetAmount = dto.TargetAmount,
                    CurrentAmount = 0,
                    EndDate = dto.EndDate,
                    IsChallenge = true,
                    Status = "Active",
                    RewardValue = dto.RewardValue,
                    ChildID = childId,
                    ParentID = parentId,
                    StartDate = DateTime.UtcNow
                };

                _context.SavingsGoals.Add(goal);
                await _context.SaveChangesAsync();
                await _notificationService.NotifyChildAsync(
                    childId,
                    "New Challenge! 🏅",
                    $"Your parent set you a new challenge: \"{goal.Title}\". Can you do it?",
                    $"/goals/{goal.GoalID}"
                );

                _logger.LogInformation("Parent {ParentId} created challenge for child {ChildId}: {Title}", parentId, childId, goal.Title);

                return (true, MapToDto(goal), string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating challenge for child {ChildId}", childId);
                return (false, null, "An error occurred while creating the challenge.");
            }
        }

        public async Task<(bool success, List<GoalResponseDto> goals, string errorMessage)>
            GetMyGoalsAsync(int childId)
        {
            try
            {
                var goals = await _context.SavingsGoals
                    .Where(g => g.ChildID == childId)
                    .OrderByDescending(g => g.Status == "Active")
                    .ThenByDescending(g => g.StartDate)
                    .Select(g => MapToDto(g))
                    .ToListAsync();

                return (true, goals, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting goals for child {ChildId}", childId);
                return (false, new List<GoalResponseDto>(), "An error occurred while retrieving your goals.");
            }
        }

        public async Task<(bool success, List<GoalResponseDto> goals, string errorMessage)>
            GetChildGoalsAsync(int parentId, int childId)
        {
            try
            {
                if (!await IsParentLinkedToChildAsync(parentId, childId))
                    return (false, new List<GoalResponseDto>(), "You are not authorized to view this child's goals.");

                var goals = await _context.SavingsGoals
                    .Where(g => g.ChildID == childId)
                    .OrderByDescending(g => g.Status == "Active")
                    .ThenByDescending(g => g.StartDate)
                    .Select(g => MapToDto(g))
                    .ToListAsync();

                return (true, goals, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting goals for child {ChildId}", childId);
                return (false, new List<GoalResponseDto>(), "An error occurred while retrieving goals.");
            }
        }
        public async Task<(bool success, decimal newBalance, decimal newGoalAmount, string errorMessage)>
            AddMoneyToGoalAsync(int childId, int goalId, AddMoneyToGoalDto dto)
        {
            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var goal = await _context.SavingsGoals
                        .FirstOrDefaultAsync(g => g.GoalID == goalId && g.ChildID == childId);

                    if (goal == null)
                        return (false, 0m, 0m, "Goal not found.");

                    if (goal.Status != "Active")
                        return (false, 0m, 0m, "You can only add money to active goals.");

                    if (goal.EndDate.HasValue && goal.EndDate.Value.Date < DateTimeHelper.EgyptNow.Date)

                        return (false, 0m, 0m, "This goal has expired and can no longer receive money.");

                    var child = await _context.Children.FindAsync(childId);
                    if (child == null)
                        return (false, 0m, 0m, "Child not found.");

                    if (dto.Amount <= 0)
                        return (false, child.CurrentBalance, goal.CurrentAmount, "Amount must be greater than zero.");

                    if (child.CurrentBalance < dto.Amount)
                        return (false, child.CurrentBalance, goal.CurrentAmount,
                            $"Insufficient balance. You have {child.CurrentBalance:F2} but tried to add {dto.Amount:F2}.");

                    decimal remaining = goal.TargetAmount - goal.CurrentAmount;
                    decimal actualAmount = Math.Min(dto.Amount, remaining);

                    child.CurrentBalance -= actualAmount;
                    goal.CurrentAmount += actualAmount;

                    _context.Children.Update(child);

                    _context.Transactions.Add(new Transaction
                    {
                        Type = "GoalTransfer",
                        Amount = -actualAmount,
                        BalanceAfter = child.CurrentBalance,
                        Description = $"Added to goal: {goal.Title}",
                        TransactionDate = DateTimeHelper.EgyptNow,
                        ChildID = childId
                    });

                    bool goalJustCompleted = false;

                    if (goal.CurrentAmount >= goal.TargetAmount)
                    {
                        goal.Status = "Success";
                        goalJustCompleted = true;

                        if (goal.IsChallenge && goal.RewardValue.HasValue && goal.RewardValue.Value > 0)
                        {
                            child.CurrentBalance += goal.RewardValue.Value;

                            _context.Transactions.Add(new Transaction
                            {
                                Type = "BonusCredit",
                                Amount = goal.RewardValue.Value,
                                BalanceAfter = child.CurrentBalance,
                                Description = $"Challenge reward: {goal.Title} 🎉",
                                TransactionDate = DateTimeHelper.EgyptNow,
                                ChildID = childId,
                                ParentID = goal.ParentID
                            });

                            _logger.LogInformation("Challenge reward of {Reward} credited to child {ChildId}", goal.RewardValue.Value, childId);
                        }

                        child.GoalCount++;
                        _context.Children.Update(child);

                        _logger.LogInformation("Goal {GoalId} completed by child {ChildId}", goalId, childId);
                    }

                    _context.SavingsGoals.Update(goal);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    await _notificationService.CheckAndNotifyLowBalanceAsync(childId);
                    string warningMessage = dto.Amount > actualAmount
                        ? $"You added more than needed. Only {actualAmount:F2} EGP was taken from your balance."
                        : string.Empty;

                    if (!goalJustCompleted)
                    {
                        decimal progressPercent = goal.TargetAmount > 0
                            ? Math.Round((goal.CurrentAmount / goal.TargetAmount) * 100, 1)
                            : 0;

                        await _notificationService.NotifyAllParentsOfChildAsync(
                            childId,
                            "Goal Progress! 💪",
                            $"{child.FName} added {actualAmount:F2} EGP to their \"{goal.Title}\" goal. Progress: {progressPercent}%",
                            $"/children/{childId}/goals"
                        );
                    }
                    if (goalJustCompleted)
                        await _achievementService.CheckAndUnlockAsync(childId, "Goal");
                    if (goalJustCompleted)
                    {
                        await _notificationService.NotifyAllParentsOfChildAsync(
                            childId,
                            "Goal Completed! 🏆",
                            $"{child.FName} just completed their \"{goal.Title}\" savings goal!",
                            $"/children/{childId}/goals"
                        );
                    }
                    return (true, child.CurrentBalance, goal.CurrentAmount, warningMessage);

                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error adding money to goal {GoalId}", goalId);
                    return (false, 0m, 0m, "An error occurred while adding money to the goal.");
                }
            });
        }

        public async Task<int> FailExpiredGoalsAsync()
        {
            int failedCount = 0;
            try
            {
                var now = DateTime.UtcNow;
                var expiredGoals = await _context.SavingsGoals
                    .Include(g => g.Child)
                    .Where(g => g.Status == "Active"
                             && g.EndDate.HasValue
                             && g.EndDate.Value < now
                             && g.CurrentAmount < g.TargetAmount)
                    .ToListAsync();

                foreach (var goal in expiredGoals)
                {
                    goal.Status = "Failure";
                    failedCount++;

                    decimal refundAmount = 0;

                    if (goal.CurrentAmount > 0)
                    {
                        var child = goal.Child;
                        refundAmount = goal.CurrentAmount;
                        child.CurrentBalance += refundAmount;
                        goal.CurrentAmount = 0;

                        _context.Transactions.Add(new Transaction
                        {
                            Type = "GoalRefund",
                            Amount = refundAmount,
                            BalanceAfter = child.CurrentBalance,
                            Description = $"Refund from failed {(goal.IsChallenge ? "challenge" : "goal")}: {goal.Title}",
                            TransactionDate = DateTime.UtcNow,
                            ChildID = goal.ChildID,
                            ParentID = goal.ParentID
                        });

                        _context.Children.Update(child);
                        _logger.LogInformation("Auto-refunded failed goal {GoalId} — {Amount} returned to child {ChildId}", goal.GoalID, refundAmount, goal.ChildID);
                    }

                    string childMessage = refundAmount > 0
                        ? $"Your \"{goal.Title}\" goal has expired. {refundAmount:F2} EGP was returned to your balance."
                        : $"Your \"{goal.Title}\" goal has expired.";

                    await _notificationService.NotifyChildAsync(
                        goal.ChildID,
                        "Goal Expired ⏰",
                        childMessage,
                        "/goals"
                    );

                    if (goal.IsChallenge)
                    {
                        string parentMessage = refundAmount > 0
                            ? $"{goal.Child.FName}'s \"{goal.Title}\" goal expired without completion. {refundAmount:F2} EGP was refunded to their balance."
                            : $"{goal.Child.FName}'s \"{goal.Title}\" goal expired without completion.";

                        await _notificationService.NotifyAllParentsOfChildAsync(
                            goal.ChildID,
                            "Goal Expired",
                            parentMessage,
                            $"/children/{goal.ChildID}/goals"
                        );
                    }

                    _logger.LogInformation("Goal {GoalId} marked as Failure (expired)", goal.GoalID);
                }

                if (failedCount > 0)
                    await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in FailExpiredGoalsAsync");
            }
            return failedCount;
        }
        public async Task<(bool success, string message, string errorMessage)>
    DeletePersonalGoalAsync(int childId, int goalId)
        {
            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var goal = await _context.SavingsGoals
                        .FirstOrDefaultAsync(g => g.GoalID == goalId && g.ChildID == childId && !g.IsChallenge);

                    if (goal == null)
                        return (false, string.Empty, "Goal not found.");

                    if (goal.Status != "Active")
                        return (false, string.Empty, "Only active goals can be deleted.");

                    if (goal.CurrentAmount > 0)
                    {
                        var child = await _context.Children.FindAsync(childId);
                        if (child == null)
                            return (false, string.Empty, "Child not found.");

                        child.CurrentBalance += goal.CurrentAmount;
                        _context.Children.Update(child);

                        _context.Transactions.Add(new Transaction
                        {
                            Type = "GoalRefund",
                            Amount = goal.CurrentAmount,
                            BalanceAfter = child.CurrentBalance,
                            Description = $"Refund from deleted goal: {goal.Title}",
                            TransactionDate = DateTime.UtcNow,
                            ChildID = childId
                        });
                    }

                    _context.SavingsGoals.Remove(goal);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    _logger.LogInformation("Child {ChildId} deleted personal goal {GoalId}", childId, goalId);

                    return (true, "Goal deleted successfully.", string.Empty);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error deleting personal goal {GoalId}", goalId);
                    return (false, string.Empty, "An error occurred while deleting the goal.");
                }
            });
        }

        public async Task<(bool success, string message, string errorMessage)>
            DeleteChallengeAsync(int parentId, int childId, int goalId)
        {
            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    if (!await IsParentLinkedToChildAsync(parentId, childId))
                        return (false, string.Empty, "You are not authorized to manage this child's goals.");

                    var goal = await _context.SavingsGoals
                        .FirstOrDefaultAsync(g => g.GoalID == goalId
                                               && g.ChildID == childId
                                               && g.ParentID == parentId
                                               && g.IsChallenge);

                    if (goal == null)
                        return (false, string.Empty, "Challenge not found.");

                    if (goal.Status != "Active")
                        return (false, string.Empty, "Only active challenges can be deleted.");

                    if (goal.CurrentAmount > 0)
                    {
                        var child = await _context.Children.FindAsync(childId);
                        if (child == null)
                            return (false, string.Empty, "Child not found.");

                        child.CurrentBalance += goal.CurrentAmount;
                        _context.Children.Update(child);

                        _context.Transactions.Add(new Transaction
                        {
                            Type = "GoalRefund",
                            Amount = goal.CurrentAmount,
                            BalanceAfter = child.CurrentBalance,
                            Description = $"Refund from deleted challenge: {goal.Title}",
                            TransactionDate = DateTime.UtcNow,
                            ChildID = childId,
                            ParentID = parentId
                        });
                    }
                    var goalTitle = goal.Title;
                    _context.SavingsGoals.Remove(goal);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    await _notificationService.NotifyChildAsync(
                            childId,
                            "Challenge Removed 📢",
                            $"Your parent removed the \"{goalTitle}\" challenge goal.",
                            "/goals"
                        );
                    _logger.LogInformation("Parent {ParentId} deleted challenge {GoalId} for child {ChildId}", parentId, goalId, childId);

                    return (true, "Challenge deleted successfully.", string.Empty);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error deleting challenge {GoalId}", goalId);
                    return (false, string.Empty, "An error occurred while deleting the challenge.");
                }
            });
        }
        public async Task<(bool success, GoalResponseDto? goal, string errorMessage)>
            EditPersonalGoalAsync(int childId, int goalId, EditPersonalGoalDto dto)
        {
            try
            {
                var goal = await _context.SavingsGoals
                    .FirstOrDefaultAsync(g => g.GoalID == goalId && g.ChildID == childId && !g.IsChallenge);

                if (goal == null)
                    return (false, null, "Goal not found.");

                if (goal.Status != "Active")
                    return (false, null, "Only active goals can be edited.");

                goal.Title = dto.Title.Trim();

                _context.SavingsGoals.Update(goal);
                await _context.SaveChangesAsync();

                return (true, MapToDto(goal), string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error editing personal goal {GoalId}", goalId);
                return (false, null, "An error occurred while editing the goal.");
            }
        }
        public async Task<(bool success, GoalResponseDto? goal, string errorMessage)>
            EditChallengeAsync(int parentId, int childId, int goalId, EditChallengeDto dto)
        {
            try
            {
                if (!await IsParentLinkedToChildAsync(parentId, childId))
                    return (false, null, "You are not authorized to manage this child's goals.");

                var goal = await _context.SavingsGoals
                    .FirstOrDefaultAsync(g => g.GoalID == goalId
                                           && g.ChildID == childId
                                           && g.ParentID == parentId
                                           && g.IsChallenge);

                if (goal == null)
                    return (false, null, "Challenge not found.");

                if (goal.Status != "Active")
                    return (false, null, "Only active challenges can be edited.");

                if (dto.EndDate <= DateTime.UtcNow)
                    return (false, null, "End date must be in the future.");

                goal.Title = dto.Title.Trim();
                goal.EndDate = dto.EndDate;

                // TargetAmount and RewardValue only editable if no money deposited yet
                if (goal.CurrentAmount > 0 && (dto.TargetAmount.HasValue || dto.RewardValue.HasValue))
                    return (false, null, "Target amount and reward cannot be changed after the child has started saving.");

                if (goal.CurrentAmount == 0)
                {
                    if (dto.TargetAmount.HasValue)
                        goal.TargetAmount = dto.TargetAmount.Value;

                    if (dto.RewardValue.HasValue)
                        goal.RewardValue = dto.RewardValue.Value;
                }

                _context.SavingsGoals.Update(goal);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Parent {ParentId} edited challenge {GoalId} for child {ChildId}", parentId, goalId, childId);

                return (true, MapToDto(goal), string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error editing challenge {GoalId}", goalId);
                return (false, null, "An error occurred while editing the challenge.");
            }

        }
      

    }
}