using FluentAssertions;
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
        options.Host.Should().Be("localhost");
        options.Port.Should().Be(3001);
        options.GetUrl().Should().Be("http://localhost:3001");
    }
    
    [Fact]
    public void TallyOptions_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var options = new TallyOptions();
        
        // Assert
        options.Host.Should().Be("localhost");
        options.Port.Should().Be(9000);
        options.GetUrl().Should().Be("http://localhost:9000");
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
        options.GetUrl().Should().Be("http://192.168.1.100:8080");
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
        options.GetUrl().Should().Be("http://192.168.1.200:9001");
    }
    
    [Fact]
    public void TallyMcpOptions_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var options = new TallyMcpOptions();
        
        // Assert
        options.Server.Should().NotBeNull();
        options.Tally.Should().NotBeNull();
        options.Server.Host.Should().Be("localhost");
        options.Server.Port.Should().Be(3001);
        options.Tally.Host.Should().Be("localhost");
        options.Tally.Port.Should().Be(9000);
    }
}