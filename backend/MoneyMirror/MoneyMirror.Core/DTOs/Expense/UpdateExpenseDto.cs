namespace MoneyMirror.Core.DTOs.Expense
{
    public class UpdateExpenseDto
    {
        public string? ItemName { get; set; }
        public int CategoryID { get; set; }
        public int MoodID { get; set; }
        public decimal MoneyAmount { get; set; }
        public string? Note { get; set; }
    }
}
