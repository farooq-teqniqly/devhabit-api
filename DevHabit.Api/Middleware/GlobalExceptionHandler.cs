using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace DevHabit.Api.Middleware
{
  public sealed class GlobalExceptionHandler : IExceptionHandler
  {
    private readonly IProblemDetailsService _problemDetailsService;

    public GlobalExceptionHandler(IProblemDetailsService problemDetailsService)
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
      var statusCode = StatusCodes.Status500InternalServerError;
      httpContext.Response.StatusCode = statusCode;

      var problemDetailsContext = new ProblemDetailsContext
      {
        HttpContext = httpContext,
        Exception = exception,
        ProblemDetails = new ProblemDetails
        {
          Title = "Internal Server Error",
          Detail = "An error occurred while processing your request.",
          Status = statusCode,
          Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
        },
      };

      return await _problemDetailsService
        .TryWriteAsync(problemDetailsContext)
        .ConfigureAwait(false);
    }
  }
}
