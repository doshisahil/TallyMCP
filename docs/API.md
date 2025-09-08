# API Reference

TallyMCP provides a Model Context Protocol (MCP) server with tools and prompts for Tally ERP integration.

## Server Endpoints

### Base URL
- Default: `http://localhost:3001`
- Configurable via `TallyMCP:Server:Host` and `TallyMCP:Server:Port`

### MCP Protocol Endpoint
- `POST /mcp` - Main MCP protocol endpoint

## Tools

### ImportVoucher

Imports a list of transactions into Tally ERP.

**Method**: `ImportVoucher`

**Parameters**:
```json
{
  "transactions": [
    {
      "Type": "string",        // "Receipt" or "Payment"
      "Date": "string",        // DD-MM-YYYY format
      "Narration": "string",   // Transaction description
      "Amount": "decimal",     // Transaction amount
      "ToLedger": "string",    // Destination ledger name
      "FromAccount": "string"  // Source account name
    }
  ]
}
```

**Response**:
```json
{
  "json": {
    "ImportResult": {
      "CREATED": "number",
      "ALTERED": "number", 
      "DELETED": "number",
      "ERRORS": "number"
    },
    "CmpInfo": {
      "COMPANYNAME": "string",
      "COMPANYNUMBER": "string"
    }
  }
}
```

**Example**:
```json
{
  "transactions": [
    {
      "Type": "Receipt",
      "Date": "01-01-2024",
      "Narration": "Cash sale",
      "Amount": 5000.00,
      "ToLedger": "Cash",
      "FromAccount": "Sales"
    }
  ]
}
```

### FetchCompanyList

Retrieves the list of companies from Tally ERP.

**Method**: `FetchCompanyList`

**Parameters**: None

**Response**:
```json
{
  "json": [
    {
      "Name": "string"  // Company name
    }
  ]
}
```

**Example Response**:
```json
{
  "json": [
    { "Name": "ABC Corp Ltd" },
    { "Name": "XYZ Industries" }
  ]
}
```

### FetchLedgerList

Fetches the list of ledgers for a specific company.

**Method**: `FetchLedgerList`

**Parameters**:
```json
{
  "companyName": "string"  // Name of the company
}
```

**Response**:
```json
{
  "json": [
    {
      "Name": "string",    // Ledger name
      "Parent": "string"   // Parent group name
    }
  ]
}
```

**Example**:
```json
{
  "companyName": "ABC Corp Ltd"
}
```

**Example Response**:
```json
{
  "json": [
    { "Name": "Cash", "Parent": "Current Assets" },
    { "Name": "Sales", "Parent": "Sales Accounts" },
    { "Name": "Purchase", "Parent": "Purchase Accounts" }
  ]
}
```

### ExtractTextFromPdf

Extracts text content from PDF files with optional password support.

**Method**: `ExtractTextFromPdf`

**Parameters**:
```json
{
  "pdfFilePath": "string",  // Absolute path to PDF file
  "password": "string"      // Optional password for encrypted PDFs
}
```

**Response**:
```json
"string"  // Extracted text content
```

**Example**:
```json
{
  "pdfFilePath": "/path/to/document.pdf",
  "password": "optional_password"
}
```

## Prompts

### GetBankStatementExtractionPromptTemplate

Returns a comprehensive prompt template for extracting transaction data from bank statement PDFs.

**Method**: `GetBankStatementExtractionPromptTemplate`

**Parameters**: None

**Response**:
```json
"string"  // Detailed prompt template
```

**Description**: The prompt template includes:
- Guidelines for table identification
- Transaction classification rules
- Date formatting requirements
- Amount parsing instructions
- Special handling for different bank formats (ICICI, Kotak, etc.)
- Validation rules for transaction data

## Error Handling

### Common Error Responses

**Connection Error**:
```json
{
  "error": "Connection to Tally failed",
  "details": "Could not connect to http://localhost:9000"
}
```

**Validation Error**:
```json
{
  "error": "Invalid transaction data",
  "details": "Amount must be greater than 0"
}
```

**File Not Found**:
```json
{
  "error": "File not found",
  "details": "The specified PDF file does not exist"
}
```

