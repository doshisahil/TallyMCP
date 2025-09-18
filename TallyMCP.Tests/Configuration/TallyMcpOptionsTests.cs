using TallyMCP.Configuration;
using Xunit;

namespace TallyMCP.Tests.Configuration;

public class TallyMcpOptionsTests
{
    [Fact]
    public void ServerOptions_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var options = new ServerOptions();

        // Assert
        Assert.Equal("localhost", options.Host);
        Assert.Equal(3001, options.Port);
        Assert.Equal("http://localhost:3001", options.GetUrl());
    }

    [Fact]
    public void TallyOptions_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var options = new TallyOptions();

        // Assert
        Assert.Equal("localhost", options.Host);
        Assert.Equal(9000, options.Port);
        Assert.Equal("http://localhost:9000", options.GetUrl());
    }

    [Fact]
    public void ServerOptions_CustomValues_ShouldGenerateCorrectUrl()
    {
        // Arrange & Act
        var options = new ServerOptions
        {
            Host = "192.168.1.100",
            Port = 8080
        };

        // Assert
        Assert.Equal("http://192.168.1.100:8080", options.GetUrl());
    }

    [Fact]
    public void TallyOptions_CustomValues_ShouldGenerateCorrectUrl()
    {
        // Arrange & Act
        var options = new TallyOptions
        {
            Host = "192.168.1.200",
            Port = 9001
        };

        // Assert
        Assert.Equal("http://192.168.1.200:9001", options.GetUrl());
    }

    [Fact]
    public void TallyMcpOptions_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var options = new TallyMcpOptions();

        // Assert
        Assert.NotNull(options.Server);
        Assert.NotNull(options.Tally);
        Assert.Equal("localhost", options.Server.Host);
        Assert.Equal(3001, options.Server.Port);
        Assert.Equal("localhost", options.Tally.Host);
        Assert.Equal(9000, options.Tally.Port);
    }
}