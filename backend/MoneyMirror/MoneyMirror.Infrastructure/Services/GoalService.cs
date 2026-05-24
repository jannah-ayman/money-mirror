using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoneyMirror.Core.DTOs.Goals;
using MoneyMirror.Core.Interfaces;
using MoneyMirror.Core.Models;
using MoneyMirror.Infrastructure.Data;

namespace MoneyMirror.Infrastructure.Services
{
    public class GoalService : IGoalService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GoalService> _logger;

        public GoalService(ApplicationDbContext context, ILogger<GoalService> logger)
        {
            _context = context;
            _logger = logger;
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

                _context.SavingsGoals.Add(goal);
                await _context.SaveChangesAsync();

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
                    if (goal.EndDate.HasValue && goal.EndDate.Value < DateTime.UtcNow)
                        return (false, 0m, 0m, "This goal has expired and can no longer receive money.");
                    var child = await _context.Children.FindAsync(childId);
                    if (child == null)
                        return (false, 0m, 0m, "Child not found.");

                    if (dto.Amount <= 0)
                        return (false, child.CurrentBalance, goal.CurrentAmount, "Amount must be greater than zero.");

                    if (child.CurrentBalance < dto.Amount)
                        return (false, child.CurrentBalance, goal.CurrentAmount,
                            $"Insufficient balance. You have {child.CurrentBalance:F2} but tried to add {dto.Amount:F2}.");

                    // Cap at what the goal actually needs
                    decimal remaining = goal.TargetAmount - goal.CurrentAmount;
                    decimal actualAmount = Math.Min(dto.Amount, remaining);

                    child.CurrentBalance -= actualAmount;
                    goal.CurrentAmount += actualAmount;

                    _context.Children.Update(child);

                    // Create transaction record
                    _context.Transactions.Add(new Transaction
                    {
                        Type = "GoalTransfer",
                        Amount = -actualAmount,
                        BalanceAfter = child.CurrentBalance,
                        Description = $"Added to goal: {goal.Title}",
                        TransactionDate = DateTime.UtcNow,
                        ChildID = childId
                    });

                    // Check completion
                    if (goal.CurrentAmount >= goal.TargetAmount)
                    {
                        goal.Status = "Success";

                        // Credit reward if challenge
                        if (goal.IsChallenge && goal.RewardValue.HasValue && goal.RewardValue.Value > 0)
                        {
                            child.CurrentBalance += goal.RewardValue.Value;

                            _context.Transactions.Add(new Transaction
                            {
                                Type = "BonusCredit",
                                Amount = goal.RewardValue.Value,
                                BalanceAfter = child.CurrentBalance,
                                Description = $"Challenge reward: {goal.Title} 🎉",
                                TransactionDate = DateTime.UtcNow,
                                ChildID = childId,
                                ParentID = goal.ParentID
                            });

                            _logger.LogInformation("Challenge reward of {Reward} credited to child {ChildId}", goal.RewardValue.Value, childId);
                        }

                        _logger.LogInformation("Goal {GoalId} completed by child {ChildId}", goalId, childId);
                    }

                    _context.SavingsGoals.Update(goal);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return (true, child.CurrentBalance, goal.CurrentAmount, string.Empty);
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
                    .Where(g => g.Status == "Active"
                             && g.EndDate.HasValue
                             && g.EndDate.Value < now
                             && g.CurrentAmount < g.TargetAmount)
                    .ToListAsync();

                foreach (var goal in expiredGoals)
                {
                    goal.Status = "Failure";
                    failedCount++;
                    _logger.LogInformation("Goal {GoalId} marked as Failure (expired)", goal.GoalID);
                }

                if (failedCount > 0)
                    await _context.SaveChangesAsync();

                _logger.LogInformation("Fail expired goals job: {Count} goal(s) marked as Failure", failedCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in FailExpiredGoalsAsync");
            }

            return failedCount;
        }
    }
}