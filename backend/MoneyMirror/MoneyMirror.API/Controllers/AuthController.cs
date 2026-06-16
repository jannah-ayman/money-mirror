using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyMirror.Core.DTOs.Auth;
using MoneyMirror.Core.DTOs.Common;
using MoneyMirror.Core.Enums;
using MoneyMirror.Core.Interfaces;
using MoneyMirror.Core.DTOs.Parent;

namespace MoneyMirror.API.Controllers
{
    /// Controller handling authentication endpoints.
    /// Routes: /api/auth/*
    /// Provides registration, login, email confirmation, and token refresh.
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        /// Constructor - dependency injection provides services.
        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }
        /// <summary>
        /// Registers a new parent account.
        /// Sends 6-digit confirmation code to email.
        /// POST /api/auth/register
        /// </summary>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<object>>> Register(
            [FromBody] RegisterParentDto registerDto)
        {
            var (success, message, parentId) = await _authService.RegisterParentAsync(registerDto);

            if (!success)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(message));
            }

            return Ok(ApiResponse<object>.SuccessResponse(
                new { ParentId = parentId },
                message
            ));
        }


        /// <summary>
        /// Confirms email using 6-digit code.
        /// Automatically logs in the parent after successful confirmation.
        /// POST /api/auth/confirm-email-with-code
        /// </summary>
        [HttpPost("confirm-email-with-code")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> ConfirmEmailWithCode(
            [FromBody] ConfirmEmailWithCodeDto dto)
        {
            var (success, message, authResponse) =
                await _authService.ConfirmEmailWithCodeAsync(dto);

            if (!success)
            {
                return BadRequest(ApiResponse<AuthResponseDto>.ErrorResponse(message));
            }

            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(
                authResponse,
                message
            ));
        }

        /// <summary>
        /// Resends email confirmation code.
        /// POST /api/auth/resend-confirmation-code
        /// </summary>
        [HttpPost("resend-confirmation-code")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<object>>> ResendConfirmationCode(
            [FromBody] ResendConfirmationCodeDto dto)
        {
            var (success, message) = await _authService.ResendConfirmationCodeAsync(dto);

            if (!success)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(message));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null, message));
        }

        /// Authenticates a parent and returns JWT tokens.
        /// POST /api/auth/login
        /// <param name="loginDto">Login credentials from client</param>
        /// <returns>JWT tokens and parent info, or error</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromBody] LoginDto loginDto)
        {
            // Validation happens automatically via FluentValidationFilter

            var (authResponse, failure) = await _authService.LoginAsync(loginDto);

            if (authResponse == null)
            {
                return failure switch
                {
                    LoginFailureReason.SoftDeletedRecoverable =>
                        Unauthorized(ApiResponse<AuthResponseDto>.ErrorResponse(
                            "This account is deleted but recoverable. Please use the 'Recover Account' option to restore access."
                        )),

                    LoginFailureReason.PermanentlyDeleted =>
                        Unauthorized(ApiResponse<AuthResponseDto>.ErrorResponse(
                            "This account has been permanently deleted and cannot be recovered. Please register with a different email."
                        )),

                    LoginFailureReason.EmailNotConfirmed =>
                        Unauthorized(ApiResponse<AuthResponseDto>.ErrorResponse(
                            "Email not confirmed. Please check your inbox and confirm your email before logging in."
                        )),

                    LoginFailureReason.InvalidCredentials or null =>
                        Unauthorized(ApiResponse<AuthResponseDto>.ErrorResponse(
                            "Invalid email or password"
                        ))
                };
            }

            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(
                authResponse,
                "Login successful"
            ));
        }
        /// Refreshes JWT tokens using a valid refresh token.
        /// POST /api/auth/refresh-token
        /// Called by client when access token expires.
        /// <param name="refreshTokenDto">Current access token and refresh token</param>
        /// <returns>New JWT tokens or error</returns>
        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
        {
            // Validation happens automatically via FluentValidationFilter

            // Call service to refresh tokens
            var authResponse = await _authService.RefreshTokenAsync(refreshTokenDto);

            if (authResponse == null)
            {
                return Unauthorized(ApiResponse<AuthResponseDto>.ErrorResponse(
                    "Invalid or expired refresh token. Please log in again."
                ));
            }

            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(
                authResponse,
                "Token refreshed successfully"
            ));
        }

        /// Logs out a parent by revoking their refresh token.
        /// POST /api/auth/logout
        /// Requires authentication (must have valid access token).
        /// <returns>Success or error message</returns>
        [HttpPost("logout")]
        [Authorize] // Must be logged in to access this endpoint
        public async Task<ActionResult<ApiResponse<object>>> Logout()
        {
            // Get parent ID from JWT claims
            var parentIdClaim = User.FindFirst("ParentId")?.Value;

            if (parentIdClaim == null || !int.TryParse(parentIdClaim, out int parentId))
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(
                    "Invalid token claims"
                ));
            }

            // Call service to revoke refresh token
            var success = await _authService.RevokeRefreshTokenAsync(parentId);

            if (!success)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(
                    "Logout failed"
                ));
            }

            return Ok(ApiResponse<object>.SuccessResponse(
                null,
                "Logged out successfully"
            ));
        }

        /// Checks if an email is available for registration.
        /// GET /api/auth/check-email?email=xxx
        /// Used by frontend to show real-time validation.
        /// <param name="email">Email address to check</param>
        /// <returns>True if available, False if already registered</returns>
        [HttpGet("check-email")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<bool>>> CheckEmail([FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse(
                    "Email is required"
                ));
            }

            var exists = await _authService.EmailExistsAsync(email);

            return Ok(ApiResponse<bool>.SuccessResponse(
                !exists, // Return true if available (doesn't exist)
                exists ? "Email is already registered" : "Email is available"
            ));
        }

        /// Test endpoint to verify authentication is working.
        /// GET /api/auth/test-protected
        /// Requires valid JWT token in Authorization header.
        /// <returns>Success message with parent info</returns>
        [HttpGet("test-protected")]
        [Authorize]
        public ActionResult<ApiResponse<object>> TestProtected()
        {
            var parentId = User.FindFirst("ParentId")?.Value;
            var email = User.FindFirst("email")?.Value;
            var name = User.FindFirst("name")?.Value;

            return Ok(ApiResponse<object>.SuccessResponse(
                new { ParentId = parentId, Email = email, Name = name },
                "You are authenticated!"
            ));
        }

        
        // ==================== PASSWORD RESET ====================

        /// <summary>
        /// Initiates password reset by sending 6-digit code to email.
        /// POST /api/auth/forgot-password
        /// </summary>
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<object>>> ForgotPassword(
            [FromBody] ForgotPasswordDto dto)
        {
            var (success, message) = await _authService.ForgotPasswordAsync(dto);

            // Always return 200 OK for security (prevent email enumeration)
            return Ok(ApiResponse<object>.SuccessResponse(null, message));
        }

        /// <summary>
        /// Resets password using verified code.
        /// Automatically logs in parent after successful reset.
        /// POST /api/auth/reset-password-with-code
        /// </summary>
        [HttpPost("reset-password-with-code")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> ResetPasswordWithCode(
            [FromBody] ResetPasswordWithCodeDto dto)
        {
            var (success, message, authResponse) =
                await _authService.ResetPasswordWithCodeAsync(dto);

            if (!success)
            {
                return BadRequest(ApiResponse<AuthResponseDto>.ErrorResponse(message));
            }

            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(
                authResponse,
                message
            ));
        }
        // ==================== ADD THESE ENDPOINTS TO AuthController.cs ====================
        // Place them at the end of the class, before the closing brace

        /// Updates parent profile information (name, phone).
        /// PUT /api/auth/profile
        /// Requires authentication.
        /// <param name="updateDto">Updated profile data</param>
        /// <returns>Success or error message</returns>
        [HttpPut("profile")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<object>>> UpdateProfile([FromBody] UpdateParentProfileDto updateDto)
        {
            // Get parent ID from JWT token
            var parentIdClaim = User.FindFirst("ParentId")?.Value;

            if (parentIdClaim == null || !int.TryParse(parentIdClaim, out int parentId))
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Invalid token claims"));
            }

            var (success, message) = await _authService.UpdateParentProfileAsync(parentId, updateDto);

            if (!success)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(message));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null, message));
        }
        // ==================== EMAIL CHANGE ====================

        /// <summary>
        /// Initiates email change by sending 6-digit code to NEW email.
        /// Requires current password for security.
        /// POST /api/auth/request-email-change
        /// </summary>
        [HttpPost("request-email-change")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<object>>> RequestEmailChange(
            [FromBody] RequestEmailChangeDto dto)
        {
            var parentIdClaim = User.FindFirst("ParentId")?.Value;

            if (parentIdClaim == null || !int.TryParse(parentIdClaim, out int parentId))
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Invalid token claims"));
            }

            var (success, message) = await _authService.RequestEmailChangeAsync(parentId, dto);

            if (!success)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(message));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null, message));
        }

        /// <summary>
        /// Confirms email change using 6-digit code.
        /// Revokes all tokens - user must log in with new email.
        /// POST /api/auth/confirm-email-change-with-code
        /// </summary>
        [HttpPost("confirm-email-change-with-code")]
        [AllowAnonymous] // User enters code from new email
        public async Task<ActionResult<ApiResponse<object>>> ConfirmEmailChangeWithCode(
            [FromBody] ConfirmEmailChangeWithCodeDto dto)
        {
            var (success, message) =
                await _authService.ConfirmEmailChangeWithCodeAsync(dto);

            if (!success)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(message));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null, message));
        }


        /// <summary>
        /// Soft deletes parent account with 30-day recovery grace period.
        /// DELETE /api/auth/delete-parent-account
        /// Requires authentication and current password.
        /// </summary>
        /// <param name="currentPassword">Parent's password for verification</param>
        /// <returns>Success message with recovery deadline, or error</returns>
        [HttpDelete("delete-parent-account")]  // Changed from "account"
        [Authorize]
        public async Task<ActionResult<ApiResponse<object>>> DeleteAccount([FromBody] string currentPassword)
        {
            // Validate password provided
            if (string.IsNullOrWhiteSpace(currentPassword))
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Current password is required"));
            }

            // Get parent ID from JWT token
            var parentIdClaim = User.FindFirst("ParentId")?.Value;

            if (parentIdClaim == null || !int.TryParse(parentIdClaim, out int parentId))
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Invalid token claims"));
            }

            var (success, message) = await _authService.DeleteParentAccountAsync(parentId, currentPassword);

            if (!success)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(message));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null, message));
        }
        /// <summary>
        /// Recovers a soft-deleted parent account within the 30-day grace period.
        /// POST /api/auth/recover-account
        /// </summary>
        /// <param name="loginDto">Email and password for verification</param>
        /// <returns>Success or error message</returns>
        [HttpPost("recover-account")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<object>>> RecoverAccount([FromBody] LoginDto loginDto)
        {
            var (success, message) = await _authService.RecoverDeletedAccountAsync(
                loginDto.Email,
                loginDto.Password
            );

            if (!success)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(message));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null, message));
        }
        /// <summary>
        /// Gets the logged-in parent's profile information.
        /// Shows current details (email, name, phone, etc.)
        /// GET /api/auth/my-profile
        /// [Parent only]
        /// </summary>
        [HttpGet("my-profile")]
        [Authorize(Roles = "Parent")]
        public async Task<ActionResult<ApiResponse<ParentProfileResponseDto>>> GetMyProfile()
        {
            // Get parent ID from JWT token
            var parentIdClaim = User.FindFirst("ParentId")?.Value;

            if (parentIdClaim == null || !int.TryParse(parentIdClaim, out int parentId))
            {
                return BadRequest(ApiResponse<ParentProfileResponseDto>.ErrorResponse("Invalid token claims"));
            }

            var (success, profile, errorMessage) = await _authService.GetMyProfileAsync(parentId);

            if (!success)
            {
                return BadRequest(ApiResponse<ParentProfileResponseDto>.ErrorResponse(errorMessage));
            }

            return Ok(ApiResponse<ParentProfileResponseDto>.SuccessResponse(
                profile,
                $"Welcome, {profile.FirstName}!"
            ));
        }

        /// <summary>
        /// Gets the parent's main dashboard.
        /// Shows welcome message and quick cards for all children.
        /// GET /api/auth/my-dashboard
        /// [Parent only]
        /// </summary>
        [HttpGet("my-dashboard")]
        [Authorize(Roles = "Parent")]
        public async Task<ActionResult<ApiResponse<ParentDashboardDto>>> GetMyDashboard()
        {
            // Get parent ID from JWT token
            var parentIdClaim = User.FindFirst("ParentId")?.Value;

            if (parentIdClaim == null || !int.TryParse(parentIdClaim, out int parentId))
            {
                return BadRequest(ApiResponse<ParentDashboardDto>.ErrorResponse("Invalid token claims"));
            }

            var (success, dashboard, errorMessage) = await _authService.GetMyDashboardAsync(parentId);

            if (!success)
            {
                return BadRequest(ApiResponse<ParentDashboardDto>.ErrorResponse(errorMessage));
            }

            return Ok(ApiResponse<ParentDashboardDto>.SuccessResponse(
                dashboard,
                $"Hello, {dashboard.FirstName}! 👋"
            ));
        }

        /// <summary>
        /// Gets detailed information for a specific child.
        /// This is called when parent clicks on a child's button on the dashboard.
        /// Shows balance, stats, and allowance info in one call.
        /// GET /api/auth/child/{childId}/card
        /// [Parent only]
        /// </summary>
        [HttpGet("child/{childId}/card")]
        [Authorize(Roles = "Parent")]
        public async Task<ActionResult<ApiResponse<ChildDetailedCardDto>>> GetChildCard(int childId)
        {
            // Get parent ID from JWT token
            var parentIdClaim = User.FindFirst("ParentId")?.Value;

            if (parentIdClaim == null || !int.TryParse(parentIdClaim, out int parentId))
            {
                return BadRequest(ApiResponse<ChildDetailedCardDto>.ErrorResponse("Invalid token claims"));
            }

            var (success, childCard, errorMessage) = await _authService.GetChildDetailedCardAsync(parentId, childId);

            if (!success)
            {
                return BadRequest(ApiResponse<ChildDetailedCardDto>.ErrorResponse(errorMessage));
            }

            return Ok(ApiResponse<ChildDetailedCardDto>.SuccessResponse(
                childCard,
                $"{childCard.FirstName}'s Overview"
            ));
        }
    }
}