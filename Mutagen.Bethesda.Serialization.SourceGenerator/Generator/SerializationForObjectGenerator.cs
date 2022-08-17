using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Mutagen.Bethesda.Serialization.SourceGenerator.Generator.Fields;
using Noggog;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator;

public class SerializationForObjectGenerator
{
    private readonly IsLoquiObjectTester _isLoquiObjectTester;
    private readonly LoquiFieldGenerator _loquiFieldGenerator;
    private readonly Dictionary<string, ISerializationForFieldGenerator> _fieldGenerators = new();

    public SerializationForObjectGenerator(
        IsLoquiObjectTester isLoquiObjectTester,
        LoquiFieldGenerator loquiFieldGenerator,
        ISerializationForFieldGenerator[] fieldGenerators)
    {
        _isLoquiObjectTester = isLoquiObjectTester;
        _loquiFieldGenerator = loquiFieldGenerator;
        foreach (var f in fieldGenerators)
        {
            foreach (var associatedType in f.AssociatedTypes)
            {
                _fieldGenerators[associatedType] = f;
            }
        }
    }
    
    public void Generate(SourceProductionContext context, ITypeSymbol obj, INamedTypeSymbol bootstrap)
    {
        var sb = new StructuredStringBuilder();

        var interf = bootstrap.Interfaces.First(x => x.Name == "IMutagenSerializationBootstrap");
        
        var reader = interf.TypeArguments[1];
        var writer = interf.TypeArguments[3];
        
        sb.AppendLine($"using Mutagen.Bethesda.Serialization;");
        sb.AppendLine($"using {obj.ContainingNamespace};");
        
        using (sb.Namespace(obj.ContainingNamespace.ToString()))
        {
        }
        
        using (var c = sb.Class($"{obj.Name}_{bootstrap.Name}_MixIns"))
        {
            c.AccessModifier = AccessModifier.Public;
            c.Static = true;
        }
        using (sb.CurlyBrace())
        {
            using (var args = sb.Function($"public static void Serialize"))
            {
                args.Add($"this {obj} item");
                args.Add($"{writer} writer");
                args.Add($"ISerializationWriterKernel<{writer}> kernel");
            }
            using (sb.CurlyBrace())
            {
                foreach (var prop in obj.GetMembers().WhereCastable<ISymbol, IPropertySymbol>())
                {
                    if (_fieldGenerators.TryGetValue(prop.Type.Name, out var gen))
                    {
                        gen.GenerateForSerialize(obj, bootstrap, prop, "item", "writer", "kernel", sb);
                    }
                    else if (_isLoquiObjectTester.IsLoqui(prop.Type))
                    {
                        _loquiFieldGenerator.GenerateForSerialize(obj, bootstrap, prop, "item", "writer", "kernel", sb);
                    }
                    else
                    {
                        sb.AppendLine($"throw new NotImplementedException(\"Unknown type: {prop.Type}\");");
                    }
                }
            }
            
            using (var args = sb.Function($"public static {obj} Deserialize"))
            {
                args.Add($"{reader} reader");
                args.Add($"ISerializationReaderKernel<{reader}> kernel");
            }
            using (sb.CurlyBrace())
            {
                sb.AppendLine("throw new NotImplementedException();");
            }
        }
        sb.AppendLine();

        var sanitizedName = obj.MetadataName;
        var genericIndex = sanitizedName.IndexOf('`');
        if (genericIndex != -1)
        {
            sanitizedName = sanitizedName.Substring(0, genericIndex);
        }

        context.AddSource($"{sanitizedName}_{bootstrap.Name}_Serializations.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
    }
}