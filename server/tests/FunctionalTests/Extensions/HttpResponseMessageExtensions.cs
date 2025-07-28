using System.Net.Http.Json;

namespace FunctionalTests.Extensions;

/// <summary>
/// Extension methods for working with <see cref="HttpResponseMessage"/> in functional tests.
/// </summary>
public static class HttpResponseMessageExtensions
{
    /// <summary>
    /// Deserialises the response content into a <see cref="CustomProblemDetails"/> object.
    /// Throws if the response was successful (i.e. not an error) or if the content cannot be deserialised.
    /// </summary>
    /// <param name="response">The HTTP response message to read from.</param>
    /// <returns>The deserialised <see cref="CustomProblemDetails"/>.</returns>
    internal static async Task<CustomProblemDetails> GetProblemDetailsAsync(this HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
            throw new InvalidOperationException("Expected error response, but got a success status.");

        var problemDetails = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();
        if (problemDetails is null)
            throw new InvalidOperationException($"Response could not be deserialised into {nameof(CustomProblemDetails)}.");

        return problemDetails;
    }
}
