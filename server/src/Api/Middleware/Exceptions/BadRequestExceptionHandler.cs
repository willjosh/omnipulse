using System;

using Application.Exceptions;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Api.Middleware.Exceptions;

public class BadRequestExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<BadRequestExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not BadRequestException badRequestException)
        {
            return false;
        }

        logger.LogWarning(badRequestException, "Bad request occurred");

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = badRequestException,
            ProblemDetails = new ProblemDetails
            {
                Detail = exception.Message,
                Status = StatusCodes.Status400BadRequest
            }
        });
    }
}