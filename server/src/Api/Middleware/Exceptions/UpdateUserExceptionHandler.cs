using System;

using Application.Exceptions.UserException;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Api.Middleware.Exceptions;

public class UpdateUserExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<UpdateUserExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not UpdateUserException updateUserException)
        {
            return false;
        }

        logger.LogWarning(updateUserException, "Update user request failed");

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = updateUserException,
            ProblemDetails = new ProblemDetails
            {
                Detail = exception.Message,
                Status = StatusCodes.Status400BadRequest
            }
        });
    }
}