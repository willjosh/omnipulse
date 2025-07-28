using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Mvc;

namespace FunctionalTests;

/// <summary>
/// Customised version of ASP.NET Core's <see cref="ProblemDetails"/> class.
/// Designed to capture validation errors in a structured way during functional testing.
/// </summary>
public class CustomProblemDetails : ProblemDetails
{
    /// <summary>
    /// Validation errors keyed by property name.
    /// </summary>
    [JsonPropertyName("errors")]
    public Dictionary<string, string[]> Errors { get; set; } = new();
}