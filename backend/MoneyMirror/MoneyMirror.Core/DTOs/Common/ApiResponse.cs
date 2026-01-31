namespace MoneyMirror.Core.DTOs.Common
{
    /// <summary>
    /// Generic wrapper for all API responses.
    /// Provides consistent structure for success and error responses.
    /// Makes it easier for frontend to handle responses uniformly.
    /// </summary>
    /// <typeparam name="T">Type of data being returned (e.g., AuthResponseDto, List<Child>)</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Indicates whether the request was successful.
        /// True = success, False = error occurred
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Human-readable message about the result.
        /// Examples:
        /// - Success: "Registration successful. Please check your email to confirm your account."
        /// - Error: "Email is already registered."
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The actual data being returned.
        /// Null if Success = false (error occurred).
        /// Type varies based on endpoint (AuthResponseDto, Child, List<Expense>, etc.)
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// List of validation errors (if any).
        /// Populated when request data fails validation.
        /// Example: ["Email is required", "Password must be at least 8 characters"]
        /// Null or empty if no validation errors.
        /// </summary>
        public List<string>? Errors { get; set; }

        // ==================== STATIC FACTORY METHODS ====================
        // These make it easy to create responses with consistent formatting

        /// <summary>
        /// Create a successful response with data.
        /// </summary>
        /// <param name="data">The data to return</param>
        /// <param name="message">Success message</param>
        /// <returns>ApiResponse with Success = true</returns>
        public static ApiResponse<T> SuccessResponse(T data, string message = "Operation successful")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data,
                Errors = null
            };
        }

        /// <summary>
        /// Create an error response without data.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="errors">Optional list of specific errors</param>
        /// <returns>ApiResponse with Success = false</returns>
        public static ApiResponse<T> ErrorResponse(string message, List<string>? errors = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Data = default,
                Errors = errors
            };
        }

        /// <summary>
        /// Create a validation error response.
        /// Used when input validation fails.
        /// </summary>
        /// <param name="errors">List of validation error messages</param>
        /// <returns>ApiResponse with Success = false</returns>
        public static ApiResponse<T> ValidationErrorResponse(List<string> errors)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = "Validation failed",
                Data = default,
                Errors = errors
            };
        }
    }
}
