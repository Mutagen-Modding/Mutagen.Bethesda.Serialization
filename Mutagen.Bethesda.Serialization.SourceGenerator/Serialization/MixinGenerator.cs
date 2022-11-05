using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
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
        IncrementalValuesProvider<BootstrapInvocation> modBootstrapInvocations)
    {
        context.RegisterSourceOutput(
            modBootstrapInvocations,
            Generate);
    }
    
    public void Generate(SourceProductionContext context, BootstrapInvocation bootstrap)
    {
        if (bootstrap.ObjectRegistration == null) return;
        if (!_serializationNaming.TryGetSerializationItems(bootstrap.ObjectRegistration, out var modSerializationItems)) return;
        
        var interf = bootstrap.Bootstrap.Interfaces.First(x => x.Name == "IMutagenSerializationBootstrap");
        
        var readerKernel = interf.TypeArguments[0];
        var reader = interf.TypeArguments[1];
        var writerKernel = interf.TypeArguments[2];
        var writer = interf.TypeArguments[3];
        var isMod = _modObjectTypeTester.IsModObject(bootstrap.ObjectRegistration);

        var names = _nameRetriever.GetNames(bootstrap.ObjectRegistration);
        
        var sb = new StructuredStringBuilder();

        sb.AppendLine($"using Mutagen.Bethesda.Plugins;");
        sb.AppendLine($"using {bootstrap.ObjectRegistration.ContainingNamespace};");
        sb.AppendLine($"using {reader.ContainingNamespace};");
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
                args.Add($"Stream stream");
            }
            using (sb.CurlyBrace())
            {
                sb.AppendLine($"var writer = WriterKernel.GetNewObject(stream);");
                sb.AppendLine($"{modSerializationItems.SerializationCall()}<{writerKernel}, {writer.Name}>(writer, item, WriterKernel);");
                sb.AppendLine($"WriterKernel.Finalize(stream, writer);");
            }
            sb.AppendLine();
            
            using (var args = sb.Function($"public static {bootstrap.ObjectRegistration.ContainingNamespace}.{names.Setter} Deserialize"))
            {
                args.Add($"this {bootstrap.Bootstrap} converterBootstrap");
                args.Add($"Stream stream");
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
                    c.Add("ReaderKernel.GetNewObject(stream)");
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
                args.Add($"Stream stream");
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
                    c.Add("ReaderKernel.GetNewObject(stream)");
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