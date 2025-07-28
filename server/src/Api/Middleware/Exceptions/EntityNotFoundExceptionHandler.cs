using System;

using Application.Exceptions;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Api.Middleware.Exceptions;

public class EntityNotFoundExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<EntityNotFoundExceptionHandler> logger
) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not EntityNotFoundException entityNotFoundException)
        {
            return false;
        }

        logger.LogWarning(entityNotFoundException, "Entity not found");

        httpContext.Response.StatusCode = StatusCodes.Status404NotFound;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = entityNotFoundException,
            ProblemDetails = new ProblemDetails
            {
                Detail = exception.Message,
                Status = StatusCodes.Status404NotFound
            }
        });
    }
}