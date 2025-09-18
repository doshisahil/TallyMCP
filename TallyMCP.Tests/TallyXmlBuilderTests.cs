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
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Contains("<ENVELOPE>", result);
        Assert.Contains("<HEADER>", result);
        Assert.Contains("<VERSION>1</VERSION>", result);
        Assert.Contains("<TALLYREQUEST>Export</TALLYREQUEST>", result);
        Assert.Contains("<TYPE>Collection</TYPE>", result);
        Assert.Contains("<ID>List of Companies</ID>", result);
        Assert.Contains("<COLLECTION NAME=\"List of Companies\"", result);
        Assert.Contains("<TYPE>Company</TYPE>", result);
        Assert.Contains("<FETCH>NAME</FETCH>", result);
    }

    [Fact]
    public void BuildLedgerListRequestXml_ShouldGenerateValidXml()
    {
        // Arrange
        var companyName = "Test Company";

        // Act
        var result = TallyXmlBuilder.BuildLedgerListRequestXml(companyName);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Contains("<ENVELOPE>", result);
        Assert.Contains("<HEADER>", result);
        Assert.Contains("<VERSION>1</VERSION>", result);
        Assert.Contains("<TALLYREQUEST>Export</TALLYREQUEST>", result);
        Assert.Contains("<TYPE>Collection</TYPE>", result);
        Assert.Contains("<ID>List of Ledgers</ID>", result);
        Assert.Contains("<SVCURRENTCOMPANY>Test Company</SVCURRENTCOMPANY>", result);
        Assert.Contains("<COLLECTION NAME=\"List of Ledgers\"", result);
        Assert.Contains("<TYPE>Ledger</TYPE>", result);
        Assert.Contains("<FETCH>NAME</FETCH>", result);
        Assert.Contains("<FETCH>PARENT</FETCH>", result);
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
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Contains("<ENVELOPE>", result);
        Assert.Contains("<HEADER>", result);
        Assert.Contains("<VERSION>1</VERSION>", result);
        Assert.Contains("<TALLYREQUEST>Import</TALLYREQUEST>", result);
        Assert.Contains("<TYPE>Data</TYPE>", result);
        Assert.Contains("<ID>Vouchers</ID>", result);
        Assert.Contains("<VOUCHER>", result);
        Assert.Contains("<DATE>20240101</DATE>", result);
        Assert.Contains("<NARRATION>Test transaction</NARRATION>", result);
        Assert.Contains("<VOUCHERTYPENAME>Receipt</VOUCHERTYPENAME>", result);
        Assert.Contains("<ALLLEDGERENTRIES.LIST>", result);
        Assert.Contains("<AMOUNT>1000.50</AMOUNT>", result);
        Assert.Contains("<AMOUNT>-1000.50</AMOUNT>", result);
    }

    [Fact]
    public void EncodeXmlText_WithSpecialCharacters_ShouldEscapeCorrectly()
    {
        // Arrange
        var input = "Test & <Company> \"Name\" 'Value'";

        // Act
        var result = TallyXmlBuilder.EncodeXmlText(input);

        // Assert
        Assert.Equal("Test &amp; &lt;Company&gt; &quot;Name&quot; &apos;Value&apos;", result);
    }

    [Fact]
    public void EncodeXmlText_WithNullOrEmpty_ShouldReturnEmpty()
    {
        // Act & Assert
        Assert.Empty(TallyXmlBuilder.EncodeXmlText(null!));
        Assert.Empty(TallyXmlBuilder.EncodeXmlText(""));
        Assert.Empty(TallyXmlBuilder.EncodeXmlText(string.Empty));
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
        Assert.Contains($"<DATE>{expected}</DATE>", result);
    }
}