// MoneyMirror.Core/Interfaces/IChatbotService.cs
using MoneyMirror.Core.DTOs.Chatbot;

namespace MoneyMirror.Core.Interfaces
{
    public interface IChatbotService
    {
        // IChatbotService.cs
        Task<(bool success, ChatbotResponseDto? response, string errorMessage)>
            GetChildReplyAsync(int childId, ChildChatRequestDto dto);

        Task<(bool success, ChatbotResponseDto? response, string errorMessage)>
            GetParentReplyAsync(int parentId, ParentChatRequestDto dto);
    }
}