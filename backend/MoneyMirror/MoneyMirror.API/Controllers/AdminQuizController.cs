using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyMirror.Core.DTOs.Common;
using MoneyMirror.Core.DTOs.Quiz;
using MoneyMirror.Core.Interfaces;

namespace MoneyMirror.API.Controllers
{
    [ApiController]
    [Route("api/admin/quiz")]
    [AllowAnonymous]
    public class AdminQuizController : ControllerBase
    {
        private readonly IQuizImportService _importService;
        private readonly IConfiguration _configuration;

        public AdminQuizController(IQuizImportService importService, IConfiguration configuration)
        {
            _importService = importService;
            _configuration = configuration;
        }

        [HttpPost("import")]
        public async Task<ActionResult<ApiResponse<QuizImportResultDto>>> Import(
            [FromHeader(Name = "X-Import-Key")] string? importKey)
        {
            var expectedKey = _configuration["QuizImport:ImportKey"];

            if (string.IsNullOrEmpty(expectedKey) || importKey != expectedKey)
                return Unauthorized(ApiResponse<QuizImportResultDto>.ErrorResponse("Invalid or missing import key"));

            var (success, result, errorMessage) = await _importService.ImportFromJsonAsync();

            if (!success)
                return BadRequest(ApiResponse<QuizImportResultDto>.ErrorResponse(errorMessage));

            return Ok(ApiResponse<QuizImportResultDto>.SuccessResponse(
                result,
                $"Imported {result!.NewlyImported} new question(s). {result.AlreadyExisted} already existed."
            ));
        }
    }
}