using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyMirror.Core.DTOs.Common;
using MoneyMirror.Core.DTOs.Expense;
using MoneyMirror.Core.Interfaces;

namespace MoneyMirror.API.Controllers
{
    /// <summary>
    /// Controller handling expense logging and retrieval endpoints.
    /// Routes: /api/expense/*
    /// Provides endpoints for both children and parents.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ExpenseController : ControllerBase
    {
        private readonly IExpenseService _expenseService;
        private readonly ILogger<ExpenseController> _logger;

        public ExpenseController(
            IExpenseService expenseService,
            ILogger<ExpenseController> logger)
        {
            _expenseService = expenseService;
            _logger = logger;
        }

        // ==================== CHILD ENDPOINTS ====================

        /// <summary>
        /// Logs a new expense for the logged-in child.
        /// Validates balance, creates expense, updates balance, creates transaction.
        /// POST /api/expense/log
        /// [Child only]
        /// </summary>
        [HttpPost("log")]
        [Authorize(Roles = "Child")]
        public async Task<ActionResult<ApiResponse<ExpenseResponseDto>>> LogExpense(
            [FromBody] LogExpenseDto dto)
        {
            // Get child ID from JWT token
            var childIdClaim = User.FindFirst("ChildId")?.Value;

            if (childIdClaim == null || !int.TryParse(childIdClaim, out int childId))
            {
                return BadRequest(ApiResponse<ExpenseResponseDto>.ErrorResponse("Invalid token claims"));
            }

            // Call service to log expense
            var (success, expense, newBalance, errorMessage) =
                await _expenseService.LogExpenseAsync(childId, dto);

            if (!success)
            {
                return BadRequest(ApiResponse<ExpenseResponseDto>.ErrorResponse(errorMessage));
            }

            _logger.LogInformation($"Child {childId} logged expense: {dto.ItemName}");

            return Ok(ApiResponse<ExpenseResponseDto>.SuccessResponse(
                expense,
                $"Expense logged! You spent {dto.MoneyAmount:F2}. Your new balance is {newBalance:F2} 💰"
            ));
        }
        /// <summary>
        /// Updates an existing expense for the child.
        /// PUT /api/expense/{expenseId}
        /// [Child only]
        /// </summary>
        [HttpPut("{expenseId}")]
        [Authorize(Roles = "Child")]
        public async Task<ActionResult<ApiResponse<ExpenseResponseDto>>> UpdateExpense(
            int expenseId,
            [FromBody] UpdateExpenseDto dto)
        {
            var childIdClaim = User.FindFirst("ChildId")?.Value;

            if (childIdClaim == null || !int.TryParse(childIdClaim, out int childId))
            {
                return BadRequest(ApiResponse<ExpenseResponseDto>.ErrorResponse("Invalid token claims"));
            }

            var (success, expense, newBalance, errorMessage) =
                await _expenseService.UpdateExpenseAsync(childId, expenseId, dto);

            if (!success)
            {
                return BadRequest(ApiResponse<ExpenseResponseDto>.ErrorResponse(errorMessage));
            }

            _logger.LogInformation($"Child {childId} updated expense {expenseId}");

            return Ok(ApiResponse<ExpenseResponseDto>.SuccessResponse(
                expense,
                $"Expense updated! Your new balance is {newBalance:F2} EGP 💰"
            ));
        }

        /// <summary>
        /// Gets the logged-in child's expense history.
        /// Can filter by date range, category, and mood.
        /// GET /api/expense/my-expenses?startDate=2026-01-01&categoryId=1
        /// [Child only]
        /// </summary>
        [HttpGet("my-expenses")]
        [Authorize(Roles = "Child")]
        public async Task<ActionResult<ApiResponse<ExpenseHistoryDto>>> GetMyExpenses(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int? categoryId = null,
            [FromQuery] int? moodId = null)
        {
            var childIdClaim = User.FindFirst("ChildId")?.Value;

            if (childIdClaim == null || !int.TryParse(childIdClaim, out int childId))
            {
                return BadRequest(ApiResponse<ExpenseHistoryDto>.ErrorResponse("Invalid token claims"));
            }

            var (success, history, errorMessage) = await _expenseService.GetMyExpensesAsync(
                childId, startDate, endDate, categoryId, moodId);

            if (!success)
            {
                return BadRequest(ApiResponse<ExpenseHistoryDto>.ErrorResponse(errorMessage));
            }

            string message = history.TotalCount == 0
                ? "You haven't logged any expenses yet"
                : $"You have {history.TotalCount} expense(s). Total spent: {history.TotalSpent:F2}";

            return Ok(ApiResponse<ExpenseHistoryDto>.SuccessResponse(
                history,
                message
            ));
        }

        // ==================== PARENT ENDPOINTS ====================

        /// <summary>
        /// Gets a child's expense history (parent view).
        /// Can filter by date range, category, and mood.
        /// GET /api/expense/{childId}/expenses?startDate=2026-01-01
        /// [Parent only]
        /// </summary>
        [HttpGet("{childId}/expenses")]
        [Authorize(Roles = "Parent")]
        public async Task<ActionResult<ApiResponse<ExpenseHistoryDto>>> GetChildExpenses(
            int childId,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int? categoryId = null,
            [FromQuery] int? moodId = null)
        {
            // Get parent ID from JWT token
            var parentIdClaim = User.FindFirst("ParentId")?.Value;

            if (parentIdClaim == null || !int.TryParse(parentIdClaim, out int parentId))
            {
                return BadRequest(ApiResponse<ExpenseHistoryDto>.ErrorResponse("Invalid token claims"));
            }

            var (success, history, errorMessage) = await _expenseService.GetChildExpensesAsync(
                parentId, childId, startDate, endDate, categoryId, moodId);

            if (!success)
            {
                return BadRequest(ApiResponse<ExpenseHistoryDto>.ErrorResponse(errorMessage));
            }

            string message = history.TotalCount == 0
                ? "No expenses found for the specified filters"
                : $"Found {history.TotalCount} expense(s). Total spent: {history.TotalSpent:F2}";

            return Ok(ApiResponse<ExpenseHistoryDto>.SuccessResponse(
                history,
                message
            ));
        }

        // ==================== SHARED ENDPOINTS (Both Child and Parent) ====================

        /// <summary>
        /// Gets all available expense categories.
        /// Used for dropdown when logging expenses.
        /// GET /api/expense/categories
        /// [Authenticated users only]
        /// </summary>
        [HttpGet("categories")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<List<CategoryDto>>>> GetCategories()
        {
            var categories = await _expenseService.GetCategoriesAsync();

            return Ok(ApiResponse<List<CategoryDto>>.SuccessResponse(
                categories,
                $"Found {categories.Count} categor{(categories.Count == 1 ? "y" : "ies")}"
            ));
        }

        /// <summary>
        /// Gets all available moods.
        /// Used for mood picker when logging expenses.
        /// GET /api/expense/moods
        /// [Authenticated users only]
        /// </summary>
        [HttpGet("moods")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<List<MoodDto>>>> GetMoods()
        {
            var moods = await _expenseService.GetMoodsAsync();

            return Ok(ApiResponse<List<MoodDto>>.SuccessResponse(
                moods,
                $"Found {moods.Count} mood{(moods.Count == 1 ? "" : "s")}"
            ));
        }
    }
}