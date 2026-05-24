using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyMirror.Core.DTOs.Common;
using MoneyMirror.Core.DTOs.Quiz;
using MoneyMirror.Core.Interfaces;

namespace MoneyMirror.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Child")]
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _quizService;
        private readonly ILogger<QuizController> _logger;

        public QuizController(IQuizService quizService, ILogger<QuizController> logger)
        {
            _quizService = quizService;
            _logger = logger;
        }

        /// GET /api/quiz/next
        /// Returns the next age-appropriate question, or a status message if done/limited
        [HttpGet("next")]
        public async Task<ActionResult<ApiResponse<NextQuizQuestionDto>>> GetNextQuestion()
        {
            var childIdClaim = User.FindFirst("ChildId")?.Value;
            if (childIdClaim == null || !int.TryParse(childIdClaim, out int childId))
                return BadRequest(ApiResponse<NextQuizQuestionDto>.ErrorResponse("Invalid token claims"));

            var (success, question, message) = await _quizService.GetNextQuestionAsync(childId);

            if (!success)
                return BadRequest(ApiResponse<NextQuizQuestionDto>.ErrorResponse(message));

            // success=true but question=null means daily limit or all done
            return Ok(ApiResponse<NextQuizQuestionDto>.SuccessResponse(question, message));
        }

        /// POST /api/quiz/submit
        /// Saves the child's answer and returns a feedback message
        [HttpPost("submit")]
        public async Task<ActionResult<ApiResponse<SubmitQuizAnswerResponseDto>>> SubmitAnswer(
            [FromBody] SubmitQuizAnswerDto dto)
        {
            var childIdClaim = User.FindFirst("ChildId")?.Value;
            if (childIdClaim == null || !int.TryParse(childIdClaim, out int childId))
                return BadRequest(ApiResponse<SubmitQuizAnswerResponseDto>.ErrorResponse("Invalid token claims"));

            var (success, response, errorMessage) = await _quizService.SubmitAnswerAsync(childId, dto);

            if (!success)
                return BadRequest(ApiResponse<SubmitQuizAnswerResponseDto>.ErrorResponse(errorMessage));

            _logger.LogInformation("Child {ChildId} submitted quiz answer {AnswerId}", childId, dto.AnswerID);

            return Ok(ApiResponse<SubmitQuizAnswerResponseDto>.SuccessResponse(
                response,
                "Answer submitted!"
            ));
        }
    }
}