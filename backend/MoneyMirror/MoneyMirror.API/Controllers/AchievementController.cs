using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyMirror.Core.DTOs.Achievement;
using MoneyMirror.Core.DTOs.Common;
using MoneyMirror.Core.Interfaces;

namespace MoneyMirror.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Child")]
    public class AchievementController : ControllerBase
    {
        private readonly IAchievementService _achievementService;

        public AchievementController(IAchievementService achievementService)
        {
            _achievementService = achievementService;
        }

        /// <summary>
        /// Returns all 12 badges grouped by category (Quiz, Goal, Expense).
        /// Each badge shows unlock status, earned date, and child's current progress count.
        /// GET /api/achievement/my-achievements
        /// [Child only]
        /// </summary>
        [HttpGet("my-achievements")]
        public async Task<ActionResult<ApiResponse<List<AchievementCategoryDto>>>> GetMyAchievements()
        {
            var childIdClaim = User.FindFirst("ChildId")?.Value;
            if (childIdClaim == null || !int.TryParse(childIdClaim, out int childId))
                return BadRequest(ApiResponse<List<AchievementCategoryDto>>.ErrorResponse("Invalid token claims"));

            var (success, categories, errorMessage) = await _achievementService.GetMyAchievementsAsync(childId);

            if (!success)
                return BadRequest(ApiResponse<List<AchievementCategoryDto>>.ErrorResponse(errorMessage));

            int totalUnlocked = categories.Sum(c => c.Badges.Count(b => b.IsUnlocked));

            return Ok(ApiResponse<List<AchievementCategoryDto>>.SuccessResponse(
                categories,
                $"You've unlocked {totalUnlocked} out of 12 badges! 🏅"
            ));
        }
    }
}