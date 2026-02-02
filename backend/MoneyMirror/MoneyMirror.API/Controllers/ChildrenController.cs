using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyMirror.Core.DTOs.Child;
using MoneyMirror.Core.DTOs.Common;
using MoneyMirror.Core.Interfaces;

namespace MoneyMirror.API.Controllers
{
    /// Controller handling child account management endpoints.
    /// Routes: /api/children/*
    /// Provides questionnaire completion, authentication, token refresh, logout, profile viewing, and child account operations.
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

        /// <summary>
        /// Completes the initial profiling questionnaire for a child.
        /// Saves all answers, assigns personality profile, and generates login code.
        /// POST /api/children/complete-initial-profiling
        /// </summary>
        /// <param name="questionnaireDto">Questionnaire answers from parent</param>
        /// <returns>Success response with login code and personality profile, or error</returns>
        [HttpPost("complete-initial-profiling")]
        [Authorize(Roles = "Parent")] // Parent must be logged in
        public async Task<ActionResult<ApiResponse<QuestionnaireCompletionResponseDto>>> CompleteInitialProfiling(
            [FromBody] CompleteInitialProfilingDto questionnaireDto)
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

        /// <summary>
        /// Authenticates a child using their unique 6-character login code.
        /// Returns JWT tokens and child profile information.
        /// POST /api/children/login-with-code
        /// </summary>
        /// <param name="loginDto">Login code from child</param>
        /// <returns>JWT tokens and child info, or error</returns>
        [HttpPost("login-with-code")]
        [AllowAnonymous] // Anyone can attempt to login
        public async Task<ActionResult<ApiResponse<ChildAuthResponseDto>>> LoginWithCode(
            [FromBody] ChildLoginDto loginDto)
        {
            // Validation happens automatically via FluentValidationFilter

            // Call service to authenticate child
            var (success, authResponse, errorMessage) =
                await _childService.LoginWithCodeAsync(loginDto.Code);

            if (!success)
            {
                _logger.LogWarning($"Failed child login attempt with code: {loginDto.Code}");
                return Unauthorized(ApiResponse<ChildAuthResponseDto>.ErrorResponse(errorMessage));
            }

            _logger.LogInformation($"Child {authResponse.ChildId} logged in successfully");

            return Ok(ApiResponse<ChildAuthResponseDto>.SuccessResponse(
                authResponse,
                $"Welcome back, {authResponse.ChildFirstName}!"
            ));
        }

        /// Refreshes JWT tokens using a valid refresh token.
        /// Allows child to stay logged in without re-entering code.
        /// POST /api/children/refresh-token
        /// <param name="refreshTokenDto">Current tokens from child</param>
        /// <returns>New JWT tokens or error</returns>
        [HttpPost("refresh-token")]
        [AllowAnonymous] // Anyone can attempt to refresh
        public async Task<ActionResult<ApiResponse<ChildAuthResponseDto>>> RefreshToken(
            [FromBody] ChildRefreshTokenDto refreshTokenDto)
        {
            // Validation happens automatically via FluentValidationFilter

            // Call service to refresh tokens
            var (success, authResponse, errorMessage) =
                await _childService.RefreshTokenAsync(refreshTokenDto);

            if (!success)
            {
                return Unauthorized(ApiResponse<ChildAuthResponseDto>.ErrorResponse(errorMessage));
            }

            _logger.LogInformation($"Child {authResponse.ChildId} refreshed tokens successfully");

            return Ok(ApiResponse<ChildAuthResponseDto>.SuccessResponse(
                authResponse,
                "Tokens refreshed successfully"
            ));
        }

        /// Logs out a child by revoking their refresh token.
        /// POST /api/children/logout
        /// <returns>Success or error message</returns>
        [HttpPost("logout")]
        [Authorize(Roles = "Child")] // Child must be logged in
        public async Task<ActionResult<ApiResponse<object>>> Logout()
        {
            // Get child ID from JWT claims
            var childIdClaim = User.FindFirst("ChildId")?.Value;

            if (childIdClaim == null || !int.TryParse(childIdClaim, out int childId))
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(
                    "Invalid token claims"
                ));
            }

            // Call service to revoke refresh token
            var success = await _childService.RevokeRefreshTokenAsync(childId);

            if (!success)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(
                    "Logout failed"
                ));
            }

            _logger.LogInformation($"Child {childId} logged out successfully");

            return Ok(ApiResponse<object>.SuccessResponse(
                null,
                "Logged out successfully. See you next time!"
            ));
        }

        /// Adds an existing child to the current parent's account using login code.
        /// Supports shared custody - multiple parents can manage the same child.
        /// POST /api/children/add-existing
        /// <param name="addChildDto">Child's login code</param>
        /// <returns>Success message or error</returns>
        [HttpPost("add-existing")]
        [Authorize(Roles = "Parent")] // Parent must be logged in
        public async Task<ActionResult<ApiResponse<object>>> AddExistingChild(
            [FromBody] AddExistingChildDto addChildDto)
        {
            // Get parent ID from JWT token
            var parentIdClaim = User.FindFirst("ParentId")?.Value;

            if (parentIdClaim == null || !int.TryParse(parentIdClaim, out int parentId))
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(
                    "Invalid token claims"
                ));
            }

            // Call service to add child
            var (success, message, errorMessage) =
                await _childService.AddExistingChildAsync(parentId, addChildDto.Code);

            if (!success)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(errorMessage));
            }

            _logger.LogInformation($"Parent {parentId} added existing child with code {addChildDto.Code}");

            return Ok(ApiResponse<object>.SuccessResponse(
                null,
                message
            ));
        }

        /// Gets all children linked to the current parent's account.
        /// Returns basic info needed for "Manage Children" tab.
        /// GET /api/children/my-children
        /// <returns>List of children with basic info</returns>
        [HttpGet("my-children")]
        [Authorize(Roles = "Parent")] // Parent must be logged in
        public async Task<ActionResult<ApiResponse<List<ChildSummaryDto>>>> GetMyChildren()
        {
            // Get parent ID from JWT token
            var parentIdClaim = User.FindFirst("ParentId")?.Value;

            if (parentIdClaim == null || !int.TryParse(parentIdClaim, out int parentId))
            {
                return BadRequest(ApiResponse<List<ChildSummaryDto>>.ErrorResponse(
                    "Invalid token claims"
                ));
            }

            // Call service to get children
            var children = await _childService.GetMyChildrenAsync(parentId);

            return Ok(ApiResponse<List<ChildSummaryDto>>.SuccessResponse(
                children,
                children.Count > 0
                    ? $"Found {children.Count} child(ren)"
                    : "No children found. Add a child to get started!"
            ));
        }

        /// Test endpoint to verify child endpoints are working.
        /// GET /api/children/test
        [HttpGet("test")]
        [Authorize]
        public ActionResult<ApiResponse<object>> Test()
        {
            var role = User.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;
            var childId = User.FindFirst("ChildId")?.Value;
            var parentId = User.FindFirst("ParentId")?.Value;

            return Ok(ApiResponse<object>.SuccessResponse(
                new
                {
                    Message = "Children controller is working!",
                    Role = role,
                    ChildId = childId,
                    ParentId = parentId
                },
                "Test successful"
            ));
        }
    }
}