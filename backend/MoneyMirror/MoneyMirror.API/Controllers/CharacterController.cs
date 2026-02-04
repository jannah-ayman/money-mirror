using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyMirror.Core.DTOs.Character;
using MoneyMirror.Core.DTOs.Common;
using MoneyMirror.Core.Interfaces;

namespace MoneyMirror.API.Controllers
{
    /// <summary>
    /// Controller handling character selection and state management endpoints.
    /// Routes: /api/character/*
    /// Provides endpoints for character selection, state retrieval, and profile pictures.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CharacterController : ControllerBase
    {
        private readonly ICharacterService _characterService;
        private readonly ILogger<CharacterController> _logger;

        public CharacterController(
            ICharacterService characterService,
            ILogger<CharacterController> logger)
        {
            _characterService = characterService;
            _logger = logger;
        }

        // ==================== PUBLIC ENDPOINTS (NO AUTH) ====================

        /// <summary>
        /// Gets list of all available characters.
        /// Used for character selection screen.
        /// GET /api/character/available
        /// [Public - no auth required for browsing characters]
        /// </summary>
        [HttpGet("available")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<List<CharacterInfoDto>>>> GetAvailableCharacters()
        {
            var characters = await _characterService.GetAvailableCharactersAsync();

            return Ok(ApiResponse<List<CharacterInfoDto>>.SuccessResponse(
                characters,
                $"Found {characters.Count} available characters"
            ));
        }

        // ==================== CHILD ENDPOINTS ====================

        /// <summary>
        /// Selects a character for the logged-in child.
        /// Updates profile picture to character's idle state.
        /// PUT /api/character/select
        /// [Child only]
        /// </summary>
        [HttpPut("select")]
        [Authorize(Roles = "Child")]
        public async Task<ActionResult<ApiResponse<SelectCharacterResponseDto>>> SelectCharacter(
            [FromBody] SelectCharacterDto dto)
        {
            // Get child ID from JWT token
            var childIdClaim = User.FindFirst("ChildId")?.Value;

            if (childIdClaim == null || !int.TryParse(childIdClaim, out int childId))
            {
                return BadRequest(ApiResponse<SelectCharacterResponseDto>.ErrorResponse("Invalid token claims"));
            }

            var (success, response, errorMessage) =
                await _characterService.SelectCharacterAsync(childId, dto);

            if (!success)
            {
                return BadRequest(ApiResponse<SelectCharacterResponseDto>.ErrorResponse(errorMessage));
            }

            _logger.LogInformation($"Child {childId} selected character: {dto.CharacterType}");

            return Ok(ApiResponse<SelectCharacterResponseDto>.SuccessResponse(
                response,
                response.Message
            ));
        }

        /// <summary>
        /// Gets character state and image for current screen context.
        /// Determines appropriate character emotion based on what child is doing.
        /// POST /api/character/state
        /// [Child only]
        /// </summary>
        [HttpPost("state")]
        [Authorize(Roles = "Child")]
        public async Task<ActionResult<ApiResponse<CharacterStateResponseDto>>> GetCharacterState(
            [FromBody] GetCharacterStateDto dto)
        {
            // Get child ID from JWT token
            var childIdClaim = User.FindFirst("ChildId")?.Value;

            if (childIdClaim == null || !int.TryParse(childIdClaim, out int childId))
            {
                return BadRequest(ApiResponse<CharacterStateResponseDto>.ErrorResponse("Invalid token claims"));
            }

            var (success, response, errorMessage) =
                await _characterService.GetCharacterStateAsync(childId, dto);

            if (!success)
            {
                return BadRequest(ApiResponse<CharacterStateResponseDto>.ErrorResponse(errorMessage));
            }

            return Ok(ApiResponse<CharacterStateResponseDto>.SuccessResponse(
                response,
                "Character state retrieved successfully"
            ));
        }

        /// <summary>
        /// Gets the logged-in child's profile picture URL.
        /// Returns idle state of their selected character.
        /// GET /api/character/profile-picture
        /// [Child only]
        /// </summary>
        [HttpGet("profile-picture")]
        [Authorize(Roles = "Child")]
        public async Task<ActionResult<ApiResponse<string>>> GetMyProfilePicture()
        {
            // Get child ID from JWT token
            var childIdClaim = User.FindFirst("ChildId")?.Value;

            if (childIdClaim == null || !int.TryParse(childIdClaim, out int childId))
            {
                return BadRequest(ApiResponse<string>.ErrorResponse("Invalid token claims"));
            }

            var (success, imageUrl, errorMessage) =
                await _characterService.GetProfilePictureUrlAsync(childId);

            if (!success)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse(errorMessage));
            }

            return Ok(ApiResponse<string>.SuccessResponse(
                imageUrl,
                "Profile picture retrieved successfully"
            ));
        }

        // ==================== PARENT ENDPOINTS ====================

        /// <summary>
        /// Gets a child's profile picture URL (parent view).
        /// Used to display child's character in parent dashboard.
        /// GET /api/character/{childId}/profile-picture
        /// [Parent only]
        /// </summary>
        [HttpGet("{childId}/profile-picture")]
        [Authorize(Roles = "Parent")]
        public async Task<ActionResult<ApiResponse<string>>> GetChildProfilePicture(int childId)
        {
            // Get parent ID from JWT token
            var parentIdClaim = User.FindFirst("ParentId")?.Value;

            if (parentIdClaim == null || !int.TryParse(parentIdClaim, out int parentId))
            {
                return BadRequest(ApiResponse<string>.ErrorResponse("Invalid token claims"));
            }

            // TODO: Verify parent-child relationship here
            // For now, we'll allow any parent to view any child's character
            // In production, add authorization check

            var (success, imageUrl, errorMessage) =
                await _characterService.GetProfilePictureUrlAsync(childId);

            if (!success)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse(errorMessage));
            }

            return Ok(ApiResponse<string>.SuccessResponse(
                imageUrl,
                "Profile picture retrieved successfully"
            ));
        }

        // ==================== TEST ENDPOINT ====================

        /// <summary>
        /// Test endpoint to verify character controller is working.
        /// GET /api/character/test
        /// </summary>
        [HttpGet("test")]
        [AllowAnonymous]
        public ActionResult<ApiResponse<object>> Test()
        {
            return Ok(ApiResponse<object>.SuccessResponse(
                new
                {
                    Message = "Character controller is working!",
                    AvailableCharacters = new[] { "Nova", "Luna", "Cosmo", "Aura" },
                    TotalStates = 10
                },
                "Test successful"
            ));
        }
    }
}