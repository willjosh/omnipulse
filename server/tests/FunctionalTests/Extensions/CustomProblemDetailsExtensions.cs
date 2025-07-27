using FluentAssertions;

namespace FunctionalTests.Extensions;

/// <summary>
/// Extension methods for validating <see cref="CustomProblemDetails"/> content.
/// </summary>
public static class CustomProblemDetailsExtensions
{
    /// <summary>
    /// Asserts that validation errors are present for the specified property.
    /// </summary>
    /// <param name="problemDetails">The deserialised error response.</param>
    /// <param name="propertyName">The name of the property to check.</param>
    public static void ShouldHaveValidationErrorFor(this CustomProblemDetails problemDetails, string propertyName)
    {
        problemDetails.Errors.Should().ContainKey(propertyName);
        problemDetails.Errors[propertyName].Should().NotBeNullOrEmpty();
    }
}