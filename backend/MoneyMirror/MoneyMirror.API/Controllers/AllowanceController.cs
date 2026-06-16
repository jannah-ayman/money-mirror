using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyMirror.Core.DTOs.Allowance;
using MoneyMirror.Core.DTOs.Common;
using MoneyMirror.Core.Interfaces;

namespace MoneyMirror.API.Controllers
{
    /// <summary>
    /// Controller handling allowance, bonus, and balance management endpoints.
    /// Routes: /api/allowance/*
    /// Provides endpoints for both parents and children to manage/view allowances, bonuses, and transaction history.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AllowanceController : ControllerBase
    {
        private readonly IAllowanceService _allowanceService;
        private readonly ILogger<AllowanceController> _logger;

        public AllowanceController(
            IAllowanceService allowanceService,
            ILogger<AllowanceController> logger)
        {
            _allowanceService = allowanceService;
            _logger = logger;
        }

        // ==================== PARENT ENDPOINTS ====================

        /// <summary>
        /// Gets a child's current balance.
        /// GET /api/allowance/{childId}/balance
        /// [Parent only]
        /// </summary>
        [HttpGet("{childId}/balance")]
        [Authorize(Roles = "Parent")]
        public async Task<ActionResult<ApiResponse<BalanceResponseDto>>> GetChildBalance(int childId)
        {
            // Get parent ID from JWT token
            var parentIdClaim = User.FindFirst("ParentId")?.Value;

            if (parentIdClaim == null || !int.TryParse(parentIdClaim, out int parentId))
            {
                return BadRequest(ApiResponse<BalanceResponseDto>.ErrorResponse("Invalid token claims"));
            }

            var (success, balance, errorMessage) = await _allowanceService.GetChildBalanceAsync(parentId, childId);

            if (!success)
            {
                return BadRequest(ApiResponse<BalanceResponseDto>.ErrorResponse(errorMessage));
            }

            return Ok(ApiResponse<BalanceResponseDto>.SuccessResponse(
                balance,
                "Balance retrieved successfully"
            ));
        }

        /// <summary>
        /// Gets the current recurring allowance configuration for a child.
        /// Returns null if no allowance is set up.
        /// GET /api/allowance/{childId}
        /// [Parent only]
        /// </summary>
        [HttpGet("{childId}")]
        [Authorize(Roles = "Parent")]
        public async Task<ActionResult<ApiResponse<AllowanceDetailsDto>>> GetAllowance(int childId)
        {
            var parentIdClaim = User.FindFirst("ParentId")?.Value;

            if (parentIdClaim == null || !int.TryParse(parentIdClaim, out int parentId))
            {
                return BadRequest(ApiResponse<AllowanceDetailsDto>.ErrorResponse("Invalid token claims"));
            }

            var (success, allowance, errorMessage) = await _allowanceService.GetAllowanceAsync(parentId, childId);

            if (!success)
            {
                return BadRequest(ApiResponse<AllowanceDetailsDto>.ErrorResponse(errorMessage));
            }

            // allowance can be null if not set up - that's okay
            string message = allowance == null
                ? "No allowance is currently set up for this child"
                : "Allowance settings retrieved successfully";

            return Ok(ApiResponse<AllowanceDetailsDto>.SuccessResponse(
                allowance,
                message
            ));
        }

        /// <summary>
        /// Creates or updates a recurring allowance schedule for a child.
        /// If child already has an active allowance, it will be replaced.
        /// PUT /api/allowance/{childId}
        /// [Parent only]
        /// </summary>
        [HttpPut("{childId}")]
        [Authorize(Roles = "Parent")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateAllowance(
            int childId,
            [FromBody] UpdateAllowanceDto dto)
        {
            var parentIdClaim = User.FindFirst("ParentId")?.Value;

            if (parentIdClaim == null || !int.TryParse(parentIdClaim, out int parentId))
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Invalid token claims"));
            }

            var (success, message) = await _allowanceService.UpdateAllowanceAsync(parentId, childId, dto);

            if (!success)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(message));
            }

            _logger.LogInformation($"Parent {parentId} updated allowance for child {childId}");

