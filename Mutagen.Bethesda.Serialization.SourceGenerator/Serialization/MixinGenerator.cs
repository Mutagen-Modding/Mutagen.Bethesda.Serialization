using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Mutagen.Bethesda.Serialization.SourceGenerator.Customizations;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization;

public class MixinGenerator
{
    private readonly LoquiNameRetriever _nameRetriever;
    private readonly LoquiSerializationNaming _serializationNaming;
    private readonly ObjectTypeTester _modObjectTypeTester;
    private readonly ReleaseRetriever _releaseRetriever;

    public MixinGenerator(
        LoquiNameRetriever nameRetriever,
        LoquiSerializationNaming serializationNaming,
        ObjectTypeTester modObjectTypeTester,
        ReleaseRetriever releaseRetriever)
    {
        _nameRetriever = nameRetriever;
        _serializationNaming = serializationNaming;
        _modObjectTypeTester = modObjectTypeTester;
        _releaseRetriever = releaseRetriever;
    }
    
    public void Initialize(
        IncrementalGeneratorInitializationContext context,
        IncrementalValuesProvider<BootstrapInvocation> modBootstrapInvocations,
        IncrementalValueProvider<CustomizationSpecifications> customization,
        IncrementalValueProvider<ImmutableDictionary<LoquiTypeSet, RecordCustomizationSpecifications>> recordCustomizationDriver)
    {
        context.RegisterSourceOutput(
            modBootstrapInvocations
                .Combine(customization),
            (c, i) => Generate(c, i.Left, i.Right));
    }
    
