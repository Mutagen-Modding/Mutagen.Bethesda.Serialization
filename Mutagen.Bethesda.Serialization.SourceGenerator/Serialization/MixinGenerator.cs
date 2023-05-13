using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Mutagen.Bethesda.Serialization.SourceGenerator.Customizations;
using Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Bootstrapping;
using Noggog;
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

    private string StreamPassAlong(CustomizationSpecifications customization, string streamAccessor)
    {
        return customization.FilePerRecord ? "stream" : "new StreamPackage(stream, null)";
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

        var pathOutput = customization.FilePerRecord ? "DirectoryPath" : "FilePath";
        var pathInput = customization.FilePerRecord ? "DirectoryPath" : "FilePath";

        sb.AppendLines(
            new string[]
                {
                    $"Noggog",
                    $"Noggog.WorkEngine",
                    $"Mutagen.Bethesda.Plugins",
                    $"Mutagen.Bethesda.Serialization.Streams",
                    "Mutagen.Bethesda.Serialization.Utility",
                    $"{bootstrap.ObjectRegistration.ContainingNamespace}",
                    $"{reader.ContainingNamespace}",
                    $"System.IO.Abstractions",
                }
                .Distinct()
                .OrderBy(x => x)
                .Select(x => $"using {x};"));
        
        if (!SymbolEqualityComparer.Default.Equals(writer.ContainingNamespace, reader.ContainingNamespace))
        {
            sb.AppendLine($"using {writer.ContainingNamespace};");
        }
        sb.AppendLine();

        sb.AppendLine("#nullable enable");
        sb.AppendLine();
        
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
            
            using (var args = sb.Function($"public static async Task Serialize"))
            {
                args.Add($"this {bootstrap.Bootstrap} converterBootstrap");
                args.Add($"{bootstrap.ObjectRegistration.ContainingNamespace}.{names.Getter} item");
                args.Add($"{pathOutput} path");
                args.Add("IWorkDropoff? workDropoff = null");
                args.Add("IFileSystem? fileSystem = null");
                args.Add("ICreateStream? streamCreator = null");
                args.Add("object? extraMeta = null");
            }
            using (sb.CurlyBrace())
            {
                sb.AppendLine("fileSystem = fileSystem.GetOrDefault();");
                if (customization.FilePerRecord)
                {
                    sb.AppendLine("path = Path.Combine(path, item.ModKey.ToString());");
                    sb.AppendLine("fileSystem.Directory.CreateDirectory(path);");
                }
                sb.AppendLine("streamCreator ??= NormalFileStreamCreator.Instance;");
                if (customization.FilePerRecord)
                {
                    sb.AppendLine("using var streamPassIn = streamCreator.GetStreamFor(fileSystem, Path.Combine(path, SerializationHelper.RecordDataFileName(ReaderKernel.ExpectedExtension)), write: true);");
                }
                else
                {
                    sb.AppendLine("using var streamPassIn = streamCreator.GetStreamFor(fileSystem, path, write: true);");
                }
                var pathStreamPassAlong = customization.FilePerRecord 
                    ? "new StreamPackage(streamPassIn, path)" 
                    : "new StreamPackage(streamPassIn, Path.GetDirectoryName(path))";
                sb.AppendLine($"var streamPackage = {pathStreamPassAlong};");
                sb.AppendLine($"var writer = WriterKernel.GetNewObject(streamPackage);");
                sb.AppendLine($"await {modSerializationItems.SerializationCall()}<{writerKernel}, {writer.Name}>(writer, item, WriterKernel, workDropoff, fileSystem, streamCreator);");
                sb.AppendLine($"WriterKernel.Finalize(streamPackage, writer);");
            }
            sb.AppendLine();

            if (!customization.FilePerRecord)
            {
                using (var args = sb.Function($"public static async Task Serialize"))
                {
                    args.Add($"this {bootstrap.Bootstrap} converterBootstrap");
                    args.Add($"{bootstrap.ObjectRegistration.ContainingNamespace}.{names.Getter} item");
                    args.Add($"Stream stream");
                    args.Add("IWorkDropoff? workDropoff = null");
                    args.Add("IFileSystem? fileSystem = null");
                    args.Add("ICreateStream? streamCreator = null");
                    args.Add("object? extraMeta = null");
                }
                using (sb.CurlyBrace())
                {
                    sb.AppendLine($"var streamPassIn = {StreamPassAlong(customization, "stream")};");
                    sb.AppendLine($"var writer = WriterKernel.GetNewObject(streamPassIn);");
                    sb.AppendLine($"await {modSerializationItems.SerializationCall()}<{writerKernel}, {writer.Name}>(writer, item, WriterKernel, workDropoff, fileSystem, streamCreator);");
                    sb.AppendLine($"WriterKernel.Finalize(streamPassIn, writer);");
                }
                sb.AppendLine();
            }

            using (var args = sb.Function($"public static async Task<{bootstrap.ObjectRegistration.ContainingNamespace}.{names.Setter}> Deserialize"))
            {
                args.Add($"this {bootstrap.Bootstrap} converterBootstrap");
                args.Add($"{pathInput} path");
                if (isMod)
                {
                    args.Add("IWorkDropoff? workDropoff = null");
                    args.Add("IFileSystem? fileSystem = null");
                    args.Add("ICreateStream? streamCreator = null");
                    args.Add("object? extraMeta = null");
                }
                else
                {
                    args.Add("SerializationMetaData metaData");
                }
            }
            using (sb.CurlyBrace())
            {
                sb.AppendLine("fileSystem = fileSystem.GetOrDefault();");
                sb.AppendLine("streamCreator ??= NormalFileStreamCreator.Instance;");
                
                if (isMod)
                {
                    using (var c = sb.Call("SerializationHelper.ExtractMeta"))
                    {
                        c.AddPassArg("fileSystem");
                        c.Add($"modKeyPath: path");
                        c.Add($"path: {(customization.FilePerRecord ? $"Path.Combine(path, SerializationHelper.RecordDataFileName(ReaderKernel.ExpectedExtension))" : "path")}");
                        c.AddPassArg("streamCreator");
                        c.Add("kernel: ReaderKernel");
                        c.Add("modKey: out var modKey");
                        c.Add("release: out var release");
                    }
                }

                if (customization.FilePerRecord)
                {
                    sb.AppendLine("using var streamPassIn = streamCreator.GetStreamFor(fileSystem, Path.Combine(path, SerializationHelper.RecordDataFileName(ReaderKernel.ExpectedExtension)), write: false);");
                }
                else
                {
                    sb.AppendLine("using var streamPassIn = streamCreator.GetStreamFor(fileSystem, path, write: false);");
                }
                var pathStreamPassAlong = customization.FilePerRecord 
                    ? "new StreamPackage(streamPassIn, path)" 
                    : "new StreamPackage(streamPassIn, Path.GetDirectoryName(path))";
                using (var c = sb.Call($"return await {modSerializationItems.DeserializationCall()}<{reader.Name}>"))
                {
                    c.Add($"ReaderKernel.GetNewObject({pathStreamPassAlong})");
                    c.Add("ReaderKernel");
                    if (isMod)
                    {
                        c.AddPassArg("modKey");
                        c.Add($"release: release.To{_releaseRetriever.GetReleaseName(bootstrap.ObjectRegistration)}Release()");
                        c.AddPassArg("workDropoff");
                        c.AddPassArg("fileSystem");
                        c.AddPassArg("streamCreator");
                    }
                    else
                    {
                        c.AddPassArg("metaData");
                    }
                }
            }
            sb.AppendLine();

            if (!customization.FilePerRecord)
            {
                using (var args = sb.Function(
                           $"public static async Task<{bootstrap.ObjectRegistration.ContainingNamespace}.{names.Setter}> Deserialize"))
                {
                    args.Add($"this {bootstrap.Bootstrap} converterBootstrap");
                    args.Add($"Stream stream");
                    if (isMod)
                    {
                        args.Add($"ModKey modKey");
                        args.Add($"{_releaseRetriever.GetReleaseName(bootstrap.ObjectRegistration)}Release release");
                        args.Add("IWorkDropoff? workDropoff = null");
                        args.Add("IFileSystem? fileSystem = null");
                        args.Add("ICreateStream? streamCreator = null");
                        args.Add("object? extraMeta = null");
                    }
                    else
                    {
                        args.Add("SerializationMetaData metaData");
                    }
                }

                using (sb.CurlyBrace())
                {
                    using (var c = sb.Call(
                               $"return await {modSerializationItems.DeserializationCall()}<{reader.Name}>"))
                    {
                        c.Add($"ReaderKernel.GetNewObject({StreamPassAlong(customization, "stream")})");
                        c.Add("ReaderKernel");
                        if (isMod)
                        {
                            c.AddPassArg("modKey");
                            c.AddPassArg("release");
                            c.AddPassArg("workDropoff");
                            c.AddPassArg("fileSystem");
                            c.AddPassArg("streamCreator");
                        }
                        else
                        {
                            c.AddPassArg("metaData");
                        }
                    }
                }

                sb.AppendLine();
            }

            using (var args = sb.Function($"public static async Task DeserializeInto"))
            {
                args.Add($"this {bootstrap.Bootstrap} converterBootstrap");
                args.Add($"{pathOutput} path");
                args.Add($"{names.Setter} obj");
                if (isMod)
                {
                    args.Add("IWorkDropoff? workDropoff = null");
                    args.Add("IFileSystem? fileSystem = null");
                    args.Add("ICreateStream? streamCreator = null");
                }
                else
                {
                    args.Add("SerializationMetaData metaData");
                }
            }
            using (sb.CurlyBrace())
            {
                sb.AppendLine("fileSystem = fileSystem.GetOrDefault();");
                sb.AppendLine("streamCreator ??= NormalFileStreamCreator.Instance;");
                if (customization.FilePerRecord)
                {
                    sb.AppendLine("using var streamPassIn = streamCreator.GetStreamFor(fileSystem, Path.Combine(path, $\"Data{ReaderKernel.ExpectedExtension}\"), write: false);");
                }
                else
                {
                    sb.AppendLine("using var streamPassIn = streamCreator.GetStreamFor(fileSystem, path, write: false);");
                }
                var pathStreamPassAlong = customization.FilePerRecord 
                    ? "new StreamPackage(streamPassIn, path)" 
                    : "new StreamPackage(streamPassIn, Path.GetDirectoryName(path)!)";
                using (var c = sb.Call($"await {modSerializationItems.DeserializationIntoCall()}<{reader.Name}>"))
                {
                    c.Add($"ReaderKernel.GetNewObject({pathStreamPassAlong})");
                    c.Add("ReaderKernel");
                    c.AddPassArg("obj");
                    if (isMod)
                    {
                        c.AddPassArg("workDropoff");
                        c.AddPassArg("fileSystem");
                        c.AddPassArg("streamCreator");
                    }
                    else
                    {
                        c.AddPassArg("metaData");
                    }
                }
            }
            sb.AppendLine();

            if (!customization.FilePerRecord)
            {
                using (var args = sb.Function($"public static async Task DeserializeInto"))
                {
                    args.Add($"this {bootstrap.Bootstrap} converterBootstrap");
                    args.Add($"Stream stream");
                    args.Add($"{names.Setter} obj");
                    if (isMod)
                    {
                        args.Add("IWorkDropoff? workDropoff = null");
                        args.Add("IFileSystem? fileSystem = null");
                        args.Add("ICreateStream? streamCreator = null");
                    }
                    else
                    {
                        args.Add("SerializationMetaData metaData");
                    }
                }
                using (sb.CurlyBrace())
                {
                    sb.AppendLine("fileSystem = fileSystem.GetOrDefault();");
                    using (var c = sb.Call($"await {modSerializationItems.DeserializationIntoCall()}<{reader.Name}>"))
                    {
                        c.Add($"ReaderKernel.GetNewObject({StreamPassAlong(customization, "stream")})");
                        c.Add("ReaderKernel");
                        c.AddPassArg("obj");
                        if (isMod)
                        {
                            c.AddPassArg("workDropoff");
                            c.AddPassArg("fileSystem");
                            c.AddPassArg("streamCreator");
                        }
                        else
                        {
                            c.AddPassArg("metaData");
                        }
                    }
                }
                sb.AppendLine();
            }
        }
        sb.AppendLine();

        context.AddSource($"{bootstrap.Bootstrap.Name}_{modSerializationItems.TermName}_MixIns.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
    }
}