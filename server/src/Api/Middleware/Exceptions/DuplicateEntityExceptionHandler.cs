using System;

using Application.Exceptions;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Api.Middleware.Exceptions;

public class DuplicateEntityExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<DuplicateEntityExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not DuplicateEntityException duplicateEntityException)
        {
            return false;
        }

        logger.LogWarning(duplicateEntityException, "Duplicate entity error occurred");

        httpContext.Response.StatusCode = StatusCodes.Status409Conflict;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = duplicateEntityException,
            ProblemDetails = new ProblemDetails
            {
                Detail = exception.Message,
                Status = StatusCodes.Status409Conflict
            }
        });
    }
}