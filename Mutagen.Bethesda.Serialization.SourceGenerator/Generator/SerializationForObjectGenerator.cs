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
    public void Generate(SourceProductionContext context, ITypeSymbol obj)
    {
        var sb = new StructuredStringBuilder();
        
        sb.AppendLine($"using Mutagen.Bethesda.Serialization;");
        sb.AppendLine($"using {obj.ContainingNamespace};");
        
        using (sb.Namespace(obj.ContainingNamespace.ToString()))
        {
        }
        
        using (var c = sb.Class($"{obj.Name}_Serialization"))
        {
            c.AccessModifier = AccessModifier.Internal;
            c.Static = true;
        }
        using (sb.CurlyBrace())
        {
            using (var args = sb.Function($"public static void Serialize<TWriteObject>"))
            {
                args.Add($"{obj} item");
                args.Add($"TWriteObject writer");
                args.Add($"ISerializationWriterKernel<TWriteObject> kernel");
            }
            using (sb.CurlyBrace())
            {
                foreach (var prop in obj.GetMembers().WhereCastable<ISymbol, IPropertySymbol>())
                {
                    if (_fieldGenerators.TryGetValue(prop.Type.Name, out var gen))
                    {
                        gen.GenerateForSerialize(obj, prop, "item", "writer", "kernel", sb);
                    }
                    else if (_isLoquiObjectTester.IsLoqui(prop.Type))
                    {
                        _loquiFieldGenerator.GenerateForSerialize(obj, prop, "item", "writer", "kernel", sb);
                    }
                    else
                    {
                        sb.AppendLine($"throw new NotImplementedException(\"Unknown type: {prop.Type}\");");
                    }
                }
            }
            sb.AppendLine();
            
            using (var args = sb.Function($"public static {obj} Deserialize<TReadObject>"))
            {
                args.Add($"TReadObject reader");
                args.Add($"ISerializationReaderKernel<TReadObject> kernel");
            }
            using (sb.CurlyBrace())
            {
                sb.AppendLine("throw new NotImplementedException();");
            }
            sb.AppendLine();
        }
        sb.AppendLine();
        
        var sanitizedName = obj.MetadataName;
        var genericIndex = sanitizedName.IndexOf('`');
        if (genericIndex != -1)
        {
            sanitizedName = sanitizedName.Substring(0, genericIndex);
        }
        
        context.AddSource($"{sanitizedName}_Serializations.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
    }
}