using MoneyMirror.Core.DTOs.Allowance;

namespace MoneyMirror.Core.Interfaces;

    public interface IAllowanceService
    {
        // ==================== ALLOWANCE CONFIGURATION ====================


    
        Task<(bool success, string message)> UpdateAllowanceAsync(
            int parentId,
            int childId,
            UpdateAllowanceDto dto);

        Task<(bool success, AllowanceDetailsDto? allowance, string errorMessage)> GetAllowanceAsync(
            int parentId,
            int childId);

        // ==================== BONUS ====================

        Task<(bool success, decimal newBalance, string errorMessage)> GiveBonusAsync(
            int parentId,
            int childId,
            GiveBonusDto dto);

        // ==================== BALANCE (PARENT VIEW) ====================

   
        Task<(bool success, BalanceResponseDto? balance, string errorMessage)> GetChildBalanceAsync(
            int parentId,
            int childId);

        // ==================== BALANCE (CHILD VIEW) ====================


        Task<(bool success, BalanceResponseDto? balance, string errorMessage)> GetMyBalanceAsync(
            int childId);

        // ==================== TRANSACTION HISTORY (PARENT VIEW) ====================

        Task<(bool success, TransactionHistoryDto? history, string errorMessage)> GetTransactionHistoryAsync(
            int parentId,
            int childId,
            DateTime? startDate = null,
            DateTime? endDate = null,
            string type = "All");

        // ==================== TRANSACTION HISTORY (CHILD VIEW) ====================


        Task<(bool success, TransactionHistoryDto? history, string errorMessage)> GetMyTransactionsAsync(
                    int childId,
                    DateTime? startDate = null,
                    int? givenByParentId = null);

        // ==================== CHILD ALLOWANCE INFO ====================

        Task<(bool success, ChildAllowanceInfoDto? info, string errorMessage)> GetMyAllowanceInfoAsync(
            int childId);

        // ==================== BACKGROUND JOB ====================

        Task<int> CreditScheduledAllowancesAsync();
 
        Task<(bool success, decimal newBalance, string errorMessage)> EditBonusAsync(
            int parentId,
            int transactionId,
            EditBonusDto dto);

    }

