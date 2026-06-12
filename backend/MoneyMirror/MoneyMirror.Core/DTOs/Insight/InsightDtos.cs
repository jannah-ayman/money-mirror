namespace MoneyMirror.Core.DTOs.Insight
{
    public class KeyInsightDto
    {
        public string Insight { get; set; }
    }

    public class KeyInsightsResponseDto
    {
        public List<KeyInsightDto> Insights { get; set; } = new();
        public bool HasData { get; set; }
        public string? EmptyStateMessage { get; set; }
    }

    public class FunFactDto
    {
        public string Observation { get; set; }
        public string Tip { get; set; }
    }

    public class FunFactsResponseDto
    {
        public List<FunFactDto> FunFacts { get; set; } = new();
    }
}