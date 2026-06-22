// MoneyMirror.Core/DTOs/Chatbot/ChatbotDtos.cs
namespace MoneyMirror.Core.DTOs.Chatbot
{
    public class ChatMessage
    {
        public string Role { get; set; } // "user" or "assistant"
        public string Content { get; set; }
    }

    public class ChildChatRequestDto
    {
        public string Message { get; set; }
        public List<ChatMessage>? History { get; set; }
    }

    public class ParentChatRequestDto
    {
        public int ChildId { get; set; }
        public string Message { get; set; }
        public List<ChatMessage>? History { get; set; }
    }
    public class ChatbotResponseDto
    {
        public string Response { get; set; }
    }
}