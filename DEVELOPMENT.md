# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Development Commands

**Building:**
```bash
# Build main solution
dotnet build Mutagen.Bethesda.Serialization.sln -c Release

# Build test solution (after building main)
dotnet build Mutagen.Bethesda.Serialization.Tests.sln -c Release
```

**Testing:**
```bash
# Test main solution
dotnet test Mutagen.Bethesda.Serialization.sln -c Release --no-build

# Test the test solution
dotnet test Mutagen.Bethesda.Serialization.Tests.sln -c Release --no-build
```

**Packaging:**
```bash
# Create NuGet packages
dotnet pack Mutagen.Bethesda.Serialization.sln -c Release --no-build --no-restore -o out
```

**Clean:**
```bash
# Clean dependencies
dotnet clean Mutagen.Bethesda.Serialization.sln -c Release
dotnet clean Mutagen.Bethesda.Serialization.Tests.sln -c Release
dotnet nuget locals all --clear
```

## Project Architecture

This is a **C# serialization library** that provides JSON and YAML serialization for Bethesda mod files via C# Source Generators. The library analyzes Mutagen object interfaces and generates serialization code at compile time.

### Key Components:

- **Mutagen.Bethesda.Serialization**: Core serialization framework and kernel
- **Mutagen.Bethesda.Serialization.SourceGenerator**: Source generator that analyzes Mutagen interfaces and produces serialization code
- **Mutagen.Bethesda.Serialization.Newtonsoft**: JSON serialization implementation using Newtonsoft.Json
- **Mutagen.Bethesda.Serialization.Yaml**: YAML serialization implementation using YamlDotNet
- **Mutagen.Bethesda.Serialization.Testing**: Testing utilities and base classes

### Test Projects:
- **Mutagen.Bethesda.Serialization.Tests**: Core unit tests
- **Mutagen.Bethesda.Serialization.Tester.NoCustomization**: Tests with no customizations
- **Mutagen.Bethesda.Serialization.Tester.FolderSplit**: Tests for file-per-record export
- **Mutagen.Bethesda.Serialization.Tester.MetaObject**: Tests for metadata object handling

## Build System

- Uses **dual solution structure**: Main solution for library code, separate test solution for integration tests
- Requires .NET 8.0/9.0
- Uses **GitVersion** for automatic versioning
- **Central Package Management** via Directory.Packages.props
- Tests use **xUnit** with **Verify** for snapshot testing

## Customization System

The library supports compile-time customizations through `ICustomize` implementations:
- File-per-record export
- Field omission
- Name mapping
- Record ordering
- Comments injection

Source generators read these customizations and generate optimized, customized serialization code rather than using runtime configuration.

## Development Notes

- Main branch for development: **dev**
- **Self-importing project**: This project imports its own generated NuGet packages for testing
- **Local NuGet cache setup required**: The test solution imports packages from the main solution's build output
  ```bash
  # Add local nuget cache after building main solution
  dotnet nuget add source "./nupkg" --name SelfNugetPackages
  dotnet nuget enable source SelfNugetPackages
  ```
- Tests include verified snapshot files (.verified.txt, .verified.json, .verified.yaml)
- The library works with Mutagen objects from the Mutagen.Bethesda ecosystem
- Related downstream project: **Spriggit** (provides UI for end users)

## Writing Source Generator Tests

When writing tests for source generator features in `Mutagen.Bethesda.Serialization.Tests`:

### Test Writing Process

1. **Ensure DisableDiff is enabled in TestHelper**
   - This prevents UI popups during automated testing
   - Check that `TestHelper.VerifySerialization()` has `DisableDiff` enabled

2. **Write unit tests**
   - Use the test helper methods like `GetObjWithMember()` to scaffold test objects
   - Write customization code that exercises the feature
   - Use descriptive test names following the existing pattern

3. **Create expected verification files**
   - Manually write the `.g.verified.cs` files in the `Customization` folder
   - Follow the naming pattern: `TestClass.TestMethod#GeneratedClass_Serializations.g.verified.cs`
   - These files should contain the expected generated source code
   - Key patterns to include:
     - Proper using statements (include `System.Linq` for sorting)
     - Generated serialization methods with correct sorting logic
     - Proper null handling and type conversions

4. **Run tests iteratively**
   - Build the solution: `dotnet build Mutagen.Bethesda.Serialization.sln -c Release`
   - Run the specific test: `dotnet test --filter "FullyQualifiedName~YourTestName"`
   - If tests fail, compare `.received.cs` files with `.verified.cs` files
   - Update verification files until output matches expectations
   - Verify the generated code contains the desired features

5. **Test validation checklist**
   - [ ] Generated code compiles without errors
   - [ ] Sorting/customization logic is correctly applied
   - [ ] Null handling is appropriate
   - [ ] All edge cases are covered
   - [ ] Verification files are committed to source control