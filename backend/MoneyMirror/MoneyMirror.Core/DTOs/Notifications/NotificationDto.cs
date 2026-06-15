namespace MoneyMirror.Core.DTOs.Notifications
{
    public class NotificationDto
    {
        public int NotificationID { get; set; }
        public required string Title { get; set; }
        public required string Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime SentDate { get; set; }
        public string? Link { get; set; }
        public required string DeliveryMethod { get; set; }
    }
}