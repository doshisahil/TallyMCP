using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using TallyMCP.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddCommandLine(args);

// Configure options
builder.Services.Configure<TallyMcpOptions>(
    builder.Configuration.GetSection(TallyMcpOptions.SectionName));

// Add logging
builder.Services.AddLogging();

// Add HTTP client for Tally
builder.Services.AddHttpClient<TallyHttpClient>();

// Add MCP server
builder.Services.AddMcpServer()
    .WithHttpTransport()
    .WithToolsFromAssembly()
    .WithPromptsFromAssembly()
    .WithResourcesFromAssembly();;

var app = builder.Build();

// Configure the HTTP request pipeline
app.MapMcp();

// Get configuration and start server
var config = builder.Configuration.GetSection(TallyMcpOptions.SectionName).Get<TallyMcpOptions>() ?? new TallyMcpOptions();
var serverUrl = config.Server.GetUrl();

app.Logger.LogInformation("Starting TallyMCP server on {ServerUrl}", serverUrl);
app.Logger.LogInformation("Tally endpoint configured at {TallyUrl}", config.Tally.GetUrl());

app.Run(serverUrl);