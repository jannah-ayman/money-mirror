using MoneyMirror.Core.DTOs.Goals;

namespace MoneyMirror.Core.Interfaces
{
    public interface IGoalService
    {
        Task<(bool success, GoalResponseDto? goal, string errorMessage)>
            CreatePersonalGoalAsync(int childId, CreatePersonalGoalDto dto);

        Task<(bool success, GoalResponseDto? goal, string errorMessage)>
            CreateChallengeAsync(int parentId, int childId, CreateChallengeDto dto);

        Task<(bool success, List<GoalResponseDto> goals, string errorMessage)>
            GetMyGoalsAsync(int childId);

        Task<(bool success, List<GoalResponseDto> goals, string errorMessage)>
            GetChildGoalsAsync(int parentId, int childId);

        Task<(bool success, decimal newBalance, decimal newGoalAmount, string errorMessage)>
            AddMoneyToGoalAsync(int childId, int goalId, AddMoneyToGoalDto dto);

        Task<int> FailExpiredGoalsAsync();
        Task<(bool success, string message, string errorMessage)>
    DeletePersonalGoalAsync(int childId, int goalId);

        Task<(bool success, string message, string errorMessage)>
            DeleteChallengeAsync(int parentId, int childId, int goalId);

        Task<(bool success, GoalResponseDto? goal, string errorMessage)>
            EditPersonalGoalAsync(int childId, int goalId, EditPersonalGoalDto dto);

        Task<(bool success, GoalResponseDto? goal, string errorMessage)>
            EditChallengeAsync(int parentId, int childId, int goalId, EditChallengeDto dto);
        Task<(bool success, decimal newBalance, string errorMessage)>
        RefundFailedGoalAsync(int childId, int goalId);
    }

}