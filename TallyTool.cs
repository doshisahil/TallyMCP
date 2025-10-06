using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using UglyToad.PdfPig;
using System.IO;

[McpServerToolType]
public static class TallyTool
{
    public class Transaction
    {
        [Description("Type of transaction (Receipt or Payment)")]
        public string Type { get; set; } = string.Empty;
        [Description("Date of the transaction in DD-MM-YYYY format")]
        public string Date { get; set; } = string.Empty;
        [Description("Narration of the transaction")]
        public string Narration { get; set; } = string.Empty;
        [Description("Amount of the transaction")]
        public decimal Amount { get; set; }
        [Description("Ledger to credit/debit (To)")]
        public string ToLedger { get; set; } = string.Empty;
        [Description("Ledger to credit/debit (From)")]
        public string FromAccount { get; set; } = string.Empty;
    }

    [McpServerTool, Description("Imports a list of transactions from JSON and sends them to Tally. Accepts the company name as a parameter.")]
    public static async Task<string> ImportVoucher(List<Transaction> transactions, string companyName)
    {
        var requestXml = TallyXmlBuilder.BuildVoucherImportXml(transactions, companyName);
        var responseXml = await TallyHttpClientExtensions.PostToTallyAsync(requestXml);
        var sanitizedXml = TallyXmlSanitizer.Sanitize(responseXml);
        var json = TallyXmlParser.ParseImportResponseToJson(sanitizedXml);
        return JsonSerializer.Serialize(new { json });
    }

    [McpServerTool, Description("Fetches the list of companies from Tally.")]
    public static async Task<string> FetchCompanyList()
    {
        var requestXml = TallyXmlBuilder.BuildCompanyListRequestXml();
        var responseXml = await TallyHttpClientExtensions.PostToTallyAsync(requestXml);
        var sanitizedXml = TallyXmlSanitizer.Sanitize(responseXml);
        var json = TallyXmlParser.ParseCompanyListToJson(sanitizedXml);
        return JsonSerializer.Serialize(new { json });
    }

    [McpServerTool, Description("Fetches the list of ledgers for a company from Tally.")]
    public static async Task<string> FetchLedgerList(string companyName)
    {
        var requestXml = TallyXmlBuilder.BuildLedgerListRequestXml(companyName);
        var responseXml = await TallyHttpClientExtensions.PostToTallyAsync(requestXml);
        var sanitizedXml = TallyXmlSanitizer.Sanitize(responseXml);
        var json = TallyXmlParser.ParseLedgerListToJson(sanitizedXml);
        return JsonSerializer.Serialize(new { json });
    }

    [McpServerTool, Description("Extracts all text from a PDF file using PdfPig. Optionally accepts a password for encrypted PDFs. Returns the full text as a string.")]
    public static string ExtractTextFromPdf(string pdfFilePath, string? password = null)
    {
        if (string.IsNullOrWhiteSpace(pdfFilePath) || !File.Exists(pdfFilePath))
            return "Invalid file path.";

        var text = new System.Text.StringBuilder();
        try
        {
            PdfDocument pdf;
            if (!string.IsNullOrEmpty(password))
            {
                pdf = PdfDocument.Open(pdfFilePath, new ParsingOptions() { Password = password });
            }
            else
            {
                pdf = PdfDocument.Open(pdfFilePath);
            }
            using (pdf)
            {
                foreach (var page in pdf.GetPages())
                {
                    text.AppendLine(page.Text);
                }
            }
        }
        catch (Exception ex)
        {
            return $"Error reading PDF: {ex.Message}";
        }
        return text.ToString();
    }
}

[McpServerPromptType]
public static class TallyPrompt
{

    [McpServerPrompt, Description("Returns a reusable prompt template for extracting transactions from a bank statement. Accepts format instructions as a parameter.")]
    public static string GetBankStatementExtractionPromptTemplate()
    {
        return @"Extract all transaction details from the attached bank statement PDF. If the PDF is password-protected, use the provided password parameter if available.\n" +
            "password : {password}\n" +
        "\n### Workflow:\n" +
        "1. **Extract Text from PDF**:\n" +
        "   - Use the `ExtractTextFromPdf` tool to extract all text from the PDF. If a password is provided, pass it as the second argument.\n" +
        "2. **Identify the Company**:\n" +
        "   - Use the `FetchCompanyList` tool to get the list of companies in Tally. Match the company name from the extracted text to the closest company in the list.\n" +
        "3. **Identify Ledgers**:\n" +
        "   - Use the `FetchLedgerList` tool with the identified company to get all ledgers.\n" +
        "   - For each transaction, use the narration and other details to determine the most appropriate `ToLedger` and `FromAccount` from the available ledgers only strictly with same case and from the same list else import request will fail.\n" +
        "   - If no matching ledger is found, usually try to map it to Suspense or Suspense A/c kind of ledger if available.\n" +
        "4. **Transaction Extraction**:\n" +
        "   - Identify the table with headers (or implied headers): DATE, MODE, PARTICULARS, DEPOSITS, WITHDRAWALS, BALANCE.\n" +
        "   - Focus only on rows that represent valid transactions. Ignore irrelevant data such as opening balances, closing balances, account numbers, headers, and any placeholder text like 'B/F' (brought forward) or 'C/F' (carried forward).\n" +
        "   - The table may not be explicitly labeled but will be evident from sequences of similar data formats.\n" +
        "5. **Transaction Classification**:\n" +
        "   - Determine the transaction type as 'Receipt' for DEPOSITS or 'Payment' for WITHDRAWALS and 'Contra' for inter-account transfers.\n" +
        "   - If columns are unclear due to PDF-to-text conversion, use the balance change to identify the transaction type. For the first transaction, use the opening balance; for others, use the previous transaction balance.\n" +
        "6. **Date Handling**:\n" +
        "   - Extract the DATE field and reformat it into the DD-MM-YYYY format regardless of its original representation (e.g., '2023/01/10' → '10-01-2023').\n" +
        "7. **Narration**:\n" +
        "   - Extract the text in the PARTICULARS column as the narration.\n" +
        "8. **Amount**:\n" +
        "   - Use the corresponding amount from either DEPOSITS or WITHDRAWALS as the amount. Ensure the numbers are parsed correctly as floating-point values with two decimal places.\n" +
        "9. **Ignore Rules**:\n" +
        "   - Ensure there is no additional explanation, summary, or non-JSON content. JSON must be minified.\n" +
        "   - Specially ignore narration 'B/F' and 'C/F' transactions in ICICI bank statements.\n" +
        "   - Specially ignore narration 'OPENING BALANCE' transactions in Kotak bank statements.\n" +
        "   - Ignore any rows with the narration containing 'B/F' or 'C/F'.\n" +
        "\nIf you need to extract text from a password-protected PDF, call the `ExtractTextFromPdf` tool with the password parameter. Use the extracted text for all further processing." +
        "use ImportVoucher tool to import the transactions.\n" +
        "From Next attached PDF optimize call to fetching company and ledger details. if applicable.\n";
    }
}
