using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyMirror.Core.DTOs.Character;
using MoneyMirror.Core.DTOs.Common;
using MoneyMirror.Core.Interfaces;

namespace MoneyMirror.API.Controllers
{
    /// <summary>
    /// Simple controller for character operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CharacterController : ControllerBase
    {
        private readonly ICharacterService _characterService;

        public CharacterController(ICharacterService characterService)
        {
            _characterService = characterService;
        }

        /// <summary>
        /// Gets all available space characters.
        /// GET /api/character
        /// [Public - anyone can see available characters]
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<List<CharacterDto>>>> GetAllCharacters()
        {
            var characters = await _characterService.GetAllCharactersAsync();

            return Ok(ApiResponse<List<CharacterDto>>.SuccessResponse(
                characters,
                $"Found {characters.Count} space characters!"
            ));
        }

        /// <summary>
        /// Selects a character for the logged-in child.
        /// PUT /api/character/select
        /// [Child only]
        /// </summary>
        [HttpPut("select")]
        [Authorize(Roles = "Child")]
        public async Task<ActionResult<ApiResponse<object>>> SelectCharacter(
            [FromBody] SelectCharacterDto dto)
        {
            // Get child ID from JWT token
            var childIdClaim = User.FindFirst("ChildId")?.Value;

            if (childIdClaim == null || !int.TryParse(childIdClaim, out int childId))
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Invalid token"));
            }

            var (success, message) = await _characterService.SelectCharacterAsync(
                childId,
                dto.CharacterID);

            if (!success)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(message));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null, message));
        }

        /// <summary>
        /// Gets character image for current screen.
        /// POST /api/character/image
        /// [Child only]
        /// </summary>
        [HttpPost("image")]
        [Authorize(Roles = "Child")]
        public async Task<ActionResult<ApiResponse<CharacterImageResponseDto>>> GetCharacterImage(
            [FromBody] GetCharacterImageDto dto)
        {
            // Get child ID from JWT token
            var childIdClaim = User.FindFirst("ChildId")?.Value;

            if (childIdClaim == null || !int.TryParse(childIdClaim, out int childId))
            {
                return BadRequest(ApiResponse<CharacterImageResponseDto>.ErrorResponse("Invalid token"));
            }

            var (success, image, errorMessage) = await _characterService.GetCharacterImageAsync(
                childId,
                dto.ScreenContext);

            if (!success)
            {
                return BadRequest(ApiResponse<CharacterImageResponseDto>.ErrorResponse(errorMessage));
            }

            return Ok(ApiResponse<CharacterImageResponseDto>.SuccessResponse(
                image,
                "Character ready!"
            ));
        }
    }
}