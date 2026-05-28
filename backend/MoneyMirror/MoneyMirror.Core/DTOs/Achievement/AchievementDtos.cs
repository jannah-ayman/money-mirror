namespace MoneyMirror.Core.DTOs.Achievement
{
    public class AchievementCategoryDto
    {
        public string Category { get; set; } // "Quiz", "Goal", "Expense"
        public int CurrentCount { get; set; } // child's progress count
        public List<AchievementBadgeDto> Badges { get; set; }
    }

    public class AchievementBadgeDto
    {
        public int AchievementTypeID { get; set; }
        public string Name { get; set; }
        public string? IconURL { get; set; }
        public int Threshold { get; set; }
        public bool IsUnlocked { get; set; }
        public DateTime? EarnedDate { get; set; } // null if not earned
        public int CurrentCount { get; set; }     // child's count toward this badge
    }
}