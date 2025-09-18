using Xunit;

namespace TallyMCP.Tests;

public class TallyXmlSanitizerTests
{
    [Fact]
    public void Sanitize_WithNullInput_ShouldReturnEmpty()
    {
        // Act
        var result = TallyXmlSanitizer.Sanitize(null!);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void Sanitize_WithEmptyInput_ShouldReturnEmpty()
    {
        // Act
        var result = TallyXmlSanitizer.Sanitize("");

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void Sanitize_WithControlCharacter_ShouldRemoveIt()
    {
        // Arrange
        var input = "Some text&#4;with control character";

        // Act
        var result = TallyXmlSanitizer.Sanitize(input);

        // Assert
        Assert.Equal("Some textwith control character", result);
    }

    [Fact]
    public void Sanitize_WithUnescapedAmpersand_ShouldEscapeIt()
    {
        // Arrange
        var input = "Company & Associates";

        // Act
        var result = TallyXmlSanitizer.Sanitize(input);

        // Assert
        Assert.Equal("Company &amp; Associates", result);
    }

    [Fact]
    public void Sanitize_WithAlreadyEscapedEntities_ShouldNotDoubleEscape()
    {
        // Arrange
        var input = "Company &amp; Associates &lt;Test&gt; &quot;Quote&quot; &apos;Apostrophe&apos;";

        // Act
        var result = TallyXmlSanitizer.Sanitize(input);

        // Assert
        Assert.Equal("Company &amp; Associates &lt;Test&gt; &quot;Quote&quot; &apos;Apostrophe&apos;", result);
    }

    [Fact]
    public void Sanitize_WithMixedContent_ShouldSanitizeCorrectly()
    {
        // Arrange
        var input = "Test&#4; & &amp; content with & special chars";

        // Act
        var result = TallyXmlSanitizer.Sanitize(input);

        // Assert
        Assert.Equal("Test &amp; &amp; content with &amp; special chars", result);
    }

    [Fact]
    public void Sanitize_WithOnlyValidContent_ShouldReturnUnchanged()
    {
        // Arrange
        var input = "Normal text without special characters";

        // Act
        var result = TallyXmlSanitizer.Sanitize(input);

        // Assert
        Assert.Equal("Normal text without special characters", result);
    }

    [Theory]
    [InlineData("Test & data", "Test &amp; data")]
    [InlineData("Already &amp; escaped", "Already &amp; escaped")]
    [InlineData("Multiple & ampersands & here", "Multiple &amp; ampersands &amp; here")]
    [InlineData("&#4;Control char", "Control char")]
    [InlineData("", "")]
    public void Sanitize_WithVariousInputs_ShouldProduceExpectedResults(string input, string expected)
    {
        // Act
        var result = TallyXmlSanitizer.Sanitize(input);

        // Assert
        Assert.Equal(expected, result);
    }
}