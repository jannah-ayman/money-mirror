using MoneyMirror.Core.DTOs.Analysis;

namespace MoneyMirror.Core.Interfaces
{
    public interface IAnalysisService
    {
        Task<(bool success, ChildAnalysisDto? analysis, string errorMessage)>
            GetChildAnalysisAsync(int parentId, int childId);
    }
}