### HTTP Status Codes

- `200 OK` - Request successful
- `400 Bad Request` - Invalid request parameters
- `404 Not Found` - Resource not found
- `500 Internal Server Error` - Server error
- `502 Bad Gateway` - Tally connection error

## Data Types

### Transaction Object

```typescript
interface Transaction {
  Type: "Receipt" | "Payment";
  Date: string;        // DD-MM-YYYY format
  Narration: string;
  Amount: number;      // Decimal value
  ToLedger: string;
  FromAccount: string;
}
```

### Company Object

```typescript
interface Company {
  Name: string;
}
```

### Ledger Object

```typescript
interface Ledger {
  Name: string;
  Parent: string;
}
```

## Configuration Schema

```typescript
interface TallyMcpOptions {
  Server: {
    Host: string;      // Default: "localhost"
    Port: number;      // Default: 3001
  };
  Tally: {
    Host: string;      // Default: "localhost"
    Port: number;      // Default: 9000
  };
}
```

## XML Protocol Details

TallyMCP communicates with Tally ERP using XML requests over HTTP. The following sections detail the XML structure.

### Company List Request

```xml
<ENVELOPE>
  <HEADER>
    <VERSION>1</VERSION>
    <TALLYREQUEST>Export</TALLYREQUEST>
    <TYPE>Collection</TYPE>
    <ID>List of Companies</ID>
  </HEADER>
  <BODY>
    <DESC>
      <TDL>
        <TDLMESSAGE>
          <COLLECTION NAME="List of Companies" ISMODIFY="No">
            <TYPE>Company</TYPE>
            <FETCH>NAME</FETCH>
          </COLLECTION>
        </TDLMESSAGE>
      </TDL>
    </DESC>
  </BODY>
</ENVELOPE>
```

### Ledger List Request

```xml
<ENVELOPE>
  <HEADER>
    <VERSION>1</VERSION>
    <TALLYREQUEST>Export</TALLYREQUEST>
    <TYPE>Collection</TYPE>
    <ID>List of Ledgers</ID>
  </HEADER>
  <BODY>
    <DESC>
      <STATICVARIABLES>
        <SVCURRENTCOMPANY>{CompanyName}</SVCURRENTCOMPANY>
      </STATICVARIABLES>
      <TDL>
        <TDLMESSAGE>
          <COLLECTION NAME="List of Ledgers" ISMODIFY="No">
            <TYPE>Ledger</TYPE>
            <FETCH>NAME</FETCH>
            <FETCH>PARENT</FETCH>
          </COLLECTION>
        </TDLMESSAGE>
      </TDL>
    </DESC>
  </BODY>
</ENVELOPE>
```

### Voucher Import Request

```xml
<ENVELOPE>
  <HEADER>
    <VERSION>1</VERSION>
    <TALLYREQUEST>Import</TALLYREQUEST>
    <TYPE>Data</TYPE>
    <ID>Vouchers</ID>
  </HEADER>
  <BODY>
    <DESC/>
    <DATA>
      <TALLYMESSAGE>
        <VOUCHER>
          <DATE>{FormattedDate}</DATE>
          <NARRATION>{Narration}</NARRATION>
          <VOUCHERTYPENAME>{Type}</VOUCHERTYPENAME>
          <ALLLEDGERENTRIES.LIST>
            <LEDGERNAME>{LedgerName}</LEDGERNAME>
            <ISDEEMEDPOSITIVE>Yes</ISDEEMEDPOSITIVE>
            <AMOUNT>{Amount}</AMOUNT>
          </ALLLEDGERENTRIES.LIST>
          <ALLLEDGERENTRIES.LIST>
            <LEDGERNAME>{CounterLedger}</LEDGERNAME>
            <ISDEEMEDPOSITIVE>No</ISDEEMEDPOSITIVE>
            <AMOUNT>{-Amount}</AMOUNT>
          </ALLLEDGERENTRIES.LIST>
        </VOUCHER>
      </TALLYMESSAGE>
    </DATA>
  </BODY>
</ENVELOPE>
```