    public void Generate(SourceProductionContext context, BootstrapInvocation bootstrap, CustomizationSpecifications customization)
    {
        if (bootstrap.ObjectRegistration == null) return;
        if (!_serializationNaming.TryGetSerializationItems(bootstrap.ObjectRegistration, out var modSerializationItems)) return;
        
        var interf = bootstrap.Bootstrap.Interfaces.First(x => x.Name == "IMutagenSerializationBootstrap");
        
        var readerKernel = interf.TypeArguments[0];
        var reader = interf.TypeArguments[1];
        var writerKernel = interf.TypeArguments[2];
        var writer = interf.TypeArguments[3];
        var isMod = _modObjectTypeTester.IsModObject(bootstrap.ObjectRegistration);

        if (!isMod && customization.FilePerRecord)
        {
            // Don't generate mixins if we're doing FilePerRecord
            return;
        }
        
        var names = _nameRetriever.GetNames(bootstrap.ObjectRegistration);
        
        var sb = new StructuredStringBuilder();

        var pathInput = customization.FilePerRecord ? "DirectoryPath" : "FilePath";
        var streamInput = customization.FilePerRecord ? "StreamPackage" : "Stream";
        var streamPassAlong = customization.FilePerRecord ? "stream" : "new StreamPackage(stream, null, IFileSystemExt.DefaultFilesystem)";

        sb.AppendLine($"using Mutagen.Bethesda.Plugins;");
        sb.AppendLine($"using {bootstrap.ObjectRegistration.ContainingNamespace};");
        sb.AppendLine($"using {reader.ContainingNamespace};");
        sb.AppendLine("using System.IO.Abstractions;");
        sb.AppendLine("using Noggog;");
        
        if (!SymbolEqualityComparer.Default.Equals(writer.ContainingNamespace, reader.ContainingNamespace))
        {
            sb.AppendLine($"using {writer.ContainingNamespace};");
        }
        
        using (sb.Namespace(bootstrap.Bootstrap.ContainingNamespace.ToString()))
        {
        }

        var className = $"{bootstrap.Bootstrap.Name}{modSerializationItems.TermName}MixIns";
        using (var c = sb.Class(className))
        {
            c.AccessModifier = AccessModifier.Public;
            c.Static = true;
        }
        using (sb.CurlyBrace())
        {
            sb.AppendLine($"private readonly static {readerKernel} ReaderKernel = new();");
            sb.AppendLine($"private readonly static MutagenSerializationWriterKernel<{writerKernel}, {writer}> WriterKernel = new();");
            sb.AppendLine();
            
            using (var args = sb.Function($"public static void Serialize"))
            {
                args.Add($"this {bootstrap.Bootstrap} converterBootstrap");
                args.Add($"{bootstrap.ObjectRegistration.ContainingNamespace}.{names.Getter} item");
                args.Add($"{pathInput} path");
                args.Add("IFileSystem? fileSystem = null");
            }
            using (sb.CurlyBrace())
            {
                sb.AppendLine("fileSystem = fileSystem.GetOrDefault();");
                if (customization.FilePerRecord)
                {
                    sb.AppendLine("fileSystem.Directory.CreateDirectory(path);");
                }
                var pathStreamPassAlong = customization.FilePerRecord ? "new StreamPackage(fileSystem.File.Create(Path.Combine(path, $\"Data{ReaderKernel.ExpectedExtension}\")), path, fileSystem)" : "fileSystem.File.Create(path)";
                using (var f = sb.Call("Serialize"))
                {
                    f.AddPassArg("converterBootstrap");
                    f.AddPassArg("item");
                    f.Add($"stream: {pathStreamPassAlong}");
                }
            }
            sb.AppendLine();
            
            using (var args = sb.Function($"public static void Serialize"))
            {
                args.Add($"this {bootstrap.Bootstrap} converterBootstrap");
                args.Add($"{bootstrap.ObjectRegistration.ContainingNamespace}.{names.Getter} item");
                args.Add($"{streamInput} stream");
            }
            using (sb.CurlyBrace())
            {
                sb.AppendLine($"var writer = WriterKernel.GetNewObject({streamPassAlong});");
                sb.AppendLine($"{modSerializationItems.SerializationCall()}<{writerKernel}, {writer.Name}>(writer, item, WriterKernel);");
                sb.AppendLine($"WriterKernel.Finalize({streamPassAlong}, writer);");
            }
            sb.AppendLine();
            
            using (var args = sb.Function($"public static {bootstrap.ObjectRegistration.ContainingNamespace}.{names.Setter} Deserialize"))
            {
                args.Add($"this {bootstrap.Bootstrap} converterBootstrap");
                args.Add($"{streamInput} stream");
                if (isMod)
                {
                    args.Add($"ModKey modKey");
                    args.Add($"{_releaseRetriever.GetReleaseName(bootstrap.ObjectRegistration)}Release release");
                }
                else
                {
                    args.Add("SerializationMetaData metaData");
                }
            }
            using (sb.CurlyBrace())
            {
                using (var c = sb.Call($"return {modSerializationItems.DeserializationCall()}<{reader.Name}>"))
                {
                    c.Add($"ReaderKernel.GetNewObject({streamPassAlong})");
                    c.Add("ReaderKernel");
                    if (isMod)
                    {
                        c.AddPassArg("modKey");
                        c.AddPassArg("release");
                    }
                    else
                    {
                        c.AddPassArg("metaData");
                    }
                }
            }
            sb.AppendLine();
            
            using (var args = sb.Function($"public static void DeserializeInto"))
            {
                args.Add($"this {bootstrap.Bootstrap} converterBootstrap");
                args.Add($"{pathInput} path");
                args.Add($"{names.Setter} obj");
                if (!isMod)
                {
                    args.Add("SerializationMetaData metaData");
                }
                args.Add("IFileSystem? fileSystem = null");
            }
            using (sb.CurlyBrace())
            {
                var pathStreamPassAlong = customization.FilePerRecord ? "new StreamPackage(fileSystem.File.Open(Path.Combine(path, $\"Data{ReaderKernel.ExpectedExtension}\"), FileMode.Create, FileAccess.ReadWrite), path, fileSystem)" : "fileSystem.File.Open(path, FileMode.Create, FileAccess.ReadWrite)";
                using (var c = sb.Call($"DeserializeInto"))
                {
                    c.AddPassArg("converterBootstrap");
                    c.Add($"stream: {pathStreamPassAlong}");
                    c.AddPassArg("obj");
                    if (!isMod)
                    {
                        c.AddPassArg("metaData");
                    }
                }
            }
            sb.AppendLine();
            
            using (var args = sb.Function($"public static void DeserializeInto"))
            {
                args.Add($"this {bootstrap.Bootstrap} converterBootstrap");
                args.Add($"{streamInput} stream");
                args.Add($"{names.Setter} obj");
                if (!isMod)
                {
                    args.Add("SerializationMetaData metaData");
                }
            }
            using (sb.CurlyBrace())
            {
                using (var c = sb.Call($"{modSerializationItems.DeserializationIntoCall()}<{reader.Name}>"))
                {
                    c.Add($"ReaderKernel.GetNewObject({streamPassAlong})");
                    c.Add("ReaderKernel");
                    c.AddPassArg("obj");
                    if (!isMod)
                    {
                        c.AddPassArg("metaData");
                    }
                }
            }
            sb.AppendLine();
        }
        sb.AppendLine();

        context.AddSource($"{bootstrap.Bootstrap.Name}_{modSerializationItems.TermName}_MixIns.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
    }
}