            return Ok(ApiResponse<object>.SuccessResponse(
                null,
                message
            ));
        }

        /// <summary>
        /// Gives a one-time bonus to a child.
        /// Credits immediately to child's balance.
        /// POST /api/allowance/{childId}/bonus
        /// [Parent only]
        /// </summary>
        [HttpPost("{childId}/bonus")]
        [Authorize(Roles = "Parent")]
        public async Task<ActionResult<ApiResponse<object>>> GiveBonus(
            int childId,
            [FromBody] GiveBonusDto dto)
        {
            var parentIdClaim = User.FindFirst("ParentId")?.Value;

            if (parentIdClaim == null || !int.TryParse(parentIdClaim, out int parentId))
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Invalid token claims"));
            }

            var (success, newBalance, errorMessage) = await _allowanceService.GiveBonusAsync(parentId, childId, dto);

            if (!success)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(errorMessage));
            }

            _logger.LogInformation($"Parent {parentId} gave bonus of {dto.Amount} to child {childId}");

            return Ok(ApiResponse<object>.SuccessResponse(
                new { NewBalance = newBalance },
                $"Bonus of {dto.Amount:F2} given successfully!"
            ));
        }

        /// <summary>
        /// Gets transaction history for a child.
        /// Can filter by date range and transaction type.
        /// GET /api/allowance/{childId}/transactions?startDate=2026-01-01&endDate=2026-01-31&type=All
        /// [Parent only]
        /// </summary>
        [HttpGet("{childId}/transactions")]
        [Authorize(Roles = "Parent")]
        public async Task<ActionResult<ApiResponse<TransactionHistoryDto>>> GetTransactionHistory(
            int childId,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string type = "All")
        {
            var parentIdClaim = User.FindFirst("ParentId")?.Value;

            if (parentIdClaim == null || !int.TryParse(parentIdClaim, out int parentId))
            {
                return BadRequest(ApiResponse<TransactionHistoryDto>.ErrorResponse("Invalid token claims"));
            }

            var (success, history, errorMessage) = await _allowanceService.GetTransactionHistoryAsync(
                parentId,
                childId,
                startDate,
                endDate,
                type
            );

            if (!success)
            {
                return BadRequest(ApiResponse<TransactionHistoryDto>.ErrorResponse(errorMessage));
            }

            string message = history.TotalCount == 0
                ? "No transactions found for the specified filters"
                : $"Found {history.TotalCount} transaction(s)";

            return Ok(ApiResponse<TransactionHistoryDto>.SuccessResponse(
                history,
                message
            ));
        }

        // ==================== CHILD ENDPOINTS ====================

        /// <summary>
        /// Gets the logged-in child's current balance.
        /// GET /api/allowance/my-balance
        /// [Child only]
        /// </summary>
        [HttpGet("my-balance")]
        [Authorize(Roles = "Child")]
        public async Task<ActionResult<ApiResponse<BalanceResponseDto>>> GetMyBalance()
        {
            // Get child ID from JWT token
            var childIdClaim = User.FindFirst("ChildId")?.Value;

            if (childIdClaim == null || !int.TryParse(childIdClaim, out int childId))
            {
                return BadRequest(ApiResponse<BalanceResponseDto>.ErrorResponse("Invalid token claims"));
            }

            var (success, balance, errorMessage) = await _allowanceService.GetMyBalanceAsync(childId);

            if (!success)
            {
                return BadRequest(ApiResponse<BalanceResponseDto>.ErrorResponse(errorMessage));
            }

            return Ok(ApiResponse<BalanceResponseDto>.SuccessResponse(
                balance,
                $"You have {balance.CurrentBalance:F2} in your account! 💰"
            ));
        }

        /// <summary>
        /// Gets the logged-in child's transaction history.
        /// GET /api/allowance/my-transactions?startDate=2026-01-01
        /// [Child only]
        /// </summary>
        [HttpGet("my-transactions")]
        [Authorize(Roles = "Child")]
        public async Task<ActionResult<ApiResponse<TransactionHistoryDto>>> GetMyTransactions(
            [FromQuery] DateTime? startDate = null)
        {
            var childIdClaim = User.FindFirst("ChildId")?.Value;

            if (childIdClaim == null || !int.TryParse(childIdClaim, out int childId))
            {
                return BadRequest(ApiResponse<TransactionHistoryDto>.ErrorResponse("Invalid token claims"));
            }

            var (success, history, errorMessage) = await _allowanceService.GetMyTransactionsAsync(childId, startDate);

            if (!success)
            {
                return BadRequest(ApiResponse<TransactionHistoryDto>.ErrorResponse(errorMessage));
            }

            string message = history.TotalCount == 0
                ? "You don't have any transactions yet"
                : $"You have {history.TotalCount} transaction(s)";

            return Ok(ApiResponse<TransactionHistoryDto>.SuccessResponse(
                history,
                message
            ));
        }

        /// <summary>
        /// Gets child-friendly information about their allowance.
        /// Shows when they'll get paid next.
        /// GET /api/allowance/my-allowance
        /// [Child only]
        /// </summary>
        [HttpGet("my-allowance")]
        [Authorize(Roles = "Child")]
        public async Task<ActionResult<ApiResponse<ChildAllowanceInfoDto>>> GetMyAllowanceInfo()
        {
            var childIdClaim = User.FindFirst("ChildId")?.Value;

            if (childIdClaim == null || !int.TryParse(childIdClaim, out int childId))
            {
                return BadRequest(ApiResponse<ChildAllowanceInfoDto>.ErrorResponse("Invalid token claims"));
            }

            var (success, info, errorMessage) = await _allowanceService.GetMyAllowanceInfoAsync(childId);

            if (!success)
            {
                return BadRequest(ApiResponse<ChildAllowanceInfoDto>.ErrorResponse(errorMessage));
            }

            return Ok(ApiResponse<ChildAllowanceInfoDto>.SuccessResponse(
                info,
                info.Message
            ));
        }
    }
}
