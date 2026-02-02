using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyMirror.Core.DTOs.Child;
using MoneyMirror.Core.DTOs.Common;
using MoneyMirror.Core.Interfaces;

namespace MoneyMirror.API.Controllers
{
    /// Controller handling child account management endpoints.
    /// Routes: /api/children/*
    /// Provides questionnaire completion, profile viewing, and child account operations.
    [ApiController]
    [Route("api/[controller]")]
    public class ChildrenController : ControllerBase
    {
        private readonly IChildService _childService;
        private readonly ILogger<ChildrenController> _logger;

        public ChildrenController(
            IChildService childService,
            ILogger<ChildrenController> logger)
        {
            _childService = childService;
            _logger = logger;
        }

        /// Completes the initial profiling questionnaire for a child.
        /// Saves all answers, assigns personality profile, and generates login code.
        /// POST /api/children/complete-initial-profiling
        /// <param name="questionnaireDto">Questionnaire answers from parent</param>
        /// <returns>Success response with login code and personality profile, or error</returns>
        [HttpPost("complete-initial-profiling")]
        [Authorize] // Parent must be logged in
        public async Task<ActionResult<ApiResponse<QuestionnaireCompletionResponseDto>>> CompleteInitialProfiling(
    [FromBody] CompleteInitialProfilingDto questionnaireDto) // <-- flat DTO, no wrapper
        {
            // Get parent ID from JWT token
            var parentIdClaim = User.FindFirst("ParentId")?.Value;

            if (parentIdClaim == null || !int.TryParse(parentIdClaim, out int parentId))
            {
                return BadRequest(ApiResponse<QuestionnaireCompletionResponseDto>.ErrorResponse(
                    "Invalid token claims"
                ));
            }

            // Call service to process questionnaire
            var (success, response, errorMessage) =
                await _childService.CompleteInitialProfilingAsync(parentId, questionnaireDto);

            if (!success)
            {
                return BadRequest(ApiResponse<QuestionnaireCompletionResponseDto>.ErrorResponse(errorMessage));
            }

            return Ok(ApiResponse<QuestionnaireCompletionResponseDto>.SuccessResponse(
                response,
                "Questionnaire completed successfully! Your child's login code is ready."
            ));
        }

        /// Test endpoint to verify child endpoints are working.
        /// GET /api/children/test
        [HttpGet("test")]
        [Authorize]
        public ActionResult<ApiResponse<object>> Test()
        {
            var parentId = User.FindFirst("ParentId")?.Value;
            var email = User.FindFirst("email")?.Value;

            return Ok(ApiResponse<object>.SuccessResponse(
                new { Message = "Children controller is working!", ParentId = parentId, Email = email },
                "Test successful"
            ));
        }
    }
}
