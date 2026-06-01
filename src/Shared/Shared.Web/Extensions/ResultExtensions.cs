using BuildingBlocks.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Shared.Web.Extensions;

public static class ResultExtensions
{
    public static Results<Ok<T>, ProblemHttpResult> ToOkResult<T>(this Result<T> result)
    {
        return result.IsError
            ? ToProblemDetails(result.Errors)
            : TypedResults.Ok(result.Value);
    }

    public static ProblemHttpResult ToProblemDetails(this List<AppError> errors) => errors.ToProblemDetails();
    public static ProblemHttpResult ToProblemDetails(this AppError error) => new[] { error }.ToProblemDetails();


    public static ProblemHttpResult ToProblemDetails(this IEnumerable<AppError> errors)
    {
        var firstError = errors.First();

        var problemDetails = new ProblemDetails
        {
            Status = firstError.ToStatusCode(),
            Title = firstError.Type.ToString(),
            Detail = firstError.Description
        };

        foreach (var error in errors)
        {
            problemDetails.Extensions.Add("code", error.Code);
        }

        return TypedResults.Problem(problemDetails);
    }


    public static int ToStatusCode(this AppError error)
    {
        return error.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.Unexpected => StatusCodes.Status500InternalServerError, 
            _ => StatusCodes.Status500InternalServerError
        };
    }
}
