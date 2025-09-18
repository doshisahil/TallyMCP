# Development Guidelines

## Development Dependencies and Licensing

### FluentAssertions Licensing Notice

This project uses FluentAssertions for unit testing. FluentAssertions is licensed under the Xceed Community License Agreement, which permits **non-commercial use only**.

**Important Notes:**
- FluentAssertions is used **only in test projects** and is not included in the distributed application
- For commercial development, you may need to obtain a commercial license from Xceed Software Inc.
- The Community License covers open source, personal, and experimental projects
- See [THIRD-PARTY-NOTICES.txt](../THIRD-PARTY-NOTICES.txt) for complete licensing information

### Alternative Testing Libraries

If you need to avoid the FluentAssertions licensing restrictions, you can replace it with alternatives:

```csharp
// Instead of FluentAssertions
result.Should().Be(expected);

// Use standard assertions
Assert.Equal(expected, result);

// Or other MIT-licensed assertion libraries
```

### Building and Testing

```bash
# Build the project
dotnet build

# Run tests (will show FluentAssertions license warning)
dotnet test

# Run tests with specific category
dotnet test --filter="Category=Unit"
```

### License Compliance

This project maintains strict license compliance:
- Production code uses only MIT and Apache-2.0 licensed dependencies
- Test dependencies are clearly separated and documented
- All third-party licenses are properly attributed in THIRD-PARTY-NOTICES.txt
- The main application remains under MIT license without restrictions