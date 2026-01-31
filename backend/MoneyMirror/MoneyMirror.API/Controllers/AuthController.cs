using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyMirror.Core.DTOs.Auth;
using MoneyMirror.Core.DTOs.Common;
using MoneyMirror.Core.Interfaces;
namespace MoneyMirror.API.Controllers
{
    /// <summary>
    /// Controller handling authentication endpoints.
    /// Routes: /api/auth/*
    /// Provides registration, login, email confirmation, and token refresh.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;
        /// <summary>
        /// Constructor - dependency injection provides services.
        /// </summary>
        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }
        /// <summary>
        /// Registers a new parent account.
        /// POST /api/auth/register
        /// </summary>
        /// <param name="registerDto">Registration data from client</param>
        /// <returns>Success message or error</returns>
        [HttpPost("register")]
        [AllowAnonymous] // Anyone can access this endpoint (not logged in)
        public async Task<ActionResult<ApiResponse<object>>> Register([FromBody] RegisterParentDto registerDto)
        {
            // ModelState automatically validates the DTO based on data annotations
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ApiResponse<object>.ValidationErrorResponse(errors));
            }
            // Call service to register parent
            var (success, message, parentId) = await _authService.RegisterParentAsync(registerDto);
            if (!success)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(message));
            }
            // Return success response
            return Ok(ApiResponse<object>.SuccessResponse(
                new { ParentId = parentId },
                message
            ));
        }
        /// <summary>
        /// Authenticates a parent and returns JWT tokens.
        /// POST /api/auth/login
        /// </summary>
        /// <param name="loginDto">Login credentials from client</param>
        /// <returns>JWT tokens and parent info, or error</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ApiResponse<AuthResponseDto>.ValidationErrorResponse(errors));
            }
            // Call service to authenticate
            var authResponse = await _authService.LoginAsync(loginDto);
            if (authResponse == null)
            {
                // Check if it's because email is not confirmed
                var emailExists = await _authService.EmailExistsAsync(loginDto.Email);

                if (emailExists)
                {
                    // Email exists but login failed - could be wrong password or unconfirmed email
                    // For security, we don't specify which one
                    return Unauthorized(ApiResponse<AuthResponseDto>.ErrorResponse(
                        "Invalid email or password, or email not confirmed"
                    ));
                }
                return Unauthorized(ApiResponse<AuthResponseDto>.ErrorResponse(
                    "Invalid email or password"
                ));
            }
            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(
                authResponse,
                "Login successful"
            ));
        }
        /// <summary>
        /// Confirms a parent's email address.
        /// GET /api/auth/confirm-email?email=xxx&token=xxx
        /// This endpoint is called when user clicks the link in their email.
        /// </summary>
        /// <param name="email">Parent's email address</param>
        /// <param name="token">Email confirmation token</param>
        /// <returns>Success or error message</returns>
        [HttpGet("confirm-email")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<object>>> ConfirmEmail([FromQuery] string email, [FromQuery] string token)
        {
            // Validate parameters
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(
                    "Email and token are required"
                ));
            }
            // Call service to confirm email
            var (success, message) = await _authService.ConfirmEmailAsync(email, token);
            if (!success)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(message));
            }
            return Ok(ApiResponse<object>.SuccessResponse(null, message));
        }
        /// <summary>
        /// Refreshes JWT tokens using a valid refresh token.
        /// POST /api/auth/refresh-token
        /// Called by client when access token expires.
        /// </summary>
        /// <param name="refreshTokenDto">Current access token and refresh token</param>
        /// <returns>New JWT tokens or error</returns>
        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ApiResponse<AuthResponseDto>.ValidationErrorResponse(errors));
            }
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
        /// <summary>
        /// Logs out a parent by revoking their refresh token.
        /// POST /api/auth/logout
        /// Requires authentication (must have valid access token).
        /// </summary>
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
        /// <summary>
        /// Checks if an email is available for registration.
        /// GET /api/auth/check-email?email=xxx
        /// Used by frontend to show real-time validation.
        /// </summary>
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
        /// <summary>
        /// Test endpoint to verify authentication is working.
        /// GET /api/auth/test-protected
        /// Requires valid JWT token in Authorization header.
        /// </summary>
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
    }
}
