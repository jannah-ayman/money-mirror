namespace MoneyMirror.Core.DTOs.Analysis
{
    public class ChildAnalysisDto
    {
        public PersonalityProfileSectionDto PersonalityProfile { get; set; }
        public BehavioralDimensionsDto? BehavioralDimensions { get; set; }
        public List<TriggeredAdviceCardDto> Alerts { get; set; }
        public List<TriggeredAdviceCardDto> Strengths { get; set; }
        public string DataWindowNote { get; set; }
    }

    public class PersonalityProfileSectionDto
    {
        public string ParentName { get; set; }
        public string Description { get; set; }
        public List<string> Traits { get; set; }
        public List<string> StaticRecommendations { get; set; }
    }

    public class BehavioralDimensionsDto
    {
        public decimal ImpulsiveSpender { get; set; }
        public decimal PrudentSaver { get; set; }
        public decimal GoalOrientedPlanner { get; set; }
        public decimal BargainHunter { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
    }

    public class TriggeredAdviceCardDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string DynamicDetail { get; set; }
        public List<string> ActionSteps { get; set; }
        public string Type { get; set; }
        public int Priority { get; set; }
    }
}