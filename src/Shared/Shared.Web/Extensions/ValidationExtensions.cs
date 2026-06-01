using System.Reflection;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Shared.Web.Extensions;

public static class ValidationExtensions
{
    public static RouteHandlerBuilder WithValidation<TRequest>(
        this RouteHandlerBuilder builder)
        where TRequest : class
    {
        return builder.AddEndpointFilter(async (context, next) =>
        {
            var validator = context.HttpContext.RequestServices
                .GetService<IValidator<TRequest>>();

            if (validator is null)
            {
                return await next(context);
            }

            var argument = context.Arguments
                .OfType<TRequest>()
                .FirstOrDefault();

            if (argument is null)
            {
                return Results.BadRequest("Request body is missing.");
            }

            ValidationResult validationResult =
                await validator.ValidateAsync(argument);

            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(
                    validationResult.ToDictionary());
            }

            return await next(context);
        });
    }

    private static IDictionary<string, string[]> ToDictionary(
        this ValidationResult validationResult)
    {
        return validationResult.Errors
            .GroupBy(x => x.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.ErrorMessage).ToArray());
    }
}
