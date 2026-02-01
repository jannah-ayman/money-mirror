using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MoneyMirror.API.Filters
{
    /// <summary>
    /// Action filter that automatically validates request DTOs using FluentValidation.
    /// Integrates FluentValidation with ASP.NET Core's model validation pipeline.
    /// Returns 400 Bad Request with validation errors if validation fails.
    /// </summary>
    public class FluentValidationFilter : IAsyncActionFilter
    {
        private readonly IServiceProvider _serviceProvider;

        public FluentValidationFilter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Loop through all action arguments (parameters)
            foreach (var argument in context.ActionArguments)
            {
                var argumentType = argument.Value?.GetType();
                if (argumentType == null) continue;

                // Try to get a validator for this type
                var validatorType = typeof(IValidator<>).MakeGenericType(argumentType);
                var validator = _serviceProvider.GetService(validatorType) as IValidator;

                if (validator != null)
                {
                    // Create validation context
                    var validationContext = new ValidationContext<object>(argument.Value);

                    // Validate
                    var validationResult = await validator.ValidateAsync(validationContext);

                    if (!validationResult.IsValid)
                    {
                        // Add errors to ModelState
                        foreach (var error in validationResult.Errors)
                        {
                            context.ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                        }
                    }
                }
            }

            // If ModelState is invalid, return 400 Bad Request
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                context.Result = new BadRequestObjectResult(new
                {
                    Success = false,
                    Message = "Validation failed",
                    Data = (object?)null,
                    Errors = errors
                });
                return;
            }

            // Validation passed, continue to controller action
            await next();
        }
    }
}