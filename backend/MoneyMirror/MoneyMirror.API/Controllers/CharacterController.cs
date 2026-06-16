
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyMirror.Core.DTOs.Character;
using MoneyMirror.Core.DTOs.Common;
using MoneyMirror.Core.Interfaces;

namespace MoneyMirror.API.Controllers
{
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

        [HttpPut("select")]
        [Authorize(Roles = "Child")]
        public async Task<ActionResult<ApiResponse<SelectCharacterResponseDto>>> SelectCharacter(
            [FromBody] SelectCharacterDto dto)
        {
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

            _logger.LogInformation($"Child {childId} selected character ID: {dto.CharacterID}");

            return Ok(ApiResponse<SelectCharacterResponseDto>.SuccessResponse(
                response,
                response.Message
            ));
        }

        [HttpPost("state")]
        [Authorize(Roles = "Child")]
        public async Task<ActionResult<ApiResponse<CharacterStateResponseDto>>> GetCharacterState(
            [FromBody] GetCharacterStateDto dto)
        {
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

        [HttpGet("profile-picture")]
        [Authorize(Roles = "Child")]
        public async Task<ActionResult<ApiResponse<string>>> GetMyProfilePicture()
        {
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

        [HttpGet("{childId}/profile-picture")]
        [Authorize(Roles = "Parent")]
        public async Task<ActionResult<ApiResponse<string>>> GetChildProfilePicture(int childId)
        {
            var parentIdClaim = User.FindFirst("ParentId")?.Value;

            if (parentIdClaim == null || !int.TryParse(parentIdClaim, out int parentId))
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

    }
}
