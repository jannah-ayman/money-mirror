using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyMirror.Core.DTOs.Common;
using MoneyMirror.Core.DTOs.Insight;
using MoneyMirror.Core.Interfaces;

namespace MoneyMirror.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InsightController : ControllerBase
    {
        private readonly IInsightService _insightService;
        private readonly ILogger<InsightController> _logger;

        public InsightController(IInsightService insightService, ILogger<InsightController> logger)
        {
            _insightService = insightService;
            _logger = logger;
        }

        /// GET /api/insight/{childId}/key-insights
        /// [Parent only]
        [HttpGet("{childId}/key-insights")]
        [Authorize(Roles = "Parent")]
        public async Task<ActionResult<ApiResponse<KeyInsightsResponseDto>>> GetKeyInsights(int childId)
        {
            var parentIdClaim = User.FindFirst("ParentId")?.Value;
            if (parentIdClaim == null || !int.TryParse(parentIdClaim, out int parentId))
                return BadRequest(ApiResponse<KeyInsightsResponseDto>.ErrorResponse("Invalid token claims"));

            var (success, response, errorMessage) = await _insightService.GetKeyInsightsAsync(parentId, childId);

            if (!success)
                return BadRequest(ApiResponse<KeyInsightsResponseDto>.ErrorResponse(errorMessage));

            string message = response.HasData
                ? "Insights retrieved successfully"
                : response.EmptyStateMessage!;

            return Ok(ApiResponse<KeyInsightsResponseDto>.SuccessResponse(response, message));
        }

        /// GET /api/insight/{childId}/fun-facts
        /// [Child only]
        [HttpGet("{childId}/fun-facts")]
        [Authorize(Roles = "Child")]
        public async Task<ActionResult<ApiResponse<FunFactsResponseDto>>> GetFunFacts(int childId)
        {
            var childIdClaim = User.FindFirst("ChildId")?.Value;
            if (childIdClaim == null || !int.TryParse(childIdClaim, out int tokenChildId))
                return BadRequest(ApiResponse<FunFactsResponseDto>.ErrorResponse("Invalid token claims"));

            if (tokenChildId != childId)
                return BadRequest(ApiResponse<FunFactsResponseDto>.ErrorResponse("You can only view your own fun facts"));

            var (success, response, errorMessage) = await _insightService.GetFunFactsAsync(childId);

            if (!success)
                return BadRequest(ApiResponse<FunFactsResponseDto>.ErrorResponse(errorMessage));

            return Ok(ApiResponse<FunFactsResponseDto>.SuccessResponse(response, "Fun facts retrieved successfully! 🎉"));
        }
    }
}