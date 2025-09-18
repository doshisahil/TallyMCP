# TallyMCP - Model Context Protocol Server for Tally ERP Integration

[![CI/CD Pipeline](https://github.com/doshisahil/TallyMCP/actions/workflows/ci.yml/badge.svg)](https://github.com/doshisahil/TallyMCP/actions/workflows/ci.yml)
[![Security Analysis](https://github.com/doshisahil/TallyMCP/actions/workflows/security.yml/badge.svg)](https://github.com/doshisahil/TallyMCP/actions/workflows/security.yml)
[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE.txt)

TallyMCP is a production-grade Model Context Protocol (MCP) server that provides seamless integration with Tally ERP systems. It enables AI assistants and other MCP clients to interact with Tally for accounting and financial operations, including voucher imports, company management, ledger operations, and PDF document processing.

## ✨ Features

### 🔗 Core Integrations
- **Voucher Import**: Import transactions from JSON to Tally ERP
- **Company Management**: Fetch and manage company lists from Tally
- **Ledger Operations**: Retrieve and work with ledger data
- **PDF Processing**: Extract text from PDF files with password support

### 🛠️ Production Ready
- **Configurable Endpoints**: JSON, environment variables, or command-line configuration
- **Cross-Platform**: Windows, Linux, and macOS support
- **Single-File Deployment**: Self-contained executables for easy deployment
- **Comprehensive Testing**: 33+ unit tests with full coverage
- **Security First**: CodeQL analysis and dependency vulnerability scanning

### 🏗️ Developer Experience
- **Clean Architecture**: Well-structured codebase with separation of concerns
- **Type Safety**: Full nullable reference type support
- **Structured Logging**: Built-in logging with configurable levels
- **Error Handling**: Robust error handling and validation

## 🚀 Quick Start

### Prerequisites
- .NET 8.0 Runtime (for running pre-built binaries)
- .NET 8.0 SDK (for building from source)
- Tally ERP 9 with Gateway enabled

### Installation

#### Option 1: Download Pre-built Binaries
1. Download the latest release for your platform from [Releases](https://github.com/doshisahil/TallyMCP/releases)
2. Extract the archive to your desired location
3. Run the executable

#### Option 2: Build from Source
```bash
# Clone the repository
git clone https://github.com/doshisahil/TallyMCP.git
cd TallyMCP

# Restore dependencies
dotnet restore

# Build the project
dotnet build --configuration Release

# Run the application
dotnet run --configuration Release
```

### Basic Usage

1. **Start TallyMCP Server**:
   ```bash
   # Default configuration (localhost:3001)
   ./TallyMCP

   # Custom configuration
   ./TallyMCP --TallyMCP:Server:Host=0.0.0.0 --TallyMCP:Server:Port=8080
   ```

2. **Configure Tally Connection** (optional):
   ```bash
   # Connect to remote Tally instance
   ./TallyMCP --TallyMCP:Tally:Host=192.168.1.100 --TallyMCP:Tally:Port=9000
   ```

3. **Connect your MCP Client** to `http://localhost:3001` (or your configured endpoint)

## ⚙️ Configuration

TallyMCP supports multiple configuration methods in order of precedence:

1. **Command Line Arguments** (highest precedence)
2. **Environment Variables**
3. **JSON Configuration Files**
4. **Default Values** (lowest precedence)

### JSON Configuration (`appsettings.json`)

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "TallyMCP": {
    "Server": {
      "Host": "localhost",
      "Port": 3001
    },
    "Tally": {
      "Host": "localhost",
      "Port": 9000
    }
  }
}
```

### Environment Variables

```bash
export TallyMCP__Server__Host=0.0.0.0
export TallyMCP__Server__Port=8080
export TallyMCP__Tally__Host=192.168.1.100
export TallyMCP__Tally__Port=9000
```

### Command Line Arguments

```bash
./TallyMCP --TallyMCP:Server:Host=0.0.0.0 \
           --TallyMCP:Server:Port=8080 \
           --TallyMCP:Tally:Host=192.168.1.100 \
           --TallyMCP:Tally:Port=9000
```

## 🔧 Available Tools

### 1. ImportVoucher
Import transactions from JSON format into Tally.

**Parameters**:
- `transactions`: List of transaction objects

**Transaction Object**:
```json
{
  "Type": "Receipt",
  "Date": "01-01-2024",
  "Narration": "Sales transaction",
  "Amount": 1000.00,
  "ToLedger": "Cash",
  "FromAccount": "Sales"
}
```

### 2. FetchCompanyList
Retrieve the list of companies from Tally.

**Returns**: Array of company objects with names

### 3. FetchLedgerList
Fetch ledgers for a specific company.

**Parameters**:
- `companyName`: Name of the company to fetch ledgers for

**Returns**: Array of ledger objects with names and parent classifications

### 4. ExtractTextFromPdf
Extract text content from PDF files.

**Parameters**:
- `pdfFilePath`: Path to the PDF file
- `password`: Optional password for encrypted PDFs

**Returns**: Extracted text content as string

## 🎯 Available Prompts

### GetBankStatementExtractionPromptTemplate
Returns a comprehensive prompt template for extracting transaction data from bank statement PDFs. Includes guidelines for:
- Table identification and structure
- Transaction classification (Receipt/Payment)
- Date formatting and validation
- Amount parsing and validation
- Special handling for different bank formats (ICICI, Kotak, etc.)

## 🏗️ Project Structure

```
TallyMCP/
├── Configuration/           # Configuration models and options
│   └── TallyMcpOptions.cs  # Configuration classes
├── TallyTool.cs            # Main MCP tools implementation
├── TallyHttpClient.cs      # HTTP client for Tally communication
├── TallyXmlBuilder.cs      # XML request builders
├── TallyXmlParser.cs       # XML response parsers
├── TallyXmlSanitizer.cs    # XML sanitization utilities
├── Program.cs              # Application entry point
├── appsettings.json        # Default configuration
├── TallyMCP.Tests/         # Comprehensive unit tests
├── docs/                   # Documentation
│   └── DEVELOPMENT.md      # Development guidelines
├── .github/workflows/      # CI/CD pipelines
│   ├── ci.yml             # Continuous integration
│   ├── ci-cd.yml          # Release pipeline
│   └── security.yml       # Security analysis
├── LICENSE.txt             # MIT license
├── THIRD-PARTY-NOTICES.txt # Third-party attributions
└── README.md              # This file
```

## 🧪 Testing

The project includes comprehensive unit tests covering all core functionality:

```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test category
dotnet test --filter="Category=Unit"
```

**Test Coverage**:
- ✅ Configuration validation (6 tests)
- ✅ XML building and encoding (8 tests)  
- ✅ XML parsing with various scenarios (7 tests)
- ✅ XML sanitization and edge cases (12 tests)
- ✅ **Total: 33 passing tests**

## 🚀 Deployment

### Docker Deployment (Recommended)

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY ./publish/linux-x64 .
EXPOSE 3001
ENTRYPOINT ["./TallyMCP"]
```

### Standalone Deployment

1. Download the appropriate binary for your platform
2. Configure via environment variables or config files
3. Run as a system service or daemon

### Cloud Deployment

The application is designed to work in containerized environments and supports:
- **Azure Container Instances**
- **AWS ECS/Fargate**
- **Google Cloud Run**
- **Kubernetes**

## 🔒 Security Considerations

- **Network Security**: Configure firewalls to restrict access to Tally ports
- **Authentication**: Implement reverse proxy with authentication for production
- **HTTPS**: Use TLS termination at load balancer or reverse proxy level
- **Input Validation**: All inputs are validated and sanitized
- **Dependency Scanning**: Automated vulnerability scanning in CI/CD

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Make your changes and add tests
4. Ensure all tests pass (`dotnet test`)
5. Commit your changes (`git commit -m 'Add amazing feature'`)
6. Push to the branch (`git push origin feature/amazing-feature`)
7. Open a Pull Request

### Development Setup

```bash
# Clone and setup
git clone https://github.com/doshisahil/TallyMCP.git
cd TallyMCP

# Install dependencies
dotnet restore

# Run in development mode
dotnet run --environment Development

# Run tests continuously
dotnet watch test
```

### Development Guidelines

For information about dependencies, licensing, and development best practices, see [docs/DEVELOPMENT.md](docs/DEVELOPMENT.md).

## 📋 Requirements

### System Requirements
- **OS**: Windows 10+, Linux (Ubuntu 18.04+), or macOS 10.15+
- **Memory**: 512MB RAM minimum
- **Storage**: 50MB free space
- **Network**: TCP connectivity to Tally ERP Gateway

### Tally ERP Requirements
- **Version**: Tally ERP 9 (Release 6.6 or later recommended)
- **Gateway**: Must be enabled and accessible
- **Port**: Default 9000 (configurable)

## 🐛 Troubleshooting

### Common Issues

**Connection to Tally Failed**
```bash
# Check if Tally Gateway is running
telnet localhost 9000

# Verify Tally configuration
# F11 (Features) > Advanced Configuration > Connectivity > Port: 9000
```

**Port Already in Use**
```bash
# Use different port
./TallyMCP --TallyMCP:Server:Port=8080
```

**Permission Denied**
```bash
# Linux/macOS: Make executable
chmod +x TallyMCP

# Windows: Run as Administrator (if needed)
```

## 📄 License

This project is licensed under the MIT License - see the [LICENSE.txt](LICENSE.txt) file for details.

### Third-Party Licenses

TallyMCP uses several third-party libraries, each with their own licenses:

- **PdfPig**: Apache-2.0 License - Used for PDF text extraction
- **Model Context Protocol SDK**: MIT License - Core MCP functionality
- **Microsoft .NET Extensions**: MIT License - Hosting and dependency injection
- **Testing Libraries**: Various licenses (Apache-2.0, MIT, BSD-3-Clause)

For complete licensing information and attributions, see [THIRD-PARTY-NOTICES.txt](THIRD-PARTY-NOTICES.txt).

### License Compatibility

All production dependencies use permissive licenses (MIT, Apache-2.0) that are compatible with the MIT license of this project. Test-only dependencies may have additional restrictions but do not affect the distribution of the main application.

## 🙏 Acknowledgments

- [Model Context Protocol](https://github.com/modelcontextprotocol) for the MCP specification
- [Tally Solutions](https://tallysolutions.com/) for Tally ERP 9
- [PdfPig](https://github.com/UglyToad/PdfPig) for PDF processing capabilities
- .NET Community for the excellent ecosystem

## 📞 Support

- **Issues**: [GitHub Issues](https://github.com/doshisahil/TallyMCP/issues)
- **Discussions**: [GitHub Discussions](https://github.com/doshisahil/TallyMCP/discussions)
- **Security**: Email security@example.com for security-related issues

---

⭐ **Star this repository if you find it useful!**