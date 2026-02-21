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
    public void BuildVoucherImportXml_WithPaymentTransaction_ShouldGenerateValidXml()
    {
        // Arrange
        var transactions = new List<TallyTool.Transaction>
        {
            new()
            {
                Type = "Payment",
                Date = "01-01-2024",
                Narration = "Office rent payment",
                Amount = 5000.00m,
                ToLedger = "Rent",
                FromAccount = "Bank"
            }
        };

        // Act
        var result = TallyXmlBuilder.BuildVoucherImportXml(transactions);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("<VOUCHERTYPENAME>Payment</VOUCHERTYPENAME>", result);
        Assert.Contains("<LEDGERNAME>Rent</LEDGERNAME>", result);
        Assert.Contains("<ISDEEMEDPOSITIVE>Yes</ISDEEMEDPOSITIVE>", result);
        Assert.Contains("<LEDGERNAME>Bank</LEDGERNAME>", result);
        Assert.Contains("<ISDEEMEDPOSITIVE>No</ISDEEMEDPOSITIVE>", result);
        Assert.Contains("<AMOUNT>-5000.00</AMOUNT>", result);
        Assert.Contains("<AMOUNT>5000.00</AMOUNT>", result);
    }

    [Fact]
    public void BuildVoucherImportXml_WithContraTransaction_ShouldGenerateValidXml()
    {
        // Arrange
        var transactions = new List<TallyTool.Transaction>
        {
            new()
            {
                Type = "Contra",
                Date = "15-03-2024",
                Narration = "Cash deposited into bank",
                Amount = 10000.00m,
                ToLedger = "HDFC Bank",
                FromAccount = "Cash"
            }
        };

        // Act
        var result = TallyXmlBuilder.BuildVoucherImportXml(transactions);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("<VOUCHERTYPENAME>Contra</VOUCHERTYPENAME>", result);
        Assert.Contains("<LEDGERNAME>HDFC Bank</LEDGERNAME>", result);
        Assert.Contains("<ISDEEMEDPOSITIVE>Yes</ISDEEMEDPOSITIVE>", result);
        Assert.Contains("<LEDGERNAME>Cash</LEDGERNAME>", result);
        Assert.Contains("<ISDEEMEDPOSITIVE>No</ISDEEMEDPOSITIVE>", result);
        Assert.Contains("<AMOUNT>-10000.00</AMOUNT>", result);
        Assert.Contains("<AMOUNT>10000.00</AMOUNT>", result);
    }

    [Fact]
    public void BuildVoucherImportXml_WithJournalTransaction_ShouldGenerateValidXml()
    {
        // Arrange
        var transactions = new List<TallyTool.Transaction>
        {
            new()
            {
                Type = "Journal",
                Date = "31-03-2024",
                Narration = "Depreciation adjustment",
                Amount = 2500.00m,
                ToLedger = "Depreciation",
                FromAccount = "Fixed Assets"
            }
        };

        // Act
        var result = TallyXmlBuilder.BuildVoucherImportXml(transactions);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("<VOUCHERTYPENAME>Journal</VOUCHERTYPENAME>", result);
        Assert.Contains("<LEDGERNAME>Depreciation</LEDGERNAME>", result);
        Assert.Contains("<ISDEEMEDPOSITIVE>Yes</ISDEEMEDPOSITIVE>", result);
        Assert.Contains("<LEDGERNAME>Fixed Assets</LEDGERNAME>", result);
        Assert.Contains("<ISDEEMEDPOSITIVE>No</ISDEEMEDPOSITIVE>", result);
        Assert.Contains("<AMOUNT>-2500.00</AMOUNT>", result);
        Assert.Contains("<AMOUNT>2500.00</AMOUNT>", result);
    }

    [Theory]
    [InlineData("Receipt")]
    [InlineData("Payment")]
    [InlineData("Contra")]
    [InlineData("Journal")]
    public void BuildVoucherImportXml_WithAllTransactionTypes_ShouldSetCorrectVoucherTypeName(string type)
    {
        // Arrange
        var transactions = new List<TallyTool.Transaction>
        {
            new() { Type = type, Date = "01-01-2024", Narration = "Test", Amount = 100, ToLedger = "LedgerA", FromAccount = "LedgerB" }
        };

        // Act
        var result = TallyXmlBuilder.BuildVoucherImportXml(transactions);

        // Assert
        Assert.Contains($"<VOUCHERTYPENAME>{type}</VOUCHERTYPENAME>", result);
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