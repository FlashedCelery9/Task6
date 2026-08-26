using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Task6.Filters;

public class ValidatorFilter
{
    /// <summary>
    /// Універсальний фільтр валідації: перехоплює аргумент дії типу T, валідує його
    /// FluentValidation-валідатором з DI і, якщо не валідно, одразу повертає 400 ValidationProblemDetails.
    /// Контролер бачить лише вже перевірені дані.
    /// </summary>
    public class ValidationFilter<T> : IAsyncActionFilter where T : class
    {
        private readonly IValidator<T> _validator;

        public ValidationFilter(IValidator<T> validator) => _validator = validator;

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var arg = context.ActionArguments.Values.OfType<T>().FirstOrDefault();

            if (arg is not null)
            {
                var result = await _validator.ValidateAsync(arg);

                if (!result.IsValid)
                {
                    var errors = result.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                    context.Result = new BadRequestObjectResult(
                        new ValidationProblemDetails(errors)
                        {
                            Status = StatusCodes.Status400BadRequest,
                            Title = "Validation Error",
                        });

                    return;
                }
            }

            await next(); // валідно — пускаємо в екшен
        }
    }
 
}