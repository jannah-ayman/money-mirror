// MoneyMirror.API/Controllers/ChatbotController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyMirror.Core.DTOs.Chatbot;
using MoneyMirror.Core.DTOs.Common;
using MoneyMirror.Core.Interfaces;

namespace MoneyMirror.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatbotController : ControllerBase
    {
        private readonly IChatbotService _chatbotService;

        public ChatbotController(IChatbotService chatbotService)
        {
            _chatbotService = chatbotService;
        }

        /// POST /api/chatbot/child
        [HttpPost("child")]
        [Authorize(Roles = "Child")]
        public async Task<ActionResult<ApiResponse<ChatbotResponseDto>>> ChildChat(
            [FromBody] ChildChatRequestDto dto)
        {
            var childIdClaim = User.FindFirst("ChildId")?.Value;
            if (childIdClaim == null || !int.TryParse(childIdClaim, out int childId))
                return BadRequest(ApiResponse<ChatbotResponseDto>.ErrorResponse("Invalid token claims"));

            if (string.IsNullOrWhiteSpace(dto.Message))
                return BadRequest(ApiResponse<ChatbotResponseDto>.ErrorResponse("Message is required"));

            var (success, response, errorMessage) =
    await _chatbotService.GetChildReplyAsync(childId, dto);

            if (!success)
                return BadRequest(ApiResponse<ChatbotResponseDto>.ErrorResponse(errorMessage));

            return Ok(ApiResponse<ChatbotResponseDto>.SuccessResponse(response));
        }

        /// POST /api/chatbot/parent
        [HttpPost("parent")]
        [Authorize(Roles = "Parent")]
        public async Task<ActionResult<ApiResponse<ChatbotResponseDto>>> ParentChat(
            [FromBody] ParentChatRequestDto dto)
        {
            var parentIdClaim = User.FindFirst("ParentId")?.Value;
            if (parentIdClaim == null || !int.TryParse(parentIdClaim, out int parentId))
                return BadRequest(ApiResponse<ChatbotResponseDto>.ErrorResponse("Invalid token claims"));

            if (string.IsNullOrWhiteSpace(dto.Message))
                return BadRequest(ApiResponse<ChatbotResponseDto>.ErrorResponse("Message is required"));

            var (success, response, errorMessage) = await _chatbotService.GetParentReplyAsync(parentId, dto);
            
            if (!success)
                return BadRequest(ApiResponse<ChatbotResponseDto>.ErrorResponse(errorMessage));

            return Ok(ApiResponse<ChatbotResponseDto>.SuccessResponse(response));
        }
    }
}