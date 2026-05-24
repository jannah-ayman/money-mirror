using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyMirror.Core.DTOs.Common;
using MoneyMirror.Core.DTOs.Goals;
using MoneyMirror.Core.Interfaces;

namespace MoneyMirror.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GoalController : ControllerBase
    {
        private readonly IGoalService _goalService;
        private readonly ILogger<GoalController> _logger;

        public GoalController(IGoalService goalService, ILogger<GoalController> logger)
        {
            _goalService = goalService;
            _logger = logger;
        }

        // ==================== CHILD ENDPOINTS ====================

        /// POST /api/goal/personal
        [HttpPost("personal")]
        [Authorize(Roles = "Child")]
        public async Task<ActionResult<ApiResponse<GoalResponseDto>>> CreatePersonalGoal(
            [FromBody] CreatePersonalGoalDto dto)
        {
            var childIdClaim = User.FindFirst("ChildId")?.Value;
            if (childIdClaim == null || !int.TryParse(childIdClaim, out int childId))
                return BadRequest(ApiResponse<GoalResponseDto>.ErrorResponse("Invalid token claims."));

            var (success, goal, errorMessage) = await _goalService.CreatePersonalGoalAsync(childId, dto);

            if (!success)
                return BadRequest(ApiResponse<GoalResponseDto>.ErrorResponse(errorMessage));

            _logger.LogInformation("Child {ChildId} created personal goal: {Title}", childId, dto.Title);

            return Ok(ApiResponse<GoalResponseDto>.SuccessResponse(goal, "Goal created successfully! 🎯"));
        }

        /// POST /api/goal/{goalId}/add-money
        [HttpPost("{goalId}/add-money")]
        [Authorize(Roles = "Child")]
        public async Task<ActionResult<ApiResponse<object>>> AddMoneyToGoal(
            int goalId,
            [FromBody] AddMoneyToGoalDto dto)
        {
            var childIdClaim = User.FindFirst("ChildId")?.Value;
            if (childIdClaim == null || !int.TryParse(childIdClaim, out int childId))
                return BadRequest(ApiResponse<object>.ErrorResponse("Invalid token claims."));

            var (success, newBalance, newGoalAmount, errorMessage) =
                await _goalService.AddMoneyToGoalAsync(childId, goalId, dto);

            if (!success)
                return BadRequest(ApiResponse<object>.ErrorResponse(errorMessage));

            _logger.LogInformation("Child {ChildId} added {Amount} to goal {GoalId}", childId, dto.Amount, goalId);

            return Ok(ApiResponse<object>.SuccessResponse(
                new { NewBalance = newBalance, NewGoalAmount = newGoalAmount },
                "Money added to goal! Keep it up! 💪"
            ));
        }

        /// GET /api/goal/my-goals
        [HttpGet("my-goals")]
        [Authorize(Roles = "Child")]
        public async Task<ActionResult<ApiResponse<List<GoalResponseDto>>>> GetMyGoals()
        {
            var childIdClaim = User.FindFirst("ChildId")?.Value;
            if (childIdClaim == null || !int.TryParse(childIdClaim, out int childId))
                return BadRequest(ApiResponse<List<GoalResponseDto>>.ErrorResponse("Invalid token claims."));

            var (success, goals, errorMessage) = await _goalService.GetMyGoalsAsync(childId);

            if (!success)
                return BadRequest(ApiResponse<List<GoalResponseDto>>.ErrorResponse(errorMessage));

            string message = goals.Count == 0
                ? "You don't have any goals yet. Create one!"
                : $"You have {goals.Count} goal(s).";

            return Ok(ApiResponse<List<GoalResponseDto>>.SuccessResponse(goals, message));
        }

        // ==================== PARENT ENDPOINTS ====================

        /// POST /api/goal/{childId}/challenge
        [HttpPost("{childId}/challenge")]
        [Authorize(Roles = "Parent")]
        public async Task<ActionResult<ApiResponse<GoalResponseDto>>> CreateChallenge(
            int childId,
            [FromBody] CreateChallengeDto dto)
        {
            var parentIdClaim = User.FindFirst("ParentId")?.Value;
            if (parentIdClaim == null || !int.TryParse(parentIdClaim, out int parentId))
                return BadRequest(ApiResponse<GoalResponseDto>.ErrorResponse("Invalid token claims."));

            var (success, goal, errorMessage) = await _goalService.CreateChallengeAsync(parentId, childId, dto);

            if (!success)
                return BadRequest(ApiResponse<GoalResponseDto>.ErrorResponse(errorMessage));

            _logger.LogInformation("Parent {ParentId} created challenge for child {ChildId}", parentId, childId);

            return Ok(ApiResponse<GoalResponseDto>.SuccessResponse(goal, "Challenge created! Your child will see it now. 🏆"));
        }

        /// GET /api/goal/{childId}/goals
        [HttpGet("{childId}/goals")]
        [Authorize(Roles = "Parent")]
        public async Task<ActionResult<ApiResponse<List<GoalResponseDto>>>> GetChildGoals(int childId)
        {
            var parentIdClaim = User.FindFirst("ParentId")?.Value;
            if (parentIdClaim == null || !int.TryParse(parentIdClaim, out int parentId))
                return BadRequest(ApiResponse<List<GoalResponseDto>>.ErrorResponse("Invalid token claims."));

            var (success, goals, errorMessage) = await _goalService.GetChildGoalsAsync(parentId, childId);

            if (!success)
                return BadRequest(ApiResponse<List<GoalResponseDto>>.ErrorResponse(errorMessage));

            string message = goals.Count == 0
                ? "This child has no goals yet."
                : $"Found {goals.Count} goal(s).";

            return Ok(ApiResponse<List<GoalResponseDto>>.SuccessResponse(goals, message));
        }
    }
}