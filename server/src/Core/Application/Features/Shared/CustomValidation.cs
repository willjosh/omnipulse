using System;

namespace Application.Features.Shared;

public static class CustomValidation
{
    public static bool HaveValidPrecision(decimal? value)
    {
        if (!value.HasValue) return true; // Null is valid

        var decimalValue = value.Value;
        var decimalString = decimalValue.ToString("F99").TrimEnd('0');

        // Check total digits (precision) - max 10
        var totalDigits = decimalString.Replace(".", "").Replace("-", "").Length;
        if (totalDigits > 10) return false;

        // Check decimal places (scale) - max 2
        var decimalIndex = decimalString.IndexOf('.');
        if (decimalIndex >= 0)
        {
            var decimalPlaces = decimalString.Length - decimalIndex - 1;
            if (decimalPlaces > 2) return false;
        }

        return true;
    }
}