using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EcommerceAPI.Filters;

public sealed class ValidationFilter : IAsyncActionFilter
{
    private readonly IServiceProvider _serviceProvider;

    public ValidationFilter(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.ActionArguments.Count == 0)
        {
            await next();
            return;
        }

        foreach (var (_, value) in context.ActionArguments)
        {
            if (value is null) continue;

            // Resolve IValidator<T> for the runtime type and use the non-generic IValidator API
            var genericValidatorType = typeof(IValidator<>).MakeGenericType(value.GetType());
            var validatorObj = _serviceProvider.GetService(genericValidatorType) as IValidator;
            if (validatorObj is null) continue; // no validator registered for this type

            var validationContext = new ValidationContext<object>(value);
            ValidationResult result = await validatorObj.ValidateAsync(validationContext, CancellationToken.None);

            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    context.ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                var problem = new ValidationProblemDetails(context.ModelState)
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Validation failed",
                    Detail = "One or more validation errors occurred."
                };

                context.Result = new BadRequestObjectResult(problem);
                return;
            }
        }

        await next();
    }
}
