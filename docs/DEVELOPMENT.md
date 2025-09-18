# Development Guidelines

## Development Dependencies and Licensing

### Testing Libraries

This project uses only Microsoft-provided and open source testing libraries with permissive licenses:

- **xUnit**: Apache-2.0 License - Primary testing framework
- **Moq**: BSD-3-Clause License - Mocking framework for unit tests
- **Microsoft.NET.Test.Sdk**: MIT License - Test platform and runner
- **Coverlet**: MIT License - Code coverage collection

All testing dependencies use permissive licenses that are fully compatible with commercial and open source projects.

### Testing Approach

The project uses standard xUnit assertions:

```csharp
// Basic assertions
Assert.Equal(expected, actual);
Assert.True(condition);
Assert.False(condition);
Assert.Null(value);
Assert.NotNull(value);

// String assertions
Assert.Empty(string);
Assert.NotEmpty(string);
Assert.Contains("substring", fullString);

// Collection assertions
Assert.Single(collection);
Assert.Empty(collection);
Assert.Equal(expectedCount, collection.Count);
```

### Building and Testing

```bash
# Build the project
dotnet build

# Run tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run tests with specific category
dotnet test --filter="Category=Unit"
```

### License Compliance

This project maintains strict license compliance:
- Production code uses only MIT and Apache-2.0 licensed dependencies
- Test dependencies use permissive licenses (Apache-2.0, MIT, BSD-3-Clause)
- All third-party licenses are properly attributed in THIRD-PARTY-NOTICES.txt
- The main application remains under MIT license without restrictions