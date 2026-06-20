using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyMirror.Core.DTOs.Common;
using MoneyMirror.Core.DTOs.Report;
using MoneyMirror.Core.Interfaces;

namespace MoneyMirror.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Parent")]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        private int? GetParentId() =>
            int.TryParse(User.FindFirst("ParentId")?.Value, out int id) ? id : null;

        [HttpGet("{childId}/summary")]
        public async Task<ActionResult<ApiResponse<SpendingSummaryDto>>> GetSummary(
            int childId,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var parentId = GetParentId();
            if (parentId == null) return BadRequest(ApiResponse<SpendingSummaryDto>.ErrorResponse("Invalid token."));

            var (success, data, error) = await _reportService.GetSpendingSummaryAsync(parentId.Value, childId, startDate, endDate);
            if (!success) return BadRequest(ApiResponse<SpendingSummaryDto>.ErrorResponse(error));

            return Ok(ApiResponse<SpendingSummaryDto>.SuccessResponse(data, "Summary loaded."));
        }

        [HttpGet("{childId}/category-breakdown")]
        public async Task<ActionResult<ApiResponse<CategoryBreakdownDto>>> GetCategoryBreakdown(
            int childId,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var parentId = GetParentId();
            if (parentId == null) return BadRequest(ApiResponse<CategoryBreakdownDto>.ErrorResponse("Invalid token."));

            var (success, data, error) = await _reportService.GetCategoryBreakdownAsync(parentId.Value, childId, startDate, endDate);
            if (!success) return BadRequest(ApiResponse<CategoryBreakdownDto>.ErrorResponse(error));

            return Ok(ApiResponse<CategoryBreakdownDto>.SuccessResponse(data, "Category breakdown loaded."));
        }

        [HttpGet("{childId}/mood-spending")]
        public async Task<ActionResult<ApiResponse<MoodSpendingDto>>> GetMoodSpending(
            int childId,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var parentId = GetParentId();
            if (parentId == null) return BadRequest(ApiResponse<MoodSpendingDto>.ErrorResponse("Invalid token."));

            var (success, data, error) = await _reportService.GetMoodSpendingAsync(parentId.Value, childId, startDate, endDate);
            if (!success) return BadRequest(ApiResponse<MoodSpendingDto>.ErrorResponse(error));

            return Ok(ApiResponse<MoodSpendingDto>.SuccessResponse(data, "Mood spending loaded."));
        }

        [HttpGet("{childId}/time-patterns")]
        public async Task<ActionResult<ApiResponse<TimePatternDto>>> GetTimePatterns(
            int childId,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var parentId = GetParentId();
            if (parentId == null) return BadRequest(ApiResponse<TimePatternDto>.ErrorResponse("Invalid token."));

            var (success, data, error) = await _reportService.GetTimePatternAsync(parentId.Value, childId, startDate, endDate);
            if (!success) return BadRequest(ApiResponse<TimePatternDto>.ErrorResponse(error));

            return Ok(ApiResponse<TimePatternDto>.SuccessResponse(data, "Time patterns loaded."));
        }

        [HttpGet("{childId}/top-categories")]
        public async Task<ActionResult<ApiResponse<TopCategoriesDto>>> GetTopCategories(
            int childId,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var parentId = GetParentId();
            if (parentId == null) return BadRequest(ApiResponse<TopCategoriesDto>.ErrorResponse("Invalid token."));

            var (success, data, error) = await _reportService.GetTopCategoriesAsync(parentId.Value, childId, startDate, endDate);
            if (!success) return BadRequest(ApiResponse<TopCategoriesDto>.ErrorResponse(error));

            return Ok(ApiResponse<TopCategoriesDto>.SuccessResponse(data, "Top categories loaded."));
        }

        [HttpGet("{childId}/balance-history")]
        public async Task<ActionResult<ApiResponse<BalanceHistoryDto>>> GetBalanceHistory(
            int childId,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var parentId = GetParentId();
            if (parentId == null) return BadRequest(ApiResponse<BalanceHistoryDto>.ErrorResponse("Invalid token."));

            var (success, data, error) = await _reportService.GetBalanceHistoryAsync(parentId.Value, childId, startDate, endDate);
            if (!success) return BadRequest(ApiResponse<BalanceHistoryDto>.ErrorResponse(error));

            return Ok(ApiResponse<BalanceHistoryDto>.SuccessResponse(data, "Balance history loaded."));
        }

        [HttpGet("{childId}/goal-report")]
        public async Task<ActionResult<ApiResponse<GoalReportDto>>> GetGoalReport(
            int childId,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var parentId = GetParentId();
            if (parentId == null) return BadRequest(ApiResponse<GoalReportDto>.ErrorResponse("Invalid token."));

            var (success, data, error) = await _reportService.GetGoalReportAsync(parentId.Value, childId, startDate, endDate);
            if (!success) return BadRequest(ApiResponse<GoalReportDto>.ErrorResponse(error));

            return Ok(ApiResponse<GoalReportDto>.SuccessResponse(data, "Goal report loaded."));
        }

    private static readonly HashSet<string> ValidSections = new(StringComparer.OrdinalIgnoreCase)
      {
                        "summary", "category-breakdown", "mood-spending",
                        "time-patterns", "top-categories", "balance-history", "goal-report"
     };

        [HttpPost("{childId}/download-pdf")]
        public async Task<IActionResult> DownloadPdf(int childId, [FromBody] PdfDownloadRequestDto dto)
        {
            var parentId = GetParentId();
            if (parentId == null) return BadRequest(ApiResponse<object>.ErrorResponse("Invalid token."));

            if (dto.Sections == null || !dto.Sections.Any())
                return BadRequest(ApiResponse<object>.ErrorResponse("Please select at least one section."));

            var invalidSections = dto.Sections.Where(s => !ValidSections.Contains(s)).ToList();
            if (invalidSections.Any())
                return BadRequest(ApiResponse<object>.ErrorResponse(
                    $"Invalid section(s): {string.Join(", ", invalidSections)}. Valid sections: {string.Join(", ", ValidSections)}"));

            var (success, pdfBytes, error) = await _reportService.GeneratePdfAsync(parentId.Value, childId, dto);
            if (!success) return BadRequest(ApiResponse<object>.ErrorResponse(error));

            return File(pdfBytes!, "application/pdf", $"report-child-{childId}.pdf");
        }
    }
}