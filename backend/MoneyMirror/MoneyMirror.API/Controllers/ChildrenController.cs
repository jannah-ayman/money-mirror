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

        /// Authenticates a child using their unique 6-character login code.
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
              await _childService.AddExistingChildAsync(parentId, addChildDto.Code, addChildDto.Role);

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

        [HttpDelete("{childId}/unlink")]
        [Authorize(Roles = "Parent")]
        public async Task<ActionResult<ApiResponse<object>>> UnlinkChild(int childId)
        {
            var parentIdClaim = User.FindFirst("ParentId")?.Value;
            if (parentIdClaim == null || !int.TryParse(parentIdClaim, out int parentId))
                return BadRequest(ApiResponse<object>.ErrorResponse("Invalid token claims"));

            var (success, message, errorMessage) = await _childService.UnlinkChildAsync(parentId, childId);

            if (!success)
                return BadRequest(ApiResponse<object>.ErrorResponse(errorMessage));

            _logger.LogInformation("Parent {ParentId} unlinked child {ChildId}", parentId, childId);
            return Ok(ApiResponse<object>.SuccessResponse(null, message));
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

        // ==================== CHILD DASHBOARD & PROFILE ENDPOINTS ====================

        /// <summary>
        /// Gets the logged-in child's profile information.
        /// Shows on "My Profile" screen.
        /// GET /api/children/my-profile
        /// [Child only]
        /// </summary>
        [HttpGet("my-profile")]
        [Authorize(Roles = "Child")]
        public async Task<ActionResult<ApiResponse<ChildProfileResponseDto>>> GetMyProfile()
        {
            // Get child ID from JWT token
            var childIdClaim = User.FindFirst("ChildId")?.Value;

            if (childIdClaim == null || !int.TryParse(childIdClaim, out int childId))
            {
                return BadRequest(ApiResponse<ChildProfileResponseDto>.ErrorResponse("Invalid token claims"));
            }

            var (success, profile, errorMessage) = await _childService.GetMyProfileAsync(childId);

            if (!success)
            {
                return BadRequest(ApiResponse<ChildProfileResponseDto>.ErrorResponse(errorMessage));
            }

            return Ok(ApiResponse<ChildProfileResponseDto>.SuccessResponse(
                profile,
                $"Welcome, {profile.FirstName}!"
            ));
        }

        /// <summary>
        /// Gets the logged-in child's dashboard data.
        /// Shows on main home screen.
        /// GET /api/children/my-dashboard
        /// [Child only]
        /// </summary>
        [HttpGet("my-dashboard")]
        [Authorize(Roles = "Child")]
        public async Task<ActionResult<ApiResponse<ChildDashboardDto>>> GetMyDashboard()
        {
            // Get child ID from JWT token
            var childIdClaim = User.FindFirst("ChildId")?.Value;

            if (childIdClaim == null || !int.TryParse(childIdClaim, out int childId))
            {
                return BadRequest(ApiResponse<ChildDashboardDto>.ErrorResponse("Invalid token claims"));
            }

            var (success, dashboard, errorMessage) = await _childService.GetMyDashboardAsync(childId);

            if (!success)
            {
                return BadRequest(ApiResponse<ChildDashboardDto>.ErrorResponse(errorMessage));
            }

            return Ok(ApiResponse<ChildDashboardDto>.SuccessResponse(
                dashboard,
                $"Hello, {dashboard.FirstName}! 👋"
            ));
        }
        // Add these endpoints to your ChildrenController.cs class

        // ==================== PARENT MANAGEMENT OF CHILDREN ENDPOINTS ====================

        /// <summary>
        /// Updates a child's basic information.
        /// Parent can change first name, last name, and date of birth.
        /// PUT /api/children/{childId}
        /// [Parent only]
        /// </summary>
        [HttpPut("{childId}")]
        [Authorize(Roles = "Parent")]
        public async Task<ActionResult<ApiResponse<UpdateChildResponseDto>>> UpdateChild(
            int childId,
            [FromBody] UpdateChildDto dto)
        {
            // Get parent ID from JWT token
            var parentIdClaim = User.FindFirst("ParentId")?.Value;

            if (parentIdClaim == null || !int.TryParse(parentIdClaim, out int parentId))
            {
                return BadRequest(ApiResponse<UpdateChildResponseDto>.ErrorResponse("Invalid token claims"));
            }

            var (success, updatedChild, errorMessage) = await _childService.UpdateChildAsync(parentId, childId, dto);

            if (!success)
            {
                return BadRequest(ApiResponse<UpdateChildResponseDto>.ErrorResponse(errorMessage));
            }

            _logger.LogInformation($"Parent {parentId} updated child {childId}");

            return Ok(ApiResponse<UpdateChildResponseDto>.SuccessResponse(
                updatedChild,
                $"Child {updatedChild.FirstName} updated successfully!"
            ));
        }

        /// <summary>
        /// Regenerates a new login code for a child.
        /// Old code becomes invalid immediately.
        /// POST /api/children/{childId}/regenerate-code
        /// [Parent only]
        /// </summary>
        [HttpPost("{childId}/regenerate-code")]
        [Authorize(Roles = "Parent")]
        public async Task<ActionResult<ApiResponse<RegenerateCodeResponseDto>>> RegenerateLoginCode(int childId)
        {
            // Get parent ID from JWT token
            var parentIdClaim = User.FindFirst("ParentId")?.Value;

            if (parentIdClaim == null || !int.TryParse(parentIdClaim, out int parentId))
            {
                return BadRequest(ApiResponse<RegenerateCodeResponseDto>.ErrorResponse("Invalid token claims"));
            }

            var (success, codeInfo, errorMessage) = await _childService.RegenerateLoginCodeAsync(parentId, childId);

            if (!success)
            {
                return BadRequest(ApiResponse<RegenerateCodeResponseDto>.ErrorResponse(errorMessage));
            }

            _logger.LogInformation($"Parent {parentId} regenerated code for child {childId}");

            return Ok(ApiResponse<RegenerateCodeResponseDto>.SuccessResponse(
                codeInfo,
                $"New login code generated for {codeInfo.ChildName}. The old code is no longer valid."
            ));
        }

        /// <summary>
        /// Permanently deletes a child and all their data.
        /// This action cannot be undone.
        /// DELETE /api/children/{childId}
        /// [Parent only]
        /// </summary>
        [HttpDelete("{childId}")]
        [Authorize(Roles = "Parent")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteChild(int childId)
        {
            // Get parent ID from JWT token
            var parentIdClaim = User.FindFirst("ParentId")?.Value;

            if (parentIdClaim == null || !int.TryParse(parentIdClaim, out int parentId))
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Invalid token claims"));
            }

            var (success, message, errorMessage) = await _childService.DeleteChildAsync(parentId, childId);

            if (!success)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(errorMessage));
            }

            _logger.LogInformation($"Parent {parentId} permanently deleted child {childId}");

            return Ok(ApiResponse<object>.SuccessResponse(
                null,
                message
            ));
        }
    }
}