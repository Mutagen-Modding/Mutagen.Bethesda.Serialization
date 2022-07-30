using System.Collections.Generic;
using System.Threading.Tasks;
using Loqui;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Serialization.Newtonsoft;
using Mutagen.Bethesda.Serialization.SourceGenerator.Generator;
using Mutagen.Bethesda.Serialization.SourceGenerator.Utility;
using Mutagen.Bethesda.Serialization.Yaml;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using StrongInject;
using VerifyXunit;

namespace Mutagen.Bethesda.Serialization.Tests.SourceGenerators;

public static class TestHelper
{
    public static Task Verify(string source)
    {
        // Parse the provided string into a C# syntax tree
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);
    
        IEnumerable<PortableExecutableReference> references = new[]
        {
            MetadataReference.CreateFromFile(typeof(Owned<>).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(MutagenJsonConverter).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(FilePath).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(LoquiRegistration).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(SkyrimMod).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(MutagenYamlConverter).Assembly.Location),
        };
        
        // Create a Roslyn compilation for the syntax tree.
        CSharpCompilation compilation = CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: new[] { syntaxTree },
            references: references);

        // Create an instance of our EnumGenerator incremental source generator
        var generator = new SerializationSourceGenerator();

        // The GeneratorDriver is used to run our generator against a compilation
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
    
        // Run the source generator!
        driver = driver.RunGenerators(compilation);
    
        // Use verify to snapshot test the source generator output!
        return Verifier.Verify(driver);
    }
    
    public static void RunSourceGenerator(string source)
    {
        // Parse the provided string into a C# syntax tree
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);

        IEnumerable<PortableExecutableReference> references = new[]
        {
            MetadataReference.CreateFromFile(typeof(ModKey).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(FormKey).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(SkyrimMod).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(ISerializationReaderKernel<>).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(LoquiRegistration).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(FilePath).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(NewtonsoftJsonSerializationReaderKernel).Assembly.Location)
        };

        // Create a Roslyn compilation for the syntax tree.
        CSharpCompilation compilation = CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: new[] { syntaxTree },
            references: references);

        // Create an instance of our EnumGenerator incremental source generator
        var generator = new SerializationSourceGenerator();

        // The GeneratorDriver is used to run our generator against a compilation
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        // Run the source generator!
        driver = driver.RunGenerators(compilation);

        driver.GetRunResult();
    }
}