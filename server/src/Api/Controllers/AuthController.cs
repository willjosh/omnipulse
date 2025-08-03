using System;

using Application.Contracts.AuthService;
using Application.Features.Auth.Command;
using Application.Features.Auth.Command.Register;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Api.Controllers;

/// <summary>
/// Controller for authentication endpoints.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Consumes("application/json")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IMediator mediator, ILogger<AuthController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token.
    /// </summary>
    /// <param name="command">Login command containing email and password.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>Authenticated user info and JWT token.</returns>
    /// <response code="200">Login successful. Returns user info and token.</response>
    /// <response code="400">Validation failed.</response>
    /// <response code="401">Invalid credentials.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthUserDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AuthUserDTO>> Login(
        [FromBody] LoginCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(Login)}() - Called for email: {command.Email}");

            var result = await _mediator.Send(command, cancellationToken);

            return Ok(result);
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Registers a new operator.
    /// </summary>
    /// <param name="command">Register operator command containing user details.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The ID of the newly registered operator.</returns>
    /// <response code="201">Operator registered successfully. Returns the user ID.</response>
    /// <response code="400">Validation failed.</response>
    /// <response code="409">Duplicate email.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Guid>> RegisterOperator(
        [FromBody] RegisterOperatorCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"{nameof(RegisterOperator)}() - Called for email: {command.Email}");

            var userId = await _mediator.Send(command, cancellationToken);

            return CreatedAtAction(nameof(RegisterOperator), new { id = userId }, userId);
        }
        catch (Application.Exceptions.BadRequestException ex)
        {
            _logger.LogWarning(ex, "Validation failed for operator registration: {Email}", command.Email);
            return BadRequest(new { error = ex.Message });
        }
        catch (Application.Exceptions.DuplicateEntityException ex)
        {
            _logger.LogWarning(ex, "Duplicate email during operator registration: {Email}", command.Email);
            return Conflict(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during operator registration for email: {Email}", command.Email);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }
}