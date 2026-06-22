namespace MoneyMirror.Core.DTOs.Common
{
   
    /// Data transfer object for the Contact Us request.
  
    public class ContactUsRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
