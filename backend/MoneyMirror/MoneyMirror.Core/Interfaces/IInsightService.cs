using MoneyMirror.Core.DTOs.Insight;

namespace MoneyMirror.Core.Interfaces
{
    public interface IInsightService
    {
        /// Parent-facing key insights. Verifies parent-child relationship.
        Task<(bool success, KeyInsightsResponseDto? response, string errorMessage)>
            GetKeyInsightsAsync(int parentId, int childId);

        /// Child-facing fun facts (always 4 items).
        Task<(bool success, FunFactsResponseDto? response, string errorMessage)>
            GetFunFactsAsync(int childId);
    }
}