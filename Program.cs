using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModelContextProtocol.Server;
using TallyMCP.Configuration;
using System.ComponentModel.DataAnnotations;

var builder = WebApplication.CreateBuilder(args);

// Add configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddCommandLine(args);

// Configure options with validation
builder.Services.Configure<TallyMcpOptions>(
    builder.Configuration.GetSection(TallyMcpOptions.SectionName));

// Add options validation
builder.Services.AddSingleton<IValidateOptions<TallyMcpOptions>, TallyMcpOptionsValidation>();

// Add logging
builder.Services.AddLogging();

// Add HTTP client for Tally
builder.Services.AddHttpClient<TallyHttpClient>();

// Add MCP server
builder.Services.AddMcpServer()
    .WithHttpTransport()
    .WithToolsFromAssembly()
    .WithPromptsFromAssembly()
    .WithResourcesFromAssembly();

var app = builder.Build();

// Validate configuration on startup
try
{
    var optionsSnapshot = app.Services.GetRequiredService<IOptionsSnapshot<TallyMcpOptions>>();
    var config = optionsSnapshot.Value;
    
    var validationResult = TallyMcpOptionsValidator.ValidateOptions(config);
    if (validationResult != null)
    {
        app.Logger.LogCritical("Configuration validation failed: {ValidationError}", validationResult.ErrorMessage);
        return 1;
    }
    
    app.Logger.LogInformation("Configuration validated successfully");
    app.Logger.LogInformation("Starting TallyMCP server on {ServerUrl}", config.Server.GetUrl());
    app.Logger.LogInformation("Tally endpoint configured at {TallyUrl}", config.Tally.GetUrl());

    // Configure the HTTP request pipeline
    app.MapMcp();

    await app.RunAsync(config.Server.GetUrl());
    return 0;
}
catch (OptionsValidationException ex)
{
    app.Logger.LogCritical("Configuration validation failed: {ValidationErrors}", 
        string.Join(", ", ex.Failures));
    return 1;
}
catch (Exception ex)
{
    app.Logger.LogCritical(ex, "Application startup failed");
    return 1;
}

// Options validation implementation
public class TallyMcpOptionsValidation : IValidateOptions<TallyMcpOptions>
{
    public ValidateOptionsResult Validate(string? name, TallyMcpOptions options)
    {
        var validationResult = TallyMcpOptionsValidator.ValidateOptions(options);
        
        if (validationResult != null)
        {
            return ValidateOptionsResult.Fail(validationResult.ErrorMessage ?? "Configuration validation failed");
        }
        
        return ValidateOptionsResult.Success;
    }
}