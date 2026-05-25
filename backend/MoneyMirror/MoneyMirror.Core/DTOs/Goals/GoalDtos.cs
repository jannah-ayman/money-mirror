namespace MoneyMirror.Core.DTOs.Goals
{
    public class CreatePersonalGoalDto
    {
        public string Title { get; set; }
        public decimal TargetAmount { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class CreateChallengeDto
    {
        public string Title { get; set; }
        public decimal TargetAmount { get; set; }
        public DateTime EndDate { get; set; }
        public decimal RewardValue { get; set; }
    }

    public class AddMoneyToGoalDto
    {
        public decimal Amount { get; set; }
    }

    public class GoalResponseDto
    {
        public int GoalID { get; set; }
        public string Title { get; set; }
        public decimal TargetAmount { get; set; }
        public decimal CurrentAmount { get; set; }
        public decimal ProgressPercent { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsChallenge { get; set; }
        public string Status { get; set; }
        public decimal? RewardValue { get; set; }
    }
}