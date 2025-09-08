using System.ComponentModel.DataAnnotations;

namespace TallyMCP.Configuration;

public static class TallyMcpOptionsValidator
{
    public static ValidationResult? ValidateOptions(TallyMcpOptions options)
    {
        var context = new ValidationContext(options);
        var results = new List<ValidationResult>();

        // Validate Server options
        if (string.IsNullOrWhiteSpace(options.Server.Host))
        {
            return new ValidationResult("Server host cannot be empty", new[] { nameof(options.Server.Host) });
        }

        if (options.Server.Port < 1 || options.Server.Port > 65535)
        {
            return new ValidationResult("Server port must be between 1 and 65535", new[] { nameof(options.Server.Port) });
        }

        // Validate Tally options
        if (string.IsNullOrWhiteSpace(options.Tally.Host))
        {
            return new ValidationResult("Tally host cannot be empty", new[] { nameof(options.Tally.Host) });
        }

        if (options.Tally.Port < 1 || options.Tally.Port > 65535)
        {
            return new ValidationResult("Tally port must be between 1 and 65535", new[] { nameof(options.Tally.Port) });
        }

        return null;
    }
}