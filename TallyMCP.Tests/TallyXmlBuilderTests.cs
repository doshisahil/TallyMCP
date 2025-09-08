using FluentAssertions;
using Xunit;

namespace TallyMCP.Tests;

public class TallyXmlBuilderTests
{
    [Fact]
    public void BuildCompanyListRequestXml_ShouldGenerateValidXml()
    {
        // Act
        var result = TallyXmlBuilder.BuildCompanyListRequestXml();
        
        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Contain("<ENVELOPE>");
        result.Should().Contain("<HEADER>");
        result.Should().Contain("<VERSION>1</VERSION>");
        result.Should().Contain("<TALLYREQUEST>Export</TALLYREQUEST>");
        result.Should().Contain("<TYPE>Collection</TYPE>");
        result.Should().Contain("<ID>List of Companies</ID>");
        result.Should().Contain("<COLLECTION NAME=\"List of Companies\"");
        result.Should().Contain("<TYPE>Company</TYPE>");
        result.Should().Contain("<FETCH>NAME</FETCH>");
    }

    [Fact]
    public void BuildLedgerListRequestXml_ShouldGenerateValidXml()
    {
        // Arrange
        var companyName = "Test Company";
        
        // Act
        var result = TallyXmlBuilder.BuildLedgerListRequestXml(companyName);
        
        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Contain("<ENVELOPE>");
        result.Should().Contain("<HEADER>");
        result.Should().Contain("<VERSION>1</VERSION>");
        result.Should().Contain("<TALLYREQUEST>Export</TALLYREQUEST>");
        result.Should().Contain("<TYPE>Collection</TYPE>");
        result.Should().Contain("<ID>List of Ledgers</ID>");
        result.Should().Contain("<SVCURRENTCOMPANY>Test Company</SVCURRENTCOMPANY>");
        result.Should().Contain("<COLLECTION NAME=\"List of Ledgers\"");
        result.Should().Contain("<TYPE>Ledger</TYPE>");
        result.Should().Contain("<FETCH>NAME</FETCH>");
        result.Should().Contain("<FETCH>PARENT</FETCH>");
    }

    [Fact]
    public void BuildVoucherImportXml_WithSingleTransaction_ShouldGenerateValidXml()
    {
        // Arrange
        var transactions = new List<TallyTool.Transaction>
        {
            new()
            {
                Type = "Receipt",
                Date = "01-01-2024",
                Narration = "Test transaction",
                Amount = 1000.50m,
                ToLedger = "Cash",
                FromAccount = "Sales"
            }
        };
        
        // Act
        var result = TallyXmlBuilder.BuildVoucherImportXml(transactions);
        
        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Contain("<ENVELOPE>");
        result.Should().Contain("<HEADER>");
        result.Should().Contain("<VERSION>1</VERSION>");
        result.Should().Contain("<TALLYREQUEST>Import</TALLYREQUEST>");
        result.Should().Contain("<TYPE>Data</TYPE>");
        result.Should().Contain("<ID>Vouchers</ID>");
        result.Should().Contain("<VOUCHER>");
        result.Should().Contain("<DATE>20240101</DATE>");
        result.Should().Contain("<NARRATION>Test transaction</NARRATION>");
        result.Should().Contain("<VOUCHERTYPENAME>Receipt</VOUCHERTYPENAME>");
        result.Should().Contain("<ALLLEDGERENTRIES.LIST>");
        result.Should().Contain("<AMOUNT>1000.50</AMOUNT>");
        result.Should().Contain("<AMOUNT>-1000.50</AMOUNT>");
    }

    [Fact]
    public void EncodeXmlText_WithSpecialCharacters_ShouldEscapeCorrectly()
    {
        // Arrange
        var input = "Test & <Company> \"Name\" 'Value'";
        
        // Act
        var result = TallyXmlBuilder.EncodeXmlText(input);
        
        // Assert
        result.Should().Be("Test &amp; &lt;Company&gt; &quot;Name&quot; &apos;Value&apos;");
    }

    [Fact]
    public void EncodeXmlText_WithNullOrEmpty_ShouldReturnEmpty()
    {
        // Act & Assert
        TallyXmlBuilder.EncodeXmlText(null!).Should().BeEmpty();
        TallyXmlBuilder.EncodeXmlText("").Should().BeEmpty();
        TallyXmlBuilder.EncodeXmlText(string.Empty).Should().BeEmpty();
    }

    [Theory]
    [InlineData("01-01-2024", "20240101")]
    [InlineData("2024-01-01", "20240101")]
    [InlineData("2024/01/01", "20240101")]
    [InlineData("01/01/2024", "20240101")]
    public void FormatDateForTally_WithValidDates_ShouldFormatCorrectly(string input, string expected)
    {
        // Act
        var result = TallyXmlBuilder.BuildVoucherImportXml(new List<TallyTool.Transaction>
        {
            new() { Type = "Receipt", Date = input, Narration = "Test", Amount = 100, ToLedger = "Cash", FromAccount = "Sales" }
        });
        
        // Assert
        result.Should().Contain($"<DATE>{expected}</DATE>");
    }
}