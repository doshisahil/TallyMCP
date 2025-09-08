using FluentAssertions;
using System.Linq;
using Xunit;

namespace TallyMCP.Tests;

public class TallyXmlParserTests
{
    [Fact]
    public void ParseImportResponseToJson_WithValidXml_ShouldReturnCorrectData()
    {
        // Arrange
        var xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<ENVELOPE>
    <HEADER>
        <VERSION>1</VERSION>
    </HEADER>
    <BODY>
        <IMPORTRESULT>
            <CREATED>5</CREATED>
            <ALTERED>0</ALTERED>
            <DELETED>0</DELETED>
            <ERRORS>0</ERRORS>
        </IMPORTRESULT>
        <CMPINFO>
            <COMPANYNAME>Test Company</COMPANYNAME>
            <COMPANYNUMBER>1</COMPANYNUMBER>
        </CMPINFO>
    </BODY>
</ENVELOPE>";
        
        // Act
        var result = TallyXmlParser.ParseImportResponseToJson(xml);
        
        // Assert
        result.Should().NotBeNull();
        var dict = result as Dictionary<string, object>;
        dict.Should().NotBeNull();
        dict.Should().ContainKey("ImportResult");
        dict.Should().ContainKey("CmpInfo");
    }

    [Fact]
    public void ParseImportResponseToJson_WithInvalidXml_ShouldReturnNull()
    {
        // Arrange
        var invalidXml = "<invalid>xml";
        
        // Act
        var result = TallyXmlParser.ParseImportResponseToJson(invalidXml);
        
        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ParseCompanyListToJson_WithValidXml_ShouldReturnCompanies()
    {
        // Arrange
        var xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<ENVELOPE>
    <HEADER>
        <VERSION>1</VERSION>
    </HEADER>
    <BODY>
        <DATA>
            <COMPANY>
                <NAME>Company 1</NAME>
            </COMPANY>
            <COMPANY>
                <NAME>Company 2</NAME>
            </COMPANY>
        </DATA>
    </BODY>
</ENVELOPE>";
        
        // Act
        var result = TallyXmlParser.ParseCompanyListToJson(xml);
        
        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<System.Collections.IEnumerable>();
        
        // Cast to IEnumerable to count items
        var companies = (System.Collections.IEnumerable)result!;
        var companyList = companies.Cast<object>().ToList();
        companyList.Should().HaveCount(2);
    }

    [Fact]
    public void ParseCompanyListToJson_WithInvalidXml_ShouldReturnNull()
    {
        // Arrange
        var invalidXml = "<invalid>xml";
        
        // Act
        var result = TallyXmlParser.ParseCompanyListToJson(invalidXml);
        
        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ParseLedgerListToJson_WithValidXml_ShouldReturnLedgers()
    {
        // Arrange
        var xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<ENVELOPE>
    <HEADER>
        <VERSION>1</VERSION>
    </HEADER>
    <BODY>
        <DATA>
            <LEDGER>
                <NAME>Cash</NAME>
                <PARENT>Current Assets</PARENT>
            </LEDGER>
            <LEDGER>
                <NAME>Sales</NAME>
                <PARENT>Sales Accounts</PARENT>
            </LEDGER>
        </DATA>
    </BODY>
</ENVELOPE>";
        
        // Act
        var result = TallyXmlParser.ParseLedgerListToJson(xml);
        
        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<System.Collections.IEnumerable>();
        
        // Cast to IEnumerable to count items
        var ledgers = (System.Collections.IEnumerable)result!;
        var ledgerList = ledgers.Cast<object>().ToList();
        ledgerList.Should().HaveCount(2);
    }

    [Fact]
    public void ParseLedgerListToJson_WithInvalidXml_ShouldReturnNull()
    {
        // Arrange
        var invalidXml = "<invalid>xml";
        
        // Act
        var result = TallyXmlParser.ParseLedgerListToJson(invalidXml);
        
        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ParseLedgerListToJson_WithEmptyData_ShouldReturnEmptyList()
    {
        // Arrange
        var xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<ENVELOPE>
    <HEADER>
        <VERSION>1</VERSION>
    </HEADER>
    <BODY>
        <DATA>
        </DATA>
    </BODY>
</ENVELOPE>";
        
        // Act
        var result = TallyXmlParser.ParseLedgerListToJson(xml);
        
        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<System.Collections.IEnumerable>();
        
        // Cast to IEnumerable to count items
        var ledgers = (System.Collections.IEnumerable)result!;
        var ledgerList = ledgers.Cast<object>().ToList();
        ledgerList.Should().BeEmpty();
    }
}