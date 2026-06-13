using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyMirror.Core.DTOs.Analysis;
using MoneyMirror.Core.DTOs.Common;
using MoneyMirror.Core.Interfaces;

namespace MoneyMirror.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Parent")]
    public class AnalysisController : ControllerBase
    {
        private readonly IAnalysisService _analysisService;
        private readonly ILogger<AnalysisController> _logger;

        public AnalysisController(IAnalysisService analysisService, ILogger<AnalysisController> logger)
        {
            _analysisService = analysisService;
            _logger = logger;
        }

        /// GET /api/analysis/{childId}
        [HttpGet("{childId}")]
        public async Task<ActionResult<ApiResponse<ChildAnalysisDto>>> GetChildAnalysis(int childId)
        {
            var parentIdClaim = User.FindFirst("ParentId")?.Value;
            if (parentIdClaim == null || !int.TryParse(parentIdClaim, out int parentId))
                return BadRequest(ApiResponse<ChildAnalysisDto>.ErrorResponse("Invalid token claims."));

            var (success, analysis, errorMessage) = await _analysisService.GetChildAnalysisAsync(parentId, childId);

            if (!success)
                return BadRequest(ApiResponse<ChildAnalysisDto>.ErrorResponse(errorMessage));

            _logger.LogInformation("Parent {ParentId} loaded analysis for child {ChildId}", parentId, childId);

            return Ok(ApiResponse<ChildAnalysisDto>.SuccessResponse(analysis, "Analysis loaded successfully."));
        }
    }
}