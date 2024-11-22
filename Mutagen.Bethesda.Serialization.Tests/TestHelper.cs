using System.IO.Abstractions;
using System.Runtime.CompilerServices;
using Loqui;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Serialization.Newtonsoft;
using Mutagen.Bethesda.Serialization.SourceGenerator.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Tests;
using Mutagen.Bethesda.Serialization.Yaml;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Starfield;
using Noggog;
using StrongInject;

namespace Mutagen.Bethesda.Serialization.Tests.SourceGenerators;

public static class TestHelper
{
    private static bool AutoVerify = false;

    private static VerifySettings GetVerifySettings()
    {
        var verifySettings = new VerifySettings();
#if DEBUG
        if (AutoVerify)
        {
            verifySettings.AutoVerify();
        }
#else
        verifySettings.DisableDiff();
#endif
        return verifySettings;
    }
    
    public static Task VerifySerialization(string source, [CallerFilePath] string sourceFile = "")
    {
        // Parse the provided string into a C# syntax tree
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);
    
        IEnumerable<PortableExecutableReference> references = new[]
        {
            MetadataReference.CreateFromFile(typeof(Owned<>).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(MutagenJsonConverter).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(FilePath).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(LoquiRegistration).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(TestMod).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(MutagenYamlConverter).Assembly.Location),
        };
        
        // Create a Roslyn compilation for the syntax tree.
        CSharpCompilation compilation = CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: new[] { syntaxTree },
            references: references);

        // Create an instance of our incremental source generator
        var generator = new SerializationSourceGenerator(generateMixIns: false);

        // The GeneratorDriver is used to run our generator against a compilation
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
    
        // Run the source generator!
        driver = driver.RunGenerators(compilation);
        
        // Use verify to snapshot test the source generator output!
        return Verifier.Verify(driver, GetVerifySettings(), sourceFile);
    }

    public static Task VerifyString(string str, [CallerFilePath] string sourceFile = "")
    {
        return Verifier.Verify(str, GetVerifySettings(), sourceFile);
    }

    public static Task VerifyFileSystem(IFileSystem fileSystem)
    {
        return Verifier.Verify(fileSystem, GetVerifySettings());
    }

    public static GeneratorDriverRunResult RunSourceGenerator(string source)
    {
        // Parse the provided string into a C# syntax tree
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);

        IEnumerable<PortableExecutableReference> references = new[]
        {
            MetadataReference.CreateFromFile(typeof(ModKey).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(FormKey).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(SkyrimMod).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Fallout4Mod).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(StarfieldMod).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(OblivionMod).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(ISerializationReaderKernel<>).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(LoquiRegistration).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(FilePath).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(TestMod).Assembly.Location),
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
        
        return driver.GetRunResult();
    }
}