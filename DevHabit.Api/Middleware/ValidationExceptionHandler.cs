using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace DevHabit.Api.Middleware
{
  public sealed class ValidationExceptionHandler : IExceptionHandler
  {
    private readonly IProblemDetailsService _problemDetailsService;

    public ValidationExceptionHandler(IProblemDetailsService problemDetailsService)
    {
      ArgumentNullException.ThrowIfNull(problemDetailsService);
      _problemDetailsService = problemDetailsService;
    }

    public async ValueTask<bool> TryHandleAsync(
      HttpContext httpContext,
      Exception exception,
      CancellationToken cancellationToken
    )
    {
      if (exception is not ValidationException validationException)
      {
        return false;
      }

      var statusCode = StatusCodes.Status400BadRequest;

      httpContext.Response.StatusCode = statusCode;

      var problemDetailsContext = new ProblemDetailsContext
      {
        HttpContext = httpContext,
        Exception = exception,
        ProblemDetails = new ProblemDetails
        {
          Title = "Validation Failed",
          Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
          Detail = "There are validation errors.",
          Status = statusCode,
        },
      };

      var errors = validationException
        .Errors.GroupBy(e => e.PropertyName)
        .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

      problemDetailsContext.ProblemDetails.Extensions.Add("errors", errors);

      return await _problemDetailsService
        .TryWriteAsync(problemDetailsContext)
        .ConfigureAwait(false);
    }
  }
}
