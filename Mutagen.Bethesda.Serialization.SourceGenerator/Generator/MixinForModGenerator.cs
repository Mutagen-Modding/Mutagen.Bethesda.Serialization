using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator;

public class MixinForModGenerator
{
    private readonly LoquiSerializationNaming _serializationNaming;

    public MixinForModGenerator(
        LoquiSerializationNaming serializationNaming)
    {
        _serializationNaming = serializationNaming;
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
        if (bootstrap.ModRegistration == null) return;
        if (!_serializationNaming.TryGetSerializationItems(bootstrap.ModRegistration, out var modSerializationItems)) return;
        
        var interf = bootstrap.Bootstrap.Interfaces.First(x => x.Name == "IMutagenSerializationBootstrap");
        
        var readerKernel = interf.TypeArguments[0];
        var reader = interf.TypeArguments[1];
        var writerKernel = interf.TypeArguments[2];
        var writer = interf.TypeArguments[3];
        
        var sb = new StructuredStringBuilder();

        sb.AppendLine($"using {bootstrap.ModRegistration.ContainingNamespace};");
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
            sb.AppendLine($"private readonly static {writerKernel} WriterKernel = new();");
            sb.AppendLine();
            
            using (var args = sb.Function($"public static void Serialize"))
            {
                args.Add($"this {bootstrap.Bootstrap} converterBootstrap");
                args.Add($"{bootstrap.ModRegistration} mod");
                args.Add($"Stream stream");
            }
            using (sb.CurlyBrace())
            {
                sb.AppendLine($"var writer = WriterKernel.GetNewObject(stream);");
                sb.AppendLine($"{modSerializationItems.SerializationCall(serialize: true)}<{writer.Name}>(mod, writer, WriterKernel);");
                sb.AppendLine($"WriterKernel.Finalize(stream, writer);");
            }
            sb.AppendLine();
            
            using (var args = sb.Function($"public static {bootstrap.ModRegistration} Deserialize"))
            {
                args.Add($"this {bootstrap.Bootstrap} converterBootstrap");
                args.Add($"Stream stream");
            }
            using (sb.CurlyBrace())
            {
                sb.AppendLine($"return {modSerializationItems.SerializationCall(serialize: false)}<{reader.Name}>(ReaderKernel.GetNewObject(stream), ReaderKernel);");
            }
            sb.AppendLine();
        }
        sb.AppendLine();

        context.AddSource($"{bootstrap.Bootstrap.Name}_{modSerializationItems.TermName}_MixIns.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
    }
}